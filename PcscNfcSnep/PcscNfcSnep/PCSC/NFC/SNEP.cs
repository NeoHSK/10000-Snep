using PcscNfcSnep.NDEF;
using System;
using System.Collections.Generic;
using System.Text;

namespace PcscNfcSnep.PCSC.NFC
{
    static class SNEP
    {
        public enum ECommand
        {
            None = 0,
            Start,
            Stop,
            Put,
            Receive,
            PutTimeout,
            RecieveTimeout
        }

        public static readonly byte[] CMD_START = new byte[] { 0xC6, 0x01 };
        public static readonly byte[] CMD_STOP = new byte[] { 0xC6, 0x02 };

        public static readonly byte[] CMD_SEND = new byte[] { 0xC6, 0x03};
        public static readonly byte[] CMD_RECEIVE = new byte[] { 0xC6, 0x04, 0xFF };

        public static readonly byte[] CMD_SET_TIMEOUT = new byte[] { 0xC6, 0x05, 0x00, 0x01, 0xFF, 0xFF, 0x00, 0x00 };
        public static readonly byte[] CMD_SET_TIMEOUT2 = new byte[] { 0xC6, 0x05, 0x00, 0x02, 0xFF, 0xFF, 0x00, 0x00 };
    
    
        public static byte[] Request(byte[] command, NdefMessage ndefMessage)
        {
            if (ndefMessage != null)
            {
                var conv = new byte[command.Length + ndefMessage.ToByteArray().Length];
                Array.Copy(command, conv, command.Length);
                Array.Copy(ndefMessage.ToByteArray(), 0,
                            conv, command.Length,
                            ndefMessage.ToByteArray().Length);
                return conv;
            }
            return command;
        }

    }
}
