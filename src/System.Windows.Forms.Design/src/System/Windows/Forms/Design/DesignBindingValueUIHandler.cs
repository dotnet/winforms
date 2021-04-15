// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows.Forms;
using Microsoft.Win32;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Drawing;

namespace System.Windows.Forms.Design
{
    /// <include file='doc\DesignBindingValueUIHandler.uex' path='docs/doc[@for="DesignBindingValueUIHandler"]/*' />
    /// <internalonly/>
    /// <devdoc>
    /// </devdoc>
    internal partial class DesignBindingValueUIHandler : IDisposable
    {

        private Bitmap dataBitmap;

        internal Bitmap DataBitmap
        {
            get
            {
                if (dataBitmap == null)
                {
                    dataBitmap = new Bitmap(typeof(DesignBindingValueUIHandler), "BoundProperty.bmp");
                    dataBitmap.MakeTransparent();
                }
                return dataBitmap;
            }
        }


        internal void OnGetUIValueItem(ITypeDescriptorContext context, PropertyDescriptor propDesc, ArrayList valueUIItemList)
        {
            if (context.Instance is Control)
            {
                Control control = (Control)context.Instance;
                foreach (Binding binding in control.DataBindings)
                {

                    // Only add the binding if it is one of the data source types we recognize.  Otherwise, our drop-down list won't show it as
                    // an option, which is confusing.
                    if ((binding.DataSource is IListSource || binding.DataSource is IList || binding.DataSource is Array) && binding.PropertyName.Equals(propDesc.Name))
                    {
                        valueUIItemList.Add(new LocalUIItem(this, binding));
                    }
                }
            }
        }

        private void OnPropertyValueUIItemInvoke(ITypeDescriptorContext context, PropertyDescriptor descriptor, PropertyValueUIItem invokedItem)
        {
            LocalUIItem localItem = (LocalUIItem)invokedItem;
            IServiceProvider sop = null;
            Control control = localItem.Binding.Control;
            if (control.Site != null)
            {
                sop = (IServiceProvider)control.Site.GetService(typeof(IServiceProvider));
            }
            if (sop != null)
            {
                AdvancedBindingPropertyDescriptor.advancedBindingEditor.EditValue(context, sop, control.DataBindings);
            }
        }

        public void Dispose()
        {
            if (dataBitmap != null)
            {
                dataBitmap.Dispose();
            }
        }
    }
}
