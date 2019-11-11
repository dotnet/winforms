// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms.Design.Editors
{
    internal class DataGridViewCellStyleEditor : UITypeEditor
    {

        private DataGridViewCellStyleBuilder builderDialog;

        /// <devdoc>
        ///      Edits the given object value using the editor style provided by GetEditorStyle. A service provider is provided so that any required editing services can be obtained.
        /// </devdoc>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {

            Debug.Assert(provider != null, "No service provider; we cannot edit the value");
            if (provider != null)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                IUIService uiService = (IUIService)provider.GetService(typeof(IUIService));
                IComponent comp = context.Instance as IComponent;

                Debug.Assert(edSvc != null, "No editor service; we cannot edit the value");
                if (edSvc != null)
                {
                    using (DpiHelper.EnterDpiAwarenessScope(DpiAwarenessContext.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE))
                    {
                        if (builderDialog == null)
                        {
                            builderDialog = new DataGridViewCellStyleBuilder(provider, comp);
                        }

                        if (uiService != null)
                        {
                            builderDialog.Font = (Font)uiService.Styles["DialogFont"];
                        }

                        DataGridViewCellStyle dgvcs = value as DataGridViewCellStyle;
                        if (dgvcs != null)
                        {
                            builderDialog.CellStyle = dgvcs;
                        }

                        builderDialog.Context = context;

                        try
                        {
                            if (builderDialog.ShowDialog() == DialogResult.OK)
                            {
                                value = builderDialog.CellStyle;
                            }
                        }
                        finally
                        {
                        }
                    }
                }
            }

            return value;
        }

        /// <devdoc>
        ///      Retrieves the editing style of the Edit method.  If the method is not supported, this will return None.
        /// </devdoc>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
