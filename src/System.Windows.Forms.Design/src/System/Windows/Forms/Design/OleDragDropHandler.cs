// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design
{
    internal partial class OleDragDropHandler
    {
        // NOTE: This is only a stub of the .NET Framework OleDragDropHandler. Disabled code that interacted with
        // this class is behind #if FEATURE_OLEDRAGDROPHANDLER

        // This is a bit that we stuff into the DoDragDrop to indicate that the thing that is being dragged should
        // only be allowed to be moved in the current DropTarget (e.g. parent designer). We use this for interited
        // components that can be modified (e.g. location/size) changed but not removed from their parent.

        protected const int AllowLocalMoveOnly = 0x04000000;
        public const string CF_CODE = "CF_XMLCODE";
        public const string CF_COMPONENTTYPES = "CF_COMPONENTTYPES";
        public const string CF_TOOLBOXITEM = "CF_NESTEDTOOLBOXITEM";

        public OleDragDropHandler(SelectionUIHandler selectionHandler, IServiceProvider serviceProvider,
            IOleDragClient client)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public static string DataFormat => throw new NotImplementedException(SR.NotImplementedByDesign);

        public static string ExtraInfoFormat => throw new NotImplementedException(SR.NotImplementedByDesign);

        public static string NestedToolboxItemFormat => throw new NotImplementedException(SR.NotImplementedByDesign);

        public bool Dragging => throw new NotImplementedException(SR.NotImplementedByDesign);

        public static bool FreezePainting => throw new NotImplementedException(SR.NotImplementedByDesign);

        protected virtual bool CanDropDataObject(IDataObject dataObj)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  This is the worker method of all CreateTool methods.  It is the only one
        ///  that can be overridden.
        /// </summary>
        public IComponent[] CreateTool(ToolboxItem tool, Control parent, int x, int y, int width, int height,
            bool hasLocation, bool hasSize)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public IComponent[] CreateTool(ToolboxItem tool, Control parent, int x, int y, int width, int height,
            bool hasLocation, bool hasSize, ToolboxSnapDragDropEventArgs e)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public bool DoBeginDrag(object[] components, SelectionRules rules, int initialX, int initialY)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public void DoEndDrag(object[] components, bool cancel)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public void DoOleDragDrop(DragEventArgs de)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public void DoOleDragEnter(DragEventArgs de)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public void DoOleDragLeave()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public void DoOleDragOver(DragEventArgs de)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public void DoOleGiveFeedback(GiveFeedbackEventArgs e)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public object[] GetDraggingObjects(IDataObject dataObj)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public object[] GetDraggingObjects(DragEventArgs de)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        protected object GetService(Type t)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        protected virtual void OnInitializeComponent(IComponent comp, int x, int y, int width, int height,
            bool hasLocation, bool hasSize)
        {
        }
    }
}
