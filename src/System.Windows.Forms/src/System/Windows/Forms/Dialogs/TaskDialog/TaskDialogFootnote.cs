// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

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
        => new(footnoteText);

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

            if (BoundPage is not null)
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
                        TASKDIALOG_ELEMENTS.TDE_FOOTER, value);
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
    ///   <see cref="Drawing.Icon"/> (or from a handle pointer)
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

            if (BoundPage is not null)
            {
                (TASKDIALOGCONFIG._Anonymous2_e__Union icon, bool? iconIsFromHandle) =
                    TaskDialogPage.GetFooterIconValue(value);

                // The native task dialog icon cannot be updated from a handle
                // type to a non-handle type and vice versa, so we need to throw
                // in such a case.
                if (iconIsFromHandle is not null && iconIsFromHandle != _boundIconIsFromHandle)
                {
                    throw new InvalidOperationException(SR.TaskDialogCannotUpdateIconType);
                }

                BoundPage.BoundDialog!.UpdateIconElement(
                     TASKDIALOG_ICON_ELEMENTS.TDIE_ICON_FOOTER,
                    _boundIconIsFromHandle ? icon.hFooterIcon : (IntPtr)(char*)icon.pszFooterIcon);
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

    internal TASKDIALOG_FLAGS Bind(TaskDialogPage page, out TASKDIALOGCONFIG._Anonymous2_e__Union icon)
    {
        TASKDIALOG_FLAGS result = Bind(page);

        icon = TaskDialogPage.GetFooterIconValue(_icon).iconUnion;

        return result;
    }

    private protected override TASKDIALOG_FLAGS BindCore()
    {
        TASKDIALOG_FLAGS flags = base.BindCore();

        _updateTextOnInitialization = false;
        _boundIconIsFromHandle = TaskDialogPage.GetFooterIconValue(_icon).iconIsFromHandle ?? false;

        if (_boundIconIsFromHandle)
        {
            flags |= TASKDIALOG_FLAGS.TDF_USE_HICON_FOOTER;
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
