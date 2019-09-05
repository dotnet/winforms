// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        public enum DispatchID : int
        {
            VALUE = unchecked((int)0x0),
            UNKNOWN = unchecked((int)0xFFFFFFFF),
            MEMBERID_NIL = UNKNOWN,
            PROPERTYPUT = unchecked((int)0xFFFFFFFD),
            AUTOSIZE = unchecked((int)0xFFFFFE0C),
            BACKCOLOR = unchecked((int)0xFFFFFE0B),
            BACKSTYLE = unchecked((int)0xFFFFFE0A),
            BORDERCOLOR = unchecked((int)0xFFFFFE09),
            BORDERSTYLE = unchecked((int)0xFFFFFE08),
            BORDERWIDTH = unchecked((int)0xFFFFFE07),
            DRAWMODE = unchecked((int)0xFFFFFE05),
            DRAWSTYLE = unchecked((int)0xFFFFFE04),
            DRAWWIDTH = unchecked((int)0xFFFFFE03),
            FILLCOLOR = unchecked((int)0xFFFFFE02),
            FILLSTYLE = unchecked((int)0xFFFFFE01),
            FONT = unchecked((int)0xFFFFFE00),
            FORECOLOR = unchecked((int)0xFFFFFDFF),
            ENABLED = unchecked((int)0xFFFFFDFE),
            HWND = unchecked((int)0xFFFFFDFD),
            TABSTOP = unchecked((int)0xFFFFFDFC),
            TEXT = unchecked((int)0xFFFFFDFB),
            CAPTION = unchecked((int)0xFFFFFDFA),
            BORDERVISIBLE = unchecked((int)0xFFFFFDF9),
            APPEARANCE = unchecked((int)0xFFFFFDF8),
            MOUSEPOINTER = unchecked((int)0xFFFFFDF7),
            MOUSEICON = unchecked((int)0xFFFFFDF6),
            PICTURE = unchecked((int)0xFFFFFDF5),
            VALID = unchecked((int)0xFFFFFDF4),
            READYSTATE = unchecked((int)0xFFFFFDF3),
            REFRESH = unchecked((int)0xFFFFFDDA),
            DOCLICK = unchecked((int)0xFFFFFDD9),
            ABOUTBOX = unchecked((int)0xFFFFFDD8),
            CLICK = unchecked((int)0xFFFFFDA8),
            DBLCLICK = unchecked((int)0xFFFFFDA7),
            KEYDOWN = unchecked((int)0xFFFFFDA6),
            KEYPRESS = unchecked((int)0xFFFFFDA5),
            KEYUP = unchecked((int)0xFFFFFDA4),
            MOUSEDOWN = unchecked((int)0xFFFFFDA3),
            MOUSEMOVE = unchecked((int)0xFFFFFDA2),
            MOUSEUP = unchecked((int)0xFFFFFDA1),
            ERROREVENT = unchecked((int)0xFFFFFDA0),
            RIGHTTOLEFT = unchecked((int)0xFFFFFD9D),
            READYSTATECHANGE = unchecked((int)0xFFFFFD9F),
            AMBIENT_BACKCOLOR = unchecked((int)0xFFFFFD43),
            AMBIENT_DISPLAYNAME = unchecked((int)0xFFFFFD42),
            AMBIENT_FONT = unchecked((int)0xFFFFFD41),
            AMBIENT_FORECOLOR = unchecked((int)0xFFFFFD40),
            AMBIENT_LOCALEID = unchecked((int)0xFFFFFD3F),
            AMBIENT_MESSAGEREFLECT = unchecked((int)0xFFFFFD3E),
            AMBIENT_SCALEUNITS = unchecked((int)0xFFFFFD3D),
            AMBIENT_TEXTALIGN = unchecked((int)0xFFFFFD3C),
            AMBIENT_USERMODE = unchecked((int)0xFFFFFD3B),
            AMBIENT_UIDEAD = unchecked((int)0xFFFFFD3A),
            AMBIENT_SHOWGRABHANDLES = unchecked((int)0xFFFFFD39),
            AMBIENT_SHOWHATCHING = unchecked((int)0xFFFFFD38),
            AMBIENT_DISPLAYASDEFAULT = unchecked((int)0xFFFFFD37),
            AMBIENT_SUPPORTSMNEMONICS = unchecked((int)0xFFFFFD36),
            AMBIENT_AUTOCLIP = unchecked((int)0xFFFFFD35),
            AMBIENT_APPEARANCE = unchecked((int)0xFFFFFD34),
            AMBIENT_PALETTE = unchecked((int)0xFFFFFD2A),
            AMBIENT_TRANSFERPRIORITY = unchecked((int)0xFFFFFD28),
            AMBIENT_RIGHTTOLEFT = unchecked((int)0xFFFFFD24),
            Name = unchecked((int)0xFFFFFCE0),
            Delete = unchecked((int)0xFFFFFCDF),
            Object = unchecked((int)0xFFFFFCDE),
            Parent = unchecked((int)0xFFFFFCDD),
        }
    }
}
