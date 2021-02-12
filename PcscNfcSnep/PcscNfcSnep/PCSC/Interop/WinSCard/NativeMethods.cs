using System;
using System.Security;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static PcscNfcSnep.PCSC.Interop.WinSCard.WinSCard.Constants;

namespace PcscNfcSnep.PCSC.Interop.WinSCard
{
    internal partial class WinSCard
    {
        [SuppressUnmanagedCodeSecurity]
        unsafe internal static class NativeMethods
        {
            const string KERNEL32_DLL = "kernel32.dll";
            const string WINSCARD_DLL = "winscard.dll";

            #region libloaderapi.h

            /// <summary>
            /// https://docs.microsoft.com/en-us/windows/desktop/api/libloaderapi/nf-libloaderapi-loadlibraryw
            /// </summary>
            /// <param name="lpLibFileName"></param>
            /// <returns></returns>
            [DllImport(KERNEL32_DLL, EntryPoint = "LoadLibraryW", CharSet = CharSet.Unicode, SetLastError = true)]
            private extern static IntPtr LoadLibrary(
                [In] string lpLibFileName);

            /// <summary>
            /// https://docs.microsoft.com/en-us/windows/desktop/api/libloaderapi/nf-libloaderapi-freelibrary
            /// </summary>
            /// <param name="hLibModule"></param>
            [DllImport(KERNEL32_DLL)]
            private extern static void FreeLibrary(
                [In] IntPtr hLibModule);

            /// <summary>
            /// https://docs.microsoft.com/en-us/windows/desktop/api/libloaderapi/nf-libloaderapi-getprocaddress
            /// </summary>
            /// <param name="hModule"></param>
            /// <param name="lpProcName"></param>
            /// <returns></returns>
            [DllImport(KERNEL32_DLL)]
            private extern static IntPtr GetProcAddress(
                [In] IntPtr hModule,
                [In] string lpProcName);

            #endregion libloaderapi.h

            internal static readonly IntPtr SCARD_PCI_T0;
            internal static readonly IntPtr SCARD_PCI_T1;
            internal static readonly IntPtr SCARD_PCI_RAW;

            /// <summary>
            /// https://docs.microsoft.com/en-us/windows/desktop/SecAuthN/scard-io-request
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            internal struct SCARD_IO_REQUEST
            {
                internal uint dwProtocol;
                internal uint cbPciLength;
            }

            /// <summary>
            /// https://docs.microsoft.com/en-us/windows/desktop/api/winscard/ns-winscard-scard_readerstatea
            /// </summary>
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            internal struct SCARD_READERSTATE
            {
                internal string szReader;
                internal IntPtr pvUserData;
                internal UInt32 dwCurrentState;
                internal UInt32 dwEventState;
                internal UInt32 cbAtr;

                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
                internal byte[] rgbAtr;
            }

            static NativeMethods()
            {
                var handle = LoadLibrary(WINSCARD_DLL);

                SCARD_PCI_T0 = GetProcAddress(handle, "g_rgSCardT0Pci");
                SCARD_PCI_T1 = GetProcAddress(handle, "g_rgSCardT1Pci");
                SCARD_PCI_RAW = GetProcAddress(handle, "g_rgSCardRawPci");
            }

            internal static IntPtr CardProtocol2PCI(Protocol protocol)
            {
                switch (protocol)
                {
                    case Protocol.SCARD_PROTOCOL_T0:
                        return SCARD_PCI_T0;

                    case Protocol.SCARD_PROTOCOL_T1:
                        return SCARD_PCI_T1;

                    case Protocol.SCARD_PROTOCOL_RAW:
                        return SCARD_PCI_RAW;

                    case Protocol.SCARD_PROTOCOL_UNDEFINED:
                        Debug.Assert(false);
                        return IntPtr.Zero;
                }
                return SCARD_PCI_T1;
            }

            /// <summary>
            /// https://docs.microsoft.com/en-us/windows/desktop/api/winscard/nf-winscard-scardcancel
            /// </summary>
            /// <param name="hContext"></param>
            /// <returns></returns>
            [DllImport(WINSCARD_DLL, SetLastError = true)]
            internal static extern int SCardCancel(
                [In] IntPtr hContext);

            /// <summary>
            /// https://docs.microsoft.com/en-us/windows/desktop/api/winscard/nf-winscard-scardconnectw
            /// </summary>
            /// <param name="hContext"></param>
            /// <param name="szReader"></param>
            /// <param name="dwShareMode"></param>
            /// <param name="dwPreferredProtocols"></param>
            /// <param name="phCard"></param>
            /// <param name="pdwActiveProtocol"></param>
            /// <returns></returns>
            [DllImport(WINSCARD_DLL, EntryPoint = "SCardConnectW", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern int SCardConnect(
                [In] IntPtr hContext,
                [In, MarshalAs(UnmanagedType.LPWStr)] string szReader,
                [In] uint dwShareMode,
                [In] uint dwPreferredProtocols,
                [Out] out IntPtr phCard,
                [Out] out uint pdwActiveProtocol);

            /// <summary>
            /// https://docs.microsoft.com/en-us/windows/desktop/api/winscard/nf-winscard-scardcontrol
            /// </summary>
            /// <param name="hCard"></param>
            /// <param name="dwControlCode"></param>
            /// <param name="lpInBuffer"></param>
            /// <param name="cbInBufferSize"></param>
            /// <param name="lpOutBuffer"></param>
            /// <param name="cbOutBufferSize"></param>
            /// <param name="lpBytesReturned"></param>
            /// <returns></returns>
            [DllImport(WINSCARD_DLL, SetLastError = true)]
            internal static extern uint SCardControl(
                [In] IntPtr hCard,
                [In] uint dwControlCode,
                [In] byte[] lpInBuffer,
                [In] uint cbInBufferSize,
                [In, Out] ref byte lpOutBuffer,
                [In] uint cbOutBufferSize,
                [Out] out uint lpBytesReturned);

            /// <summary>
            /// https://docs.microsoft.com/en-us/windows/desktop/api/winscard/nf-winscard-scarddisconnect
            /// </summary>
            /// <param name="hCard"></param>
            /// <param name="dwDisposition"></param>
            /// <returns></returns>
            [DllImport(WINSCARD_DLL, SetLastError = true)]
            internal static extern int SCardDisconnect(
                [In] IntPtr hCard,
                [In] uint dwDisposition);

            /// <summary>
            /// https://docs.microsoft.com/en-us/windows/desktop/api/winscard/nf-winscard-scardestablishcontext
            /// </summary>
            /// <param name="dwScope"></param>
            /// <param name="pvReserved1"></param>
            /// <param name="pvReserved2"></param>
            /// <param name="phContext"></param>
            /// <returns></returns>
            [DllImport(WINSCARD_DLL, SetLastError = true)]
            internal static extern int SCardEstablishContext(
                [In] uint dwScope,
                [In] IntPtr pvReserved1,
                [In] IntPtr pvReserved2,
                [Out] out IntPtr phContext);

            /// <summary>
            /// https://docs.microsoft.com/en-us/windows/desktop/api/winscard/nf-winscard-scardfreememory
            /// </summary>
            /// <param name="hContext"></param>
            /// <param name="pvMem"></param>
            /// <returns></returns>
            [DllImport(WINSCARD_DLL, SetLastError = true)]
            internal static extern int SCardFreeMemory(
                [In] IntPtr hContext,
                [In] byte* pvMem);

            /// <summary>
            /// https://docs.microsoft.com/en-us/windows/desktop/api/winscard/nf-winscard-scardgetstatuschangew
            /// </summary>
            /// <param name="hContext"></param>
            /// <param name="dwTimeout"></param>
            /// <param name="rgReaderStates"></param>
            /// <param name="cReaders"></param>
            /// <returns></returns>
            [DllImport(WINSCARD_DLL, EntryPoint = "SCardGetStatusChangeW", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern int SCardGetStatusChange(
                [In] IntPtr hContext,
                [In] uint dwTimeout,
                [In, Out] SCARD_READERSTATE[] rgReaderStates,
                [In] uint cReaders);

            /// <summary>
            /// https://docs.microsoft.com/en-us/windows/desktop/api/winscard/nf-winscard-scardlistreadersw
            /// </summary>
            /// <param name="hContext"></param>
            /// <param name="mszGroups"></param>
            /// <param name="mszReaders"></param>
            /// <param name="pcchReaders"></param>
            /// <returns></returns>
            [DllImport(WINSCARD_DLL, EntryPoint = "SCardListReadersW", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern int SCardListReaders(
                [In] IntPtr hContext,
                [In] string mszGroups,
                [Out] out byte* mszReaders,
                [In, Out] ref uint pcchReaders);

            /// <summary>
            /// https://docs.microsoft.com/en-us/windows/desktop/api/winscard/nf-winscard-scardreleasecontext
            /// </summary>
            /// <param name="phContext"></param>
            /// <returns></returns>
            [DllImport(WINSCARD_DLL, SetLastError = true)]
            internal static extern int SCardReleaseContext(
                [In] IntPtr phContext);

            /// <summary>
            /// https://docs.microsoft.com/en-us/windows/desktop/api/winscard/nf-winscard-scardtransmit
            /// </summary>
            /// <param name="hCard"></param>
            /// <param name="pioSendPci"></param>
            /// <param name="pbSendBuffer"></param>
            /// <param name="cbSendLength"></param>
            /// <param name="pioRecvPci"></param>
            /// <param name="pbRecvBuffer"></param>
            /// <param name="pcbRecvLength"></param>
            /// <returns></returns>
            [DllImport(WINSCARD_DLL, SetLastError = true)]
            internal static extern int SCardTransmit(
                [In] IntPtr hCard,
                [In] IntPtr pioSendPci,
                [In] byte[] pbSendBuffer,
                [In] uint cbSendLength,
                [In, Out] IntPtr pioRecvPci,
                [In, Out] ref byte pbRecvBuffer,
                [Out] out uint pcbRecvLength);
        }
    }
}
