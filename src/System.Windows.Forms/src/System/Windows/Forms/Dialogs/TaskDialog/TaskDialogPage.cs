// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using TASKDIALOGCONFIG_FooterIcon = Windows.Win32.UI.Controls.TASKDIALOGCONFIG._Anonymous2_e__Union;
using TASKDIALOGCONFIG_MainIcon = Windows.Win32.UI.Controls.TASKDIALOGCONFIG._Anonymous1_e__Union;

namespace System.Windows.Forms;

/// <summary>
///   Represents a page of content of a task dialog.
/// </summary>
/// <remarks>
/// <para>
///   It is possible to navigate a task dialog while it is shown by invoking the
///   <see cref="Navigate(TaskDialogPage)"/> method with a target <see cref="TaskDialogPage"/>
///   instance.
/// </para>
/// </remarks>
public class TaskDialogPage
{
    /// <summary>
    ///   The start ID for custom buttons.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   We need to ensure we don't use a ID that is already used for a
    ///   standard button (TaskDialogResult), so we start with 100 to be safe
    ///   (100 is also used as first ID in MSDN examples for the task dialog).
    /// </para>
    /// </remarks>
    private const int CustomButtonStartID = 100;

    /// <summary>
    ///   The start ID for radio buttons.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   This must be at least 1 because 0 already stands for "no button".
    /// </para>
    /// </remarks>
    private const int RadioButtonStartID = 1;

    private TaskDialogButtonCollection _buttons;
    private TaskDialogRadioButtonCollection _radioButtons;
    private TaskDialogVerificationCheckBox? _checkBox;
    private TaskDialogExpander? _expander;
    private TaskDialogFootnote? _footnote;
    private TaskDialogProgressBar? _progressBar;

    private TASKDIALOG_FLAGS _flags;
    private TaskDialogIcon? _icon;
    private string? _caption;
    private string? _heading;
    private string? _text;
    private bool _boundIconIsFromHandle;

    private TaskDialogButton[]? _boundCustomButtons;
    private Dictionary<int, TaskDialogButton>? _boundStandardButtonsByID;

    private bool _appliedInitialization;
    private bool _updateHeadingOnInitialization;
    private bool _updateTextOnInitialization;

    /// <summary>
    ///   Occurs after this instance is bound to a task dialog and the task dialog
    ///   has created the GUI elements represented by this <see cref="TaskDialogPage"/> instance.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   This will happen after showing or navigating the dialog.
    /// </para>
    /// <para>
    ///   When this event occurs, the <see cref="BoundDialog"/> property will return
    ///   the <see cref="TaskDialog"/> instance which this page is bound to.
    /// </para>
    /// </remarks>
    public event EventHandler? Created;

    /// <summary>
    ///   Occurs when the task dialog is about to destroy the GUI elements represented
    ///   by this <see cref="TaskDialogPage"/> instance and it is about to be
    ///   unbound from the task dialog.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   This will happen when closing or navigating the dialog.
    /// </para>
    /// <para>
    ///   After this event occurs, the <see cref="BoundDialog"/> property will return
    ///   <see langword="null"/>.
    /// </para>
    /// </remarks>
    public event EventHandler? Destroyed;

    /// <summary>
    ///   Occurs when the user presses F1 while the task dialog has focus, or when the
    ///   user clicks the <see cref="TaskDialogButton.Help"/> button.
    /// </summary>
    public event EventHandler? HelpRequest;

    /// <summary>
    ///   Initializes a new instance of the <see cref="TaskDialogPage"/> class.
    /// </summary>
    public TaskDialogPage()
    {
        _buttons = [];
        _radioButtons = [];

        // Create empty (hidden) controls.
        _checkBox = new TaskDialogVerificationCheckBox();
        _expander = new TaskDialogExpander();
        _footnote = new TaskDialogFootnote();
        _progressBar = new TaskDialogProgressBar(TaskDialogProgressBarState.None);
    }

    /// <summary>
    ///   Gets or sets the collection of push buttons
    ///   to be shown in this page.
    /// </summary>
    /// <value>
    ///   The collection of custom buttons to be shown in this page.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///   The property is set and this page instance is currently bound to a task dialog.
    /// </exception>
    public TaskDialogButtonCollection Buttons
    {
        get => _buttons;
        set
        {
            // We must deny this if we are bound because we need to be able to
            // access the controls from the task dialog's callback.
            DenyIfBound();

            _buttons = value.OrThrowIfNull();
        }
    }

    /// <summary>
    ///   Gets or sets the default button in the task dialog.
    /// </summary>
    /// <value>
    ///   The default button in the task dialog.
    /// </value>
    public TaskDialogButton? DefaultButton { get; set; }

    /// <summary>
    ///   Gets or sets the collection of radio buttons
    ///   to be shown in this page.
    /// </summary>
    /// <value>
    ///   The collection of radio buttons to be shown in this page.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///   The property is set and this page instance is currently bound to a task dialog.
    /// </exception>
    public TaskDialogRadioButtonCollection RadioButtons
    {
        get => _radioButtons;
        set
        {
            // We must deny this if we are bound because we need to be able to
            // access the controls from the task dialog's callback.
            DenyIfBound();

            _radioButtons = value.OrThrowIfNull();
        }
    }

    /// <summary>
    ///   Gets or sets the verification checkbox to be shown in this page.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   The verification checkbox will only be shown if its <see cref="TaskDialogVerificationCheckBox.Text"/>
    ///   property is not <see langword="null"/> or an empty string.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///   The property is set and this page instance is currently bound to a task dialog.
    /// </exception>
    public TaskDialogVerificationCheckBox? Verification
    {
        get => _checkBox;
        set
        {
            // We must deny this if we are bound because we need to be able to
            // access the control from the task dialog's callback.
            DenyIfBound();

            _checkBox = value;
        }
    }

    /// <summary>
    ///   Gets or sets the dialog expander to be shown in this page.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   The expander button (and the expanded area) will only be shown if its
    ///   <see cref="TaskDialogExpander.Text"/> property is not <see langword="null"/> or an empty string.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///   The property is set and this page instance is currently bound to a task dialog.
    /// </exception>
    public TaskDialogExpander? Expander
    {
        get => _expander;
        set
        {
            // We must deny this if we are bound because we need to be able to
            // access the control from the task dialog's callback.
            DenyIfBound();

            _expander = value;
        }
    }

    /// <summary>
    ///   Gets or sets the footnote to be shown in this page.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   The footnote will only be shown if its <see cref="TaskDialogFootnote.Text"/> property
    ///   is not <see langword="null"/> or an empty string.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///   The property is set and this page instance is currently bound to a task dialog.
    /// </exception>
    public TaskDialogFootnote? Footnote
    {
        get => _footnote;
        set
        {
            // We must deny this if we are bound because we need to be able to
            // access the control from the task dialog's callback.
            DenyIfBound();

            _footnote = value;
        }
    }

    /// <summary>
    ///   Gets or sets the progress bar to be shown in this page.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   The progress bar will only be shown if its <see cref="TaskDialogProgressBar.State"/>
    ///   property is not <see cref="TaskDialogProgressBarState.None"/>.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///   The property is set and this page instance is currently bound to a task dialog.
    /// </exception>
    public TaskDialogProgressBar? ProgressBar
    {
        get => _progressBar;
        set
        {
            // We must deny this if we are bound because we need to be able to
            // access the control from the task dialog's callback.
            DenyIfBound();

            _progressBar = value;
        }
    }

    /// <summary>
    ///   Gets or sets the text to display in the title bar of the task dialog.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   This property can be set while the dialog is shown.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///   The property is set and the task dialog has started navigating to this page instance,
    ///   but the <see cref="Created"/> event has not been raised yet.
    ///   - or -
    ///   The property is set on a page instance that is currently bound to a task dialog, but the dialog
    ///   has just started navigating to a different page.
    /// </exception>
    public string? Caption
    {
        get => _caption;
        set
        {
            DenyIfWaitingForInitialization();

            // Note: We set the field values after calling the method to ensure
            // it still has the previous value it the method throws.
            BoundDialog?.UpdateCaption(value);

            _caption = value;
        }
    }

    /// <summary>
    ///   Gets or sets the heading (main instruction).
    /// </summary>
    /// <remarks>
    /// <para>
    ///   This property can be set while the dialog is shown.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///   The property is set on a page instance that is currently bound to a task dialog, but the dialog
    ///   has just started navigating to a different page.
    /// </exception>
    public string? Heading
    {
        get => _heading;
        set
        {
            if (BoundDialog is not null)
            {
                // If we are bound but waiting for initialization (e.g. immediately after
                // starting a navigation), we buffer the change until we apply the
                // initialization (when navigation is finished).
                if (WaitingForInitialization)
                {
                    _updateHeadingOnInitialization = true;
                }
                else
                {
                    BoundDialog.UpdateTextElement(TASKDIALOG_ELEMENTS.TDE_MAIN_INSTRUCTION, value);
                }
            }

            _heading = value;
        }
    }

    /// <summary>
    ///   Gets or sets the dialog's primary text content.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   This property can be set while the dialog is shown.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///   The property is set on a page instance that is currently bound to a task dialog, but the dialog
    ///   has just started navigating to a different page.
    /// </exception>
    public string? Text
    {
        get => _text;
        set
        {
            if (BoundDialog is not null)
            {
                if (WaitingForInitialization)
                {
                    _updateTextOnInitialization = true;
                }
                else
                {
                    BoundDialog.UpdateTextElement(TASKDIALOG_ELEMENTS.TDE_CONTENT, value);
                }
            }

            _text = value;
        }
    }

    /// <summary>
    ///   Gets or sets the main icon.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   This property can be set while the dialog is shown, however, it
    ///   cannot be switched between instances created from an
    ///   <see cref="Drawing.Icon"/> (or from a handle pointer)
    ///   and standard icon instances.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///   The property is set and the task dialog has started navigating to this page instance,
    ///   but the <see cref="Created"/> event has not been raised yet.
    ///   - or -
    ///   The property is set on a page instance that is currently bound to a task dialog, but the dialog
    ///   has just started navigating to a different page.
    /// </exception>
    public unsafe TaskDialogIcon? Icon
    {
        get => _icon;
        set
        {
            // We currently don't support to buffer changes while waiting for the
            // initialization like it is done for string properties (Text, Heading),
            // because for handle icons, this would mean that the previous icon cannot
            // yet be disposed (until initialization is applied) even though the property
            // has been updated to a different icon.
            // It would be possible to initially specify a NULL icon in the TASKDIALOFCONFIG
            // and then update to the actual icon in ApplyInitialization() to avoid this, but
            // that seems like an overkill.
            DenyIfWaitingForInitialization();

            if (BoundDialog is not null)
            {
                (TASKDIALOGCONFIG_MainIcon icon, bool? iconIsFromHandle) =
                    GetMainIconValue(value);

                // The native task dialog icon cannot be updated from a handle
                // type to a non-handle type and vice versa, so we need to throw
                // throw in such a case.
                if (iconIsFromHandle is not null && iconIsFromHandle != _boundIconIsFromHandle)
                {
                    throw new InvalidOperationException(SR.TaskDialogCannotUpdateIconType);
                }

                BoundDialog.UpdateIconElement(
                     TASKDIALOG_ICON_ELEMENTS.TDIE_ICON_MAIN,
                    _boundIconIsFromHandle ? icon.hMainIcon : (IntPtr)(char*)icon.pszMainIcon);
            }

            _icon = value;
        }
    }

    /// <summary>
    ///   Gets or sets a value that indicates whether the task dialog can be closed with
    ///   <see cref="TaskDialogButton.Cancel"/> as resulting button by pressing ESC or Alt+F4
    ///   or by clicking the title bar's close button, even if a <see cref="TaskDialogButton.Cancel"/>
    ///   button isn't added to the <see cref="Buttons"/> collection.
    /// </summary>
    /// <value>
    ///   <see langword="true"/> to allow to close the dialog by pressing ESC or Alt+F4 or by clicking
    ///   the title bar's close button; otherwise, <see langword="false"/>. The default value is <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// <para>
    ///   You can intercept cancellation of the dialog without displaying a "Cancel"
    ///   button by adding the <see cref="TaskDialogButton.Cancel"/> button that has its
    ///   <see cref="TaskDialogButton.Visible"/> property set to <see langword="false"/>.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///   The property is set and this page instance is currently bound to a task dialog.
    /// </exception>
    public bool AllowCancel
    {
        get => GetFlag(TASKDIALOG_FLAGS.TDF_ALLOW_DIALOG_CANCELLATION);
        set => SetFlag(TASKDIALOG_FLAGS.TDF_ALLOW_DIALOG_CANCELLATION, value);
    }

    /// <summary>
    ///   Gets or sets a value that indicates whether text and controls are displayed
    ///   reading right to left.
    /// </summary>
    /// <value>
    ///   <see langword="true"/> to display text and controls reading right to left; <see langword="false"/>
    ///   to display controls reading left to right. The default value is <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// <para>
    ///   Note that once a task dialog has been opened with or has navigated to a
    ///   <see cref="TaskDialogPage"/> where this flag is set, it will keep on
    ///   subsequent navigations to a new <see cref="TaskDialogPage"/> even when
    ///   it doesn't have this flag set.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///   The property is set and this page instance is currently bound to a task dialog.
    /// </exception>
    public bool RightToLeftLayout
    {
        get => GetFlag(TASKDIALOG_FLAGS.TDF_RTL_LAYOUT);
        set => SetFlag(TASKDIALOG_FLAGS.TDF_RTL_LAYOUT, value);
    }

    /// <summary>
    ///   Gets or sets a value that indicates whether the task dialog can be minimized
    ///   when it is shown modeless.
    /// </summary>
    /// <value>
    ///   <see langword="true"/> to specify that the task dialog can be minimized; otherwise, <see langword="false"/>.
    ///   The default value is <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// <para>
    ///   When setting this property to <see langword="true"/>, <see cref="AllowCancel"/> is
    ///   automatically implied.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///   The property is set and this page instance is currently bound to a task dialog.
    /// </exception>
    public bool AllowMinimize
    {
        get => GetFlag(TASKDIALOG_FLAGS.TDF_CAN_BE_MINIMIZED);
        set => SetFlag(TASKDIALOG_FLAGS.TDF_CAN_BE_MINIMIZED, value);
    }

    /// <summary>
    ///   Indicates that the width of the task dialog is determined by the width
    ///   of its content area (similar to Message Box sizing behavior).
    /// </summary>
    /// <value>
    ///   <see langword="true"/> to determine the width of the task dialog by the width of
    ///   its content area; otherwise, <see langword="false"/>. The default value is <see langword="false"/>.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///   The property is set and this page instance is currently bound to a task dialog.
    /// </exception>
    public bool SizeToContent
    {
        get => GetFlag(TASKDIALOG_FLAGS.TDF_SIZE_TO_CONTENT);
        set => SetFlag(TASKDIALOG_FLAGS.TDF_SIZE_TO_CONTENT, value);
    }

    /// <summary>
    /// <para>
    ///   Gets or sets a value that specifies whether the task dialog should
    ///   interpret strings in the form <c>&lt;a href="target"&gt;link Text&lt;/a&gt;</c>
    ///   as hyperlink when specified in the <see cref="Text"/>,
    ///   <see cref="TaskDialogExpander.Text"/>,
    ///   or <see cref="TaskDialogFootnote.Text"/> properties.
    ///   When the user clicks on such a link, the <see cref="LinkClicked"/>
    ///   event is raised, containing the value of the <c>target</c> attribute.
    /// </para>
    /// </summary>
    /// <value>
    ///   <see langword="true"/> to enable links; otherwise, <see langword="false"/>.
    ///   The default value is <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// <para>
    ///   The Task Dialog will not actually execute any links.
    ///   Link execution must be handled in the <see cref="LinkClicked"/> event.
    /// </para>
    /// <para>
    ///   Note: Enabling this setting causes the <c>"&amp;"</c> character to be
    ///   interpreted as a prefix for an access key character (mnemonic) if at least
    ///   one link is used. To show a literal <c>"&amp;"</c> character, it must be escaped
    ///   as <c>"&amp;&amp;"</c>.
    /// </para>
    /// <para>
    ///   When you enable this setting and you want to display text
    ///   without interpreting links, you must replace the strings <c>"&lt;a"</c>
    ///   and <c>"&lt;A"</c> with something like <c>"&lt;\u200Ba"</c>.
    /// </para>
    /// </remarks>
    public bool EnableLinks { get; set; }

    /// <summary>
    ///   Occurs when the user has clicked on a link.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   This event will only be raised if <see cref="EnableLinks"/> is set to <see langword="true"/>.
    /// </para>
    /// </remarks>
    public event EventHandler<TaskDialogLinkClickedEventArgs>? LinkClicked;

    /// <summary>
    ///   Gets the <see cref="TaskDialog"/> instance which this page
    ///   is currently bound to.
    /// </summary>
    /// <value>
    ///   The <see cref="TaskDialog"/> instance which this page is bound to, or <see langword="null"/>
    ///   if this page is not currently bound.
    /// </value>
    /// <remarks>
    /// <para>
    ///   A page will be bound while it is being displayed, which is indicated by the events
    ///   <see cref="Created"/> and <see cref="Destroyed"/>.
    /// </para>
    /// <para>
    ///   While a page is bound to a task dialog, you cannot show that page instance using a
    ///   different <see cref="TaskDialog"/> instance at the same time.
    /// </para>
    /// </remarks>
    public TaskDialog? BoundDialog { get; private set; }

    /// <summary>
    ///   Gets a value that indicates if the <see cref="BoundDialog"/>
    ///   started navigation to this page but navigation did not yet complete
    ///   (in which case we cannot modify the dialog even though we are bound).
    /// </summary>
    internal bool WaitingForInitialization => BoundDialog is not null && !_appliedInitialization;

    /// <summary>
    ///  Shows the new content in the current task dialog.
    /// <paramref name="page"/>.
    /// </summary>
    /// <param name="page">
    ///   The page instance that contains the contents which this task dialog will display.
    /// </param>
    /// <remarks>
    /// <para>
    ///   During the navigation the dialog will recreate the dialog from the specified
    ///   <paramref name="page"/> and its controls, and unbind and destroy the currently shown page.
    /// </para>
    /// <para>
    ///   You can't manipulate the page or its controls
    ///   immediately after navigating the dialog (except for calling
    ///   <see cref="TaskDialog.Close"/> or navigating the dialog again).
    ///   You will need to wait for the <see cref="Created"/>
    ///   event to occur before you can manipulate the page or its controls.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///   <paramref name="page"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///   The page instance is not currently bound to a dialog, <see cref="BoundDialog"/> is <see langword="null"/>.
    ///   - or -
    ///   This page instance contains an invalid configuration.
    ///   - or -
    ///   This method is called from within the <see cref="TaskDialogRadioButton.CheckedChanged"/> event
    ///   of one of the radio buttons of the current task dialog.
    ///   - or -
    ///   The task dialog has already been closed.
    /// </exception>
    public void Navigate(TaskDialogPage page)
    {
        ArgumentNullException.ThrowIfNull(page);

        if (BoundDialog is null)
        {
            throw new InvalidOperationException(SR.TaskDialogCannotNavigateWithoutDialog);
        }

        BoundDialog.Navigate(page);
    }

    internal static bool IsNativeStringNullOrEmpty([NotNullWhen(false)] string? str)
    {
        // From a native point of view, the string is empty if its first
        // character is a NUL char.
        return string.IsNullOrEmpty(str) || str[0] == '\0';
    }

    internal static unsafe (TASKDIALOGCONFIG_MainIcon iconUnion, bool? iconIsFromHandle)
        GetMainIconValue(TaskDialogIcon? icon)
    {
        TASKDIALOGCONFIG_MainIcon iconUnion = default;
        bool? iconIsFromHandle = null;

        // If no icon is specified (icon is null), the iconIsFromHandle variable will be
        // null, which allows to clear the dialog's icon while it is shown, regardless
        // of whether the bound icon is a handle icon or a non-handle icon.
        if (icon?.IsHandleIcon == true)
        {
            iconUnion.hMainIcon = (HICON)icon.IconHandle;
            iconIsFromHandle = true;
        }
        else if (icon?.IsStandardIcon == true)
        {
            // Convert the value to an ushort before converting it to a pointer,
            // which corresponds to using the MAKEINTRESOURCEW macro in native code.
            iconUnion.pszMainIcon = (char*)checked((ushort)icon.StandardIcon);
            iconIsFromHandle = false;
        }

        return (iconUnion, iconIsFromHandle);
    }

    internal static unsafe (TASKDIALOGCONFIG_FooterIcon iconUnion, bool? iconIsFromHandle)
        GetFooterIconValue(TaskDialogIcon? icon)
    {
        TASKDIALOGCONFIG_FooterIcon iconUnion = default;
        bool? iconIsFromHandle = null;

        // If no icon is specified (icon is null), the iconIsFromHandle variable will be
        // null, which allows to clear the dialog's icon while it is shown, regardless
        // of whether the bound icon is a handle icon or a non-handle icon.
        if (icon?.IsHandleIcon == true)
        {
            iconUnion.hFooterIcon = (HICON)icon.IconHandle;
            iconIsFromHandle = true;
        }
        else if (icon?.IsStandardIcon == true)
        {
            // Convert the value to an ushort before converting it to a pointer,
            // which corresponds to using the MAKEINTRESOURCEW macro in native code.
            iconUnion.pszFooterIcon = (char*)checked((ushort)icon.StandardIcon);
            iconIsFromHandle = false;
        }

        return (iconUnion, iconIsFromHandle);
    }

    internal void DenyIfBound()
    {
        if (BoundDialog is not null)
        {
            throw new InvalidOperationException(SR.TaskDialogCannotSetPropertyOfBoundPage);
        }
    }

    internal void DenyIfWaitingForInitialization()
    {
        if (WaitingForInitialization)
        {
            throw new InvalidOperationException(string.Format(
                SR.TaskDialogNavigationNotCompleted,
                $"{nameof(TaskDialogPage)}.{nameof(Created)}"));
        }
    }

    internal TaskDialogButton? GetBoundButtonByID(int buttonID)
    {
        if (BoundDialog is null)
        {
            throw new InvalidOperationException();
        }

        if (buttonID == 0)
        {
            return null;
        }

        // Check if the button is part of the custom buttons.
        if (buttonID >= CustomButtonStartID)
        {
            return _boundCustomButtons![buttonID - CustomButtonStartID];
        }
        else
        {
            // Note: We deliberately return null instead of throwing when
            // the common button ID is not part of the collection, because
            // the caller might not know if such a button exists.
            _boundStandardButtonsByID!.TryGetValue(buttonID, out TaskDialogButton? button);
            return button;
        }
    }

    internal TaskDialogRadioButton? GetBoundRadioButtonByID(int buttonID)
    {
        if (BoundDialog is null)
        {
            throw new InvalidOperationException();
        }

        return buttonID == 0 ? null : _radioButtons[buttonID - RadioButtonStartID];
    }

    internal void Validate()
    {
        // Check that this page instance is not already bound to a TaskDialog instance.
        if (BoundDialog is not null)
        {
            throw new InvalidOperationException(string.Format(
                SR.TaskDialogPageIsAlreadyBound,
                nameof(TaskDialogPage),
                nameof(TaskDialog)));
        }

        // We also need to check the controls (and collections) since they could also be
        // bound to a TaskDialogPage at the same time.
        if (_buttons.BoundPage is not null ||
            _radioButtons.BoundPage is not null)
        {
            throw new InvalidOperationException(string.Format(
                SR.TaskDialogCollectionAlreadyBound,
                nameof(TaskDialogPage),
                nameof(TaskDialog)));
        }

        if (_buttons.Concat<TaskDialogControl>(_radioButtons)
            .Append(_checkBox)
            .Append(_expander)
            .Append(_footnote)
            .Append(_progressBar)
            .Any(c => c?.BoundPage is not null))
        {
            throw new InvalidOperationException(string.Format(
                SR.TaskDialogControlAlreadyBound,
                nameof(TaskDialogPage),
                nameof(TaskDialog)));
        }

        // Note: Theoretically we would need to exclude the standard buttons from the
        // "_buttons" collection when retrieving the Count (because they already have
        // a fixed button ID), but since not excluding them won't allow more buttons
        // than possible, we don't need to do it.
        if (_buttons.Count > int.MaxValue - CustomButtonStartID + 1 ||
            _radioButtons.Count > int.MaxValue - RadioButtonStartID + 1)
        {
            throw new InvalidOperationException(SR.TaskDialogTooManyButtonsAdded);
        }

        if (_radioButtons.Count(e => e.Checked) > 1)
        {
            throw new InvalidOperationException(SR.TaskDialogOnlySingleRadioButtonCanBeChecked);
        }

        // Check that we don't have custom buttons and command links at the same time.
        bool foundCustomButton = false;
        bool foundCommandLink = false;
        foreach (TaskDialogButton button in _buttons)
        {
            if (button is TaskDialogCommandLinkButton commandLink)
            {
                foundCommandLink = true;
            }
            else if (!button.IsStandardButton)
            {
                foundCustomButton = true;
            }
        }

        if (foundCustomButton && foundCommandLink)
        {
            throw new InvalidOperationException(SR.TaskDialogCannotShowCustomButtonsAndCommandLinks);
        }

        // For custom and radio buttons, we need to ensure the strings are not null or empty
        // (except for invisible buttons), as otherwise an error would occur when
        // showing/navigating the dialog.
        if (_buttons.Any(e => e.IsCreatable && !e.IsStandardButton && IsNativeStringNullOrEmpty(e.Text)))
        {
            throw new InvalidOperationException(SR.TaskDialogButtonTextMustNotBeNull);
        }

        if (_radioButtons.Any(e => e.IsCreatable && IsNativeStringNullOrEmpty(e.Text)))
        {
            throw new InvalidOperationException(SR.TaskDialogRadioButtonTextMustNotBeNull);
        }

        // Ensure the default button is part of the button collection.
        if (DefaultButton is not null && !_buttons.Contains(DefaultButton))
        {
            throw new InvalidOperationException(SR.TaskDialogDefaultButtonMustExistInCollection);
        }
    }

    internal void Bind(
        TaskDialog owner,
        out TASKDIALOG_FLAGS flags,
        out TASKDIALOG_COMMON_BUTTON_FLAGS buttonFlags,
        out IEnumerable<(int buttonID, string text)> customButtonElements,
        out IEnumerable<(int buttonID, string text)> radioButtonElements,
        out TASKDIALOGCONFIG_MainIcon mainIcon,
        out TASKDIALOGCONFIG_FooterIcon footnoteIcon,
        out int defaultButtonID,
        out int defaultRadioButtonID)
    {
        if (BoundDialog is not null)
        {
            throw new InvalidOperationException();
        }

        // This method assumes Validate() has already been called.

        BoundDialog = owner;
        flags = _flags;

        _updateTextOnInitialization = false;
        _updateHeadingOnInitialization = false;

        (TASKDIALOGCONFIG_MainIcon localIconValue, bool? iconIsFromHandle) = GetMainIconValue(_icon);
        (mainIcon, _boundIconIsFromHandle) = (localIconValue, iconIsFromHandle ?? false);

        if (_boundIconIsFromHandle)
        {
            flags |= TASKDIALOG_FLAGS.TDF_USE_HICON_MAIN;
        }

        TaskDialogButtonCollection buttons = _buttons;
        TaskDialogRadioButtonCollection radioButtons = _radioButtons;

        buttons.BoundPage = this;
        radioButtons.BoundPage = this;

        // Sort the buttons.
        _boundCustomButtons = [..buttons.Where(e => !e.IsStandardButton)];
        _boundStandardButtonsByID = buttons
            .Where(e => e.IsStandardButton)
            .ToDictionary(e => e.ButtonID);

        // Assign IDs to the buttons based on their index.
        defaultButtonID = 0;
        buttonFlags = default;
        foreach (TaskDialogButton standardButton in _boundStandardButtonsByID.Values)
        {
            flags |= standardButton.Bind(this);

            if (standardButton.IsCreated)
            {
                buttonFlags |= standardButton.GetStandardButtonFlag();
            }
        }

        for (int i = 0; i < _boundCustomButtons.Length; i++)
        {
            TaskDialogButton customButton = _boundCustomButtons[i];
            flags |= customButton.Bind(this, CustomButtonStartID + i);
        }

        if (DefaultButton is not null)
        {
            // Retrieve the button from the collection, to handle the case for standard buttons
            // when the user set an equal (but not same) instance as default button.
            TaskDialogButton defaultButton = buttons[buttons.IndexOf(DefaultButton)];
            if (defaultButton.IsCreated)
            {
                defaultButtonID = defaultButton.ButtonID;
            }
        }

        defaultRadioButtonID = 0;
        for (int i = 0; i < radioButtons.Count; i++)
        {
            TaskDialogRadioButton radioButton = radioButtons[i];
            flags |= radioButton.Bind(this, RadioButtonStartID + i);

            // Validate() will already have verified that no more than one radio button is checked.
            if (radioButton.IsCreated && radioButton.Checked)
            {
                defaultRadioButtonID = radioButton.RadioButtonID;
            }
        }

        if (defaultRadioButtonID == 0)
        {
            flags |= TASKDIALOG_FLAGS.TDF_NO_DEFAULT_RADIO_BUTTON;
        }

        customButtonElements = _boundCustomButtons.Where(e => e.IsCreated).Select(e => (e.ButtonID, e.GetResultingText()!));
        radioButtonElements = radioButtons.Where(e => e.IsCreated).Select(e => (e.RadioButtonID, e.Text!));

        // If we have command links, specify the TDF_USE_COMMAND_LINKS flag.
        // Note: The USE_COMMAND_LINKS_NO_ICON is currently not used.
        if (_boundCustomButtons.Any(e => e.IsCreated && e is TaskDialogCommandLinkButton))
            flags |= TASKDIALOG_FLAGS.TDF_USE_COMMAND_LINKS;

        if (EnableLinks)
        {
            flags |= TASKDIALOG_FLAGS.TDF_ENABLE_HYPERLINKS;
        }

        if (_checkBox is not null)
        {
            flags |= _checkBox.Bind(this);
        }

        if (_expander is not null)
        {
            flags |= _expander.Bind(this);
        }

        if (_footnote is not null)
        {
            flags |= _footnote.Bind(this, out footnoteIcon);
        }
        else
        {
            footnoteIcon = default;
        }

        if (_progressBar is not null)
        {
            flags |= _progressBar.Bind(this);
        }
    }

    internal void Unbind()
    {
        if (BoundDialog is null)
        {
            throw new InvalidOperationException();
        }

        _boundCustomButtons = null;
        _boundStandardButtonsByID = null;

        TaskDialogButtonCollection buttons = _buttons;
        TaskDialogRadioButtonCollection radioButtons = _radioButtons;

        foreach (TaskDialogButton button in buttons)
        {
            button.Unbind();
        }

        foreach (TaskDialogRadioButton radioButton in radioButtons)
        {
            radioButton.Unbind();
        }

        buttons.BoundPage = null;
        radioButtons.BoundPage = null;

        _checkBox?.Unbind();
        _expander?.Unbind();
        _footnote?.Unbind();
        _progressBar?.Unbind();

        BoundDialog = null;
        _appliedInitialization = false;
    }

    internal void ApplyInitialization()
    {
        if (_appliedInitialization)
        {
            throw new InvalidOperationException();
        }

        _appliedInitialization = true;

        // Check if we need to update some of our elements (if they have been modified
        // after starting navigation, but before navigation was finished).
        if (_updateHeadingOnInitialization)
        {
            Heading = _heading;
            _updateHeadingOnInitialization = false;
        }

        if (_updateTextOnInitialization)
        {
            Text = _text;
            _updateTextOnInitialization = false;
        }

        TaskDialogButtonCollection buttons = _buttons;
        TaskDialogRadioButtonCollection radioButtons = _radioButtons;

        foreach (TaskDialogButton button in buttons)
        {
            button.ApplyInitialization();
        }

        foreach (TaskDialogRadioButton button in radioButtons)
        {
            button.ApplyInitialization();
        }

        _checkBox?.ApplyInitialization();
        _expander?.ApplyInitialization();
        _footnote?.ApplyInitialization();
        _progressBar?.ApplyInitialization();
    }

    /// <summary>
    ///   Raises the <see cref="Created"/> event.
    /// </summary>
    /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
    protected internal void OnCreated(EventArgs e) => Created?.Invoke(this, e);

    /// <summary>
    ///   Raises the <see cref="Destroyed"/> event.
    /// </summary>
    /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
    protected internal void OnDestroyed(EventArgs e) => Destroyed?.Invoke(this, e);

    /// <summary>
    ///   Raises the <see cref="HelpRequest"/> event.
    /// </summary>
    /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
    protected internal void OnHelpRequest(EventArgs e) => HelpRequest?.Invoke(this, e);

    /// <summary>
    ///   Raises the <see cref="LinkClicked"/> event.
    /// </summary>
    /// <param name="e">A <see cref="TaskDialogLinkClickedEventArgs"/> that contains the event data.</param>
    protected internal void OnLinkClicked(TaskDialogLinkClickedEventArgs e) => LinkClicked?.Invoke(this, e);

    private bool GetFlag(TASKDIALOG_FLAGS flag) => (_flags & flag) == flag;

    private void SetFlag(TASKDIALOG_FLAGS flag, bool value)
    {
        DenyIfBound();

        if (value)
        {
            _flags |= flag;
        }
        else
        {
            _flags &= ~flag;
        }
    }
}
