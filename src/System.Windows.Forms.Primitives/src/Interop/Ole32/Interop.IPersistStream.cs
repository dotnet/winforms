// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [ComImport]
        [Guid("00000109-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IPersistStream /* : IPersist */
        {
            [PreserveSig]
            HRESULT GetClassID(
                Guid* pClassID);

            [PreserveSig]
            HRESULT IsDirty();

            void Load(IStream pstm);

            void Save(IStream pstm, Interop.BOOL fClearDirty);

            long GetSizeMax();
        }
    }
}
