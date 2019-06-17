// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using TaskDialogFlags = Interop.TaskDialog.TASKDIALOG_FLAGS;
using TaskDialogTextElement = Interop.TaskDialog.TASKDIALOG_ELEMENTS;

namespace System.Windows.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TaskDialogExpander : TaskDialogControl
    {
        private string _text;

        private string _expandedButtonText;

        private string _collapsedButtonText;

        private bool _expandFooterArea;

        private bool _expanded;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler ExpandedChanged;

        /// <summary>
        /// 
        /// </summary>
        public TaskDialogExpander()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public TaskDialogExpander(string text)
            : this()
        {
            _text = text;
        }

        /// <summary>
        /// Gets or sets the text to be displayed in the dialog's expanded area.
        /// </summary>
        /// <remarks>
        /// This property can be set while the dialog is shown.
        /// </remarks>
        public string Text
        {
            get => _text;

            set
            {
                DenyIfBoundAndNotCreated();
                DenyIfWaitingForInitialization();

                // Update the text if we are bound.
                BoundPage?.BoundTaskDialog.UpdateTextElement(
                        TaskDialogTextElement.TDE_EXPANDED_INFORMATION,
                        value);

                _text = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ExpandedButtonText
        {
            get => _expandedButtonText;

            set
            {
                DenyIfBound();

                _expandedButtonText = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string CollapsedButtonText
        {
            get => _collapsedButtonText;

            set
            {
                DenyIfBound();

                _collapsedButtonText = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Expanded
        {
            get => _expanded;

            set
            {
                // The Task Dialog doesn't provide a message type to click the expando
                // button, so we don't allow to change this property (it will however
                // be updated when we receive an ExpandoButtonClicked notification).
                // TODO: Should we throw only if the new value is different than the
                // old one?
                DenyIfBound();

                _expanded = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ExpandFooterArea
        {
            get => _expandFooterArea;

            set
            {
                DenyIfBound();

                _expandFooterArea = value;
            }
        }

        internal override bool IsCreatable
        {
            get => base.IsCreatable && !TaskDialogPage.IsNativeStringNullOrEmpty(_text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _text ?? base.ToString();
        }

        internal void HandleExpandoButtonClicked(bool expanded)
        {
            _expanded = expanded;
            OnExpandedChanged(EventArgs.Empty);
        }

        private protected override TaskDialogFlags BindCore()
        {
            TaskDialogFlags flags = base.BindCore();

            if (_expanded)
                flags |= TaskDialogFlags.TDF_EXPANDED_BY_DEFAULT;
            if (_expandFooterArea)
                flags |= TaskDialogFlags.TDF_EXPAND_FOOTER_AREA;

            return flags;
        }

        private void OnExpandedChanged(EventArgs e)
        {
            ExpandedChanged?.Invoke(this, e);
        }
    }
}
