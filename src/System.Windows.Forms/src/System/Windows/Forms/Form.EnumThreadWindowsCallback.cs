// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public partial class Form
    {
        // Class used to temporarily reset the owners of windows owned by this Form
        // before its handle recreation, then setting them back to the new handle
        // after handle recreation
        private class EnumThreadWindowsCallback
        {
            private List<HandleRef> ownedWindows;

            private readonly IntPtr _formHandle;

            internal EnumThreadWindowsCallback(IntPtr formHandle)
            {
                this._formHandle = formHandle;
            }

            internal BOOL Callback(IntPtr hWnd)
            {
                HandleRef hRef = new HandleRef(null, hWnd);
                IntPtr parent = User32.GetWindowLong(hRef, User32.GWL.HWNDPARENT);
                if (parent == _formHandle)
                {
                    // Enumerated window is owned by this Form.
                    // Store it in a list for further treatment.
                    if (ownedWindows is null)
                    {
                        ownedWindows = new List<HandleRef>();
                    }

                    ownedWindows.Add(hRef);
                }

                return BOOL.TRUE;
            }

            // Resets the owner of all the windows owned by this Form before handle recreation.
            internal void ResetOwners()
            {
                if (ownedWindows != null)
                {
                    foreach (HandleRef hRef in ownedWindows)
                    {
                        User32.SetWindowLong(hRef, User32.GWL.HWNDPARENT, NativeMethods.NullHandleRef);
                    }
                }
            }

            // Sets the owner of the windows back to this Form after its handle recreation.
            internal void SetOwners(HandleRef hRefOwner)
            {
                if (ownedWindows != null)
                {
                    foreach (HandleRef hRef in ownedWindows)
                    {
                        User32.SetWindowLong(hRef, User32.GWL.HWNDPARENT, hRefOwner);
                    }
                }
            }
        }
    }
}
