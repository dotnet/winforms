// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class ToolStripPanel
    {
        private partial class FeedbackRectangle
        {
            private class FeedbackDropDown : ToolStripDropDown
            {
                private const int MaxPaintsToService = 20;
                private int _numPaintsServiced; // member variable to protect against re-entrancy

                public FeedbackDropDown(Rectangle bounds) : base()
                {
                    SetStyle(ControlStyles.AllPaintingInWmPaint, false);
                    SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                    SetStyle(ControlStyles.CacheText, true);
                    AutoClose = false;
                    AutoSize = false;
                    DropShadowEnabled = false;
                    Bounds = bounds;

                    // caching as this is unlikely to change during the lifetime
                    // of the dropdown.

                    Rectangle regionRect = bounds;    //create a region the size of the client area
                    regionRect.Inflate(-1, -1);        //squish down by one pixel

                    Region rgn = new Region(bounds);  // create region
                    rgn.Exclude(regionRect);          // exclude the center part

                    // set it into the toolstripdropdown’s region
                    Region = rgn;
                }

                // ForceSynchronousPaint - peeks through the message queue, looking for WM_PAINTs
                // calls UpdateWindow on the hwnd to force the paint to happen now.
                //
                // When we're changing the location of the feedback dropdown, we need to
                // force WM_PAINTS to happen, as things that don't respond to WM_ERASEBKGND
                // have bits of the dropdown region drawn all over them.
                private void ForceSynchronousPaint()
                {
                    if (IsDisposed || _numPaintsServiced != 0)
                    {
                        return;
                    }

                    // Protect against re-entrancy.
                    try
                    {
                        MSG msg = default(MSG);
                        while (User32.PeekMessageW(ref msg, IntPtr.Zero, User32.WM.PAINT, User32.WM.PAINT, User32.PM.REMOVE))
                        {
                            PInvoke.UpdateWindow(msg.hwnd);

                            // Infinite loop protection
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
                    // respond to everything in WM_ERASEBKGND
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
                    // if we don't force a paint here, we'll only send WM_ERASEBKGNDs right away
                    // and leave rectangles all over controls that don't respond to that window message.
                    ForceSynchronousPaint();
                }

                protected override void WndProc(ref Message m)
                {
                    if (m.MsgInternal == User32.WM.NCHITTEST)
                    {
                        m.ResultInternal = (LRESULT)(nint)User32.HT.TRANSPARENT;
                    }

                    base.WndProc(ref m);
                }
            }
        }
    }
}
