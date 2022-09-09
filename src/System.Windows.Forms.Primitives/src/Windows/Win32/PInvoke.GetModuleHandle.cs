// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public static unsafe HINSTANCE GetModuleHandle(string? lpModuleName)
        {
            // We are using GetModuleHandleEx because of the difficulties of the safe handle overload generated when
            // using GetModuleHandle
            fixed (char* lpModuleNameLocal = lpModuleName)
            {
                HINSTANCE hModule;
                BOOL result = GetModuleHandleEx(GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT, lpModuleNameLocal, &hModule);
                return hModule;
            }
        }
    }
}
