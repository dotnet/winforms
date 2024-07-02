// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms.Layout;
using Microsoft.Win32;
using Windows.Win32.UI.Controls.Dialogs;
using Windows.Win32.UI.Controls.RichEdit;
using static Interop.Richedit;

namespace System.Windows.Forms;

/// <summary>
///  Rich Text control. The RichTextBox is a control that contains formatted text.
///  It supports font selection, boldface, and other type attributes.
/// </summary>
[Docking(DockingBehavior.Ask)]
[Designer($"System.Windows.Forms.Design.RichTextBoxDesigner, {AssemblyRef.SystemDesign}")]
[SRDescription(nameof(SR.DescriptionRichTextBox))]
public partial class RichTextBox : TextBoxBase
{
    /// <summary>
    ///  Paste special flags.
    /// </summary>
    internal const int INPUT = 0x0001;
    internal const int OUTPUT = 0x0002;
    internal const int DIRECTIONMASK = INPUT | OUTPUT;
    internal const int ANSI = 0x0004;
    internal const int UNICODE = 0x0008;
    internal const int FORMATMASK = ANSI | UNICODE;
    internal const int TEXTLF = 0x0010;
    internal const int TEXTCRLF = 0x0020;
    internal const int RTF = 0x0040;
    internal const int KINDMASK = TEXTLF | TEXTCRLF | RTF;

    // This is where we store the Rich Edit library.
    private static IntPtr s_moduleHandle;

    private const string SZ_RTF_TAG = "{\\rtf";
    private const int CHAR_BUFFER_LEN = 512;

    // Event objects
    private static readonly object s_hscrollEvent = new();
    private static readonly object s_linkActivateEvent = new();
    private static readonly object s_imeChangeEvent = new();
    private static readonly object s_protectedEvent = new();
    private static readonly object s_requestSizeEvent = new();
    private static readonly object s_selectionChangeEvent = new();
    private static readonly object s_vscrollEvent = new();

    // Persistent state
    //
    private int _bulletIndent;
    private int _rightMargin;
    private string? _textRtf; // If not null, takes precedence over cached Text value
    private string? _textPlain;
    private Color _selectionBackColorToSetOnHandleCreated;
    private RichTextBoxLanguageOptions _languageOption = RichTextBoxLanguageOptions.AutoFont | RichTextBoxLanguageOptions.DualFont;

    // Non-persistent state
    //
    private static int s_logPixelsX;
    private static int s_logPixelsY;
    private Stream? _editStream;
    private float _zoomMultiplier = 1.0f;

    // used to decide when to fire the selectionChange event.
    private int _curSelStart;
    private int _curSelEnd;
    private short _curSelType;
    private object? _oleCallback;

    private static int[]? s_shortcutsToDisable;
    private static int s_richEditMajorVersion = 3;

    private BitVector32 _richTextBoxFlags;
    private static readonly BitVector32.Section s_autoWordSelectionSection = BitVector32.CreateSection(1);
    private static readonly BitVector32.Section s_showSelBarSection = BitVector32.CreateSection(1, s_autoWordSelectionSection);
    private static readonly BitVector32.Section s_autoUrlDetectSection = BitVector32.CreateSection(1, s_showSelBarSection);
    private static readonly BitVector32.Section s_fInCtorSection = BitVector32.CreateSection(1, s_autoUrlDetectSection);
    private static readonly BitVector32.Section s_protectedErrorSection = BitVector32.CreateSection(1, s_fInCtorSection);
    private static readonly BitVector32.Section s_linkcursorSection = BitVector32.CreateSection(1, s_protectedErrorSection);
    private static readonly BitVector32.Section s_allowOleDropSection = BitVector32.CreateSection(1, s_linkcursorSection);
    private static readonly BitVector32.Section s_suppressTextChangedEventSection = BitVector32.CreateSection(1, s_allowOleDropSection);
    private static readonly BitVector32.Section s_callOnContentsResizedSection = BitVector32.CreateSection(1, s_suppressTextChangedEventSection);
    private static readonly BitVector32.Section s_richTextShortcutsEnabledSection = BitVector32.CreateSection(1, s_callOnContentsResizedSection);
    private static readonly BitVector32.Section s_allowOleObjectsSection = BitVector32.CreateSection(1, s_richTextShortcutsEnabledSection);
    private static readonly BitVector32.Section s_scrollBarsSection = BitVector32.CreateSection((short)RichTextBoxScrollBars.ForcedBoth, s_allowOleObjectsSection);
    private static readonly BitVector32.Section s_enableAutoDragDropSection = BitVector32.CreateSection(1, s_scrollBarsSection);

    /// <summary>
    ///  Constructs a new RichTextBox.
    /// </summary>
    public RichTextBox()
    {
        InConstructor = true;
        _richTextBoxFlags[s_autoWordSelectionSection] = 0; // This is false by default
        DetectUrls = true;
        ScrollBars = RichTextBoxScrollBars.Both;
        RichTextShortcutsEnabled = true;
        MaxLength = int.MaxValue;
        Multiline = true;
        AutoSize = false;
        _curSelStart = _curSelEnd = _curSelType = -1;
        InConstructor = false;
    }

    /// <summary>
    ///  RichTextBox controls have built-in drag and drop support, but AllowDrop, DragEnter, DragDrop
    ///  may still be used: this should be hidden in the property grid, but not in code
    /// </summary>
    [Browsable(false)]
    public override bool AllowDrop
    {
        get => _richTextBoxFlags[s_allowOleDropSection] != 0;
        set
        {
            _richTextBoxFlags[s_allowOleDropSection] = value ? 1 : 0;
            UpdateOleCallback();
        }
    }

    internal bool AllowOleObjects
    {
        get => _richTextBoxFlags[s_allowOleObjectsSection] != 0;
        set
        {
            _richTextBoxFlags[s_allowOleObjectsSection] = value ? 1 : 0;
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the size
    ///  of the control automatically adjusts when the font assigned to the control
    ///  is changed.
    ///
    ///  Note: this works differently than other Controls' AutoSize, so we're hiding
    ///  it to avoid confusion.
    /// </summary>
    [DefaultValue(false)]
    [RefreshProperties(RefreshProperties.Repaint)]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public override bool AutoSize
    {
        get => base.AutoSize;
        set => base.AutoSize = value;
    }

    /// <summary>
    ///  Controls whether whether mouse selection snaps to whole words.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.RichTextBoxAutoWordSelection))]
    public bool AutoWordSelection
    {
        get => _richTextBoxFlags[s_autoWordSelectionSection] != 0;
        set
        {
            _richTextBoxFlags[s_autoWordSelectionSection] = value ? 1 : 0;
            if (IsHandleCreated)
            {
                PInvoke.SendMessage(
                    this,
                    PInvoke.EM_SETOPTIONS,
                    (WPARAM)(int)(value ? PInvoke.ECOOP_OR : PInvoke.ECOOP_XOR),
                    (LPARAM)(int)PInvoke.ECO_AUTOWORDSELECTION);
            }
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
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
    ///  Returns the amount of indent used in a RichTextBox control when
    ///  SelectionBullet is set to true.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(0)]
    [Localizable(true)]
    [SRDescription(nameof(SR.RichTextBoxBulletIndent))]
    public int BulletIndent
    {
        get => _bulletIndent;

        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            _bulletIndent = value;

            // Call to update the control only if the bullet is set.
            if (IsHandleCreated && SelectionBullet)
            {
                SelectionBullet = true;
            }
        }
    }

    private bool CallOnContentsResized
    {
        get => _richTextBoxFlags[s_callOnContentsResizedSection] != 0;
        set => _richTextBoxFlags[s_callOnContentsResizedSection] = value ? 1 : 0;
    }

    internal override bool CanRaiseTextChangedEvent => !SuppressTextChangedEvent;

    /// <summary>
    ///  Whether or not there are actions that can be Redone on the RichTextBox control.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.RichTextBoxCanRedoDescr))]
    public bool CanRedo => IsHandleCreated && (int)PInvoke.SendMessage(this, PInvoke.EM_CANREDO) != 0;

    protected override CreateParams CreateParams
    {
        get
        {
            // Check for library
            if (s_moduleHandle == IntPtr.Zero)
            {
                s_moduleHandle = PInvoke.LoadLibraryFromSystemPathIfAvailable(Libraries.RichEdit41);

                int lastWin32Error = Marshal.GetLastWin32Error();

                // This code has been here since the inception of the project,
                // we can't determine why we have to compare w/ 32 here.
                // This fails on 3-GB mode, (once the dll is loaded above 3GB memory space)
                if ((ulong)s_moduleHandle < 32)
                {
                    throw new Win32Exception(lastWin32Error, string.Format(SR.LoadDLLError, Libraries.RichEdit41));
                }

                string path = PInvoke.GetModuleFileNameLongPath(new HINSTANCE(s_moduleHandle));
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(path);

                Debug.Assert(versionInfo is not null && !string.IsNullOrEmpty(versionInfo.ProductVersion), "Couldn't get the version info for the richedit dll");
                if (versionInfo is not null && !string.IsNullOrEmpty(versionInfo.ProductVersion))
                {
                    // Note: this only allows for one digit version
                    if (int.TryParse(versionInfo.ProductVersion.AsSpan(0, 1), out int parsedValue))
                    {
                        s_richEditMajorVersion = parsedValue;
                    }
                }
            }

            CreateParams cp = base.CreateParams;
            cp.ClassName = PInvoke.MSFTEDIT_CLASS;

            if (Multiline)
            {
                if (((int)ScrollBars & RichTextBoxConstants.RTB_HORIZ) != 0 && !WordWrap)
                {
                    // RichEd infers word wrap from the absence of horizontal scroll bars
                    cp.Style |= (int)WINDOW_STYLE.WS_HSCROLL;
                    if (((int)ScrollBars & RichTextBoxConstants.RTB_FORCE) != 0)
                    {
                        cp.Style |= (int)PInvoke.ES_DISABLENOSCROLL;
                    }
                }

                if (((int)ScrollBars & RichTextBoxConstants.RTB_VERT) != 0)
                {
                    cp.Style |= (int)WINDOW_STYLE.WS_VSCROLL;
                    if (((int)ScrollBars & RichTextBoxConstants.RTB_FORCE) != 0)
                    {
                        cp.Style |= (int)PInvoke.ES_DISABLENOSCROLL;
                    }
                }
            }

            // Remove the WS_BORDER style from the control, if we're trying to set it,
            // to prevent the control from displaying the single point rectangle around the 3D border
            if (BorderStyle == BorderStyle.FixedSingle && ((cp.Style & (int)WINDOW_STYLE.WS_BORDER) != 0))
            {
                cp.Style &= ~(int)WINDOW_STYLE.WS_BORDER;
                cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_CLIENTEDGE;
            }

            return cp;
        }
    }

    // public bool CanUndo {}; <-- inherited from TextBoxBase

    /// <summary>
    ///  Controls whether or not the rich edit control will automatically highlight URLs.
    ///  By default, this is true. Note that changing this property will not update text that is
    ///  already present in the RichTextBox control; it only affects text which is entered after the
    ///  property is changed.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.RichTextBoxDetectURLs))]
    public bool DetectUrls
    {
        get => _richTextBoxFlags[s_autoUrlDetectSection] != 0;
        set
        {
            if (value != DetectUrls)
            {
                _richTextBoxFlags[s_autoUrlDetectSection] = value ? 1 : 0;
                if (IsHandleCreated)
                {
                    PInvoke.SendMessage(this, PInvoke.EM_AUTOURLDETECT, (WPARAM)(BOOL)(value));
                    RecreateHandle();
                }
            }
        }
    }

    protected override Size DefaultSize => new(100, 96);

    /// <summary>
    ///  Defines <see cref="VisualStylesMode.Latest"/> as default for this control, so this control provides the latest visual styles for .NET 9+.
    /// </summary>
    protected override VisualStylesMode DefaultVisualStylesMode => VisualStylesMode.Latest;

    /// <summary>
    ///  We can't just enable drag/drop of text by default: it's a breaking change.
    ///  Should be false by default.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.RichTextBoxEnableAutoDragDrop))]
    public bool EnableAutoDragDrop
    {
        get => _richTextBoxFlags[s_enableAutoDragDropSection] != 0;
        set
        {
            _richTextBoxFlags[s_enableAutoDragDropSection] = value ? 1 : 0;
            UpdateOleCallback();
        }
    }

    public override Color ForeColor
    {
        get => base.ForeColor;
        set
        {
            if (IsHandleCreated)
            {
                if (InternalSetForeColor(value))
                {
                    base.ForeColor = value;
                }
            }
            else
            {
                base.ForeColor = value;
            }
        }
    }

    [AllowNull]
    public override Font Font
    {
        get => base.Font;
        set
        {
            if (!IsHandleCreated || PInvoke.GetWindowTextLength(this) <= 0)
            {
                base.Font = value;
                return;
            }

            if (value is null)
            {
                base.Font = null;
                SetCharFormatFont(selectionOnly: false, Font);
                return;
            }

            try
            {
                Font? font = GetCharFormatFont(selectionOnly: false);
                if (font is null || !font.Equals(value))
                {
                    SetCharFormatFont(selectionOnly: false, value);

                    // Update controlfont from "resolved" font from the attempt to set the document font.
                    CallOnContentsResized = true;
                    base.Font = GetCharFormatFont(selectionOnly: false);
                }
            }
            finally
            {
                CallOnContentsResized = false;
            }
        }
    }

    internal override Size GetPreferredSizeCore(Size proposedConstraints)
    {
        Size scrollBarPadding = Size.Empty;

        // If the RTB is multiline, we won't have a horizontal scrollbar.
        if (!WordWrap && Multiline && (ScrollBars & RichTextBoxScrollBars.Horizontal) != 0)
        {
            scrollBarPadding.Height += SystemInformation.HorizontalScrollBarHeight;
        }

        if (Multiline && (ScrollBars & RichTextBoxScrollBars.Vertical) != 0)
        {
            scrollBarPadding.Width += SystemInformation.VerticalScrollBarWidth;
        }

        // Subtract the scroll bar padding before measuring
        proposedConstraints -= scrollBarPadding;

        Size prefSize = base.GetPreferredSizeCore(proposedConstraints);

        return prefSize + scrollBarPadding;
    }

    private bool InConstructor
    {
        get => _richTextBoxFlags[s_fInCtorSection] != 0;
        set => _richTextBoxFlags[s_fInCtorSection] = value ? 1 : 0;
    }

    /// <summary>
    ///  Sets or gets the rich text box control' language option.
    ///  The IMF_AUTOFONT flag is set by default.
    ///  The IMF_AUTOKEYBOARD and IMF_IMECANCELCOMPLETE flags are cleared by default.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public RichTextBoxLanguageOptions LanguageOption
    {
        get => IsHandleCreated
            ? (RichTextBoxLanguageOptions)(int)PInvoke.SendMessage(this, PInvoke.EM_GETLANGOPTIONS)
            : _languageOption;
        set
        {
            if (LanguageOption != value)
            {
                _languageOption = value;
                if (IsHandleCreated)
                {
                    PInvoke.SendMessage(this, PInvoke.EM_SETLANGOPTIONS, 0, (nint)value);
                }
            }
        }
    }

    private bool LinkCursor
    {
        get => _richTextBoxFlags[s_linkcursorSection] != 0;
        set => _richTextBoxFlags[s_linkcursorSection] = value ? 1 : 0;
    }

    [DefaultValue(int.MaxValue)]
    public override int MaxLength
    {
        get => base.MaxLength;
        set => base.MaxLength = value;
    }

    [DefaultValue(true)]
    public override bool Multiline
    {
        get => base.Multiline;
        set => base.Multiline = value;
    }

    private bool ProtectedError
    {
        get => _richTextBoxFlags[s_protectedErrorSection] != 0;
        set => _richTextBoxFlags[s_protectedErrorSection] = value ? 1 : 0;
    }

    private protected override void RaiseAccessibilityTextChangedEvent()
    {
        // Do not do anything because Win32 provides unmanaged Text pattern for RichTextBox
    }

    /// <summary>
    ///  Returns the name of the action that will be performed if the user
    ///  Redo's their last Undone operation. If no operation can be redone,
    ///  an empty string ("") is returned.
    /// </summary>
    // NOTE: This is overridable, because we want people to be able to
    //      mess with the names if necessary...?
    [SRCategory(nameof(SR.CatBehavior))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.RichTextBoxRedoActionNameDescr))]
    public string RedoActionName
    {
        get
        {
            if (!CanRedo)
            {
                return string.Empty;
            }

            int n = (int)PInvoke.SendMessage(this, PInvoke.EM_GETREDONAME);
            return GetEditorActionName(n);
        }
    }

    // Description: Specifies whether rich text formatting keyboard shortcuts are enabled.
    [DefaultValue(true)]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool RichTextShortcutsEnabled
    {
        get => _richTextBoxFlags[s_richTextShortcutsEnabledSection] != 0;
        set
        {
            s_shortcutsToDisable ??= [(int)Shortcut.CtrlL, (int)Shortcut.CtrlR, (int)Shortcut.CtrlE, (int)Shortcut.CtrlJ];

            _richTextBoxFlags[s_richTextShortcutsEnabledSection] = value ? 1 : 0;
        }
    }

    /// <summary>
    ///  The right margin of a RichTextBox control.  A nonzero margin implies WordWrap.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(0)]
    [Localizable(true)]
    [SRDescription(nameof(SR.RichTextBoxRightMargin))]
    public unsafe int RightMargin
    {
        get => _rightMargin;
        set
        {
            if (_rightMargin != value)
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);

                _rightMargin = value;

                if (value == 0)
                {
                    // Once you set EM_SETTARGETDEVICE to something nonzero, RichEd will assume
                    // word wrap forever and ever.
                    RecreateHandle();
                }
                else if (IsHandleCreated)
                {
                    using CreateDcScope hdc = new("DISPLAY");
                    PInvoke.SendMessage(this, PInvoke.EM_SETTARGETDEVICE, (WPARAM)hdc, Pixel2Twip(value, true));
                }
            }
        }
    }

    /// <summary>
    ///  The text of a RichTextBox control, including all Rtf codes.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.RichTextBoxRTF))]
    [RefreshProperties(RefreshProperties.All)]
    public string? Rtf
    {
        get
        {
            if (IsHandleCreated)
            {
                return StreamOut(PInvoke.SF_RTF);
            }
            else if (_textPlain is not null)
            {
                ForceHandleCreate();
                return StreamOut(PInvoke.SF_RTF);
            }
            else
            {
                return _textRtf;
            }
        }
        set
        {
            value ??= string.Empty;

            if (value.Equals(Rtf))
            {
                return;
            }

            ForceHandleCreate();
            _textRtf = value;
            StreamIn(value, PInvoke.SF_RTF);
            if (CanRaiseTextChangedEvent)
            {
                OnTextChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    ///  The current scrollbar settings for a multi-line rich edit control.
    ///  Possible return values are given by the RichTextBoxScrollBars enumeration.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(RichTextBoxScrollBars.Both)]
    [Localizable(true)]
    [SRDescription(nameof(SR.RichTextBoxScrollBars))]
    public RichTextBoxScrollBars ScrollBars
    {
        get => (RichTextBoxScrollBars)_richTextBoxFlags[s_scrollBarsSection];
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (value != ScrollBars)
            {
                using (LayoutTransaction.CreateTransactionIf(AutoSize, ParentInternal, this, PropertyNames.ScrollBars))
                {
                    _richTextBoxFlags[s_scrollBarsSection] = (int)value;
                    RecreateHandle();
                }
            }
        }
    }

    /// <summary>
    ///  The alignment of the paragraphs in a RichTextBox control.
    /// </summary>
    [DefaultValue(HorizontalAlignment.Left)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.RichTextBoxSelAlignment))]
    public unsafe HorizontalAlignment SelectionAlignment
    {
        get
        {
            HorizontalAlignment selectionAlignment = HorizontalAlignment.Left;

            ForceHandleCreate();
            PARAFORMAT pf = new()
            {
                cbSize = (uint)sizeof(PARAFORMAT)
            };

            // Get the format for our currently selected paragraph.
            PInvoke.SendMessage(this, PInvoke.EM_GETPARAFORMAT, 0, ref pf);

            // check if alignment has been set yet
            if ((PFM.ALIGNMENT & pf.dwMask) != 0)
            {
                switch (pf.wAlignment)
                {
                    case PFA.LEFT:
                        selectionAlignment = HorizontalAlignment.Left;
                        break;

                    case PFA.RIGHT:
                        selectionAlignment = HorizontalAlignment.Right;
                        break;

                    case PFA.CENTER:
                        selectionAlignment = HorizontalAlignment.Center;
                        break;
                }
            }

            return selectionAlignment;
        }
        set
        {
            // valid values are 0x0 to 0x2
            SourceGenerated.EnumValidator.Validate(value);

            ForceHandleCreate();
            PARAFORMAT pf = new()
            {
                cbSize = (uint)sizeof(PARAFORMAT),
                dwMask = PFM.ALIGNMENT
            };

            switch (value)
            {
                case HorizontalAlignment.Left:
                    pf.wAlignment = PFA.LEFT;
                    break;

                case HorizontalAlignment.Right:
                    pf.wAlignment = PFA.RIGHT;
                    break;

                case HorizontalAlignment.Center:
                    pf.wAlignment = PFA.CENTER;
                    break;
            }

            // Set the format for our current paragraph or selection.
            PInvoke.SendMessage(this, PInvoke.EM_SETPARAFORMAT, 0, ref pf);
        }
    }

    /// <summary>
    ///  Determines if a paragraph in the RichTextBox control
    ///  contains the current selection or insertion point has the bullet style.
    /// </summary>
    [DefaultValue(false)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.RichTextBoxSelBullet))]
    public unsafe bool SelectionBullet
    {
        get
        {
            RichTextBoxSelectionAttribute selectionBullet = RichTextBoxSelectionAttribute.None;

            ForceHandleCreate();
            PARAFORMAT pf = new()
            {
                cbSize = (uint)sizeof(PARAFORMAT)
            };

            // Get the format for our currently selected paragraph.
            PInvoke.SendMessage(this, PInvoke.EM_GETPARAFORMAT, 0, ref pf);

            // check if alignment has been set yet
            if ((PFM.NUMBERING & pf.dwMask) != 0)
            {
                if (pf.wNumbering == PARAFORMAT_NUMBERING.PFN_BULLET)
                {
                    selectionBullet = RichTextBoxSelectionAttribute.All;
                }
            }
            else
            {
                // For paragraphs with mixed SelectionBullets, we just return false
                return false;
            }

            return selectionBullet == RichTextBoxSelectionAttribute.All;
        }
        set
        {
            ForceHandleCreate();

            PARAFORMAT pf = new()
            {
                cbSize = (uint)sizeof(PARAFORMAT),
                dwMask = PFM.NUMBERING | PFM.OFFSET
            };

            if (!value)
            {
                pf.wNumbering = 0;
                pf.dxOffset = 0;
            }
            else
            {
                pf.wNumbering = PARAFORMAT_NUMBERING.PFN_BULLET;
                pf.dxOffset = Pixel2Twip(_bulletIndent, true);
            }

            // Set the format for our current paragraph or selection.
            PInvoke.SendMessage(this, PInvoke.EM_SETPARAFORMAT, 0, ref pf);
        }
    }

    /// <summary>
    ///  Determines whether text in the RichTextBox control
    ///  appears on the baseline (normal), as a superscript above the baseline,
    ///  or as a subscript below the baseline.
    /// </summary>
    [DefaultValue(0)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.RichTextBoxSelCharOffset))]
    public unsafe int SelectionCharOffset
    {
        get
        {
            ForceHandleCreate();
            CHARFORMAT2W cf = GetCharFormat(true);
            return Twip2Pixel(cf.yOffset, false);
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 2000);
            ArgumentOutOfRangeException.ThrowIfLessThan(value, -2000);

            ForceHandleCreate();
            CHARFORMAT2W cf = new()
            {
                cbSize = (uint)sizeof(CHARFORMAT2W),
                dwMask = CFM_MASK.CFM_OFFSET,
                yOffset = Pixel2Twip(value, false)
            };

            // Set the format information.
            //
            // SendMessage will force the handle to be created if it hasn't already. Normally,
            // we would cache property values until the handle is created - but for this property,
            // it's far more simple to just create the handle.
            PInvoke.SendMessage(this, PInvoke.EM_SETCHARFORMAT, PInvoke.SCF_SELECTION, ref cf);
        }
    }

    /// <summary>
    ///  The color of the currently selected text in the RichTextBox control.
    /// </summary>
    /// <returns>The color or <see cref="Color.Empty"/> if the selection has more than one color.</returns>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.RichTextBoxSelColor))]
    public Color SelectionColor
    {
        get
        {
            Color selColor = Color.Empty;

            ForceHandleCreate();
            CHARFORMAT2W cf = GetCharFormat(true);
            // if the effects member contains valid info
            if ((cf.dwMask & CFM_MASK.CFM_COLOR) != 0)
            {
                selColor = ColorTranslator.FromOle(cf.crTextColor);
            }

            return selColor;
        }
        set
        {
            ForceHandleCreate();
            CHARFORMAT2W cf = GetCharFormat(true);
            cf.dwMask = CFM_MASK.CFM_COLOR;
            cf.dwEffects = 0;
            cf.crTextColor = ColorTranslator.ToWin32(value);

            // Set the format information.
            PInvoke.SendMessage(this, PInvoke.EM_SETCHARFORMAT, (WPARAM)PInvoke.SCF_SELECTION, ref cf);
        }
    }

    /// <summary>
    ///  The background color of the currently selected text in the RichTextBox control.
    ///  Returns Color.Empty if the selection has more than one color.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.RichTextBoxSelBackColor))]
    public unsafe Color SelectionBackColor
    {
        get
        {
            Color selColor = Color.Empty;

            if (IsHandleCreated)
            {
                CHARFORMAT2W cf2 = GetCharFormat(true);
                // If the effects member contains valid info
                if ((cf2.dwEffects & CFE_EFFECTS.CFE_AUTOBACKCOLOR) != 0)
                {
                    selColor = BackColor;
                }
                else if ((cf2.dwMask & CFM_MASK.CFM_BACKCOLOR) != 0)
                {
                    selColor = ColorTranslator.FromOle(cf2.crBackColor);
                }
            }
            else
            {
                selColor = _selectionBackColorToSetOnHandleCreated;
            }

            return selColor;
        }
        set
        {
            // Note: don't compare the value to the old value here: it's possible that
            // you have a different range selected.
            _selectionBackColorToSetOnHandleCreated = value;
            if (IsHandleCreated)
            {
                CHARFORMAT2W cf2 = new()
                {
                    cbSize = (uint)sizeof(CHARFORMAT2W)
                };

                if (value == Color.Empty)
                {
                    cf2.dwEffects = CFE_EFFECTS.CFE_AUTOBACKCOLOR;
                }
                else
                {
                    cf2.dwMask = CFM_MASK.CFM_BACKCOLOR;
                    cf2.crBackColor = ColorTranslator.ToWin32(value);
                }

                PInvoke.SendMessage(this, PInvoke.EM_SETCHARFORMAT, (WPARAM)PInvoke.SCF_SELECTION, ref cf2);
            }
        }
    }

    /// <summary>
    ///  The font used to display the currently selected text
    ///  or the characters(s) immediately following the insertion point in the
    ///  RichTextBox control.  Null if the selection has more than one font.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.RichTextBoxSelFont))]
    [DisallowNull]
    public Font? SelectionFont
    {
        get => GetCharFormatFont(true);
        set => SetCharFormatFont(true, value);
    }

    /// <summary>
    ///  The distance (in pixels) between the left edge of the first line of text
    ///  in the selected paragraph(s) (as specified by the SelectionIndent property)
    ///  and the left edge of subsequent lines of text in the same paragraph(s).
    /// </summary>
    [DefaultValue(0)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.RichTextBoxSelHangingIndent))]
    public unsafe int SelectionHangingIndent
    {
        get
        {
            int selHangingIndent = 0;

            ForceHandleCreate();
            PARAFORMAT pf = new()
            {
                cbSize = (uint)sizeof(PARAFORMAT)
            };

            // Get the format for our currently selected paragraph.
            PInvoke.SendMessage(this, PInvoke.EM_GETPARAFORMAT, 0, ref pf);

            // Check if alignment has been set yet.
            if ((PFM.OFFSET & pf.dwMask) != 0)
            {
                selHangingIndent = pf.dxOffset;
            }

            return Twip2Pixel(selHangingIndent, true);
        }
        set
        {
            ForceHandleCreate();

            PARAFORMAT pf = new()
            {
                cbSize = (uint)sizeof(PARAFORMAT),
                dwMask = PFM.OFFSET,
                dxOffset = Pixel2Twip(value, true)
            };

            // Set the format for our current paragraph or selection.
            PInvoke.SendMessage(this, PInvoke.EM_SETPARAFORMAT, 0, ref pf);
        }
    }

    /// <summary>
    ///  The distance (in pixels) between the left edge of the RichTextBox control and
    ///  the left edge of the text that is selected or added at the current
    ///  insertion point.
    /// </summary>
    [DefaultValue(0)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.RichTextBoxSelIndent))]
    public unsafe int SelectionIndent
    {
        get
        {
            int selIndent = 0;

            ForceHandleCreate();
            PARAFORMAT pf = new()
            {
                cbSize = (uint)sizeof(PARAFORMAT)
            };

            // Get the format for our currently selected paragraph.
            PInvoke.SendMessage(this, PInvoke.EM_GETPARAFORMAT, 0, ref pf);

            // Check if alignment has been set yet.
            if ((PFM.STARTINDENT & pf.dwMask) != 0)
            {
                selIndent = pf.dxStartIndent;
            }

            return Twip2Pixel(selIndent, true);
        }
        set
        {
            ForceHandleCreate();

            PARAFORMAT pf = new()
            {
                cbSize = (uint)sizeof(PARAFORMAT),
                dwMask = PFM.STARTINDENT,
                dxStartIndent = Pixel2Twip(value, true)
            };

            // Set the format for our current paragraph or selection.
            PInvoke.SendMessage(this, PInvoke.EM_SETPARAFORMAT, 0, ref pf);
        }
    }

    /// <summary>
    ///  Gets or sets the number of characters selected in the text
    ///  box.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.TextBoxSelectionLengthDescr))]
    public override int SelectionLength
    {
        get
        {
            if (!IsHandleCreated)
            {
                return base.SelectionLength;
            }

            // RichTextBox allows the user to select the EOF character,
            // but we don't want to include this in the SelectionLength.
            // So instead of sending EM_GETSEL, we just obtain the SelectedText and return
            // the length of it.
            //
            return SelectedText.Length;
        }
        set => base.SelectionLength = value;
    }

    /// <summary>
    ///  true if the current selection prevents any changes to its contents.
    /// </summary>
    [DefaultValue(false)]
    [SRDescription(nameof(SR.RichTextBoxSelProtected))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool SelectionProtected
    {
        get
        {
            ForceHandleCreate();
            return GetCharFormat(CFM_MASK.CFM_PROTECTED, CFE_EFFECTS.CFE_PROTECTED) == RichTextBoxSelectionAttribute.All;
        }
        set
        {
            ForceHandleCreate();
            SetCharFormat(CFM_MASK.CFM_PROTECTED, value ? CFE_EFFECTS.CFE_PROTECTED : 0, RichTextBoxSelectionAttribute.All);
        }
    }

    /// <summary>
    ///  The currently selected text of a RichTextBox control, including
    ///  all Rtf codes.
    /// </summary>
    [DefaultValue("")]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.RichTextBoxSelRTF))]
    [AllowNull]
    public string SelectedRtf
    {
        get
        {
            ForceHandleCreate();
            return StreamOut(PInvoke.SFF_SELECTION | PInvoke.SF_RTF);
        }
        set
        {
            ForceHandleCreate();
            value ??= string.Empty;

            StreamIn(value, PInvoke.SFF_SELECTION | PInvoke.SF_RTF);
        }
    }

    /// <summary>
    ///  The distance (in pixels) between the right edge of the RichTextBox control and
    ///  the right edge of the text that is selected or added at the current
    ///  insertion point.
    /// </summary>
    [DefaultValue(0)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.RichTextBoxSelRightIndent))]
    public unsafe int SelectionRightIndent
    {
        get
        {
            int selRightIndent = 0;

            ForceHandleCreate();

            PARAFORMAT pf = new()
            {
                cbSize = (uint)sizeof(PARAFORMAT)
            };

            // Get the format for our currently selected paragraph.
            PInvoke.SendMessage(this, PInvoke.EM_GETPARAFORMAT, 0, ref pf);

            // Check if alignment has been set yet.
            if ((PFM.RIGHTINDENT & pf.dwMask) != 0)
            {
                selRightIndent = pf.dxRightIndent;
            }

            return Twip2Pixel(selRightIndent, true);
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            ForceHandleCreate();
            PARAFORMAT pf = new()
            {
                cbSize = (uint)sizeof(PARAFORMAT),
                dwMask = PFM.RIGHTINDENT,
                dxRightIndent = Pixel2Twip(value, true)
            };

            // Set the format for our current paragraph or selection.
            PInvoke.SendMessage(this, PInvoke.EM_SETPARAFORMAT, 0, ref pf);
        }
    }

    /// <summary>
    ///  The absolute tab positions (in pixels) of text in a RichTextBox control.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.RichTextBoxSelTabs))]
    [AllowNull]
    public unsafe int[] SelectionTabs
    {
        get
        {
            int[] selTabs = [];

            ForceHandleCreate();
            PARAFORMAT pf = new()
            {
                cbSize = (uint)sizeof(PARAFORMAT)
            };

            // get the format for our currently selected paragraph
            PInvoke.SendMessage(this, PInvoke.EM_GETPARAFORMAT, 0, ref pf);

            // check if alignment has been set yet
            if ((PFM.TABSTOPS & pf.dwMask) != 0)
            {
                selTabs = new int[pf.cTabCount];
                for (int x = 0; x < pf.cTabCount; x++)
                {
                    selTabs[x] = Twip2Pixel(pf.rgxTabs[x], true);
                }
            }

            return selTabs;
        }
        set
        {
            // Verify the argument, and throw an error if is bad
            if (value is not null && value.Length > PInvoke.MAX_TAB_STOPS)
            {
                throw new ArgumentOutOfRangeException(nameof(value), SR.SelTabCountRange);
            }

            ForceHandleCreate();
            PARAFORMAT pf = new()
            {
                cbSize = (uint)sizeof(PARAFORMAT)
            };

            // get the format for our currently selected paragraph because
            // we need to get the number of tabstops to copy
            PInvoke.SendMessage(this, PInvoke.EM_GETPARAFORMAT, 0, ref pf);

            pf.cTabCount = (short)((value is null) ? 0 : value.Length);
            pf.dwMask = PFM.TABSTOPS;
            for (int x = 0; x < pf.cTabCount; x++)
            {
                pf.rgxTabs[x] = Pixel2Twip(value![x], true);
            }

            // Set the format for our current paragraph or selection.
            PInvoke.SendMessage(this, PInvoke.EM_SETPARAFORMAT, 0, ref pf);
        }
    }

    /// <summary>
    ///  The currently selected text of a RichTextBox control; consists of a
    ///  zero length string if no characters are selected.
    /// </summary>
    [DefaultValue("")]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.RichTextBoxSelText))]
    [AllowNull]
    public override string SelectedText
    {
        get
        {
            ForceHandleCreate();
            return GetTextEx(GETTEXTEX_FLAGS.GT_SELECTION);
        }
        set
        {
            ForceHandleCreate();
            value ??= string.Empty;
            StreamIn(value, PInvoke.SFF_SELECTION | PInvoke.SF_TEXT | PInvoke.SF_UNICODE);
        }
    }

    /// <summary>
    ///  The type of the current selection. The returned value is one
    ///  of the values enumerated in RichTextBoxSelectionType.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.RichTextBoxSelTypeDescr))]
    public RichTextBoxSelectionTypes SelectionType
    {
        get
        {
            ForceHandleCreate();
            if (SelectionLength > 0)
            {
                int n = (int)PInvoke.SendMessage(this, PInvoke.EM_SELECTIONTYPE);
                return (RichTextBoxSelectionTypes)n;
            }
            else
            {
                return RichTextBoxSelectionTypes.Empty;
            }
        }
    }

    /// <summary>
    ///  Whether or not the left edge of the control will have a "selection margin" which
    ///  can be used to select entire lines
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.RichTextBoxSelMargin))]
    public bool ShowSelectionMargin
    {
        get { return _richTextBoxFlags[s_showSelBarSection] != 0; }
        set
        {
            if (value != ShowSelectionMargin)
            {
                _richTextBoxFlags[s_showSelBarSection] = value ? 1 : 0;
                if (IsHandleCreated)
                {
                    PInvoke.SendMessage(
                        this,
                        PInvoke.EM_SETOPTIONS,
                        (WPARAM)(int)(value ? PInvoke.ECOOP_OR : PInvoke.ECOOP_XOR),
                        (LPARAM)(int)PInvoke.ECO_SELECTIONBAR);
                }
            }
        }
    }

    [Localizable(true)]
    [RefreshProperties(RefreshProperties.All)]
    [AllowNull]
    public override string Text
    {
        get
        {
            if (IsDisposed)
            {
                return base.Text;
            }

            if (RecreatingHandle || GetAnyDisposingInHierarchy())
            {
                // We can return any old garbage if we're in the process of recreating the handle
                return string.Empty;
            }

            if (!IsHandleCreated && _textRtf is null)
            {
                if (_textPlain is not null)
                {
                    return _textPlain;
                }
                else
                {
                    return base.Text;
                }
            }
            else
            {
                // if the handle is created, we are golden, however
                // if the handle isn't created, but textRtf was
                // specified, we need the RichEdit to translate
                // for us, so we must create the handle;
                //
                ForceHandleCreate();

                return GetTextEx();
            }
        }
        set
        {
            using (LayoutTransaction.CreateTransactionIf(AutoSize, ParentInternal, this, PropertyNames.Text))
            {
                _textRtf = null;
                if (!IsHandleCreated)
                {
                    _textPlain = value;
                }
                else
                {
                    _textPlain = null;
                    value ??= string.Empty;

                    StreamIn(value, PInvoke.SF_TEXT | PInvoke.SF_UNICODE);
                    // reset Modified
                    PInvoke.SendMessage(this, PInvoke.EM_SETMODIFY);
                }
            }
        }
    }

    private bool SuppressTextChangedEvent
    {
        get { return _richTextBoxFlags[s_suppressTextChangedEventSection] != 0; }
        set
        {
            bool oldValue = SuppressTextChangedEvent;
            if (value != oldValue)
            {
                _richTextBoxFlags[s_suppressTextChangedEventSection] = value ? 1 : 0;
                CommonProperties.xClearPreferredSizeCache(this);
            }
        }
    }

    [Browsable(false)]
    public override unsafe int TextLength
    {
        get
        {
            GETTEXTLENGTHEX gtl = new()
            {
                flags = GETTEXTLENGTHEX_FLAGS.GTL_NUMCHARS,
                codepage = 1200u /* CP_UNICODE */
            };

            return (int)PInvoke.SendMessage(this, PInvoke.EM_GETTEXTLENGTHEX, (WPARAM)(&gtl));
        }
    }

    /// <summary>
    ///  Returns the name of the action that will be undone if the user
    ///  Undo's their last operation. If no operation can be undone, it will
    ///  return an empty string ("").
    /// </summary>
    // NOTE: This is overridable, because we want people to be able to
    //      mess with the names if necessary...?
    [SRCategory(nameof(SR.CatBehavior))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.RichTextBoxUndoActionNameDescr))]
    public string UndoActionName
    {
        get
        {
            if (!CanUndo)
            {
                return string.Empty;
            }

            int n = (int)PInvoke.SendMessage(this, PInvoke.EM_GETUNDONAME);
            return GetEditorActionName(n);
        }
    }

    private static string GetEditorActionName(int actionID)
    {
        switch (actionID)
        {
            case 0:
                return SR.RichTextBox_IDUnknown;
            case 1:
                return SR.RichTextBox_IDTyping;
            case 2:
                return SR.RichTextBox_IDDelete;
            case 3:
                return SR.RichTextBox_IDDragDrop;
            case 4:
                return SR.RichTextBox_IDCut;
            case 5:
                return SR.RichTextBox_IDPaste;
            default:
                goto
            case 0;
        }
    }

    /// <summary>
    ///  The current zoom level for the RichTextBox control. This may be between 1/64 and 64. 1.0 indicates
    ///  no zoom (i.e. normal viewing).  Zoom works best with TrueType fonts;
    ///  for non-TrueType fonts, ZoomFactor will be treated as the nearest whole number.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(1.0f)]
    [Localizable(true)]
    [SRDescription(nameof(SR.RichTextBoxZoomFactor))]
    public unsafe float ZoomFactor
    {
        get
        {
            if (IsHandleCreated)
            {
                int numerator = 0;
                int denominator = 0;
                PInvoke.SendMessage(this, PInvoke.EM_GETZOOM, (WPARAM)(&numerator), ref denominator);
                if ((numerator != 0) && (denominator != 0))
                {
                    _zoomMultiplier = numerator / ((float)denominator);
                }
                else
                {
                    _zoomMultiplier = 1.0f;
                }

                return _zoomMultiplier;
            }
            else
            {
                return _zoomMultiplier;
            }
        }

        set
        {
            if (!float.IsNaN(value))
            {
                ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, 0.015625f);
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, 64.0f);
            }

            if (value != _zoomMultiplier)
            {
                SendZoomFactor(value);
            }
        }
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.RichTextBoxContentsResized))]
    public event ContentsResizedEventHandler? ContentsResized
    {
        add => Events.AddHandler(s_requestSizeEvent, value);
        remove => Events.RemoveHandler(s_requestSizeEvent, value);
    }

    /// <summary>
    ///  RichTextBox controls have built-in drag and drop support, but AllowDrop, DragEnter, DragDrop
    ///  may still be used: this should be hidden in the property grid, but not in code
    /// </summary>
    [Browsable(false)]
    public new event DragEventHandler? DragDrop
    {
        add => base.DragDrop += value;
        remove => base.DragDrop -= value;
    }

    /// <summary>
    ///  RichTextBox controls have built-in drag and drop support, but AllowDrop, DragEnter, DragDrop
    ///  may still be used: this should be hidden in the property grid, but not in code
    /// </summary>
    [Browsable(false)]
    public new event DragEventHandler? DragEnter
    {
        add => base.DragEnter += value;
        remove => base.DragEnter -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? DragLeave
    {
        add => base.DragLeave += value;
        remove => base.DragLeave -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event DragEventHandler? DragOver
    {
        add => base.DragOver += value;
        remove => base.DragOver -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event GiveFeedbackEventHandler? GiveFeedback
    {
        add => base.GiveFeedback += value;
        remove => base.GiveFeedback -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event QueryContinueDragEventHandler? QueryContinueDrag
    {
        add => base.QueryContinueDrag += value;
        remove => base.QueryContinueDrag -= value;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.RichTextBoxHScroll))]
    public event EventHandler? HScroll
    {
        add => Events.AddHandler(s_hscrollEvent, value);
        remove => Events.RemoveHandler(s_hscrollEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.RichTextBoxLinkClick))]
    public event LinkClickedEventHandler? LinkClicked
    {
        add => Events.AddHandler(s_linkActivateEvent, value);
        remove => Events.RemoveHandler(s_linkActivateEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.RichTextBoxIMEChange))]
    public event EventHandler? ImeChange
    {
        add => Events.AddHandler(s_imeChangeEvent, value);
        remove => Events.RemoveHandler(s_imeChangeEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.RichTextBoxProtected))]
    public event EventHandler? Protected
    {
        add => Events.AddHandler(s_protectedEvent, value);
        remove => Events.RemoveHandler(s_protectedEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.RichTextBoxSelChange))]
    public event EventHandler? SelectionChanged
    {
        add => Events.AddHandler(s_selectionChangeEvent, value);
        remove => Events.RemoveHandler(s_selectionChangeEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.RichTextBoxVScroll))]
    public event EventHandler? VScroll
    {
        add => Events.AddHandler(s_vscrollEvent, value);
        remove => Events.RemoveHandler(s_vscrollEvent, value);
    }

    /// <summary>
    ///  Returns a boolean indicating whether the RichTextBoxConstants control can paste the
    ///  given clipboard format.
    /// </summary>
    public bool CanPaste(DataFormats.Format clipFormat)
        => PInvoke.SendMessage(this, PInvoke.EM_CANPASTE, (WPARAM)clipFormat.Id) != 0;

    // DrawToBitmap doesn't work for this control, so we should hide it.  We'll
    // still call base so that this has a chance to work if it can.
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds)
    {
        base.DrawToBitmap(bitmap, targetBounds);
    }

    private unsafe int EditStreamProc(nint dwCookie, nint buf, int cb, out int transferred)
    {
        int ret = 0;    // assume that everything is Okay

        byte[] bytes = new byte[cb];

        int cookieVal = (int)dwCookie;

        transferred = 0;
        try
        {
            switch (cookieVal & DIRECTIONMASK)
            {
                case OUTPUT:
                    {
                        _editStream ??= new MemoryStream();

                        switch (cookieVal & KINDMASK)
                        {
                            case RTF:
                            case TEXTCRLF:
                                Marshal.Copy(buf, bytes, 0, cb);
                                _editStream.Write(bytes, 0, cb);
                                break;
                            case TEXTLF:
                                // Strip out \r characters so that we consistently return
                                // \n for linefeeds. In a future version the RichEdit control
                                // may support a SF_NOXLATCRLF flag which would do this for
                                // us. Internally the RichEdit stores the text with only
                                // a \n, so we want to keep that the same here.
                                //
                                if ((cookieVal & UNICODE) != 0)
                                {
                                    Debug.Assert(cb % 2 == 0, "EditStreamProc call out of cycle. Expected to always get character boundary calls");
                                    int requestedCharCount = cb / 2;
                                    int consumedCharCount = 0;

                                    fixed (byte* pb = bytes)
                                    {
                                        char* pChars = (char*)pb;
                                        char* pBuffer = (char*)(long)buf;

                                        for (int i = 0; i < requestedCharCount; i++)
                                        {
                                            if (*pBuffer == '\r')
                                            {
                                                pBuffer++;
                                                continue;
                                            }

                                            *pChars = *pBuffer;
                                            pChars++;
                                            pBuffer++;
                                            consumedCharCount++;
                                        }
                                    }

                                    _editStream.Write(bytes, 0, consumedCharCount * 2);
                                }
                                else
                                {
                                    int requestedCharCount = cb;
                                    int consumedCharCount = 0;

                                    fixed (byte* pb = bytes)
                                    {
                                        byte* pChars = pb;
                                        byte* pBuffer = (byte*)(long)buf;

                                        for (int i = 0; i < requestedCharCount; i++)
                                        {
                                            if (*pBuffer == (byte)'\r')
                                            {
                                                pBuffer++;
                                                continue;
                                            }

                                            *pChars = *pBuffer;
                                            pChars++;
                                            pBuffer++;
                                            consumedCharCount++;
                                        }
                                    }

                                    _editStream.Write(bytes, 0, consumedCharCount);
                                }

                                break;
                        }

                        // set up number of bytes transferred
                        transferred = cb;
                        break;
                    }

                case INPUT:
                    {
                        // Several customers complained that they were getting Random NullReference exceptions inside EditStreamProc.
                        // We had a case of a customer using Everett bits and another case of a customer using Whidbey Beta1 bits.
                        // We don't have a repro in house which makes it problematic to determine the cause for this behavior.
                        // Looking at the code it seems that the only possibility for editStream to be null is when the user
                        // calls RichTextBox::LoadFile(Stream, RichTextBoxStreamType) with a null Stream.
                        // However, the user said that his app is not using LoadFile method.
                        // The only possibility left open is that the native Edit control sends random calls into EditStreamProc.
                        // We have to guard against this.
                        if (_editStream is not null)
                        {
                            transferred = _editStream.Read(bytes, 0, cb);

                            Marshal.Copy(bytes, 0, buf, transferred);
                            // set up number of bytes transferred
                            if (transferred < 0)
                            {
                                transferred = 0;
                            }
                        }
                        else
                        {
                            // Set transferred to 0 so the native Edit controls knows that they should stop calling our EditStreamProc.
                            transferred = 0;
                        }

                        break;
                    }
            }
        }
#if DEBUG
        catch (IOException e)
        {
            Debug.Fail("Failed to edit proc operation.", e.ToString());
            transferred = 0;
            ret = 1;
        }
#else
        catch (IOException)
        {
            transferred = 0;
            ret = 1;
        }
#endif

        return ret;       // tell the RichTextBoxConstants how we are doing 0 - Okay, 1 - quit
    }

    /// <summary>
    ///  Searches the text in a RichTextBox control for a given string.
    /// </summary>
    public int Find(string str)
    {
        return Find(str, 0, 0, RichTextBoxFinds.None);
    }

    /// <summary>
    ///  Searches the text in a RichTextBox control for a given string.
    /// </summary>
    public int Find(string str, RichTextBoxFinds options)
    {
        return Find(str, 0, 0, options);
    }

    /// <summary>
    ///  Searches the text in a RichTextBox control for a given string.
    /// </summary>
    public int Find(string str, int start, RichTextBoxFinds options)
    {
        return Find(str, start, -1, options);
    }

    /// <summary>
    ///  Searches the text in a RichTextBox control for a given string.
    /// </summary>
    public unsafe int Find(string str, int start, int end, RichTextBoxFinds options)
    {
        ArgumentNullException.ThrowIfNull(str);

        // Perform the find, will return ubyte position
        int position = FindInternal(str, start, end, options);

        // if we didn't find anything, or we don't have to select what was found,
        // we're done
        bool selectWord = (options & RichTextBoxFinds.NoHighlight) != RichTextBoxFinds.NoHighlight;
        if (position != -1 && selectWord)
        {
            // Select the string found, this is done in ubyte units
            CHARRANGE chrg = new()
            {
                cpMin = position
            };

            // Look for kashidas in the string. A kashida is an arabic visual justification character
            // that's not semantically meaningful. Searching for ABC might find AB_C (where A,B, and C
            // represent Arabic characters and _ represents a kashida). We should highlight the text
            // including the kashida.
            const char kashida = (char)0x640;
            ReadOnlySpan<char> kashidaString = [kashida];

            // Using FindInternal here because RichEdit handles position/length differently than .NET strings
            // depending on characters and text formatting elements involved.
            int startIndex = FindInternal(kashidaString, position, position + str.Length, options);
            if (startIndex == -1)
            {
                // No kashida in the string
                chrg.cpMax = position + str.Length;
            }
            else
            {
                // There's at least one kashida
                int searchingCursor; // index into search string
                int foundCursor; // index into Text
                for (searchingCursor = startIndex, foundCursor = position + startIndex; searchingCursor < str.Length;
                    searchingCursor++, foundCursor++)
                {
                    while (FindInternal(kashidaString, foundCursor, foundCursor + 1, options) != -1 && str[searchingCursor] != kashida)
                    {
                        foundCursor++;
                    }
                }

                chrg.cpMax = foundCursor;
            }

            PInvoke.SendMessage(this, PInvoke.EM_EXSETSEL, 0, ref chrg);
            PInvoke.SendMessage(this, PInvoke.EM_SCROLLCARET);
        }

        return position;

        unsafe int FindInternal(ReadOnlySpan<char> str, int start, int end, RichTextBoxFinds options)
        {
            int textLen = TextLength;
            ArgumentOutOfRangeException.ThrowIfNegative(start);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(start, textLen);

            if (end < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(end), end, string.Format(SR.RichTextFindEndInvalid, end));
            }

            if (end == -1)
            {
                end = textLen;
            }

            if (start > end)
            {
                throw new ArgumentException(string.Format(SR.RichTextFindEndInvalid, end));
            }

            FINDTEXTW ft = default;
            if ((options & RichTextBoxFinds.Reverse) != RichTextBoxFinds.Reverse)
            {
                // normal
                ft.chrg.cpMin = start;
                ft.chrg.cpMax = end;
            }
            else
            {
                // reverse
                ft.chrg.cpMin = end;
                ft.chrg.cpMax = start;
            }

            // force complete search if we ended up with a zero length search
            if (ft.chrg.cpMin == ft.chrg.cpMax)
            {
                if ((options & RichTextBoxFinds.Reverse) != RichTextBoxFinds.Reverse)
                {
                    ft.chrg.cpMin = 0;
                    ft.chrg.cpMax = -1;
                }
                else
                {
                    ft.chrg.cpMin = textLen;
                    ft.chrg.cpMax = 0;
                }
            }

            // set up the options for the search
            FINDREPLACE_FLAGS findOptions = 0;
            if ((options & RichTextBoxFinds.WholeWord) == RichTextBoxFinds.WholeWord)
            {
                findOptions |= FINDREPLACE_FLAGS.FR_WHOLEWORD;
            }

            if ((options & RichTextBoxFinds.MatchCase) == RichTextBoxFinds.MatchCase)
            {
                findOptions |= FINDREPLACE_FLAGS.FR_MATCHCASE;
            }

            if ((options & RichTextBoxFinds.Reverse) != RichTextBoxFinds.Reverse)
            {
                // The default for RichEdit 2.0 is to search in reverse
                findOptions |= FINDREPLACE_FLAGS.FR_DOWN;
            }

            // Perform the find, will return ubyte position
            int position;
            fixed (char* pText = str)
            {
                ft.lpstrText = pText;
                position = (int)PInvoke.SendMessage(this, PInvoke.EM_FINDTEXT, (WPARAM)(uint)findOptions, ref ft);
            }

            return position;
        }
    }

    /// <summary>
    ///  Searches the text in a RichTextBox control for the given characters.
    /// </summary>
    public int Find(char[] characterSet)
    {
        return Find(characterSet, 0, -1);
    }

    /// <summary>
    ///  Searches the text in a RichTextBox control for the given characters.
    /// </summary>
    public int Find(char[] characterSet, int start)
    {
        return Find(characterSet, start, -1);
    }

    /// <summary>
    ///  Searches the text in a RichTextBox control for the given characters.
    /// </summary>
    public int Find(char[] characterSet, int start, int end)
    {
        // Code used to support ability to search backwards and negate character sets.
        // The API no longer supports this, but in case we change our mind, I'm leaving
        // the ability in here.
        bool forward = true;
        bool negate = false;

        int textLength = TextLength;

        ArgumentNullException.ThrowIfNull(characterSet);
        ArgumentOutOfRangeException.ThrowIfNegative(start);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(start, textLength);

        if (end != -1)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(end, start);
        }

        // Don't do anything if we get nothing to look for
        if (characterSet.Length == 0)
        {
            return -1;
        }

        textLength = PInvoke.GetWindowTextLength(this);
        if (start == end)
        {
            start = 0;
            end = textLength;
        }

        if (end == -1)
        {
            end = textLength;
        }

        CHARRANGE chrg = default; // The range of characters we have searched
        chrg.cpMax = chrg.cpMin = start;

        // Use the TEXTRANGE to move our text buffer forward
        // or backwards within the main text
        TEXTRANGE txrg = new()
        {
            chrg = new CHARRANGE
            {
                cpMin = chrg.cpMin,
                cpMax = chrg.cpMax
            }
        };

        // Characters we have slurped into memory in order to search
        UnicodeCharBuffer charBuffer = new(CHAR_BUFFER_LEN + 1);
        txrg.lpstrText = charBuffer.AllocCoTaskMem();
        if (txrg.lpstrText == 0)
        {
            throw new OutOfMemoryException();
        }

        try
        {
            bool done = false;

            // We want to loop as long as it takes.  This loop will grab a
            // chunk of text out from the control as directed by txrg.chrg;
            while (!done)
            {
                if (forward)
                {
                    // Move forward by starting at the end of the
                    // previous text window and extending by the
                    // size of our buffer
                    txrg.chrg.cpMin = chrg.cpMax;
                    txrg.chrg.cpMax += CHAR_BUFFER_LEN;
                }
                else
                {
                    // Move backwards by anchoring at the start
                    // of the previous buffer window, and backing
                    // up by the desired size of our buffer
                    txrg.chrg.cpMax = chrg.cpMin;
                    txrg.chrg.cpMin -= CHAR_BUFFER_LEN;

                    // We need to keep our request within the
                    // lower bound of zero
                    if (txrg.chrg.cpMin < 0)
                    {
                        txrg.chrg.cpMin = 0;
                    }
                }

                if (end != -1)
                {
                    txrg.chrg.cpMax = Math.Min(txrg.chrg.cpMax, end);
                }

                // go get the text in this range, if we didn't get any text then punt
                int len;
                len = (int)PInvoke.SendMessage(this, PInvoke.EM_GETTEXTRANGE, 0, ref txrg);
                if (len == 0)
                {
                    chrg.cpMax = chrg.cpMin = -1; // Hit end of control without finding what we wanted
                    break;
                }

                // get the data from RichTextBoxConstants into a string for us to use.
                charBuffer.PutCoTaskMem(txrg.lpstrText);
                string str = charBuffer.GetString();

                // Loop through our text
                if (forward)
                {
                    // Start at the beginning of the buffer
                    for (int x = 0; x < len; x++)
                    {
                        // Is it in char set?
                        bool found = GetCharInCharSet(str[x], characterSet, negate);

                        if (found)
                        {
                            done = true;
                            break;
                        }

                        // Advance the buffer
                        chrg.cpMax++;
                    }
                }
                else
                { // Span reverse.
                    int x = len;
                    while (x-- != 0)
                    {
                        // Is it in char set?
                        bool found = GetCharInCharSet(str[x], characterSet, negate);

                        if (found)
                        {
                            done = true;
                            break;
                        }

                        // Bring the selection back while keeping it anchored
                        chrg.cpMin--;
                    }
                }
            }
        }
        finally
        {
            // release the resources we got for our GETTEXTRANGE operation.
            if (txrg.lpstrText != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(txrg.lpstrText);
            }
        }

        int index = (forward) ? chrg.cpMax : chrg.cpMin;
        return index;
    }

    private void ForceHandleCreate()
    {
        if (!IsHandleCreated)
        {
            CreateHandle();
        }
    }

    // Sends set color message to HWND; doesn't call Control.SetForeColor
    private bool InternalSetForeColor(Color value)
    {
        CHARFORMAT2W cf = GetCharFormat(false);
        if ((cf.dwMask & CFM_MASK.CFM_COLOR) != 0
            && ColorTranslator.ToWin32(value) == cf.crTextColor)
        {
            return true;
        }

        cf.dwMask = CFM_MASK.CFM_COLOR;
        cf.dwEffects = 0;
        cf.crTextColor = ColorTranslator.ToWin32(value);
        return SetCharFormat(PInvoke.SCF_ALL, cf);
    }

    private unsafe CHARFORMAT2W GetCharFormat(bool fSelection)
    {
        CHARFORMAT2W cf = new()
        {
            cbSize = (uint)sizeof(CHARFORMAT2W)
        };

        PInvoke.SendMessage(this, PInvoke.EM_GETCHARFORMAT, (WPARAM)(fSelection ? PInvoke.SCF_SELECTION : PInvoke.SCF_DEFAULT), ref cf);
        return cf;
    }

    private RichTextBoxSelectionAttribute GetCharFormat(CFM_MASK mask, CFE_EFFECTS effect)
    {
        RichTextBoxSelectionAttribute charFormat = RichTextBoxSelectionAttribute.None;

        // check to see if the control has been created
        if (IsHandleCreated)
        {
            CHARFORMAT2W cf = GetCharFormat(true);
            // if the effects member contains valid info
            if ((cf.dwMask & mask) != 0)
            {
                // if the text has the desired effect
                if ((cf.dwEffects & effect) != 0)
                {
                    charFormat = RichTextBoxSelectionAttribute.All;
                }
            }
        }

        return charFormat;
    }

    private Font? GetCharFormatFont(bool selectionOnly)
    {
        ForceHandleCreate();

        CHARFORMAT2W cf = GetCharFormat(selectionOnly);
        if ((cf.dwMask & CFM_MASK.CFM_FACE) == 0)
        {
            return null;
        }

        float fontSize = 13;
        if ((cf.dwMask & CFM_MASK.CFM_SIZE) != 0)
        {
            fontSize = cf.yHeight / (float)20.0;
            if (fontSize == 0 && cf.yHeight > 0)
            {
                fontSize = 1;
            }
        }

        FontStyle style = FontStyle.Regular;
        if ((cf.dwMask & CFM_MASK.CFM_BOLD) != 0 && (cf.dwEffects & CFE_EFFECTS.CFE_BOLD) != 0)
        {
            style |= FontStyle.Bold;
        }

        if ((cf.dwMask & CFM_MASK.CFM_ITALIC) != 0 && (cf.dwEffects & CFE_EFFECTS.CFE_ITALIC) != 0)
        {
            style |= FontStyle.Italic;
        }

        if ((cf.dwMask & CFM_MASK.CFM_STRIKEOUT) != 0 && (cf.dwEffects & CFE_EFFECTS.CFE_STRIKEOUT) != 0)
        {
            style |= FontStyle.Strikeout;
        }

        if ((cf.dwMask & CFM_MASK.CFM_UNDERLINE) != 0 && (cf.dwEffects & CFE_EFFECTS.CFE_UNDERLINE) != 0)
        {
            style |= FontStyle.Underline;
        }

        try
        {
            return new Font(cf.FaceName.ToString(), fontSize, style, GraphicsUnit.Point, cf.bCharSet);
        }
        catch
        {
        }

        return null;
    }

    /// <summary>
    ///  Returns the index of the character nearest to the given point.
    /// </summary>
    public override int GetCharIndexFromPosition(Point pt)
    {
        Point wpt = new(pt.X, pt.Y);
        int index = (int)PInvoke.SendMessage(this, PInvoke.EM_CHARFROMPOS, 0, ref wpt);

        string t = Text;
        // EM_CHARFROMPOS will return an invalid number if the last character in the RichEdit
        // is a newline.
        //
        if (index >= t.Length)
        {
            index = Math.Max(t.Length - 1, 0);
        }

        return index;
    }

    private static bool GetCharInCharSet(char c, char[] charSet, bool negate)
    {
        bool match = false;
        int charSetLen = charSet.Length;

        // Loop through the given character set and compare for a match
        for (int i = 0; !match && i < charSetLen; i++)
        {
            match = c == charSet[i];
        }

        return negate ? !match : match;
    }

    /// <summary>
    ///  Returns the number of the line containing a specified character position
    ///  in a RichTextBox control. Note that this returns the physical line number
    ///  and not the conceptual line number. For example, if the first conceptual
    ///  line (line number 0) word-wraps and extends to the second line, and if
    ///  you pass the index of a overflowed character, GetLineFromCharIndex would
    ///  return 1 and not 0.
    /// </summary>
    public override int GetLineFromCharIndex(int index)
        => (int)PInvoke.SendMessage(this, PInvoke.EM_EXLINEFROMCHAR, 0, index);

    /// <summary>
    ///  Returns the location of the character at the given index.
    /// </summary>
    public override unsafe Point GetPositionFromCharIndex(int index)
    {
        if (s_richEditMajorVersion == 2)
        {
            return base.GetPositionFromCharIndex(index);
        }

        if (index < 0 || index > Text.Length)
        {
            return Point.Empty;
        }

        Point position = default;
        PInvoke.SendMessage(this, PInvoke.EM_POSFROMCHAR, (WPARAM)(&position), index);
        return position;
    }

    private bool GetProtectedError()
    {
        if (ProtectedError)
        {
            ProtectedError = false;
            return true;
        }

        return false;
    }

    /// <summary>
    ///  Loads the contents of the given RTF or text file into a RichTextBox control.
    /// </summary>
    public void LoadFile(string path)
    {
        LoadFile(path, RichTextBoxStreamType.RichText);
    }

    /// <summary>
    ///  Loads the contents of a RTF or text into a RichTextBox control.
    /// </summary>
    public void LoadFile(string path, RichTextBoxStreamType fileType)
    {
        // valid values are 0x0 to 0x4
        SourceGenerated.EnumValidator.Validate(fileType, nameof(fileType));

        FileStream file = new(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        try
        {
            LoadFile(file, fileType);
        }
        finally
        {
            file.Close();
        }
    }

    /// <summary>
    ///  Loads the contents of a RTF or text into a RichTextBox control.
    /// </summary>
    public void LoadFile(Stream data, RichTextBoxStreamType fileType)
    {
        ArgumentNullException.ThrowIfNull(data);

        SourceGenerated.EnumValidator.Validate(fileType, nameof(fileType));

        uint flags;
        switch (fileType)
        {
            case RichTextBoxStreamType.RichText:
                flags = PInvoke.SF_RTF;
                break;
            case RichTextBoxStreamType.PlainText:
                Rtf = string.Empty;
                flags = PInvoke.SF_TEXT;
                break;
            case RichTextBoxStreamType.UnicodePlainText:
                flags = PInvoke.SF_UNICODE | PInvoke.SF_TEXT;
                break;
            default:
                throw new ArgumentException(SR.InvalidFileType);
        }

        StreamIn(data, flags);
    }

    protected override void OnBackColorChanged(EventArgs e)
    {
        if (IsHandleCreated)
        {
            PInvoke.SendMessage(this, PInvoke.EM_SETBKGNDCOLOR, 0, BackColor.ToWin32());
        }

        base.OnBackColorChanged(e);
    }

    protected override void OnRightToLeftChanged(EventArgs e)
    {
        base.OnRightToLeftChanged(e);
        // When the RTL property is changed, here's what happens. Let's assume that we change from
        // RTL.No to RTL.Yes.

        // 1.   RecreateHandle is called.
        // 2.   In RTB.OnHandleDestroyed, we cache off any RTF that might have been set.
        //      The RTB has been set to the empty string, so we do get RTF back. The RTF
        //      contains formatting info, but doesn't contain any reading-order info,
        //      so RichEdit defaults to LTR reading order.
        // 3.   In RTB.OnHandleCreated, we check if we have any cached RTF, and if so,
        //      we want to set the RTF to that value. This is to ensure that the original
        //      text doesn't get lost.
        // 4.   In the RTF setter, we get the current RTF, compare it to the old RTF, and
        //      since those are not equal, we set the RichEdit content to the old RTF.
        // 5.   But... since the original RTF had no reading-order info, the reading-order
        //      will default to LTR.

        // That's why in Everett we set the text back since that clears the RTF, thus restoring
        // the reading order to that of the window style. The problem here is that when there's
        // no initial text (the empty string), then WindowText would not actually set the text,
        // and we were left with the LTR reading order. There's no longer any initial text (as in Everett,
        // e.g richTextBox1), since we changed the designers to not set text. Sigh...

        // So the fix is to force windowtext, whether or not that window text is equal to what's already there.
        // Note that in doing so we will lose any formatting info you might have set on the RTF. We are okay with that.

        // We use WindowText rather than Text because this way we can avoid
        // spurious TextChanged events.
        //
        string oldText = WindowText;
        ForceWindowText(null);
        ForceWindowText(oldText);
    }

    /// <summary>
    ///  Fires an event when the user changes the control's contents
    ///  are either smaller or larger than the control's window size.
    /// </summary>
    protected virtual void OnContentsResized(ContentsResizedEventArgs e)
    {
        ((ContentsResizedEventHandler?)Events[s_requestSizeEvent])?.Invoke(this, e);
    }

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);

        // Use parent's accessible object because RichTextBox doesn't support UIA Providers, and its
        // AccessibilityObject doesn't get created even when assistive tech (e.g. Narrator) is used
        if (Parent?.IsAccessibilityObjectCreated == true)
        {
            Parent.AccessibilityObject.InternalRaiseAutomationNotification(
                Automation.AutomationNotificationKind.Other,
                Automation.AutomationNotificationProcessing.MostRecent,
                Text);
        }
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        // base.OnHandleCreated is called somewhere in the middle of this

        _curSelStart = _curSelEnd = _curSelType = -1;

        // We will always set the control to use the maximum text, it defaults to 32k..
        // This must be done before we start loading files, because some files may
        // be larger than 32k.
        //
        UpdateMaxLength();

        // This is needed so that the control will fire change and update events
        // even if it is hidden
        PInvoke.SendMessage(
            this,
            PInvoke.EM_SETEVENTMASK,
            0,
            (nint)(PInvoke.ENM_PROTECTED | PInvoke.ENM_SELCHANGE |
                     PInvoke.ENM_DROPFILES | PInvoke.ENM_REQUESTRESIZE |
                     PInvoke.ENM_IMECHANGE | PInvoke.ENM_CHANGE |
                     PInvoke.ENM_UPDATE | PInvoke.ENM_SCROLL |
                     PInvoke.ENM_KEYEVENTS | PInvoke.ENM_MOUSEEVENTS |
                     PInvoke.ENM_SCROLLEVENTS | PInvoke.ENM_LINK));

        int rm = _rightMargin;
        _rightMargin = 0;
        RightMargin = rm;

        PInvoke.SendMessage(this, PInvoke.EM_AUTOURLDETECT, (WPARAM)(DetectUrls ? 1 : 0));
        if (_selectionBackColorToSetOnHandleCreated != Color.Empty)
        {
            SelectionBackColor = _selectionBackColorToSetOnHandleCreated;
        }

        // Initialize colors before initializing RTF, otherwise CFE_AUTOCOLOR will be in effect
        // and our text will all be Color.WindowText.
        bool autoWordSelection = AutoWordSelection;
        AutoWordSelection = autoWordSelection;

        PInvoke.SendMessage(this, PInvoke.EM_SETBKGNDCOLOR, (WPARAM)0, (LPARAM)BackColor);
        InternalSetForeColor(ForeColor);

        // base sets the Text property.  It's important to do this *after* setting EM_AUTOUrlDETECT.
        base.OnHandleCreated(e);

        // For some reason, we need to set the OleCallback before setting the RTF property.
        UpdateOleCallback();

        // RTF property takes precedence over Text property
        //
        try
        {
            SuppressTextChangedEvent = true;
            if (_textRtf is not null)
            {
                // setting RTF calls back on Text, which relies on textRTF being null
                string text = _textRtf;
                _textRtf = null;
                Rtf = text;
            }
            else if (_textPlain is not null)
            {
                string text = _textPlain;
                _textPlain = null;
                Text = text;
            }
        }
        finally
        {
            SuppressTextChangedEvent = false;
        }

        // Since we can't send EM_SETSEL until RTF has been set,
        // we can't rely on base to do it for us.
        SetSelectionOnHandle();

        if (ShowSelectionMargin)
        {
            // If you call SendMessage instead of PostMessage, the control
            // will resize itself to the size of the parent's client area.  Don't know why...
            PInvoke.PostMessage(
                this,
                PInvoke.EM_SETOPTIONS,
                (WPARAM)(int)PInvoke.ECOOP_OR,
                (LPARAM)(int)PInvoke.ECO_SELECTIONBAR);
        }

        if (_languageOption != LanguageOption)
        {
            LanguageOption = _languageOption;
        }

        ClearUndo();

        SendZoomFactor(_zoomMultiplier);

        SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(UserPreferenceChangedHandler);
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        base.OnHandleDestroyed(e);

        if (!InConstructor)
        {
            _textRtf = Rtf;
            if (_textRtf!.Length == 0)
            {
                _textRtf = null;
            }
        }

        _oleCallback = null;
        SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(UserPreferenceChangedHandler);
    }

    /// <summary>
    ///  Fires an event when the user clicks a RichTextBox control's horizontal
    ///  scroll bar.
    /// </summary>
    protected virtual void OnHScroll(EventArgs e)
    {
        ((EventHandler?)Events[s_hscrollEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Fires an event when the user clicks on a link
    ///  in a rich-edit control.
    /// </summary>
    protected virtual void OnLinkClicked(LinkClickedEventArgs e)
    {
        ((LinkClickedEventHandler?)Events[s_linkActivateEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Fires an event when the user changes the control's IME conversion status.
    /// </summary>
    protected virtual void OnImeChange(EventArgs e)
    {
        ((EventHandler?)Events[s_imeChangeEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Fires an event when the user is taking an action that would change
    ///  a protected range of text in the RichTextBox control.
    /// </summary>
    protected virtual void OnProtected(EventArgs e)
    {
        ProtectedError = true;
        ((EventHandler?)Events[s_protectedEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Fires an event when the current selection of text in the RichTextBox
    ///  control has changed or the insertion point has moved.
    /// </summary>
    protected virtual void OnSelectionChanged(EventArgs e)
    {
        ((EventHandler?)Events[s_selectionChangeEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Fires an event when the user clicks a RichTextBox control's vertical
    ///  scroll bar.
    /// </summary>
    protected virtual void OnVScroll(EventArgs e)
    {
        ((EventHandler?)Events[s_vscrollEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Pastes the contents of the clipboard in the given clipboard format.
    /// </summary>
    public void Paste(DataFormats.Format clipFormat)
    {
        PInvoke.SendMessage(this, PInvoke.EM_PASTESPECIAL, (WPARAM)clipFormat.Id);
    }

    protected override bool ProcessCmdKey(ref Message m, Keys keyData)
    {
        if (!RichTextShortcutsEnabled)
        {
            foreach (int shortcutValue in s_shortcutsToDisable!)
            {
                if ((int)keyData == shortcutValue)
                {
                    return true;
                }
            }
        }

        return base.ProcessCmdKey(ref m, keyData);
    }

    /// <summary>
    ///  Redoes the last undone editing operation.
    /// </summary>
    public void Redo() => PInvoke.SendMessage(this, PInvoke.EM_REDO);

    // NOTE: Undo is implemented on TextBox

    /// <summary>
    ///  Saves the contents of a RichTextBox control to a file.
    /// </summary>
    public void SaveFile(string path)
    {
        SaveFile(path, RichTextBoxStreamType.RichText);
    }

    /// <summary>
    ///  Saves the contents of a RichTextBox control to a file.
    /// </summary>
    public void SaveFile(string path, RichTextBoxStreamType fileType)
    {
        // valid values are 0x0 to 0x4
        SourceGenerated.EnumValidator.Validate(fileType, nameof(fileType));

        FileStream file = File.Create(path);
        try
        {
            SaveFile(file, fileType);
        }
        finally
        {
            file.Close();
        }
    }

    /// <summary>
    ///  Saves the contents of a RichTextBox control to a file.
    /// </summary>
    public void SaveFile(Stream data, RichTextBoxStreamType fileType)
    {
        uint flags = fileType switch
        {
            RichTextBoxStreamType.RichText => PInvoke.SF_RTF,
            RichTextBoxStreamType.PlainText => PInvoke.SF_TEXT,
            RichTextBoxStreamType.UnicodePlainText => PInvoke.SF_UNICODE | PInvoke.SF_TEXT,
            RichTextBoxStreamType.RichNoOleObjs => PInvoke.SF_RTFNOOBJS,
            RichTextBoxStreamType.TextTextOleObjs => PInvoke.SF_TEXTIZED,
            _ => throw new InvalidEnumArgumentException(nameof(fileType), (int)fileType, typeof(RichTextBoxStreamType)),
        };

        StreamOut(data, flags, includeCrLfs: true);
    }

    /// <summary>
    ///  Core Zoom calculation and message passing (used by ZoomFactor property and CreateHandle()
    /// </summary>
    private void SendZoomFactor(float zoom)
    {
        int numerator;
        int denominator;

        if (zoom == 1.0f)
        {
            denominator = 0;
            numerator = 0;
        }
        else
        {
            denominator = 1000;
            float multiplier = 1000 * zoom;
            numerator = (int)Math.Ceiling(multiplier);
            if (numerator >= 64000)
            {
                numerator = (int)Math.Floor(multiplier);
            }
        }

        if (IsHandleCreated)
        {
            PInvoke.SendMessage(this, PInvoke.EM_SETZOOM, (WPARAM)numerator, (LPARAM)denominator);
        }

        if (numerator != 0)
        {
            _zoomMultiplier = numerator / ((float)denominator);
        }
        else
        {
            _zoomMultiplier = 1.0f;
        }
    }

    private unsafe bool SetCharFormat(CFM_MASK mask, CFE_EFFECTS effect, RichTextBoxSelectionAttribute charFormat)
    {
        // check to see if the control has been created
        if (IsHandleCreated)
        {
            CHARFORMAT2W cf = new()
            {
                cbSize = (uint)sizeof(CHARFORMAT2W),
                dwMask = mask,
                dwEffects = charFormat switch
                {
                    RichTextBoxSelectionAttribute.All => effect,
                    RichTextBoxSelectionAttribute.None => 0,
                    _ => throw new ArgumentException(SR.UnknownAttr),
                }
            };

            // Set the format information.
            return PInvoke.SendMessage(this, PInvoke.EM_SETCHARFORMAT, (WPARAM)PInvoke.SCF_SELECTION, ref cf) != 0;
        }

        return false;
    }

    private bool SetCharFormat(uint charRange, CHARFORMAT2W cf)
    {
        return PInvoke.SendMessage(this, PInvoke.EM_SETCHARFORMAT, (WPARAM)charRange, ref cf) != 0;
    }

    private unsafe void SetCharFormatFont(bool selectionOnly, Font value)
    {
        ForceHandleCreate();

        CFM_MASK dwMask = CFM_MASK.CFM_FACE | CFM_MASK.CFM_SIZE | CFM_MASK.CFM_BOLD |
            CFM_MASK.CFM_ITALIC | CFM_MASK.CFM_STRIKEOUT | CFM_MASK.CFM_UNDERLINE |
            CFM_MASK.CFM_CHARSET;

        CFE_EFFECTS dwEffects = 0;
        if (value.Bold)
        {
            dwEffects |= CFE_EFFECTS.CFE_BOLD;
        }

        if (value.Italic)
        {
            dwEffects |= CFE_EFFECTS.CFE_ITALIC;
        }

        if (value.Strikeout)
        {
            dwEffects |= CFE_EFFECTS.CFE_STRIKEOUT;
        }

        if (value.Underline)
        {
            dwEffects |= CFE_EFFECTS.CFE_UNDERLINE;
        }

        LOGFONTW logFont = value.ToLogicalFont();
        CHARFORMAT2W charFormat = new()
        {
            cbSize = (uint)sizeof(CHARFORMAT2W),
            dwMask = dwMask,
            dwEffects = dwEffects,
            yHeight = (int)(value.SizeInPoints * 20),
            bCharSet = (byte)logFont.lfCharSet,
            bPitchAndFamily = logFont.lfPitchAndFamily,
            FaceName = logFont.FaceName
        };

        PInvoke.SendMessage(
            this,
            PInvoke.EM_SETCHARFORMAT,
            (WPARAM)(selectionOnly ? PInvoke.SCF_SELECTION : PInvoke.SCF_ALL),
            ref charFormat);
    }

    private static void SetupLogPixels()
    {
        using var dc = GetDcScope.ScreenDC;
        s_logPixelsX = PInvokeCore.GetDeviceCaps(dc, GET_DEVICE_CAPS_INDEX.LOGPIXELSX);
        s_logPixelsY = PInvokeCore.GetDeviceCaps(dc, GET_DEVICE_CAPS_INDEX.LOGPIXELSY);
    }

    private static int Pixel2Twip(int v, bool xDirection)
    {
        SetupLogPixels();
        int logP = xDirection ? s_logPixelsX : s_logPixelsY;
        return (int)((((double)v) / logP) * 72.0 * 20.0);
    }

    private static int Twip2Pixel(int v, bool xDirection)
    {
        SetupLogPixels();
        int logP = xDirection ? s_logPixelsX : s_logPixelsY;
        return (int)(((v / 20.0) / 72.0) * logP);
    }

    private void StreamIn(string str, uint flags)
    {
        if (str.Length == 0)
        {
            // Destroy the selection if callers was setting selection text
            if ((PInvoke.SFF_SELECTION & flags) != 0)
            {
                PInvoke.SendMessage(this, PInvoke.WM_CLEAR);
                ProtectedError = false;
                return;
            }

            // WM_SETTEXT is allowed even if we have protected text
            PInvoke.SendMessage(this, PInvoke.WM_SETTEXT, 0, string.Empty);
            return;
        }

        // Rather than work only some of the time with null characters,
        // we're going to be consistent and never work with them.
        int nullTerminatedLength = str.IndexOf((char)0);
        if (nullTerminatedLength != -1)
        {
            str = str[..nullTerminatedLength];
        }

        // Get the string into a byte array
        byte[] encodedBytes;
        if ((flags & PInvoke.SF_UNICODE) != 0)
        {
            encodedBytes = Encoding.Unicode.GetBytes(str);
        }
        else
        {
            // Encode using the default code page.
            encodedBytes = (CodePagesEncodingProvider.Instance.GetEncoding(0) ?? Encoding.UTF8).GetBytes(str);
        }

        _editStream = new MemoryStream(encodedBytes.Length);
        _editStream.Write(encodedBytes, 0, encodedBytes.Length);
        _editStream.Position = 0;
        StreamIn(_editStream, flags);
    }

    private void StreamIn(Stream data, uint flags)
    {
        // Clear out the selection only if we are replacing all the text.
        if ((flags & PInvoke.SFF_SELECTION) == 0)
        {
            CHARRANGE range = default;
            PInvoke.SendMessage(this, PInvoke.EM_EXSETSEL, 0, ref range);
        }

        try
        {
            _editStream = data;
            Debug.Assert(data is not null, "StreamIn passed a null stream");

            // If SF_RTF is requested then check for the RTF tag at the start
            // of the file.  We don't load if the tag is not there.

            if ((flags & PInvoke.SF_RTF) != 0)
            {
                long streamStart = _editStream.Position;
                byte[] bytes = new byte[SZ_RTF_TAG.Length];
                _editStream.Read(bytes, (int)streamStart, SZ_RTF_TAG.Length);

                // Encode using the default encoding.
                string str = (CodePagesEncodingProvider.Instance.GetEncoding(0) ?? Encoding.UTF8).GetString(bytes);
                if (!SZ_RTF_TAG.Equals(str))
                {
                    throw new ArgumentException(SR.InvalidFileFormat);
                }

                // put us back at the start of the file
                _editStream.Position = streamStart;
            }

            int cookieVal = 0;

            // set up structure to do stream operation
            EDITSTREAM es = default;

            if ((flags & PInvoke.SF_UNICODE) != 0)
            {
                cookieVal = INPUT | UNICODE;
            }
            else
            {
                cookieVal = INPUT | ANSI;
            }

            if ((flags & PInvoke.SF_RTF) != 0)
            {
                cookieVal |= RTF;
            }
            else
            {
                cookieVal |= TEXTLF;
            }

            es.dwCookie = (UIntPtr)cookieVal;
            EDITSTREAMCALLBACK callback = EditStreamProc;
            es.pfnCallback = Marshal.GetFunctionPointerForDelegate(callback);

            // gives us TextBox compatible behavior, programatic text change shouldn't
            // be limited...
            PInvoke.SendMessage(this, PInvoke.EM_EXLIMITTEXT, 0, int.MaxValue);

            // go get the text for the control
            PInvoke.SendMessage(this, PInvoke.EM_STREAMIN, (WPARAM)flags, ref es);
            GC.KeepAlive(callback);

            UpdateMaxLength();

            // If we failed to load because of protected
            // text then return protect event was fired so no
            // exception is required for the error
            if (GetProtectedError())
            {
                return;
            }

            if (es.dwError != 0)
            {
                throw new InvalidOperationException(SR.LoadTextError);
            }

            // set the modify tag on the control
            PInvoke.SendMessage(this, PInvoke.EM_SETMODIFY, (WPARAM)(-1));

            // EM_GETLINECOUNT will cause the RichTextBox to recalculate its line indexes
            PInvoke.SendMessage(this, PInvoke.EM_GETLINECOUNT);
        }
        finally
        {
            // release any storage space held.
            _editStream = null;
        }
    }

    private string StreamOut(uint flags)
    {
        MemoryStream stream = new();
        StreamOut(stream, flags, false);
        stream.Position = 0;
        int streamLength = (int)stream.Length;
        string result = string.Empty;

        if (streamLength > 0)
        {
            byte[] bytes = new byte[streamLength];
            stream.Read(bytes, 0, streamLength);

            if ((flags & PInvoke.SF_UNICODE) != 0)
            {
                result = Encoding.Unicode.GetString(bytes, 0, bytes.Length);
            }
            else
            {
                // Convert from the current code page
                result = (CodePagesEncodingProvider.Instance.GetEncoding(0) ?? Encoding.UTF8).GetString(bytes, 0, bytes.Length);
            }

            // Trimming off a null char is usually a sign of incorrect marshalling.
            // We should consider removing this in the future, but it would need to
            // be checked against input strings with a trailing null.

            if (!string.IsNullOrEmpty(result) && (result[^1] == '\0'))
            {
                result = result[..^1];
            }
        }

        return result;
    }

    private void StreamOut(Stream data, uint flags, bool includeCrLfs)
    {
        // set up the EDITSTREAM structure for the callback.
        _editStream = data;

        try
        {
            int cookieVal = 0;
            EDITSTREAM es = default;

            cookieVal = (flags & PInvoke.SF_UNICODE) != 0 ? OUTPUT | UNICODE : OUTPUT | ANSI;

            if ((flags & PInvoke.SF_RTF) != 0)
            {
                cookieVal |= RTF;
            }
            else
            {
                if (includeCrLfs)
                {
                    cookieVal |= TEXTCRLF;
                }
                else
                {
                    cookieVal |= TEXTLF;
                }
            }

            es.dwCookie = (UIntPtr)cookieVal;
            EDITSTREAMCALLBACK callback = EditStreamProc;
            es.pfnCallback = Marshal.GetFunctionPointerForDelegate(callback);

            // Get Text
            PInvoke.SendMessage(this, PInvoke.EM_STREAMOUT, (WPARAM)flags, ref es);
            GC.KeepAlive(callback);

            // check to make sure things went well
            if (es.dwError != 0)
            {
                throw new InvalidOperationException(SR.SaveTextError);
            }
        }
        finally
        {
            // release any storage space held.
            _editStream = null;
        }
    }

    private unsafe string GetTextEx(GETTEXTEX_FLAGS flags = GETTEXTEX_FLAGS.GT_DEFAULT)
    {
        Debug.Assert(IsHandleCreated);

        // Unicode UTF-16, little endian byte order (BMP of ISO 10646); available only to managed applications
        // https://docs.microsoft.com/windows/win32/intl/code-page-identifiers
        const int UNICODE = 1200;

        GETTEXTLENGTHEX gtl = new GETTEXTLENGTHEX
        {
            codepage = UNICODE,
            flags = GETTEXTLENGTHEX_FLAGS.GTL_DEFAULT
        };

        if (flags.HasFlag(GETTEXTEX_FLAGS.GT_USECRLF))
        {
            gtl.flags |= GETTEXTLENGTHEX_FLAGS.GTL_USECRLF;
        }

        GETTEXTLENGTHEX* pGtl = &gtl;
        int expectedLength = (int)PInvoke.SendMessage(this, PInvoke.EM_GETTEXTLENGTHEX, (WPARAM)pGtl);
        if (expectedLength == (int)HRESULT.E_INVALIDARG)
            throw new Win32Exception(expectedLength);

        // buffer has to have enough space for final \0. Without this, the last character is missing!
        // in case flags contains GT_SELECTION we'll allocate too much memory (for the whole text and not just the selection),
        // but there's no appropriate flag for EM_GETTEXTLENGTHEX
        int maxLength = (expectedLength + 1) * sizeof(char);

        GETTEXTEX gt = new GETTEXTEX
        {
            cb = (uint)maxLength,
            flags = flags,
            codepage = UNICODE,
        };

        BufferScope<char> buffer = new(maxLength);
        GETTEXTEX* pGt = &gt;
        fixed (char* b = buffer)
        {
            int actualLength = (int)PInvoke.SendMessage(this, PInvoke.EM_GETTEXTEX, (WPARAM)pGt, (LPARAM)b);

            // The default behaviour of EM_GETTEXTEX is to normalise line endings to '\r'
            // (see: GT_DEFAULT, https://docs.microsoft.com/windows/win32/api/richedit/ns-richedit-gettextex#members),
            // whereas previously we would normalise to '\n'. Unfortunately we can only ask for '\r\n' line endings via GT.USECRLF,
            // but unable to ask for '\n'. Unless GT.USECRLF was set, convert '\r' with '\n' to retain the original behaviour.
            if (!flags.HasFlag(GETTEXTEX_FLAGS.GT_USECRLF))
            {
                int index = 0;
                while (index < actualLength)
                {
                    if (b[index] == '\r')
                    {
                        b[index] = '\n';
                    }

                    index++;
                }
            }

            return buffer[..actualLength].ToString();
        }
    }

    private void UpdateOleCallback()
    {
        if (!IsHandleCreated)
        {
            return;
        }

        if (_oleCallback is null)
        {
            AllowOleObjects = true;

            _oleCallback = CreateRichEditOleCallback();
            using var oleCallback = ComHelpers.GetComScope<IRichEditOleCallback>(_oleCallback);
            PInvoke.SendMessage(this, PInvoke.EM_SETOLECALLBACK, 0, (nint)oleCallback);
        }

        PInvoke.DragAcceptFiles(this, fAccept: false);
    }

    // Note: RichTextBox doesn't work like other controls as far as setting ForeColor/
    // BackColor -- you need to send messages to update the colors
    private void UserPreferenceChangedHandler(object o, UserPreferenceChangedEventArgs e)
    {
        if (IsHandleCreated)
        {
            if (BackColor.IsSystemColor)
            {
                PInvoke.SendMessage(this, PInvoke.EM_SETBKGNDCOLOR, 0, BackColor.ToWin32());
            }

            if (ForeColor.IsSystemColor)
            {
                InternalSetForeColor(ForeColor);
            }
        }
    }

    protected override AccessibleObject CreateAccessibilityInstance() => new ControlAccessibleObject(this);

    /// <summary>
    ///  Creates the IRichEditOleCallback compatible object for handling RichEdit callbacks. For more
    ///  information look up the MSDN info on this interface. This is designed to be a back door of
    ///  sorts, which is why it is fairly obscure, and uses the RichEdit name instead of RichTextBox.
    /// </summary>
    protected virtual object CreateRichEditOleCallback() => new OleCallback(this);

    /// <summary>
    ///  Handles link messages (mouse move, down, up, dblclk, etc)
    /// </summary>
    private unsafe void EnLinkMsgHandler(ref Message m)
    {
        ENLINK enlink;
        enlink = *(ENLINK*)(nint)m.LParamInternal;

        switch ((uint)enlink.msg)
        {
            case PInvoke.WM_SETCURSOR:
                LinkCursor = true;
                m.ResultInternal = (LRESULT)1;
                return;
            // Mouse-down triggers Url; this matches Outlook 2000's behavior.
            case PInvoke.WM_LBUTTONDOWN:
                string linktext = CharRangeToString(enlink.charrange);
                if (!string.IsNullOrEmpty(linktext))
                {
                    OnLinkClicked(new LinkClickedEventArgs(linktext, enlink.charrange.cpMin, enlink.charrange.cpMax - enlink.charrange.cpMin));
                }

                m.ResultInternal = (LRESULT)1;
                return;
        }

        m.ResultInternal = (LRESULT)0;
        return;
    }

    /// <summary>
    ///  Converts a CHARRANGE to a string.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The behavior of this is dependent on the current window class name being used.
    ///   We have to create a CharBuffer of the type of RichTextBox DLL we're using,
    ///   not based on the SystemCharWidth.
    ///  </para>
    /// </remarks>
    private string CharRangeToString(CHARRANGE c)
    {
        TEXTRANGE txrg = new()
        {
            chrg = c
        };

        Debug.Assert((c.cpMax - c.cpMin) > 0, "CHARRANGE was null or negative - can't do it!");
        if (c.cpMax - c.cpMin <= 0)
        {
            return string.Empty;
        }

        int characters = (c.cpMax - c.cpMin) + 1; // +1 for null termination
        UnicodeCharBuffer charBuffer = new(characters);
        nint unmanagedBuffer = charBuffer.AllocCoTaskMem();
        if (unmanagedBuffer == 0)
        {
            throw new OutOfMemoryException(SR.OutOfMemory);
        }

        txrg.lpstrText = unmanagedBuffer;
        int len = (int)PInvoke.SendMessage(this, PInvoke.EM_GETTEXTRANGE, 0, ref txrg);
        Debug.Assert(len != 0, "CHARRANGE from RichTextBox was bad! - impossible?");
        charBuffer.PutCoTaskMem(unmanagedBuffer);
        if (txrg.lpstrText != 0)
        {
            Marshal.FreeCoTaskMem(unmanagedBuffer);
        }

        string result = charBuffer.GetString();
        return result;
    }

    internal override void UpdateMaxLength()
    {
        if (IsHandleCreated)
        {
            PInvoke.SendMessage(this, PInvoke.EM_EXLIMITTEXT, 0, (IntPtr)MaxLength);
        }
    }

    private void WmReflectCommand(ref Message m)
    {
        // We check if we're in the middle of handle creation because
        // the rich edit control fires spurious events during this time.
        if (m.LParamInternal == Handle && !GetState(States.CreatingHandle))
        {
            switch ((uint)m.WParamInternal.HIWORD)
            {
                case PInvoke.EN_HSCROLL:
                    OnHScroll(EventArgs.Empty);
                    break;
                case PInvoke.EN_VSCROLL:
                    OnVScroll(EventArgs.Empty);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
        else
        {
            base.WndProc(ref m);
        }
    }

    internal unsafe void WmReflectNotify(ref Message m)
    {
        if (m.HWnd != Handle)
        {
            base.WndProc(ref m);
            return;
        }

        NMHDR* nmhdr = (NMHDR*)(nint)m.LParamInternal;
        switch (nmhdr->code)
        {
            case PInvoke.EN_LINK:
                EnLinkMsgHandler(ref m);
                break;
            case PInvoke.EN_DROPFILES:
                HDROP endropfiles = (HDROP)((ENDROPFILES*)m.LParamInternal)->hDrop;

                // Only look at the first file.
                using (BufferScope<char> buffer = new(PInvoke.MAX_PATH + 1))
                {
                    fixed (char* b = buffer)
                    {
                        uint length = PInvoke.DragQueryFile(endropfiles, iFile: 0, b, cch: (uint)buffer.Length);
                        if (length != 0)
                        {
                            // Try to load the file as RTF.
                            string path = buffer[..(int)length].ToString();

                            try
                            {
                                LoadFile(path, RichTextBoxStreamType.RichText);
                            }
                            catch
                            {
                                // We failed to load as rich text, try again as plain text.
                                try
                                {
                                    LoadFile(path, RichTextBoxStreamType.PlainText);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }

                // Confirm that we did the drop
                m.ResultInternal = (LRESULT)1;
                break;

            case PInvoke.EN_REQUESTRESIZE:
                if (!CallOnContentsResized)
                {
                    REQRESIZE* reqResize = (REQRESIZE*)(nint)m.LParamInternal;
                    if (BorderStyle == BorderStyle.Fixed3D)
                    {
                        reqResize->rc.bottom++;
                    }

                    OnContentsResized(new ContentsResizedEventArgs(reqResize->rc));
                }

                break;

            case PInvoke.EN_SELCHANGE:
                SELCHANGE* selChange = (SELCHANGE*)(nint)m.LParamInternal;
                WmSelectionChange(*selChange);
                break;

            case PInvoke.EN_PROTECTED:
                {
                    ENPROTECTED enprotected;

                    enprotected = *(ENPROTECTED*)(nint)m.LParamInternal;

                    switch ((uint)enprotected.msg)
                    {
                        case PInvoke.EM_SETCHARFORMAT:
                            // Allow change of protected style
                            CHARFORMAT2W* charFormat = (CHARFORMAT2W*)enprotected.lParam;
                            if ((charFormat->dwMask & CFM_MASK.CFM_PROTECTED) != 0)
                            {
                                m.ResultInternal = (LRESULT)0;
                                return;
                            }

                            break;

                        // Throw an exception for the following
                        case PInvoke.EM_SETPARAFORMAT:
                        case PInvoke.EM_REPLACESEL:
                            break;

                        case PInvoke.EM_STREAMIN:
                            // Don't allow STREAMIN to replace protected selection
                            if ((unchecked((uint)(long)enprotected.wParam) & PInvoke.SFF_SELECTION) != 0)
                            {
                                break;
                            }

                            m.ResultInternal = (LRESULT)0;
                            return;

                        // Allow the following
                        case PInvoke.WM_COPY:
                        case PInvoke.WM_SETTEXT:
                        case PInvoke.EM_EXLIMITTEXT:
                            m.ResultInternal = (LRESULT)0;
                            return;

                        // Beep and disallow change for all other messages
                        default:
                            PInvoke.MessageBeep(MESSAGEBOX_STYLE.MB_OK);
                            break;
                    }

                    OnProtected(EventArgs.Empty);
                    m.ResultInternal = (LRESULT)1;
                    break;
                }

            default:
                base.WndProc(ref m);
                break;
        }
    }

    private void WmSelectionChange(SELCHANGE selChange)
    {
        int selStart = selChange.chrg.cpMin;
        int selEnd = selChange.chrg.cpMax;
        short selType = (short)selChange.seltyp;

        // The IME retains characters in the composition window even after MaxLength
        // has been reached in the rich edit control. So, if the Hangul or HangulFull IME is in use, and the
        // number of characters in the control is equal to MaxLength, and the selection start equals the
        // selection end (nothing is currently selected), then kill and restore focus to the control. Then,
        // to prevent any further partial composition from occurring, post a message back to myself to select
        // the last character being composed so that any further composition will occur within the context of
        // the string contained within the control.
        //
        // Since the IME window completes the composition string when the control loses focus and the
        // EIMES_COMPLETECOMPSTRKILLFOCUS status type is set in the control by the EM_SETIMESTATUS message,
        // simply killing focus and resetting focus to the control will force the contents of the composition
        // window to be removed. This forces the undo buffer to be emptied and the backspace key will properly
        // remove the last completed character typed.

        // Is either the Hangul or HangulFull IME currently in use?
        if (ImeMode is ImeMode.Hangul or ImeMode.HangulFull)
        {
            // Is the IME CompositionWindow open?
            LRESULT compMode = PInvoke.SendMessage(this, PInvoke.EM_GETIMECOMPMODE);
            if (compMode != PInvoke.ICM_NOTOPEN)
            {
                int textLength = PInvoke.GetWindowTextLength(this);
                if (selStart == selEnd && textLength == MaxLength)
                {
                    PInvoke.SendMessage(this, PInvoke.WM_KILLFOCUS);
                    PInvoke.SendMessage(this, PInvoke.WM_SETFOCUS);
                    PInvoke.PostMessage(this, PInvoke.EM_SETSEL, (WPARAM)(selEnd - 1), (LPARAM)selEnd);
                }
            }
        }

        if (selStart != _curSelStart || selEnd != _curSelEnd || selType != _curSelType)
        {
            _curSelStart = selStart;
            _curSelEnd = selEnd;
            _curSelType = selType;
            OnSelectionChanged(EventArgs.Empty);
        }
    }

    private void WmSetFont(ref Message m)
    {
        // This function would normally cause two TextChanged events to be fired, one
        // from the base.WndProc, and another from InternalSetForeColor.
        // To prevent this, we suppress the first event fire.
        //
        try
        {
            SuppressTextChangedEvent = true;
            base.WndProc(ref m);
        }
        finally
        {
            SuppressTextChangedEvent = false;
        }

        InternalSetForeColor(ForeColor);
    }

    protected override void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            case MessageId.WM_REFLECT_NOTIFY:
                WmReflectNotify(ref m);
                break;

            case MessageId.WM_REFLECT_COMMAND:
                WmReflectCommand(ref m);
                break;

            case PInvoke.WM_SETCURSOR:
                // NOTE: RichTextBox uses the WM_SETCURSOR message over links to allow us to
                //      change the cursor to a hand. It does this through a synchronous notification
                //      message. So we have to pass the message to the DefWndProc first, and
                //      then, if we receive a notification message in the meantime (indicated by
                //      changing "LinkCursor", we set it to a hand. Otherwise, we call the
                //      WM_SETCURSOR implementation on Control to set it to the user's selection for
                //      the RichTextBox's cursor.
                LinkCursor = false;
                DefWndProc(ref m);
                if (LinkCursor && !Cursor.Equals(Cursors.WaitCursor))
                {
                    Cursor.Current = Cursors.Hand;
                    m.ResultInternal = (LRESULT)1;
                }
                else
                {
                    base.WndProc(ref m);
                }

                break;

            case PInvoke.WM_SETFONT:
                WmSetFont(ref m);
                break;

            case PInvoke.WM_IME_NOTIFY:
                OnImeChange(EventArgs.Empty);
                base.WndProc(ref m);
                break;

            case PInvoke.WM_GETDLGCODE:
                base.WndProc(ref m);
                m.ResultInternal = (LRESULT)(AcceptsTab ? m.ResultInternal | (nint)PInvoke.DLGC_WANTTAB : m.ResultInternal & ~(nint)PInvoke.DLGC_WANTTAB);
                break;

            case PInvoke.WM_GETOBJECT:
                base.WndProc(ref m);

                // OLEACC.DLL uses window class names to identify standard control types. But WinForm controls use app-specific window
                // classes. Usually this doesn't matter, because system controls always identify their window class explicitly through
                // the WM_GETOBJECT+OBJID_QUERYCLASSNAMEIDX message. But RICHEDIT20 doesn't do that - so we must do it ourselves.
                // Otherwise OLEACC will treat rich edit controls as custom controls, so the accessible Role and Value will be wrong.
                if ((int)m.LParamInternal == (int)OBJECT_IDENTIFIER.OBJID_QUERYCLASSNAMEIDX)
                {
                    m.ResultInternal = (LRESULT)(65536 + 30);
                }

                break;

            case PInvoke.WM_RBUTTONUP:
                // since RichEdit eats up the WM_CONTEXTMENU message, we need to force DefWndProc
                // to spit out this message again on receiving WM_RBUTTONUP message. By setting UserMouse
                // style to true, we effectively let the WmMouseUp method in Control.cs to generate
                // the WM_CONTEXTMENU message for us.
                bool oldStyle = GetStyle(ControlStyles.UserMouse);
                SetStyle(ControlStyles.UserMouse, true);
                base.WndProc(ref m);
                SetStyle(ControlStyles.UserMouse, oldStyle);
                break;

            case PInvoke.WM_VSCROLL:
                {
                    base.WndProc(ref m);
                    SCROLLBAR_COMMAND loWord = (SCROLLBAR_COMMAND)m.WParamInternal.LOWORD;
                    if (loWord == SCROLLBAR_COMMAND.SB_THUMBTRACK)
                    {
                        OnVScroll(EventArgs.Empty);
                    }
                    else if (loWord == SCROLLBAR_COMMAND.SB_THUMBPOSITION)
                    {
                        OnVScroll(EventArgs.Empty);
                    }

                    break;
                }

            case PInvoke.WM_HSCROLL:
                {
                    base.WndProc(ref m);
                    SCROLLBAR_COMMAND loWord = (SCROLLBAR_COMMAND)m.WParamInternal.LOWORD;
                    if (loWord == SCROLLBAR_COMMAND.SB_THUMBTRACK)
                    {
                        OnHScroll(EventArgs.Empty);
                    }
                    else if (loWord == SCROLLBAR_COMMAND.SB_THUMBPOSITION)
                    {
                        OnHScroll(EventArgs.Empty);
                    }

                    break;
                }

            default:
                base.WndProc(ref m);
                break;
        }
    }
}
