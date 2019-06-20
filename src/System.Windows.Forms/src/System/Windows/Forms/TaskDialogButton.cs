// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Windows.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class TaskDialogButton : TaskDialogControl
    {
        private bool _enabled = true;

        private bool _defaultButton;

        private bool _elevationRequired;

        private IReadOnlyList<TaskDialogButton> _collection;

        /// <summary>
        /// Occurs when the button is clicked.
        /// </summary>
        /// <remarks>
        /// By default, the dialog will be closed after the event handler returns 
        /// (except for the <see cref="TaskDialogResult.Help"/> button which instead
        /// will raise the <see cref="TaskDialogPage.HelpRequest"/> event afterwards).
        /// To prevent the dialog from closing, set the
        /// <see cref="TaskDialogButtonClickedEventArgs.CancelClose"/> property to
        /// <c>true</c>.
        /// </remarks>
        public event EventHandler<TaskDialogButtonClickedEventArgs> Click;

        // Disallow inheritance by specifying a private protected constructor.
        private protected TaskDialogButton()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// This property can be set while the dialog is shown.
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
                    BoundPage.BoundTaskDialog.SetButtonEnabled(ButtonID, value);
                }

                _enabled = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// This property can be set while the dialog is shown.
        /// </remarks>
        public bool ElevationRequired
        {
            get => _elevationRequired;

            set
            {
                DenyIfBoundAndNotCreated();

                if (CanUpdate())
                {
                    BoundPage.BoundTaskDialog.SetButtonElevationRequiredState(ButtonID, value);
                }

                _elevationRequired = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates if this button will be the default button
        /// in the Task Dialog.
        /// </summary>
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
                if (_collection != null && value)
                {
                    foreach (TaskDialogButton button in _collection)
                    {
                        button._defaultButton = button == this;
                    }
                }
            }
        }

        internal abstract int ButtonID
        {
            get;
        }

        // Note: Instead of declaring an abstract Collection getter, we implement
        // the field and the property here so that the subclass doesn't have to
        // do the implementation, in order to avoid duplicating the logic
        // (e.g. if we ever need to add actions in the setter, it normally would
        // be the same for all subclasses). Instead, the subclass can declare
        // a new (internal) Collection property which has a more specific type.
        private protected IReadOnlyList<TaskDialogButton> Collection
        {
            get => _collection;
            set => _collection = value;
        }

        /// <summary>
        /// Simulates a click on this button.
        /// </summary>
        public void PerformClick()
        {
            // Note: We allow a click even if the button is not visible/created.
            DenyIfNotBoundOrWaitingForInitialization();

            BoundPage.BoundTaskDialog.ClickButton(ButtonID);
        }

        internal bool HandleButtonClicked()
        {
            var e = new TaskDialogButtonClickedEventArgs();
            OnClick(e);

            return !e.CancelClose;
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

        private protected void OnClick(TaskDialogButtonClickedEventArgs e)
        {
            Click?.Invoke(this, e);
        }

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
