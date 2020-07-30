// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using static Interop;

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///  The BehaviorService essentially manages all things UI in the designer.
    ///  When the BehaviorService is created it adds a transparent window over the
    ///  designer frame.  The BehaviorService can then use this window to render UI
    ///  elements (called Glyphs) as well as catch all mouse messages.  By doing
    ///  so - the BehaviorService can control designer behavior.  The BehaviorService
    ///  supports a BehaviorStack.  'Behavior' objects can be pushed onto this stack.
    ///  When a message is intercepted via the transparent window, the BehaviorService
    ///  can send the message to the Behavior at the top of the stack.  This allows
    ///  for different UI modes depending on the currently pushed Behavior.  The
    ///  BehaviorService is used to render all 'Glyphs': selection borders, grab handles,
    ///  smart tags etc... as well as control many of the design-time behaviors: dragging,
    ///  selection, snap lines, etc...
    /// </summary>
    public sealed class BehaviorService : IDisposable
    {
        private readonly IServiceProvider _serviceProvider; //standard service provider
        private readonly AdornerWindow _adornerWindow; //the transparent window all glyphs are drawn to
        private readonly BehaviorServiceAdornerCollection _adorners; //we manage all adorners (glyph-containers) here
        private readonly ArrayList _behaviorStack;  //the stack behavior objects can be pushed to and popped from
        private Behavior _captureBehavior;  //the behavior that currently has capture; may be null
        private Glyph _hitTestedGlyph; //the last valid glyph that was hit tested
        private IToolboxService _toolboxSvc; //allows us to have the toolbox choose a cursor
        private Control _dropSource; //actual control used to call .dodragdrop
        private DragEventArgs _validDragArgs; //if valid - this is used to fabricate drag enter/leave envents
        private BehaviorDragDropEventHandler _beginDragHandler; //fired directly before we call .DoDragDrop()
        private BehaviorDragDropEventHandler _endDragHandler; //fired directly after we call .DoDragDrop()
        private EventHandler _synchronizeEventHandler; //fired when we want to synchronize the selection
        private User32.TRACKMOUSEEVENT _trackMouseEvent; //demand created (once) used to track the mouse hover event
        private bool _trackingMouseEvent; //state identifying current mouse tracking
        private string[] _testHook_RecentSnapLines; //we keep track of the last snaplines we found - for testing purposes
        private readonly MenuCommandHandler _menuCommandHandler; //private object that handles all menu commands
        private bool _useSnapLines; //indicates if this designer session is using snaplines or snapping to a grid
        private bool _queriedSnapLines; //only query for this once since we require the user restart design sessions when this changes
        private readonly Hashtable _dragEnterReplies; // we keep track of whether glyph has already responded to a DragEnter this D&D.
        private static readonly TraceSwitch s_dragDropSwitch = new TraceSwitch("BSDRAGDROP", "Behavior service drag & drop messages");

        private bool _dragging; // are we in a drag
        private bool _cancelDrag; // should we cancel the drag on the next QueryContinueDrag

        private readonly int _adornerWindowIndex = -1;

        //test hooks for SnapLines
        private static User32.WM WM_GETALLSNAPLINES;
        private static User32.WM WM_GETRECENTSNAPLINES;

        private DesignerActionUI _actionPointer; // pointer to the designer action service so we can supply mouse over notifications

        private const string ToolboxFormat = ".NET Toolbox Item"; // used to detect if a drag is coming from the toolbox.
        internal BehaviorService(IServiceProvider serviceProvider, Control windowFrame)
        {
            _serviceProvider = serviceProvider;
            //create the AdornerWindow
            _adornerWindow = new AdornerWindow(this, windowFrame);

            //use the adornerWindow as an overlay
            IOverlayService os = (IOverlayService)serviceProvider.GetService(typeof(IOverlayService));
            if (os != null)
            {
                _adornerWindowIndex = os.PushOverlay(_adornerWindow);
            }

            _dragEnterReplies = new Hashtable();

            //start with an empty adorner collection & no behavior on the stack
            _adorners = new BehaviorServiceAdornerCollection(this);
            _behaviorStack = new ArrayList();

            _hitTestedGlyph = null;
            _validDragArgs = null;
            _actionPointer = null;
            _trackMouseEvent = default;
            _trackingMouseEvent = false;

            //create out object that will handle all menucommands
            if (serviceProvider.GetService(typeof(IMenuCommandService)) is IMenuCommandService menuCommandService && serviceProvider.GetService(typeof(IDesignerHost)) is IDesignerHost host)
            {
                _menuCommandHandler = new MenuCommandHandler(this, menuCommandService);
                host.RemoveService(typeof(IMenuCommandService));
                host.AddService(typeof(IMenuCommandService), _menuCommandHandler);
            }

            //default layoutmode is SnapToGrid.
            _useSnapLines = false;
            _queriedSnapLines = false;

            //test hooks
            WM_GETALLSNAPLINES = User32.RegisterWindowMessageW("WM_GETALLSNAPLINES");
            WM_GETRECENTSNAPLINES = User32.RegisterWindowMessageW("WM_GETRECENTSNAPLINES");

            // Listen to the SystemEvents so that we can resync selection based on display settings etc.
            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
        }
        /// <summary>
        ///  Read-only property that returns the AdornerCollection that the BehaivorService manages.
        /// </summary>
        public BehaviorServiceAdornerCollection Adorners
        {
            get => _adorners;
        }

        /// <summary>
        ///  Returns the actual Control that represents the transparent AdornerWindow.
        /// </summary>
        internal Control AdornerWindowControl
        {
            get => _adornerWindow;
        }

        internal int AdornerWindowIndex
        {
            get => _adornerWindowIndex;
        }

        internal bool HasCapture
        {
            get => _captureBehavior != null;
        }

        /// <summary>
        ///  Returns the LayoutMode setting of the current designer session.  Either SnapLines or SnapToGrid.
        /// </summary>
        internal bool UseSnapLines
        {
            get
            {
                //we only check for this service/value once since we require the  user to re-open the designer session after these types of option have been modified
                if (!_queriedSnapLines)
                {
                    _queriedSnapLines = true;
                    _useSnapLines = DesignerUtils.UseSnapLines(_serviceProvider);
                }

                return _useSnapLines;
            }
        }

        /// <summary>
        ///  Creates and returns a Graphics object for the AdornerWindow
        /// </summary>
        public Graphics AdornerWindowGraphics
        {
            get
            {
                Graphics result = _adornerWindow.CreateGraphics();
                result.Clip = new Region(_adornerWindow.DesignerFrameDisplayRectangle);
                return result;
            }
        }

        public Behavior CurrentBehavior
        {
            get
            {
                if (_behaviorStack != null && _behaviorStack.Count > 0)
                {
                    return (_behaviorStack[0] as Behavior);
                }
                else
                {
                    return null;
                }
            }
        }

        internal bool Dragging
        {
            get => _dragging;
        }

        internal bool CancelDrag
        {
            get => _cancelDrag;
            set => _cancelDrag = value;
        }

        internal DesignerActionUI DesignerActionUI
        {
            get => _actionPointer;
            set => _actionPointer = value;
        }
        /// <summary>
        ///  Called by the DragAssistanceManager after a snapline/drag op has completed - we store this data for testing purposes. See TestHook_GetRecentSnapLines method.
        /// </summary>
        internal string[] RecentSnapLines
        {
            set => _testHook_RecentSnapLines = value;
        }
        /// <summary>
        ///  Disposes the behavior service.
        /// </summary>
        public void Dispose()
        {
            // remove adorner window from overlay service
            IOverlayService os = (IOverlayService)_serviceProvider.GetService(typeof(IOverlayService));
            if (os != null)
            {
                os.RemoveOverlay(_adornerWindow);
            }

            MenuCommandHandler menuCommandHandler = null;
            if (_serviceProvider.GetService(typeof(IMenuCommandService)) is IMenuCommandService menuCommandService)
            {
                menuCommandHandler = menuCommandService as MenuCommandHandler;
            }

            if (menuCommandHandler != null && _serviceProvider.GetService(typeof(IDesignerHost)) is IDesignerHost host)
            {
                IMenuCommandService oldMenuCommandService = menuCommandHandler.MenuService;
                host.RemoveService(typeof(IMenuCommandService));
                host.AddService(typeof(IMenuCommandService), oldMenuCommandService);
            }

            _adornerWindow.Dispose();
            SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
        }

        private Control DropSource
        {
            get
            {
                if (_dropSource is null)
                {
                    _dropSource = new Control();
                }
                return _dropSource;
            }
        }

        internal DragDropEffects DoDragDrop(DropSourceBehavior dropSourceBehavior)
        {
            //hook events
            DropSource.QueryContinueDrag += new QueryContinueDragEventHandler(dropSourceBehavior.QueryContinueDrag);
            DropSource.GiveFeedback += new GiveFeedbackEventHandler(dropSourceBehavior.GiveFeedback);

            DragDropEffects res = DragDropEffects.None;

            //build up the eventargs for firing our dragbegin/end events
            ICollection dragComponents = ((DropSourceBehavior.BehaviorDataObject)dropSourceBehavior.DataObject).DragComponents;
            BehaviorDragDropEventArgs eventArgs = new BehaviorDragDropEventArgs(dragComponents);

            try
            {
                try
                {
                    OnBeginDrag(eventArgs);
                    _dragging = true;
                    _cancelDrag = false;
                    // This is normally cleared on OnMouseUp, but we might not get an OnMouseUp to clear it. VSWhidbey #474259
                    // So let's make sure it is really cleared when we start the drag.
                    _dragEnterReplies.Clear();
                    res = DropSource.DoDragDrop(dropSourceBehavior.DataObject, dropSourceBehavior.AllowedEffects);
                }
                finally
                {
                    DropSource.QueryContinueDrag -= new QueryContinueDragEventHandler(dropSourceBehavior.QueryContinueDrag);
                    DropSource.GiveFeedback -= new GiveFeedbackEventHandler(dropSourceBehavior.GiveFeedback);
                    //If the drop gets cancelled, we won't get a OnDragDrop, so let's make sure that we stop
                    //processing drag notifications. Also VSWhidbey #354552 and 133339.
                    EndDragNotification();
                    _validDragArgs = null;
                    _dragging = false;
                    _cancelDrag = false;
                    OnEndDrag(eventArgs);
                }
            }
            catch (CheckoutException cex)
            {
                if (cex == CheckoutException.Canceled)
                {
                    res = DragDropEffects.None;
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                // It's possible we did not receive an EndDrag, and therefore we weren't able to cleanup the drag.  We will do that here. Scenarios where this happens: dragging from designer to recycle-bin, or over the taskbar.
                if (dropSourceBehavior != null)
                {
                    dropSourceBehavior.CleanupDrag();
                }
            }
            return res;
        }

        internal void EndDragNotification()
        {
            _adornerWindow.EndDragNotification();
        }

        private void OnEndDrag(BehaviorDragDropEventArgs e)
        {
            _endDragHandler?.Invoke(this, e);
        }

        private void OnBeginDrag(BehaviorDragDropEventArgs e)
        {
            _beginDragHandler?.Invoke(this, e);
        }

        /// <summary>
        ///  Translates a point in the AdornerWindow to screen coords.
        /// </summary>
        public Point AdornerWindowPointToScreen(Point p)
        {
            User32.MapWindowPoints(_adornerWindow.Handle, IntPtr.Zero, ref p, 1);
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
        ///  Returns the location of a Control translated to AdornerWindow coords.
        /// </summary>
        public Point ControlToAdornerWindow(Control c)
        {
            if (c.Parent is null)
            {
                return Point.Empty;
            }

            var pt = new Point(c.Left, c.Top);
            User32.MapWindowPoints(c.Parent.Handle, _adornerWindow.Handle, ref pt, 1);
            if (c.Parent.IsMirrored)
            {
                pt.X -= c.Width;
            }
            return pt;
        }

        /// <summary>
        ///  Converts a point in handle's coordinate system to AdornerWindow coords.
        /// </summary>
        public Point MapAdornerWindowPoint(IntPtr handle, Point pt)
        {
            User32.MapWindowPoints(handle, _adornerWindow.Handle, ref pt, 1);
            return pt;
        }

        /// <summary>
        ///  Returns the bounding rectangle of a Control translated to AdornerWindow coords.
        /// </summary>
        public Rectangle ControlRectInAdornerWindow(Control c)
        {
            if (c.Parent is null)
            {
                return Rectangle.Empty;
            }
            Point loc = ControlToAdornerWindow(c);

            return new Rectangle(loc, c.Size);
        }

        /// <summary>
        ///  The BehaviorService fires the BeginDrag event immediately
        ///  before it starts a drop/drop operation via DoBeginDragDrop.
        /// </summary>
        public event BehaviorDragDropEventHandler BeginDrag
        {
            add => _beginDragHandler += value;
            remove => _beginDragHandler -= value;
        }

        /// <summary>
        ///  The BehaviorService fires the EndDrag event immediately
        ///  after the drag operation has completed.
        /// </summary>
        public event BehaviorDragDropEventHandler EndDrag
        {
            add => _endDragHandler += value;
            remove => _endDragHandler -= value;
        }

        /// <summary>
        ///  The BehaviorService fires the Synchronize event when the current selection should be synchronized (refreshed).
        /// </summary>
        public event EventHandler Synchronize
        {
            add => _synchronizeEventHandler += value;

            remove => _synchronizeEventHandler -= value;
        }

        /// <summary>
        ///  Given a behavior returns the behavior immediately
        ///  after the behavior in the behaviorstack.
        ///  Can return null.
        /// </summary>
        public Behavior GetNextBehavior(Behavior behavior)
        {
            if (_behaviorStack != null && _behaviorStack.Count > 0)
            {
                int index = _behaviorStack.IndexOf(behavior);
                if ((index != -1) && (index < _behaviorStack.Count - 1))
                {
                    return _behaviorStack[index + 1] as Behavior;
                }
            }
            return null;
        }

        internal void EnableAllAdorners(bool enabled)
        {
            foreach (Adorner adorner in Adorners)
            {
                adorner.EnabledInternal = enabled;
            }
            Invalidate();
        }

        /// <summary>
        ///  Invalidates the BehaviorService's AdornerWindow.  This will force a refesh of all Adorners
        ///  and, in turn, all Glyphs.
        /// </summary>
        public void Invalidate()
        {
            _adornerWindow.InvalidateAdornerWindow();
        }

        /// <summary>
        ///  Invalidates the BehaviorService's AdornerWindow.  This will force a refesh of all Adorners
        ///  and, in turn, all Glyphs.
        /// </summary>
        public void Invalidate(Rectangle rect)
        {
            _adornerWindow.InvalidateAdornerWindow(rect);
        }

        /// <summary>
        ///  Invalidates the BehaviorService's AdornerWindow.  This will force a refesh of all Adorners
        ///  and, in turn, all Glyphs.
        /// </summary>
        public void Invalidate(Region r)
        {
            _adornerWindow.InvalidateAdornerWindow(r);
        }

        /// <summary>
        ///  Synchronizes all selection glyphs.
        /// </summary>
        public void SyncSelection()
        {
            _synchronizeEventHandler?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///  Removes the behavior from the behavior stack
        /// </summary>
        public Behavior PopBehavior(Behavior behavior)
        {
            if (_behaviorStack.Count == 0)
            {
                throw new InvalidOperationException();
            }

            int index = _behaviorStack.IndexOf(behavior);
            if (index == -1)
            {
                Debug.Assert(false, "Could not find the behavior to pop - did it already get popped off? " + behavior.ToString());
                return null;
            }

            _behaviorStack.RemoveAt(index);
            if (behavior == _captureBehavior)
            {
                _adornerWindow.Capture = false;
                // Defensive:  adornerWindow should get a WM_CAPTURECHANGED, but do this by hand if it didn't.
                if (_captureBehavior != null)
                {
                    OnLoseCapture();
                    Debug.Assert(_captureBehavior is null, "OnLostCapture should have cleared captureBehavior");
                }
            }
            return behavior;
        }

        internal void ProcessPaintMessage(Rectangle paintRect)
        {
            //Note, we don't call BehSvc.Invalidate because this will just cause the messages to recurse. Instead, invalidating this adornerWindow will just cause a "propagatePaint" and draw the glyphs.
            _adornerWindow.Invalidate(paintRect);
        }

        /// <summary>
        ///  Pushes a Behavior object onto the BehaviorStack.  This is often done through hit-tested
        ///  Glyph.
        /// </summary>
        public void PushBehavior(Behavior behavior)
        {
            if (behavior is null)
            {
                throw new ArgumentNullException(nameof(behavior));
            }

            // Should we catch this
            _behaviorStack.Insert(0, behavior);
            // If there is a capture behavior, and it isn't this behavior, notify it that it no longer has capture.
            if (_captureBehavior != null && _captureBehavior != behavior)
            {
                OnLoseCapture();
            }
        }

        /// <summary>
        ///  Pushes a Behavior object onto the BehaviorStack and assigns mouse capture to the behavior.
        ///  This is often done through hit-tested Glyph.  If a behavior calls this the behavior's OnLoseCapture
        ///  will be called if mouse capture is lost.
        /// </summary>
        public void PushCaptureBehavior(Behavior behavior)
        {
            PushBehavior(behavior);
            _captureBehavior = behavior;
            _adornerWindow.Capture = true;

            //VSWhidbey #373836. Since we are now capturing all mouse messages, we might miss some WM_MOUSEACTIVATE which would have activated the app. So if the DialogOwnerWindow (e.g. VS) is not the active window, let's activate it here.
            IUIService uiService = (IUIService)_serviceProvider.GetService(typeof(IUIService));
            if (uiService != null)
            {
                IWin32Window hwnd = uiService.GetDialogOwnerWindow();
                if (hwnd != null && hwnd.Handle != IntPtr.Zero && hwnd.Handle != User32.GetActiveWindow())
                {
                    User32.SetActiveWindow(hwnd.Handle);
                }
            }
        }

        /// <summary>
        ///  Translates a screen coord into a coord relative to the BehaviorService's AdornerWindow.
        /// </summary>
        public Point ScreenToAdornerWindow(Point p)
        {
            User32.MapWindowPoints(IntPtr.Zero, _adornerWindow.Handle, ref p, 1);
            return p;
        }

        internal void OnLoseCapture()
        {
            if (_captureBehavior != null)
            {
                Behavior b = _captureBehavior;
                _captureBehavior = null;
                try
                {
                    b.OnLoseCapture(_hitTestedGlyph, EventArgs.Empty);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        ///  The AdornerWindow is a transparent window that resides ontop of the Designer's Frame.  This window is used by the BehaviorService to  intercept all messages.  It also serves as a unified canvas on which to paint Glyphs.
        /// </summary>
        private class AdornerWindow : Control
        {
            private readonly BehaviorService _behaviorService;//ptr back to BehaviorService
            private Control _designerFrame;//the designer's frame
            private static MouseHook s_mouseHook; // shared mouse hook
            private static readonly List<AdornerWindow> s_adornerWindowList = new List<AdornerWindow>();
            private bool _processingDrag; // is this particular window in a drag operation

            /// <summary>
            ///  Constructor that parents itself to the Designer Frame and hooks all
            ///  necessary events.
            /// </summary>
            internal AdornerWindow(BehaviorService behaviorService, Control designerFrame)
            {
                _behaviorService = behaviorService;
                _designerFrame = designerFrame;
                Dock = DockStyle.Fill;
                AllowDrop = true;
                Text = "AdornerWindow";
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
                    cp.Style &= ~(int)(User32.WS.CLIPCHILDREN | User32.WS.CLIPSIBLINGS);
                    cp.ExStyle |= (int)User32.WS_EX.TRANSPARENT;
                    return cp;
                }
            }

            internal bool ProcessingDrag
            {
                get => _processingDrag;
                set => _processingDrag = value;
            }

            /// <summary>
            ///  We'll use CreateHandle as our notification for creating our mouse attacher.
            /// </summary>
            protected override void OnHandleCreated(EventArgs e)
            {
                base.OnHandleCreated(e);
                s_adornerWindowList.Add(this);
                if (s_mouseHook is null)
                {
                    s_mouseHook = new MouseHook();
                }
            }

            /// <summary>
            ///  Unhook and null out our mouseHook.
            /// </summary>
            protected override void OnHandleDestroyed(EventArgs e)
            {
                s_adornerWindowList.Remove(this);
                // unregister the mouse hook once all adorner windows have been disposed.
                if (s_adornerWindowList.Count == 0 && s_mouseHook != null)
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
                if (disposing)
                {
                    if (_designerFrame != null)
                    {
                        _designerFrame = null;
                    }
                }
                base.Dispose(disposing);
            }

            internal Control DesignerFrame
            {
                get => _designerFrame;
            }

            /// <summary>
            ///  Returns the display rectangle for the adorner window
            /// </summary>
            internal Rectangle DesignerFrameDisplayRectangle
            {
                get
                {
                    if (DesignerFrameValid)
                    {
                        return ((DesignerFrame)_designerFrame).DisplayRectangle;
                    }
                    else
                    {
                        return Rectangle.Empty;
                    }
                }
            }

            /// <summary>
            ///  Returns true if the DesignerFrame is created and not being disposed.
            /// </summary>
            internal bool DesignerFrameValid
            {
                get
                {
                    if (_designerFrame is null || _designerFrame.IsDisposed || !_designerFrame.IsHandleCreated)
                    {
                        return false;
                    }
                    return true;
                }
            }

            public IEnumerable<Adorner> Adorners { get; private set; }

            /// <summary>
            ///  Ultimately called by ControlDesigner when it receives a DragDrop message - here, we'll exit from 'drag mode'.
            /// </summary>
            internal void EndDragNotification()
            {
                ProcessingDrag = false;
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
                    //translate for non-zero scroll positions
                    Point scrollPosition = ((DesignerFrame)_designerFrame).AutoScrollPosition;
                    region.Translate(scrollPosition.X, scrollPosition.Y);

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
                    //translate for non-zero scroll positions
                    Point scrollPosition = ((DesignerFrame)_designerFrame).AutoScrollPosition;
                    rectangle.Offset(scrollPosition.X, scrollPosition.Y);

                    _designerFrame.Invalidate(rectangle, true);
                    _designerFrame.Update();
                }
            }

            /// <summary>
            ///  The AdornerWindow hooks all Drag/Drop notification so  they can be forwarded to the appropriate Behavior via the BehaviorService.
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

            internal void EnableAllAdorners(bool enabled)
            {
                foreach (Adorner adorner in Adorners)
                {
                    adorner.EnabledInternal = enabled;
                }
                Invalidate();
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
                    string[] allFormats = e.Data.GetFormats();

                    for (int i = 0; i < allFormats.Length; i++)
                    {
                        if (allFormats[i].Length == ToolboxFormat.Length &&
                            string.Equals(ToolboxFormat, allFormats[i]))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            /// <summary>
            ///  The AdornerWindow hooks all Drag/Drop notification so  they can be forwarded to the appropriate Behavior via the BehaviorService.
            /// </summary>
            protected override void OnDragEnter(DragEventArgs e)
            {
                ProcessingDrag = true;

                // determine if this is a local drag, if it is, do normal processing otherwise, force a PropagateHitTest.  We need to force this because the OLE D&D service suspends mouse messages when the drag is not local so the mouse hook never sees them.
                if (!IsLocalDrag(e))
                {
                    _behaviorService._validDragArgs = e;
                    User32.GetCursorPos(out Point pt);
                    User32.MapWindowPoints(IntPtr.Zero, Handle, ref pt, 1);
                    _behaviorService.PropagateHitTest(pt);
                }
                _behaviorService.OnDragEnter(null, e);
            }

            /// <summary>
            ///  The AdornerWindow hooks all Drag/Drop notification so they can be forwarded to the appropriate Behavior via the BehaviorService.
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
            ///  The AdornerWindow hooks all Drag/Drop notification so they can be forwarded to the appropriate Behavior via the BehaviorService.
            /// </summary>
            protected override void OnDragOver(DragEventArgs e)
            {
                ProcessingDrag = true;
                if (!IsLocalDrag(e))
                {
                    _behaviorService._validDragArgs = e;
                    User32.GetCursorPos(out Point pt);
                    User32.MapWindowPoints(IntPtr.Zero, Handle, ref pt, 1);
                    _behaviorService.PropagateHitTest(pt);
                }

                _behaviorService.OnDragOver(e);
            }

            /// <summary>
            ///  The AdornerWindow hooks all Drag/Drop notification so they can be forwarded to the appropriate Behavior via the BehaviorService.
            /// </summary>
            protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
            {
                _behaviorService.OnGiveFeedback(e);
            }

            /// <summary>
            ///  The AdornerWindow hooks all Drag/Drop notification so they can be forwarded to the appropriate Behavior via the BehaviorService.
            /// </summary>
            protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e)
            {
                _behaviorService.OnQueryContinueDrag(e);
            }

            /// <summary>
            ///  Called by ControlDesigner when it receives a DragEnter message - we'll let listen to all Mouse Messages so we can send drag notifcations.
            /// </summary>
            internal void StartDragNotification()
            {
                ProcessingDrag = true;
            }

            /// <summary>
            ///  The AdornerWindow intercepts all designer-related messages and forwards them to the BehaviorService for appropriate actions.  Note that Paint and HitTest messages are correctly parsed and translated to AdornerWindow coords.
            /// </summary>
            protected override void WndProc(ref Message m)
            {
                //special test hooks
                if (m.Msg == (int)BehaviorService.WM_GETALLSNAPLINES)
                {
                    _behaviorService.TestHook_GetAllSnapLines(ref m);
                }
                else if (m.Msg == (int)BehaviorService.WM_GETRECENTSNAPLINES)
                {
                    _behaviorService.TestHook_GetRecentSnapLines(ref m);
                }

                switch ((User32.WM)m.Msg)
                {
                    case User32.WM.PAINT:
                        {
                            // Stash off the region we have to update.
                            using var hrgn = new Gdi32.RegionScope(0, 0, 0, 0);
                            User32.GetUpdateRgn(m.HWnd, hrgn, BOOL.TRUE);

                            // The region we have to update in terms of the smallest rectangle that completely encloses
                            // the update region of the window gives us the clip rectangle.
                            RECT clip = new RECT();
                            User32.GetUpdateRect(m.HWnd, ref clip, BOOL.TRUE);
                            Rectangle paintRect = clip;

                            using Region region = hrgn.CreateGdiPlusRegion();

                            // Call the base class to do its painting.
                            DefWndProc(ref m);

                            // Now do our own painting.
                            using Graphics g = Graphics.FromHwnd(m.HWnd);
                            using PaintEventArgs pevent = new PaintEventArgs(g, paintRect);
                            g.Clip = region;
                            _behaviorService.PropagatePaint(pevent);

                            break;
                        }

                    case User32.WM.NCHITTEST:
                        Point pt = new Point((short)PARAM.LOWORD(m.LParam),
                                             (short)PARAM.HIWORD(m.LParam));
                        var pt1 = new Point();
                        User32.MapWindowPoints(IntPtr.Zero, Handle, ref pt1, 1);
                        pt.Offset(pt1.X, pt1.Y);
                        if (_behaviorService.PropagateHitTest(pt) && !ProcessingDrag)
                        {
                            m.Result = (IntPtr)User32.HT.TRANSPARENT;
                        }
                        else
                        {
                            m.Result = (IntPtr)User32.HT.CLIENT;
                        }
                        break;

                    case User32.WM.CAPTURECHANGED:
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
                Point mouseLoc = new Point(x, y);
                _behaviorService.PropagateHitTest(mouseLoc);
                switch ((User32.WM)m.Msg)
                {
                    case User32.WM.LBUTTONDOWN:
                        if (_behaviorService.OnMouseDown(MouseButtons.Left, mouseLoc))
                        {
                            return false;
                        }
                        break;

                    case User32.WM.RBUTTONDOWN:
                        if (_behaviorService.OnMouseDown(MouseButtons.Right, mouseLoc))
                        {
                            return false;
                        }
                        break;

                    case User32.WM.MOUSEMOVE:
                        if (_behaviorService.OnMouseMove(Control.MouseButtons, mouseLoc))
                        {
                            return false;
                        }
                        break;

                    case User32.WM.LBUTTONUP:
                        if (_behaviorService.OnMouseUp(MouseButtons.Left))
                        {
                            return false;
                        }
                        break;

                    case User32.WM.RBUTTONUP:
                        if (_behaviorService.OnMouseUp(MouseButtons.Right))
                        {
                            return false;
                        }
                        break;

                    case User32.WM.MOUSEHOVER:
                        if (_behaviorService.OnMouseHover(mouseLoc))
                        {
                            return false;
                        }
                        break;

                    case User32.WM.LBUTTONDBLCLK:
                        if (_behaviorService.OnMouseDoubleClick(MouseButtons.Left, mouseLoc))
                        {
                            return false;
                        }
                        break;

                    case User32.WM.RBUTTONDBLCLK:
                        if (_behaviorService.OnMouseDoubleClick(MouseButtons.Right, mouseLoc))
                        {
                            return false;
                        }
                        break;
                }
                return true;
            }

            /// <summary>
            ///  This class knows how to hook all the messages to a given process/thread.
            ///  On any mouse clicks, it asks the designer what to do with the message, that is to eat it or propogate it to the control it was meant for.   This allows us to synchrounously process mouse messages when the AdornerWindow itself may be pumping messages.
            /// </summary>
            private class MouseHook
            {
                private AdornerWindow _currentAdornerWindow;
                private uint _thisProcessID;
                private GCHandle _mouseHookRoot;
                private IntPtr _mouseHookHandle = IntPtr.Zero;
                private bool _processingMessage;

                private bool _isHooked; //VSWHIDBEY # 474112
                private int _lastLButtonDownTimeStamp;

                public MouseHook()
                {
#if DEBUG
                    _callingStack = Environment.StackTrace;
#endif
                    HookMouse();
                }
#if DEBUG
                readonly string _callingStack;
                ~MouseHook()
                {
                    Debug.Assert(_mouseHookHandle == IntPtr.Zero, "Finalizing an active mouse hook.  This will crash the process.  Calling stack: " + _callingStack);
                }
#endif

                public void Dispose()
                {
                    UnhookMouse();
                }

                private void HookMouse()
                {
                    Debug.Assert(s_adornerWindowList.Count > 0, "No AdornerWindow available to create the mouse hook");
                    lock (this)
                    {
                        if (_mouseHookHandle != IntPtr.Zero || s_adornerWindowList.Count == 0)
                        {
                            return;
                        }

                        if (_thisProcessID == 0)
                        {
                            AdornerWindow adornerWindow = s_adornerWindowList[0];
                            User32.GetWindowThreadProcessId(adornerWindow, out _thisProcessID);
                        }

                        var hook = new User32.HOOKPROC(MouseHookProc);
                        _mouseHookRoot = GCHandle.Alloc(hook);
                        _mouseHookHandle = User32.SetWindowsHookExW(
                            User32.WH.MOUSE,
                            hook,
                            IntPtr.Zero,
                            (uint)AppDomain.GetCurrentThreadId());
                        if (_mouseHookHandle != IntPtr.Zero)
                        {
                            _isHooked = true;
                        }
                        Debug.Assert(_mouseHookHandle != IntPtr.Zero, "Failed to install mouse hook");
                    }
                }

                private unsafe IntPtr MouseHookProc(User32.HC nCode, IntPtr wparam, IntPtr lparam)
                {
                    if (_isHooked && nCode == User32.HC.ACTION)
                    {
                        User32.MOUSEHOOKSTRUCT* mhs = (User32.MOUSEHOOKSTRUCT*)lparam;
                        if (mhs != null)
                        {
                            try
                            {
                                if (ProcessMouseMessage(mhs->hWnd, unchecked((int)(long)wparam), mhs->pt.X, mhs->pt.Y))
                                {
                                    return (IntPtr)1;
                                }
                            }
                            catch (Exception ex)
                            {
                                _currentAdornerWindow.Capture = false;
                                if (ex != CheckoutException.Canceled)
                                {
                                    _currentAdornerWindow._behaviorService.ShowError(ex);
                                }
                                if (ClientUtils.IsCriticalException(ex))
                                {
                                    throw;
                                }
                            }
                            finally
                            {
                                _currentAdornerWindow = null;
                            }
                        }
                    }

                    Debug.Assert(_isHooked, "How did we get here when we are diposed?");
                    return User32.CallNextHookEx(new HandleRef(this, _mouseHookHandle), nCode, wparam, lparam);
                }

                private void UnhookMouse()
                {
                    lock (this)
                    {
                        if (_mouseHookHandle != IntPtr.Zero)
                        {
                            User32.UnhookWindowsHookEx(new HandleRef(this, _mouseHookHandle));
                            _mouseHookRoot.Free();
                            _mouseHookHandle = IntPtr.Zero;
                            _isHooked = false;
                        }
                    }
                }

                private bool ProcessMouseMessage(IntPtr hWnd, int msg, int x, int y)
                {
                    if (_processingMessage)
                    {
                        return false;
                    }

                    foreach (AdornerWindow adornerWindow in AdornerWindow.s_adornerWindowList)
                    {
                        if (!adornerWindow.DesignerFrameValid)
                        {
                            continue;
                        }

                        _currentAdornerWindow = adornerWindow;
                        IntPtr handle = adornerWindow.DesignerFrame.Handle;

                        // if it's us or one of our children, just process as normal
                        if (adornerWindow.ProcessingDrag || (hWnd != handle && User32.IsChild(new HandleRef(this, handle), hWnd).IsTrue()))
                        {
                            Debug.Assert(_thisProcessID != 0, "Didn't get our process id!");

                            // Make sure the window is in our process
                            User32.GetWindowThreadProcessId(hWnd, out uint pid);

                            // If this isn't our process, bail
                            if (pid != _thisProcessID)
                            {
                                return false;
                            }

                            try
                            {
                                _processingMessage = true;
                                var pt = new Point(x, y);
                                User32.MapWindowPoints(IntPtr.Zero, adornerWindow.Handle, ref pt, 1);
                                Message m = Message.Create(hWnd, msg, (IntPtr)0, PARAM.FromLowHigh(pt.Y, pt.X));

                                // No one knows why we get an extra click here from VS. As a workaround, we check the TimeStamp and discard it.
                                if (m.Msg == (int)User32.WM.LBUTTONDOWN)
                                {
                                    _lastLButtonDownTimeStamp = User32.GetMessageTime();
                                }
                                else if (m.Msg == (int)User32.WM.LBUTTONDBLCLK)
                                {
                                    int lButtonDoubleClickTimeStamp = User32.GetMessageTime();
                                    if (lButtonDoubleClickTimeStamp == _lastLButtonDownTimeStamp)
                                    {
                                        return true;
                                    }
                                }

                                if (!adornerWindow.WndProcProxy(ref m, pt.X, pt.Y))
                                {
                                    // we did the work, stop the message propogation
                                    return true;
                                }
                            }
                            finally
                            {
                                _processingMessage = false;
                            }
                            break; // no need to enumerate the other adorner windows since only one can be focused at a time.
                        }
                    }
                    return false;
                }
            }
        }

        private bool PropagateHitTest(Point pt)
        {
            for (int i = _adorners.Count - 1; i >= 0; i--)
            {
                if (!_adorners[i].Enabled)
                {
                    continue;
                }

                for (int j = 0; j < _adorners[i].Glyphs.Count; j++)
                {
                    Cursor hitTestCursor = _adorners[i].Glyphs[j].GetHitTest(pt);
                    if (hitTestCursor != null)
                    {
                        // InvokeMouseEnterGlyph will cause the selection to change, which might change the number of glyphs, so we need to remember the new glyph before calling InvokeMouseEnterLeave. VSWhidbey #396611
                        Glyph newGlyph = _adorners[i].Glyphs[j];

                        //with a valid hit test, fire enter/leave events
                        InvokeMouseEnterLeave(_hitTestedGlyph, newGlyph);
                        if (_validDragArgs is null)
                        {
                            //if we're not dragging, set the appropriate cursor
                            SetAppropriateCursor(hitTestCursor);
                        }

                        _hitTestedGlyph = newGlyph;
                        //return true if we hit on a transparentBehavior, otherwise false
                        return (_hitTestedGlyph.Behavior is ControlDesigner.TransparentBehavior);
                    }
                }
            }

            InvokeMouseEnterLeave(_hitTestedGlyph, null);
            if (_validDragArgs is null)
            {
                Cursor cursor = Cursors.Default;
                if ((_behaviorStack != null) && (_behaviorStack.Count > 0))
                {
                    if (_behaviorStack[0] is Behavior behavior)
                    {
                        cursor = behavior.Cursor;
                    }
                }
                SetAppropriateCursor(cursor);
            }
            _hitTestedGlyph = null;
            return true; // Returning false will cause the transparent window to return HTCLIENT when handling WM_NCHITTEST, thus blocking underline window to receive mouse events.
        }

        private class MenuCommandHandler : IMenuCommandService
        {
            private readonly BehaviorService _owner; // ptr back to the behavior service
            private readonly IMenuCommandService _menuService; // core service used for most implementations of the IMCS interface
            private readonly Stack<CommandID> _currentCommands = new Stack<CommandID>();

            public MenuCommandHandler(BehaviorService owner, IMenuCommandService menuService)
            {
                _owner = owner;
                _menuService = menuService;
            }

            public IMenuCommandService MenuService
            {
                get => _menuService;
            }

            void IMenuCommandService.AddCommand(MenuCommand command)
            {
                _menuService.AddCommand(command);
            }

            void IMenuCommandService.RemoveVerb(DesignerVerb verb)
            {
                _menuService.RemoveVerb(verb);
            }

            void IMenuCommandService.RemoveCommand(MenuCommand command)
            {
                _menuService.RemoveCommand(command);
            }

            MenuCommand IMenuCommandService.FindCommand(CommandID commandID)
            {
                try
                {
                    if (_currentCommands.Contains(commandID))
                    {
                        return null;
                    }
                    _currentCommands.Push(commandID);
                    return _owner.FindCommand(commandID, _menuService);
                }
                finally
                {
                    _currentCommands.Pop();
                }
            }

            bool IMenuCommandService.GlobalInvoke(CommandID commandID)
            {
                return _menuService.GlobalInvoke(commandID);
            }

            void IMenuCommandService.ShowContextMenu(CommandID menuID, int x, int y)
            {
                _menuService.ShowContextMenu(menuID, x, y);
            }

            void IMenuCommandService.AddVerb(DesignerVerb verb)
            {
                _menuService.AddVerb(verb);
            }

            DesignerVerbCollection IMenuCommandService.Verbs
            {
                get => _menuService.Verbs;
            }
        }

        internal void StartDragNotification()
        {
            _adornerWindow.StartDragNotification();
        }

        private MenuCommand FindCommand(CommandID commandID, IMenuCommandService menuService)
        {
            Behavior behavior = GetAppropriateBehavior(_hitTestedGlyph);
            if (behavior != null)
            {
                //if the behavior wants all commands disabled..
                if (behavior.DisableAllCommands)
                {
                    MenuCommand menuCommand = menuService.FindCommand(commandID);
                    if (menuCommand != null)
                    {
                        menuCommand.Enabled = false;
                    }
                    return menuCommand;
                }
                // check to see if the behavior wants to interrupt this command
                else
                {
                    MenuCommand menuCommand = behavior.FindCommand(commandID);
                    if (menuCommand != null)
                    {
                        // the behavior chose to interrupt - so return the new command
                        return menuCommand;
                    }
                }
            }
            return menuService.FindCommand(commandID);
        }

        private Behavior GetAppropriateBehavior(Glyph g)
        {
            if (_behaviorStack != null && _behaviorStack.Count > 0)
            {
                return _behaviorStack[0] as Behavior;
            }

            if (g != null && g.Behavior != null)
            {
                return g.Behavior;
            }

            return null;
        }

        private void ShowError(Exception ex)
        {
            if (_serviceProvider.GetService(typeof(IUIService)) is IUIService uis)
            {
                uis.ShowError(ex);
            }
        }

        private void SetAppropriateCursor(Cursor cursor)
        {
            //default cursors will let the toolbox svc set a cursor if needed
            if (cursor == Cursors.Default)
            {
                if (_toolboxSvc is null)
                {
                    _toolboxSvc = (IToolboxService)_serviceProvider.GetService(typeof(IToolboxService));
                }

                if (_toolboxSvc != null && _toolboxSvc.SetCursor())
                {
                    cursor = new Cursor(User32.GetCursor());
                }
            }
            _adornerWindow.Cursor = cursor;
        }

        private void InvokeMouseEnterLeave(Glyph leaveGlyph, Glyph enterGlyph)
        {
            if (leaveGlyph != null)
            {
                if (enterGlyph != null && leaveGlyph.Equals(enterGlyph))
                {
                    //same glyph - no change
                    return;
                }
                if (_validDragArgs != null)
                {
                    OnDragLeave(leaveGlyph, EventArgs.Empty);
                }
                else
                {
                    OnMouseLeave(leaveGlyph);
                }
            }

            if (enterGlyph != null)
            {
                if (_validDragArgs != null)
                {
                    OnDragEnter(enterGlyph, _validDragArgs);
                }
                else
                {
                    OnMouseEnter(enterGlyph);
                }
            }
        }
        private void OnDragEnter(Glyph g, DragEventArgs e)
        {
            // if the AdornerWindow receives a drag message, this fn() will be called w/o a glyph - so we'll assign the last hit tested one
            Debug.WriteLineIf(s_dragDropSwitch.TraceVerbose, "BS::OnDragEnter");
            if (g is null)
            {
                g = _hitTestedGlyph;
            }

            Behavior behavior = GetAppropriateBehavior(g);
            if (behavior is null)
            {
                Debug.WriteLineIf(s_dragDropSwitch.TraceVerbose, "\tNo behavior, returning");
                return;
            }
            Debug.WriteLineIf(s_dragDropSwitch.TraceVerbose, "\tForwarding to behavior");
            behavior.OnDragEnter(g, e);

            if (g != null && g is ControlBodyGlyph && e.Effect == DragDropEffects.None)
            {
                _dragEnterReplies[g] = this; // dummy value, we just need to set something.
                Debug.WriteLineIf(s_dragDropSwitch.TraceVerbose, "\tCalled DragEnter on this glyph. Caching");
            }
        }

        private void OnDragLeave(Glyph g, EventArgs e)
        {
            Debug.WriteLineIf(s_dragDropSwitch.TraceVerbose, "BS::DragLeave");
            // This is normally cleared on OnMouseUp, but we might not get an OnMouseUp to clear it. VSWhidbey #474259
            // So let's make sure it is really cleared when we start the drag.
            _dragEnterReplies.Clear();

            // if the AdornerWindow receives a drag message, this fn() will be called w/o a glyph - so we'll assign the last hit tested one
            if (g is null)
            {
                g = _hitTestedGlyph;
            }

            Behavior behavior = GetAppropriateBehavior(g);
            if (behavior is null)
            {
                Debug.WriteLineIf(s_dragDropSwitch.TraceVerbose, "\t No behavior returning ");
                return;
            }
            Debug.WriteLineIf(s_dragDropSwitch.TraceVerbose, "\tBehavior found calling OnDragLeave");
            behavior.OnDragLeave(g, e);
        }

        private bool OnMouseDoubleClick(MouseButtons button, Point mouseLoc)
        {
            Behavior behavior = GetAppropriateBehavior(_hitTestedGlyph);
            if (behavior is null)
            {
                return false;
            }
            return behavior.OnMouseDoubleClick(_hitTestedGlyph, button, mouseLoc);
        }

        private bool OnMouseDown(MouseButtons button, Point mouseLoc)
        {
            Behavior behavior = GetAppropriateBehavior(_hitTestedGlyph);
            if (behavior is null)
            {
                return false;
            }
            return behavior.OnMouseDown(_hitTestedGlyph, button, mouseLoc);
        }

        private bool OnMouseEnter(Glyph g)
        {
            Behavior behavior = GetAppropriateBehavior(g);
            if (behavior is null)
            {
                return false;
            }
            return behavior.OnMouseEnter(g);
        }

        private bool OnMouseHover(Point mouseLoc)
        {
            Behavior behavior = GetAppropriateBehavior(_hitTestedGlyph);
            if (behavior is null)
            {
                return false;
            }
            return behavior.OnMouseHover(_hitTestedGlyph, mouseLoc);
        }

        private bool OnMouseLeave(Glyph g)
        {
            //stop tracking mouse events for MouseHover
            UnHookMouseEvent();

            Behavior behavior = GetAppropriateBehavior(g);
            if (behavior is null)
            {
                return false;
            }
            return behavior.OnMouseLeave(g);
        }

        private bool OnMouseMove(MouseButtons button, Point mouseLoc)
        {
            //hook mouse events (if we haven't already) for MouseHover
            HookMouseEvent();

            Behavior behavior = GetAppropriateBehavior(_hitTestedGlyph);
            if (behavior is null)
            {
                return false;
            }
            return behavior.OnMouseMove(_hitTestedGlyph, button, mouseLoc);
        }

        private bool OnMouseUp(MouseButtons button)
        {
            _dragEnterReplies.Clear();
            _validDragArgs = null;
            Behavior behavior = GetAppropriateBehavior(_hitTestedGlyph);
            if (behavior is null)
            {
                return false;
            }
            return behavior.OnMouseUp(_hitTestedGlyph, button);
        }
        private void HookMouseEvent()
        {
            if (!_trackingMouseEvent)
            {
                _trackingMouseEvent = true;
                if (_trackMouseEvent.IsDefault())
                {
                    _trackMouseEvent = new User32.TRACKMOUSEEVENT
                    {
                        cbSize = (uint)Marshal.SizeOf<User32.TRACKMOUSEEVENT>(),
                        dwFlags = User32.TME.HOVER,
                        hwndTrack = _adornerWindow.Handle,
                        dwHoverTime = 100
                    };
                }

                User32.TrackMouseEvent(ref _trackMouseEvent);
            }
        }
        private void UnHookMouseEvent()
        {
            _trackingMouseEvent = false;
        }

        private void OnDragDrop(DragEventArgs e)
        {
            Debug.WriteLineIf(s_dragDropSwitch.TraceVerbose, "BS::OnDragDrop");
            _validDragArgs = null;//be sure to null out our cached drag args
            Behavior behavior = GetAppropriateBehavior(_hitTestedGlyph);
            if (behavior is null)
            {
                Debug.WriteLineIf(s_dragDropSwitch.TraceVerbose, "\tNo behavior. returning");
                return;
            }
            Debug.WriteLineIf(s_dragDropSwitch.TraceVerbose, "\tForwarding to behavior");
            behavior.OnDragDrop(_hitTestedGlyph, e);
        }
        private void PropagatePaint(PaintEventArgs pe)
        {
            for (int i = 0; i < _adorners.Count; i++)
            {
                if (!_adorners[i].Enabled)
                {
                    continue;
                }
                for (int j = _adorners[i].Glyphs.Count - 1; j >= 0; j--)
                {
                    _adorners[i].Glyphs[j].Paint(pe);
                }
            }
        }

        private void TestHook_GetRecentSnapLines(ref Message m)
        {
            string snapLineInfo = string.Empty;
            if (_testHook_RecentSnapLines != null)
            {
                foreach (string line in _testHook_RecentSnapLines)
                {
                    snapLineInfo += line + "\n";
                }
            }
            TestHook_SetText(ref m, snapLineInfo);
        }

        private void TestHook_SetText(ref Message m, string text)
        {
            if (m.LParam == IntPtr.Zero)
            {
                m.Result = (IntPtr)((text.Length + 1) * Marshal.SystemDefaultCharSize);
                return;
            }

            if (unchecked((int)(long)m.WParam) < text.Length + 1)
            {
                m.Result = (IntPtr)(-1);
                return;
            }

            // Copy the name into the given IntPtr
            char[] nullChar = new char[] { (char)0 };
            byte[] nullBytes;
            byte[] bytes;

            if (Marshal.SystemDefaultCharSize == 1)
            {
                bytes = System.Text.Encoding.Default.GetBytes(text);
                nullBytes = System.Text.Encoding.Default.GetBytes(nullChar);
            }
            else
            {
                bytes = System.Text.Encoding.Unicode.GetBytes(text);
                nullBytes = System.Text.Encoding.Unicode.GetBytes(nullChar);
            }

            Marshal.Copy(bytes, 0, m.LParam, bytes.Length);
            Marshal.Copy(nullBytes, 0, unchecked((IntPtr)((long)m.LParam + (long)bytes.Length)), nullBytes.Length);
            m.Result = (IntPtr)((bytes.Length + nullBytes.Length) / Marshal.SystemDefaultCharSize);
        }

        private void TestHook_GetAllSnapLines(ref Message m)
        {
            string snapLineInfo = string.Empty;
            if (!(_serviceProvider.GetService(typeof(IDesignerHost)) is IDesignerHost host))
            {
                return;
            }

            foreach (Component comp in host.Container.Components)
            {
                if (!(comp is Control))
                {
                    continue;
                }

                if (host.GetDesigner(comp) is ControlDesigner designer)
                {
                    foreach (SnapLine line in designer.SnapLines)
                    {
                        snapLineInfo += line.ToString() + "\tAssociated Control = " + designer.Control.Name + ":::";
                    }
                }
            }
            TestHook_SetText(ref m, snapLineInfo);
        }

        private void OnDragOver(DragEventArgs e)
        {
            // cache off our validDragArgs so we can re-fabricate enter/leave drag events
            _validDragArgs = e;
            Debug.WriteLineIf(s_dragDropSwitch.TraceVerbose, "BS::DragOver");
            Behavior behavior = GetAppropriateBehavior(_hitTestedGlyph);
            if (behavior is null)
            {
                Debug.WriteLineIf(s_dragDropSwitch.TraceVerbose, "\tNo behavior, exiting with DragDropEffects.None");
                e.Effect = DragDropEffects.None;
                return;
            }
            if (_hitTestedGlyph is null ||
               (_hitTestedGlyph != null && !_dragEnterReplies.ContainsKey(_hitTestedGlyph)))
            {
                Debug.WriteLineIf(s_dragDropSwitch.TraceVerbose, "\tFound glyph, forwarding to behavior");
                behavior.OnDragOver(_hitTestedGlyph, e);
            }
            else
            {
                Debug.WriteLineIf(s_dragDropSwitch.TraceVerbose, "\tFell through");
                e.Effect = DragDropEffects.None;
            }
        }

        private void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            Behavior behavior = GetAppropriateBehavior(_hitTestedGlyph);
            if (behavior is null)
            {
                return;
            }
            behavior.OnGiveFeedback(_hitTestedGlyph, e);
        }

        private void OnQueryContinueDrag(QueryContinueDragEventArgs e)
        {
            Behavior behavior = GetAppropriateBehavior(_hitTestedGlyph);
            if (behavior is null)
            {
                return;
            }
            behavior.OnQueryContinueDrag(_hitTestedGlyph, e);
        }

        private void OnSystemSettingChanged(object sender, EventArgs e)
        {
            SyncSelection();
            DesignerUtils.SyncBrushes();
        }

        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            SyncSelection();
            DesignerUtils.SyncBrushes();
        }
    }
}
