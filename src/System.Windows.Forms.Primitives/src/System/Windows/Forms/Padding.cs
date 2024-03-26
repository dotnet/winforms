// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

[TypeConverter(typeof(PaddingConverter))]
[Serializable] // This type is participating in resx serialization scenarios.
[Runtime.CompilerServices.TypeForwardedFrom("System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
public struct Padding : IEquatable<Padding>
{
    private bool _all;      // Do NOT rename (binary serialization).
    private int _top;       // Do NOT rename (binary serialization).
    private int _left;      // Do NOT rename (binary serialization).
    private int _right;     // Do NOT rename (binary serialization).
    private int _bottom;    // Do NOT rename (binary serialization).

#pragma warning disable IDE1006 // Naming Styles: Shipped API
    public static readonly Padding Empty = new(0);
#pragma warning restore IDE1006

    public Padding(int all)
    {
        _all = true;
        _top = _left = _right = _bottom = all;
        Debug_SanityCheck();
    }

    public Padding(int left, int top, int right, int bottom)
    {
        _top = top;
        _left = left;
        _right = right;
        _bottom = bottom;
        _all = _top == _left && _top == _right && _top == _bottom;
        Debug_SanityCheck();
    }

    [RefreshProperties(RefreshProperties.All)]
    public int All
    {
        readonly get => _all ? _top : -1;
        set
        {
            if (!_all || _top != value)
            {
                _all = true;
                _top = _left = _right = _bottom = value;
            }

            Debug_SanityCheck();
        }
    }

    [RefreshProperties(RefreshProperties.All)]
    public int Bottom
    {
        readonly get => _all ? _top : _bottom;
        set
        {
            if (_all || _bottom != value)
            {
                _all = false;
                _bottom = value;
            }

            Debug_SanityCheck();
        }
    }

    [RefreshProperties(RefreshProperties.All)]
    public int Left
    {
        readonly get => _all ? _top : _left;
        set
        {
            if (_all || _left != value)
            {
                _all = false;
                _left = value;
            }

            Debug_SanityCheck();
        }
    }

    [RefreshProperties(RefreshProperties.All)]
    public int Right
    {
        readonly get => _all ? _top : _right;
        set
        {
            if (_all || _right != value)
            {
                _all = false;
                _right = value;
            }

            Debug_SanityCheck();
        }
    }

    [RefreshProperties(RefreshProperties.All)]
    public int Top
    {
        readonly get => _top;
        set
        {
            if (_all || _top != value)
            {
                _all = false;
                _top = value;
            }

            Debug_SanityCheck();
        }
    }

    [Browsable(false)]
    public readonly int Horizontal => Left + Right;

    [Browsable(false)]
    public readonly int Vertical => Top + Bottom;

    [Browsable(false)]
    public readonly Size Size => new(Horizontal, Vertical);

    public static Padding Add(Padding p1, Padding p2) => p1 + p2;

    public static Padding Subtract(Padding p1, Padding p2) => p1 - p2;

#pragma warning disable CA1725 // Parameter names should match base declaration. Shipped API.
    public override readonly bool Equals(object? other) => other is Padding otherPadding && Equals(otherPadding);
#pragma warning restore CA1725

    public readonly bool Equals(Padding other) =>
        Left == other.Left
            && Top == other.Top
            && Right == other.Right
            && Bottom == other.Bottom;

    /// <summary>
    ///  Performs vector addition of two <see cref="Padding"/> objects.
    /// </summary>
    public static Padding operator +(Padding p1, Padding p2) =>
        new(p1.Left + p2.Left, p1.Top + p2.Top, p1.Right + p2.Right, p1.Bottom + p2.Bottom);

    /// <summary>
    ///  Contracts a <see cref="Drawing.Size"/> by another <see cref="Drawing.Size"/>.
    /// </summary>
    public static Padding operator -(Padding p1, Padding p2) =>
        new(p1.Left - p2.Left, p1.Top - p2.Top, p1.Right - p2.Right, p1.Bottom - p2.Bottom);

    /// <summary>
    ///  Tests whether two <see cref="Padding"/> objects are identical.
    /// </summary>
    public static bool operator ==(Padding p1, Padding p2) =>
        p1.Left == p2.Left && p1.Top == p2.Top && p1.Right == p2.Right && p1.Bottom == p2.Bottom;

    /// <summary>
    ///  Tests whether two <see cref="Padding"/> objects are different.
    /// </summary>
    public static bool operator !=(Padding p1, Padding p2) => !(p1 == p2);

    public override readonly int GetHashCode() => HashCode.Combine(Left, Top, Right, Bottom);

    public override readonly string ToString() => $"{{Left={Left},Top={Top},Right={Right},Bottom={Bottom}}}";

    private void ResetAll() => All = 0;

    private void ResetBottom() => Bottom = 0;

    private void ResetLeft() => Left = 0;

    private void ResetRight() => Right = 0;

    private void ResetTop() => Top = 0;

    internal void Scale(float dx, float dy)
    {
        _top = (int)(_top * dy);
        _left = (int)(_left * dx);
        _right = (int)(_right * dx);
        _bottom = (int)(_bottom * dy);
    }

    internal readonly bool ShouldSerializeAll() => _all;

    [Conditional("DEBUG")]
    private readonly void Debug_SanityCheck()
    {
        if (_all)
        {
            Debug.Assert(ShouldSerializeAll(), "_all is true, but ShouldSerializeAll() is false.");
            Debug.Assert(All == Left && Left == Top && Top == Right && Right == Bottom, "_all is true, but All/Left/Top/Right/Bottom inconsistent.");
        }
        else
        {
            Debug.Assert(All == -1, "_all is false, but All != -1.");
            Debug.Assert(!ShouldSerializeAll(), "ShouldSerializeAll() should not be true when all flag is not set.");
        }
    }
}
