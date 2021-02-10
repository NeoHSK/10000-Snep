using System;
using System.Collections.Generic;
using System.Text;

namespace PcscNfcSnep.POC
{
    class TimeSyncMessage : Serialization
    {
        byte Dst;
        sbyte TimeZone;
        byte Second;
        byte Hour;
        byte Day;
        byte Month;
        UInt16 Year;

        public override void ParseMessage(byte[] rawData)
        {
            Deserialize(rawData);
        }
    }
}
