using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Represents the event data for a popup closing event.
/// </summary>
public class PopupCloseRequestEventArgs : CancelEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PopupCloseRequestEventArgs"/> class.
    /// </summary>
    /// <param name="closeReason">The reason why the popup is closing.</param>
    public PopupCloseRequestEventArgs(PopupCloseRequestReason closeReason)
    {
        PopupClosingRequestReason = closeReason;
        KeyData = Keys.None;
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="PopupCloseRequestEventArgs"/> class.
    /// </summary>
    /// <param name="closingRequestReason">The reason why the popup is closing.</param>
    /// <param name="keyData">The key data associated with the popup closing event.</param>
    public PopupCloseRequestEventArgs(PopupCloseRequestReason closingRequestReason, Keys keyData)
    {
        PopupClosingRequestReason = closingRequestReason;
        KeyData = keyData;
    }

    /// <summary>
    ///  Gets or sets the reason why the popup is closing.
    /// </summary>
    /// <value>
    ///  The reason why the popup is closing represented by a <see cref="Forms.PopupCloseRequestReason"/> value.
    /// </value>
    public PopupCloseRequestReason PopupClosingRequestReason { get; set; }

    /// <summary>
    ///  Gets or sets the key data associated with the popup closing event.
    /// </summary>
    /// <value>
    ///  The key data associated with the popup closing event.
    /// </value>
    public Keys KeyData { get; set; }

    /// <summary>
    ///  Gets or sets the cause of the popup closing event.
    /// </summary>
    /// <value>
    ///  The request original of the popup closing event, which can be
    ///  <see cref="PopupCloseRequestOrigin.InternalByComponent"/> or <see cref="PopupCloseRequestOrigin.ExternalByUser"/>."/>
    /// </value>
    public PopupCloseRequestOrigin ClosingRequestOrigin { get; set; }
}
