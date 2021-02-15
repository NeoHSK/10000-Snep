using PcscNfcSnep.NDEF;
using PcscNfcSnep.POC;
using System;
using System.Text;
using static PcscNfcSnep.PCSC.Interop.WinSCard.WinSCard;
using static PcscNfcSnep.PCSC.Interop.WinSCard.WinSCard.Constants;
using static PcscNfcSnep.PCSC.Interop.WinSCard.WinSCard.NativeMethods;

namespace PcscNfcSnep.PCSC.NFC
{
    public class Reader : IDisposable
    {
        IntPtr mReader = IntPtr.Zero;
        internal SCARD_READERSTATE ReaderState { get; set; }


        internal dynamic Handle(byte[] ndefRecord)
        {
            var ret = Control(mReader, ndefRecord);
            return ret;
        }

        internal Reader(string name)
        {
            ReaderState = new SCARD_READERSTATE()
            {
                szReader = name,
                dwCurrentState = (uint)State.SCARD_STATE_UNAWARE,
            };
        }


        internal void Connect(IntPtr context)
        {
            /* restart or error handle*/
            Connect(context, ReaderState.szReader, Share.SCARD_SHARE_DIRECT, Protocol.SCARD_PROTOCOL_UNDEFINED);
        }

        internal void Disconnect()
        {
            if (mReader != null)
            {
                Disconnect(mReader, Disposition.SCARD_LEAVE_CARD);
                mReader = IntPtr.Zero;
            }
        }

        #region WinSCard
        ReturnCode Connect(IntPtr context, string readerName, Share share, Protocol protocol)
        {
           return (ReturnCode)NativeMethods.SCardConnect(
                context,
                readerName,
                (uint)share,
                (uint)protocol,
                out mReader,
                out uint activeProtocol);
        }

        void Disconnect(IntPtr card, Disposition disconection)
        {
            if (card != IntPtr.Zero)
            {
                NativeMethods.SCardDisconnect(card, (uint)disconection);
            }
        }

        dynamic Control(IntPtr reader, byte[] inBuffer)
        {
            /*TODO Control out size*/
            var outBuffer = new byte[256];

            var ret = (ReturnCode)NativeMethods.SCardControl(
                reader,
                (int)PaSoRi.Contrl.SCARD_CTL_CODE,
                inBuffer,
                (uint)inBuffer.Length,
                ref outBuffer[0],
                (uint)outBuffer.Length,
                out uint bytesReturned);
            if (ret != ReturnCode.SCARD_S_SUCCESS)
            {
                throw new ApplicationException("Failed to execute the SCardControl: Returned value = " + ret);
            }
            return new
            {
                OutBuffer = outBuffer,
                BytesReturned = bytesReturned,
            };
        }
#endregion

#region IDisposable 
private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Disconnect();
                }

                // TODO: 비관리형 리소스(비관리형 개체)를 해제하고 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.
                disposedValue = true;
            }
        }

        // // TODO: 비관리형 리소스를 해제하는 코드가 'Dispose(bool disposing)'에 포함된 경우에만 종료자를 재정의합니다.
        // ~Reader()
        // {
        //     // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
