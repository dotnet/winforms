// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms.ComponentModel.Com2Interop;
using System.Windows.Forms.Design;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Wraps ActiveX controls and exposes them as
    ///  fully featured windows forms controls.
    /// </summary>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    ToolboxItem(false),
    DesignTimeVisible(false),
    DefaultEvent(nameof(Enter)),
    Designer("System.Windows.Forms.Design.AxHostDesigner, " + AssemblyRef.SystemDesign),
    ]
    public abstract class AxHost : Control, ISupportInitialize, ICustomTypeDescriptor
    {
        private static readonly TraceSwitch AxHTraceSwitch = new TraceSwitch("AxHTrace", "ActiveX handle tracing");
        private static readonly TraceSwitch AxPropTraceSwitch = new TraceSwitch("AxPropTrace", "ActiveX property tracing");
        private static readonly TraceSwitch AxHostSwitch = new TraceSwitch("AxHost", "ActiveX host creation");
        private static readonly BooleanSwitch AxIgnoreTMSwitch = new BooleanSwitch("AxIgnoreTM", "ActiveX switch to ignore thread models");
        private static readonly BooleanSwitch AxAlwaysSaveSwitch = new BooleanSwitch("AxAlwaysSave", "ActiveX to save all controls regardless of their IsDirty function return value");

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

        private static readonly COMException E_NOTIMPL = new COMException(SR.AXNotImplemented, unchecked((int)0x80000001));
        private static readonly COMException E_INVALIDARG = new COMException(SR.AXInvalidArgument, unchecked((int)0x80070057));
        private static readonly COMException E_FAIL = new COMException(SR.AXUnknownError, unchecked((int)0x80004005));
        private static readonly COMException E_NOINTERFACE = new COMException(SR.AxInterfaceNotSupported, unchecked((int)0x80004002));

        private const int INPROC_SERVER = 1;
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

        private const int OLEIVERB_SHOW = -1;
        private const int OLEIVERB_HIDE = -3;
        private const int OLEIVERB_UIACTIVATE = -4;
        private const int OLEIVERB_INPLACEACTIVATE = -5;
        private const int OLEIVERB_PROPERTIES = -7;
        private const int OLEIVERB_PRIMARY = 0;

        private readonly int REGMSG_MSG = SafeNativeMethods.RegisterWindowMessage(Application.WindowMessagesVersion + "_subclassCheck");
        private const int REGMSG_RETVAL = 123;

        private static int logPixelsX = -1;
        private static int logPixelsY = -1;

        private static Guid icf2_Guid = typeof(UnsafeNativeMethods.IClassFactory2).GUID;
        private static Guid ifont_Guid = typeof(UnsafeNativeMethods.IFont).GUID;
        private static Guid ifontDisp_Guid = typeof(SafeNativeMethods.IFontDisp).GUID;
        private static Guid ipicture_Guid = typeof(UnsafeNativeMethods.IPicture).GUID;
        private static Guid ipictureDisp_Guid = typeof(UnsafeNativeMethods.IPictureDisp).GUID;
        private static Guid ivbformat_Guid = typeof(UnsafeNativeMethods.IVBFormat).GUID;
        private static Guid ioleobject_Guid = typeof(UnsafeNativeMethods.IOleObject).GUID;
        private static Guid dataSource_Guid = new Guid("{7C0FFAB3-CD84-11D0-949A-00A0C91110ED}");
        private static Guid windowsMediaPlayer_Clsid = new Guid("{22d6f312-b0f6-11d0-94ab-0080c74c7e95}");
        private static Guid comctlImageCombo_Clsid = new Guid("{a98a24c0-b06f-3684-8c12-c52ae341e0bc}");
        private static Guid maskEdit_Clsid = new Guid("{c932ba85-4374-101b-a56c-00aa003668dc}");

        // Static state for perf optimization
        //
        private static Hashtable fontTable;

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

        private BitVector32 axState = new BitVector32();

        private int storageType = STG_UNKNOWN;
        private int ocState = OC_PASSIVE;
        private int miscStatusBits;
        private int freezeCount = 0;
        private readonly int flags = 0;
        private int selectionStyle = 0;
        private int editMode = EDITM_NONE;
        private int noComponentChange = 0;

        private IntPtr wndprocAddr = IntPtr.Zero;

        private Guid clsid;
        private string text = string.Empty;
        private string licenseKey = null;

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
        private Hashtable properties = null;
        private Hashtable propertyInfos = null;
        private PropertyDescriptorCollection propsStash = null;
        private Attribute[] attribsStash = null;

        // interface pointers to the ocx
        //
        private object instance;
        private UnsafeNativeMethods.IOleInPlaceObject iOleInPlaceObject;
        private UnsafeNativeMethods.IOleObject iOleObject;
        private UnsafeNativeMethods.IOleControl iOleControl;
        private UnsafeNativeMethods.IOleInPlaceActiveObject iOleInPlaceActiveObject;
        private UnsafeNativeMethods.IOleInPlaceActiveObject iOleInPlaceActiveObjectExternal;
        private NativeMethods.IPerPropertyBrowsing iPerPropertyBrowsing;
        private NativeMethods.ICategorizeProperties iCategorizeProperties;
        private UnsafeNativeMethods.IPersistPropertyBag iPersistPropBag;
        private Ole32.IPersistStream iPersistStream;
        private Ole32.IPersistStreamInit iPersistStreamInit;
        private Ole32.IPersistStorage iPersistStorage;

        private AboutBoxDelegate aboutBoxDelegate = null;
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

        private Hashtable objectDefinedCategoryNames = null; // Integer -> String

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
                    cp.Style &= (~NativeMethods.WS_VISIBLE);
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

        private void RealizeStyles()
        {
            SetStyle(ControlStyles.UserPaint, false);
            int hr = GetOleObject().GetMiscStatus(NativeMethods.ActiveX.DVASPECT_CONTENT, out int bits);
            if (!NativeMethods.Failed(hr))
            {
                miscStatusBits = bits;
                ParseMiscBits(miscStatusBits);
            }
        }

        // Control overrides:

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }

            set
            {
                base.BackColor = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        /// <summary>
        ///  Hide ImeMode: it doesn't make sense for this control
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
        new public event EventHandler MouseClick
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseClick"));
            remove { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler MouseDoubleClick
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseDoubleClick"));
            remove { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Cursor Cursor
        {
            get
            {
                return base.Cursor;
            }

            set
            {
                base.Cursor = value;
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

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual new bool Enabled
        {
            get
            {
                return base.Enabled;
            }

            set
            {
                base.Enabled = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Font Font
        {
            get
            {
                return base.Font;
            }

            set
            {
                base.Font = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }

            set
            {
                base.ForeColor = value;
            }
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Never),
            Localizable(true)
        ]
        public new virtual bool RightToLeft
        {
            get
            {
                RightToLeft rtol = base.RightToLeft;
                return rtol == System.Windows.Forms.RightToLeft.Yes;
            }

            set
            {
                base.RightToLeft = (value) ? System.Windows.Forms.RightToLeft.Yes : System.Windows.Forms.RightToLeft.No;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
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
                return (axState[fOwnWindow] &&
                       (ocState > OC_RUNNING || (IsUserMode() && ocState >= OC_RUNNING)) ||
                       ocState >= OC_INPLACE);
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
                else if (f.Visible && !GetState(STATE_VISIBLE) && IsHandleCreated && GetOcState() >= OC_INPLACE)
                {
                    HideAxControl();
                }
            }
        }

        //
        /// <summary>
        ///  Determines if the control is in edit mode.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
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
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
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

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackColorChanged
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "BackColorChanged"));
            remove { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageChanged
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "BackgroundImageChanged"));
            remove { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageLayoutChanged
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "BackgroundImageLayoutChanged"));
            remove { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BindingContextChanged
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "BindingContextChanged"));
            remove { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler CursorChanged
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "CursorChanged"));
            remove { }
        }

        /// <summary>
        ///  Occurs when the control is enabled.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler EnabledChanged
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "EnabledChanged"));
            remove { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler FontChanged
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "FontChanged"));
            remove { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler ForeColorChanged
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "ForeColorChanged"));
            remove { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler RightToLeftChanged
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "RightToLeftChanged"));
            remove { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TextChanged
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "TextChanged"));
            remove { }
        }

        /// <summary>
        ///  Occurs when the control is clicked.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler Click
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "Click"));
            remove { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event DragEventHandler DragDrop
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "DragDrop"));
            remove { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event DragEventHandler DragEnter
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "DragEnter"));
            remove { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event DragEventHandler DragOver
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "DragOver"));
            remove { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler DragLeave
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "DragLeave"));
            remove { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event GiveFeedbackEventHandler GiveFeedback
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "GiveFeedback"));
            remove { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event HelpEventHandler HelpRequested
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "HelpRequested"));
            remove { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event PaintEventHandler Paint
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "Paint"));
            remove { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event QueryContinueDragEventHandler QueryContinueDrag
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "QueryContinueDrag"));
            remove { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "QueryAccessibilityHelp"));
            remove { }
        }

        /// <summary>
        ///  Occurs when the control is double clicked.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler DoubleClick
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "DoubleClick"));
            remove { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler ImeModeChanged
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "ImeModeChanged"));
            remove { }
        }

        /// <summary>
        ///  Occurs when a key is pressed down while the control has focus.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event KeyEventHandler KeyDown
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "KeyDown"));
            remove { }
        }

        /// <summary>
        ///  Occurs when a key is pressed while the control has focus.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event KeyPressEventHandler KeyPress
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "KeyPress"));
            remove { }
        }

        /// <summary>
        ///  Occurs when a key is released while the control has focus.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event KeyEventHandler KeyUp
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "KeyUp"));
            remove { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event LayoutEventHandler Layout
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "Layout"));
            remove { }
        }

        /// <summary>
        ///  Occurs when the mouse pointer is over the control and a mouse button is
        ///  pressed.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event MouseEventHandler MouseDown
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseDown"));
            remove { }
        }

        /// <summary>
        ///  Occurs when the mouse pointer enters the AxHost.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler MouseEnter
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseEnter"));
            remove { }
        }

        /// <summary>
        ///  Occurs when the mouse pointer leaves the AxHost.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler MouseLeave
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseLeave"));
            remove { }
        }

        /// <summary>
        ///  Occurs when the mouse pointer hovers over the contro.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler MouseHover
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseHover"));
            remove { }
        }

        /// <summary>
        ///  Occurs when the mouse pointer is moved over the AxHost.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event MouseEventHandler MouseMove
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseMove"));
            remove { }
        }

        /// <summary>
        ///  Occurs when the mouse pointer is over the control and a mouse button is released.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event MouseEventHandler MouseUp
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseUp"));
            remove { }
        }

        /// <summary>
        ///  Occurs when the mouse wheel moves while the control has focus.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event MouseEventHandler MouseWheel
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseWheel"));
            remove { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event UICuesEventHandler ChangeUICues
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "ChangeUICues"));
            remove { }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler StyleChanged
        {
            add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "StyleChanged"));
            remove { }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            AmbientChanged(NativeMethods.ActiveX.DISPID_AMBIENT_FONT);
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            AmbientChanged(NativeMethods.ActiveX.DISPID_AMBIENT_FORECOLOR);
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            AmbientChanged(NativeMethods.ActiveX.DISPID_AMBIENT_BACKCOLOR);
        }

        private void AmbientChanged(int dispid)
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
        {
            ISite site = ctl.Site;
            if (site != null)
            {
                object o = site.GetService(typeof(ISelectionService));
                Debug.Assert(o == null || o is ISelectionService, "service must implement ISelectionService");
                //Note: if o is null, we want to return null anyway.  Happy day.
                return o as ISelectionService;
            }
            return null;
        }

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
                //
                if (GetOcx() is UnsafeNativeMethods.IOleControl oleCtl)
                {
                    oleCtl.OnAmbientPropertyChange(NativeMethods.ActiveX.DISPID_AMBIENT_DISPLAYNAME);
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
            //

            bool uiDeactivate = (GetHandleNoCreate() != hwndFocus);

            if (uiDeactivate && IsHandleCreated)
            {
                uiDeactivate = !UnsafeNativeMethods.IsChild(new HandleRef(this, GetHandleNoCreate()), new HandleRef(null, hwndFocus));
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
                    int hr = UiDeactivate();
                    if (NativeMethods.Failed(hr))
                    {
                        // not much we can do here...
                        Debug.Fail("Failed to UiDeactivate: " + hr.ToString(CultureInfo.InvariantCulture));
                    }
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
                SetState(STATE_VISIBLE, false);
                base.CreateHandle();
            }

            GetParentContainer().ControlCreated(this);
        }

        private NativeMethods.COMRECT GetClipRect(NativeMethods.COMRECT clipRect)
        {
            if (clipRect != null)
            {
                FillInRect(clipRect, new Rectangle(0, 0, 32000, 32000));
            }
            return clipRect;
        }

        private static HRESULT SetupLogPixels(bool force)
        {
            if (logPixelsX == -1 || force)
            {
                using ScreenDC dc = ScreenDC.Create();
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
            ((UnsafeNativeMethods.IOleControlSite)oleSite).TransformCoords(&phm, &pcont, NativeMethods.ActiveX.XFORMCOORDS_SIZE | NativeMethods.ActiveX.XFORMCOORDS_HIMETRICTOCONTAINER);
            sz.Width = (int)pcont.X;
            sz.Height = (int)pcont.Y;
        }

        private unsafe void Pixel2hiMetric(ref Size sz)
        {
            var phm = new Point();
            var pcont = new PointF(sz.Width, sz.Height);
            ((UnsafeNativeMethods.IOleControlSite)oleSite).TransformCoords(&phm, &pcont, NativeMethods.ActiveX.XFORMCOORDS_SIZE | NativeMethods.ActiveX.XFORMCOORDS_CONTAINERTOHIMETRIC);
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
            Interop.HRESULT hr = GetOleObject().SetExtent(NativeMethods.ActiveX.DVASPECT_CONTENT, &sz);
            if (hr != Interop.HRESULT.S_OK)
            {
                resetExtents = true;
            }
            if (resetExtents)
            {
                GetOleObject().GetExtent(NativeMethods.ActiveX.DVASPECT_CONTENT, &sz);
                GetOleObject().SetExtent(NativeMethods.ActiveX.DVASPECT_CONTENT, &sz);
            }

            return GetExtent();
        }

        private unsafe Size GetExtent()
        {
            var sz = new Size();
            GetOleObject().GetExtent(NativeMethods.ActiveX.DVASPECT_CONTENT, &sz);
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

        private void SetObjectRects(Rectangle bounds)
        {
            if (GetOcState() < OC_INPLACE)
            {
                return;
            }

            GetInPlaceObject().SetObjectRects(FillInRect(new NativeMethods.COMRECT(), bounds), GetClipRect(new NativeMethods.COMRECT()));
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
            IntPtr currentWndproc = UnsafeNativeMethods.GetWindowLong(new HandleRef(this, handle), NativeMethods.GWL_WNDPROC);
            if (currentWndproc == wndprocAddr)
            {
                return true;
            }

            if (unchecked((int)(long)SendMessage(REGMSG_MSG, 0, 0)) == REGMSG_RETVAL)
            {
                wndprocAddr = currentWndproc;
                return true;
            }
            // yikes, we were resubclassed...
            Debug.WriteLineIf(AxHostSwitch.TraceVerbose, "The horrible control subclassed itself w/o calling the old wndproc...");
            // we need to resubclass outselves now...
            Debug.Assert(!OwnWindow(), "why are we here if we own our window?");
            WindowReleaseHandle();
            UnsafeNativeMethods.SetWindowLong(new HandleRef(this, handle), NativeMethods.GWL_WNDPROC, new HandleRef(this, currentWndproc));
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
                            int hr = UiDeactivate();
                            Debug.Assert(NativeMethods.Succeeded(hr), "Failed in UiDeactivate: " + hr.ToString(CultureInfo.InvariantCulture));
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
                                    // a) the hWnd goes away on a OLEIVERB_HIDE and
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

                            //Debug.Assert(GetOcState() == OC_INPLACE, " failed transition");
                            if (GetOcState() < OC_INPLACE)
                            {
                                SetOcState(OC_INPLACE);
                            }
                            OnInPlaceActive();
                            break;
                        case OC_INPLACE:
                            DoVerb(OLEIVERB_SHOW);
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
                DoVerb(OLEIVERB_INPLACEACTIVATE);
            }
            catch (Exception t)
            {
                Debug.Fail(t.ToString());
                throw new TargetInvocationException(string.Format(SR.AXNohWnd, GetType().Name), t);
            }
            EnsureWindowPresent();
        }

        private void InPlaceDeactivate()
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

            try
            {
                GetInPlaceObject().InPlaceDeactivate();
            }
            catch (Exception e)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Exception calling InPlaceDeactivate: " + e.ToString());
            }
        }

        private void UiActivate()
        {
            Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "calling uiActivate for " + ToString());
            Debug.Assert(GetOcState() >= OC_INPLACE, "we have to be in place in order to ui activate...");
            Debug.Assert(CanUIActivate, "we have to be able to uiactivate");
            if (CanUIActivate)
            {
                DoVerb(OLEIVERB_UIACTIVATE);
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
                    ((UnsafeNativeMethods.IOleClientSite)oleSite).ShowObject();
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
            if (GetState(STATE_VISIBLE) != value)
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
                    SetState(STATE_VISIBLE, value);
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
                DoVerb(OLEIVERB_SHOW);
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

            DoVerb(OLEIVERB_HIDE);
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
        public override bool PreProcessMessage(ref Message msg)
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

                NativeMethods.MSG win32Message = new NativeMethods.MSG
                {
                    message = msg.Msg,
                    wParam = msg.WParam,
                    lParam = msg.LParam,
                    hwnd = msg.HWnd
                };

                axState[siteProcessedInputKey] = false;
                try
                {
                    UnsafeNativeMethods.IOleInPlaceActiveObject activeObj = GetInPlaceActiveObject();
                    if (activeObj != null)
                    {
                        int hr = activeObj.TranslateAccelerator(ref win32Message);

                        msg.Msg = win32Message.message;
                        msg.WParam = win32Message.wParam;
                        msg.LParam = win32Message.lParam;
                        msg.HWnd = win32Message.hwnd;

                        if (hr == NativeMethods.S_OK)
                        {
                            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "\t Message translated by control to " + msg);
                            return true;
                        }
                        else if (hr == NativeMethods.S_FALSE)
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
        protected internal override bool ProcessMnemonic(char charCode)
        {
            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "In AxHost.ProcessMnemonic: " + (int)charCode);
            if (CanSelect)
            {
                try
                {
                    NativeMethods.tagCONTROLINFO ctlInfo = new NativeMethods.tagCONTROLINFO();
                    int hr = GetOleControl().GetControlInfo(ctlInfo);
                    if (NativeMethods.Failed(hr))
                    {
                        return false;
                    }
                    NativeMethods.MSG msg = new NativeMethods.MSG
                    {
                        // Sadly, we don't have a message so we must fake one ourselves...
                        // A bit of ugliness here (a bit?  more like a bucket...)
                        // The message we are faking is a WM_SYSKEYDOWN w/ the right alt key setting...
                        hwnd = (ContainingControl == null) ? IntPtr.Zero : ContainingControl.Handle,
                        message = WindowMessages.WM_SYSKEYDOWN,
                        wParam = (IntPtr)char.ToUpper(charCode, CultureInfo.CurrentCulture),
                        lParam = (IntPtr)0x20180001,
                        time = SafeNativeMethods.GetTickCount()
                    };
                    UnsafeNativeMethods.GetCursorPos(out Point p);
                    msg.pt = p;
                    if (SafeNativeMethods.IsAccelerator(new HandleRef(ctlInfo, ctlInfo.hAccel), ctlInfo.cAccel, ref msg, null))
                    {
                        GetOleControl().OnMnemonic(ref msg);
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
        [
        DefaultValue(null),
        RefreshProperties(RefreshProperties.All),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public State OcxState
        {
            get
            {
                if (IsDirty() || ocxState == null)
                {
                    Debug.Assert(!axState[disposed], "we chould not be asking for the object when we are axState[disposed]...");
                    ocxState = CreateNewOcxState(ocxState);
                }
                return ocxState;
            }

            set
            {
                axState[ocxStateSet] = true;

                if (value == null)
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
        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public ContainerControl ContainingControl
        {
            get
            {
                if (containingControl == null)
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
            return site == null || !site.DesignMode;
        }

        private object GetAmbientProperty(int dispid)
        {
            Control richParent = ParentInternal;

            switch (dispid)
            {
                case NativeMethods.ActiveX.DISPID_AMBIENT_USERMODE:
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "asked for usermode");
                    return IsUserMode();
                case NativeMethods.ActiveX.DISPID_AMBIENT_AUTOCLIP:
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "asked for autoclip");
                    return true;
                case NativeMethods.ActiveX.DISPID_AMBIENT_MESSAGEREFLECT:
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "asked for message reflect");
                    return true;
                case NativeMethods.ActiveX.DISPID_AMBIENT_UIDEAD:
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "asked for uidead");
                    return false;
                case NativeMethods.ActiveX.DISPID_AMBIENT_DISPLAYASDEFAULT:
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "asked for displayasdefault");
                    return false;
                case NativeMethods.ActiveX.DISPID_AMBIENT_FONT:
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "asked for font");
                    if (richParent != null)
                    {
                        return GetIFontFromFont(richParent.Font);
                    }
                    return null;
                case NativeMethods.ActiveX.DISPID_AMBIENT_SHOWGRABHANDLES:
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "asked for showGrabHandles");
                    return false;
                case NativeMethods.ActiveX.DISPID_AMBIENT_SHOWHATCHING:
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "asked for showHatching");
                    return false;
                case NativeMethods.ActiveX.DISPID_AMBIENT_BACKCOLOR:
                    if (richParent != null)
                    {
                        return GetOleColorFromColor(richParent.BackColor);
                    }
                    return null;
                case NativeMethods.ActiveX.DISPID_AMBIENT_FORECOLOR:
                    if (richParent != null)
                    {
                        return GetOleColorFromColor(richParent.ForeColor);
                    }
                    return null;
                case NativeMethods.ActiveX.DISPID_AMBIENT_DISPLAYNAME:
                    string rval = GetParentContainer().GetNameForControl(this);
                    if (rval == null)
                    {
                        rval = string.Empty;
                    }

                    return rval;
                case NativeMethods.ActiveX.DISPID_AMBIENT_LOCALEID:
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "asked for localeid");
                    return Thread.CurrentThread.CurrentCulture.LCID;
                case NativeMethods.ActiveX.DISPID_AMBIENT_RIGHTTOLEFT:
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

        public void DoVerb(int verb)
        {
            Control parent = ParentInternal;
            GetOleObject().DoVerb(verb, IntPtr.Zero, oleSite, -1, parent != null ? parent.Handle : IntPtr.Zero, FillInRect(new NativeMethods.COMRECT(), Bounds));
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
                try
                {
                    GetOleControl().FreezeEvents(-1);
                }
                catch (COMException t)
                {
                    Debug.Fail(t.ToString());
                }
                freezeCount++;
            }
            else
            {
                try
                {
                    GetOleControl().FreezeEvents(0);
                }
                catch (COMException t)
                {
                    Debug.Fail(t.ToString());
                }
                freezeCount--;
            }
            Debug.Assert(freezeCount >= 0, "invalid freeze count!");
        }

        private int UiDeactivate()
        {
            Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "calling uiDeactivate for " + ToString());
            bool ownDispose = axState[ownDisposing];
            axState[ownDisposing] = true;
            int hr = 0;
            try
            {
                hr = GetInPlaceObject().UIDeactivate();
            }
            finally
            {
                axState[ownDisposing] = ownDispose;
            }
            return hr;
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

        private string GetLicenseKey(Guid clsid)
        {
            if (licenseKey != null || !axState[needLicenseKey])
            {
                return licenseKey;
            }

            try
            {
                UnsafeNativeMethods.IClassFactory2 icf2 = UnsafeNativeMethods.CoGetClassObject(ref clsid, INPROC_SERVER, 0, ref icf2_Guid);
                NativeMethods.tagLICINFO licInfo = new NativeMethods.tagLICINFO();
                icf2.GetLicInfo(licInfo);
                if (licInfo.fRuntimeAvailable != 0)
                {
                    string[] rval = new string[1];
                    icf2.RequestLicKey(0, rval);
                    licenseKey = rval[0];
                    return licenseKey;
                }
            }
            catch (COMException e)
            {
                if (e.ErrorCode == E_NOINTERFACE.ErrorCode)
                {
                    return null;
                }

                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Failed to get the license key: " + e.ToString());
                axState[needLicenseKey] = false;
            }
            catch (Exception t)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Failed to get the license key: " + t.ToString());
                axState[needLicenseKey] = false;
            }
            return null;
        }

        private void CreateWithoutLicense(Guid clsid)
        {
            object ret = null;
            Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Creating object without license: " + clsid.ToString());
            ret = UnsafeNativeMethods.CoCreateInstance(ref clsid, null, INPROC_SERVER, ref NativeMethods.ActiveX.IID_IUnknown);
            Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "\t" + (ret != null).ToString());
            instance = ret;
        }

        private void CreateWithLicense(string license, Guid clsid)
        {
            if (license != null)
            {
                try
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Creating object with license: " + clsid.ToString());
                    UnsafeNativeMethods.IClassFactory2 icf2 = UnsafeNativeMethods.CoGetClassObject(ref clsid, INPROC_SERVER, 0, ref icf2_Guid);

                    if (icf2 != null)
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "\tClassFactory" + (icf2 != null).ToString());
                        icf2.CreateInstanceLic(null, null, ref NativeMethods.ActiveX.IID_IUnknown, license, out instance);
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "\t" + (instance != null).ToString());
                    }
                }
                catch (Exception t)
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Failed to create with license: " + t.ToString());
                }
            }

            if (instance == null)
            {
                CreateWithoutLicense(clsid);
            }
        }

        private void CreateInstance()
        {
            Debug.Assert(instance == null, "instance must be null");
            //Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "before created "+Windows.GetCurrentThreadId());
            //Debug.WriteStackTraceIf("AxHTrace");
            //checkThreadingModel();
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

        private CategoryAttribute GetCategoryForDispid(int dispid)
        {
            NativeMethods.ICategorizeProperties icp = GetCategorizeProperties();
            if (icp == null)
            {
                return null;
            }

            CategoryAttribute rval = null;
            int propcat = 0;
            try
            {
                icp.MapPropertyToCategory(dispid, ref propcat);
                if (propcat != 0)
                {
                    int cat = -propcat;
                    if (cat > 0 && cat < categoryNames.Length && categoryNames[cat] != null)
                    {
                        return categoryNames[cat];
                    }
                    cat = -cat;
                    int key = cat;
                    if (objectDefinedCategoryNames != null)
                    {
                        rval = (CategoryAttribute)objectDefinedCategoryNames[key];
                        if (rval != null)
                        {
                            return rval;
                        }
                    }

                    int hr = icp.GetCategoryName(cat, CultureInfo.CurrentCulture.LCID, out string name);
                    if (hr == NativeMethods.S_OK && name != null)
                    {
                        rval = new CategoryAttribute(name);
                        if (objectDefinedCategoryNames == null)
                        {
                            objectDefinedCategoryNames = new Hashtable();
                        }
                        objectDefinedCategoryNames.Add(key, rval);
                        return rval;
                    }
                }
            }
            catch (Exception t)
            {
                Debug.Fail(t.ToString());
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
            IntPtr hwndFocus = UnsafeNativeMethods.GetFocus();
            try
            {
                UiActivate();
            }
            catch (Exception t)
            {
                Debug.Fail(t.ToString());
            }
            // It so happens that some controls don't get focus in this case, so
            // we have got to force it onto them...
            //         int hwndFocusNow = NativeMethods.GetFocus();
            //         Windows.SetFocus(getHandle());
            //         while (hwndFocusNow != getHandle()) {
            //             if (hwndFocusNow == 0) {
            //                 break;
            //             }
            //             hwndFocusNow = Windows.GetParent(hwndFocusNow);
            //         }
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

            if (editor == null && HasPropertyPages())
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
                if (attributes == null && attribsStash == null)
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

            if (properties == null)
            {
                properties = new Hashtable();
            }

            if (propertyInfos == null)
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
                        if ((propInfo == null && axPropDesc != null) || (propInfo != null && axPropDesc == null))
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
                                    if (removeList == null)
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

        private AxPropertyDescriptor GetPropertyDescriptorFromDispid(int dispid)
        {
            Debug.Assert(dispid != NativeMethods.ActiveX.DISPID_UNKNOWN, "Wrong dispid sent to GetPropertyDescriptorFromDispid");

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

        private void DepersistFromIPropertyBag(UnsafeNativeMethods.IPropertyBag propBag)
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

            if (ocxState == null)
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
                if (instance is UnsafeNativeMethods.IPersistPropertyBag)
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, this + " supports IPersistPropertyBag.");
                    iPersistPropBag = (UnsafeNativeMethods.IPersistPropertyBag)instance;
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
                    iPersistPropBag = (UnsafeNativeMethods.IPersistPropertyBag)instance;
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
            if (instance == null)
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

            return (GetOcx() is NativeMethods.ISpecifyPropertyPages);
        }

        public bool HasPropertyPages()
        {
            if (!CanShowPropertyPages())
            {
                return false;
            }

            NativeMethods.ISpecifyPropertyPages ispp = (NativeMethods.ISpecifyPropertyPages)GetOcx();
            try
            {
                NativeMethods.tagCAUUID uuids = new NativeMethods.tagCAUUID();
                try
                {
                    ispp.GetPages(uuids);
                    if (uuids.cElems > 0)
                    {
                        return true;
                    }
                }
                finally
                {
                    if (uuids.pElems != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(uuids.pElems);
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        unsafe private void ShowPropertyPageForDispid(int dispid, Guid guid)
        {
            try
            {
                IntPtr pUnk = Marshal.GetIUnknownForObject(GetOcx());
                NativeMethods.OCPFIPARAMS opcparams = new NativeMethods.OCPFIPARAMS
                {
                    hwndOwner = (ContainingControl == null) ? IntPtr.Zero : ContainingControl.Handle,
                    lpszCaption = Name,
                    ppUnk = (IntPtr)(long)&pUnk,
                    uuid = (IntPtr)(long)&guid,
                    dispidInitial = dispid
                };
                UnsafeNativeMethods.OleCreatePropertyFrameIndirect(opcparams);
            }
            catch (Exception t)
            {
                Debug.Fail(t.ToString());
                throw t;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void MakeDirty()
        {
            ISite isite = Site;
            if (isite == null)
            {
                return;
            }

            IComponentChangeService ccs = (IComponentChangeService)isite.GetService(typeof(IComponentChangeService));
            if (ccs == null)
            {
                return;
            }

            ccs.OnComponentChanging(this, null);

            ccs.OnComponentChanged(this, null, null, null);
        }

        public void ShowPropertyPages()
        {
            if (ParentInternal == null)
            {
                return;
            }

            if (!ParentInternal.IsHandleCreated)
            {
                return;
            }

            ShowPropertyPages(ParentInternal);
        }

        public void ShowPropertyPages(Control control)
        {
            try
            {
                if (!CanShowPropertyPages())
                {
                    return;
                }

                NativeMethods.ISpecifyPropertyPages ispp = (NativeMethods.ISpecifyPropertyPages)GetOcx();
                NativeMethods.tagCAUUID uuids = new NativeMethods.tagCAUUID();
                try
                {
                    ispp.GetPages(uuids);
                    if (uuids.cElems <= 0)
                    {
                        return;
                    }
                }
                catch
                {
                    return;
                }

                // State oldOcxState = OcxState;

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

                    string name = null;
                    object o = GetOcx();
                    IntPtr handle = (ContainingControl == null) ? IntPtr.Zero : ContainingControl.Handle;
                    SafeNativeMethods.OleCreatePropertyFrame(new HandleRef(this, handle), 0, 0, name, 1, ref o, uuids.cElems, new HandleRef(null, uuids.pElems), Application.CurrentCulture.LCID, 0, IntPtr.Zero);
                }
                finally
                {
                    if (oleSite != null)
                    {
                        ((UnsafeNativeMethods.IPropertyNotifySink)oleSite).OnChanged(NativeMethods.MEMBERID_NIL);
                    }

                    if (trans != null)
                    {
                        trans.Commit();
                    }

                    if (uuids.pElems != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(uuids.pElems);
                    }
                }
            }
            catch (Exception t)
            {
                Debug.Fail(t.ToString());
                throw t;
            }
        }

        internal override IntPtr InitializeDCForWmCtlColor(IntPtr dc, int msg)
        {
            if (isMaskEdit)
            {
                return base.InitializeDCForWmCtlColor(dc, msg);
            }
            else
            {
                return IntPtr.Zero; // bypass Control's anti-reflect logic
            }
        }

        /// <summary>
        ///  AxHost wndProc. All messages are sent to wndProc after getting filtered
        ///  through the preProcessMessage function.
        ///  Certain messages are forwarder directly to the ActiveX control,
        ///  others are first processed by the wndProc of Control
        /// </summary>
        protected override void WndProc(ref Message m)
        {
#pragma warning disable 162, 429
            // Ignore the warnings generated by the following code (unreachable code, and unreachable expression)
            if (false && (axState[manualUpdate] && IsUserMode()))
            {
                DefWndProc(ref m);
                return;
            }
#pragma warning restore 162, 429

            switch (m.Msg)
            {
                // Things we explicitly ignore and pass to the ocx's windproc
                case WindowMessages.WM_ERASEBKGND:

                case WindowMessages.WM_REFLECT + WindowMessages.WM_NOTIFYFORMAT:

                case WindowMessages.WM_SETCURSOR:
                case WindowMessages.WM_SYSCOLORCHANGE:

                // Some of the MSComCtl controls respond to this message
                // to do some custom painting. So, we should just pass this message
                // through.
                //
                case WindowMessages.WM_DRAWITEM:

                case WindowMessages.WM_LBUTTONDBLCLK:
                case WindowMessages.WM_LBUTTONUP:
                case WindowMessages.WM_MBUTTONDBLCLK:
                case WindowMessages.WM_MBUTTONUP:
                case WindowMessages.WM_RBUTTONDBLCLK:
                case WindowMessages.WM_RBUTTONUP:
                    DefWndProc(ref m);
                    break;

                case WindowMessages.WM_LBUTTONDOWN:
                case WindowMessages.WM_MBUTTONDOWN:
                case WindowMessages.WM_RBUTTONDOWN:
                    if (IsUserMode())
                    {
                        Focus();
                    }
                    DefWndProc(ref m);
                    break;

                case WindowMessages.WM_KILLFOCUS:
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

                case WindowMessages.WM_COMMAND:
                    if (!ReflectMessage(m.LParam, ref m))
                    {
                        DefWndProc(ref m);
                    }
                    break;

                case WindowMessages.WM_CONTEXTMENU:
                    DefWndProc(ref m);
                    break;

                case WindowMessages.WM_DESTROY:
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
                        UnsafeNativeMethods.IOleInPlaceObject ipo = GetInPlaceObject();
                        if (NativeMethods.Succeeded(ipo.GetWindow(out IntPtr hwnd)))
                        {
                            Application.ParkHandle(new HandleRef(ipo, hwnd));
                        }
                    }

                    bool visible = GetState(STATE_VISIBLE);

                    TransitionDownTo(OC_RUNNING);
                    DetachAndForward(ref m);

                    if (visible != GetState(STATE_VISIBLE))
                    {
                        SetState(STATE_VISIBLE, visible);
                    }

                    break;
                case WindowMessages.WM_HELP:
                    // We want to both fire the event, and let the ocx have the message...
                    base.WndProc(ref m);
                    DefWndProc(ref m);
                    break;

                case WindowMessages.WM_KEYUP:
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

                case WindowMessages.WM_NCDESTROY:
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
                    if (m.Msg == REGMSG_MSG)
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
                IntPtr wndProc = UnsafeNativeMethods.GetWindowLong(new HandleRef(this, handle), NativeMethods.GWL_WNDPROC);
                m.Result = UnsafeNativeMethods.CallWindowProc(wndProc, handle, m.Msg, m.WParam, m.LParam);
            }
        }

        private void DetachWindow()
        {
            if (IsHandleCreated)
            {
                OnHandleDestroyed(EventArgs.Empty);
                for (Control c = this; c != null; c = c.ParentInternal)
                {
                    /* NOT NEEDED
                    if (c.GetAxState(STATE_HANDLEHOOK)) {
                        ((IHandleHook)c.Site.GetService(IHandleHook.class)).OnDestroyHandle(GetHandle());
                        break;
                    }
                    */
                }
                WindowReleaseHandle();
            }
        }

        private void InformOfNewHandle()
        {
            Debug.Assert(IsHandleCreated, "we got to have a handle to be here...");
            for (Control c = this; c != null; c = c.ParentInternal)
            {
                /* NOT NEEDED
                if (c.GetAxState(STATE_HANDLEHOOK)) {
                    ((IHandleHook)c.Site.GetService(IHandleHook.class)).OnCreateHandle(GetHandle());
                    break;
                }
                */
            }
            wndprocAddr = UnsafeNativeMethods.GetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_WNDPROC);
            /* NOT NEEDED
            SetAxState(STATE_CREATENOTIFIED, true);
            */
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

        [AttributeUsage(AttributeTargets.Class, Inherited = false)]
        public sealed class ClsidAttribute : Attribute
        {
            private readonly string val;

            public ClsidAttribute(string clsid)
            {
                val = clsid;
            }

            public string Value
            {
                get
                {
                    return val;
                }
            }
        }

        [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
        public sealed class TypeLibraryTimeStampAttribute : Attribute
        {
            private readonly DateTime val;

            public TypeLibraryTimeStampAttribute(string timestamp)
            {
                val = DateTime.Parse(timestamp, CultureInfo.InvariantCulture);
            }

            public DateTime Value
            {
                get
                {
                    return val;
                }
            }
        }

        public class ConnectionPointCookie
        {
            private UnsafeNativeMethods.IConnectionPoint connectionPoint;
            private int cookie;
            internal int threadId;
#if DEBUG
            private readonly string callStack;
#endif
            /// <summary>
            ///  Creates a connection point to of the given interface type.
            ///  which will call on a managed code sink that implements that interface.
            /// </summary>
            public ConnectionPointCookie(object source, object sink, Type eventInterface)
                : this(source, sink, eventInterface, true)
            {
            }

            internal ConnectionPointCookie(object source, object sink, Type eventInterface, bool throwException)
            {
                if (source is UnsafeNativeMethods.IConnectionPointContainer cpc)
                {
                    try
                    {
                        Guid tmp = eventInterface.GUID;
                        if (cpc.FindConnectionPoint(ref tmp, out connectionPoint) != NativeMethods.S_OK)
                        {
                            connectionPoint = null;
                        }
                    }
                    catch
                    {
                        connectionPoint = null;
                    }

                    if (connectionPoint == null)
                    {
                        if (throwException)
                        {
                            throw new ArgumentException(string.Format(SR.AXNoEventInterface, eventInterface.Name));
                        }
                    }
                    else if (sink == null || !eventInterface.IsInstanceOfType(sink))
                    {
                        if (throwException)
                        {
                            throw new InvalidCastException(string.Format(SR.AXNoSinkImplementation, eventInterface.Name));
                        }
                    }
                    else
                    {
                        int hr = connectionPoint.Advise(sink, ref cookie);
                        if (hr == NativeMethods.S_OK)
                        {
                            threadId = Thread.CurrentThread.ManagedThreadId;
                        }
                        else
                        {
                            cookie = 0;
                            Marshal.ReleaseComObject(connectionPoint);
                            connectionPoint = null;
                            if (throwException)
                            {
                                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, string.Format(SR.AXNoSinkAdvise, eventInterface.Name), hr));
                            }
                        }
                    }
                }
                else
                {
                    if (throwException)
                    {
                        throw new InvalidCastException(SR.AXNoConnectionPointContainer);
                    }
                }

                if (connectionPoint == null || cookie == 0)
                {
                    if (connectionPoint != null)
                    {
                        Marshal.ReleaseComObject(connectionPoint);
                    }

                    if (throwException)
                    {
                        throw new ArgumentException(string.Format(SR.AXNoConnectionPoint, eventInterface.Name));
                    }
                }
#if DEBUG
                callStack = Environment.StackTrace;
#endif
            }

            /// <summary>
            ///  Disconnect the current connection point.  If the object is not connected,
            ///  this method will do nothing.
            /// </summary>
            public void Disconnect()
            {
                if (connectionPoint != null && cookie != 0)
                {
                    try
                    {
                        connectionPoint.Unadvise(cookie);
                    }
                    catch (Exception ex)
                    {
                        if (ClientUtils.IsCriticalException(ex))
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        cookie = 0;
                    }

                    try
                    {
                        Marshal.ReleaseComObject(connectionPoint);
                    }
                    catch (Exception ex)
                    {
                        if (ClientUtils.IsCriticalException(ex))
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        connectionPoint = null;
                    }
                }
            }

            ~ConnectionPointCookie()
            {
                if (connectionPoint != null && cookie != 0)
                {
                    if (!AppDomain.CurrentDomain.IsFinalizingForUnload())
                    {
                        SynchronizationContext context = SynchronizationContext.Current;
                        if (context == null)
                        {
                            Debug.Fail("Attempted to disconnect ConnectionPointCookie from the finalizer with no SynchronizationContext.");
                        }
                        else
                        {
                            context.Post(new SendOrPostCallback(AttemptDisconnect), null);
                        }
                    }
                }
            }

            void AttemptDisconnect(object trash)
            {
                if (threadId == Thread.CurrentThread.ManagedThreadId)
                {
                    Disconnect();
                }
                else
                {
                    Debug.Fail("Attempted to disconnect ConnectionPointCookie from the wrong thread (finalizer).");
                }
            }

            internal bool Connected
            {
                get
                {
                    return connectionPoint != null && cookie != 0;
                }
            }
        }

        public enum ActiveXInvokeKind
        {
            MethodInvoke,
            PropertyGet,
            PropertySet
        }

        public class InvalidActiveXStateException : Exception
        {
            private readonly string name;
            private readonly ActiveXInvokeKind kind;

            public InvalidActiveXStateException(string name, ActiveXInvokeKind kind)
            {
                this.name = name;
                this.kind = kind;
            }

            public InvalidActiveXStateException()
            {
            }

            public override string ToString()
            {
                switch (kind)
                {
                    case ActiveXInvokeKind.MethodInvoke:
                        return string.Format(SR.AXInvalidMethodInvoke, name);
                    case ActiveXInvokeKind.PropertyGet:
                        return string.Format(SR.AXInvalidPropertyGet, name);
                    case ActiveXInvokeKind.PropertySet:
                        return string.Format(SR.AXInvalidPropertySet, name);
                    default:
                        return base.ToString();
                }
            }
        }

        // This private class encapsulates all of the ole interfaces so that users
        // will not be able to access and call them directly...

        private class OleInterfaces
            : UnsafeNativeMethods.IOleControlSite, UnsafeNativeMethods.IOleClientSite, UnsafeNativeMethods.IOleInPlaceSite, UnsafeNativeMethods.ISimpleFrameSite, UnsafeNativeMethods.IVBGetControl, UnsafeNativeMethods.IGetVBAObject, UnsafeNativeMethods.IPropertyNotifySink, IReflect, IDisposable
        {
            private readonly AxHost host;
            private ConnectionPointCookie connectionPoint;

            internal OleInterfaces(AxHost host)
            {
                this.host = host ?? throw new ArgumentNullException(nameof(host));
            }

            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (!AppDomain.CurrentDomain.IsFinalizingForUnload())
                    {
                        SynchronizationContext context = SynchronizationContext.Current;
                        if (context == null)
                        {
                            Debug.Fail("Attempted to disconnect ConnectionPointCookie from the finalizer with no SynchronizationContext.");
                        }
                        else
                        {
                            context.Post(new SendOrPostCallback(AttemptStopEvents), null);
                        }
                    }
                }
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            internal AxHost GetAxHost()
            {
                return host;
            }

            internal void OnOcxCreate()
            {
                StartEvents();
            }

            internal void StartEvents()
            {
                if (connectionPoint != null)
                {
                    return;
                }

                object nativeObject = host.GetOcx();

                try
                {
                    connectionPoint = new ConnectionPointCookie(nativeObject, this, typeof(UnsafeNativeMethods.IPropertyNotifySink));
                }
                catch
                {
                }
            }

            void AttemptStopEvents(object trash)
            {
                if (connectionPoint == null)
                {
                    return;
                }

                if (connectionPoint.threadId == Thread.CurrentThread.ManagedThreadId)
                {
                    StopEvents();
                }
                else
                {
                    Debug.Fail("Attempted to disconnect ConnectionPointCookie from the wrong thread (finalizer).");
                }
            }

            internal void StopEvents()
            {
                if (connectionPoint != null)
                {
                    connectionPoint.Disconnect();
                    connectionPoint = null;
                }
            }

            // IGetVBAObject methods:

            int UnsafeNativeMethods.IGetVBAObject.GetObject(ref Guid riid, UnsafeNativeMethods.IVBFormat[] rval, int dwReserved)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in GetObject");

                if (rval == null || riid.Equals(Guid.Empty))
                {
                    return NativeMethods.E_INVALIDARG;
                }

                if (riid.Equals(ivbformat_Guid))
                {
                    rval[0] = new VBFormat();
                    return NativeMethods.S_OK;
                }
                else
                {
                    rval[0] = null;
                    return NativeMethods.E_NOINTERFACE;
                }
            }

            // IVBGetControl methods:

            int UnsafeNativeMethods.IVBGetControl.EnumControls(int dwOleContF, int dwWhich, out UnsafeNativeMethods.IEnumUnknown ppenum)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in EnumControls");
                ppenum = null;
                ppenum = host.GetParentContainer().EnumControls(host, dwOleContF, dwWhich);
                return NativeMethods.S_OK;
            }

            // ISimpleFrameSite methods:

            int UnsafeNativeMethods.ISimpleFrameSite.PreMessageFilter(IntPtr hwnd, int msg, IntPtr wp, IntPtr lp, ref IntPtr plResult, ref int pdwCookie)
            {
                return NativeMethods.S_OK;
            }

            int UnsafeNativeMethods.ISimpleFrameSite.PostMessageFilter(IntPtr hwnd, int msg, IntPtr wp, IntPtr lp, ref IntPtr plResult, int dwCookie)
            {
                return NativeMethods.S_FALSE;
            }

            // IReflect methods:

            MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
            {
                return null;
            }

            MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr)
            {
                return null;
            }

            MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr)
            {
                return Array.Empty<MethodInfo>();
            }

            FieldInfo IReflect.GetField(string name, BindingFlags bindingAttr)
            {
                return null;
            }

            FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr)
            {
                return Array.Empty<FieldInfo>();
            }

            PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr)
            {
                return null;
            }

            PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
            {
                return null;
            }

            PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr)
            {
                return Array.Empty<PropertyInfo>();
            }

            MemberInfo[] IReflect.GetMember(string name, BindingFlags bindingAttr)
            {
                return Array.Empty<MemberInfo>();
            }

            MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr)
            {
                return Array.Empty<MemberInfo>();
            }

            object IReflect.InvokeMember(string name, BindingFlags invokeAttr, Binder binder,
                                                    object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
            {
                if (name.StartsWith("[DISPID="))
                {
                    int endIndex = name.IndexOf(']');
                    int dispid = int.Parse(name.Substring(8, endIndex - 8), CultureInfo.InvariantCulture);
                    object ambient = host.GetAmbientProperty(dispid);
                    if (ambient != null)
                    {
                        return ambient;
                    }
                }

                throw E_FAIL;
            }

            Type IReflect.UnderlyingSystemType
            {
                get
                {
                    return null;
                }
            }

            // IOleControlSite methods:

            int UnsafeNativeMethods.IOleControlSite.OnControlInfoChanged()
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in OnControlInfoChanged");
                return NativeMethods.S_OK;
            }

            int UnsafeNativeMethods.IOleControlSite.LockInPlaceActive(int fLock)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in LockInPlaceActive");
                return NativeMethods.E_NOTIMPL;
            }

            int UnsafeNativeMethods.IOleControlSite.GetExtendedControl(out object ppDisp)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in GetExtendedControl " + host.ToString());
                ppDisp = host.GetParentContainer().GetProxyForControl(host);
                if (ppDisp == null)
                {
                    return NativeMethods.E_NOTIMPL;
                }

                return NativeMethods.S_OK;
            }

            unsafe HRESULT UnsafeNativeMethods.IOleControlSite.TransformCoords(Point* pPtlHimetric, PointF* pPtfContainer, uint dwFlags)
            {
                if (pPtlHimetric == null || pPtfContainer == null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                HRESULT hr = SetupLogPixels(false);
                if (hr < 0)
                {
                    return hr;
                }

                if ((dwFlags & NativeMethods.ActiveX.XFORMCOORDS_HIMETRICTOCONTAINER) != 0)
                {
                    if ((dwFlags & NativeMethods.ActiveX.XFORMCOORDS_SIZE) != 0)
                    {
                        pPtfContainer->X = (float)host.HM2Pix(pPtlHimetric->X, logPixelsX);
                        pPtfContainer->Y = (float)host.HM2Pix(pPtlHimetric->Y, logPixelsY);
                    }
                    else if ((dwFlags & NativeMethods.ActiveX.XFORMCOORDS_POSITION) != 0)
                    {
                        pPtfContainer->X = (float)host.HM2Pix(pPtlHimetric->X, logPixelsX);
                        pPtfContainer->Y = (float)host.HM2Pix(pPtlHimetric->Y, logPixelsY);
                    }
                    else
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "\t dwFlags not supported: " + dwFlags);
                        return HRESULT.E_INVALIDARG;
                    }
                }
                else if ((dwFlags & NativeMethods.ActiveX.XFORMCOORDS_CONTAINERTOHIMETRIC) != 0)
                {
                    if ((dwFlags & NativeMethods.ActiveX.XFORMCOORDS_SIZE) != 0)
                    {
                        pPtlHimetric->X = host.Pix2HM((int)pPtfContainer->X, logPixelsX);
                        pPtlHimetric->Y = host.Pix2HM((int)pPtfContainer->Y, logPixelsY);
                    }
                    else if ((dwFlags & NativeMethods.ActiveX.XFORMCOORDS_POSITION) != 0)
                    {
                        pPtlHimetric->X = host.Pix2HM((int)pPtfContainer->X, logPixelsX);
                        pPtlHimetric->Y = host.Pix2HM((int)pPtfContainer->Y, logPixelsY);
                    }
                    else
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "\t dwFlags not supported: " + dwFlags);
                        return HRESULT.E_INVALIDARG;
                    }
                }
                else
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "\t dwFlags not supported: " + dwFlags);
                    return HRESULT.E_INVALIDARG;
                }

                return HRESULT.S_OK;
            }

            int UnsafeNativeMethods.IOleControlSite.TranslateAccelerator(ref NativeMethods.MSG pMsg, int grfModifiers)
            {
                Debug.Assert(!host.GetAxState(AxHost.siteProcessedInputKey), "Re-entering UnsafeNativeMethods.IOleControlSite.TranslateAccelerator!!!");
                host.SetAxState(AxHost.siteProcessedInputKey, true);

                Message msg = new Message
                {
                    Msg = pMsg.message,
                    WParam = pMsg.wParam,
                    LParam = pMsg.lParam,
                    HWnd = pMsg.hwnd
                };

                try
                {
                    bool f = ((Control)host).PreProcessMessage(ref msg);
                    return f ? NativeMethods.S_OK : NativeMethods.S_FALSE;
                }
                finally
                {
                    host.SetAxState(AxHost.siteProcessedInputKey, false);
                }
            }

            int UnsafeNativeMethods.IOleControlSite.OnFocus(int fGotFocus)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in OnFocus " + ((fGotFocus == 0) ? "lost" : "gained"));
                return NativeMethods.S_OK;
            }

            int UnsafeNativeMethods.IOleControlSite.ShowPropertyFrame()
            {
                if (host.CanShowPropertyPages())
                {
                    host.ShowPropertyPages();
                    return NativeMethods.S_OK;
                }
                return NativeMethods.E_NOTIMPL;
            }

            // IOleClientSite methods:
            int UnsafeNativeMethods.IOleClientSite.SaveObject()
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in SaveObject");
                return NativeMethods.E_NOTIMPL;
            }

            int UnsafeNativeMethods.IOleClientSite.GetMoniker(int dwAssign, int dwWhichMoniker, out object moniker)
            {
                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetMoniker");
                moniker = null;
                return NativeMethods.E_NOTIMPL;
            }

            int UnsafeNativeMethods.IOleClientSite.GetContainer(out UnsafeNativeMethods.IOleContainer container)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in getContainer");
                container = host.GetParentContainer();
                return NativeMethods.S_OK;
            }

            int UnsafeNativeMethods.IOleClientSite.ShowObject()
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in ShowObject");
                if (host.GetAxState(AxHost.fOwnWindow))
                {
                    Debug.Fail("we can't be in showobject if we own our window...");
                    return NativeMethods.S_OK;
                }
                if (host.GetAxState(AxHost.fFakingWindow))
                {
                    // we really should not be here...
                    // this means that the ctl inplace deactivated and didn't call on inplace activate before calling showobject
                    // so we need to destroy our fake window first...
                    host.DestroyFakeWindow();

                    // The fact that we have a fake window means that the OCX inplace deactivated when we hid it. It means
                    // that we have to bring it back from RUNNING to INPLACE so that it can re-create its handle properly.
                    //
                    host.TransitionDownTo(OC_LOADED);
                    host.TransitionUpTo(OC_INPLACE);
                }
                if (host.GetOcState() < OC_INPLACE)
                {
                    return NativeMethods.S_OK;
                }

                if (NativeMethods.Succeeded(host.GetInPlaceObject().GetWindow(out IntPtr hwnd)))
                {
                    if (host.GetHandleNoCreate() != hwnd)
                    {
                        host.DetachWindow();
                        if (hwnd != IntPtr.Zero)
                        {
                            host.AttachWindow(hwnd);
                        }
                    }
                }
                else if (host.GetInPlaceObject() is UnsafeNativeMethods.IOleInPlaceObjectWindowless)
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Windowless control.");
                    throw new InvalidOperationException(SR.AXWindowlessControl);
                }

                return NativeMethods.S_OK;
            }

            int UnsafeNativeMethods.IOleClientSite.OnShowWindow(int fShow)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in OnShowWindow");
                return NativeMethods.S_OK;
            }

            int UnsafeNativeMethods.IOleClientSite.RequestNewObjectLayout()
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in RequestNewObjectLayout");
                return NativeMethods.E_NOTIMPL;
            }

            // IOleInPlaceSite methods:

            IntPtr UnsafeNativeMethods.IOleInPlaceSite.GetWindow()
            {
                try
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in GetWindow");
                    Control parent = host.ParentInternal;
                    return parent != null ? parent.Handle : IntPtr.Zero;
                }
                catch (Exception t)
                {
                    Debug.Fail(t.ToString());
                    throw t;
                }
            }

            int UnsafeNativeMethods.IOleInPlaceSite.ContextSensitiveHelp(int fEnterMode)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in ContextSensitiveHelp");
                return NativeMethods.E_NOTIMPL;
            }

            int UnsafeNativeMethods.IOleInPlaceSite.CanInPlaceActivate()
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in CanInPlaceActivate");
                return NativeMethods.S_OK;
            }

            int UnsafeNativeMethods.IOleInPlaceSite.OnInPlaceActivate()
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in OnInPlaceActivate");
                host.SetAxState(AxHost.ownDisposing, false);
                host.SetAxState(AxHost.rejectSelection, false);
                host.SetOcState(OC_INPLACE);
                return NativeMethods.S_OK;
            }

            int UnsafeNativeMethods.IOleInPlaceSite.OnUIActivate()
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in OnUIActivate for " + host.ToString());
                host.SetOcState(OC_UIACTIVE);
                host.GetParentContainer().OnUIActivate(host);
                return NativeMethods.S_OK;
            }

            int UnsafeNativeMethods.IOleInPlaceSite.GetWindowContext(out UnsafeNativeMethods.IOleInPlaceFrame ppFrame, out UnsafeNativeMethods.IOleInPlaceUIWindow ppDoc,
                                                 NativeMethods.COMRECT lprcPosRect, NativeMethods.COMRECT lprcClipRect, NativeMethods.tagOIFI lpFrameInfo)
            {
                ppDoc = null;
                ppFrame = host.GetParentContainer();
                FillInRect(lprcPosRect, host.Bounds);
                host.GetClipRect(lprcClipRect);
                if (lpFrameInfo != null)
                {
                    lpFrameInfo.cb = Marshal.SizeOf<NativeMethods.tagOIFI>();
                    lpFrameInfo.fMDIApp = false;
                    lpFrameInfo.hAccel = IntPtr.Zero;
                    lpFrameInfo.cAccelEntries = 0;
                    lpFrameInfo.hwndFrame = host.ParentInternal.Handle;
                }
                return NativeMethods.S_OK;
            }

            Interop.HRESULT UnsafeNativeMethods.IOleInPlaceSite.Scroll(Size scrollExtant)
            {
                return Interop.HRESULT.S_FALSE;
            }

            int UnsafeNativeMethods.IOleInPlaceSite.OnUIDeactivate(int fUndoable)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in OnUIDeactivate for " + host.ToString());
                host.GetParentContainer().OnUIDeactivate(host);
                if (host.GetOcState() > OC_INPLACE)
                {
                    host.SetOcState(OC_INPLACE);
                }
                return NativeMethods.S_OK;
            }

            int UnsafeNativeMethods.IOleInPlaceSite.OnInPlaceDeactivate()
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in OnInPlaceDeactivate");
                if (host.GetOcState() == OC_UIACTIVE)
                {
                    ((UnsafeNativeMethods.IOleInPlaceSite)this).OnUIDeactivate(0);
                }

                host.GetParentContainer().OnInPlaceDeactivate(host);
                host.DetachWindow();
                host.SetOcState(OC_RUNNING);
                return NativeMethods.S_OK;
            }

            int UnsafeNativeMethods.IOleInPlaceSite.DiscardUndoState()
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in DiscardUndoState");
                return NativeMethods.S_OK;
            }

            int UnsafeNativeMethods.IOleInPlaceSite.DeactivateAndUndo()
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in DeactivateAndUndo for " + host.ToString());
                return host.GetInPlaceObject().UIDeactivate();
            }

            int UnsafeNativeMethods.IOleInPlaceSite.OnPosRectChange(NativeMethods.COMRECT lprcPosRect)
            {
                // The MediaPlayer control has a AllowChangeDisplaySize property that users
                // can set to control size changes at runtime, but the control itself ignores that and sets the new size.
                // We prevent this by not allowing controls to call OnPosRectChange(), unless we instantiated the resize.
                // visual basic6 does the same.
                //
                bool useRect = true;
                if (AxHost.windowsMediaPlayer_Clsid.Equals(host.clsid))
                {
                    useRect = host.GetAxState(AxHost.handlePosRectChanged);
                }

                if (useRect)
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in OnPosRectChange" + lprcPosRect.ToString());
                    host.GetInPlaceObject().SetObjectRects(lprcPosRect, host.GetClipRect(new NativeMethods.COMRECT()));
                    host.MakeDirty();
                }
                else
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Control directly called OnPosRectChange... ignoring the new size");
                }
                return NativeMethods.S_OK;
            }

            // IPropertyNotifySink methods

            void UnsafeNativeMethods.IPropertyNotifySink.OnChanged(int dispid)
            {
                // Some controls fire OnChanged() notifications when getting values of some properties.
                // To prevent this kind of recursion, we check to see if we are already inside a OnChanged() call.
                //
                if (host.NoComponentChangeEvents != 0)
                {
                    return;
                }

                host.NoComponentChangeEvents++;
                try
                {
                    AxPropertyDescriptor prop = null;

                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in OnChanged");

                    if (dispid != NativeMethods.ActiveX.DISPID_UNKNOWN)
                    {
                        prop = host.GetPropertyDescriptorFromDispid(dispid);
                        if (prop != null)
                        {
                            prop.OnValueChanged(host);
                            if (!prop.SettingValue)
                            {
                                prop.UpdateTypeConverterAndTypeEditor(true);
                            }
                        }
                    }
                    else
                    {
                        // update them all for DISPID_UNKNOWN.
                        //
                        PropertyDescriptorCollection props = ((ICustomTypeDescriptor)host).GetProperties();
                        foreach (PropertyDescriptor p in props)
                        {
                            prop = p as AxPropertyDescriptor;
                            if (prop != null && !prop.SettingValue)
                            {
                                prop.UpdateTypeConverterAndTypeEditor(true);
                            }
                        }
                    }

                    ISite site = host.Site;
                    if (site != null)
                    {
                        IComponentChangeService changeService = (IComponentChangeService)site.GetService(typeof(IComponentChangeService));

                        if (changeService != null)
                        {
                            try
                            {
                                changeService.OnComponentChanging(host, prop);
                            }
                            catch (CheckoutException coEx)
                            {
                                if (coEx == CheckoutException.Canceled)
                                {
                                    return;
                                }
                                throw coEx;
                            }

                            // Now notify the change service that the change was successful.
                            //
                            changeService.OnComponentChanged(host, prop, null, (prop?.GetValue(host)));
                        }
                    }
                }
                catch (Exception t)
                {
                    Debug.Fail(t.ToString());
                    throw t;
                }
                finally
                {
                    host.NoComponentChangeEvents--;
                }
            }

            int UnsafeNativeMethods.IPropertyNotifySink.OnRequestEdit(int dispid)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in OnRequestEdit for " + host.ToString());
                return NativeMethods.S_OK;
            }
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

        private bool QuickActivate()
        {
            if (!(instance is UnsafeNativeMethods.IQuickActivate))
            {
                return false;
            }

            UnsafeNativeMethods.IQuickActivate iqa = (UnsafeNativeMethods.IQuickActivate)instance;

            UnsafeNativeMethods.tagQACONTAINER qaContainer = new UnsafeNativeMethods.tagQACONTAINER();
            UnsafeNativeMethods.tagQACONTROL qaControl = new UnsafeNativeMethods.tagQACONTROL();

            qaContainer.pClientSite = oleSite;
            qaContainer.pPropertyNotifySink = oleSite;
            //         qaContainer.pControlSite = oleSite;
            //         qaContainer.pAdviseSink = null;
            //         qaContainer.pUnkEventSink = null;
            //         qaContainer.pUndoMgr = null;
            //         qaContainer.pBindHost = null;
            //         qaContainer.pServiveProvider = null;
            //         qaContainer.hpal = 0;
            qaContainer.pFont = GetIFontFromFont(GetParentContainer().parent.Font);
            qaContainer.dwAppearance = 0;
            qaContainer.lcid = Application.CurrentCulture.LCID;

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
            qaContainer.dwAmbientFlags = NativeMethods.ActiveX.QACONTAINER_AUTOCLIP | NativeMethods.ActiveX.QACONTAINER_MESSAGEREFLECT |
                                         NativeMethods.ActiveX.QACONTAINER_SUPPORTSMNEMONICS;
            if (IsUserMode())
            {
                qaContainer.dwAmbientFlags |= NativeMethods.ActiveX.QACONTAINER_USERMODE;
            }
            else
            {
                // Can't set ui dead becuase MFC controls return NOWHERE on NCHITTEST which
                // messes up the designer...
                // But, without this the FarPoint SpreadSheet and the Office SpreadSheet
                // controls take keyboard input at design time.
                //qaContainer.dwAmbientFlags |= NativeMethods.ActiveX.QACONTAINER_UIDEAD;
                // qaContainer.dwAmbientFlags |= ActiveX.QACONTAINER_SHOWHATCHING;
            }

            try
            {
                iqa.QuickActivate(qaContainer, qaControl);
            }
            catch (Exception t)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Failed to QuickActivate: " + t.ToString());
                DisposeAxControl();
                return false;
            }
            miscStatusBits = qaControl.dwMiscStatus;
            ParseMiscBits(miscStatusBits);
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

        private void ParseMiscBits(int bits)
        {
            axState[fOwnWindow] = ((bits & NativeMethods.ActiveX.OLEMISC_INVISIBLEATRUNTIME) != 0) && IsUserMode();
            axState[fSimpleFrame] = ((bits & NativeMethods.ActiveX.OLEMISC_SIMPLEFRAME) != 0);
        }

        private void SlowActivate()
        {
            bool setClientSite = false;

            if ((miscStatusBits & NativeMethods.ActiveX.OLEMISC_SETCLIENTSITEFIRST) != 0)
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

        private static NativeMethods.COMRECT FillInRect(NativeMethods.COMRECT dest, Rectangle source)
        {
            dest.left = source.X;
            dest.top = source.Y;
            dest.right = source.Width + source.X;
            dest.bottom = source.Height + source.Y;
            return dest;
        }

        private AxContainer GetParentContainer()
        {
            if (container == null)
            {
                container = AxContainer.FindContainerForControl(this);
            }
            if (container == null)
            {
                ContainerControl f = ContainingControl;
                if (f == null)
                {
                    // ContainingCointrol can be null if the AxHost is still not parented to a containerControl
                    // In everett we used to return a parking window.
                    // now we just set the containingControl to a dummyValue.
                    if (newParent == null)
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

        private UnsafeNativeMethods.IOleControl GetOleControl()
        {
            if (iOleControl == null)
            {
                Debug.Assert(instance != null, "must have the ocx");
                iOleControl = (UnsafeNativeMethods.IOleControl)instance;
            }
            return iOleControl;
        }

        private UnsafeNativeMethods.IOleInPlaceActiveObject GetInPlaceActiveObject()
        {
            // if our AxContainer was set an external active object then use it.
            if (iOleInPlaceActiveObjectExternal != null)
            {
                return iOleInPlaceActiveObjectExternal;
            }

            // otherwise use our instance.
            if (iOleInPlaceActiveObject == null)
            {
                Debug.Assert(instance != null, "must have the ocx");
                try
                {
                    iOleInPlaceActiveObject = (UnsafeNativeMethods.IOleInPlaceActiveObject)instance;
                }
                catch (InvalidCastException e)
                {
                    Debug.Fail("Invalid cast in GetInPlaceActiveObject: " + e.ToString());
                }
            }
            return iOleInPlaceActiveObject;
        }

        private UnsafeNativeMethods.IOleObject GetOleObject()
        {
            if (iOleObject == null)
            {
                Debug.Assert(instance != null, "must have the ocx");
                iOleObject = (UnsafeNativeMethods.IOleObject)instance;
            }
            return iOleObject;
        }

        private UnsafeNativeMethods.IOleInPlaceObject GetInPlaceObject()
        {
            if (iOleInPlaceObject == null)
            {
                Debug.Assert(instance != null, "must have the ocx");
                iOleInPlaceObject = (UnsafeNativeMethods.IOleInPlaceObject)instance;

#if DEBUG
                if (iOleInPlaceObject is UnsafeNativeMethods.IOleInPlaceObjectWindowless)
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, GetType().FullName + " Can also be a Windowless control.");
                }
#endif //DEBUG
            }
            return iOleInPlaceObject;
        }

        private NativeMethods.ICategorizeProperties GetCategorizeProperties()
        {
            if (iCategorizeProperties == null && !axState[checkedCP] && instance != null)
            {
                axState[checkedCP] = true;
                if (instance is NativeMethods.ICategorizeProperties)
                {
                    iCategorizeProperties = (NativeMethods.ICategorizeProperties)instance;
                }
            }
            return iCategorizeProperties;
        }

        private NativeMethods.IPerPropertyBrowsing GetPerPropertyBrowsing()
        {
            if (iPerPropertyBrowsing == null && !axState[checkedIppb] && instance != null)
            {
                axState[checkedIppb] = true;
                if (instance is NativeMethods.IPerPropertyBrowsing)
                {
                    iPerPropertyBrowsing = (NativeMethods.IPerPropertyBrowsing)instance;
                }
            }
            return iPerPropertyBrowsing;
        }

        // Mapping functions:
        private static object GetPICTDESCFromPicture(Image image)
        {
            if (image is Bitmap bmp)
            {
                return new NativeMethods.PICTDESCbmp(bmp);
            }

            if (image is Metafile mf)
            {
                return new NativeMethods.PICTDESCemf(mf);
            }
            throw new ArgumentException(SR.AXUnknownImage, "image");
        }

        /// <summary>
        ///  Maps from a System.Drawing.Image to an OLE IPicture
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected static object GetIPictureFromPicture(Image image)
        {
            if (image == null)
            {
                return null;
            }

            object pictdesc = GetPICTDESCFromPicture(image);
            return UnsafeNativeMethods.OleCreateIPictureIndirect(pictdesc, ref ipicture_Guid, true);
        }

        /// <summary>
        ///  Maps from a System.Drawing.Cursor to an OLE IPicture
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected static object GetIPictureFromCursor(Cursor cursor)
        {
            if (cursor == null)
            {
                return null;
            }

            NativeMethods.PICTDESCicon pictdesc = new NativeMethods.PICTDESCicon(Icon.FromHandle(cursor.Handle));
            return UnsafeNativeMethods.OleCreateIPictureIndirect(pictdesc, ref ipicture_Guid, true);
        }

        /// <summary>
        ///  Maps from a System.Drawing.Image to an OLE IPictureDisp
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected static object GetIPictureDispFromPicture(Image image)
        {
            if (image == null)
            {
                return null;
            }

            object pictdesc = GetPICTDESCFromPicture(image);
            return UnsafeNativeMethods.OleCreateIPictureDispIndirect(pictdesc, ref ipictureDisp_Guid, true);
        }

        /// <summary>
        ///  Maps from an OLE IPicture to a System.Drawing.Image
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected static Image GetPictureFromIPicture(object picture)
        {
            if (picture == null)
            {
                return null;
            }

            IntPtr hPal = IntPtr.Zero;
            UnsafeNativeMethods.IPicture pict = (UnsafeNativeMethods.IPicture)picture;
            int type = pict.GetPictureType();
            if (type == NativeMethods.Ole.PICTYPE_BITMAP)
            {
                try
                {
                    hPal = pict.GetHPal();
                }
                catch (COMException)
                {
                }
            }
            return GetPictureFromParams(pict, pict.GetHandle(), type, hPal, pict.GetWidth(), pict.GetHeight());
        }

        /// <summary>
        ///  Maps from an OLE IPictureDisp to a System.Drawing.Image
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected static Image GetPictureFromIPictureDisp(object picture)
        {
            if (picture == null)
            {
                return null;
            }

            IntPtr hPal = IntPtr.Zero;
            UnsafeNativeMethods.IPictureDisp pict = (UnsafeNativeMethods.IPictureDisp)picture;
            int type = pict.PictureType;
            if (type == NativeMethods.Ole.PICTYPE_BITMAP)
            {
                try
                {
                    hPal = pict.HPal;
                }
                catch (COMException)
                {
                }
            }
            return GetPictureFromParams(pict, pict.Handle, type, hPal, pict.Width, pict.Height);
        }

        private static Image GetPictureFromParams(object pict, IntPtr handle, int type, IntPtr paletteHandle, int width, int height)
        {
            switch (type)
            {
                case NativeMethods.Ole.PICTYPE_ICON:
                    return (Image)(Icon.FromHandle(handle)).Clone();
                case NativeMethods.Ole.PICTYPE_METAFILE:
                    WmfPlaceableFileHeader header = new WmfPlaceableFileHeader
                    {
                        BboxRight = (short)width,
                        BboxBottom = (short)height
                    };
                    return (Image)(new Metafile(handle, header, false)).Clone();
                case NativeMethods.Ole.PICTYPE_ENHMETAFILE:
                    return (Image)(new Metafile(handle, false)).Clone();
                case NativeMethods.Ole.PICTYPE_BITMAP:
                    return Image.FromHbitmap(handle, paletteHandle);
                case NativeMethods.Ole.PICTYPE_NONE:
                    // MSDN sez this should not be a valid value, but comctl32 returns it...
                    return null;
                case NativeMethods.Ole.PICTYPE_UNINITIALIZED:
                    return null;
                default:
                    Debug.Fail("Invalid image type " + type.ToString(CultureInfo.InvariantCulture));
                    throw new ArgumentException(SR.AXUnknownImage, "type");
            }
        }

        private static NativeMethods.FONTDESC GetFONTDESCFromFont(Font font)
        {
            NativeMethods.FONTDESC fdesc = null;

            if (fontTable == null)
            {
                fontTable = new Hashtable();
            }
            else
            {
                fdesc = (NativeMethods.FONTDESC)fontTable[font];
            }

            if (fdesc == null)
            {
                fdesc = new NativeMethods.FONTDESC
                {
                    lpstrName = font.Name,
                    cySize = (long)(font.SizeInPoints * 10000)
                };

                NativeMethods.LOGFONTW logfont = NativeMethods.LOGFONTW.FromFont(font);
                fdesc.sWeight = (short)logfont.lfWeight;
                fdesc.sCharset = logfont.lfCharSet;
                fdesc.fItalic = font.Italic;
                fdesc.fUnderline = font.Underline;
                fdesc.fStrikethrough = font.Strikeout;

                fontTable[font] = fdesc;
            }

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
            if (font == null)
            {
                return null;
            }

            if (font.Unit != GraphicsUnit.Point)
            {
                throw new ArgumentException(SR.AXFontUnitNotPoint, "font");
            }

            try
            {
                return (UnsafeNativeMethods.IFont)UnsafeNativeMethods.OleCreateIFontIndirect(GetFONTDESCFromFont(font), ref ifont_Guid);
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
            if (font == null)
            {
                return null;
            }

            UnsafeNativeMethods.IFont oleFont = (UnsafeNativeMethods.IFont)font;
            try
            {
                Font f = Font.FromHfont(oleFont.GetHFont());

                if (f.Unit != GraphicsUnit.Point)
                {
                    f = new Font(f.Name, f.SizeInPoints, f.Style, GraphicsUnit.Point, f.GdiCharSet, f.GdiVerticalFont);
                }

                return f;
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
            if (font == null)
            {
                return null;
            }

            if (font.Unit != GraphicsUnit.Point)
            {
                throw new ArgumentException(SR.AXFontUnitNotPoint, "font");
            }

            SafeNativeMethods.IFontDisp rval = SafeNativeMethods.OleCreateIFontDispIndirect(GetFONTDESCFromFont(font), ref ifontDisp_Guid);
            return rval;
        }

        /// <summary>
        ///  Maps from an IFontDisp to a System.Drawing.Font object
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected static Font GetFontFromIFontDisp(object font)
        {
            if (font == null)
            {
                return null;
            }

            if (font is UnsafeNativeMethods.IFont ifont)
            {
                return GetFontFromIFont(ifont);
            }

            SafeNativeMethods.IFontDisp oleFont = (SafeNativeMethods.IFontDisp)font;
            FontStyle style = FontStyle.Regular;

            Font f = null;
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

                if ((int)oleFont.Weight >= 700) // bold
                {
                    style |= FontStyle.Bold;
                }

                f = new Font(oleFont.Name, (float)oleFont.Size / (float)10000, style, GraphicsUnit.Point, (byte)oleFont.Charset);
                return f;
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

        private class VBFormat : UnsafeNativeMethods.IVBFormat
        {
            // IVBFormat methods:
            //
            int UnsafeNativeMethods.IVBFormat.Format(ref object var, IntPtr pszFormat, IntPtr lpBuffer, short cpBuffer, int lcid, short firstD, short firstW, short[] result)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in Format");
                if (result == null)
                {
                    return NativeMethods.E_INVALIDARG;
                }

                result[0] = 0;
                if (lpBuffer == IntPtr.Zero || cpBuffer < 2)
                {
                    return NativeMethods.E_INVALIDARG;
                }

                IntPtr pbstr = IntPtr.Zero;
                int hr = UnsafeNativeMethods.VarFormat(ref var, new HandleRef(null, pszFormat), firstD, firstW, 32 /* VAR_FORMAT_NOSUBSTITUTE */, ref pbstr);

                try
                {
                    int i = 0;
                    if (pbstr != IntPtr.Zero)
                    {
                        short ch = 0;
                        cpBuffer--;
                        for (; i < cpBuffer && (ch = Marshal.ReadInt16(pbstr, i * 2)) != 0; i++)
                        {
                            Marshal.WriteInt16(lpBuffer, i * 2, ch);
                        }
                    }
                    Marshal.WriteInt16(lpBuffer, i * 2, (short)0);
                    result[0] = (short)i;
                }
                finally
                {
                    SafeNativeMethods.SysFreeString(new HandleRef(null, pbstr));
                }

                return NativeMethods.S_OK;
            }
        }

        internal class EnumUnknown : UnsafeNativeMethods.IEnumUnknown
        {
            private readonly object[] arr;
            private int loc;
            private readonly int size;

            internal EnumUnknown(object[] arr)
            {
                //if (AxHTraceSwitch.TraceVerbose) Debug.WriteObject(arr);
                this.arr = arr;
                loc = 0;
                size = (arr == null) ? 0 : arr.Length;
            }

            private EnumUnknown(object[] arr, int loc) : this(arr)
            {
                this.loc = loc;
            }

            unsafe int UnsafeNativeMethods.IEnumUnknown.Next(int celt, IntPtr rgelt, IntPtr pceltFetched)
            {
                if (pceltFetched != IntPtr.Zero)
                {
                    Marshal.WriteInt32(pceltFetched, 0, 0);
                }

                if (celt < 0)
                {
                    return NativeMethods.E_INVALIDARG;
                }

                int fetched = 0;
                if (loc >= size)
                {
                    fetched = 0;
                }
                else
                {
                    for (; loc < size && fetched < celt; ++loc)
                    {
                        if (arr[loc] != null)
                        {
                            Marshal.WriteIntPtr(rgelt, Marshal.GetIUnknownForObject(arr[loc]));
                            rgelt = (IntPtr)((long)rgelt + (long)sizeof(IntPtr));
                            ++fetched;
                        }
                    }
                }

                if (pceltFetched != IntPtr.Zero)
                {
                    Marshal.WriteInt32(pceltFetched, 0, fetched);
                }

                if (fetched != celt)
                {
                    return (NativeMethods.S_FALSE);
                }
                return NativeMethods.S_OK;
            }

            int UnsafeNativeMethods.IEnumUnknown.Skip(int celt)
            {
                loc += celt;
                if (loc >= size)
                {
                    return (NativeMethods.S_FALSE);
                }
                return NativeMethods.S_OK;
            }

            void UnsafeNativeMethods.IEnumUnknown.Reset()
            {
                loc = 0;
            }

            void UnsafeNativeMethods.IEnumUnknown.Clone(out UnsafeNativeMethods.IEnumUnknown ppenum)
            {
                ppenum = new EnumUnknown(arr, loc);
            }
        }

        internal class AxContainer : UnsafeNativeMethods.IOleContainer, UnsafeNativeMethods.IOleInPlaceFrame, IReflect
        {
            internal ContainerControl parent;
            private IContainer assocContainer; // associated IContainer...
            // the assocContainer may be null, in which case all this container does is
            // forward [de]activation messages to the requisite container...
            private AxHost siteUIActive;
            private AxHost siteActive;
            private bool formAlreadyCreated = false;
            private readonly Hashtable containerCache = new Hashtable();  // name -> Control
            private int lockCount = 0;
            private Hashtable components = null;  // Control -> any
            private Hashtable proxyCache = null;
            private AxHost ctlInEditMode = null;

            private const int GC_CHILD = 0x1;
            private const int GC_LASTSIBLING = 0x2;
            private const int GC_FIRSTSIBLING = 0x4;
            private const int GC_CONTAINER = 0x20;
            private const int GC_PREVSIBLING = 0x40;
            private const int GC_NEXTSIBLING = 0x80;

            internal AxContainer(ContainerControl parent)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in constructor.  Parent created : " + parent.Created.ToString());
                this.parent = parent;
                if (parent.Created)
                {
                    FormCreated();
                }
            }

            // IReflect methods:

            MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
            {
                return null;
            }

            MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr)
            {
                return null;
            }

            MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr)
            {
                return Array.Empty<MethodInfo>();
            }

            FieldInfo IReflect.GetField(string name, BindingFlags bindingAttr)
            {
                return null;
            }

            FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr)
            {
                return Array.Empty<FieldInfo>();
            }

            PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr)
            {
                return null;
            }

            PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
            {
                return null;
            }

            PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr)
            {
                return Array.Empty<PropertyInfo>();
            }

            MemberInfo[] IReflect.GetMember(string name, BindingFlags bindingAttr)
            {
                return Array.Empty<MemberInfo>();
            }

            MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr)
            {
                return Array.Empty<MemberInfo>();
            }

            object IReflect.InvokeMember(string name, BindingFlags invokeAttr, Binder binder,
                                                    object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
            {

                foreach (DictionaryEntry e in containerCache)
                {
                    string ctlName = GetNameForControl((Control)e.Key);
                    if (ctlName.Equals(name))
                    {
                        return GetProxyForControl((Control)e.Value);
                    }
                }

                throw E_FAIL;
            }

            Type IReflect.UnderlyingSystemType
            {
                get
                {
                    return null;
                }
            }

            internal UnsafeNativeMethods.IExtender GetProxyForControl(Control ctl)
            {
                UnsafeNativeMethods.IExtender rval = null;
                if (proxyCache == null)
                {
                    proxyCache = new Hashtable();
                }
                else
                {
                    rval = (UnsafeNativeMethods.IExtender)proxyCache[ctl];
                }
                if (rval == null)
                {
                    if (ctl != parent && !GetControlBelongs(ctl))
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "!parent || !belongs NYI");
                        AxContainer c = FindContainerForControl(ctl);
                        if (c != null)
                        {
                            rval = new ExtenderProxy(ctl, c);
                        }
                        else
                        {
                            Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "unable to find proxy, returning null");
                            return null;
                        }
                    }
                    else
                    {
                        rval = new ExtenderProxy(ctl, this);
                    }
                    proxyCache.Add(ctl, rval);
                }
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "found proxy " + rval.ToString());
                return rval;
            }

            internal string GetNameForControl(Control ctl)
            {
                string name = (ctl.Site != null) ? ctl.Site.Name : ctl.Name;
                return name ?? "";
            }

            internal object GetProxyForContainer()
            {
                return this;
            }

            internal void AddControl(Control ctl)
            {
                //
                lock (this)
                {
                    if (containerCache.Contains(ctl))
                    {
                        throw new ArgumentException(string.Format(SR.AXDuplicateControl, GetNameForControl(ctl)), "ctl");
                    }

                    containerCache.Add(ctl, ctl);

                    if (assocContainer == null)
                    {
                        ISite site = ctl.Site;
                        if (site != null)
                        {
                            assocContainer = site.Container;
                            IComponentChangeService ccs = (IComponentChangeService)site.GetService(typeof(IComponentChangeService));
                            if (ccs != null)
                            {
                                ccs.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
                            }
                        }
                    }
                    else
                    {
#if DEBUG
                        ISite site = ctl.Site;
                        if (site != null && assocContainer != site.Container)
                        {
                            Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "mismatch between assoc container & added control");
                        }
#endif
                    }
                }
            }

            internal void RemoveControl(Control ctl)
            {
                //
                lock (this)
                {
                    if (containerCache.Contains(ctl))
                    {
                        containerCache.Remove(ctl);
                    }
                }
            }

            private void LockComponents()
            {
                lockCount++;
            }

            private void UnlockComponents()
            {
                lockCount--;
                if (lockCount == 0)
                {
                    components = null;
                }
            }

            internal UnsafeNativeMethods.IEnumUnknown EnumControls(Control ctl, int dwOleContF, int dwWhich)
            {
                GetComponents();

                LockComponents();
                try
                {
                    ArrayList l = null;
                    bool selected = (dwWhich & NativeMethods.ActiveX.GC_WCH_FSELECTED) != 0;
                    bool reverse = (dwWhich & NativeMethods.ActiveX.GC_WCH_FREVERSEDIR) != 0;
                    // Note that visual basic actually ignores the next/prev flags... we will not
                    bool onlyNext = (dwWhich & NativeMethods.ActiveX.GC_WCH_FONLYNEXT) != 0;
                    bool onlyPrev = (dwWhich & NativeMethods.ActiveX.GC_WCH_FONLYPREV) != 0;
                    dwWhich &= ~(NativeMethods.ActiveX.GC_WCH_FSELECTED | NativeMethods.ActiveX.GC_WCH_FREVERSEDIR |
                                          NativeMethods.ActiveX.GC_WCH_FONLYNEXT | NativeMethods.ActiveX.GC_WCH_FONLYPREV);
                    if (onlyNext && onlyPrev)
                    {
                        Debug.Fail("onlyNext && onlyPrev are both set!");
                        throw E_INVALIDARG;
                    }
                    if (dwWhich == NativeMethods.ActiveX.GC_WCH_CONTAINER || dwWhich == NativeMethods.ActiveX.GC_WCH_CONTAINED)
                    {
                        if (onlyNext || onlyPrev)
                        {
                            Debug.Fail("GC_WCH_FONLYNEXT or FONLYPREV used with CONTANER or CONATINED");
                            throw E_INVALIDARG;
                        }
                    }
                    int first = 0;
                    int last = -1; // meaning all
                    Control[] ctls = null;
                    switch (dwWhich)
                    {
                        default:
                            Debug.Fail("Bad GC_WCH");
                            throw E_INVALIDARG;
                        case NativeMethods.ActiveX.GC_WCH_CONTAINED:
                            ctls = ctl.GetChildControlsInTabOrder(false);
                            ctl = null;
                            break;
                        case NativeMethods.ActiveX.GC_WCH_SIBLING:
                            Control p = ctl.ParentInternal;
                            if (p != null)
                            {
                                ctls = p.GetChildControlsInTabOrder(false);
                                if (onlyPrev)
                                {
                                    last = ctl.TabIndex;
                                }
                                else if (onlyNext)
                                {
                                    first = ctl.TabIndex + 1;
                                }
                            }
                            else
                            {
                                ctls = Array.Empty<Control>();
                            }
                            ctl = null;
                            break;
                        case NativeMethods.ActiveX.GC_WCH_CONTAINER:
                            l = new ArrayList();
                            MaybeAdd(l, ctl, selected, dwOleContF, false);
                            while (ctl != null)
                            {
                                AxContainer cont = FindContainerForControl(ctl);
                                if (cont != null)
                                {
                                    MaybeAdd(l, cont.parent, selected, dwOleContF, true);
                                    ctl = cont.parent;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            break;
                        case NativeMethods.ActiveX.GC_WCH_ALL:
                            Hashtable htbl = GetComponents();
                            ctls = new Control[htbl.Keys.Count];
                            htbl.Keys.CopyTo(ctls, 0);
                            ctl = parent;
                            break;
                    }
                    if (l == null)
                    {
                        l = new ArrayList();
                        if (last == -1 && ctls != null)
                        {
                            last = ctls.Length;
                        }

                        if (ctl != null)
                        {
                            MaybeAdd(l, ctl, selected, dwOleContF, false);
                        }

                        for (int i = first; i < last; i++)
                        {
                            MaybeAdd(l, ctls[i], selected, dwOleContF, false);
                        }
                    }
                    object[] rval = new object[l.Count];
                    l.CopyTo(rval, 0);
                    if (reverse)
                    {
                        for (int i = 0, j = rval.Length - 1; i < j; i++, j--)
                        {
                            object temp = rval[i];
                            rval[i] = rval[j];
                            rval[j] = temp;
                        }
                    }
                    return new EnumUnknown(rval);
                }
                finally
                {
                    UnlockComponents();
                }
            }

            private void MaybeAdd(ArrayList l, Control ctl, bool selected, int dwOleContF, bool ignoreBelong)
            {
                if (!ignoreBelong && ctl != parent && !GetControlBelongs(ctl))
                {
                    return;
                }

                if (selected)
                {
                    ISelectionService iss = GetSelectionService(ctl);
                    if (iss == null || !iss.GetComponentSelected(this))
                    {
                        return;
                    }
                }
                if (ctl is AxHost hostctl && (dwOleContF & NativeMethods.ActiveX.OLECONTF_EMBEDDINGS) != 0)
                {
                    l.Add(hostctl.GetOcx());
                }
                else if ((dwOleContF & NativeMethods.ActiveX.OLECONTF_OTHERS) != 0)
                {
                    object item = GetProxyForControl(ctl);
                    if (item != null)
                    {
                        l.Add(item);
                    }
                }
            }

            private void FillComponentsTable(IContainer container)
            {
                if (container != null)
                {
                    ComponentCollection comps = container.Components;
                    if (comps != null)
                    {
                        components = new Hashtable();
                        foreach (IComponent comp in comps)
                        {
                            if (comp is Control && comp != parent && comp.Site != null)
                            {
                                components.Add(comp, comp);
                            }
                        }
                        return;
                    }
                }

                Debug.Assert(parent.Site == null, "Parent is sited but we could not find IContainer!!!");
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Did not find a container in FillComponentsTable!!!");

                bool checkHashTable = true;
                Control[] ctls = new Control[containerCache.Values.Count];
                containerCache.Values.CopyTo(ctls, 0);
                if (ctls != null)
                {
                    if (ctls.Length > 0 && components == null)
                    {
                        components = new Hashtable();
                        checkHashTable = false;
                    }
                    for (int i = 0; i < ctls.Length; i++)
                    {
                        if (checkHashTable && !components.Contains(ctls[i]))
                        {
                            components.Add(ctls[i], ctls[i]);
                        }
                    }
                }

                GetAllChildren(parent);
            }

            private void GetAllChildren(Control ctl)
            {
                if (ctl == null)
                {
                    return;
                }

                if (components == null)
                {
                    components = new Hashtable();
                }

                if (ctl != parent && !components.Contains(ctl))
                {
                    components.Add(ctl, ctl);
                }

                foreach (Control c in ctl.Controls)
                {
                    GetAllChildren(c);
                }
            }

            private Hashtable GetComponents()
            {
                return GetComponents(GetParentsContainer());
            }

            private Hashtable GetComponents(IContainer cont)
            {
                if (lockCount == 0)
                {
                    FillComponentsTable(cont);
                }
                return components;
            }

            private bool GetControlBelongs(Control ctl)
            {
                Hashtable comps = GetComponents();
                return comps[ctl] != null;
            }

            private IContainer GetParentIsDesigned()
            {
                ISite site = parent.Site;
                if (site != null && site.DesignMode)
                {
                    return site.Container;
                }

                return null;
            }

            private IContainer GetParentsContainer()
            {
                IContainer rval = GetParentIsDesigned();
                Debug.Assert(rval == null || assocContainer == null || (rval == assocContainer),
                             "mismatch between getIPD & aContainer");
                return rval ?? assocContainer;
            }

            private bool RegisterControl(AxHost ctl)
            {
                ISite site = ctl.Site;
                if (site != null)
                {
                    IContainer cont = site.Container;
                    if (cont != null)
                    {
                        if (assocContainer != null)
                        {
                            return cont == assocContainer;
                        }
                        else
                        {
                            assocContainer = cont;
                            IComponentChangeService ccs = (IComponentChangeService)site.GetService(typeof(IComponentChangeService));
                            if (ccs != null)
                            {
                                ccs.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
                            }
                            return true;
                        }
                    }
                }
                return false;
            }

            private void OnComponentRemoved(object sender, ComponentEventArgs e)
            {
                if (sender == assocContainer && e.Component is Control c)
                {
                    RemoveControl(c);
                }
            }

            internal static AxContainer FindContainerForControl(Control ctl)
            {
                if (ctl is AxHost axctl)
                {
                    if (axctl.container != null)
                    {
                        return axctl.container;
                    }

                    ContainerControl f = axctl.ContainingControl;
                    if (f != null)
                    {
                        AxContainer container = f.CreateAxContainer();
                        if (container.RegisterControl(axctl))
                        {
                            container.AddControl(axctl);
                            return container;
                        }
                    }
                }
                return null;
            }

            internal void OnInPlaceDeactivate(AxHost site)
            {
                if (siteActive == site)
                {
                    siteActive = null;
                    if (site.GetSiteOwnsDeactivation())
                    {
                        parent.ActiveControl = null;
                    }
                    else
                    {
                        // we need to tell the form to switch activation to the next thingie...
                        Debug.Fail("what pathological control is calling inplacedeactivate by itself?");
                    }
                }
            }

            internal void OnUIDeactivate(AxHost site)
            {
#if DEBUG
                if (siteUIActive != null)
                {
                    Debug.Assert(siteUIActive == site, "deactivating when not active...");
                }
#endif // DEBUG

                siteUIActive = null;
                site.RemoveSelectionHandler();
                site.SetSelectionStyle(1);
                site.editMode = EDITM_NONE;
                if (site.GetSiteOwnsDeactivation())
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, " our site owns deactivation ");
                    ContainerControl f = site.ContainingControl;
                    Debug.Assert(f != null, "a control has to be on a ContainerControl...");
                    if (f != null)
                    {
                        //    f.setActiveControl(null);
                    }
                }
            }

            internal void OnUIActivate(AxHost site)
            {
                // The ShDocVw control repeatedly calls OnUIActivate() with the same
                // site. This causes the assert below to fire.
                //
                if (siteUIActive == site)
                {
                    return;
                }

                if (siteUIActive != null && siteUIActive != site)
                {
                    AxHost tempSite = siteUIActive;
                    bool ownDisposing = tempSite.GetAxState(AxHost.ownDisposing);
                    try
                    {
                        tempSite.SetAxState(AxHost.ownDisposing, true);
                        tempSite.GetInPlaceObject().UIDeactivate();
                    }
                    finally
                    {
                        tempSite.SetAxState(AxHost.ownDisposing, ownDisposing);
                    }
                }
                site.AddSelectionHandler();
                Debug.Assert(siteUIActive == null, "Object did not call OnUIDeactivate");
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "active Object is now " + site.ToString());
                siteUIActive = site;
                ContainerControl f = site.ContainingControl;
                Debug.Assert(f != null, "a control has to be on a ContainerControl...");
                if (f != null)
                {
                    f.ActiveControl = site;
                }
            }

            private void ListAxControls(ArrayList list, bool fuseOcx)
            {
                Hashtable components = GetComponents();
                if (components == null)
                {
                    return;
                }

                Control[] ctls = new Control[components.Keys.Count];
                components.Keys.CopyTo(ctls, 0);
                if (ctls != null)
                {
                    for (int i = 0; i < ctls.Length; i++)
                    {
                        Control ctl = ctls[i];
                        if (ctl is AxHost hostctl)
                        {
                            if (fuseOcx)
                            {
                                list.Add(hostctl.GetOcx());
                            }
                            else
                            {
                                list.Add(ctl);
                            }
                        }
                    }
                }
            }

            internal void ControlCreated(AxHost invoker)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in controlCreated for " + invoker.ToString() + " fAC: " + formAlreadyCreated.ToString());
                if (formAlreadyCreated)
                {
                    if (invoker.IsUserMode() && invoker.AwaitingDefreezing())
                    {
                        invoker.Freeze(false);
                    }
                }
                else
                {
                    // the form will be created in the future
                    parent.CreateAxContainer();
                }
            }

            internal void FormCreated()
            {
                if (formAlreadyCreated)
                {
                    return;
                }

                formAlreadyCreated = true;
                ArrayList l = new ArrayList();
                ListAxControls(l, false);
                AxHost[] axControls = new AxHost[l.Count];
                l.CopyTo(axControls, 0);
                for (int i = 0; i < axControls.Length; i++)
                {
                    AxHost control = axControls[i];
                    if (control.GetOcState() >= OC_RUNNING && control.IsUserMode() && control.AwaitingDefreezing())
                    {
                        control.Freeze(false);
                    }
                }
            }

            // IOleContainer methods:

            int UnsafeNativeMethods.IOleContainer.ParseDisplayName(object pbc, string pszDisplayName, int[] pchEaten, object[] ppmkOut)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in ParseDisplayName");
                if (ppmkOut != null)
                {
                    ppmkOut[0] = null;
                }

                return NativeMethods.E_NOTIMPL;
            }

            int UnsafeNativeMethods.IOleContainer.EnumObjects(int grfFlags, out UnsafeNativeMethods.IEnumUnknown ppenum)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in EnumObjects");
                ppenum = null;
                if ((grfFlags & 1) != 0)
                { // 1 == OLECONTF_EMBEDDINGS
                    Debug.Assert(parent != null, "gotta have it...");
                    ArrayList list = new ArrayList();
                    ListAxControls(list, true);
                    if (list.Count > 0)
                    {
                        object[] temp = new object[list.Count];
                        list.CopyTo(temp, 0);
                        ppenum = new EnumUnknown(temp);
                        return NativeMethods.S_OK;
                    }
                }
                ppenum = new EnumUnknown(null);
                return NativeMethods.S_OK;
            }

            int UnsafeNativeMethods.IOleContainer.LockContainer(bool fLock)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in LockContainer");
                return NativeMethods.E_NOTIMPL;
            }

            // IOleInPlaceFrame methods:

            IntPtr UnsafeNativeMethods.IOleInPlaceFrame.GetWindow()
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in GetWindow");
                return parent.Handle;
            }

            int UnsafeNativeMethods.IOleInPlaceFrame.ContextSensitiveHelp(int fEnterMode)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in ContextSensitiveHelp");
                return NativeMethods.S_OK;
            }

            int UnsafeNativeMethods.IOleInPlaceFrame.GetBorder(NativeMethods.COMRECT lprectBorder)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in GetBorder");
                return NativeMethods.E_NOTIMPL;
            }

            int UnsafeNativeMethods.IOleInPlaceFrame.RequestBorderSpace(NativeMethods.COMRECT pborderwidths)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in RequestBorderSpace");
                return NativeMethods.E_NOTIMPL;
            }

            int UnsafeNativeMethods.IOleInPlaceFrame.SetBorderSpace(NativeMethods.COMRECT pborderwidths)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in SetBorderSpace");
                return NativeMethods.E_NOTIMPL;
            }

            internal void OnExitEditMode(AxHost ctl)
            {
                Debug.Assert(ctlInEditMode == null || ctlInEditMode == ctl, "who is exiting edit mode?");
                if (ctlInEditMode == null || ctlInEditMode != ctl)
                {
                    return;
                }

                ctlInEditMode = null;
            }

            int UnsafeNativeMethods.IOleInPlaceFrame.SetActiveObject(UnsafeNativeMethods.IOleInPlaceActiveObject pActiveObject, string pszObjName)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in SetActiveObject " + (pszObjName ?? "<null>"));
                if (siteUIActive != null)
                {
                    if (siteUIActive.iOleInPlaceActiveObjectExternal != pActiveObject)
                    {
                        if (siteUIActive.iOleInPlaceActiveObjectExternal != null)
                        {
                            Marshal.ReleaseComObject(siteUIActive.iOleInPlaceActiveObjectExternal);
                        }
                        siteUIActive.iOleInPlaceActiveObjectExternal = pActiveObject;
                    }
                }
                if (pActiveObject == null)
                {
                    if (ctlInEditMode != null)
                    {
                        ctlInEditMode.editMode = EDITM_NONE;
                        ctlInEditMode = null;
                    }
                    return NativeMethods.S_OK;
                }
                AxHost ctl = null;
                if (pActiveObject is UnsafeNativeMethods.IOleObject oleObject)
                {
                    UnsafeNativeMethods.IOleClientSite clientSite = null;
                    try
                    {
                        clientSite = oleObject.GetClientSite();
                        if (clientSite is OleInterfaces)
                        {
                            ctl = ((OleInterfaces)(clientSite)).GetAxHost();
                        }
                    }
                    catch (COMException t)
                    {
                        Debug.Fail(t.ToString());
                    }
                    if (ctlInEditMode != null)
                    {
                        Debug.Fail("control " + ctlInEditMode.ToString() + " did not reset its edit mode to null");
                        ctlInEditMode.SetSelectionStyle(1);
                        ctlInEditMode.editMode = EDITM_NONE;
                    }

                    if (ctl == null)
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "control w/o a valid site called setactiveobject");
                        ctlInEditMode = null;
                    }
                    else
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "resolved to " + ctl.ToString());
                        if (!ctl.IsUserMode())
                        {
                            ctlInEditMode = ctl;
                            ctl.editMode = EDITM_OBJECT;
                            ctl.AddSelectionHandler();
                            ctl.SetSelectionStyle(2);
                        }
                    }
                }
                return NativeMethods.S_OK;
            }

            int UnsafeNativeMethods.IOleInPlaceFrame.InsertMenus(IntPtr hmenuShared, NativeMethods.tagOleMenuGroupWidths lpMenuWidths)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in InsertMenus");
                return NativeMethods.S_OK;
            }

            int UnsafeNativeMethods.IOleInPlaceFrame.SetMenu(IntPtr hmenuShared, IntPtr holemenu, IntPtr hwndActiveObject)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in SetMenu");
                return NativeMethods.E_NOTIMPL;
            }

            int UnsafeNativeMethods.IOleInPlaceFrame.RemoveMenus(IntPtr hmenuShared)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in RemoveMenus");
                return NativeMethods.E_NOTIMPL;
            }

            int UnsafeNativeMethods.IOleInPlaceFrame.SetStatusText(string pszStatusText)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in SetStatusText");
                return NativeMethods.E_NOTIMPL;
            }

            int UnsafeNativeMethods.IOleInPlaceFrame.EnableModeless(bool fEnable)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in EnableModeless");
                return NativeMethods.E_NOTIMPL;
            }
            int UnsafeNativeMethods.IOleInPlaceFrame.TranslateAccelerator(ref NativeMethods.MSG lpmsg, short wID)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in IOleInPlaceFrame.TranslateAccelerator");
                return NativeMethods.S_FALSE;
            }

            // EXPOSED

            /// <summary>
            /// </summary>
            private class ExtenderProxy : UnsafeNativeMethods.IExtender, UnsafeNativeMethods.IVBGetControl, UnsafeNativeMethods.IGetVBAObject, UnsafeNativeMethods.IGetOleObject, IReflect
            {
                private readonly WeakReference pRef;
                private readonly WeakReference pContainer;

                internal ExtenderProxy(Control principal, AxContainer container)
                {
                    pRef = new WeakReference(principal);
                    pContainer = new WeakReference(container);
                }

                private Control GetP()
                {
                    return (Control)pRef.Target;
                }

                private AxContainer GetC()
                {
                    return (AxContainer)pContainer.Target;
                }

                int UnsafeNativeMethods.IVBGetControl.EnumControls(int dwOleContF, int dwWhich, out UnsafeNativeMethods.IEnumUnknown ppenum)
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in EnumControls for proxy");
                    ppenum = GetC().EnumControls(GetP(), dwOleContF, dwWhich);
                    return NativeMethods.S_OK;
                }

                object UnsafeNativeMethods.IGetOleObject.GetOleObject(ref Guid riid)
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in GetOleObject for proxy");
                    if (!riid.Equals(ioleobject_Guid))
                    {
                        throw E_INVALIDARG;
                    }

                    Control ctl = GetP();
                    if (ctl != null && ctl is AxHost)
                    {
                        return ((AxHost)ctl).GetOcx();
                    }

                    throw E_FAIL;
                }

                int UnsafeNativeMethods.IGetVBAObject.GetObject(ref Guid riid, UnsafeNativeMethods.IVBFormat[] rval, int dwReserved)
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in GetObject for proxy");
                    if (rval == null || riid.Equals(Guid.Empty))
                    {
                        return NativeMethods.E_INVALIDARG;
                    }

                    if (riid.Equals(ivbformat_Guid))
                    {
                        rval[0] = new VBFormat();
                        return NativeMethods.S_OK;
                    }
                    else
                    {
                        rval[0] = null;
                        return NativeMethods.E_NOINTERFACE;
                    }
                }

                public int Align
                {
                    get
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in getAlign for proxy for " + GetP().ToString());
                        int rval = (int)((Control)GetP()).Dock;
                        if (rval < NativeMethods.ActiveX.ALIGN_MIN || rval > NativeMethods.ActiveX.ALIGN_MAX)
                        {
                            rval = NativeMethods.ActiveX.ALIGN_NO_CHANGE;
                        }
                        return rval;
                    }

                    set
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in setAlign for proxy for " + GetP().ToString() + " " +
                                          value.ToString(CultureInfo.InvariantCulture));
                        GetP().Dock = (DockStyle)value;
                    }
                }

                public uint BackColor
                {
                    get
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in getBackColor for proxy for " + GetP().ToString());
                        return AxHost.GetOleColorFromColor(((Control)GetP()).BackColor);
                    }

                    set
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in setBackColor for proxy for " + GetP().ToString() + " " +
                                          value.ToString(CultureInfo.InvariantCulture));
                        GetP().BackColor = AxHost.GetColorFromOleColor(value);
                    }
                }

                public bool Enabled
                {
                    get
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in getEnabled for proxy for " + GetP().ToString());
                        return GetP().Enabled;
                    }

                    set
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in setEnabled for proxy for " + GetP().ToString() + " " + value.ToString());
                        GetP().Enabled = value;
                    }
                }

                public uint ForeColor
                {
                    get
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in getForeColor for proxy for " + GetP().ToString());
                        return AxHost.GetOleColorFromColor(((Control)GetP()).ForeColor);
                    }

                    set
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in setForeColor for proxy for " + GetP().ToString() + " " +
                                          value.ToString(CultureInfo.InvariantCulture));
                        GetP().ForeColor = AxHost.GetColorFromOleColor(value);
                    }
                }

                public int Height
                {
                    get
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in getHeight for proxy for " + GetP().ToString());
                        return Pixel2Twip(GetP().Height, false);
                    }

                    set
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in setHeight for proxy for " + GetP().ToString() + " " +
                                          Twip2Pixel(value, false).ToString(CultureInfo.InvariantCulture));
                        GetP().Height = Twip2Pixel(value, false);
                    }
                }

                public int Left
                {
                    get
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in getLeft for proxy for " + GetP().ToString());
                        return Pixel2Twip(GetP().Left, true);
                    }

                    set
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in setLeft for proxy for " + GetP().ToString() + " " +
                                          Twip2Pixel(value, true).ToString(CultureInfo.InvariantCulture));
                        GetP().Left = Twip2Pixel(value, true);
                    }
                }

                public object Parent
                {
                    get
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in getParent for proxy for " + GetP().ToString());
                        return GetC().GetProxyForControl(GetC().parent);
                    }
                }

                public short TabIndex
                {
                    get
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in getTabIndex for proxy for " + GetP().ToString());
                        return (short)GetP().TabIndex;
                    }

                    set
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in setTabIndex for proxy for " + GetP().ToString() + " " +
                                          value.ToString(CultureInfo.InvariantCulture));
                        GetP().TabIndex = (int)value;
                    }
                }

                public bool TabStop
                {
                    get
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in getTabStop for proxy for " + GetP().ToString());
                        return GetP().TabStop;
                    }

                    set
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in setTabStop for proxy for " + GetP().ToString() + " " + value.ToString());
                        GetP().TabStop = value;
                    }
                }

                public int Top
                {
                    get
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in getTop for proxy for " + GetP().ToString());
                        return Pixel2Twip(GetP().Top, false);
                    }

                    set
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in setTop for proxy for " + GetP().ToString() + " " +
                                          Twip2Pixel(value, false).ToString(CultureInfo.InvariantCulture));
                        GetP().Top = Twip2Pixel(value, false);
                    }
                }

                public bool Visible
                {
                    get
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in getVisible for proxy for " + GetP().ToString());
                        return GetP().Visible;
                    }

                    set
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in setVisible for proxy for " + GetP().ToString() + " " + value.ToString());
                        GetP().Visible = value;
                    }
                }

                public int Width
                {
                    get
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in getWidth for proxy for " + GetP().ToString());
                        return Pixel2Twip(GetP().Width, true);
                    }

                    set
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in setWidth for proxy for " + GetP().ToString() + " " +
                                          Twip2Pixel(value, true).ToString(CultureInfo.InvariantCulture));
                        GetP().Width = Twip2Pixel(value, true);
                    }
                }

                public string Name
                {
                    get
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in getName for proxy for " + GetP().ToString());
                        return GetC().GetNameForControl(GetP());
                    }
                }

                public IntPtr Hwnd
                {
                    get
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in getHwnd for proxy for " + GetP().ToString());
                        return GetP().Handle;
                    }
                }

                public object Container
                {
                    get
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in getContainer for proxy for " + GetP().ToString());
                        return GetC().GetProxyForContainer();
                    }
                }

                public string Text
                {
                    get
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in getText for proxy for " + GetP().ToString());
                        return GetP().Text;
                    }

                    set
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in setText for proxy for " + GetP().ToString());
                        GetP().Text = value;
                    }
                }

                public void Move(object left, object top, object width, object height)
                {
                }

                // IReflect methods:

                MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
                {
                    return null;
                }

                MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr)
                {
                    return null;
                }

                MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr)
                {
                    return new MethodInfo[] { GetType().GetMethod("Move") };
                }

                FieldInfo IReflect.GetField(string name, BindingFlags bindingAttr)
                {
                    return null;
                }

                FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr)
                {
                    return Array.Empty<FieldInfo>();
                }

                PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr)
                {
                    PropertyInfo prop = GetP().GetType().GetProperty(name, bindingAttr);
                    if (prop == null)
                    {
                        prop = GetType().GetProperty(name, bindingAttr);
                    }
                    return prop;
                }

                PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
                {
                    PropertyInfo prop = GetP().GetType().GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
                    if (prop == null)
                    {
                        prop = GetType().GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
                    }
                    return prop;
                }

                PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr)
                {
                    PropertyInfo[] extenderProps = GetType().GetProperties(bindingAttr);
                    PropertyInfo[] ctlProps = GetP().GetType().GetProperties(bindingAttr);

                    if (extenderProps == null)
                    {
                        return ctlProps;
                    }
                    else if (ctlProps == null)
                    {
                        return extenderProps;
                    }
                    else
                    {
                        int iProp = 0;
                        PropertyInfo[] props = new PropertyInfo[extenderProps.Length + ctlProps.Length];

                        foreach (PropertyInfo prop in extenderProps)
                        {
                            props[iProp++] = prop;
                        }

                        foreach (PropertyInfo prop in ctlProps)
                        {
                            props[iProp++] = prop;
                        }

                        return props;
                    }
                }

                MemberInfo[] IReflect.GetMember(string name, BindingFlags bindingAttr)
                {
                    MemberInfo[] memb = GetP().GetType().GetMember(name, bindingAttr);
                    if (memb == null)
                    {
                        memb = GetType().GetMember(name, bindingAttr);
                    }
                    return memb;
                }

                MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr)
                {
                    MemberInfo[] extenderMembs = GetType().GetMembers(bindingAttr);
                    MemberInfo[] ctlMembs = GetP().GetType().GetMembers(bindingAttr);

                    if (extenderMembs == null)
                    {
                        return ctlMembs;
                    }
                    else if (ctlMembs == null)
                    {
                        return extenderMembs;
                    }
                    else
                    {
                        MemberInfo[] membs = new MemberInfo[extenderMembs.Length + ctlMembs.Length];

                        Array.Copy(extenderMembs, 0, membs, 0, extenderMembs.Length);
                        Array.Copy(ctlMembs, 0, membs, extenderMembs.Length, ctlMembs.Length);

                        return membs;
                    }
                }

                object IReflect.InvokeMember(string name, BindingFlags invokeAttr, Binder binder,
                                             object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
                {
                    try
                    {
                        return GetType().InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
                    }
                    catch (MissingMethodException)
                    {
                        return GetP().GetType().InvokeMember(name, invokeAttr, binder, GetP(), args, modifiers, culture, namedParameters);
                    }
                }

                Type IReflect.UnderlyingSystemType
                {
                    get
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "In UnderlyingSystemType");
                        return null;
                    }
                }
            }
        }

        /// <summary>
        ///  StateConverter is a class that can be used to convert
        ///  State from one data type to another.  Access this
        ///  class through the TypeDescriptor.
        /// </summary>
        public class StateConverter : TypeConverter
        {
            /// <summary>
            ///  Gets a value indicating whether this converter can
            ///  convert an object in the given source type to the native type of the converter
            ///  using the context.
            /// </summary>
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(byte[]))
                {
                    return true;
                }

                return base.CanConvertFrom(context, sourceType);
            }

            /// <summary>
            ///  Gets a value indicating whether this converter can
            ///  convert an object to the given destination type using the context.
            /// </summary>
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(byte[]))
                {
                    return true;
                }

                return base.CanConvertTo(context, destinationType);
            }

            /// <summary>
            ///  Converts the given object to the converter's native type.
            /// </summary>
            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value is byte[])
                {
                    MemoryStream ms = new MemoryStream((byte[])value);
                    return new State(ms);
                }

                return base.ConvertFrom(context, culture, value);
            }

            /// <summary>
            ///  Converts the given object to another type.  The most common types to convert
            ///  are to and from a string object.  The default implementation will make a call
            ///  to ToString on the object if the object is valid and if the destination
            ///  type is string.  If this cannot convert to the desitnation type, this will
            ///  throw a NotSupportedException.
            /// </summary>
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == null)
                {
                    throw new ArgumentNullException(nameof(destinationType));
                }

                if (destinationType == typeof(byte[]))
                {
                    if (value != null)
                    {
                        MemoryStream ms = new MemoryStream();
                        State state = (State)value;
                        state.Save(ms);
                        ms.Close();
                        return ms.ToArray();
                    }
                    else
                    {
                        return Array.Empty<byte>();
                    }
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        /// <summary>
        ///  The class which encapsulates the persisted state of the underlying activeX control
        ///  An instance of this class my be obtained either by calling getOcxState on an
        ///  AxHost object, or by reading in from a stream.
        /// </summary>
        [TypeConverterAttribute(typeof(TypeConverter))]
        [Serializable] // This exchanges with the native code.
        public class State : ISerializable
        {
            private readonly int VERSION = 1;
            private int length;
            private byte[] buffer;
            internal int type;
            private MemoryStream ms;
            private Ole32.IStorage storage;
            private Ole32.ILockBytes iLockBytes;
            private bool manualUpdate = false;
            private string licenseKey = null;
#pragma warning disable IDE1006
            private readonly PropertyBagStream PropertyBagBinary; // Do NOT rename (binary serialization).
#pragma warning restore IDE1006

            // create on save from ipersist stream
            internal State(MemoryStream ms, int storageType, AxHost ctl, PropertyBagStream propBag)
            {
                type = storageType;
                PropertyBagBinary = propBag;
                // dangerous?
                length = (int)ms.Length;
                this.ms = ms;
                manualUpdate = ctl.GetAxState(AxHost.manualUpdate);
                licenseKey = ctl.GetLicenseKey();
            }

            internal State(PropertyBagStream propBag)
            {
                PropertyBagBinary = propBag;
            }

            internal State(MemoryStream ms)
            {
                this.ms = ms;
                length = (int)ms.Length;
                InitializeFromStream(ms);
            }

            // create on init new w/ storage...
            internal State(AxHost ctl)
            {
                CreateStorage();
                manualUpdate = ctl.GetAxState(AxHost.manualUpdate);
                licenseKey = ctl.GetLicenseKey();
                type = STG_STORAGE;
            }

            public State(Stream ms, int storageType, bool manualUpdate, string licKey)
            {
                type = storageType;
                // dangerous?
                length = (int)ms.Length;
                this.manualUpdate = manualUpdate;
                licenseKey = licKey;

                InitializeBufferFromStream(ms);
            }

            /**
             * Constructor used in deserialization
             */
            protected State(SerializationInfo info, StreamingContext context)
            {
                SerializationInfoEnumerator sie = info.GetEnumerator();
                if (sie == null)
                {
                    return;
                }
                for (; sie.MoveNext();)
                {
                    if (string.Compare(sie.Name, "Data", true, CultureInfo.InvariantCulture) == 0)
                    {
                        try
                        {
                            byte[] dat = (byte[])sie.Value;
                            if (dat != null)
                            {
                                InitializeFromStream(new MemoryStream(dat));
                            }

                        }
                        catch (Exception e)
                        {
                            Debug.Fail("failure: " + e.ToString());
                        }
                    }
                    else if (string.Compare(sie.Name, nameof(PropertyBagBinary), true, CultureInfo.InvariantCulture) == 0)
                    {
                        try
                        {
                            Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Loading up property bag from stream...");
                            byte[] dat = (byte[])sie.Value;
                            if (dat != null)
                            {
                                PropertyBagBinary = new PropertyBagStream();
                                PropertyBagBinary.Read(new MemoryStream(dat));
                            }

                        }
                        catch (Exception e)
                        {
                            Debug.Fail("failure: " + e.ToString());
                        }
                    }
                }
            }

            internal int Type
            {
                get
                {
                    return type;
                }
                set
                {
                    type = value;
                }
            }

            internal bool _GetManualUpdate()
            {
                return manualUpdate;
            }

            internal string _GetLicenseKey()
            {
                return licenseKey;
            }

            private void CreateStorage()
            {
                Debug.Assert(storage == null, "but we already have a storage!!!");
                IntPtr hglobal = IntPtr.Zero;
                if (buffer != null)
                {
                    hglobal = UnsafeNativeMethods.GlobalAlloc(NativeMethods.GMEM_MOVEABLE, length);
                    IntPtr pointer = UnsafeNativeMethods.GlobalLock(new HandleRef(null, hglobal));
                    try
                    {
                        if (pointer != IntPtr.Zero)
                        {
                            Marshal.Copy(buffer, 0, pointer, length);
                        }
                    }
                    finally
                    {
                        UnsafeNativeMethods.GlobalUnlock(new HandleRef(null, hglobal));
                    }
                }
                bool failed = false;
                try
                {
                    iLockBytes = Ole32.CreateILockBytesOnHGlobal(hglobal, true);
                    if (buffer == null)
                    {
                        storage = Ole32.StgCreateDocfileOnILockBytes(
                            iLockBytes,
                            Ole32.STGM.STGM_CREATE | Ole32.STGM.STGM_READWRITE | Ole32.STGM.STGM_SHARE_EXCLUSIVE,
                            0);
                    }
                    else
                    {
                        storage = Ole32.StgOpenStorageOnILockBytes(
                            iLockBytes,
                            null,
                            Ole32.STGM.STGM_READWRITE | Ole32.STGM.STGM_SHARE_EXCLUSIVE,
                            IntPtr.Zero,
                            0);
                    }
                }
                catch (Exception t)
                {
                    Debug.Fail(t.ToString());
                    failed = true;
                }
                if (failed)
                {
                    if (iLockBytes == null && hglobal != IntPtr.Zero)
                    {
                        UnsafeNativeMethods.GlobalFree(new HandleRef(null, hglobal));
                    }
                    else
                    {
                        iLockBytes = null;
                    }
                    storage = null;
                }
            }

            internal UnsafeNativeMethods.IPropertyBag GetPropBag()
            {
                return PropertyBagBinary;
            }

            internal Ole32.IStorage GetStorage()
            {
                if (storage == null)
                {
                    CreateStorage();
                }

                return storage;
            }

            internal Ole32.IStream GetStream()
            {
                if (ms == null)
                {
                    Debug.Assert(buffer != null, "gotta have the buffer already...");
                    if (buffer == null)
                    {
                        return null;
                    }

                    ms = new MemoryStream(buffer);
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                }
                return new Ole32.GPStream(ms);
            }

            private void InitializeFromStream(Stream ids)
            {
                BinaryReader br = new BinaryReader(ids);

                type = br.ReadInt32();
                int version = br.ReadInt32();
                manualUpdate = br.ReadBoolean();
                int cc = br.ReadInt32();
                if (cc != 0)
                {
                    licenseKey = new string(br.ReadChars(cc));
                }
                for (int skipUnits = br.ReadInt32(); skipUnits > 0; skipUnits--)
                {
                    int len = br.ReadInt32();
                    ids.Position += len;
                }

                length = br.ReadInt32();
                if (length > 0)
                {
                    buffer = br.ReadBytes(length);
                }
            }

            private void InitializeBufferFromStream(Stream ids)
            {
                BinaryReader br = new BinaryReader(ids);

                length = br.ReadInt32();
                if (length > 0)
                {
                    buffer = br.ReadBytes(length);
                }
            }

            internal State RefreshStorage(Ole32.IPersistStorage iPersistStorage)
            {
                Debug.Assert(storage != null, "how can we not have a storage object?");
                Debug.Assert(iLockBytes != null, "how can we have a storage w/o ILockBytes?");
                if (storage == null || iLockBytes == null)
                {
                    return null;
                }

                iPersistStorage.Save(storage, BOOL.TRUE);
                storage.Commit(0);
                iPersistStorage.HandsOffStorage();
                try
                {
                    buffer = null;
                    ms = null;
                    iLockBytes.Stat(out Ole32.STATSTG stat, Ole32.STATFLAG.STATFLAG_NONAME);
                    length = (int)stat.cbSize;
                    buffer = new byte[length];
                    IntPtr hglobal = Ole32.GetHGlobalFromILockBytes(iLockBytes);
                    IntPtr pointer = UnsafeNativeMethods.GlobalLock(new HandleRef(null, hglobal));
                    try
                    {
                        if (pointer != IntPtr.Zero)
                        {
                            Marshal.Copy(pointer, buffer, 0, length);
                        }
                        else
                        {
                            length = 0;
                            buffer = null;
                        }
                    }
                    finally
                    {
                        UnsafeNativeMethods.GlobalUnlock(new HandleRef(null, hglobal));
                    }
                }
                finally
                {
                    iPersistStorage.SaveCompleted(storage);
                }
                return this;
            }

            internal void Save(MemoryStream stream)
            {
                BinaryWriter bw = new BinaryWriter(stream);

                bw.Write(type);
                bw.Write(VERSION);
                bw.Write(manualUpdate);
                if (licenseKey != null)
                {
                    bw.Write(licenseKey.Length);
                    bw.Write(licenseKey.ToCharArray());
                }
                else
                {
                    bw.Write((int)0);
                }
                bw.Write((int)0); // skip units
                bw.Write(length);
                if (buffer != null)
                {
                    bw.Write(buffer);
                }
                else if (ms != null)
                {
                    ms.Position = 0;
                    ms.WriteTo(stream);
                }
                else
                {
                    Debug.Assert(length == 0, "if we have no data, then our length has to be 0");
                }
            }

            /// <summary>
            ///  ISerializable private implementation
            /// </summary>
            void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
            {
                MemoryStream stream = new MemoryStream();
                Save(stream);

                si.AddValue("Data", stream.ToArray());

                if (PropertyBagBinary != null)
                {
                    try
                    {
                        stream = new MemoryStream();
                        PropertyBagBinary.Write(stream);
                        si.AddValue(nameof(PropertyBagBinary), stream.ToArray());
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Failed to serialize the property bag into ResX : " + e.ToString());
                    }
                }
            }
        }

        internal class PropertyBagStream : UnsafeNativeMethods.IPropertyBag
        {
            private Hashtable bag = new Hashtable();

            internal void Read(Stream stream)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    bag = (Hashtable)formatter.Deserialize(stream);
                }
                catch
                {
                    // Error reading.  Just init an empty hashtable.
                    bag = new Hashtable();
                }
            }

            int UnsafeNativeMethods.IPropertyBag.Read(string pszPropName, ref object pVar, UnsafeNativeMethods.IErrorLog pErrorLog)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Reading property " + pszPropName + " from OCXState propertybag.");

                if (!bag.Contains(pszPropName))
                {
                    return NativeMethods.E_INVALIDARG;
                }

                pVar = bag[pszPropName];
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "\tValue=" + ((pVar == null) ? "<null>" : pVar.ToString()));

                // The EE returns a VT_EMPTY for a null. The problem is that visual basic6 expects the caller to respect the
                // "hint" it gives in the VariantType. For eg., for a VT_BSTR, it expects that the callee will null
                // out the BSTR field of the variant. Since, the EE or us cannot do anything about this, we will return
                // a E_INVALIDARG rather than let visual basic6 crash.
                //
                return (pVar == null) ? NativeMethods.E_INVALIDARG : NativeMethods.S_OK;
            }

            int UnsafeNativeMethods.IPropertyBag.Write(string pszPropName, ref object pVar)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Writing property " + pszPropName + " [" + pVar + "] into OCXState propertybag.");
                if (pVar != null && !pVar.GetType().IsSerializable)
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "\t " + pVar.GetType().FullName + " is not serializable.");
                    return NativeMethods.S_OK;
                }

                bag[pszPropName] = pVar;
                return NativeMethods.S_OK;
            }

            internal void Write(Stream stream)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, bag);
            }
        }

        protected delegate void AboutBoxDelegate();

        [ComVisible(false)]
        public class AxComponentEditor : WindowsFormsComponentEditor
        {
            public override bool EditComponent(ITypeDescriptorContext context, object obj, IWin32Window parent)
            {
                if (obj is AxHost host)
                {
                    try
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in AxComponentEditor.EditComponent");
                        ((UnsafeNativeMethods.IOleControlSite)host.oleSite).ShowPropertyFrame();
                        return true;
                    }
                    catch (Exception t)
                    {
                        Debug.Fail(t.ToString());
                        throw;
                    }
                }
                return false;
            }
        }

        internal class AxPropertyDescriptor : PropertyDescriptor
        {
            private readonly PropertyDescriptor baseProp;
            internal AxHost owner;
            private readonly DispIdAttribute dispid;

            private TypeConverter converter;
            private UITypeEditor editor;
            private readonly ArrayList updateAttrs = new ArrayList();
            private int flags = 0;

            private const int FlagUpdatedEditorAndConverter = 0x00000001;
            private const int FlagCheckGetter = 0x00000002;
            private const int FlagGettterThrew = 0x00000004;
            private const int FlagIgnoreCanAccessProperties = 0x00000008;
            private const int FlagSettingValue = 0x00000010;

            internal AxPropertyDescriptor(PropertyDescriptor baseProp, AxHost owner) : base(baseProp)
            {
                this.baseProp = baseProp;
                this.owner = owner;

                // Get the category for this dispid.
                //
                dispid = (DispIdAttribute)baseProp.Attributes[typeof(DispIdAttribute)];
                if (dispid != null)
                {
                    // Look to see if this property has a property page.
                    // If it does, then it needs to be Browsable(true).
                    //
                    if (!IsBrowsable && !IsReadOnly)
                    {
                        Guid g = GetPropertyPage(dispid.Value);

                        if (!Guid.Empty.Equals(g))
                        {
                            Debug.WriteLineIf(AxPropTraceSwitch.TraceVerbose, "Making property: " + Name + " browsable because we found an property page.");
                            AddAttribute(new BrowsableAttribute(true));
                        }
                    }

                    // Use the CategoryAttribute provided by the OCX.
                    //
                    CategoryAttribute cat = owner.GetCategoryForDispid(dispid.Value);
                    if (cat != null)
                    {
                        AddAttribute(cat);
                    }

                    // Check to see if this a DataSource property.
                    // If it is, we can always get and set the value of this property.
                    //
                    if (PropertyType.GUID.Equals(dataSource_Guid))
                    {
                        SetFlag(FlagIgnoreCanAccessProperties, true);
                    }
                }
            }

            public override Type ComponentType
            {
                get
                {
                    return baseProp.ComponentType;
                }
            }

            public override TypeConverter Converter
            {
                get
                {

                    if (dispid != null)
                    {
                        UpdateTypeConverterAndTypeEditorInternal(false, Dispid);
                    }
                    return converter ?? base.Converter;
                }
            }

            internal int Dispid
            {
                get
                {
                    DispIdAttribute dispid = (DispIdAttribute)baseProp.Attributes[typeof(DispIdAttribute)];
                    if (dispid != null)
                    {
                        return dispid.Value;
                    }

                    return NativeMethods.ActiveX.DISPID_UNKNOWN;
                }
            }

            public override bool IsReadOnly
            {
                get
                {
                    return baseProp.IsReadOnly;
                }
            }

            public override Type PropertyType
            {
                get
                {
                    return baseProp.PropertyType;
                }
            }

            internal bool SettingValue
            {
                get
                {
                    return GetFlag(FlagSettingValue);
                }
            }

            private void AddAttribute(Attribute attr)
            {
                updateAttrs.Add(attr);
            }

            public override bool CanResetValue(object o)
            {
                return baseProp.CanResetValue(o);
            }

            public override object GetEditor(Type editorBaseType)
            {

                UpdateTypeConverterAndTypeEditorInternal(false, dispid.Value);

                if (editorBaseType.Equals(typeof(UITypeEditor)) && editor != null)
                {
                    return editor;
                }

                return base.GetEditor(editorBaseType);
            }

            private bool GetFlag(int flagValue)
            {
                return ((flags & flagValue) == flagValue);
            }

            private Guid GetPropertyPage(int dispid)
            {
                try
                {
                    NativeMethods.IPerPropertyBrowsing ippb = owner.GetPerPropertyBrowsing();
                    if (ippb == null)
                    {
                        return Guid.Empty;
                    }

                    if (NativeMethods.Succeeded(ippb.MapPropertyToPage(dispid, out Guid rval)))
                    {
                        return rval;
                    }
                }
                catch (COMException)
                {
                }
                catch (Exception t)
                {
                    Debug.Fail(t.ToString());
                }
                return Guid.Empty;
            }

            public override object GetValue(object component)
            {
                if ((!GetFlag(FlagIgnoreCanAccessProperties) && !owner.CanAccessProperties) || GetFlag(FlagGettterThrew))
                {
                    return null;
                }

                try
                {
                    // Some controls fire OnChanged() notifications when getting values of some properties.
                    // To prevent this kind of recursion, we check to see if we are already inside a OnChanged() call.
                    //
                    owner.NoComponentChangeEvents++;
                    return baseProp.GetValue(component);
                }
                catch (Exception e)
                {
                    if (!GetFlag(FlagCheckGetter))
                    {
                        Debug.WriteLineIf(AxPropTraceSwitch.TraceVerbose, "Get failed for : " + Name + " with exception: " + e.Message + " .Making property non-browsable.");
                        SetFlag(FlagCheckGetter, true);
                        AddAttribute(new BrowsableAttribute(false));
                        owner.RefreshAllProperties = true;
                        SetFlag(FlagGettterThrew, true);
                    }
                    throw e;
                }
                finally
                {
                    owner.NoComponentChangeEvents--;
                }
            }

            public void OnValueChanged(object component)
            {
                OnValueChanged(component, EventArgs.Empty);
            }

            public override void ResetValue(object o)
            {
                baseProp.ResetValue(o);
            }

            private void SetFlag(int flagValue, bool value)
            {
                if (value)
                {
                    flags |= flagValue;
                }
                else
                {
                    flags &= ~flagValue;
                }
            }

            public override void SetValue(object component, object value)
            {
                if (!GetFlag(FlagIgnoreCanAccessProperties) && !owner.CanAccessProperties)
                {
                    return;
                }

                // State oldOcxState = owner.OcxState;

                try
                {
                    SetFlag(FlagSettingValue, true);
                    if (PropertyType.IsEnum && (value.GetType() != PropertyType))
                    {
                        baseProp.SetValue(component, Enum.ToObject(PropertyType, value));
                    }
                    else
                    {
                        baseProp.SetValue(component, value);
                    }
                }
                finally
                {
                    SetFlag(FlagSettingValue, false);
                }

                OnValueChanged(component);
                if (owner == component)
                {
                    owner.SetAxState(AxHost.valueChanged, true);
                }
            }

            public override bool ShouldSerializeValue(object o)
            {
                return baseProp.ShouldSerializeValue(o);
            }

            internal void UpdateAttributes()
            {
                if (updateAttrs.Count == 0)
                {
                    return;
                }

                ArrayList attributes = new ArrayList(AttributeArray);
                foreach (Attribute attr in updateAttrs)
                {
                    attributes.Add(attr);
                }

                Attribute[] temp = new Attribute[attributes.Count];
                attributes.CopyTo(temp, 0);
                AttributeArray = temp;

                updateAttrs.Clear();
            }

            /// <summary>
            ///  Called externally to update the editor or type converter.
            ///  This simply sets flags so this will happen, it doesn't actually to the update...
            ///  we wait and do that on-demand for perf.
            /// </summary>
            internal void UpdateTypeConverterAndTypeEditor(bool force)
            {
                // if this is an external request, flip the flag to false so we do the update on demand.
                //
                if (GetFlag(FlagUpdatedEditorAndConverter) && force)
                {
                    SetFlag(FlagUpdatedEditorAndConverter, false);
                }
            }

            /// <summary>
            ///  Called externally to update the editor or type converter.
            ///  This simply sets flags so this will happen, it doesn't actually to the update...
            ///  we wait and do that on-demand for perf.
            /// </summary>
            internal void UpdateTypeConverterAndTypeEditorInternal(bool force, int dispid)
            {

                // check to see if we're being forced here or if the work really
                // needs to be done.
                //
                if (GetFlag(FlagUpdatedEditorAndConverter) && !force)
                {
                    return;
                }

                if (owner.GetOcx() == null)
                {
                    return;
                }

                try
                {
                    NativeMethods.IPerPropertyBrowsing ppb = owner.GetPerPropertyBrowsing();

                    if (ppb != null)
                    {
                        bool hasStrings = false;

                        // check for enums
                        NativeMethods.CA_STRUCT caStrings = new NativeMethods.CA_STRUCT();
                        NativeMethods.CA_STRUCT caCookies = new NativeMethods.CA_STRUCT();

                        int hr = NativeMethods.S_OK;

                        try
                        {
                            hr = ppb.GetPredefinedStrings(dispid, caStrings, caCookies);
                        }
                        catch (ExternalException ex)
                        {
                            hr = ex.ErrorCode;
                            Debug.Fail("An exception occurred inside IPerPropertyBrowsing::GetPredefinedStrings(dispid=" +
                                       dispid + "), object type=" + new ComNativeDescriptor().GetClassName(ppb));
                        }

                        if (hr != NativeMethods.S_OK)
                        {
                            hasStrings = false;
                            // Destroy the existing editor if we created the current one
                            // so if the items have disappeared, we don't hold onto the old
                            // items.
                            if (converter is Com2EnumConverter)
                            {
                                converter = null;
                            }
                        }
                        else
                        {
                            hasStrings = true;
                        }

                        if (hasStrings)
                        {
                            OleStrCAMarshaler stringMarshaler = new OleStrCAMarshaler(caStrings);
                            Int32CAMarshaler intMarshaler = new Int32CAMarshaler(caCookies);

                            if (stringMarshaler.Count > 0 && intMarshaler.Count > 0)
                            {
                                if (converter == null)
                                {
                                    converter = new AxEnumConverter(this, new AxPerPropertyBrowsingEnum(this, owner, stringMarshaler, intMarshaler, true));
                                }
                                else if (converter is AxEnumConverter)
                                {
                                    ((AxEnumConverter)converter).RefreshValues();
                                    if (((AxEnumConverter)converter).com2Enum is AxPerPropertyBrowsingEnum axEnum)
                                    {
                                        axEnum.RefreshArrays(stringMarshaler, intMarshaler);
                                    }

                                }

                            }
                            else
                            {
                                //hasStrings = false;
                            }
                        }
                        else
                        {
                            // if we didn't get any strings, try the proppage edtior
                            //
                            // Check to see if this is a property that we have already massaged to be a
                            // .Net type. If it is, don't bother with custom property pages. We already
                            // have a .Net Editor for this type.
                            //
                            ComAliasNameAttribute comAlias = (ComAliasNameAttribute)baseProp.Attributes[typeof(ComAliasNameAttribute)];
                            if (comAlias == null)
                            {
                                Guid g = GetPropertyPage(dispid);

                                if (!Guid.Empty.Equals(g))
                                {
                                    editor = new AxPropertyTypeEditor(this, g);

                                    // Show any non-browsable property that has an editor through a
                                    // property page.
                                    //
                                    if (!IsBrowsable)
                                    {
                                        Debug.WriteLineIf(AxPropTraceSwitch.TraceVerbose, "Making property: " + Name + " browsable because we found an editor.");
                                        AddAttribute(new BrowsableAttribute(true));
                                    }
                                }
                            }
                        }
                    }

                    SetFlag(FlagUpdatedEditorAndConverter, true);
                }
                catch (Exception e)
                {
                    Debug.WriteLineIf(AxPropTraceSwitch.TraceVerbose, "could not get the type editor for property: " + Name + " Exception: " + e);
                }
            }
        }

        private class AxPropertyTypeEditor : UITypeEditor
        {
            private readonly AxPropertyDescriptor propDesc;
            private Guid guid;

            public AxPropertyTypeEditor(AxPropertyDescriptor pd, Guid guid)
            {
                propDesc = pd;
                this.guid = guid;
            }

            /// <summary>
            ///  Takes the value returned from valueAccess.getValue() and modifies or replaces
            ///  the value, passing the result into valueAccess.setValue().  This is where
            ///  an editor can launch a modal dialog or create a drop down editor to allow
            ///  the user to modify the value.  Host assistance in presenting UI to the user
            ///  can be found through the valueAccess.getService function.
            /// </summary>
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                try
                {
                    object instance = context.Instance;
                    propDesc.owner.ShowPropertyPageForDispid(propDesc.Dispid, guid);
                }
                catch (Exception ex1)
                {
                    if (provider != null)
                    {
                        IUIService uiSvc = (IUIService)provider.GetService(typeof(IUIService));
                        if (uiSvc != null)
                        {
                            uiSvc.ShowError(ex1, SR.ErrorTypeConverterFailed);
                        }
                    }
                }
                return value;
            }

            /// <summary>
            ///  Retrieves the editing style of the Edit method.  If the method
            ///  is not supported, this will return None.
            /// </summary>
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }
        }

        /// <summary>
        ///  simple derivation of the com2enumconverter that allows us to intercept
        ///  the call to GetStandardValues so we can on-demand update the enum values.
        /// </summary>
        private class AxEnumConverter : Com2EnumConverter
        {
            private readonly AxPropertyDescriptor target;

            public AxEnumConverter(AxPropertyDescriptor target, Com2Enum com2Enum) : base(com2Enum)
            {
                this.target = target;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {

                // make sure the converter has been properly refreshed -- calling
                // the Converter property does this.
                //
                TypeConverter tc = this;
                tc = target.Converter;
                return base.GetStandardValues(context);

            }

        }

        // This exists for perf reasons.   We delay doing this until we
        // are actually asked for the array of values.
        //
        private class AxPerPropertyBrowsingEnum : Com2Enum
        {
            private readonly AxPropertyDescriptor target;
            private readonly AxHost owner;
            private OleStrCAMarshaler nameMarshaller;
            private Int32CAMarshaler valueMarshaller;
            private bool arraysFetched;

            public AxPerPropertyBrowsingEnum(AxPropertyDescriptor targetObject, AxHost owner, OleStrCAMarshaler names, Int32CAMarshaler values, bool allowUnknowns) : base(Array.Empty<string>(), Array.Empty<object>(), allowUnknowns)
            {
                target = targetObject;
                nameMarshaller = names;
                valueMarshaller = values;
                this.owner = owner;
                arraysFetched = false;
            }

            /// <summary>
            ///  Retrieve a copy of the value array
            /// </summary>
            public override object[] Values
            {
                get
                {
                    EnsureArrays();
                    return base.Values;
                }
            }

            /// <summary>
            ///  Retrieve a copy of the nme array.
            /// </summary>
            public override string[] Names
            {
                get
                {
                    EnsureArrays();
                    return base.Names;
                }
            }

            // ensure that we have processed the caStructs into arrays
            // of values and strings
            //
            private void EnsureArrays()
            {
                if (arraysFetched)
                {
                    return;
                }

                arraysFetched = true;

                try
                {

                    // marshal the items.
                    object[] nameItems = nameMarshaller.Items;
                    object[] cookieItems = valueMarshaller.Items;
                    NativeMethods.IPerPropertyBrowsing ppb = (NativeMethods.IPerPropertyBrowsing)owner.GetPerPropertyBrowsing();
                    int itemCount = 0;

                    Debug.Assert(cookieItems != null && nameItems != null, "An item array is null");

                    if (nameItems.Length > 0)
                    {
                        object[] valueItems = new object[cookieItems.Length];
                        NativeMethods.VARIANT var = new NativeMethods.VARIANT();
                        int cookie;

                        Debug.Assert(cookieItems.Length == nameItems.Length, "Got uneven names and cookies");

                        // for each name item, we ask the object for it's corresponding value.
                        //
                        for (int i = 0; i < nameItems.Length; i++)
                        {
                            cookie = (int)cookieItems[i];
                            if (nameItems[i] == null || !(nameItems[i] is string))
                            {
                                Debug.Fail("Bad IPerPropertyBrowsing item [" + i.ToString(CultureInfo.InvariantCulture) + "], name=" + (nameItems == null ? "(unknown)" : nameItems[i].ToString()));
                                continue;
                            }
                            var.vt = (short)NativeMethods.tagVT.VT_EMPTY;
                            int hr = ppb.GetPredefinedValue(target.Dispid, cookie, var);
                            if (hr == NativeMethods.S_OK && var.vt != (short)NativeMethods.tagVT.VT_EMPTY)
                            {
                                valueItems[i] = var.ToObject();
                            }
                            var.Clear();
                            itemCount++;
                        }

                        // pass this data down to the base Com2Enum object...
                        if (itemCount > 0)
                        {
                            string[] strings = new string[itemCount];
                            Array.Copy(nameItems, 0, strings, 0, itemCount);
                            base.PopulateArrays(strings, valueItems);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Fail("Failed to build IPerPropertyBrowsing editor. " + ex.GetType().Name + ", " + ex.Message);
                }
            }

            internal void RefreshArrays(OleStrCAMarshaler names, Int32CAMarshaler values)
            {
                nameMarshaller = names;
                valueMarshaller = values;
                arraysFetched = false;
            }

            protected override void PopulateArrays(string[] names, object[] values)
            {
                // we call base.PopulateArrays directly when we actually want to do this.
            }

            public override object FromString(string s)
            {
                EnsureArrays();
                return base.FromString(s);
            }

            public override string ToString(object v)
            {
                EnsureArrays();
                return base.ToString(v);
            }
        }
    }
}
