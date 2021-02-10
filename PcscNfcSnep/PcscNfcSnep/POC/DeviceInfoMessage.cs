using System;
using System.Collections.Generic;
using System.Text;

namespace PcscNfcSnep.POC
{
    class DeviceInfoMessage : Serialization
    {
        char[] ModelName = new char[30];
        char[] SerialNumber = new char[30];
        char[] ProtocolVersionNumber = new char[30];

        public override void ParseMessage(byte[] rawData)
        {
            Deserialize(rawData);
        }
    }
}
