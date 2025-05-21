// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.PropertyGridInternal;

namespace System.Windows.Forms.Tests;

public class GridEntryRecreateChildrenEventArgsTests
{
    [WinFormsTheory]
    [InlineData(0, 0)]
    [InlineData(1, 2)]
    [InlineData(-1, 5)]
    [InlineData(10, -3)]
    [InlineData(int.MaxValue, int.MinValue)]
    [InlineData(int.MinValue, int.MaxValue)]
    public void GridEntryRecreateChildrenEventArgs_SetsProperties(int oldCount, int newCount)
    {
        GridEntryRecreateChildrenEventArgs args = new(oldCount, newCount);

        args.OldChildCount.Should().Be(oldCount);
        args.NewChildCount.Should().Be(newCount);
    }

    [WinFormsFact]
    public void GridEntryRecreateChildrenEventArgs_Inherits_EventArgs()
    {
        GridEntryRecreateChildrenEventArgs args = new(1, 2);
        args.Should().BeAssignableTo<EventArgs>();
    }

    [WinFormsFact]
    public void GridEntryRecreateChildrenEventArgs_Properties_AreReadOnly()
    {
        typeof(GridEntryRecreateChildrenEventArgs)
            .GetProperty(nameof(GridEntryRecreateChildrenEventArgs.OldChildCount))!
            .CanWrite.Should().BeFalse();

        typeof(GridEntryRecreateChildrenEventArgs)
            .GetProperty(nameof(GridEntryRecreateChildrenEventArgs.NewChildCount))!
            .CanWrite.Should().BeFalse();
    }
}
