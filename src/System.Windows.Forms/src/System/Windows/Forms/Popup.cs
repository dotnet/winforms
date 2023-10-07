// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

public delegate void PopupCloseRequestEventHandler(PopupCloseRequestEventArgs e);

[ToolboxItem(false)]
public partial class Popup : Panel, IMessageFilter
{
    private bool _isVisible;

    public event EventHandler? PopupOpening;

    public event EventHandler? PopupOpened;

    public event EventHandler? PopupClosed;

    public event EventHandler? PopupClosing;

    public event PopupCloseRequestEventHandler? PopupCloseRequested;

    private delegate bool SetForegroundWindowDelegate(IntPtr hWnd);

    private Control? _popupContentControl;
    private bool _canResize;
    private Size _originalHostingControlSize;
    private Control? _lastKnownAssociatingControl;

    public Popup()
    {
        base.AutoSize = false;
        AutoSize = true;
        BackColor = Color.White;
        BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        SetStyle(ControlStyles.ResizeRedraw, true);
        SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
    }

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= (int) WINDOW_EX_STYLE.WS_EX_NOACTIVATE;
            cp.Parent = IntPtr.Zero;
            return cp;
        }
    }

    public void OpenPopup(Control associatingControl)
    {
        _lastKnownAssociatingControl = associatingControl;

        PopupOpeningEventArgs e = new PopupOpeningEventArgs(false, Size.Empty);
        OnPopupOpening(e);

        if (e.Cancel)
        {
            return;
        }

        Show(associatingControl);

        OnPopupOpened(EventArgs.Empty);
    }

    public void ClosePopup()
    {
        ClosePopupInternally(new PopupCloseRequestEventArgs(PopupCloseRequestReason.CloseMethodInvoked));
    }

    internal void ClosePopupInternally(PopupCloseRequestEventArgs ClosingReason)
    {
        if (ClosingReason.ClosingRequestOrigin == PopupCloseRequestOrigin.ExternalByUser)
        {
            ClosingReason.ClosingRequestOrigin = PopupCloseRequestOrigin.InternalByComponent;
            OnPopupClosing(ClosingReason);
        }

        if (!IsOpen)
        {
            return;
        }

        if (!ClosingReason.Cancel)
        {
            Hide();
            OnPopupClosed(EventArgs.Empty);
        }
    }

    private void Show(Control referringControl)
    {
        Location = GetPopupLocation(referringControl, Size);

        PInvoke.SetParent((HWND)Handle, HWND.Null);
        PInvoke.ShowWindow((HWND)Handle, (SHOW_WINDOW_CMD)1);
        PInvoke.SetForegroundWindow((HWND)Handle);

        Application.AddMessageFilter(this);

        _isVisible = true;
    }

    private new void Hide()
    {
        if (!_isVisible)
        {
            return;
        }

        PInvoke.ShowWindow((HWND)Handle, (SHOW_WINDOW_CMD)0);

        _isVisible = false;
    }

    bool IMessageFilter.PreFilterMessage(ref Message m)
    {
        // Check if the control is disposed
        if (IsDisposed)
        {
            return false;
        }

        // Handle mouse left-button down events inside the control hierarchy
        if (_lastKnownAssociatingControl is not null
            && m.Msg == PInvoke.WM_LBUTTONDOWN
            && IsHandleInHierarchy(_lastKnownAssociatingControl, m.HWnd)
            && IsOpen)
        {
            return false;
        }

        // Handle the Escape key to close the popup
        if (m.Msg == PInvoke.WM_KEYDOWN && m.WParamInternal == (int)Keys.Escape && IsOpen)
        {
            var e = new PopupCloseRequestEventArgs(PopupCloseRequestReason.Keyboard)
            {
                KeyData = Keys.Escape
            };

            PopupCloseRequested?.Invoke(e);
            return true;
        }

        // Handle the Enter key to commit the popup
        if (m.Msg == PInvoke.WM_KEYDOWN && m.WParamInternal == (int)Keys.Return && IsOpen)
        {
            var e = new PopupCloseRequestEventArgs(PopupCloseRequestReason.Keyboard)
            {
                KeyData = Keys.Return
            };

            PopupCloseRequested?.Invoke(e);
            return true;
        }

        // Handle the Tab key for navigation
        if (m.Msg == PInvoke.WM_KEYDOWN && m.WParamInternal == (int)Keys.Tab && IsOpen)
        {
            var e = new PopupCloseRequestEventArgs(PopupCloseRequestReason.Keyboard)
            {
                KeyData = Keys.Tab
            };
            PopupCloseRequested?.Invoke(e);

            return false;
        }

        PopupCloseRequestEventArgs? popupClosingEventArgs = null;

        // Handle if the application loses focus
        if (_isVisible && (Form.ActiveForm is null || Form.ActiveForm.Equals(this)))
        {
            popupClosingEventArgs = new PopupCloseRequestEventArgs(PopupCloseRequestReason.AppLostFocus);
        }

        // Handle mouse clicks outside of this control hierarchy
        if ((m.Msg == PInvoke.WM_LBUTTONDOWN || m.Msg == PInvoke.WM_NCLBUTTONDOWN)
            && !IsHandleInHierarchy(m.HWnd))
        {
            popupClosingEventArgs = new PopupCloseRequestEventArgs(PopupCloseRequestReason.PopupLostFocus);
        }

        // Trigger the closing event if applicable
        if (popupClosingEventArgs is not null)
        {
            if (IsOpen)
            {
                OnPopupClosing(popupClosingEventArgs);
            }
        }

        return false;
    }

    internal bool IsHandleInHierarchy(IntPtr hwnd)
    {
        return IsHandleInHierarchy(this, hwnd);
    }

    internal static bool IsHandleInHierarchy(Control? ctrl, IntPtr hwnd)
    {
        if (ctrl is null)
        {
            return false;
        }

        if (hwnd == ctrl.Handle)
        {
            return true;
        }

        IntPtr currentParentHwnd = hwnd;
        IntPtr parentHwnd = PInvoke.GetParent((HWND)currentParentHwnd);

        if (parentHwnd == IntPtr.Zero)
        {
            return false;
        }

        do
        {
            parentHwnd = PInvoke.GetParent((HWND)currentParentHwnd);

            if (parentHwnd == ctrl.Handle)
            {
                return true;
            }

            if (parentHwnd == IntPtr.Zero)
            {
                return false;
            }

            currentParentHwnd = parentHwnd;
        }

        while (true);
    }

    private Rectangle BottomGripArea
    {
        get
        {
            Rectangle rect = ClientRectangle;
            rect.Y = rect.Bottom - 4;
            rect.Height = 4;
            return rect;
        }
    }

    private Rectangle RightGripArea
    {
        get
        {
            Rectangle rect = this.ClientRectangle;
            rect.X = rect.Width - 10;
            rect.Width = 10;
            return rect;
        }
    }

    protected override void WndProc(ref Message m)
    {
        if (CanResize)
        {
            if (m.Msg == PInvoke.WM_GETMINMAXINFO)
            {
                var nullableMinMaxInfo = Marshal.PtrToStructure(m.LParamInternal, typeof(MINMAXINFO));

                if (nullableMinMaxInfo is not null)
                {
                    MINMAXINFO minMaxInfo = (MINMAXINFO)nullableMinMaxInfo;
                    minMaxInfo.ptMinTrackSize.X = MinimumSize.Width;
                    minMaxInfo.ptMinTrackSize.Y = MinimumSize.Height;

                    if (!(MaximumSize.Width == 0 && MaximumSize.Height == 0))
                    {
                        minMaxInfo.ptMaxTrackSize.X = MaximumSize.Width;
                        minMaxInfo.ptMaxTrackSize.Y = MaximumSize.Height;
                    }

                    Marshal.StructureToPtr(minMaxInfo, m.LParamInternal, true);
                }
            }
            else if (m.Msg == PInvoke.WM_NCHITTEST)
            {
                int x = m.LParamInternal.LOWORD;
                int y = m.LParamInternal.HIWORD;

                Point cLoc = PointToClient(new Point(x, y));

                if (BottomGripArea.Contains(cLoc) && !RightGripArea.Contains(cLoc))
                {
                    m.ResultInternal = new LRESULT((nint)PInvoke.HTBOTTOM);
                    return;
                }

                if (RightGripArea.Contains(cLoc))
                {
                    m.ResultInternal = new LRESULT((nint)PInvoke.HTBOTTOMRIGHT);
                    return;
                }
            }
        }

        base.WndProc(ref m);
    }

    private void DoLayout()
    {
        if (AutoSize)
        {
            if (_popupContentControl is not null)
            {
                Size tmpSize = Size.Add(_originalHostingControlSize, new Size(1, 1));

                if (CanResize)
                {
                    tmpSize = Size.Add(tmpSize, new Size(0, 22));
                }

                Size = tmpSize;
            }
        }
    }

    public bool CanResize
    {
        get => _canResize;

        set
        {
            AutoSize = !value;
            _canResize = value;
        }
    }

    public Control? PopupContent
    {
        get => _popupContentControl;

        set
        {
            if (_popupContentControl == value)
            {
                return;
            }

            _popupContentControl = value;

            if (_popupContentControl is null)
            {
                return;
            }

            _originalHostingControlSize = _popupContentControl.Size;

            if (typeof(Form).IsAssignableFrom(_popupContentControl.GetType()))
            {
                ((Form)_popupContentControl).TopLevel = false;
                _popupContentControl.Dock = DockStyle.Fill;
            }

            SuspendLayout();
            Controls.Add(PopupContent);
            ResumeLayout(true);
        }
    }

    public bool IsOpen => _isVisible;

    protected virtual void OnPopupOpening(PopupOpeningEventArgs e)
    {
        PopupOpening?.Invoke(this, e);

        if (e.Cancel)
        {
            return;
        }

        if (!e.PreferredNewSize.IsEmpty)
        {
            Size = e.PreferredNewSize;
            if (e.PreventResizing)
            {
                MaximumSize = e.PreferredNewSize;
                MinimumSize = e.PreferredNewSize;
            }
        }
    }

    private void OnPopupOpened(EventArgs e)
        => PopupOpened?.Invoke(this, e);

    protected virtual void OnPopupClosing(PopupCloseRequestEventArgs e)
        => PopupClosing?.Invoke(this, e);

    protected virtual void OnPopupClosed(EventArgs e)
    {
        PopupClosed?.Invoke(this, e);
        _lastKnownAssociatingControl = null;
    }

    public static Point GetPopupLocation(Control referringControl, Size popupSize)
    {
        Point screenCoordinates;

        if (referringControl.Parent is null)
        {
            throw new ArgumentNullException("Control must be assigned to a Form when calculating the screen coordinates for its popup position");
        }

        screenCoordinates = referringControl.Parent.PointToScreen(referringControl.Location);
        screenCoordinates += new Size(0, referringControl.Height);

        var currentScreen = Screen.FromPoint(screenCoordinates).WorkingArea;
        Rectangle tmpRec = new Rectangle(screenCoordinates, popupSize);

        if (currentScreen.Y + currentScreen.Height < tmpRec.Bottom)
        {
            screenCoordinates = referringControl.Parent.PointToScreen(referringControl.Location);
            screenCoordinates -= new Size(0, popupSize.Height);
        }

        if (currentScreen.X + currentScreen.Width < tmpRec.Right)
        {
            screenCoordinates -= new Size(popupSize.Width - referringControl.Width, 0);
        }

        return screenCoordinates;
    }
}
