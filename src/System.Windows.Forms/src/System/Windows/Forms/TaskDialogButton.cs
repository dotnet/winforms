// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///   Represents a button control of a task dialog.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   A button can either be a standard button (whose text is provided by the OS), or
    ///   a custom button (or command link) where you can provide your own text.
    /// </para>
    /// <para>
    ///   <see cref="TaskDialogButton"/> instances retrieved by static getters like <see cref="OK"/> are
    ///   standard buttons. Their <see cref="Text"/> property cannot be set as the OS will provide the
    ///   localized text for the buttons when showing them in the dialog.
    /// </para>
    /// <para>
    ///   Button instances created with one of the constructors are custom buttons, which allow you to provide
    ///   your own text as button label.
    /// </para>
    /// <para>
    ///   Note: It's not possible to show both custom buttons and command links (<see cref="TaskDialogCommandLinkButton"/> instances)
    ///   at the same time - it's only one or the other. In either case, you can combine them with
    ///   standard buttons.
    /// </para>
    /// </remarks>
    public class TaskDialogButton : TaskDialogControl
    {
        private readonly TaskDialogResult? _standardButtonResult;
        private bool _enabled = true;
        private bool _elevationRequired;
        private bool _visible = true;
        private string? _text;
        private int _customButtonID;

        /// <summary>
        ///   Occurs when the button is clicked.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   By default, the dialog will be closed after the event handler returns
        ///   (except for the <see cref="Help"/> button, which instead will raise the
        ///   <see cref="TaskDialogPage.HelpRequest"/> event afterwards).
        /// </para>
        /// <para>
        ///   To prevent the dialog from closing when this button is clicked, set the
        ///   <see cref="AllowCloseDialog"/> property to <see langword="false"/>.
        /// </para>
        /// </remarks>
        public event EventHandler? Click;

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogButton"/> class.
        /// </summary>
        // TODO: Find a way to avoid making the class inheritable
#pragma warning disable RS0022 // Constructor make noninheritable base class inheritable
        public TaskDialogButton()
#pragma warning restore RS0022 // Constructor make noninheritable base class inheritable
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogButton"/> class
        ///   using the given text and, optionally, a description text.
        /// </summary>
        /// <param name="text">The text of the control.</param>
        /// <param name="allowCloseDialog">A value that indicates whether the task dialog should close
        ///   when this button is clicked.
        /// </param>
        // TODO
#pragma warning disable RS0022 // Constructor make noninheritable base class inheritable
        public TaskDialogButton(string? text, bool enabled = true, bool allowCloseDialog = true)
#pragma warning restore RS0022 // Constructor make noninheritable base class inheritable
            : this()
        {
            _text = text;
            Enabled = enabled;
            AllowCloseDialog = allowCloseDialog;
        }

        internal TaskDialogButton(TaskDialogResult standardButtonResult)
        {
            _standardButtonResult = standardButtonResult;
            _text = standardButtonResult.ToString();
        }

        // Static factory properties that return a new instance of the button.

        /// <summary>
        ///  Gets a standard <see cref="TaskDialogButton"/> instance representing the <c>OK</c> button.
        /// </summary>
        public static TaskDialogButton OK => new TaskDialogButton(TaskDialogResult.OK);

        /// <summary>
        ///  Gets a standard <see cref="TaskDialogButton"/> instance representing the <c>Cancel</c> button.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   Note: Adding a Cancel button will automatically add a close button
        ///   to the task dialog's title bar and will allow to close the dialog by
        ///   pressing ESC or Alt+F4 (just as if you enabled
        ///   <see cref="TaskDialogPage.AllowCancel"/>).
        /// </para>
        /// </remarks>
        public static TaskDialogButton Cancel => new TaskDialogButton(TaskDialogResult.Cancel);

        /// <summary>
        ///  Gets a standard <see cref="TaskDialogButton"/> instance representing the <c>Abort</c> button.
        /// </summary>
        public static TaskDialogButton Abort => new TaskDialogButton(TaskDialogResult.Abort);

        /// <summary>
        ///  Gets a standard <see cref="TaskDialogButton"/> instance representing the <c>Retry</c> button.
        /// </summary>
        public static TaskDialogButton Retry => new TaskDialogButton(TaskDialogResult.Retry);

        /// <summary>
        ///  Gets a standard <see cref="TaskDialogButton"/> instance representing the <c>Ignore</c> button.
        /// </summary>
        public static TaskDialogButton Ignore => new TaskDialogButton(TaskDialogResult.Ignore);

        /// <summary>
        ///  Gets a standard <see cref="TaskDialogButton"/> instance representing the <c>Yes</c> button.
        /// </summary>
        public static TaskDialogButton Yes => new TaskDialogButton(TaskDialogResult.Yes);

        /// <summary>
        ///  Gets a standard <see cref="TaskDialogButton"/> instance representing the <c>No</c> button.
        /// </summary>
        public static TaskDialogButton No => new TaskDialogButton(TaskDialogResult.No);

        /// <summary>
        ///  Gets a standard <see cref="TaskDialogButton"/> instance representing the <c>Close</c> button.
        /// </summary>
        public static TaskDialogButton Close => new TaskDialogButton(TaskDialogResult.Close);

        /// <summary>
        ///  Gets a standard <see cref="TaskDialogButton"/> instance representing the <c>Help</c> button.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   Note: Clicking this button will not close the dialog, but will raise the
        ///   <see cref="TaskDialogPage.HelpRequest"/> event.
        /// </para>
        /// </remarks>
        public static TaskDialogButton Help => new TaskDialogButton(TaskDialogResult.Help);

        /// <summary>
        ///  Gets a standard <see cref="TaskDialogButton"/> instance representing the <c>Try Again</c> button.
        /// </summary>
        public static TaskDialogButton TryAgain => new TaskDialogButton(TaskDialogResult.TryAgain);

        /// <summary>
        ///  Gets a standard <see cref="TaskDialogButton"/> instance representing the <c>Continue</c> button.
        /// </summary>
        public static TaskDialogButton Continue => new TaskDialogButton(TaskDialogResult.Continue);

        /// <summary>
        ///   Gets or sets a value that indicates whether the task dialog should close
        ///   when this button is clicked. Or, if this button is the
        ///   <see cref="TaskDialogButton.Help"/> button, indicates whether the
        ///   <see cref="TaskDialogPage.HelpRequest"/> should be raised.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if the task dialog should close when
        ///   this button is clicked; otherwise, <see langword="false"/>. The default value is <see langword="true"/>.
        /// </value>
        public bool AllowCloseDialog { get; set; } = true;

        /// <summary>
        ///   Gets or sets a value indicating whether the button can respond to user interaction.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if the button can respond to user interaction; otherwise,
        ///   <see langword="false"/>. The default value is <see langword="true"/>.
        /// </value>
        /// <remarks>
        /// <para>
        ///   This property can be set while the dialog is shown.
        /// </para>
        /// </remarks>
        public bool Enabled
        {
            get => _enabled;
            set
            {
                DenyIfBoundAndNotCreated();

                // Check if we can update the button.
                if (CanUpdate())
                {
                    BoundPage!.BoundDialog!.SetButtonEnabled(ButtonID, value);
                }

                _enabled = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that indicates if the User Account Control (UAC) shield icon
        ///   should be shown near the button; that is, whether the action invoked by the button
        ///   requires elevation.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> to show the UAC shield icon; otherwise, <see langword="false"/>.
        ///   The default value is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// <para>
        ///   This property can be set while the dialog is shown.
        /// </para>
        /// </remarks>
        public bool ElevationRequired
        {
            get => _elevationRequired;
            set
            {
                DenyIfBoundAndNotCreated();

                if (CanUpdate())
                {
                    BoundPage!.BoundDialog!.SetButtonElevationRequiredState(ButtonID, value);
                }

                _elevationRequired = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that indicates if this
        ///   <see cref="TaskDialogButton"/> should be shown when displaying
        ///   the task dialog.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   Setting this property to <see langword="false"/> allows you to still receive the
        ///   <see cref="Click"/> event (e.g. for the
        ///   <see cref="Cancel"/> button when
        ///   <see cref="TaskDialogPage.AllowCancel"/> is set), or to call the
        ///   <see cref="PerformClick"/> method even if the button
        ///   is not shown.
        /// </para>
        /// </remarks>
        public bool Visible
        {
            get => _visible;
            set
            {
                DenyIfBound();

                _visible = value;
            }
        }

        /// <summary>
        ///   Gets or sets the text associated with this control.
        /// </summary>
        /// <value>
        ///   The text associated with this control. The default value is <see langword="null"/>.
        /// </value>
        /// <remarks>
        /// <para>
        ///   You cannot set this property if this button is a standard button, as its text will be provided by the OS.
        /// </para>
        /// <para>
        ///   This property must not be <see langword="null"/> or an empty string when showing or navigating
        ///   the dialog; otherwise, the operation will fail.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">This button is a standard button, for which the text is provided by the OS.</exception>
        /// <exception cref="InvalidOperationException">This control is currently bound to a task dialog.</exception>
        public string? Text
        {
            get => _text;
            set
            {
                // For standard buttons, the text is set by the constructor (but it's only
                // the enum value name, the actual text is provided by the OS).
                if (IsStandardButton)
                    throw new InvalidOperationException(SR.TaskDialogCannotSetTextForStandardButton);

                DenyIfBound();

                _text = value;
            }
        }

        internal override bool IsCreatable => base.IsCreatable && _visible;

        internal bool IsStandardButton => _standardButtonResult != null;

        internal TaskDialogResult StandardButtonResult => _standardButtonResult ?? throw new InvalidOperationException();

        internal int ButtonID => IsStandardButton ? (int)StandardButtonResult : _customButtonID;

        internal TaskDialogButtonCollection? Collection { get; set; }

        public static bool operator ==(TaskDialogButton? b1, TaskDialogButton? b2)
        {
            return Equals(b1, b2);
        }

        public static bool operator !=(TaskDialogButton? b1, TaskDialogButton? b2)
        {
            return !(b1 == b2);
        }

        private static ComCtl32.TDCBF GetStandardButtonFlagForResult(TaskDialogResult result) => result switch
        {
            TaskDialogResult.OK => ComCtl32.TDCBF.OK_BUTTON,
            TaskDialogResult.Cancel => ComCtl32.TDCBF.CANCEL_BUTTON,
            TaskDialogResult.Abort => ComCtl32.TDCBF.ABORT_BUTTON,
            TaskDialogResult.Retry => ComCtl32.TDCBF.RETRY_BUTTON,
            TaskDialogResult.Ignore => ComCtl32.TDCBF.IGNORE_BUTTON,
            TaskDialogResult.Yes => ComCtl32.TDCBF.YES_BUTTON,
            TaskDialogResult.No => ComCtl32.TDCBF.NO_BUTTON,
            TaskDialogResult.Close => ComCtl32.TDCBF.CLOSE_BUTTON,
            TaskDialogResult.Help => ComCtl32.TDCBF.HELP_BUTTON,
            TaskDialogResult.TryAgain => ComCtl32.TDCBF.TRYAGAIN_BUTTON,
            TaskDialogResult.Continue => ComCtl32.TDCBF.CONTINUE_BUTTON,
            _ => default
        };

        /// <summary>
        ///   Simulates a click on this button.
        /// </summary>
        public void PerformClick()
        {
            // Note: We allow a click even if the button is not visible/created.
            DenyIfNotBoundOrWaitingForInitialization();

            BoundPage!.BoundDialog!.ClickButton(ButtonID);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            // For standard buttons, we consider them to be equal if they have the same
            // dialog result. This will allow for checking the return value of
            // TaskDialog.ShowDialog with code like "if (result == TaskDialogButton.Yes)".
            if (IsStandardButton && obj is TaskDialogButton otherButton && otherButton.IsStandardButton)
                return _standardButtonResult!.Value == otherButton._standardButtonResult!.Value;

            // Otherwise, check for reference equality.
            return base.Equals(obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            if (IsStandardButton)
                return (int)_standardButtonResult!.Value;

            return base.GetHashCode();
        }

        /// <summary>
        ///   Returns a string that represents the current <see cref="TaskDialogButton"/> control.
        /// </summary>
        /// <returns>A string that contains the control text.</returns>
        public override string ToString() => _text ?? base.ToString() ?? string.Empty;

        internal ComCtl32.TDF Bind(TaskDialogPage page, int customButtonID)
        {
            if (_standardButtonResult != null)
                throw new InvalidOperationException();

            ComCtl32.TDF result = Bind(page);
            _customButtonID = customButtonID;

            return result;
        }

        internal bool HandleButtonClicked()
        {
            OnClick(EventArgs.Empty);

            return AllowCloseDialog;
        }

        internal virtual string? GetResultingText()
        {
            // Remove LFs from the text. Otherwise, the dialog would display the
            // part of the text after the LF in the command link note, but for
            // this we have the "DescriptionText" property, so we should ensure that
            // there is not an discrepancy here and that the contents of the "Text"
            // property are not displayed in the command link note.
            // Therefore, we replace a combined CR+LF with CR, and then also single
            // LFs with CR, because CR is treated as a line break.
            string? text = _text?.Replace("\r\n", "\r").Replace("\n", "\r");

            return text;
        }

        internal ComCtl32.TDCBF GetStandardButtonFlag()
        {
            if (!IsStandardButton)
                throw new InvalidOperationException();

            return GetStandardButtonFlagForResult(_standardButtonResult!.Value);
        }

        private protected override void ApplyInitializationCore()
        {
            // Re-set the properties so they will make the necessary calls.
            if (!_enabled)
            {
                Enabled = _enabled;
            }
            if (_elevationRequired)
            {
                ElevationRequired = _elevationRequired;
            }
        }

        private protected override void UnbindCore()
        {
            _customButtonID = 0;

            base.UnbindCore();
        }

        private protected void OnClick(EventArgs e) => Click?.Invoke(this, e);

        private bool CanUpdate()
        {
            // Only update the button when bound to a task dialog and we are not
            // waiting for the Navigated event. In the latter case we don't throw
            // an exception however, because ApplyInitialization() will be called
            // in the Navigated handler that does the necessary updates.
            return BoundPage?.WaitingForInitialization == false;
        }
    }
}
