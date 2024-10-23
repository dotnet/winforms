// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Private.Windows.Core.BinaryFormat.Serializer;

/// <summary>
///  Base class for array records.
/// </summary>
/// <devdoc>
///  <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/f57d41e5-d3c0-4340-add8-fa4449a68d1c">
///   [MS-NRBF] 2.4
///  </see>
///  describes how item records must follow the array record and how multiple null records
///  can be coalesced into an <see cref="NullRecord.ObjectNullMultiple"/> or
///  <see cref="NullRecord.ObjectNullMultiple256"/> record.
/// </devdoc>
internal abstract class ArrayRecord : ObjectRecord, IEnumerable
{
    private protected readonly ArrayInfo _arrayInfo;

    /// <summary>
    ///  Identifier for the array.
    /// </summary>
    public override Id ObjectId => _arrayInfo.ObjectId;

    /// <summary>
    ///  Length of the array.
    /// </summary>
    public Count Length => _arrayInfo.Length;

    internal ArrayRecord(ArrayInfo arrayInfo) => _arrayInfo = arrayInfo;

    public abstract BinaryType ElementType { get; }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private protected abstract IEnumerator GetEnumerator();
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

    internal ArrayRecord(ArrayInfo arrayInfo, IReadOnlyList<T> arrayObjects) : base(arrayInfo)
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
