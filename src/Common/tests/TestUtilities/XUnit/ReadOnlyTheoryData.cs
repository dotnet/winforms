// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace Xunit;

/// <summary>
///  Xunit compatible readonly theory data. Can be constructed around <see cref="TheoryData"/>.
/// </summary>
/// <remarks>
///  <para>
///   This type avoids inadvertently adding data, which is of a particular concern when broadly sharing test data.
///  </para>
/// </remarks>
public class ReadOnlyTheoryData : IEnumerable<object?[]>
{
    private readonly IEnumerable<object?[]> _data;

    public ReadOnlyTheoryData(IEnumerable<object?[]> data) => _data = data;

    public ReadOnlyTheoryData(IEnumerable data)
        => _data = data.Cast<object>().Select(i => new object?[] { i }).ToArray();

    public ReadOnlyTheoryData(IEnumerable<object> data)
        => _data = data.Select(i => new object?[] { i }).ToArray();

    public ReadOnlyTheoryData(params object?[] data)
        => _data = data.Select(i => new object?[] { i }).ToArray();

    IEnumerator<object?[]> IEnumerable<object?[]>.GetEnumerator() => _data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _data.GetEnumerator();
}
