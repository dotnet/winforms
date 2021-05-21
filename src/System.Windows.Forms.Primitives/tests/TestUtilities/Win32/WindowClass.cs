// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using static Interop;

namespace System
{
    internal class WindowClass
    {
        [DllImport(Libraries.User32, SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true)]
        public unsafe static extern IntPtr LoadIconW(
            IntPtr hInstance,
            IntPtr lpIconName);

        private const int CW_USEDEFAULT = unchecked((int)0x80000000);
        private const uint IDI_APPLICATION = 32512;

        private static RECT DefaultBounds => new RECT(CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT);

        // Stash the delegate to keep it from being collected
        private readonly User32.WNDPROC _windowProcedure;
        private User32.WNDCLASS _wndClass;
        private readonly string _className;
        private readonly string _menuName;

        public Atom Atom { get; private set; }
        public IntPtr MainWindow { get; private set; }
        public IntPtr ModuleInstance { get; }

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="className">Name, or default will be generated.</param>
        /// <param name="moduleInstance">Module to associate with the window. The entry assembly is the default.</param>
        /// <param name="backgroundBrush">Use (IntPtr)(-1) for no background brush.</param>
        /// <param name="icon">Use (IntPtr)(-1) for no icon.</param>
        /// <param name="cursor">Use (IntPtr)(-1) for no cursor.</param>
        /// <param name="menuName">Menu name, can not set with <paramref name="menuId"/>.</param>
        /// <param name="menuId">Menu id, can not set with <paramref name="menuName"/>.</param>
        public unsafe WindowClass(
            string className = default,
            IntPtr moduleInstance = default,
            User32.CS classStyle = User32.CS.HREDRAW | User32.CS.VREDRAW,
            Gdi32.HBRUSH backgroundBrush = default,
            IntPtr icon = default,
            IntPtr cursor = default,
            string menuName = null,
            int menuId = 0,
            int classExtraBytes = 0,
            int windowExtraBytes = 0)
        {
            // Handle default values
            className ??= Guid.NewGuid().ToString();

            if (backgroundBrush.IsNull)
            {
                backgroundBrush = User32.GetSysColorBrush(User32.COLOR.WINDOW);
            }
            else if (backgroundBrush.Handle == (IntPtr)(-1))
            {
                backgroundBrush = default;
            }

            if (icon == default)
            {
                icon = LoadIconW(IntPtr.Zero, (IntPtr)IDI_APPLICATION);
            }
            else if (icon == (IntPtr)(-1))
            {
                icon = default;
            }

            if (cursor == default)
            {
                cursor = User32.LoadCursorW(IntPtr.Zero, (IntPtr)User32.CursorResourceId.IDC_ARROW);
            }
            else if (cursor == (IntPtr)(-1))
            {
                cursor = default;
            }

            if (moduleInstance == IntPtr.Zero)
                Marshal.GetHINSTANCE(Assembly.GetCallingAssembly().Modules.First());

            if (menuId != 0 && menuName != null)
                throw new ArgumentException($"Can't set both {nameof(menuName)} and {nameof(menuId)}.");

            _windowProcedure = WNDPROC;
            ModuleInstance = moduleInstance;

            _className = className;
            _menuName = menuName ?? string.Empty;

            _wndClass = new User32.WNDCLASS
            {
                style = classStyle,
                lpfnWndProc = Marshal.GetFunctionPointerForDelegate(_windowProcedure),
                cbClsExtra = classExtraBytes,
                cbWndExtra = windowExtraBytes,
                hInstance = moduleInstance,
                hIcon = icon,
                hCursor = cursor,
                hbrBackground = backgroundBrush,
                lpszMenuName = (char*)menuId
            };
        }

        public bool IsRegistered => Atom.IsValid || ModuleInstance == IntPtr.Zero;

        public unsafe WindowClass Register()
        {
            fixed (char* name = _className)
            fixed (char* menuName = _menuName)
            {
                _wndClass.lpszClassName = name;
                if (!string.IsNullOrEmpty(_menuName))
                    _wndClass.lpszMenuName = menuName;

                Atom atom = User32.RegisterClassW(ref _wndClass);
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
            User32.WS style = User32.WS.OVERLAPPED,
            User32.WS_EX extendedStyle = default,
            bool isMainWindow = false,
            IntPtr parentWindow = default,
            IntPtr parameters = default,
            IntPtr menuHandle = default)
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

        public unsafe IntPtr CreateWindow(
            RECT bounds,
            string windowName = null,
            User32.WS style = User32.WS.OVERLAPPED,
            User32.WS_EX extendedStyle = default,
            bool isMainWindow = false,
            IntPtr parentWindow = default,
            IntPtr parameters = default,
            IntPtr menuHandle = default)
        {
            if (!IsRegistered)
                throw new ArgumentException("Window class must be registered before using.");

            IntPtr window;
            if (Atom.IsValid)
            {
                window = User32.CreateWindowExW(
                    dwExStyle: extendedStyle,
                    lpClassName: (char*)Atom.ATOM,
                    lpWindowName: windowName,
                    dwStyle: style,
                    X: bounds.X,
                    Y: bounds.Y,
                    nWidth: bounds.Width,
                    nHeight: bounds.Height,
                    hWndParent: parentWindow,
                    hMenu: menuHandle,
                    hInst: IntPtr.Zero,
                    lpParam: parameters);
            }
            else
            {
                fixed (char* atom = _className)
                {
                    window = User32.CreateWindowExW(
                        dwExStyle: extendedStyle,
                        lpClassName: atom,
                        lpWindowName: windowName,
                        dwStyle: style,
                        X: bounds.X,
                        Y: bounds.Y,
                        nWidth: bounds.Width,
                        nHeight: bounds.Height,
                        hWndParent: parentWindow,
                        hMenu: menuHandle,
                        hInst: IntPtr.Zero,
                        lpParam: parameters);
                }
            }

            if (window == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            if (!Atom.IsValid)
            {
                Atom = User32.GetClassLong(window, User32.GCL.ATOM);
            }

            if (isMainWindow)
                MainWindow = window;

            return window;
        }

        protected virtual IntPtr WNDPROC(IntPtr hWnd, User32.WM msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case User32.WM.DESTROY:
                    if (hWnd == MainWindow)
                        User32.PostQuitMessage(0);
                    return (IntPtr)0;
            }

            return User32.DefWindowProcW(hWnd, msg, wParam, lParam);
        }
    }
}
