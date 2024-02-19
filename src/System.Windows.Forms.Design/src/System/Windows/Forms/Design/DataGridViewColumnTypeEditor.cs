// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;
internal class DataGridViewColumnTypeEditor : UITypeEditor
{
    public DataGridViewColumnTypeEditor() : base() { }

    private DataGridViewColumnTypePicker? _columnTypePicker;

    public override bool IsDropDownResizable
    {
        get
        {
            return true;
        }
    }

    public override object EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        if (provider is not null)
        {
            IWindowsFormsEditorService? edSvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

            if (edSvc is not null && context?.Instance is not null)
            {
                _columnTypePicker ??= new DataGridViewColumnTypePicker();

                DataGridViewColumnCollectionDialog.ListBoxItem item = (DataGridViewColumnCollectionDialog.ListBoxItem)context.Instance;

                IDesignerHost? host = provider.GetService(typeof(IDesignerHost)) as IDesignerHost;
                ITypeDiscoveryService? discoveryService = null;
                if (host is not null)
                {
                    discoveryService = host.GetService(typeof(ITypeDiscoveryService)) as ITypeDiscoveryService;
                }

                _columnTypePicker.Start(edSvc, discoveryService!, item.DataGridViewColumn.GetType());
                edSvc.DropDownControl(_columnTypePicker);
                if (_columnTypePicker.SelectedType is not null)
                {
                    value = _columnTypePicker.SelectedType;
                }
            }
        }

        return value!;
    }

    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
    {
        return UITypeEditorEditStyle.DropDown;
    }
}
