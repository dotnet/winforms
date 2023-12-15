// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Printing;

internal readonly partial struct TriState : IEquatable<TriState>
{
    private readonly byte _value; // 0 is "default", not false

    public static TriState Default { get; } = new(0);
    public static TriState False { get; } = new(1);
    public static TriState True { get; } = new(2);

    private TriState(byte value) => _value = value;

    public bool IsDefault => this == Default;

    public bool IsFalse => this == False;

    public bool IsNotDefault => this != Default;

    public bool IsTrue => this == True;

    public static bool operator ==(TriState left, TriState right) => left.Equals(right);

    public static bool operator !=(TriState left, TriState right) => !left.Equals(right);

    public override bool Equals([NotNullWhen(true)] object? o)
    {
        Debug.Assert(o is TriState);
        return Equals((TriState)o);
    }

    public bool Equals(TriState other) => _value == other._value;

    public override int GetHashCode() => _value;

    public static implicit operator TriState(bool value) => value ? True : False;

    public static explicit operator bool(TriState value) =>
        value.IsDefault ? throw new InvalidCastException(SR.TriStateCompareError) : value == True;

    /// <summary>Provides some interesting information about the TriState in String form.</summary>
    public override string ToString() =>
        this == Default ? "Default" :
        this == False ? "False" :
        "True";
}
