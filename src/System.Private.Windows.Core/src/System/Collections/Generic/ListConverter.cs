// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Collections.Generic;

/// <summary>
///  Helper class for converting values.
/// </summary>
/// <remarks>
///  <para>
///   It is intended to save the allocation of a temporary list when converting values. If there are multiple passes
///   through the list this class should usually be avoided.
///  </para>
/// </remarks>
internal class ListConverter<TIn, TOut> : IReadOnlyList<TOut>
{
    private readonly IList _values;
    private readonly Func<TIn, TOut> _converter;

    public ListConverter(IList values, Func<TIn, TOut> converter)
    {
        _values = values;
        _converter = converter;
    }

    public TOut this[int index] => _converter((TIn)_values[index]!);

    public int Count => _values.Count;

    // Saving a little bit of code by not writing an enumerator. It isn't huge, so this can be
    // added if needed.

    IEnumerator<TOut> IEnumerable<TOut>.GetEnumerator() => throw new NotImplementedException();
    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
}
