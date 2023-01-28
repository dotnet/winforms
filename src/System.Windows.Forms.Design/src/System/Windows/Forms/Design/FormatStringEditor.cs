// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing.Design;

namespace System.Windows.Forms.Design
{
    internal class FormatStringEditor : UITypeEditor
    {
        private FormatStringDialog _formatStringDialog;

        /// Edits the specified value using the specified provider within the specified context.
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (!provider.TryGetService(out IWindowsFormsEditorService editorService))
            {
                return value;
            }

            var cellStyle = context.Instance as DataGridViewCellStyle;
            var listControl = context.Instance as ListControl;

            Debug.Assert(
                listControl is not null || cellStyle is not null,
                "this editor is used for the DataGridViewCellStyle::Format and the ListControl::FormatString properties");

            Application.SetHighDpiMode(HighDpiMode.SystemAware);

            _formatStringDialog ??= new FormatStringDialog(context);

            if (listControl is not null)
            {
                _formatStringDialog.ListControl = listControl;
            }
            else
            {
                _formatStringDialog.DataGridViewCellStyle = cellStyle;
            }

            if (provider.TryGetService(out IComponentChangeService changeService))
            {
                if (cellStyle is not null)
                {
                    changeService.OnComponentChanging(cellStyle, TypeDescriptor.GetProperties(cellStyle)["Format"]);
                    changeService.OnComponentChanging(cellStyle, TypeDescriptor.GetProperties(cellStyle)["NullValue"]);
                    changeService.OnComponentChanging(cellStyle, TypeDescriptor.GetProperties(cellStyle)["FormatProvider"]);
                }
                else
                {
                    changeService.OnComponentChanging(listControl, TypeDescriptor.GetProperties(listControl)["FormatString"]);
                    changeService.OnComponentChanging(listControl, TypeDescriptor.GetProperties(listControl)["FormatInfo"]);
                }
            }

            editorService.ShowDialog(_formatStringDialog);
            FormatStringDialog.End();

            if (!_formatStringDialog.Dirty)
            {
                return value;
            }

            // Since the bindings may have changed, the properties listed in the properties window need to be refreshed.
            TypeDescriptor.Refresh(context.Instance);

            if (changeService is not null)
            {
                if (cellStyle is not null)
                {
                    changeService.OnComponentChanged(cellStyle, TypeDescriptor.GetProperties(cellStyle)["Format"]);
                    changeService.OnComponentChanged(cellStyle, TypeDescriptor.GetProperties(cellStyle)["NullValue"]);
                    changeService.OnComponentChanged(cellStyle, TypeDescriptor.GetProperties(cellStyle)["FormatProvider"]);
                }
                else
                {
                    changeService.OnComponentChanged(listControl, TypeDescriptor.GetProperties(listControl)["FormatString"]);
                    changeService.OnComponentChanged(listControl, TypeDescriptor.GetProperties(listControl)["FormatInfo"]);
                }
            }

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            => UITypeEditorEditStyle.Modal;
    }
}
