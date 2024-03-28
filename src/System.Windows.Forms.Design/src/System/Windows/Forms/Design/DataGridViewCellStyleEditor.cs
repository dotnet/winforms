// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

internal class DataGridViewCellStyleEditor : UITypeEditor
{
    private DataGridViewCellStyleBuilder? _builderDialog;

    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        ArgumentNullException.ThrowIfNull(provider);

        if (provider.GetService<IWindowsFormsEditorService>() is null)
        {
            throw new InvalidOperationException($"Service provider couldn't fetch {nameof(IWindowsFormsEditorService)}.");
        }

        IUIService? uiService = provider.GetService<IUIService>();
        IComponent? component = context?.Instance as IComponent;
        using (ScaleHelper.EnterDpiAwarenessScope(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE))
        {
            _builderDialog ??= new DataGridViewCellStyleBuilder(provider, component!);
            if (uiService is not null)
            {
                _builderDialog.Font = (Font)uiService.Styles["DialogFont"]!;
            }

            if (value is DataGridViewCellStyle style)
            {
                _builderDialog.CellStyle = style;
            }

            _builderDialog.Context = context;
            if (_builderDialog.ShowDialog() == DialogResult.OK)
            {
                value = _builderDialog.CellStyle;
            }
        }

        return value;
    }

    /// <inheritdoc />
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.Modal;
}
