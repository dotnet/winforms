// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Runtime.Serialization;

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  Base class for array records.
/// </summary>
/// <devdoc>
///  <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/f57d41e5-d3c0-4340-add8-fa4449a68d1c">
///  [MS-NRBF] 2.4</see> describes how item records must follow the array record and how multiple null records
///  can be coalesced into an <see cref="NullRecord.ObjectNullMultiple"/> or <see cref="NullRecord.ObjectNullMultiple256"/>
///  record.
/// </devdoc>
internal abstract class ArrayRecord : ObjectRecord, IEnumerable
{
    public ArrayInfo ArrayInfo { get; }

    /// <summary>
    ///  Identifier for the array.
    /// </summary>
    public override Id ObjectId => ArrayInfo.ObjectId;

    /// <summary>
    ///  Length of the array.
    /// </summary>
    public Count Length => ArrayInfo.Length;

    public ArrayRecord(ArrayInfo arrayInfo) => ArrayInfo = arrayInfo;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private protected abstract IEnumerator GetEnumerator();

    /// <summary>
    ///  Reads records, expanding null records into individual entries.
    /// </summary>
    private protected static IReadOnlyList<object?> ReadObjectArrayValues(BinaryFormattedObject.IParseState state, Count count)
        => ReadObjectArrayValues(state, BinaryType.Object, null, count);

    /// <summary>
    ///  Reads a count of object member values of <paramref name="type"/> with optional clarifying <paramref name="typeInfo"/>.
    /// </summary>
    /// <exception cref="SerializationException"><paramref name="type"/> was unexpected.</exception>
    private protected static IReadOnlyList<object?> ReadObjectArrayValues(
        BinaryFormattedObject.IParseState state,
        BinaryType type,
        object? typeInfo,
        int count)
    {
        if (count == 0)
        {
            return [];
        }

        ArrayBuilder<object?> memberValues = new(count);
        for (int i = 0; i < count; i++)
        {
            object value = ReadValue(state, type, typeInfo);
            if (value is not NullRecord nullRecord)
            {
                memberValues.Add(value);
                continue;
            }

            i = checked(i + nullRecord.NullCount - 1);
            if (i >= count)
            {
                throw new SerializationException();
            }

            for (int j = 0; j < nullRecord.NullCount; j++)
            {
                memberValues.Add(null);
            }
        }

        return memberValues.ToArray();
    }
}

/// <summary>
///  Typed class for array records.
/// </summary>
internal abstract class ArrayRecord<T> : ArrayRecord, IEnumerable<T>
{
    /// <summary>
    ///  The array items.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Multi-null records are always expanded to individual <see cref="ObjectNull"/> entries when reading.
    ///  </para>
    /// </remarks>
    public IReadOnlyList<T> ArrayObjects { get; }

    /// <summary>
    ///  Returns the item at the given index.
    /// </summary>
    public T this[int index] => ArrayObjects[index];

    public ArrayRecord(ArrayInfo arrayInfo, IReadOnlyList<T> arrayObjects) : base(arrayInfo)
    {
        if (arrayInfo.Length != arrayObjects.Count)
        {
            throw new ArgumentException($"{nameof(arrayInfo)} doesn't match count of {nameof(arrayObjects)}");
        }

        ArrayObjects = arrayObjects;
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => ArrayObjects.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ArrayObjects.GetEnumerator();
    private protected override IEnumerator GetEnumerator() => ArrayObjects.GetEnumerator();
}
