// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;
using System.Windows.Forms.VisualStyles;
using Microsoft.Win32;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class PropertyGridView
    {
        // RenderMode
        public const int RENDERMODE_LEFTDOT = 2;
        public const int RENDERMODE_BOLD = 3;
        public const int RENDERMODE_TRIANGLE = 4;
        private const int LEFTDOT_SIZE = 4;

        // flags
        private const short FlagNeedsRefresh = 0x0001;
        private const short FlagIsNewSelection = 0x0002;
        private const short FlagIsSplitterMove = 0x0004;
        private const short FlagIsSpecialKey = 0x0008;
        private const short FlagInPropertySet = 0x0010;
        private const short FlagDropDownClosing = 0x0020;
        private const short FlagDropDownCommit = 0x0040;
        private const short FlagNeedUpdateUIBasedOnFont = 0x0080;
        private const short FlagBtnLaunchedEditor = 0x0100;
        private const short FlagNoDefault = 0x0200;
        private const short FlagResizableDropDown = 0x0400;

        // other
        private const int EDIT_INDENT = 0;
        private const int OUTLINE_INDENT = 10;
        private const int OUTLINE_SIZE = 9;
        private const int OUTLINE_SIZE_EXPLORER_TREE_STYLE = 16;
        private const int PAINT_WIDTH = 20;
        private const int PAINT_INDENT = 26;
        private const int ROWLABEL = 1;
        private const int ROWVALUE = 2;
        private const int MAX_LISTBOX_HEIGHT = 200;

        private const int OFFSET_2PIXELS = 2;

        internal const short GDIPLUS_SPACE = 2;
        internal const int MAX_RECURSE_EXPAND = 10;
        private const short ERROR_NONE = 0;
        private const short ERROR_THROWN = 1;
        private const short ERROR_MSGBOX_UP = 2;

        private const int DOTDOTDOT_ICONWIDTH = 7;
        private const int DOTDOTDOT_ICONHEIGHT = 8;
        private const int DOWNARROW_ICONWIDTH = 16;
        private const int DOWNARROW_ICONHEIGHT = 16;
    }
}
