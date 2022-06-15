// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.Automation;
using static Interop;
using static Interop.User32;

namespace System
{
    public static partial class TestAccessors
    {
        internal class UiaTextRangeTestAccessor : TestAccessor<UiaTextRange>
        {
            // Accessor for static members
            private static readonly dynamic Static = typeof(UiaTextRange).TestAccessor().Dynamic;

            public UiaTextRangeTestAccessor(UiaTextRange instance)
                : base(instance) { }

            public int _start
            {
                get => Dynamic._start;
                set => Dynamic._start = value;
            }

            public int _end
            {
                get => Dynamic._end;
                set => Dynamic._end = value;
            }

            public UiaTextProvider _provider => Dynamic._provider;

            public CapStyle GetCapStyle(ES editStyle) => Dynamic.GetCapStyle(editStyle);

            public double GetFontSize(LOGFONTW logfont) => Dynamic.GetFontSize(logfont);

            public HorizontalTextAlignment GetHorizontalTextAlignment(ES editStyle) => Dynamic.GetHorizontalTextAlignment(editStyle);

            public bool GetReadOnly() => Dynamic.GetReadOnly();

            public void MoveTo(int start, int end) => Dynamic.MoveTo(start, end);

            public void ValidateEndpoints() => Dynamic.ValidateEndpoints();

            public bool AtParagraphBoundary(string text, int index) => Static.AtParagraphBoundary(text, index);

            public bool AtWordBoundary(string text, int index) => Static.AtWordBoundary(text, index);

            public COLORREF GetBackgroundColor() => Static.GetBackgroundColor();

            public string GetFontName(LOGFONTW logfont) => Static.GetFontName(logfont);

            public bool IsApostrophe(char ch) => Static.IsApostrophe(ch);

            public Gdi32.FW GetFontWeight(LOGFONTW logfont) => Static.GetFontWeight(logfont);

            public COLORREF GetForegroundColor() => Static.GetForegroundColor();

            public bool GetItalic(LOGFONTW logfont) => Static.GetItalic(logfont);

            public TextDecorationLineStyle GetStrikethroughStyle(LOGFONTW logfont) => Static.GetStrikethroughStyle(logfont);

            public TextDecorationLineStyle GetUnderlineStyle(LOGFONTW logfont) => Static.GetUnderlineStyle(logfont);
        }

        internal static UiaTextRangeTestAccessor TestAccessor(this UiaTextRange textRange)
            => new UiaTextRangeTestAccessor(textRange);
    }
}
