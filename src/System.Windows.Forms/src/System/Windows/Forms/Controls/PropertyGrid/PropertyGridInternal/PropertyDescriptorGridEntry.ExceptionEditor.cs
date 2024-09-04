// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class PropertyDescriptorGridEntry
{
    /// <summary>
    ///  The exception editor displays a message to the user.
    /// </summary>
    private class ExceptionEditor : UITypeEditor
    {
        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {
            if (value is Exception ex)
            {
                if (context.TryGetService(out IUIService? uiService))
                {
                    uiService.ShowError(ex);
                }
                else
                {
                    string message = ex.Message;
                    if (message is null || message.Length == 0)
                    {
                        message = ex.ToString();
                    }

                    RTLAwareMessageBox.Show(
                        null,
                        message,
                        SR.PropertyGridExceptionInfo,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1,
                        0);
                }
            }

            return value;
        }

        /// <summary>
        ///  Retrieves the editing style of the Edit method. If the method
        ///  is not supported, this will return None.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
            => UITypeEditorEditStyle.Modal;
    }
}
