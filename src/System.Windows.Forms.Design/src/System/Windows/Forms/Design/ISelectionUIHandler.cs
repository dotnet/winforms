// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  This interface allows a designer to provide information to the selection UI
    ///  service that is needed to allow it to draw selection UI and to provide
    ///  automatic component drag support.
    /// </summary>
    internal interface ISelectionUIHandler
    {
        /// <summary>
        ///  Begins a drag on the currently selected designer.  The designer should provide
        ///  UI feedback about the drag at this time.  Typically, this feedback consists
        ///  of an inverted rectangle for each component, or a caret if the component
        ///  is text.
        /// </summary>
        bool BeginDrag(object[] components, SelectionRules rules, int initialX, int initialY);

        /// <summary>
        ///  Called when the user has moved the mouse.  This will only be called on
        ///  the designer that returned true from beginDrag.  The designer
        ///  should update its UI feedback here.
        /// </summary>
        void DragMoved(object[] components, Rectangle offset);

        /// <summary>
        ///  Called when the user has completed the drag.  The designer should
        ///  remove any UI feedback it may be providing.
        /// </summary>
        void EndDrag(object[] components, bool cancel);

        /// <summary>
        ///  Retrieves the shape of the component.  The component's shape should be in
        ///  absolute coordinates and in pixels, where 0,0 is the upper left corner of
        ///  the screen.
        /// </summary>
        Rectangle GetComponentBounds(object component);

        /// <summary>
        ///  Retrieves a set of rules concerning the movement capabilities of a component.
        ///  This should be one or more flags from the SelectionRules class.  If no designer
        ///  provides rules for a component, the component will not get any UI services.
        /// </summary>
        SelectionRules GetComponentRules(object component);

        /// <summary>
        ///  Determines the rectangle that any selection adornments should be clipped
        ///  to. This is normally the client area (in screen coordinates) of the
        ///  container.
        /// </summary>
        Rectangle GetSelectionClipRect(object component);

        /// <summary>
        ///  Handle a double-click on the selection rectangle
        ///  of the given component.
        /// </summary>
        void OnSelectionDoubleClick(IComponent component);

        /// <summary>
        ///  Queries to see if a drag operation
        ///  is valid on this handler for the given set of components.
        ///  If it returns true, BeginDrag will be called immediately after.
        /// </summary>
        bool QueryBeginDrag(object[] components, SelectionRules rules, int initialX, int initialY);

        /// <summary>
        ///  Shows the context menu for the given component.
        /// </summary>
        void ShowContextMenu(IComponent component);

        void OleDragEnter(DragEventArgs de);
        void OleDragDrop(DragEventArgs de);
        void OleDragOver(DragEventArgs de);
        void OleDragLeave();
    }
}
