// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.PropertyGridInternal;

internal class GridToolTip : Control
{
    private readonly Control[] _controls;
    private string? _toolTipText;
    private bool _dontShow;
    private const int MaximumToolTipLength = 1000;

    internal GridToolTip(Control[] controls)
    {
        _controls = controls;
        SetStyle(ControlStyles.UserPaint, false);
        Font = controls[0].Font;

        for (int i = 0; i < controls.Length; i++)
        {
            controls[i].HandleCreated += OnControlCreateHandle;
            controls[i].HandleDestroyed += OnControlDestroyHandle;

            if (controls[i].IsHandleCreated)
            {
                SetupToolTip(controls[i]);
            }
        }
    }

    public string? ToolTip
    {
        get => _toolTipText;
        set
        {
            if (IsHandleCreated || !string.IsNullOrEmpty(value))
            {
                Reset();
            }

            if (value is not null && value.Length > MaximumToolTipLength)
            {
                // Let the user know the text was truncated by throwing on an ellipsis.
                value = string.Concat(value.AsSpan(0, MaximumToolTipLength), "...");
            }

            _toolTipText = value;

            if (IsHandleCreated)
            {
                bool visible = Visible;

                if (visible)
                {
                    Visible = false;
                }

                // If we give the tooltip an empty string, it won't come back so we just force it hidden instead.
                _dontShow = string.IsNullOrEmpty(value);

                for (int i = 0; i < _controls.Length; i++)
                {
                    ToolInfoWrapper<Control> info = GetTOOLINFO(_controls[i]);
                    info.SendMessage(this, PInvoke.TTM_UPDATETIPTEXTW);
                }

                if (visible && !_dontShow)
                {
                    Visible = true;
                }
            }
        }
    }

    protected override unsafe CreateParams CreateParams
    {
        get
        {
            PInvoke.InitCommonControlsEx(new INITCOMMONCONTROLSEX
            {
                dwSize = (uint)sizeof(INITCOMMONCONTROLSEX),
                dwICC = INITCOMMONCONTROLSEX_ICC.ICC_TAB_CLASSES
            });

            return new CreateParams()
            {
                Parent = IntPtr.Zero,
                ClassName = PInvoke.TOOLTIPS_CLASS,
                Style = (int)(PInvoke.TTS_ALWAYSTIP | PInvoke.TTS_NOPREFIX),
                ExStyle = 0,
                Caption = ToolTip,
            };
        }
    }

    private ToolInfoWrapper<Control> GetTOOLINFO(Control c)
        => new(c, TOOLTIP_FLAGS.TTF_TRANSPARENT | TOOLTIP_FLAGS.TTF_SUBCLASS, _toolTipText);

    private void OnControlCreateHandle(object? sender, EventArgs e) => SetupToolTip((Control?)sender);

    private void OnControlDestroyHandle(object? sender, EventArgs e)
    {
        if (IsHandleCreated && sender is not null)
        {
            GetTOOLINFO((Control)sender).SendMessage(this, PInvoke.TTM_DELTOOLW);
        }
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        for (int i = 0; i < _controls.Length; i++)
        {
            if (_controls[i].IsHandleCreated)
            {
                SetupToolTip(_controls[i]);
            }
        }
    }

    private void SetupToolTip(Control? control)
    {
        if (IsHandleCreated && control is not null)
        {
            PInvoke.SetWindowPos(
                this,
                HWND.HWND_TOPMOST,
                0, 0, 0, 0,
                SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE);

            ToolInfoWrapper<Control> info = GetTOOLINFO(control);
            if (info.SendMessage(this, PInvoke.TTM_ADDTOOLW) == 0)
            {
                Debug.Fail($"TTM_ADDTOOL failed for {control.GetType().Name}");
            }

            // Setting the max width has the added benefit of enabling multiline tool tips
            PInvokeCore.SendMessage(
                this,
                PInvoke.TTM_SETMAXTIPWIDTH,
                (WPARAM)0,
                (LPARAM)SystemInformation.MaxWindowTrackSize.Width);
        }
    }

    public void Reset()
    {
        // This resets the tooltip state, which can get broken when we leave the window
        // then reenter. So we set the tooltip to null, update the text, then it back to
        // what it was, so the tooltip thinks it's back in the regular state again.

        string? oldText = ToolTip;
        _toolTipText = string.Empty;

        for (int i = 0; i < _controls.Length; i++)
        {
            ToolInfoWrapper<Control> info = GetTOOLINFO(_controls[i]);
            info.SendMessage(this, PInvoke.TTM_UPDATETIPTEXTW);
        }

        _toolTipText = oldText;
        PInvokeCore.SendMessage(this, PInvoke.TTM_UPDATE);
    }

    protected override void WndProc(ref Message msg)
    {
        switch (msg.MsgInternal)
        {
            case PInvokeCore.WM_SHOWWINDOW:
                if ((int)msg.WParamInternal != 0 && _dontShow)
                {
                    msg.WParamInternal = 0u;
                }

                break;
            case PInvokeCore.WM_NCHITTEST:
                // When using v6 common controls, the native tooltip does not end up returning HTTRANSPARENT
                // all the time, so its TTF_TRANSPARENT behavior does not work, ie. mouse events do not fall
                // thru to controls underneath. This is due to a combination of old app-specific code in comctl32,
                // functional changes between v5 and v6, and the specific way the property grid drives its tooltip.
                // Workaround is to just force HTTRANSPARENT all the time.
                msg.ResultInternal = (LRESULT)PInvoke.HTTRANSPARENT;
                return;
        }

        base.WndProc(ref msg);
    }
}
