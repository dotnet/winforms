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
        [Guid("7FD52380-4E07-101B-AE2D-08002B2EC713")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IPersistStreamInit /* : IPersist */
        {
            void GetClassID(out Guid pClassID);

            [PreserveSig]
            HRESULT IsDirty();

            void Load(IStream pstm);

            void Save(IStream pstm, Interop.BOOL fClearDirty);

            // pcbSize is optional so it must be a pointer
            void GetSizeMax(ulong* pcbSize);

            void InitNew();
        }
    }
}
