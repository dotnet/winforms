// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32.Foundation
{
    internal readonly partial struct WPARAM
    {
        public static implicit operator nint(WPARAM value) => (nint)value.Value;
        public static implicit operator WPARAM(nint value) => (WPARAM)(nuint)value;
    }
}
