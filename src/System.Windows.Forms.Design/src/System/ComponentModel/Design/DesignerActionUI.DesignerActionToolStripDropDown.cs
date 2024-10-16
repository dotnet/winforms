// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design.Behavior;

namespace System.ComponentModel.Design;

internal partial class DesignerActionUI
{
    internal class DesignerActionToolStripDropDown : ToolStripDropDown
    {
        private readonly IWin32Window? _mainParentWindow;
        private ToolStripControlHost? _panel;
        private readonly DesignerActionUI _designerActionUI;
        private bool _cancelClose;
        private Glyph? _relatedGlyph;

        public DesignerActionToolStripDropDown(DesignerActionUI designerActionUI, IWin32Window? mainParentWindow)
        {
            _mainParentWindow = mainParentWindow;
            _designerActionUI = designerActionUI;
        }

        public DesignerActionPanel? CurrentPanel => _panel?.Control as DesignerActionPanel;

        // we're not topmost because we can show modal editors above us.
        protected override bool TopMost => false;

        public void UpdateContainerSize()
        {
            if (CurrentPanel is not null)
            {
                Size panelSize = CurrentPanel.GetPreferredSize(new Size(150, int.MaxValue));
                if (CurrentPanel.Size == panelSize)
                {
                    // If the panel size didn't actually change, we still have to force a call to PerformLayout to make
                    // sure that controls get repositioned properly within the panel. The issue arises because we did a
                    // measure-only Layout that determined some sizes, and then we end up painting with those values even
                    // though there wasn't an actual Layout performed.
                    CurrentPanel.PerformLayout();
                }
                else
                {
                    CurrentPanel.Size = panelSize;
                }

                ClientSize = panelSize;
            }
        }

        public void CheckFocusIsRight()
        {
            // fix to get the focus to NOT stay on ContainerControl
            HWND focusedControl = PInvoke.GetFocus();
            if (focusedControl == Handle)
            {
                _panel?.Focus();
            }

            focusedControl = PInvoke.GetFocus();
            if (CurrentPanel is not null && CurrentPanel.Handle == focusedControl)
            {
                CurrentPanel.SelectNextControl(null, true, true, true, true);
            }
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            UpdateContainerSize();
        }

        protected override void OnClosing(ToolStripDropDownClosingEventArgs e)
        {
            if (e.CloseReason == ToolStripDropDownCloseReason.AppFocusChange && _cancelClose)
            {
                _cancelClose = false;
                e.Cancel = true;
            }

            // When we get closing event as a result of an activation change, pre-populate e.Cancel based on why we're exiting.
            // - if it's a modal window that's owned by VS don't exit
            // - if it's a window that's owned by the toolstrip dropdown don't exit
            else if (e.CloseReason is ToolStripDropDownCloseReason.AppFocusChange or ToolStripDropDownCloseReason.AppClicked)
            {
                HWND hwndActivating = PInvoke.GetActiveWindow();
                if (Handle == hwndActivating && e.CloseReason == ToolStripDropDownCloseReason.AppClicked)
                {
                    e.Cancel = false;
                }
                else if (WindowOwnsWindow((HWND)Handle, hwndActivating))
                {
                    // We're being de-activated for someone owned by the panel.
                    e.Cancel = true;
                }
                else if (_mainParentWindow is not null && !WindowOwnsWindow((HWND)_mainParentWindow.Handle, hwndActivating))
                {
                    if (IsWindowEnabled(_mainParentWindow.Handle))
                    {
                        // The activated windows is not a child/owned windows of the main top level windows let toolstripdropdown handle this
                        e.Cancel = false;
                    }
                    else
                    {
                        e.Cancel = true;
                    }

                    base.OnClosing(e);
                    return;
                }

                // What's the owner of the windows being activated?
                HWND parent = (HWND)PInvokeCore.GetWindowLong(
                    new HandleRef<HWND>(this, hwndActivating),
                    WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT);

                // is it currently disabled (ie, the activating windows is in modal mode)
                if (!IsWindowEnabled(parent))
                {
                    // We are in a modal case
                    e.Cancel = true;
                }
            }

            base.OnClosing(e);
        }

        public void SetDesignerActionPanel(DesignerActionPanel panel, Glyph relatedGlyph)
        {
            if (_panel is not null && panel == (DesignerActionPanel)_panel.Control)
            {
                return;
            }

            Debug.Assert(relatedGlyph is not null, "related glyph cannot be null");
            _relatedGlyph = relatedGlyph;
            panel.SizeChanged += PanelResized;

            if (_panel is not null)
            {
                Items.Remove(_panel);
                _panel.Dispose();
                _panel = null;
            }

            _panel = new ToolStripControlHost(panel)
            {
                // We don't want a margin
                Margin = Padding.Empty,
                Size = panel.Size
            };

            SuspendLayout();
            Size = panel.Size;
            Items.Add(_panel);
            ResumeLayout();
            if (Visible)
            {
                CheckFocusIsRight();
            }
        }

        private void PanelResized(object? sender, EventArgs e)
        {
            Control ctrl = (Control)sender!;
            if (Size.Width != ctrl.Size.Width || Size.Height != ctrl.Size.Height)
            {
                SuspendLayout();
                Size = ctrl.Size;
                if (_panel is not null)
                {
                    _panel.Size = ctrl.Size;
                }

                _designerActionUI.UpdateDAPLocation(component: null, _relatedGlyph as DesignerActionGlyph);
                ResumeLayout();
            }
        }

        protected override void SetVisibleCore(bool visible)
        {
            base.SetVisibleCore(visible);
            if (visible)
            {
                CheckFocusIsRight();
            }
        }

        /// <summary>
        ///  Determines whether a given window (specified using native window handle) is a descendant of this control.
        ///  This catches both contained descendants and 'owned' windows such as modal dialogs. Using window handles
        ///  rather than Control objects allows it to catch un-managed windows as well.
        /// </summary>
        private static bool WindowOwnsWindow(HWND hWndOwner, HWND hWndDescendant)
        {
            if (hWndDescendant == hWndOwner)
            {
                return true;
            }

            while (!hWndDescendant.IsNull)
            {
                hWndDescendant = (HWND)PInvokeCore.GetWindowLong(hWndDescendant, WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT);
                if (hWndDescendant.IsNull)
                {
                    return false;
                }

                if (hWndDescendant == hWndOwner)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsWindowEnabled(IntPtr handle)
        {
            int style = (int)PInvokeCore.GetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
            return (style & (int)WINDOW_STYLE.WS_DISABLED) == 0;
        }

        private void WmActivate(ref Message m)
        {
            if ((nint)m.WParamInternal == PInvoke.WA_INACTIVE)
            {
                HWND hwndActivating = (HWND)m.LParamInternal;
                _cancelClose = WindowOwnsWindow((HWND)Handle, hwndActivating);
            }
            else
            {
                _cancelClose = false;
            }

            base.WndProc(ref m);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.MsgInternal)
            {
                case PInvokeCore.WM_ACTIVATE:
                    WmActivate(ref m);
                    return;
            }

            base.WndProc(ref m);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            // since we're not hosted in a form we need to do the same logic as Form.cs.
            // If we get an enter key we need to find the current focused control.
            // if it's a button, we click it and return that we handled the message
            if (keyData == Keys.Enter)
            {
                HWND focusedControlPtr = PInvoke.GetFocus();
                Control? focusedControl = FromChildHandle(focusedControlPtr);
                if (focusedControl is IButtonControl button)
                {
                    button.PerformClick();
                    return true;
                }
            }

            return base.ProcessDialogKey(keyData);
        }
    }
}
