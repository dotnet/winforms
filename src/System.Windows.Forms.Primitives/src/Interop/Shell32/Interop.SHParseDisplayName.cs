// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Shell32
    {
        [DllImport(Libraries.Shell32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static unsafe extern HRESULT SHParseDisplayName(char* pszName, IntPtr bindingContext, out IntPtr ppidl, uint sfgaoIn, out uint sfgaoOut);

        public static unsafe HRESULT SHParseDisplayName(string displayName, IntPtr bindingContext, out IntPtr ppidl, uint sfgaoIn, out uint sfgaoOut)
        {
            fixed (char* pszName = displayName)
            {
                return SHParseDisplayName(pszName, bindingContext, out ppidl, sfgaoIn, out sfgaoOut);
            }
        }
    }
}
