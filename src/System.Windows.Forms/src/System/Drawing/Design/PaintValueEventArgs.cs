// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Drawing.Design;

/// <summary>
///  Object passed as an argument to <see cref="UITypeEditor.PaintValue(PaintValueEventArgs)"/> containing
///  information needed by the editor to paint the given value.
/// </summary>
public class PaintValueEventArgs : EventArgs
{
    /// <summary>
    ///  Constructor that accepts the information needed by the editor to paint the given value.
    /// </summary>
    /// <param name="context">The <see cref="ITypeDescriptorContext"/> in which this value appears in.</param>
    /// <param name="value">The value to paint.</param>
    /// <param name="graphics">The <see cref="Drawing.Graphics"/> object with which drawing should be done.</param>
    /// <param name="bounds">The <see cref="Rectangle"/> that indicates the area in which the drawing should be done.</param>
    public PaintValueEventArgs(ITypeDescriptorContext? context, object? value, Graphics graphics, Rectangle bounds)
    {
        Context = context;
        Value = value;
        Graphics = graphics.OrThrowIfNull();
        Bounds = bounds;
    }

    /// <summary>
    /// <see cref="Rectangle"/> outlining the area in which the painting should be done.
    /// </summary>
    public Rectangle Bounds { get; }

    /// <summary>
    /// <see cref="ITypeDescriptorContext"/> object for additional information about the context this value appears in.
    /// </summary>
    public ITypeDescriptorContext? Context { get; }

    /// <summary>
    /// <see cref="Drawing.Graphics"/> object with which painting should be done.
    /// </summary>
    public Graphics Graphics { get; }

    /// <summary>
    ///  Value to paint.
    /// </summary>
    public object? Value { get; }
}
