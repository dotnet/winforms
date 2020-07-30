// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.ButtonInternal;
using System.Windows.Forms.Layout;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Implements the basic functionality required by a button control.
    /// </summary>
    [Designer("System.Windows.Forms.Design.ButtonBaseDesigner, " + AssemblyRef.SystemDesign)]
    public abstract partial class ButtonBase : Control
    {
        private FlatStyle _flatStyle = FlatStyle.Standard;
        private ContentAlignment _imageAlign = ContentAlignment.MiddleCenter;
        private ContentAlignment _textAlign = ContentAlignment.MiddleCenter;
        private TextImageRelation _textImageRelation = TextImageRelation.Overlay;
        private readonly ImageList.Indexer _imageIndex = new ImageList.Indexer();
        private FlatButtonAppearance _flatAppearance;
        private ImageList _imageList;
        private Image _image;

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

        private ToolTip _textToolTip;

        // This allows the user to disable visual styles for the button so that it inherits its background color
        private bool _enableVisualStyleBackground = true;

        private bool _isEnableVisualStyleBackgroundSet;

        /// <summary>
        ///  Initializes a new instance of the <see cref='ButtonBase'/> class.
        /// </summary>
        protected ButtonBase()
        {
            // If Button doesn't want double-clicks, we should introduce a StandardDoubleClick style.
            // Checkboxes probably want double-click's, and RadioButtons certainly do
            // (useful e.g. on a Wizard).
            SetStyle(ControlStyles.SupportsTransparentBackColor |
                      ControlStyles.Opaque |
                      ControlStyles.ResizeRedraw |
                      ControlStyles.OptimizedDoubleBuffer |
                      ControlStyles.CacheText | // We gain about 2% in painting by avoiding extra GetWindowText calls
                      ControlStyles.StandardClick,
                      true);
            // this class overrides GetPreferredSizeCore, let Control automatically cache the result
            SetExtendedState(ExtendedStates.UserPreferredSizeCache, true);

            SetStyle(ControlStyles.UserMouse |
                     ControlStyles.UserPaint, OwnerDraw);
            SetFlag(FlagUseMnemonic, true);
            SetFlag(FlagShowToolTip, false);
        }

        /// <summary>
        ///  This property controls the activation handling of bleedover for the text that
        ///  extends beyond the width of the button.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [SRDescription(nameof(SR.ButtonAutoEllipsisDescr))]
        public bool AutoEllipsis
        {
            get
            {
                return GetFlag(FlagAutoEllipsis);
            }

            set
            {
                if (value == AutoEllipsis)
                {
                    return;
                }

                SetFlag(FlagAutoEllipsis, value);
                if (value)
                {
                    if (_textToolTip is null)
                    {
                        _textToolTip = new ToolTip();
                    }
                }
                Invalidate();
            }
        }

        /// <summary>
        ///  Indicates whether the control is automatically resized to fit its contents
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
                //don't show ellipsis if the control is autosized
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
        new public event EventHandler AutoSizeChanged
        {
            add => base.AutoSizeChanged += value;
            remove => base.AutoSizeChanged -= value;
        }

        /// <summary>
        ///  The background color of this control. This is an ambient property and
        ///  will always return a non-null value.
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
                        PropertyDescriptor pd = TypeDescriptor.GetProperties(this)["UseVisualStyleBackColor"];
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
        ///  Deriving classes can override this to configure a default size for their control.
        ///  This is more efficient than setting the size in the control's constructor.
        /// </summary>
        protected override Size DefaultSize
        {
            get
            {
                return new Size(75, 23);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (!OwnerDraw)
                {
                    cp.ExStyle &= ~(int)User32.WS_EX.RIGHT;   // WS_EX_RIGHT overrides the BS_XXXX alignment styles

                    cp.Style |= (int)User32.BS.MULTILINE;

                    if (IsDefault)
                    {
                        cp.Style |= (int)User32.BS.DEFPUSHBUTTON;
                    }

                    ContentAlignment align = RtlTranslateContent(TextAlign);

                    if ((int)(align & WindowsFormsUtils.AnyLeftAlign) != 0)
                    {
                        cp.Style |= (int)User32.BS.LEFT;
                    }
                    else if ((int)(align & WindowsFormsUtils.AnyRightAlign) != 0)
                    {
                        cp.Style |= (int)User32.BS.RIGHT;
                    }
                    else
                    {
                        cp.Style |= (int)User32.BS.CENTER;
                    }
                    if ((int)(align & WindowsFormsUtils.AnyTopAlign) != 0)
                    {
                        cp.Style |= (int)User32.BS.TOP;
                    }
                    else if ((int)(align & WindowsFormsUtils.AnyBottomAlign) != 0)
                    {
                        cp.Style |= (int)User32.BS.BOTTOM;
                    }
                    else
                    {
                        cp.Style |= (int)User32.BS.VCENTER;
                    }
                }
                return cp;
            }
        }

        protected override ImeMode DefaultImeMode
        {
            get
            {
                return ImeMode.Disable;
            }
        }

        protected internal bool IsDefault
        {
            get
            {
                return GetFlag(FlagIsDefault);
            }
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
        ///  Gets or
        ///  sets
        ///  the flat style appearance of the button control.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(FlatStyle.Standard)]
        [Localizable(true)]
        [SRDescription(nameof(SR.ButtonFlatStyleDescr))]
        public FlatStyle FlatStyle
        {
            get
            {
                return _flatStyle;
            }
            set
            {
                if (value == FlatStyle)
                {
                    return;
                }

                if (!ClientUtils.IsEnumValid(value, (int)value, (int)FlatStyle.Flat, (int)FlatStyle.System))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(FlatStyle));
                }
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
        public FlatButtonAppearance FlatAppearance
        {
            get
            {
                if (_flatAppearance is null)
                {
                    _flatAppearance = new FlatButtonAppearance(this);
                }

                return _flatAppearance;
            }
        }

        /// <summary>
        ///  Gets or sets the image
        ///  that is displayed on a button control.
        /// </summary>
        [SRDescription(nameof(SR.ButtonImageDescr))]
        [Localizable(true)]
        [SRCategory(nameof(SR.CatAppearance))]
        public Image Image
        {
            get
            {
                if (_image is null && _imageList != null)
                {
                    int actualIndex = _imageIndex.ActualIndex;

                    // Pre-whidbey we used to use ImageIndex rather than ImageIndexer.ActualIndex.
                    // ImageIndex clamps to the length of the image list.  We need to replicate
                    // this logic here for backwards compatibility.
                    // We do not bake this into ImageIndexer because different controls
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
                if (_image != null)
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
            get
            {
                return _imageAlign;
            }
            set
            {
                if (!WindowsFormsUtils.EnumValidator.IsValidContentAlignment(value))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ContentAlignment));
                }
                if (value != _imageAlign)
                {
                    _imageAlign = value;
                    LayoutTransaction.DoLayoutIf(AutoSize, ParentInternal, this, PropertyNames.ImageAlign);
                    Invalidate();
                }
            }
        }

        /// <summary>
        ///  Gets or sets the image list index value of the image
        ///  displayed on the button control.
        /// </summary>
        [TypeConverter(typeof(ImageIndexConverter))]
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [Localizable(true)]
        [DefaultValue(ImageList.Indexer.DefaultIndex)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [SRDescription(nameof(SR.ButtonImageIndexDescr))]
        [SRCategory(nameof(SR.CatAppearance))]
        public int ImageIndex
        {
            get
            {
                if (_imageIndex.Index != ImageList.Indexer.DefaultIndex && _imageList != null && _imageIndex.Index >= _imageList.Images.Count)
                {
                    return _imageList.Images.Count - 1;
                }
                return _imageIndex.Index;
            }
            set
            {
                if (value < ImageList.Indexer.DefaultIndex)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(ImageIndex), value, ImageList.Indexer.DefaultIndex));
                }

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
        ///  Gets or sets the image list index key of the image
        ///  displayed on the button control.  Note - setting this unsets the ImageIndex
        /// </summary>
        [TypeConverter(typeof(ImageKeyConverter))]
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [Localizable(true)]
        [DefaultValue(ImageList.Indexer.DefaultKey)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [SRDescription(nameof(SR.ButtonImageIndexDescr))]
        [SRCategory(nameof(SR.CatAppearance))]
        public string ImageKey
        {
            get
            {
                return _imageIndex.Key;
            }
            set
            {
                if (value == _imageIndex.Key && !string.Equals(value, ImageList.Indexer.DefaultKey))
                {
                    return;
                }

                if (value != null)
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
        ///  Gets or sets the <see cref='Forms.ImageList'/> that contains the <see cref='Drawing.Image'/> displayed on a button control.
        /// </summary>
        [DefaultValue(null)]
        [SRDescription(nameof(SR.ButtonImageListDescr))]
        [RefreshProperties(RefreshProperties.Repaint)]
        [SRCategory(nameof(SR.CatAppearance))]
        public ImageList ImageList
        {
            get
            {
                return _imageList;
            }
            set
            {
                if (value == _imageList)
                {
                    return;
                }

                EventHandler recreateHandler = new EventHandler(ImageListRecreateHandle);
                EventHandler disposedHandler = new EventHandler(DetachImageList);

                // Detach old event handlers
                //
                if (_imageList != null)
                {
                    _imageList.RecreateHandle -= recreateHandler;
                    _imageList.Disposed -= disposedHandler;
                }

                // Make sure we don't have an Image as well as an ImageList
                //
                if (value != null)
                {
                    _image = null; // Image.set calls ImageList = null
                }

                _imageList = value;
                _imageIndex.ImageList = value;

                // Wire up new event handlers
                //
                if (value != null)
                {
                    value.RecreateHandle += recreateHandler;
                    value.Disposed += disposedHandler;
                }

                Invalidate();
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public ImeMode ImeMode
        {
            get => base.ImeMode;
            set => base.ImeMode = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler ImeModeChanged
        {
            add => base.ImeModeChanged += value;
            remove => base.ImeModeChanged -= value;
        }

        /// <summary>
        ///  Specifies whether the control is willing to process mnemonics when hosted in an container ActiveX (Ax Sourcing).
        /// </summary>
        internal override bool IsMnemonicsListenerAxSourced
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///  The area of the button encompassing any changes between the button's
        ///  resting appearance and its appearance when the mouse is over it.
        ///  Consider overriding this property if you override any painting methods,
        ///  or your button may not paint correctly or may have flicker. Returning
        ///  ClientRectangle is safe for correct painting but may still cause flicker.
        /// </summary>
        internal virtual Rectangle OverChangeRectangle
        {
            get
            {
                if (FlatStyle == FlatStyle.Standard)
                {
                    // this Rectangle will cause no Invalidation
                    // can't use Rectangle.Empty because it will cause Invalidate(ClientRectangle)
                    return new Rectangle(-1, -1, 1, 1);
                }
                else
                {
                    return ClientRectangle;
                }
            }
        }

        internal bool OwnerDraw
        {
            get
            {
                return FlatStyle != FlatStyle.System;
            }
        }

        /// <summary>
        ///  The area of the button encompassing any changes between the button's
        ///  appearance when the mouse is over it but not pressed and when it is pressed.
        ///  Consider overriding this property if you override any painting methods,
        ///  or your button may not paint correctly or may have flicker. Returning
        ///  ClientRectangle is safe for correct painting but may still cause flicker.
        /// </summary>
        internal virtual Rectangle DownChangeRectangle
        {
            get
            {
                return ClientRectangle;
            }
        }

        internal bool MouseIsPressed
        {
            get
            {
                return GetFlag(FlagMousePressed);
            }
        }

        // a "smart" version of mouseDown for Appearance.Button CheckBoxes & RadioButtons
        // for these, instead of being based on the actual mouse state, it's based on the appropriate button state
        internal bool MouseIsDown
        {
            get
            {
                return GetFlag(FlagMouseDown);
            }
        }

        // a "smart" version of mouseOver for Appearance.Button CheckBoxes & RadioButtons
        // for these, instead of being based on the actual mouse state, it's based on the appropriate button state
        internal bool MouseIsOver
        {
            get
            {
                return GetFlag(FlagMouseOver);
            }
        }

        /// <summary>
        ///  Indicates whether the tooltip should be shown
        /// </summary>
        internal bool ShowToolTip
        {
            get
            {
                return GetFlag(FlagShowToolTip);
            }
            set
            {
                SetFlag(FlagShowToolTip, value);
            }
        }

        [Editor("System.ComponentModel.Design.MultilineStringEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
            SettingsBindable(true)]
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
            get
            {
                return _textAlign;
            }
            set
            {
                if (!WindowsFormsUtils.EnumValidator.IsValidContentAlignment(value))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ContentAlignment));
                }
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
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)TextImageRelation.Overlay, (int)TextImageRelation.TextBeforeImage, 1))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(TextImageRelation));
                }

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
            get
            {
                return GetFlag(FlagUseMnemonic);
            }

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

        private void Animate()
        {
            Animate(!DesignMode && Visible && Enabled && ParentInternal != null);
        }

        private void StopAnimate()
        {
            Animate(false);
        }

        private void Animate(bool animate)
        {
            if (animate != GetFlag(FlagCurrentlyAnimating))
            {
                if (animate)
                {
                    if (_image != null)
                    {
                        ImageAnimator.Animate(_image, new EventHandler(OnFrameChanged));
                        SetFlag(FlagCurrentlyAnimating, animate);
                    }
                }
                else
                {
                    if (_image != null)
                    {
                        ImageAnimator.StopAnimate(_image, new EventHandler(OnFrameChanged));
                        SetFlag(FlagCurrentlyAnimating, animate);
                    }
                }
            }
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new ButtonBaseAccessibleObject(this);
        }

        private void DetachImageList(object sender, EventArgs e)
        {
            ImageList = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopAnimate();
                if (_imageList != null)
                {
                    _imageList.Disposed -= new EventHandler(DetachImageList);
                }
                //Dipose the tooltip if one present..
                if (_textToolTip != null)
                {
                    _textToolTip.Dispose();
                    _textToolTip = null;
                }
            }
            base.Dispose(disposing);
        }

        private bool GetFlag(int flag)
        {
            return ((_state & flag) == flag);
        }

        private void ImageListRecreateHandle(object sender, EventArgs e)
        {
            if (IsHandleCreated)
            {
                Invalidate();
            }
        }

        /// <summary>
        ///  Raises the <see cref='OnGotFocus'/> event.
        /// </summary>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        /// <summary>
        ///  Raises the <see cref='OnLostFocus'/> event.
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
        ///  Raises the <see cref='Control.OnMouseEnter'/> event.
        /// </summary>
        protected override void OnMouseEnter(EventArgs eventargs)
        {
            SetFlag(FlagMouseOver, true);
            Invalidate();
            if (!DesignMode && AutoEllipsis && ShowToolTip && _textToolTip != null)
            {
                _textToolTip.Show(WindowsFormsUtils.TextWithoutMnemonics(Text), this);
            }
            // call base last, so if it invokes any listeners that disable the button, we
            // don't have to recheck
            base.OnMouseEnter(eventargs);
        }

        /// <summary>
        ///  Raises the <see cref='Control.OnMouseLeave'/> event.
        /// </summary>
        protected override void OnMouseLeave(EventArgs eventargs)
        {
            SetFlag(FlagMouseOver, false);
            if (_textToolTip != null)
            {
                _textToolTip.Hide(this);
            }
            Invalidate();
            // call base last, so if it invokes any listeners that disable the button, we
            // don't have to recheck
            base.OnMouseLeave(eventargs);
        }

        /// <summary>
        ///  Raises the <see cref='Control.OnMouseMove'/> event.
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
        ///  Raises the <see cref='Control.OnMouseDown'/> event.
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
        ///  Raises the <see cref='OnMouseUp'/> event.
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
            // TableLayoutPanel passes width = 1 to get the minimum autosize width, since Buttons word-break text
            // that width would be the size of the widest caracter in the text.  We need to make the proposed size
            // unbounded.
            // This is the same as what Label does.
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
            Size prefSize = Adapter.GetPreferredSizeCore(proposedConstraints);
            return LayoutUtils.UnionSizes(prefSize + Padding.Size, MinimumSize);
        }

        private ButtonBaseAdapter _adapter;
        private FlatStyle _cachedAdapterType;

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
                            Debug.Fail("Unsupported FlatStyle: '" + FlatStyle + '"');
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

        private void OnFrameChanged(object o, EventArgs e)
        {
            if (Disposing || IsDisposed)
            {
                return;
            }
            if (IsHandleCreated && InvokeRequired)
            {
                BeginInvoke(new EventHandler(OnFrameChanged), new object[] { o, e });
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
        }

        /// <summary>
        ///  Raises the <see cref='OnKeyDown'/> event.
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
                    //
                    if (!OwnerDraw)
                    {
                        User32.SendMessageW(this, (User32.WM)User32.BM.SETSTATE, PARAM.FromBool(true));
                    }
                    Invalidate(DownChangeRectangle);
                }
                kevent.Handled = true;
            }
            // call base last, so if it invokes any listeners that disable the button, we
            // don't have to recheck
            base.OnKeyDown(kevent);
        }

        /// <summary>
        ///  Raises the <see cref='OnKeyUp'/> event.
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
                    User32.SendMessageW(this, (User32.WM)User32.BM.SETSTATE, PARAM.FromBool(false));
                }
                // Breaking change: specifically filter out Keys.Enter and Keys.Space as the only
                // two keystrokes to execute OnClick.
                if (kevent.KeyCode == Keys.Enter || kevent.KeyCode == Keys.Space)
                {
                    OnClick(EventArgs.Empty);
                }
                kevent.Handled = true;
            }
            // call base last, so if it invokes any listeners that disable the button, we
            // don't have to recheck
            base.OnKeyUp(kevent);
        }

        /// <summary>
        ///  Raises the <see cref='OnPaint'/> event.
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

        private void ResetImage()
        {
            Image = null;
        }

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

        private bool ShouldSerializeImage()
        {
            return _image != null;
        }

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
            get => base.UseCompatibleTextRenderingInt;
            set => base.UseCompatibleTextRenderingInt = value;
        }

        /// <summary>
        ///  Determines whether the control supports rendering text using GDI+ and GDI.
        ///  This is provided for container controls to iterate through its children to set UseCompatibleTextRendering to the same
        ///  value if the child control supports it.
        /// </summary>
        internal override bool SupportsUseCompatibleTextRendering
        {
            get
            {
                return true;
            }
        }

        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ButtonUseVisualStyleBackColorDescr))]
        public bool UseVisualStyleBackColor
        {
            get
            {
                if (_isEnableVisualStyleBackgroundSet || ((RawBackColor.IsEmpty) && (BackColor == SystemColors.Control)))
                {
                    return _enableVisualStyleBackground;
                }
                else
                {
                    return false;
                }
            }
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

        private bool ShouldSerializeUseVisualStyleBackColor()
        {
            return _isEnableVisualStyleBackgroundSet;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                // We don't respect this because the code below eats BM_SETSTATE.
                // So we just invoke the click.
                case (int)User32.BM.CLICK:
                    if (this is IButtonControl)
                    {
                        ((IButtonControl)this).PerformClick();
                    }
                    else
                    {
                        OnClick(EventArgs.Empty);
                    }
                    return;
            }

            if (OwnerDraw)
            {
                switch (m.Msg)
                {
                    case (int)User32.BM.SETSTATE:
                        // Ignore BM_SETSTATE - Windows gets confused and paints things,
                        // even though we are ownerdraw.
                        break;

                    case (int)User32.WM.KILLFOCUS:
                    case (int)User32.WM.CANCELMODE:
                    case (int)User32.WM.CAPTURECHANGED:
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

                    case (int)User32.WM.LBUTTONUP:
                    case (int)User32.WM.MBUTTONUP:
                    case (int)User32.WM.RBUTTONUP:
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
                switch ((User32.WM)m.Msg)
                {
                    case User32.WM.REFLECT_COMMAND:
                        if (PARAM.HIWORD(m.WParam) == (int)User32.BN.CLICKED && !ValidationCancelled)
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
}
