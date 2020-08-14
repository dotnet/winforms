// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using static Interop;

namespace System.Windows.Forms
{
    public partial class NativeWindow
    {
        /// <summary>
        ///  WindowClass encapsulates a Windows window class.
        /// </summary>
        private class WindowClass
        {
            internal static WindowClass s_cache;

            internal WindowClass _next;
            internal string _className;
            internal string _windowClassName;
            internal NativeWindow _targetWindow;

            private readonly User32.CS _classStyle;
            private IntPtr _defaultWindProc;

            // This needs to be a field so the GC doesn't collect the managed callback
            private User32.WNDPROC _windProc;

            // There is only ever one AppDomain
            private static readonly string s_currentAppDomainHash = Convert.ToString(AppDomain.CurrentDomain.GetHashCode(), 16);

            private static readonly object s_wcInternalSyncObject = new object();

            internal WindowClass(string className, User32.CS classStyle)
            {
                _className = className;
                _classStyle = classStyle;
                RegisterClass();
            }

            public IntPtr Callback(IntPtr hWnd, User32.WM msg, IntPtr wparam, IntPtr lparam)
            {
                Debug.Assert(hWnd != IntPtr.Zero, "Windows called us with an HWND of 0");

                // Set the window procedure to the default window procedure
                User32.SetWindowLong(hWnd, User32.GWL.WNDPROC, _defaultWindProc);
                _targetWindow.AssignHandle(hWnd);
                return _targetWindow.Callback(hWnd, msg, wparam, lparam);
            }

            /// <summary>
            ///  Retrieves a WindowClass object for use.  This will create a new
            ///  object if there is no such class/style available, or retrun a
            ///  cached object if one exists.
            /// </summary>
            internal static WindowClass Create(string className, User32.CS classStyle)
            {
                lock (s_wcInternalSyncObject)
                {
                    WindowClass wc = s_cache;
                    if (className is null)
                    {
                        // If we weren't given a class name, look for a window
                        // that has the exact class style.
                        while (wc != null
                            && (wc._className != null || wc._classStyle != classStyle))
                        {
                            wc = wc._next;
                        }
                    }
                    else
                    {
                        while (wc != null && !className.Equals(wc._className))
                        {
                            wc = wc._next;
                        }
                    }

                    if (wc is null)
                    {
                        // Didn't find an existing class, create one and attatch it to
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
            private string GetFullClassName(string className)
            {
                StringBuilder b = new StringBuilder(50);
                b.Append(Application.WindowsFormsVersion);
                b.Append('.');
                b.Append(className);

                // While we don't have multiple AppDomains any more, we'll still include the information
                // to keep the names in the same historical format for now.

                b.Append(".app.0.");

                // VersioningHelper does a lot of string allocations, and on .NET Core for our purposes
                // it always returns the exact same string (process is hardcoded to r3 and the AppDomain
                // id is always 1 as there is only one AppDomain).

                const string versionSuffix = "_r3_ad1";
                Debug.Assert(string.Equals(
                    VersioningHelper.MakeVersionSafeName(s_currentAppDomainHash, ResourceScope.Process, ResourceScope.AppDomain),
                    s_currentAppDomainHash + versionSuffix));
                b.Append(s_currentAppDomainHash);
                b.Append(versionSuffix);

                return b.ToString();
            }

            /// <summary>
            ///  Once the classname and style bits have been set, this can be called to register the class.
            /// </summary>
            private unsafe void RegisterClass()
            {
                User32.WNDCLASS windowClass = new User32.WNDCLASS();

                string localClassName = _className;

                if (localClassName is null)
                {
                    // If we don't use a hollow brush here, Windows will "pre paint" us with COLOR_WINDOW which
                    // creates a little bit if flicker.  This happens even though we are overriding wm_erasebackgnd.
                    // Make this hollow to avoid all flicker.

                    windowClass.hbrBackground = (Gdi32.HBRUSH)Gdi32.GetStockObject(Gdi32.StockObject.NULL_BRUSH);
                    windowClass.style = _classStyle;

                    _defaultWindProc = DefaultWindowProc;
                    localClassName = "Window." + Convert.ToString((int)_classStyle, 16);
                }
                else
                {
                    // A system defined Window class was specified, get its info.
                    if (User32.GetClassInfoW(NativeMethods.NullHandleRef, _className, ref windowClass).IsFalse())
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error(), SR.InvalidWndClsName);
                    }

                    localClassName = _className;
                    _defaultWindProc = windowClass.lpfnWndProc;
                }

                _windowClassName = GetFullClassName(localClassName);
                _windProc = new User32.WNDPROC(Callback);
                windowClass.lpfnWndProc = Marshal.GetFunctionPointerForDelegate(_windProc);
                windowClass.hInstance = Kernel32.GetModuleHandleW(null);

                fixed (char* c = _windowClassName)
                {
                    windowClass.lpszClassName = c;

                    if (User32.RegisterClassW(ref windowClass) == 0)
                    {
                        _windProc = null;
                        throw new Win32Exception();
                    }
                }
            }
        }
    }
}
