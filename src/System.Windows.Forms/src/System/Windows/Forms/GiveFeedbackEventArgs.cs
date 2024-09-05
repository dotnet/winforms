// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  Provides data for the <see cref="Control.GiveFeedback"/> event.
/// </summary>
public class GiveFeedbackEventArgs : EventArgs
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="GiveFeedbackEventArgs"/> class.
    /// </summary>
    public GiveFeedbackEventArgs(DragDropEffects effect, bool useDefaultCursors)
        : this(effect, useDefaultCursors, dragImage: default!, cursorOffset: default, useDefaultDragImage: false)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="GiveFeedbackEventArgs"/> class.
    /// </summary>
    public GiveFeedbackEventArgs(DragDropEffects effect, bool useDefaultCursors, Bitmap? dragImage, Point cursorOffset, bool useDefaultDragImage)
    {
        Effect = effect;
        UseDefaultCursors = useDefaultCursors;
        DragImage = dragImage;
        CursorOffset = cursorOffset;
        UseDefaultDragImage = useDefaultDragImage;
    }

    /// <summary>
    ///  Gets the type of drag-and-drop operation.
    /// </summary>
    public DragDropEffects Effect { get; }

    /// <summary>
    ///  Gets or sets a value indicating whether a default pointer is used.
    /// </summary>
    public bool UseDefaultCursors { get; set; }

    /// <summary>
    ///  Gets or sets the drag image bitmap.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Note the outer edges of <see cref="DragImage"/> are blended out if the image width or height exceeds 300 pixels.
    ///  </para>
    /// </remarks>
    public Bitmap? DragImage { get; set; }

    /// <summary>
    ///  Gets or sets the drag image cursor offset.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Specifies the location of the cursor within <see cref="DragImage"/>, which is an offset from the upper-left corner.
    ///  </para>
    /// </remarks>
    public Point CursorOffset { get; set; }

    /// <summary>
    ///  Gets or sets a value indicating whether a layered window drag image is used.
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Specify <see langword="true"/> for <see cref="UseDefaultDragImage"/> to use a layered window drag image with a size of 96x96;
    ///  otherwise <see langword="false"/>.
    /// </para>
    /// </remarks>
    public bool UseDefaultDragImage { get; set; }

    internal GiveFeedbackEventArgs Clone()
    {
        return (GiveFeedbackEventArgs)MemberwiseClone();
    }

    internal bool Equals(GiveFeedbackEventArgs? giveFeedbackEventArgs)
    {
        if (giveFeedbackEventArgs == this)
        {
            return true;
        }

        return giveFeedbackEventArgs is not null
            && giveFeedbackEventArgs.Effect == Effect
            && giveFeedbackEventArgs.UseDefaultCursors == UseDefaultCursors
            && ((giveFeedbackEventArgs.DragImage is null && DragImage is null)
                || (giveFeedbackEventArgs.DragImage is not null && giveFeedbackEventArgs.DragImage.Equals(DragImage)))
            && giveFeedbackEventArgs.CursorOffset.Equals(CursorOffset)
            && giveFeedbackEventArgs.UseDefaultDragImage == UseDefaultDragImage;
    }
}
