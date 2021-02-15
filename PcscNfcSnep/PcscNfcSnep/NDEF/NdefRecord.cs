using PcscNfcSnep.POC;
using System;
using System.Collections.Generic;
using System.Text;

namespace PcscNfcSnep.NDEF
{
    public class NdefRecord
    {
        public enum EMessageInfoFlags 
        {
            None = 0,
            MB = 0x1 << 7,
            ME = 0x1 << 6,
            CF = 0x1 << 5,
            SR = 0x1 << 4,
            IL = 0x1 << 3,
            TNF = 0x7,
            Max = byte.MaxValue
        }

        public EMessageInfoFlags MessageInfoFlag { get; set; }
        
        public enum ETypeNameFormat
        {
            Empty = 0,
            Unknown = 0x05,
            Unchanged = 0x06,
            Reserved = 0x07
        }

        public ETypeNameFormat TypeNameFormat { get; set; }

        private byte[] mType;

        public byte[] Type
        {
            get { return mType; }
            set 
            { 
                if(value == null)
                {
                    mType = null;
                    return;
                }
                mType = new byte[value.Length];
                Array.Copy(value, mType, value.Length);
            }
        }


        private byte[] mPayload;

        public byte[] Payload
        {
            get { return mPayload; }
            set 
            {
                if(value == null)
                {
                    mPayload = null;
                    return;
                }
                mPayload = new byte[value.Length];
                Array.Copy(value, mPayload, value.Length);
            }
        }

        public NdefRecord()
        {
            TypeNameFormat = ETypeNameFormat.Empty;
        }

        public NdefRecord(Serialization serialization)
        {
            TypeNameFormat = ETypeNameFormat.Unknown;
            Payload = serialization.RequestMessage();
        }

        public NdefRecord(NdefRecord ndefRecord)
        {
            TypeNameFormat = ndefRecord.TypeNameFormat;

            if(ndefRecord.Payload != null)
            {
                mPayload = new byte[ndefRecord.Payload.Length];
                Array.Copy(ndefRecord.Payload, mPayload, ndefRecord.Payload.Length);
            }
        }
    }
}
