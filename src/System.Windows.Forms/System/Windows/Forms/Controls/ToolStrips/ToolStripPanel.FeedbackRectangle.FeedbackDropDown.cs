// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

public partial class ToolStripPanel
{
    private partial class FeedbackRectangle
    {
        private class FeedbackDropDown : ToolStripDropDown
        {
            private const int MaxPaintsToService = 20;
            private int _numPaintsServiced; // member variable to protect against reentrancy

            public FeedbackDropDown(Rectangle bounds) : base()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint, false);
                SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                SetStyle(ControlStyles.CacheText, true);
                AutoClose = false;
                AutoSize = false;
                DropShadowEnabled = false;
                Bounds = bounds;

                // Set a clipping region with the center excluded.
                Region region = new(bounds);
                bounds.Inflate(-1, -1);
                region.Exclude(bounds);
                Region = region;
            }

            // ForceSynchronousPaint - peeks through the message queue, looking for WM_PAINTs
            // calls UpdateWindow on the hwnd to force the paint to happen now.
            //
            // When we're changing the location of the feedback dropdown, we need to
            // force WM_PAINTS to happen, as things that don't respond to WM_ERASEBKGND
            // have bits of the dropdown region drawn all over them.
            private unsafe void ForceSynchronousPaint()
            {
                if (IsDisposed || _numPaintsServiced != 0)
                {
                    return;
                }

                // Protect against reentrancy.
                try
                {
                    MSG msg = default;
                    while (PInvokeCore.PeekMessage(
                        &msg,
                        HWND.Null,
                        PInvokeCore.WM_PAINT,
                        PInvokeCore.WM_PAINT,
                        PEEK_MESSAGE_REMOVE_TYPE.PM_REMOVE))
                    {
                        PInvoke.UpdateWindow(msg.hwnd);

                        // Infinite loop protection.
                        if (_numPaintsServiced++ > MaxPaintsToService)
                        {
                            Debug.Fail("Somehow we've gotten ourself in a situation where we're pumping an unreasonable number of paint messages, investigate.");
                            break;
                        }
                    }
                }
                finally
                {
                    _numPaintsServiced = 0;
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
                // Respond to everything in WM_ERASEBKGND.
                Renderer.DrawToolStripBackground(new ToolStripRenderEventArgs(e.Graphics, this));
                Renderer.DrawToolStripBorder(new ToolStripRenderEventArgs(e.Graphics, this));
            }

            protected override void OnOpening(CancelEventArgs e)
            {
                base.OnOpening(e);
                e.Cancel = false;
            }

            public void MoveTo(Point newLocation)
            {
                Location = newLocation;

                // If we don't force a paint here, we'll only send WM_ERASEBKGNDs right away
                // and leave rectangles all over controls that don't respond to that window message.
                ForceSynchronousPaint();
            }

            protected override void WndProc(ref Message m)
            {
                if (m.MsgInternal == PInvokeCore.WM_NCHITTEST)
                {
                    m.ResultInternal = (LRESULT)PInvoke.HTTRANSPARENT;
                }

                base.WndProc(ref m);
            }
        }
    }
}
