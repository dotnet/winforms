// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Oleaut32
    {
        [ComImport]
        [Guid("37D84F60-42CB-11CE-8135-00AA004BB851")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IPersistPropertyBag /* : IPersist */
        {
            [PreserveSig]
            HRESULT GetClassID(
                Guid* pClassID);

            [PreserveSig]
            HRESULT InitNew();

            void Load(
                IPropertyBag pPropBag,
                IErrorLog pErrorLog);

            void Save(
                IPropertyBag pPropBag,
                BOOL fClearDirty,
                BOOL fSaveAllProperties);
        }
    }
}
