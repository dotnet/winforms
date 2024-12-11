// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System;

internal class WindowClass
{
    private static RECT DefaultBounds
        => new(PInvoke.CW_USEDEFAULT, PInvoke.CW_USEDEFAULT, PInvoke.CW_USEDEFAULT, PInvoke.CW_USEDEFAULT);

    // Stash the delegate to keep it from being collected
    private readonly WNDPROC _windowProcedure;
    private WNDCLASSW _wndClass;
    private readonly string _className;
    private readonly string _menuName;

    public ATOM Atom { get; private set; }
    public HWND MainWindow { get; private set; }
    public HINSTANCE ModuleInstance { get; }

    /// <summary>
    ///  Constructor.
    /// </summary>
    /// <param name="className">Name, or default will be generated.</param>
    /// <param name="moduleInstance">Module to associate with the window. The entry assembly is the default.</param>
    /// <param name="backgroundBrush">Use (HINSTANCE)(-1) for no background brush.</param>
    /// <param name="icon">Use (HICON)(-1) for no icon.</param>
    /// <param name="cursor">Use (HCURSOR)(-1) for no cursor.</param>
    /// <param name="menuName">Menu name, can not set with <paramref name="menuId"/>.</param>
    /// <param name="menuId">Menu id, can not set with <paramref name="menuName"/>.</param>
    [UnconditionalSuppressMessage("SingleFile", "IL3002:Avoid calling members marked with 'RequiresAssemblyFilesAttribute' when publishing as a single-file", Justification = "Test only binary and not shippable.")]
    public unsafe WindowClass(
        string className = default,
        HINSTANCE moduleInstance = default,
        WNDCLASS_STYLES classStyle = WNDCLASS_STYLES.CS_HREDRAW | WNDCLASS_STYLES.CS_VREDRAW,
        HBRUSH backgroundBrush = default,
        HICON icon = default,
        HCURSOR cursor = default,
        string menuName = null,
        int menuId = 0,
        int classExtraBytes = 0,
        int windowExtraBytes = 0)
    {
        // Handle default values
        className ??= Guid.NewGuid().ToString();

        if (backgroundBrush.IsNull)
        {
            backgroundBrush = PInvokeCore.GetSysColorBrush(SYS_COLOR_INDEX.COLOR_WINDOW);
        }
        else if (backgroundBrush == (HBRUSH)(-1))
        {
            backgroundBrush = default;
        }

        if (icon.IsNull)
        {
            icon = PInvokeCore.LoadIcon(HINSTANCE.Null, (PCWSTR)(char*)PInvokeCore.IDI_APPLICATION);
        }
        else if (icon == (-1))
        {
            icon = default;
        }

        if (cursor == default)
        {
            cursor = PInvoke.LoadCursor(HINSTANCE.Null, (PCWSTR)(char*)PInvoke.IDC_ARROW);
        }
        else if (cursor == (-1))
        {
            cursor = default;
        }

        if (moduleInstance.IsNull)
            Marshal.GetHINSTANCE(Assembly.GetCallingAssembly().Modules.First());

        if (menuId != 0 && menuName is not null)
            throw new ArgumentException($"Can't set both {nameof(menuName)} and {nameof(menuId)}.");

        _windowProcedure = WNDPROC;
        ModuleInstance = moduleInstance;

        _className = className;
        _menuName = menuName ?? string.Empty;

        _wndClass = new WNDCLASSW
        {
            style = classStyle,
            lpfnWndProc = (delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, LRESULT>)Marshal.GetFunctionPointerForDelegate(_windowProcedure),
            cbClsExtra = classExtraBytes,
            cbWndExtra = windowExtraBytes,
            hInstance = moduleInstance,
            hIcon = icon,
            hCursor = cursor,
            hbrBackground = backgroundBrush,
            lpszMenuName = (char*)menuId
        };
    }

    public bool IsRegistered => Atom.IsValid || ModuleInstance.IsNull;

    public unsafe WindowClass Register()
    {
        fixed (char* name = _className)
        fixed (char* menuName = _menuName)
        {
            _wndClass.lpszClassName = name;
            if (!string.IsNullOrEmpty(_menuName))
                _wndClass.lpszMenuName = menuName;

            ATOM atom = PInvoke.RegisterClass(in _wndClass);
            if (!atom.IsValid)
            {
                throw new Win32Exception();
            }

            Atom = atom;
            return this;
        }
    }

    public IntPtr CreateWindow(
        string windowName = null,
        WINDOW_STYLE style = WINDOW_STYLE.WS_OVERLAPPED,
        WINDOW_EX_STYLE extendedStyle = default,
        bool isMainWindow = false,
        HWND parentWindow = default,
        nint parameters = default,
        HMENU menuHandle = default)
    {
        return CreateWindow(
            DefaultBounds,
            windowName,
            style,
            extendedStyle,
            isMainWindow,
            parentWindow,
            parameters,
            menuHandle);
    }

    public unsafe HWND CreateWindow(
        RECT bounds,
        string windowName = null,
        WINDOW_STYLE style = WINDOW_STYLE.WS_OVERLAPPED,
        WINDOW_EX_STYLE extendedStyle = default,
        bool isMainWindow = false,
        HWND parentWindow = default,
        nint parameters = default,
        HMENU menuHandle = default)
    {
        if (!IsRegistered)
            throw new ArgumentException("Window class must be registered before using.");

        fixed (char* wn = windowName)
        {
            HWND window;
            if (Atom.IsValid)
            {
                window = PInvoke.CreateWindowEx(
                    dwExStyle: extendedStyle,
                    lpClassName: (PCWSTR)(char*)Atom.Value,
                    lpWindowName: (PCWSTR)wn,
                    dwStyle: style,
                    X: bounds.X,
                    Y: bounds.Y,
                    nWidth: bounds.Width,
                    nHeight: bounds.Height,
                    hWndParent: parentWindow,
                    hMenu: menuHandle,
                    hInstance: HINSTANCE.Null,
                    lpParam: (void*)parameters);
            }
            else
            {
                fixed (char* cn = _className)
                {
                    window = PInvoke.CreateWindowEx(
                        dwExStyle: extendedStyle,
                        lpClassName: (PCWSTR)cn,
                        lpWindowName: (PCWSTR)wn,
                        dwStyle: style,
                        X: bounds.X,
                        Y: bounds.Y,
                        nWidth: bounds.Width,
                        nHeight: bounds.Height,
                        hWndParent: parentWindow,
                        hMenu: menuHandle,
                        hInstance: HINSTANCE.Null,
                        lpParam: (void*)parameters);
                }
            }

            if (window.IsNull)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            if (!Atom.IsValid)
            {
                Atom = PInvokeCore.GetClassLong(window, GET_CLASS_LONG_INDEX.GCW_ATOM);
            }

            if (isMainWindow)
            {
                MainWindow = window;
            }

            return window;
        }
    }

    protected virtual LRESULT WNDPROC(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam)
    {
        switch (msg)
        {
            case PInvokeCore.WM_DESTROY:
                if (hWnd == MainWindow)
                {
                    PInvoke.PostQuitMessage(0);
                }

                return (LRESULT)0;
        }

        return PInvokeCore.DefWindowProc(hWnd, msg, wParam, lParam);
    }
}
