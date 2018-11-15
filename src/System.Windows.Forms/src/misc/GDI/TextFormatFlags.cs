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

    /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags"]/*' />
    [Flags]
#if WINFORMS_PUBLIC_GRAPHICS_LIBRARY
    public
#else
    internal
#endif
    enum IntTextFormatFlags
    {
        /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags.Bottom"]/*' />
        Bottom = IntNativeMethods.DT_BOTTOM,
        /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags.CalculateRectangle"]/*' />
        CalculateRectangle = IntNativeMethods.DT_CALCRECT,
        /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags.EndEllipsis"]/*' />
        EndEllipsis = IntNativeMethods.DT_END_ELLIPSIS,
        /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags.ExpandTabs"]/*' />
        ExpandTabs = IntNativeMethods.DT_EXPANDTABS,
        /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags.ExternalLeading"]/*' />
        ExternalLeading = IntNativeMethods.DT_EXTERNALLEADING,
        /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags.Left"]/*' />
        Default = Top | Left,
        /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags.HidePrefix"]/*' />
        HidePrefix = IntNativeMethods.DT_HIDEPREFIX,
        /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags.HorizontalCenter"]/*' />
        HorizontalCenter = IntNativeMethods.DT_CENTER,
        /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags.Internal"]/*' />
        Internal = IntNativeMethods.DT_INTERNAL,
        /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags.Left"]/*' />
        Left = IntNativeMethods.DT_LEFT, // default.
        /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags.ModifyString"]/*' />
        ModifyString = IntNativeMethods.DT_MODIFYSTRING,
        /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags.NoClipping"]/*' />
        NoClipping = IntNativeMethods.DT_NOCLIP,
        /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags.NoPrefix"]/*' />
        NoPrefix = IntNativeMethods.DT_NOPREFIX,
        /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags.NoFullWidthCharacterBreak"]/*' />
        NoFullWidthCharacterBreak = IntNativeMethods.DT_NOFULLWIDTHCHARBREAK,
        /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags.PathEllipsis"]/*' />
        PathEllipsis = IntNativeMethods.DT_PATH_ELLIPSIS,
        /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags.PrefixOnly"]/*' />
        PrefixOnly = IntNativeMethods.DT_PREFIXONLY,
        /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags.Right"]/*' />
        Right = IntNativeMethods.DT_RIGHT,
        /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags.RightToLeft"]/*' />
        RightToLeft = IntNativeMethods.DT_RTLREADING,
        /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags.SingleLine"]/*' />
        SingleLine = IntNativeMethods.DT_SINGLELINE,
        /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags.TabStop"]/*' />
        TabStop = IntNativeMethods.DT_TABSTOP,
        /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags.TextBoxControl"]/*' />
        TextBoxControl = IntNativeMethods.DT_EDITCONTROL,
        /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags.Top"]/*' />
        Top = IntNativeMethods.DT_TOP, // default.
        /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags.VerticalCenter"]/*' />
        VerticalCenter = IntNativeMethods.DT_VCENTER,
        /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags.WordBreak"]/*' />
        WordBreak = IntNativeMethods.DT_WORDBREAK,
        /// <include file='doc\TextFormatFlags.uex' path='docs/doc[@for="TextFormatFlags.WordEllipsis"]/*' />
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
