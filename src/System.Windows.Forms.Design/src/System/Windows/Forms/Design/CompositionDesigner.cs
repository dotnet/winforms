// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///     <para> Provides a root designer implementation for designing components.</para>
    /// </summary>
    public class ComponentDocumentDesigner : ComponentDesigner, IRootDesigner, IToolboxUser, IOleDragClient,
        ITypeDescriptorFilterService
    {
        /// <summary>
        ///     <para>Gets  the control for this designer.</para>
        /// </summary>
        public Control Control => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     <para>Indicates whether the tray should auto arrange controls.</para>
        /// </summary>
        public bool TrayAutoArrange
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);

            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     <para>Indicates whether the tray should contain a large icon.</para>
        /// </summary>
        public bool TrayLargeIcon
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);

            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        bool IOleDragClient.CanModifyComponents
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        bool IOleDragClient.AddComponent(IComponent component, string name, bool firstAdd)
        {
            throw new NotImplementedException();
        }

        bool IOleDragClient.IsDropOk(IComponent component)
        {
            throw new NotImplementedException();
        }

        Control IOleDragClient.GetDesignerControl()
        {
            throw new NotImplementedException();
        }

        Control IOleDragClient.GetControlForComponent(object component)
        {
            throw new NotImplementedException();
        }

        ViewTechnology[] IRootDesigner.SupportedTechnologies => throw new NotImplementedException();

        /// <summary>
        ///     <para>
        ///         Initializes the designer with the specified component.
        ///     </para>
        /// </summary>
        public override void Initialize(IComponent component)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <internalonly />
        /// <summary>
        ///     The view for this document.  The designer
        ///     should assume that the view will be shown shortly
        ///     after this call is made and make any necessary
        ///     preparations.
        /// </summary>

        //We can live with this one. We have obsoleted some of the enum values. This method
        //only takes on argument, so it is pretty obvious what argument is bad.
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        object IRootDesigner.GetView(ViewTechnology technology)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        bool IToolboxUser.GetToolSupported(ToolboxItem tool)
        {
            throw new NotImplementedException();
        }

        void IToolboxUser.ToolPicked(ToolboxItem tool)
        {
            throw new NotImplementedException();
        }

        bool ITypeDescriptorFilterService.FilterAttributes(IComponent component, IDictionary attributes)
        {
            throw new NotImplementedException();
        }

        bool ITypeDescriptorFilterService.FilterEvents(IComponent component, IDictionary events)
        {
            throw new NotImplementedException();
        }

        bool ITypeDescriptorFilterService.FilterProperties(IComponent component, IDictionary properties)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     <para>
        ///         Disposes of the resources (other than memory) used by
        ///         the <see cref='System.Windows.Forms.Design.ComponentDocumentDesigner' />.
        ///     </para>
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     <para>
        ///         Gets a value indicating whether the specified tool is supported by this
        ///         designer.
        ///     </para>
        /// </summary>
        [CLSCompliant(false)]
        protected virtual bool GetToolSupported(ToolboxItem tool)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     <para>
        ///         Allows a
        ///         designer to filter the set of properties the component
        ///         it is designing will expose through the TypeDescriptor
        ///         object.
        ///     </para>
        /// </summary>
        protected override void PreFilterProperties(IDictionary properties)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
    }
}
