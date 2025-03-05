// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;
internal class DataGridViewColumnCollectionEditor : UITypeEditor
{
    public DataGridViewColumnCollectionEditor() : base() { }

    private DataGridViewColumnCollectionDialog? _dataGridViewColumnCollectionDialog;

    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        if (provider is null ||
            !provider.TryGetService(out IWindowsFormsEditorService? editorService) ||
            context?.Instance is null ||
            !provider.TryGetService(out IDesignerHost? host))
        {
            return value;
        }

        using (ScaleHelper.EnterDpiAwarenessScope(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE))
        {
            _dataGridViewColumnCollectionDialog ??= new();

            _dataGridViewColumnCollectionDialog.SetLiveDataGridView((DataGridView)context.Instance);

            using DesignerTransaction transaction = host.CreateTransaction(SR.DataGridViewColumnCollectionTransaction);
            if (editorService.ShowDialog(_dataGridViewColumnCollectionDialog) == DialogResult.OK)
            {
                transaction.Commit();
            }
            else
            {
                transaction.Cancel();
            }
        }

        return value;
    }

    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.Modal;
}
