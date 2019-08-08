// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Provides an implementation of IWindowsFormsEditorService and ITypeDescriptorContext. Also provides a static method to invoke a UITypeEditor given a designer, an object  and a property name.
    /// </summary>
    internal class EditorServiceContext : IWindowsFormsEditorService, ITypeDescriptorContext
    {
        private readonly ComponentDesigner _designer;
        private IComponentChangeService _componentChangeSvc;
        private readonly PropertyDescriptor _targetProperty;

        internal EditorServiceContext(ComponentDesigner designer)
        {
            _designer = designer;
        }

        internal EditorServiceContext(ComponentDesigner designer, PropertyDescriptor prop)
        {
            _designer = designer;
            _targetProperty = prop;
            if (prop == null)
            {
                prop = TypeDescriptor.GetDefaultProperty(designer.Component);
                if (prop != null && typeof(ICollection).IsAssignableFrom(prop.PropertyType))
                {
                    _targetProperty = prop;
                }
            }
            Debug.Assert(_targetProperty != null, "Need PropertyDescriptor for ICollection property to associate collectoin edtior with.");
        }

        internal EditorServiceContext(ComponentDesigner designer, PropertyDescriptor prop, string newVerbText) : this(designer, prop)
        {
            Debug.Assert(!string.IsNullOrEmpty(newVerbText), "newVerbText cannot be null or empty");
            _designer.Verbs.Add(new DesignerVerb(newVerbText, new EventHandler(OnEditItems)));
        }

        public static object EditValue(ComponentDesigner designer, object objectToChange, string propName)
        {
            // Get PropertyDescriptor
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(objectToChange)[propName];
            // Create a Context
            EditorServiceContext context = new EditorServiceContext(designer, descriptor);
            // Get Editor
            UITypeEditor editor = descriptor.GetEditor(typeof(UITypeEditor)) as UITypeEditor;
            // Get value to edit
            object value = descriptor.GetValue(objectToChange);
            // Edit value
            object newValue = editor.EditValue(context, context, value);

            if (newValue != value)
            {
                try
                {
                    descriptor.SetValue(objectToChange, newValue);
                }
                catch (CheckoutException)
                {

                }
            }
            return newValue;
        }

        /// <summary>
        ///  Our caching property for the IComponentChangeService
        /// </summary>
        private IComponentChangeService ChangeService
        {
            get
            {
                if (_componentChangeSvc == null)
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

        /// <summary>
        ///  Self-explanitory interface impl.
        /// </summary>
        void ITypeDescriptorContext.OnComponentChanged()
        {
            ChangeService.OnComponentChanged(_designer.Component, _targetProperty, null, null);
        }

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

        object ITypeDescriptorContext.Instance
        {
            get => _designer.Component;
        }

        PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor
        {
            get => _targetProperty;
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            if (serviceType == typeof(ITypeDescriptorContext) || serviceType == typeof(IWindowsFormsEditorService))
            {
                return this;
            }
            if (_designer.Component != null && _designer.Component.Site != null)
            {
                return _designer.Component.Site.GetService(serviceType);
            }
            return null;
        }

        void IWindowsFormsEditorService.CloseDropDown()
        {
            // we'll never be called to do this.
            Debug.Fail("NOTIMPL");
            return;
        }

        void IWindowsFormsEditorService.DropDownControl(Control control)
        {
            Debug.Fail("NOTIMPL");
            return;
        }

        System.Windows.Forms.DialogResult IWindowsFormsEditorService.ShowDialog(Form dialog)
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
            object propertyValue = _targetProperty.GetValue(_designer.Component);
            if (propertyValue == null)
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
