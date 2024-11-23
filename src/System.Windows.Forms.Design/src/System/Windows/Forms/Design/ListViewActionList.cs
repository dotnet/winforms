// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.ComponentModel;

namespace System.Windows.Forms.Design;

internal class ListViewActionList : DesignerActionList
{
    private readonly ComponentDesigner _designer;
    private readonly ListView _listView;

    public ListViewActionList(ComponentDesigner designer)
        : base(designer.Component)
    {
        _designer = designer;
        _listView = (ListView)Component!;
    }

    public void InvokeItemsDialog() =>
        EditorServiceContext.EditValue(_designer, Component!, nameof(ListView.Items));

    public void InvokeColumnsDialog() =>
        EditorServiceContext.EditValue(_designer, Component!, nameof(ListView.Columns));

    public void InvokeGroupsDialog() =>
        EditorServiceContext.EditValue(_designer, Component!, nameof(ListView.Groups));

    public View View
    {
        get => _listView.View;
        set => TypeDescriptor.GetProperties(_listView)[nameof(View)]!.SetValue(Component, value);
    }

    public ImageList? LargeImageList
    {
        get => _listView.LargeImageList;
        set => TypeDescriptor.GetProperties(_listView)[nameof(LargeImageList)]!.SetValue(Component, value);
    }

    public ImageList? SmallImageList
    {
        get => _listView.SmallImageList;
        set => TypeDescriptor.GetProperties(_listView)[nameof(SmallImageList)]!.SetValue(Component, value);
    }

    public override DesignerActionItemCollection GetSortedActionItems() =>
    [
        new DesignerActionMethodItem(this, nameof(InvokeItemsDialog),
            SR.ListViewActionListEditItemsDisplayName,
            SR.PropertiesCategoryName,
            SR.ListViewActionListEditItemsDescription, true),
        new DesignerActionMethodItem(this, nameof(InvokeColumnsDialog),
            SR.ListViewActionListEditColumnsDisplayName,
            SR.PropertiesCategoryName,
            SR.ListViewActionListEditColumnsDescription, true),
        new DesignerActionMethodItem(this, nameof(InvokeGroupsDialog),
            SR.ListViewActionListEditGroupsDisplayName,
            SR.PropertiesCategoryName,
            SR.ListViewActionListEditGroupsDescription, true),
        new DesignerActionPropertyItem(nameof(View),
            SR.ListViewActionListViewDisplayName,
            SR.PropertiesCategoryName,
            SR.ListViewActionListViewDescription),
        new DesignerActionPropertyItem(nameof(SmallImageList),
            SR.ListViewActionListSmallImagesDisplayName,
            SR.PropertiesCategoryName,
            SR.ListViewActionListSmallImagesDescription),
        new DesignerActionPropertyItem(nameof(LargeImageList),
            SR.ListViewActionListLargeImagesDisplayName,
            SR.PropertiesCategoryName,
            SR.ListViewActionListLargeImagesDescription),
    ];
}
