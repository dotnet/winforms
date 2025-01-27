// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

/// <summary>
///  Provides an editor for filenames.
/// </summary>
[CLSCompliant(false)]
public class FileNameEditor : UITypeEditor
{
    private OpenFileDialog? _openFileDialog;

    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        if (!provider.TryGetService(out IWindowsFormsEditorService? _))
        {
            return value;
        }

        if (_openFileDialog is null)
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

        return value;
    }

    /// <inheritdoc />
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.Modal;

    /// <summary>
    ///  Initializes the open file dialog when it is created. This gives you an opportunity to
    ///  configure the dialog as you please. The default implementation provides a generic file
    ///  filter and title.
    /// </summary>
    protected virtual void InitializeDialog(OpenFileDialog openFileDialog)
    {
        ArgumentNullException.ThrowIfNull(openFileDialog);

        openFileDialog.Filter = SR.GenericFileFilter;
        openFileDialog.Title = SR.GenericOpenFile;
    }
}
