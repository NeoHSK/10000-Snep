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
        public sbyte TimeZone = 8;
        public byte Second =7;
        public byte Minute = 20;
        public byte Hour=6;
        public byte Day=5;
        public byte Month = 4;
        public UInt16 Year = 2021;

        public override void ResponseMessage(byte[] rawData)
        {
            // TODO Command verified
            var convSize = rawData.Length - 2;
            var conv = new byte[convSize];
            Array.Copy(rawData, 2, conv, 0, convSize);
            Array.Reverse(conv);
            Deserialize(conv);
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
        public override string ToString()
        {
            return "Year - " + Year.ToString() + "\n" +
                "Month - " + Month.ToString() + "\n" +
                "Day - " + Day.ToString() + "\n" +
                "Hour - " + Hour.ToString() + "\n" +
                "Minute - " + Minute.ToString() + "\n" +
                "Second - " + Second.ToString() + "\n" +
                "TimeZone - " + TimeZone.ToString() + "\n" +
                "Dst - " + Dst.ToString() + "\n";
        }
    }
}
