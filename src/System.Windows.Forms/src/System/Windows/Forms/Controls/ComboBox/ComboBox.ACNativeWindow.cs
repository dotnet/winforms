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
        private static readonly Dictionary<HWND, ACNativeWindow?> s_ACWindows = new();

        internal ACNativeWindow(HWND acHandle)
        {
            Debug.Assert(!s_ACWindows.ContainsKey(acHandle));
            AssignHandle(acHandle);
            s_ACWindows.Add(acHandle, this);
            PInvoke.EnumChildWindows(new HandleRef<HWND>(this, acHandle), RegisterACWindowRecursive);
        }

        private static BOOL RegisterACWindowRecursive(HWND handle)
        {
            if (!s_ACWindows.ContainsKey(handle))
            {
                ACNativeWindow newAC = new(handle);
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

            if (m.MsgInternal == PInvoke.WM_NCDESTROY)
            {
                Debug.Assert(s_ACWindows.ContainsKey(HWND));
                s_ACWindows.Remove(HWND);   // so we do not leak ac windows.
            }
        }

        internal static void RegisterACWindow(HWND acHandle, bool subclass)
        {
            if (subclass && s_ACWindows.TryGetValue(acHandle, out ACNativeWindow? value))
            {
                if (value is null)
                {
                    s_ACWindows.Remove(acHandle); // if an external handle got destroyed, don't let it stop us.
                }
            }

            if (!s_ACWindows.ContainsKey(acHandle))
            {
                if (subclass)
                {
                    ACNativeWindow newAC = new(acHandle);
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
            List<HWND> toRemove = new();
            foreach (var acNativeWindowByHandle in s_ACWindows)
            {
                if (acNativeWindowByHandle.Value is null)
                {
                    toRemove.Add(acNativeWindowByHandle.Key);
                }
            }

            foreach (HWND handle in toRemove)
            {
                s_ACWindows.Remove(handle);
            }
        }
    }
}
