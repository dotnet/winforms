// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

namespace System.Windows.Forms;

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
                // generating a before snapshot -- lets lose the null handles
                ACNativeWindow.ClearNullACWindows();
            }

            // Look for a popped up dropdown
            _shouldSubClass = subclass;
            PInvokeCore.EnumCurrentThreadWindows(Callback);
        }

        [SkipLocalsInit]
        private unsafe BOOL Callback(HWND hwnd)
        {
            Span<char> buffer = stackalloc char[AutoCompleteClassName.Length + 2];
            fixed (char* b = buffer)
            {
                int length = PInvoke.GetClassName(hwnd, (PWSTR)b, buffer.Length);

                // Check class name and see if it's visible
                if (length == AutoCompleteClassName.Length && buffer.StartsWith(AutoCompleteClassName))
                {
                    ACNativeWindow.RegisterACWindow(hwnd, _shouldSubClass);
                }
            }

            return true;
        }
    }
}
