// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using TaskDialogFlags = Interop.TaskDialog.TASKDIALOG_FLAGS;

namespace System.Windows.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TaskDialogCheckBox : TaskDialogControl
    {
        private string _text;

        private bool _checked;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler CheckedChanged;

        /// <summary>
        /// 
        /// </summary>
        public TaskDialogCheckBox()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public TaskDialogCheckBox(string text)
            : this()
        {
            _text = text;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get => _text;

            set
            {
                DenyIfBound();

                _text = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
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
                    BoundPage.BoundTaskDialog.ClickCheckBox(
                            value);
                }
            }
        }

        internal override bool IsCreatable
        {
            get => base.IsCreatable && !TaskDialogPage.IsNativeStringNullOrEmpty(_text);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Focus()
        {
            DenyIfNotBoundOrWaitingForInitialization();
            DenyIfBoundAndNotCreated();

            BoundPage.BoundTaskDialog.ClickCheckBox(
                    _checked,
                    true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
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
                flags |= TaskDialogFlags.TDF_VERIFICATION_FLAG_CHECKED;

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
