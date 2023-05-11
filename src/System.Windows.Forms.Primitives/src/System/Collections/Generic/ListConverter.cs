// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    private readonly IReadOnlyList<TIn> _values;
    private readonly Func<TIn, TOut> _converter;

    public ListConverter(IReadOnlyList<TIn> values, Func<TIn, TOut> conveter)
    {
        _values = values;
        _converter = conveter;
    }

    public TOut this[int index] => _converter(_values[index]);

    public int Count => _values.Count;

    // Saving a little bit of code by not writing an enumerator. It isn't huge, so this can be
    // added if needed.

    IEnumerator<TOut> IEnumerable<TOut>.GetEnumerator() => throw new NotImplementedException();
    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
}
