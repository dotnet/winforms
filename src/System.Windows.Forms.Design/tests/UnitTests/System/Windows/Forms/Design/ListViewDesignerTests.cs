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
        _listViewDesigner.Initialize(_listView);
    }

    public void Dispose()
    {
        _listViewDesigner.Dispose();
        _listView.Dispose();
    }

    [WinFormsFact]
    public void ListViewDesigner_AssociatedComponentsTest()
    {
        _listView.Columns.Add("123");
        _listView.Columns.Add("abc");

        _listViewDesigner.AssociatedComponents.Count.Should().Be(2);
    }

    [WinFormsFact]
    public void ListViewDesigner_GetHitTest_ReturnsFalse_WhenViewIsNotDetails()
    {
        _listView.View = View.LargeIcon;

        bool result = _listViewDesigner.TestAccessor().Dynamic.GetHitTest(new Point(10, 10));

        result.Should().BeFalse();
    }

    [WinFormsFact]
    public void ListViewDesigner_GetHitTest_ReturnsFalse_WhenPointIsNotOnHeader()
    {
        _listView.View = View.Details;
        _listView.Columns.Add("Column1");

        bool result = _listViewDesigner.TestAccessor().Dynamic.GetHitTest(new Point(10, 10));

        result.Should().BeFalse();
    }

    [WinFormsFact]
    public void ListViewDesigner_GetHitTest_ReturnsFalse_WhenHeaderHandleIsNull()
    {
        _listView.View = View.Details;
        _listView.Columns.Add("Column1");

        Point point = new(10, 5);
        bool result = _listViewDesigner.TestAccessor().Dynamic.GetHitTest(point);

        result.Should().BeFalse();
    }

    [WinFormsFact]
    public void ListViewDesigner_GetHitTest_ReturnsFalse_WhenPointIsOutsideControl()
    {
        _listView.View = View.Details;
        _listView.Columns.Add("Column1");

        Point outsidePoint = new(-10, -10);
        bool result = _listViewDesigner.TestAccessor().Dynamic.GetHitTest(outsidePoint);

        result.Should().BeFalse();
    }

    [WinFormsFact]
    public void ListViewDesigner_Initialize_SetsOwnerDrawToFalse() =>
    _listView.OwnerDraw.Should().BeFalse();

    [WinFormsFact]
    public void ListViewDesigner_Initialize_SetsUseCompatibleStateImageBehaviorToFalse() =>
        _listView.UseCompatibleStateImageBehavior.Should().BeFalse();

    [WinFormsFact]
    public void ListViewDesigner_Initialize_HooksChildHandles_WhenViewIsDetails()
    {
        _listView.View = View.Details;

        _listViewDesigner.Should().BeOfType<ListViewDesigner>();
    }

    [WinFormsFact]
    public void ListViewDesigner_Initialize_DoesNotHookChildHandles_WhenViewIsNotDetails()
    {
        _listView.View = View.LargeIcon;

        _listViewDesigner.Should().BeOfType<ListViewDesigner>();
    }

    [WinFormsFact]
    public void ListViewDesigner_ActionLists_ReturnsNonNullCollection()
    {
        DesignerActionListCollection actionLists = _listViewDesigner.ActionLists;

        actionLists.Should().BeOfType<DesignerActionListCollection>();
        actionLists[0].Should().BeOfType<ListViewActionList>();
    }

    [WinFormsFact]
    public void ListViewDesigner_ActionLists_ReturnsSameInstanceOnSubsequentCalls()
    {
        DesignerActionListCollection firstCall = _listViewDesigner.ActionLists;
        DesignerActionListCollection secondCall = _listViewDesigner.ActionLists;

        firstCall.Should().BeSameAs(secondCall);
    }

    [WinFormsFact]
    public void ListViewDesigner_ActionLists_ThreadSafeInitialization()
    {
        DesignerActionListCollection[] results = new DesignerActionListCollection[2];
        Parallel.Invoke(
            () => results[0] = _listViewDesigner.ActionLists,
            () => results[1] = _listViewDesigner.ActionLists
        );

        results[0].Should().BeSameAs(results[1]);
    }
}
