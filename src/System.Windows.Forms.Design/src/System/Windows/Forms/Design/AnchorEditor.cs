// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

/// <summary>
///  Provides a design-time editor for specifying the <see cref="Control.Anchor"/> property.
/// </summary>
[CLSCompliant(false)]
public sealed partial class AnchorEditor : UITypeEditor
{
    private AnchorUI? _anchorUI;

    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        if (!provider.TryGetService(out IWindowsFormsEditorService? editorService))
        {
            return value;
        }

        _anchorUI ??= new AnchorUI();

        _anchorUI.Start(editorService, value);
        editorService.DropDownControl(_anchorUI);
        value = _anchorUI.Value;
        _anchorUI.End();

        return value;
    }

    /// <inheritdoc />
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.DropDown;
}
