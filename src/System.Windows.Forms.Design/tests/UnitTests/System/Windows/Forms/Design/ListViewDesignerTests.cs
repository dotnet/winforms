// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design.Tests;

public class ListViewDesignerTests : IDisposable
{
    private readonly ListViewDesigner _listViewDesigner;
    private readonly ListView _listView;

    public ListViewDesignerTests()
    {
        _listViewDesigner = new();
        _listView = new();
    }

    public void Dispose()
    {
        _listViewDesigner.Dispose();
        _listView.Dispose();
    }

    [WinFormsFact]
    public void ListViewDesigner_AssociatedComponentsTest()
    {
        _listViewDesigner.Initialize(_listView);

        _listView.Columns.Add("123");
        _listView.Columns.Add("abc");

        _listViewDesigner.AssociatedComponents.Count.Should().Be(2);
    }

    [WinFormsFact]
    public void ListViewDesigner_GetHitTest_ReturnsFalse_WhenViewIsNotDetails()
    {
        _listView.View = View.LargeIcon;
        _listViewDesigner.Initialize(_listView);

        bool result = _listViewDesigner.TestAccessor().Dynamic.GetHitTest(new Point(10, 10));

        result.Should().BeFalse();
    }

    [WinFormsFact]
    public void ListViewDesigner_GetHitTest_ReturnsFalse_WhenPointIsNotOnHeader()
    {
        _listView.View = View.Details;
        _listView.Columns.Add("Column1");
        _listViewDesigner.Initialize(_listView);

        bool result = _listViewDesigner.TestAccessor().Dynamic.GetHitTest(new Point(10, 10));

        result.Should().BeFalse();
    }

    [WinFormsFact]
    public void ListViewDesigner_GetHitTest_ReturnsFalse_WhenHeaderHandleIsNull()
    {
        _listView.View = View.Details;
        _listView.Columns.Add("Column1");
        _listViewDesigner.Initialize(_listView);

        Point point = new(10, 5);
        bool result = _listViewDesigner.TestAccessor().Dynamic.GetHitTest(point);

        result.Should().BeFalse();
    }

    [WinFormsFact]
    public void ListViewDesigner_GetHitTest_ReturnsFalse_WhenPointIsOutsideControl()
    {
        _listView.View = View.Details;
        _listView.Columns.Add("Column1");
        _listViewDesigner.Initialize(_listView);

        Point outsidePoint = new(-10, -10);
        bool result = _listViewDesigner.TestAccessor().Dynamic.GetHitTest(outsidePoint);

        result.Should().BeFalse();
    }

    [WinFormsFact]
    public void ListViewDesigner_Initialize_SetsOwnerDrawToFalse()
    {
        _listView.OwnerDraw = true;

        _listViewDesigner.Initialize(_listView);

        _listView.OwnerDraw.Should().BeFalse();
    }

    [WinFormsFact]
    public void ListViewDesigner_Initialize_SetsUseCompatibleStateImageBehaviorToFalse()
    {
        _listView.UseCompatibleStateImageBehavior = true;

        _listViewDesigner.Initialize(_listView);

        _listView.UseCompatibleStateImageBehavior.Should().BeFalse();
    }

    [WinFormsFact]
    public void ListViewDesigner_Initialize_HooksChildHandles_WhenViewIsDetails()
    {
        _listView.View = View.Details;

        _listViewDesigner.Initialize(_listView);

        _listViewDesigner.Should().NotBeNull();
    }

    [WinFormsFact]
    public void ListViewDesigner_Initialize_DoesNotHookChildHandles_WhenViewIsNotDetails()
    {
        _listView.View = View.LargeIcon;

        _listViewDesigner.Initialize(_listView);

        _listViewDesigner.Should().NotBeNull();
    }

    [WinFormsFact]
    public void ListViewDesigner_ActionLists_ReturnsNonNullCollection()
    {
        _listViewDesigner.Initialize(_listView);

        DesignerActionListCollection actionLists = _listViewDesigner.ActionLists;

        actionLists.Should().NotBeNull();
        actionLists[0].Should().BeOfType<ListViewActionList>();
    }

    [WinFormsFact]
    public void ListViewDesigner_ActionLists_ReturnsSameInstanceOnSubsequentCalls()
    {
        _listViewDesigner.Initialize(_listView);

        DesignerActionListCollection firstCall = _listViewDesigner.ActionLists;
        DesignerActionListCollection secondCall = _listViewDesigner.ActionLists;

        firstCall.Should().BeSameAs(secondCall);
    }

    [WinFormsFact]
    public void ListViewDesigner_ActionLists_ThreadSafeInitialization()
    {
        _listViewDesigner.Initialize(_listView);

        DesignerActionListCollection[] results = new DesignerActionListCollection[2];
        Parallel.Invoke(
            () => results[0] = _listViewDesigner.ActionLists,
            () => results[1] = _listViewDesigner.ActionLists
        );

        results[0].Should().BeSameAs(results[1]);
    }
}
