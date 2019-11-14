// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms.Design
{
    internal class MaskedTextBoxTextEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context?.Instance == null || provider == null)
            {
                return value;
            }

            if (!(provider.GetService(typeof(IWindowsFormsEditorService)) is IWindowsFormsEditorService editorService))
            {
                return value;
            }

            MaskedTextBox maskedTextBox = context.Instance as MaskedTextBox;
            if (maskedTextBox == null)
            {
                maskedTextBox = new MaskedTextBox();
                maskedTextBox.Text = value as string;
            }

            MaskedTextBoxTextEditorDropDown dropDown = new MaskedTextBoxTextEditorDropDown(maskedTextBox);
            editorService.DropDownControl(dropDown);

            if (dropDown.Value != null)
            {
                value = dropDown.Value;
            }

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                return UITypeEditorEditStyle.DropDown;
            }

            return base.GetEditStyle(context);
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                return false;
            }

            return base.GetPaintValueSupported(context);
        }

        public override bool IsDropDownResizable => false;
    }
}
