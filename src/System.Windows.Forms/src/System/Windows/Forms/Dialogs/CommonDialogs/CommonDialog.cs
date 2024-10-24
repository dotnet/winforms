// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

/// <summary>
///  Specifies the base class used for displaying dialog boxes on the screen.
/// </summary>
[ToolboxItemFilter("System.Windows.Forms")]
public abstract class CommonDialog : Component
{
    private static readonly object s_helpRequestEvent = new();
    private const int CDM_SETDEFAULTFOCUS = (int)PInvokeCore.WM_USER + 0x51;
    private static MessageId s_helpMessage;

    private nint _priorWindowProcedure;
    private HWND _defaultControlHwnd;
    private readonly WNDPROC _hookProc;
    private readonly unsafe delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, nuint> _functionPointer;

    /// <summary>
    ///  Initializes a new instance of the <see cref="CommonDialog"/> class.
    /// </summary>
    public unsafe CommonDialog()
    {
        // Keep the delegate in a field to avoid having it collected prematurely.
        _hookProc = HookProcInternal;
        _functionPointer = (delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, nuint>)(void*)Marshal.GetFunctionPointerForDelegate(_hookProc);
    }

    [SRCategory(nameof(SR.CatData))]
    [Localizable(false)]
    [Bindable(true)]
    [SRDescription(nameof(SR.ControlTagDescr))]
    [DefaultValue(null)]
    [TypeConverter(typeof(StringConverter))]
    public object? Tag { get; set; }

    /// <summary>
    ///  Occurs when the user clicks the Help button on a common dialog box.
    /// </summary>
    [SRDescription(nameof(SR.CommonDialogHelpRequested))]
    public event EventHandler? HelpRequest
    {
        add => Events.AddHandler(s_helpRequestEvent, value);
        remove => Events.RemoveHandler(s_helpRequestEvent, value);
    }

    internal LRESULT HookProcInternal(HWND hWnd, uint msg, WPARAM wparam, LPARAM lparam)
        => (LRESULT)HookProc(hWnd, (int)msg, (nint)wparam, lparam);

    private protected unsafe delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, nuint> HookProcFunctionPointer
        => _functionPointer;

    /// <summary>
    ///  Defines the common dialog box hook procedure that is overridden to add specific
    ///  functionality to a common dialog box.
    /// </summary>
    protected virtual IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
    {
        if (msg == (int)PInvokeCore.WM_INITDIALOG)
        {
            MoveToScreenCenter((HWND)hWnd);

            // Under some circumstances, the dialog does not initially focus on any
            // control. We fix that by explicitly setting focus ourselves.
            _defaultControlHwnd = (HWND)wparam;
            PInvoke.SetFocus((HWND)wparam);
        }
        else if (msg == (int)PInvokeCore.WM_SETFOCUS)
        {
            PInvokeCore.PostMessage((HWND)hWnd, CDM_SETDEFAULTFOCUS);
        }
        else if (msg == CDM_SETDEFAULTFOCUS)
        {
            // If the dialog box gets focus, bounce it to the default control.
            // So we post a message back to ourselves to wait for the focus change
            // then push it to the default control.
            PInvoke.SetFocus(_defaultControlHwnd);
        }

        return IntPtr.Zero;
    }

    /// <summary>
    ///  Centers the given window on the screen. This method is used by the default
    ///  common dialog hook procedure to center the dialog on the screen before it
    ///  is shown.
    /// </summary>
    private protected static void MoveToScreenCenter(HWND hwnd)
    {
        PInvokeCore.GetWindowRect(hwnd, out var r);
        Rectangle screen = Screen.GetWorkingArea(Control.MousePosition);
        int x = screen.X + (screen.Width - r.right + r.left) / 2;
        int y = screen.Y + (screen.Height - r.bottom + r.top) / 3;
        PInvoke.SetWindowPos(
            hwnd,
            HWND.HWND_TOP,
            x, y, 0, 0,
            SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE);
    }

    /// <summary>
    ///  Raises the <see cref="HelpRequest"/> event.
    /// </summary>
    protected virtual void OnHelpRequest(EventArgs e)
    {
        EventHandler? handler = (EventHandler?)Events[s_helpRequestEvent];
        handler?.Invoke(this, e);
    }

    private LRESULT OwnerWndProcInternal(HWND hWnd, uint msg, WPARAM wparam, LPARAM lparam)
        => (LRESULT)OwnerWndProc(hWnd, (int)msg, (nint)wparam, lparam);

    /// <summary>
    ///  Defines the owner window procedure that is overridden to add specific functionality to a common dialog box.
    /// </summary>
    protected virtual unsafe IntPtr OwnerWndProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
    {
        if (msg == (int)s_helpMessage)
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

        return PInvokeCore.CallWindowProc((void*)_priorWindowProcedure, (HWND)hWnd, (uint)msg, (nuint)wparam, lparam);
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
    public DialogResult ShowDialog(IWin32Window? owner)
    {
        if (!SystemInformation.UserInteractive)
        {
            throw new InvalidOperationException(SR.CantShowModalOnNonInteractive);
        }

        // This will be used if there is no owner or active window.
        // Declared here so it can be kept alive.
        NativeWindow? nativeWindow = null;

        HandleRef<HWND> ownerHwnd = default;
        DialogResult result = DialogResult.Cancel;
        try
        {
            if (owner is not null)
            {
                ownerHwnd = Control.GetSafeHandle(owner);
            }

            if (ownerHwnd.IsNull)
            {
                ownerHwnd = Control.GetHandleRef(PInvoke.GetActiveWindow());
            }

            if (ownerHwnd.IsNull)
            {
                // We will have to create our own Window
                nativeWindow = new NativeWindow();
                nativeWindow.CreateHandle(new CreateParams());
                ownerHwnd = new(nativeWindow, nativeWindow.HWND);
            }

            if (s_helpMessage == PInvokeCore.WM_NULL)
            {
                s_helpMessage = PInvoke.RegisterWindowMessage("commdlg_help");
            }

            WNDPROC ownerWindowProcedure = OwnerWndProcInternal;
            nint hookedWndProc = Marshal.GetFunctionPointerForDelegate(ownerWindowProcedure);
            Debug.Assert(_priorWindowProcedure == 0, "The previous subclass wasn't properly cleaned up");

            try
            {
                _priorWindowProcedure = PInvokeCore.SetWindowLong(
                    ownerHwnd,
                    WINDOW_LONG_PTR_INDEX.GWL_WNDPROC,
                    hookedWndProc);

                using ThemingScope scope = new(Application.UseVisualStyles);
                Application.BeginModalMessageLoop();
                try
                {
                    result = RunDialog(ownerHwnd.Handle) ? DialogResult.OK : DialogResult.Cancel;
                }
                finally
                {
                    Application.EndModalMessageLoop();
                }
            }
            finally
            {
                nint currentSubClass = PInvokeCore.GetWindowLong(ownerHwnd.Handle, WINDOW_LONG_PTR_INDEX.GWL_WNDPROC);
                if (_priorWindowProcedure != 0 || currentSubClass != hookedWndProc)
                {
                    PInvokeCore.SetWindowLong(ownerHwnd.Handle, WINDOW_LONG_PTR_INDEX.GWL_WNDPROC, _priorWindowProcedure);
                }

                _priorWindowProcedure = 0;

                // Ensure that the subclass delegate will not be GC collected until after it has been subclassed.
                GC.KeepAlive(ownerWindowProcedure);
            }
        }
        finally
        {
            nativeWindow?.DestroyHandle();
        }

        GC.KeepAlive(ownerHwnd.Wrapper);

        return result;
    }
}
