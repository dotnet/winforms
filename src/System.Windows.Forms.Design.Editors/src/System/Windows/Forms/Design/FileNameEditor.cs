// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Provides an editor for filenames.
    /// </summary>
    [CLSCompliant(false)]
    public class FileNameEditor : UITypeEditor
    {
        private OpenFileDialog _openFileDialog;

        /// <summary>
        ///  Edits the given object value using the editor style provided by GetEditorStyle.
        ///  A service provider is provided so that any required editing services can be obtained.
        /// </summary>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                if (provider.GetService(typeof(IWindowsFormsEditorService)) is IWindowsFormsEditorService edSvc)
                {
                    if (_openFileDialog == null)
                    {
                        _openFileDialog = new OpenFileDialog();
                        InitializeDialog(_openFileDialog);
                    }

                    if (value is string stringValue)
                    {
                        _openFileDialog.FileName = stringValue;
                    }

                    if (_openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        return _openFileDialog.FileName;
                    }
                }
            }

            return value;
        }

        /// <summary>
        ///  Gets the editing style of the Edit method.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        ///  Initializes the open file dialog when it is created. This gives you an opportunity to
        ///  configure the dialog as you please. The default implementation provides a generic file
        ///  filter and title.
        /// </summary>
        protected virtual void InitializeDialog(OpenFileDialog openFileDialog)
        {
            if (openFileDialog == null)
            {
                throw new ArgumentNullException(nameof(openFileDialog));
            }

            openFileDialog.Filter = SR.GenericFileFilter;
            openFileDialog.Title = SR.GenericOpenFile;
        }
    }
}
