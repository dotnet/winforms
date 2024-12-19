// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;
using Microsoft.Win32;

namespace System.Windows.Forms.Tests;

public class LinkUtilitiesTests
{
    [WinFormsTheory]
    [InlineData("no", LinkBehavior.NeverUnderline)]
    [InlineData("hover", LinkBehavior.HoverUnderline)]
    [InlineData("always", LinkBehavior.AlwaysUnderline)]
    [InlineData(null, LinkBehavior.AlwaysUnderline)] // Default behavior when key is not set
    public void LinkUtilities_GetIELinkBehavior_ReturnsExpected(string? registryValue, LinkBehavior expectedBehavior)
    {
        // Set registry value if provided
        RegistryKey? key = Registry.CurrentUser.CreateSubKey(LinkUtilities.IEMainRegPath);
        if (registryValue is not null)
        {
            key.SetValue("Anchor Underline", registryValue);
        }
        else
        {
            key.DeleteValue("Anchor Underline", false); // Ensure no value is set
        }

        LinkUtilities.GetIELinkBehavior().Should().Be(expectedBehavior);
    }

    [WinFormsTheory]
    [InlineData(LinkBehavior.AlwaysUnderline)]
    [InlineData(LinkBehavior.HoverUnderline)]
    [InlineData(LinkBehavior.NeverUnderline)]
    [InlineData(LinkBehavior.SystemDefault)]
    public void LinkUtilities_EnsureLinkFonts_CreatesExpectedFonts(LinkBehavior behavior)
    {
        using Font baseFont = new("Arial", 12);
        Font? linkFont = null;
        Font? hoverLinkFont = null;

        Action act = () => LinkUtilities.EnsureLinkFonts(baseFont, behavior, ref linkFont, ref hoverLinkFont);

        act.Should().NotThrow();
        linkFont.Should().BeOfType<Font>();
        hoverLinkFont.Should().BeOfType<Font>();
    }

    [WinFormsTheory]
    [InlineData(LinkBehavior.AlwaysUnderline)]
    [InlineData(LinkBehavior.HoverUnderline)]
    [InlineData(LinkBehavior.NeverUnderline)]
    public void LinkUtilities_EnsureLinkFonts_CreatesExpectedFonts_WithActive(LinkBehavior behavior)
    {
        using Font baseFont = new("Arial", 12);
        Font? linkFont = null;
        Font? hoverLinkFont = null;

        Action act = () => LinkUtilities.EnsureLinkFonts(baseFont, behavior, ref linkFont, ref hoverLinkFont, isActive: true);

        act.Should().NotThrow();
        linkFont.Should().BeOfType<Font>();
        hoverLinkFont.Should().BeOfType<Font>();
    }
}
