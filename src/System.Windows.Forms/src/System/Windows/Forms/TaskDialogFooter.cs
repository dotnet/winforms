// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using TaskDialogFlags = Interop.TaskDialog.TASKDIALOG_FLAGS;
using TaskDialogIconElement = Interop.TaskDialog.TASKDIALOG_ICON_ELEMENTS;
using TaskDialogTextElement = Interop.TaskDialog.TASKDIALOG_ELEMENTS;

namespace System.Windows.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TaskDialogFooter : TaskDialogControl
    {
        private string _text;

        private TaskDialogIcon _icon;

        private bool _boundIconIsFromHandle;

        /// <summary>
        /// 
        /// </summary>
        public TaskDialogFooter()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public TaskDialogFooter(string text)
            : this()
        {
            _text = text;
        }

        /// <summary>
        /// Gets or sets the text to be displayed in the dialog's footer area.
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
                        TaskDialogTextElement.TDE_FOOTER,
                        value);

                _text = value;
            }
        }

        /// <summary>
        /// Gets or sets the footer icon.
        /// </summary>
        /// <remarks>
        /// This property can be set while the dialog is shown (but in that case, it
        /// cannot be switched between instances of <see cref="TaskDialogIconHandle"/>
        /// and instances of other icon types).
        /// </remarks>
        public TaskDialogIcon Icon
        {
            get => _icon;

            set
            {
                DenyIfBoundAndNotCreated();
                DenyIfWaitingForInitialization();

                (IntPtr iconValue, bool? iconIsFromHandle) =
                        TaskDialogPage.GetIconValue(value);

                // The native task dialog icon cannot be updated from a handle
                // type to a non-handle type and vice versa, so we need to throw
                // throw in such a case.
                if (BoundPage != null &&
                        iconIsFromHandle != null &&
                        iconIsFromHandle != _boundIconIsFromHandle)
                    throw new InvalidOperationException(
                            "Cannot update the icon from a handle icon type to a " +
                            "non-handle icon type, and vice versa.");

                BoundPage?.BoundTaskDialog.UpdateIconElement(
                        TaskDialogIconElement.TDIE_ICON_FOOTER,
                        iconValue);

                _icon = value;
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

        internal TaskDialogFlags Bind(TaskDialogPage page, out IntPtr footerIconValue)
        {
            TaskDialogFlags result = base.Bind(page);

            footerIconValue = TaskDialogPage.GetIconValue(_icon).iconValue;

            return result;
        }

        private protected override TaskDialogFlags BindCore()
        {
            TaskDialogFlags flags = base.BindCore();

            _boundIconIsFromHandle = TaskDialogPage.GetIconValue(_icon).iconIsFromHandle
                    ?? false;

            if (_boundIconIsFromHandle)
                flags |= TaskDialogFlags.TDF_USE_HICON_FOOTER;

            return flags;
        }

        private protected override void UnbindCore()
        {
            _boundIconIsFromHandle = false;

            base.UnbindCore();
        }
    }
}
