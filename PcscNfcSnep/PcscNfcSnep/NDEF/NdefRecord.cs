using System;
using System.Collections.Generic;
using System.Text;

namespace PcscNfcSnep.NDEF
{
    class NdefRecord
    {
        public enum ETypeNameFormat
        {
            Empty = 0,
            Unknown = 0x05,
            Unchanged = 0x06,
            Reserved = 0x07
        }

        public ETypeNameFormat TypeNameFormat { get; set; }
        
        enum EMessageInfoFlags
        {
            MB = 0x1 << 7,
            ME = 0x1 << 6,
            CF = 0x1 << 5,
            SR = 0x1 << 4,
            IL = 0x1 << 3,
            TNF = 0x3,
        }

        const int MAX_PAYLOAD_SIZE = 1024;

        public NdefRecord(ETypeNameFormat eTypeNameFormat, byte[] payload)
        {
            TypeNameFormat = eTypeNameFormat;

        }
    }
}
