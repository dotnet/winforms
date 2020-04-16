// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///   Represents the footnote area of a task dialog.
    /// </summary>
    public sealed class TaskDialogFootnote : TaskDialogControl
    {
        private string? _text;
        private TaskDialogIcon? _icon;
        private bool _boundIconIsFromHandle;
        private bool _updateTextOnInitialization;

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogFootnote"/> class.
        /// </summary>
        public TaskDialogFootnote()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogFootnote"/> class
        ///   using the given <paramref name="text"/>.
        /// </summary>
        /// <param name="text">The text to be displayed in the dialog's footnote area.</param>
        public TaskDialogFootnote(string? text)
            : this()
        {
            _text = text;
        }

        public static implicit operator TaskDialogFootnote(string footnoteText)
            => new TaskDialogFootnote(footnoteText);

        /// <summary>
        ///   Gets or sets the text to be displayed in the dialog's footnote area.
        /// </summary>
        /// <value>
        ///   The text to be displayed in the dialog's footnote area. The default value is <see langword="null"/>.
        /// </value>
        /// <remarks>
        /// <para>
        ///   This control will only be shown if this property is not <see langword="null"/> or an empty string.
        /// </para>
        /// <para>
        ///   This property can be set while the dialog is shown.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        ///   The property is set on a footnote instance that is currently bound to a task dialog, but it's not visible as its initial
        ///   <see cref="Text"/> property value was <see langword="null"/> or an empty string.
        ///   - or -
        ///   The property is set on a footnote instance that is currently bound to a task dialog, but the dialog
        ///   has just started navigating to a different page.
        /// </exception>
        public string? Text
        {
            get => _text;
            set
            {
                DenyIfBoundAndNotCreated();

                if (BoundPage != null)
                {
                    // If we are bound but waiting for initialization (e.g. immediately after
                    // starting a navigation), we buffer the change until we apply the
                    // initialization (when navigation is finished).
                    if (BoundPage.WaitingForInitialization)
                    {
                        _updateTextOnInitialization = true;
                    }
                    else
                    {
                        BoundPage.BoundDialog!.UpdateTextElement(
                            ComCtl32.TDE.FOOTER, value);
                    }
                }

                _text = value;
            }
        }

        /// <summary>
        ///   Gets or sets the footnote icon.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   This property can be set while the dialog is shown (but in that case, it
        ///   cannot be switched between instances created from an
        ///   <see cref="System.Drawing.Icon"/> (or from a handle pointer)
        ///   and standard icon instances).
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        ///   The property is set on a footnote instance that is currently bound to a task dialog, but it's not visible as its initial
        ///   <see cref="Text"/> property value was <see langword="null"/> or an empty string.
        ///   - or -
        ///   The property is set and the task dialog has started navigating to a new page containing this footnote instance, but the
        ///   <see cref="TaskDialogPage.Created"/> event has not been raised yet.
        ///   - or -
        ///   The property is set on a footnote instance that is currently bound to a task dialog, but the dialog
        ///   has just started navigating to a different page.
        /// </exception>
        public unsafe TaskDialogIcon? Icon
        {
            get => _icon;
            set
            {
                DenyIfBoundAndNotCreated();

                // See comment in TaskDialogPage.Icon for why we don't allow to buffer changes
                // while waiting for the initialization.
                DenyIfWaitingForInitialization();

                if (BoundPage != null)
                {
                    (ComCtl32.TASKDIALOGCONFIG.IconUnion icon, bool? iconIsFromHandle) =
                        TaskDialogPage.GetIconValue(value);

                    // The native task dialog icon cannot be updated from a handle
                    // type to a non-handle type and vice versa, so we need to throw
                    // in such a case.
                    if (iconIsFromHandle != null && iconIsFromHandle != _boundIconIsFromHandle)
                    {
                        throw new InvalidOperationException(SR.TaskDialogCannotUpdateIconType);
                    }

                    BoundPage.BoundDialog!.UpdateIconElement(
                        ComCtl32.TDIE.ICON_FOOTER,
                        _boundIconIsFromHandle ? icon.hIcon : (IntPtr)icon.pszIcon);
                }

                _icon = value;
            }
        }

        internal override bool IsCreatable => base.IsCreatable && !TaskDialogPage.IsNativeStringNullOrEmpty(_text);

        /// <summary>
        ///   Returns a string that represents the current <see cref="TaskDialogFootnote"/> control.
        /// </summary>
        /// <returns>A string that contains the control text.</returns>
        public override string ToString() => _text ?? base.ToString() ?? string.Empty;

        internal ComCtl32.TDF Bind(TaskDialogPage page, out ComCtl32.TASKDIALOGCONFIG.IconUnion icon)
        {
            ComCtl32.TDF result = base.Bind(page);

            icon = TaskDialogPage.GetIconValue(_icon).iconUnion;

            return result;
        }

        private protected override ComCtl32.TDF BindCore()
        {
            ComCtl32.TDF flags = base.BindCore();

            _updateTextOnInitialization = false;
            _boundIconIsFromHandle = TaskDialogPage.GetIconValue(_icon).iconIsFromHandle ?? false;

            if (_boundIconIsFromHandle)
            {
                flags |= ComCtl32.TDF.USE_HICON_FOOTER;
            }

            return flags;
        }

        private protected override void ApplyInitializationCore()
        {
            base.ApplyInitializationCore();

            if (_updateTextOnInitialization)
            {
                Text = _text;
                _updateTextOnInitialization = false;
            }
        }

        private protected override void UnbindCore()
        {
            _boundIconIsFromHandle = false;

            base.UnbindCore();
        }
    }
}
