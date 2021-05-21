// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace System.Windows.Forms
{
    public abstract partial class AxHost
    {
        private class AxPropertyTypeEditor : UITypeEditor
        {
            private readonly AxPropertyDescriptor propDesc;
            private Guid guid;

            public AxPropertyTypeEditor(AxPropertyDescriptor pd, Guid guid)
            {
                propDesc = pd;
                this.guid = guid;
            }

            /// <summary>
            ///  Takes the value returned from valueAccess.getValue() and modifies or replaces
            ///  the value, passing the result into valueAccess.setValue().  This is where
            ///  an editor can launch a modal dialog or create a drop down editor to allow
            ///  the user to modify the value.  Host assistance in presenting UI to the user
            ///  can be found through the valueAccess.getService function.
            /// </summary>
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                try
                {
                    object instance = context.Instance;
                    propDesc._owner.ShowPropertyPageForDispid(propDesc.Dispid, guid);
                }
                catch (Exception ex1)
                {
                    if (provider is not null)
                    {
                        IUIService uiSvc = (IUIService)provider.GetService(typeof(IUIService));
                        if (uiSvc is not null)
                        {
                            uiSvc.ShowError(ex1, SR.ErrorTypeConverterFailed);
                        }
                    }
                }

                return value;
            }

            /// <summary>
            ///  Retrieves the editing style of the Edit method.  If the method
            ///  is not supported, this will return None.
            /// </summary>
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }
        }
    }
}
