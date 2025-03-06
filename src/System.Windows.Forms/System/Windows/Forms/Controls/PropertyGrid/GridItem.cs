// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
///  Representation of one row item in the <see cref="PropertyGrid"/>. These items represent the hierarchy of the
///  grid's "tree-like" view and can be used to get information about the grid's state and contents.
/// </summary>
/// <remarks>
///  <para>
///   These objects should not be cached because they represent a snapshot of the <see cref="PropertyGrid"/>'s
///   state and may be disposed by grid activity. The <see cref="PropertyGrid"/> often recreates these objects
///   internally even if it doesn't appear to change to the user.
///  </para>
/// </remarks>
public abstract class GridItem
{
    /// <summary>
    ///  Gets or sets user-defined data about the <see cref="GridItem"/>.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [Localizable(false)]
    [Bindable(true)]
    [SRDescription(nameof(SR.ControlTagDescr))]
    [DefaultValue(null)]
    [TypeConverter(typeof(StringConverter))]
    public object? Tag { get; set; }

    /// <summary>
    ///  Retrieves the child <see cref="GridItem"/>s, if any, of this <see cref="GridItem"/>.
    /// </summary>
    public abstract GridItemCollection GridItems { get; }

    /// <summary>
    ///  Retrieves the type of this <see cref="GridItem"/>.
    /// </summary>
    public abstract GridItemType GridItemType { get; }

    /// <summary>
    ///  Retrieves the text label of this <see cref="GridItem"/>. This may be different from the actual
    ///  PropertyName. For <see cref="GridItemType.Property"/> <see cref="GridItem"/>s, retrieve the
    ///  <see cref="System.ComponentModel.PropertyDescriptor"/> and check its Name property.
    /// </summary>
    public abstract string? Label { get; }

    /// <summary>
    ///  Retrieves parent <see cref="GridItem"/> of this <see cref="GridItem"/>, if any.
    /// </summary>
    public abstract GridItem? Parent { get; }

    /// <summary>
    ///  If this item is a <see cref="GridItemType.Property"/> <see cref="GridItem"/>, this retrieves the
    ///  <see cref="System.ComponentModel.PropertyDescriptor"/> that is associated with this <see cref="GridItem"/>.
    ///  This can be used to retrieve information such as property type, name, or <see cref="TypeConverter"/>.
    /// </summary>
    public abstract PropertyDescriptor? PropertyDescriptor { get; }

    /// <summary>
    ///  Retrieves the current value of this grid Item. This may be null.
    /// </summary>
    /// <devdoc>
    ///  We don't do set because of the value class semantics, etc.
    /// </devdoc>
    public abstract object? Value { get; }

    /// <summary>
    ///  Retrieves whether the given property is expandable to show nested properties.
    /// </summary>
    public virtual bool Expandable => false;

    /// <summary>
    ///  Retrieves or sets whether the <see cref="GridItem"/> is in an expanded state.
    /// </summary>
    public virtual bool Expanded
    {
        get => false;
        set => throw new NotSupportedException(SR.GridItemNotExpandable);
    }

    /// <summary>
    ///  Attempts to select this <see cref="GridItem"/> in the <see cref="PropertyGrid"/>.
    /// </summary>
    public abstract bool Select();
}
