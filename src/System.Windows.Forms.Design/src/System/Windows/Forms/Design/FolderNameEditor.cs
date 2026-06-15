// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

/// <summary>
///  Provides an editor for choosing a folder from the filesystem.
/// </summary>
[CLSCompliant(false)]
public partial class FolderNameEditor : UITypeEditor
{
    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        // The dialog is created locally and disposed at the end of the call so its
        // native resources (Component/CommonDialog state) are released, and no stale
        // configuration leaks between successive invocations of EditValue.
        using FolderBrowserDialog folderBrowserDialog = new();
        InitializeDialog(folderBrowserDialog);

        folderBrowserDialog.SelectedPath = value as string ?? string.Empty;

        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
        {
            return folderBrowserDialog.SelectedPath;
        }

        return value;
    }

    /// <inheritdoc />
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.Modal;

    /// <summary>
    ///  Initializes the folder browser dialog when it is created. This gives you an opportunity
    ///  to configure the dialog as you please. The default implementation provides a generic folder browser.
    /// </summary>
    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void InitializeDialog(FolderBrowser folderBrowser)
    {
    }

    /// <summary>
    ///  Initializes the folder browser dialog when it is created. This gives you an opportunity
    ///  to configure the dialog as you please. The default implementation provides a generic folder browser.
    /// </summary>
    protected virtual void InitializeDialog(FolderBrowserDialog folderBrowserDialog)
    {
    }
}
