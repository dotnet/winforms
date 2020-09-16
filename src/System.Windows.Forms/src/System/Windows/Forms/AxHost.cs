// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Wraps ActiveX controls and exposes them as fully featured windows forms controls.
    /// </summary>
    [ToolboxItem(false)]
    [DesignTimeVisible(false)]
    [DefaultEvent(nameof(Enter))]
    [Designer("System.Windows.Forms.Design.AxHostDesigner, " + AssemblyRef.SystemDesign)]
    public abstract partial class AxHost : Control, ISupportInitialize, ICustomTypeDescriptor
    {
        private static readonly TraceSwitch AxHTraceSwitch = new TraceSwitch("AxHTrace", "ActiveX handle tracing");
        private static readonly TraceSwitch AxPropTraceSwitch = new TraceSwitch("AxPropTrace", "ActiveX property tracing");
        private static readonly TraceSwitch AxHostSwitch = new TraceSwitch("AxHost", "ActiveX host creation");
#if DEBUG
        private static readonly BooleanSwitch AxAlwaysSaveSwitch = new BooleanSwitch("AxAlwaysSave", "ActiveX to save all controls regardless of their IsDirty function return value");
#endif

        /// <summary>
        ///  Flags which may be passed to the AxHost constructor
        /// </summary>
        internal class AxFlags
        {
            /// <summary>
            ///  Indicates that the context menu for the control should not contain an
            ///  "Edit" verb unless the activeX controls itself decides to proffer it.
            ///  By default, all wrapped activeX controls will contain an edit verb.
            /// </summary>
            internal const int PreventEditMode = 0x1;

            /// <summary>
            ///  Indicated that the context menu for the control should contain
            ///  a "Properties..." verb which may be used to show the property
            ///  pages for the control.  Note that even if this flag is
            ///  specified, the verb will not appear unless the control
            ///  proffers a set of property pages.
            ///  [Since most activeX controls alreay have their own properties verb
            ///  on the context menu, the default is not to include one specified by
            ///  this flag.]
            /// </summary>
            internal const int IncludePropertiesVerb = 0x2;

            /// <summary>
            /// </summary>
            internal const int IgnoreThreadModel = 0x10000000;
        }

        private static readonly COMException E_INVALIDARG = new COMException(SR.AXInvalidArgument, unchecked((int)0x80070057));
        private static readonly COMException E_FAIL = new COMException(SR.AXUnknownError, unchecked((int)0x80004005));

        private const int OC_PASSIVE = 0;
        private const int OC_LOADED = 1;  // handler, but no server   [ocx created]
        private const int OC_RUNNING = 2; // server running, invisible [iqa & depersistance]
        private const int OC_INPLACE = 4; // server in-place active [inplace]
        private const int OC_UIACTIVE = 8;// server is UI active [uiactive]
        private const int OC_OPEN = 16;    // server is being open edited [not used]

        private const int EDITM_NONE = 0;   // object not being edited
        private const int EDITM_OBJECT = 1; // object provided an edit verb and we invoked it
        private const int EDITM_HOST = 2;   // we invoked our own edit verb

        private const int STG_UNKNOWN = -1;
        private const int STG_STREAM = 0;
        private const int STG_STREAMINIT = 1;
        private const int STG_STORAGE = 2;

        private readonly User32.WM REGMSG_MSG = User32.RegisterWindowMessageW(Application.WindowMessagesVersion + "_subclassCheck");
        private const int REGMSG_RETVAL = 123;

        private static int logPixelsX = -1;
        private static int logPixelsY = -1;

        private static Guid icf2_Guid = typeof(Ole32.IClassFactory2).GUID;
        private static Guid ifont_Guid = typeof(Ole32.IFont).GUID;
        private static Guid ifontDisp_Guid = typeof(Ole32.IFontDisp).GUID;
        private static Guid ipicture_Guid = typeof(Ole32.IPicture).GUID;
        private static Guid ipictureDisp_Guid = typeof(Ole32.IPictureDisp).GUID;
        private static Guid ivbformat_Guid = typeof(Ole32.IVBFormat).GUID;
        private static Guid ioleobject_Guid = typeof(Ole32.IOleObject).GUID;
        private static Guid dataSource_Guid = new Guid("{7C0FFAB3-CD84-11D0-949A-00A0C91110ED}");
        private static Guid windowsMediaPlayer_Clsid = new Guid("{22d6f312-b0f6-11d0-94ab-0080c74c7e95}");
        private static Guid comctlImageCombo_Clsid = new Guid("{a98a24c0-b06f-3684-8c12-c52ae341e0bc}");
        private static Guid maskEdit_Clsid = new Guid("{c932ba85-4374-101b-a56c-00aa003668dc}");

        // Static state for perf optimization
        //
        private static Dictionary<Font, Oleaut32.FONTDESC> fontTable;

        // BitVector32 masks for various internal state flags.
        //
        private static readonly int ocxStateSet = BitVector32.CreateMask();
        private static readonly int editorRefresh = BitVector32.CreateMask(ocxStateSet);
        private static readonly int listeningToIdle = BitVector32.CreateMask(editorRefresh);
        private static readonly int refreshProperties = BitVector32.CreateMask(listeningToIdle);

        private static readonly int checkedIppb = BitVector32.CreateMask(refreshProperties);
        private static readonly int checkedCP = BitVector32.CreateMask(checkedIppb);
        private static readonly int fNeedOwnWindow = BitVector32.CreateMask(checkedCP);
        private static readonly int fOwnWindow = BitVector32.CreateMask(fNeedOwnWindow);

        private static readonly int fSimpleFrame = BitVector32.CreateMask(fOwnWindow);
        private static readonly int fFakingWindow = BitVector32.CreateMask(fSimpleFrame);
        private static readonly int rejectSelection = BitVector32.CreateMask(fFakingWindow);
        private static readonly int ownDisposing = BitVector32.CreateMask(rejectSelection);

        private static readonly int sinkAttached = BitVector32.CreateMask(ownDisposing);
        private static readonly int disposed = BitVector32.CreateMask(sinkAttached);
        private static readonly int manualUpdate = BitVector32.CreateMask(disposed);
        private static readonly int addedSelectionHandler = BitVector32.CreateMask(manualUpdate);

        private static readonly int valueChanged = BitVector32.CreateMask(addedSelectionHandler);
        private static readonly int handlePosRectChanged = BitVector32.CreateMask(valueChanged);
        private static readonly int siteProcessedInputKey = BitVector32.CreateMask(handlePosRectChanged);
        private static readonly int needLicenseKey = BitVector32.CreateMask(siteProcessedInputKey);

        private static readonly int inTransition = BitVector32.CreateMask(needLicenseKey);
        private static readonly int processingKeyUp = BitVector32.CreateMask(inTransition);
        private static readonly int assignUniqueID = BitVector32.CreateMask(processingKeyUp);
        private static readonly int renameEventHooked = BitVector32.CreateMask(assignUniqueID);

        private BitVector32 axState;

        private int storageType = STG_UNKNOWN;
        private int ocState = OC_PASSIVE;
        private Ole32.OLEMISC _miscStatusBits;
        private int freezeCount;
        private readonly int flags;
        private int selectionStyle;
        private int editMode = EDITM_NONE;
        private int noComponentChange;

        private IntPtr wndprocAddr = IntPtr.Zero;

        private Guid clsid;
        private string text = string.Empty;
        private string licenseKey;

        private readonly OleInterfaces oleSite;
        private AxComponentEditor editor;
        private AxContainer container;
        private ContainerControl containingControl;
        private ContainerControl newParent;
        private AxContainer axContainer;
        private State ocxState;
        private IntPtr hwndFocus = IntPtr.Zero;

        // CustomTypeDescriptor related state
        //
        private Hashtable properties;
        private Hashtable propertyInfos;
        private PropertyDescriptorCollection propsStash;
        private Attribute[] attribsStash;

        // interface pointers to the ocx
        //
        private object instance;
        private Ole32.IOleInPlaceObject iOleInPlaceObject;
        private Ole32.IOleObject iOleObject;
        private Ole32.IOleControl iOleControl;
        private Ole32.IOleInPlaceActiveObject iOleInPlaceActiveObject;
        private Ole32.IOleInPlaceActiveObject iOleInPlaceActiveObjectExternal;
        private Oleaut32.IPerPropertyBrowsing iPerPropertyBrowsing;
        private VSSDK.ICategorizeProperties iCategorizeProperties;
        private Oleaut32.IPersistPropertyBag iPersistPropBag;
        private Ole32.IPersistStream iPersistStream;
        private Ole32.IPersistStreamInit iPersistStreamInit;
        private Ole32.IPersistStorage iPersistStorage;

        private AboutBoxDelegate aboutBoxDelegate;
        private readonly EventHandler selectionChangeHandler;

        private readonly bool isMaskEdit;
        private bool ignoreDialogKeys;

        private readonly EventHandler onContainerVisibleChanged;

        // These should be in the order given by the PROPCAT_X values
        // Also, note that they are not to be localized...

        private static readonly CategoryAttribute[] categoryNames = new CategoryAttribute[] {
            null,
            new WinCategoryAttribute("Default"),
            new WinCategoryAttribute("Default"),
            new WinCategoryAttribute("Font"),
            new WinCategoryAttribute("Layout"),
            new WinCategoryAttribute("Appearance"),
            new WinCategoryAttribute("Behavior"),
            new WinCategoryAttribute("Data"),
            new WinCategoryAttribute("List"),
            new WinCategoryAttribute("Text"),
            new WinCategoryAttribute("Scale"),
            new WinCategoryAttribute("DDE")
        };

        private Hashtable objectDefinedCategoryNames; // Integer -> String

#if DEBUG
        static AxHost()
        {
            Debug.Assert((int)DockStyle.None == (int)NativeMethods.ActiveX.ALIGN_NO_CHANGE, "align value mismatch");
            Debug.Assert((int)DockStyle.Top == (int)NativeMethods.ActiveX.ALIGN_TOP, "align value mismatch");
            Debug.Assert((int)DockStyle.Bottom == (int)NativeMethods.ActiveX.ALIGN_BOTTOM, "align value mismatch");
            Debug.Assert((int)DockStyle.Left == (int)NativeMethods.ActiveX.ALIGN_LEFT, "align value mismatch");
            Debug.Assert((int)DockStyle.Right == (int)NativeMethods.ActiveX.ALIGN_RIGHT, "align value mismatch");
            Debug.Assert((int)MouseButtons.Left == 0x00100000, "mb.left mismatch");
            Debug.Assert((int)MouseButtons.Right == 0x00200000, "mb.right mismatch");
            Debug.Assert((int)MouseButtons.Middle == 0x00400000, "mb.middle mismatch");
            Debug.Assert((int)Keys.Shift == 0x00010000, "key.shift mismatch");
            Debug.Assert((int)Keys.Control == 0x00020000, "key.control mismatch");
            Debug.Assert((int)Keys.Alt == 0x00040000, "key.alt mismatch");
        }
#endif

        /// <summary>
        ///  Creates a new instance of a control which wraps an activeX control given by the
        ///  clsid parameter and flags of 0.
        /// </summary>
        protected AxHost(string clsid) : this(clsid, 0)
        {
        }

        /// <summary>
        ///  Creates a new instance of a control which wraps an activeX control given by the
        ///  clsid and flags parameters.
        /// </summary>
        protected AxHost(string clsid, int flags) : base()
        {
            if (Application.OleRequired() != ApartmentState.STA)
            {
                throw new ThreadStateException(string.Format(SR.AXMTAThread, clsid));
            }

            oleSite = new OleInterfaces(this);
            selectionChangeHandler = new EventHandler(OnNewSelection);
            this.clsid = new Guid(clsid);
            this.flags = flags;

            axState[assignUniqueID] = !GetType().GUID.Equals(comctlImageCombo_Clsid);
            axState[needLicenseKey] = true;
            axState[rejectSelection] = true;

            isMaskEdit = this.clsid.Equals(AxHost.maskEdit_Clsid);
            onContainerVisibleChanged = new EventHandler(OnContainerVisibleChanged);
        }

        private bool CanUIActivate
        {
            get
            {
                return IsUserMode() || editMode != EDITM_NONE;
            }
        }

        /// <summary>
        ///  Returns the CreateParams used to create the handle for this control.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (axState[fOwnWindow] && IsUserMode())
                {
                    cp.Style &= ~(int)User32.WS.VISIBLE;
                }
                return cp;
            }
        }

        private bool GetAxState(int mask)
        {
            return axState[mask];
        }

        private void SetAxState(int mask, bool value)
        {
            axState[mask] = value;
        }

        /// <summary>
        ///  AxHost will call this when it is ready to create the underlying ActiveX object.
        ///  Wrappers will override this and cast the pointer obtained by calling getOcx() to
        ///  their own interfaces.  getOcx() should not usually be called before this function.
        ///  Note: calling begin will result in a call to this function.
        /// </summary>
        protected virtual void AttachInterfaces()
        {
        }

        private unsafe void RealizeStyles()
        {
            SetStyle(ControlStyles.UserPaint, false);
            Ole32.OLEMISC bits = 0;
            HRESULT hr = GetOleObject().GetMiscStatus(Ole32.DVASPECT.CONTENT, &bits);
            if (hr.Succeeded())
            {
                _miscStatusBits = bits;
                ParseMiscBits(_miscStatusBits);
            }
        }

        // Control overrides:

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Color BackColor
        {
            get => base.BackColor;
            set => base.BackColor = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image BackgroundImage
        {
            get => base.BackgroundImage;
            set => base.BackgroundImage = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ImageLayout BackgroundImageLayout
        {
            get => base.BackgroundImageLayout;
            set => base.BackgroundImageLayout = value;
        }

        /// <summary>
        ///  Hide ImeMode: it doesn't make sense for this control
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        new public ImeMode ImeMode
        {
            get => base.ImeMode;
            set => base.ImeMode = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler MouseClick
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseClick"));
            remove { }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler MouseDoubleClick
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseDoubleClick"));
            remove { }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Cursor Cursor
        {
            get => base.Cursor;
            set => base.Cursor = value;
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

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual new bool Enabled
        {
            get => base.Enabled;
            set => base.Enabled = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Font Font
        {
            get => base.Font;
            set => base.Font = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Color ForeColor
        {
            get => base.ForeColor;
            set => base.ForeColor = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Localizable(true)]
        public new virtual bool RightToLeft
        {
            get
            {
                RightToLeft rtol = base.RightToLeft;
                return rtol == System.Windows.Forms.RightToLeft.Yes;
            }
            set => base.RightToLeft = (value) ? System.Windows.Forms.RightToLeft.Yes : System.Windows.Forms.RightToLeft.No;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
            }
        }

        internal override bool CanAccessProperties
        {
            get
            {
                int ocState = GetOcState();
#pragma warning disable SA1408 // Conditional expressions should declare precedence
                return (axState[fOwnWindow] &&
                       (ocState > OC_RUNNING || (IsUserMode() && ocState >= OC_RUNNING)) ||
                       ocState >= OC_INPLACE);
#pragma warning restore SA1408 // Conditional expressions should declare precedence
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected bool PropsValid()
        {
            return CanAccessProperties;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void BeginInit()
        {
        }

        /// <summary>
        ///  Signals the object that loading of all peer components and property
        ///  sets are complete.
        ///  It should be possible to invoke any property get or set after calling this method.
        ///  Note that a sideeffect of this method is the creation of the parent control's
        ///  handle, therefore, this control must be parented before begin is called
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void EndInit()
        {
            if (ParentInternal != null)
            {
                ParentInternal.CreateControl(true);

                ContainerControl f = ContainingControl;
                if (f != null)
                {
                    f.VisibleChanged += onContainerVisibleChanged;
                }
            }
        }

        private void OnContainerVisibleChanged(object sender, EventArgs e)
        {
            ContainerControl f = ContainingControl;
            if (f != null)
            {
                if (f.Visible && Visible && !axState[fOwnWindow])
                {
                    MakeVisibleWithShow();
                }
                else if (!f.Visible && Visible && IsHandleCreated && GetOcState() >= OC_INPLACE)
                {
                    HideAxControl();
                }
                else if (f.Visible && !GetState(States.Visible) && IsHandleCreated && GetOcState() >= OC_INPLACE)
                {
                    HideAxControl();
                }
            }
        }

        //
        /// <summary>
        ///  Determines if the control is in edit mode.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EditMode
        {
            get
            {
                return editMode != EDITM_NONE;
            }
        }

        /// <summary>
        ///  Determines if this control has an about box.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasAboutBox
        {
            get
            {
                return aboutBoxDelegate != null;
            }
        }

        private int NoComponentChangeEvents
        {
            get
            {
                return noComponentChange;
            }

            set
            {
                noComponentChange = value;
            }
        }

        //
        /// <summary>
        ///  Shows the about box for this control.
        /// </summary>
        public void ShowAboutBox()
        {
            aboutBoxDelegate?.Invoke();
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackColorChanged
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "BackColorChanged"));
            remove { }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageChanged
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "BackgroundImageChanged"));
            remove { }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageLayoutChanged
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "BackgroundImageLayoutChanged"));
            remove { }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BindingContextChanged
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "BindingContextChanged"));
            remove { }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler CursorChanged
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "CursorChanged"));
            remove { }
        }

        /// <summary>
        ///  Occurs when the control is enabled.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler EnabledChanged
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "EnabledChanged"));
            remove { }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler FontChanged
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "FontChanged"));
            remove { }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler ForeColorChanged
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "ForeColorChanged"));
            remove { }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler RightToLeftChanged
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "RightToLeftChanged"));
            remove { }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TextChanged
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "TextChanged"));
            remove { }
        }

        /// <summary>
        ///  Occurs when the control is clicked.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler Click
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "Click"));
            remove { }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event DragEventHandler DragDrop
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "DragDrop"));
            remove { }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event DragEventHandler DragEnter
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "DragEnter"));
            remove { }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event DragEventHandler DragOver
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "DragOver"));
            remove { }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler DragLeave
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "DragLeave"));
            remove { }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event GiveFeedbackEventHandler GiveFeedback
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "GiveFeedback"));
            remove { }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event HelpEventHandler HelpRequested
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "HelpRequested"));
            remove { }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event PaintEventHandler Paint
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "Paint"));
            remove { }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event QueryContinueDragEventHandler QueryContinueDrag
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "QueryContinueDrag"));
            remove { }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "QueryAccessibilityHelp"));
            remove { }
        }

        /// <summary>
        ///  Occurs when the control is double clicked.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler DoubleClick
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "DoubleClick"));
            remove { }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler ImeModeChanged
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "ImeModeChanged"));
            remove { }
        }

        /// <summary>
        ///  Occurs when a key is pressed down while the control has focus.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event KeyEventHandler KeyDown
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "KeyDown"));
            remove { }
        }

        /// <summary>
        ///  Occurs when a key is pressed while the control has focus.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event KeyPressEventHandler KeyPress
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "KeyPress"));
            remove { }
        }

        /// <summary>
        ///  Occurs when a key is released while the control has focus.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event KeyEventHandler KeyUp
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "KeyUp"));
            remove { }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event LayoutEventHandler Layout
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "Layout"));
            remove { }
        }

        /// <summary>
        ///  Occurs when the mouse pointer is over the control and a mouse button is
        ///  pressed.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event MouseEventHandler MouseDown
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseDown"));
            remove { }
        }

        /// <summary>
        ///  Occurs when the mouse pointer enters the AxHost.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler MouseEnter
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseEnter"));
            remove { }
        }

        /// <summary>
        ///  Occurs when the mouse pointer leaves the AxHost.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler MouseLeave
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseLeave"));
            remove { }
        }

        /// <summary>
        ///  Occurs when the mouse pointer hovers over the contro.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler MouseHover
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseHover"));
            remove { }
        }

        /// <summary>
        ///  Occurs when the mouse pointer is moved over the AxHost.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event MouseEventHandler MouseMove
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseMove"));
            remove { }
        }

        /// <summary>
        ///  Occurs when the mouse pointer is over the control and a mouse button is released.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event MouseEventHandler MouseUp
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseUp"));
            remove { }
        }

        /// <summary>
        ///  Occurs when the mouse wheel moves while the control has focus.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event MouseEventHandler MouseWheel
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseWheel"));
            remove { }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event UICuesEventHandler ChangeUICues
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "ChangeUICues"));
            remove { }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler StyleChanged
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "StyleChanged"));
            remove { }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            AmbientChanged(Ole32.DispatchID.AMBIENT_FONT);
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            AmbientChanged(Ole32.DispatchID.AMBIENT_FORECOLOR);
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            AmbientChanged(Ole32.DispatchID.AMBIENT_BACKCOLOR);
        }

        private void AmbientChanged(Ole32.DispatchID dispid)
        {
            if (GetOcx() != null)
            {
                try
                {
                    Invalidate();
                    GetOleControl().OnAmbientPropertyChange(dispid);
                }
                catch (Exception t)
                {
                    Debug.Fail(t.ToString());
                }
            }
        }

        private bool OwnWindow()
        {
            return axState[fOwnWindow] || axState[fFakingWindow];
        }

        private IntPtr GetHandleNoCreate()
        {
            if (IsHandleCreated)
            {
                return Handle;
            }

            return IntPtr.Zero;
        }

        private ISelectionService GetSelectionService()
        {
            return GetSelectionService(this);
        }

        private static ISelectionService GetSelectionService(Control ctl)
            => ctl.Site?.GetService(typeof(ISelectionService)) as ISelectionService;

        private void AddSelectionHandler()
        {
            if (axState[addedSelectionHandler])
            {
                return;
            }

            ISelectionService iss = GetSelectionService();
            if (iss != null)
            {
                iss.SelectionChanging += selectionChangeHandler;
            }
            axState[addedSelectionHandler] = true;
        }

        private void OnComponentRename(object sender, ComponentRenameEventArgs e)
        {
            // When we're notified of a rename, see if this is the componnent that is being
            // renamed.
            //
            if (e.Component == this)
            {
                // if it is, call DISPID_AMBIENT_DISPLAYNAME directly on the
                // control itself.
                if (GetOcx() is Ole32.IOleControl oleCtl)
                {
                    oleCtl.OnAmbientPropertyChange(Ole32.DispatchID.AMBIENT_DISPLAYNAME);
                }
            }
        }

        private bool RemoveSelectionHandler()
        {
            if (!axState[addedSelectionHandler])
            {
                return false;
            }

            ISelectionService iss = GetSelectionService();
            if (iss != null)
            {
                iss.SelectionChanging -= selectionChangeHandler;
            }
            axState[addedSelectionHandler] = false;
            return true;
        }

        private void SyncRenameNotification(bool hook)
        {
            if (DesignMode && hook != axState[renameEventHooked])
            {
                // if we're in design mode, listen to the following events from the component change service
                //
                IComponentChangeService changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

                if (changeService != null)
                {
                    if (hook)
                    {
                        changeService.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
                    }
                    else
                    {
                        changeService.ComponentRename -= new ComponentRenameEventHandler(OnComponentRename);
                    }
                    axState[renameEventHooked] = hook;
                }
            }
        }

        /// <summary>
        ///  Sets the site of this component. A non-null value indicates that the
        ///  component has been added to a container, and a null value indicates that
        ///  the component is being removed from a container.
        /// </summary>
        public override ISite Site
        {
            set
            {
                // If we are disposed then just return.
                if (axState[disposed])
                {
                    return;
                }
                bool reAddHandler = RemoveSelectionHandler();
                bool olduMode = IsUserMode();

                // clear the old hook
                //
                SyncRenameNotification(false);

                base.Site = value;
                bool newuMode = IsUserMode();
                if (!newuMode)
                {
                    GetOcxCreate();
                }

                if (reAddHandler)
                {
                    AddSelectionHandler();
                }

                SyncRenameNotification(value != null);

                // For inherited forms we create the OCX first in User mode
                // and then we get sited. At that time, we have to re-activate
                // the OCX by transitioning down to and up to the current state.
                //
                if (value != null && !newuMode && olduMode != newuMode && GetOcState() > OC_LOADED)
                {
                    TransitionDownTo(OC_LOADED);
                    TransitionUpTo(OC_INPLACE);
                    ContainerControl f = ContainingControl;
                    if (f != null && f.Visible && Visible)
                    {
                        MakeVisibleWithShow();
                    }
                }

                if (olduMode != newuMode && !IsHandleCreated && !axState[disposed])
                {
                    if (GetOcx() != null)
                    {
                        RealizeStyles();
                    }
                }
                if (!newuMode)
                {
                    //SetupClass_Info(this);
                }
            }
        }

        /// <summary>
        ///  Raises the <see cref='Control.LostFocus'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnLostFocus(EventArgs e)
        {
            // Office WebControl and MS DDS control create a child window that gains
            // focus in order to handle keyboard input. Since, UIDeactivate() could
            // destroy that window, these controls will crash trying to process WM_CHAR.
            // We now check to see if we are losing focus to a child, and if so, not call
            // UIDeactivate().
            bool uiDeactivate = (GetHandleNoCreate() != hwndFocus);
            if (uiDeactivate && IsHandleCreated)
            {
                uiDeactivate = !User32.IsChild(new HandleRef(this, GetHandleNoCreate()), hwndFocus).IsTrue();
            }

            base.OnLostFocus(e);
            if (uiDeactivate)
            {
                UiDeactivate();
            }
        }

        private void OnNewSelection(object sender, EventArgs e)
        {
            if (IsUserMode())
            {
                return;
            }

            ISelectionService iss = GetSelectionService();
            // What we care about:
            // if we are uiactive and we lose selection, then we need to uideactivate ourselves...
            if (iss != null)
            {
                if (GetOcState() >= OC_UIACTIVE && !iss.GetComponentSelected(this))
                {
                    // need to deactivate...
                    HRESULT hr = UiDeactivate();
                    Debug.Assert(hr.Succeeded(), "Failed to UiDeactivate: " + hr.ToString(CultureInfo.InvariantCulture));
                }
                if (!iss.GetComponentSelected(this))
                {
                    if (editMode != EDITM_NONE)
                    {
                        GetParentContainer().OnExitEditMode(this);
                        editMode = EDITM_NONE;
                    }
                    // need to exit edit mode...
                    SetSelectionStyle(1);
                    RemoveSelectionHandler();
                }
                else
                {
                    // The AX Host designer will offer an extender property called "SelectionStyle"
                    //
                    PropertyDescriptor prop = TypeDescriptor.GetProperties(this)["SelectionStyle"];

                    if (prop != null && prop.PropertyType == typeof(int))
                    {
                        int curSelectionStyle = (int)prop.GetValue(this);
                        if (curSelectionStyle != selectionStyle)
                        {
                            prop.SetValue(this, selectionStyle);
                        }
                    }
                }
            }
        }

        //DrawToBitmap doesn't work for this control, so we should hide it.  We'll
        //still call base so that this has a chance to work if it can.
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds)
        {
            base.DrawToBitmap(bitmap, targetBounds);
        }

        /// <summary>
        ///  Creates a handle for this control. This method is called by the framework, this should
        ///  not be called directly.
        /// </summary>
        protected override void CreateHandle()
        {
            if (IsHandleCreated)
            {
                return;
            }

            TransitionUpTo(OC_RUNNING);
            if (!axState[fOwnWindow])
            {
                if (axState[fNeedOwnWindow])
                {
                    Debug.Assert(!Visible, "if we were visible we would not be needing a fake window...");
                    axState[fNeedOwnWindow] = false;
                    axState[fFakingWindow] = true;
                    base.CreateHandle();
                    // note that we do not need to attach the handle because the work usually done in there
                    // will be done in Control's wndProc on WM_CREATE...
                }
                else
                {
                    TransitionUpTo(OC_INPLACE);
                    // it is possible that we were hidden while in place activating, in which case we don't
                    // really have a handle now because the act of hiding could have destroyed it
                    // so, just call ourselves again recursively, and if we dont't have a handle, we will
                    // just take the "axState[fNeedOwnWindow]" path above...
                    if (axState[fNeedOwnWindow])
                    {
                        Debug.Assert(!IsHandleCreated, "if we need a fake window, we can't have a real one");
                        CreateHandle();
                        return;
                    }
                }
            }
            else
            {
                SetState(States.Visible, false);
                base.CreateHandle();
            }

            GetParentContainer().ControlCreated(this);
        }

        private static HRESULT SetupLogPixels(bool force)
        {
            if (logPixelsX == -1 || force)
            {
                using var dc = User32.GetDcScope.ScreenDC;
                if (dc == IntPtr.Zero)
                {
                    return HRESULT.E_FAIL;
                }

                logPixelsX = Gdi32.GetDeviceCaps(dc, Gdi32.DeviceCapability.LOGPIXELSX);
                logPixelsY = Gdi32.GetDeviceCaps(dc, Gdi32.DeviceCapability.LOGPIXELSY);
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, $"log pixels are: {logPixelsX} {logPixelsY}");
            }

            return HRESULT.S_OK;
        }

        private unsafe void HiMetric2Pixel(ref Size sz)
        {
            var phm = new Point(sz.Width, sz.Height);
            var pcont = new PointF();
            ((Ole32.IOleControlSite)oleSite).TransformCoords(&phm, &pcont, Ole32.XFORMCOORDS.SIZE | Ole32.XFORMCOORDS.HIMETRICTOCONTAINER);
            sz.Width = (int)pcont.X;
            sz.Height = (int)pcont.Y;
        }

        private unsafe void Pixel2hiMetric(ref Size sz)
        {
            var phm = new Point();
            var pcont = new PointF(sz.Width, sz.Height);
            ((Ole32.IOleControlSite)oleSite).TransformCoords(&phm, &pcont, Ole32.XFORMCOORDS.SIZE | Ole32.XFORMCOORDS.CONTAINERTOHIMETRIC);
            sz.Width = phm.X;
            sz.Height = phm.Y;
        }

        private static int Pixel2Twip(int v, bool xDirection)
        {
            SetupLogPixels(false);
            int logP = xDirection ? logPixelsX : logPixelsY;
            return (int)((((double)v) / logP) * 72.0 * 20.0);
        }

        private static int Twip2Pixel(double v, bool xDirection)
        {
            SetupLogPixels(false);
            int logP = xDirection ? logPixelsX : logPixelsY;
            return (int)(((v / 20.0) / 72.0) * logP);
        }

        private static int Twip2Pixel(int v, bool xDirection)
        {
            SetupLogPixels(false);
            int logP = xDirection ? logPixelsX : logPixelsY;
            return (int)(((((double)v) / 20.0) / 72.0) * logP);
        }

        private unsafe Size SetExtent(int width, int height)
        {
            Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "setting extent to " + width.ToString(CultureInfo.InvariantCulture) + " " + height.ToString(CultureInfo.InvariantCulture));
            Size sz = new Size(width, height);
            bool resetExtents = !IsUserMode();
            Pixel2hiMetric(ref sz);
            Interop.HRESULT hr = GetOleObject().SetExtent(Ole32.DVASPECT.CONTENT, &sz);
            if (hr != Interop.HRESULT.S_OK)
            {
                resetExtents = true;
            }
            if (resetExtents)
            {
                GetOleObject().GetExtent(Ole32.DVASPECT.CONTENT, &sz);
                GetOleObject().SetExtent(Ole32.DVASPECT.CONTENT, &sz);
            }

            return GetExtent();
        }

        private unsafe Size GetExtent()
        {
            var sz = new Size();
            GetOleObject().GetExtent(Ole32.DVASPECT.CONTENT, &sz);
            HiMetric2Pixel(ref sz);
            return sz;
        }

        /// <summary>
        ///  ActiveX controls scale themselves, so GetScaledBounds simply returns their
        ///  original unscaled bounds.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified)
        {
            return bounds;
        }

        private unsafe void SetObjectRects(Rectangle bounds)
        {
            if (GetOcState() < OC_INPLACE)
            {
                return;
            }

            RECT posRect = bounds;
            RECT clipRect = WebBrowserHelper.GetClipRect();
            GetInPlaceObject().SetObjectRects(&posRect, &clipRect);
        }

        /// <summary>
        ///  Performs the work of setting the bounds of this control.
        ///  User code should usually not call this function.
        /// </summary>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            // We have already been in this Code so please avoid re-entering this CODE PATH or else the
            // IOleObject will "give a Catastrophic error" in SetObjectRects( ).

            if (GetAxState(AxHost.handlePosRectChanged))
            {
                return;
            }

            axState[handlePosRectChanged] = true;

            // Provide control with an opportunity to apply self imposed constraints on its size.
            Size adjustedSize = ApplySizeConstraints(width, height);
            width = adjustedSize.Width;
            height = adjustedSize.Height;

            try
            {
                if (axState[fFakingWindow])
                {
                    base.SetBoundsCore(x, y, width, height, specified);
                    return;
                }
                Rectangle oldBounds = Bounds;

                if (oldBounds.X == x && oldBounds.Y == y && oldBounds.Width == width &&
                    oldBounds.Height == height)
                {
                    return;
                }
                if (!IsHandleCreated)
                {
                    UpdateBounds(x, y, width, height);
                    return;
                }

                if (GetOcState() > OC_RUNNING)
                {
                    CheckSubclassing();
                    if (width != oldBounds.Width || height != oldBounds.Height)
                    {
                        Size p = SetExtent(width, height);
                        width = p.Width;
                        height = p.Height;
                    }
                }
                if (axState[manualUpdate])
                {
                    SetObjectRects(new Rectangle(x, y, width, height));
                    CheckSubclassing();
                    UpdateBounds();
                }
                else
                {
                    SetObjectRects(new Rectangle(x, y, width, height));
                    base.SetBoundsCore(x, y, width, height, specified);
                    Invalidate();
                }
            }
            finally
            {
                axState[handlePosRectChanged] = false;
            }
        }

        private bool CheckSubclassing()
        {
            if (!IsHandleCreated || wndprocAddr == IntPtr.Zero)
            {
                return true;
            }

            IntPtr handle = Handle;
            IntPtr currentWndproc = User32.GetWindowLong(new HandleRef(this, handle), User32.GWL.WNDPROC);
            if (currentWndproc == wndprocAddr)
            {
                return true;
            }

            if (unchecked((int)(long)User32.SendMessageW(this, (User32.WM)REGMSG_MSG)) == (int)REGMSG_RETVAL)
            {
                wndprocAddr = currentWndproc;
                return true;
            }
            // yikes, we were resubclassed...
            Debug.WriteLineIf(AxHostSwitch.TraceVerbose, "The horrible control subclassed itself w/o calling the old wndproc...");
            // we need to resubclass outselves now...
            Debug.Assert(!OwnWindow(), "why are we here if we own our window?");
            WindowReleaseHandle();
            User32.SetWindowLong(new HandleRef(this, handle), User32.GWL.WNDPROC, new HandleRef(this, currentWndproc));
            WindowAssignHandle(handle, axState[assignUniqueID]);
            InformOfNewHandle();
            axState[manualUpdate] = true;
            return false;
        }

        /// <summary>
        ///  Destroys the handle associated with this control.
        ///  User code should in general not call this function.
        /// </summary>
        protected override void DestroyHandle()
        {
            if (axState[fOwnWindow])
            {
                base.DestroyHandle();
            }
            else
            {
                if (IsHandleCreated)
                {
                    TransitionDownTo(OC_RUNNING);
                }
            }
        }

        private void TransitionDownTo(int state)
        {
            if (axState[inTransition])
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Recursively entering TransitionDownTo...");
                return;
            }

            try
            {
                axState[inTransition] = true;

                while (state < GetOcState())
                {
                    switch (GetOcState())
                    {
                        case OC_OPEN:
                            Debug.Fail("how did we ever get into the open state?");
                            SetOcState(OC_UIACTIVE);
                            break;
                        case OC_UIACTIVE:
                            HRESULT hr = UiDeactivate();
                            Debug.Assert(hr.Succeeded(), "Failed in UiDeactivate: " + hr.ToString(CultureInfo.InvariantCulture));
                            Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose && GetOcState() == OC_INPLACE, "failed transition");
                            SetOcState(OC_INPLACE);
                            break;
                        case OC_INPLACE:
                            if (axState[fFakingWindow])
                            {
                                DestroyFakeWindow();
                                SetOcState(OC_RUNNING);
                            }
                            else
                            {
                                InPlaceDeactivate();
                            }
                            Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose && GetOcState() == OC_RUNNING, "failed transition");
                            SetOcState(OC_RUNNING);
                            break;
                        case OC_RUNNING:
                            StopEvents();
                            DisposeAxControl();
                            Debug.Assert(GetOcState() == OC_LOADED, " failed transition");
                            SetOcState(OC_LOADED);
                            break;
                        case OC_LOADED:
                            ReleaseAxControl();
                            Debug.Assert(GetOcState() == OC_PASSIVE, " failed transition");
                            SetOcState(OC_PASSIVE);
                            break;
                        default:
                            Debug.Fail("bad state");
                            SetOcState(GetOcState() - 1);
                            break;
                    }
                }
            }
            finally
            {
                axState[inTransition] = false;
            }
        }

        private void TransitionUpTo(int state)
        {
            if (axState[inTransition])
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Recursively entering TransitionUpTo...");
                return;
            }

            try
            {
                axState[inTransition] = true;

                while (state > GetOcState())
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Transitioning up from: " + GetOcState().ToString(CultureInfo.InvariantCulture) + " to: " + state.ToString(CultureInfo.InvariantCulture));
                    switch (GetOcState())
                    {
                        case OC_PASSIVE:
                            axState[disposed] = false;
                            GetOcxCreate();
                            Debug.Assert(GetOcState() == OC_LOADED, " failed transition");
                            SetOcState(OC_LOADED);
                            break;
                        case OC_LOADED:
                            ActivateAxControl();
                            Debug.Assert(GetOcState() == OC_RUNNING, " failed transition");
                            SetOcState(OC_RUNNING);
                            if (IsUserMode())
                            {
                                // start the events flowing!
                                //createSink();
                                StartEvents();
                            }
                            break;
                        case OC_RUNNING:
                            axState[ownDisposing] = false;
                            Debug.Assert(!axState[fOwnWindow], "If we are invis at runtime, we should never be going beynd OC_RUNNING");
                            if (!axState[fOwnWindow])
                            {
                                InPlaceActivate();

                                if (!Visible && ContainingControl != null && ContainingControl.Visible)
                                {
                                    HideAxControl();
                                }
                                else
                                {
                                    // if we do this in both codepaths, then we will force handle creation of the fake window
                                    // even if we don't need it...
                                    // This optimization will break, however, if:
                                    // a) the hWnd goes away on a Ole32.OLEIVERB.HIDE and
                                    // b) this is a simple frame control
                                    // However, if you satisfy both of these conditions then you must be REALLY
                                    // brain dead and you don't deserve to work anyway...
                                    CreateControl(true);
                                    // if our default size is wrong for the control, let's resize ourselves...
                                    // Note: some controls haven't updated their extents at this time
                                    // (even though they got it from the DoVerb call and
                                    // also from GetWindowContext) so we don't poke in a new value.
                                    // The reason to do this at design time is that that's the only way we
                                    // can find out if the control has a default which we have to obey.
                                    if (!IsUserMode() && !axState[ocxStateSet])
                                    {
                                        Size p = GetExtent();
                                        Rectangle b = Bounds;

                                        if ((b.Size.Equals(DefaultSize)) && (!b.Size.Equals(p)))
                                        {
                                            b.Width = p.Width;
                                            b.Height = p.Height;
                                            Bounds = b;
                                        }
                                    }
                                }
                            }

                            if (GetOcState() < OC_INPLACE)
                            {
                                SetOcState(OC_INPLACE);
                            }
                            OnInPlaceActive();
                            break;
                        case OC_INPLACE:
                            DoVerb((int)Ole32.OLEIVERB.SHOW);
                            Debug.Assert(GetOcState() == OC_UIACTIVE, " failed transition");
                            SetOcState(OC_UIACTIVE);
                            break;
                        default:
                            Debug.Fail("bad state");
                            SetOcState(GetOcState() + 1);
                            break;
                    }
                }
            }
            finally
            {
                axState[inTransition] = false;
            }
        }

        protected virtual void OnInPlaceActive()
        {
        }

        private void InPlaceActivate()
        {
            try
            {
                DoVerb((int)Ole32.OLEIVERB.INPLACEACTIVATE);
            }
            catch (Exception t)
            {
                Debug.Fail(t.ToString());
                throw new TargetInvocationException(string.Format(SR.AXNohWnd, GetType().Name), t);
            }
            EnsureWindowPresent();
        }

        private HRESULT InPlaceDeactivate()
        {
            axState[ownDisposing] = true;
            ContainerControl f = ContainingControl;
            if (f != null)
            {
                if (f.ActiveControl == this)
                {
                    f.ActiveControl = null;
                }
            }

            return GetInPlaceObject().InPlaceDeactivate();
        }

        private void UiActivate()
        {
            Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "calling uiActivate for " + ToString());
            Debug.Assert(CanUIActivate, "we have to be able to uiactivate");
            if (CanUIActivate)
            {
                DoVerb((int)Ole32.OLEIVERB.UIACTIVATE);
            }
        }

        private void DestroyFakeWindow()
        {
            Debug.Assert(axState[fFakingWindow], "have to be faking it in order to destroy it...");

            // The problem seems to be that when we try to destroy the fake window,
            // we recurse in and transition the control down to OC_RUNNING. This causes the control's
            // new window to get destroyed also, and the control never shows up.
            // We now prevent this by changing our state about the fakeWindow _before_ we actually
            // destroy the window.
            //
            axState[fFakingWindow] = false;
            base.DestroyHandle();
        }

        private void EnsureWindowPresent()
        {
            // if the ctl didn't call showobject, we need to do it for it...
            if (!IsHandleCreated)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Naughty control didn't call showObject...");
                try
                {
                    ((Ole32.IOleClientSite)oleSite).ShowObject();
                }
                catch
                {
                    // The exception, if any was already dumped in ShowObject
                }
            }
            if (IsHandleCreated)
            {
                return;
            }

            if (ParentInternal != null)
            {    // ==> we are in a valid state
                Debug.Fail("extremely naughty ctl is refusing to give us an hWnd... giving up...");
                throw new NotSupportedException(string.Format(SR.AXNohWnd, GetType().Name));
            }
        }

        protected override void SetVisibleCore(bool value)
        {
            if (GetState(States.Visible) != value)
            {
                bool oldVisible = Visible;
                if ((IsHandleCreated || value) && ParentInternal != null && ParentInternal.Created)
                {
                    if (!axState[fOwnWindow])
                    {
                        TransitionUpTo(OC_RUNNING);
                        if (value)
                        {
                            if (axState[fFakingWindow])
                            {
                                // first we need to destroy the fake window...
                                DestroyFakeWindow();
                            }
                            // We want to avoid using SHOW since that may uiactivate us, and we don't
                            // want that...
                            if (!IsHandleCreated)
                            {
                                // So, if we don't have a handle, we just try to create it and hope that this will make
                                // us appear...
                                try
                                {
                                    SetExtent(Width, Height);
                                    InPlaceActivate();
                                    CreateControl(true);
                                }
                                catch
                                {
                                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Could not make ctl visible by using INPLACE. Will try SHOW");
                                    MakeVisibleWithShow();
                                }
                            }
                            else
                            {
                                // if, otoh, we had a handle to begin with, we need to use show since INPLACE is just
                                // a noop...
                                MakeVisibleWithShow();
                            }
                        }
                        else
                        {
                            Debug.Assert(!axState[fFakingWindow], "if we were visible, we could not have had a fake window...");
                            HideAxControl();
                        }
                    }
                }
                if (!value)
                {
                    axState[fNeedOwnWindow] = false;
                }
                if (!axState[fOwnWindow])
                {
                    SetState(States.Visible, value);
                    if (Visible != oldVisible)
                    {
                        OnVisibleChanged(EventArgs.Empty);
                    }
                }
            }
        }

        private void MakeVisibleWithShow()
        {
            ContainerControl f = ContainingControl;
            Control ctl = f?.ActiveControl;
            try
            {
                DoVerb((int)Ole32.OLEIVERB.SHOW);
            }
            catch (Exception t)
            {
                Debug.Fail(t.ToString());
                throw new TargetInvocationException(string.Format(SR.AXNohWnd, GetType().Name), t);
            }

            EnsureWindowPresent();
            CreateControl(true);
            if (f != null && f.ActiveControl != ctl)
            {
                f.ActiveControl = ctl;
            }
        }

        private void HideAxControl()
        {
            Debug.Assert(!axState[fOwnWindow], "can't own our window when hiding");
            Debug.Assert(IsHandleCreated, "gotta have a window to hide");
            Debug.Assert(GetOcState() >= OC_INPLACE, "have to be in place in order to hide.");

            DoVerb((int)Ole32.OLEIVERB.HIDE);
            if (GetOcState() < OC_INPLACE)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Naughty control inplace deactivated on a hide verb...");
                Debug.Assert(!IsHandleCreated, "if we are inplace deactivated we should not have a window.");
                // all we do here is set a flag saying that we need the window to be created if
                // create handle is ever called...
                axState[fNeedOwnWindow] = true;

                // also, set the state to our "pretend oc_inplace state"
                //
                SetOcState(OC_INPLACE);
            }
        }

        /// <summary>
        ///  Determines if charCode is an input character that the control
        ///  wants. This method is called during window message pre-processing to
        ///  determine whether the given input character should be pre-processed or
        ///  sent directly to the control. If isInputChar returns true, the
        ///  given character is sent directly to the control. If isInputChar
        ///  returns false, the character is pre-processed and only sent to the
        ///  control if it is not consumed by the pre-processing phase. The
        ///  pre-processing of a character includes checking whether the character
        ///  is a mnemonic of another control.
        /// </summary>
        protected override bool IsInputChar(char charCode)
        {
            return true;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (ignoreDialogKeys)
            {
                return false;
            }

            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        ///  This method is called by the application's message loop to pre-process
        ///  input messages before they are dispatched. Possible values for the
        ///  msg.message field are WM_KEYDOWN, WM_SYSKEYDOWN, WM_CHAR, and WM_SYSCHAR.
        ///  If this method processes the message it must return true, in which case
        ///  the message loop will not dispatch the message.
        ///  This method should not be called directly by the user.
        ///
        ///  The keyboard processing of input keys to AxHost controls go in 3 steps inside AxHost.PreProcessMessage()
        ///
        ///  (1) Call the OCX's TranslateAccelarator. This may or may not call back into us using IOleControlSite::TranslateAccelarator()
        ///
        ///  (2) If the control completely processed this without calling us back:
        ///  -- If this returns S_OK, then it means that the control already processed this message and we return true,
        ///  forcing us to not do any more processing or dispatch the message.
        ///  -- If this returns S_FALSE, then it means that the control wants us to dispatch the message without doing any processing on our side.
        ///
        ///  (3) If the control completely processed this by calling us back:
        ///  -- If this returns S_OK, then it means that the control processed this message and we return true,
        ///  forcing us to not do any more processing or dispatch the message.
        ///  -- If this returns S_FALSE, then it means that the control did not process this message,
        ///  but we did, and so we should route it through our PreProcessMessage().
        /// </summary>
        public unsafe override bool PreProcessMessage(ref Message msg)
        {
            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "AxHost.PreProcessMessage " + msg.ToString());

            if (IsUserMode())
            {
                if (axState[siteProcessedInputKey])
                {
                    // In this case, the control called the us back through the IControlSite
                    // and giving us a chance to see if we want to process it. We in turn
                    // call the base implementation which normally would call the control's
                    // IsInputKey() or IsInputChar(). So, we short-circuit those to return false
                    // and only return true, if the container-chain wanted to process the keystroke
                    // (e.g. tab, accelarators etc.)
                    //
                    return base.PreProcessMessage(ref msg);
                }

                User32.MSG win32Message = msg;
                axState[siteProcessedInputKey] = false;
                try
                {
                    Ole32.IOleInPlaceActiveObject activeObj = GetInPlaceActiveObject();
                    if (activeObj != null)
                    {
                        HRESULT hr = activeObj.TranslateAccelerator(&win32Message);
                        msg.Msg = (int)win32Message.message;
                        msg.WParam = win32Message.wParam;
                        msg.LParam = win32Message.lParam;
                        msg.HWnd = win32Message.hwnd;

                        if (hr == HRESULT.S_OK)
                        {
                            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "\t Message translated by control to " + msg);
                            return true;
                        }
                        else if (hr == HRESULT.S_FALSE)
                        {
                            bool ret = false;

                            ignoreDialogKeys = true;
                            try
                            {
                                ret = base.PreProcessMessage(ref msg);
                            }
                            finally
                            {
                                ignoreDialogKeys = false;
                            }
                            return ret;
                        }
                        else if (axState[siteProcessedInputKey])
                        {
                            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "\t Message processed by site. Calling base.PreProcessMessage() " + msg);
                            return base.PreProcessMessage(ref msg);
                        }
                        else
                        {
                            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "\t Message not processed by site. Returning false. " + msg);
                            return false;
                        }
                    }
                }
                finally
                {
                    axState[siteProcessedInputKey] = false;
                }
            }

            return false;
        }

        /// <summary>
        ///  Process a mnemonic character.
        ///  This is done by manufacturing a WM_SYSKEYDOWN message and passing it to the
        ///  ActiveX control.
        /// </summary>
        protected internal unsafe override bool ProcessMnemonic(char charCode)
        {
            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "In AxHost.ProcessMnemonic: " + (int)charCode);
            if (CanSelect)
            {
                try
                {
                    var ctlInfo = new Ole32.CONTROLINFO
                    {
                        cb = (uint)Marshal.SizeOf<Ole32.CONTROLINFO>()
                    };
                    HRESULT hr = GetOleControl().GetControlInfo(&ctlInfo);
                    if (!hr.Succeeded())
                    {
                        return false;
                    }

                    var msg = new User32.MSG
                    {
                        // Sadly, we don't have a message so we must fake one ourselves...
                        // A bit of ugliness here (a bit?  more like a bucket...)
                        // The message we are faking is a WM_SYSKEYDOWN w/ the right alt key setting...
                        hwnd = (ContainingControl is null) ? IntPtr.Zero : ContainingControl.Handle,
                        message = User32.WM.SYSKEYDOWN,
                        wParam = (IntPtr)char.ToUpper(charCode, CultureInfo.CurrentCulture),
                        lParam = (IntPtr)0x20180001,
                        time = Kernel32.GetTickCount()
                    };
                    User32.GetCursorPos(out Point p);
                    msg.pt = p;
                    if (Ole32.IsAccelerator(new HandleRef(ctlInfo, ctlInfo.hAccel), ctlInfo.cAccel, ref msg, null).IsTrue())
                    {
                        GetOleControl().OnMnemonic(&msg);
                        Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "\t Processed mnemonic " + msg);
                        Focus();
                        return true;
                    }
                }
                catch (Exception t)
                {
                    Debug.Fail("error in processMnemonic");
                    Debug.Fail(t.ToString());
                    return false;
                }
            }
            return false;
        }

        // misc methods:

        /// <summary>
        ///  Sets the delegate which will be called when the user selects the "About..."
        ///  entry on the context menu.
        /// </summary>
        protected void SetAboutBoxDelegate(AboutBoxDelegate d)
        {
            aboutBoxDelegate += d;
        }

        /// <summary>
        ///  Sets the persisted state of the control.
        ///  This should either be null, obtained from getOcxState, or
        ///  read from a resource.  The value of this property will
        ///  be used after the control is created but before it is
        ///  shown.
        ///  Computes the persisted state of the underlying ActiveX control and
        ///  returns it in the encapsulated State object.
        ///  If the control has been modified since it was last saved to a
        ///  persisted state, it will be asked to save itself.
        /// </summary>
        [DefaultValue(null)]
        [RefreshProperties(RefreshProperties.All)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public State OcxState
        {
            get
            {
                if (IsDirty() || ocxState is null)
                {
                    Debug.Assert(!axState[disposed], "we chould not be asking for the object when we are axState[disposed]...");
                    ocxState = CreateNewOcxState(ocxState);
                }
                return ocxState;
            }

            set
            {
                axState[ocxStateSet] = true;

                if (value is null)
                {
                    return;
                }

                if (storageType != STG_UNKNOWN && storageType != value.type)
                {
                    Debug.Fail("Trying to reload with a OcxState that is of a different type.");
                    throw new InvalidOperationException(SR.AXOcxStateLoaded);
                }

                if (ocxState == value)
                {
                    return;
                }

                ocxState = value;

                if (ocxState != null)
                {
                    axState[manualUpdate] = ocxState._GetManualUpdate();
                    licenseKey = ocxState._GetLicenseKey();
                }
                else
                {
                    axState[manualUpdate] = false;
                    licenseKey = null;
                }

                if (ocxState != null && GetOcState() >= OC_RUNNING)
                {
                    DepersistControl();
                }
            }
        }

        private State CreateNewOcxState(State oldOcxState)
        {
            NoComponentChangeEvents++;

            try
            {
                if (GetOcState() < OC_RUNNING)
                {
                    return null;
                }

                try
                {
                    PropertyBagStream propBag = null;

                    if (iPersistPropBag != null)
                    {
                        propBag = new PropertyBagStream();
                        iPersistPropBag.Save(propBag, BOOL.TRUE, BOOL.TRUE);
                    }

                    MemoryStream ms = null;
                    switch (storageType)
                    {
                        case STG_STREAM:
                        case STG_STREAMINIT:
                            ms = new MemoryStream();
                            if (storageType == STG_STREAM)
                            {
                                iPersistStream.Save(new Ole32.GPStream(ms), BOOL.TRUE);
                            }
                            else
                            {
                                iPersistStreamInit.Save(new Ole32.GPStream(ms), BOOL.TRUE);
                            }
                            break;
                        case STG_STORAGE:
                            Debug.Assert(oldOcxState != null, "we got to have an old state which holds out scribble storage...");
                            if (oldOcxState != null)
                            {
                                return oldOcxState.RefreshStorage(iPersistStorage);
                            }

                            return null;
                        default:
                            Debug.Fail("unknown storage type.");
                            return null;
                    }
                    if (ms != null)
                    {
                        return new State(ms, storageType, this, propBag);
                    }
                    else if (propBag != null)
                    {
                        return new State(propBag);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Could not create new OCX State: " + e.ToString());
                }
            }
            finally
            {
                NoComponentChangeEvents--;
            }

            return null;
        }

        /// <summary>
        ///  Returns this control's logicaly containing form.
        ///  At design time this is always the form being designed.
        ///  At runtime it is either the form set with setContainingForm or,
        ///  by default, the parent form.
        ///  Sets the form which is the logical container of this control.
        ///  By default, the parent form performs that function.  It is
        ///  however possible for another form higher in the parent chain
        ///  to serve in that role.  The logical container of this
        ///  control determines the set of logical sibling control.
        ///  In general this property exists only to enable some speficic
        ///  behaviours of ActiveX controls and should in general not be set
        ///  by the user.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ContainerControl ContainingControl
        {
            get
            {
                if (containingControl is null)
                {
                    containingControl = FindContainerControlInternal();
                }

                return containingControl;
            }

            set
            {
                containingControl = value;
            }
        }

        /// <summary>
        ///  Determines if the Text property needs to be persisted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal override bool ShouldSerializeText()
        {
            bool ret = false;
            try
            {
                ret = (Text.Length != 0);
            }
            catch (COMException)
            {
            }
            return ret;
        }

        /// <summary>
        ///  Determines whether to persist the ContainingControl property.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeContainingControl()
        {
            return ContainingControl != ParentInternal;
        }

        private ContainerControl FindContainerControlInternal()
        {
            if (Site != null)
            {
                IDesignerHost host = (IDesignerHost)Site.GetService(typeof(IDesignerHost));
                if (host != null)
                {
                    if (host.RootComponent is ContainerControl rootControl)
                    {
                        return rootControl;
                    }
                }
            }

            ContainerControl cc = null;
            Control control = this;
            while (control != null)
            {
                if (control is ContainerControl tempCC)
                {
                    cc = tempCC;
                    break;
                }
                control = control.ParentInternal;
            }
            return cc;
        }

        private bool IsDirty()
        {
            if (GetOcState() < OC_RUNNING)
            {
                return false;
            }

            Debug.Assert(storageType != STG_UNKNOWN, "if we are loaded, out storage type must be set!");

            if (axState[valueChanged])
            {
                axState[valueChanged] = false;
                return true;
            }

#if DEBUG
            if (AxAlwaysSaveSwitch.Enabled)
            {
                return true;
            }
#endif
            HRESULT hr = HRESULT.E_FAIL;
            switch (storageType)
            {
                case STG_STREAM:
                    hr = iPersistStream.IsDirty();
                    break;
                case STG_STREAMINIT:
                    hr = iPersistStreamInit.IsDirty();
                    break;
                case STG_STORAGE:
                    hr = iPersistStorage.IsDirty();
                    break;
                default:
                    Debug.Fail("unknown storage type");
                    return true;
            }

            // NOTE: This was a note from the old AxHost codebase. The problem
            // with doing this is that the some controls that do not run in
            // unlicensed mode (e.g. ProtoView ScheduleX pvtaskpad.ocx) will
            // always return S_FALSE to disallow design-time support.

            // Sadly, some controls lie and never say that they are dirty...
            // SO, we don't believe them unless they told us that they were
            // dirty at least once...
            return hr != HRESULT.S_FALSE;
        }

        internal bool IsUserMode()
        {
            ISite site = Site;
            return site is null || !site.DesignMode;
        }

        private object GetAmbientProperty(Ole32.DispatchID dispid)
        {
            Control richParent = ParentInternal;

            switch (dispid)
            {
                case Ole32.DispatchID.AMBIENT_USERMODE:
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "asked for usermode");
                    return IsUserMode();
                case Ole32.DispatchID.AMBIENT_AUTOCLIP:
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "asked for autoclip");
                    return true;
                case Ole32.DispatchID.AMBIENT_MESSAGEREFLECT:
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "asked for message reflect");
                    return true;
                case Ole32.DispatchID.AMBIENT_UIDEAD:
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "asked for uidead");
                    return false;
                case Ole32.DispatchID.AMBIENT_DISPLAYASDEFAULT:
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "asked for displayasdefault");
                    return false;
                case Ole32.DispatchID.AMBIENT_FONT:
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "asked for font");
                    if (richParent != null)
                    {
                        return GetIFontFromFont(richParent.Font);
                    }
                    return null;
                case Ole32.DispatchID.AMBIENT_SHOWGRABHANDLES:
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "asked for showGrabHandles");
                    return false;
                case Ole32.DispatchID.AMBIENT_SHOWHATCHING:
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "asked for showHatching");
                    return false;
                case Ole32.DispatchID.AMBIENT_BACKCOLOR:
                    if (richParent != null)
                    {
                        return GetOleColorFromColor(richParent.BackColor);
                    }
                    return null;
                case Ole32.DispatchID.AMBIENT_FORECOLOR:
                    if (richParent != null)
                    {
                        return GetOleColorFromColor(richParent.ForeColor);
                    }
                    return null;
                case Ole32.DispatchID.AMBIENT_DISPLAYNAME:
                    string rval = GetParentContainer().GetNameForControl(this);
                    if (rval is null)
                    {
                        rval = string.Empty;
                    }

                    return rval;
                case Ole32.DispatchID.AMBIENT_LOCALEID:
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "asked for localeid");
                    return Kernel32.GetThreadLocale().RawValue;
                case Ole32.DispatchID.AMBIENT_RIGHTTOLEFT:
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "asked for right to left");
                    Control ctl = this;
                    while (ctl != null)
                    {
                        if (ctl.RightToLeft == System.Windows.Forms.RightToLeft.No)
                        {
                            return false;
                        }

                        if (ctl.RightToLeft == System.Windows.Forms.RightToLeft.Yes)
                        {
                            return true;
                        }

                        if (ctl.RightToLeft == System.Windows.Forms.RightToLeft.Inherit)
                        {
                            ctl = ctl.Parent;
                        }
                    }
                    return null;
                default:
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "unsupported ambient " + dispid.ToString(CultureInfo.InvariantCulture));
                    return null;
            }
        }

        public unsafe void DoVerb(int verb)
        {
            Control parent = ParentInternal;
            RECT posRect = Bounds;
            GetOleObject().DoVerb((Ole32.OLEIVERB)verb, null, oleSite, -1, parent != null ? parent.Handle : IntPtr.Zero, &posRect);
        }

        private bool AwaitingDefreezing()
        {
            return freezeCount > 0;
        }

        private void Freeze(bool v)
        {
            Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "freezing " + v.ToString());
            if (v)
            {
                GetOleControl().FreezeEvents(BOOL.TRUE);
                freezeCount++;
            }
            else
            {
                GetOleControl().FreezeEvents(BOOL.FALSE);
                freezeCount--;
            }
            Debug.Assert(freezeCount >= 0, "invalid freeze count!");
        }

        private HRESULT UiDeactivate()
        {
            Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "calling uiDeactivate for " + ToString());
            bool ownDispose = axState[ownDisposing];
            axState[ownDisposing] = true;
            try
            {
                return GetInPlaceObject().UIDeactivate();
            }
            finally
            {
                axState[ownDisposing] = ownDispose;
            }
        }

        private int GetOcState()
        {
            return ocState;
        }

        private void SetOcState(int nv)
        {
            ocState = nv;
        }

        private string GetLicenseKey()
        {
            return GetLicenseKey(clsid);
        }

        private unsafe string GetLicenseKey(Guid clsid)
        {
            if (licenseKey != null || !axState[needLicenseKey])
            {
                return licenseKey;
            }

            HRESULT hr = Ole32.CoGetClassObject(
                ref clsid,
                Ole32.CLSCTX.INPROC_SERVER,
                IntPtr.Zero,
                ref icf2_Guid,
                out Ole32.IClassFactory2 icf2);
            if (!hr.Succeeded())
            {
                if (hr == HRESULT.E_NOINTERFACE)
                {
                    return null;
                }

                axState[needLicenseKey] = false;
                return null;
            }

            var licInfo = new Ole32.LICINFO
            {
                cbLicInfo = sizeof(Ole32.LICINFO)
            };
            icf2.GetLicInfo(&licInfo);
            if (licInfo.fRuntimeAvailable.IsTrue())
            {
                var rval = new string[1];
                icf2.RequestLicKey(0, rval);
                licenseKey = rval[0];
                return licenseKey;
            }

            return null;
        }

        private void CreateWithoutLicense(Guid clsid)
        {
            Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Creating object without license: " + clsid.ToString());
            HRESULT hr = Ole32.CoCreateInstance(
                ref clsid,
                IntPtr.Zero,
                Ole32.CLSCTX.INPROC_SERVER,
                ref NativeMethods.ActiveX.IID_IUnknown,
                out object ret);
            if (!hr.Succeeded())
            {
                throw Marshal.GetExceptionForHR((int)hr);
            }

            instance = ret;
            Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "\t" + (instance != null).ToString());
        }

        private void CreateWithLicense(string license, Guid clsid)
        {
            if (license != null)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Creating object with license: " + clsid.ToString());
                HRESULT hr = Ole32.CoGetClassObject(
                    ref clsid,
                    Ole32.CLSCTX.INPROC_SERVER,
                    IntPtr.Zero,
                    ref icf2_Guid,
                    out Ole32.IClassFactory2 icf2);
                if (hr.Succeeded())
                {
                    icf2.CreateInstanceLic(IntPtr.Zero, IntPtr.Zero, ref NativeMethods.ActiveX.IID_IUnknown, license, out instance);
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "\t" + (instance != null).ToString());
                }
            }

            if (instance is null)
            {
                CreateWithoutLicense(clsid);
            }
        }

        private void CreateInstance()
        {
            Debug.Assert(instance is null, "instance must be null");
            try
            {
                instance = CreateInstanceCore(clsid);
                Debug.Assert(instance != null, "w/o an exception being thrown we must have an object...");
            }
            catch (ExternalException e)
            {
                if (e.ErrorCode == unchecked((int)0x80040112))
                { // CLASS_E_NOTLICENSED
                    throw new LicenseException(GetType(), this, SR.AXNoLicenseToUse);
                }
                throw;
            }

            Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "created");
            SetOcState(OC_LOADED);
        }

        /// <summary>
        ///  Called to create the ActiveX control.  Override this member to perform your own creation logic
        ///  or call base to do the default creation logic.
        /// </summary>
        protected virtual object CreateInstanceCore(Guid clsid)
        {
            if (IsUserMode())
            {
                CreateWithLicense(licenseKey, clsid);
            }
            else
            {
                CreateWithoutLicense(clsid);
            }
            return instance;
        }

        private unsafe CategoryAttribute GetCategoryForDispid(Ole32.DispatchID dispid)
        {
            VSSDK.ICategorizeProperties icp = GetCategorizeProperties();
            if (icp is null)
            {
                return null;
            }

            VSSDK.PROPCAT propcat = 0;
            HRESULT hr = icp.MapPropertyToCategory(dispid, &propcat);
            if (hr != HRESULT.S_OK || propcat == 0)
            {
                return null;
            }

            int index = -(int)propcat;
            if (index > 0 && index < categoryNames.Length && categoryNames[index] != null)
            {
                return categoryNames[index];
            }

            if (objectDefinedCategoryNames != null)
            {
                CategoryAttribute rval = (CategoryAttribute)objectDefinedCategoryNames[propcat];
                if (rval != null)
                {
                    return rval;
                }
            }

            hr = icp.GetCategoryName(propcat, Kernel32.GetThreadLocale(), out string name);
            if (hr == HRESULT.S_OK && name != null)
            {
                var rval = new CategoryAttribute(name);
                objectDefinedCategoryNames ??= new Hashtable();
                objectDefinedCategoryNames.Add(propcat, rval);
                return rval;
            }

            return null;
        }

        private void SetSelectionStyle(int selectionStyle)
        {
            if (!IsUserMode())
            {
                // selectionStyle can be 0 (not selected), 1 (selected) or 2 (active)
                Debug.Assert(selectionStyle >= 0 && selectionStyle <= 2, "Invalid selection style");

                ISelectionService iss = GetSelectionService();
                this.selectionStyle = selectionStyle;
                if (iss != null && iss.GetComponentSelected(this))
                {
                    // The AX Host designer will offer an extender property called "SelectionStyle"
                    //
                    PropertyDescriptor prop = TypeDescriptor.GetProperties(this)["SelectionStyle"];
                    if (prop != null && prop.PropertyType == typeof(int))
                    {
                        prop.SetValue(this, selectionStyle);
                    }
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void InvokeEditMode()
        {
            Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "invoking EditMode for " + ToString());
            Debug.Assert((flags & AxFlags.PreventEditMode) == 0, "edit mode should have been disabled");
            if (editMode != EDITM_NONE)
            {
                return;
            }

            AddSelectionHandler();
            editMode = EDITM_HOST;
            SetSelectionStyle(2);
            IntPtr hwndFocus = User32.GetFocus();
            try
            {
                UiActivate();
            }
            catch (Exception)
            {
            }
        }

        //
        // ICustomTypeDescriptor implementation.
        //

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            if (!axState[editorRefresh] && HasPropertyPages())
            {
                axState[editorRefresh] = true;
                TypeDescriptor.Refresh(GetType());
            }
            return TypeDescriptor.GetAttributes(this, true);
        }

        /// <summary>
        ///  Retrieves the class name for this object.  If null is returned,
        ///  the type name is used.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        string ICustomTypeDescriptor.GetClassName()
        {
            return null;
        }

        /// <summary>
        ///  Retrieves the name for this object.  If null is returned,
        ///  the default is used.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        string ICustomTypeDescriptor.GetComponentName()
        {
            return null;
        }

        /// <summary>
        ///  Retrieves the type converter for this object.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return null;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        /// <summary>
        ///  Retrieves the an editor for this object.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            if (editorBaseType != typeof(ComponentEditor))
            {
                return null;
            }

            if (editor != null)
            {
                return editor;
            }

            if (editor is null && HasPropertyPages())
            {
                editor = new AxComponentEditor();
            }

            return editor;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        private void OnIdle(object sender, EventArgs e)
        {
            if (axState[refreshProperties])
            {
                TypeDescriptor.Refresh(GetType());
            }
        }

        private bool RefreshAllProperties
        {
            get
            {
                return axState[refreshProperties];
            }
            set
            {
                axState[refreshProperties] = value;
                if (value && !axState[listeningToIdle])
                {
                    Application.Idle += new EventHandler(OnIdle);
                    axState[listeningToIdle] = true;
                }
                else if (!value && axState[listeningToIdle])
                {
                    Application.Idle -= new EventHandler(OnIdle);
                    axState[listeningToIdle] = false;
                }
            }
        }

        private PropertyDescriptorCollection FillProperties(Attribute[] attributes)
        {
            if (RefreshAllProperties)
            {
                RefreshAllProperties = false;
                propsStash = null;
                attribsStash = null;
            }
            else if (propsStash != null)
            {
                if (attributes is null && attribsStash is null)
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Returning stashed values for : " + "<null>");
                    return propsStash;
                }
                else if (attributes != null && attribsStash != null && attributes.Length == attribsStash.Length)
                {
                    bool attribsEqual = true;
                    int i = 0;
                    foreach (Attribute attrib in attributes)
                    {
                        if (!attrib.Equals(attribsStash[i++]))
                        {
                            attribsEqual = false;
                            break;
                        }
                    }

                    if (attribsEqual)
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Returning stashed values for : " + attributes.Length);
                        return propsStash;
                    }
                }
            }

            ArrayList retProps = new ArrayList();

            if (properties is null)
            {
                properties = new Hashtable();
            }

            if (propertyInfos is null)
            {
                propertyInfos = new Hashtable();

                PropertyInfo[] propInfos = GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

                foreach (PropertyInfo propInfo in propInfos)
                {
                    propertyInfos.Add(propInfo.Name, propInfo);
                }
            }

            PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(this, null, true);
            if (baseProps != null)
            {
                for (int i = 0; i < baseProps.Count; ++i)
                {
                    Debug.Assert(baseProps[i] != null, "Null base prop at location: " + i.ToString(CultureInfo.InvariantCulture));

                    if (baseProps[i].DesignTimeOnly)
                    {
                        retProps.Add(baseProps[i]);
                        continue;
                    }

                    string propName = baseProps[i].Name;
                    PropertyDescriptor prop = null;
                    PropertyInfo propInfo = (PropertyInfo)propertyInfos[propName];

                    // We do not support "write-only" properties that some activex controls support.
                    if (propInfo != null && !propInfo.CanRead)
                    {
                        continue;
                    }

                    if (!properties.ContainsKey(propName))
                    {
                        if (propInfo != null)
                        {
                            Debug.WriteLineIf(AxPropTraceSwitch.TraceVerbose, "Added AxPropertyDescriptor for: " + propName);
                            prop = new AxPropertyDescriptor(baseProps[i], this);
                            ((AxPropertyDescriptor)prop).UpdateAttributes();
                        }
                        else
                        {
                            Debug.WriteLineIf(AxPropTraceSwitch.TraceVerbose, "Added PropertyDescriptor for: " + propName);
                            prop = baseProps[i];
                        }
                        properties.Add(propName, prop);
                        retProps.Add(prop);
                    }
                    else
                    {
                        PropertyDescriptor propDesc = (PropertyDescriptor)properties[propName];
                        Debug.Assert(propDesc != null, "Cannot find cached entry for: " + propName);
                        AxPropertyDescriptor axPropDesc = propDesc as AxPropertyDescriptor;
                        if ((propInfo is null && axPropDesc != null) || (propInfo != null && axPropDesc is null))
                        {
                            Debug.Fail("Duplicate property with same name: " + propName);
                            Debug.WriteLineIf(AxPropTraceSwitch.TraceVerbose, "Duplicate property with same name: " + propName);
                        }
                        else
                        {
                            if (axPropDesc != null)
                            {
                                axPropDesc.UpdateAttributes();
                            }
                            retProps.Add(propDesc);
                        }
                    }
                }

                // Filter only the Browsable attribute, since that is the only
                // one we mess with.
                //
                if (attributes != null)
                {
                    Attribute browse = null;
                    foreach (Attribute attr in attributes)
                    {
                        if (attr is BrowsableAttribute)
                        {
                            browse = attr;
                        }
                    }

                    if (browse != null)
                    {
                        ArrayList removeList = null;

                        foreach (PropertyDescriptor prop in retProps)
                        {
                            if (prop is AxPropertyDescriptor)
                            {
                                Attribute attr = prop.Attributes[typeof(BrowsableAttribute)];
                                if (attr != null && !attr.Equals(browse))
                                {
                                    if (removeList is null)
                                    {
                                        removeList = new ArrayList();
                                    }
                                    removeList.Add(prop);
                                }
                            }
                        }

                        if (removeList != null)
                        {
                            foreach (object prop in removeList)
                            {
                                retProps.Remove(prop);
                            }
                        }
                    }
                }
            }

            PropertyDescriptor[] temp = new PropertyDescriptor[retProps.Count];
            retProps.CopyTo(temp, 0);

            // Update our stashed values.
            //
            Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Updating stashed values for : " + ((attributes != null) ? attributes.Length.ToString(CultureInfo.InvariantCulture) : "<null>"));
            propsStash = new PropertyDescriptorCollection(temp);
            attribsStash = attributes;

            return propsStash;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return FillProperties(null);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            return FillProperties(attributes);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        private AxPropertyDescriptor GetPropertyDescriptorFromDispid(Ole32.DispatchID dispid)
        {
            Debug.Assert(dispid != Ole32.DispatchID.UNKNOWN, "Wrong dispid sent to GetPropertyDescriptorFromDispid");

            PropertyDescriptorCollection props = FillProperties(null);
            foreach (PropertyDescriptor prop in props)
            {
                if (prop is AxPropertyDescriptor axprop && axprop.Dispid == dispid)
                {
                    return axprop;
                }
            }

            return null;
        }

        private void ActivateAxControl()
        {
            if (QuickActivate())
            {
                DepersistControl();
            }
            else
            {
                SlowActivate();
            }

            SetOcState(OC_RUNNING);
        }

        private void DepersistFromIPropertyBag(Oleaut32.IPropertyBag propBag)
        {
            iPersistPropBag.Load(propBag, null);
        }

        private void DepersistFromIStream(Ole32.IStream istream)
        {
            storageType = STG_STREAM;
            iPersistStream.Load(istream);
        }

        private void DepersistFromIStreamInit(Ole32.IStream istream)
        {
            storageType = STG_STREAMINIT;
            iPersistStreamInit.Load(istream);
        }

        private void DepersistFromIStorage(Ole32.IStorage storage)
        {
            storageType = STG_STORAGE;

            // Looks like MapPoint control does not create a valid IStorage
            // until some property has changed. Since we end up creating a bogus (empty)
            // storage, we end up not being able to re-create a valid one and this would
            // fail.
            //
            if (storage != null)
            {
                HRESULT hr = iPersistStorage.Load(storage);
                if (hr != HRESULT.S_OK)
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Error trying load depersist from IStorage: " + hr);
                }
            }
        }

        private void DepersistControl()
        {
            Freeze(true);

            if (ocxState is null)
            {
                // must init new:
                //
                if (instance is Ole32.IPersistStreamInit)
                {
                    iPersistStreamInit = (Ole32.IPersistStreamInit)instance;
                    try
                    {
                        storageType = STG_STREAMINIT;
                        iPersistStreamInit.InitNew();
                    }
                    catch (Exception e1)
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Exception thrown trying to IPersistStreamInit.InitNew(). Is this good?" + e1.ToString());
                    }
                    return;
                }
                if (instance is Ole32.IPersistStream)
                {
                    storageType = STG_STREAM;
                    iPersistStream = (Ole32.IPersistStream)instance;
                    return;
                }
                if (instance is Ole32.IPersistStorage)
                {
                    storageType = STG_STORAGE;
                    ocxState = new State(this);
                    iPersistStorage = (Ole32.IPersistStorage)instance;
                    try
                    {
                        iPersistStorage.InitNew(ocxState.GetStorage());
                    }
                    catch (Exception e2)
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Exception thrown trying to IPersistStorage.InitNew(). Is this good?" + e2.ToString());
                    }
                    return;
                }
                if (instance is Oleaut32.IPersistPropertyBag)
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, this + " supports IPersistPropertyBag.");
                    iPersistPropBag = (Oleaut32.IPersistPropertyBag)instance;
                    try
                    {
                        iPersistPropBag.InitNew();
                    }
                    catch (Exception e1)
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Exception thrown trying to IPersistPropertyBag.InitNew(). Is this good?" + e1.ToString());
                    }
                }

                Debug.Fail("no implemented persitance interfaces on object");
                throw new InvalidOperationException(SR.UnableToInitComponent);
            }

            // Otherwise, we have state to deperist from:
            switch (ocxState.Type)
            {
                case STG_STREAM:
                    try
                    {
                        iPersistStream = (Ole32.IPersistStream)instance;
                        DepersistFromIStream(ocxState.GetStream());
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Exception thrown trying to IPersistStream.DepersistFromIStream(). Is this good?" + e.ToString());
                    }
                    break;
                case STG_STREAMINIT:
                    if (instance is Ole32.IPersistStreamInit)
                    {
                        try
                        {
                            iPersistStreamInit = (Ole32.IPersistStreamInit)instance;
                            DepersistFromIStreamInit(ocxState.GetStream());
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Exception thrown trying to IPersistStreamInit.DepersistFromIStreamInit(). Is this good?" + e.ToString());
                        }
                        GetControlEnabled();
                    }
                    else
                    {
                        ocxState.Type = STG_STREAM;
                        DepersistControl();
                        return;
                    }
                    break;
                case STG_STORAGE:
                    try
                    {
                        iPersistStorage = (Ole32.IPersistStorage)instance;
                        DepersistFromIStorage(ocxState.GetStorage());
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Exception thrown trying to IPersistStorage.DepersistFromIStorage(). Is this good?" + e.ToString());
                    }
                    break;
                default:
                    Debug.Fail("unknown storage type.");
                    throw new InvalidOperationException(SR.UnableToInitComponent);
            }

            if (ocxState.GetPropBag() != null)
            {
                try
                {
                    iPersistPropBag = (Oleaut32.IPersistPropertyBag)instance;
                    DepersistFromIPropertyBag(ocxState.GetPropBag());
                }
                catch (Exception e)
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Exception thrown trying to IPersistPropertyBag.DepersistFromIPropertyBag(). Is this good?" + e.ToString());
                }
            }
        }

        /// <summary>
        ///  Returns the IUnknown pointer to the enclosed ActiveX control.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public object GetOcx()
        {
            return instance;
        }

        private object GetOcxCreate()
        {
            if (instance is null)
            {
                CreateInstance();
                RealizeStyles();
                AttachInterfaces();
                oleSite.OnOcxCreate();
            }
            return instance;
        }

        private void StartEvents()
        {
            if (!axState[sinkAttached])
            {
                try
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Creating sink for events...");
                    CreateSink();
                    oleSite.StartEvents();
                }
                catch (Exception t)
                {
                    Debug.Fail(t.ToString());
                }
                axState[sinkAttached] = true;
            }
        }

        private void StopEvents()
        {
            if (axState[sinkAttached])
            {
                try
                {
                    DetachSink();
                }
                catch (Exception t)
                {
                    Debug.Fail(t.ToString());
                }
                axState[sinkAttached] = false;
            }
            oleSite.StopEvents();
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void CreateSink()
        {
            // nop...  windows forms wrapper will override...
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void DetachSink()
        {
            // nop...  windows forms wrapper will override...
        }

        private bool CanShowPropertyPages()
        {
            if (GetOcState() < OC_RUNNING)
            {
                return false;
            }

            return GetOcx() is Ole32.ISpecifyPropertyPages;
        }

        public unsafe bool HasPropertyPages()
        {
            if (!CanShowPropertyPages())
            {
                return false;
            }

            Ole32.ISpecifyPropertyPages ispp = (Ole32.ISpecifyPropertyPages)GetOcx();
            var uuids = new Ole32.CAUUID();
            try
            {
                HRESULT hr = ispp.GetPages(&uuids);
                if (!hr.Succeeded())
                {
                    return false;
                }

                return uuids.cElems > 0;
            }
            finally
            {
                if (uuids.pElems != null)
                {
                    Marshal.FreeCoTaskMem((IntPtr)uuids.pElems);
                }
            }
        }

        private unsafe void ShowPropertyPageForDispid(Ole32.DispatchID dispid, Guid guid)
        {
            IntPtr pUnk = Marshal.GetIUnknownForObject(GetOcx());
            try
            {
                fixed (char* pName = Name)
                {
                    var opcparams = new Oleaut32.OCPFIPARAMS
                    {
                        cbStructSize = (uint)Marshal.SizeOf<Oleaut32.OCPFIPARAMS>(),
                        hwndOwner = (ContainingControl is null) ? IntPtr.Zero : ContainingControl.Handle,
                        lpszCaption = pName,
                        cObjects = 1,
                        ppUnk = (IntPtr)(&pUnk),
                        cPages = 1,
                        lpPages = (IntPtr)(&guid),
                        lcid = Kernel32.GetThreadLocale(),
                        dispidInitialProperty = dispid
                    };
                    Oleaut32.OleCreatePropertyFrameIndirect(ref opcparams);
                }
            }
            finally
            {
                Marshal.Release(pUnk);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void MakeDirty()
        {
            ISite isite = Site;
            if (isite is null)
            {
                return;
            }

            IComponentChangeService ccs = (IComponentChangeService)isite.GetService(typeof(IComponentChangeService));
            if (ccs is null)
            {
                return;
            }

            ccs.OnComponentChanging(this, null);

            ccs.OnComponentChanged(this, null, null, null);
        }

        public void ShowPropertyPages()
        {
            if (ParentInternal is null)
            {
                return;
            }

            if (!ParentInternal.IsHandleCreated)
            {
                return;
            }

            ShowPropertyPages(ParentInternal);
        }

        public unsafe void ShowPropertyPages(Control control)
        {
            if (!CanShowPropertyPages())
            {
                return;
            }

            Ole32.ISpecifyPropertyPages ispp = (Ole32.ISpecifyPropertyPages)GetOcx();
            var uuids = new Ole32.CAUUID();
            HRESULT hr = ispp.GetPages(&uuids);
            if (!hr.Succeeded() || uuids.cElems == 0)
            {
                return;
            }

            IDesignerHost host = null;
            if (Site != null)
            {
                host = (IDesignerHost)Site.GetService(typeof(IDesignerHost));
            }

            DesignerTransaction trans = null;
            try
            {
                if (host != null)
                {
                    trans = host.CreateTransaction(SR.AXEditProperties);
                }

                IntPtr handle = (ContainingControl is null) ? IntPtr.Zero : ContainingControl.Handle;
                IntPtr pUnk = Marshal.GetIUnknownForObject(GetOcx());
                try
                {
                    Oleaut32.OleCreatePropertyFrame(
                        new HandleRef(this, handle),
                        0,
                        0,
                        null,
                        1,
                        &pUnk,
                        uuids.cElems,
                        uuids.pElems,
                        Kernel32.GetThreadLocale(),
                        0,
                        IntPtr.Zero);
                }
                finally
                {
                    Marshal.Release(pUnk);
                }
            }
            finally
            {
                if (oleSite != null)
                {
                    ((Ole32.IPropertyNotifySink)oleSite).OnChanged(Ole32.DispatchID.UNKNOWN);
                }

                if (trans != null)
                {
                    trans.Commit();
                }

                if (uuids.pElems != null)
                {
                    Marshal.FreeCoTaskMem((IntPtr)uuids.pElems);
                }
            }
        }

        internal override Gdi32.HBRUSH InitializeDCForWmCtlColor(Gdi32.HDC dc, User32.WM msg)
        {
            if (isMaskEdit)
            {
                return base.InitializeDCForWmCtlColor(dc, msg);
            }
            else
            {
                return default; // bypass Control's anti-reflect logic
            }
        }

        /// <summary>
        ///  AxHost wndProc. All messages are sent to wndProc after getting filtered
        ///  through the preProcessMessage function.
        ///  Certain messages are forwarder directly to the ActiveX control,
        ///  others are first processed by the wndProc of Control
        /// </summary>
        protected unsafe override void WndProc(ref Message m)
        {
            // Ignore the warnings generated by the following code (unreachable code, and unreachable expression)
            if (false && (axState[manualUpdate] && IsUserMode()))
            {
                DefWndProc(ref m);
                return;
            }

            switch ((User32.WM)m.Msg)
            {
                // Things we explicitly ignore and pass to the ocx's windproc
                case User32.WM.ERASEBKGND:

                case User32.WM.REFLECT_NOTIFYFORMAT:

                case User32.WM.SETCURSOR:
                case User32.WM.SYSCOLORCHANGE:

                // Some of the MSComCtl controls respond to this message
                // to do some custom painting. So, we should just pass this message
                // through.
                //
                case User32.WM.DRAWITEM:

                case User32.WM.LBUTTONDBLCLK:
                case User32.WM.LBUTTONUP:
                case User32.WM.MBUTTONDBLCLK:
                case User32.WM.MBUTTONUP:
                case User32.WM.RBUTTONDBLCLK:
                case User32.WM.RBUTTONUP:
                    DefWndProc(ref m);
                    break;

                case User32.WM.LBUTTONDOWN:
                case User32.WM.MBUTTONDOWN:
                case User32.WM.RBUTTONDOWN:
                    if (IsUserMode())
                    {
                        Focus();
                    }
                    DefWndProc(ref m);
                    break;

                case User32.WM.KILLFOCUS:
                    {
                        hwndFocus = m.WParam;
                        try
                        {
                            base.WndProc(ref m);
                        }
                        finally
                        {
                            hwndFocus = IntPtr.Zero;
                        }
                        break;
                    }

                case User32.WM.COMMAND:
                    if (!ReflectMessage(m.LParam, ref m))
                    {
                        DefWndProc(ref m);
                    }
                    break;

                case User32.WM.CONTEXTMENU:
                    DefWndProc(ref m);
                    break;

                case User32.WM.DESTROY:
#if DEBUG
                    if (!OwnWindow())
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "WM_DESTROY naughty control is destroying the window from under us..." + GetType().ToString());
                    }
#endif
                    //
                    // If we are currently in a state of InPlaceActive or above,
                    // we should first reparent the ActiveX control to our parking
                    // window before we transition to a state below InPlaceActive.
                    // Otherwise we face all sorts of problems when we try to
                    // transition back to a state >= InPlaceActive.
                    //
                    if (GetOcState() >= OC_INPLACE)
                    {
                        Ole32.IOleInPlaceObject ipo = GetInPlaceObject();
                        IntPtr hwnd = IntPtr.Zero;
                        if (ipo.GetWindow(&hwnd).Succeeded())
                        {
                            Application.ParkHandle(new HandleRef(ipo, hwnd));
                        }
                    }

                    bool visible = GetState(States.Visible);

                    TransitionDownTo(OC_RUNNING);
                    DetachAndForward(ref m);

                    if (visible != GetState(States.Visible))
                    {
                        SetState(States.Visible, visible);
                    }

                    break;
                case User32.WM.HELP:
                    // We want to both fire the event, and let the ocx have the message...
                    base.WndProc(ref m);
                    DefWndProc(ref m);
                    break;

                case User32.WM.KEYUP:
                    if (axState[processingKeyUp])
                    {
                        break;
                    }

                    axState[processingKeyUp] = true;
                    try
                    {
                        if (PreProcessControlMessage(ref m) != PreProcessControlState.MessageProcessed)
                        {
                            DefWndProc(ref m);
                        }
                    }
                    finally
                    {
                        axState[processingKeyUp] = false;
                    }

                    break;

                case User32.WM.NCDESTROY:
#if DEBUG
                    if (!OwnWindow())
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "WM_NCDESTROY naughty control is destroying the window from under us..." + GetType().ToString());
                    }
#endif
                    // need to detach it now...
                    DetachAndForward(ref m);
                    break;

                default:
                    if (m.Msg == (int)REGMSG_MSG)
                    {
                        m.Result = (IntPtr)REGMSG_RETVAL;
                        return;
                    }
                    // Other things we may care about and we will pass them to the Control's wndProc
                    base.WndProc(ref m);
                    break;
            }
        }

        private void DetachAndForward(ref Message m)
        {
            IntPtr handle = GetHandleNoCreate();
            DetachWindow();
            if (handle != IntPtr.Zero)
            {
                IntPtr wndProc = User32.GetWindowLong(new HandleRef(this, handle), User32.GWL.WNDPROC);
                m.Result = User32.CallWindowProcW(wndProc, handle, (User32.WM)m.Msg, m.WParam, m.LParam);
            }
        }

        private void DetachWindow()
        {
            if (IsHandleCreated)
            {
                OnHandleDestroyed(EventArgs.Empty);
                WindowReleaseHandle();
            }
        }

        private void InformOfNewHandle()
        {
            Debug.Assert(IsHandleCreated, "we got to have a handle to be here...");
            wndprocAddr = User32.GetWindowLong(this, User32.GWL.WNDPROC);
        }

        private void AttachWindow(IntPtr hwnd)
        {
            Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "attaching window for " + ToString() + " " + hwnd.ToString());
            if (!axState[fFakingWindow])
            {
                WindowAssignHandle(hwnd, axState[assignUniqueID]);
            }
            UpdateZOrder();

            // Get the latest bounds set by the user.
            Size setExtent = Size;
            Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "SetBounds " + setExtent.ToString());

            // Get the default bounds set by the ActiveX control.
            UpdateBounds();
            Size ocxExtent = GetExtent();
            Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "OcxBounds " + ocxExtent.ToString());

            Point location = Location;

            // Choose the setBounds unless it is smaller than the default bounds.
            if (setExtent.Width < ocxExtent.Width || setExtent.Height < ocxExtent.Height)
            {
                Bounds = new Rectangle(location.X, location.Y, ocxExtent.Width, ocxExtent.Height);
            }
            else
            {
                Size newSize = SetExtent(setExtent.Width, setExtent.Height);
                if (!newSize.Equals(setExtent))
                {
                    Bounds = new Rectangle(location.X, location.Y, newSize.Width, newSize.Height);
                }
            }

            OnHandleCreated(EventArgs.Empty);
            InformOfNewHandle();
        }

        /// <summary>
        ///  Inheriting classes should override this method to find out when the
        ///  handle has been created.
        ///  Call base.OnHandleCreated first.
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            // This is needed to prevent some controls (for e.g. Office Web Components) from
            // failing to InPlaceActivate() when they call RegisterDragDrop() but do not call
            // OleInitialize(). The EE calls CoInitializeEx() on the thread, but I believe
            // that is not good enough for DragDrop.
            //
            if (Application.OleRequired() != System.Threading.ApartmentState.STA)
            {
                throw new ThreadStateException(SR.ThreadMustBeSTA);
            }

            SetAcceptDrops(AllowDrop);
            RaiseCreateHandleEvent(e);
        }

        private const int HMperInch = 2540;
        private int Pix2HM(int pix, int logP)
        {
            return (HMperInch * pix + (logP >> 1)) / logP;
        }

        private int HM2Pix(int hm, int logP)
        {
            return (logP * hm + HMperInch / 2) / HMperInch;
        }

        private unsafe bool QuickActivate()
        {
            if (!(instance is Ole32.IQuickActivate))
            {
                return false;
            }

            Ole32.IQuickActivate iqa = (Ole32.IQuickActivate)instance;

            var qaContainer = new Ole32.QACONTAINER
            {
                cbSize = (uint)Marshal.SizeOf<Ole32.QACONTAINER>()
            };
            var qaControl = new Ole32.QACONTROL
            {
                cbSize = (uint)Marshal.SizeOf<Ole32.QACONTROL>()
            };

            qaContainer.pClientSite = oleSite;
            qaContainer.pPropertyNotifySink = oleSite;
            qaContainer.pFont = (Ole32.IFont)GetIFontFromFont(GetParentContainer().parent.Font);
            qaContainer.dwAppearance = 0;
            qaContainer.lcid = Kernel32.GetThreadLocale();

            Control p = ParentInternal;

            if (p != null)
            {
                qaContainer.colorFore = GetOleColorFromColor(p.ForeColor);
                qaContainer.colorBack = GetOleColorFromColor(p.BackColor);
            }
            else
            {
                qaContainer.colorFore = GetOleColorFromColor(SystemColors.WindowText);
                qaContainer.colorBack = GetOleColorFromColor(SystemColors.Window);
            }
            qaContainer.dwAmbientFlags = Ole32.QACONTAINERFLAGS.AUTOCLIP | Ole32.QACONTAINERFLAGS.MESSAGEREFLECT | Ole32.QACONTAINERFLAGS.SUPPORTSMNEMONICS;
            if (IsUserMode())
            {
                // In design mode we'd ideally set QACONTAINER_UIDEAD on dwAmbientFlags
                // so controls don't take keyboard input, but MFC controls return NOWHERE on
                // NCHITTEST, which messes up the designer.
                qaContainer.dwAmbientFlags |= Ole32.QACONTAINERFLAGS.USERMODE;
            }

            HRESULT hr = iqa.QuickActivate(qaContainer, &qaControl);
            if (!hr.Succeeded())
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Failed to QuickActivate: " + hr.ToString());
                DisposeAxControl();
                return false;
            }

            _miscStatusBits = qaControl.dwMiscStatus;
            ParseMiscBits(_miscStatusBits);
            return true;
        }

        internal override void DisposeAxControls()
        {
            axState[rejectSelection] = true;
            base.DisposeAxControls();
            TransitionDownTo(OC_PASSIVE);
        }

        private bool GetControlEnabled()
        {
            try
            {
                return IsHandleCreated;
            }
            catch (Exception t)
            {
                Debug.Fail(t.ToString());
                return true;
            }
        }

        internal override bool CanSelectCore()
        {
            if (!GetControlEnabled() || axState[rejectSelection])
            {
                return false;
            }

            return base.CanSelectCore();
        }

        /// <summary>
        ///  Frees all resources assocaited with this control. This method may not be
        ///  called at runtime. Any resources used by the control should be setup to
        ///  be released when the control is garbage collected. Inheriting classes should always
        ///  call base.dispose.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                TransitionDownTo(OC_PASSIVE);
                if (newParent != null)
                {
                    newParent.Dispose();
                }

                if (oleSite != null)
                {
                    oleSite.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private bool GetSiteOwnsDeactivation()
        {
            return axState[ownDisposing];
        }

        private void DisposeAxControl()
        {
            if (GetParentContainer() != null)
            {
                GetParentContainer().RemoveControl(this);
            }
            TransitionDownTo(OC_RUNNING);
            if (GetOcState() == OC_RUNNING)
            {
                GetOleObject().SetClientSite(null);
                SetOcState(OC_LOADED);
            }
        }

        private void ReleaseAxControl()
        {
            // This line is like a bit of magic...
            // sometimes, we crash with it on,
            // sometimes, with it off...
            // Lately, I have decided to leave it on...
            // (oh, yes, and the crashes seemed to disappear...)
            //cpr: ComLib.Release(instance);

            NoComponentChangeEvents++;

            ContainerControl f = ContainingControl;
            if (f != null)
            {
                f.VisibleChanged -= onContainerVisibleChanged;
            }

            try
            {
                if (instance != null)
                {
                    Marshal.FinalReleaseComObject(instance);
                    instance = null;
                    iOleInPlaceObject = null;
                    iOleObject = null;
                    iOleControl = null;
                    iOleInPlaceActiveObject = null;
                    iOleInPlaceActiveObjectExternal = null;
                    iPerPropertyBrowsing = null;
                    iCategorizeProperties = null;
                    iPersistStream = null;
                    iPersistStreamInit = null;
                    iPersistStorage = null;
                }

                axState[checkedIppb] = false;
                axState[checkedCP] = false;
                axState[disposed] = true;

                freezeCount = 0;
                axState[sinkAttached] = false;
                wndprocAddr = IntPtr.Zero;

                SetOcState(OC_PASSIVE);
            }
            finally
            {
                NoComponentChangeEvents--;
            }
        }

        private void ParseMiscBits(Ole32.OLEMISC bits)
        {
            axState[fOwnWindow] = ((bits & Ole32.OLEMISC.INVISIBLEATRUNTIME) != 0) && IsUserMode();
            axState[fSimpleFrame] = ((bits & Ole32.OLEMISC.SIMPLEFRAME) != 0);
        }

        private void SlowActivate()
        {
            bool setClientSite = false;

            if ((_miscStatusBits & Ole32.OLEMISC.SETCLIENTSITEFIRST) != 0)
            {
                GetOleObject().SetClientSite(oleSite);
                setClientSite = true;
            }

            DepersistControl();

            if (!setClientSite)
            {
                GetOleObject().SetClientSite(oleSite);
            }
        }

        private AxContainer GetParentContainer()
        {
            if (container is null)
            {
                container = AxContainer.FindContainerForControl(this);
            }
            if (container is null)
            {
                ContainerControl f = ContainingControl;
                if (f is null)
                {
                    // ContainingCointrol can be null if the AxHost is still not parented to a containerControl
                    // In everett we used to return a parking window.
                    // now we just set the containingControl to a dummyValue.
                    if (newParent is null)
                    {
                        newParent = new ContainerControl();
                        axContainer = newParent.CreateAxContainer();
                        axContainer.AddControl(this);
                    }
                    return axContainer;
                }
                else
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "calling upon " + f.ToString() + " to create a container");
                    container = f.CreateAxContainer();
                    container.AddControl(this);
                    containingControl = f;
                }
            }
            return container;
        }

        private Ole32.IOleControl GetOleControl() => iOleControl ??= (Ole32.IOleControl)instance;

        private Ole32.IOleInPlaceActiveObject GetInPlaceActiveObject()
        {
            // if our AxContainer was set an external active object then use it.
            if (iOleInPlaceActiveObjectExternal != null)
            {
                return iOleInPlaceActiveObjectExternal;
            }

            // otherwise use our instance.
            if (iOleInPlaceActiveObject is null)
            {
                Debug.Assert(instance != null, "must have the ocx");
                try
                {
                    iOleInPlaceActiveObject = (Ole32.IOleInPlaceActiveObject)instance;
                }
                catch (InvalidCastException e)
                {
                    Debug.Fail("Invalid cast in GetInPlaceActiveObject: " + e.ToString());
                }
            }
            return iOleInPlaceActiveObject;
        }

        private Ole32.IOleObject GetOleObject() => iOleObject ??= (Ole32.IOleObject)instance;

        private Ole32.IOleInPlaceObject GetInPlaceObject()
        {
            if (iOleInPlaceObject is null)
            {
                Debug.Assert(instance != null, "must have the ocx");
                iOleInPlaceObject = (Ole32.IOleInPlaceObject)instance;

#if DEBUG
                if (iOleInPlaceObject is Ole32.IOleInPlaceObjectWindowless)
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, GetType().FullName + " Can also be a Windowless control.");
                }
#endif //DEBUG
            }
            return iOleInPlaceObject;
        }

        private VSSDK.ICategorizeProperties GetCategorizeProperties()
        {
            if (iCategorizeProperties is null && !axState[checkedCP] && instance != null)
            {
                axState[checkedCP] = true;
                if (instance is VSSDK.ICategorizeProperties)
                {
                    iCategorizeProperties = (VSSDK.ICategorizeProperties)instance;
                }
            }
            return iCategorizeProperties;
        }

        private Oleaut32.IPerPropertyBrowsing GetPerPropertyBrowsing()
        {
            if (iPerPropertyBrowsing is null && !axState[checkedIppb] && instance != null)
            {
                axState[checkedIppb] = true;
                if (instance is Oleaut32.IPerPropertyBrowsing)
                {
                    iPerPropertyBrowsing = (Oleaut32.IPerPropertyBrowsing)instance;
                }
            }
            return iPerPropertyBrowsing;
        }

        // Mapping functions:
        private static Ole32.PICTDESC GetPICTDESCFromPicture(Image image)
        {
            if (image is Bitmap bmp)
            {
                return Ole32.PICTDESC.FromBitmap(bmp);
            }

            if (image is Metafile mf)
            {
                return Ole32.PICTDESC.FromMetafile(mf);
            }

            throw new ArgumentException(SR.AXUnknownImage, nameof(image));
        }

        /// <summary>
        ///  Maps from a System.Drawing.Image to an OLE IPicture
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected static object GetIPictureFromPicture(Image image)
        {
            if (image is null)
            {
                return null;
            }

            Ole32.PICTDESC pictdesc = GetPICTDESCFromPicture(image);
            return Ole32.OleCreatePictureIndirect(ref pictdesc, ref ipicture_Guid, fOwn: BOOL.TRUE);
        }

        /// <summary>
        ///  Maps from a System.Drawing.Cursor to an OLE IPicture
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected static object GetIPictureFromCursor(Cursor cursor)
        {
            if (cursor is null)
            {
                return null;
            }

            Ole32.PICTDESC desc = Ole32.PICTDESC.FromIcon(Icon.FromHandle(cursor.Handle), copy: true);
            return Ole32.OleCreatePictureIndirect(ref desc, ref ipicture_Guid, fOwn: BOOL.TRUE);
        }

        /// <summary>
        ///  Maps from a System.Drawing.Image to an OLE IPictureDisp
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected static object GetIPictureDispFromPicture(Image image)
        {
            if (image is null)
            {
                return null;
            }

            Ole32.PICTDESC desc = GetPICTDESCFromPicture(image);
            return Ole32.OleCreatePictureIndirect(ref desc, ref ipictureDisp_Guid, fOwn: BOOL.TRUE);
        }

        /// <summary>
        ///  Maps from an OLE IPicture to a System.Drawing.Image
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected static Image GetPictureFromIPicture(object picture)
        {
            if (picture is null)
            {
                return null;
            }

            int hPal = default;
            Ole32.IPicture pict = (Ole32.IPicture)picture;
            Ole32.PICTYPE type = (Ole32.PICTYPE)pict.Type;
            if (type == Ole32.PICTYPE.BITMAP)
            {
                try
                {
                    hPal = pict.hPal;
                }
                catch (COMException)
                {
                }
            }

            return GetPictureFromParams(pict.Handle, type, hPal, pict.Width, pict.Height);
        }

        /// <summary>
        ///  Maps from an OLE IPictureDisp to a System.Drawing.Image
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected unsafe static Image GetPictureFromIPictureDisp(object picture)
        {
            if (picture is null)
            {
                return null;
            }

            int hPal = default;
            Ole32.IPictureDisp pict = (Ole32.IPictureDisp)picture;
            Ole32.PICTYPE type = (Ole32.PICTYPE)pict.Type;
            if (type == Ole32.PICTYPE.BITMAP)
            {
                try
                {
                    hPal = pict.hPal;
                }
                catch (COMException)
                {
                }
            }

            Image image = GetPictureFromParams(pict.Handle, type, hPal, pict.Width, pict.Height);
            GC.KeepAlive(pict);
            return image;
        }

        private static Image GetPictureFromParams(
            int handle,
            Ole32.PICTYPE type,
            int paletteHandle,
            int width,
            int height)
        {
            switch (type)
            {
                case Ole32.PICTYPE.ICON:
                    return (Image)(Icon.FromHandle((IntPtr)handle)).Clone();
                case Ole32.PICTYPE.METAFILE:
                    WmfPlaceableFileHeader header = new WmfPlaceableFileHeader
                    {
                        BboxRight = (short)width,
                        BboxBottom = (short)height
                    };

                    using (var metafile = new Metafile((IntPtr)handle, header, deleteWmf: false))
                    {
                        return (Image)metafile.Clone();
                    }
                case Ole32.PICTYPE.ENHMETAFILE:
                    using (var metafile = new Metafile((IntPtr)handle, deleteEmf: false))
                    {
                        return (Image)metafile.Clone();
                    }
                case Ole32.PICTYPE.BITMAP:
                    return Image.FromHbitmap((IntPtr)handle, (IntPtr)paletteHandle);
                case Ole32.PICTYPE.NONE:
                    // MSDN says this should not be a valid value, but comctl32 returns it...
                    return null;
                case Ole32.PICTYPE.UNINITIALIZED:
                    return null;
                default:
                    Debug.Fail($"Invalid image type {type}");
                    throw new ArgumentException(SR.AXUnknownImage, nameof(type));
            }
        }

        private static Oleaut32.FONTDESC GetFONTDESCFromFont(Font font)
        {
            if (fontTable is null)
            {
                fontTable = new Dictionary<Font, Oleaut32.FONTDESC>();
            }
            else if (fontTable.TryGetValue(font, out Oleaut32.FONTDESC cachedFDesc))
            {
                return cachedFDesc;
            }

            User32.LOGFONTW logfont = User32.LOGFONTW.FromFont(font);
            var fdesc = new Oleaut32.FONTDESC
            {
                cbSizeOfStruct = (uint)Marshal.SizeOf<Oleaut32.FONTDESC>(),
                lpstrName = font.Name,
                cySize = (long)(font.SizeInPoints * 10000),
                sWeight = (short)logfont.lfWeight,
                sCharset = logfont.lfCharSet,
                fItalic = font.Italic.ToBOOL(),
                fUnderline = font.Underline.ToBOOL(),
                fStrikethrough = font.Strikeout.ToBOOL()
            };
            fontTable[font] = fdesc;
            return fdesc;
        }

        /// <summary>
        ///  Maps from an OLE COLOR to a System.Drawing.Color
        /// </summary>
        [CLSCompliant(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected static Color GetColorFromOleColor(uint color)
        {
            return ColorTranslator.FromOle((int)color);
        }

        /// <summary>
        ///  Maps from an System.Drawing.Color to an OLE COLOR
        /// </summary>
        [CLSCompliant(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected static uint GetOleColorFromColor(Color color)
        {
            return (uint)ColorTranslator.ToOle(color);
        }

        /// <summary>
        ///  Maps from a System.Drawing.Font object to an OLE IFont
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected static object GetIFontFromFont(Font font)
        {
            if (font is null)
            {
                return null;
            }

            if (font.Unit != GraphicsUnit.Point)
            {
                throw new ArgumentException(SR.AXFontUnitNotPoint, nameof(font));
            }

            try
            {
                Oleaut32.FONTDESC fontDesc = GetFONTDESCFromFont(font);
                return Oleaut32.OleCreateFontIndirect(ref fontDesc, ref ifont_Guid);
            }
            catch
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Failed to create IFrom from font: " + font.ToString());
                return null;
            }
        }

        /// <summary>
        ///  Maps from an OLE IFont to a System.Drawing.Font object
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected static Font GetFontFromIFont(object font)
        {
            if (font is null)
            {
                return null;
            }

            Ole32.IFont oleFont = (Ole32.IFont)font;
            try
            {
                Font f = Font.FromHfont(oleFont.hFont);
                if (f.Unit == GraphicsUnit.Point)
                {
                    return f;
                }

                return new Font(f.Name, f.SizeInPoints, f.Style, GraphicsUnit.Point, f.GdiCharSet, f.GdiVerticalFont);
            }
            catch (Exception e)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Could not create font." + e.Message);
                return DefaultFont;
            }
        }

        /// <summary>
        ///  Maps from a System.Drawing.Font object to an OLE IFontDisp
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected static object GetIFontDispFromFont(Font font)
        {
            if (font is null)
            {
                return null;
            }

            if (font.Unit != GraphicsUnit.Point)
            {
                throw new ArgumentException(SR.AXFontUnitNotPoint, nameof(font));
            }

            Oleaut32.FONTDESC fontdesc = GetFONTDESCFromFont(font);
            return Oleaut32.OleCreateIFontDispIndirect(ref fontdesc, ref ifontDisp_Guid);
        }

        /// <summary>
        ///  Maps from an IFontDisp to a System.Drawing.Font object
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected static Font GetFontFromIFontDisp(object font)
        {
            if (font is null)
            {
                return null;
            }

            if (font is Ole32.IFont ifont)
            {
                return GetFontFromIFont(ifont);
            }

            Ole32.IFontDisp oleFont = (Ole32.IFontDisp)font;
            FontStyle style = FontStyle.Regular;

            try
            {
                if (oleFont.Bold)
                {
                    style |= FontStyle.Bold;
                }

                if (oleFont.Italic)
                {
                    style |= FontStyle.Italic;
                }

                if (oleFont.Underline)
                {
                    style |= FontStyle.Underline;
                }

                if (oleFont.Strikethrough)
                {
                    style |= FontStyle.Strikeout;
                }

                if (oleFont.Weight >= 700)
                {
                    style |= FontStyle.Bold;
                }

                return new Font(oleFont.Name, (float)oleFont.Size / (float)10000, style, GraphicsUnit.Point, (byte)oleFont.Charset);
            }
            catch (Exception e)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Could not create font from: " + oleFont.Name + ". " + e.Message);
                return DefaultFont;
            }
        }

        /// <summary>
        ///  Maps from a DateTime object to an OLE DATE (expressed as a double)
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected static double GetOADateFromTime(DateTime time)
        {
            return time.ToOADate();
        }

        /// <summary>
        ///  Maps from an OLE DATE (expressed as a double) to a DateTime object
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected static DateTime GetTimeFromOADate(double date)
        {
            return DateTime.FromOADate(date);
        }

        private int Convert2int(object o, bool xDirection)
        {
            o = ((Array)o).GetValue(0);
            // yacky yacky yacky...
            // so, usercontrols & other visual basic related controls give us coords as floats in twips
            // but mfc controls give us integers as pixels...
            if (o.GetType() == typeof(float))
            {
                return Twip2Pixel(Convert.ToDouble(o, CultureInfo.InvariantCulture), xDirection);
            }
            return Convert.ToInt32(o, CultureInfo.InvariantCulture);
        }

        private short Convert2short(object o)
        {
            o = ((Array)o).GetValue(0);
            return Convert.ToInt16(o, CultureInfo.InvariantCulture);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void RaiseOnMouseMove(object o1, object o2, object o3, object o4)
        {
            RaiseOnMouseMove(Convert2short(o1), Convert2short(o2), Convert2int(o3, true), Convert2int(o4, false));
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void RaiseOnMouseMove(short button, short shift, float x, float y)
        {
            RaiseOnMouseMove(button, shift, Twip2Pixel((int)x, true), Twip2Pixel((int)y, false));
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void RaiseOnMouseMove(short button, short shift, int x, int y)
        {
            base.OnMouseMove(new MouseEventArgs((MouseButtons)(((int)button) << 20), 1, x, y, 0));
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void RaiseOnMouseUp(object o1, object o2, object o3, object o4)
        {
            RaiseOnMouseUp(Convert2short(o1), Convert2short(o2), Convert2int(o3, true), Convert2int(o4, false));
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void RaiseOnMouseUp(short button, short shift, float x, float y)
        {
            RaiseOnMouseUp(button, shift, Twip2Pixel((int)x, true), Twip2Pixel((int)y, false));
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void RaiseOnMouseUp(short button, short shift, int x, int y)
        {
            base.OnMouseUp(new MouseEventArgs((MouseButtons)(((int)button) << 20), 1, x, y, 0));
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void RaiseOnMouseDown(object o1, object o2, object o3, object o4)
        {
            RaiseOnMouseDown(Convert2short(o1), Convert2short(o2), Convert2int(o3, true), Convert2int(o4, false));
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void RaiseOnMouseDown(short button, short shift, float x, float y)
        {
            RaiseOnMouseDown(button, shift, Twip2Pixel((int)x, true), Twip2Pixel((int)y, false));
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void RaiseOnMouseDown(short button, short shift, int x, int y)
        {
            base.OnMouseDown(new MouseEventArgs((MouseButtons)(((int)button) << 20), 1, x, y, 0));
        }

        protected delegate void AboutBoxDelegate();
    }
}
