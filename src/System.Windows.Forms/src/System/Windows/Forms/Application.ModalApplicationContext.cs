// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    public sealed partial class Application
    {
        private class ModalApplicationContext : ApplicationContext
        {
            private ThreadContext parentWindowContext;

            private delegate void ThreadWindowCallback(ThreadContext context, bool onlyWinForms);

            public ModalApplicationContext(Form modalForm) : base(modalForm)
            {
            }

            public void DisableThreadWindows(bool disable, bool onlyWinForms)
            {

                Control parentControl = null;

                // Get ahold of the parent HWND -- if it's a different thread we need to do
                // do the disable over there too.  Note we only do this if we're parented by a Windows Forms
                // parent.
                //
                if (MainForm != null && MainForm.IsHandleCreated)
                {

                    // get ahold of the parenting control
                    //
                    IntPtr parentHandle = UnsafeNativeMethods.GetWindowLong(new HandleRef(this, MainForm.Handle), NativeMethods.GWL_HWNDPARENT);

                    parentControl = Control.FromHandle(parentHandle);

                    if (parentControl != null && parentControl.InvokeRequired)
                    {
                        parentWindowContext = GetContextForHandle(new HandleRef(this, parentHandle));
                    }
                    else
                    {
                        parentWindowContext = null;
                    }
                }

                // if we got a thread context, that means our parent is in a different thread, make the call on that thread.
                //
                if (parentWindowContext != null)
                {

                    // in case we've already torn down, ask the context for this.
                    //
                    if (parentControl == null)
                    {

                        parentControl = parentWindowContext.ApplicationContext.MainForm;
                    }

                    if (disable)
                    {
                        parentControl.Invoke(new ThreadWindowCallback(DisableThreadWindowsCallback), new object[] { parentWindowContext, onlyWinForms });
                    }
                    else
                    {
                        parentControl.Invoke(new ThreadWindowCallback(EnableThreadWindowsCallback), new object[] { parentWindowContext, onlyWinForms });
                    }
                }
            }

            private void DisableThreadWindowsCallback(ThreadContext context, bool onlyWinForms)
            {
                context.DisableWindowsForModalLoop(onlyWinForms, this);
            }

            private void EnableThreadWindowsCallback(ThreadContext context, bool onlyWinForms)
            {
                context.EnableWindowsForModalLoop(onlyWinForms, this);
            }

            protected override void ExitThreadCore()
            {
                // do nothing... modal dialogs exit by setting dialog result
            }
        }
    }
}
