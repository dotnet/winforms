// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///   Represents the footer area of a task dialog.
    /// </summary>
    public sealed class TaskDialogFooter : TaskDialogControl
    {
        private string? _text;
        private TaskDialogIcon? _icon;
        private bool _boundIconIsFromHandle;

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogFooter"/> class.
        /// </summary>
        public TaskDialogFooter()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogFooter"/> class
        ///   using the given <paramref name="text"/>.
        /// </summary>
        /// <param name="text">The text to be displayed in the dialog's footer area.</param>
        public TaskDialogFooter(string? text)
            : this()
        {
            _text = text;
        }

        public static implicit operator TaskDialogFooter(string footerText)
          => new TaskDialogFooter(footerText);

        /// <summary>
        ///   Gets or sets the text to be displayed in the dialog's footer area.
        /// </summary>
        /// <value>
        ///   The text to be displayed in the dialog's footer area. The default value is <see langword="null"/>.
        /// </value>
        /// <remarks>
        /// <para>
        ///   This control will only be shown if this property is not <see langword="null"/> or an empty string.
        /// </para>
        /// <para>
        ///   This property can be set while the dialog is shown.
        /// </para>
        /// </remarks>
        public string? Text
        {
            get => _text;

            set
            {
                DenyIfBoundAndNotCreated();
                DenyIfWaitingForInitialization();

                // Update the text if we are bound.
                BoundTaskDialog?.UpdateTextElement(
                    ComCtl32.TDE.FOOTER, value);

                _text = value;
            }
        }

        /// <summary>
        ///   Gets or sets the footer icon.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   This property can be set while the dialog is shown (but in that case, it
        ///   cannot be switched between instances created from an
        ///   <see cref="System.Drawing.Icon"/> (or from a handle pointer)
        ///   and standard icon instances).
        /// </para>
        /// </remarks>
        public TaskDialogIcon? Icon
        {
            get => _icon;

            set
            {
                DenyIfBoundAndNotCreated();
                DenyIfWaitingForInitialization();

                (IntPtr iconValue, bool? iconIsFromHandle) = TaskDialogPage.GetIconValue(value);

                // The native task dialog icon cannot be updated from a handle
                // type to a non-handle type and vice versa, so we need to throw
                // throw in such a case.
                if (BoundPage != null && iconIsFromHandle != null && iconIsFromHandle != _boundIconIsFromHandle)
                {
                    throw new InvalidOperationException(SR.TaskDialogCannotUpdateIconType);
                }

                BoundTaskDialog?.UpdateIconElement(
                    ComCtl32.TDIE.ICON_FOOTER, iconValue);

                _icon = value;
            }
        }

        internal override bool IsCreatable
        {
            get => base.IsCreatable && !TaskDialogPage.IsNativeStringNullOrEmpty(_text);
        }

        /// <summary>
        ///   Returns a string that represents the current <see cref="TaskDialogFooter"/> control.
        /// </summary>
        /// <returns>A string that contains the control text.</returns>
        public override string ToString() => _text ?? base.ToString() ?? string.Empty;

        internal ComCtl32.TDF Bind(TaskDialogPage page, out IntPtr footerIconValue)
        {
            ComCtl32.TDF result = base.Bind(page);

            footerIconValue = TaskDialogPage.GetIconValue(_icon).iconValue;

            return result;
        }

        private protected override ComCtl32.TDF BindCore()
        {
            ComCtl32.TDF flags = base.BindCore();

            _boundIconIsFromHandle = TaskDialogPage.GetIconValue(_icon).iconIsFromHandle ?? false;

            if (_boundIconIsFromHandle)
            {
                flags |= ComCtl32.TDF.USE_HICON_FOOTER;
            }

            return flags;
        }

        private protected override void UnbindCore()
        {
            _boundIconIsFromHandle = false;

            base.UnbindCore();
        }
    }
}
