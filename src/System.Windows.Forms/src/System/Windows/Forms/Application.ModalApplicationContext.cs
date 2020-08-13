// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public sealed partial class Application
    {
        private class ModalApplicationContext : ApplicationContext
        {
            private ThreadContext _parentWindowContext;

            private delegate void ThreadWindowCallback(ThreadContext context, bool onlyWinForms);

            public ModalApplicationContext(Form modalForm) : base(modalForm)
            {
            }

            public void DisableThreadWindows(bool disable, bool onlyWinForms)
            {
                Control parentControl = null;

                // Get ahold of the parent HWND -- if it's a different thread we need to do the disable
                // over there too.  Note we only do this if we're parented by a Windows Forms parent.

                if (MainForm != null && MainForm.IsHandleCreated)
                {
                    // Get ahold of the parenting control
                    IntPtr parentHandle = Interop.User32.GetWindowLong(new HandleRef(this, MainForm.Handle), User32.GWL.HWNDPARENT);

                    parentControl = Control.FromHandle(parentHandle);

                    _parentWindowContext = parentControl != null && parentControl.InvokeRequired
                        ? GetContextForHandle(new HandleRef(this, parentHandle)) : null;
                }

                // If we got a thread context, that means our parent is in a different thread, make the call on that thread.
                if (_parentWindowContext != null)
                {
                    // In case we've already torn down, ask the context for this.
                    if (parentControl is null)
                    {
                        parentControl = _parentWindowContext.ApplicationContext.MainForm;
                    }

                    parentControl.Invoke(
                        disable ? new ThreadWindowCallback(DisableThreadWindowsCallback) : new ThreadWindowCallback(EnableThreadWindowsCallback),
                        new object[] { _parentWindowContext, onlyWinForms });
                }
            }

            private void DisableThreadWindowsCallback(ThreadContext context, bool onlyWinForms)
                => context.DisableWindowsForModalLoop(onlyWinForms, this);

            private void EnableThreadWindowsCallback(ThreadContext context, bool onlyWinForms)
                => context.EnableWindowsForModalLoop(onlyWinForms, this);

            protected override void ExitThreadCore()
            {
                // do nothing... modal dialogs exit by setting dialog result
            }
        }
    }
}
