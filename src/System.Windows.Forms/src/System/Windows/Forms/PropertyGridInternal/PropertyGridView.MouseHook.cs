// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class PropertyGridView
    {
        internal class MouseHook
        {
            private readonly PropertyGridView gridView;
            private readonly Control control;
            private readonly IMouseHookClient client;

            internal int thisProcessID = 0;
            private GCHandle mouseHookRoot;
            private IntPtr mouseHookHandle = IntPtr.Zero;
            private bool hookDisable = false;

            private bool processing;

            public MouseHook(Control control, IMouseHookClient client, PropertyGridView gridView)
            {
                this.control = control;
                this.gridView = gridView;
                this.client = client;
#if DEBUG
                callingStack = Environment.StackTrace;
#endif
            }

#if DEBUG
            readonly string callingStack;
            ~MouseHook()
            {
                Debug.Assert(mouseHookHandle == IntPtr.Zero, "Finalizing an active mouse hook.  This will crash the process.  Calling stack: " + callingStack);
            }
#endif

            public bool DisableMouseHook
            {

                set
                {
                    hookDisable = value;
                    if (value)
                    {
                        UnhookMouse();
                    }
                }
            }

            public virtual bool HookMouseDown
            {
                get
                {
                    GC.KeepAlive(this);
                    return mouseHookHandle != IntPtr.Zero;
                }
                set
                {
                    if (value && !hookDisable)
                    {
                        HookMouse();
                    }
                    else
                    {
                        UnhookMouse();
                    }
                }
            }

            public void Dispose()
            {
                UnhookMouse();
            }

            /// <summary>
            ///  Sets up the needed windows hooks to catch messages.
            /// </summary>
            private void HookMouse()
            {
                GC.KeepAlive(this);
                // Locking 'this' here is ok since this is an internal class.
                lock (this)
                {
                    if (mouseHookHandle != IntPtr.Zero)
                    {
                        return;
                    }

                    if (thisProcessID == 0)
                    {
                        SafeNativeMethods.GetWindowThreadProcessId(new HandleRef(control, control.Handle), out thisProcessID);
                    }

                    NativeMethods.HookProc hook = new NativeMethods.HookProc(new MouseHookObject(this).Callback);
                    mouseHookRoot = GCHandle.Alloc(hook);

                    mouseHookHandle = UnsafeNativeMethods.SetWindowsHookEx(NativeMethods.WH_MOUSE,
                                                               hook,
                                                               NativeMethods.NullHandleRef,
                                                               SafeNativeMethods.GetCurrentThreadId());
                    Debug.Assert(mouseHookHandle != IntPtr.Zero, "Failed to install mouse hook");
                    Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "DropDownHolder:HookMouse()");
                }
            }

            /// <summary>
            ///  HookProc used for catch mouse messages.
            /// </summary>
            private IntPtr MouseHookProc(int nCode, IntPtr wparam, IntPtr lparam)
            {
                GC.KeepAlive(this);
                if (nCode == NativeMethods.HC_ACTION)
                {
                    NativeMethods.MOUSEHOOKSTRUCT mhs = Marshal.PtrToStructure<NativeMethods.MOUSEHOOKSTRUCT>(lparam);
                    if (mhs != null)
                    {
                        switch (unchecked((int)(long)wparam))
                        {
                            case WindowMessages.WM_LBUTTONDOWN:
                            case WindowMessages.WM_MBUTTONDOWN:
                            case WindowMessages.WM_RBUTTONDOWN:
                            case WindowMessages.WM_NCLBUTTONDOWN:
                            case WindowMessages.WM_NCMBUTTONDOWN:
                            case WindowMessages.WM_NCRBUTTONDOWN:
                            case WindowMessages.WM_MOUSEACTIVATE:
                                if (ProcessMouseDown(mhs.hWnd, mhs.pt.X, mhs.pt.Y))
                                {
                                    return (IntPtr)1;
                                }
                                break;
                        }

                    }
                }

                return UnsafeNativeMethods.CallNextHookEx(new HandleRef(this, mouseHookHandle), nCode, wparam, lparam);
            }

            /// <summary>
            ///  Removes the windowshook that was installed.
            /// </summary>
            private void UnhookMouse()
            {
                GC.KeepAlive(this);
                // Locking 'this' here is ok since this is an internal class.
                lock (this)
                {
                    if (mouseHookHandle != IntPtr.Zero)
                    {
                        UnsafeNativeMethods.UnhookWindowsHookEx(new HandleRef(this, mouseHookHandle));
                        mouseHookRoot.Free();
                        mouseHookHandle = IntPtr.Zero;
                        Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "DropDownHolder:UnhookMouse()");
                    }
                }
            }

            /*
           * Here is where we force validation on any clicks outside the
           */
            private bool ProcessMouseDown(IntPtr hWnd, int x, int y)
            {

                // if we put up the "invalid" message box, it appears this
                // method is getting called re-entrantly when it shouldn't be.
                // this prevents us from recursing.
                //
                if (processing)
                {
                    return false;
                }

                IntPtr hWndAtPoint = hWnd;
                IntPtr handle = control.Handle;
                Control ctrlAtPoint = Control.FromHandle(hWndAtPoint);

                // if it's us or one of our children, just process as normal
                //
                if (hWndAtPoint != handle && !control.Contains(ctrlAtPoint))
                {
                    Debug.Assert(thisProcessID != 0, "Didn't get our process id!");

                    // make sure the window is in our process
                    SafeNativeMethods.GetWindowThreadProcessId(new HandleRef(null, hWndAtPoint), out int pid);

                    // if this isn't our process, unhook the mouse.
                    if (pid != thisProcessID)
                    {
                        HookMouseDown = false;
                        return false;
                    }

                    bool needCommit = false;

                    // if this a sibling control (e.g. the drop down or buttons), just forward the message and skip the commit
                    needCommit = ctrlAtPoint == null ? true : !gridView.IsSiblingControl(control, ctrlAtPoint);

                    try
                    {
                        processing = true;

                        if (needCommit)
                        {
                            if (client.OnClickHooked())
                            {
                                return true; // there was an error, so eat the mouse
                            }
                            /* This breaks all sorts of stuff.  Need to find a better way to do this but we can't figure
                               out the scenario this addressed.
                            else {
                                // Returning false lets the message go to its destination.  Only
                                // return false if there is still a mouse button down.  That might not be the
                                // case if committing the entry opened a modal dialog.
                                //
                                MouseButtons state = Control.MouseButtons;
                                return (int)state == 0;
                            }
                            */
                        }
                    }
                    finally
                    {
                        processing = false;
                    }

                    // cancel our hook at this point
                    HookMouseDown = false;
                    //gridView.UnfocusSelection();
                }
                return false;
            }

            /// <summary>
            ///  Forwards messageHook calls to ToolTip.messageHookProc
            /// </summary>
            private class MouseHookObject
            {
                internal WeakReference reference;

                public MouseHookObject(MouseHook parent)
                {
                    reference = new WeakReference(parent, false);
                }

                public virtual IntPtr Callback(int nCode, IntPtr wparam, IntPtr lparam)
                {
                    IntPtr ret = IntPtr.Zero;
                    try
                    {
                        MouseHook control = (MouseHook)reference.Target;
                        if (control != null)
                        {
                            ret = control.MouseHookProc(nCode, wparam, lparam);
                        }
                    }
                    catch
                    {
                        // ignore
                    }
                    return ret;
                }
            }
        }

    }
}
