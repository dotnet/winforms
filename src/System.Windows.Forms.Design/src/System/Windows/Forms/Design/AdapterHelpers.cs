// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.Design;

/// <summary>
///  Helpers for adapting between generic and non-generic lists.
/// </summary>
internal static class AdapterHelpers
{
    /// <summary>
    ///  Provides an extension method to safely extract the underlying non-generic <see cref="IList"/>
    ///  from a wrapped <see cref="IList&lt;T&gt;"/> when available.
    /// </summary>
    /// <typeparam name="T">The type of elements in the <see cref="IList&lt;T&gt;"/>.</typeparam>
    /// <param name="list">The IList&lt;T&gt; to unwrap.</param>
    /// <returns>
    ///  The underlying non-generic <see cref="IList"/> if available; otherwise, the original <see cref="IList&lt;T&gt;"/>.
    /// </returns>
    internal static IList Unwrap<T>(this IList<T> list) => list is IWrapper<IList> wrapper ? wrapper.Unwrap() : (IList)list;

    /// <summary>
    ///  Provides an extension method to adapt a non-generic IList to a generic <see cref="IList&lt;T&gt;"/> by creating
    ///  a <see cref="ListAdapter&lt;T&gt;"/> when needed.
    /// </summary>
    /// <typeparam name="T">The desired type of elements in the resulting <see cref="IList&lt;T&gt;"/>.</typeparam>
    /// <param name="list">The non-generic <see cref="IList"/> to adapt.</param>
    /// <returns>
    ///  A generic <see cref="IList&lt;T&gt;"/> if the input IList can be cast to <see cref="IList&lt;T&gt;"/> otherwise,
    ///  a new <see cref="ListAdapter&lt;T&gt;"/> wrapping the input IList.
    /// </returns>
    internal static IList<T> Adapt<T>(this IList list) => list is IList<T> iList ? iList : new ListAdapter<T>(list);
}
