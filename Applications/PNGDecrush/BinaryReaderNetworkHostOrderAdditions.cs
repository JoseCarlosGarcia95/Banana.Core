using System.IO;
using System.Net;

namespace PNGDecrush
{
    public static class BinaryReaderNetworkHostOrderAdditions
    {
        public static uint ReadUInt32NetworkByteOrder(this BinaryReader reader)
        {
            return (uint) IPAddress.NetworkToHostOrder((int) reader.ReadUInt32());
        }

        public static int ReadInt32NetworkByteOrder(this BinaryReader reader)
        {
            return IPAddress.NetworkToHostOrder(reader.ReadInt32());
        }

        public static void WriteNetworkOrder(this BinaryWriter writer, uint value)
        {
            writer.Write((uint) IPAddress.HostToNetworkOrder((int) value));
        }

        public static void WriteNetworkOrder(this BinaryWriter writer, int value)
        {
            writer.Write(IPAddress.HostToNetworkOrder(value));
        }
    }
}