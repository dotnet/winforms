// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Forms.Analyzers.Diagnostics;
using System.Windows.Forms.Primitives;
using Windows.Win32.Graphics.Dwm;
namespace System.Windows.Forms;

/// <summary>
///  Displays a message box that can contain text, buttons, and symbols that inform and instruct the user.
/// </summary>
public class MessageBox
{
    [ThreadStatic]
    private static HelpInfo[]? t_helpInfoTable;
    private const int MBOKId = 1;
    private const int MBCancelId = 2;
    private const int MBAbortId = 3;
    private const int MBRetryId = 4;
    private const int MBIgnoreId = 5;
    private const int MBYesId = 6;
    private const int MBNoId = 7;
    private const int MBHelpId = 9;
    // see
    // https://devblogs.microsoft.com/oldnewthing/20140224-00/?p=1683
    // and https://learn.microsoft.com/archive/msdn-magazine/2002/november/cutting-edge-using-windows-hooks-to-enhance-messagebox-in-net
    // Unique ID for static Edit in MessageBox, This ID has not changed since Windows 95 and will remain so.
    private const int MBTextId = ushort.MaxValue; // 0xFFFF
    private const int MBIconId = 20; // 0X0014
    private const int STM_SETICON = 0x00000170;
    private static HHOOK s_messageBoxHook;
    private static bool s_isMessageBoxHooked;
    private static readonly HOOKPROC s_hookCallBack = HookProc;
    private static readonly Lock s_lock = new();
    private static readonly nint s_hookPointer = Marshal.GetFunctionPointerForDelegate(s_hookCallBack);
    private static HWND s_hWndInternal;
    private static nint s_priorDlgProc;
    private static readonly PInvokeCore.EnumChildWindowsCallback s_childWindowsCallback = new PInvokeCore.EnumChildWindowsCallback(EnumChildProc);

#pragma warning disable WFO5003 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    private static ResourceManager? s_resourceManager => ResourceType is null ? null : new ResourceManager(ResourceType);
#pragma warning restore WFO5003 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    private static string? OK { get; set; }
    [Localizable(true)]
    private static string? Cancel { get; set; }
    [Localizable(true)]
    private static string? Abort { get; set; }
    [Localizable(true)]
    private static string? Retry { get; set; }
    [Localizable(true)]
    private static string? Ignore { get; set; }
    [Localizable(true)]
    private static string? Yes { get; set; }
    [Localizable(true)]
    private static string? No { get; set; }
    [Localizable(true)]
    private static string? Help { get; set; }

    /// <summary>
    ///  Type of <see cref="Resources"/> that used to localize Buttons in <see cref="MessageBox"/>.
    /// </summary>
    [Experimental(DiagnosticIDs.ExperimentalMessageBox, UrlFormat = DiagnosticIDs.UrlFormat)]
    public static Type? ResourceType { get; set; }
    /// <summary>
    ///  The foreground color of the  <see cref="MessageBox"/>.
    /// </summary>
    [Experimental(DiagnosticIDs.ExperimentalMessageBox, UrlFormat = DiagnosticIDs.UrlFormat)]
    public static Color ForeColor { get; set; } = SystemColors.ControlText;
    /// <summary>
    ///  The background color of the  <see cref="MessageBox"/>.
    /// </summary>

    [Experimental(DiagnosticIDs.ExperimentalMessageBox, UrlFormat = DiagnosticIDs.UrlFormat)]
    public static Color BackColor { get; set; } = SystemColors.Window;
    /// <summary>
    ///  The background color of the Footer in <see cref="MessageBox"/>.
    /// </summary>

    [Experimental(DiagnosticIDs.ExperimentalMessageBox, UrlFormat = DiagnosticIDs.UrlFormat)]
    public static Color FooterBackColor { get; set; } = SystemColors.Control;
    private static LRESULT DlgProcInternal(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam)
        => DlgProc(hWnd, (int)msg, (nint)wParam, lParam);
    private static Icon? s_customIcon;
    // This is meant to be a static class, but predates that feature.
    private MessageBox()
    {
    }

    private static LRESULT OnWmCtlColor(uint msg, IntPtr wParam)
    {
        switch (msg)
        {
            case PInvokeCore.WM_CTLCOLORBTN:
                return new LRESULT(PInvokeCore.CreateSolidBrush(SystemColors.ButtonFace));
            case PInvokeCore.WM_CTLCOLORDLG:
            case PInvokeCore.WM_CTLCOLORSTATIC:
                HDC hdc = new HDC(wParam);
                PInvokeCore.SetBkMode(hdc, BACKGROUND_MODE.TRANSPARENT);
#pragma warning disable WFO5003 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                PInvokeCore.SetTextColor(hdc, ForeColor);
                return new LRESULT(PInvokeCore.CreateSolidBrush(BackColor));
#pragma warning restore WFO5003 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        }

        return new LRESULT(0);
    }

    private static LRESULT OnWmPaint(HWND hWnd, IntPtr wParam)
    {
        HDC hdc = (HDC)wParam;
        bool usingBeginPaint = hdc.IsNull;
        using var paintScope = usingBeginPaint ? new BeginPaintScope(hWnd) : default;

        RECT clipRect;
        if (usingBeginPaint)
        {
            hdc = paintScope!.HDC;
            clipRect = paintScope.PaintRectangle;
        }
        else
        {
            PInvokeCore.GetClientRect(hWnd, out clipRect);
        }

#pragma warning disable WFO5003 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        using CreateBrushScope backGroundBrushScope = new CreateBrushScope(BackColor);
#pragma warning restore WFO5003 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        hdc.FillRectangle(clipRect, backGroundBrushScope);
        RECT FooterRect = clipRect;
        FooterRect.top = clipRect.Height - SystemInformation.CaptionHeight * 2;
#pragma warning disable WFO5003 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        using CreateBrushScope FooterBrushScope = new CreateBrushScope(FooterBackColor);
#pragma warning restore WFO5003 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        hdc.FillRectangle(FooterRect, FooterBrushScope);
        return new LRESULT(0);
    }

    private static unsafe BOOL EnumChildProc(HWND handle)
    {
        string className = string.Empty;
        Span<char> buffer = stackalloc char[PInvokeCore.MaxClassName];
        int length = 0;
        fixed (char* lpClassName = buffer)
        {
            length = PInvoke.GetClassName(handle, lpClassName, buffer.Length);
        }

        className = buffer.ToString()[..length];
        switch (className)
        {
            case PInvoke.WC_BUTTON:
#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

                PInvoke.SetWindowTheme(handle, Application.IsDarkModeEnabled ? "DarkMode_Explorer" : className, null);

#pragma warning restore WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
                if (s_resourceManager is null)
                {
                    return true;
                }

                int dlgCtrlID = PInvoke.GetDlgCtrlID(s_hWndInternal);
                switch (dlgCtrlID)
                {
                    case MBOKId:
                        OK = s_resourceManager.GetString("OK", CultureInfo.CurrentCulture) ?? "&OK";
                        PInvoke.SetWindowText(s_hWndInternal, OK);
                        break;
                    case MBCancelId:
                        OK = s_resourceManager.GetString("OK", CultureInfo.CurrentCulture);
                        Cancel = s_resourceManager.GetString("Cancel", CultureInfo.CurrentCulture) ?? "&Cancel";
                        PInvoke.SetWindowText(s_hWndInternal, Cancel);
                        break;
                    case MBAbortId:
                        Abort = s_resourceManager.GetString("Abort", CultureInfo.CurrentCulture) ?? "&Abort";
                        PInvoke.SetWindowText(s_hWndInternal, Abort);
                        break;
                    case MBRetryId:
                        Retry = s_resourceManager.GetString("Retry", CultureInfo.CurrentCulture) ?? "&Retry";
                        PInvoke.SetWindowText(s_hWndInternal, Retry);
                        break;
                    case MBIgnoreId:
                        Ignore = s_resourceManager.GetString("Ignore", CultureInfo.CurrentCulture) ?? "&Ignore";
                        PInvoke.SetWindowText(s_hWndInternal, Ignore);
                        break;
                    case MBYesId:
                        Yes = s_resourceManager.GetString("Yes", CultureInfo.CurrentCulture) ?? "&Yes";
                        PInvoke.SetWindowText(s_hWndInternal, Yes);
                        break;
                    case MBNoId:
                        No = s_resourceManager.GetString("No", CultureInfo.CurrentCulture) ?? "&No";
                        PInvoke.SetWindowText(s_hWndInternal, No);
                        break;
                    case MBHelpId:
                        Help = s_resourceManager.GetString("Help", CultureInfo.CurrentCulture) ?? "&Help";
                        PInvoke.SetWindowText(s_hWndInternal, Help);
                        break;
                }

                break;
        }

        return true;
    }

    private static unsafe void AllowDarkNonClientArea(HWND hWnd, bool allow)
    {
        BOOL currentValue;
        HRESULT result = PInvoke.DwmGetWindowAttribute(
          hWnd,
           DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE,
          &currentValue,
          (uint)sizeof(BOOL));
        if (result.Succeeded)
        {
            if (currentValue == allow)
            {
                // no need for using DwmSetWindowAttribute
                return;
            }

            PInvoke.DwmSetWindowAttribute(
            hWnd,
            DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE,
            &allow,
            (uint)sizeof(BOOL)).AssertSuccess();
        }
    }

    private static unsafe LRESULT DlgProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
    {
        s_hWndInternal = (HWND)hWnd;
        switch ((uint)msg)
        {
            case PInvokeCore.WM_INITDIALOG:
#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
                AllowDarkNonClientArea(s_hWndInternal, Application.IsDarkModeEnabled);
                if (s_customIcon is not null && s_customIcon.Handle != IntPtr.Zero)
                {
                    HWND hwndIcon = PInvoke.GetDlgItem(s_hWndInternal, MBIconId);
                    if (!hwndIcon.IsNull)
                    {
                        PInvokeCore.SendMessage<HWND>(hwndIcon, STM_SETICON, (WPARAM)s_customIcon.Handle, 0);
                    }
                }
#pragma warning restore WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
                PInvokeCore.EnumChildWindows(s_hWndInternal, s_childWindowsCallback);
                return PInvokeCore.CallWindowProc((void*)s_priorDlgProc, s_hWndInternal, (uint)msg, (nuint)wparam, lparam);
            case PInvokeCore.WM_PAINT:
                return OnWmPaint(s_hWndInternal, wparam);
            case PInvokeCore.WM_CTLCOLORBTN:
            case PInvokeCore.WM_CTLCOLORDLG:
            case PInvokeCore.WM_CTLCOLORSTATIC:
                return OnWmCtlColor((uint)msg, wparam);
            default:
                return PInvokeCore.CallWindowProc((void*)s_priorDlgProc, s_hWndInternal, (uint)msg, (nuint)wparam, lparam);
        }
    }

    private static unsafe void InstallHook()
    {
        lock (s_lock)
        {
            if (s_messageBoxHook != 0)
            {
                return;
            }

            s_messageBoxHook = PInvoke.SetWindowsHookEx(
                WINDOWS_HOOK_ID.WH_CALLWNDPROC,
                (delegate* unmanaged[Stdcall]<int, WPARAM, LPARAM, LRESULT>)s_hookPointer,
                HINSTANCE.Null,
                PInvokeCore.GetCurrentThreadId());

            s_isMessageBoxHooked = s_messageBoxHook != 0;

            Debug.Assert(s_isMessageBoxHooked, "Failed to install MessageBox hook.");
        }
    }

    private static void UnInstallHook()
    {
        lock (s_lock)
        {
            if (s_messageBoxHook != 0)
            {
                if (!PInvoke.UnhookWindowsHookEx(s_messageBoxHook))
                {
                    Debug.Fail("Failed to remove MessageBox hook.");
                }

                s_messageBoxHook = default;
                s_isMessageBoxHooked = false;
            }

            if (s_priorDlgProc != 0)
            {
                PInvokeCore.SetWindowLong(s_hWndInternal, (WINDOW_LONG_PTR_INDEX)IntPtr.Size, s_priorDlgProc);
            }

            s_priorDlgProc = 0;
        }
    }

    private static unsafe LRESULT HookProc(int nCode, WPARAM wParam, LPARAM lParam)
    {
        if (s_isMessageBoxHooked && nCode == PInvoke.HC_ACTION && lParam != 0)
        {
            CWPSTRUCT msg = Marshal.PtrToStructure<CWPSTRUCT>(lParam);
            if (msg.message == PInvokeCore.WM_INITDIALOG)
            {
                HWND hwndText = PInvoke.GetDlgItem(msg.hwnd, MBTextId);
                if (!hwndText.IsNull)
                {
                    WNDPROC ownerWindowProcedure = DlgProcInternal;
                    nint newDlgProcPointer = Marshal.GetFunctionPointerForDelegate(ownerWindowProcedure);
                    Debug.Assert(s_priorDlgProc == 0, "The previous subclass wasn't properly cleaned up");
                    s_priorDlgProc = PInvokeCore.SetWindowLong(
                       msg.hwnd,
                       (WINDOW_LONG_PTR_INDEX)IntPtr.Size,
                       newDlgProcPointer);
                }

            }
        }

        return PInvoke.CallNextHookEx(s_messageBoxHook, nCode, wParam, lParam);
    }

    internal static HelpInfo? HelpInfo
    {
        get
        {
            // Unfortunately, there's no easy way to obtain handle of a message box.
            // We'll have to rely on the fact that modal message loops have to pop off in an orderly way.

            if (t_helpInfoTable is not null && t_helpInfoTable.Length > 0)
            {
                // The top of the stack is actually at the end of the array.
                return t_helpInfoTable[^1];
            }

            return null;
        }
    }

    private static MESSAGEBOX_STYLE GetMessageBoxStyle(
        IWin32Window? owner,
        MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options,
        bool showHelp)
    {
        SourceGenerated.EnumValidator.Validate(buttons, nameof(buttons));
        SourceGenerated.EnumValidator.Validate(icon, nameof(icon));
        SourceGenerated.EnumValidator.Validate(defaultButton, nameof(defaultButton));

        // options intentionally not verified because we don't expose all the options Win32 supports.

        if (!SystemInformation.UserInteractive && (options & (MessageBoxOptions.ServiceNotification | MessageBoxOptions.DefaultDesktopOnly)) == 0)
        {
            throw new InvalidOperationException(SR.CantShowModalOnNonInteractive);
        }

        if (owner is not null && (options & (MessageBoxOptions.ServiceNotification | MessageBoxOptions.DefaultDesktopOnly)) != 0)
        {
            throw new ArgumentException(SR.CantShowMBServiceWithOwner, nameof(options));
        }

        if (showHelp && (options & (MessageBoxOptions.ServiceNotification | MessageBoxOptions.DefaultDesktopOnly)) != 0)
        {
            throw new ArgumentException(SR.CantShowMBServiceWithHelp, nameof(options));
        }

        MESSAGEBOX_STYLE style = (showHelp) ? MESSAGEBOX_STYLE.MB_HELP : 0;
        style |= (MESSAGEBOX_STYLE)buttons | (MESSAGEBOX_STYLE)icon | (MESSAGEBOX_STYLE)defaultButton | (MESSAGEBOX_STYLE)options;
        return style;
    }

    private static void PopHelpInfo()
    {
        // we roll our own stack here because we want a pretty lightweight implementation.
        // usually there's only going to be one message box shown at a time.  But if
        // someone shows two message boxes (say by launching them via a WM_TIMER message)
        // we've got to gracefully handle the current help info.
        if (t_helpInfoTable is null)
        {
            Debug.Fail("Why are we being called when there's nothing to pop?");
        }
        else
        {
            if (t_helpInfoTable.Length == 1)
            {
                t_helpInfoTable = null;
            }
            else
            {
                int newCount = t_helpInfoTable.Length - 1;
                HelpInfo[] newTable = new HelpInfo[newCount];
                Array.Copy(t_helpInfoTable, newTable, newCount);
                t_helpInfoTable = newTable;
            }
        }
    }

    private static void PushHelpInfo(HelpInfo hpi)
    {
        // we roll our own stack here because we want a pretty lightweight implementation.
        // usually there's only going to be one message box shown at a time.  But if
        // someone shows two message boxes (say by launching them via a WM_TIMER message)
        // we've got to gracefully handle the current help info.

        int lastCount = 0;
        HelpInfo[] newTable;

        if (t_helpInfoTable is null)
        {
            newTable = new HelpInfo[lastCount + 1];
        }
        else
        {
            // if we already have a table - allocate a new slot
            lastCount = t_helpInfoTable.Length;
            newTable = new HelpInfo[lastCount + 1];
            Array.Copy(t_helpInfoTable, newTable, lastCount);
        }

        newTable[lastCount] = hpi;
        t_helpInfoTable = newTable;
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, and style with Help Button.
    /// </summary>
    public static DialogResult Show(
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options,
        bool displayHelpButton)
    {
        return ShowCore(null, text, caption, buttons, icon, defaultButton, options, displayHelpButton);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, style and Help file Path .
    /// </summary>
    public static DialogResult Show(
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options,
        string helpFilePath)
    {
        HelpInfo hpi = new(helpFilePath);
        return ShowCore(null, text, caption, buttons, icon, defaultButton, options, hpi);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, style and Help file Path for a IWin32Window.
    /// </summary>
    public static DialogResult Show(
        IWin32Window? owner,
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options,
        string helpFilePath)
    {
        HelpInfo hpi = new(helpFilePath);
        return ShowCore(owner, text, caption, buttons, icon, defaultButton, options, hpi);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, style, Help file Path and keyword.
    /// </summary>
    public static DialogResult Show(
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options,
        string helpFilePath,
        string keyword)
    {
        HelpInfo hpi = new(helpFilePath, keyword);
        return ShowCore(null, text, caption, buttons, icon, defaultButton, options, hpi);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, style, Help file Path and keyword for a IWin32Window.
    /// </summary>
    public static DialogResult Show(
        IWin32Window? owner,
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options,
        string helpFilePath,
        string keyword)
    {
        HelpInfo hpi = new(helpFilePath, keyword);
        return ShowCore(owner, text, caption, buttons, icon, defaultButton, options, hpi);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, style, Help file Path and HelpNavigator.
    /// </summary>
    public static DialogResult Show(
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options,
        string helpFilePath,
        HelpNavigator navigator)
    {
        HelpInfo hpi = new(helpFilePath, navigator);
        return ShowCore(null, text, caption, buttons, icon, defaultButton, options, hpi);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, style, Help file Path and HelpNavigator for IWin32Window.
    /// </summary>
    public static DialogResult Show(
        IWin32Window? owner,
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options,
        string helpFilePath,
        HelpNavigator navigator)
    {
        HelpInfo hpi = new(helpFilePath, navigator);
        return ShowCore(owner, text, caption, buttons, icon, defaultButton, options, hpi);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, style, Help file Path ,HelpNavigator and object.
    /// </summary>
    public static DialogResult Show(
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options,
        string helpFilePath,
        HelpNavigator navigator,
        object? param)
    {
        HelpInfo hpi = new(helpFilePath, navigator, param);

        return ShowCore(null, text, caption, buttons, icon, defaultButton, options, hpi);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, style, Help file Path ,HelpNavigator and object for a IWin32Window.
    /// </summary>
    public static DialogResult Show(
        IWin32Window? owner,
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options,
        string helpFilePath,
        HelpNavigator navigator,
        object? param)
    {
        HelpInfo hpi = new(helpFilePath, navigator, param);

        return ShowCore(owner, text, caption, buttons, icon, defaultButton, options, hpi);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, and style.
    /// </summary>
    public static DialogResult Show(
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options)
    {
        return ShowCore(null, text, caption, buttons, icon, defaultButton, options, false);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, and style.
    /// </summary>
    public static DialogResult Show(
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton)
    {
        return ShowCore(null, text, caption, buttons, icon, defaultButton, 0, false);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, and style.
    /// </summary>
    public static DialogResult Show(
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        MessageBoxIcon icon)
    {
        return ShowCore(null, text, caption, buttons, icon, MessageBoxDefaultButton.Button1, 0, false);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, and style with Help Button.
    /// </summary>
    [Experimental(DiagnosticIDs.ExperimentalMessageBox, UrlFormat = DiagnosticIDs.UrlFormat)]
    public static DialogResult Show(
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        Icon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options,
        bool displayHelpButton)
    {
        s_customIcon = icon;
        return ShowCore(null, text, caption, buttons, MessageBoxIcon.Information, defaultButton, options, displayHelpButton);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, style and Help file Path .
    /// </summary>
    [Experimental(DiagnosticIDs.ExperimentalMessageBox, UrlFormat = DiagnosticIDs.UrlFormat)]
    public static DialogResult Show(
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        Icon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options,
        string helpFilePath)
    {
        HelpInfo hpi = new(helpFilePath);
        s_customIcon = icon;
        return ShowCore(null, text, caption, buttons, MessageBoxIcon.Information, defaultButton, options, hpi);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, style and Help file Path for a IWin32Window.
    /// </summary>
    [Experimental(DiagnosticIDs.ExperimentalMessageBox, UrlFormat = DiagnosticIDs.UrlFormat)]
    public static DialogResult Show(
        IWin32Window? owner,
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        Icon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options,
        string helpFilePath)
    {
        HelpInfo hpi = new(helpFilePath);
        s_customIcon = icon;
        return ShowCore(owner, text, caption, buttons, MessageBoxIcon.Information, defaultButton, options, hpi);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, style, Help file Path, keyword and custom Icon.
    /// </summary>
    [Experimental(DiagnosticIDs.ExperimentalMessageBox, UrlFormat = DiagnosticIDs.UrlFormat)]
    public static DialogResult Show(
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        Icon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options,
        string helpFilePath,
        string keyword)
    {
        HelpInfo hpi = new(helpFilePath, keyword);
        s_customIcon = icon;
        return ShowCore(null, text, caption, buttons, MessageBoxIcon.Information, defaultButton, options, hpi);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, style, Help file Path, keyword and custom Icon for a IWin32Window.
    /// </summary>
    [Experimental(DiagnosticIDs.ExperimentalMessageBox, UrlFormat = DiagnosticIDs.UrlFormat)]
    public static DialogResult Show(
        IWin32Window? owner,
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        Icon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options,
        string helpFilePath,
        string keyword)
    {
        HelpInfo hpi = new(helpFilePath, keyword);
        s_customIcon = icon;
        return ShowCore(owner, text, caption, buttons, MessageBoxIcon.Information, defaultButton, options, hpi);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, style, Help file Path HelpNavigator, and custom Icon.
    /// </summary>
    [Experimental(DiagnosticIDs.ExperimentalMessageBox, UrlFormat = DiagnosticIDs.UrlFormat)]
    public static DialogResult Show(
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        Icon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options,
        string helpFilePath,
        HelpNavigator navigator)
    {
        HelpInfo hpi = new(helpFilePath, navigator);
        s_customIcon = icon;
        return ShowCore(null, text, caption, buttons, MessageBoxIcon.Information, defaultButton, options, hpi);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, style, Help file Path HelpNavigator, and custom Icon for IWin32Window.
    /// </summary>
    [Experimental(DiagnosticIDs.ExperimentalMessageBox, UrlFormat = DiagnosticIDs.UrlFormat)]
    public static DialogResult Show(
        IWin32Window? owner,
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        Icon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options,
        string helpFilePath,
        HelpNavigator navigator)
    {
        HelpInfo hpi = new(helpFilePath, navigator);
        s_customIcon = icon;
        return ShowCore(owner, text, caption, buttons, MessageBoxIcon.Information, defaultButton, options, hpi);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, style, Help file Path ,HelpNavigator,object and custom Icon.
    /// </summary>
    [Experimental(DiagnosticIDs.ExperimentalMessageBox, UrlFormat = DiagnosticIDs.UrlFormat)]
    public static DialogResult Show(
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        Icon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options,
        string helpFilePath,
        HelpNavigator navigator,
        object? param)
    {
        HelpInfo hpi = new(helpFilePath, navigator, param);
        s_customIcon = icon;
        return ShowCore(null, text, caption, buttons, MessageBoxIcon.Information, defaultButton, options, hpi);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, style, Help file Path ,HelpNavigator, object and custom Icon for a IWin32Window.
    /// </summary>
    [Experimental(DiagnosticIDs.ExperimentalMessageBox, UrlFormat = DiagnosticIDs.UrlFormat)]
    public static DialogResult Show(
        IWin32Window? owner,
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        Icon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options,
        string helpFilePath,
        HelpNavigator navigator,
        object? param)
    {
        HelpInfo hpi = new(helpFilePath, navigator, param);
        s_customIcon = icon;
        return ShowCore(owner, text, caption, buttons, MessageBoxIcon.Information, defaultButton, options, hpi);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, style, and custom Icon.
    /// </summary>
    [Experimental(DiagnosticIDs.ExperimentalMessageBox, UrlFormat = DiagnosticIDs.UrlFormat)]
    public static DialogResult Show(
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        Icon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options)
    {
        s_customIcon = icon;
        return ShowCore(null, text, caption, buttons, MessageBoxIcon.Information, defaultButton, options, false);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, style, and custom Icon.
    /// </summary>
    [Experimental(DiagnosticIDs.ExperimentalMessageBox, UrlFormat = DiagnosticIDs.UrlFormat)]
    public static DialogResult Show(
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        Icon icon,
        MessageBoxDefaultButton defaultButton)
    {
        s_customIcon = icon;
        return ShowCore(null, text, caption, buttons, MessageBoxIcon.Information, defaultButton, 0, false);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, style, and custom Icon.
    /// </summary>
    [Experimental(DiagnosticIDs.ExperimentalMessageBox, UrlFormat = DiagnosticIDs.UrlFormat)]
    public static DialogResult Show(
      string? text,
      string? caption,
      MessageBoxButtons buttons,
      Icon icon)
    {
        s_customIcon = icon;
        return ShowCore(null, text, caption, buttons, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, 0, false);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, style .
    /// </summary>
    public static DialogResult Show(string? text, string? caption, MessageBoxButtons buttons)
    {
        return ShowCore(null, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);
    }

    /// <summary>
    ///  Displays a message box with specified text and caption.
    /// </summary>
    public static DialogResult Show(string? text, string? caption)
    {
        return ShowCore(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);
    }

    /// <summary>
    ///  Displays a message box with specified text.
    /// </summary>
    public static DialogResult Show(string? text)
    {
        return ShowCore(null, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, and style.
    /// </summary>
    public static DialogResult Show(
        IWin32Window? owner,
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options)
    {
        return ShowCore(owner, text, caption, buttons, icon, defaultButton, options, false);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, and style.
    /// </summary>
    public static DialogResult Show(
        IWin32Window? owner,
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton)
    {
        return ShowCore(owner, text, caption, buttons, icon, defaultButton, 0, false);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, and style.
    /// </summary>
    public static DialogResult Show(
        IWin32Window? owner,
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        MessageBoxIcon icon)
    {
        return ShowCore(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1, 0, false);
    }

    /// <summary>
    ///  Displays a message box with specified text, caption, and style.
    /// </summary>
    public static DialogResult Show(
        IWin32Window? owner,
        string? text,
        string? caption,
        MessageBoxButtons buttons)
    {
        return ShowCore(owner, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);
    }

    /// <summary>
    ///  Displays a message box with specified text and caption.
    /// </summary>
    public static DialogResult Show(IWin32Window? owner, string? text, string? caption)
    {
        return ShowCore(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);
    }

    /// <summary>
    ///  Displays a message box with specified text.
    /// </summary>
    public static DialogResult Show(IWin32Window? owner, string? text)
    {
        return ShowCore(owner, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);
    }

    private static DialogResult ShowCore(
        IWin32Window? owner,
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options,
        HelpInfo hpi)
    {
        DialogResult result = DialogResult.None;
        try
        {
            PushHelpInfo(hpi);
            result = ShowCore(owner, text, caption, buttons, icon, defaultButton, options, true);
        }
        finally
        {
            PopHelpInfo();
        }

        return result;
    }

    private static DialogResult ShowCore(
        IWin32Window? owner,
        string? text,
        string? caption,
        MessageBoxButtons buttons,
        MessageBoxIcon icon,
        MessageBoxDefaultButton defaultButton,
        MessageBoxOptions options,
        bool showHelp)
    {
        if (LocalAppContextSwitches.NoClientNotifications)
        {
            return DialogResult.None;
        }

        MESSAGEBOX_STYLE style = GetMessageBoxStyle(owner, buttons, icon, defaultButton, options, showHelp);

        HandleRef<HWND> handle = default;
        if (showHelp || ((options & (MessageBoxOptions.ServiceNotification | MessageBoxOptions.DefaultDesktopOnly)) == 0))
        {
            handle = owner is null ? Control.GetHandleRef(PInvoke.GetActiveWindow()) : Control.GetSafeHandle(owner);
        }

        if (Application.UseVisualStyles)
        {
            // CLR4.0 or later, shell32.dll needs to be loaded explicitly.
            if (PInvoke.GetModuleHandle(Libraries.Shell32) == 0)
            {
                if (PInvoke.LoadLibraryFromSystemPathIfAvailable(Libraries.Shell32) == HINSTANCE.Null)
                {
                    int lastWin32Error = Marshal.GetLastWin32Error();
                    throw new Win32Exception(lastWin32Error, string.Format(SR.LoadDLLError, Libraries.Shell32));
                }
            }
        }

        // Activate theming scope to get theming for controls at design time and when hosted in browser.
        // NOTE: If a theming context is already active, this call is very fast, so shouldn't be a perf issue.
        using ThemingScope scope = new(Application.UseVisualStyles);
        Application.BeginModalMessageLoop();

        try
        {
            InstallHook();
            return (DialogResult)PInvoke.MessageBox(handle.Handle, text, caption, style);
        }
        finally
        {
            UnInstallHook();
            Application.EndModalMessageLoop();

            // Right after the dialog box is closed, Windows sends WM_SETFOCUS back to the previously active control
            // but since we have disabled this thread main window the message is lost. So we have to send it again after
            // we enable the main window.
            PInvokeCore.SendMessage(handle, PInvokeCore.WM_SETFOCUS);
        }
    }
}
