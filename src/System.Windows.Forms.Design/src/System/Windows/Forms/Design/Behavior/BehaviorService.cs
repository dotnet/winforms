﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
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
    public sealed partial class BehaviorService : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;             // standard service provider
        private readonly AdornerWindow _adornerWindow;                  // the transparent window all glyphs are drawn to
        private readonly ArrayList _behaviorStack;                      // the stack behavior objects can be pushed to and popped from
        private Behavior _captureBehavior;                              // the behavior that currently has capture; may be null
        private Glyph _hitTestedGlyph;                                  // the last valid glyph that was hit tested
        private IToolboxService _toolboxSvc;                            // allows us to have the toolbox choose a cursor
        private Control _dropSource;                                    // actual control used to call .dodragdrop
        private DragEventArgs _validDragArgs;                           // if valid - this is used to fabricate drag enter/leave envents
        private BehaviorDragDropEventHandler _beginDragHandler;         // fired directly before we call .DoDragDrop()
        private BehaviorDragDropEventHandler _endDragHandler;           // fired directly after we call .DoDragDrop()
        private EventHandler _synchronizeEventHandler;                  // fired when we want to synchronize the selection
        private User32.TRACKMOUSEEVENT _trackMouseEvent;                // demand created (once) used to track the mouse hover event
        private bool _trackingMouseEvent;                               // state identifying current mouse tracking
        private string[] _testHook_RecentSnapLines;                     // we keep track of the last snaplines we found - for testing purposes
        private readonly MenuCommandHandler _menuCommandHandler;        // private object that handles all menu commands
        private bool _useSnapLines;                                     // indicates if this designer session is using snaplines or snapping to a grid
        private bool _queriedSnapLines;                                 // only query for this once since we require the user restart design sessions when this changes
        private readonly Hashtable _dragEnterReplies;                   // we keep track of whether glyph has already responded to a DragEnter this D&D.
        private static readonly TraceSwitch s_dragDropSwitch
            = new TraceSwitch("BSDRAGDROP", "Behavior service drag & drop messages");

        //test hooks for SnapLines
        private static User32.WM WM_GETALLSNAPLINES;
        private static User32.WM WM_GETRECENTSNAPLINES;
        private const string ToolboxFormat = ".NET Toolbox Item"; // used to detect if a drag is coming from the toolbox.

        internal BehaviorService(IServiceProvider serviceProvider, Control windowFrame)
        {
            _serviceProvider = serviceProvider;
            _adornerWindow = new AdornerWindow(this, windowFrame);

            // Use the adornerWindow as an overlay
            IOverlayService os = (IOverlayService)serviceProvider.GetService(typeof(IOverlayService));
            if (os != null)
            {
                AdornerWindowIndex = os.PushOverlay(_adornerWindow);
            }

            _dragEnterReplies = new Hashtable();

            // Start with an empty adorner collection & no behavior on the stack
            Adorners = new BehaviorServiceAdornerCollection(this);
            _behaviorStack = new ArrayList();

            _hitTestedGlyph = null;
            _validDragArgs = null;
            DesignerActionUI = null;
            _trackMouseEvent = default;
            _trackingMouseEvent = false;

            // Create out object that will handle all menucommands
            if (serviceProvider.GetService(typeof(IMenuCommandService)) is IMenuCommandService menuCommandService
                && serviceProvider.GetService(typeof(IDesignerHost)) is IDesignerHost host)
            {
                _menuCommandHandler = new MenuCommandHandler(this, menuCommandService);
                host.RemoveService(typeof(IMenuCommandService));
                host.AddService(typeof(IMenuCommandService), _menuCommandHandler);
            }

            // Default layoutmode is SnapToGrid.
            _useSnapLines = false;
            _queriedSnapLines = false;

            // Test hooks
            WM_GETALLSNAPLINES = User32.RegisterWindowMessageW("WM_GETALLSNAPLINES");
            WM_GETRECENTSNAPLINES = User32.RegisterWindowMessageW("WM_GETRECENTSNAPLINES");

            // Listen to the SystemEvents so that we can resync selection based on display settings etc.
            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
        }

        /// <summary>
        ///  Read-only property that returns the AdornerCollection that the BehaivorService manages.
        /// </summary>
        public BehaviorServiceAdornerCollection Adorners { get; }

        /// <summary>
        ///  Returns the actual Control that represents the transparent AdornerWindow.
        /// </summary>
        internal Control AdornerWindowControl => _adornerWindow;

        internal int AdornerWindowIndex { get; } = -1;

        internal bool HasCapture => _captureBehavior != null;

        /// <summary>
        ///  Returns the LayoutMode setting of the current designer session.  Either SnapLines or SnapToGrid.
        /// </summary>
        internal bool UseSnapLines
        {
            get
            {
                // we only check for this service/value once since we require the  user to re-open the designer session
                // after these types of option have been modified
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

        internal bool Dragging { get; private set; }

        internal bool CancelDrag { get; set; }

        internal DesignerActionUI DesignerActionUI { get; set; }

        /// <summary>
        ///  Called by the DragAssistanceManager after a snapline/drag op has completed - we store this data for
        ///  testing purposes. See TestHook_GetRecentSnapLines method.
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
            // Remove adorner window from overlay service
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

        private Control DropSource => _dropSource ??= new Control();

        internal DragDropEffects DoDragDrop(DropSourceBehavior dropSourceBehavior)
        {
            // Hook events
            DropSource.QueryContinueDrag += new QueryContinueDragEventHandler(dropSourceBehavior.QueryContinueDrag);
            DropSource.GiveFeedback += new GiveFeedbackEventHandler(dropSourceBehavior.GiveFeedback);

            DragDropEffects res = DragDropEffects.None;

            // Build up the eventargs for firing our dragbegin/end events
            ICollection dragComponents = ((DropSourceBehavior.BehaviorDataObject)dropSourceBehavior.DataObject).DragComponents;
            BehaviorDragDropEventArgs eventArgs = new BehaviorDragDropEventArgs(dragComponents);

            try
            {
                try
                {
                    OnBeginDrag(eventArgs);
                    Dragging = true;
                    CancelDrag = false;

                    // This is normally cleared on OnMouseUp, but we might not get an OnMouseUp to clear it. VSWhidbey #474259
                    // So let's make sure it is really cleared when we start the drag.
                    _dragEnterReplies.Clear();
                    res = DropSource.DoDragDrop(dropSourceBehavior.DataObject, dropSourceBehavior.AllowedEffects);
                }
                finally
                {
                    DropSource.QueryContinueDrag -= new QueryContinueDragEventHandler(dropSourceBehavior.QueryContinueDrag);
                    DropSource.GiveFeedback -= new GiveFeedbackEventHandler(dropSourceBehavior.GiveFeedback);

                    // If the drop gets cancelled, we won't get a OnDragDrop, so let's make sure that we stop
                    // processing drag notifications. Also VSWhidbey #354552 and 133339.
                    EndDragNotification();
                    _validDragArgs = null;
                    Dragging = false;
                    CancelDrag = false;
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
                // It's possible we did not receive an EndDrag, and therefore we weren't able to cleanup the drag.
                // We will do that here. Scenarios where this happens: dragging from designer to recycle-bin,
                // or over the taskbar.
                if (dropSourceBehavior != null)
                {
                    dropSourceBehavior.CleanupDrag();
                }
            }

            return res;
        }

        internal void EndDragNotification() => _adornerWindow.EndDragNotification();

        private void OnEndDrag(BehaviorDragDropEventArgs e) => _endDragHandler?.Invoke(this, e);

        private void OnBeginDrag(BehaviorDragDropEventArgs e) => _beginDragHandler?.Invoke(this, e);

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
        ///  The BehaviorService fires the BeginDrag event immediately before it starts a drop/drop operation
        ///  via DoBeginDragDrop.
        /// </summary>
        public event BehaviorDragDropEventHandler BeginDrag
        {
            add => _beginDragHandler += value;
            remove => _beginDragHandler -= value;
        }

        /// <summary>
        ///  The BehaviorService fires the EndDrag event immediately after the drag operation has completed.
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
        ///  Given a behavior returns the behavior immediately after the behavior in the behaviorstack.
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
        public void Invalidate() => _adornerWindow.InvalidateAdornerWindow();

        /// <summary>
        ///  Invalidates the BehaviorService's AdornerWindow.  This will force a refesh of all Adorners
        ///  and, in turn, all Glyphs.
        /// </summary>
        public void Invalidate(Rectangle rect) => _adornerWindow.InvalidateAdornerWindow(rect);

        /// <summary>
        ///  Invalidates the BehaviorService's AdornerWindow.  This will force a refesh of all Adorners
        ///  and, in turn, all Glyphs.
        /// </summary>
        public void Invalidate(Region r) => _adornerWindow.InvalidateAdornerWindow(r);

        /// <summary>
        ///  Synchronizes all selection glyphs.
        /// </summary>
        public void SyncSelection() => _synchronizeEventHandler?.Invoke(this, EventArgs.Empty);

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
            // Note, we don't call BehSvc.Invalidate because this will just cause the messages to recurse.
            // Instead, invalidating this adornerWindow will just cause a "propagatePaint" and draw the glyphs.
            _adornerWindow.Invalidate(paintRect);
        }

        /// <summary>
        ///  Pushes a Behavior object onto the BehaviorStack.  This is often done through hit-tested Glyph.
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

            // VSWhidbey #373836. Since we are now capturing all mouse messages, we might miss some WM_MOUSEACTIVATE
            // which would have activated the app. So if the DialogOwnerWindow (e.g. VS) is not the active window,
            // let's activate it here.
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

        private bool PropagateHitTest(Point pt)
        {
            for (int i = Adorners.Count - 1; i >= 0; i--)
            {
                if (!Adorners[i].Enabled)
                {
                    continue;
                }

                for (int j = 0; j < Adorners[i].Glyphs.Count; j++)
                {
                    Cursor hitTestCursor = Adorners[i].Glyphs[j].GetHitTest(pt);
                    if (hitTestCursor != null)
                    {
                        // InvokeMouseEnterGlyph will cause the selection to change, which might change the number of glyphs, so we need to remember the new glyph before calling InvokeMouseEnterLeave. VSWhidbey #396611
                        Glyph newGlyph = Adorners[i].Glyphs[j];

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

            // Returning false will cause the transparent window to return HTCLIENT when handling WM_NCHITTEST,
            // thus blocking underline window to receive mouse events.
            return true;
        }

        internal void StartDragNotification() => _adornerWindow.StartDragNotification();

        private MenuCommand FindCommand(CommandID commandID, IMenuCommandService menuService)
        {
            Behavior behavior = GetAppropriateBehavior(_hitTestedGlyph);

            if (behavior != null)
            {
                if (behavior.DisableAllCommands)
                {
                    MenuCommand menuCommand = menuService.FindCommand(commandID);

                    if (menuCommand != null)
                    {
                        menuCommand.Enabled = false;
                    }

                    return menuCommand;
                }
                else
                {
                    // Check to see if the behavior wants to interrupt this command
                    MenuCommand menuCommand = behavior.FindCommand(commandID);
                    if (menuCommand != null)
                    {
                        // The behavior chose to interrupt - so return the new command
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
                    // Same glyph - no change
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
            Debug.WriteLineIf(s_dragDropSwitch.TraceVerbose, "BS::OnDragEnter");

            // If the AdornerWindow receives a drag message, this method will be called without
            // a glyph - assign the last hit tested one
            g ??= _hitTestedGlyph;

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

            // If the AdornerWindow receives a drag message, this method will be called without
            // a glyph - assign the last hit tested one
            g ??= _hitTestedGlyph;

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
            => GetAppropriateBehavior(_hitTestedGlyph)?.OnMouseDoubleClick(_hitTestedGlyph, button, mouseLoc) ?? false;

        private bool OnMouseDown(MouseButtons button, Point mouseLoc)
            => GetAppropriateBehavior(_hitTestedGlyph)?.OnMouseDown(_hitTestedGlyph, button, mouseLoc) ?? false;

        private bool OnMouseEnter(Glyph g) => GetAppropriateBehavior(g)?.OnMouseEnter(g) ?? false;

        private bool OnMouseHover(Point mouseLoc)
            => GetAppropriateBehavior(_hitTestedGlyph)?.OnMouseHover(_hitTestedGlyph, mouseLoc) ?? false;

        private bool OnMouseLeave(Glyph g)
        {
            // Stop tracking mouse events for MouseHover
            UnHookMouseEvent();
            return GetAppropriateBehavior(g)?.OnMouseLeave(g) ?? false;
        }

        private bool OnMouseMove(MouseButtons button, Point mouseLoc)
        {
            // Hook mouse events (if we haven't already) for MouseHover
            HookMouseEvent();
            return GetAppropriateBehavior(_hitTestedGlyph)?.OnMouseMove(_hitTestedGlyph, button, mouseLoc) ?? false;
        }

        private bool OnMouseUp(MouseButtons button)
        {
            _dragEnterReplies.Clear();
            _validDragArgs = null;
            return GetAppropriateBehavior(_hitTestedGlyph)?.OnMouseUp(_hitTestedGlyph, button) ?? false;
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

        private void UnHookMouseEvent() => _trackingMouseEvent = false;

        private void OnDragDrop(DragEventArgs e)
        {
            Debug.WriteLineIf(s_dragDropSwitch.TraceVerbose, "BS::OnDragDrop");
            _validDragArgs = null;

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
            for (int i = 0; i < Adorners.Count; i++)
            {
                if (!Adorners[i].Enabled)
                {
                    continue;
                }

                for (int j = Adorners[i].Glyphs.Count - 1; j >= 0; j--)
                {
                    Adorners[i].Glyphs[j].Paint(pe);
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
                bytes = Text.Encoding.Default.GetBytes(text);
                nullBytes = Text.Encoding.Default.GetBytes(nullChar);
            }
            else
            {
                bytes = Text.Encoding.Unicode.GetBytes(text);
                nullBytes = Text.Encoding.Unicode.GetBytes(nullChar);
            }

            Marshal.Copy(bytes, 0, m.LParam, bytes.Length);
            Marshal.Copy(nullBytes, 0, unchecked((IntPtr)((long)m.LParam + bytes.Length)), nullBytes.Length);
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
            => GetAppropriateBehavior(_hitTestedGlyph)?.OnGiveFeedback(_hitTestedGlyph, e);

        private void OnQueryContinueDrag(QueryContinueDragEventArgs e)
            => GetAppropriateBehavior(_hitTestedGlyph)?.OnQueryContinueDrag(_hitTestedGlyph, e);

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
