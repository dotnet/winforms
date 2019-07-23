// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///  This abstract class represents the Behavior objects that are managed
    ///  by the BehaviorService.  This class can be extended to develop any
    ///  type of UI 'behavior'.  Ex: selection, drag, and resize behaviors.
    /// </summary>
    public abstract class Behavior
    {
        protected Behavior()
        {
        }

        /// <summary>
        ///  callParentBehavior - true if the parentBehavior should be called if it exists. The
        ///  parentBehavior is the next behavior on the behaviorService stack.
        ///  If callParentBehavior is true, then behaviorService must be non-null
        /// </summary>
        protected Behavior(bool callParentBehavior, BehaviorService behaviorService)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        private Behavior GetNextBehavior => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///  The cursor that should be displayed for this behavior.
        /// </summary>
        public virtual Cursor Cursor => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///  Rerturning true from here indicates to the BehaviorService that
        ///  all MenuCommands the designer receives should have their
        ///  state set to 'Enabled = false' when this Behavior is active.
        /// </summary>
        public virtual bool DisableAllCommands => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///  Called from the BehaviorService, this function provides an opportunity
        ///  for the Behavior to return its own custom MenuCommand thereby
        ///  intercepting this message.
        /// </summary>
        public virtual MenuCommand FindCommand(CommandID commandId)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  A behavior can request mouse capture through the behavior service by pushing
        ///  itself with PushCaptureBehavior.  If it does so, it will be notified through
        ///  OnLoseCapture when capture is lost.  Generally the behavior pops itself at
        ///  this time.  Capture is lost when one of the following occurs:
        ///  1.  Someone else requests capture.
        ///  2.  Another behavior is pushed.
        ///  3.  This behavior is popped.
        ///  In each of these cases OnLoseCapture on the behavior will be called.
        /// </summary>
        public virtual void OnLoseCapture(Glyph g, EventArgs e)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  When any MouseDown message enters the BehaviorService's AdornerWindow
        ///  (nclbuttondown, lbuttondown, rbuttondown, nclrbuttondown) it is first
        ///  passed here, to the top-most Behavior in the BehaviorStack.  Returning
        ///  'true' from this function signifies that the Message was 'handled' by
        ///  the Behavior and should not continue to be processed.
        /// </summary>
        public virtual bool OnMouseDoubleClick(Glyph g, MouseButtons button, Point mouseLoc)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  When any MouseDown message enters the BehaviorService's AdornerWindow
        ///  (nclbuttondown, lbuttondown, rbuttondown, nclrbuttondown) it is first
        ///  passed here, to the top-most Behavior in the BehaviorStack.  Returning
        ///  'true' from this function signifies that the Message was 'handled' by
        ///  the Behavior and should not continue to be processed.
        /// </summary>
        public virtual bool OnMouseDown(Glyph g, MouseButtons button, Point mouseLoc)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  When the mouse pointer's location is positively hit-tested with a
        ///  different Glyph than previous hit-tests, this event is fired on the
        ///  Behavior associated with the Glyph.
        /// </summary>
        public virtual bool OnMouseEnter(Glyph g)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  When a MouseHover message enters the BehaviorService's AdornerWindow
        ///  it is first passed here, to the top-most Behavior
        ///  in the BehaviorStack.  Returning 'true' from this function signifies that
        ///  the Message was 'handled' by the Behavior and should not continue to be processed.
        /// </summary>
        public virtual bool OnMouseHover(Glyph g, Point mouseLoc)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  When the mouse pointer leaves a positively hit-tested Glyph
        ///  with a valid Behavior, this method is invoked.
        /// </summary>
        public virtual bool OnMouseLeave(Glyph g)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  When any MouseMove message enters the BehaviorService's AdornerWindow
        ///  (mousemove, ncmousemove) it is first passed here, to the top-most Behavior
        ///  in the BehaviorStack.  Returning 'true' from this function signifies that
        ///  the Message was 'handled' by the Behavior and should not continue to be processed.
        /// </summary>
        public virtual bool OnMouseMove(Glyph g, MouseButtons button, Point mouseLoc)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  When any MouseUp message enters the BehaviorService's AdornerWindow
        ///  (nclbuttonupown, lbuttonup, rbuttonup, nclrbuttonup) it is first
        ///  passed here, to the top-most Behavior in the BehaviorStack.  Returning
        ///  'true' from this function signifies that the Message was 'handled' by
        ///  the Behavior and should not continue to be processed.
        /// </summary>
        public virtual bool OnMouseUp(Glyph g, MouseButtons button)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        //OLE DragDrop virtual methods
        //

        /// <summary>
        ///  OnDragDrop can be overridden so that a Behavior can specify its own
        ///  Drag/Drop rules.
        /// </summary>
        public virtual void OnDragDrop(Glyph g, DragEventArgs e)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  OnDragEnter can be overridden so that a Behavior can specify its own
        ///  Drag/Drop rules.
        /// </summary>
        public virtual void OnDragEnter(Glyph g, DragEventArgs e)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  OnDragLeave can be overridden so that a Behavior can specify its own
        ///  Drag/Drop rules.
        /// </summary>
        public virtual void OnDragLeave(Glyph g, EventArgs e)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  OnDragOver can be overridden so that a Behavior can specify its own
        ///  Drag/Drop rules.
        /// </summary>
        public virtual void OnDragOver(Glyph g, DragEventArgs e)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  OnGiveFeedback can be overridden so that a Behavior can specify its own
        ///  Drag/Drop rules.
        /// </summary>
        public virtual void OnGiveFeedback(Glyph g, GiveFeedbackEventArgs e)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  QueryContinueDrag can be overridden so that a Behavior can specify its own
        ///  Drag/Drop rules.
        /// </summary>
        public virtual void OnQueryContinueDrag(Glyph g, QueryContinueDragEventArgs e)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
    }
}
