// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms.Layout;
using Microsoft.Win32;
using static Interop;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
using Util = System.Windows.Forms.NativeMethods.Util;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Rich Text control. The RichTextBox is a control that contains formatted text.
    ///  It supports font selection, boldface, and other type attributes.
    /// </summary>
    [ClassInterface(ClassInterfaceType.AutoDispatch),
     ComVisible(true),
     Docking(DockingBehavior.Ask),
     Designer("System.Windows.Forms.Design.RichTextBoxDesigner, " + AssemblyRef.SystemDesign),
     SRDescription(nameof(SR.DescriptionRichTextBox))
   ]
    public class RichTextBox : TextBoxBase
    {
        static TraceSwitch richTextDbg;
        static TraceSwitch RichTextDbg
        {
            get
            {
                if (richTextDbg == null)
                {
                    richTextDbg = new TraceSwitch("RichTextDbg", "Debug info about RichTextBox");
                }
                return richTextDbg;
            }
        }

        /// <summary>
        ///  Paste special flags.
        /// </summary>
        private const int DV_E_DVASPECT = unchecked((int)0x8004006B);
        private const int DVASPECT_CONTENT = 1;
        private const int DVASPECT_THUMBNAIL = 2;
        private const int DVASPECT_ICON = 4;
        private const int DVASPECT_DOCPRINT = 8;

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

        // This is where we store the reched library.
        private static IntPtr moduleHandle;

        private static readonly string SZ_RTF_TAG = "{\\rtf";
        private const int CHAR_BUFFER_LEN = 512;

        // Event objects
        //
        private static readonly object EVENT_HSCROLL = new object();
        private static readonly object EVENT_LINKACTIVATE = new object();
        private static readonly object EVENT_IMECHANGE = new object();
        private static readonly object EVENT_PROTECTED = new object();
        private static readonly object EVENT_REQUESTRESIZE = new object();
        private static readonly object EVENT_SELCHANGE = new object();
        private static readonly object EVENT_VSCROLL = new object();

        // Persistent state
        //
        private int bulletIndent;
        private int rightMargin;
        private string textRtf; // If not null, takes precedence over cached Text value
        private string textPlain;
        private Color selectionBackColorToSetOnHandleCreated;
        RichTextBoxLanguageOptions languageOption = RichTextBoxLanguageOptions.AutoFont | RichTextBoxLanguageOptions.DualFont;

        // Non-persistent state
        //
        static int logPixelsX;
        static int logPixelsY;
        Stream editStream = null;
        float zoomMultiplier = 1.0f;

        // used to decide when to fire the selectionChange event.
        private int curSelStart;
        private int curSelEnd;
        private short curSelType;
        object oleCallback;

        private static int[] shortcutsToDisable;
        private static int richEditMajorVersion = 3;

        private BitVector32 richTextBoxFlags = new BitVector32();
        private static readonly BitVector32.Section autoWordSelectionSection = BitVector32.CreateSection(1);
        private static readonly BitVector32.Section showSelBarSection = BitVector32.CreateSection(1, autoWordSelectionSection);
        private static readonly BitVector32.Section autoUrlDetectSection = BitVector32.CreateSection(1, showSelBarSection);
        private static readonly BitVector32.Section fInCtorSection = BitVector32.CreateSection(1, autoUrlDetectSection);
        private static readonly BitVector32.Section protectedErrorSection = BitVector32.CreateSection(1, fInCtorSection);
        private static readonly BitVector32.Section linkcursorSection = BitVector32.CreateSection(1, protectedErrorSection);
        private static readonly BitVector32.Section allowOleDropSection = BitVector32.CreateSection(1, linkcursorSection);
        private static readonly BitVector32.Section suppressTextChangedEventSection = BitVector32.CreateSection(1, allowOleDropSection);
        private static readonly BitVector32.Section callOnContentsResizedSection = BitVector32.CreateSection(1, suppressTextChangedEventSection);
        private static readonly BitVector32.Section richTextShortcutsEnabledSection = BitVector32.CreateSection(1, callOnContentsResizedSection);
        private static readonly BitVector32.Section allowOleObjectsSection = BitVector32.CreateSection(1, richTextShortcutsEnabledSection);
        private static readonly BitVector32.Section scrollBarsSection = BitVector32.CreateSection((short)RichTextBoxScrollBars.ForcedBoth, allowOleObjectsSection);
        private static readonly BitVector32.Section enableAutoDragDropSection = BitVector32.CreateSection(1, scrollBarsSection);

        /// <summary>
        ///  Constructs a new RichTextBox.
        /// </summary>
        public RichTextBox()
        {
            InConstructor = true;
            richTextBoxFlags[autoWordSelectionSection] = 0;// This is false by default
            DetectUrls = true;
            ScrollBars = RichTextBoxScrollBars.Both;
            RichTextShortcutsEnabled = true;
            MaxLength = int.MaxValue;
            Multiline = true;
            AutoSize = false;
            curSelStart = curSelEnd = curSelType = -1;
            InConstructor = false;
        }

        /// <summary>
        ///  RichTextBox controls have built-in drag and drop support, but AllowDrop, DragEnter, DragDrop
        ///  may still be used: this should be hidden in the property grid, but not in code
        /// </summary>
        [Browsable(false)]
        public override bool AllowDrop
        {
            get
            {
                return richTextBoxFlags[allowOleDropSection] != 0;
            }
            set
            {
                richTextBoxFlags[allowOleDropSection] = value ? 1 : 0;
                UpdateOleCallback();
            }
        }

        internal bool AllowOleObjects
        {
            get
            {
                return richTextBoxFlags[allowOleObjectsSection] != 0;
            }
            set
            {
                richTextBoxFlags[allowOleObjectsSection] = value ? 1 : 0;
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
        [
        DefaultValue(false),
        RefreshProperties(RefreshProperties.Repaint),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
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
                base.AutoSize = value;
            }
        }

        /// <summary>
        ///  Controls whether whether mouse selection snaps to whole words.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.RichTextBoxAutoWordSelection))
        ]
        public bool AutoWordSelection
        {
            get { return richTextBoxFlags[autoWordSelectionSection] != 0; }
            set
            {
                richTextBoxFlags[autoWordSelectionSection] = value ? 1 : 0;
                if (IsHandleCreated)
                {
                    SendMessage(RichEditMessages.EM_SETOPTIONS,
                                value ? RichTextBoxConstants.ECOOP_OR : RichTextBoxConstants.ECOOP_XOR,
                                RichTextBoxConstants.ECO_AUTOWORDSELECTION);
                }
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
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

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
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
        ///  Returns the amount of indent used in a RichTextBox control when
        ///  SelectionBullet is set to true.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(0),
        Localizable(true),
        SRDescription(nameof(SR.RichTextBoxBulletIndent))
        ]
        public int BulletIndent
        {
            get
            {
                return bulletIndent;
            }

            set
            {

                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(BulletIndent), value));
                }

                bulletIndent = value;

                // Call to update the control only if the bullet is set.
                if (IsHandleCreated && SelectionBullet)
                {
                    SelectionBullet = true;
                }
            }
        }

        private bool CallOnContentsResized
        {
            get { return richTextBoxFlags[callOnContentsResizedSection] != 0; }
            set { richTextBoxFlags[callOnContentsResizedSection] = value ? 1 : 0; }
        }

        internal override bool CanRaiseTextChangedEvent
        {
            get
            {
                return !SuppressTextChangedEvent;
            }
        }

        /// <summary>
        ///  Whether or not there are actions that can be Redone on the RichTextBox control.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.RichTextBoxCanRedoDescr))
        ]
        public bool CanRedo
        {
            get
            {
                if (IsHandleCreated)
                {
                    bool b;
                    b = unchecked((int)(long)SendMessage(RichEditMessages.EM_CANREDO, 0, 0)) != 0;

                    return b;
                }
                return false;
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                // Check for library
                if (moduleHandle == IntPtr.Zero)
                {
                    string richEditControlDllVersion = Libraries.RichEdit41;
                    moduleHandle = Kernel32.LoadLibraryFromSystemPathIfAvailable(richEditControlDllVersion);

                    int lastWin32Error = Marshal.GetLastWin32Error();

                    // This code has been here since the inception of the project,
                    // we can't determine why we have to compare w/ 32 here.
                    // This fails on 3-GB mode, (once the dll is loaded above 3GB memory space)
                    if ((ulong)moduleHandle < (ulong)32)
                    {
                        throw new Win32Exception(lastWin32Error, string.Format(SR.LoadDLLError, richEditControlDllVersion));
                    }

                    StringBuilder pathBuilder = UnsafeNativeMethods.GetModuleFileNameLongPath(new HandleRef(null, moduleHandle));
                    string path = pathBuilder.ToString();
                    FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(path);

                    Debug.Assert(versionInfo != null && !string.IsNullOrEmpty(versionInfo.ProductVersion), "Couldn't get the version info for the richedit dll");
                    if (versionInfo != null && !string.IsNullOrEmpty(versionInfo.ProductVersion))
                    {
                        //Note: this only allows for one digit version
                        if (int.TryParse(versionInfo.ProductVersion[0].ToString(), out int parsedValue))
                        {
                            richEditMajorVersion = parsedValue;
                        }
                    }
                }

                CreateParams cp = base.CreateParams;
                cp.ClassName = RichTextBoxConstants.WC_RICHEDITW_41;

                if (Multiline)
                {
                    if (((int)ScrollBars & RichTextBoxConstants.RTB_HORIZ) != 0 && !WordWrap)
                    {
                        // RichEd infers word wrap from the absence of horizontal scroll bars
                        cp.Style |= NativeMethods.WS_HSCROLL;
                        if (((int)ScrollBars & RichTextBoxConstants.RTB_FORCE) != 0)
                        {
                            cp.Style |= RichTextBoxConstants.ES_DISABLENOSCROLL;
                        }
                    }

                    if (((int)ScrollBars & RichTextBoxConstants.RTB_VERT) != 0)
                    {
                        cp.Style |= NativeMethods.WS_VSCROLL;
                        if (((int)ScrollBars & RichTextBoxConstants.RTB_FORCE) != 0)
                        {
                            cp.Style |= RichTextBoxConstants.ES_DISABLENOSCROLL;
                        }
                    }
                }

                // Remove the WS_BORDER style from the control, if we're trying to set it,
                // to prevent the control from displaying the single point rectangle around the 3D border
                if (BorderStyle.FixedSingle == BorderStyle && ((cp.Style & NativeMethods.WS_BORDER) != 0))
                {
                    cp.Style &= (~NativeMethods.WS_BORDER);
                    cp.ExStyle |= NativeMethods.WS_EX_CLIENTEDGE;
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
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.RichTextBoxDetectURLs))
        ]
        public bool DetectUrls
        {
            get
            {
                return richTextBoxFlags[autoUrlDetectSection] != 0;
            }
            set
            {
                if (value != DetectUrls)
                {
                    richTextBoxFlags[autoUrlDetectSection] = value ? 1 : 0;
                    if (IsHandleCreated)
                    {
                        SendMessage(RichEditMessages.EM_AUTOURLDETECT, value ? 1 : 0, 0);
                        RecreateHandle();
                    }
                }
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 96);
            }
        }

        /// <summary>
        ///  We can't just enable drag/drop of text by default: it's a breaking change.
        ///  Should be false by default.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.RichTextBoxEnableAutoDragDrop))
        ]
        public bool EnableAutoDragDrop
        {
            get
            {
                return richTextBoxFlags[enableAutoDragDropSection] != 0;
            }
            set
            {
                richTextBoxFlags[enableAutoDragDropSection] = value ? 1 : 0;
                UpdateOleCallback();
            }
        }

        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
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

        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                if (IsHandleCreated)
                {
                    if (User32.GetWindowTextLengthW(new HandleRef(this, Handle)) > 0)
                    {
                        if (value == null)
                        {
                            base.Font = null;
                            SetCharFormatFont(false, Font);
                        }
                        else
                        {
                            try
                            {
                                Font f = GetCharFormatFont(false);
                                if (f == null || !f.Equals(value))
                                {
                                    SetCharFormatFont(false, value);
                                    // update controlfont from "resolved" font from the attempt
                                    // to set the document font...
                                    //
                                    CallOnContentsResized = true;
                                    base.Font = GetCharFormatFont(false);
                                }
                            }
                            finally
                            {
                                CallOnContentsResized = false;
                            }
                        }
                    }
                    else
                    {
                        base.Font = value;
                    }
                }
                else
                {
                    base.Font = value;
                }
            }
        }

        internal override Size GetPreferredSizeCore(Size proposedConstraints)
        {
            Size scrollBarPadding = Size.Empty;

            //If the RTB is multiline, we won't have a horizontal scrollbar.
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
            get { return richTextBoxFlags[fInCtorSection] != 0; }
            set { richTextBoxFlags[fInCtorSection] = value ? 1 : 0; }
        }

        /// <summary>
        ///  Sets or gets the rich text box control' language option.
        ///  The IMF_AUTOFONT flag is set by default.
        ///  The IMF_AUTOKEYBOARD and IMF_IMECANCELCOMPLETE flags are cleared by default.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public RichTextBoxLanguageOptions LanguageOption
        {
            get
            {
                RichTextBoxLanguageOptions opt;
                if (IsHandleCreated)
                {
                    opt = (RichTextBoxLanguageOptions)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_GETLANGOPTIONS, 0, 0);
                }
                else
                {
                    opt = languageOption;
                }
                return opt;
            }
            set
            {
                if (LanguageOption != value)
                {
                    languageOption = value;
                    if (IsHandleCreated)
                    {
                        UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_SETLANGOPTIONS, 0, (int)value);
                    }
                }
            }
        }

        private bool LinkCursor
        {
            get { return richTextBoxFlags[linkcursorSection] != 0; }
            set { richTextBoxFlags[linkcursorSection] = value ? 1 : 0; }
        }

        [
        DefaultValue(int.MaxValue),
        ]
        public override int MaxLength
        {
            get
            {
                return base.MaxLength;
            }
            set
            {
                base.MaxLength = value;
            }
        }
        [DefaultValue(true)]
        public override bool Multiline
        {
            get
            {
                return base.Multiline;
            }
            set
            {
                base.Multiline = value;
            }
        }

        private bool ProtectedError
        {
            get { return richTextBoxFlags[protectedErrorSection] != 0; }
            set { richTextBoxFlags[protectedErrorSection] = value ? 1 : 0; }
        }

        /// <summary>
        ///  Returns the name of the action that will be performed if the user
        ///  Redo's their last Undone operation. If no operation can be redone,
        ///  an empty string ("") is returned.
        /// </summary>
        //NOTE: This is overridable, because we want people to be able to
        //      mess with the names if necessary...?
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.RichTextBoxRedoActionNameDescr))
        ]
        public string RedoActionName
        {
            get
            {
                if (!CanRedo)
                {
                    return "";
                }

                int n;
                n = unchecked((int)(long)SendMessage(RichEditMessages.EM_GETREDONAME, 0, 0));
                return GetEditorActionName(n);
            }
        }

        //Description: Specifies whether rich text formatting keyboard shortcuts are enabled.
        [
        DefaultValue(true),
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        public bool RichTextShortcutsEnabled
        {
            get { return richTextBoxFlags[richTextShortcutsEnabledSection] != 0; }
            set
            {
                if (shortcutsToDisable == null)
                {
                    shortcutsToDisable = new int[] { (int)Shortcut.CtrlL, (int)Shortcut.CtrlR, (int)Shortcut.CtrlE, (int)Shortcut.CtrlJ };
                }
                richTextBoxFlags[richTextShortcutsEnabledSection] = value ? 1 : 0;
            }
        }

        /// <summary>
        ///  The right margin of a RichTextBox control.  A nonzero margin implies WordWrap.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(0),
        Localizable(true),
        SRDescription(nameof(SR.RichTextBoxRightMargin))
        ]
        public int RightMargin
        {
            get
            {
                return rightMargin;
            }
            set
            {
                if (rightMargin != value)
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(RightMargin), value, 0));
                    }

                    rightMargin = value;

                    if (value == 0)
                    {
                        // Once you set EM_SETTARGETDEVICE to something nonzero, RichEd will assume
                        // word wrap forever and ever.
                        RecreateHandle();
                    }
                    else if (IsHandleCreated)
                    {
                        IntPtr hDC = UnsafeNativeMethods.CreateIC("DISPLAY", null, null, new HandleRef(null, IntPtr.Zero));
                        try
                        {
                            SendMessage(RichEditMessages.EM_SETTARGETDEVICE, hDC, (IntPtr)Pixel2Twip(hDC, value, true));
                        }
                        finally
                        {
                            if (hDC != IntPtr.Zero)
                            {
                                Gdi32.DeleteDC(hDC);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  The text of a RichTextBox control, including all Rtf codes.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.RichTextBoxRTF)),
        RefreshProperties(RefreshProperties.All)
        ]
        public string Rtf
        {
            get
            {
                if (IsHandleCreated)
                {
                    return StreamOut(RichTextBoxConstants.SF_RTF);
                }
                else if (textPlain != null)
                {
                    ForceHandleCreate();
                    return StreamOut(RichTextBoxConstants.SF_RTF);
                }
                else
                {
                    return textRtf;
                }
            }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }

                if (value.Equals(Rtf))
                {
                    return;
                }

                ForceHandleCreate();
                textRtf = value;
                StreamIn(value, RichTextBoxConstants.SF_RTF);
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
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(RichTextBoxScrollBars.Both),
        Localizable(true),
        SRDescription(nameof(SR.RichTextBoxScrollBars))
        ]
        public RichTextBoxScrollBars ScrollBars
        {
            get
            {
                return (RichTextBoxScrollBars)richTextBoxFlags[scrollBarsSection];
            }
            set
            {
                // we could be more clever here, but it doesnt seem like this would get set enough
                // to warrant a clever bitmask.
                if (!ClientUtils.IsEnumValid_NotSequential(value,
                    (int)value,
                    (int)RichTextBoxScrollBars.Both,
                    (int)RichTextBoxScrollBars.None,
                    (int)RichTextBoxScrollBars.Horizontal,
                    (int)RichTextBoxScrollBars.Vertical,
                    (int)RichTextBoxScrollBars.ForcedHorizontal,
                    (int)RichTextBoxScrollBars.ForcedVertical,
                    (int)RichTextBoxScrollBars.ForcedBoth))
                {

                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(RichTextBoxScrollBars));
                }

                if (value != ScrollBars)
                {
                    using (LayoutTransaction.CreateTransactionIf(AutoSize, ParentInternal, this, PropertyNames.ScrollBars))
                    {
                        richTextBoxFlags[scrollBarsSection] = (int)value;
                        RecreateHandle();
                    }
                }
            }
        }

        /// <summary>
        ///  The alignment of the paragraphs in a RichTextBox control.
        /// </summary>
        [
        DefaultValue(HorizontalAlignment.Left),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.RichTextBoxSelAlignment))
        ]
        public HorizontalAlignment SelectionAlignment
        {
            get
            {
                HorizontalAlignment selectionAlignment = HorizontalAlignment.Left;

                ForceHandleCreate();
                NativeMethods.PARAFORMAT pf = new NativeMethods.PARAFORMAT
                {
                    rgxTabs = new int[RichTextBoxConstants.MAX_TAB_STOPS]
                };

                // get the format for our currently selected paragraph
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_GETPARAFORMAT, 0, pf);

                // check if alignment has been set yet
                if ((RichTextBoxConstants.PFM_ALIGNMENT & pf.dwMask) != 0)
                {
                    switch (pf.wAlignment)
                    {
                        case RichTextBoxConstants.PFA_LEFT:
                            selectionAlignment = HorizontalAlignment.Left;
                            break;

                        case RichTextBoxConstants.PFA_RIGHT:
                            selectionAlignment = HorizontalAlignment.Right;
                            break;

                        case RichTextBoxConstants.PFA_CENTER:
                            selectionAlignment = HorizontalAlignment.Center;
                            break;
                    }
                }

                return selectionAlignment;
            }
            set
            {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)HorizontalAlignment.Left, (int)HorizontalAlignment.Center))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(HorizontalAlignment));
                }

                ForceHandleCreate();
                NativeMethods.PARAFORMAT pf = new NativeMethods.PARAFORMAT
                {
                    dwMask = RichTextBoxConstants.PFM_ALIGNMENT
                };
                switch (value)
                {

                    case HorizontalAlignment.Left:
                        pf.wAlignment = RichTextBoxConstants.PFA_LEFT;
                        break;

                    case HorizontalAlignment.Right:
                        pf.wAlignment = RichTextBoxConstants.PFA_RIGHT;
                        break;

                    case HorizontalAlignment.Center:
                        pf.wAlignment = RichTextBoxConstants.PFA_CENTER;
                        break;
                }

                // set the format for our current paragraph or selection
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_SETPARAFORMAT, 0, pf);
            }
        }

        /// <summary>
        ///  Determines if a paragraph in the RichTextBox control
        ///  contains the current selection or insertion point has the bullet style.
        /// </summary>
        [
        DefaultValue(false),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.RichTextBoxSelBullet))
        ]
        public bool SelectionBullet
        {
            get
            {
                RichTextBoxSelectionAttribute selectionBullet = RichTextBoxSelectionAttribute.None;

                ForceHandleCreate();
                NativeMethods.PARAFORMAT pf = new NativeMethods.PARAFORMAT
                {
                    rgxTabs = new int[RichTextBoxConstants.MAX_TAB_STOPS]
                };

                // get the format for our currently selected paragraph
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_GETPARAFORMAT, 0, pf);

                // check if alignment has been set yet
                if ((RichTextBoxConstants.PFM_NUMBERING & pf.dwMask) != 0)
                {
                    if (RichTextBoxConstants.PFN_BULLET == pf.wNumbering)
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

                NativeMethods.PARAFORMAT pf = new NativeMethods.PARAFORMAT
                {
                    dwMask = RichTextBoxConstants.PFM_NUMBERING | RichTextBoxConstants.PFM_OFFSET
                };

                if (!value)
                {
                    pf.wNumbering = 0;
                    pf.dxOffset = 0;
                }
                else
                {
                    pf.wNumbering = RichTextBoxConstants.PFN_BULLET;
                    pf.dxOffset = Pixel2Twip(IntPtr.Zero, bulletIndent, true);
                }
                // set the format for our current paragraph or selection
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_SETPARAFORMAT, 0, pf);
            }
        }

        /// <summary>
        ///  Determines whether text in the RichTextBox control
        ///  appears on the baseline (normal), as a superscript above the baseline,
        ///  or as a subscript below the baseline.
        /// </summary>
        [
        DefaultValue(0),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.RichTextBoxSelCharOffset))
        ]
        public int SelectionCharOffset
        {
            get
            {
                int selCharOffset = 0;

                ForceHandleCreate();
                NativeMethods.CHARFORMATA cf = GetCharFormat(true);
                // if the effects member contains valid info
                if ((cf.dwMask & RichTextBoxConstants.CFM_OFFSET) != 0)
                {
                    selCharOffset = cf.yOffset;
                }
                else
                {
                    // The selection contains characters of different offsets,
                    // so we just return the offset of the first character.
                    selCharOffset = cf.yOffset;
                }

                return Twip2Pixel(IntPtr.Zero, selCharOffset, false);
            }
            set
            {
                if (value > 2000 || value < -2000)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidBoundArgument, nameof(SelectionCharOffset), value, -2000, 2000));
                }

                ForceHandleCreate();
                NativeMethods.CHARFORMATA cf = new NativeMethods.CHARFORMATA
                {
                    dwMask = RichTextBoxConstants.CFM_OFFSET,
                    yOffset = Pixel2Twip(IntPtr.Zero, value, false)
                };

                // Set the format information
                // SendMessage will force the handle to be created if it hasn't already. Normally,
                // we would cache property values until the handle is created - but for this property,
                // it's far more simple to just create the handle.
                //
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_SETCHARFORMAT, RichTextBoxConstants.SCF_SELECTION, cf);
            }
        }

        /// <summary>
        ///  The color of the currently selected text in the
        ///  RichTextBox control.
        ///  Returns Color.Empty if the selection has more than one color.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.RichTextBoxSelColor))
        ]
        public Color SelectionColor
        {
            get
            {
                Color selColor = Color.Empty;

                ForceHandleCreate();
                NativeMethods.CHARFORMATA cf = GetCharFormat(true);
                // if the effects member contains valid info
                if ((cf.dwMask & RichTextBoxConstants.CFM_COLOR) != 0)
                {
                    selColor = ColorTranslator.FromOle(cf.crTextColor);
                }

                return selColor;
            }
            set
            {
                ForceHandleCreate();
                NativeMethods.CHARFORMATA cf = GetCharFormat(true);
                cf.dwMask = RichTextBoxConstants.CFM_COLOR;
                cf.dwEffects = 0;
                cf.crTextColor = ColorTranslator.ToWin32(value);

                // set the format information
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_SETCHARFORMAT, RichTextBoxConstants.SCF_SELECTION, cf);
            }
        }

        /// <summary>
        ///  The background color of the currently selected text in the RichTextBox control.
        ///  Returns Color.Empty if the selection has more than one color.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.RichTextBoxSelBackColor))
        ]
        public Color SelectionBackColor
        {
            get
            {
                Color selColor = Color.Empty;

                if (IsHandleCreated)
                {
                    NativeMethods.CHARFORMAT2A cf2 = GetCharFormat2(true);
                    // If the effects member contains valid info
                    if ((cf2.dwEffects & RichTextBoxConstants.CFE_AUTOBACKCOLOR) != 0)
                    {
                        selColor = BackColor;
                    }
                    else if ((cf2.dwMask & RichTextBoxConstants.CFM_BACKCOLOR) != 0)
                    {
                        selColor = ColorTranslator.FromOle(cf2.crBackColor);
                    }
                }
                else
                {
                    selColor = selectionBackColorToSetOnHandleCreated;
                }
                return selColor;
            }
            set
            {
                //Note: don't compare the value to the old value here: it's possible that
                //you have a different range selected.
                selectionBackColorToSetOnHandleCreated = value;
                if (IsHandleCreated)
                {
                    NativeMethods.CHARFORMAT2A cf2 = new NativeMethods.CHARFORMAT2A();
                    if (value == Color.Empty)
                    {
                        cf2.dwEffects = RichTextBoxConstants.CFE_AUTOBACKCOLOR;
                    }
                    else
                    {
                        cf2.dwMask = RichTextBoxConstants.CFM_BACKCOLOR;
                        cf2.crBackColor = ColorTranslator.ToWin32(value);
                    }

                    UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_SETCHARFORMAT, RichTextBoxConstants.SCF_SELECTION, cf2);
                }
            }
        }

        /// <summary>
        ///  The font used to display the currently selected text
        ///  or the characters(s) immediately following the insertion point in the
        ///  RichTextBox control.  Null if the selection has more than one font.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.RichTextBoxSelFont))
        ]
        public Font SelectionFont
        {
            get
            {
                return GetCharFormatFont(true);
            }
            set
            {
                SetCharFormatFont(true, value);
            }
        }

        /// <summary>
        ///  The distance (in pixels) between the left edge of the first line of text
        ///  in the selected paragraph(s) (as specified by the SelectionIndent property)
        ///  and the left edge of subsequent lines of text in the same paragraph(s).
        /// </summary>
        [
        DefaultValue(0),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.RichTextBoxSelHangingIndent))
        ]
        public int SelectionHangingIndent
        {
            get
            {
                int selHangingIndent = 0;

                ForceHandleCreate();
                NativeMethods.PARAFORMAT pf = new NativeMethods.PARAFORMAT
                {
                    rgxTabs = new int[RichTextBoxConstants.MAX_TAB_STOPS]
                };

                // get the format for our currently selected paragraph
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_GETPARAFORMAT, 0, pf);

                // check if alignment has been set yet
                if ((RichTextBoxConstants.PFM_OFFSET & pf.dwMask) != 0)
                {
                    selHangingIndent = pf.dxOffset;
                }

                return Twip2Pixel(IntPtr.Zero, selHangingIndent, true);
            }
            set
            {
                ForceHandleCreate();

                NativeMethods.PARAFORMAT pf = new NativeMethods.PARAFORMAT
                {
                    dwMask = RichTextBoxConstants.PFM_OFFSET,
                    dxOffset = Pixel2Twip(IntPtr.Zero, value, true)
                };

                // set the format for our current paragraph or selection
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_SETPARAFORMAT, 0, pf);
            }
        }

        /// <summary>
        ///  The distance (in pixels) between the left edge of the RichTextBox control and
        ///  the left edge of the text that is selected or added at the current
        ///  insertion point.
        /// </summary>
        [
        DefaultValue(0),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.RichTextBoxSelIndent))
        ]
        public int SelectionIndent
        {
            get
            {
                int selIndent = 0;

                ForceHandleCreate();
                NativeMethods.PARAFORMAT pf = new NativeMethods.PARAFORMAT
                {
                    rgxTabs = new int[RichTextBoxConstants.MAX_TAB_STOPS]
                };

                // get the format for our currently selected paragraph
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_GETPARAFORMAT, 0, pf);

                // check if alignment has been set yet
                if ((RichTextBoxConstants.PFM_STARTINDENT & pf.dwMask) != 0)
                {
                    selIndent = pf.dxStartIndent;
                }

                return Twip2Pixel(IntPtr.Zero, selIndent, true);
            }
            set
            {
                ForceHandleCreate();

                NativeMethods.PARAFORMAT pf = new NativeMethods.PARAFORMAT
                {
                    dwMask = RichTextBoxConstants.PFM_STARTINDENT,
                    dxStartIndent = Pixel2Twip(IntPtr.Zero, value, true)
                };

                // set the format for our current paragraph or selection
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_SETPARAFORMAT, 0, pf);
            }
        }

        /// <summary>
        ///  Gets or sets the number of characters selected in the text
        ///  box.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.TextBoxSelectionLengthDescr))
        ]
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

            set
            {
                base.SelectionLength = value;
            }
        }

        /// <summary>
        ///  true if the current selection prevents any changes to its contents.
        /// </summary>
        [
        DefaultValue(false),
        SRDescription(nameof(SR.RichTextBoxSelProtected)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public bool SelectionProtected
        {
            get
            {
                ForceHandleCreate();
                return GetCharFormat(RichTextBoxConstants.CFM_PROTECTED, RichTextBoxConstants.CFM_PROTECTED) == RichTextBoxSelectionAttribute.All;
            }
            set
            {
                ForceHandleCreate();
                SetCharFormat(RichTextBoxConstants.CFM_PROTECTED, value ? RichTextBoxConstants.CFE_PROTECTED : 0, RichTextBoxSelectionAttribute.All);
            }
        }

        /// <summary>
        ///  The currently selected text of a RichTextBox control, including
        ///  all Rtf codes.
        /// </summary>
        [
        DefaultValue(""),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.RichTextBoxSelRTF))
        ]
        public string SelectedRtf
        {
            get
            {
                ForceHandleCreate();
                return StreamOut(RichTextBoxConstants.SFF_SELECTION | RichTextBoxConstants.SF_RTF);
            }
            set
            {
                ForceHandleCreate();
                if (value == null)
                {
                    value = string.Empty;
                }

                StreamIn(value, RichTextBoxConstants.SFF_SELECTION | RichTextBoxConstants.SF_RTF);
            }
        }

        /// <summary>
        ///  The distance (in pixels) between the right edge of the RichTextBox control and
        ///  the right edge of the text that is selected or added at the current
        ///  insertion point.
        /// </summary>
        [
        DefaultValue(0),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.RichTextBoxSelRightIndent))
        ]
        public int SelectionRightIndent
        {
            get
            {
                int selRightIndent = 0;

                ForceHandleCreate();

                NativeMethods.PARAFORMAT pf = new NativeMethods.PARAFORMAT
                {
                    rgxTabs = new int[RichTextBoxConstants.MAX_TAB_STOPS]
                };

                // get the format for our currently selected paragraph
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_GETPARAFORMAT, 0, pf);

                // check if alignment has been set yet
                if ((RichTextBoxConstants.PFM_RIGHTINDENT & pf.dwMask) != 0)
                {
                    selRightIndent = pf.dxRightIndent;
                }

                return Twip2Pixel(IntPtr.Zero, selRightIndent, true);
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(SelectionRightIndent), value, 0));
                }

                ForceHandleCreate();
                NativeMethods.PARAFORMAT pf = new NativeMethods.PARAFORMAT
                {
                    dwMask = RichTextBoxConstants.PFM_RIGHTINDENT,
                    dxRightIndent = Pixel2Twip(IntPtr.Zero, value, true)
                };

                // set the format for our current paragraph or selection
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_SETPARAFORMAT, 0, pf);
            }
        }

        /// <summary>
        ///  The absolute tab positions (in pixels) of text in a RichTextBox control.
        /// </summary>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.RichTextBoxSelTabs))
        ]
        public int[] SelectionTabs
        {
            get
            {
                int[] selTabs = Array.Empty<int>();

                ForceHandleCreate();
                NativeMethods.PARAFORMAT pf = new NativeMethods.PARAFORMAT
                {
                    rgxTabs = new int[RichTextBoxConstants.MAX_TAB_STOPS]
                };

                // get the format for our currently selected paragraph
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_GETPARAFORMAT, 0, pf);

                // check if alignment has been set yet
                if ((RichTextBoxConstants.PFM_TABSTOPS & pf.dwMask) != 0)
                {
                    selTabs = new int[pf.cTabCount];
                    for (int x = 0; x < pf.cTabCount; x++)
                    {
                        selTabs[x] = Twip2Pixel(IntPtr.Zero, pf.rgxTabs[x], true);
                    }
                }

                return selTabs;
            }
            set
            {
                // Verify the argument, and throw an error if is bad
                if (value != null && value.Length > RichTextBoxConstants.MAX_TAB_STOPS)
                {
                    throw new ArgumentOutOfRangeException(nameof(SelectionTabs), SR.SelTabCountRange);
                }

                ForceHandleCreate();
                NativeMethods.PARAFORMAT pf = new NativeMethods.PARAFORMAT
                {
                    rgxTabs = new int[RichTextBoxConstants.MAX_TAB_STOPS]
                };

                // get the format for our currently selected paragraph because
                // we need to get the number of tabstops to copy
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_GETPARAFORMAT, 0, pf);

                pf.cTabCount = (short)((value == null) ? 0 : value.Length);
                pf.dwMask = RichTextBoxConstants.PFM_TABSTOPS;
                for (int x = 0; x < pf.cTabCount; x++)
                {
                    pf.rgxTabs[x] = Pixel2Twip(IntPtr.Zero, value[x], true);
                }

                // set the format for our current paragraph or selection
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_SETPARAFORMAT, 0, pf);
            }
        }

        /// <summary>
        ///  The currently selected text of a RichTextBox control; consists of a
        ///  zero length string if no characters are selected.
        /// </summary>
        [
        DefaultValue(""),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.RichTextBoxSelText))
        ]
        public override string SelectedText
        {
            get
            {
                ForceHandleCreate();

                string text = StreamOut(RichTextBoxConstants.SFF_SELECTION | RichTextBoxConstants.SF_TEXT | RichTextBoxConstants.SF_UNICODE);
                return text;
            }
            set
            {
                ForceHandleCreate();
                StreamIn(value, RichTextBoxConstants.SFF_SELECTION | RichTextBoxConstants.SF_TEXT | RichTextBoxConstants.SF_UNICODE);
            }
        }

        /// <summary>
        ///  The type of the current selection. The returned value is one
        ///  of the values enumerated in RichTextBoxSelectionType.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.RichTextBoxSelTypeDescr))
        ]
        public RichTextBoxSelectionTypes SelectionType
        {
            get
            {
                ForceHandleCreate();
                if (SelectionLength > 0)
                {
                    int n;
                    n = unchecked((int)(long)SendMessage(RichEditMessages.EM_SELECTIONTYPE, 0, 0));
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
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.RichTextBoxSelMargin))
        ]
        public bool ShowSelectionMargin
        {
            get { return richTextBoxFlags[showSelBarSection] != 0; }
            set
            {
                if (value != ShowSelectionMargin)
                {
                    richTextBoxFlags[showSelBarSection] = value ? 1 : 0;
                    if (IsHandleCreated)
                    {
                        SendMessage(RichEditMessages.EM_SETOPTIONS,
                            value ? RichTextBoxConstants.ECOOP_OR :
                            RichTextBoxConstants.ECOOP_XOR,
                            RichTextBoxConstants.ECO_SELECTIONBAR);
                    }
                }
            }
        }

        [
        Localizable(true),
        RefreshProperties(RefreshProperties.All)
        ]
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
                    return "";
                }

                if (!IsHandleCreated && textRtf == null)
                {
                    if (textPlain != null)
                    {
                        return textPlain;
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

                    return StreamOut(RichTextBoxConstants.SF_TEXT | RichTextBoxConstants.SF_UNICODE);
                }
            }
            set
            {
                using (LayoutTransaction.CreateTransactionIf(AutoSize, ParentInternal, this, PropertyNames.Text))
                {
                    textRtf = null;
                    if (!IsHandleCreated)
                    {
                        textPlain = value;
                    }
                    else
                    {
                        textPlain = null;
                        if (value == null)
                        {
                            value = string.Empty;
                        }
                        StreamIn(value, RichTextBoxConstants.SF_TEXT | RichTextBoxConstants.SF_UNICODE);
                        // reset Modified
                        SendMessage(EditMessages.EM_SETMODIFY, 0, 0);
                    }
                }
            }
        }

        private bool SuppressTextChangedEvent
        {
            get { return richTextBoxFlags[suppressTextChangedEventSection] != 0; }
            set
            {
                bool oldValue = SuppressTextChangedEvent;
                if (value != oldValue)
                {
                    richTextBoxFlags[suppressTextChangedEventSection] = value ? 1 : 0;
                    CommonProperties.xClearPreferredSizeCache(this);
                }
            }
        }

        [Browsable(false)]
        public override int TextLength
        {
            get
            {
                NativeMethods.GETTEXTLENGTHEX gtl = new NativeMethods.GETTEXTLENGTHEX
                {
                    flags = RichTextBoxConstants.GTL_NUMCHARS,
                    codepage = 1200 /* CP_UNICODE */
                };

                return unchecked((int)(long)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_GETTEXTLENGTHEX, gtl, 0 /*ignored*/));
            }
        }

        /// <summary>
        ///  Returns the name of the action that will be undone if the user
        ///  Undo's their last operation. If no operation can be undone, it will
        ///  return an empty string ("").
        /// </summary>
        //NOTE: This is overridable, because we want people to be able to
        //      mess with the names if necessary...?
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.RichTextBoxUndoActionNameDescr))
        ]
        public string UndoActionName
        {
            get
            {
                if (!CanUndo)
                {
                    return "";
                }

                int n;
                n = unchecked((int)(long)SendMessage(RichEditMessages.EM_GETUNDONAME, 0, 0));
                return GetEditorActionName(n);
            }
        }

        private string GetEditorActionName(int actionID)
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
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(1.0f),
        Localizable(true),
        SRDescription(nameof(SR.RichTextBoxZoomFactor))
        ]
        public float ZoomFactor
        {
            get
            {
                if (IsHandleCreated)
                {
                    int numerator = 0;
                    int denominator = 0;
                    SendMessage(RichEditMessages.EM_GETZOOM, ref numerator, ref denominator);
                    if ((numerator != 0) && (denominator != 0))
                    {
                        zoomMultiplier = ((float)numerator) / ((float)denominator);
                    }
                    else
                    {
                        zoomMultiplier = 1.0f;
                    }
                    return zoomMultiplier;
                }
                else
                {
                    return zoomMultiplier;
                }
            }

            set
            {
                if (value <= 0.015625f || value >= 64.0f)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidExBoundArgument, nameof(ZoomFactor), value, 0.015625f, 64.0f));
                }

                if (value != zoomMultiplier)
                {
                    SendZoomFactor(value);
                }
            }
        }

        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.RichTextBoxContentsResized))]
        public event ContentsResizedEventHandler ContentsResized
        {
            add => Events.AddHandler(EVENT_REQUESTRESIZE, value);
            remove => Events.RemoveHandler(EVENT_REQUESTRESIZE, value);
        }

        /// <summary>
        ///  RichTextBox controls have built-in drag and drop support, but AllowDrop, DragEnter, DragDrop
        ///  may still be used: this should be hidden in the property grid, but not in code
        /// </summary>
        [Browsable(false)]
        public new event DragEventHandler DragDrop
        {
            add => base.DragDrop += value;
            remove => base.DragDrop -= value;
        }

        /// <summary>
        ///  RichTextBox controls have built-in drag and drop support, but AllowDrop, DragEnter, DragDrop
        ///  may still be used: this should be hidden in the property grid, but not in code
        /// </summary>
        [Browsable(false)]
        public new event DragEventHandler DragEnter
        {
            add => base.DragEnter += value;
            remove => base.DragEnter -= value;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler DragLeave
        {
            add => base.DragLeave += value;
            remove => base.DragLeave -= value;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event DragEventHandler DragOver
        {
            add => base.DragOver += value;
            remove => base.DragOver -= value;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event GiveFeedbackEventHandler GiveFeedback
        {
            add => base.GiveFeedback += value;
            remove => base.GiveFeedback -= value;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event QueryContinueDragEventHandler QueryContinueDrag
        {
            add => base.QueryContinueDrag += value;
            remove => base.QueryContinueDrag -= value;
        }

        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.RichTextBoxHScroll))]
        public event EventHandler HScroll
        {
            add => Events.AddHandler(EVENT_HSCROLL, value);
            remove => Events.RemoveHandler(EVENT_HSCROLL, value);
        }

        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.RichTextBoxLinkClick))]
        public event LinkClickedEventHandler LinkClicked
        {
            add => Events.AddHandler(EVENT_LINKACTIVATE, value);
            remove => Events.RemoveHandler(EVENT_LINKACTIVATE, value);
        }

        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.RichTextBoxIMEChange))]
        public event EventHandler ImeChange
        {
            add => Events.AddHandler(EVENT_IMECHANGE, value);
            remove => Events.RemoveHandler(EVENT_IMECHANGE, value);
        }

        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.RichTextBoxProtected))]
        public event EventHandler Protected
        {
            add => Events.AddHandler(EVENT_PROTECTED, value);
            remove => Events.RemoveHandler(EVENT_PROTECTED, value);
        }

        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.RichTextBoxSelChange))]
        public event EventHandler SelectionChanged
        {
            add => Events.AddHandler(EVENT_SELCHANGE, value);
            remove => Events.RemoveHandler(EVENT_SELCHANGE, value);
        }

        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.RichTextBoxVScroll))]
        public event EventHandler VScroll
        {
            add => Events.AddHandler(EVENT_VSCROLL, value);
            remove => Events.RemoveHandler(EVENT_VSCROLL, value);
        }

        /// <summary>
        ///  Returns a boolean indicating whether the RichTextBoxConstants control can paste the
        ///  given clipboard format.
        /// </summary>
        public bool CanPaste(DataFormats.Format clipFormat)
        {
            bool b = false;
            b = unchecked((int)(long)SendMessage(RichEditMessages.EM_CANPASTE, clipFormat.Id, 0)) != 0;

            return b;
        }

        //DrawToBitmap doesn't work for this control, so we should hide it.  We'll
        //still call base so that this has a chance to work if it can.
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds)
        {
            base.DrawToBitmap(bitmap, targetBounds);
        }

        private unsafe int EditStreamProc(IntPtr dwCookie, IntPtr buf, int cb, out int transferred)
        {
            int ret = 0;    // assume that everything is Okay

            byte[] bytes = new byte[cb];

            int cookieVal = (int)dwCookie;

            transferred = 0;
            try
            {
                switch (cookieVal & DIRECTIONMASK)
                {
                    case RichTextBox.OUTPUT:
                        {
                            if (editStream == null)
                            {
                                editStream = new MemoryStream();
                            }

                            switch (cookieVal & KINDMASK)
                            {
                                case RichTextBox.RTF:
                                case RichTextBox.TEXTCRLF:
                                    Marshal.Copy(buf, bytes, 0, cb);
                                    editStream.Write(bytes, 0, cb);
                                    break;
                                case RichTextBox.TEXTLF:
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
                                        editStream.Write(bytes, 0, consumedCharCount * 2);
                                    }
                                    else
                                    {
                                        int requestedCharCount = cb;
                                        int consumedCharCount = 0;

                                        fixed (byte* pb = bytes)
                                        {
                                            byte* pChars = (byte*)pb;
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
                                        editStream.Write(bytes, 0, consumedCharCount);
                                    }
                                    break;
                            }

                            // set up number of bytes transferred
                            transferred = cb;
                            break;
                        }

                    case RichTextBox.INPUT:
                        {
                            // Several customers complained that they were getting Random NullReference exceptions inside EditStreamProc.
                            // We had a case of  acustomer using Everett bits and another case of a customer using Whidbey Beta1 bits.
                            // We don't have a repro in house which makes it problematic to determine the cause for this behavior.
                            // Looking at the code it seems that the only posibility for editStream to be null is when the user
                            // calls RichTextBox::LoadFile(Stream, RichTextBoxStreamType) with a null Stream.
                            // However, the user said that his app is not using LoadFile method.
                            // The only possibility left open is that the native Edit control sends random calls into EditStreamProc.
                            // We have to guard against this.
                            if (editStream != null)
                            {
                                transferred = editStream.Read(bytes, 0, cb);

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
            catch (IOException) {
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
        public int Find(string str, int start, int end, RichTextBoxFinds options)
        {
            int textLen = TextLength;
            if (start < 0 || start > textLen)
            {
                throw new ArgumentOutOfRangeException(nameof(start), start, string.Format(SR.InvalidBoundArgument, nameof(start), start, 0, textLen));
            }

            if (end < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(end), end, string.Format(SR.RichTextFindEndInvalid, end));
            }

            bool selectWord = true;
            NativeMethods.FINDTEXT ft = new NativeMethods.FINDTEXT
            {
                chrg = new Richedit.CHARRANGE(),

                // set up the default values for the FINDTEXT structure, that is
                // the given string and the whole range of the text stream
                lpstrText = str ?? throw new ArgumentNullException(nameof(str))
            };
            if (end == -1)
            {
                end = textLen;
            }

            if (start > end)
            {
                throw new ArgumentException(string.Format(SR.RichTextFindEndInvalid, end));
            }

            if ((options & RichTextBoxFinds.Reverse) != RichTextBoxFinds.Reverse)
            {
                // normal
                //
                ft.chrg.cpMin = start;
                ft.chrg.cpMax = end;
            }
            else
            {
                // reverse
                //
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
            int findOptions = 0;
            if ((options & RichTextBoxFinds.WholeWord) == RichTextBoxFinds.WholeWord)
            {
                findOptions |= RichTextBoxConstants.FR_WHOLEWORD;
            }

            if ((options & RichTextBoxFinds.MatchCase) == RichTextBoxFinds.MatchCase)
            {
                findOptions |= RichTextBoxConstants.FR_MATCHCASE;
            }

            if ((options & RichTextBoxFinds.NoHighlight) == RichTextBoxFinds.NoHighlight)
            {
                selectWord = false;
            }

            if ((options & RichTextBoxFinds.Reverse) != RichTextBoxFinds.Reverse)
            {
                // The default for RichEdit 2.0 is to search in reverse
                findOptions |= RichTextBoxConstants.FR_DOWN;
            }

            // Perform the find, will return ubyte position
            int position;

            position = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_FINDTEXT, findOptions, ft);

            // if we didn't find anything, or we don't have to select what was found,
            // we're done
            if (position != -1 && selectWord)
            {
                // Select the string found, this is done in ubyte units
                var chrg = new Richedit.CHARRANGE
                {
                    cpMin = position
                };
                //Look for kashidas in the string.  A kashida is an arabic visual justification character
                //that's not semantically meaningful.  Searching for ABC might find AB_C (where A,B, and C
                //represent Arabic characters and _ represents a kashida).  We should highlight the text
                //including the kashida.
                char kashida = (char)0x640;
                string text = Text;
                string foundString = text.Substring(position, str.Length);
                int startIndex = foundString.IndexOf(kashida);
                if (startIndex == -1)
                {
                    //No kashida in the string
                    chrg.cpMax = position + str.Length;
                }
                else
                {
                    //There's at least one kashida
                    int searchingCursor; //index into search string
                    int foundCursor; //index into Text
                    for (searchingCursor = startIndex, foundCursor = position + startIndex; searchingCursor < str.Length;
                        searchingCursor++, foundCursor++)
                    {
                        while (text[foundCursor] == kashida && str[searchingCursor] != kashida)
                        {
                            foundCursor++;
                        }
                    }
                    chrg.cpMax = foundCursor;
                }

                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_EXSETSEL, 0, ref chrg);
                SendMessage(EditMessages.EM_SCROLLCARET, 0, 0);

            }

            return position;
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

            if (characterSet == null)
            {
                throw new ArgumentNullException(nameof(characterSet));
            }

            if (start < 0 || start > textLength)
            {
                throw new ArgumentOutOfRangeException(nameof(start), start, string.Format(SR.InvalidBoundArgument, nameof(start), start, 0, textLength));
            }

            if (end < start && end != -1)
            {
                throw new ArgumentOutOfRangeException(nameof(end), end, string.Format(SR.InvalidLowBoundArgumentEx, nameof(end), end, nameof(start)));
            }

            // Don't do anything if we get nothing to look for
            if (characterSet.Length == 0)
            {
                return -1;
            }

            int textLen = User32.GetWindowTextLengthW(new HandleRef(this, Handle));
            if (start == end)
            {
                start = 0;
                end = textLen;
            }
            if (end == -1)
            {
                end = textLen;
            }

            var chrg = new Richedit.CHARRANGE(); // The range of characters we have searched
            chrg.cpMax = chrg.cpMin = start;

            // Use the TEXTRANGE to move our text buffer forward
            // or backwards within the main text
            NativeMethods.TEXTRANGE txrg = new NativeMethods.TEXTRANGE
            {
                chrg = new Richedit.CHARRANGE
                {
                    cpMin = chrg.cpMin,
                    cpMax = chrg.cpMax
                }
            }; // Characters we have slurped into memory in order to search
            UnsafeNativeMethods.CharBuffer charBuffer;
            charBuffer = UnsafeNativeMethods.CharBuffer.CreateBuffer(CHAR_BUFFER_LEN + 1);

            txrg.lpstrText = charBuffer.AllocCoTaskMem();
            if (txrg.lpstrText == IntPtr.Zero)
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
                    len = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_GETTEXTRANGE, 0, txrg);
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
                        // Start at the begining of the buffer
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
            NativeMethods.CHARFORMATA cf = GetCharFormat(false);
            if ((cf.dwMask & RichTextBoxConstants.CFM_COLOR) != 0
                && ColorTranslator.ToWin32(value) == cf.crTextColor)
            {

                return true;
            }

            cf.dwMask = RichTextBoxConstants.CFM_COLOR;
            cf.dwEffects = 0;
            cf.crTextColor = ColorTranslator.ToWin32(value);
            return SetCharFormat(RichTextBoxConstants.SCF_ALL, cf);
        }

        private NativeMethods.CHARFORMATA GetCharFormat(bool fSelection)
        {
            NativeMethods.CHARFORMATA cf = new NativeMethods.CHARFORMATA();
            UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_GETCHARFORMAT, fSelection ? RichTextBoxConstants.SCF_SELECTION : RichTextBoxConstants.SCF_DEFAULT, cf);
            return cf;
        }

        private NativeMethods.CHARFORMAT2A GetCharFormat2(bool fSelection)
        {
            NativeMethods.CHARFORMAT2A cf2 = new NativeMethods.CHARFORMAT2A();
            UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_GETCHARFORMAT, fSelection ? RichTextBoxConstants.SCF_SELECTION : RichTextBoxConstants.SCF_DEFAULT, cf2);
            return cf2;
        }

        private RichTextBoxSelectionAttribute GetCharFormat(int mask, int effect)
        {
            RichTextBoxSelectionAttribute charFormat = RichTextBoxSelectionAttribute.None;

            // check to see if the control has been created
            if (IsHandleCreated)
            {
                NativeMethods.CHARFORMATA cf = GetCharFormat(true);
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

        Font GetCharFormatFont(bool selectionOnly)
        {
            ForceHandleCreate();

            NativeMethods.CHARFORMATA cf = GetCharFormat(selectionOnly);
            if ((cf.dwMask & RichTextBoxConstants.CFM_FACE) == 0)
            {
                return null;
            }

            string fontName = Encoding.Default.GetString(cf.szFaceName);
            int index = fontName.IndexOf('\0');
            if (index != -1)
            {
                fontName = fontName.Substring(0, index);
            }

            float fontSize = 13;
            if ((cf.dwMask & RichTextBoxConstants.CFM_SIZE) != 0)
            {
                fontSize = (float)cf.yHeight / (float)20.0;
                if (fontSize == 0 && cf.yHeight > 0)
                {
                    fontSize = 1;
                }
            }

            FontStyle style = FontStyle.Regular;
            if ((cf.dwMask & RichTextBoxConstants.CFM_BOLD) != 0 && (cf.dwEffects & RichTextBoxConstants.CFE_BOLD) != 0)
            {
                style |= FontStyle.Bold;
            }

            if ((cf.dwMask & RichTextBoxConstants.CFM_ITALIC) != 0 && (cf.dwEffects & RichTextBoxConstants.CFE_ITALIC) != 0)
            {
                style |= FontStyle.Italic;
            }

            if ((cf.dwMask & RichTextBoxConstants.CFM_STRIKEOUT) != 0 && (cf.dwEffects & RichTextBoxConstants.CFE_STRIKEOUT) != 0)
            {
                style |= FontStyle.Strikeout;
            }

            if ((cf.dwMask & RichTextBoxConstants.CFM_UNDERLINE) != 0 && (cf.dwEffects & RichTextBoxConstants.CFE_UNDERLINE) != 0)
            {
                style |= FontStyle.Underline;
            }

            try
            {
                return new Font(fontName, fontSize, style, GraphicsUnit.Point, cf.bCharSet);
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
            var wpt = new Point(pt.X, pt.Y);
            int index = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), EditMessages.EM_CHARFROMPOS, 0, ref wpt);

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

        private bool GetCharInCharSet(char c, char[] charSet, bool negate)
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
        {
            return unchecked((int)(long)SendMessage(RichEditMessages.EM_EXLINEFROMCHAR, 0, index));
        }

        /// <summary>
        ///  Returns the location of the character at the given index.
        /// </summary>
        public override Point GetPositionFromCharIndex(int index)
        {
            if (richEditMajorVersion == 2)
            {
                return base.GetPositionFromCharIndex(index);
            }

            if (index < 0 || index > Text.Length)
            {
                return Point.Empty;
            }

            var pt = new Point();
            UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), EditMessages.EM_POSFROMCHAR, ref pt, index);
            return pt;
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
            //valid values are 0x0 to 0x4
            if (!ClientUtils.IsEnumValid(fileType, (int)fileType, (int)RichTextBoxStreamType.RichText, (int)RichTextBoxStreamType.UnicodePlainText))
            {
                throw new InvalidEnumArgumentException(nameof(fileType), (int)fileType, typeof(RichTextBoxStreamType));
            }

            Stream file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
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
            //valid values are 0x0 to 0x4
            if (!ClientUtils.IsEnumValid(fileType, (int)fileType, (int)RichTextBoxStreamType.RichText, (int)RichTextBoxStreamType.UnicodePlainText))
            {
                throw new InvalidEnumArgumentException(nameof(fileType), (int)fileType, typeof(RichTextBoxStreamType));
            }

            int flags;
            switch (fileType)
            {
                case RichTextBoxStreamType.RichText:
                    flags = RichTextBoxConstants.SF_RTF;
                    break;
                case RichTextBoxStreamType.PlainText:
                    Rtf = string.Empty;
                    flags = RichTextBoxConstants.SF_TEXT;
                    break;
                case RichTextBoxStreamType.UnicodePlainText:
                    flags = RichTextBoxConstants.SF_UNICODE | RichTextBoxConstants.SF_TEXT;
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
                SendMessage(RichEditMessages.EM_SETBKGNDCOLOR, 0, ColorTranslator.ToWin32(BackColor));
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
            ((ContentsResizedEventHandler)Events[EVENT_REQUESTRESIZE])?.Invoke(this, e);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            // base.OnHandleCreated is called somewhere in the middle of this

            curSelStart = curSelEnd = curSelType = -1;

            // We will always set the control to use the maximum text, it defaults to 32k..
            // This must be done before we start loading files, because some files may
            // be larger than 32k.
            //
            UpdateMaxLength();

            // This is needed so that the control will fire change and update events
            // even if it is hidden
            //
            SendMessage(RichEditMessages.EM_SETEVENTMASK,
                        0,
                        RichTextBoxConstants.ENM_PROTECTED | RichTextBoxConstants.ENM_SELCHANGE |
                        RichTextBoxConstants.ENM_DROPFILES | RichTextBoxConstants.ENM_REQUESTRESIZE |
                        RichTextBoxConstants.ENM_IMECHANGE | RichTextBoxConstants.ENM_CHANGE |
                        RichTextBoxConstants.ENM_UPDATE | RichTextBoxConstants.ENM_SCROLL |
                        RichTextBoxConstants.ENM_KEYEVENTS | RichTextBoxConstants.ENM_MOUSEEVENTS |
                        RichTextBoxConstants.ENM_SCROLLEVENTS | RichTextBoxConstants.ENM_LINK);

            int rm = rightMargin;
            rightMargin = 0;
            RightMargin = rm;

            //

            SendMessage(RichEditMessages.EM_AUTOURLDETECT, DetectUrls ? 1 : 0, 0);
            if (selectionBackColorToSetOnHandleCreated != Color.Empty)
            {
                SelectionBackColor = selectionBackColorToSetOnHandleCreated;
            }

            // Initialize colors before initializing RTF, otherwise CFE_AUTOCOLOR will be in effect
            // and our text will all be Color.WindowText.
            AutoWordSelection = AutoWordSelection;
            SendMessage(RichEditMessages.EM_SETBKGNDCOLOR, 0, ColorTranslator.ToWin32(BackColor));
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
                if (textRtf != null)
                {
                    // setting RTF calls back on Text, which relies on textRTF being null
                    string text = textRtf;
                    textRtf = null;
                    Rtf = text;
                }
                else if (textPlain != null)
                {
                    string text = textPlain;
                    textPlain = null;
                    Text = text;
                }
            }
            finally
            {
                SuppressTextChangedEvent = false;
            }

            // Since we can't send EM_SETSEL until RTF has been set,
            // we can't rely on base to do it for us.
            base.SetSelectionOnHandle();

            if (ShowSelectionMargin)
            {
                // If you call SendMessage instead of PostMessage, the control
                // will resize itself to the size of the parent's client area.  Don't know why...
                UnsafeNativeMethods.PostMessage(new HandleRef(this, Handle), RichEditMessages.EM_SETOPTIONS, (IntPtr)RichTextBoxConstants.ECOOP_OR,
                                                (IntPtr)RichTextBoxConstants.ECO_SELECTIONBAR);
            }

            if (languageOption != LanguageOption)
            {
                LanguageOption = languageOption;
            }

            ClearUndo();

            SendZoomFactor(zoomMultiplier);

            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(UserPreferenceChangedHandler);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            if (!InConstructor)
            {
                textRtf = Rtf;
                if (textRtf.Length == 0)
                {
                    textRtf = null;
                }
            }

            oleCallback = null;
            SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(UserPreferenceChangedHandler);
        }

        /// <summary>
        ///  Fires an event when the user clicks a RichTextBox control's horizontal
        ///  scroll bar.
        /// </summary>
        protected virtual void OnHScroll(EventArgs e)
        {
            ((EventHandler)Events[EVENT_HSCROLL])?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires an event when the user clicks on a link
        ///  in a rich-edit control.
        /// </summary>
        protected virtual void OnLinkClicked(LinkClickedEventArgs e)
        {
            ((LinkClickedEventHandler)Events[EVENT_LINKACTIVATE])?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires an event when the user changes the control's IME conversion status.
        /// </summary>
        protected virtual void OnImeChange(EventArgs e)
        {
            ((EventHandler)Events[EVENT_IMECHANGE])?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires an event when the user is taking an action that would change
        ///  a protected range of text in the RichTextBox control.
        /// </summary>
        protected virtual void OnProtected(EventArgs e)
        {
            ProtectedError = true;
            ((EventHandler)Events[EVENT_PROTECTED])?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires an event when the current selection of text in the RichTextBox
        ///  control has changed or the insertion point has moved.
        /// </summary>
        protected virtual void OnSelectionChanged(EventArgs e)
        {
            ((EventHandler)Events[EVENT_SELCHANGE])?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires an event when the user clicks a RichTextBox control's vertical
        ///  scroll bar.
        /// </summary>
        protected virtual void OnVScroll(EventArgs e)
        {
            ((EventHandler)Events[EVENT_VSCROLL])?.Invoke(this, e);
        }

        /// <summary>
        ///  Pastes the contents of the clipboard in the given clipboard format.
        /// </summary>
        public void Paste(DataFormats.Format clipFormat)
        {
            UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_PASTESPECIAL, clipFormat.Id, 0);
        }

        protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            if (RichTextShortcutsEnabled == false)
            {
                foreach (int shortcutValue in shortcutsToDisable)
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
        public void Redo()
        {
            SendMessage(RichEditMessages.EM_REDO, 0, 0);
        }

        //NOTE: Undo is implemented on TextBox

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
            //valid values are 0x0 to 0x4
            if (!ClientUtils.IsEnumValid(fileType, (int)fileType, (int)RichTextBoxStreamType.RichText, (int)RichTextBoxStreamType.UnicodePlainText))
            {
                throw new InvalidEnumArgumentException(nameof(fileType), (int)fileType, typeof(RichTextBoxStreamType));
            }

            Stream file = File.Create(path);
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
            int flags;
            switch (fileType)
            {
                case RichTextBoxStreamType.RichText:
                    flags = RichTextBoxConstants.SF_RTF;
                    break;
                case RichTextBoxStreamType.PlainText:
                    flags = RichTextBoxConstants.SF_TEXT;
                    break;
                case RichTextBoxStreamType.UnicodePlainText:
                    flags = RichTextBoxConstants.SF_UNICODE | RichTextBoxConstants.SF_TEXT;
                    break;
                case RichTextBoxStreamType.RichNoOleObjs:
                    flags = RichTextBoxConstants.SF_RTFNOOBJS;
                    break;
                case RichTextBoxStreamType.TextTextOleObjs:
                    flags = RichTextBoxConstants.SF_TEXTIZED;
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(fileType), (int)fileType, typeof(RichTextBoxStreamType));
            }

            StreamOut(data, flags, true);
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
                SendMessage(RichEditMessages.EM_SETZOOM, numerator, denominator);

#if DEBUG

                // DEBUG CODE: Verify that EM_SETZOOM actually set the zoom
                int n = 0, d = 0;
                SendMessage(RichEditMessages.EM_GETZOOM, ref n, ref d);
                Debug.Assert(n == numerator && d == denominator, "EM_SETZOOM failed");
                // END DEBUG CODE
#endif
            }

            if (numerator != 0)
            {
                zoomMultiplier = ((float)numerator) / ((float)denominator);
            }
            else
            {
                zoomMultiplier = 1.0f;
            }
        }

        private bool SetCharFormat(int mask, int effect, RichTextBoxSelectionAttribute charFormat)
        {
            // check to see if the control has been created
            if (IsHandleCreated)
            {
                NativeMethods.CHARFORMATA cf = new NativeMethods.CHARFORMATA
                {
                    dwMask = mask
                };

                switch (charFormat)
                {
                    case RichTextBoxSelectionAttribute.All:
                        cf.dwEffects = effect;
                        break;
                    case RichTextBoxSelectionAttribute.None:
                        cf.dwEffects = 0;
                        break;
                    default:
                        throw new ArgumentException(SR.UnknownAttr);
                }

                // set the format information
                return IntPtr.Zero != UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_SETCHARFORMAT, RichTextBoxConstants.SCF_SELECTION, cf);
            }
            return false;
        }

        private bool SetCharFormat(int charRange, NativeMethods.CHARFORMATA cf)
        {
            return IntPtr.Zero != UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_SETCHARFORMAT, charRange, cf);
        }

        private unsafe void SetCharFormatFont(bool selectionOnly, Font value)
        {
            ForceHandleCreate();

            int dwMask = RichTextBoxConstants.CFM_FACE | RichTextBoxConstants.CFM_SIZE | RichTextBoxConstants.CFM_BOLD |
                RichTextBoxConstants.CFM_ITALIC | RichTextBoxConstants.CFM_STRIKEOUT | RichTextBoxConstants.CFM_UNDERLINE |
                RichTextBoxConstants.CFM_CHARSET;

            int dwEffects = 0;
            if (value.Bold)
            {
                dwEffects |= RichTextBoxConstants.CFE_BOLD;
            }

            if (value.Italic)
            {
                dwEffects |= RichTextBoxConstants.CFE_ITALIC;
            }

            if (value.Strikeout)
            {
                dwEffects |= RichTextBoxConstants.CFE_STRIKEOUT;
            }

            if (value.Underline)
            {
                dwEffects |= RichTextBoxConstants.CFE_UNDERLINE;
            }

            NativeMethods.LOGFONTW logFont = NativeMethods.LOGFONTW.FromFont(value);
            NativeMethods.CHARFORMATW charFormat = new NativeMethods.CHARFORMATW
            {
                cbSize = sizeof(NativeMethods.CHARFORMATW),
                dwMask = dwMask,
                dwEffects = dwEffects,
                yHeight = (int)(value.SizeInPoints * 20),
                bCharSet = logFont.lfCharSet,
                bPitchAndFamily = logFont.lfPitchAndFamily,
                FaceName = logFont.FaceName
            };

            UnsafeNativeMethods.SendMessage(
                new HandleRef(this, Handle),
                RichEditMessages.EM_SETCHARFORMAT,
                selectionOnly ? RichTextBoxConstants.SCF_SELECTION : RichTextBoxConstants.SCF_ALL,
                ref charFormat);
        }

        private static void SetupLogPixels(IntPtr hDC)
        {
            using ScreenDC dc = ScreenDC.Create();
            logPixelsX = Gdi32.GetDeviceCaps(dc, Gdi32.DeviceCapability.LOGPIXELSX);
            logPixelsY = Gdi32.GetDeviceCaps(dc, Gdi32.DeviceCapability.LOGPIXELSY);
        }

        private static int Pixel2Twip(IntPtr hDC, int v, bool xDirection)
        {
            SetupLogPixels(hDC);
            int logP = xDirection ? logPixelsX : logPixelsY;
            return (int)((((double)v) / logP) * 72.0 * 20.0);
        }

        private static int Twip2Pixel(IntPtr hDC, int v, bool xDirection)
        {
            SetupLogPixels(hDC);
            int logP = xDirection ? logPixelsX : logPixelsY;
            return (int)(((((double)v) / 20.0) / 72.0) * logP);
        }

        private void StreamIn(string str, int flags)
        {
            if (str.Length == 0)
            {
                // Destroy the selection if callers was setting
                // selection text
                //
                if ((RichTextBoxConstants.SFF_SELECTION & flags) != 0)
                {
                    SendMessage(WindowMessages.WM_CLEAR, 0, 0);
                    ProtectedError = false;
                    return;
                }
                // WM_SETTEXT is allowed even if we have protected text
                //
                SendMessage(WindowMessages.WM_SETTEXT, 0, "");
                return;
            }

            // Rather than work only some of the time with null characters,
            // we're going to be consistent and never work with them.
            int nullTerminatedLength = str.IndexOf((char)0);
            if (nullTerminatedLength != -1)
            {
                str = str.Substring(0, nullTerminatedLength);
            }

            // get the string into a byte array
            byte[] encodedBytes;
            if ((flags & RichTextBoxConstants.SF_UNICODE) != 0)
            {
                encodedBytes = Encoding.Unicode.GetBytes(str);
            }
            else
            {
                encodedBytes = Encoding.Default.GetBytes(str);
            }
            editStream = new MemoryStream(encodedBytes.Length);
            editStream.Write(encodedBytes, 0, encodedBytes.Length);
            editStream.Position = 0;
            StreamIn(editStream, flags);
        }

        private void StreamIn(Stream data, int flags)
        {
            // clear out the selection only if we are replacing all the text
            //
            if ((flags & RichTextBoxConstants.SFF_SELECTION) == 0)
            {
                var cr = new Richedit.CHARRANGE();
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_EXSETSEL, 0, ref cr);
            }

            try
            {
                editStream = data;
                Debug.Assert(data != null, "StreamIn passed a null stream");

                // If SF_RTF is requested then check for the RTF tag at the start
                // of the file.  We don't load if the tag is not there
                //
                if ((flags & RichTextBoxConstants.SF_RTF) != 0)
                {
                    long streamStart = editStream.Position;
                    byte[] bytes = new byte[SZ_RTF_TAG.Length];
                    editStream.Read(bytes, (int)streamStart, SZ_RTF_TAG.Length);
                    string str = Encoding.Default.GetString(bytes);
                    if (!SZ_RTF_TAG.Equals(str))
                    {
                        throw new ArgumentException(SR.InvalidFileFormat);
                    }

                    // put us back at the start of the file
                    editStream.Position = streamStart;
                }

                int cookieVal = 0;
                // set up structure to do stream operation
                NativeMethods.EDITSTREAM es = new NativeMethods.EDITSTREAM();
                if ((flags & RichTextBoxConstants.SF_UNICODE) != 0)
                {
                    cookieVal = INPUT | UNICODE;
                }
                else
                {
                    cookieVal = INPUT | ANSI;
                }
                if ((flags & RichTextBoxConstants.SF_RTF) != 0)
                {
                    cookieVal |= RTF;
                }
                else
                {
                    cookieVal |= TEXTLF;
                }
                es.dwCookie = (IntPtr)cookieVal;
                es.pfnCallback = new NativeMethods.EditStreamCallback(EditStreamProc);

                // gives us TextBox compatible behavior, programatic text change shouldn't
                // be limited...
                //
                SendMessage(RichEditMessages.EM_EXLIMITTEXT, 0, int.MaxValue);

                // go get the text for the control
                // Needed for 64-bit
                if (IntPtr.Size == 8)
                {
                    NativeMethods.EDITSTREAM64 es64 = ConvertToEDITSTREAM64(es);
                    UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_STREAMIN, flags, es64);

                    //Assign back dwError value
                    es.dwError = GetErrorValue64(es64);
                }
                else
                {
                    UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_STREAMIN, flags, es);
                }

                UpdateMaxLength();

                // If we failed to load because of protected
                // text then return protect event was fired so no
                // exception is required for the the error
                if (GetProtectedError())
                {
                    return;
                }

                if (es.dwError != 0)
                {
                    throw new InvalidOperationException(SR.LoadTextError);
                }

                // set the modify tag on the control
                SendMessage(EditMessages.EM_SETMODIFY, -1, 0);

                // EM_GETLINECOUNT will cause the RichTextBox to recalculate its line indexes
                SendMessage(EditMessages.EM_GETLINECOUNT, 0, 0);

            }
            finally
            {
                // release any storage space held.
                editStream = null;
            }
        }

        private string StreamOut(int flags)
        {
            Stream stream = new MemoryStream();
            StreamOut(stream, flags, false);
            stream.Position = 0;
            int streamLength = (int)stream.Length;
            string result = string.Empty;

            if (streamLength > 0)
            {
                byte[] bytes = new byte[streamLength];
                stream.Read(bytes, 0, streamLength);

                if ((flags & RichTextBoxConstants.SF_UNICODE) != 0)
                {
                    result = Encoding.Unicode.GetString(bytes, 0, bytes.Length);
                }
                else
                {
                    result = Encoding.Default.GetString(bytes, 0, bytes.Length);
                }
                // workaround ??? for

                if (!string.IsNullOrEmpty(result) && (result[result.Length - 1] == '\0'))
                {
                    result = result.Substring(0, result.Length - 1);
                }
            }

            return result;
        }

        private void StreamOut(Stream data, int flags, bool includeCrLfs)
        {
            // set up the EDITSTREAM structure for the callback.
            Debug.Assert(data != null, "StreamOut passed a null stream");
            editStream = data;

            try
            {
                int cookieVal = 0;
                NativeMethods.EDITSTREAM es = new NativeMethods.EDITSTREAM();
                if ((flags & RichTextBoxConstants.SF_UNICODE) != 0)
                {
                    cookieVal = OUTPUT | UNICODE;
                }
                else
                {
                    cookieVal = OUTPUT | ANSI;
                }
                if ((flags & RichTextBoxConstants.SF_RTF) != 0)
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
                es.dwCookie = (IntPtr)cookieVal;
                es.pfnCallback = new NativeMethods.EditStreamCallback(EditStreamProc);

                // Get Text
                // Needed for 64-bit
                if (IntPtr.Size == 8)
                {
                    NativeMethods.EDITSTREAM64 es64 = ConvertToEDITSTREAM64(es);
                    UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_STREAMOUT, flags, es64);

                    //Assign back dwError value
                    es.dwError = GetErrorValue64(es64);
                }
                else
                {
                    UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_STREAMOUT, flags, es);
                }

                // check to make sure things went well
                if (es.dwError != 0)
                {
                    throw new InvalidOperationException(SR.SaveTextError);
                }
            }
            finally
            {
                // release any storage space held.
                editStream = null;
            }
        }

        private unsafe NativeMethods.EDITSTREAM64 ConvertToEDITSTREAM64(NativeMethods.EDITSTREAM es)
        {
            NativeMethods.EDITSTREAM64 es64 = new NativeMethods.EDITSTREAM64();

            fixed (byte* es64p = &es64.contents[0])
            {
                byte* bp;
                long l;

                /*
                l = (long) es.dwCookie;
                bp = (byte *) &l;
                for (int i=0; i < sizeof(long); i++) {
                    es64.contents[i] = bp[i];
                }*/
                *((long*)es64p) = (long)es.dwCookie;
                /*
                int il = es.dwError;
                bp = (byte *) &il;
                for (int i=0; i < sizeof(int); i++) {
                    es64.contents[i+8] = bp[i];
                }*/
                *((int*)(es64p + 8)) = es.dwError;

                l = (long)Marshal.GetFunctionPointerForDelegate(es.pfnCallback);
                bp = (byte*)&l;
                for (int i = 0; i < sizeof(long); i++)
                {
                    es64.contents[i + 12] = bp[i];
                }
                //*((long *)(es64p + 12)) = (long) Marshal.GetFunctionPointerForDelegate(es.pfnCallback);
            }

            return es64;
        }

        private unsafe int GetErrorValue64(NativeMethods.EDITSTREAM64 es64)
        {
            int errorVal;

            fixed (byte* es64p = &es64.contents[0])
            {
                errorVal = *((int*)(es64p + 8));
            }

            return errorVal;
        }

        /* FOR 64 BIT DEBUGGING
                private unsafe string PrintBytes(NativeMethods.EDITSTREAM es) {
                    StringBuilder sb = new StringBuilder();
                    fixed (IntPtr *ip = &es.dwCookie) {
                        byte *bytep = (byte *) ip;
                        for (int i=0; i < Marshal.SizeOf(es); i++) {
                            sb.Append(bytep[i].ToString() + " ");
                        }
                    }
                    return sb.ToString();
                }

                private unsafe string PrintBytes(NativeMethods.EDITSTREAM64 es64) {
                    StringBuilder sb = new StringBuilder();
                    fixed (byte *bytep = &es64.contents[0]) {
                        for (int i=0; i < Marshal.SizeOf(es64); i++) {
                            sb.Append(bytep[i].ToString() + " ");
                        }
                    }
                    return sb.ToString();
                }
        */

        private void UpdateOleCallback()
        {
            Debug.WriteLineIf(RichTextDbg.TraceVerbose, "update ole callback (" + AllowDrop + ")");
            if (IsHandleCreated)
            {
                if (oleCallback == null)
                {
                    Debug.WriteLineIf(RichTextDbg.TraceVerbose, "binding ole callback");

                    AllowOleObjects = true;

                    oleCallback = CreateRichEditOleCallback();

                    // Forcibly QI (through IUnknown::QueryInterface) to handle multiple
                    // definitions of the interface.
                    //
                    IntPtr punk = Marshal.GetIUnknownForObject(oleCallback);
                    try
                    {
                        Guid iidRichEditOleCallback = typeof(Richedit.IRichEditOleCallback).GUID;
                        Marshal.QueryInterface(punk, ref iidRichEditOleCallback, out IntPtr pRichEditOleCallback);
                        try
                        {
                            UnsafeNativeMethods.SendCallbackMessage(new HandleRef(this, Handle), RichEditMessages.EM_SETOLECALLBACK, IntPtr.Zero, pRichEditOleCallback);
                        }
                        finally
                        {
                            Marshal.Release(pRichEditOleCallback);
                        }
                    }
                    finally
                    {
                        Marshal.Release(punk);
                    }
                }
                UnsafeNativeMethods.DragAcceptFiles(new HandleRef(this, Handle), false);
            }
        }

        //Note: RichTextBox doesn't work like other controls as far as setting ForeColor/
        //BackColor -- you need to send messages to update the colors
        private void UserPreferenceChangedHandler(object o, UserPreferenceChangedEventArgs e)
        {
            if (IsHandleCreated)
            {
                if (BackColor.IsSystemColor)
                {
                    SendMessage(RichEditMessages.EM_SETBKGNDCOLOR, 0, ColorTranslator.ToWin32(BackColor));
                }
                if (ForeColor.IsSystemColor)
                {
                    InternalSetForeColor(ForeColor);
                }
            }
        }

        /// <summary>
        ///  Creates the IRichEditOleCallback compatible object for handling RichEdit callbacks. For more
        ///  information look up the MSDN info on this interface. This is designed to be a back door of
        ///  sorts, which is why it is fairly obscure, and uses the RichEdit name instead of RichTextBox.
        /// </summary>
        protected virtual object CreateRichEditOleCallback()
        {
            return new OleCallback(this);
        }

        /// <summary>
        ///  Handles link messages (mouse move, down, up, dblclk, etc)
        /// </summary>
        private void EnLinkMsgHandler(ref Message m)
        {
            NativeMethods.ENLINK enlink;
            //On 64-bit, we do some custom marshalling to get this to work. The richedit control
            //unfortunately does not respect IA64 struct alignment conventions.
            if (IntPtr.Size == 8)
            {
                enlink = ConvertFromENLINK64((NativeMethods.ENLINK64)m.GetLParam(typeof(NativeMethods.ENLINK64)));
            }
            else
            {
                enlink = (NativeMethods.ENLINK)m.GetLParam(typeof(NativeMethods.ENLINK));
            }

            switch (enlink.msg)
            {
                case WindowMessages.WM_SETCURSOR:
                    LinkCursor = true;
                    m.Result = (IntPtr)1;
                    return;
                // Mouse-down triggers Url; this matches Outlook 2000's behavior.
                case WindowMessages.WM_LBUTTONDOWN:
                    string linktext = CharRangeToString(enlink.charrange);
                    if (!string.IsNullOrEmpty(linktext))
                    {
                        OnLinkClicked(new LinkClickedEventArgs(linktext));
                    }
                    m.Result = (IntPtr)1;
                    return;
            }
            m.Result = IntPtr.Zero;
            return;
        }

        /// <summary>
        ///  Converts a CHARRANGE to a string.
        /// </summary>
        /// <remarks>
        ///  The behavior of this is dependent on the current window class name being used. 
        ///  We have to create a CharBuffer of the type of RichTextBox DLL we're using,
        ///  not based on the SystemCharWidth.
        /// </remarks>
        private string CharRangeToString(Richedit.CHARRANGE c)
        {
            NativeMethods.TEXTRANGE txrg = new NativeMethods.TEXTRANGE
            {
                chrg = c
            };

            Debug.Assert((c.cpMax - c.cpMin) > 0, "CHARRANGE was null or negative - can't do it!");
            if (c.cpMax - c.cpMin <= 0)
            {
                return string.Empty;
            }

            int characters = (c.cpMax - c.cpMin) + 1; // +1 for null termination
            UnsafeNativeMethods.CharBuffer charBuffer = UnsafeNativeMethods.CharBuffer.CreateBuffer(characters);
            IntPtr unmanagedBuffer = charBuffer.AllocCoTaskMem();
            if (unmanagedBuffer == IntPtr.Zero)
            {
                throw new OutOfMemoryException(SR.OutOfMemory);
            }

            txrg.lpstrText = unmanagedBuffer;
            int len = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), RichEditMessages.EM_GETTEXTRANGE, 0, txrg);
            Debug.Assert(len != 0, "CHARRANGE from RichTextBox was bad! - impossible?");
            charBuffer.PutCoTaskMem(unmanagedBuffer);
            if (txrg.lpstrText != IntPtr.Zero)
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
                SendMessage(RichEditMessages.EM_EXLIMITTEXT, 0, MaxLength);
            }
        }

        private void WmReflectCommand(ref Message m)
        {
            // We check if we're in the middle of handle creation because
            // the rich edit control fires spurious events during this time.
            //
            if (m.LParam == Handle && !GetState(STATE_CREATINGHANDLE))
            {
                switch (Util.HIWORD(m.WParam))
                {

                    case NativeMethods.EN_HSCROLL:
                        OnHScroll(EventArgs.Empty);
                        break;

                    case NativeMethods.EN_VSCROLL:
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

        internal void WmReflectNotify(ref Message m)
        {
            if (m.HWnd == Handle)
            {
                NativeMethods.NMHDR nmhdr = (NativeMethods.NMHDR)m.GetLParam(typeof(NativeMethods.NMHDR));
                switch (nmhdr.code)
                {
                    case RichTextBoxConstants.EN_LINK:
                        EnLinkMsgHandler(ref m);
                        break;
                    case RichTextBoxConstants.EN_DROPFILES:
                        NativeMethods.ENDROPFILES endropfiles = (NativeMethods.ENDROPFILES)m.GetLParam(typeof(NativeMethods.ENDROPFILES));

                        // Only look at the first file.
                        StringBuilder path = new StringBuilder(Kernel32.MAX_PATH);
                        if (UnsafeNativeMethods.DragQueryFileLongPath(new HandleRef(endropfiles, endropfiles.hDrop), 0, path) != 0)
                        {
                            // Try to load the file as an RTF
                            try
                            {
                                LoadFile(path.ToString(), RichTextBoxStreamType.RichText);
                            }
                            catch
                            {
                                // we failed to load as rich text so try it as plain text
                                try
                                {
                                    LoadFile(path.ToString(), RichTextBoxStreamType.PlainText);
                                }
                                catch
                                {
                                    // ignore any problems we have
                                }
                            }
                        }
                        m.Result = (IntPtr)1;   // tell them we did the drop
                        break;

                    case RichTextBoxConstants.EN_REQUESTRESIZE:
                        if (!CallOnContentsResized)
                        {
                            NativeMethods.REQRESIZE reqResize = (NativeMethods.REQRESIZE)m.GetLParam(typeof(NativeMethods.REQRESIZE));
                            if (BorderStyle == System.Windows.Forms.BorderStyle.Fixed3D)
                            {
                                reqResize.rc.bottom++;
                            }
                            OnContentsResized(new ContentsResizedEventArgs(Rectangle.FromLTRB(reqResize.rc.left, reqResize.rc.top, reqResize.rc.right, reqResize.rc.bottom)));
                        }
                        break;

                    case RichTextBoxConstants.EN_SELCHANGE:
                        NativeMethods.SELCHANGE selChange = (NativeMethods.SELCHANGE)m.GetLParam(typeof(NativeMethods.SELCHANGE));
                        WmSelectionChange(selChange);
                        break;

                    case RichTextBoxConstants.EN_PROTECTED:
                        {
                            NativeMethods.ENPROTECTED enprotected;

                            //On 64-bit, we do some custom marshalling to get this to work. The richedit control
                            //unfortunately does not respect IA64 struct alignment conventions.
                            if (IntPtr.Size == 8)
                            {
                                enprotected = ConvertFromENPROTECTED64((NativeMethods.ENPROTECTED64)m.GetLParam(typeof(NativeMethods.ENPROTECTED64)));
                            }
                            else
                            {
                                enprotected = (NativeMethods.ENPROTECTED)m.GetLParam(typeof(NativeMethods.ENPROTECTED));
                            }

                            switch (enprotected.msg)
                            {
                                case RichEditMessages.EM_SETCHARFORMAT:
                                    // Allow change of protected style
                                    //
                                    NativeMethods.CHARFORMATA charFormat = Marshal.PtrToStructure<NativeMethods.CHARFORMATA>(enprotected.lParam);
                                    if ((charFormat.dwMask & RichTextBoxConstants.CFM_PROTECTED) != 0)
                                    {
                                        m.Result = IntPtr.Zero;
                                        return;
                                    }
                                    break;

                                // Throw an exception for the following
                                //
                                case RichEditMessages.EM_SETPARAFORMAT:
                                case EditMessages.EM_REPLACESEL:
                                    break;

                                case RichEditMessages.EM_STREAMIN:
                                    // Don't allow STREAMIN to replace protected selection
                                    //
                                    if ((unchecked((int)(long)enprotected.wParam) & RichTextBoxConstants.SFF_SELECTION) != 0)
                                    {
                                        break;
                                    }

                                    m.Result = IntPtr.Zero;
                                    return;

                                // Allow the following
                                //
                                case WindowMessages.WM_COPY:
                                case WindowMessages.WM_SETTEXT:
                                case RichEditMessages.EM_EXLIMITTEXT:
                                    m.Result = IntPtr.Zero;
                                    return;

                                // Beep and disallow change for all other messages
                                //
                                default:
                                    SafeNativeMethods.MessageBeep(0);
                                    break;
                            }

                            OnProtected(EventArgs.Empty);
                            m.Result = (IntPtr)1;
                            break;
                        }

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

        private unsafe NativeMethods.ENPROTECTED ConvertFromENPROTECTED64(NativeMethods.ENPROTECTED64 es64)
        {
            NativeMethods.ENPROTECTED es = new NativeMethods.ENPROTECTED();

            fixed (byte* es64p = &es64.contents[0])
            {
                es.nmhdr = new NativeMethods.NMHDR();
                es.chrg = new Richedit.CHARRANGE();

                es.nmhdr.hwndFrom = Marshal.ReadIntPtr((IntPtr)es64p);
                es.nmhdr.idFrom = Marshal.ReadIntPtr((IntPtr)(es64p + 8));
                es.nmhdr.code = Marshal.ReadInt32((IntPtr)(es64p + 16));
                es.msg = Marshal.ReadInt32((IntPtr)(es64p + 24));
                es.wParam = Marshal.ReadIntPtr((IntPtr)(es64p + 28));
                es.lParam = Marshal.ReadIntPtr((IntPtr)(es64p + 36));
                es.chrg.cpMin = Marshal.ReadInt32((IntPtr)(es64p + 44));
                es.chrg.cpMax = Marshal.ReadInt32((IntPtr)(es64p + 48));
            }

            return es;
        }

        private static unsafe NativeMethods.ENLINK ConvertFromENLINK64(NativeMethods.ENLINK64 es64)
        {
            NativeMethods.ENLINK es = new NativeMethods.ENLINK();

            fixed (byte* es64p = &es64.contents[0])
            {
                es.nmhdr = new NativeMethods.NMHDR();
                es.charrange = new Richedit.CHARRANGE();

                es.nmhdr.hwndFrom = Marshal.ReadIntPtr((IntPtr)es64p);
                es.nmhdr.idFrom = Marshal.ReadIntPtr((IntPtr)(es64p + 8));
                es.nmhdr.code = Marshal.ReadInt32((IntPtr)(es64p + 16));
                es.msg = Marshal.ReadInt32((IntPtr)(es64p + 24));
                es.wParam = Marshal.ReadIntPtr((IntPtr)(es64p + 28));
                es.lParam = Marshal.ReadIntPtr((IntPtr)(es64p + 36));
                es.charrange.cpMin = Marshal.ReadInt32((IntPtr)(es64p + 44));
                es.charrange.cpMax = Marshal.ReadInt32((IntPtr)(es64p + 48));
            }

            return es;
        }

        private void WmSelectionChange(NativeMethods.SELCHANGE selChange)
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
            if (ImeMode == ImeMode.Hangul || ImeMode == ImeMode.HangulFull)
            {

                // Is the IME CompositionWindow open?
                int compMode = unchecked((int)(long)SendMessage(RichEditMessages.EM_GETIMECOMPMODE, 0, 0));
                if (RichTextBoxConstants.ICM_NOTOPEN != compMode)
                {

                    int textLength = User32.GetWindowTextLengthW(new HandleRef(this, Handle));
                    if (selStart == selEnd && textLength == MaxLength)
                    {

                        SendMessage(WindowMessages.WM_KILLFOCUS, 0, 0);
                        SendMessage(WindowMessages.WM_SETFOCUS, 0, 0);
                        UnsafeNativeMethods.PostMessage(new HandleRef(this, Handle), EditMessages.EM_SETSEL, selEnd - 1, selEnd);
                    }
                }
            }

            if (selStart != curSelStart || selEnd != curSelEnd || selType != curSelType)
            {
                curSelStart = selStart;
                curSelEnd = selEnd;
                curSelType = selType;
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

        //
        // </doc>
        //
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WindowMessages.WM_REFLECT + WindowMessages.WM_NOTIFY:
                    WmReflectNotify(ref m);
                    break;

                case WindowMessages.WM_REFLECT + WindowMessages.WM_COMMAND:
                    WmReflectCommand(ref m);
                    break;

                case WindowMessages.WM_SETCURSOR:
                    //NOTE: RichTextBox uses the WM_SETCURSOR message over links to allow us to
                    //      change the cursor to a hand. It does this through a synchronous notification
                    //      message. So we have to pass the message to the DefWndProc first, and
                    //      then, if we receive a notification message in the meantime (indicated by
                    //      changing "LinkCursor", we set it to a hand. Otherwise, we call the
                    //      WM_SETCURSOR implementation on Control to set it to the user's selection for
                    //      the RichTextBox's cursor.
                    //
                    //      Similarly,
                    LinkCursor = false;
                    DefWndProc(ref m);
                    if (LinkCursor && !Cursor.Equals(Cursors.WaitCursor))
                    {
                        UnsafeNativeMethods.SetCursor(new HandleRef(Cursors.Hand, Cursors.Hand.Handle));
                        m.Result = (IntPtr)1;
                    }
                    else
                    {
                        base.WndProc(ref m);
                    }
                    break;

                case WindowMessages.WM_SETFONT:
                    WmSetFont(ref m);
                    break;

                case WindowMessages.WM_IME_NOTIFY:
                    OnImeChange(EventArgs.Empty);
                    base.WndProc(ref m);
                    break;

                case WindowMessages.WM_GETDLGCODE:
                    base.WndProc(ref m);
                    m.Result = (IntPtr)((AcceptsTab) ? unchecked((int)(long)m.Result) | NativeMethods.DLGC_WANTTAB : unchecked((int)(long)m.Result) & ~NativeMethods.DLGC_WANTTAB);
                    break;

                case WindowMessages.WM_GETOBJECT:
                    base.WndProc(ref m);

                    // OLEACC.DLL uses window class names to identify standard control types. But WinForm controls use app-specific window
                    // classes. Usually this doesn't matter, because system controls always identify their window class explicitly through
                    // the WM_GETOBJECT+OBJID_QUERYCLASSNAMEIDX message. But RICHEDIT20 doesn't do that - so we must do it ourselves.
                    // Otherwise OLEACC will treat rich edit controls as custom controls, so the accessible Role and Value will be wrong.
                    if (unchecked((int)(long)m.LParam) == NativeMethods.OBJID_QUERYCLASSNAMEIDX)
                    {
                        m.Result = (IntPtr)(65536 + 30);
                    }
                    break;

                case WindowMessages.WM_RBUTTONUP:
                    //since RichEdit eats up the WM_CONTEXTMENU message, we need to force DefWndProc
                    //to spit out this message again on receiving WM_RBUTTONUP message. By setting UserMouse
                    //style to true, we effectily let the WmMouseUp method in Control.cs to generate
                    //the WM_CONTEXTMENU message for us.
                    bool oldStyle = GetStyle(ControlStyles.UserMouse);
                    SetStyle(ControlStyles.UserMouse, true);
                    base.WndProc(ref m);
                    SetStyle(ControlStyles.UserMouse, oldStyle);
                    break;

                case WindowMessages.WM_VSCROLL:
                    base.WndProc(ref m);
                    int loWord = Util.LOWORD(m.WParam);
                    if (loWord == NativeMethods.SB_THUMBTRACK)
                    {
                        OnVScroll(EventArgs.Empty);
                    }
                    else
                        if (loWord == NativeMethods.SB_THUMBPOSITION)
                    {
                        OnVScroll(EventArgs.Empty);
                    }
                    break;

                case WindowMessages.WM_HSCROLL:
                    base.WndProc(ref m);
                    loWord = Util.LOWORD(m.WParam);
                    if (loWord == NativeMethods.SB_THUMBTRACK)
                    {
                        OnHScroll(EventArgs.Empty);
                    }
                    if (loWord == NativeMethods.SB_THUMBPOSITION)
                    {
                        OnHScroll(EventArgs.Empty);
                    }
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        // I used the visual basic 6 RichText (REOleCB.CPP) as a guide for this
        private class OleCallback : Richedit.IRichEditOleCallback
        {
            private readonly RichTextBox owner;
            IDataObject lastDataObject;
            DragDropEffects lastEffect;

            internal OleCallback(RichTextBox owner)
            {
                this.owner = owner;
            }

            public HRESULT GetNewStorage(out Ole32.IStorage storage)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichEditOleCallback::GetNewStorage");
                if (!owner.AllowOleObjects)
                {
                    storage = null;
                    return HRESULT.E_FAIL;
                }

                Ole32.ILockBytes pLockBytes = Ole32.CreateILockBytesOnHGlobal(IntPtr.Zero, true);
                Debug.Assert(pLockBytes != null, "pLockBytes is NULL!");

                storage = Ole32.StgCreateDocfileOnILockBytes(
                    pLockBytes,
                    Ole32.STGM.STGM_SHARE_EXCLUSIVE | Ole32.STGM.STGM_CREATE | Ole32.STGM.STGM_READWRITE,
                    0);
                Debug.Assert(storage != null, "storage is NULL!");

                return HRESULT.S_OK;
            }

            public HRESULT GetInPlaceContext(IntPtr lplpFrame,
                                         IntPtr lplpDoc,
                                         IntPtr lpFrameInfo)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichEditOleCallback::GetInPlaceContext");
                return HRESULT.E_NOTIMPL;
            }

            public HRESULT ShowContainerUI(BOOL fShow)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichEditOleCallback::ShowContainerUI");
                // Do nothing
                return HRESULT.S_OK;
            }

            public HRESULT QueryInsertObject(ref Guid lpclsid, IntPtr lpstg, int cp)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichEditOleCallback::QueryInsertObject(" + lpclsid.ToString() + ")");
                return HRESULT.S_OK;
            }

            public HRESULT DeleteObject(IntPtr lpoleobj)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichEditOleCallback::DeleteObject");
                // Do nothing
                return HRESULT.S_OK;
            }

            public HRESULT QueryAcceptData(IComDataObject lpdataobj, IntPtr lpcfFormat, uint reco, BOOL fReally, IntPtr hMetaPict)
            {

                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichEditOleCallback::QueryAcceptData(reco=" + reco + ")");

                if (reco == NativeMethods.RECO_DROP)
                {
                    if (owner.AllowDrop || owner.EnableAutoDragDrop)
                    {

                        MouseButtons b = Control.MouseButtons;
                        Keys k = Control.ModifierKeys;

                        int keyState = 0;

                        // Due to the order in which we get called, we have to set up the keystate here.
                        // First GetDragDropEffect is called with grfKeyState == 0, and then
                        // QueryAcceptData is called. Since this is the time we want to fire
                        // OnDragEnter, but we have yet to get the keystate, we set it up ourselves.

                        if ((b & MouseButtons.Left) == MouseButtons.Left)
                        {
                            keyState |= NativeMethods.MK_LBUTTON;
                        }

                        if ((b & MouseButtons.Right) == MouseButtons.Right)
                        {
                            keyState |= NativeMethods.MK_RBUTTON;
                        }

                        if ((b & MouseButtons.Middle) == MouseButtons.Middle)
                        {
                            keyState |= NativeMethods.MK_MBUTTON;
                        }

                        if ((k & Keys.Control) == Keys.Control)
                        {
                            keyState |= NativeMethods.MK_CONTROL;
                        }

                        if ((k & Keys.Shift) == Keys.Shift)
                        {
                            keyState |= NativeMethods.MK_SHIFT;
                        }

                        lastDataObject = new DataObject(lpdataobj);

                        if (!owner.EnableAutoDragDrop)
                        {
                            lastEffect = DragDropEffects.None;
                        }

                        DragEventArgs e = new DragEventArgs(lastDataObject,
                                                        keyState,
                                                        Control.MousePosition.X,
                                                        Control.MousePosition.Y,
                                                        DragDropEffects.All,
                                                        lastEffect);
                        if (fReally == 0)
                        {
                            // we are just querying

                            // We can get here without GetDragDropEffects actually being called first.
                            // This happens when you drag/drop between two rtb's. Say you drag from rtb1 to rtb2.
                            // GetDragDropEffects will first be called for rtb1, then QueryAcceptData for rtb1 just
                            // like in the local drag case. Then you drag into rtb2. rtb2 will first be called in this method,
                            // and not GetDragDropEffects. Now lastEffect is initialized to None for rtb2, so we would not allow
                            // the drag. Thus we need to set the effect here as well.
                            e.Effect = ((keyState & NativeMethods.MK_CONTROL) == NativeMethods.MK_CONTROL) ? DragDropEffects.Copy : DragDropEffects.Move;
                            owner.OnDragEnter(e);
                        }
                        else
                        {
                            owner.OnDragDrop(e);
                            lastDataObject = null;
                        }

                        lastEffect = e.Effect;
                        if (e.Effect == DragDropEffects.None)
                        {
                            Debug.WriteLineIf(RichTextDbg.TraceVerbose, "\tCancel data");
                            return HRESULT.E_FAIL;
                        }
                        else
                        {
                            Debug.WriteLineIf(RichTextDbg.TraceVerbose, "\tAccept data");
                            return HRESULT.S_OK;
                        }
                    }
                    else
                    {
                        Debug.WriteLineIf(RichTextDbg.TraceVerbose, "\tCancel data, allowdrop == false");
                        lastDataObject = null;
                        return HRESULT.E_FAIL;
                    }
                }
                else
                {
                    return HRESULT.E_NOTIMPL;
                }
            }

            public HRESULT ContextSensitiveHelp(BOOL fEnterMode)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichEditOleCallback::ContextSensitiveHelp");
                return HRESULT.E_NOTIMPL;
            }

            public HRESULT GetClipboardData(ref Richedit.CHARRANGE lpchrg, uint reco, IntPtr lplpdataobj)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichEditOleCallback::GetClipboardData");
                return HRESULT.E_NOTIMPL;
            }

            public HRESULT GetDragDropEffect(BOOL fDrag, int grfKeyState, ref int pdwEffect)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichEditOleCallback::GetDragDropEffect");

                if (owner.AllowDrop || owner.EnableAutoDragDrop)
                {

                    if (fDrag != BOOL.FALSE && grfKeyState == 0)
                    {
                        // This is the very first call we receive in a Drag-Drop operation,
                        // so we will let the control know what we support.

                        // Note that we haven't gotten any data yet, so we will let QueryAcceptData
                        // do the OnDragEnter. Note too, that grfKeyState does not yet reflect the
                        // current keystate
                        if (owner.EnableAutoDragDrop)
                        {
                            lastEffect = (DragDropEffects.All | DragDropEffects.None);
                        }
                        else
                        {
                            lastEffect = DragDropEffects.None;
                        }
                    }
                    else
                    {
                        // We are either dragging over or dropping

                        // The below is the complete reverse of what the docs on MSDN suggest,
                        // but if we follow the docs, we would be firing OnDragDrop all the
                        // time instead of OnDragOver (see

                        // drag - fDrag = false, grfKeyState != 0
                        // drop - fDrag = false, grfKeyState = 0
                        // We only care about the drag.
                        //
                        // When we drop, lastEffect will have the right state
                        if (fDrag == BOOL.FALSE && lastDataObject != null && grfKeyState != 0)
                        {

                            DragEventArgs e = new DragEventArgs(lastDataObject,
                                                                grfKeyState,
                                                                Control.MousePosition.X,
                                                                Control.MousePosition.Y,
                                                                DragDropEffects.All,
                                                                lastEffect);

                            // Now tell which of the allowable effects we want to use, but only if we are not already none
                            if (lastEffect != DragDropEffects.None)
                            {
                                e.Effect = ((grfKeyState & NativeMethods.MK_CONTROL) == NativeMethods.MK_CONTROL) ? DragDropEffects.Copy : DragDropEffects.Move;
                            }

                            owner.OnDragOver(e);
                            lastEffect = e.Effect;
                        }
                    }

                    pdwEffect = (int)lastEffect;

                }
                else
                {
                    pdwEffect = (int)DragDropEffects.None;
                }
                return HRESULT.S_OK;
            }

            public HRESULT GetContextMenu(short seltype, IntPtr lpoleobj, ref Richedit.CHARRANGE lpchrg, out IntPtr hmenu)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichEditOleCallback::GetContextMenu");

                // do nothing, we don't have ContextMenu any longer
                hmenu = IntPtr.Zero;
                return HRESULT.S_OK;
            }
        }
    }
}

