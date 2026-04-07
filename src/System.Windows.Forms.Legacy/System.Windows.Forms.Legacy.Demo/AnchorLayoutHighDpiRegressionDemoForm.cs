using System.Drawing;
#if NET
using System.Runtime.InteropServices;
#endif
using System.Windows.Forms;

namespace Demo;

/// <summary>
///  Demo form that reproduces the anchor-layout high-DPI regression (WI00955507).
///  See anchor-layout-highDpi-regression.md for the full analysis.
/// </summary>
/// <remarks>
///  <para>
///   The regression manifests when <c>ScaleHelper.IsScalingRequirementMet</c> returns
///   <see langword="true"/> — i.e. when at least one monitor is at a DPI scale above 100%.
///   The demo application must run with <c>HighDpiMode.SystemAware</c> (see <c>Program.cs</c>).
///  </para>
///  <para>
///   Symptom: <c>OtherReferenceTextBox</c> is displaced below — or entirely outside — the
///   <c>MiscGroupBox</c> client area after the form is shown. The title bar reports the
///   layout outcome once the form becomes visible.
///  </para>
///  <para>
///   Fix: add <c>"System.Windows.Forms.AnchorLayoutV2": true</c> to the app's
///   <c>runtimeconfig.json</c> to enable the V2 deferred-anchor path.
///  </para>
///  <para>
///   <b>Observed platform behavior (counter-intuitive):</b> this demo <b>fails on
///   .NET Framework 4.8</b> even at 100% DPI (96 dpi), while it <b>passes on the WTG
///   .NET 10 fork</b>.  The forced transient <c>MiscGroupBox</c> height of 50 px means
///   the captured <c>Bottom</c> anchor offset is always a large positive value
///   (e.g. +230 px at 96 dpi), which displaces <c>OtherReferenceTextBox</c> far below
///   the group box regardless of DPI scale.  .NET Framework has no mechanism to correct
///   a stale positive anchor.  The WTG fork's Solution 3 repair in
///   <c>DefaultLayout.AnchorLayoutCompat.cs</c> detects the positive offset via
///   <c>ShouldRefreshAnchorInfoForStalePositiveAnchors</c> and recomputes it against the
///   stable parent <c>DisplayRectangle</c> on the next layout pass, which is why the
///   demo passes on .NET 10 even before any DPI scaling is involved.
///  </para>
/// </remarks>
public class AnchorLayoutHighDpiRegressionDemoForm : Form
{
#if NET
    private static readonly string s_frameworkDescription = RuntimeInformation.FrameworkDescription;
#endif
    private static readonly int s_scaleDpi = GetScaleDpi();
    private static readonly Size s_miscGroupBoxSize = ScaledSize(324, 240);
    private static readonly int s_expectedBottomMargin = Scale(10);

    private readonly GroupBox _miscGroupBox;
    private readonly TextBox _otherReferenceTextBox;
    private readonly CheckBox _goodsServicesIndicatorCheckBox;
    private readonly CheckBox _royaltyPaymentIndicatorCheckBox;
    private readonly TextBox _diagnosticsTextBox;

    // Captured in OnLoad (before the form becomes visible) to mirror the
    // "before-show" diagnostics logged by DefaultLayoutTest.
    private Rectangle _miscGroupBoxClientAtAnchorCapture;
    private Rectangle _otherRefBoundsBeforeShow;
    private Rectangle _miscGroupBoxClientBeforeShow;

    public AnchorLayoutHighDpiRegressionDemoForm()
    {
        Font = new Font(new FontFamily("Microsoft Sans Serif"), 8.25f);

        _diagnosticsTextBox = new TextBox
        {
            Name = "DiagnosticsTextBox",
            Dock = DockStyle.Bottom,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Height = Scale(96),
            TabStop = false
        };

        // Mirror the hierarchy and initialization order from the CargoWise repro:
        // TabControl → TabPage → GroupBox, with the GroupBox already parented into the
        // tab hierarchy before its bottom-anchored child is added.
        TabControl invoiceTabControl = new()
        {
            Dock = DockStyle.Fill,
            Name = "InvoiceTabControl",
            MinimumSize = ScaledSize(480, 470),
            Size = ScaledSize(480, 480)
        };

        TabPage otherInfoTabPage = new()
        {
            Text = "Other Info",
            Name = "OtherInfoTabPage",
            Location = ScaledPoint(4, 23),
            Padding = new Padding(Scale(3)),
            Size = ScaledSize(480, 323)
        };

        // The CargoWise host uses a custom group box with hidden-caption semantics.
        // Using ClientRectangle as DisplayRectangle keeps the reduced repro aligned
        // with the authoritative layout math.
        _miscGroupBox = new CargoWiseLikeGroupBox
        {
            Name = "MiscGroupBox",
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left,
            Location = ScaledPoint(7, 3),
            Size = s_miscGroupBoxSize,
            TabStop = false
        };

        otherInfoTabPage.Controls.Add(_miscGroupBox);
        invoiceTabControl.Controls.Add(otherInfoTabPage);

        Label lastPortDateLabel = new()
        {
            Text = "Date of Direct Shipment:",
            Location = ScaledPoint(16, 75),
            AutoSize = true
        };

        TextBox lastPortDateTextBox = new()
        {
            Name = "LastPortDateTextBox",
            Location = ScaledPoint(133, 71),
            Size = ScaledSize(183, 20),
            TabIndex = 2
        };

        Label conditionsLabel = new()
        {
            Text = "Conditions:",
            Location = ScaledPoint(16, 101),
            AutoSize = true
        };

        TextBox conditionsOfSaleTextBox = new()
        {
            Name = "ConditionsOfSaleTextBox",
            Location = ScaledPoint(133, 97),
            Size = ScaledSize(183, 20),
            TabIndex = 3
        };

        Label termsLabel = new()
        {
            Text = "Terms:",
            Location = ScaledPoint(16, 127),
            AutoSize = true
        };

        TextBox termsOfPaymentTextBox = new()
        {
            Name = "TermsOfPaymentTextBox",
            Location = ScaledPoint(133, 123),
            Size = ScaledSize(183, 20),
            TabIndex = 4
        };

        _goodsServicesIndicatorCheckBox = new CheckBox
        {
            Name = "ServicesIndCheckBox",
            Text = "Goods/Services Indicator",
            Location = ScaledPoint(133, 149),
            AutoSize = true,
            TabIndex = 5,
            UseVisualStyleBackColor = true
        };

        _royaltyPaymentIndicatorCheckBox = new CheckBox
        {
            Name = "RoyaltyIndCheckBox",
            Text = "Royalty Payment Indicator",
            Location = ScaledPoint(133, 169),
            AutoSize = true,
            TabIndex = 6,
            UseVisualStyleBackColor = true
        };

        _otherReferenceTextBox = new TextBox
        {
            Name = "OtherReferenceTextBox",
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            Location = ScaledPoint(6, 142),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Size = ScaledSize(310, 88),
            TabIndex = 7
        };

        _miscGroupBox.Controls.Add(lastPortDateLabel);
        _miscGroupBox.Controls.Add(lastPortDateTextBox);
        _miscGroupBox.Controls.Add(conditionsLabel);
        _miscGroupBox.Controls.Add(conditionsOfSaleTextBox);
        _miscGroupBox.Controls.Add(termsLabel);
        _miscGroupBox.Controls.Add(termsOfPaymentTextBox);
        _miscGroupBox.Controls.Add(_goodsServicesIndicatorCheckBox);
        _miscGroupBox.Controls.Add(_royaltyPaymentIndicatorCheckBox);

        _miscGroupBoxClientAtAnchorCapture = _miscGroupBox.ClientRectangle;
        _miscGroupBox.Controls.Add(_otherReferenceTextBox);

        Controls.Add(invoiceTabControl);
        invoiceTabControl.SelectedTab = otherInfoTabPage;

        Controls.Add(_diagnosticsTextBox);

        ClientSize = ScaledSize(500, 560);
        Name = nameof(AnchorLayoutHighDpiRegressionDemoForm);
        Text = "Anchor Layout High-DPI Regression Demo";
        StartPosition = FormStartPosition.CenterScreen;
    }

    /// <inheritdoc/>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Capture the pre-show layout state.  This mirrors the "before-show" logging in
        // DefaultLayoutTest and is the first place where the anchor-offset bug is visible:
        // if OtherReferenceTextBox.Bounds is already outside MiscGroupBox.ClientRectangle
        // here, the anchor info was captured with a wrong transient parent height.
        _otherRefBoundsBeforeShow = _otherReferenceTextBox.Bounds;
        _miscGroupBoxClientBeforeShow = _miscGroupBox.ClientRectangle;
    }

    /// <inheritdoc/>
    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        LogLayoutOutcome();
    }

    private void LogLayoutOutcome()
    {
        Rectangle groupBoxClientAfterShow = _miscGroupBox.ClientRectangle;
        Rectangle otherRefBoundsAfterShow = _otherReferenceTextBox.Bounds;

        bool withinGroupBox = groupBoxClientAfterShow.Contains(otherRefBoundsAfterShow);
        bool overlapsServices = otherRefBoundsAfterShow.IntersectsWith(_goodsServicesIndicatorCheckBox.Bounds);
        bool overlapsRoyalty = otherRefBoundsAfterShow.IntersectsWith(_royaltyPaymentIndicatorCheckBox.Bounds);
        bool belowIndicators = otherRefBoundsAfterShow.Top >= _royaltyPaymentIndicatorCheckBox.Bounds.Bottom;
        int expectedY = groupBoxClientAfterShow.Height - s_expectedBottomMargin - otherRefBoundsAfterShow.Height;
        bool alignedToExpectedBottomMargin = otherRefBoundsAfterShow.Y == expectedY;

        bool pass = withinGroupBox
            && !overlapsServices
            && !overlapsRoyalty
            && belowIndicators
            && alignedToExpectedBottomMargin;

        using Graphics g = CreateGraphics();
        int dpi = (int)g.DpiX;

        string dpiNote = dpi == 96 ? " (DPI=100%: set a monitor >100% to see FAIL)" : string.Empty;

#if NET
        string runtimeInfo = $"TFM={s_frameworkDescription} | HighDpiMode={Application.HighDpiMode} | ScaleDpi={s_scaleDpi} | DeviceDpi={DeviceDpi} | GraphicsDpi={dpi}";
#else
        string runtimeInfo = $"TFM=.NET Framework | ScaleDpi={s_scaleDpi} | GraphicsDpi={dpi}";
#endif

        Text = $"{(pass ? "PASS" : "FAIL")}{dpiNote} — DPI={dpi}";
        _diagnosticsTextBox.Text =
            $"ANCHOR-CAPTURE: GroupBox={_miscGroupBoxClientAtAnchorCapture}{Environment.NewLine}" +
            $"BEFORE-SHOW: GroupBox={_miscGroupBoxClientBeforeShow}  OtherRef={_otherRefBoundsBeforeShow}{Environment.NewLine}" +
            $"AFTER-SHOW:  GroupBox={groupBoxClientAfterShow}  OtherRef={otherRefBoundsAfterShow}{Environment.NewLine}" +
            $"EXPECT:      BottomMargin={s_expectedBottomMargin}  ExpectedY={expectedY}  BelowIndicators={belowIndicators}{Environment.NewLine}" +
            runtimeInfo;
    }

    private sealed class CargoWiseLikeGroupBox : GroupBox
    {
        public override Rectangle DisplayRectangle => ClientRectangle;
    }

    // Scale a logical (96-dpi) pixel value to device pixels, matching the behaviour of
    // ControlDpiScalingHelper.NewScaledPoint/NewScaledSize in the CargoWise codebase.
    private static int GetScaleDpi()
    {
#if NET
        try
        {
            return (int)GetDpiForSystem();
        }
        catch (EntryPointNotFoundException)
        {
        }
#endif

        using Graphics g = Graphics.FromHwnd(IntPtr.Zero);

        return (int)Math.Round(g.DpiX);
    }

    private static int Scale(int value) => (int)Math.Round(value * s_scaleDpi / 96.0);

#if NET
    [DllImport("user32.dll")]
    private static extern uint GetDpiForSystem();
#endif

    private static Point ScaledPoint(int x, int y) => new(Scale(x), Scale(y));

    private static Size ScaledSize(int w, int h) => new(Scale(w), Scale(h));
}
