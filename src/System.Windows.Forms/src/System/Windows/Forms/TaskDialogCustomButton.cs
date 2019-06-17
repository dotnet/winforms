// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using TaskDialogFlags = Interop.TaskDialog.TASKDIALOG_FLAGS;

namespace System.Windows.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TaskDialogCustomButton : TaskDialogButton
    {
        private string _text;

        private string _descriptionText;

        private int _buttonID;

        /// <summary>
        /// 
        /// </summary>
        public TaskDialogCustomButton()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public TaskDialogCustomButton(string text, string descriptionText = null)
            : this()
        {
            _text = text;
            _descriptionText = descriptionText;
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
        /// Gets or sets an additional description text that will be displayed in
        /// a separate line of the command link when
        /// <see cref="TaskDialogPage.CustomButtonStyle"/> is set to
        /// <see cref="TaskDialogCustomButtonStyle.CommandLinks"/> or
        /// <see cref="TaskDialogCustomButtonStyle.CommandLinksNoIcon"/>.
        /// </summary>
        public string DescriptionText
        {
            get => _descriptionText;

            set
            {
                DenyIfBound();

                _descriptionText = value;
            }
        }

        internal override bool IsCreatable
        {
            get => base.IsCreatable && !TaskDialogPage.IsNativeStringNullOrEmpty(_text);
        }

        internal override int ButtonID
        {
            get => _buttonID;
        }

        internal new TaskDialogCustomButtonCollection Collection
        {
            get => (TaskDialogCustomButtonCollection)base.Collection;
            set => base.Collection = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _text ?? base.ToString();
        }

        internal TaskDialogFlags Bind(TaskDialogPage page, int buttonID)
        {
            TaskDialogFlags result = Bind(page);
            _buttonID = buttonID;

            return result;
        }

        internal string GetResultingText()
        {
            TaskDialogPage page = BoundPage;

            // Remove LFs from the text. Otherwise, the dialog would display the
            // part of the text after the LF in the command link note, but for
            // this we have the "DescriptionText" property, so we should ensure that
            // there is not an discrepancy here and that the contents of the "Text"
            // property are not displayed in the command link note.
            // Therefore, we replace a combined CR+LF with CR, and then also single
            // LFs with CR, because CR is treated as a line break.
            string text = _text?.Replace("\r\n", "\r").Replace("\n", "\r");

            if ((page?.CustomButtonStyle == TaskDialogCustomButtonStyle.CommandLinks ||
                    page?.CustomButtonStyle == TaskDialogCustomButtonStyle.CommandLinksNoIcon) &&
                    text != null && _descriptionText != null)
                text += '\n' + _descriptionText;

            return text;
        }

        private protected override void UnbindCore()
        {
            _buttonID = 0;

            base.UnbindCore();
        }
    }
}
