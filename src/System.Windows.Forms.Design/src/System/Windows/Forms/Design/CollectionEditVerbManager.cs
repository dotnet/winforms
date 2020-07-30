// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing.Design;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Class for sharing code for launching the ToolStripItemsCollectionEditor from a verb. This class implments the IWindowsFormsEditorService and ITypeDescriptorContext to display the dialog.
    /// </summary>
    internal class CollectionEditVerbManager : IWindowsFormsEditorService, ITypeDescriptorContext
    {
        private readonly ComponentDesigner _designer;
        private IComponentChangeService _componentChangeSvc;
        private readonly PropertyDescriptor _targetProperty;
        private readonly DesignerVerb _editItemsVerb;

        /// <summary>
        ///  Create one of these things...
        /// </summary>
        internal CollectionEditVerbManager(string text, ComponentDesigner designer, PropertyDescriptor prop, bool addToDesignerVerbs)
        {
            Debug.Assert(designer != null, "Can't have a CollectionEditVerbManager without an associated designer");
            _designer = designer;
            _targetProperty = prop;
            if (prop is null)
            {
                prop = TypeDescriptor.GetDefaultProperty(designer.Component);
                if (prop != null && typeof(ICollection).IsAssignableFrom(prop.PropertyType))
                {
                    _targetProperty = prop;
                }
            }
            Debug.Assert(_targetProperty != null, "Need PropertyDescriptor for ICollection property to associate collectoin edtior with.");
            if (text is null)
            {
                text = SR.ToolStripItemCollectionEditorVerb;
            }
            _editItemsVerb = new DesignerVerb(text, new EventHandler(OnEditItems));

            if (addToDesignerVerbs)
            {
                _designer.Verbs.Add(_editItemsVerb);
            }
        }

        /// <summary>
        ///  Our caching property for the IComponentChangeService
        /// </summary>
        private IComponentChangeService ChangeService
        {
            get
            {
                if (_componentChangeSvc is null)
                {
                    _componentChangeSvc = (IComponentChangeService)((IServiceProvider)this).GetService(typeof(IComponentChangeService));
                }
                return _componentChangeSvc;
            }
        }

        /// <summary>
        ///  Self-explanitory interface impl.
        /// </summary>
        IContainer ITypeDescriptorContext.Container
        {
            get
            {
                if (_designer.Component.Site != null)
                {
                    return _designer.Component.Site.Container;
                }
                return null;
            }
        }

        public DesignerVerb EditItemsVerb
        {
            get => _editItemsVerb;
        }

        /// <summary>
        ///  Self-explanitory interface impl.
        /// </summary>
        void ITypeDescriptorContext.OnComponentChanged() => ChangeService.OnComponentChanged(_designer.Component, _targetProperty, null, null);

        /// <summary>
        ///  Self-explanitory interface impl.
        /// </summary>
        bool ITypeDescriptorContext.OnComponentChanging()
        {
            try
            {
                ChangeService.OnComponentChanging(_designer.Component, _targetProperty);
            }
            catch (CheckoutException checkoutException)
            {
                if (checkoutException == CheckoutException.Canceled)
                {
                    return false;
                }
                throw;
            }
            return true;
        }

        /// <summary>
        ///  Self-explanitory interface impl.
        /// </summary>
        object ITypeDescriptorContext.Instance
        {
            get => _designer.Component;
        }

        /// <summary>
        ///  Self-explanitory interface impl.
        /// </summary>
        PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor
        {
            get => _targetProperty;
        }

        /// <summary>
        ///  Self-explanitory interface impl.
        /// </summary>
        object IServiceProvider.GetService(Type serviceType)
        {
            if (serviceType == typeof(ITypeDescriptorContext) || serviceType == typeof(IWindowsFormsEditorService))
            {
                return this;
            }
            if (_designer.Component.Site != null)
            {
                return _designer.Component.Site.GetService(serviceType);
            }
            return null;
        }

        /// <summary>
        ///  Self-explanitory interface impl.
        /// </summary>
        void IWindowsFormsEditorService.CloseDropDown()
        {
            // we'll never be called to do this.
            Debug.Fail("NOTIMPL");
            return;
        }

        /// <summary>
        ///  Self-explanitory interface impl.
        /// </summary>
        void IWindowsFormsEditorService.DropDownControl(Control control)
        {
            Debug.Fail("NOTIMPL");
            return;
        }

        /// <summary>
        ///  Self-explanitory interface impl.
        /// </summary>
        DialogResult IWindowsFormsEditorService.ShowDialog(Form dialog)
        {
            IUIService uiSvc = (IUIService)((IServiceProvider)this).GetService(typeof(IUIService));
            if (uiSvc != null)
            {
                return uiSvc.ShowDialog(dialog);
            }
            else
            {
                return dialog.ShowDialog(_designer.Component as IWin32Window);
            }
        }

        /// <summary>
        ///  When the verb is invoked, use all the stuff above to show the dialog, etc.
        /// </summary>
        private void OnEditItems(object sender, EventArgs e)
        {
            DesignerActionUIService actionUIService = (DesignerActionUIService)((IServiceProvider)this).GetService(typeof(DesignerActionUIService));
            if (actionUIService != null)
            {
                actionUIService.HideUI(_designer.Component);
            }
            object propertyValue = _targetProperty.GetValue(_designer.Component);
            if (propertyValue is null)
            {
                return;
            }
            CollectionEditor itemsEditor = TypeDescriptor.GetEditor(propertyValue, typeof(UITypeEditor)) as CollectionEditor;
            Debug.Assert(itemsEditor != null, "Didn't get a collection editor for type '" + _targetProperty.PropertyType.FullName + "'");
            if (itemsEditor != null)
            {
                itemsEditor.EditValue(this, this, propertyValue);
            }
        }
    }
}
