using System;
using System.Collections.Generic;
using System.Text;

namespace PcscNfcSnep.POC
{
    class TimeSyncMessage : Serialization
    {
        const byte COMMAND = 0x5A;
        const byte RESERVED = 0x00;
        public byte Dst=9;
        public sbyte TimeZone;
        public byte Second;
        public byte Hour;
        public byte Day;
        public byte Month;
        public UInt16 Year = 2021;

        public override void ResponseMessage(byte[] rawData)
        {
            Deserialize(rawData);
        }

        public override byte[] RequestMessage()
        {
            var conv = Serialize();

            Array.Reverse(conv);

            byte[] res = new byte[2+conv.Length];

            res[0] = COMMAND;
            res[1] = RESERVED;

            Array.Copy(conv, 0, res, 2, conv.Length);

            return res;
        }
    }
}
