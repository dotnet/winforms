﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.Design.Behavior;
using static Interop;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  The ScrollableControlDesigner class builds on the ParentControlDesigner, and adds the implementation
    ///  of IWinFormsDesigner so that the designer can be hosted as a document.
    /// </summary>
    public class ScrollableControlDesigner : ParentControlDesigner
    {
        private SelectionManager selManager;

        /// <summary>
        ///  Overrides the base class's GetHitTest method to determine regions of the
        ///  control that should always be UI-Active.  For a form, if it has autoscroll
        ///  set the scroll bars are always UI active.
        /// </summary>
        protected override bool GetHitTest(Point pt)
        {
            if (base.GetHitTest(pt))
            {
                return true;
            }

            // The scroll bars on a form are "live"
            //
            ScrollableControl f = (ScrollableControl)Control;
            if (f.IsHandleCreated && f.AutoScroll)
            {
                int hitTest = (int)(long)User32.SendMessageW(f.Handle, User32.WM.NCHITTEST, IntPtr.Zero, PARAM.FromLowHigh(pt.X, pt.Y));
                if (hitTest == (int)User32.HT.VSCROLL || hitTest == (int)User32.HT.HSCROLL)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///  We override our base class's WndProc to monitor certain messages.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            switch (m.Msg)
            {
                case (int)User32.WM.HSCROLL:
                case (int)User32.WM.VSCROLL:

                    // When we scroll, we reposition a control without causing a
                    // property change event.  Therefore, we must tell the
                    // SelectionManager to refresh its glyphs.
                    if (selManager == null)
                    {
                        selManager = GetService(typeof(SelectionManager)) as SelectionManager;
                    }

                    if (selManager != null)
                    {
                        selManager.Refresh();
                    }

                    // Now we must paint our adornments, since the scroll does not
                    // trigger a paint event
                    //
                    Control.Invalidate();
                    Control.Update();
                    break;
            }
        }
    }
}
