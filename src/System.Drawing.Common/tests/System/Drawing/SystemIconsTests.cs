// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Tests;

public class SystemIconsTests
{
    public static IEnumerable<object[]> SystemIcons_TestData()
    {
        yield return Icon(() => SystemIcons.Application);
        yield return Icon(() => SystemIcons.Asterisk);
        yield return Icon(() => SystemIcons.Error);
        yield return Icon(() => SystemIcons.Exclamation);
        yield return Icon(() => SystemIcons.Hand);
        yield return Icon(() => SystemIcons.Information);
        yield return Icon(() => SystemIcons.Question);
        yield return Icon(() => SystemIcons.Shield);
        yield return Icon(() => SystemIcons.Warning);
        yield return Icon(() => SystemIcons.WinLogo);
    }

    public static object[] Icon(Func<Icon> getIcon) => [getIcon];

    [Theory]
    [MemberData(nameof(SystemIcons_TestData))]
    public void SystemIcons_Get_ReturnsExpected(Func<Icon> getIcon)
    {
        Icon icon = getIcon();
        Assert.Same(icon, getIcon());
    }

    [Theory]
    [EnumData<StockIconId>]
    public void SystemIcons_GetStockIcon(StockIconId stockIcon)
    {
        using Icon icon = SystemIcons.GetStockIcon(stockIcon);
        Assert.NotNull(icon);
    }

    [Fact]
    public void SystemIcons_GetStockIcon_BySize()
    {
        using Icon icon = SystemIcons.GetStockIcon(StockIconId.Lock, size: 256);
        Assert.NotNull(icon);
        Assert.Equal(256, icon.Width);
        Assert.Equal(256, icon.Height);
    }

    [Fact]
    public void SystemIcons_GetStockIcon_InvalidId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => SystemIcons.GetStockIcon((StockIconId)(-1)));
    }

    public static TheoryData<StockIconOptions> StockIconOptions_TestData => new()
    {
        StockIconOptions.LinkOverlay,
        StockIconOptions.Selected,
        StockIconOptions.ShellIconSize,
        StockIconOptions.SmallIcon,
        StockIconOptions.LinkOverlay | StockIconOptions.Selected | StockIconOptions.ShellIconSize | StockIconOptions.SmallIcon,
    };

    [Theory]
    [MemberData(nameof(StockIconOptions_TestData))]
    public void SystemIcons_GetStockIcon_Options(StockIconOptions options)
    {
        using Icon icon = SystemIcons.GetStockIcon(StockIconId.Shield, options);
        Assert.NotNull(icon);
    }
}
