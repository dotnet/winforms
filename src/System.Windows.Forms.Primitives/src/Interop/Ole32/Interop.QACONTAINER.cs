// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [StructLayout(LayoutKind.Sequential)]
        public sealed class QACONTAINER
        {
            public uint cbSize;
            public IOleClientSite? pClientSite;
            public IAdviseSink? pAdviseSink;
            public IPropertyNotifySink? pPropertyNotifySink;
            [MarshalAs(UnmanagedType.Interface)]
            public object? pUnkEventSink;
            public QACONTAINERFLAGS dwAmbientFlags;
            public uint colorFore;
            public uint colorBack;
            public IFont? pFont;
            public IntPtr pUndoMgr;
            public uint dwAppearance;
            public Kernel32.LCID lcid;
            public IntPtr hpal;
            public IServiceProvider? pBindHost;
        }
    }
}
