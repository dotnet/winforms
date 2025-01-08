// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;
using Microsoft.Win32;

namespace System.Windows.Forms.Tests;

public class LinkUtilitiesTests
{
    [WinFormsFact]
    public void LinkUtilities_GetIELinkBehavior_ReturnsExpected()
    {
        // Read the registry value
        RegistryKey? key = Registry.CurrentUser.OpenSubKey(LinkUtilities.IEMainRegPath);
        string? registryValue = key?.GetValue("Anchor Underline") as string;

        // Determine the expected behavior based on the registry value
        LinkBehavior expectedBehavior = registryValue switch
        {
            "no" => LinkBehavior.NeverUnderline,
            "hover" => LinkBehavior.HoverUnderline,
            "always" => LinkBehavior.AlwaysUnderline,
            _ => LinkBehavior.AlwaysUnderline
        };

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
