// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.Drawing.Design
{
    /// <summary>
    ///       Provides a font editor that
    ///       is used to visually select and configure a Font
    ///       object.
    /// </summary>
    [CLSCompliant(false)]
    public class FontEditor : UITypeEditor
    {
        private FontDialog fontDialog;
        private object value;

        /// <summary>
        ///      Edits the given object value using the editor style provided by
        ///      GetEditorStyle.  A service provider is provided so that any
        ///      required editing services can be obtained.
        /// </summary>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            this.value = value;

            Debug.Assert(provider != null, "No service provider; we cannot edit the value");
            if (provider != null)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                Debug.Assert(edSvc != null, "No editor service; we cannot edit the value");
                if (edSvc != null)
                {
                    if (fontDialog == null)
                    {
                        fontDialog = new FontDialog();
                        fontDialog.ShowApply = false;
                        fontDialog.ShowColor = false;
                        fontDialog.AllowVerticalFonts = false;
                    }

                    Font fontvalue = value as Font;
                    if (fontvalue != null)
                    {
                        fontDialog.Font = fontvalue;
                    }

                    IntPtr hwndFocus = UnsafeNativeMethods.GetFocus();
                    try
                    {
                        if (fontDialog.ShowDialog() == DialogResult.OK)
                        {
                            this.value = fontDialog.Font;
                        }
                    }
                    finally
                    {
                        if (hwndFocus != IntPtr.Zero)
                        {
                            UnsafeNativeMethods.SetFocus(new HandleRef(null, hwndFocus));
                        }
                    }
                }
            }

            // Now pull out the updated value, if there was one.
            value = this.value;
            this.value = null;

            return value;
        }

        /// <summary>
        ///      Retrieves the editing style of the Edit method.  If the method
        ///      is not supported, this will return None.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}

