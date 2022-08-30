// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.ButtonInternal;
using System.Windows.Forms.Layout;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

/// <summary>
///  Implements the basic functionality required by a button control.
/// </summary>
[Designer($"System.Windows.Forms.Design.ButtonBaseDesigner, {AssemblyRef.SystemDesign}")]
public abstract partial class ButtonBase : Control, ICommandBindingTargetProvider
{
    private FlatStyle _flatStyle = FlatStyle.Standard;
    private ContentAlignment _imageAlign = ContentAlignment.MiddleCenter;
    private ContentAlignment _textAlign = ContentAlignment.MiddleCenter;
    private TextImageRelation _textImageRelation = TextImageRelation.Overlay;
    private readonly ImageList.Indexer _imageIndex = new();
    private FlatButtonAppearance? _flatAppearance;
    private ImageList? _imageList;
    private Image? _image;

    private const int FlagMouseOver = 0x0001;
    private const int FlagMouseDown = 0x0002;
    private const int FlagMousePressed = 0x0004;
    private const int FlagInButtonUp = 0x0008;
    private const int FlagCurrentlyAnimating = 0x0010;
    private const int FlagAutoEllipsis = 0x0020;
    private const int FlagIsDefault = 0x0040;
    private const int FlagUseMnemonic = 0x0080;
    private const int FlagShowToolTip = 0x0100;
    private int _state;

    private ToolTip? _textToolTip;

    // This allows the user to disable visual styles for the button so that it inherits its background color
    private bool _enableVisualStyleBackground = true;

    private bool _isEnableVisualStyleBackgroundSet;

    private ButtonBaseAdapter? _adapter;
    private FlatStyle _cachedAdapterType;

    // Backing fields for the infrastructure to make ToolStripItem bindable and introduce (bindable) ICommand.
    private Input.ICommand? _command;
    private object? _commandParameter;

    internal static readonly object s_commandChangedEvent = new();
    internal static readonly object s_commandParameterChangedEvent = new();
    internal static readonly object s_commandCanExecuteChangedEvent = new();

    /// <summary>
    ///  Initializes a new instance of the <see cref="ButtonBase"/> class.
    /// </summary>
    protected ButtonBase()
    {
        // If Button doesn't want double-clicks, we should introduce a StandardDoubleClick style.
        // Checkboxes probably want double-click's, and RadioButtons certainly do
        // (useful e.g. on a Wizard).
        SetStyle(
            ControlStyles.SupportsTransparentBackColor
                | ControlStyles.Opaque
                | ControlStyles.ResizeRedraw
                | ControlStyles.OptimizedDoubleBuffer
                // We gain about 2% in painting by avoiding extra GetWindowText calls
                | ControlStyles.CacheText
                | ControlStyles.StandardClick,
            true);

        // This class overrides GetPreferredSizeCore, let Control automatically cache the result
        SetExtendedState(ExtendedStates.UserPreferredSizeCache, true);

        SetStyle(ControlStyles.UserMouse | ControlStyles.UserPaint, OwnerDraw);
        SetFlag(FlagUseMnemonic, true);
        SetFlag(FlagShowToolTip, false);
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the ellipsis character (...) appears at the right edge of the control,
    ///  denoting that the control text extends beyond the specified length of the control.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [SRDescription(nameof(SR.ButtonAutoEllipsisDescr))]
    public bool AutoEllipsis
    {
        get => GetFlag(FlagAutoEllipsis);
        set
        {
            if (value == AutoEllipsis)
            {
                return;
            }

            SetFlag(FlagAutoEllipsis, value);
            if (value)
            {
                _textToolTip ??= new ToolTip();
            }

            Invalidate();
        }
    }

    /// <summary>
    ///  Indicates whether the control is automatically resized to fit its contents.
    /// </summary>
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public override bool AutoSize
    {
        get => base.AutoSize;
        set
        {
            base.AutoSize = value;

            // Don't show ellipsis if the control is auto sized.
            if (value)
            {
                AutoEllipsis = false;
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
    ///  The background color of this control. This is an ambient property and will always return a non-null value.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ControlBackColorDescr))]
    public override Color BackColor
    {
        get => base.BackColor;
        set
        {
            if (DesignMode)
            {
                if (value != Color.Empty)
                {
                    if (!AreDesignTimeFeaturesSupported)
                    {
                        throw new NotSupportedException(SR.DesignTimeFeaturesNotSupported);
                    }

                    PropertyDescriptor? pd = TypeDescriptor.GetProperties(this)["UseVisualStyleBackColor"];
                    pd?.SetValue(this, false);
                }
            }
            else
            {
                UseVisualStyleBackColor = false;
            }

            base.BackColor = value;
        }
    }

    /// <summary>
    ///  Gets or sets the <see cref="Input.ICommand"/> whose <see cref="Input.ICommand.Execute(object?)"/>
    ///  method will be called when the <see cref="Control.Click"/> event gets invoked.
    /// </summary>
    [Bindable(true)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.CommandComponentCommandDescr))]
    public Input.ICommand? Command
    {
        get => _command;
        set => ICommandBindingTargetProvider.CommandSetter(this, value, ref _command);
    }

    /// <summary>
    ///  Occurs when the <see cref="Input.ICommand.CanExecute(object?)"/> status of the
    ///  <see cref="Input.ICommand"/> which is assigned to the <see cref="Command"/> property has changed.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.CommandCanExecuteChangedEventDescr))]
    public event EventHandler? CommandCanExecuteChanged
    {
        add => Events.AddHandler(s_commandCanExecuteChangedEvent, value);
        remove => Events.RemoveHandler(s_commandCanExecuteChangedEvent, value);
    }

    /// <summary>
    ///  Occurs when the assigned <see cref="Input.ICommand"/> of the <see cref="Command"/> property has changed.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.CommandChangedEventDescr))]
    public event EventHandler? CommandChanged
    {
        add => Events.AddHandler(s_commandChangedEvent, value);
        remove => Events.RemoveHandler(s_commandChangedEvent, value);
    }

    /// <summary>
    ///  Gets or sets the parameter that is passed to the <see cref="Input.ICommand"/>
    ///  which is assigned to the <see cref="Command"/> property.
    /// </summary>
    [Bindable(true)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.CommandComponentCommandParameterDescr))]
    public object? CommandParameter
    {
        get => _commandParameter;
        set
        {
            if (!Equals(_commandParameter, value))
            {
                _commandParameter = value;
                OnCommandParameterChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    ///  Occurs when the value of the <see cref="CommandParameter"/> property has changed.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRDescription(nameof(SR.CommandParameterChangedEventDescr))]
    public event EventHandler? CommandParameterChanged
    {
        add => Events.AddHandler(s_commandParameterChangedEvent, value);
        remove => Events.RemoveHandler(s_commandParameterChangedEvent, value);
    }

    protected override Size DefaultSize => new(75, 23);

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            if (!OwnerDraw)
            {
                // WS_EX_RIGHT overrides the BS_XXXX alignment styles
                cp.ExStyle &= ~(int)WINDOW_EX_STYLE.WS_EX_RIGHT;

                cp.Style |= PInvoke.BS_MULTILINE;

                if (IsDefault)
                {
                    cp.Style |= PInvoke.BS_DEFPUSHBUTTON;
                }

                ContentAlignment align = RtlTranslateContent(TextAlign);

                if ((align & WindowsFormsUtils.AnyLeftAlign) != 0)
                {
                    cp.Style |= PInvoke.BS_LEFT;
                }
                else if ((align & WindowsFormsUtils.AnyRightAlign) != 0)
                {
                    cp.Style |= PInvoke.BS_RIGHT;
                }
                else
                {
                    cp.Style |= PInvoke.BS_CENTER;
                }

                if ((align & WindowsFormsUtils.AnyTopAlign) != 0)
                {
                    cp.Style |= PInvoke.BS_TOP;
                }
                else if ((align & WindowsFormsUtils.AnyBottomAlign) != 0)
                {
                    cp.Style |= PInvoke.BS_BOTTOM;
                }
                else
                {
                    cp.Style |= PInvoke.BS_VCENTER;
                }
            }

            return cp;
        }
    }

    protected override ImeMode DefaultImeMode => ImeMode.Disable;

    protected internal bool IsDefault
    {
        get => GetFlag(FlagIsDefault);
        set
        {
            if (value == IsDefault)
            {
                return;
            }

            SetFlag(FlagIsDefault, value);
            if (IsHandleCreated)
            {
                if (OwnerDraw)
                {
                    Invalidate();
                }
                else
                {
                    UpdateStyles();
                }
            }
        }
    }

    /// <summary>
    ///  Gets or sets the flat style appearance of the button control.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(FlatStyle.Standard)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ButtonFlatStyleDescr))]
    public FlatStyle FlatStyle
    {
        get => _flatStyle;
        set
        {
            if (value == FlatStyle)
            {
                return;
            }

            SourceGenerated.EnumValidator.Validate(value);

            _flatStyle = value;
            LayoutTransaction.DoLayoutIf(AutoSize, ParentInternal, this, PropertyNames.FlatStyle);
            Invalidate();
            UpdateOwnerDraw();
        }
    }

    [Browsable(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ButtonFlatAppearance))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public FlatButtonAppearance FlatAppearance => _flatAppearance ??= new FlatButtonAppearance(this);

    /// <summary>
    ///  Gets or sets the image that is displayed on a button control.
    /// </summary>
    [SRDescription(nameof(SR.ButtonImageDescr))]
    [Localizable(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    public Image? Image
    {
        get
        {
            if (_image is null && _imageList is not null)
            {
                int actualIndex = _imageIndex.ActualIndex;

                // Before VS 2005 (Whidbey) we used to use ImageIndex rather than ImageIndexer.ActualIndex.
                // ImageIndex clamps to the length of the image list.  We need to replicate this logic here for
                // backwards compatibility. We do not bake this into ImageIndexer because different controls
                // treat this scenario differently.
                if (actualIndex >= _imageList.Images.Count)
                {
                    actualIndex = _imageList.Images.Count - 1;
                }

                if (actualIndex >= 0)
                {
                    return _imageList.Images[actualIndex];
                }

                Debug.Assert(_image is null, "We expect to be returning null.");
            }

            return _image;
        }
        set
        {
            if (value == Image)
            {
                return;
            }

            StopAnimate();

            _image = value;
            if (_image is not null)
            {
                ImageIndex = ImageList.Indexer.DefaultIndex;
                ImageList = null;
            }

            LayoutTransaction.DoLayoutIf(AutoSize, ParentInternal, this, PropertyNames.Image);
            Animate();
            Invalidate();
        }
    }

    /// <summary>
    ///  Gets or sets the alignment of the image on the button control.
    /// </summary>
    [DefaultValue(ContentAlignment.MiddleCenter)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ButtonImageAlignDescr))]
    [SRCategory(nameof(SR.CatAppearance))]
    public ContentAlignment ImageAlign
    {
        get => _imageAlign;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (value != _imageAlign)
            {
                _imageAlign = value;
                LayoutTransaction.DoLayoutIf(AutoSize, ParentInternal, this, PropertyNames.ImageAlign);
                Invalidate();
            }
        }
    }

    /// <summary>
    ///  Gets or sets the image list index value of the image displayed on the button control.
    /// </summary>
    /// <inheritdoc cref="ImageKey"/>
    [TypeConverter(typeof(ImageIndexConverter))]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [Localizable(true)]
    [DefaultValue(ImageList.Indexer.DefaultIndex)]
    [RefreshProperties(RefreshProperties.Repaint)]
    [SRDescription(nameof(SR.ButtonImageIndexDescr))]
    [SRCategory(nameof(SR.CatAppearance))]
    public int ImageIndex
    {
        get => _imageIndex.Index != ImageList.Indexer.DefaultIndex && _imageList is not null && _imageIndex.Index >= _imageList.Images.Count
            ? _imageList.Images.Count - 1
            : _imageIndex.Index;
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, ImageList.Indexer.DefaultIndex);

            if (value == _imageIndex.Index && value != ImageList.Indexer.DefaultIndex)
            {
                return;
            }

            if (value != ImageList.Indexer.DefaultIndex)
            {
                // Image.set calls ImageIndex = -1
                _image = null;
            }

            // If they were previously using keys - this should clear out the image key field.
            _imageIndex.Index = value;
            Invalidate();
        }
    }

    /// <summary>
    ///  Gets or sets the key accessor for the image in the <see cref="ImageList"/>.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The <see cref="ImageKey"/> property specifies the image from the image list to display on the control.
    ///  </para>
    ///  <para>
    ///   <see cref="ImageKey"/> and <see cref="ImageIndex"/> are mutually exclusive, meaning if one is set, the other
    ///   is set to an invalid value and ignored. If you set the <see cref="ImageKey"/> property, the
    ///   <see cref="ImageIndex"/> property is automatically set to -1. Alternatively, if you set the
    ///   <see cref="ImageIndex"/> property, the <see cref="ImageKey"/> is automatically set to an empty string ("").
    ///  </para>
    /// </remarks>
    [TypeConverter(typeof(ImageKeyConverter))]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [Localizable(true)]
    [DefaultValue(ImageList.Indexer.DefaultKey)]
    [RefreshProperties(RefreshProperties.Repaint)]
    [SRDescription(nameof(SR.ButtonImageIndexDescr))]
    [SRCategory(nameof(SR.CatAppearance))]
    public string ImageKey
    {
        get => _imageIndex.Key;
        set
        {
            if (value == _imageIndex.Key && !string.Equals(value, ImageList.Indexer.DefaultKey))
            {
                return;
            }

            if (value is not null)
            {
                // Image.set calls ImageIndex = -1
                _image = null;
            }

            // If they were previously using indexes - this should clear out the image index field.
            _imageIndex.Key = value;
            Invalidate();
        }
    }

    /// <summary>
    ///  Gets or sets the <see cref="Forms.ImageList"/> that contains the <see cref="Drawing.Image"/>
    ///  displayed on a button control.
    /// </summary>
    [DefaultValue(null)]
    [SRDescription(nameof(SR.ButtonImageListDescr))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [SRCategory(nameof(SR.CatAppearance))]
    public ImageList? ImageList
    {
        get => _imageList;
        set
        {
            if (value == _imageList)
            {
                return;
            }

            EventHandler recreateHandler = ImageListRecreateHandle;
            EventHandler disposedHandler = DetachImageList;

            // Detach old event handlers
            if (_imageList is not null)
            {
                _imageList.RecreateHandle -= recreateHandler;
                _imageList.Disposed -= disposedHandler;
            }

            // Make sure we don't have an Image as well as an ImageList
            if (value is not null)
            {
                _image = null; // Image.set calls ImageList = null
            }

            _imageList = value;
            _imageIndex.ImageList = value;

            // Wire up new event handlers
            if (value is not null)
            {
                value.RecreateHandle += recreateHandler;
                value.Disposed += disposedHandler;
            }

            Invalidate();
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

    /// <summary>
    ///  Specifies whether the control is willing to process mnemonics when hosted in an container ActiveX (Ax Sourcing).
    /// </summary>
    internal override bool IsMnemonicsListenerAxSourced => true;

    /// <summary>
    ///  The area of the button encompassing any changes between the button's resting appearance and its appearance
    ///  when the mouse is over it.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Consider overriding this property if you override any painting methods, or your button may not paint correctly
    ///   or may have flicker. Returning <see cref="Control.ClientRectangle"/> is safe for correct painting but may
    ///   still cause flicker.
    ///  </para>
    /// </remarks>
    internal virtual Rectangle OverChangeRectangle
    {
        get
        {
            if (FlatStyle == FlatStyle.Standard)
            {
                // Return an out of bounds rectangle to avoid invalidation.
                return new Rectangle(-1, -1, 1, 1);
            }
            else
            {
                return ClientRectangle;
            }
        }
    }

    internal bool OwnerDraw => FlatStyle != FlatStyle.System;

    bool? ICommandBindingTargetProvider.PreviousEnabledStatus { get; set; }

    /// <summary>
    ///  The area of the button encompassing any changes between the button's appearance when the mouse is over it
    ///  but not pressed and when it is pressed.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Consider overriding this property if you override any painting methods, or your button may not paint correctly
    ///   or may have flicker. Returning <see cref="Control.ClientRectangle"/> is safe for correct painting but may
    ///   still cause flicker.
    ///  </para>
    /// </remarks>
    internal virtual Rectangle DownChangeRectangle => ClientRectangle;

    internal bool MouseIsPressed => GetFlag(FlagMousePressed);

    // a "smart" version of mouseDown for Appearance.Button CheckBoxes & RadioButtons
    // for these, instead of being based on the actual mouse state, it's based on the appropriate button state
    internal bool MouseIsDown => GetFlag(FlagMouseDown);

    // a "smart" version of mouseOver for Appearance.Button CheckBoxes & RadioButtons
    // for these, instead of being based on the actual mouse state, it's based on the appropriate button state
    internal bool MouseIsOver => GetFlag(FlagMouseOver);

    /// <summary>
    ///  Indicates whether the tooltip should be shown
    /// </summary>
    internal bool ShowToolTip
    {
        get => GetFlag(FlagShowToolTip);
        set => SetFlag(FlagShowToolTip, value);
    }

    [Editor(
        $"System.ComponentModel.Design.MultilineStringEditor, {AssemblyRef.SystemDesign}",
        typeof(UITypeEditor)),
        SettingsBindable(true)]
    [AllowNull]
    public override string Text
    {
        get => base.Text;
        set => base.Text = value;
    }

    /// <summary>
    ///  Gets or sets the alignment of the text on the button control.
    /// </summary>
    [DefaultValue(ContentAlignment.MiddleCenter)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ButtonTextAlignDescr))]
    [SRCategory(nameof(SR.CatAppearance))]
    public virtual ContentAlignment TextAlign
    {
        get => _textAlign;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (value == TextAlign)
            {
                return;
            }

            _textAlign = value;
            LayoutTransaction.DoLayoutIf(AutoSize, ParentInternal, this, PropertyNames.TextAlign);
            if (OwnerDraw)
            {
                Invalidate();
            }
            else
            {
                UpdateStyles();
            }
        }
    }

    [DefaultValue(TextImageRelation.Overlay)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ButtonTextImageRelationDescr))]
    [SRCategory(nameof(SR.CatAppearance))]
    public TextImageRelation TextImageRelation
    {
        get => _textImageRelation;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (value == TextImageRelation)
            {
                return;
            }

            _textImageRelation = value;
            LayoutTransaction.DoLayoutIf(AutoSize, ParentInternal, this, PropertyNames.TextImageRelation);
            Invalidate();
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether an ampersand (&amp;) included in the text of
    ///  the control.
    /// </summary>
    [SRDescription(nameof(SR.ButtonUseMnemonicDescr))]
    [DefaultValue(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    public bool UseMnemonic
    {
        get => GetFlag(FlagUseMnemonic);
        set
        {
            if (value == UseMnemonic)
            {
                return;
            }

            SetFlag(FlagUseMnemonic, value);
            LayoutTransaction.DoLayoutIf(AutoSize, ParentInternal, this, PropertyNames.Text);
            Invalidate();
        }
    }

    private void Animate() => Animate(!DesignMode && Visible && Enabled && ParentInternal is not null);

    private void StopAnimate() => Animate(animate: false);

    private void Animate(bool animate)
    {
        if (animate != GetFlag(FlagCurrentlyAnimating))
        {
            if (animate)
            {
                if (_image is not null)
                {
                    ImageAnimator.Animate(_image, new EventHandler(OnFrameChanged));
                    SetFlag(FlagCurrentlyAnimating, animate);
                }
            }
            else
            {
                if (_image is not null)
                {
                    ImageAnimator.StopAnimate(_image, new EventHandler(OnFrameChanged));
                    SetFlag(FlagCurrentlyAnimating, animate);
                }
            }
        }
    }

    protected override AccessibleObject CreateAccessibilityInstance() => new ButtonBaseAccessibleObject(this);

    private void DetachImageList(object? sender, EventArgs e) => ImageList = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            StopAnimate();
            if (_imageList is not null)
            {
                _imageList.Disposed -= DetachImageList;
            }

            _textToolTip?.Dispose();
            _textToolTip = null;
        }

        base.Dispose(disposing);
    }

    private bool GetFlag(int flag)
    {
        return ((_state & flag) == flag);
    }

    private void ImageListRecreateHandle(object? sender, EventArgs e)
    {
        if (IsHandleCreated)
        {
            Invalidate();
        }
    }

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);
        OnRequestCommandExecute(e);
    }

    /// <summary>
    ///  Raises the <see cref="OnGotFocus"/> event.
    /// </summary>
    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        Invalidate();
    }

    /// <summary>
    ///  Raises the <see cref="OnLostFocus"/> event.
    /// </summary>
    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);

        // Hitting tab while holding down the space key.
        SetFlag(FlagMouseDown, false);
        Capture = false;

        Invalidate();
    }

    /// <summary>
    ///  Raises the <see cref="Control.OnMouseEnter"/> event.
    /// </summary>
    protected override void OnMouseEnter(EventArgs eventargs)
    {
        SetFlag(FlagMouseOver, true);
        Invalidate();
        if (!DesignMode && AutoEllipsis && ShowToolTip && _textToolTip is not null)
        {
            _textToolTip.Show(WindowsFormsUtils.TextWithoutMnemonics(Text), this);
        }

        // Call base last, so if it invokes any listeners that disable the button, we don't have to recheck.
        base.OnMouseEnter(eventargs);
    }

    /// <summary>
    ///  Raises the <see cref="Control.OnMouseLeave"/> event.
    /// </summary>
    protected override void OnMouseLeave(EventArgs eventargs)
    {
        SetFlag(FlagMouseOver, false);
        _textToolTip?.Hide(this);

        Invalidate();
        // call base last, so if it invokes any listeners that disable the button, we
        // don't have to recheck
        base.OnMouseLeave(eventargs);
    }

    /// <summary>
    ///  Raises the <see cref="Control.OnMouseMove"/> event.
    /// </summary>
    protected override void OnMouseMove(MouseEventArgs mevent)
    {
        if (mevent.Button != MouseButtons.None && GetFlag(FlagMousePressed))
        {
            Rectangle r = ClientRectangle;
            if (!r.Contains(mevent.X, mevent.Y))
            {
                if (GetFlag(FlagMouseDown))
                {
                    SetFlag(FlagMouseDown, false);
                    Invalidate(DownChangeRectangle);
                }
            }
            else
            {
                if (!GetFlag(FlagMouseDown))
                {
                    SetFlag(FlagMouseDown, true);
                    Invalidate(DownChangeRectangle);
                }
            }
        }

        // call base last, so if it invokes any listeners that disable the button, we
        // don't have to recheck
        base.OnMouseMove(mevent);
    }

    /// <summary>
    ///  Raises the <see cref="Control.OnMouseDown"/> event.
    /// </summary>
    protected override void OnMouseDown(MouseEventArgs mevent)
    {
        if (mevent.Button == MouseButtons.Left)
        {
            SetFlag(FlagMouseDown, true);
            SetFlag(FlagMousePressed, true);
            Invalidate(DownChangeRectangle);
        }

        // call base last, so if it invokes any listeners that disable the button, we
        // don't have to recheck
        base.OnMouseDown(mevent);
    }

    /// <summary>
    ///  Raises the <see cref="OnMouseUp"/> event.
    /// </summary>
    protected override void OnMouseUp(MouseEventArgs mevent)
    {
        base.OnMouseUp(mevent);
    }

    /// <summary>
    ///  Used for quick re-painting of the button after the pressed state.
    /// </summary>
    protected void ResetFlagsandPaint()
    {
        SetFlag(FlagMousePressed, false);
        SetFlag(FlagMouseDown, false);
        Invalidate(DownChangeRectangle);
        Update();
    }

    /// <summary>
    ///  Central paint dispatcher to one of the three styles of painting.
    /// </summary>
    private void PaintControl(PaintEventArgs pevent)
    {
        Debug.Assert(GetStyle(ControlStyles.UserPaint), "Shouldn't be in PaintControl when control is not UserPaint style");
        Adapter.Paint(pevent);
    }

    public override Size GetPreferredSize(Size proposedSize)
    {
        // TableLayoutPanel passes width = 1 to get the minimum auto size width, since Buttons word-break text
        // that width would be the size of the widest character in the text. We need to make the proposed size
        // unbounded. This is the same as what Label does.
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

    internal override Size GetPreferredSizeCore(Size proposedConstraints)
    {
        Size preferedSize = Adapter.GetPreferredSizeCore(proposedConstraints);
        return LayoutUtils.UnionSizes(preferedSize + Padding.Size, MinimumSize);
    }

    internal ButtonBaseAdapter Adapter
    {
        get
        {
            if (_adapter is null || FlatStyle != _cachedAdapterType)
            {
                switch (FlatStyle)
                {
                    case FlatStyle.Standard:
                        _adapter = CreateStandardAdapter();
                        break;
                    case FlatStyle.Popup:
                        _adapter = CreatePopupAdapter();
                        break;
                    case FlatStyle.Flat:
                        _adapter = CreateFlatAdapter();
                        ;
                        break;
                    default:
                        Debug.Fail($"Unsupported FlatStyle: \"{FlatStyle}\"");
                        break;
                }

                _cachedAdapterType = FlatStyle;
            }

            return _adapter;
        }
    }

    internal virtual ButtonBaseAdapter CreateFlatAdapter()
    {
        Debug.Fail("Derived classes need to provide a meaningful implementation.");
        return null;
    }

    internal virtual ButtonBaseAdapter CreatePopupAdapter()
    {
        Debug.Fail("Derived classes need to provide a meaningful implementation.");
        return null;
    }

    internal virtual ButtonBaseAdapter CreateStandardAdapter()
    {
        Debug.Fail("Derived classes need to provide a meaningful implementation.");
        return null;
    }

    internal virtual StringFormat CreateStringFormat()
    {
        if (Adapter is null)
        {
            Debug.Fail("Adapter not expected to be null at this point");
            return new StringFormat();
        }

        return Adapter.CreateStringFormat();
    }

    internal virtual TextFormatFlags CreateTextFormatFlags()
    {
        if (Adapter is null)
        {
            Debug.Fail("Adapter not expected to be null at this point");
            return TextFormatFlags.Default;
        }

        return Adapter.CreateTextFormatFlags();
    }

    /// <summary>
    ///  Raises the <see cref="CommandChanged"/> event.
    /// </summary>
    /// <param name="e">An empty <see cref="EventArgs"/> instance.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnCommandChanged(EventArgs e) => RaiseEvent(s_commandChangedEvent, e);

    /// <summary>
    ///  Raises the <see cref="CommandCanExecuteChanged"/> event.
    /// </summary>
    /// <param name="e">An empty <see cref="EventArgs"/> instance.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnCommandCanExecuteChanged(EventArgs e) =>
        ((EventHandler?)Events[s_commandCanExecuteChangedEvent])?.Invoke(this, e);

    /// <summary>
    ///  Raises the <see cref="CommandParameterChanged"/> event.
    /// </summary>
    /// <param name="e">An empty <see cref="EventArgs"/> instance.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnCommandParameterChanged(EventArgs e) => RaiseEvent(s_commandParameterChangedEvent, e);

    /// <summary>
    ///  Called in the context of <see cref="OnClick(EventArgs)"/> to invoke <see cref="Input.ICommand.Execute(object?)"/> if the context allows.
    /// </summary>
    /// <param name="e">An empty <see cref="EventArgs"/> instance.</param>
    protected virtual void OnRequestCommandExecute(EventArgs e) =>
        ICommandBindingTargetProvider.RequestCommandExecute(this);

    // Called by the CommandProviderManager's command handling logic.
    void ICommandBindingTargetProvider.RaiseCommandChanged(EventArgs e) => OnCommandChanged(e);

    // Called by the CommandProviderManager's command handling logic.
    void ICommandBindingTargetProvider.RaiseCommandCanExecuteChanged(EventArgs e) => OnCommandCanExecuteChanged(e);

    private void OnFrameChanged(object? o, EventArgs e)
    {
        if (Disposing || IsDisposed)
        {
            return;
        }

        if (IsHandleCreated && InvokeRequired)
        {
            BeginInvoke(OnFrameChanged, [o, e]);
            return;
        }

        Invalidate();
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        base.OnEnabledChanged(e);
        Animate();

        if (!Enabled)
        {
            // disabled button is always "up"
            SetFlag(FlagMouseDown, false);
            SetFlag(FlagMouseOver, false);
            Invalidate();
        }
    }

    protected override void OnTextChanged(EventArgs e)
    {
        using (LayoutTransaction.CreateTransactionIf(AutoSize, ParentInternal, this, PropertyNames.Text))
        {
            base.OnTextChanged(e);
            Invalidate();
        }

        if (IsAccessibilityObjectCreated)
        {
            using var textVariant = (VARIANT)Text;
            AccessibilityObject.RaiseAutomationPropertyChangedEvent(UIA_PROPERTY_ID.UIA_NamePropertyId, textVariant, textVariant);
        }
    }

    /// <summary>
    ///  Raises the <see cref="OnKeyDown"/> event.
    /// </summary>
    protected override void OnKeyDown(KeyEventArgs kevent)
    {
        if (kevent.KeyData == Keys.Space)
        {
            if (!GetFlag(FlagMouseDown))
            {
                SetFlag(FlagMouseDown, true);
                // It looks like none of the "SPACE" key downs generate the BM_SETSTATE.
                // This causes to not draw the focus rectangle inside the button and also
                // not paint the button as "un-depressed".
                if (!OwnerDraw)
                {
                    PInvoke.SendMessage(this, PInvoke.BM_SETSTATE, (WPARAM)(BOOL)true);
                }

                Invalidate(DownChangeRectangle);
            }

            kevent.Handled = true;
        }

        // Call base last, so if it invokes any listeners that disable the button, we don't have to recheck.
        base.OnKeyDown(kevent);
    }

    /// <summary>
    ///  Raises the <see cref="OnKeyUp"/> event.
    /// </summary>
    protected override void OnKeyUp(KeyEventArgs kevent)
    {
        if (GetFlag(FlagMouseDown) && !ValidationCancelled)
        {
            if (OwnerDraw)
            {
                ResetFlagsandPaint();
            }
            else
            {
                SetFlag(FlagMousePressed, false);
                SetFlag(FlagMouseDown, false);
                PInvoke.SendMessage(this, PInvoke.BM_SETSTATE, (WPARAM)(BOOL)false);
            }

            // Breaking change: specifically filter out Keys.Enter and Keys.Space as the only
            // two keystrokes to execute OnClick.
            if (kevent.KeyCode is Keys.Enter or Keys.Space)
            {
                OnClick(EventArgs.Empty);
            }

            kevent.Handled = true;
        }

        // Call base last, so if it invokes any listeners that disable the button, we don't have to recheck.
        base.OnKeyUp(kevent);
    }

    /// <summary>
    ///  Raises the <see cref="OnPaint"/> event.
    /// </summary>
    protected override void OnPaint(PaintEventArgs pevent)
    {
        if (AutoEllipsis)
        {
            Size preferredSize = PreferredSize;
            ShowToolTip = (ClientRectangle.Width < preferredSize.Width || ClientRectangle.Height < preferredSize.Height);
        }
        else
        {
            ShowToolTip = false;
        }

        if (GetStyle(ControlStyles.UserPaint))
        {
            Animate();
            ImageAnimator.UpdateFrames(Image);

            PaintControl(pevent);
        }

        base.OnPaint(pevent);
    }

    protected override void OnParentChanged(EventArgs e)
    {
        base.OnParentChanged(e);
        Animate();
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
        base.OnVisibleChanged(e);
        Animate();
    }

    private void RaiseEvent(object key, EventArgs e) => ((EventHandler?)Events[key])?.Invoke(this, e);

    private void ResetImage() => Image = null;

    private void SetFlag(int flag, bool value)
    {
        bool oldValue = ((_state & flag) != 0);

        if (value)
        {
            _state |= flag;
        }
        else
        {
            _state &= ~flag;
        }

        if (OwnerDraw && (flag & FlagMouseDown) != 0 && value != oldValue)
        {
            AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);
        }
    }

    private bool ShouldSerializeImage() => _image is not null;

    private void UpdateOwnerDraw()
    {
        if (OwnerDraw != GetStyle(ControlStyles.UserPaint))
        {
            SetStyle(ControlStyles.UserMouse | ControlStyles.UserPaint, OwnerDraw);
            RecreateHandle();
        }
    }

    /// <summary>
    ///  Determines whether to use compatible text rendering engine (GDI+) or not (GDI).
    /// </summary>
    [DefaultValue(false)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.UseCompatibleTextRenderingDescr))]
    public bool UseCompatibleTextRendering
    {
        get => UseCompatibleTextRenderingInternal;
        set => UseCompatibleTextRenderingInternal = value;
    }

    internal override bool SupportsUseCompatibleTextRendering => true;

    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.ButtonUseVisualStyleBackColorDescr))]
    public bool UseVisualStyleBackColor
    {
        get => (_isEnableVisualStyleBackgroundSet || (RawBackColor.IsEmpty && (BackColor == Application.SystemColors.Control)))
            && _enableVisualStyleBackground;
        set
        {
            if (_isEnableVisualStyleBackgroundSet && value == _enableVisualStyleBackground)
            {
                return;
            }

            _isEnableVisualStyleBackgroundSet = true;
            _enableVisualStyleBackground = value;
            Invalidate();
        }
    }

    private void ResetUseVisualStyleBackColor()
    {
        _isEnableVisualStyleBackgroundSet = false;
        _enableVisualStyleBackground = true;
        Invalidate();
    }

    private bool ShouldSerializeUseVisualStyleBackColor() => _isEnableVisualStyleBackgroundSet;

    protected override void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            // We don't respect this because the code below eats BM_SETSTATE.
            // So we just invoke the click.
            case PInvoke.BM_CLICK:
                if (this is IButtonControl control)
                {
                    control.PerformClick();
                }
                else
                {
                    OnClick(EventArgs.Empty);
                }

                return;
        }

        if (OwnerDraw)
        {
            switch (m.MsgInternal)
            {
                case PInvoke.BM_SETSTATE:
                    // Ignore BM_SETSTATE - Windows gets confused and paints things,
                    // even though we are owner draw.
                    break;

                case PInvoke.WM_KILLFOCUS:
                case PInvoke.WM_CANCELMODE:
                case PInvoke.WM_CAPTURECHANGED:
                    if (!GetFlag(FlagInButtonUp) && GetFlag(FlagMousePressed))
                    {
                        SetFlag(FlagMousePressed, false);

                        if (GetFlag(FlagMouseDown))
                        {
                            SetFlag(FlagMouseDown, false);
                            Invalidate(DownChangeRectangle);
                        }
                    }

                    base.WndProc(ref m);
                    break;

                case PInvoke.WM_LBUTTONUP:
                case PInvoke.WM_MBUTTONUP:
                case PInvoke.WM_RBUTTONUP:
                    try
                    {
                        SetFlag(FlagInButtonUp, true);
                        base.WndProc(ref m);
                    }
                    finally
                    {
                        SetFlag(FlagInButtonUp, false);
                    }

                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }
        else
        {
            switch (m.MsgInternal)
            {
                case MessageId.WM_REFLECT_COMMAND:
                    if (m.WParamInternal.HIWORD == PInvoke.BN_CLICKED && !ValidationCancelled)
                    {
                        OnClick(EventArgs.Empty);
                    }

                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}
