// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Tests;

public class ISpanFormattableTests
{
    private readonly struct FormattableInt32 : ISpanFormattable
    {
        private readonly int _value;

        public FormattableInt32(int value) => _value = value;

        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        {
            string formatted = _value.ToString(format.Length > 0 ? format.ToString() : null, provider);
            if (formatted.Length > destination.Length)
            {
                charsWritten = 0;
                return false;
            }

            formatted.AsSpan().CopyTo(destination);
            charsWritten = formatted.Length;
            return true;
        }

        public string ToString(string? format, IFormatProvider? formatProvider) => _value.ToString(format, formatProvider);
    }

    [Fact]
    public void TryFormat_Success()
    {
        ISpanFormattable formattable = new FormattableInt32(42);
        Span<char> buffer = stackalloc char[10];

        bool result = formattable.TryFormat(buffer, out int charsWritten, default, null);

        result.Should().BeTrue();
        charsWritten.Should().Be(2);
        buffer[..charsWritten].SequenceEqual("42").Should().BeTrue();
    }

    [Fact]
    public void TryFormat_BufferTooSmall_ReturnsFalse()
    {
        ISpanFormattable formattable = new FormattableInt32(12345);
        Span<char> buffer = stackalloc char[2];

        bool result = formattable.TryFormat(buffer, out int charsWritten, default, null);

        result.Should().BeFalse();
        charsWritten.Should().Be(0);
    }

    [Fact]
    public void TryFormat_NegativeValue()
    {
        ISpanFormattable formattable = new FormattableInt32(-7);
        Span<char> buffer = stackalloc char[10];

        bool result = formattable.TryFormat(buffer, out int charsWritten, default, null);

        result.Should().BeTrue();
        buffer[..charsWritten].SequenceEqual("-7").Should().BeTrue();
    }

    [Fact]
    public void TryFormat_MatchesToString()
    {
        FormattableInt32 formattable = new(12345);
        Span<char> buffer = stackalloc char[20];

        bool result = formattable.TryFormat(buffer, out int charsWritten, default, null);
        string fromTryFormat = buffer[..charsWritten].ToString();
        string fromToString = formattable.ToString(null, null);

        result.Should().BeTrue();
        fromTryFormat.Should().Be(fromToString);
    }

    [Fact]
    public void ISpanFormattable_IsIFormattable()
    {
        typeof(ISpanFormattable).IsAssignableTo(typeof(IFormattable)).Should().BeTrue();
    }
}
