using PcscNfcSnep.NDEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static PcscNfcSnep.PCSC.Interop.WinSCard.WinSCard;
using static PcscNfcSnep.PCSC.Interop.WinSCard.WinSCard.Constants;

namespace PcscNfcSnep.PCSC.NFC
{
    class ReaderContext : IDisposable
    {
        IntPtr context = IntPtr.Zero;
        Reader[] mReaders = null;
        private bool disposedValue;

        public void InitializeReader()
        {
            context = EstablishContext(Scope.SCARD_SCOPE_USER);

            mReaders = ListReaders();

            foreach(var reader in mReaders)
            {
                reader.Connect(context);
            }
       
        }

        public void ReaderControl()
        {
            mReaders[0].Handle(SNEP.ECommand.Start, new NdefMessage());
        }


        public IEnumerable<Reader> Readers
        {
            get { return mReaders; }
        }

        bool IsEstablished
        {
            get { return context != IntPtr.Zero; }
        }


        #region IDisposable 
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(mReaders != null)
                    {
                        foreach (var reader in mReaders)
                        {
                            reader.Disconnect();
                        }
                    }
                    Cancel();
                    ReleaseContext();
                }

                // TODO: 비관리형 리소스(비관리형 개체)를 해제하고 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.
                mReaders = null;
                disposedValue = true;
            }
        }

        // // TODO: 비관리형 리소스를 해제하는 코드가 'Dispose(bool disposing)'에 포함된 경우에만 종료자를 재정의합니다.
        // ~ReaderContext()
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
        #endregion IDisposable

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
            if (IsEstablished)
            {
                NativeMethods.SCardCancel(context);
            }
        }
        #endregion
    }
}
