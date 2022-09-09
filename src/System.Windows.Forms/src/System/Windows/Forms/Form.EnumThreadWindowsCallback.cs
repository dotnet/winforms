// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class Form
    {
        /// <summary>
        ///  Class used to temporarily reset the owners of windows owned by this Form before its handle recreation,
        ///  then setting them back to the new handle after handle recreation.
        /// </summary>
        private class EnumThreadWindowsCallback
        {
            private List<IntPtr>? _ownedWindows;

            private readonly IntPtr _formHandle;

            internal EnumThreadWindowsCallback(IntPtr formHandle)
            {
                _formHandle = formHandle;
            }

            internal BOOL Callback(IntPtr hwnd)
            {
                IntPtr parent = User32.GetWindowLong(hwnd, User32.GWL.HWNDPARENT);
                if (parent == _formHandle)
                {
                    // Enumerated window is owned by this Form.
                    // Store it in a list for further treatment.
                    _ownedWindows ??= new();
                    _ownedWindows.Add(parent);
                }

                return true;
            }

            // Resets the owner of all the windows owned by this Form before handle recreation.
            internal void ResetOwners()
            {
                if (_ownedWindows is not null)
                {
                    foreach (IntPtr hwnd in _ownedWindows)
                    {
                        User32.SetWindowLong(hwnd, User32.GWL.HWNDPARENT, 0);
                    }
                }
            }

            // Sets the owner of the windows back to this Form after its handle recreation.
            internal void SetOwners(IntPtr ownerHwnd)
            {
                if (_ownedWindows is not null)
                {
                    foreach (IntPtr hwnd in _ownedWindows)
                    {
                        User32.SetWindowLong(hwnd, User32.GWL.HWNDPARENT, ownerHwnd);
                    }
                }
            }
        }
    }
}
