// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public sealed partial class NotifyIcon
    {
        /// <summary>
        ///  Defines a placeholder window that the NotifyIcon is attached to.
        /// </summary>
        private class NotifyIconNativeWindow : NativeWindow
        {
            internal NotifyIcon reference;
            private GCHandle rootRef;   // We will root the control when we do not want to be eligible for garbage collection.

            /// <summary>
            ///  Create a new NotifyIcon, and bind the window to the NotifyIcon component.
            /// </summary>
            internal NotifyIconNativeWindow(NotifyIcon component)
            {
                reference = component;
            }

            ~NotifyIconNativeWindow()
            {
                // This same post is done in Control's Dispose method, so if you change
                // it, change it there too.
                //
                if (Handle != IntPtr.Zero)
                {
                    User32.PostMessageW(this, User32.WM.CLOSE);
                }

                // This releases the handle from our window proc, re-routing it back to
                // the system.
            }

            public void LockReference(bool locked)
            {
                if (locked)
                {
                    if (!rootRef.IsAllocated)
                    {
                        rootRef = GCHandle.Alloc(reference, GCHandleType.Normal);
                    }
                }
                else
                {
                    if (rootRef.IsAllocated)
                    {
                        rootRef.Free();
                    }
                }
            }

            protected override void OnThreadException(Exception e)
            {
                Application.OnThreadException(e);
            }

            /// <summary>
            ///  Pass messages on to the NotifyIcon object's wndproc handler.
            /// </summary>
            protected override void WndProc(ref Message m)
            {
                Debug.Assert(reference is not null, "NotifyIcon was garbage collected while it was still visible.  How did we let that happen?");
                reference.WndProc(ref m);
            }
        }
    }
}
