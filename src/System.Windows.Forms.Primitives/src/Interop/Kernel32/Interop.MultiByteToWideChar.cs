// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Kernel32
    {
        // SECREVIEW: From MSDN: Using the MultiByteToWideChar/WideCharToMultiByte function incorrectly can compromise the security of your application. 
        //            Calling the WideCharToMultiByte function can easily cause a buffer overrun because the size of the In buffer equals the number 
        //            of WCHARs in the string, while the size of the Out buffer equals the number of bytes. To avoid a buffer overrun, be sure to specify 
        //            a buffer size appropriate for the data type the buffer receives. For more information, see Security Considerations: International Features. 
        //            Always call these functions passing a null destination buffer to get its size and the create the buffer with the exact size.
        [DllImport(ExternDll.Kernel32, ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int MultiByteToWideChar(int CodePage, int dwFlags, byte[] lpMultiByteStr, int cchMultiByte, char[] lpWideCharStr, int cchWideChar);
    }
}
