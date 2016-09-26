using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Ionic.Crc;
using Ionic.Zlib;
using CompressionMode = System.IO.Compression.CompressionMode;
using DeflateStream = System.IO.Compression.DeflateStream;

// https://github.com/MikeWeller/PNGDecrush

namespace PNGDecrush
{
    public class PNGDecrusher
    {
        public static void Decrush(Stream input, Stream output)
        {
            using (var fixedChunksOutput = new MemoryStream())
            {
                DecrushAtChunkLevel(input, fixedChunksOutput);
                DecrushAtPixelLevel(fixedChunksOutput, output);
            }
        }

        private static void DecrushAtChunkLevel(Stream input, Stream output)
        {
            var fixedChunks = DecrushChunks(PNGChunkParser.ChunksFromStream(input));
            PNGChunkParser.WriteChunksAsPNG(fixedChunks, output);
        }

        public static IEnumerable<PNGChunk> DecrushChunks(IEnumerable<PNGChunk> chunks)
        {
            var chunksWithoutAppleChunk = ChunksByRemovingAppleCgBIChunks(chunks);

            if (chunksWithoutAppleChunk.Count() == chunks.Count())
                throw new InvalidDataException(
                    "Could not find a CgBI chunk. Image wasn't crushed with Apple's -iohone option.");

            return ConvertIDATChunksFromDeflateToZlib(chunksWithoutAppleChunk);
            ;
        }

        private static IEnumerable<PNGChunk> ConvertIDATChunksFromDeflateToZlib(IEnumerable<PNGChunk> inputChunks)
        {
            // Multiple IDAT chunks must be combined together to form a single DEFLATE payload.
            // This payload is recompressed with zlib headers intact, and then split up into chunks again

            var idatChunks = inputChunks.Where(c => c.Type == PNGChunk.ChunkType.IDAT);
            var zlibData = RecompressedZlibDataFromChunks(idatChunks);

            var newIDATChunks = CreateIdatChunksFromData(zlibData, idatChunks.Count());

            return ReplaceOldIdatChunksWithNewChunks(inputChunks, newIDATChunks);
        }

        private static IEnumerable<PNGChunk> CreateIdatChunksFromData(byte[] data, int numberOfChunks)
        {
            var dataChunks = SplitBufferIntoChunks(data, numberOfChunks);
            return dataChunks.Select(chunkData => IDATChunkWithBytes(chunkData));
        }

        private static byte[] RecompressedZlibDataFromChunks(IEnumerable<PNGChunk> idatChunks)
        {
            var deflateData = CombinedDataFromChunks(idatChunks);
            return ConvertDeflateToZlib(deflateData);
        }

        public static IEnumerable<byte[]> SplitBufferIntoChunks(byte[] input, int numberOfChunks)
        {
            var result = new List<byte[]>();

            var bytesLeft = input.Length;
            var currentInputIndex = 0;

            for (var chunksLeft = numberOfChunks; chunksLeft > 0; chunksLeft--)
            {
                var maxChunkSize = (int) Math.Ceiling(bytesLeft/(double) chunksLeft);
                var thisChunkSize = Math.Min(maxChunkSize, bytesLeft);

                var chunkData = SubArray(input, currentInputIndex, thisChunkSize);

                currentInputIndex += thisChunkSize;
                bytesLeft -= thisChunkSize;

                result.Add(chunkData);
            }

            if ((currentInputIndex != input.Length) || (bytesLeft != 0))
                throw new InvalidOperationException();

            return result;
        }

        private static TType[] SubArray<TType>(TType[] input, int startIndex, int count)
        {
            var result = new TType[count];
            Array.Copy(input, startIndex, result, 0, result.Length);
            return result;
        }

        private static IEnumerable<PNGChunk> ReplaceOldIdatChunksWithNewChunks(IEnumerable<PNGChunk> chunks,
            IEnumerable<PNGChunk> newIdatChunks)
        {
            var indexOfFirstIdat = chunks.Select((c, i) => new {index = i, chunk = c})
                .First(o => o.chunk.Type == PNGChunk.ChunkType.IDAT)
                .index;

            var result = chunks.Where(c => c.Type != PNGChunk.ChunkType.IDAT).ToList();
            result.InsertRange(indexOfFirstIdat, newIdatChunks);
            return result;
        }

        private static PNGChunk IDATChunkWithBytes(byte[] bytes)
        {
            var type = PNGChunk.StringFromType(PNGChunk.ChunkType.IDAT);
            return new PNGChunk(type, bytes, CalculateCRCForChunk(type, bytes));
        }

        private static byte[] CombinedDataFromChunks(IEnumerable<PNGChunk> chunks)
        {
            var totalLength = chunks.Select(c => c.Data.Length).Sum();

            var result = new byte[totalLength];
            var bytesWritten = 0;

            foreach (var chunk in chunks)
            {
                chunk.Data.CopyTo(result, bytesWritten);
                bytesWritten += chunk.Data.Length;
            }

            return result;
        }

        private static byte[] ConvertDeflateToZlib(byte[] input)
        {
            using (var deflateData = new MemoryStream(input))
            {
                using (var deflateStream = new DeflateStream(deflateData, CompressionMode.Decompress))
                {
                    using (var zlibStream = new ZlibStream(deflateStream, Ionic.Zlib.CompressionMode.Compress))
                    {
                        using (var zlibData = new MemoryStream())
                        {
                            zlibStream.CopyTo(zlibData);
                            return zlibData.ToArray();
                        }
                    }
                }
            }
        }

        private static IEnumerable<PNGChunk> ChunksByRemovingAppleCgBIChunks(IEnumerable<PNGChunk> chunks)
        {
            return chunks.Where(c => c.Type != PNGChunk.ChunkType.CgBI);
        }

        public static uint CalculateCRCForChunk(string chunkType, byte[] chunkData)
        {
            var chunkTypeBytes = Encoding.UTF8.GetBytes(chunkType);

            var crc32calculator = new CRC32();
            crc32calculator.SlurpBlock(chunkTypeBytes, 0, chunkTypeBytes.Length);
            crc32calculator.SlurpBlock(chunkData, 0, chunkData.Length);

            return (uint) crc32calculator.Crc32Result;
        }

        public static void DecrushAtPixelLevel(Stream pngInputStream, Stream outputStream)
        {
            using (var bitmap = new Bitmap(pngInputStream))
            {
                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadWrite, bitmap.PixelFormat);
                {
                    FixPixelsInBitmapData(bitmapData);
                }
                bitmap.UnlockBits(bitmapData);

                bitmap.Save(outputStream, ImageFormat.Png);
            }
        }

        private static void FixPixelsInBitmapData(BitmapData bitmapData)
        {
            var totalBytes = bitmapData.Stride*bitmapData.Height;
            var pixelData = new byte[totalBytes];

            var hasAlpha = BitmapDataHasAlpha(bitmapData);
            var bytesPerPixel = BytesPerPixelFromBitmapData(bitmapData);

            Marshal.Copy(bitmapData.Scan0, pixelData, 0, pixelData.Length);
            {
                FixPixelsInBuffer(pixelData, hasAlpha, bytesPerPixel);
            }
            Marshal.Copy(pixelData, 0, bitmapData.Scan0, pixelData.Length);
        }

        private static void FixPixelsInBuffer(byte[] pixelData, bool hasAlpha, uint bytesPerPixel)
        {
            for (uint i = 0; i < pixelData.Length; i += bytesPerPixel)
            {
                ReverseRGBtoBGRByteSwap(pixelData, i);

                if (hasAlpha)
                    ReversePremultipliedAlpha(pixelData, i);
            }
        }

        private static void ReverseRGBtoBGRByteSwap(byte[] pixelData, uint i)
        {
            var temp = pixelData[i + 2];
            pixelData[i + 2] = pixelData[i + 0];
            pixelData[i + 0] = temp;
        }

        private static uint BytesPerPixelFromBitmapData(BitmapData bitmapData)
        {
            switch (bitmapData.PixelFormat)
            {
                case PixelFormat.Format32bppArgb:
                    return 4;

                case PixelFormat.Format32bppRgb:
                    return 3;

                default:
                    throw new InvalidDataException("Only 32 bit RGB(A) PNGs are supported by PNGDecrusher");
            }
        }

        private static bool BitmapDataHasAlpha(BitmapData bitmapData)
        {
            switch (bitmapData.PixelFormat)
            {
                case PixelFormat.Format32bppArgb:
                    return true;

                case PixelFormat.Format32bppRgb:
                    return false;

                default:
                    throw new InvalidDataException("Only 32 bit RGB(A) PNGs are supported by PNGDecrusher");
            }
        }

        private static void ReversePremultipliedAlpha(byte[] pixelData, uint startOffset)
        {
            // premultipliedValue = originalValue * (alpha / 255)
            //                    = (originalValue * alpha) / 255
            // therefore
            //
            // oldValue = (premultipliedValue * 255) / alpha

            var alpha = pixelData[startOffset + 3];

            if (alpha == 0)
                return;

            pixelData[startOffset + 0] = (byte) (pixelData[startOffset + 0]*255/alpha);
            pixelData[startOffset + 1] = (byte) (pixelData[startOffset + 1]*255/alpha);
            pixelData[startOffset + 2] = (byte) (pixelData[startOffset + 2]*255/alpha);
        }
    }
}