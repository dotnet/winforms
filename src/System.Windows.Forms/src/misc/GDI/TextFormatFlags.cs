// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Internal
{
    [Flags]
    internal enum IntTextFormatFlags
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
        Left = IntNativeMethods.DT_LEFT,
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
        Top = IntNativeMethods.DT_TOP,
        VerticalCenter = IntNativeMethods.DT_VCENTER,
        WordBreak = IntNativeMethods.DT_WORDBREAK,
        WordEllipsis = IntNativeMethods.DT_WORD_ELLIPSIS,
    }
}
