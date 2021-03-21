// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class PropertyDescriptorGridEntry
    {
        /// <summary>
        ///  The exception editor displays a message to the user.
        /// </summary>
        private class ExceptionEditor : UITypeEditor
        {
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                if (value is Exception except)
                {
                    IUIService uis = null;

                    if (context is not null)
                    {
                        uis = (IUIService)context.GetService(typeof(IUIService));
                    }

                    if (uis is not null)
                    {
                        uis.ShowError(except);
                    }
                    else
                    {
                        string message = except.Message;
                        if (message is null || message.Length == 0)
                        {
                            message = except.ToString();
                        }

                        RTLAwareMessageBox.Show(null, message, SR.PropertyGridExceptionInfo,
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);
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
