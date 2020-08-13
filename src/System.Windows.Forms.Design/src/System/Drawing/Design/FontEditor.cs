// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using static Interop;

namespace System.Drawing.Design
{
    /// <summary>
    ///  Provides a font editor that is used to visually select and configure a Font object.
    /// </summary>
    [CLSCompliant(false)]
    public class FontEditor : UITypeEditor
    {
        private FontDialog _fontDialog;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                if (provider.GetService(typeof(IWindowsFormsEditorService)) is IWindowsFormsEditorService edSvc)
                {
                    if (_fontDialog is null)
                    {
                        _fontDialog = new FontDialog
                        {
                            ShowApply = false,
                            ShowColor = false,
                            AllowVerticalFonts = false
                        };
                    }

                    if (value is Font fontValue)
                    {
                        _fontDialog.Font = fontValue;
                    }

                    IntPtr hwndFocus = User32.GetFocus();
                    try
                    {
                        if (_fontDialog.ShowDialog() == DialogResult.OK)
                        {
                            return _fontDialog.Font;
                        }
                    }
                    finally
                    {
                        if (hwndFocus != IntPtr.Zero)
                        {
                            User32.SetFocus(hwndFocus);
                        }
                    }
                }
            }

            return value;
        }

        /// <summary>
        ///  Retrieves the editing style of the Edit method.false
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
