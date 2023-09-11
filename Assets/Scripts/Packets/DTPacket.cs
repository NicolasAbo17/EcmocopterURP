using System;

namespace MixedUpSocket
{
    public enum DTPacketType
    {
        Copter = 0, //it's nothing on this code.
        endType
    }

    [Serializable]
    public class DTPacket
    {
        public DTPacket(byte id, byte val)
        {
            data = new byte[6];
            data[0] = 0x3C;
            data[1] = 0x3C;
            data[2] = id;
            data[3] = val;
            data[4] = 0x3E;
            data[5] = 0x3E;
        }

        public byte Id()
        {
            return data[2];
        }
        public byte Val()
        {
            return data[3];
        }

        public byte[] data;
    }

}