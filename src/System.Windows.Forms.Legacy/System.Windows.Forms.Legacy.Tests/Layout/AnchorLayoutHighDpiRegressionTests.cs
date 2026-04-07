// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Reflection;
using System.Windows.Forms.Layout;
using System.Windows.Forms.Primitives;


namespace System.Windows.Forms.Legacy.Tests;

/// <summary>
///  Issue-specific regression tests for WI00955507.
/// </summary>
/// <remarks>
///  <para>
///   These tests stay focused on the real CargoWise regression shape: a bottom/right-anchored
///   text box inside a group box inside a tab page, under high DPI, with transient parent
///   geometry during anchor capture.
///  </para>
///  <para>
///   See <c>anchor-layout-highDpi-regression.md</c> for the full analysis.
///  </para>
/// </remarks>
public class AnchorLayoutHighDpiRegressionTests
{
    private const string AnchorLayoutV2SwitchName = "System.Windows.Forms.AnchorLayoutV2";
    private static readonly int s_scaleDpi = GetScaleDpi();

    private readonly ITestOutputHelper _output;

    public AnchorLayoutHighDpiRegressionTests(ITestOutputHelper output)
    {
        _output = output;
    }

    /// <summary>
    ///  Sets the <c>AnchorLayoutV2</c> AppContext switch and resets the
    ///  <see cref="LocalAppContextSwitches"/> cached field so the new value is
    ///  actually observed — without relying on <c>TestSwitch.LocalAppContext.DisableCaching</c>
    ///  in a runtimeconfig file.
    /// </summary>
    private static void SetAnchorLayoutV2Switch(bool value)
    {
        // LocalAppContextSwitches caches switch values in a private static int field
        // (0 = uncached, 1 = true, -1 = false).  Reset it to 0 so the next access
        // re-reads the value from AppContext rather than short-circuiting from cache.
        typeof(LocalAppContextSwitches)
            .GetField("s_anchorLayoutV2", BindingFlags.NonPublic | BindingFlags.Static)!
            .SetValue(null, 0);

        AppContext.SetSwitch(AnchorLayoutV2SwitchName, value);
    }

    private static int ScaleLogicalPixels(int value, float dpi) => (int)Math.Round(value * dpi / 96f);

    private static int ScaleLogicalPixels(int value) => (int)Math.Round(value * s_scaleDpi / 96f);

    private static Point ScaledPoint(int x, int y) => new(ScaleLogicalPixels(x), ScaleLogicalPixels(y));

    private static Size ScaledSize(int width, int height) => new(ScaleLogicalPixels(width), ScaleLogicalPixels(height));

    private static int GetScaleDpi()
    {
        using Graphics graphics = Graphics.FromHwnd(IntPtr.Zero);

        return (int)Math.Round(graphics.DpiX);
    }

    /// <summary>
    ///  Integration-level regression: replicates the <c>OtherReferenceTextBox</c> scenario from
    ///  <c>DefaultLayoutTest.cs</c> (WI00955507) using standard WinForms controls.
    ///  A bottom/right-anchored <see cref="TextBox"/> inside a <see cref="GroupBox"/> inside a
    ///  <see cref="TabPage"/> must remain fully contained within the group box after the form
    ///  is shown and layout is complete.
    /// </summary>
    [StaFact]
    public void BottomRightAnchoredTextBoxInsideGroupBoxInsideTabPage_WithAnchorLayoutV2_RemainsWithinGroupBoxAfterFormIsShown()
    {
        SetAnchorLayoutV2Switch(true);
        try
        {
            // Sizes chosen so that the textbox's design-time Bottom does NOT exceed the
            // group box's final client height, which avoids a permanent deferral.
            // This mirrors the net48 geometry from the CargoWise investigation.
            using Form form = new() { ClientSize = new Size(400, 430) };
            using TabControl tabControl = new() { Dock = DockStyle.Fill };
            using TabPage tabPage = new();
            using GroupBox groupBox = new()
            {
                Location = new Point(7, 3),
                Size = new Size(324, 240),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
            };
            using TextBox otherRef = new()
            {
                // Anchor = Bottom | Left | Right — must float to the bottom of the group box.
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(6, 142),
                Size = new Size(310, 88),
                Multiline = true
            };

            groupBox.Controls.Add(otherRef);
            tabPage.Controls.Add(groupBox);
            tabControl.TabPages.Add(tabPage);
            tabControl.SelectedTab = tabPage;
            form.Controls.Add(tabControl);

            form.Show();
            Application.DoEvents();
            form.PerformLayout();
            Application.DoEvents();
            form.Close();

            Assert.True(
                groupBox.ClientRectangle.Contains(otherRef.Bounds),
                $"OtherRef should be fully inside GroupBox. "
                + $"GroupBox.ClientRectangle={groupBox.ClientRectangle}, OtherRef.Bounds={otherRef.Bounds}");

            Assert.True(otherRef.Visible, "OtherRef should be visible after form is shown.");
        }
        finally
        {
            SetAnchorLayoutV2Switch(false);
        }
    }

    /// <summary>
    ///  Mirrors the authoritative CargoWise initialization order under the V1 path using
    ///  standard WinForms controls plus a GroupBox whose <see cref="Control.DisplayRectangle"/>
    ///  matches the hidden-caption semantics of the production host.
    /// </summary>
    [StaFact]
    public void BottomRightAnchoredTextBoxInsideGroupBoxInsideTabPage_V1Path_CargoWiseGeometry_ShouldRemainWithinGroupBoxAfterFormIsShown()
    {
        bool setHighDpiModeResult = Application.SetHighDpiMode(HighDpiMode.SystemAware);
        _output.WriteLine($"{nameof(AnchorLayoutHighDpiRegressionTests)}: SetHighDpiMode(SystemAware)={setHighDpiModeResult}, HighDpiMode={Application.HighDpiMode}");

        using Form form = new() { ClientSize = ScaledSize(500, 560) };
        using TabControl tabControl = new()
        {
            Dock = DockStyle.Fill,
            MinimumSize = ScaledSize(480, 470),
            Size = ScaledSize(480, 480)
        };
        using TabPage tabPage = new()
        {
            Location = ScaledPoint(4, 23),
            Padding = new Padding(ScaleLogicalPixels(3)),
            Size = ScaledSize(480, 323)
        };
        using CargoWiseLikeGroupBox groupBox = new();
        using TextBox lastPortDate = new()
        {
            Location = ScaledPoint(133, 71),
            Size = ScaledSize(183, 20),
            TabIndex = 2
        };
        using TextBox conditionsOfSale = new()
        {
            Location = ScaledPoint(133, 97),
            Size = ScaledSize(183, 20),
            TabIndex = 3
        };
        using TextBox termsOfPayment = new()
        {
            Location = ScaledPoint(133, 123),
            Size = ScaledSize(183, 20),
            TabIndex = 4
        };
        using CheckBox servicesIndicator = new()
        {
            Text = "Goods/Services Indicator",
            Location = ScaledPoint(133, 149),
            AutoSize = true
        };
        using CheckBox royaltyIndicator = new()
        {
            Text = "Royalty Payment Indicator",
            Location = ScaledPoint(133, 169),
            AutoSize = true
        };
        using TextBox otherRef = new()
        {
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            Location = ScaledPoint(6, 142),
            Size = ScaledSize(310, 88),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            TabIndex = 7
        };

        groupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        groupBox.Location = ScaledPoint(7, 3);
        groupBox.Size = ScaledSize(324, 240);
        groupBox.TabStop = false;

        tabPage.Controls.Add(groupBox);
        tabControl.TabPages.Add(tabPage);

        groupBox.Controls.Add(lastPortDate);
        groupBox.Controls.Add(conditionsOfSale);
        groupBox.Controls.Add(termsOfPayment);
        groupBox.Controls.Add(servicesIndicator);
        groupBox.Controls.Add(royaltyIndicator);
        groupBox.Controls.Add(otherRef);

        tabControl.SelectedTab = tabPage;
        form.Controls.Add(tabControl);

        form.Show();
        Application.DoEvents();
        form.PerformLayout();
        Application.DoEvents();

        using Graphics graphics = form.CreateGraphics();
        _output.WriteLine($"{nameof(AnchorLayoutHighDpiRegressionTests)}: GraphicsDpiX={graphics.DpiX}, GraphicsDpiY={graphics.DpiY}");

        int expectedY = groupBox.ClientRectangle.Height - ScaleLogicalPixels(10, graphics.DpiY) - otherRef.Bounds.Height;
        _output.WriteLine($"Post-show: GroupBox.ClientRectangle={groupBox.ClientRectangle}, GroupBox.DisplayRectangle={groupBox.DisplayRectangle}, OtherRef.Bounds={otherRef.Bounds}, ExpectedY={expectedY}, ServicesIndicator={servicesIndicator.Bounds}, RoyaltyIndicator={royaltyIndicator.Bounds}");

        Assert.True(
            groupBox.ClientRectangle.Contains(otherRef.Bounds),
            $"CargoWise geometry: OtherRef should be fully inside GroupBox. "
            + $"GroupBox.ClientRectangle={groupBox.ClientRectangle}, OtherRef.Bounds={otherRef.Bounds}");

        Assert.True(
            groupBox.DisplayRectangle.Contains(otherRef.Bounds),
            $"CargoWise geometry: OtherRef should be fully inside GroupBox display area. "
            + $"GroupBox.DisplayRectangle={groupBox.DisplayRectangle}, OtherRef.Bounds={otherRef.Bounds}");

        Assert.Equal(expectedY, otherRef.Location.Y);

        Assert.False(
            otherRef.Bounds.IntersectsWith(servicesIndicator.Bounds),
            $"CargoWise geometry: OtherRef should not overlap ServicesIndicator. "
            + $"ServicesIndicator={servicesIndicator.Bounds}, OtherRef={otherRef.Bounds}");

        Assert.False(
            otherRef.Bounds.IntersectsWith(royaltyIndicator.Bounds),
            $"CargoWise geometry: OtherRef should not overlap RoyaltyIndicator. "
            + $"RoyaltyIndicator={royaltyIndicator.Bounds}, OtherRef={otherRef.Bounds}");

        Assert.True(
            otherRef.Bounds.Top >= royaltyIndicator.Bounds.Bottom,
            $"CargoWise geometry: OtherRef should be below RoyaltyIndicator. "
            + $"RoyaltyIndicator={royaltyIndicator.Bounds}, OtherRef={otherRef.Bounds}");

        Assert.Equal(ScaleLogicalPixels(6, graphics.DpiX), otherRef.Location.X);

        form.Close();
    }

    [StaFact]
    public void ShouldRefreshAnchorInfoForStalePositiveAnchors_BottomAnchoredTrailingAxis_ReturnsTrue()
    {
        DefaultLayout.AnchorInfo anchorInfo = CreateAnchorInfo(
            left: 10,
            top: 70,
            right: -190,
            bottom: 170,
            displayRectangle: new Rectangle(0, 0, 300, 100));
        Rectangle bounds = new(10, 120, 100, 50);
        Rectangle currentDisplayRectangle = new(0, 0, 300, 200);

        bool shouldRefresh = ShouldRefreshAnchorInfoForStalePositiveAnchors(
            anchorInfo,
            bounds,
            currentDisplayRectangle,
            AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);

        Assert.True(
            shouldRefresh,
            $"A trailing-only bottom anchor with a positive cached bottom offset should be refreshed. "
            + $"Bounds={bounds}, DisplayRectangle={currentDisplayRectangle}, CachedBottom={anchorInfo.Bottom}");
    }

    [StaFact]
    public void ShouldRefreshAnchorInfoForStalePositiveAnchors_StretchAnchoredVerticalAxis_ReturnsFalse()
    {
        DefaultLayout.AnchorInfo anchorInfo = CreateAnchorInfo(
            left: 8,
            top: 0,
            right: 656,
            bottom: 6,
            displayRectangle: new Rectangle(6, 6, 788, 474));
        Rectangle bounds = new(14, 6, 648, 766);
        Rectangle currentDisplayRectangle = new(6, 6, 772, 760);

        bool shouldRefresh = ShouldRefreshAnchorInfoForStalePositiveAnchors(
            anchorInfo,
            bounds,
            currentDisplayRectangle,
            AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left);

        Assert.False(
            shouldRefresh,
            $"A stretch-anchored vertical axis should not be treated as a stale positive bottom anchor. "
            + $"Bounds={bounds}, DisplayRectangle={currentDisplayRectangle}, CachedBottom={anchorInfo.Bottom}");
    }

    [StaFact]
    public void ShouldRefreshAnchorInfoForStalePositiveAnchors_RightAnchoredTrailingAxis_ReturnsTrue()
    {
        // Symmetric horizontal-axis case: Right without Left, stale positive right offset.
        // Captured right = 140 against a parent width of 100 (transient), parent has since grown to 300.
        DefaultLayout.AnchorInfo anchorInfo = CreateAnchorInfo(
            left: 190,
            top: 10,
            right: 140,
            bottom: -40,
            displayRectangle: new Rectangle(0, 0, 100, 200));
        Rectangle bounds = new(190, 10, 90, 40);
        Rectangle currentDisplayRectangle = new(0, 0, 300, 200);

        bool shouldRefresh = ShouldRefreshAnchorInfoForStalePositiveAnchors(
            anchorInfo,
            bounds,
            currentDisplayRectangle,
            AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom);

        Assert.True(
            shouldRefresh,
            $"A trailing-only right anchor with a positive cached right offset should be refreshed. "
            + $"Bounds={bounds}, DisplayRectangle={currentDisplayRectangle}, CachedRight={anchorInfo.Right}");
    }

    /// <summary>
    ///  Fast local regression for the current CargoWise-only issue: a stretch-anchored parent is
    ///  recovered once from its original captured display rectangle, then is incorrectly eligible
    ///  for a second stale-positive refresh that overwrites the recovered anchors.
    /// </summary>
    [StaFact]
    public void StretchAnchoredGroupBox_RecoveredStretchAnchor_ShouldNotTriggerFollowUpPositiveRefresh()
    {
        using StretchTestContainer parent = new()
        {
            Bounds = new Rectangle(8, 40, 784, 772),
            SimulatedDisplayRectangle = new Rectangle(6, 6, 772, 760)
        };
        using CargoWiseLikeGroupBox groupBox = new()
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left,
            Bounds = new Rectangle(14, 6, 648, 100)
        };

        parent.Controls.Add(groupBox);

        SetSpecifiedBounds(parent, new Rectangle(8, 40, 784, 772));
        SetSpecifiedBounds(groupBox, new Rectangle(14, 6, 648, 480));

        DefaultLayout.AnchorInfo anchorInfo = CreateAnchorInfo(8, 0, 656, -374, new Rectangle(6, 6, 788, 474));
        Rectangle currentDisplayRectangle = parent.DisplayRectangle;

        RefreshAnchorInfoForDisplayRectangleGrowth(groupBox, anchorInfo, currentDisplayRectangle, groupBox.Anchor, isStretchAnchorRefresh: true);

        Rectangle recoveredBounds = ComputeAnchoredBounds(anchorInfo, currentDisplayRectangle, groupBox.Anchor);
        groupBox.Bounds = recoveredBounds;
        SetSpecifiedBounds(groupBox, new Rectangle(14, 6, 648, 480));

        bool shouldRefreshAgain = ShouldRefreshAnchorInfoForStalePositiveAnchors(anchorInfo, recoveredBounds, currentDisplayRectangle, groupBox.Anchor);

        _output.WriteLine($"Recovered stretch anchor bounds={recoveredBounds}, shouldRefreshAgain={shouldRefreshAgain}, recoveredBottom={anchorInfo.Bottom}, recoveredDisplayRect={anchorInfo.DisplayRectangle}");

        Assert.False(
            shouldRefreshAgain,
            $"Recovered stretch anchor should not trigger a follow-up stale-positive refresh. "
            + $"RecoveredBounds={recoveredBounds}, RecoveredBottom={anchorInfo.Bottom}, RecoveredDisplayRect={anchorInfo.DisplayRectangle}");
    }

    private static DefaultLayout.AnchorInfo CreateAnchorInfo(int left, int top, int right, int bottom, Rectangle displayRectangle) =>
        new()
        {
            Left = left,
            Top = top,
            Right = right,
            Bottom = bottom,
            DisplayRectangle = displayRectangle
        };

    private static void SetSpecifiedBounds(Control control, Rectangle bounds) =>
        CommonProperties.UpdateSpecifiedBounds(control, bounds.X, bounds.Y, bounds.Width, bounds.Height);

    private static bool ShouldRefreshAnchorInfoForStalePositiveAnchors(DefaultLayout.AnchorInfo anchorInfo, Rectangle bounds, Rectangle displayRectangle, AnchorStyles anchor) =>
        DefaultLayout.ShouldRefreshAnchorInfoForStalePositiveAnchors(anchorInfo, bounds, displayRectangle, anchor);

    private static void RefreshAnchorInfoForDisplayRectangleGrowth(Control control, DefaultLayout.AnchorInfo anchorInfo, Rectangle displayRectangle, AnchorStyles anchor, bool isStretchAnchorRefresh) =>
        DefaultLayout.RefreshAnchorInfoForDisplayRectangleGrowth(control, anchorInfo, displayRectangle, anchor, isStretchAnchorRefresh);

    private static Rectangle ComputeAnchoredBounds(DefaultLayout.AnchorInfo anchorInfo, Rectangle displayRectangle, AnchorStyles anchor)
    {
        int left = anchorInfo.Left + displayRectangle.X;
        int top = anchorInfo.Top + displayRectangle.Y;
        int right = anchorInfo.Right + displayRectangle.X;
        int bottom = anchorInfo.Bottom + displayRectangle.Y;

        if ((anchor & AnchorStyles.Right) != AnchorStyles.None)
        {
            right += displayRectangle.Width;

            if ((anchor & AnchorStyles.Left) == AnchorStyles.None)
            {
                left += displayRectangle.Width;
            }
        }
        else if ((anchor & AnchorStyles.Left) == AnchorStyles.None)
        {
            int center = displayRectangle.Width / 2;
            right += center;
            left += center;
        }

        if ((anchor & AnchorStyles.Bottom) != AnchorStyles.None)
        {
            bottom += displayRectangle.Height;

            if ((anchor & AnchorStyles.Top) == AnchorStyles.None)
            {
                top += displayRectangle.Height;
            }
        }
        else if ((anchor & AnchorStyles.Top) == AnchorStyles.None)
        {
            int center = displayRectangle.Height / 2;
            bottom += center;
            top += center;
        }

        if (right < left)
        {
            right = left;
        }

        if (bottom < top)
        {
            bottom = top;
        }

        return new Rectangle(left, top, right - left, bottom - top);
    }

    private sealed class CargoWiseLikeGroupBox : GroupBox
    {
        public override Rectangle DisplayRectangle => ClientRectangle;
    }

    private sealed class StretchTestContainer : Panel
    {
        public Rectangle SimulatedDisplayRectangle { get; set; }

        public override Rectangle DisplayRectangle => SimulatedDisplayRectangle;
    }
}
