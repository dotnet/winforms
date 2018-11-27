// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.Windows.Forms.Design.Editors.Resources;

namespace System.Windows.Forms.Design
{
    /// <summary>>
    ///         Provides an
    ///         editor for filenames.
    /// </summary>
    [CLSCompliant(false)]
    public class FileNameEditor : UITypeEditor
    {
        private OpenFileDialog openFileDialog;

        /// <summary>
        ///     Edits the given object value using the editor style provided by
        ///     GetEditorStyle.  A service provider is provided so that any
        ///     required editing services can be obtained.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            Debug.Assert(provider != null, "No service provider; we cannot edit the value");
            if (provider != null)
            {
                IWindowsFormsEditorService edSvc =
                    (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                Debug.Assert(edSvc != null, "No editor service; we cannot edit the value");
                if (edSvc != null)
                {
                    if (openFileDialog == null)
                    {
                        openFileDialog = new OpenFileDialog();
                        InitializeDialog(openFileDialog);
                    }

                    if (value is string) openFileDialog.FileName = (string)value;

                    if (openFileDialog.ShowDialog() == DialogResult.OK) value = openFileDialog.FileName;
                }
            }

            return value;
        }

        /// <summary>>
        ///         Gets the editing style of the Edit method. If the method
        ///         is not supported, this will return None.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        // everything in this assembly is full trust.
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        ///     Initializes the open file dialog when it is created.  This gives you
        ///     an opportunity to configure the dialog as you please.  The default
        ///     implementation provides a generic file filter and title.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        protected virtual void InitializeDialog(OpenFileDialog openFileDialog)
        {
            openFileDialog.Filter = SR.GenericFileFilter;
            openFileDialog.Title = SR.GenericOpenFile;
        }
    }
}
