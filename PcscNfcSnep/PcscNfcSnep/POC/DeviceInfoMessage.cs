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
            for (int i = 0; i < 3; i++)
            {
                Array.Reverse(rawData, i* RESPONSE_SIZE, RESPONSE_SIZE);
            }
            Deserialize(rawData);
        }
        public override byte[] RequestMessage()
        {
            return new byte[2] { COMMAND, RESEVED };
        }
    }
}
