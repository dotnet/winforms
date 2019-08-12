// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Drawing;
using System.Windows.Forms.Design.Behavior;
using static Interop;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Transparent Window to parent the DropDowns.
    /// </summary>
    internal sealed class ToolStripAdornerWindowService : IDisposable
    {
        private readonly IServiceProvider _serviceProvider; //standard service provider
        private readonly ToolStripAdornerWindow _toolStripAdornerWindow; //the transparent window all glyphs are drawn to
        private BehaviorService _behaviorService;
        private Adorner _dropDownAdorner;
        private ArrayList _dropDownCollection;
        private readonly IOverlayService _overlayService;

        /// <summary>
        ///  This constructor is called from DocumentDesigner's Initialize method.
        /// </summary>
        internal ToolStripAdornerWindowService(IServiceProvider serviceProvider, Control windowFrame)
        {
            _serviceProvider = serviceProvider;
            //create the AdornerWindow
            _toolStripAdornerWindow = new ToolStripAdornerWindow(windowFrame);
            _behaviorService = (BehaviorService)serviceProvider.GetService(typeof(BehaviorService));
            int indexToInsert = _behaviorService.AdornerWindowIndex;

            //use the adornerWindow as an overlay
            _overlayService = (IOverlayService)serviceProvider.GetService(typeof(IOverlayService));
            if (_overlayService != null)
            {
                _overlayService.InsertOverlay(_toolStripAdornerWindow, indexToInsert);
            }

            _dropDownAdorner = new Adorner();
            int count = _behaviorService.Adorners.Count;

            // Why this is NEEDED ?  To Add the Adorner at proper index in the AdornerCollection for the BehaviorService. So that the DesignerActionGlyph always stays on the Top.
            if (count > 1)
            {
                _behaviorService.Adorners.Insert(count - 1, _dropDownAdorner);
            }
        }

        /// <summary>
        ///  Returns the actual Control that represents the transparent AdornerWindow.
        /// </summary>
        internal Control ToolStripAdornerWindowControl
        {
            get => _toolStripAdornerWindow;
        }

        /// <summary>
        ///  Creates and returns a Graphics object for the AdornerWindow
        /// </summary>
        public Graphics ToolStripAdornerWindowGraphics
        {
            get => _toolStripAdornerWindow.CreateGraphics();
        }

        internal Adorner DropDownAdorner
        {
            get => _dropDownAdorner;
        }

        /// <summary>
        ///  Disposes the behavior service.
        /// </summary>
        public void Dispose()
        {
            if (_overlayService != null)
            {
                _overlayService.RemoveOverlay(_toolStripAdornerWindow);
            }
            _toolStripAdornerWindow.Dispose();
            if (_behaviorService != null)
            {
                _behaviorService.Adorners.Remove(_dropDownAdorner);
                _behaviorService = null;
            }
            if (_dropDownAdorner != null)
            {
                _dropDownAdorner.Glyphs.Clear();
                _dropDownAdorner = null;
            }
        }

        /// <summary>
        ///  Translates a point in the AdornerWindow to screen coords.
        /// </summary>
        public Point AdornerWindowPointToScreen(Point p)
        {
            NativeMethods.MapWindowPoints(_toolStripAdornerWindow.Handle, IntPtr.Zero, ref p, 1);
            return p;
        }

        /// <summary>
        ///  Gets the location (upper-left corner) of the AdornerWindow in screen coords.
        /// </summary>
        public Point AdornerWindowToScreen()
        {
            Point origin = new Point(0, 0);
            return AdornerWindowPointToScreen(origin);
        }

        /// <summary>
        ///  Returns the location of a Control translated to AdornerWidnow coords.
        /// </summary>
        public Point ControlToAdornerWindow(Control c)
        {
            if (c.Parent == null)
            {
                return Point.Empty;
            }

            var pt = new Point(c.Left, c.Top);
            NativeMethods.MapWindowPoints(c.Parent.Handle, _toolStripAdornerWindow.Handle, ref pt, 1);
            return pt;
        }

        /// <summary>
        ///  Invalidates the BehaviorService's AdornerWindow.  This will force a refesh of all Adorners and, in turn, all Glyphs.
        /// </summary>
        public void Invalidate()
        {
            _toolStripAdornerWindow.InvalidateAdornerWindow();
        }

        /// <summary>
        ///  Invalidates the BehaviorService's AdornerWindow.  This will force a refesh of all Adorners and, in turn, all Glyphs.
        /// </summary>
        public void Invalidate(Rectangle rect)
        {
            _toolStripAdornerWindow.InvalidateAdornerWindow(rect);
        }

        /// <summary>
        ///  Invalidates the BehaviorService's AdornerWindow.  This will force a refesh of all Adorners and, in turn, all Glyphs.
        /// </summary>
        public void Invalidate(Region r)
        {
            _toolStripAdornerWindow.InvalidateAdornerWindow(r);
        }

        internal ArrayList DropDowns
        {
            get => _dropDownCollection;
            set
            {
                if (_dropDownCollection == null)
                {
                    _dropDownCollection = new ArrayList();
                }
            }

        }

        /// <summary>
        ///  ControlDesigner calls this internal method in response to a WmPaint. We need to know when a ControlDesigner paints - 'cause we will need to re-paint any glyphs above of this Control.
        /// </summary>
        internal void ProcessPaintMessage(Rectangle paintRect)
        {
            // Note, we don't call BehSvc.Invalidate because this will just cause the messages to recurse. Instead, invalidating this adornerWindow will just cause a "propagatePaint" and draw the glyphs.
            _toolStripAdornerWindow.Invalidate(paintRect);
        }

        /// <summary>
        ///  The AdornerWindow is a transparent window that resides ontop of the Designer's Frame.  This window is used by the ToolStripAdornerWindowService to parent the MenuItem DropDowns.
        /// </summary>
        private class ToolStripAdornerWindow : Control
        {
            private Control _designerFrame; //the designer's frame

            internal ToolStripAdornerWindow(Control designerFrame)
            {
                _designerFrame = designerFrame;
                Dock = DockStyle.Fill;
                AllowDrop = true;
                Text = "ToolStripAdornerWindow";
                SetStyle(ControlStyles.Opaque, true);
            }

            /// <summary>
            ///  The key here is to set the appropriate TransparetWindow style.
            /// </summary>
            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;
                    cp.Style &= ~(NativeMethods.WS_CLIPCHILDREN | NativeMethods.WS_CLIPSIBLINGS);
                    cp.ExStyle |= NativeMethods.WS_EX_TRANSPARENT;
                    return cp;
                }
            }

            /// <summary>
            ///  We'll use CreateHandle as our notification for creating our mouse attacher.
            /// </summary>
            protected override void OnHandleCreated(EventArgs e)
            {
                base.OnHandleCreated(e);
            }

            /// <summary>
            ///  Unhook and null out our mouseHook.
            /// </summary>
            protected override void OnHandleDestroyed(EventArgs e)
            {
                base.OnHandleDestroyed(e);
            }

            /// <summary>
            ///  Null out our mouseHook and unhook any events.
            /// </summary>
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (_designerFrame != null)
                    {
                        _designerFrame = null;
                    }

                }
                base.Dispose(disposing);
            }

            /// <summary>
            ///  Returns true if the DesignerFrame is created & not being disposed.
            /// </summary>
            private bool DesignerFrameValid
            {
                get
                {
                    if (_designerFrame == null || _designerFrame.IsDisposed || !_designerFrame.IsHandleCreated)
                    {
                        return false;
                    }
                    return true;
                }
            }

            /// <summary>
            ///  Invalidates the transparent AdornerWindow by asking the Designer Frame beneath it to invalidate.  Note the they use of the .Update() call for perf. purposes.
            /// </summary>
            internal void InvalidateAdornerWindow()
            {
                if (DesignerFrameValid)
                {
                    _designerFrame.Invalidate(true);
                    _designerFrame.Update();
                }
            }

            /// <summary>
            ///  Invalidates the transparent AdornerWindow by asking the Designer Frame beneath it to invalidate.  Note the they use of the .Update() call for perf. purposes.
            /// </summary>
            internal void InvalidateAdornerWindow(Region region)
            {
                if (DesignerFrameValid)
                {
                    _designerFrame.Invalidate(region, true);
                    _designerFrame.Update();
                }
            }

            /// <summary>
            ///  Invalidates the transparent AdornerWindow by asking the Designer Frame beneath it to invalidate.  Note the they use of the .Update() call for perf. purposes.
            /// </summary>
            internal void InvalidateAdornerWindow(Rectangle rectangle)
            {
                if (DesignerFrameValid)
                {
                    _designerFrame.Invalidate(rectangle, true);
                    _designerFrame.Update();
                }
            }

            /// <summary>
            ///  The AdornerWindow intercepts all designer-related messages and forwards them to the BehaviorService for appropriate actions.  Note that Paint and HitTest  messages are correctly parsed and translated to AdornerWindow coords.
            /// </summary>
            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case WindowMessages.WM_NCHITTEST:
                        m.Result = (IntPtr)(NativeMethods.HTTRANSPARENT);
                        break;
                    default:
                        base.WndProc(ref m);
                        break;
                }
            }
        }
    }
}
