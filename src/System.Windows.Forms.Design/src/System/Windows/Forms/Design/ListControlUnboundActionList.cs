// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

internal class ListControlUnboundActionList : DesignerActionList
{
    private readonly ComponentDesigner _designer;
    private readonly DesignerActionUIService? _uiService;
    private bool _boundMode;
    private object? _boundSelectedValue;

    public ListControlUnboundActionList(ComponentDesigner designer)
        : base(designer.Component)
    {
        _designer = designer;
        ListControl listControl = (ListControl)Component!;
        _boundMode = listControl.DataSource is not null;
        _uiService = GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
    }

    private void RefreshPanelContent()
    {
        _uiService?.Refresh(_designer.Component);
    }

    public bool BoundMode
    {
        get => _boundMode;
        set
        {
            if (!value)
            {
                DataSource = null;
            }

            if (((ListControl)Component!).DataSource is null)
            {
                _boundMode = value;
            }

            RefreshPanelContent();
        }
    }

    [AttributeProvider(typeof(IListSource))]
    public object? DataSource
    {
        get => ((ListControl)Component!).DataSource;
        set
        {
            ListControl listControl = (ListControl)Component!;
            IDesignerHost? host = GetService(typeof(IDesignerHost)) as IDesignerHost;
            IComponentChangeService? changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            PropertyDescriptor? prop = TypeDescriptor.GetProperties(listControl)["DataSource"];

            if (host is not null && changeService is not null)
            {
                using DesignerTransaction transaction = host.CreateTransaction("Set ListControl.DataSource");
                changeService.OnComponentChanging(Component!, prop);
                listControl.DataSource = value;

                if (value is null)
                {
                    listControl.DisplayMember = string.Empty;
                    listControl.ValueMember = string.Empty;
                }

                changeService.OnComponentChanged(Component!, prop, null, null);
                transaction.Commit();
                RefreshPanelContent();
            }
        }
    }

    [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, " + AssemblyRef.SystemDesign, typeof(Drawing.Design.UITypeEditor))]
    public string DisplayMember
    {
        get => ((ListControl)Component!).DisplayMember;
        set
        {
            ListControl listControl = (ListControl)Component!;
            IDesignerHost? host = GetService(typeof(IDesignerHost)) as IDesignerHost;
            IComponentChangeService? changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            PropertyDescriptor? prop = TypeDescriptor.GetProperties(listControl)["DisplayMember"];

            if (host is not null && changeService is not null)
            {
                using DesignerTransaction transaction = host.CreateTransaction("Set ListControl.DisplayMember");
                changeService.OnComponentChanging(Component!, prop);
                listControl.DisplayMember = value;
                changeService.OnComponentChanged(Component!, prop, null, null);
                transaction.Commit();
            }
        }
    }

    [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, " + AssemblyRef.SystemDesign, typeof(Drawing.Design.UITypeEditor))]
    public string ValueMember
    {
        get => ((ListControl)Component!).ValueMember;
        set
        {
            ListControl listControl = (ListControl)Component!;
            IDesignerHost? host = GetService(typeof(IDesignerHost)) as IDesignerHost;
            IComponentChangeService? changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            PropertyDescriptor? prop = TypeDescriptor.GetProperties(listControl)["ValueMember"];

            if (host is not null && changeService is not null)
            {
                using DesignerTransaction transaction = host.CreateTransaction("Set ListControl.ValueMember");
                changeService.OnComponentChanging(Component!, prop);
                listControl.ValueMember = value;
                changeService.OnComponentChanged(Component!, prop, null, null);
                transaction.Commit();
            }
        }
    }

    [TypeConverter("System.Windows.Forms.Design.DesignBindingConverter")]
    [Editor("System.Windows.Forms.Design.DesignBindingEditor, " + AssemblyRef.SystemDesign, typeof(Drawing.Design.UITypeEditor))]
    public object? BoundSelectedValue
    {
        get
        {
            Binding? binding = GetSelectedValueBinding();
            string? dataMember = binding?.BindingMemberInfo.BindingMember;
            object? dataSource = binding?.DataSource;

            string typeName = $"System.Windows.Forms.Design.DesignBinding, {typeof(ComponentDesigner).Assembly.FullName}";

            _boundSelectedValue = TypeDescriptor.CreateInstance(
                null,
                Type.GetType(typeName)!,
                [typeof(object), typeof(string)],
                [ dataSource, dataMember]);

            return _boundSelectedValue;
        }
        set
        {
            if (value is string stringValue)
            {
                PropertyDescriptor? prop = TypeDescriptor.GetProperties(this)["BoundSelectedValue"];
                TypeConverter? converter = prop?.Converter;
                _boundSelectedValue = converter?.ConvertFrom(new EditorServiceContext(_designer, prop), null, stringValue);
            }
            else
            {
                _boundSelectedValue = value;
                if (value is not null && _boundSelectedValue is not null)
                {
                    object? dataSource = TypeDescriptor.GetProperties(_boundSelectedValue)["DataSource"]?
                                                       .GetValue(_boundSelectedValue);
                    string? dataMember = (string?)TypeDescriptor.GetProperties(_boundSelectedValue)["DataMember"]?
                                                           .GetValue(_boundSelectedValue);
                    SetSelectedValueBinding(dataSource, dataMember);
                }
            }
        }
    }

    private Binding? GetSelectedValueBinding()
    {
        ListControl listControl = (ListControl)Component!;
        foreach (Binding binding in listControl.DataBindings)
        {
            if (binding.PropertyName == "SelectedValue")
            {
                return binding;
            }
        }

        return null;
    }

    private void SetSelectedValueBinding(object? dataSource, string? dataMember)
    {
        ListControl listControl = (ListControl)Component!;
        IDesignerHost? host = GetService(typeof(IDesignerHost)) as IDesignerHost;
        IComponentChangeService? changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
        PropertyDescriptor? prop = TypeDescriptor.GetProperties(listControl)["DataBindings"];

        if (host is not null && changeService is not null)
        {
            using DesignerTransaction transaction = host.CreateTransaction("Set ListControl.SelectedValue binding");
            changeService.OnComponentChanging(Component!, prop);

            Binding? existing = GetSelectedValueBinding();
            if (existing is not null)
            {
                listControl.DataBindings.Remove(existing);
            }

            if (dataSource is not null && !string.IsNullOrEmpty(dataMember))
            {
                listControl.DataBindings.Add("SelectedValue", dataSource, dataMember);
            }

            changeService.OnComponentChanged(Component!, prop, null, null);
            transaction.Commit();
        }
    }

    public void InvokeItemsDialog()
    {
        EditorServiceContext.EditValue(_designer, Component!, "Items");
    }

    public override DesignerActionItemCollection GetSortedActionItems()
    {
        DesignerActionItemCollection items = new();

        items.Add(new DesignerActionPropertyItem(
            "BoundMode", "SR.ListControlBoundModeDisplayName", "SR.DataCategoryName", "SR.ListControlBoundModeDescription"));

        if (_boundMode || ((ListControl)Component!).DataSource is not null)
        {
            _boundMode = true;

            // TODO: Find definitive solution after build works
            items.Add(new DesignerActionHeaderItem("SR.ListControlBoundModeHeader", "SR.DataCategoryName"));
            items.Add(new DesignerActionPropertyItem("DataSource", "SR.DataSourceDisplayName", "SR.DataCategoryName", "SR.DataSourceDescription"));
            items.Add(new DesignerActionPropertyItem("DisplayMember", "SR.DisplayMemberDisplayName", "SR.DataCategoryName", "SR.DisplayMemberDescription"));
            items.Add(new DesignerActionPropertyItem("ValueMember", "SR.ValueMemberDisplayName", "SR.DataCategoryName", "SR.ValueMemberDescription"));
            items.Add(new DesignerActionPropertyItem("BoundSelectedValue", "SR.BoundSelectedValueDisplayName", "SR.DataCategoryName", "SR.BoundSelectedValueDescription"));
        }
        else
        {
            items.Add(new DesignerActionHeaderItem("SR.ListControlUnboundModeHeader", "SR.DataCategoryName"));
            items.Add(new DesignerActionMethodItem(this, nameof(InvokeItemsDialog),
                SR.ListControlUnboundActionListEditItemsDisplayName,
                SR.ItemsCategoryName,
                SR.ListControlUnboundActionListEditItemsDescription,
                true));
        }

        return items;
    }
}
