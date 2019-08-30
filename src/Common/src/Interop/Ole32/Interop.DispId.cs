// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        public const int DISPID_VALUE = unchecked((int)0x0);
        public const int DISPID_UNKNOWN = unchecked((int)0xFFFFFFFF);
        public const int DISPID_PROPERTYPUT = unchecked((int)0xFFFFFFFD);
        public const int DISPID_AUTOSIZE = unchecked((int)0xFFFFFE0C);
        public const int DISPID_BACKCOLOR = unchecked((int)0xFFFFFE0B);
        public const int DISPID_BACKSTYLE = unchecked((int)0xFFFFFE0A);
        public const int DISPID_BORDERCOLOR = unchecked((int)0xFFFFFE09);
        public const int DISPID_BORDERSTYLE = unchecked((int)0xFFFFFE08);
        public const int DISPID_BORDERWIDTH = unchecked((int)0xFFFFFE07);
        public const int DISPID_DRAWMODE = unchecked((int)0xFFFFFE05);
        public const int DISPID_DRAWSTYLE = unchecked((int)0xFFFFFE04);
        public const int DISPID_DRAWWIDTH = unchecked((int)0xFFFFFE03);
        public const int DISPID_FILLCOLOR = unchecked((int)0xFFFFFE02);
        public const int DISPID_FILLSTYLE = unchecked((int)0xFFFFFE01);
        public const int DISPID_FONT = unchecked((int)0xFFFFFE00);
        public const int DISPID_FORECOLOR = unchecked((int)0xFFFFFDFF);
        public const int DISPID_ENABLED = unchecked((int)0xFFFFFDFE);
        public const int DISPID_HWND = unchecked((int)0xFFFFFDFD);
        public const int DISPID_TABSTOP = unchecked((int)0xFFFFFDFC);
        public const int DISPID_TEXT = unchecked((int)0xFFFFFDFB);
        public const int DISPID_CAPTION = unchecked((int)0xFFFFFDFA);
        public const int DISPID_BORDERVISIBLE = unchecked((int)0xFFFFFDF9);
        public const int DISPID_APPEARANCE = unchecked((int)0xFFFFFDF8);
        public const int DISPID_MOUSEPOINTER = unchecked((int)0xFFFFFDF7);
        public const int DISPID_MOUSEICON = unchecked((int)0xFFFFFDF6);
        public const int DISPID_PICTURE = unchecked((int)0xFFFFFDF5);
        public const int DISPID_VALID = unchecked((int)0xFFFFFDF4);
        public const int DISPID_READYSTATE = unchecked((int)0xFFFFFDF3);
        public const int DISPID_REFRESH = unchecked((int)0xFFFFFDDA);
        public const int DISPID_DOCLICK = unchecked((int)0xFFFFFDD9);
        public const int DISPID_ABOUTBOX = unchecked((int)0xFFFFFDD8);
        public const int DISPID_CLICK = unchecked((int)0xFFFFFDA8);
        public const int DISPID_DBLCLICK = unchecked((int)0xFFFFFDA7);
        public const int DISPID_KEYDOWN = unchecked((int)0xFFFFFDA6);
        public const int DISPID_KEYPRESS = unchecked((int)0xFFFFFDA5);
        public const int DISPID_KEYUP = unchecked((int)0xFFFFFDA4);
        public const int DISPID_MOUSEDOWN = unchecked((int)0xFFFFFDA3);
        public const int DISPID_MOUSEMOVE = unchecked((int)0xFFFFFDA2);
        public const int DISPID_MOUSEUP = unchecked((int)0xFFFFFDA1);
        public const int DISPID_ERROREVENT = unchecked((int)0xFFFFFDA0);
        public const int DISPID_RIGHTTOLEFT = unchecked((int)0xFFFFFD9D);
        public const int DISPID_READYSTATECHANGE = unchecked((int)0xFFFFFD9F);
        public const int DISPID_AMBIENT_BACKCOLOR = unchecked((int)0xFFFFFD43);
        public const int DISPID_AMBIENT_DISPLAYNAME = unchecked((int)0xFFFFFD42);
        public const int DISPID_AMBIENT_FONT = unchecked((int)0xFFFFFD41);
        public const int DISPID_AMBIENT_FORECOLOR = unchecked((int)0xFFFFFD40);
        public const int DISPID_AMBIENT_LOCALEID = unchecked((int)0xFFFFFD3F);
        public const int DISPID_AMBIENT_MESSAGEREFLECT = unchecked((int)0xFFFFFD3E);
        public const int DISPID_AMBIENT_SCALEUNITS = unchecked((int)0xFFFFFD3D);
        public const int DISPID_AMBIENT_TEXTALIGN = unchecked((int)0xFFFFFD3C);
        public const int DISPID_AMBIENT_USERMODE = unchecked((int)0xFFFFFD3B);
        public const int DISPID_AMBIENT_UIDEAD = unchecked((int)0xFFFFFD3A);
        public const int DISPID_AMBIENT_SHOWGRABHANDLES = unchecked((int)0xFFFFFD39);
        public const int DISPID_AMBIENT_SHOWHATCHING = unchecked((int)0xFFFFFD38);
        public const int DISPID_AMBIENT_DISPLAYASDEFAULT = unchecked((int)0xFFFFFD37);
        public const int DISPID_AMBIENT_SUPPORTSMNEMONICS = unchecked((int)0xFFFFFD36);
        public const int DISPID_AMBIENT_AUTOCLIP = unchecked((int)0xFFFFFD35);
        public const int DISPID_AMBIENT_APPEARANCE = unchecked((int)0xFFFFFD34);
        public const int DISPID_AMBIENT_PALETTE = unchecked((int)0xFFFFFD2A);
        public const int DISPID_AMBIENT_TRANSFERPRIORITY = unchecked((int)0xFFFFFD28);
        public const int DISPID_AMBIENT_RIGHTTOLEFT = unchecked((int)0xFFFFFD24);
        public const int DISPID_Name = unchecked((int)0xFFFFFCE0);
        public const int DISPID_Delete = unchecked((int)0xFFFFFCDF);
        public const int DISPID_Object = unchecked((int)0xFFFFFCDE);
        public const int DISPID_Parent = unchecked((int)0xFFFFFCDD);
    }
}
