// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Provides a designer that extends the ScrollableControlDesigner and implements
    ///  IRootDesigner.
    /// </summary>
    [
        ToolboxItemFilter("System.Windows.Forms")
    ]
    public class DocumentDesigner : ScrollableControlDesigner, IRootDesigner, IToolboxUser, IOleDragClient
    {
        static internal IDesignerSerializationManager manager;

        /// <summary>
        ///  We override our selectino rules to make the document non-sizeable.
        /// </summary>
        public override SelectionRules SelectionRules => throw new NotImplementedException(SR.NotImplementedByDesign);

        ViewTechnology[] IRootDesigner.SupportedTechnologies => throw new NotImplementedException();

        /// <summary>
        ///  Initializes the designer with the given component.  The designer can
        ///  get the component's site and request services from it in this call.
        /// </summary>
        public override void Initialize(IComponent component)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        object IRootDesigner.GetView(ViewTechnology technology)
        {
            throw new NotImplementedException();
        }

        bool IToolboxUser.GetToolSupported(ToolboxItem tool)
        {
            throw new NotImplementedException();
        }

        void IToolboxUser.ToolPicked(ToolboxItem tool)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  Disposes of this designer.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Returns an array of Glyph objects representing the selection
        ///  borders and grab handles for the related Component.  Note that
        ///  based on 'selType' the Glyphs returned will either: represent
        ///  a fully resizeable selection border with grab handles, a locked
        ///  selection border, or a single 'hidden' selection Glyph.
        /// </summary>
        public override GlyphCollection GetGlyphs(GlyphSelectionType selectionType)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Determines if the given tool is supported by this designer.
        ///  If a tool is supported then it will be enabled in the toolbox
        ///  when this designer regains focus.  Otherwise, it will be disabled.
        ///  Once a tool is marked as enabled or disabled it may not be
        ///  queried again.
        /// </summary>
        [CLSCompliant(false)]
        protected virtual bool GetToolSupported(ToolboxItem tool)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Called when the context menu should be displayed.  This displays the document
        ///  context menu.
        /// </summary>
        protected override void OnContextMenu(int x, int y)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  This is called immediately after the control handle has been created.
        /// </summary>
        protected override void OnCreateHandle()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Determines if a MenuEditorService has already been started.  If not,
        ///  this method will create a new instance of the service.
        /// </summary>
        protected virtual void EnsureMenuEditorService(IComponent c)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Allows a designer to filter the set of properties
        ///  the component it is designing will expose through the
        ///  TypeDescriptor object.  This method is called
        ///  immediately before its corresponding "Post" method.
        ///  If you are overriding this method you should call
        ///  the base implementation before you perform your own
        ///  filtering.
        /// </summary>
        protected override void PreFilterProperties(IDictionary properties)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  This will be called when the user double-clicks on a
        ///  toolbox item.  The document designer should create
        ///  a component for the given tool.
        /// </summary>
        [CLSCompliant(false)]
        protected virtual void ToolPicked(ToolboxItem tool)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Overrides our base class WndProc to provide support for
        ///  the menu editor service.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
    }
}
