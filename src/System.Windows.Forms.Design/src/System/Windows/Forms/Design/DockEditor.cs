// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

/// <summary>
///  Implements the design time editor for specifying the <see cref="Control.Dock"/> property.
/// </summary>
[CLSCompliant(false)]
public sealed partial class DockEditor : UITypeEditor
{
    private DockUI? _dockUI;

    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        if (!provider.TryGetService(out IWindowsFormsEditorService? editorService))
        {
            return value;
        }

        _dockUI ??= new DockUI();

        _dockUI.Start(editorService, value);
        editorService.DropDownControl(_dockUI);
        value = _dockUI.Value;
        _dockUI.End();

        return value;
    }

    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.DropDown;
}
