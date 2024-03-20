// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Automation;

namespace System;

public static partial class TestAccessors
{
    internal class UiaTextRangeTestAccessor : TestAccessor<UiaTextRange>
    {
        // Accessor for static members
        private static readonly dynamic s_static = typeof(UiaTextRange).TestAccessor().Dynamic;

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

        public CapStyle GetCapStyle(WINDOW_STYLE windowStyle) => (CapStyle)Dynamic.GetCapStyle(windowStyle);

        public double GetFontSize(LOGFONTW logfont) => Dynamic.GetFontSize(logfont);

        public HorizontalTextAlignment GetHorizontalTextAlignment(WINDOW_STYLE windowStyle)
            => (HorizontalTextAlignment)Dynamic.GetHorizontalTextAlignment(windowStyle);

        public bool GetReadOnly() => Dynamic.GetReadOnly();

        public void MoveTo(int start, int end) => Dynamic.MoveTo(start, end);

        public void ValidateEndpoints() => Dynamic.ValidateEndpoints();

        public bool AtParagraphBoundary(string text, int index) => s_static.AtParagraphBoundary(text, index);

        public bool AtWordBoundary(string text, int index) => s_static.AtWordBoundary(text, index);

        public COLORREF GetBackgroundColor() => s_static.GetBackgroundColor();

        public string GetFontName(LOGFONTW logfont) => s_static.GetFontName(logfont);

        public bool IsApostrophe(char ch) => s_static.IsApostrophe(ch);

        public FW GetFontWeight(LOGFONTW logfont) => (FW)s_static.GetFontWeight(logfont);

        public COLORREF GetForegroundColor() => s_static.GetForegroundColor();

        public bool GetItalic(LOGFONTW logfont) => s_static.GetItalic(logfont);

        public TextDecorationLineStyle GetStrikethroughStyle(LOGFONTW logfont) => (TextDecorationLineStyle)s_static.GetStrikethroughStyle(logfont);

        public TextDecorationLineStyle GetUnderlineStyle(LOGFONTW logfont) => (TextDecorationLineStyle)s_static.GetUnderlineStyle(logfont);
    }

    internal static UiaTextRangeTestAccessor TestAccessor(this UiaTextRange textRange)
        => new(textRange);
}
