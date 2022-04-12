// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using static Interop;
using static Interop.User32;

namespace System.Windows.Forms
{
    public partial class ComboBox
    {
        /// <summary>
        ///  This subclasses an autocomplete window so that we can determine if control is inside the AC wndproc.
        /// </summary>
        private sealed class ACNativeWindow : NativeWindow
        {
            internal static int s_inWndProcCnt;

            // This dictionary can contain null for those ACWindows we find, but are sure are not ours.
            private static readonly Dictionary<IntPtr, ACNativeWindow?> s_ACWindows = new();

            internal ACNativeWindow(IntPtr acHandle)
            {
                Debug.Assert(!s_ACWindows.ContainsKey(acHandle));
                AssignHandle(acHandle);
                s_ACWindows.Add(acHandle, this);
                EnumChildWindows(new HandleRef(this, acHandle), RegisterACWindowRecursive);
            }

            private static BOOL RegisterACWindowRecursive(IntPtr handle)
            {
                if (!s_ACWindows.ContainsKey(handle))
                {
                    ACNativeWindow newAC = new ACNativeWindow(handle);
                }

                return BOOL.TRUE;
            }

            internal bool Visible => IsWindowVisible(this).IsTrue();

            static internal bool AutoCompleteActive
            {
                get
                {
                    if (s_inWndProcCnt > 0)
                    {
                        return true;
                    }

                    foreach (ACNativeWindow? window in s_ACWindows.Values)
                    {
                        if (window is not null && window.Visible)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }

            protected override void WndProc(ref Message m)
            {
                s_inWndProcCnt++;
                try
                {
                    base.WndProc(ref m);
                }
                finally
                {
                    s_inWndProcCnt--;
                }

                if (m.Msg == (int)WM.NCDESTROY)
                {
                    Debug.Assert(s_ACWindows.ContainsKey(Handle));
                    s_ACWindows.Remove(Handle);   //so we do not leak ac windows.
                }
            }

            internal static void RegisterACWindow(IntPtr acHandle, bool subclass)
            {
                if (subclass && s_ACWindows.ContainsKey(acHandle))
                {
                    if (s_ACWindows[acHandle] is null)
                    {
                        s_ACWindows.Remove(acHandle); //if an external handle got destroyed, don't let it stop us.
                    }
                }

                if (!s_ACWindows.ContainsKey(acHandle))
                {
                    if (subclass)
                    {
                        ACNativeWindow newAC = new ACNativeWindow(acHandle);
                    }
                    else
                    {
                        s_ACWindows.Add(acHandle, null);
                    }
                }
            }

            /// <summary>
            ///  This method clears out null entries so we get a clean BEFORE and AFTER snapshot
            ///  null entries are ACWindows that belong to someone else.
            /// </summary>
            internal static void ClearNullACWindows()
            {
                List<IntPtr> toRemove = new();
                foreach (KeyValuePair<IntPtr, ACNativeWindow?> acNativeWindowByHandle in s_ACWindows)
                {
                    if (acNativeWindowByHandle.Value is null)
                    {
                        toRemove.Add(acNativeWindowByHandle.Key);
                    }
                }

                foreach (IntPtr handle in toRemove)
                {
                    s_ACWindows.Remove(handle);
                }
            }
        }
    }
}
