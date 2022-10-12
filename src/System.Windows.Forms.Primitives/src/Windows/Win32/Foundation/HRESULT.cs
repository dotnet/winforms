// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace Windows.Win32.Foundation
{
    internal readonly partial struct HRESULT
    {
        public static HRESULT HRESULT_FROM_WIN32(WIN32_ERROR error)
            => new(((int)error & 0x0000FFFF) | unchecked((int)0x80070000));

        public static implicit operator HRESULT(Exception ex)
        {
            Debug.WriteLine(ex);
            return (HRESULT)ex.HResult;
        }

        // COR_* HRESULTs are .NET HRESULTs
        internal static readonly HRESULT COR_E_OBJECTDISPOSED = (HRESULT)unchecked((int)0x80131622);
    }
}
