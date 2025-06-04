// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace System.Windows.Forms;

public partial class NativeWindow
{
    /// <summary>
    ///  WindowClass encapsulates a Windows window class.
    /// </summary>
    private class WindowClass
    {
        internal static WindowClass? s_cache;

        internal WindowClass? _next;
        internal string? _className;
        internal string? _windowClassName;
        internal NativeWindow? _targetWindow;

        private readonly WNDCLASS_STYLES _classStyle;
        private IntPtr _defaultWindProc;

        // This needs to be a field so the GC doesn't collect the managed callback
        private WNDPROC? _windProc;

        // There is only ever one AppDomain
        private static readonly string s_currentAppDomainHash = Convert.ToString(AppDomain.CurrentDomain.GetHashCode(), 16);

        private static readonly Lock s_wcInternalSyncObject = new();

        internal WindowClass(string? className, WNDCLASS_STYLES classStyle)
        {
            _className = className;
            _classStyle = classStyle;
            RegisterClass();
        }

        public LRESULT Callback(HWND hwnd, uint msg, WPARAM wparam, LPARAM lparam)
        {
            Debug.Assert(!hwnd.IsNull, "Windows called us with an HWND of 0");

            // Set the window procedure to the default window procedure
            PInvokeCore.SetWindowLong(hwnd, WINDOW_LONG_PTR_INDEX.GWL_WNDPROC, _defaultWindProc);

            Debug.Assert(_targetWindow is not null);

            _targetWindow.AssignHandle(hwnd);
            return _targetWindow.Callback(hwnd, msg, wparam, lparam);
        }

        /// <summary>
        ///  Retrieves a WindowClass object for use. This will create a new
        ///  object if there is no such class/style available, or return a
        ///  cached object if one exists.
        /// </summary>
        internal static WindowClass FindOrCreate(string? className, WNDCLASS_STYLES classStyle)
        {
            lock (s_wcInternalSyncObject)
            {
                WindowClass? wc = s_cache;
                if (className is null)
                {
                    // If we weren't given a class name, look for a window
                    // that has the exact class style.
                    while (wc is not null
                        && (wc._className is not null || wc._classStyle != classStyle))
                    {
                        wc = wc._next;
                    }
                }
                else
                {
                    while (wc is not null && !className.Equals(wc._className))
                    {
                        wc = wc._next;
                    }
                }

                if (wc is null)
                {
                    // Didn't find an existing class, create one and attach it to
                    // the end of the linked list.
                    wc = new WindowClass(className, classStyle)
                    {
                        _next = s_cache
                    };

                    s_cache = wc;
                }

                return wc;
            }
        }

        /// <summary>
        ///  Fabricates a full class name from a partial.
        /// </summary>
        private static string GetFullClassName(string className)
        {
            // VersioningHelper does a lot of string allocations, and on .NET Core for our purposes
            // it always returns the exact same string (process is hardcoded to r3 and the AppDomain
            // id is always 1 as there is only one AppDomain).

            const string versionSuffix = "_r3_ad1";
            Debug.Assert(string.Equals(
                VersioningHelper.MakeVersionSafeName(s_currentAppDomainHash, ResourceScope.Process, ResourceScope.AppDomain),
                $"{s_currentAppDomainHash}{versionSuffix}"));

            // While we don't have multiple AppDomains any more, we'll still include the information
            // to keep the names in the same historical format for now.

            return $"{Application.WindowsFormsVersion}.{className}.app.0.{s_currentAppDomainHash}{versionSuffix}";
        }

        /// <summary>
        ///  Once the class name and style bits have been set, this can be called to register the class.
        /// </summary>
        private unsafe void RegisterClass()
        {
            WNDCLASSW windowClass = default;

            string? localClassName = _className;

            if (_className is null)
            {
                // If we don't use a hollow brush here, Windows will "pre paint" us with COLOR_WINDOW which
                // creates a little bit if flicker. This happens even though we are overriding wm_erasebackgnd.
                // Make this hollow to avoid all flicker.

                windowClass.hbrBackground = (HBRUSH)PInvokeCore.GetStockObject(GET_STOCK_OBJECT_FLAGS.NULL_BRUSH);
                windowClass.style = _classStyle;

                _defaultWindProc = DefaultWindowProc;
                localClassName = $"Window.{(int)_classStyle:x}";
            }
            else
            {
                // A system defined Window class was specified, get its info.
                fixed (char* n = localClassName)
                {
                    if (!PInvoke.GetClassInfo(HINSTANCE.Null, n, &windowClass))
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error(), SR.InvalidWndClsName);
                    }
                }

                localClassName = _className;
                _defaultWindProc = (nint)windowClass.lpfnWndProc;
            }

            _windowClassName = GetFullClassName(localClassName);
            _windProc = Callback;
            nint callback = Marshal.GetFunctionPointerForDelegate(_windProc);
            windowClass.lpfnWndProc = (delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, LRESULT>)callback;
            windowClass.hInstance = PInvoke.GetModuleHandle((PCWSTR)null);

            fixed (char* c = _windowClassName)
            {
                windowClass.lpszClassName = c;

                if (PInvoke.RegisterClass(&windowClass) == 0)
                {
                    _windProc = null;
                    throw new Win32Exception();
                }
            }
        }
    }
}
