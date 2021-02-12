using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static PcscNfcSnep.PCSC.Interop.WinSCard.WinSCard;
using static PcscNfcSnep.PCSC.Interop.WinSCard.WinSCard.Constants;
using static PcscNfcSnep.PCSC.Interop.WinSCard.WinSCard.NativeMethods;

namespace PcscNfcSnep.PCSC.NFC
{
    public class Reader : IDisposable
    {
        IntPtr context = IntPtr.Zero;
        IntPtr mCard = IntPtr.Zero;
        Reader[] mReaders = null;
        internal SCARD_READERSTATE ReaderState { get; set; }

        bool IsEstablished
        {
            get { return context != IntPtr.Zero; }
        }

        public IEnumerable<Reader> Readers
        {
            get { return mReaders; }
        }

        internal Reader(string name)
        {
            ReaderState = new SCARD_READERSTATE()
            {
                szReader = name,
                dwCurrentState = (uint)State.SCARD_STATE_UNAWARE,
            };
        }


        internal void  Connect(IntPtr context)
        {
            var ret = Connect(context, ReaderState.szReader, Share.SCARD_SHARE_DIRECT, Protocol.SCARD_PROTOCOL_UNDEFINED);
        }

        internal void Disconnect()
        {
            if (mCard != null)
            {
                Disconnect(mCard, Disposition.SCARD_LEAVE_CARD);
                mCard = IntPtr.Zero;
            }

            Cancel();
            ReleaseContext();
        }

        #region WinSCard
        IntPtr EstablishContext(Scope scope)
        {
            var context = IntPtr.Zero;
            if (!IsEstablished)
            {
                var ret = (ReturnCode)NativeMethods.SCardEstablishContext(
                    (uint)scope,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    out context);
                if (ret != ReturnCode.SCARD_S_SUCCESS)
                {
                    throw new ApplicationException("Failed to execute the SCardEstablishContext: Returned value = " + ret);
                }
            }
            return context;
        }

        void ReleaseContext()
        {
            if (IsEstablished)
            {
                var ret = NativeMethods.SCardReleaseContext(context);
                context = IntPtr.Zero;
            }
        }

        unsafe Reader[] ListReaders()
        {
            var readersCount = (uint)SCARD_AUTOALLOCATE; // auto alocate
            var ret = (ReturnCode)NativeMethods.SCardListReaders(
                context,
                null,
                out byte* cReaders,
                ref readersCount);
            if (ret != ReturnCode.SCARD_S_SUCCESS)
            {
                switch (ret)
                {
                    case ReturnCode.SCARD_E_NO_READERS_AVAILABLE:
                        break;

                    default:
                        break;
                }
            }

            var readers = Utils.SplitStringByNull(cReaders);
            NativeMethods.SCardFreeMemory(context, cReaders); // free

            return readers.Select(reader => new Reader(reader)).ToArray();
        }
        void Cancel()
        {
            NativeMethods.SCardCancel(context);
        }
        ReturnCode Connect(IntPtr context, string readerName, Share share, Protocol protocol)
        {
            return (ReturnCode)NativeMethods.SCardConnect(
                context,
                readerName,
                (uint)share,
                (uint)protocol,
                out IntPtr card,
                out uint activeProtocol);
        }

        void Disconnect(IntPtr card, Disposition disconection)
        {
            if (card != IntPtr.Zero)
            {
                NativeMethods.SCardDisconnect(card, (uint)disconection);
            }
        }

        ReturnCode Control(IntPtr card, uint controlCode, byte[] inBuffer)
        {
            var outBuffer = new byte[100000];

            return (ReturnCode)NativeMethods.SCardControl(
                card,
                (int)PaSoRi.Contrl.SCARD_CTL_CODE,
                inBuffer,
                (uint)inBuffer.Length,
                ref outBuffer[0],
                (uint)outBuffer.Length,
                out uint bytesReturned);
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
