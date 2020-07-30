// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using static Interop;

namespace System.Windows.Forms.Design
{
    internal class DataGridViewCellStyleEditor : UITypeEditor
    {
        private DataGridViewCellStyleBuilder _builderDialog;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider is null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc is null)
            {
                throw new InvalidOperationException("Service provider couldn't fetch " + nameof(edSvc));
            }

            IUIService uiService = (IUIService)provider.GetService(typeof(IUIService));
            IComponent comp = context.Instance as IComponent;
            using (DpiHelper.EnterDpiAwarenessScope(User32.DPI_AWARENESS_CONTEXT.SYSTEM_AWARE))
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
                if (_builderDialog.ShowDialog() == DialogResult.OK)
                {
                    value = _builderDialog.CellStyle;
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
