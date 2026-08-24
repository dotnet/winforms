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
        bool darkMode = false,
        bool highContrast = false)
        => new(
            BackColor: Color.White,
            Enabled: enabled,
            ReadOnly: readOnly,
            Focused: focused,
            Hovered: hovered,
            DarkMode: darkMode,
            HighContrast: highContrast,
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
    public void Hover_tints_the_surface_relative_to_rest()
    {
        ModernFieldStroke hover = ModernFieldStrokeResolver.GetStroke(Context(hovered: true));

        hover.SurfaceColor.Should().Be(ModernControlColorMath.GetFieldHoverSurface(Color.White, darkMode: false));
        hover.SurfaceColor.Should().NotBe(Color.White);
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

    // ---- High Contrast bypass ----

    [Fact]
    public void HighContrast_uses_system_frame_and_window()
    {
        ModernFieldStroke rest = ModernFieldStrokeResolver.GetStroke(Context(highContrast: true));

        rest.SideTopColor.Should().Be(SystemColors.WindowFrame);
        rest.SurfaceColor.Should().Be(SystemColors.Window);
    }

    [Fact]
    public void HighContrast_focus_bottom_is_highlight()
        => ModernFieldStrokeResolver.GetStroke(Context(focused: true, highContrast: true))
            .BottomColor.Should().Be(SystemColors.Highlight);

    [Fact]
    public void HighContrast_disabled_uses_gray_text()
    {
        ModernFieldStroke disabled = ModernFieldStrokeResolver.GetStroke(Context(enabled: false, highContrast: true));

        disabled.SideTopColor.Should().Be(SystemColors.GrayText);
        disabled.BottomColor.Should().Be(SystemColors.GrayText);
    }
}
