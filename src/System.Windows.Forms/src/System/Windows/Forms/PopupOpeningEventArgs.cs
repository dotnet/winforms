using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  Represents the event data for a popup opening event.
/// </summary>
/// <seealso cref="CancelEventArgs" />
public class PopupOpeningEventArgs : CancelEventArgs
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="PopupOpeningEventArgs"/> class.
    /// </summary>
    /// <param name="cancel">true if the event should be canceled; otherwise, false.</param>
    /// <param name="preferredNewSize">The preferred new size of the popup.</param>
    public PopupOpeningEventArgs(bool cancel, Size preferredNewSize)
    {
        base.Cancel = cancel;
        PreferredNewSize = preferredNewSize;
    }

    /// <summary>
    ///  Gets or sets the preferred new size of the popup.
    /// </summary>
    /// <value>
    /// The preferred new size of the popup.
    /// </value>
    public Size PreferredNewSize { get; set; }

    /// <summary>
    ///  Gets or sets a value indicating whether the popup is should be prevented from resizing.
    /// </summary>
    /// <value>
    ///   <see langword="true"/> if the popup is should not be resized; otherwise, <see langword="false"/>.
    /// </value>
    public bool PreventResizing { get; set; }
}
