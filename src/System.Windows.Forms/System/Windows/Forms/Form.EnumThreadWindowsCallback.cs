// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class Form
{
    /// <summary>
    ///  Class used to temporarily reset the owners of windows owned by this Form before its handle recreation,
    ///  then setting them back to the new handle after handle recreation.
    /// </summary>
    private class EnumThreadWindowsCallback
    {
        private List<HWND>? _ownedWindows;

        private readonly HWND _formHandle;

        internal EnumThreadWindowsCallback(HWND formHandle)
        {
            _formHandle = formHandle;
        }

        internal BOOL Callback(HWND hwnd)
        {
            HWND parent = (HWND)PInvokeCore.GetWindowLong(hwnd, WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT);
            if (parent == _formHandle)
            {
                // Enumerated window is owned by this Form.
                // Store it in a list for further treatment.
                _ownedWindows ??= [];
                _ownedWindows.Add(hwnd);
            }

            return true;
        }

        // Resets the owner of all the windows owned by this Form before handle recreation.
        internal unsafe void ResetOwners()
        {
            if (_ownedWindows is not null)
            {
                foreach (HWND hwnd in _ownedWindows)
                {
                    nint oldValue = PInvokeCore.SetWindowLong(hwnd, WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT, 0);
                    Debug.Assert(oldValue == (nint)_formHandle.Value);
                }
            }
        }

        // Sets the owner of the windows back to this Form after its handle recreation.
        internal void SetOwners(nint ownerHwnd)
        {
            if (_ownedWindows is not null)
            {
                foreach (HWND hwnd in _ownedWindows)
                {
                    PInvokeCore.SetWindowLong(hwnd, WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT, ownerHwnd);
                }
            }
        }
    }
}
