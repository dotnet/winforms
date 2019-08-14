// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal class GridToolTip : Control
    {
        readonly Control[] controls;
        string toolTipText;
        bool dontShow;
        private readonly int maximumToolTipLength = 1000;

        internal GridToolTip(Control[] controls)
        {
            this.controls = controls;
            SetStyle(ControlStyles.UserPaint, false);
            Font = controls[0].Font;

            for (int i = 0; i < controls.Length; i++)
            {
                controls[i].HandleCreated += new EventHandler(OnControlCreateHandle);
                controls[i].HandleDestroyed += new EventHandler(OnControlDestroyHandle);

                if (controls[i].IsHandleCreated)
                {
                    SetupToolTip(controls[i]);
                }
            }
        }

        public string ToolTip
        {
            get
            {
                return toolTipText;
            }
            set
            {
                if (IsHandleCreated || !string.IsNullOrEmpty(value))
                {
                    Reset();
                }

                if (value != null && value.Length > maximumToolTipLength)
                {
                    //Let the user know the text was truncated by throwing on an ellipsis
                    value = value.Substring(0, maximumToolTipLength) + "...";
                }

                toolTipText = value;

                if (IsHandleCreated)
                {
                    bool visible = Visible;

                    if (visible)
                    {
                        Visible = false;
                    }

                    // Here's a workaround.  If we give the tooltip an empty string, it won't come back
                    // so we just force it hidden instead.
                    dontShow = string.IsNullOrEmpty(value);

                    for (int i = 0; i < controls.Length; i++)
                    {
                        ComCtl32.ToolInfoWrapper info = GetTOOLINFO(controls[i]);
                        info.SendMessage(this, WindowMessages.TTM_UPDATETIPTEXTW);
                    }

                    if (visible && !dontShow)
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
                NativeMethods.INITCOMMONCONTROLSEX icc = new NativeMethods.INITCOMMONCONTROLSEX
                {
                    dwICC = NativeMethods.ICC_TAB_CLASSES
                };
                SafeNativeMethods.InitCommonControlsEx(icc);
                CreateParams cp = new CreateParams
                {
                    Parent = IntPtr.Zero,
                    ClassName = NativeMethods.TOOLTIPS_CLASS
                };
                cp.Style |= (NativeMethods.TTS_ALWAYSTIP | NativeMethods.TTS_NOPREFIX);
                cp.ExStyle = 0;
                cp.Caption = ToolTip;
                return cp;
            }
        }

        private ComCtl32.ToolInfoWrapper GetTOOLINFO(Control c)
            => new ComCtl32.ToolInfoWrapper(c, ComCtl32.TTF.TRANSPARENT | ComCtl32.TTF.SUBCLASS, toolTipText);

        private void OnControlCreateHandle(object sender, EventArgs e)
        {
            SetupToolTip((Control)sender);
        }

        private void OnControlDestroyHandle(object sender, EventArgs e)
        {
            if (IsHandleCreated)
            {
                GetTOOLINFO((Control)sender).SendMessage(this, WindowMessages.TTM_DELTOOLW);
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            for (int i = 0; i < controls.Length; i++)
            {
                if (controls[i].IsHandleCreated)
                {
                    SetupToolTip(controls[i]);
                }
            }
        }

        private void SetupToolTip(Control control)
        {
            if (IsHandleCreated)
            {
                SafeNativeMethods.SetWindowPos(new HandleRef(this, Handle), NativeMethods.HWND_TOPMOST,
                                  0, 0, 0, 0,
                                  NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE |
                                  NativeMethods.SWP_NOACTIVATE);

                ComCtl32.ToolInfoWrapper info = GetTOOLINFO(control);
                if (info.SendMessage(this, WindowMessages.TTM_ADDTOOLW) == IntPtr.Zero)
                {
                    Debug.Fail($"TTM_ADDTOOL failed for {control.GetType().Name}");
                }

                // Setting the max width has the added benefit of enabling multiline tool tips
                User32.SendMessageW(this, WindowMessages.TTM_SETMAXTIPWIDTH, IntPtr.Zero, (IntPtr)SystemInformation.MaxWindowTrackSize.Width);
            }
        }

        public void Reset()
        {
            // This resets the tooltip state, which can get broken when we leave the window
            // then reenter. So we set the tooltip to null, update the text, then it back to
            // what it was, so the tooltip thinks it's back in the regular state again

            string oldText = ToolTip;
            toolTipText = string.Empty;

            for (int i = 0; i < controls.Length; i++)
            {
                ComCtl32.ToolInfoWrapper info = GetTOOLINFO(controls[i]);
                if (info.SendMessage(this, WindowMessages.TTM_UPDATETIPTEXTW) == IntPtr.Zero)
                {
                    // Debug.Fail("TTM_UPDATETIPTEXT failed for " + controls[i].GetType().Name);
                }
            }

            toolTipText = oldText;
            User32.SendMessageW(this, WindowMessages.TTM_UPDATE);
        }

        protected override void WndProc(ref Message msg)
        {
            switch (msg.Msg)
            {
                case WindowMessages.WM_SHOWWINDOW:
                    if (unchecked((int)(long)msg.WParam) != 0 && dontShow)
                    {
                        msg.WParam = IntPtr.Zero;
                    }
                    break;
                case WindowMessages.WM_NCHITTEST:
                    // When using v6 common controls, the native tooltip does not end up returning HTTRANSPARENT
                    // all the time, so its TTF_TRANSPARENT behavior does not work, ie. mouse events do not fall
                    // thru to controls underneath. This is due to a combination of old app-specific code in comctl32,
                    // functional changes between v5 and v6, and the specfic way the property grid drives its tooltip.
                    // Workaround is to just force HTTRANSPARENT all the time.
                    msg.Result = (IntPtr)NativeMethods.HTTRANSPARENT;
                    return;
            }
            base.WndProc(ref msg);
        }
    }
}
