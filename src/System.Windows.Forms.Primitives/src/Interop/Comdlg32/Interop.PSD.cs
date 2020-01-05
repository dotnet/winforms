// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal partial class Comdlg32
    {
        [Flags]
        public enum PSD : uint
        {
            DEFAULTMINMARGINS = 0x00000000,
            INWININIINTLMEASURE = 0x00000000,
            MINMARGINS = 0x00000001,
            MARGINS = 0x00000002,
            INTHOUSANDTHSOFINCHES = 0x00000004,
            INHUNDREDTHSOFMILLIMETERS = 0x00000008,
            DISABLEMARGINS = 0x00000010,
            DISABLEPRINTER = 0x00000020,
            NOWARNING = 0x00000080,
            DISABLEORIENTATION = 0x00000100,
            RETURNDEFAULT = 0x00000400,
            DISABLEPAPER = 0x00000200,
            SHOWHELP = 0x00000800,
            ENABLEPAGESETUPHOOK = 0x00002000,
            ENABLEPAGESETUPTEMPLATE = 0x00008000,
            ENABLEPAGESETUPTEMPLATEHANDLE = 0x00020000,
            ENABLEPAGEPAINTHOOK = 0x00040000,
            DISABLEPAGEPAINTING = 0x00080000,
            NONETWORKBUTTON = 0x00200000,
        }
    }
}
