// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms.Automation;
using System.Windows.Forms.Internal;
using System.Windows.Forms.Layout;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a standard Windows label.
    /// </summary>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    DefaultProperty(nameof(Text)),
    DefaultBindingProperty(nameof(Text)),
    Designer("System.Windows.Forms.Design.LabelDesigner, " + AssemblyRef.SystemDesign),
    ToolboxItem("System.Windows.Forms.Design.AutoSizeToolboxItem," + AssemblyRef.SystemDesign),
    SRDescription(nameof(SR.DescriptionLabel))
    ]
    // If not for FormatControl, we could inherit from ButtonBase and get foreground images for free.
    public class Label : Control, IAutomationLiveRegion
    {
        private static readonly object EVENT_TEXTALIGNCHANGED = new object();

        private static readonly BitVector32.Section StateUseMnemonic = BitVector32.CreateSection(1);
        private static readonly BitVector32.Section StateAutoSize = BitVector32.CreateSection(1, StateUseMnemonic);
        private static readonly BitVector32.Section StateAnimating = BitVector32.CreateSection(1, StateAutoSize);
        private static readonly BitVector32.Section StateFlatStyle = BitVector32.CreateSection((int)FlatStyle.System, StateAnimating);
        private static readonly BitVector32.Section StateBorderStyle = BitVector32.CreateSection((int)BorderStyle.Fixed3D, StateFlatStyle);
        private static readonly BitVector32.Section StateAutoEllipsis = BitVector32.CreateSection(1, StateBorderStyle);

        private static readonly int PropImageList = PropertyStore.CreateKey();
        private static readonly int PropImage = PropertyStore.CreateKey();

        private static readonly int PropTextAlign = PropertyStore.CreateKey();
        private static readonly int PropImageAlign = PropertyStore.CreateKey();
        private static readonly int PropImageIndex = PropertyStore.CreateKey();

        ///////////////////////////////////////////////////////////////////////
        // Label per instance members
        //
        // Note: Do not add anything to this list unless absolutely neccessary.
        //
        // Begin Members {

        // List of properties that are generally set, so we keep them directly on
        // Label.
        //

        BitVector32 labelState = new BitVector32();
        int requestedHeight;
        int requestedWidth;
        LayoutUtils.MeasureTextCache textMeasurementCache;

        //Tooltip is shown only if the Text in the Label is cut.
        internal bool showToolTip = false;
        ToolTip textToolTip;
        // This bool suggests that the User has added a toolTip to this label
        // In such a case we should not show the AutoEllipsis tooltip.
        bool controlToolTip = false;
        AutomationLiveSetting liveSetting;

        // } End Members
        ///////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Initializes a new instance of the <see cref='Label'/> class.
        /// </summary>
        public Label()
        : base()
        {
            // this class overrides GetPreferredSizeCore, let Control automatically cache the result
            SetState2(STATE2_USEPREFERREDSIZECACHE, true);

            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.OptimizedDoubleBuffer, IsOwnerDraw());

            SetStyle(ControlStyles.FixedHeight |
                     ControlStyles.Selectable, false);

            SetStyle(ControlStyles.ResizeRedraw, true);

            CommonProperties.SetSelfAutoSizeInDefaultLayout(this, true);

            labelState[StateFlatStyle] = (int)FlatStyle.Standard;
            labelState[StateUseMnemonic] = 1;
            labelState[StateBorderStyle] = (int)BorderStyle.None;

            TabStop = false;
            requestedHeight = Height;
            requestedWidth = Width;
        }

        /// <summary>
        ///  Indicates whether the control is automatically resized
        ///  to fit its contents.
        /// </summary>
        [
        SRCategory(nameof(SR.CatLayout)),
        DefaultValue(false),
        RefreshProperties(RefreshProperties.All),
        Localizable(true),
        SRDescription(nameof(SR.LabelAutoSizeDescr)),
        Browsable(true),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
        ]
        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                if (AutoSize != value)
                {
                    base.AutoSize = value;
                    AdjustSize();
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnAutoSizeChangedDescr))]
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        new public event EventHandler AutoSizeChanged
        {
            add => base.AutoSizeChanged += value;
            remove => base.AutoSizeChanged -= value;
        }

        /// <summary>
        ///  This property controls the activation handling of bleedover for the text that
        ///  extends beyond the width of the label.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        Browsable(true),
        EditorBrowsable(EditorBrowsableState.Always),
        SRDescription(nameof(SR.LabelAutoEllipsisDescr))
        ]
        public bool AutoEllipsis
        {
            get
            {
                return labelState[StateAutoEllipsis] != 0;
            }

            set
            {
                if (AutoEllipsis != value)
                {
                    labelState[StateAutoEllipsis] = value ? 1 : 0;
                    MeasureTextCache.InvalidateCache();

                    OnAutoEllipsisChanged(/*EventArgs.Empty*/);

                    if (value)
                    {
                        if (textToolTip == null)
                        {
                            textToolTip = new ToolTip();
                        }
                    }

                    if (ParentInternal != null)
                    {
                        LayoutTransaction.DoLayoutIf(AutoSize, ParentInternal, this, PropertyNames.AutoEllipsis);
                    }
                    Invalidate();
                }
            }
        }

        /// <summary>
        ///  Gets or sets the image rendered on the background of the control.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.LabelBackgroundImageDescr))
        ]
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
            set
            {
                base.BackgroundImage = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageChanged
        {
            add => base.BackgroundImageChanged += value;
            remove => base.BackgroundImageChanged -= value;
        }

        /// <summary>
        ///  Gets or sets the image layout for the background of the control.
        /// </summary>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Never)
        ]
        public override ImageLayout BackgroundImageLayout
        {
            get
            {
                return base.BackgroundImageLayout;
            }
            set
            {
                base.BackgroundImageLayout = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageLayoutChanged
        {
            add => base.BackgroundImageLayoutChanged += value;
            remove => base.BackgroundImageLayoutChanged -= value;
        }

        /// <summary>
        ///  Gets or sets the border style for the control.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(BorderStyle.None),
        DispId(NativeMethods.ActiveX.DISPID_BORDERSTYLE),
        SRDescription(nameof(SR.LabelBorderDescr))
        ]
        public virtual BorderStyle BorderStyle
        {
            get
            {
                return (BorderStyle)labelState[StateBorderStyle];
            }
            set
            {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)BorderStyle.None, (int)BorderStyle.Fixed3D))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(BorderStyle));
                }

                if (BorderStyle != value)
                {
                    labelState[StateBorderStyle] = (int)value;
                    if (ParentInternal != null)
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
        internal virtual bool CanUseTextRenderer
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///  Overrides Control.  A Label is a Win32 STATIC control, which we setup here.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassName = "STATIC";

                if (OwnerDraw)
                {
                    // An unfortunate side effect of this style is Windows sends us WM_DRAWITEM
                    // messages instead of WM_PAINT, but since Windows insists on repainting
                    // *without* a WM_PAINT after SetWindowText, I don't see much choice.
                    cp.Style |= NativeMethods.SS_OWNERDRAW;

                    // Since we're owner draw, I don't see any point in setting the
                    // SS_CENTER/SS_RIGHT styles.
                    //
                    cp.ExStyle &= ~NativeMethods.WS_EX_RIGHT;   // WS_EX_RIGHT overrides the SS_XXXX alignment styles
                }

                if (!OwnerDraw)
                {
                    switch (TextAlign)
                    {
                        case ContentAlignment.TopLeft:
                        case ContentAlignment.MiddleLeft:
                        case ContentAlignment.BottomLeft:
                            cp.Style |= NativeMethods.SS_LEFT;
                            break;

                        case ContentAlignment.TopRight:
                        case ContentAlignment.MiddleRight:
                        case ContentAlignment.BottomRight:
                            cp.Style |= NativeMethods.SS_RIGHT;
                            break;

                        case ContentAlignment.TopCenter:
                        case ContentAlignment.MiddleCenter:
                        case ContentAlignment.BottomCenter:
                            cp.Style |= NativeMethods.SS_CENTER;
                            break;
                    }
                }
                else
                {
                    cp.Style |= NativeMethods.SS_LEFT;
                }

                switch (BorderStyle)
                {
                    case BorderStyle.FixedSingle:
                        cp.Style |= NativeMethods.WS_BORDER;
                        break;
                    case BorderStyle.Fixed3D:
                        cp.Style |= NativeMethods.SS_SUNKEN;
                        break;
                }

                if (!UseMnemonic)
                {
                    cp.Style |= NativeMethods.SS_NOPREFIX;
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

        protected override Padding DefaultMargin
        {
            get
            {
                return new Padding(3, 0, 3, 0);
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
                return new Size(100, AutoSize ? PreferredHeight : 23);
            }
        }

        [
            SRCategory(nameof(SR.CatAppearance)),
            DefaultValue(FlatStyle.Standard),
            SRDescription(nameof(SR.ButtonFlatStyleDescr))
        ]
        public FlatStyle FlatStyle
        {
            get
            {
                return (FlatStyle)labelState[StateFlatStyle];
            }
            set
            {
                //valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)FlatStyle.Flat, (int)FlatStyle.System))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(FlatStyle));
                }

                if (labelState[StateFlatStyle] != (int)value)
                {
                    bool needRecreate = (labelState[StateFlatStyle] == (int)FlatStyle.System) || (value == FlatStyle.System);

                    labelState[StateFlatStyle] = (int)value;

                    SetStyle(ControlStyles.UserPaint
                             | ControlStyles.SupportsTransparentBackColor
                             | ControlStyles.OptimizedDoubleBuffer, OwnerDraw);

                    if (needRecreate)
                    {
                        // this will clear the preferred size cache - it's OK if the parent is null - it would be a NOP.
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
        }

        /// <summary>
        ///  Gets or sets the image that is displayed on a <see cref='Label'/>.
        /// </summary>
        [
        Localizable(true),
        SRDescription(nameof(SR.ButtonImageDescr)),
        SRCategory(nameof(SR.CatAppearance))
        ]
        public Image Image
        {
            get
            {
                Image image = (Image)Properties.GetObject(PropImage);

                if (image == null && ImageList != null && ImageIndexer.ActualIndex >= 0)
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
                if (Image != value)
                {
                    StopAnimate();

                    Properties.SetObject(PropImage, value);
                    if (value != null)
                    {
                        ImageIndex = -1;
                        ImageList = null;
                    }

                    // Hook up the frame changed event
                    //
                    Animate();
                    Invalidate();
                }
            }
        }

        /// <summary>
        ///  Gets or sets the index value of the images displayed on the
        ///  <see cref='Label'/>.
        /// </summary>
        [
        TypeConverter(typeof(ImageIndexConverter)),
        Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        DefaultValue(-1),
        Localizable(true),
        RefreshProperties(RefreshProperties.Repaint),
        SRDescription(nameof(SR.ButtonImageIndexDescr)),
        SRCategory(nameof(SR.CatAppearance))
        ]
        public int ImageIndex
        {
            get
            {

                if (ImageIndexer != null)
                {
                    int index = ImageIndexer.Index;

                    if (ImageList != null && (index >= ImageList.Images.Count))
                    {
                        return ImageList.Images.Count - 1;
                    }
                    return index;

                }
                return -1;
            }
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(ImageIndex), value, -1));
                }
                if (ImageIndex != value)
                {
                    if (value != -1)
                    {
                        // Image.set calls ImageIndex = -1
                        Properties.SetObject(PropImage, null);
                    }

                    ImageIndexer.Index = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        ///  Gets or sets the key accessor for the image list.  This specifies the image
	    ///	  from the image list to display on
        ///  <see cref='Label'/>.
        /// </summary>
        [
        TypeConverter(typeof(ImageKeyConverter)),
        Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        DefaultValue(""),
        Localizable(true),
        RefreshProperties(RefreshProperties.Repaint),
        SRDescription(nameof(SR.ButtonImageIndexDescr)),
        SRCategory(nameof(SR.CatAppearance))
        ]
        public string ImageKey
        {
            get
            {

                if (ImageIndexer != null)
                {
                    return ImageIndexer.Key;
                }
                return null;
            }
            set
            {
                if (ImageKey != value)
                {

                    // Image.set calls ImageIndex = -1
                    Properties.SetObject(PropImage, null);

                    ImageIndexer.Key = value;
                    Invalidate();
                }
            }
        }

        internal LabelImageIndexer ImageIndexer
        {
            get
            {
                // Demand create the ImageIndexer property
                if ((!(Properties.GetObject(PropImageIndex, out bool found) is LabelImageIndexer imageIndexer)) || (!found))
                {
                    imageIndexer = new LabelImageIndexer(this);
                    ImageIndexer = imageIndexer;
                }

                return imageIndexer;
            }
            set
            {
                Properties.SetObject(PropImageIndex, value);
            }

        }

        /// <summary>
        ///  Gets or sets the images displayed in a <see cref='Label'/>.
        /// </summary>
        [
        DefaultValue(null),
        SRDescription(nameof(SR.ButtonImageListDescr)),
        RefreshProperties(RefreshProperties.Repaint),
        SRCategory(nameof(SR.CatAppearance))
        ]
        public ImageList ImageList
        {
            get
            {
                return (ImageList)Properties.GetObject(PropImageList);
            }
            set
            {
                if (ImageList != value)
                {

                    EventHandler recreateHandler = new EventHandler(ImageListRecreateHandle);
                    EventHandler disposedHandler = new EventHandler(DetachImageList);

                    // Remove the previous imagelist handle recreate handler
                    //
                    ImageList imageList = ImageList;
                    if (imageList != null)
                    {
                        imageList.RecreateHandle -= recreateHandler;
                        imageList.Disposed -= disposedHandler;
                    }

                    // Make sure we don't have an Image as well as an ImageList
                    //
                    if (value != null)
                    {
                        Properties.SetObject(PropImage, null); // Image.set calls ImageList = null
                    }

                    Properties.SetObject(PropImageList, value);

                    // Add the new imagelist handle recreate handler
                    //
                    if (value != null)
                    {
                        value.RecreateHandle += recreateHandler;
                        value.Disposed += disposedHandler;
                    }

                    Invalidate();
                }
            }
        }

        /// <summary>
        ///  Gets or sets the alignment of the image on the <see cref='Label'/>.
        /// </summary>
        [
        DefaultValue(ContentAlignment.MiddleCenter),
        Localizable(true),
        SRDescription(nameof(SR.ButtonImageAlignDescr)),
        SRCategory(nameof(SR.CatAppearance))
        ]
        public ContentAlignment ImageAlign
        {
            get
            {
                int imageAlign = Properties.GetInteger(PropImageAlign, out bool found);
                if (found)
                {
                    return (ContentAlignment)imageAlign;
                }
                return ContentAlignment.MiddleCenter;
            }
            set
            {
                if (!WindowsFormsUtils.EnumValidator.IsValidContentAlignment(value))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ContentAlignment));
                }
                if (value != ImageAlign)
                {
                    Properties.SetInteger(PropImageAlign, (int)value);
                    LayoutTransaction.DoLayoutIf(AutoSize, ParentInternal, this, PropertyNames.ImageAlign);
                    Invalidate();
                }
            }
        }

        /// <summary>
        ///  Indicates the "politeness" level that a client should use
        ///  to notify the user of changes to the live region.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAccessibility)),
        DefaultValue(AutomationLiveSetting.Off),
        SRDescription(nameof(SR.LiveRegionAutomationLiveSettingDescr)),
        Browsable(true),
        EditorBrowsable(EditorBrowsableState.Always)
        ]
        public AutomationLiveSetting LiveSetting
        {
            get
            {
                return liveSetting;
            }
            set
            {
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)AutomationLiveSetting.Off, (int)AutomationLiveSetting.Assertive))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(AutomationLiveSetting));
                }
                liveSetting = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public ImeMode ImeMode
        {
            get
            {
                return base.ImeMode;
            }
            set
            {
                base.ImeMode = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler ImeModeChanged
        {
            add => base.ImeModeChanged += value;
            remove => base.ImeModeChanged -= value;
        }

        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event KeyEventHandler KeyUp
        {
            add => base.KeyUp += value;
            remove => base.KeyUp -= value;
        }

        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event KeyEventHandler KeyDown
        {
            add => base.KeyDown += value;
            remove => base.KeyDown -= value;
        }

        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event KeyPressEventHandler KeyPress
        {
            add => base.KeyPress += value;
            remove => base.KeyPress -= value;
        }

        internal LayoutUtils.MeasureTextCache MeasureTextCache
        {
            get
            {
                if (textMeasurementCache == null)
                {
                    textMeasurementCache = new LayoutUtils.MeasureTextCache();
                }
                return textMeasurementCache;
            }
        }

        internal virtual bool OwnerDraw
        {
            get
            {
                return IsOwnerDraw();
            }
        }

        /// <summary>
        ///  Gets the height of the control (in pixels), assuming a
        ///  single line of text is displayed.
        /// </summary>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.LabelPreferredHeightDescr))
        ]
        public virtual int PreferredHeight
        {
            get { return PreferredSize.Height; }
        }

        /// <summary>
        ///  Gets the width of the control (in pixels), assuming a single line
        ///  of text is displayed.
        /// </summary>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.LabelPreferredWidthDescr))
        ]
        public virtual int PreferredWidth
        {
            get { return PreferredSize.Width; }
        }

        /// <summary>
        ///  Indicates whether
        ///  the container control background is rendered on the <see cref='Label'/>.
        /// </summary>
        [Obsolete("This property has been deprecated. Use BackColor instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        virtual new protected bool RenderTransparent
        {
            get
            {
                return ((Control)this).RenderTransparent;
            }
            set
            {
            }
        }

        private bool SelfSizing
        {
            get
            {
                return CommonProperties.ShouldSelfSize(this);
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the user can tab to the
        ///  <see cref='Label'/>.
        /// </summary>
        [DefaultValue(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool TabStop
        {
            get
            {
                return base.TabStop;
            }
            set
            {
                base.TabStop = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TabStopChanged
        {
            add => base.TabStopChanged += value;
            remove => base.TabStopChanged -= value;
        }

        /// <summary>
        ///  Gets or sets the
        ///  horizontal alignment of the text in the control.
        /// </summary>
        [
        SRDescription(nameof(SR.LabelTextAlignDescr)),
        Localizable(true),
        DefaultValue(ContentAlignment.TopLeft),
        SRCategory(nameof(SR.CatAppearance))
        ]
        public virtual ContentAlignment TextAlign
        {
            get
            {
                int textAlign = Properties.GetInteger(PropTextAlign, out bool found);
                if (found)
                {
                    return (ContentAlignment)textAlign;
                }

                return ContentAlignment.TopLeft;
            }
            set
            {

                if (!WindowsFormsUtils.EnumValidator.IsValidContentAlignment(value))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ContentAlignment));
                }

                if (TextAlign != value)
                {
                    Properties.SetInteger(PropTextAlign, (int)value);
                    Invalidate();
                    //Change the TextAlignment for SystemDrawn Labels ....
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
        ///  this property just overides the base to pluck in the Multiline editor.
        /// </summary>
        [
        Editor("System.ComponentModel.Design.MultilineStringEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        SettingsBindable(true)
        ]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.LabelOnTextAlignChangedDescr))]
        public event EventHandler TextAlignChanged
        {
            add => Events.AddHandler(EVENT_TEXTALIGNCHANGED, value);

            remove => Events.RemoveHandler(EVENT_TEXTALIGNCHANGED, value);
        }

        /// <summary>
        ///  Determines whether to use compatible text rendering engine (GDI+) or not (GDI).
        /// </summary>
        [
        DefaultValue(false),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.UseCompatibleTextRenderingDescr))
        ]
        public bool UseCompatibleTextRendering
        {
            get
            {
                if (CanUseTextRenderer)
                {
                    return base.UseCompatibleTextRenderingInt;
                }

                // Use compat text rendering (GDI+).
                return true;
            }
            set
            {
                if (base.UseCompatibleTextRenderingInt != value)
                {
                    base.UseCompatibleTextRenderingInt = value;
                    AdjustSize();
                }
            }
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

        /// <summary>
        ///  Gets or sets a value indicating whether an ampersand (&amp;) included in the text of
        ///  the control.
        /// </summary>
        [
        SRDescription(nameof(SR.LabelUseMnemonicDescr)),
        DefaultValue(true),
        SRCategory(nameof(SR.CatAppearance))
        ]
        public bool UseMnemonic
        {
            get
            {
                return labelState[StateUseMnemonic] != 0;
            }

            set
            {

                if (UseMnemonic != value)
                {
                    labelState[StateUseMnemonic] = value ? 1 : 0;
                    MeasureTextCache.InvalidateCache();

                    // The size of the label need to be adjusted when the Mnemonic
                    // is set irrespective of auto-sizing
                    using (LayoutTransaction.CreateTransactionIf(AutoSize, ParentInternal, this, PropertyNames.Text))
                    {
                        AdjustSize();
                        Invalidate();
                    }

                    //set windowStyle directly instead of recreating handle to increase efficiency
                    if (IsHandleCreated)
                    {
                        int style = WindowStyle;
                        if (!UseMnemonic)
                        {

                            style |= NativeMethods.SS_NOPREFIX;
                        }
                        else
                        {
                            style &= ~NativeMethods.SS_NOPREFIX;
                        }
                        WindowStyle = style;
                    }
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
            //
            int saveHeight = requestedHeight;
            int saveWidth = requestedWidth;
            try
            {
                Size preferredSize = (AutoSize) ? PreferredSize : new Size(saveWidth, saveHeight);
                Size = preferredSize;
            }
            finally
            {
                requestedHeight = saveHeight;
                requestedWidth = saveWidth;
            }
        }

        internal void Animate()
        {
            Animate(!DesignMode && Visible && Enabled && ParentInternal != null);
        }

        internal void StopAnimate()
        {
            Animate(false);
        }

        private void Animate(bool animate)
        {
            bool currentlyAnimating = labelState[StateAnimating] != 0;
            if (animate != currentlyAnimating)
            {
                Image image = (Image)Properties.GetObject(PropImage);
                if (animate)
                {
                    if (image != null)
                    {
                        ImageAnimator.Animate(image, new EventHandler(OnFrameChanged));
                        labelState[StateAnimating] = animate ? 1 : 0;
                    }
                }
                else
                {
                    if (image != null)
                    {
                        ImageAnimator.StopAnimate(image, new EventHandler(OnFrameChanged));
                        labelState[StateAnimating] = animate ? 1 : 0;
                    }
                }
            }
        }

        protected Rectangle CalcImageRenderBounds(Image image, Rectangle r, ContentAlignment align)
        {
            Size pointImageSize = image.Size;

            int xLoc = r.X + 2;
            int yLoc = r.Y + 2;

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

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new LabelAccessibleObject(this);
        }

        /// <summary>
        ///  Get StringFormat object for rendering text using GDI+ (Graphics).
        /// </summary>
        internal virtual StringFormat CreateStringFormat()
        {
            return ControlPaint.CreateStringFormat(this, TextAlign, AutoEllipsis, UseMnemonic);
        }

        private TextFormatFlags CreateTextFormatFlags()
        {
            return CreateTextFormatFlags(Size - GetBordersAndPadding());
        }
        /// <summary>
        ///  Get TextFormatFlags flags for rendering text using GDI (TextRenderer).
        /// </summary>
        internal virtual TextFormatFlags CreateTextFormatFlags(Size constrainingSize)
        {
            // PREFERRED SIZE CACHING:
            // Please read if you're adding a new TextFormatFlag.
            // whenever something can change the TextFormatFlags used
            // MeasureTextCache.InvalidateCache() should be called so we can approprately clear.

            TextFormatFlags flags = ControlPaint.CreateTextFormatFlags(this, TextAlign, AutoEllipsis, UseMnemonic);

            // Remove WordBreak if the size is large enough to display all the text.
            if (!MeasureTextCache.TextRequiresWordBreak(Text, Font, constrainingSize, flags))
            {
                // The effect of the TextBoxControl flag is that in-word line breaking will occur if needed, this happens when AutoSize
                // is false and a one-word line still doesn't fit the binding box (width).  The other effect is that partially visible
                // lines are clipped; this is how GDI+ works by default.
                flags &= ~(TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
            }

            return flags;
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
                // Holding on to images and image list is a memory leak.
                if (ImageList != null)
                {
                    ImageList.Disposed -= new EventHandler(DetachImageList);
                    ImageList.RecreateHandle -= new EventHandler(ImageListRecreateHandle);
                    Properties.SetObject(PropImageList, null);
                }
                if (Image != null)
                {
                    Properties.SetObject(PropImage, null);
                }

                //Dipose the tooltip if one present..
                if (textToolTip != null)
                {
                    textToolTip.Dispose();
                    textToolTip = null;
                }
                controlToolTip = false;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///  Draws an <see cref='Drawing.Image'/> within the specified bounds.
        /// </summary>
        protected void DrawImage(Graphics g, Image image, Rectangle r, ContentAlignment align)
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
                //Always return the Fontheight + some buffer else the Text gets clipped for Autosize = true..
                //(
                if (BorderStyle != BorderStyle.None)
                {
                    bordersAndPadding.Height += 6; // taken from Everett.PreferredHeight
                    bordersAndPadding.Width += 2; // taken from Everett.PreferredWidth
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
            //make sure the behavior is consistent with GetPreferredSizeCore
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

        internal virtual bool UseGDIMeasuring()
        {
            return (FlatStyle == FlatStyle.System || !UseCompatibleTextRendering);
        }

// See ComboBox.cs GetComboHeight
        internal override Size GetPreferredSizeCore(Size proposedConstraints)
        {
            Size bordersAndPadding = GetBordersAndPadding();
            // Subtract border area from constraints
            proposedConstraints -= bordersAndPadding;

            // keep positive
            proposedConstraints = LayoutUtils.UnionSizes(proposedConstraints, Size.Empty);

            Size requiredSize;

            //
            // TEXT Measurement
            //

            if (string.IsNullOrEmpty(Text))
            {
                // empty labels return the font height + borders
                using (WindowsFont font = WindowsFont.FromFont(Font))
                {
                    // this is the character that Windows uses to determine the extent
                    requiredSize = WindowsGraphicsCacheManager.MeasurementGraphics.GetTextExtent("0", font);
                    requiredSize.Width = 0;
                }
            }
            else if (UseGDIMeasuring())
            {
                TextFormatFlags format = FlatStyle == FlatStyle.System ? TextFormatFlags.Default : CreateTextFormatFlags(proposedConstraints);
                requiredSize = MeasureTextCache.GetTextSize(Text, Font, proposedConstraints, format);
            }
            else
            {
                // GDI+ rendering.
                using (Graphics measurementGraphics = WindowsFormsUtils.CreateMeasurementGraphics())
                {
                    using (StringFormat stringFormat = CreateStringFormat())
                    {
                        SizeF bounds = (proposedConstraints.Width == 1) ?
                            new SizeF(0, proposedConstraints.Height) :
                            new SizeF(proposedConstraints.Width, proposedConstraints.Height);

                        requiredSize = Size.Ceiling(measurementGraphics.MeasureString(Text, Font, bounds, stringFormat));
                    }
                }

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

            //If we are using GDI+ the code below will not work, except
            //if the style is FlatStyle.System since GDI will be used in that case.
            if (UseCompatibleTextRendering && FlatStyle != FlatStyle.System)
            {
                return 0;
            }

            using (WindowsGraphics wg = WindowsGraphics.FromHwnd(Handle))
            {
                TextFormatFlags flags = CreateTextFormatFlags();

                if ((flags & TextFormatFlags.NoPadding) == TextFormatFlags.NoPadding)
                {
                    wg.TextPadding = TextPaddingOptions.NoPadding;
                }
                else if ((flags & TextFormatFlags.LeftAndRightPadding) == TextFormatFlags.LeftAndRightPadding)
                {
                    wg.TextPadding = TextPaddingOptions.LeftAndRightPadding;
                }

                using (WindowsFont wf = WindowsGraphicsCacheManager.GetWindowsFont(Font))
                {
                    User32.DRAWTEXTPARAMS dtParams = wg.GetTextMargins(wf);

                    // This is actually leading margin.
                    return dtParams.iLeftMargin;
                }
            }
        }

        private void ImageListRecreateHandle(object sender, EventArgs e)
        {
            if (IsHandleCreated)
            {
                Invalidate();
            }
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
        ///  This method is required because the Label constructor needs to know if the control is
        ///  OwnerDraw but it should not call the virtual property because if a derived class has
        ///  overridden the method, the derived class version will be called (before the derived
        ///  class constructor is called).
        /// </summary>
        private bool IsOwnerDraw() => FlatStyle != FlatStyle.System;

        /// <summary>
        ///  Raises the <see cref='Control.OnMouseEnter'/> event.
        /// </summary>
        protected override void OnMouseEnter(EventArgs e)
        {
            if (!controlToolTip && !DesignMode && AutoEllipsis && showToolTip && textToolTip != null)
            {

                try
                {
                    controlToolTip = true;
                    textToolTip.Show(WindowsFormsUtils.TextWithoutMnemonics(Text), this);
                }
                finally
                {
                    controlToolTip = false;
                }

            }
            base.OnMouseEnter(e);
        }

        /// <summary>
        ///  Raises the <see cref='Control.OnMouseLeave'/> event.
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            if (!controlToolTip && textToolTip != null && textToolTip.GetHandleCreated())
            {
                textToolTip.RemoveAll();

                textToolTip.Hide(this);
            }

            base.OnMouseLeave(e);
        }

        private void OnFrameChanged(object o, EventArgs e)
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
            if (textToolTip != null && textToolTip.GetHandleCreated())
            {
                textToolTip.DestroyHandle();
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

            if (LiveSetting != AutomationLiveSetting.Off)
            {
                AccessibilityObject.RaiseLiveRegionChanged();
            }
        }

        protected virtual void OnTextAlignChanged(EventArgs e)
        {
            if (Events[EVENT_TEXTALIGNCHANGED] is EventHandler eh)
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
            Image i = Image;
            if (i != null)
            {
                DrawImage(e.Graphics, i, face, RtlTranslateAlignment(ImageAlign));
            }

            Color color;
            if (Enabled && SystemInformation.HighContrast)
            {
                color = SystemColors.WindowText;
            }
            else
            {
                IntPtr hdc = e.Graphics.GetHdc();
                try
                {
                    using (WindowsGraphics wg = WindowsGraphics.FromHdc(hdc))
                    {
                        color = wg.GetNearestColor((Enabled) ? ForeColor : DisabledColor);
                    }
                }
                finally
                {
                    e.Graphics.ReleaseHdc();
                }
            }

            // Do actual drawing

            if (AutoEllipsis)
            {
                Rectangle clientRect = ClientRectangle;
                Size preferredSize = GetPreferredSize(new Size(clientRect.Width, clientRect.Height));
                showToolTip = (clientRect.Width < preferredSize.Width || clientRect.Height < preferredSize.Height);
            }
            else
            {
                showToolTip = false;
            }

            if (UseCompatibleTextRendering)
            {
                using (StringFormat stringFormat = CreateStringFormat())
                {
                    if (Enabled)
                    {
                        using (Brush brush = new SolidBrush(color))
                        {
                            e.Graphics.DrawString(Text, Font, brush, face, stringFormat);
                        }
                    }
                    else
                    {
                        ControlPaint.DrawStringDisabled(e.Graphics, Text, Font, color, face, stringFormat);
                    }
                }
            }
            else
            {
                TextFormatFlags flags = CreateTextFormatFlags();

                if (Enabled)
                {
                    TextRenderer.DrawText(e.Graphics, Text, Font, face, color, flags);
                }
                else
                {
                    //Theme specs -- if the backcolor is darker than Control, we use
                    // ControlPaint.Dark(backcolor).  Otherwise we use ControlDark.

                    Color disabledTextForeColor = TextRenderer.DisabledTextColor(BackColor);
                    TextRenderer.DrawText(e.Graphics, Text, Font, face, disabledTextForeColor, flags);
                }
            }

            base.OnPaint(e); // raise paint event
        }

        /// <summary>
        ///  Overriden by LinkLabel.
        /// </summary>
        internal virtual void OnAutoEllipsisChanged(/*EventArgs e*/)
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
                // In the case of SelfSizing
                // we dont know what size to be until we're parented
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

        internal override void PrintToMetaFileRecursive(HandleRef hDC, IntPtr lParam, Rectangle bounds)
        {
            base.PrintToMetaFileRecursive(hDC, lParam, bounds);

            using (WindowsFormsUtils.DCMapping mapping = new WindowsFormsUtils.DCMapping(hDC, bounds))
            {
                using (Graphics g = Graphics.FromHdcInternal(hDC.Handle))
                {
                    ControlPaint.PrintBorder(g, new Rectangle(Point.Empty, Size), BorderStyle, Border3DStyle.SunkenOuter);
                }
            }
        }

        /// <summary>
        ///  Overrides Control. This is called when the user has pressed an Alt-CHAR
        ///  key combination and determines if that combination is an interesting
        ///  mnemonic for this control.
        /// </summary>
        protected internal override bool ProcessMnemonic(char charCode)
        {
            if (UseMnemonic && IsMnemonic(charCode, Text) && CanProcessMnemonic())
            {
                Control parent = ParentInternal;
                if (parent != null)
                {
                    if (parent.SelectNextControl(this, true, false, true, false))
                    {
                        if (!parent.ContainsFocus)
                        {
                            parent.Focus();
                        }
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        ///  Overrides Control.setBoundsCore to enforce autoSize.
        /// </summary>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if ((specified & BoundsSpecified.Height) != BoundsSpecified.None)
            {
                requestedHeight = height;
            }

            if ((specified & BoundsSpecified.Width) != BoundsSpecified.None)
            {
                requestedWidth = width;
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

        private void ResetImage()
        {
            Image = null;
        }

        private bool ShouldSerializeImage()
        {
            return Properties.GetObject(PropImage) != null;
        }

        /// <summary>
        ///  Called by ToolTip to poke in that Tooltip into this ComCtl so that the Native ChildToolTip is not exposed.
        /// </summary>
        internal void SetToolTip(ToolTip toolTip)
        {
            if (toolTip != null && !controlToolTip)
            {
                controlToolTip = true;
            }
        }

        internal override bool SupportsUiaProviders => true;

        /// <summary>
        ///  Returns a string representation for this control.
        /// </summary>
        public override string ToString()
        {
            string s = base.ToString();
            return s + ", Text: " + Text;
        }

        /// <summary>
        ///  Overrides Control. This processes certain messages that the Win32 STATIC
        ///  class would normally override.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WindowMessages.WM_NCHITTEST:
                    // label returns HT_TRANSPARENT for everything, so all messages get
                    // routed to the parent.  Change this so we can tell what's going on.
                    //
                    Rectangle rectInScreen = RectangleToScreen(new Rectangle(0, 0, Width, Height));
                    Point pt = new Point(unchecked((int)(long)m.LParam));
                    m.Result = (IntPtr)((rectInScreen.Contains(pt) ? NativeMethods.HTCLIENT : NativeMethods.HTNOWHERE));
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        [ComVisible(true)]
        internal class LabelAccessibleObject : ControlAccessibleObject
        {
            public LabelAccessibleObject(Label owner) : base(owner)
            {
            }

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = Owner.AccessibleRole;
                    if (role != AccessibleRole.Default)
                    {
                        return role;
                    }
                    return AccessibleRole.StaticText;
                }
            }

            internal override bool IsIAccessibleExSupported() => true;

            internal override object GetPropertyValue(int propertyID)
            {
                if (propertyID == NativeMethods.UIA_ControlTypePropertyId)
                {
                    return NativeMethods.UIA_TextControlTypeId;
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }

    /// <summary>
    ///  Override ImageList.Indexer to support Label's ImageList semantics.
    /// </summary>
    internal class LabelImageIndexer : ImageList.Indexer
    {
        private readonly Label owner;
        private bool useIntegerIndex = true;

        public LabelImageIndexer(Label owner)
        {
            this.owner = owner;
        }

        public override ImageList ImageList
        {
            get { return owner?.ImageList; }

            set { Debug.Assert(false, "Setting the image list in this class is not supported"); }
        }

        public override string Key
        {
            get { return base.Key; }
            set
            {
                base.Key = value;
                useIntegerIndex = false;
            }
        }

        public override int Index
        {
            get { return base.Index; }
            set
            {
                base.Index = value;
                useIntegerIndex = true;
            }

        }

        public override int ActualIndex
        {
            get
            {
                if (useIntegerIndex)
                {
                    // The behavior of label is to return the last item in the Images collection
                    // if the index is currently set higher than the count.
                    return (Index < ImageList.Images.Count) ? Index : ImageList.Images.Count - 1;
                }
                else if (ImageList != null)
                {
                    return ImageList.Images.IndexOfKey(Key);
                }

                return -1;
            }
        }

    }

}
