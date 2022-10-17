// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal class GridToolTip : Control
    {
        private readonly Control[] _controls;
        private string _toolTipText;
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

        public string ToolTip
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
                        ComCtl32.ToolInfoWrapper<Control> info = GetTOOLINFO(_controls[i]);
                        info.SendMessage(this, (User32.WM)ComCtl32.TTM.UPDATETIPTEXTW);
                    }

                    if (visible && !_dontShow)
                    {
                        Visible = true;
                    }
                }
            }
        }

        /// <summary>
        ///  The createParams to create the window.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                var icc = new ComCtl32.INITCOMMONCONTROLSEX
                {
                    dwICC = ComCtl32.ICC.TAB_CLASSES
                };

                ComCtl32.InitCommonControlsEx(ref icc);

                var cp = new CreateParams
                {
                    Parent = IntPtr.Zero,
                    ClassName = PInvoke.TOOLTIPS_CLASS
                };

                cp.Style |= (int)(ComCtl32.TTS.ALWAYSTIP | ComCtl32.TTS.NOPREFIX);
                cp.ExStyle = 0;
                cp.Caption = ToolTip;
                return cp;
            }
        }

        private ComCtl32.ToolInfoWrapper<Control> GetTOOLINFO(Control c)
            => new(c, ComCtl32.TTF.TRANSPARENT | ComCtl32.TTF.SUBCLASS, _toolTipText);

        private void OnControlCreateHandle(object sender, EventArgs e) => SetupToolTip((Control)sender);

        private void OnControlDestroyHandle(object sender, EventArgs e)
        {
            if (IsHandleCreated)
            {
                GetTOOLINFO((Control)sender).SendMessage(this, (User32.WM)ComCtl32.TTM.DELTOOLW);
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

        private void SetupToolTip(Control control)
        {
            if (IsHandleCreated)
            {
                PInvoke.SetWindowPos(
                    this,
                    HWND.HWND_TOPMOST,
                    0, 0, 0, 0,
                    SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE);

                ComCtl32.ToolInfoWrapper<Control> info = GetTOOLINFO(control);
                if (info.SendMessage(this, (User32.WM)ComCtl32.TTM.ADDTOOLW) == 0)
                {
                    Debug.Fail($"TTM_ADDTOOL failed for {control.GetType().Name}");
                }

                // Setting the max width has the added benefit of enabling multiline tool tips
                PInvoke.SendMessage(
                    this,
                    (User32.WM)ComCtl32.TTM.SETMAXTIPWIDTH,
                    (WPARAM)0,
                    (LPARAM)SystemInformation.MaxWindowTrackSize.Width);
            }
        }

        public void Reset()
        {
            // This resets the tooltip state, which can get broken when we leave the window
            // then reenter. So we set the tooltip to null, update the text, then it back to
            // what it was, so the tooltip thinks it's back in the regular state again.

            string oldText = ToolTip;
            _toolTipText = string.Empty;

            for (int i = 0; i < _controls.Length; i++)
            {
                ComCtl32.ToolInfoWrapper<Control> info = GetTOOLINFO(_controls[i]);
                info.SendMessage(this, (User32.WM)ComCtl32.TTM.UPDATETIPTEXTW);
            }

            _toolTipText = oldText;
            PInvoke.SendMessage(this, (User32.WM)ComCtl32.TTM.UPDATE);
        }

        protected override void WndProc(ref Message msg)
        {
            switch (msg.MsgInternal)
            {
                case User32.WM.SHOWWINDOW:
                    if ((int)msg.WParamInternal != 0 && _dontShow)
                    {
                        msg.WParamInternal = 0u;
                    }

                    break;
                case User32.WM.NCHITTEST:
                    // When using v6 common controls, the native tooltip does not end up returning HTTRANSPARENT
                    // all the time, so its TTF_TRANSPARENT behavior does not work, ie. mouse events do not fall
                    // thru to controls underneath. This is due to a combination of old app-specific code in comctl32,
                    // functional changes between v5 and v6, and the specific way the property grid drives its tooltip.
                    // Workaround is to just force HTTRANSPARENT all the time.
                    msg.ResultInternal = (LRESULT)(nint)User32.HT.TRANSPARENT;
                    return;
            }

            base.WndProc(ref msg);
        }
    }
}
