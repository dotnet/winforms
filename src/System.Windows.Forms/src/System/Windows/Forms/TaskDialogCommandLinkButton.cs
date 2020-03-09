// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///   Represents a command link button control of a task dialog.
    /// </summary>
    public sealed class TaskDialogCommandLinkButton : TaskDialogButton
    {
        private string? _descriptionText;

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogButton"/> class.
        /// </summary>
        public TaskDialogCommandLinkButton()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogButton"/> class
        ///   using the given text and, optionally, a description text.
        /// </summary>
        /// <param name="text">The text of the control.</param>
        /// <param name="descriptionText">An additional description text that will be displayed in
        /// a separate line when the <see cref="TaskDialogButton"/>s of the task dialog are
        /// shown as command links (see <see cref="DescriptionText"/>).</param>
        /// <param name="allowCloseDialog">A value that indicates whether the task dialog should close
        ///   when this button is clicked.
        /// </param>
        public TaskDialogCommandLinkButton(
            string? text,
            string? descriptionText = null,
            bool enabled = true,
            bool allowCloseDialog = true)
            : base(text, enabled, allowCloseDialog)
        {
            _descriptionText = descriptionText;
        }

        /// <summary>
        ///   Gets or sets an additional description text that will be displayed in a separate
        ///   line.
        /// </summary>
        public string? DescriptionText
        {
            get => _descriptionText;

            set
            {
                DenyIfBound();

                _descriptionText = value;
            }
        }

        internal override string? GetResultingText()
        {
            string? text = base.GetResultingText();

            if (text != null && _descriptionText != null)
            {
                text += '\n' + _descriptionText;
            }

            return text;
        }
    }
}
