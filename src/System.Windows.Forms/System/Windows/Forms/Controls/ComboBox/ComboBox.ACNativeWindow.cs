// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class ComboBox
{
    /// <summary>
    ///  This subclasses an autocomplete window so that we can determine if control is inside the AC wndproc.
    /// </summary>
    private sealed class ACNativeWindow : NativeWindow
    {
        internal static int s_inWndProcCnt;

        // This dictionary can contain null for those ACWindows we find, but are sure are not ours.
        private static readonly Dictionary<HWND, ACNativeWindow?> s_acWindows = [];

        internal ACNativeWindow(HWND acHandle)
        {
            Debug.Assert(!s_acWindows.ContainsKey(acHandle));
            AssignHandle(acHandle);
            s_acWindows.Add(acHandle, this);
            PInvokeCore.EnumChildWindows(new HandleRef<HWND>(this, acHandle), RegisterACWindowRecursive);
        }

        private static BOOL RegisterACWindowRecursive(HWND handle)
        {
            if (!s_acWindows.ContainsKey(handle))
            {
                _ = new ACNativeWindow(handle);
            }

            return true;
        }

        internal bool Visible => PInvoke.IsWindowVisible(this);

        internal static bool AutoCompleteActive
        {
            get
            {
                if (s_inWndProcCnt > 0)
                {
                    return true;
                }

                foreach (ACNativeWindow? window in s_acWindows.Values)
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

            if (m.MsgInternal == PInvokeCore.WM_NCDESTROY)
            {
                Debug.Assert(s_acWindows.ContainsKey(HWND));
                s_acWindows.Remove(HWND);   // so we do not leak ac windows.
            }
        }

        internal static void RegisterACWindow(HWND acHandle, bool subclass)
        {
            if (subclass && s_acWindows.TryGetValue(acHandle, out ACNativeWindow? value))
            {
                if (value is null)
                {
                    s_acWindows.Remove(acHandle); // if an external handle got destroyed, don't let it stop us.
                }
            }

            if (!s_acWindows.ContainsKey(acHandle))
            {
                if (subclass)
                {
                    new ACNativeWindow(acHandle);
                }
                else
                {
                    s_acWindows.Add(acHandle, null);
                }
            }
        }

        /// <summary>
        ///  This method clears out null entries so we get a clean BEFORE and AFTER snapshot
        ///  null entries are ACWindows that belong to someone else.
        /// </summary>
        internal static void ClearNullACWindows()
        {
            List<HWND> toRemove = [];
            foreach (var acNativeWindowByHandle in s_acWindows)
            {
                if (acNativeWindowByHandle.Value is null)
                {
                    toRemove.Add(acNativeWindowByHandle.Key);
                }
            }

            foreach (HWND handle in toRemove)
            {
                s_acWindows.Remove(handle);
            }
        }
    }
}
