// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Collections.Generic;

namespace System.Windows.Forms
{
    /// <summary>
    ///   Represents a button control of a task dialog.
    /// </summary>
    public abstract class TaskDialogButton : TaskDialogControl
    {
        private bool _enabled = true;
        private bool _defaultButton;
        private bool _elevationRequired;

        /// <summary>
        ///   Occurs when the button is clicked.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   By default, the dialog will be closed after the event handler returns 
        ///   (except for the <see cref="TaskDialogResult.Help"/> button, which instead
        ///   will raise the <see cref="TaskDialogPage.HelpRequest"/> event afterwards).
        ///   To prevent the dialog from closing when this button is clicked, set the
        ///   <see cref="ShouldCloseDialog"/> property to <see langword="false"/>.
        /// </para>
        /// <para>
        ///   When <see cref="ShouldCloseDialog"/> is set to <see langword="true"/>,
        ///   the <see cref="TaskDialog.Closing"/> event will occur afterwards,
        ///   which also allows you to prevent the dialog from closing.
        /// </para>
        /// </remarks>
        public event EventHandler<EventArgs>? Click;

        // Disallow inheritance by specifying a private protected constructor.
        private protected TaskDialogButton()
        {
        }

        /// <summary>
        ///   Gets or sets a value that indicates whether the task dialog should close
        ///   when this button is clicked. Or, if this button represents the
        ///   <see cref="TaskDialogResult.Help"/> result, indicates whether the
        ///   <see cref="TaskDialogPage.HelpRequest"/> should be raised.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if the task dialog should close when
        ///   this button is clicked; otherwise, <see langword="false"/>. The default value is <see langword="true"/>.
        /// </value>
        /// <remarks>
        /// <para>
        ///   If this property is set to <see langword="true"/> after the <see cref="Click"/>
        ///   event handler returns, the <see cref="TaskDialog.Closing"/> event will occur
        ///   (except if this button represents the <see cref="TaskDialogResult.Help"/> result),
        ///   which allows you to cancel the close. If it isn't canceled, the dialog closes and
        ///   sets the clicked button as result value.
        /// </para>
        /// </remarks>
        public bool ShouldCloseDialog { get; set; } = true;

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
        ///   Gets or sets a value that indicates whether this button is the default button
        ///   in the task dialog.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if this button is the default button in the task dialog;
        ///   otherwise, <see langword="false"/>. The default value is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// <para>
        ///   Only a single button in a task dialog can be set as the default button.
        /// </para>
        /// </remarks>
        public bool DefaultButton
        {
            get => _defaultButton;

            set
            {
                _defaultButton = value;

                // If we are part of a collection, set the defaultButton value of
                // all other buttons to false.
                // Note that this does not handle buttons that are added later to
                // the collection.
                if (Collection == null || !value)
                {
                    return;
                }

                foreach (TaskDialogButton button in Collection)
                {
                    button._defaultButton = button == this;
                }
            }
        }

        internal abstract int ButtonID { get; }

        // Note: Instead of declaring an abstract Collection getter, we implement
        // the field and the property here so that the subclass doesn't have to
        // do the implementation, in order to avoid duplicating the logic
        // (e.g. if we ever need to add actions in the setter, it normally would
        // be the same for all subclasses). Instead, the subclass can declare
        // a new (internal) Collection property which has a more specific type.
        private protected IReadOnlyList<TaskDialogButton>? Collection { get; set; }

        /// <summary>
        ///   Simulates a click on this button.
        /// </summary>
        public void PerformClick()
        {
            // Note: We allow a click even if the button is not visible/created.
            DenyIfNotBoundOrWaitingForInitialization();

            BoundPage!.BoundDialog!.ClickButton(ButtonID);
        }

        internal bool HandleButtonClicked()
        {
            OnClick(EventArgs.Empty);

            return ShouldCloseDialog;
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
