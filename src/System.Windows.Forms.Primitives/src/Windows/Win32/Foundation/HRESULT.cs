// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32.Foundation
{
    internal readonly partial struct HRESULT
    {
        public static HRESULT HRESULT_FROM_WIN32(WIN32_ERROR error)
            => new(((int)error & 0x0000FFFF) | unchecked((int)0x80070000));
    }
}
