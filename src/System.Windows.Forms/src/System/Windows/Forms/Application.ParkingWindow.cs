// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

public sealed partial class Application
{
    /// <summary>
    ///  This class embodies our parking window, which we create when the
    ///  first message loop is pushed onto the thread.
    /// </summary>
    internal sealed class ParkingWindow : ContainerControl, IArrangedElement
    {
        // In .NET 2.0 we now aggressively tear down the parking window
        //   when the last control has been removed off of it.

        private const int WM_CHECKDESTROY = (int)PInvokeCore.WM_USER + 0x01;

        private int _childCount;

        public ParkingWindow()
        {
            SetExtendedState(ExtendedStates.InterestedInUserPreferenceChanged, false);
            SetState(States.TopLevel, true);
            Text = "WindowsFormsParkingWindow";
            Visible = false;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                // Message only windows are cheaper and have fewer issues than full blown invisible windows.
                cp.Parent = HWND.HWND_MESSAGE;
                return cp;
            }
        }

        internal override void AddReflectChild()
        {
            if (_childCount < 0)
            {
                Debug.Fail("How did parkingwindow childcount go negative???");
                _childCount = 0;
            }

            _childCount++;
        }

        internal override void RemoveReflectChild()
        {
            if (--_childCount < 0)
            {
                Debug.Fail("How did parkingwindow childcount go negative???");
                _childCount = 0;
            }

            if (_childCount != 0 || !IsHandleCreated)
            {
                return;
            }

            // Check to see if we are running on the thread that owns the parkingwindow.
            // If so, we can destroy immediately.
            //
            // This is important for scenarios where apps leak controls until after the
            // messagepump is gone and then decide to clean them up. We should clean
            // up the parkingwindow in this case and a postmessage won't do it.
            uint id = PInvoke.GetWindowThreadProcessId(HWNDInternal, out _);
            ThreadContext? context = ThreadContext.FromId(id);

            // We only do this if the ThreadContext tells us that we are currently handling a window message.
            if (context is null || !ReferenceEquals(context, ThreadContext.FromCurrent()))
            {
                PInvokeCore.PostMessage(HWNDInternal, WM_CHECKDESTROY);
            }
            else
            {
                CheckDestroy();
            }
        }

        private void CheckDestroy()
        {
            if (_childCount != 0)
                return;

            HWND hwndChild = PInvoke.GetWindow(this, GET_WINDOW_CMD.GW_CHILD);
            if (hwndChild.IsNull)
            {
                DestroyHandle();
            }
        }

        public void Destroy()
        {
            DestroyHandle();
        }

        /// <summary>
        ///  "Parks" the given HWND to a temporary HWND. This allows WS_CHILD windows to be parked.
        /// </summary>
        internal void ParkHandle<T>(T handle) where T : IHandle<HWND>
        {
            if (!IsHandleCreated)
            {
                CreateHandle();
            }

            PInvoke.SetParent(handle, (IHandle<HWND>)this);
        }

        /// <summary>
        ///  "Unparks" the given HWND to a temporary HWND. This allows WS_CHILD windows to be parked.
        /// </summary>
        internal void UnparkHandle<T>(T handle) where T : IHandle<HWND>
        {
            if (!IsHandleCreated)
            {
                return;
            }

            Debug.Assert(
                PInvoke.GetParent(handle) != HWND,
                "Always set the handle's parent to someone else before calling UnparkHandle");

            // If there are no child windows in this handle any longer, destroy the parking window.
            CheckDestroy();
        }

        // Do nothing on layout to reduce the calls into the LayoutEngine while debugging.
        protected override void OnLayout(LayoutEventArgs levent) { }
        void IArrangedElement.PerformLayout(IArrangedElement affectedElement, string? affectedProperty) { }

        protected override void WndProc(ref Message m)
        {
            if (m.MsgInternal == PInvokeCore.WM_SHOWWINDOW)
                return;

            base.WndProc(ref m);
            switch (m.MsgInternal)
            {
                case PInvokeCore.WM_PARENTNOTIFY:
                    if (m.WParamInternal.LOWORD == PInvokeCore.WM_DESTROY)
                    {
                        PInvokeCore.PostMessage(this, WM_CHECKDESTROY);
                    }

                    break;
                case WM_CHECKDESTROY:
                    CheckDestroy();
                    break;
            }
        }
    }
}
