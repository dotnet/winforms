// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms.Design
{
    internal class DataGridViewCellStyleEditor : UITypeEditor
    {
        private DataGridViewCellStyleBuilder _builderDialog;

        /// <summary>
        /// Edits the given object value using the editor style provided by GetEditorStyle. A service provider is provided so that any required editing services can be obtained.
        /// </summary>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc == null)
            {
                throw new InvalidOperationException("Service provider couldn't fetch " + nameof(edSvc));
            }

            IUIService uiService = (IUIService)provider.GetService(typeof(IUIService));
            IComponent comp = context.Instance as IComponent;
            using (DpiHelper.EnterDpiAwarenessScope(DpiAwarenessContext.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE))
            {
                _builderDialog ??= new DataGridViewCellStyleBuilder(provider, comp);
                if (uiService != null)
                {
                    _builderDialog.Font = (Font)uiService.Styles["DialogFont"];
                }

                if (value is DataGridViewCellStyle dgvcs)
                {
                    _builderDialog.CellStyle = dgvcs;
                }

                _builderDialog.Context = context;
                try
                {
                    if (_builderDialog.ShowDialog() == DialogResult.OK)
                    {
                        value = _builderDialog.CellStyle;
                    }
                }
                finally
                {
                }
            }
            return value;
        }

        /// <summary>
        /// Retrieves the editing style of the Edit method. If the method is not supported, this will return None.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => UITypeEditorEditStyle.Modal;
    }
}
