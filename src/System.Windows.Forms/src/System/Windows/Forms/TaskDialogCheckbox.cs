// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using TaskDialogFlags = Interop.TaskDialog.TASKDIALOG_FLAGS;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a checkbox control of a task dialog.
    /// </summary>
    public sealed class TaskDialogCheckBox : TaskDialogControl
    {
        private string? _text;

        private bool _checked;

        /// <summary>
        /// Occurs when the value of the <see cref="Checked"/> property changes while 
        /// this control is shown in a task dialog.
        /// </summary>
        public event EventHandler CheckedChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDialogCheckBox"/> class.
        /// </summary>
        public TaskDialogCheckBox()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDialogCheckBox"/> class with
        /// the given <paramref name="text"/>.
        /// </summary>
        /// <param name="text"></param>
        public TaskDialogCheckBox(string? text)
            : this()
        {
            _text = text;
        }

        /// <summary>
        /// Gets or sets the text associated with this control.
        /// </summary>
        /// <value>
        /// The text associated with this control. The default value is <c>null</c>.
        /// </value>
        /// <remarks>
        /// This control will only be shown if this property is not <c>null</c> or an empty string.
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
        /// Gets or set a value indicating whether the <see cref="TaskDialogCheckBox"/> is in
        /// the checked state.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the <see cref="TaskDialogCheckBox"/> is in the checked state;
        /// otherwise, <see langword="false"/>. The default value is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// This property can be set while the dialog is shown.
        /// </remarks>
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
                }
                else
                {
                    // Click the checkbox which should cause a call to
                    // HandleCheckBoxClicked(), where we will update the checked
                    // state.
                    BoundPage.BoundTaskDialog!.ClickCheckBox(value);
                }
            }
        }

        internal override bool IsCreatable
        {
            get => base.IsCreatable && !TaskDialogPage.IsNativeStringNullOrEmpty(_text);
        }

        // TODO: Maybe remove this method for now
        /// <summary>
        /// 
        /// </summary>
        public void Focus()
        {
            DenyIfNotBoundOrWaitingForInitialization();
            DenyIfBoundAndNotCreated();

            BoundPage!.BoundTaskDialog!.ClickCheckBox(_checked, true);
        }

        /// <summary>
        /// Returns a string that represents the current <see cref="TaskDialogCheckBox"/> control.
        /// </summary>
        /// <returns>A string that contains the control text.</returns>
        public override string? ToString()
        {
            return _text ?? base.ToString();
        }

        internal void HandleCheckBoxClicked(bool @checked)
        {
            // Only raise the event if the state actually changed.
            if (@checked != _checked)
            {
                _checked = @checked;
                OnCheckedChanged(EventArgs.Empty);
            }
        }

        private protected override TaskDialogFlags BindCore()
        {
            TaskDialogFlags flags = base.BindCore();

            if (_checked)
            {
                flags |= TaskDialogFlags.TDF_VERIFICATION_FLAG_CHECKED;
            }

            return flags;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void OnCheckedChanged(EventArgs e)
        {
            CheckedChanged?.Invoke(this, e);
        }
    }
}
