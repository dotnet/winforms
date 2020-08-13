// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Mshtml
    {
        [ComImport]
        [Guid("163BB1E0-6E00-11CF-837A-48DC04C10000")]
        [InterfaceType(ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLLocation
        {
            void SetHref([In] string p);
            string GetHref();
            void SetProtocol([In] string p);
            string GetProtocol();
            void SetHost([In] string p);
            string GetHost();
            void SetHostname([In] string p);
            string GetHostname();
            void SetPort([In] string p);
            string GetPort();
            void SetPathname([In] string p);
            string GetPathname();
            void SetSearch([In] string p);
            string GetSearch();
            void SetHash([In] string p);
            string GetHash();
            void Reload([In] bool flag);
            void Replace([In] string bstr);
            void Assign([In] string bstr);
        }
    }
}
