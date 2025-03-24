// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Ole;
using Windows.Win32.UI.Shell;

namespace System.Private.Windows.Ole;

internal interface IDragEvent
{
    /// <summary>
    ///  Gets the x-coordinate of the mouse pointer.
    /// </summary>
    int X { get; }

    /// <summary>
    ///  Gets the y-coordinate of the mouse pointer.
    /// </summary>
    int Y { get; }

    /// <summary>
    ///  Gets which drag-and-drop operations are allowed by the target of the drag event.
    /// </summary>
    DROPEFFECT Effect { get; }

    /// <summary>
    ///  Gets the drop description image type.
    /// </summary>
    DROPIMAGETYPE DropImageType { get; }

    /// <summary>
    ///  Data object, if any, associated with the drag event.
    /// </summary>
    IComVisibleDataObject? DataObject { get; }

    /// <summary>
    ///  Gets or sets the drop description text such as "Move to %1".
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   UI coloring is applied to the text in <see cref="MessageReplacementToken"/>
    ///   if used by specifying %1 in <see cref="Message"/>.
    ///  </para>
    /// </remarks>
    string? Message { get; set; }

    /// <summary>
    ///  Gets or sets the drop description text such as "Documents" when %1 is specified in <see cref="Message"/>.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   UI coloring is applied to the text in <see cref="MessageReplacementToken"/>
    ///   if used by specifying %1 in <see cref="Message"/>.
    ///  </para>
    /// </remarks>
    string? MessageReplacementToken { get; set; }
}
