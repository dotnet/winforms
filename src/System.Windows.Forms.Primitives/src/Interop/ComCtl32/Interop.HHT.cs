// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public enum HHT : uint
        {
             NOWHERE             = 0x0001,
             ONHEADER            = 0x0002,
             ONDIVIDER           = 0x0004,
             ONDIVOPEN           = 0x0008,
             ONFILTER            = 0x0010,
             ONFILTERBUTTON      = 0x0020,
             ABOVE               = 0x0100,
             BELOW               = 0x0200,
             TORIGHT             = 0x0400,
             TOLEFT              = 0x0800,
             ONITEMSTATEICON     = 0x1000,
             ONDROPDOWN          = 0x2000,
             ONOVERFLOW          = 0x4000,
        }
    }
}
