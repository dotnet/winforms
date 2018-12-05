// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINFORMS_NAMESPACE
namespace System.Windows.Forms.Internal
#elif DRAWING_NAMESPACE
namespace System.Drawing.Internal
#else
namespace System.Experimental.Gdi
#endif
{
    using System;

    [Flags]
#if WINFORMS_PUBLIC_GRAPHICS_LIBRARY
    public
#else
    internal
#endif
    enum IntTextFormatFlags
    {
        Bottom = IntNativeMethods.DT_BOTTOM,
        CalculateRectangle = IntNativeMethods.DT_CALCRECT,
        EndEllipsis = IntNativeMethods.DT_END_ELLIPSIS,
        ExpandTabs = IntNativeMethods.DT_EXPANDTABS,
        ExternalLeading = IntNativeMethods.DT_EXTERNALLEADING,
        Default = Top | Left,
        HidePrefix = IntNativeMethods.DT_HIDEPREFIX,
        HorizontalCenter = IntNativeMethods.DT_CENTER,
        Internal = IntNativeMethods.DT_INTERNAL,
        Left = IntNativeMethods.DT_LEFT, // default.
        ModifyString = IntNativeMethods.DT_MODIFYSTRING,
        NoClipping = IntNativeMethods.DT_NOCLIP,
        NoPrefix = IntNativeMethods.DT_NOPREFIX,
        NoFullWidthCharacterBreak = IntNativeMethods.DT_NOFULLWIDTHCHARBREAK,
        PathEllipsis = IntNativeMethods.DT_PATH_ELLIPSIS,
        PrefixOnly = IntNativeMethods.DT_PREFIXONLY,
        Right = IntNativeMethods.DT_RIGHT,
        RightToLeft = IntNativeMethods.DT_RTLREADING,
        SingleLine = IntNativeMethods.DT_SINGLELINE,
        TabStop = IntNativeMethods.DT_TABSTOP,
        TextBoxControl = IntNativeMethods.DT_EDITCONTROL,
        Top = IntNativeMethods.DT_TOP, // default.
        VerticalCenter = IntNativeMethods.DT_VCENTER,
        WordBreak = IntNativeMethods.DT_WORDBREAK,
        WordEllipsis = IntNativeMethods.DT_WORD_ELLIPSIS,
        //		#define DT_TOP                      0x00000000
        //		#define DT_LEFT                     0x00000000
        //		#define DT_CENTER                   0x00000001
        //		#define DT_RIGHT                    0x00000002
        //		#define DT_VCENTER                  0x00000004
        //		#define DT_BOTTOM                   0x00000008
        //		#define DT_WORDBREAK                0x00000010
        //		#define DT_SINGLELINE               0x00000020
        //		#define DT_EXPANDTABS               0x00000040
        //		#define DT_TABSTOP                  0x00000080
        //		#define DT_NOCLIP                   0x00000100
        //		#define DT_EXTERNALLEADING          0x00000200
        //		#define DT_CALCRECT                 0x00000400
        //		#define DT_NOPREFIX                 0x00000800
        //		#define DT_INTERNAL                 0x00001000
        //		#define DT_EDITCONTROL              0x00002000
        //		#define DT_PATH_ELLIPSIS            0x00004000
        //		#define DT_END_ELLIPSIS             0x00008000
        //		#define DT_MODIFYSTRING             0x00010000
        //		#define DT_RTLREADING               0x00020000
        //		#define DT_WORD_ELLIPSIS            0x00040000
        //		#define DT_NOFULLWIDTHCHARBREAK     0x00080000
        //		#define DT_HIDEPREFIX               0x00100000
        //		#define DT_PREFIXONLY               0x00200000
    }
}
