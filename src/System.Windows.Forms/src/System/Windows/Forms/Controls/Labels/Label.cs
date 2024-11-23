// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms.Automation;
using System.Windows.Forms.Internal;
using System.Windows.Forms.Layout;
using Windows.Win32.System.SystemServices;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

/// <summary>
///  Represents a standard Windows label.
/// </summary>
[DefaultProperty(nameof(Text))]
[DefaultBindingProperty(nameof(Text))]
[Designer($"System.Windows.Forms.Design.LabelDesigner, {AssemblyRef.SystemDesign}")]
[ToolboxItem($"System.Windows.Forms.Design.AutoSizeToolboxItem,{AssemblyRef.SystemDesign}")]
[SRDescription(nameof(SR.DescriptionLabel))]
// If not for FormatControl, we could inherit from ButtonBase and get foreground images for free.
public partial class Label : Control, IAutomationLiveRegion
{
    private static readonly object s_eventTextAlignChanged = new();

    private static readonly BitVector32.Section s_stateUseMnemonic = BitVector32.CreateSection(1);
    private static readonly BitVector32.Section s_stateAutoSize = BitVector32.CreateSection(1, s_stateUseMnemonic);
    private static readonly BitVector32.Section s_stateAnimating = BitVector32.CreateSection(1, s_stateAutoSize);
    private static readonly BitVector32.Section s_stateFlatStyle = BitVector32.CreateSection((int)FlatStyle.System, s_stateAnimating);
    private static readonly BitVector32.Section s_stateBorderStyle = BitVector32.CreateSection((int)BorderStyle.Fixed3D, s_stateFlatStyle);
    private static readonly BitVector32.Section s_stateAutoEllipsis = BitVector32.CreateSection(1, s_stateBorderStyle);

    private static readonly int s_propImageList = PropertyStore.CreateKey();
    private static readonly int s_propImage = PropertyStore.CreateKey();

    private static readonly int s_propTextAlign = PropertyStore.CreateKey();
    private static readonly int s_propImageAlign = PropertyStore.CreateKey();
    private static readonly int s_propImageIndex = PropertyStore.CreateKey();

    private BitVector32 _labelState;
    private int _requestedHeight;
    private int _requestedWidth;
    private LayoutUtils.MeasureTextCache? _textMeasurementCache;

    // Tooltip is shown only if the Text in the Label is cut.
    internal bool _showToolTip;
    private ToolTip? _textToolTip;

    // This bool suggests that the User has added a toolTip to this label. In such a case we should not show the
    // AutoEllipsis tooltip.
    private bool _controlToolTip;

    private AutomationLiveSetting _liveSetting;

    /// <summary>
    ///  Initializes a new instance of the <see cref="Label"/> class.
    /// </summary>
    public Label() : base()
    {
        // This class overrides GetPreferredSizeCore, let Control automatically cache the result.
        SetExtendedState(ExtendedStates.UserPreferredSizeCache, true);

        SetStyle(ControlStyles.UserPaint |
                 ControlStyles.SupportsTransparentBackColor |
                 ControlStyles.OptimizedDoubleBuffer, IsOwnerDraw());

        SetStyle(ControlStyles.FixedHeight |
                 ControlStyles.Selectable, false);

        SetStyle(ControlStyles.ResizeRedraw, true);

#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        SetStyle(ControlStyles.ApplyThemingImplicitly, true);
#pragma warning restore WFO5001

        CommonProperties.SetSelfAutoSizeInDefaultLayout(this, true);

        _labelState[s_stateFlatStyle] = (int)FlatStyle.Standard;
        _labelState[s_stateUseMnemonic] = 1;
        _labelState[s_stateBorderStyle] = (int)BorderStyle.None;

        TabStop = false;
        _requestedHeight = Height;
        _requestedWidth = Width;
    }

    /// <summary>
    ///  Indicates whether the control is automatically resized to fit its contents.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [DefaultValue(false)]
    [RefreshProperties(RefreshProperties.All)]
    [Localizable(true)]
    [SRDescription(nameof(SR.LabelAutoSizeDescr))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public override bool AutoSize
    {
        get => base.AutoSize;
        set
        {
            if (AutoSize != value)
            {
                base.AutoSize = value;
                AdjustSize();
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnAutoSizeChangedDescr))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public new event EventHandler? AutoSizeChanged
    {
        add => base.AutoSizeChanged += value;
        remove => base.AutoSizeChanged -= value;
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the ellipsis character (...) appears at the right edge of the Label,
    ///  denoting that the Label text extends beyond the specified length of the Label.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [SRDescription(nameof(SR.LabelAutoEllipsisDescr))]
    public bool AutoEllipsis
    {
        get => _labelState[s_stateAutoEllipsis] != 0;
        set
        {
            if (AutoEllipsis == value)
            {
                return;
            }

            _labelState[s_stateAutoEllipsis] = value ? 1 : 0;
            MeasureTextCache.InvalidateCache();

            OnAutoEllipsisChanged();

            if (value)
            {
                _textToolTip ??= new ToolTip();
            }

            if (ParentInternal is not null)
            {
                LayoutTransaction.DoLayoutIf(AutoSize, ParentInternal, this, PropertyNames.AutoEllipsis);
            }

            Invalidate();
        }
    }

    /// <summary>
    ///  Gets or sets the image rendered on the background of the control.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.LabelBackgroundImageDescr))]
    public override Image? BackgroundImage
    {
        get => base.BackgroundImage;
        set => base.BackgroundImage = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackgroundImageChanged
    {
        add => base.BackgroundImageChanged += value;
        remove => base.BackgroundImageChanged -= value;
    }

    /// <summary>
    ///  Gets or sets the image layout for the background of the control.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override ImageLayout BackgroundImageLayout
    {
        get => base.BackgroundImageLayout;
        set => base.BackgroundImageLayout = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackgroundImageLayoutChanged
    {
        add => base.BackgroundImageLayoutChanged += value;
        remove => base.BackgroundImageLayoutChanged -= value;
    }

    /// <summary>
    ///  Gets or sets the border style for the control.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(BorderStyle.None)]
    [DispId(PInvokeCore.DISPID_BORDERSTYLE)]
    [SRDescription(nameof(SR.LabelBorderDescr))]
    public virtual BorderStyle BorderStyle
    {
        get => (BorderStyle)_labelState[s_stateBorderStyle];
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (BorderStyle != value)
            {
                _labelState[s_stateBorderStyle] = (int)value;
                if (ParentInternal is not null)
                {
                    LayoutTransaction.DoLayoutIf(AutoSize, ParentInternal, this, PropertyNames.BorderStyle);
                }

                if (AutoSize)
                {
                    AdjustSize();
                }

                RecreateHandle();
            }
        }
    }

    /// <summary>
    ///  Determines whether the current state of the control allows for rendering text using TextRenderer (GDI).
    ///  See LinkLabel implementation for details.
    /// </summary>
    internal virtual bool CanUseTextRenderer => true;

    /// <summary>
    ///  Overrides Control. A Label is a Win32 STATIC control, which we setup here.
    /// </summary>
    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ClassName = PInvoke.WC_STATIC;

            if (OwnerDraw)
            {
                // An unfortunate side effect of this style is Windows sends us WM_DRAWITEM
                // messages instead of WM_PAINT, but since Windows insists on repainting
                // *without* a WM_PAINT after SetWindowText, I don't see much choice.
                cp.Style |= (int)STATIC_STYLES.SS_OWNERDRAW;

                // Since we're owner draw, I don't see any point in setting the
                // SS_CENTER/SS_RIGHT styles.
                cp.ExStyle &= ~(int)WINDOW_EX_STYLE.WS_EX_RIGHT;   // WS_EX_RIGHT overrides the SS_XXXX alignment styles
            }

            if (!OwnerDraw)
            {
                switch (TextAlign)
                {
                    case ContentAlignment.TopLeft:
                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.BottomLeft:
                        cp.Style |= (int)STATIC_STYLES.SS_LEFT;
                        break;
                    case ContentAlignment.TopRight:
                    case ContentAlignment.MiddleRight:
                    case ContentAlignment.BottomRight:
                        cp.Style |= (int)STATIC_STYLES.SS_RIGHT;
                        break;
                    case ContentAlignment.TopCenter:
                    case ContentAlignment.MiddleCenter:
                    case ContentAlignment.BottomCenter:
                        cp.Style |= (int)STATIC_STYLES.SS_CENTER;
                        break;
                }
            }
            else
            {
                cp.Style |= (int)STATIC_STYLES.SS_LEFT;
            }

            switch (BorderStyle)
            {
                case BorderStyle.FixedSingle:
                    cp.Style |= (int)WINDOW_STYLE.WS_BORDER;
                    break;
                case BorderStyle.Fixed3D:
                    cp.Style |= (int)STATIC_STYLES.SS_SUNKEN;
                    break;
            }

            if (!UseMnemonic)
            {
                cp.Style |= (int)STATIC_STYLES.SS_NOPREFIX;
            }

            return cp;
        }
    }

    protected override ImeMode DefaultImeMode => ImeMode.Disable;

    protected override Padding DefaultMargin => new(3, 0, 3, 0);

    /// <summary>
    ///  Deriving classes can override this to configure a default size for their control.
    ///  This is more efficient than setting the size in the control's constructor.
    /// </summary>
    protected override Size DefaultSize => new(100, AutoSize ? PreferredHeight : 23);

    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(FlatStyle.Standard)]
    [SRDescription(nameof(SR.ButtonFlatStyleDescr))]
    public FlatStyle FlatStyle
    {
        get => (FlatStyle)_labelState[s_stateFlatStyle];
        set
        {
            // valid values are 0x0 to 0x3
            SourceGenerated.EnumValidator.Validate(value);

            if (_labelState[s_stateFlatStyle] == (int)value)
            {
                return;
            }

            bool needRecreate = (_labelState[s_stateFlatStyle] == (int)FlatStyle.System) || (value == FlatStyle.System);

            _labelState[s_stateFlatStyle] = (int)value;

            SetStyle(ControlStyles.UserPaint
                | ControlStyles.SupportsTransparentBackColor
                | ControlStyles.OptimizedDoubleBuffer, OwnerDraw);

            if (needRecreate)
            {
                // This will clear the preferred size cache - it's OK if the parent is null - it would be a NOP.
                LayoutTransaction.DoLayoutIf(AutoSize, ParentInternal, this, PropertyNames.BorderStyle);
                if (AutoSize)
                {
                    AdjustSize();
                }

                RecreateHandle();
            }
            else
            {
                Refresh();
            }
        }
    }

    /// <summary>
    ///  Gets or sets the image that is displayed on a <see cref="Label"/>.
    /// </summary>
    [Localizable(true)]
    [SRDescription(nameof(SR.ButtonImageDescr))]
    [SRCategory(nameof(SR.CatAppearance))]
    public Image? Image
    {
        get
        {
            Image? image = Properties.GetValueOrDefault<Image>(s_propImage);

            if (image is null && ImageList is not null && ImageIndexer.ActualIndex >= 0)
            {
                return ImageList.Images[ImageIndexer.ActualIndex];
            }
            else
            {
                return image;
            }
        }
        set
        {
            if (Image == value)
            {
                return;
            }

            StopAnimate();

            Properties.AddOrRemoveValue(s_propImage, value);
            if (value is not null)
            {
                ImageIndex = -1;
                ImageList = null;
            }

            // Hook up the frame changed event
            Animate();
            Invalidate();
        }
    }

    /// <summary>
    ///  Gets or sets the index value of the images displayed on the <see cref="Label"/>.
    /// </summary>
    [TypeConverter(typeof(ImageIndexConverter))]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [DefaultValue(ImageList.Indexer.DefaultIndex)]
    [Localizable(true)]
    [RefreshProperties(RefreshProperties.Repaint)]
    [SRDescription(nameof(SR.ButtonImageIndexDescr))]
    [SRCategory(nameof(SR.CatAppearance))]
    public int ImageIndex
    {
        get
        {
            if (ImageIndexer is not null)
            {
                int index = ImageIndexer.Index;

                if (ImageList is not null && (index >= ImageList.Images.Count))
                {
                    return ImageList.Images.Count - 1;
                }

                return index;
            }

            return ImageList.Indexer.DefaultIndex;
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, ImageList.Indexer.DefaultIndex);

            if (ImageIndex == value && value != ImageList.Indexer.DefaultIndex)
            {
                return;
            }

            if (value != ImageList.Indexer.DefaultIndex)
            {
                // Image.set calls ImageIndex = -1
                Properties.RemoveValue(s_propImage);
            }

            ImageIndexer.Index = value;
            Invalidate();
        }
    }

    /// <summary>
    ///  Gets or sets the key accessor for the image list. This specifies the image
    ///  from the image list to display on the <see cref="Label"/>.
    /// </summary>
    [TypeConverter(typeof(ImageKeyConverter))]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [DefaultValue(ImageList.Indexer.DefaultKey)]
    [Localizable(true)]
    [RefreshProperties(RefreshProperties.Repaint)]
    [SRDescription(nameof(SR.ButtonImageIndexDescr))]
    [SRCategory(nameof(SR.CatAppearance))]
    public string? ImageKey
    {
        get => ImageIndexer?.Key;
        set
        {
            if (ImageKey == value && !string.Equals(value, ImageList.Indexer.DefaultKey))
            {
                return;
            }

            // Image.set calls ImageIndex = -1
            Properties.RemoveValue(s_propImage);

            ImageIndexer.Key = value;
            Invalidate();
        }
    }

    internal LabelImageIndexer ImageIndexer
    {
        get
        {
            // Demand create the ImageIndexer property
            if (!Properties.TryGetValue(s_propImageIndex, out LabelImageIndexer? imageIndexer))
            {
                imageIndexer = new LabelImageIndexer(this);
                ImageIndexer = imageIndexer;
            }

            return imageIndexer;
        }
        set => Properties.AddOrRemoveValue(s_propImageIndex, value);
    }

    /// <summary>
    ///  Gets or sets the images displayed in a <see cref="Label"/>.
    /// </summary>
    [DefaultValue(null)]
    [SRDescription(nameof(SR.ButtonImageListDescr))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [SRCategory(nameof(SR.CatAppearance))]
    public ImageList? ImageList
    {
        get => Properties.GetValueOrDefault<ImageList>(s_propImageList);
        set
        {
            ImageList? imageList = ImageList;

            if (imageList == value)
            {
                return;
            }

            // Remove the previous imagelist handle recreate handler
            if (imageList is not null)
            {
                imageList.RecreateHandle -= ImageListRecreateHandle;
                imageList.Disposed -= DetachImageList;
            }

            // Make sure we don't have an Image as well as an ImageList
            if (value is not null)
            {
                Properties.RemoveValue(s_propImage);
            }

            Properties.AddOrRemoveValue(s_propImageList, value);

            // Add the new ImageList handle recreate handler
            if (value is not null)
            {
                value.RecreateHandle += ImageListRecreateHandle;
                value.Disposed += DetachImageList;
            }

            Invalidate();
        }
    }

    /// <summary>
    ///  Gets or sets the alignment of the image on the <see cref="Label"/>.
    /// </summary>
    [DefaultValue(ContentAlignment.MiddleCenter)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ButtonImageAlignDescr))]
    [SRCategory(nameof(SR.CatAppearance))]
    public ContentAlignment ImageAlign
    {
        get => Properties.GetValueOrDefault(s_propImageAlign, ContentAlignment.MiddleCenter);
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (value != ImageAlign)
            {
                Properties.AddOrRemoveValue(s_propImageAlign, value, defaultValue: ContentAlignment.MiddleCenter);
                LayoutTransaction.DoLayoutIf(AutoSize, ParentInternal, this, PropertyNames.ImageAlign);
                Invalidate();
            }
        }
    }

    /// <summary>
    ///  Indicates the "politeness" level that a client should use
    ///  to notify the user of changes to the live region.
    /// </summary>
    [SRCategory(nameof(SR.CatAccessibility))]
    [DefaultValue(AutomationLiveSetting.Off)]
    [SRDescription(nameof(SR.LiveRegionAutomationLiveSettingDescr))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public AutomationLiveSetting LiveSetting
    {
        get => _liveSetting;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);
            _liveSetting = value;
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new ImeMode ImeMode
    {
        get => base.ImeMode;
        set => base.ImeMode = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? ImeModeChanged
    {
        add => base.ImeModeChanged += value;
        remove => base.ImeModeChanged -= value;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event KeyEventHandler? KeyUp
    {
        add => base.KeyUp += value;
        remove => base.KeyUp -= value;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event KeyEventHandler? KeyDown
    {
        add => base.KeyDown += value;
        remove => base.KeyDown -= value;
    }

    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event KeyPressEventHandler? KeyPress
    {
        add => base.KeyPress += value;
        remove => base.KeyPress -= value;
    }

    internal LayoutUtils.MeasureTextCache MeasureTextCache
        => _textMeasurementCache ??= new LayoutUtils.MeasureTextCache();

    internal virtual bool OwnerDraw => IsOwnerDraw();

    /// <summary>
    ///  Gets the height of the control (in pixels), assuming a
    ///  single line of text is displayed.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.LabelPreferredHeightDescr))]
    public virtual int PreferredHeight => PreferredSize.Height;

    /// <summary>
    ///  Gets the width of the control (in pixels), assuming a single line of text is displayed.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.LabelPreferredWidthDescr))]
    public virtual int PreferredWidth => PreferredSize.Width;

    /// <summary>
    ///  Indicates whether
    ///  the container control background is rendered on the <see cref="Label"/>.
    /// </summary>
    [Obsolete("This property has been deprecated. Use BackColor instead.  https://go.microsoft.com/fwlink/?linkid=14202")]
    protected new virtual bool RenderTransparent
    {
        get => ((Control)this).RenderTransparent;
        set { }
    }

    private bool SelfSizing => CommonProperties.ShouldSelfSize(this);

    /// <summary>
    ///  Gets or sets a value indicating whether the user can tab to the <see cref="Label"/>.
    /// </summary>
    [DefaultValue(false)]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool TabStop
    {
        get => base.TabStop;
        set => base.TabStop = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? TabStopChanged
    {
        add => base.TabStopChanged += value;
        remove => base.TabStopChanged -= value;
    }

    /// <summary>
    ///  Gets or sets the horizontal alignment of the text in the control.
    /// </summary>
    [SRDescription(nameof(SR.LabelTextAlignDescr))]
    [Localizable(true)]
    [DefaultValue(ContentAlignment.TopLeft)]
    [SRCategory(nameof(SR.CatAppearance))]
    public virtual ContentAlignment TextAlign
    {
        get => Properties.GetValueOrDefault(s_propTextAlign, ContentAlignment.TopLeft);
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (TextAlign != value)
            {
                Properties.AddOrRemoveValue(s_propTextAlign, value, defaultValue: ContentAlignment.TopLeft);
                Invalidate();

                // Change the TextAlignment for SystemDrawn Labels
                if (!OwnerDraw)
                {
                    RecreateHandle();
                }

                OnTextAlignChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    ///  Gets or sets the text in the Label. Since we can have multiline support
    ///  this property just overrides the base to pluck in the Multiline editor.
    /// </summary>
    [Editor($"System.ComponentModel.Design.MultilineStringEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor)), SettingsBindable(true)]
    [AllowNull]
    public override string Text
    {
        get => base.Text;
        set => base.Text = value;
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.LabelOnTextAlignChangedDescr))]
    public event EventHandler? TextAlignChanged
    {
        add => Events.AddHandler(s_eventTextAlignChanged, value);
        remove => Events.RemoveHandler(s_eventTextAlignChanged, value);
    }

    /// <summary>
    ///  Determines whether to use compatible text rendering engine (GDI+) or not (GDI).
    /// </summary>
    [DefaultValue(false)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.UseCompatibleTextRenderingDescr))]
    public bool UseCompatibleTextRendering
    {
        get
        {
            if (CanUseTextRenderer)
            {
                return UseCompatibleTextRenderingInternal;
            }

            // Use compat text rendering (GDI+).
            return true;
        }
        set
        {
            if (UseCompatibleTextRenderingInternal != value)
            {
                UseCompatibleTextRenderingInternal = value;
                AdjustSize();
            }
        }
    }

    internal override bool SupportsUseCompatibleTextRendering => true;

    /// <summary>
    ///  Gets or sets a value indicating whether an ampersand (&amp;) included in the text of  the control.
    /// </summary>
    [SRDescription(nameof(SR.LabelUseMnemonicDescr))]
    [DefaultValue(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    public bool UseMnemonic
    {
        get => _labelState[s_stateUseMnemonic] != 0;
        set
        {
            if (UseMnemonic == value)
            {
                return;
            }

            _labelState[s_stateUseMnemonic] = value ? 1 : 0;
            MeasureTextCache.InvalidateCache();

            // The size of the label need to be adjusted when the Mnemonic is set irrespective of auto-sizing.
            using (LayoutTransaction.CreateTransactionIf(AutoSize, ParentInternal, this, PropertyNames.Text))
            {
                AdjustSize();
                Invalidate();
            }

            // Set windowStyle directly instead of recreating handle to increase efficiency.
            if (IsHandleCreated)
            {
                WINDOW_STYLE style = WindowStyle;
                if (!UseMnemonic)
                {
                    style |= (WINDOW_STYLE)STATIC_STYLES.SS_NOPREFIX;
                }
                else
                {
                    style &= ~(WINDOW_STYLE)STATIC_STYLES.SS_NOPREFIX;
                }

                WindowStyle = style;
            }
        }
    }

    /// <summary>
    ///  Updates the control in response to events that could affect either
    ///  the size of the control, or the size of the text within it.
    /// </summary>
    internal void AdjustSize()
    {
        if (!SelfSizing)
        {
            return;
        }

        // the rest is here for RTM compat.

        // If width and/or height are constrained by anchoring, don't adjust control size
        // to fit around text, since this will cause us to lose the original anchored size.
        if (!AutoSize &&
            ((Anchor & (AnchorStyles.Left | AnchorStyles.Right)) == (AnchorStyles.Left | AnchorStyles.Right) ||
             (Anchor & (AnchorStyles.Top | AnchorStyles.Bottom)) == (AnchorStyles.Top | AnchorStyles.Bottom)))
        {
            return;
        }

        // Resize control to fit around current text

        int saveHeight = _requestedHeight;
        int saveWidth = _requestedWidth;
        try
        {
            Size preferredSize = (AutoSize) ? PreferredSize : new Size(saveWidth, saveHeight);
            Size = preferredSize;
        }
        finally
        {
            _requestedHeight = saveHeight;
            _requestedWidth = saveWidth;
        }
    }

    internal void Animate() => Animate(!DesignMode && Visible && Enabled && ParentInternal is not null);

    internal void StopAnimate() => Animate(false);

    private void Animate(bool animate)
    {
        bool currentlyAnimating = _labelState[s_stateAnimating] != 0;
        if (animate == currentlyAnimating || !Properties.TryGetValue(s_propImage, out Image? image))
        {
            return;
        }

        if (animate)
        {
            ImageAnimator.Animate(image, OnFrameChanged);
            _labelState[s_stateAnimating] = animate ? 1 : 0;
        }
        else
        {
            ImageAnimator.StopAnimate(image, OnFrameChanged);
            _labelState[s_stateAnimating] = animate ? 1 : 0;
        }
    }

    protected Rectangle CalcImageRenderBounds(Image image, Rectangle r, ContentAlignment align)
    {
        Size pointImageSize = image.Size;

        int xLoc = r.X + 2;
        int yLoc;

        if ((align & WindowsFormsUtils.AnyRightAlign) != 0)
        {
            xLoc = (r.X + r.Width - 4) - pointImageSize.Width;
        }
        else if ((align & WindowsFormsUtils.AnyCenterAlign) != 0)
        {
            xLoc = r.X + (r.Width - pointImageSize.Width) / 2;
        }

        if ((align & WindowsFormsUtils.AnyBottomAlign) != 0)
        {
            yLoc = (r.Y + r.Height - 4) - pointImageSize.Height;
        }
        else if ((align & WindowsFormsUtils.AnyTopAlign) != 0)
        {
            yLoc = r.Y + 2;
        }
        else
        {
            yLoc = r.Y + (r.Height - pointImageSize.Height) / 2;
        }

        return new Rectangle(xLoc, yLoc, pointImageSize.Width, pointImageSize.Height);
    }

    protected override AccessibleObject CreateAccessibilityInstance() => new LabelAccessibleObject(this);

    /// <summary>
    ///  Get StringFormat object for rendering text using GDI+ (Graphics).
    /// </summary>
    internal virtual StringFormat CreateStringFormat()
        => ControlPaint.CreateStringFormat(this, TextAlign, AutoEllipsis, UseMnemonic);

    private TextFormatFlags CreateTextFormatFlags()
        => CreateTextFormatFlags(Size - GetBordersAndPadding());

    /// <summary>
    ///  Get TextFormatFlags flags for rendering text using GDI (TextRenderer).
    /// </summary>
    private protected TextFormatFlags CreateTextFormatFlags(Size constrainingSize)
    {
        // PREFERRED SIZE CACHING:
        //
        // Please read if you're adding a new TextFormatFlag. Whenever something can change the TextFormatFlags used
        // MeasureTextCache.InvalidateCache() should be called so we can appropriately clear.

        TextFormatFlags flags = ControlPaint.CreateTextFormatFlags(this, TextAlign, AutoEllipsis, UseMnemonic);

        // Remove WordBreak if the size is large enough to display all the text.
        if (!MeasureTextCache.TextRequiresWordBreak(Text, Font, constrainingSize, flags))
        {
            // The effect of the TextBoxControl flag is that in-word line breaking will occur if needed, this happens when AutoSize
            // is false and a one-word line still doesn't fit the binding box (width). The other effect is that partially visible
            // lines are clipped; this is how GDI+ works by default.
            flags &= ~(TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
        }

        return flags;
    }

    private void DetachImageList(object? sender, EventArgs e) => ImageList = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            StopAnimate();

            // Holding on to images and image list is a memory leak.
            if (ImageList is { } imageList)
            {
                imageList.Disposed -= DetachImageList;
                imageList.RecreateHandle -= ImageListRecreateHandle;
                Properties.RemoveValue(s_propImageList);
            }

            Properties.RemoveValue(s_propImage);

            _textToolTip?.Dispose();
            _textToolTip = null;
            _controlToolTip = false;
        }

        base.Dispose(disposing);
    }

    private void DrawImage(PaintEventArgs e, Image image, Rectangle r, ContentAlignment align)
    {
        if (GetType() == typeof(Label))
        {
            // We're not overridden, use the internal graphics accessor as we know it won't be modified.
            DrawImage(e.GraphicsInternal, image, r, align);
        }
        else
        {
            DrawImage(e.Graphics, image, r, align);
        }
    }

    /// <summary>
    ///  Draws an <see cref="Drawing.Image"/> within the specified bounds.
    /// </summary>
    protected void DrawImage(Graphics g, Image image, Rectangle r, ContentAlignment align)
        => DrawImageInternal(g, image, r, align);

    private void DrawImageInternal(Graphics g, Image image, Rectangle r, ContentAlignment align)
    {
        Rectangle loc = CalcImageRenderBounds(image, r, align);

        if (!Enabled)
        {
            ControlPaint.DrawImageDisabled(g, image, loc.X, loc.Y, BackColor);
        }
        else
        {
            g.DrawImage(image, loc.X, loc.Y, image.Width, image.Height);
        }
    }

    private Size GetBordersAndPadding()
    {
        Size bordersAndPadding = Padding.Size;

        // COMPAT: Everett added random numbers to the height of the label
        if (UseCompatibleTextRendering)
        {
            // Always return the Fontheight + some buffer else the Text gets clipped for Autosize = true..
            if (BorderStyle != BorderStyle.None)
            {
                bordersAndPadding.Height += 6; // taken from Everett.PreferredHeight
                bordersAndPadding.Width += 2;  // taken from Everett.PreferredWidth
            }
            else
            {
                bordersAndPadding.Height += 3; // taken from Everett.PreferredHeight
            }
        }
        else
        {
            // in Whidbey we'll actually ask the control the border size.

            bordersAndPadding += SizeFromClientSize(Size.Empty);
            if (BorderStyle == BorderStyle.Fixed3D)
            {
                bordersAndPadding += new Size(2, 2);
            }
        }

        return bordersAndPadding;
    }

    public override Size GetPreferredSize(Size proposedSize)
    {
        // Make sure the behavior is consistent with GetPreferredSizeCore
        if (proposedSize.Width == 1)
        {
            proposedSize.Width = 0;
        }

        if (proposedSize.Height == 1)
        {
            proposedSize.Height = 0;
        }

        return base.GetPreferredSize(proposedSize);
    }

    internal virtual bool UseGDIMeasuring() => (FlatStyle == FlatStyle.System || !UseCompatibleTextRendering);

    // See ComboBox.cs GetComboHeight
    internal override Size GetPreferredSizeCore(Size proposedConstraints)
    {
        Size bordersAndPadding = GetBordersAndPadding();

        // Subtract border area from constraints
        proposedConstraints -= bordersAndPadding;

        // Keep positive
        proposedConstraints = LayoutUtils.UnionSizes(proposedConstraints, Size.Empty);

        Size requiredSize;

        if (string.IsNullOrEmpty(Text))
        {
            // Empty labels return the font height + borders
            using var hfont = GdiCache.GetHFONTScope(Font);
            using var screen = GdiCache.GetScreenHdc();

            // This is the character that Windows uses to determine the extent
            requiredSize = screen.HDC.GetTextExtent("0", hfont);
            requiredSize.Width = 0;
        }
        else if (UseGDIMeasuring())
        {
            TextFormatFlags format = FlatStyle == FlatStyle.System ? TextFormatFlags.Default : CreateTextFormatFlags(proposedConstraints);
            requiredSize = MeasureTextCache.GetTextSize(Text, Font, proposedConstraints, format);
        }
        else
        {
            // GDI+ rendering.
            using var screen = GdiCache.GetScreenDCGraphics();
            using StringFormat stringFormat = CreateStringFormat();
            SizeF bounds = (proposedConstraints.Width == 1) ?
                new SizeF(0, proposedConstraints.Height) :
                new SizeF(proposedConstraints.Width, proposedConstraints.Height);

            requiredSize = Size.Ceiling(screen.Graphics.MeasureString(Text, Font, bounds, stringFormat));
        }

        requiredSize += bordersAndPadding;

        return requiredSize;
    }

    /// <summary>
    ///  This method is to be called by LabelDesigner, using private reflection, to get the location of the text snaplines.
    /// </summary>
    private int GetLeadingTextPaddingFromTextFormatFlags()
    {
        if (!IsHandleCreated)
        {
            return 0;
        }

        // If we are using GDI+ the code below will not work, except/if the style is FlatStyle.System since GDI
        // will be used in that case.
        if (UseCompatibleTextRendering && FlatStyle != FlatStyle.System)
        {
            return 0;
        }

        TextFormatFlags flags = CreateTextFormatFlags();
        TextPaddingOptions padding = default;

        if ((flags & TextFormatFlags.NoPadding) == TextFormatFlags.NoPadding)
        {
            padding = TextPaddingOptions.NoPadding;
        }
        else if ((flags & TextFormatFlags.LeftAndRightPadding) == TextFormatFlags.LeftAndRightPadding)
        {
            padding = TextPaddingOptions.LeftAndRightPadding;
        }

        using var hfont = GdiCache.GetHFONTScope(Font);
        DRAWTEXTPARAMS dtParams = hfont.GetTextMargins(padding);

        // This is actually leading margin.
        return dtParams.iLeftMargin;
    }

    private void ImageListRecreateHandle(object? sender, EventArgs e)
    {
        if (IsHandleCreated)
        {
            Invalidate();
        }
    }

    internal override bool IsMnemonicsListenerAxSourced => true;

    /// <summary>
    ///  This method is required because the Label constructor needs to know if the control is
    ///  OwnerDraw but it should not call the virtual property because if a derived class has
    ///  overridden the method, the derived class version will be called (before the derived
    ///  class constructor is called).
    /// </summary>
    private bool IsOwnerDraw() => FlatStyle != FlatStyle.System;

    protected override void OnMouseEnter(EventArgs e)
    {
        if (!_controlToolTip && !DesignMode && AutoEllipsis && _showToolTip && _textToolTip is not null)
        {
            try
            {
                _controlToolTip = true;
                _textToolTip.Show(WindowsFormsUtils.TextWithoutMnemonics(Text), this);
            }
            finally
            {
                _controlToolTip = false;
            }
        }

        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        if (!_controlToolTip && _textToolTip is not null && _textToolTip.GetHandleCreated())
        {
            _textToolTip.RemoveAll();

            _textToolTip.Hide(this);
        }

        base.OnMouseLeave(e);
    }

    private void OnFrameChanged(object? o, EventArgs e)
    {
        if (Disposing || IsDisposed)
        {
            return;
        }

        if (IsHandleCreated && InvokeRequired)
        {
            BeginInvoke(new EventHandler(OnFrameChanged), o, e);
            return;
        }

        Invalidate();
    }

    protected override void OnFontChanged(EventArgs e)
    {
        MeasureTextCache.InvalidateCache();
        base.OnFontChanged(e);
        AdjustSize();
        Invalidate();
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleDestroyed(e);
        if (_textToolTip is not null && _textToolTip.GetHandleCreated())
        {
            _textToolTip.DestroyHandle();
        }
    }

    protected override void OnTextChanged(EventArgs e)
    {
        using (LayoutTransaction.CreateTransactionIf(AutoSize, ParentInternal, this, PropertyNames.Text))
        {
            MeasureTextCache.InvalidateCache();
            base.OnTextChanged(e);
            AdjustSize();
            Invalidate();
        }

        if (!IsAccessibilityObjectCreated)
        {
            return;
        }

        if (LiveSetting != AutomationLiveSetting.Off)
        {
            AccessibilityObject.RaiseLiveRegionChanged();
        }

        using var textVariant = (VARIANT)Text;
        AccessibilityObject.RaiseAutomationPropertyChangedEvent(UIA_PROPERTY_ID.UIA_NamePropertyId, textVariant, textVariant);
    }

    protected virtual void OnTextAlignChanged(EventArgs e)
    {
        if (Events[s_eventTextAlignChanged] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    protected override void OnPaddingChanged(EventArgs e)
    {
        base.OnPaddingChanged(e);
        AdjustSize();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Animate();
        ImageAnimator.UpdateFrames(Image);

        Rectangle face = LayoutUtils.DeflateRect(ClientRectangle, Padding);
        Image? i = Image;
        if (i is not null)
        {
            DrawImage(e, i, face, RtlTranslateAlignment(ImageAlign));
        }

        Color color;
        using (DeviceContextHdcScope hdc = new(e))
        {
            color = hdc.FindNearestColor(Enabled ? ForeColor : DisabledColor);
        }

        // Do actual drawing

        if (AutoEllipsis)
        {
            Rectangle clientRect = ClientRectangle;
            Size preferredSize = GetPreferredSize(new Size(clientRect.Width, clientRect.Height));
            _showToolTip = (clientRect.Width < preferredSize.Width || clientRect.Height < preferredSize.Height);
        }
        else
        {
            _showToolTip = false;
        }

        if (UseCompatibleTextRendering)
        {
            using StringFormat stringFormat = CreateStringFormat();
            if (Enabled)
            {
                using var brush = color.GetCachedSolidBrushScope();
                e.GraphicsInternal.DrawString(Text, Font, brush, face, stringFormat);
            }
            else
            {
                ControlPaint.DrawStringDisabled(e.GraphicsInternal, Text, Font, color, face, stringFormat);
            }
        }
        else
        {
            TextFormatFlags flags = CreateTextFormatFlags();

            if (Enabled)
            {
                TextRenderer.DrawTextInternal(e, Text, Font, face, color, flags: flags);
            }
            else
            {
                // Theme specs -- if the BackColor is darker than Control, we use
                // ControlPaint.Dark(BackColor). Otherwise we use ControlDark.

                Color disabledTextForeColor = TextRenderer.DisabledTextColor(BackColor);
                TextRenderer.DrawTextInternal(e, Text, Font, face, disabledTextForeColor, flags: flags);
            }
        }

        base.OnPaint(e); // raise paint event
    }

    /// <summary>
    ///  Overridden by LinkLabel.
    /// </summary>
    internal virtual void OnAutoEllipsisChanged()
    {
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        base.OnEnabledChanged(e);
        Animate();
    }

    protected override void OnParentChanged(EventArgs e)
    {
        base.OnParentChanged(e);
        if (SelfSizing)
        {
            // In the case of SelfSizing we don't know what size to be until we're parented.
            AdjustSize();
        }

        Animate();
    }

    protected override void OnRightToLeftChanged(EventArgs e)
    {
        MeasureTextCache.InvalidateCache();
        base.OnRightToLeftChanged(e);
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
        base.OnVisibleChanged(e);
        Animate();
    }

    private protected override void PrintToMetaFileRecursive(HDC hDC, IntPtr lParam, Rectangle bounds)
    {
        base.PrintToMetaFileRecursive(hDC, lParam, bounds);

        using DCMapping mapping = new(hDC, bounds);
        using Graphics g = hDC.CreateGraphics();
        ControlPaint.PrintBorder(g, new Rectangle(Point.Empty, Size), BorderStyle, Border3DStyle.SunkenOuter);
    }

    protected internal override bool ProcessMnemonic(char charCode)
    {
        if (UseMnemonic && IsMnemonic(charCode, Text) && CanProcessMnemonic())
        {
            Control? parent = ParentInternal;
            if (parent is not null)
            {
                if (parent.SelectNextControl(this, true, false, true, false) && !parent.ContainsFocus)
                {
                    parent.Focus();
                }
            }

            return true;
        }

        return false;
    }

    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
        if ((specified & BoundsSpecified.Height) != BoundsSpecified.None)
        {
            _requestedHeight = height;
        }

        if ((specified & BoundsSpecified.Width) != BoundsSpecified.None)
        {
            _requestedWidth = width;
        }

        if (AutoSize && SelfSizing)
        {
            Size preferredSize = PreferredSize;
            width = preferredSize.Width;
            height = preferredSize.Height;
        }

        base.SetBoundsCore(x, y, width, height, specified);

        Debug.Assert(!AutoSize || (AutoSize && !SelfSizing) || Size == PreferredSize,
            "It is SetBoundsCore's responsibility to ensure Size = PreferredSize when AutoSize is true.");
    }

    private void ResetImage() => Image = null;

    private bool ShouldSerializeImage() => Properties.ContainsKey(s_propImage);

    internal override void SetToolTip(ToolTip toolTip)
    {
        if (toolTip is null || _controlToolTip)
        {
            return;
        }

        // Label now has its own Tooltip for AutoEllipsis. So this control too falls in special casing.
        // We need to disable the LABEL AutoEllipsis tooltip and show this tooltip always.
        _controlToolTip = true;
    }

    internal override bool SupportsUiaProviders => true;

    /// <summary>
    ///  Returns a string representation for this control.
    /// </summary>
    public override string ToString() => $"{base.ToString()}, Text: {Text}";

    protected override void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            case PInvokeCore.WM_NCHITTEST:
                // Label returns HT_TRANSPARENT for everything, so all messages get routed to the parent. Change
                // this so we can tell what's going on.

                Rectangle rectInScreen = RectangleToScreen(new Rectangle(0, 0, Width, Height));
                Point pt = new((int)m.LParamInternal);
                m.ResultInternal = (LRESULT)(nint)(rectInScreen.Contains(pt) ? PInvoke.HTCLIENT : PInvoke.HTNOWHERE);
                break;

            default:
                base.WndProc(ref m);
                break;
        }
    }
}
