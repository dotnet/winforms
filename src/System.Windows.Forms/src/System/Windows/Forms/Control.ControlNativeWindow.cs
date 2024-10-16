// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace System.Windows.Forms;

public partial class Control
{
    internal sealed class ControlNativeWindow : NativeWindow, IWindowTarget
    {
        private readonly Control _control;
        private GCHandle _rootRef;   // We will root the control when we do not want to be eligible for garbage collection.

        internal ControlNativeWindow(Control control)
        {
            _control = control.OrThrowIfNull();
            WindowTarget = this;
        }

        internal Control GetControl()
        {
            return _control;
        }

        protected override void OnHandleChange()
        {
            WindowTarget.OnHandleChange(Handle);
        }

        // IWindowTarget method
        public void OnHandleChange(IntPtr newHandle)
        {
            _control.SetHandle(newHandle);
        }

        internal void LockReference(bool locked)
        {
            if (locked)
            {
                if (!_rootRef.IsAllocated)
                {
                    _rootRef = GCHandle.Alloc(GetControl(), GCHandleType.Normal);
                }
            }
            else
            {
                if (_rootRef.IsAllocated)
                {
                    _rootRef.Free();
                }
            }
        }

        protected override void OnThreadException(Exception e)
        {
            if (!_control.SuppressApplicationOnThreadException(e))
            {
                Application.OnThreadException(e);
            }
        }

        // IWindowTarget method
        public void OnMessage(ref Message m)
        {
            _control.WndProc(ref m);
        }

        internal IWindowTarget WindowTarget { get; set; }

#if DEBUG
        // We override ToString so in debug asserts that fire for
        // non-released controls will show what control wasn't released.
        public override string ToString()
        {
            return _control.GetType().FullName!;
        }
#endif

        protected override void WndProc(ref Message m)
        {
            // There are certain messages that we want to process
            // regardless of what window target we are using. These
            // messages cause other messages or state transitions
            // to occur within control.
            switch (m.MsgInternal)
            {
                case PInvokeCore.WM_MOUSELEAVE:
                    _control.UnhookMouseEvent();
                    break;

                case PInvokeCore.WM_MOUSEMOVE:
                    if (!_control.GetState(States.TrackingMouseEvent))
                    {
                        _control.HookMouseEvent();
                        if (!_control.GetState(States.MouseEnterPending))
                        {
                            PInvokeCore.SendMessage(_control, RegisteredMessage.WM_MOUSEENTER);
                        }
                        else
                        {
                            _control.SetState(States.MouseEnterPending, false);
                        }
                    }

                    break;

                case PInvokeCore.WM_MOUSEWHEEL:
                    // TrackMouseEvent's mousehover implementation doesn't watch the wheel
                    // correctly...
                    _control.ResetMouseEventArgs();
                    break;
            }

            WindowTarget.OnMessage(ref m);
        }
    }
}
