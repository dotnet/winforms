// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design.Behavior;
using static Interop;

namespace System.ComponentModel.Design
{
    internal partial class DesignerActionUI
    {
        internal class DesignerActionToolStripDropDown : ToolStripDropDown
        {
            private readonly IWin32Window _mainParentWindow;
            private ToolStripControlHost _panel;
            private readonly DesignerActionUI _designerActionUI;
            private bool _cancelClose;
            private Glyph _relatedGlyph;

            public DesignerActionToolStripDropDown(DesignerActionUI designerActionUI, IWin32Window mainParentWindow)
            {
                _mainParentWindow = mainParentWindow;
                _designerActionUI = designerActionUI;
            }

            public DesignerActionPanel CurrentPanel
                => _panel is not null ? _panel.Control as DesignerActionPanel : null;

            // we're not topmost because we can show modal editors above us.
            protected override bool TopMost
            {
                get => false;
            }

            public void UpdateContainerSize()
            {
                if (CurrentPanel is not null)
                {
                    Size panelSize = CurrentPanel.GetPreferredSize(new Size(150, int.MaxValue));
                    if (CurrentPanel.Size == panelSize)
                    {
                        // If the panel size didn't actually change, we still have to force a call to PerformLayout to make sure that controls get repositioned properly within the panel. The issue arises because we did a measure-only Layout that determined some sizes, and then we end up painting with those values even though there wasn't an actual Layout performed.
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
                Debug.WriteLineIf(DropDownVisibilityDebug.TraceVerbose, "Checking focus...");
                HWND focusedControl = PInvoke.GetFocus();
                if (focusedControl == Handle)
                {
                    Debug.WriteLineIf(DropDownVisibilityDebug.TraceVerbose, "    putting focus on the panel...");
                    _panel.Focus();
                }

                focusedControl = PInvoke.GetFocus();
                if (CurrentPanel is not null && CurrentPanel.Handle == focusedControl)
                {
                    Debug.WriteLineIf(DropDownVisibilityDebug.TraceVerbose, "    selecting next available control on the panel...");
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
                Debug.WriteLineIf(
                    DropDownVisibilityDebug.TraceVerbose,
                    $"_____________________________Begin OnClose {e.CloseReason}");
                Debug.Indent();
                if (e.CloseReason == ToolStripDropDownCloseReason.AppFocusChange && _cancelClose)
                {
                    _cancelClose = false;
                    e.Cancel = true;
                    Debug.WriteLineIf(DropDownVisibilityDebug.TraceVerbose, "cancel close prepopulated");
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
                        Debug.WriteLineIf(
                            DropDownVisibilityDebug.TraceVerbose,
                            "[DesignerActionToolStripDropDown.OnClosing] activation hasnt changed, but we've certainly clicked somewhere else.");
                    }
                    else if (WindowOwnsWindow((HWND)Handle, hwndActivating))
                    {
                        // We're being de-activated for someone owned by the panel.
                        e.Cancel = true;
                        Debug.WriteLineIf(
                            DropDownVisibilityDebug.TraceVerbose,
                            "[DesignerActionToolStripDropDown.OnClosing] Cancel close - the window activating is owned by this window");
                    }
                    else if (_mainParentWindow is not null && !WindowOwnsWindow((HWND)_mainParentWindow.Handle, hwndActivating))
                    {
                        if (IsWindowEnabled(_mainParentWindow.Handle))
                        {
                            // The activated windows is not a child/owned windows of the main top level windows let toolstripdropdown handle this
                            e.Cancel = false;
                            Debug.WriteLineIf(
                                DropDownVisibilityDebug.TraceVerbose,
                                "[DesignerActionToolStripDropDown.OnClosing] Call close: the activated windows is not a child/owned windows of the main top level windows ");
                        }
                        else
                        {
                            e.Cancel = true;
                            Debug.WriteLineIf(
                                DropDownVisibilityDebug.TraceVerbose,
                                "[DesignerActionToolStripDropDown.OnClosing] we're being deactivated by a foreign window, but the main window is not enabled - we should stay up");
                        }

                        base.OnClosing(e);
                        Debug.Unindent();
                        Debug.WriteLineIf(
                            DropDownVisibilityDebug.TraceVerbose,
                            $"_____________________________End OnClose e.Cancel: {e.Cancel}");
                        return;
                    }
                    else
                    {
                        Debug.WriteLineIf(
                            DropDownVisibilityDebug.TraceVerbose,
                            $"[DesignerActionToolStripDropDown.OnClosing] since the designer action panel dropdown doesnt own the activating window {hwndActivating.Value:x)}, calling close. ");
                    }

                    // What's the owner of the windows being activated?
                    HWND parent = (HWND)PInvoke.GetWindowLong(
                        new HandleRef<HWND>(this, hwndActivating),
                        WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT);

                    // is it currently disabled (ie, the activating windows is in modal mode)
                    if (!IsWindowEnabled(parent))
                    {
                        Debug.WriteLineIf(DropDownVisibilityDebug.TraceVerbose, "[DesignerActionToolStripDropDown.OnClosing] modal window activated - cancelling close");
                        // we are in a modal case
                        e.Cancel = true;
                    }
                }

                Debug.WriteLineIf(DropDownVisibilityDebug.TraceVerbose, "[DesignerActionToolStripDropDown.OnClosing] calling base.OnClosing with e.Cancel: " + e.Cancel.ToString());
                base.OnClosing(e);
                Debug.Unindent();
                Debug.WriteLineIf(DropDownVisibilityDebug.TraceVerbose, "_____________________________End OnClose e.Cancel: " + e.Cancel.ToString());
            }

            public void SetDesignerActionPanel(DesignerActionPanel panel, Glyph relatedGlyph)
            {
                if (_panel is not null && panel == (DesignerActionPanel)_panel.Control)
                {
                    return;
                }

                Debug.Assert(relatedGlyph is not null, "related glyph cannot be null");
                _relatedGlyph = relatedGlyph;
                panel.SizeChanged += new EventHandler(PanelResized);
                // hook up the event
                if (_panel is not null)
                {
                    Items.Remove(_panel);
                    _panel.Dispose();
                    _panel = null;
                }

                _panel = new ToolStripControlHost(panel)
                {
                    // we don't want no margin
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

            private void PanelResized(object sender, EventArgs e)
            {
                Control ctrl = sender as Control;
                if (Size.Width != ctrl.Size.Width || Size.Height != ctrl.Size.Height)
                {
                    SuspendLayout();
                    Size = ctrl.Size;
                    if (_panel is not null)
                    {
                        _panel.Size = ctrl.Size;
                    }

                    _designerActionUI.UpdateDAPLocation(null, _relatedGlyph as DesignerActionGlyph);
                    ResumeLayout();
                }
            }

            protected override void SetVisibleCore(bool visible)
            {
                Debug.WriteLineIf(DropDownVisibilityDebug.TraceVerbose, "[DesignerActionToolStripDropDown.SetVisibleCore] setting dropdown visible=" + visible.ToString());
                base.SetVisibleCore(visible);
                if (visible)
                {
                    CheckFocusIsRight();
                }
            }

            /// <summary>
            ///  General purpose method, based on Control.Contains()...
            ///  Determines whether a given window (specified using native window handle) is a descendant of this control. This catches both contained descendants and 'owned' windows such as modal dialogs. Using window handles rather than Control objects allows it to catch un-managed windows as well.
            /// </summary>
            private static bool WindowOwnsWindow(HWND hWndOwner, HWND hWndDescendant)
            {
                Debug.WriteLineIf(
                    DropDownVisibilityDebug.TraceVerbose,
                    $"[WindowOwnsWindow] Testing if {hWndOwner.Value.ToString("x")} is a owned by {hWndDescendant.Value.ToString("x")}... ");
#if DEBUG
                if (DropDownVisibilityDebug.TraceVerbose)
                {
                    Debug.WriteLine("\t\tOWNER: " + GetControlInformation(hWndOwner));
                    Debug.WriteLine("\t\tOWNEE: " + GetControlInformation(hWndDescendant));
                    IntPtr claimedOwnerHwnd = PInvoke.GetWindowLong(hWndDescendant, WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT);
                    Debug.WriteLine("OWNEE's CLAIMED OWNER: " + GetControlInformation(claimedOwnerHwnd));
                }
#endif
                if (hWndDescendant == hWndOwner)
                {
                    Debug.WriteLineIf(DropDownVisibilityDebug.TraceVerbose, "they match, YES.");
                    return true;
                }

                while (!hWndDescendant.IsNull)
                {
                    hWndDescendant = (HWND)PInvoke.GetWindowLong(hWndDescendant, WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT);
                    if (hWndDescendant == IntPtr.Zero)
                    {
                        Debug.WriteLineIf(DropDownVisibilityDebug.TraceVerbose, "NOPE.");
                        return false;
                    }

                    if (hWndDescendant == hWndOwner)
                    {
                        Debug.WriteLineIf(DropDownVisibilityDebug.TraceVerbose, "YES.");
                        return true;
                    }
                }

                Debug.WriteLineIf(DropDownVisibilityDebug.TraceVerbose, "NO.");
                return false;
            }

            // helper function for generating infomation about a particular control use AssertControlInformation if sticking in an assert - then the work to figure out the control info will only be done when the assertion is false.
            internal static string GetControlInformation(IntPtr hwnd)
            {
                if (hwnd == IntPtr.Zero)
                {
                    return "Handle is IntPtr.Zero";
                }
#if DEBUG
                if (!DropDownVisibilityDebug.TraceVerbose)
                {
                    return string.Empty;
                }

                string windowText = User32.GetWindowText(hwnd);
                string typeOfControl = "Unknown";
                string nameOfControl = string.Empty;
                Control c = FromHandle(hwnd);
                if (c is not null)
                {
                    typeOfControl = c.GetType().Name;
                    if (!string.IsNullOrEmpty(c.Name))
                    {
                        nameOfControl += c.Name;
                    }
                    else
                    {
                        nameOfControl += "Unknown";
                        // some extra debug info for toolstripdropdowns...
                        if (c is ToolStripDropDown dd)
                        {
                            if (dd.OwnerItem is not null)
                            {
                                nameOfControl += "OwnerItem: [" + dd.OwnerItem.ToString() + "]";
                            }
                        }
                    }
                }

                return windowText + "\r\n\t\t\tType: [" + typeOfControl + "] Name: [" + nameOfControl + "]";
#else
            return string.Empty;
#endif

            }

            private bool IsWindowEnabled(IntPtr handle)
            {
                int style = (int)PInvoke.GetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
                return (style & (int)WINDOW_STYLE.WS_DISABLED) == 0;
            }

            private void WmActivate(ref Message m)
            {
                if ((User32.WA)(nint)m.WParamInternal == User32.WA.INACTIVE)
                {
                    HWND hwndActivating = (HWND)m.LParamInternal;
                    if (WindowOwnsWindow((HWND)Handle, hwndActivating))
                    {
                        Debug.WriteLineIf(DropDownVisibilityDebug.TraceVerbose, "[DesignerActionUI WmActivate] setting cancel close true because WindowsOwnWindow");
                        Debug.WriteLineIf(DropDownVisibilityDebug.TraceVerbose, "[DesignerActionUI WmActivate] checking the focus... " + GetControlInformation(PInvoke.GetFocus()));
                        _cancelClose = true;
                    }
                    else
                    {
                        _cancelClose = false;
                    }
                }
                else
                {
                    _cancelClose = false;
                }

                base.WndProc(ref m);
            }

            protected override void WndProc(ref Message m)
            {
                switch ((User32.WM)m.Msg)
                {
                    case User32.WM.ACTIVATE:
                        WmActivate(ref m);
                        return;
                }

                base.WndProc(ref m);
            }

            protected override bool ProcessDialogKey(Keys keyData)
            {
                // since we're not hosted in a form we need to do the same logic as Form.cs. If we get an enter key we need to find the current focused control. if it's a button, we click it and return that we handled the message
                if (keyData == Keys.Enter)
                {
                    HWND focusedControlPtr = PInvoke.GetFocus();
                    Control focusedControl = FromChildHandle(focusedControlPtr);
                    if (focusedControl is IButtonControl button && button is Control)
                    {
                        button.PerformClick();
                        return true;
                    }
                }

                return base.ProcessDialogKey(keyData);
            }
        }
    }
}
