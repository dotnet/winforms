// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design.Tests;

public sealed class NotifyIconActionListTests : IDisposable
{
    private readonly NotifyIconDesigner _designer;
    private readonly NotifyIcon _notifyIcon;
    private readonly NotifyIconActionList _actionList;

    public NotifyIconActionListTests()
    {
        _designer = new();
        _notifyIcon = new();
        _designer.Initialize(_notifyIcon);
        _actionList = new(_designer);
    }

    public void Dispose()
    {
        _designer.Dispose();
        _notifyIcon.Dispose();
    }

    [Fact]
    public void Constructor_ShouldInitializeActionList() => _actionList.Should().NotBeNull();

    [Fact]
    public void ChooseIcon_ShouldNotBeNull()
    {
        Action act = _actionList.ChooseIcon;
        act.Should().NotBeNull();
    }

    [Fact]
    public void GetSortedActionItems_ShouldReturnCorrectItems()
    {
        DesignerActionItemCollection items = _actionList.GetSortedActionItems();

        items.Should().NotBeNull();
        items.Count.Should().Be(1);
        items[0].Should().BeOfType<DesignerActionMethodItem>();
        DesignerActionMethodItem methodItem = (DesignerActionMethodItem)items[0];
        methodItem.MemberName.Should().Be(nameof(NotifyIconActionList.ChooseIcon));
        methodItem.DisplayName.Should().Be(SR.ChooseIconDisplayName);
        methodItem.IncludeAsDesignerVerb.Should().BeTrue();
    }
}
