// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.Drawing.Design;

internal abstract partial class SelectionPanelBase : Control
{
    private bool _allowExit = true;
    private RadioButton? _checkedControl;
    private IWindowsFormsEditorService? _editorService;

    protected RadioButton CheckedControl
    {
        get => _checkedControl!;
        set
        {
            _checkedControl = value;
            FocusCheckedControl();
        }
    }

    protected abstract ControlCollection SelectionOptions { get; }

    public object? Value { get; protected set; }

    public void End()
    {
        _editorService = null;
        Value = null;
    }

    private void FocusCheckedControl()
    {
        // To actually move focus to a radio button, we need to call Focus() method.
        // However, that would raise OnClick event, which would close the editor.
        // We set allowExit to false, to block editor exit, on radio button selection change.
        _allowExit = false;
        CheckedControl.Focus();
        _allowExit = true;
    }

    private void OnClick(object? sender, EventArgs e)
    {
        if (_allowExit)
        {
            CheckedControl = (RadioButton)sender!;
            UpdateValue();
            Teardown();
        }
    }

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        FocusCheckedControl();
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        var radioButton = (RadioButton)sender!;
        Keys key = e.KeyCode;
        RadioButton target;

        switch (key)
        {
            case Keys.Up:
                target = ProcessUpKey(radioButton);
                break;
            case Keys.Down:
                target = ProcessDownKey(radioButton);
                break;
            case Keys.Left:
                target = ProcessLeftKey(radioButton);
                break;
            case Keys.Right:
                target = ProcessRightKey(radioButton);
                break;
            case Keys.Return:
                // Will tear down editor
                InvokeOnClick(radioButton, EventArgs.Empty);
                return;
            default:
                return;
        }

        e.Handled = true;

        if (target != sender)
        {
            CheckedControl = target;
        }
    }

    protected override bool ProcessDialogKey(Keys keyData)
    {
        if ((keyData & Keys.KeyCode) == Keys.Tab && (keyData & (Keys.Alt | Keys.Control)) == 0)
        {
            CheckedControl = ProcessTabKey(keyData);
            return true;
        }

        return base.ProcessDialogKey(keyData);
    }

    protected abstract RadioButton ProcessDownKey(RadioButton checkedControl);

    protected abstract RadioButton ProcessLeftKey(RadioButton checkedControl);

    protected abstract RadioButton ProcessRightKey(RadioButton checkedControl);

    protected abstract RadioButton ProcessTabKey(Keys keyData);

    protected abstract RadioButton ProcessUpKey(RadioButton checkedControl);

    protected abstract void SetInitialCheckedControl();

    public void Start(IWindowsFormsEditorService edSvc, object? value)
    {
        foreach (RadioButton radioButton in SelectionOptions)
        {
            radioButton.Checked = false;
        }

        _editorService = edSvc;
        Value = value;

        SetInitialCheckedControl();
        CheckedControl.Checked = true;
    }

    protected void ConfigureButtons()
    {
        foreach (RadioButton radioButton in SelectionOptions)
        {
            radioButton.Click += OnClick;
            radioButton.KeyDown += OnKeyDown;
        }
    }

    private void Teardown()
    {
        _editorService!.CloseDropDown();
    }

    protected abstract void UpdateValue();
}
