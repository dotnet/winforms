// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Core.BinaryFormat.Serializer;

/// <summary>
///  Base class for null records.
/// </summary>
internal abstract partial class NullRecord
{
    private Count _count;

    public virtual Count NullCount
    {
        get => _count;
        private protected set
        {
#pragma warning disable CA1512 // Use ArgumentOutOfRangeException throw helper - not possible in this case
            if (value == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
#pragma warning restore CA1512

            _count = value;
        }
    }

    internal static void Write(BinaryWriter writer, int nullCount)
    {
        switch (nullCount)
        {
            case 0:
                throw new ArgumentOutOfRangeException(nameof(nullCount));
            case 1:
                ObjectNull.Write(writer);
                break;
            case <= 255:
                new ObjectNullMultiple256(nullCount).Write(writer);
                break;
            default:
                new ObjectNullMultiple(nullCount).Write(writer);
                break;
        }
    }
}
