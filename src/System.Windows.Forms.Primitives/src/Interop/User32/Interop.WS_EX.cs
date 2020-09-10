// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        /// <summary>
        ///  Extended Window Styles
        /// </summary>
        [Flags]
        public enum WS_EX : uint
        {
            DEFAULT             = 0x00000000,
            DLGMODALFRAME       = 0x00000001,
            NOPARENTNOTIFY      = 0x00000004,
            TOPMOST             = 0x00000008,
            ACCEPTFILES         = 0x00000010,
            TRANSPARENT         = 0x00000020,
            MDICHILD            = 0x00000040,
            TOOLWINDOW          = 0x00000080,
            WINDOWEDGE          = 0x00000100,
            CLIENTEDGE          = 0x00000200,
            CONTEXTHELP         = 0x00000400,
            RIGHT               = 0x00001000,
            LEFT                = 0x00000000,
            RTLREADING          = 0x00002000,
            LTRREADING          = 0x00000000,
            LEFTSCROLLBAR       = 0x00004000,
            RIGHTSCROLLBAR      = 0x00000000,
            CONTROLPARENT       = 0x00010000,
            STATICEDGE          = 0x00020000,
            APPWINDOW           = 0x00040000,
            OVERLAPPEDWINDOW    = WINDOWEDGE | CLIENTEDGE,
            PALETTEWINDOW       = WINDOWEDGE | TOOLWINDOW | TOPMOST,
            LAYERED             = 0x00080000,
            NOINHERITLAYOUT     = 0x00100000,
            NOREDIRECTIONBITMAP = 0x00200000,
            LAYOUTRTL           = 0x00400000,
            COMPOSITED          = 0x02000000,
            NOACTIVATE          = 0x08000000
        }
    }
}
