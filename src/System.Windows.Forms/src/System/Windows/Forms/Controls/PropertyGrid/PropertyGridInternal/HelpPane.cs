// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.PropertyGridInternal;

/// <summary>
///  The help (description) pane optionally shown at the bottom of the <see cref="PropertyGrid"/>.
/// </summary>
/// <remarks>
///  <para>
///   <see cref="PropertyGrid.HelpVisible"/> controls the visibility of this control.
///  </para>
/// </remarks>
internal partial class HelpPane : PropertyGrid.SnappableControl
{
    private readonly Label _titleLabel;
    private readonly Label _descriptionLabel;
    private string? _fullDescription;

    private int _lineHeight;
    private bool _needUpdateUIWithFont = true;

    private const int LogicalDefaultWidth = 0;
    private const int LogicalDefaultHeight = 59;
    private const int MinimumLines = 2;

    private int _borderSize;

    private Rectangle _lastClientRectangle = Rectangle.Empty;

    internal HelpPane(PropertyGrid owner) : base(owner)
    {
        using SuspendLayoutScope scope = new(this, performLayout: false);

        _titleLabel = new()
        {
            UseMnemonic = false,
            Cursor = Cursors.Default
        };

        _descriptionLabel = new()
        {
            AutoEllipsis = true,
            Cursor = Cursors.Default
        };

        UpdateTextRenderingEngine();

        Controls.Add(_titleLabel);
        Controls.Add(_descriptionLabel);

        Size = new(LogicalToDeviceUnits(LogicalDefaultWidth), LogicalToDeviceUnits(LogicalDefaultHeight));

        Text = SR.PropertyGridHelpPaneTitle;
        SetStyle(ControlStyles.Selectable, false);
    }

    public virtual int Lines
    {
        get
        {
            UpdateUIWithFont();
            return Height / _lineHeight;
        }
        set
        {
            UpdateUIWithFont();
            Size = new(Width, 1 + value * _lineHeight);
        }
    }

    public override int GetOptimalHeight(int width)
    {
        UpdateUIWithFont();

        // Compute optimal label height as one line only.
        int height = _titleLabel.Size.Height;

        // Do this to avoid getting parented to the Parking window.
        if (OwnerPropertyGrid.IsHandleCreated && !IsHandleCreated)
        {
            CreateControl();
        }

        // Compute optimal text height.
        bool isScalingRequirementMet = ScaleHelper.IsScalingRequirementMet;
        using Graphics g = _descriptionLabel.CreateGraphicsInternal();
        SizeF sizef = PropertyGrid.MeasureTextHelper.MeasureText(OwnerPropertyGrid, g, _titleLabel.Text, Font, width);
        Size size = Size.Ceiling(sizef);
        int padding = isScalingRequirementMet ? LogicalToDeviceUnits(2) : 2;
        height += (size.Height * 2) + padding;
        return Math.Max(height + 2 * padding, isScalingRequirementMet ? LogicalToDeviceUnits(LogicalDefaultHeight) : LogicalDefaultHeight);
    }

    internal virtual void LayoutWindow()
    {
    }

    protected override void OnFontChanged(EventArgs e)
    {
        _needUpdateUIWithFont = true;
        PerformLayout();
        base.OnFontChanged(e);
    }

    protected override void OnLayout(LayoutEventArgs e)
    {
        UpdateUIWithFont();
        SetLabelBounds();
        _descriptionLabel.Text = _fullDescription;
        _descriptionLabel.AccessibleName = _fullDescription; // Don't crop the description for accessibility clients
        base.OnLayout(e);
    }

    protected override void OnResize(EventArgs e)
    {
        Rectangle currentClientRectangle = ClientRectangle;
        if (!_lastClientRectangle.IsEmpty && currentClientRectangle.Width > _lastClientRectangle.Width)
        {
            Invalidate(new Rectangle(
                _lastClientRectangle.Width - 1,
                0,
                currentClientRectangle.Width - _lastClientRectangle.Width + 1,
                _lastClientRectangle.Height));
        }

        if (ScaleHelper.IsScalingRequirementMet)
        {
            int oldLineHeight = _lineHeight;
            _lineHeight = Font.Height + LogicalToDeviceUnits(2);
            if (oldLineHeight != _lineHeight)
            {
                _titleLabel.Location = new(_borderSize, _borderSize);
                _descriptionLabel.Location = new(_borderSize, _borderSize + _lineHeight);

                SetLabelBounds();
            }
        }

        _lastClientRectangle = currentClientRectangle;
        base.OnResize(e);
    }

    private void SetLabelBounds()
    {
        Size size = ClientSize;

        // If the client size is 0, setting this to a negative number will force an extra layout.
        size.Width = Math.Max(0, size.Width - 2 * _borderSize);
        size.Height = Math.Max(0, size.Height - 2 * _borderSize);

        _titleLabel.SetBounds(
            _titleLabel.Top,
            _titleLabel.Left,
            size.Width,
            Math.Min(_lineHeight, size.Height),
            BoundsSpecified.Size);

        _descriptionLabel.SetBounds(
            _descriptionLabel.Top,
            _descriptionLabel.Left,
            size.Width,
            Math.Max(0, size.Height - _lineHeight - (ScaleHelper.IsScalingRequirementMet ? LogicalToDeviceUnits(1) : 1)),
            BoundsSpecified.Size);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        UpdateUIWithFont();
    }

    protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
    {
        base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
        ScaleConstants();
    }

    private protected override void InitializeConstantsForInitialDpi(int initialDpi) => ScaleConstants();

    private void ScaleConstants()
    {
        const int LogicalBorderSize = 3;
        _borderSize = LogicalToDeviceUnits(LogicalBorderSize);
    }

    public virtual void SetDescription(string? title, string? description)
    {
        if (_descriptionLabel.Text != title)
        {
            _titleLabel.Text = title;
        }

        if (description != _fullDescription)
        {
            _fullDescription = description;
            _descriptionLabel.Text = _fullDescription;

            // Don't crop the description for accessibility clients.
            _descriptionLabel.AccessibleName = _fullDescription;
        }
    }

    public override int SnapHeightRequest(int newHeight)
    {
        UpdateUIWithFont();
        int lines = Math.Max(MinimumLines, newHeight / _lineHeight);
        return 1 + lines * _lineHeight;
    }

    /// <inheritdoc />
    protected override AccessibleObject CreateAccessibilityInstance() => new HelpPaneAccessibleObject(this, OwnerPropertyGrid);

    /// <inheritdoc />
    internal override bool SupportsUiaProviders => true;

    internal void UpdateTextRenderingEngine()
    {
        _titleLabel.UseCompatibleTextRendering = OwnerPropertyGrid.UseCompatibleTextRendering;
        _descriptionLabel.UseCompatibleTextRendering = OwnerPropertyGrid.UseCompatibleTextRendering;
    }

    private void UpdateUIWithFont()
    {
        if (IsHandleCreated && _needUpdateUIWithFont)
        {
            // Some fonts throw because Bold is not a valid option for them. Fail gracefully.
            try
            {
                _titleLabel.Font = new(Font, FontStyle.Bold);
            }
            catch (Exception e) when (!e.IsCriticalException())
            {
            }

            _lineHeight = Font.Height + 2;
            _titleLabel.Location = new(_borderSize, _borderSize);
            _descriptionLabel.Location = new(_borderSize, _borderSize + _lineHeight);

            _needUpdateUIWithFont = false;
            PerformLayout();
        }
    }
}
