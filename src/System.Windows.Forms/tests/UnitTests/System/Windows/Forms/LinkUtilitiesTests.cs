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
    [InlineData(LinkBehavior.AlwaysUnderline, FontStyle.Underline, FontStyle.Underline)]
    [InlineData(LinkBehavior.HoverUnderline, FontStyle.Regular, FontStyle.Underline)]
    [InlineData(LinkBehavior.NeverUnderline, FontStyle.Regular, FontStyle.Regular)]
    [InlineData(LinkBehavior.SystemDefault, FontStyle.Regular, FontStyle.Regular)] // Adjust if different default styles are expected
    public void LinkUtilities_EnsureLinkFonts_CreatesExpectedFonts(LinkBehavior behavior, FontStyle linkFontStyle, FontStyle hoverLinkFontStyle)
    {
        using Font baseFont = new("Arial", 12);
        Font? linkFont = null;
        Font? hoverLinkFont = null;

        LinkUtilities.EnsureLinkFonts(baseFont, behavior, ref linkFont, ref hoverLinkFont);

        linkFont.Should().NotBeNull();
        hoverLinkFont.Should().NotBeNull();
        linkFont!.Style.Should().Be(linkFontStyle);
        hoverLinkFont!.Style.Should().Be(hoverLinkFontStyle);
    }

    [WinFormsTheory]
    [InlineData(LinkBehavior.AlwaysUnderline, FontStyle.Underline | FontStyle.Bold, FontStyle.Underline | FontStyle.Bold)]
    [InlineData(LinkBehavior.HoverUnderline, FontStyle.Regular, FontStyle.Underline)]
    [InlineData(LinkBehavior.NeverUnderline, FontStyle.Bold, FontStyle.Bold)]
    public void LinkUtilities_EnsureLinkFonts_CreatesExpectedFonts_WithActive(LinkBehavior behavior, FontStyle linkFontStyle, FontStyle hoverLinkFontStyle)
    {
        using Font baseFont = new("Arial", 12);
        Font? linkFont = null;
        Font? hoverLinkFont = null;

        LinkUtilities.EnsureLinkFonts(baseFont, behavior, ref linkFont, ref hoverLinkFont, isActive: true);

        linkFont.Should().NotBeNull();
        hoverLinkFont.Should().NotBeNull();
        linkFont!.Style.Should().Be(linkFontStyle);
        hoverLinkFont!.Style.Should().Be(hoverLinkFontStyle);
    }
}
