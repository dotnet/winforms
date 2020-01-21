// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [ComImport]
        [Guid("0000010A-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IPersistStorage /* : IPersist */
        {
            [PreserveSig]
            HRESULT GetClassID(
                Guid* pClassID);

            [PreserveSig]
            HRESULT IsDirty();

            void InitNew(IStorage pstg);

            [PreserveSig]
            HRESULT Load(IStorage pstg);

            void Save(IStorage pStgSave, Interop.BOOL fSameAsLoad);

            void SaveCompleted(IStorage pStgNew);

            void HandsOffStorage();
        }
    }
}
