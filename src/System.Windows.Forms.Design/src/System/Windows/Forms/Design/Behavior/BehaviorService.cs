// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///     The BehaviorService essentially manages all things UI in the designer.
    ///     When the BehaviorService is created it adds a transparent window over the
    ///     designer frame.  The BehaviorService can then use this window to render UI
    ///     elements (called Glyphs) as well as catch all mouse messages.  By doing
    ///     so - the BehaviorService can control designer behavior.  The BehaviorService
    ///     supports a BehaviorStack.  'Behavior' objects can be pushed onto this stack.
    ///     When a message is intercepted via the transparent window, the BehaviorService
    ///     can send the message to the Behavior at the top of the stack.  This allows
    ///     for different UI modes depending on the currently pushed Behavior.  The
    ///     BehaviorService is used to render all 'Glyphs': selection borders, grab handles,
    ///     smart tags etc... as well as control many of the design-time behaviors: dragging,
    ///     selection, snap lines, etc...
    /// </summary>
    public sealed class BehaviorService : IDisposable
    {
        [SuppressMessage("Microsoft.Performance", "CA1805:DoNotInitializeUnnecessarily")]
        internal BehaviorService(IServiceProvider serviceProvider, Control windowFrame)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
        /// <summary>
        ///     Read-only property that returns the AdornerCollection that the BehaivorService manages.
        /// </summary>
        public BehaviorServiceAdornerCollection Adorners =>
            throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     Creates and returns a Graphics object for the AdornerWindow
        /// </summary>
        public Graphics AdornerWindowGraphics => throw new NotImplementedException(SR.NotImplementedByDesign);

        public Behavior CurrentBehavior => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     Disposes the behavior service.
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Translates a point in the AdornerWindow to screen coords.
        /// </summary>
        public Point AdornerWindowPointToScreen(Point p)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Gets the location (upper-left corner) of the AdornerWindow in screen coords.
        /// </summary>
        public Point AdornerWindowToScreen()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Returns the location of a Control translated to AdornerWindow coords.
        /// </summary>
        public Point ControlToAdornerWindow(Control c)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Converts a point in handle's coordinate system to AdornerWindow coords.
        /// </summary>
        public Point MapAdornerWindowPoint(IntPtr handle, Point pt)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Returns the bounding rectangle of a Control translated to AdornerWindow coords.
        /// </summary>
        public Rectangle ControlRectInAdornerWindow(Control c)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     The BehaviorService fires the BeginDrag event immediately
        ///     before it starts a drop/drop operation via DoBeginDragDrop.
        /// </summary>
        public event BehaviorDragDropEventHandler BeginDrag
        {
            add => throw new NotImplementedException(SR.NotImplementedByDesign);
            remove => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     The BehaviorService fires the EndDrag event immediately
        ///     after the drag operation has completed.
        /// </summary>
        public event BehaviorDragDropEventHandler EndDrag
        {
            add => throw new NotImplementedException(SR.NotImplementedByDesign);
            remove => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     The BehaviorService fires the Synchronize event when the current selection should be synchronized (refreshed).
        /// </summary>
        public event EventHandler Synchronize
        {
            add => throw new NotImplementedException(SR.NotImplementedByDesign);

            remove => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Given a behavior returns the behavior immediately
        ///     after the behavior in the behaviorstack.
        ///     Can return null.
        /// </summary>
        public Behavior GetNextBehavior(Behavior behavior)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Invalidates the BehaviorService's AdornerWindow.  This will force a refesh of all Adorners
        ///     and, in turn, all Glyphs.
        /// </summary>
        public void Invalidate()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Invalidates the BehaviorService's AdornerWindow.  This will force a refesh of all Adorners
        ///     and, in turn, all Glyphs.
        /// </summary>
        public void Invalidate(Rectangle rect)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Invalidates the BehaviorService's AdornerWindow.  This will force a refesh of all Adorners
        ///     and, in turn, all Glyphs.
        /// </summary>
        public void Invalidate(Region r)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Synchronizes all selection glyphs.
        /// </summary>
        public void SyncSelection()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Removes the behavior from the behavior stack
        /// </summary>
        public Behavior PopBehavior(Behavior behavior)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Pushes a Behavior object onto the BehaviorStack.  This is often done through hit-tested
        ///     Glyph.
        /// </summary>
        public void PushBehavior(Behavior behavior)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Pushes a Behavior object onto the BehaviorStack and assigns mouse capture to the behavior.
        ///     This is often done through hit-tested Glyph.  If a behavior calls this the behavior's OnLoseCapture
        ///     will be called if mouse capture is lost.
        /// </summary>
        public void PushCaptureBehavior(Behavior behavior)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Translates a screen coord into a coord relative to the BehaviorService's AdornerWindow.
        /// </summary>
        public Point ScreenToAdornerWindow(Point p)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
    }
}
