// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Design.Behavior;

public sealed partial class BehaviorService
{
    /// <summary>
    ///  The AdornerWindow is a transparent window that resides ontop of the Designer's Frame. This window is used
    ///  by the BehaviorService to intercept all messages. It also serves as a unified canvas on which to paint Glyphs.
    /// </summary>
    private partial class AdornerWindow : Control
    {
        private readonly BehaviorService _behaviorService;
        private static MouseHook? s_mouseHook;
        private static readonly List<AdornerWindow> s_adornerWindowList = [];

        /// <summary>
        ///  Constructor that parents itself to the Designer Frame and hooks all
        ///  necessary events.
        /// </summary>
        internal AdornerWindow(BehaviorService behaviorService, DesignerFrame designerFrame)
        {
            _behaviorService = behaviorService;
            DesignerFrame = designerFrame;
            Dock = DockStyle.Fill;
            AllowDrop = true;
            Text = "AdornerWindow";
            SetStyle(ControlStyles.Opaque, true);
        }

        /// <summary>
        ///  The key here is to set the appropriate TransparentWindow style.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~(int)(WINDOW_STYLE.WS_CLIPCHILDREN | WINDOW_STYLE.WS_CLIPSIBLINGS);
                cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_TRANSPARENT;
                return cp;
            }
        }

        internal bool ProcessingDrag { get; set; }

        /// <summary>
        ///  We'll use CreateHandle as our notification for creating our mouse attacher.
        /// </summary>
        [MemberNotNull(nameof(s_mouseHook))]
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            s_adornerWindowList.Add(this);
            s_mouseHook ??= new MouseHook();
        }

        /// <summary>
        ///  Unhook and null out our mouseHook.
        /// </summary>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            s_adornerWindowList.Remove(this);

            // Unregister the mouse hook once all adorner windows have been disposed.
            if (s_adornerWindowList.Count == 0 && s_mouseHook is not null)
            {
                s_mouseHook.Dispose();
                s_mouseHook = null;
            }

            base.OnHandleDestroyed(e);
        }

        /// <summary>
        ///  Null out our mouseHook and unhook any events.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && DesignerFrame is not null)
            {
                DesignerFrame = null!;
            }

            base.Dispose(disposing);
        }

        internal DesignerFrame DesignerFrame { get; private set; }

        /// <summary>
        ///  Returns the display rectangle for the adorner window
        /// </summary>
        internal Rectangle DesignerFrameDisplayRectangle
            => DesignerFrameValid ? DesignerFrame.DisplayRectangle : Rectangle.Empty;

        /// <summary>
        ///  Returns true if the DesignerFrame is created and not being disposed.
        /// </summary>
        internal bool DesignerFrameValid
            => DesignerFrame is not null && !DesignerFrame.IsDisposed && DesignerFrame.IsHandleCreated;

        /// <summary>
        ///  Ultimately called by ControlDesigner when it receives a DragDrop message - here, we'll exit from 'drag mode'.
        /// </summary>
        internal void EndDragNotification() => ProcessingDrag = false;

        /// <summary>
        ///  Invalidates the transparent AdornerWindow by asking the Designer Frame beneath it to invalidate.
        ///  Note the they use of the .Update() call for perf. purposes.
        /// </summary>
        internal void InvalidateAdornerWindow()
        {
            if (DesignerFrameValid)
            {
                DesignerFrame.Invalidate(true);
                DesignerFrame.Update();
            }
        }

        /// <summary>
        ///  Invalidates the transparent AdornerWindow by asking the Designer Frame beneath it to invalidate.
        ///  Note the they use of the .Update() call for perf. purposes.
        /// </summary>
        internal void InvalidateAdornerWindow(Region region)
        {
            if (DesignerFrameValid)
            {
                // Translate for non-zero scroll positions
                Point scrollPosition = DesignerFrame.AutoScrollPosition;
                region.Translate(scrollPosition.X, scrollPosition.Y);

                DesignerFrame.Invalidate(region, true);
                DesignerFrame.Update();
            }
        }

        /// <summary>
        ///  Invalidates the transparent AdornerWindow by asking the Designer Frame beneath it to invalidate.
        ///  Note the they use of the .Update() call for perf. purposes.
        /// </summary>
        internal void InvalidateAdornerWindow(Rectangle rectangle)
        {
            if (DesignerFrameValid)
            {
                // Translate for non-zero scroll positions
                Point scrollPosition = DesignerFrame.AutoScrollPosition;
                rectangle.Offset(scrollPosition.X, scrollPosition.Y);

                DesignerFrame.Invalidate(rectangle, true);
                DesignerFrame.Update();
            }
        }

        /// <summary>
        ///  The AdornerWindow hooks all Drag/Drop notification so they can be forwarded to the appropriate
        ///  Behavior via the BehaviorService.
        /// </summary>
        protected override void OnDragDrop(DragEventArgs e)
        {
            try
            {
                _behaviorService.OnDragDrop(e);
            }
            finally
            {
                ProcessingDrag = false;
            }
        }

        private static bool IsLocalDrag(DragEventArgs e)
        {
            if (e.Data is DropSourceBehavior.BehaviorDataObject)
            {
                return true;
            }
            else
            {
                // Gets all the data formats and data conversion formats in the data object.
                string[] allFormats = e.Data!.GetFormats();

                for (int i = 0; i < allFormats.Length; i++)
                {
                    if (string.Equals(ToolboxFormat, allFormats[i]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///  The AdornerWindow hooks all Drag/Drop notification so they can be forwarded to the appropriate
        ///  Behavior via the BehaviorService.
        /// </summary>
        protected override void OnDragEnter(DragEventArgs e)
        {
            ProcessingDrag = true;

            // Determine if this is a local drag, if it is, do normal processing otherwise, force a
            // PropagateHitTest. We need to force this because the OLE D&D service suspends mouse messages
            // when the drag is not local so the mouse hook never sees them.
            if (!IsLocalDrag(e))
            {
                _behaviorService._validDragArgs = e;
                PInvoke.GetCursorPos(out Point point);
                point = PointToClient(point);
                _behaviorService.PropagateHitTest(point);
            }

            _behaviorService.OnDragEnter(null, e);
        }

        /// <summary>
        ///  The AdornerWindow hooks all Drag/Drop notification so they can be forwarded to the appropriate
        ///  Behavior via the BehaviorService.
        /// </summary>
        protected override void OnDragLeave(EventArgs e)
        {
            // set our dragArgs to null so we know not to send drag enter/leave events when we re-enter the dragging area
            _behaviorService._validDragArgs = null;
            try
            {
                _behaviorService.OnDragLeave(null, e);
            }
            finally
            {
                ProcessingDrag = false;
            }
        }

        /// <summary>
        ///  The AdornerWindow hooks all Drag/Drop notification so they can be forwarded to the appropriate
        ///  Behavior via the BehaviorService.
        /// </summary>
        protected override void OnDragOver(DragEventArgs e)
        {
            ProcessingDrag = true;
            if (!IsLocalDrag(e))
            {
                _behaviorService._validDragArgs = e;
                PInvoke.GetCursorPos(out Point point);
                point = PointToClient(point);
                _behaviorService.PropagateHitTest(point);
            }

            _behaviorService.OnDragOver(e);
        }

        /// <summary>
        ///  The AdornerWindow hooks all Drag/Drop notification so they can be forwarded to the appropriate
        ///  Behavior via the BehaviorService.
        /// </summary>
        protected override void OnGiveFeedback(GiveFeedbackEventArgs e) => _behaviorService.OnGiveFeedback(e);

        /// <summary>
        ///  The AdornerWindow hooks all Drag/Drop notification so they can be forwarded to the appropriate
        ///  Behavior via the BehaviorService.
        /// </summary>
        protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e) => _behaviorService.OnQueryContinueDrag(e);

        /// <summary>
        ///  Called by ControlDesigner when it receives a DragEnter message - we'll let listen to all Mouse
        ///  Messages so we can send drag notifications.
        /// </summary>
        internal void StartDragNotification() => ProcessingDrag = true;

        /// <summary>
        ///  The AdornerWindow intercepts all designer-related messages and forwards them to the BehaviorService
        ///  for appropriate actions. Note that Paint and HitTest messages are correctly parsed and translated
        ///  to AdornerWindow coords.
        /// </summary>
        protected override unsafe void WndProc(ref Message m)
        {
            // special test hooks
            if (m.Msg == (int)WM_GETALLSNAPLINES)
            {
                _behaviorService.TestHook_GetAllSnapLines(ref m);
            }
            else if (m.Msg == (int)WM_GETRECENTSNAPLINES)
            {
                _behaviorService.TestHook_GetRecentSnapLines(ref m);
            }

            switch (m.MsgInternal)
            {
                case PInvokeCore.WM_PAINT:
                    {
                        // Stash off the region we have to update.
                        using RegionScope hrgn = new(0, 0, 0, 0);
                        PInvoke.GetUpdateRgn(m.HWND, hrgn, true);

                        // The region we have to update in terms of the smallest rectangle that completely encloses
                        // the update region of the window gives us the clip rectangle.
                        RECT clip = default;
                        PInvoke.GetUpdateRect(m.HWND, &clip, true);
                        Rectangle paintRect = clip;

                        using Region region = hrgn.ToRegion();

                        // Call the base class to do its painting.
                        DefWndProc(ref m);

                        // Now do our own painting.
                        using Graphics g = Graphics.FromHwnd(m.HWnd);
                        using PaintEventArgs pevent = new(g, paintRect);
                        g.Clip = region;
                        _behaviorService.PropagatePaint(pevent);

                        break;
                    }

                case PInvokeCore.WM_NCHITTEST:
                    Point pt = PARAM.ToPoint(m.LParamInternal);
                    Point pt1 = PointToClient(default);
                    pt.Offset(pt1.X, pt1.Y);

                    m.ResultInternal = _behaviorService.PropagateHitTest(pt) && !ProcessingDrag
                        ? (LRESULT)PInvoke.HTTRANSPARENT
                        : (LRESULT)(int)PInvoke.HTCLIENT;

                    break;

                case PInvokeCore.WM_CAPTURECHANGED:
                    base.WndProc(ref m);
                    _behaviorService.OnLoseCapture();
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        /// <summary>
        ///  Called by our mouseHook when it spies a mouse message that the adornerWindow would be interested in.
        ///  Returning 'true' signifies that the message was processed and should not continue to child windows.
        /// </summary>
        private bool WndProcProxy(ref Message m, int x, int y)
        {
            Point mouseLoc = new(x, y);
            _behaviorService.PropagateHitTest(mouseLoc);
            switch (m.MsgInternal)
            {
                case PInvokeCore.WM_LBUTTONDOWN:
                    if (_behaviorService.OnMouseDown(MouseButtons.Left, mouseLoc))
                    {
                        return false;
                    }

                    break;

                case PInvokeCore.WM_RBUTTONDOWN:
                    if (_behaviorService.OnMouseDown(MouseButtons.Right, mouseLoc))
                    {
                        return false;
                    }

                    break;

                case PInvokeCore.WM_MOUSEMOVE:
                    if (_behaviorService.OnMouseMove(MouseButtons, mouseLoc))
                    {
                        return false;
                    }

                    break;

                case PInvokeCore.WM_LBUTTONUP:
                    if (_behaviorService.OnMouseUp(MouseButtons.Left))
                    {
                        return false;
                    }

                    break;

                case PInvokeCore.WM_RBUTTONUP:
                    if (_behaviorService.OnMouseUp(MouseButtons.Right))
                    {
                        return false;
                    }

                    break;

                case PInvokeCore.WM_MOUSEHOVER:
                    if (_behaviorService.OnMouseHover(mouseLoc))
                    {
                        return false;
                    }

                    break;

                case PInvokeCore.WM_LBUTTONDBLCLK:
                    if (_behaviorService.OnMouseDoubleClick(MouseButtons.Left, mouseLoc))
                    {
                        return false;
                    }

                    break;

                case PInvokeCore.WM_RBUTTONDBLCLK:
                    if (_behaviorService.OnMouseDoubleClick(MouseButtons.Right, mouseLoc))
                    {
                        return false;
                    }

                    break;
            }

            return true;
        }
    }
}
