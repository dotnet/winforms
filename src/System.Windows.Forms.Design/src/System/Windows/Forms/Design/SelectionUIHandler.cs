// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  This is an abstract base class that encapsulates a lot of
    ///  the details of handling selection drags. Just about everyone
    ///  that implements a selection UI handler will extend this.
    /// </summary>
    internal abstract class SelectionUIHandler
    {
        /// <summary>
        ///  Begins a drag operation.  A designer should examine the list of components
        ///  to see if it wants to support the drag.  If it does, it should return
        ///  true.  If it returns true, the designer should provide
        ///  UI feedback about the drag at this time.  Typically, this feedback consists
        ///  of an inverted rectangle for each component, or a caret if the component
        ///  is text.
        /// </summary>
        public virtual bool BeginDrag(object[] components, SelectionRules rules, int initialX, int initialY)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Called when the user has moved the mouse.  This will only be called on
        ///  the designer that returned true from beginDrag.  The designer
        ///  should update its UI feedback here.
        /// </summary>
        public virtual void DragMoved(object[] components, Rectangle offset)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Called when the user has completed the drag.  The designer should
        ///  remove any UI feedback it may be providing.
        /// </summary>
        public virtual void EndDrag(object[] components, bool cancel)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Retrieves the base component for the selection handler.
        /// </summary>
        protected abstract IComponent GetComponent();

        /// <summary>
        ///  Retrieves the base component's UI control for the selection handler.
        /// </summary>
        protected abstract Control GetControl();

        /// <summary>
        ///  Retrieves the UI control for the given component.
        /// </summary>
        protected abstract Control GetControl(IComponent component);

        /// <summary>
        ///  Retrieves the current grid snap size we should snap objects
        ///  to.
        /// </summary>
        protected abstract Size GetCurrentSnapSize();

        /// <summary>
        ///  We use this to request often-used services.
        /// </summary>
        protected abstract object GetService(Type serviceType);

        /// <summary>
        ///  Determines if the selection UI handler should attempt to snap
        ///  objects to a grid.
        /// </summary>
        protected abstract bool GetShouldSnapToGrid();

        /// <summary>
        ///  Given a rectangle, this updates the dimensions of it
        ///  with any grid snaps and returns a new rectangle.  If
        ///  no changes to the rectangle's size were needed, this
        ///  may return the same rectangle.
        /// </summary>
        public abstract Rectangle GetUpdatedRect(Rectangle orignalRect, Rectangle dragRect, bool updateSize);

        /// <summary>
        ///  Queries to see if a drag operation
        ///  is valid on this handler for the given set of components.
        ///  If it returns true, BeginDrag will be called immediately after.
        /// </summary>
        public bool QueryBeginDrag(object[] components, SelectionRules rules, int initialX, int initialY)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Asks the handler to set the appropriate cursor
        /// </summary>
        public abstract void SetCursor();

        public virtual void OleDragEnter(DragEventArgs de)
        {
        }

        public virtual void OleDragDrop(DragEventArgs de)
        {
        }

        public virtual void OleDragOver(DragEventArgs de)
        {
        }

        public virtual void OleDragLeave()
        {
        }
    }
}
