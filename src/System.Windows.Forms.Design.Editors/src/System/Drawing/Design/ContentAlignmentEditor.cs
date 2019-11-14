// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows.Forms.Design;

namespace System.Drawing.Design
{
    /// <summary>
    /// Provides a <see cref='System.Drawing.Design.UITypeEditor'/> for visually editing content alignment.
    /// </summary>
    public partial class ContentAlignmentEditor : UITypeEditor
    {
        private ContentUI _contentUI;

        /// <summary>
        /// Edits the given object value using the editor style provided by GetEditStyle.
        /// </summary>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider == null)
            {
                return value;
            }

            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc == null)
            {
                return value;
            }

            if (_contentUI == null)
            {
                _contentUI = new ContentUI();
            }

            _contentUI.Start(edSvc, value);
            edSvc.DropDownControl(_contentUI);
            value = _contentUI.Value;
            _contentUI.End();

            return value;
        }

        /// <summary>
        /// Gets the editing style of the Edit method.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            => UITypeEditorEditStyle.DropDown;
    }
}

