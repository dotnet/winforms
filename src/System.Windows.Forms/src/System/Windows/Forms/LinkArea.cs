// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

[TypeConverter(typeof(LinkAreaConverter))]
[Serializable] // This type is participating in resx serialization scenarios.
public partial struct LinkArea : IEquatable<LinkArea>
{
#pragma warning disable IDE1006
    private int start; // Do NOT rename (binary serialization).
    private int length; // Do NOT rename (binary serialization).
#pragma warning restore IDE1006

    public LinkArea(int start, int length)
    {
        this.start = start;
        this.length = length;
    }

    public int Start
    {
        readonly get => start;
        set => start = value;
    }

    public int Length
    {
        readonly get => length;
        set => length = value;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public readonly bool IsEmpty => length == start && start == 0;

    public override readonly bool Equals(object? o)
    {
        if (o is not LinkArea a)
        {
            return false;
        }

        return Equals(a);
    }

    public readonly bool Equals(LinkArea other)
        => other.Start == start && other.Length == length;

    public override readonly string ToString() => $"{{Start={Start}, Length={Length}}}";

    public static bool operator ==(LinkArea linkArea1, LinkArea linkArea2)
        => linkArea1.Equals(linkArea2);

    public static bool operator !=(LinkArea linkArea1, LinkArea linkArea2)
        => !linkArea1.Equals(linkArea2);

    public override readonly int GetHashCode() => HashCode.Combine(start, length);
}
