// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Design;

/// <summary>
///  Provides a base class for property tabs.
/// </summary>
/// <remarks>
///  <para>
///   The <see cref="PropertyTab"/> class provides the base class behavior for a property tab. Property tabs are
///   displayed on the toolbar of the <see cref="PropertyGrid"/> control of the Properties window, and allow a
///   component to display different views of its properties or other data.
///  </para>
///  <para>
///   User code will usually not create an instance of a <see cref="PropertyTab"/> directly. Instead, a
///   <see cref="PropertyTabAttribute"/> that indicates the type of the property tab or property tabs to display
///   for a component can be associated with the properties or types that the <see cref="PropertyTab"/> should be
///   displayed for.
///  </para>
///  <para>
///   The <see cref="PropertyGrid"/> will instantiate a <see cref="PropertyTab"/> of the type specified by a
///   <see cref="PropertyTabAttribute"/> associated with the type or property field of the component that is
///   being browsed.
///  </para>
/// </remarks>
public abstract class PropertyTab : IExtenderProvider
{
    private Bitmap? _bitmap;
    private bool _checkedBitmap;

    /// <summary>
    ///  Gets the bitmap that is displayed for the <see cref="PropertyTab"/>.
    /// </summary>
    public virtual Bitmap? Bitmap
    {
        get
        {
            if (!_checkedBitmap && _bitmap is null)
            {
                try
                {
                    _bitmap = ScaleHelper.GetIconResourceAsBestMatchBitmap(
                        GetType(),
                        GetType().Name,
                        ScaleHelper.SystemIconSize);
                }
                catch (Exception)
                {
                }

                _checkedBitmap = true;
            }

            return _bitmap;
        }
    }

    /// <summary>
    ///  Gets or sets the array of components the property tab is associated with.
    /// </summary>
    public virtual object[]? Components { get; set; }

    /// <summary>
    ///  Gets or sets the name for the property tab.
    /// </summary>
    public abstract string? TabName { get; }

    /// <summary>
    ///  Gets or sets the help keyword that is to be associated with this tab.
    /// </summary>
    /// <remarks>
    ///  <para>This defaults to the tab name.</para>
    /// </remarks>
    public virtual string? HelpKeyword => TabName;

    /// <summary>
    ///  Gets a value indicating whether this <see cref="PropertyTab"/> can display properties for the
    ///  specified <paramref name="extendee"/>.
    /// </summary>
    public virtual bool CanExtend(object? extendee) => true;

    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _bitmap?.Dispose();
            _bitmap = null;
        }
    }

    ~PropertyTab() => Dispose(disposing: false);

    /// <summary>
    ///  Gets the default property of the specified <paramref name="component"/>.
    /// </summary>
    public virtual PropertyDescriptor? GetDefaultProperty(object component)
        => TypeDescriptor.GetDefaultProperty(component);

    /// <summary>
    ///  Gets the properties of the specified <paramref name="component"/>.
    /// </summary>
    public virtual PropertyDescriptorCollection? GetProperties(object component)
        => GetProperties(component, attributes: null);

    /// <summary>
    ///  Gets the properties of the specified <paramref name="component"/> which match the specified
    ///  <paramref name="attributes"/>.
    /// </summary>
    public abstract PropertyDescriptorCollection? GetProperties(object component, Attribute[]? attributes);

    /// <summary>
    ///  Gets the properties of the specified <paramref name="component"/> that match the specified
    ///  <paramref name="attributes"/> and <paramref name="context"/>.
    /// </summary>
    public virtual PropertyDescriptorCollection? GetProperties(
        ITypeDescriptorContext? context,
        object component,
        Attribute[]? attributes) => GetProperties(component, attributes);
}
