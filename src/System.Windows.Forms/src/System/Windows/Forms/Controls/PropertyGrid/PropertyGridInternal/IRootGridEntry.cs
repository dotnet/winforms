// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.PropertyGridInternal;

/// <summary>
///  A top level <see cref="GridEntry"/> that represents the selected object or objects in the
///  <see cref="PropertyGrid"/>.
/// </summary>
public interface IRootGridEntry
{
    /// <summary>
    ///  The set of attributes that will be used for browse filtering.
    /// </summary>
    AttributeCollection BrowsableAttributes { get; set; }

    /// <summary>
    ///  Resets <see cref="BrowsableAttributes"/> to the default value.
    /// </summary>
    /// <devdoc>
    ///  The default filter is currently <see cref="BrowsableAttribute.Yes"/>.
    /// </devdoc>
    void ResetBrowsableAttributes();

    /// <summary>
    ///  Set whether or not to show categories.
    /// </summary>
    void ShowCategories(bool showCategories);
}
