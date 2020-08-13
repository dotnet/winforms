// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Oleaut32
    {
        public struct EXCEPINFO
        {
            public ushort wCode;
            public ushort wReserved;
            public IntPtr bstrSource;
            public IntPtr bstrDescription;
            public IntPtr bstrHelpFile;
            public uint dwHelpContext;
            public IntPtr pvReserved;
            public IntPtr pfnDeferredFillIn;
            public HRESULT scode;
        }
    }
}
