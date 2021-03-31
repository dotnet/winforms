// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime.InteropServices;
using static Interop;
using static Interop.User32;

namespace System.Windows.Forms
{
    public partial class ComboBox
    {
        /// <summary>
        ///  This finds all autocomplete windows that belong to the active thread.
        /// </summary>
        private class AutoCompleteDropDownFinder
        {
            private const string AutoCompleteClassName = "Auto-Suggest Dropdown";
            private bool _shouldSubClass;

            internal void FindDropDowns()
            {
                FindDropDowns(true);
            }

            internal void FindDropDowns(bool subclass)
            {
                if (!subclass)
                {
                    //generating a before snapshot -- lets lose the null handles
                    ACNativeWindow.ClearNullACWindows();
                }

                // Look for a popped up dropdown
                _shouldSubClass = subclass;
                EnumThreadWindows(
                    Kernel32.GetCurrentThreadId(),
                    Callback);
            }

            private BOOL Callback(IntPtr hWnd)
            {
                HandleRef hRef = new HandleRef(null, hWnd);

                // Check class name and see if it's visible
                if (GetClassName(hRef) == AutoCompleteClassName)
                {
                    ACNativeWindow.RegisterACWindow(hRef.Handle, _shouldSubClass);
                }

                return BOOL.TRUE;
            }

            private static unsafe string GetClassName(HandleRef hRef)
            {
                Span<char> buffer = stackalloc char[256];
                fixed (char* valueChars = buffer)
                {
                    int result = UnsafeNativeMethods.GetClassName(hRef, valueChars, buffer.Length);
                    if (result == 0)
                    {
                        // probably should localize this.
                        throw new Win32Exception("Failed to get window class name.");
                    }
                }

                return buffer.SliceAtFirstNull().ToString();
            }
        }
    }
}
