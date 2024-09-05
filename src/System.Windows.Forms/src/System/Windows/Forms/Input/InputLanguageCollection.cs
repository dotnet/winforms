// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms;

/// <summary>
///  A collection that stores <see cref="InputLanguage"/> objects.
/// </summary>
public class InputLanguageCollection : ReadOnlyCollectionBase
{
    /// <summary>
    ///  Initializes a new instance of <see cref="InputLanguageCollection"/> containing any array of
    ///  <see cref="InputLanguage"/> objects.
    /// </summary>
    internal InputLanguageCollection(InputLanguage[] value)
    {
        Debug.Assert(value is not null);
        Debug.Assert(Array.IndexOf(value, null!) == -1, "Should not contain null");

        InnerList.AddRange(value);
    }

    /// <summary>
    ///  Represents the entry at the specified index of the <see cref="InputLanguage"/>.
    /// </summary>
    public InputLanguage this[int index] => (InputLanguage)InnerList[index]!; // Forcing non-nullable due to legacy requirements.

    /// <summary>
    ///  Gets a value indicating whether the
    ///  <see cref="InputLanguageCollection"/> contains the specified <see cref="InputLanguage"/>.
    /// </summary>
    public bool Contains(InputLanguage? value) => InnerList.Contains(value);

    /// <summary>
    ///  Copies the <see cref="InputLanguageCollection"/> values to a one-dimensional <see cref="Array"/> instance at the
    ///  specified index.
    /// </summary>
    public void CopyTo(InputLanguage[] array, int index) => InnerList.CopyTo(array, index);

    /// <summary>
    ///  Returns the index of a <see cref="InputLanguage"/> in
    ///  the <see cref="InputLanguageCollection"/> .
    /// </summary>
    public int IndexOf(InputLanguage? value) => InnerList.IndexOf(value);
}
