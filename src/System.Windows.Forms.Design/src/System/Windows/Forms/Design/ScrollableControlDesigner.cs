// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design;

/// <summary>
///  The ScrollableControlDesigner class builds on the ParentControlDesigner, and adds the implementation
///  of IWinFormsDesigner so that the designer can be hosted as a document.
/// </summary>
public class ScrollableControlDesigner : ParentControlDesigner
{
    private SelectionManager? _selectionManager;

    /// <summary>
    ///  Overrides the base class's GetHitTest method to determine regions of the
    ///  control that should always be UI-Active. For a form, if it has autoscroll
    ///  set the scroll bars are always UI active.
    /// </summary>
    protected override bool GetHitTest(Point pt)
    {
        if (base.GetHitTest(pt))
        {
            return true;
        }

        // The scroll bars on a form are "live".
        ScrollableControl f = (ScrollableControl)Control;
        if (f.IsHandleCreated && f.AutoScroll)
        {
            int hitTest = (int)PInvokeCore.SendMessage(f, PInvokeCore.WM_NCHITTEST, 0, PARAM.FromLowHigh(pt.X, pt.Y));
            if (hitTest is ((int)PInvoke.HTVSCROLL) or ((int)PInvoke.HTHSCROLL))
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
            case (int)PInvokeCore.WM_HSCROLL:
            case (int)PInvokeCore.WM_VSCROLL:

                // When we scroll, we reposition a control without causing a
                // property change event. Therefore, we must tell the
                // SelectionManager to refresh its glyphs.
                _selectionManager ??= GetService<SelectionManager>();

                _selectionManager?.Refresh();

                // Now we must paint our adornments, since the scroll does not
                // trigger a paint event
                //
                Control.Invalidate();
                Control.Update();
                break;
        }
    }
}
