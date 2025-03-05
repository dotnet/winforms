// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///   Represents a verification checkbox control of a task dialog.
/// </summary>
public sealed class TaskDialogVerificationCheckBox : TaskDialogControl
{
    private string? _text;
    private bool _checked;

    /// <summary>
    ///   Occurs when the value of the <see cref="Checked"/> property changes while
    ///   this control is shown in a task dialog.
    /// </summary>
    public event EventHandler? CheckedChanged;

    /// <summary>
    ///   Initializes a new instance of the <see cref="TaskDialogVerificationCheckBox"/> class.
    /// </summary>
    public TaskDialogVerificationCheckBox()
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="TaskDialogVerificationCheckBox"/> class with
    ///   the given text.
    /// </summary>
    /// <param name="text">A text associated with this control.</param>
    /// <param name="isChecked">A value indicating whether the <see cref="TaskDialogVerificationCheckBox"/> is in
    ///   the checked state.</param>
    public TaskDialogVerificationCheckBox(string? text, bool isChecked = false)
        : this()
    {
        _text = text;
        Checked = isChecked;
    }

    public static implicit operator TaskDialogVerificationCheckBox(string verificationText)
        => new(verificationText);

    /// <summary>
    ///   Gets or sets the text associated with this control.
    /// </summary>
    /// <value>
    ///   The text associated with this control. The default value is <see langword="null"/>.
    /// </value>
    /// <remarks>
    /// <para>
    ///   This control is only shown if this property is not <see langword="null"/> or an empty string.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///   The property is set and this verification checkbox instance is currently bound to a task dialog.
    /// </exception>
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
    ///   Gets or set a value indicating whether the <see cref="TaskDialogVerificationCheckBox"/> is in
    ///   the checked state.
    /// </summary>
    /// <value>
    ///   <see langword="true"/> if the <see cref="TaskDialogVerificationCheckBox"/> is in the checked state;
    ///   otherwise, <see langword="false"/>. The default value is <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// <para>
    ///   This property can be set while the dialog is shown.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///   The property is set on a verification checkbox instance that is currently bound to a task dialog,
    ///   but it's not visible as its initial <see cref="Text"/> property value was
    ///   <see langword="null"/> or an empty string.
    ///   - or -
    ///   The property is set and the task dialog has started navigating to a new page containing this
    ///   verification checkbox instance, but the <see cref="TaskDialogPage.Created"/> event has not been raised yet.
    ///   - or -
    ///   The property is set on a verification checkbox instance that is currently bound to a task dialog,
    ///   but the dialog has just started navigating to a different page.
    /// </exception>
    public bool Checked
    {
        get => _checked;
        set
        {
            DenyIfBoundAndNotCreated();
            DenyIfWaitingForInitialization();

            if (BoundPage is null)
            {
                _checked = value;
            }
            else
            {
                // Click the checkbox which should cause a call to
                // HandleCheckBoxClicked(), where we will update the checked
                // state.
                BoundPage.BoundDialog!.ClickCheckBox(value);
            }
        }
    }

    internal override bool IsCreatable => base.IsCreatable && !TaskDialogPage.IsNativeStringNullOrEmpty(_text);

    /// <summary>
    ///   Sets input focus to the control.
    /// </summary>
    internal void Focus()
    {
        DenyIfNotBoundOrWaitingForInitialization();
        DenyIfBoundAndNotCreated();

        BoundPage!.BoundDialog!.ClickCheckBox(_checked, true);
    }

    /// <summary>
    ///   Returns a string that represents the current <see cref="TaskDialogVerificationCheckBox"/> control.
    /// </summary>
    /// <returns>The control text.</returns>
    public override string ToString() => _text ?? base.ToString() ?? string.Empty;

    internal void HandleCheckBoxClicked(bool @checked)
    {
        // Only raise the event if the state actually changed.
        if (@checked != _checked)
        {
            _checked = @checked;
            OnCheckedChanged(EventArgs.Empty);
        }
    }

    private protected override TASKDIALOG_FLAGS BindCore()
    {
        TASKDIALOG_FLAGS flags = base.BindCore();

        if (_checked)
        {
            flags |= TASKDIALOG_FLAGS.TDF_VERIFICATION_FLAG_CHECKED;
        }

        return flags;
    }

    private void OnCheckedChanged(EventArgs e) => CheckedChanged?.Invoke(this, e);
}
