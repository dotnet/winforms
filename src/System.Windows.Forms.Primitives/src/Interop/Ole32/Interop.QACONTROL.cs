// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        /// <remarks>
        ///  Specifies control information for IQuickActivate::QuickActivate.
        ///  See https://docs.microsoft.com/en-us/windows/win32/api/ocidl/ns-ocidl-qacontrol
        /// </remarks>
        public struct QACONTROL
        {
            public uint cbSize;
            public OLEMISC dwMiscStatus;
            public VIEWSTATUS dwViewStatus;
            public uint dwEventCookie;
            public uint dwPropNotifyCookie;
            public POINTERINACTIVE dwPointerActivationPolicy;
        }
    }
}
