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
        [Guid("9BFBBC02-EFF1-101A-84ED-00AA00341D07")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPropertyNotifySink
        {
            [PreserveSig]
            HRESULT OnChanged(Ole32.DispatchID dispID);

            [PreserveSig]
            HRESULT OnRequestEdit(Ole32.DispatchID dispID);
        }
    }
}
