using PcscNfcSnep.NDEF;
using System;
using System.Collections.Generic;
using System.Text;

namespace PcscNfcSnep.PCSC.NFC
{
    class SNEP
    {
        public static readonly byte[] CMD_START = new byte[] { 0xC6, 0x01 };
        public static readonly byte[] CMD_STOP = new byte[] { 0xC6, 0x02 };

        static readonly byte[] CMD_SEND = new byte[] { 0xC6, 0x03};
        static readonly byte[] CMD_REVEIVE = new byte[] { 0xC6, 0x04, 0xFF };

        public static readonly byte[] CMD_SET_TIMEOUT = new byte[] { 0xC6, 0x05, 0x00, 0x01, 0xFF, 0xFF, 0x00, 0x00 };
        public static readonly byte[] CMD_SET_TIMEOUT2 = new byte[] { 0xC6, 0x05, 0x00, 0x02, 0xFF, 0xFF, 0x00, 0x00 };
    
    
        public byte[] Request(NdefMessage ndefMessage)
        {
            var conv = new byte[CMD_SEND.Length + ndefMessage.ToByteArray().Length]; 
            Array.Copy(CMD_SEND,conv, CMD_SEND.Length);
            Array.Copy(ndefMessage.ToByteArray(), 0,                 
                        conv, CMD_SEND.Length, 
                        ndefMessage.ToByteArray().Length);
            return conv;
        }

    }
}
