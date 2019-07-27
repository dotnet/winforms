// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using TaskDialogFlags = Interop.TaskDialog.TASKDIALOG_FLAGS;
using TaskDialogTextElement = Interop.TaskDialog.TASKDIALOG_ELEMENTS;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents an expander button and the associated expanded area
    /// of a task dialog.
    /// </summary>
    public sealed class TaskDialogExpander : TaskDialogControl
    {
        private string? _text;

        private string? _expandedButtonText;

        private string? _collapsedButtonText;

        private bool _expandFooterArea;

        private bool _expanded;

        /// <summary>
        /// Occurs when the value of the <see cref="Expanded"/> property changes while
        /// this control is shown in a task dialog.
        /// </summary>
        /// <remarks>
        /// This event will only occur when the expanded state is changed by the user,
        /// because it is not possible to programmatically change the <see cref="Expanded"/>
        /// property while this control is shown in a task dialog.
        /// </remarks>
        public event EventHandler ExpandedChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDialogExpander"/> class.
        /// </summary>
        public TaskDialogExpander()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDialogExpander"/> class
        /// using the given <paramref name="text"/>.
        /// </summary>
        /// <param name="text">The text to be displayed in the dialog's expanded area.</param>
        public TaskDialogExpander(string? text)
            : this()
        {
            _text = text;
        }

        /// <summary>
        /// Gets or sets the text to be displayed in the dialog's expanded area.
        /// </summary>
        /// <value>
        /// The text to be displayed in the dialog's expanded area. The default value is
        /// <c>null</c>.
        /// </value>
        /// <remarks>
        /// This control will only be shown if this property is not <c>null</c> or an empty string.
        /// 
        /// This property can be set while the dialog is shown.
        /// </remarks>
        public string? Text
        {
            get => _text;

            set
            {
                DenyIfBoundAndNotCreated();
                DenyIfWaitingForInitialization();

                // Update the text if we are bound.
                // Using the null-forgiving operator ("!") here would conflict with the null-conditional operator ("?").
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                BoundPage?.BoundTaskDialog.UpdateTextElement(
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    TaskDialogTextElement.TDE_EXPANDED_INFORMATION, value);

                _text = value;
            }
        }

        /// <summary>
        /// Gets or sets the text to be displayed in the expander button when it
        /// is in the expanded state.
        /// </summary>
        /// <value>
        /// The text that is to be displayed in the expander button when it
        /// is in the expanded state, or <c>null</c> or an empty string to use a
        /// text provided by the operating system. The default value is <c>null</c>.
        /// </value>
        public string? ExpandedButtonText
        {
            get => _expandedButtonText;

            set
            {
                DenyIfBound();

                _expandedButtonText = value;
            }
        }

        /// <summary>
        /// Gets or sets the text to be displayed in the expander button when it
        /// is in the collapsed state.
        /// </summary>
        /// <value>
        /// The text that is to be displayed in the expander button when it
        /// is in the collapsed state, or <c>null</c> or an empty string to use a
        /// text provided by the operating system. The default value is <c>null</c>.
        /// </value>
        public string? CollapsedButtonText
        {
            get => _collapsedButtonText;

            set
            {
                DenyIfBound();

                _collapsedButtonText = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates if the expander button is in the
        /// expanded state (so that the dialog's expanded area is visible).
        /// </summary>
        /// <remarks>
        /// <see langword="true"/> if the expander button is in the expanded state; <see langword="false"/> if
        /// it is in the collapsed state. The default value is <see langword="false"/>.
        /// </remarks>
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
        /// Gets or sets a value that indicates if the expanded area is displayed at the bottom
        /// of the task dialog's footer area, instead of immediately after the dialog's
        /// <see cref="TaskDialogPage.Text"/>.
        /// </summary>
        /// <value>
        /// <see langword="true"/> to display the expanded area at the bottom of the task dialog's
        /// footer area, <see langword="false"/> to display the expanded area immediately after the
        /// dialog's <see cref="TaskDialogPage.Text"/>. The default value is <see langword="false"/>.
        /// </value>
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
        /// Returns a string that represents the current <see cref="TaskDialogExpander"/> control.
        /// </summary>
        /// <returns>A string that contains the control text.</returns>
        public override string ToString()
        {
            return _text ?? base.ToString() ?? string.Empty;
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
            {
                flags |= TaskDialogFlags.TDF_EXPANDED_BY_DEFAULT;
            }
            if (_expandFooterArea)
            {
                flags |= TaskDialogFlags.TDF_EXPAND_FOOTER_AREA;
            }

            return flags;
        }

        private void OnExpandedChanged(EventArgs e)
        {
            ExpandedChanged?.Invoke(this, e);
        }
    }
}
