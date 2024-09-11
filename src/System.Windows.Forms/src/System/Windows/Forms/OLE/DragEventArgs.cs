// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Provides data for the <see cref="Control.DragDrop"/>, <see cref="Control.DragEnter"/>,
///  or <see cref="Control.DragOver"/> event.
/// </summary>
public class DragEventArgs : EventArgs
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="DragEventArgs"/> class.
    /// </summary>
    public DragEventArgs(
        IDataObject? data,
        int keyState,
        int x,
        int y,
        DragDropEffects allowedEffect,
        DragDropEffects effect)
        : this(data, keyState, x, y, allowedEffect, effect, dropImageType: DropImageType.Invalid, message: string.Empty, messageReplacementToken: string.Empty)
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="DragEventArgs"/> class.
    /// </summary>
    public DragEventArgs(
        IDataObject? data,
        int keyState,
        int x,
        int y,
        DragDropEffects allowedEffect,
        DragDropEffects effect,
        DropImageType dropImageType,
        string? message,
        string? messageReplacementToken)
    {
        Data = data;
        KeyState = keyState;
        X = x;
        Y = y;
        AllowedEffect = allowedEffect;
        Effect = effect;
        DropImageType = dropImageType;
        Message = message;
        MessageReplacementToken = messageReplacementToken;
    }

    /// <summary>
    ///  The <see cref="IDataObject"/> that contains the data associated
    ///  with this event.
    /// </summary>
    public IDataObject? Data { get; }

    /// <summary>
    ///  Gets the current state of the SHIFT, CTRL, and ALT keys.
    /// </summary>
    public int KeyState { get; }

    /// <summary>
    ///  Gets the x-coordinate of the mouse pointer.
    /// </summary>
    public int X { get; }

    /// <summary>
    ///  Gets the y-coordinate of the mouse pointer.
    /// </summary>
    public int Y { get; }

    /// <summary>
    ///  Gets which drag-and-drop operations are allowed by the originator (or source)
    ///  of the drag event.
    /// </summary>
    public DragDropEffects AllowedEffect { get; }

    /// <summary>
    ///  Gets or sets which drag-and-drop operations are allowed by the target of the drag event.
    /// </summary>
    public DragDropEffects Effect { get; set; }

    /// <summary>
    ///  Gets or sets the drop description image type.
    /// </summary>
    public DropImageType DropImageType { get; set; }

    /// <summary>
    ///  Gets or sets the drop description text such as "Move to %1".
    /// </summary>
    /// <remarks>
    /// <para>
    ///  UI coloring is applied to the text in <see cref="MessageReplacementToken"/>
    ///  if used by specifying %1 in <see cref="Message"/>.
    /// </para>
    /// </remarks>
    public string? Message { get; set; }

    /// <summary>
    ///  Gets or sets the drop description text such as "Documents" when %1 is specified in <see cref="Message"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    ///  UI coloring is applied to the text in <see cref="MessageReplacementToken"/>
    ///  if used by specifying %1 in <see cref="Message"/>.
    /// </para>
    /// </remarks>
    public string? MessageReplacementToken { get; set; }

    internal DragEventArgs Clone()
    {
        return (DragEventArgs)MemberwiseClone();
    }

    internal bool Equals(DragEventArgs? dragEventArgs)
    {
        if (dragEventArgs == this)
        {
            return true;
        }

        return dragEventArgs is not null
            && dragEventArgs.Data is not null
            && dragEventArgs.Data.Equals(Data)
            && dragEventArgs.KeyState == KeyState
            && dragEventArgs.X == X
            && dragEventArgs.Y == Y
            && dragEventArgs.AllowedEffect == AllowedEffect
            && dragEventArgs.Effect == Effect
            && dragEventArgs.DropImageType == DropImageType
            && dragEventArgs.Message == Message
            && dragEventArgs.MessageReplacementToken == MessageReplacementToken;
    }
}
