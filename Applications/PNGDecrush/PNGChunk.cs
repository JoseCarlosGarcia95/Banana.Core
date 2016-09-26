namespace PNGDecrush
{
    public class PNGChunk
    {
        public enum ChunkType
        {
            Unknown,
            IDAT,
            CgBI // apple's pngcrush -iphone chunk
        }

        public PNGChunk(string type, byte[] data, uint dataCRC)
        {
            TypeString = type;
            Type = TypeFromString(type);
            Data = data;
            DataCRC = dataCRC;
        }

        public ChunkType Type { get; private set; }
        public string TypeString { get; private set; }
        public byte[] Data { get; private set; }
        public uint DataCRC { get; private set; }

        public static string StringFromType(ChunkType type)
        {
            return type.ToString();
        }

        public static ChunkType TypeFromString(string type)
        {
            switch (type)
            {
                case "IDAT":
                    return ChunkType.IDAT;

                case "CgBI":
                    return ChunkType.CgBI;

                default:
                    return ChunkType.Unknown;
            }
        }
    }
}