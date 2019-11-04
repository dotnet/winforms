// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing.Design;

namespace System.Windows.Forms.Design.Editors
{
    internal class FormatStringEditor : UITypeEditor {

        private FormatStringDialog formatStringDialog;
        
        /// Edits the specified value using the specified provider within the specified context.
        public override object EditValue(ITypeDescriptorContext context,  IServiceProvider  provider, object value) {
            if (provider != null) {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (edSvc != null) {
                    DataGridViewCellStyle dgvCellStyle = context.Instance as DataGridViewCellStyle;
                    ListControl listControl = context.Instance as ListControl;

                    Debug.Assert(listControl != null || dgvCellStyle != null, "this editor is used for the DataGridViewCellStyle::Format and the ListControl::FormatString properties");
                    Application.SetHighDpiMode(HighDpiMode.SystemAware);
                    if (formatStringDialog == null) {
                        formatStringDialog = new FormatStringDialog(context);
                    }

                    if (listControl != null) {
                        formatStringDialog.ListControl = listControl;
                    } else {
                        formatStringDialog.DataGridViewCellStyle = dgvCellStyle;
                    }

                    IComponentChangeService changeSvc = (IComponentChangeService)provider.GetService(typeof(IComponentChangeService));
                    if (changeSvc != null) {
                        if (dgvCellStyle != null) {
                            changeSvc.OnComponentChanging(dgvCellStyle, TypeDescriptor.GetProperties(dgvCellStyle)["Format"]);
                            changeSvc.OnComponentChanging(dgvCellStyle, TypeDescriptor.GetProperties(dgvCellStyle)["NullValue"]);
                            changeSvc.OnComponentChanging(dgvCellStyle, TypeDescriptor.GetProperties(dgvCellStyle)["FormatProvider"]);
                        } else {
                            changeSvc.OnComponentChanging(listControl, TypeDescriptor.GetProperties(listControl)["FormatString"]);
                            changeSvc.OnComponentChanging(listControl, TypeDescriptor.GetProperties(listControl)["FormatInfo"]);
                        }
                    }

                    edSvc.ShowDialog(formatStringDialog);
                    formatStringDialog.End();

                    if (formatStringDialog.Dirty) {
                        // since the bindings may have changed, the properties listed in the properties window need to be refreshed
                        TypeDescriptor.Refresh(context.Instance);
                        if (changeSvc != null) {
                            if (dgvCellStyle != null) {
                                changeSvc.OnComponentChanged(dgvCellStyle, TypeDescriptor.GetProperties(dgvCellStyle)["Format"], null, null);
                                changeSvc.OnComponentChanged(dgvCellStyle, TypeDescriptor.GetProperties(dgvCellStyle)["NullValue"], null, null);
                                changeSvc.OnComponentChanged(dgvCellStyle, TypeDescriptor.GetProperties(dgvCellStyle)["FormatProvider"], null, null);
                            } else {
                                changeSvc.OnComponentChanged(listControl, TypeDescriptor.GetProperties(listControl)["FormatString"], null, null);
                                changeSvc.OnComponentChanged(listControl, TypeDescriptor.GetProperties(listControl)["FormatInfo"], null, null);
                            }
                        }
                    }
                }
            }

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
