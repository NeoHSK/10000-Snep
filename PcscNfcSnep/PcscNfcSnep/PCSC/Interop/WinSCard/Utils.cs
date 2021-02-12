using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace PcscNfcSnep.PCSC.Interop.WinSCard
{
    internal partial class WinSCard
    {
        internal static class Utils
        {
            /// <summary>
            /// Split the unmanaged C char
            /// </summary>
            /// <param name="chars"></param>
            /// <returns></returns>
            internal unsafe static IEnumerable<string> SplitStringByNull(byte* chars)
            {
                var array = new ArrayList();
                int offset = 0;
                string str = null;

                do
                {
                    str = Marshal.PtrToStringUni(new IntPtr(chars + offset));
                    if (str == null || str.Length == 0)
                    {
                        break;
                    }
                    array.Add(str);
                    offset += Encoding.Unicode.GetBytes(str).Length + 1; // for next char
                } while (true);

                return (string[])array.ToArray(typeof(string));
            }

            /// <summary>
            /// Convert byte to string
            /// </summary>
            /// <param name="bytes"></param>
            /// <param name="length"></param>
            /// <param name="includedNull"></param>
            /// <returns></returns>
            internal static string ByteTextToString(byte[] bytes, int length, bool includedNull)
            {
                int offset = (includedNull) ? -1 : 0; // -1: null
                return new ASCIIEncoding().GetString(bytes, 0, length + offset);
            }
        }
    }
}
