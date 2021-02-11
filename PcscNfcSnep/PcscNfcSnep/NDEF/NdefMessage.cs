using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PcscNfcSnep.NDEF
{
    class NdefMessage : List<NdefRecord>
    {
        static bool messageBegin;

        NdefMessage FromByteArray(byte[] rawData)
        {
            uint payloadLength;

            uint index = 0;
            NdefMessage ndefMessage = new NdefMessage();

            while (index < rawData.Length)
            {
                NdefRecord ndefRecord = new NdefRecord();

                ndefRecord.MessageInfoFlag = (NdefRecord.EMessageInfoFlags)rawData[index];

                if ((ndefRecord.MessageInfoFlag & NdefRecord.EMessageInfoFlags.MB) == NdefRecord.EMessageInfoFlags.MB)
                {
                    if (messageBegin != true)
                    {
                        messageBegin = true;
                    }
                    else
                    {
                        /* Invalid */
                        messageBegin = false;
                        return ndefMessage;
                    }
                }

                if ((ndefRecord.MessageInfoFlag & NdefRecord.EMessageInfoFlags.ME) == NdefRecord.EMessageInfoFlags.ME)
                {
                    if(messageBegin == true)
                    {
                        messageBegin = false;
                    }
                    else
                    {
                        /* Invalid */
                        messageBegin = false;

                        return ndefMessage;
                    }
                }


                /* not supported*/
                if (
                    (ndefRecord.MessageInfoFlag & NdefRecord.EMessageInfoFlags.CF) == NdefRecord.EMessageInfoFlags.CF ||
                    (ndefRecord.MessageInfoFlag & NdefRecord.EMessageInfoFlags.IL) == NdefRecord.EMessageInfoFlags.IL
                    )
                {
                    return ndefMessage;
                }

                if ((ndefRecord.MessageInfoFlag & NdefRecord.EMessageInfoFlags.SR) == NdefRecord.EMessageInfoFlags.SR)
                {
                    payloadLength = rawData[++index];
                }
                else
                {
                    payloadLength = (uint)((rawData[++index]) << 24);
                    payloadLength |= (uint)((rawData[++index]) << 16);
                    payloadLength |= (uint)((rawData[++index]) << 8);
                    payloadLength |= (uint)((rawData[++index]) << 0);
                }

                if (payloadLength > 0)
                {
                    var payload = new byte[payloadLength];

                    Array.Copy(rawData, ++index, payload, 0, payload.Length);

                }

                switch ((NdefRecord.ETypeNameFormat)(ndefRecord.MessageInfoFlag & NdefRecord.EMessageInfoFlags.TNF))
                {
                    case NdefRecord.ETypeNameFormat.Unknown:
                        ndefRecord.TypeNameFormat = NdefRecord.ETypeNameFormat.Unknown;
                        break;
                    case NdefRecord.ETypeNameFormat.Unchanged:
                        ndefRecord.TypeNameFormat = NdefRecord.ETypeNameFormat.Unchanged;
                        break;
                    default: break;
                }

                ndefMessage.Add(ndefRecord);

                index += payloadLength;
            }

            return ndefMessage;
        }
            

        byte[] ToByteArray()
        {
            if(Count == 0)
            {
                return new NdefMessage{ new NdefRecord() }.ToByteArray();
            }

            MemoryStream memoryStream = new MemoryStream();
            for (int i = 0; i < Count; i++)
            {
                var record = this[i];

                var recordHeader = (byte)record.TypeNameFormat;

                if (i == 0)
                {
                    recordHeader |= (byte)NdefRecord.EMessageInfoFlags.MB;
                }
                
                if (i == Count - 1)
                {
                    recordHeader |= (byte)NdefRecord.EMessageInfoFlags.ME;
                }

                if (record.Payload == null || record.Payload.Length < 255)
                {
                    recordHeader |= (byte)NdefRecord.EMessageInfoFlags.SR;

                }

                memoryStream.WriteByte(recordHeader);

                if (record.Payload == null)
                {
                    memoryStream.WriteByte(0);
                }
                else if (recordHeader == (byte)NdefRecord.EMessageInfoFlags.SR)
                {
                    memoryStream.WriteByte((byte)record.Payload.Length);
                }
                else
                {
                    memoryStream.WriteByte((byte)(record.Payload.Length >> 24));
                    memoryStream.WriteByte((byte)(record.Payload.Length >> 16));
                    memoryStream.WriteByte((byte)(record.Payload.Length >> 8));
                    memoryStream.WriteByte((byte)(record.Payload.Length >> 0xff));
                }

                if (record.Payload != null && record.Payload.Length > 0)
                {
                    memoryStream.Write(record.Payload, 0, record.Payload.Length);
                }
            }            
    
            return memoryStream.ToArray();
        }
    }
}
