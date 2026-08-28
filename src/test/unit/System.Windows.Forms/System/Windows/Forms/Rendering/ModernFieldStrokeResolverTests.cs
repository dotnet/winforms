// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class ModernFieldStrokeResolverTests
{
    private static readonly Color s_accent = Color.FromArgb(0, 120, 215);

    private static ModernFieldStrokeContext Context(
        bool enabled = true,
        bool readOnly = false,
        bool focused = false,
        bool hovered = false,
        bool darkMode = false)
        => new(
            BackColor: Color.White,
            Enabled: enabled,
            ReadOnly: readOnly,
            Focused: focused,
            Hovered: hovered,
            DarkMode: darkMode,
            AccentColor: s_accent,
            DeviceDpi: 96);

    // ---- Precedence: Disabled > Focused > ReadOnly > Hover > Rest ----

    [Fact]
    public void Disabled_wins_over_everything()
    {
        ModernFieldStroke stroke = ModernFieldStrokeResolver.GetStroke(
            Context(enabled: false, focused: true, readOnly: true, hovered: true));

        stroke.SideTopColor.Should().Be(ModernControlColorMath.GetDisabledBorderColor());
        stroke.BottomColor.Should().Be(ModernControlColorMath.GetDisabledStrongBorderColor());
        stroke.SurfaceColor.Should().Be(ModernControlColorMath.GetDisabledSurfaceColor());
    }

    [Fact]
    public void Focused_wins_over_readonly_and_hover()
    {
        ModernFieldStroke stroke = ModernFieldStrokeResolver.GetStroke(
            Context(focused: true, readOnly: true, hovered: true));

        stroke.BottomColor.Should().Be(s_accent);
        stroke.BottomThicknessDip.Should().Be(4f);
    }

    [Fact]
    public void ReadOnly_wins_over_hover_matches_rest_strokes_but_tints_surface()
    {
        ModernFieldStroke readOnly = ModernFieldStrokeResolver.GetStroke(Context(readOnly: true, hovered: true));
        ModernFieldStroke rest = ModernFieldStrokeResolver.GetStroke(Context());

        readOnly.SideTopColor.Should().Be(rest.SideTopColor);
        readOnly.BottomColor.Should().Be(rest.BottomColor);
        readOnly.SurfaceColor.Should().NotBe(rest.SurfaceColor);
    }

    [Fact]
    public void Hover_keeps_the_rest_surface_and_differs_by_border()
    {
        ModernFieldStroke hover = ModernFieldStrokeResolver.GetStroke(Context(hovered: true));
        ModernFieldStroke rest = ModernFieldStrokeResolver.GetStroke(Context());

        hover.SurfaceColor.Should().Be(rest.SurfaceColor);
        hover.SideTopColor.Should().NotBe(rest.SideTopColor);
    }

    // ---- Thicknesses (in DIPs) ----

    [Fact]
    public void Rest_uses_two_dip_side_and_bottom()
    {
        ModernFieldStroke rest = ModernFieldStrokeResolver.GetStroke(Context());

        rest.SideTopThicknessDip.Should().Be(2f);
        rest.BottomThicknessDip.Should().Be(2f);
    }

    [Fact]
    public void Focus_bottom_is_four_dip()
        => ModernFieldStrokeResolver.GetStroke(Context(focused: true)).BottomThicknessDip.Should().Be(4f);

    [Theory]
    [InlineData(96)]
    [InlineData(144)]
    [InlineData(192)]
    public void Thickness_is_expressed_in_dips_independent_of_dpi(int dpi)
    {
        ModernFieldStroke stroke = ModernFieldStrokeResolver.GetStroke(Context() with { DeviceDpi = dpi });

        stroke.SideTopThicknessDip.Should().Be(2f);
        stroke.BottomThicknessDip.Should().Be(2f);
    }

    // ---- Color math (linear-light overlays) ----

    [Fact]
    public void Resolved_colors_are_opaque()
    {
        ModernFieldStroke rest = ModernFieldStrokeResolver.GetStroke(Context());

        rest.SideTopColor.A.Should().Be(255);
        rest.BottomColor.A.Should().Be(255);
        rest.SurfaceColor.A.Should().Be(255);
    }

    [Fact]
    public void Stronger_overlay_is_darker_and_neutral_over_white()
    {
        Color light = ModernControlColorMath.GetFieldStrokeDefault(Color.White, darkMode: false);
        Color strong = ModernControlColorMath.GetFieldStrokeStrong(Color.White, darkMode: false);

        light.R.Should().BeGreaterThan(strong.R);
        light.R.Should().Be(light.G);
        light.G.Should().Be(light.B);
    }

    [Fact]
    public void Dark_mode_lightens_the_stroke_over_a_dark_surface()
    {
        Color darkBackground = Color.FromArgb(32, 32, 32);

        Color stroke = ModernControlColorMath.GetFieldStrokeDefault(darkBackground, darkMode: true);

        stroke.R.Should().BeGreaterThan(darkBackground.R);
    }

    [Fact]
    public void ForeColor_cannot_affect_the_stroke_by_construction()
        => typeof(ModernFieldStrokeContext).GetProperty("ForeColor").Should().BeNull();
}
