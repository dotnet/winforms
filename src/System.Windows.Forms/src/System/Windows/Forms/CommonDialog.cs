// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the base class used for displaying dialog boxes on the screen.
    /// </summary>
    [ToolboxItemFilter("System.Windows.Forms")]
    public abstract class CommonDialog : Component
    {
        private static readonly object s_helpRequestEvent = new object();
        private const int CDM_SETDEFAULTFOCUS = (int)User32.WM.USER + 0x51;
        private static User32.WM s_helpMsg;

        private IntPtr _defOwnerWndProc;

        private IntPtr _hookedWndProc;

        private IntPtr _defaultControlHwnd;

        /// <summary>
        ///  Initializes a new instance of the <see cref='CommonDialog'/> class.
        /// </summary>
        public CommonDialog()
        {
        }

        [SRCategory(nameof(SR.CatData))]
        [Localizable(false)]
        [Bindable(true)]
        [SRDescription(nameof(SR.ControlTagDescr))]
        [DefaultValue(null)]
        [TypeConverter(typeof(StringConverter))]
        public object Tag { get; set; }

        /// <summary>
        ///  Occurs when the user clicks the Help button on a common
        ///  dialog box.
        /// </summary>
        [SRDescription(nameof(SR.CommonDialogHelpRequested))]
        public event EventHandler HelpRequest
        {
            add => Events.AddHandler(s_helpRequestEvent, value);
            remove => Events.RemoveHandler(s_helpRequestEvent, value);
        }

        /// <summary>
        ///  Defines the common dialog box hook procedure that is overridden to add specific
        ///  functionality to a common dialog box.
        /// </summary>
        protected virtual IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            if (msg == (int)User32.WM.INITDIALOG)
            {
                MoveToScreenCenter(hWnd);

                // Under some circumstances, the dialog does not initially focus on any
                // control. We fix that by explicitly setting focus ourselves.
                _defaultControlHwnd = wparam;
                User32.SetFocus(wparam);
            }
            else if (msg == (int)User32.WM.SETFOCUS)
            {
                User32.PostMessageW(hWnd, (User32.WM)CDM_SETDEFAULTFOCUS);
            }
            else if (msg == CDM_SETDEFAULTFOCUS)
            {
                // If the dialog box gets focus, bounce it to the default control.
                // So we post a message back to ourselves to wait for the focus change
                // then push it to the default control.
                User32.SetFocus(new HandleRef(this, _defaultControlHwnd));
            }

            return IntPtr.Zero;
        }

        /// <summary>
        ///  Centers the given window on the screen. This method is used by the default
        ///  common dialog hook procedure to center the dialog on the screen before it
        ///  is shown.
        /// </summary>
        private protected static void MoveToScreenCenter(IntPtr hWnd)
        {
            var r = new RECT();
            User32.GetWindowRect(hWnd, ref r);
            Rectangle screen = Screen.GetWorkingArea(Control.MousePosition);
            int x = screen.X + (screen.Width - r.right + r.left) / 2;
            int y = screen.Y + (screen.Height - r.bottom + r.top) / 3;
            User32.SetWindowPos(
                hWnd,
                User32.HWND_TOP,
                x,
                y,
                flags: User32.SWP.NOSIZE | User32.SWP.NOZORDER | User32.SWP.NOACTIVATE);
        }

        /// <summary>
        ///  Raises the <see cref='HelpRequest'/> event.
        /// </summary>
        protected virtual void OnHelpRequest(EventArgs e)
        {
            EventHandler handler = (EventHandler)Events[s_helpRequestEvent];
            handler?.Invoke(this, e);
        }

        /// <summary>
        ///  Defines the owner window procedure that is overridden to add specific
        ///  functionality to a common dialog box.
        /// </summary>
        protected virtual IntPtr OwnerWndProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            if (msg == (int)s_helpMsg)
            {
                if (NativeWindow.WndProcShouldBeDebuggable)
                {
                    OnHelpRequest(EventArgs.Empty);
                }
                else
                {
                    try
                    {
                        OnHelpRequest(EventArgs.Empty);
                    }
                    catch (Exception e)
                    {
                        Application.OnThreadException(e);
                    }
                }

                return IntPtr.Zero;
            }

            return User32.CallWindowProcW(_defOwnerWndProc, hWnd, (User32.WM)msg, wparam, lparam);
        }

        /// <summary>
        ///  When overridden in a derived class, resets the properties of a common dialog
        ///  to their default values.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        ///  When overridden in a derived class, specifies a common dialog box.
        /// </summary>
        protected abstract bool RunDialog(IntPtr hwndOwner);

        /// <summary>
        ///  Runs a common dialog box.
        /// </summary>
        public DialogResult ShowDialog() => ShowDialog(owner: null);

        /// <summary>
        ///  Runs a common dialog box, parented to the given IWin32Window.
        /// </summary>
        public DialogResult ShowDialog(IWin32Window owner)
        {
            if (!SystemInformation.UserInteractive)
            {
                throw new InvalidOperationException(SR.CantShowModalOnNonInteractive);
            }

            // This will be used if there is no owner or active window.
            // Declared here so it can be kept alive.
            NativeWindow native = null;

            IntPtr hwndOwner = IntPtr.Zero;
            DialogResult result = DialogResult.Cancel;
            try
            {
                if (owner != null)
                {
                    hwndOwner = Control.GetSafeHandle(owner);
                }

                if (hwndOwner == IntPtr.Zero)
                {
                    hwndOwner = User32.GetActiveWindow();
                }

                if (hwndOwner == IntPtr.Zero)
                {
                    // We will have to create our own Window
                    native = new NativeWindow();
                    native.CreateHandle(new CreateParams());
                    hwndOwner = native.Handle;
                }

                if (s_helpMsg == User32.WM.NULL)
                {
                    s_helpMsg = User32.RegisterWindowMessageW("commdlg_help");
                }

                User32.WNDPROCINT ownerProc = new User32.WNDPROCINT(OwnerWndProc);
                _hookedWndProc = Marshal.GetFunctionPointerForDelegate(ownerProc);
                Debug.Assert(IntPtr.Zero == _defOwnerWndProc, "The previous subclass wasn't properly cleaned up");

                IntPtr userCookie = IntPtr.Zero;
                try
                {
                    // UnsafeNativeMethods.[Get|Set]WindowLong is smart enough to call SetWindowLongPtr on 64-bit OS
                    _defOwnerWndProc = User32.SetWindowLong(new HandleRef(this, hwndOwner), User32.GWL.WNDPROC, ownerProc);

                    if (Application.UseVisualStyles)
                    {
                        userCookie = ThemingScope.Activate(Application.UseVisualStyles);
                    }

                    Application.BeginModalMessageLoop();
                    try
                    {
                        result = RunDialog(hwndOwner) ? DialogResult.OK : DialogResult.Cancel;
                    }
                    finally
                    {
                        Application.EndModalMessageLoop();
                    }
                }
                finally
                {
                    IntPtr currentSubClass = User32.GetWindowLong(new HandleRef(this, hwndOwner), User32.GWL.WNDPROC);
                    if (_defOwnerWndProc != IntPtr.Zero || currentSubClass != _hookedWndProc)
                    {
                        User32.SetWindowLong(new HandleRef(this, hwndOwner), User32.GWL.WNDPROC, new HandleRef(this, _defOwnerWndProc));
                    }

                    ThemingScope.Deactivate(userCookie);

                    _defOwnerWndProc = IntPtr.Zero;
                    _hookedWndProc = IntPtr.Zero;
                    // Ensure that the subclass delegate will not be GC collected until
                    // after it has been subclassed
                    GC.KeepAlive(ownerProc);
                }
            }
            finally
            {
                native?.DestroyHandle();
            }

            return result;
        }
    }
}
