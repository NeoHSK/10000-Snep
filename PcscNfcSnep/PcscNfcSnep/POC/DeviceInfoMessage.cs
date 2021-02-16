using System;
using System.Collections.Generic;
using System.Text;

namespace PcscNfcSnep.POC
{
    class DeviceInfoMessage : Serialization
    {
        const byte COMMAND = 0xA5;
        const byte RESEVED = 0x00;
        const byte RESPONSE_SIZE = 30;

        char[] ModelName = new char[RESPONSE_SIZE];
        char[] SerialNumber = new char[RESPONSE_SIZE];
        char[] ProtocolVersionNumber = new char[RESPONSE_SIZE];

        public override void ResponseMessage(byte[] rawData)
        {
            // TODO Command verified
            var convSize = rawData.Length - 2;
            var conv = new byte[convSize];
            Array.Copy(rawData, 2, conv, 0, convSize);
            Deserialize(conv);
        }
        public override byte[] RequestMessage()
        {
            return new byte[2] { COMMAND, RESEVED };
        }

        public override string ToString()
        {
            return new string(ModelName) + "\n" + new string(SerialNumber) + "\n" + new string(ProtocolVersionNumber);
        }
    }
}
