// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///   Represents a radio button control of a task dialog.
    /// </summary>
    public sealed class TaskDialogRadioButton : TaskDialogControl
    {
        private string? _text;
        private int _radioButtonID;
        private bool _enabled = true;
        private bool _checked;
        private TaskDialogRadioButtonCollection? _collection;
        private bool _ignoreRadioButtonClickedNotification;

        /// <summary>
        ///   Occurs when the value of the <see cref="Checked"/> property changes
        ///   while this control is shown in a task dialog.
        /// </summary>
        public event EventHandler? CheckedChanged;

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogRadioButton"/> class.
        /// </summary>
        public TaskDialogRadioButton()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogRadioButton"/> class
        ///   using the given <paramref name="text"/>.
        /// </summary>
        public TaskDialogRadioButton(string? text)
            : this()
        {
            _text = text;
        }

        /// <summary>
        ///   Gets or sets a value indicating whether the button can respond to user interaction.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the button can respond to user interaction; otherwise,
        /// <see langword="false"/>. The default value is <see langword="true"/>.
        /// </value>
        /// <remarks>
        /// <para>
        ///   This property can be set while the dialog is shown.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">This control is currently bound to a task dialog
        /// but it has not been created.</exception>
        public bool Enabled
        {
            get => _enabled;
            set
            {
                DenyIfBoundAndNotCreated();

                // Check if we can update the button.
                if (CanUpdate())
                {
                    BoundPage!.BoundDialog!.SetRadioButtonEnabled(_radioButtonID, value);
                }

                _enabled = value;
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
        ///   This property must not be <see langword="null"/> or an empty string when showing or navigating
        ///   the dialog; otherwise the operation will fail.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">This control is currently bound to a task dialog.</exception>
        public string? Text
        {
            get => _text;
            set
            {
                DenyIfBound();

                _text = value;
            }
        }

        /// <summary>
        ///   Gets or set a value indicating whether the <see cref="TaskDialogRadioButton"/> is
        ///   in the checked state.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if the <see cref="TaskDialogRadioButton"/> is in the checked state;
        ///   otherwise, <see langword="false"/>. The default value is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// <para>
        ///   While the dialog is shown, this property can only be set to <see langword="true"/> and you cannot
        ///   set it from within the <see cref="CheckedChanged"/> event.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException"></exception>
        public bool Checked
        {
            get => _checked;
            set
            {
                DenyIfBoundAndNotCreated();
                DenyIfWaitingForInitialization();

                if (BoundPage == null)
                {
                    _checked = value;

                    // If we are part of a collection, set the checked value of all
                    // all other buttons to False.
                    // Note that this does not handle buttons that are added later
                    // to the collection.
                    if (_collection != null && value)
                    {
                        foreach (TaskDialogRadioButton radioButton in _collection)
                        {
                            radioButton._checked = radioButton == this;
                        }
                    }

                    return;
                }

                // Unchecking a radio button is not possible in the task dialog.
                // TODO: Should we throw only if the new value is different than the
                // old one?
                if (!value)
                {
                    throw new InvalidOperationException(SR.TaskDialogCannotUncheckRadioButtonWhileBound);
                }

                // Note: We do not allow to set the "Checked" property of any
                // radio button of the current task dialog while we are within
                // the TDN_RADIO_BUTTON_CLICKED notification handler. This is
                // because the logic of the task dialog is such that the radio
                // button will be selected AFTER the callback returns (not
                // before it is called), at least when the event is caused by
                // code sending the TDM_CLICK_RADIO_BUTTON message. This is
                // mentioned in the documentation for TDM_CLICK_RADIO_BUTTON:
                // "The specified radio button ID is sent to the
                // TaskDialogCallbackProc callback function as part of a
                // TDN_RADIO_BUTTON_CLICKED notification code. After the
                // callback function returns, the radio button will be
                // selected."
                //
                // While we handle this by ignoring the
                // TDN_RADIO_BUTTON_CLICKED notification when it is caused by
                // sending a TDM_CLICK_RADIO_BUTTON message, and then raising
                // the events after the notification handler returned, this
                // still seems to cause problems for TDN_RADIO_BUTTON_CLICKED
                // notifications initially caused by the user clicking the radio
                // button in the UI.
                //
                // For example, consider a scenario with two radio buttons
                // [ID 1 and 2], and the user added an event handler to
                // automatically select the first radio button (ID 1) when the
                // second one (ID 2) is selected in the UI.
                // This means the stack will then look as follows:
                // Show() ->
                // Callback: TDN_RADIO_BUTTON_CLICKED [ID 2] ->
                // SendMessage: TDM_CLICK_RADIO_BUTTON [ID 1] ->
                // Callback: TDN_RADIO_BUTTON_CLICKED [ID 1]
                //
                // However, when the initial TDN_RADIO_BUTTON_CLICKED handler
                // (ID 2) returns, the task dialog again calls the handler for
                // ID 1 (which wouldn't be a problem), and then again calls it
                // for ID 2, which is unexpected (and it doesn't seem that we
                // can prevent this by returning S_FALSE in the notification
                // handler). Additionally, after that it even seems we get an
                // endless loop of TDN_RADIO_BUTTON_CLICKED notifications even
                // when we don't send any further messages to the dialog.
                // See documentation/repro in
                // /Documentation/src/System/Windows/Forms/TaskDialog/Issue_RadioButton_InfiniteLoop.md
                //
                // See also:
                // /Documentation/src/System/Windows/Forms/TaskDialog/Issue_RadioButton_WeirdBehavior.md
                if (BoundPage.BoundDialog!.RadioButtonClickedStackCount > 0)
                {
                    throw new InvalidOperationException(string.Format(
                        SR.TaskDialogCannotSetRadioButtonCheckedWithinCheckedChangedEvent,
                        $"{nameof(TaskDialogRadioButton)}.{nameof(Checked)}",
                        $"{nameof(TaskDialogRadioButton)}.{nameof(CheckedChanged)}"));
                }

                // Click the radio button which will (recursively) raise the
                // TDN_RADIO_BUTTON_CLICKED notification. However, we ignore
                // the notification and then raise the events afterwards - see
                // above.
                _ignoreRadioButtonClickedNotification = true;
                try
                {
                    BoundPage.BoundDialog.ClickRadioButton(_radioButtonID);
                }
                finally
                {
                    _ignoreRadioButtonClickedNotification = false;
                }

                // Now raise the events.
                // Note: We also increment the stack count here to prevent
                // navigating the dialog and setting the Checked property
                // within the event handlers here even though this would work
                // correctly for the native API (as we are not in the
                // TDN_RADIO_BUTTON_CLICKED notification), because we are
                // raising two events (Unchecked+Checked), and when the
                // second event is called, the dialog might already be
                // navigated or another radio button may have been checked.
                TaskDialog boundDialog = BoundPage.BoundDialog;
                checked
                {
                    boundDialog.RadioButtonClickedStackCount++;
                }
                try
                {
                    HandleRadioButtonClicked();
                }
                finally
                {
                    boundDialog.RadioButtonClickedStackCount--;
                }
            }
        }

        internal int RadioButtonID => _radioButtonID;

        internal TaskDialogRadioButtonCollection? Collection
        {
            get => _collection;
            set => _collection = value;
        }

        /// <summary>
        ///   Returns a string that represents the current <see cref="TaskDialogRadioButton"/> control.
        /// </summary>
        /// <returns>A string that contains the control text.</returns>
        public override string ToString() => _text ?? base.ToString() ?? string.Empty;

        internal ComCtl32.TDF Bind(TaskDialogPage page, int radioButtonID)
        {
            ComCtl32.TDF result = Bind(page);
            _radioButtonID = radioButtonID;

            return result;
        }

        internal void HandleRadioButtonClicked()
        {
            // Check if we need to ignore the notification when it is caused by
            // sending the TDM_CLICK_RADIO_BUTTON message.
            if (_ignoreRadioButtonClickedNotification)
            {
                return;
            }

            if (_checked)
            {
                return;
            }

            _checked = true;

            // Before raising the CheckedChanged event for the current button,
            // uncheck the other radio buttons and call their events (there
            // should be no more than one other button that is already checked).
            foreach (TaskDialogRadioButton radioButton in BoundPage!.RadioButtons)
            {
                if (radioButton != this && radioButton._checked)
                {
                    radioButton._checked = false;
                    radioButton.OnCheckedChanged(EventArgs.Empty);
                }
            }

            // Finally, call the event for the current button.
            OnCheckedChanged(EventArgs.Empty);
        }

        private protected override void UnbindCore()
        {
            _radioButtonID = 0;

            base.UnbindCore();
        }

        private protected override void ApplyInitializationCore()
        {
            // Re-set the properties so they will make the necessary calls.
            if (!_enabled)
            {
                Enabled = _enabled;
            }
        }

        private bool CanUpdate()
        {
            // Only update the button when bound to a task dialog and we are not
            // waiting for the Navigated event. In the latter case we don't throw
            // an exception however, because ApplyInitialization() will be called
            // in the Navigated handler that does the necessary updates.
            return BoundPage?.WaitingForInitialization == false;
        }

        private void OnCheckedChanged(EventArgs e) => CheckedChanged?.Invoke(this, e);
    }
}
