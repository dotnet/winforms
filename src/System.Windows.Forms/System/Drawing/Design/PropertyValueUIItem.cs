// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Design;

/// <summary>
///  Provides information about the property value UI including the invoke handler, tool tip and glyph icon.
/// </summary>
public class PropertyValueUIItem
{
    /// <summary>Constructor that accepts the necessary information to display this item.</summary>
    /// <param name="uiItemImage"><see cref="Drawing.Image"/> representing the 8 x 8 icon to display.</param>
    /// <param name="handler">The <see cref="PropertyValueUIItemInvokeHandler"/> to invoke when the item is double clicked.</param>
    /// <param name="tooltip">The ToolTip to display for this item.</param>
    public PropertyValueUIItem(Image uiItemImage, PropertyValueUIItemInvokeHandler handler, string? tooltip)
    {
        Image = uiItemImage.OrThrowIfNull();
        InvokeHandler = handler.OrThrowIfNull();
        ToolTip = tooltip;
    }

    /// <summary>Gets the 8 x 8 pixel image that will be drawn on the properties window.</summary>
    public virtual Image Image { get; }

    /// <summary>Gets the handler that will be raised when this item is double clicked.</summary>
    public virtual PropertyValueUIItemInvokeHandler InvokeHandler { get; }

    /// <summary>Gets the ToolTip to display for this item.</summary>
    public virtual string? ToolTip { get; }

    /// <summary>Resets the UI item.</summary>
    public virtual void Reset() { }
}
