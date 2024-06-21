namespace System.Windows.Forms;

/// <summary>
///  Represents the closing request reason in the context of the the <see cref="PopupCloseRequestEventArgs"/>.
/// </summary>
public enum PopupCloseRequestReason
{
    /// <summary>
    ///  The popup was closed because another part of the surface was clicked, which caused the popup to close.
    /// </summary>
    PopupLostFocus = 0,

    /// <summary>
    ///  The application was put in the background because another application was switched to.
    /// </summary>
    AppLostFocus = 1,

    /// <summary>
    ///  The popup's Close method was invoked.
    /// </summary>
    CloseMethodInvoked = 2,

    /// <summary>
    ///  Some content of the popup was clicked.
    /// </summary>
    ContentClicked = 3,

    /// <summary>
    ///  The popup was closed by a corresponding keyboard functionality.
    /// </summary>
    Keyboard = 4,
}
