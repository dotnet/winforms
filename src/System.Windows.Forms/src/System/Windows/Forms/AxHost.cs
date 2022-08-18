// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Wraps ActiveX controls and exposes them as fully featured windows forms controls.
    /// </summary>
    [ToolboxItem(false)]
    [DesignTimeVisible(false)]
    [DefaultEvent(nameof(Enter))]
    [Designer($"System.Windows.Forms.Design.AxHostDesigner, {AssemblyRef.SystemDesign}")]
    public abstract partial class AxHost : Control, ISupportInitialize, ICustomTypeDescriptor
    {
        private static readonly TraceSwitch s_axHTraceSwitch = new("AxHTrace", "ActiveX handle tracing");
        private static readonly TraceSwitch s_axPropTraceSwitch = new("AxPropTrace", "ActiveX property tracing");
        private static readonly TraceSwitch s_axHostSwitch = new("AxHost", "ActiveX host creation");
#if DEBUG
        private static readonly BooleanSwitch s_axAlwaysSaveSwitch = new(
            "AxAlwaysSave",
            "ActiveX to save all controls regardless of their IsDirty function return value");
#endif

        /// <summary>
        ///  Flags which may be passed to the AxHost constructor
        /// </summary>
        internal static class AxFlags
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
            ///  [Since most activeX controls already have their own properties verb
            ///  on the context menu, the default is not to include one specified by
            ///  this flag.]
            /// </summary>
            internal const int IncludePropertiesVerb = 0x2;

            /// <summary>
            /// </summary>
            internal const int IgnoreThreadModel = 0x10000000;
        }

        // E_INVALID_ARG
        private static readonly COMException s_invalidArgumentException = new(SR.AXInvalidArgument, unchecked((int)0x80070057));

        // E_FAIL
        private static readonly COMException s_unknownErrorException = new(SR.AXUnknownError, unchecked((int)0x80004005));

        private const int OC_PASSIVE = 0;
        private const int OC_LOADED = 1;    // handler, but no server   [ocx created]
        private const int OC_RUNNING = 2;   // server running, invisible [iqa & depersistance]
        private const int OC_INPLACE = 4;   // server in-place active [inplace]
        private const int OC_UIACTIVE = 8;  // server is UI active [uiactive]
        private const int OC_OPEN = 16;     // server is being open edited [not used]

        private const int EDITM_NONE = 0;   // object not being edited
        private const int EDITM_OBJECT = 1; // object provided an edit verb and we invoked it
        private const int EDITM_HOST = 2;   // we invoked our own edit verb

        private const int STG_UNKNOWN = -1;
        private const int STG_STREAM = 0;
        private const int STG_STREAMINIT = 1;
        private const int STG_STORAGE = 2;

        private readonly User32.WM _registeredMessage = User32.RegisterWindowMessageW($"{Application.WindowMessagesVersion}_subclassCheck");
        private const int REGMSG_RETVAL = 123;

        private static int s_logPixelsX = -1;
        private static int s_logPixelsY = -1;

        private static readonly Guid s_icf2_Guid = typeof(Ole32.IClassFactory2).GUID;
        private static readonly Guid s_ifont_Guid = typeof(Ole32.IFont).GUID;
        private static readonly Guid s_ifontDisp_Guid = typeof(Ole32.IFontDisp).GUID;
        private static readonly Guid s_ipicture_Guid = typeof(Ole32.IPicture).GUID;
        private static readonly Guid s_ipictureDisp_Guid = typeof(Ole32.IPictureDisp).GUID;
        private static readonly Guid s_ivbformat_Guid = typeof(Ole32.IVBFormat).GUID;
        private static readonly Guid s_ioleobject_Guid = typeof(Ole32.IOleObject).GUID;
        private static readonly Guid s_dataSource_Guid = new("{7C0FFAB3-CD84-11D0-949A-00A0C91110ED}");
        private static readonly Guid s_windowsMediaPlayer_Clsid = new("{22d6f312-b0f6-11d0-94ab-0080c74c7e95}");
        private static readonly Guid s_comctlImageCombo_Clsid = new("{a98a24c0-b06f-3684-8c12-c52ae341e0bc}");
        private static readonly Guid s_maskEdit_Clsid = new("{c932ba85-4374-101b-a56c-00aa003668dc}");

        // Static state for perf optimization
        private static Dictionary<Font, Oleaut32.FONTDESC> s_fontTable;

        // BitVector32 masks for various internal state flags.
        private static readonly int s_ocxStateSet = BitVector32.CreateMask();
        private static readonly int s_editorRefresh = BitVector32.CreateMask(s_ocxStateSet);
        private static readonly int s_listeningToIdle = BitVector32.CreateMask(s_editorRefresh);
        private static readonly int s_refreshProperties = BitVector32.CreateMask(s_listeningToIdle);

        private static readonly int s_checkedIppb = BitVector32.CreateMask(s_refreshProperties);
        private static readonly int s_checkedCP = BitVector32.CreateMask(s_checkedIppb);
        private static readonly int s_fNeedOwnWindow = BitVector32.CreateMask(s_checkedCP);
        private static readonly int s_fOwnWindow = BitVector32.CreateMask(s_fNeedOwnWindow);

        private static readonly int s_fSimpleFrame = BitVector32.CreateMask(s_fOwnWindow);
        private static readonly int s_fFakingWindow = BitVector32.CreateMask(s_fSimpleFrame);
        private static readonly int s_rejectSelection = BitVector32.CreateMask(s_fFakingWindow);
        private static readonly int s_ownDisposing = BitVector32.CreateMask(s_rejectSelection);

        private static readonly int s_sinkAttached = BitVector32.CreateMask(s_ownDisposing);
        private static readonly int s_disposed = BitVector32.CreateMask(s_sinkAttached);
        private static readonly int s_manualUpdate = BitVector32.CreateMask(s_disposed);
        private static readonly int s_addedSelectionHandler = BitVector32.CreateMask(s_manualUpdate);

        private static readonly int s_valueChanged = BitVector32.CreateMask(s_addedSelectionHandler);
        private static readonly int s_handlePosRectChanged = BitVector32.CreateMask(s_valueChanged);
        private static readonly int s_siteProcessedInputKey = BitVector32.CreateMask(s_handlePosRectChanged);
        private static readonly int s_needLicenseKey = BitVector32.CreateMask(s_siteProcessedInputKey);

        private static readonly int s_inTransition = BitVector32.CreateMask(s_needLicenseKey);
        private static readonly int s_processingKeyUp = BitVector32.CreateMask(s_inTransition);
        private static readonly int s_assignUniqueID = BitVector32.CreateMask(s_processingKeyUp);
        private static readonly int s_renameEventHooked = BitVector32.CreateMask(s_assignUniqueID);

        private BitVector32 _axState;

        private int _storageType = STG_UNKNOWN;
        private int _ocState = OC_PASSIVE;
        private Ole32.OLEMISC _miscStatusBits;
        private int _freezeCount;
        private readonly int _flags;
        private int _selectionStyle;
        private int _editMode = EDITM_NONE;
        private int _noComponentChange;

        private IntPtr _wndprocAddr = IntPtr.Zero;

        private readonly Guid _clsid;
        private string _text = string.Empty;
        private string _licenseKey;

        private readonly OleInterfaces _oleSite;
        private AxComponentEditor _editor;
        private AxContainer _container;
        private ContainerControl _containingControl;
        private ContainerControl _newParent;
        private AxContainer _axContainer;
        private State _ocxState;
        private HWND _hwndFocus;

        // CustomTypeDescriptor related state

        private Hashtable _properties;
        private Hashtable _propertyInfos;
        private PropertyDescriptorCollection _propsStash;
        private Attribute[] _attribsStash;

        // Interface pointers to the ocx

        private object _instance;
        private Ole32.IOleInPlaceObject _iOleInPlaceObject;
        private Ole32.IOleObject _iOleObject;
        private Ole32.IOleControl _iOleControl;
        private Ole32.IOleInPlaceActiveObject _iOleInPlaceActiveObject;
        private Ole32.IOleInPlaceActiveObject _iOleInPlaceActiveObjectExternal;
        private Oleaut32.IPerPropertyBrowsing _iPerPropertyBrowsing;
        private VSSDK.ICategorizeProperties _iCategorizeProperties;
        private Oleaut32.IPersistPropertyBag _iPersistPropBag;
        private Ole32.IPersistStream _iPersistStream;
        private Ole32.IPersistStreamInit _iPersistStreamInit;
        private Ole32.IPersistStorage _iPersistStorage;

        private AboutBoxDelegate _aboutBoxDelegate;
        private readonly EventHandler _selectionChangeHandler;

        private readonly bool _isMaskEdit;
        private bool _ignoreDialogKeys;

        private readonly EventHandler _onContainerVisibleChanged;

        // These should be in the order given by the PROPCAT_X values
        // Also, note that they are not to be localized...

        private static readonly CategoryAttribute[] s_categoryNames = new CategoryAttribute[]
        {
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

        private Hashtable _objectDefinedCategoryNames; // Integer -> String

#if DEBUG
        static AxHost()
        {
            Debug.Assert(DockStyle.None == NativeMethods.ActiveX.ALIGN_NO_CHANGE, "align value mismatch");
            Debug.Assert((int)DockStyle.Top == NativeMethods.ActiveX.ALIGN_TOP, "align value mismatch");
            Debug.Assert((int)DockStyle.Bottom == NativeMethods.ActiveX.ALIGN_BOTTOM, "align value mismatch");
            Debug.Assert((int)DockStyle.Left == NativeMethods.ActiveX.ALIGN_LEFT, "align value mismatch");
            Debug.Assert((int)DockStyle.Right == NativeMethods.ActiveX.ALIGN_RIGHT, "align value mismatch");
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

            _oleSite = new OleInterfaces(this);
            _selectionChangeHandler = new EventHandler(OnNewSelection);
            _clsid = new Guid(clsid);
            _flags = flags;

            _axState[s_assignUniqueID] = !GetType().GUID.Equals(s_comctlImageCombo_Clsid);
            _axState[s_needLicenseKey] = true;
            _axState[s_rejectSelection] = true;

            _isMaskEdit = _clsid.Equals(s_maskEdit_Clsid);
            _onContainerVisibleChanged = new EventHandler(OnContainerVisibleChanged);
        }

        private bool CanUIActivate => IsUserMode() || _editMode != EDITM_NONE;

        /// <summary>
        ///  Returns the CreateParams used to create the handle for this control.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (_axState[s_fOwnWindow] && IsUserMode())
                {
                    cp.Style &= ~(int)WINDOW_STYLE.WS_VISIBLE;
                }

                return cp;
            }
        }

        private bool GetAxState(int mask)
        {
            return _axState[mask];
        }

        private void SetAxState(int mask, bool value)
        {
            _axState[mask] = value;
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
            if (hr.Succeeded)
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
            get => _text;
            set => _text = value;
        }

        internal override bool CanAccessProperties
        {
            get
            {
                int ocState = GetOcState();
                return (_axState[s_fOwnWindow] && (ocState > OC_RUNNING || (IsUserMode() && ocState >= OC_RUNNING))) ||
                       ocState >= OC_INPLACE;
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
            if (ParentInternal is not null)
            {
                ParentInternal.CreateControl(true);

                ContainerControl f = ContainingControl;
                if (f is not null)
                {
                    f.VisibleChanged += _onContainerVisibleChanged;
                }
            }
        }

        private void OnContainerVisibleChanged(object sender, EventArgs e)
        {
            ContainerControl f = ContainingControl;
            if (f is not null)
            {
                if (f.Visible && Visible && !_axState[s_fOwnWindow])
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

        /// <summary>
        ///  Determines if the control is in edit mode.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EditMode => _editMode != EDITM_NONE;

        /// <summary>
        ///  Determines if this control has an about box.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasAboutBox => _aboutBoxDelegate is not null;

        private int NoComponentChangeEvents
        {
            get => _noComponentChange;
            set => _noComponentChange = value;
        }

        /// <summary>
        ///  Shows the about box for this control.
        /// </summary>
        public void ShowAboutBox()
        {
            _aboutBoxDelegate?.Invoke();
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
        ///  Occurs when the mouse pointer is over the control and a mouse button is pressed.
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
        ///  Occurs when the mouse pointer hovers over the control.
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
            if (GetOcx() is not null)
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
            return _axState[s_fOwnWindow] || _axState[s_fFakingWindow];
        }

        private HWND GetHandleNoCreate() => IsHandleCreated ? (HWND)Handle : default;

        private ISelectionService GetSelectionService()
        {
            return GetSelectionService(this);
        }

        private static ISelectionService GetSelectionService(Control ctl)
            => ctl.Site?.GetService(typeof(ISelectionService)) as ISelectionService;

        private void AddSelectionHandler()
        {
            if (_axState[s_addedSelectionHandler])
            {
                return;
            }

            ISelectionService iss = GetSelectionService();
            if (iss is not null)
            {
                iss.SelectionChanging += _selectionChangeHandler;
            }

            _axState[s_addedSelectionHandler] = true;
        }

        private void OnComponentRename(object sender, ComponentRenameEventArgs e)
        {
            // When we're notified of a rename, see if this is the component that is being renamed.
            if (e.Component == this)
            {
                // If it is, call DISPID_AMBIENT_DISPLAYNAME directly on the control itself.
                if (GetOcx() is Ole32.IOleControl oleCtl)
                {
                    oleCtl.OnAmbientPropertyChange(Ole32.DispatchID.AMBIENT_DISPLAYNAME);
                }
            }
        }

        private bool RemoveSelectionHandler()
        {
            if (!_axState[s_addedSelectionHandler])
            {
                return false;
            }

            ISelectionService iss = GetSelectionService();
            if (iss is not null)
            {
                iss.SelectionChanging -= _selectionChangeHandler;
            }

            _axState[s_addedSelectionHandler] = false;
            return true;
        }

        private void SyncRenameNotification(bool hook)
        {
            if (DesignMode && hook != _axState[s_renameEventHooked])
            {
                // If we're in design mode, listen to the following events from the component change service.
                IComponentChangeService changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

                if (changeService is not null)
                {
                    if (hook)
                    {
                        changeService.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
                    }
                    else
                    {
                        changeService.ComponentRename -= new ComponentRenameEventHandler(OnComponentRename);
                    }

                    _axState[s_renameEventHooked] = hook;
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
                if (_axState[s_disposed])
                {
                    return;
                }

                bool reAddHandler = RemoveSelectionHandler();
                bool olduMode = IsUserMode();

                // Clear the old hook
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

                SyncRenameNotification(value is not null);

                // For inherited forms we create the OCX first in User mode
                // and then we get sited. At that time, we have to re-activate
                // the OCX by transitioning down to and up to the current state.

                if (value is not null && !newuMode && olduMode != newuMode && GetOcState() > OC_LOADED)
                {
                    TransitionDownTo(OC_LOADED);
                    TransitionUpTo(OC_INPLACE);
                    ContainerControl f = ContainingControl;
                    if (f is not null && f.Visible && Visible)
                    {
                        MakeVisibleWithShow();
                    }
                }

                if (olduMode != newuMode && !IsHandleCreated && !_axState[s_disposed])
                {
                    if (GetOcx() is not null)
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
        ///  Raises the <see cref="Control.LostFocus"/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnLostFocus(EventArgs e)
        {
            // Office WebControl and MS DDS control create a child window that gains
            // focus in order to handle keyboard input. Since, UIDeactivate() could
            // destroy that window, these controls will crash trying to process WM_CHAR.
            // We now check to see if we are losing focus to a child, and if so, not call
            // UIDeactivate().
            bool uiDeactivate = GetHandleNoCreate() != _hwndFocus;
            if (uiDeactivate && IsHandleCreated)
            {
                uiDeactivate = !PInvoke.IsChild(this, _hwndFocus);
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
            if (iss is not null)
            {
                if (GetOcState() >= OC_UIACTIVE && !iss.GetComponentSelected(this))
                {
                    // Need to deactivate.
                    HRESULT hr = UiDeactivate();
                    Debug.Assert(hr.Succeeded, $"Failed to UiDeactivate: {hr}");
                }

                if (!iss.GetComponentSelected(this))
                {
                    if (_editMode != EDITM_NONE)
                    {
                        GetParentContainer().OnExitEditMode(this);
                        _editMode = EDITM_NONE;
                    }

                    // Need to exit edit mode.
                    SetSelectionStyle(1);
                    RemoveSelectionHandler();
                }
                else
                {
                    // The AX Host designer will offer an extender property called "SelectionStyle"
                    PropertyDescriptor prop = TypeDescriptor.GetProperties(this)["SelectionStyle"];

                    if (prop is not null && prop.PropertyType == typeof(int))
                    {
                        int curSelectionStyle = (int)prop.GetValue(this);
                        if (curSelectionStyle != _selectionStyle)
                        {
                            prop.SetValue(this, _selectionStyle);
                        }
                    }
                }
            }
        }

        // DrawToBitmap doesn't work for this control, so we should hide it.  We'll
        // still call base so that this has a chance to work if it can.
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
            if (!_axState[s_fOwnWindow])
            {
                if (_axState[s_fNeedOwnWindow])
                {
                    Debug.Assert(!Visible, "if we were visible we would not be needing a fake window...");
                    _axState[s_fNeedOwnWindow] = false;
                    _axState[s_fFakingWindow] = true;
                    base.CreateHandle();
                    // note that we do not need to attach the handle because the work usually done in there
                    // will be done in Control's wndProc on WM_CREATE...
                }
                else
                {
                    TransitionUpTo(OC_INPLACE);
                    // it is possible that we were hidden while in place activating, in which case we don't
                    // really have a handle now because the act of hiding could have destroyed it
                    // so, just call ourselves again recursively, and if we don't have a handle, we will
                    // just take the "axState[fNeedOwnWindow]" path above...
                    if (_axState[s_fNeedOwnWindow])
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
            if (s_logPixelsX == -1 || force)
            {
                using var dc = User32.GetDcScope.ScreenDC;
                if (dc == IntPtr.Zero)
                {
                    return HRESULT.Values.E_FAIL;
                }

                s_logPixelsX = PInvoke.GetDeviceCaps(dc, GET_DEVICE_CAPS_INDEX.LOGPIXELSX);
                s_logPixelsY = PInvoke.GetDeviceCaps(dc, GET_DEVICE_CAPS_INDEX.LOGPIXELSY);
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"log pixels are: {s_logPixelsX} {s_logPixelsY}");
            }

            return HRESULT.Values.S_OK;
        }

        private unsafe void HiMetric2Pixel(ref Size sz)
        {
            var phm = new Point(sz.Width, sz.Height);
            var pcont = new PointF();
            ((Ole32.IOleControlSite)_oleSite).TransformCoords(&phm, &pcont, Ole32.XFORMCOORDS.SIZE | Ole32.XFORMCOORDS.HIMETRICTOCONTAINER);
            sz.Width = (int)pcont.X;
            sz.Height = (int)pcont.Y;
        }

        private unsafe void Pixel2hiMetric(ref Size sz)
        {
            var phm = new Point();
            var pcont = new PointF(sz.Width, sz.Height);
            ((Ole32.IOleControlSite)_oleSite).TransformCoords(&phm, &pcont, Ole32.XFORMCOORDS.SIZE | Ole32.XFORMCOORDS.CONTAINERTOHIMETRIC);
            sz.Width = phm.X;
            sz.Height = phm.Y;
        }

        private static int Pixel2Twip(int v, bool xDirection)
        {
            SetupLogPixels(false);
            int logP = xDirection ? s_logPixelsX : s_logPixelsY;
            return (int)((((double)v) / logP) * 72.0 * 20.0);
        }

        private static int Twip2Pixel(double v, bool xDirection)
        {
            SetupLogPixels(false);
            int logP = xDirection ? s_logPixelsX : s_logPixelsY;
            return (int)(((v / 20.0) / 72.0) * logP);
        }

        private static int Twip2Pixel(int v, bool xDirection)
        {
            SetupLogPixels(false);
            int logP = xDirection ? s_logPixelsX : s_logPixelsY;
            return (int)(((v / 20.0) / 72.0) * logP);
        }

        private unsafe Size SetExtent(int width, int height)
        {
            Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"setting extent to {width} {height}");
            Size sz = new Size(width, height);
            bool resetExtents = !IsUserMode();
            Pixel2hiMetric(ref sz);
            HRESULT hr = GetOleObject().SetExtent(Ole32.DVASPECT.CONTENT, &sz);
            if (hr != HRESULT.Values.S_OK)
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

            if (GetAxState(s_handlePosRectChanged))
            {
                return;
            }

            _axState[s_handlePosRectChanged] = true;

            // Provide control with an opportunity to apply self imposed constraints on its size.
            Size adjustedSize = ApplySizeConstraints(width, height);
            width = adjustedSize.Width;
            height = adjustedSize.Height;

            try
            {
                if (_axState[s_fFakingWindow])
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

                if (_axState[s_manualUpdate])
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
                _axState[s_handlePosRectChanged] = false;
            }
        }

        private bool CheckSubclassing()
        {
            if (!IsHandleCreated || _wndprocAddr == IntPtr.Zero)
            {
                return true;
            }

            HWND handle = HWND;
            IntPtr currentWndproc = PInvoke.GetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_WNDPROC);
            if (currentWndproc == _wndprocAddr)
            {
                return true;
            }

            if ((int)PInvoke.SendMessage(this, _registeredMessage) == REGMSG_RETVAL)
            {
                _wndprocAddr = currentWndproc;
                return true;
            }

            // We were resubclassed, we need to resublass ourselves.
            Debug.WriteLineIf(s_axHostSwitch.TraceVerbose, "The control subclassed itself w/o calling the old wndproc.");
            Debug.Assert(!OwnWindow(), "Why are we here if we own our window?");
            WindowReleaseHandle();
            PInvoke.SetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_WNDPROC, currentWndproc);
            WindowAssignHandle(handle, _axState[s_assignUniqueID]);
            InformOfNewHandle();
            _axState[s_manualUpdate] = true;
            return false;
        }

        /// <summary>
        ///  Destroys the handle associated with this control.
        ///  User code should in general not call this function.
        /// </summary>
        protected override void DestroyHandle()
        {
            if (_axState[s_fOwnWindow])
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
            if (_axState[s_inTransition])
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "Recursively entering TransitionDownTo...");
                return;
            }

            try
            {
                _axState[s_inTransition] = true;

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
                            Debug.Assert(hr.Succeeded, $"Failed in UiDeactivate: {hr}");
                            Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose && GetOcState() == OC_INPLACE, "failed transition");
                            SetOcState(OC_INPLACE);
                            break;
                        case OC_INPLACE:
                            if (_axState[s_fFakingWindow])
                            {
                                DestroyFakeWindow();
                                SetOcState(OC_RUNNING);
                            }
                            else
                            {
                                InPlaceDeactivate();
                            }

                            Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose && GetOcState() == OC_RUNNING, "failed transition");
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
                _axState[s_inTransition] = false;
            }
        }

        private void TransitionUpTo(int state)
        {
            if (_axState[s_inTransition])
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "Recursively entering TransitionUpTo...");
                return;
            }

            try
            {
                _axState[s_inTransition] = true;

                while (state > GetOcState())
                {
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"Transitioning up from: {GetOcState()} to: {state}");
                    switch (GetOcState())
                    {
                        case OC_PASSIVE:
                            _axState[s_disposed] = false;
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
                            _axState[s_ownDisposing] = false;
                            Debug.Assert(!_axState[s_fOwnWindow], "If we are invis at runtime, we should never be going beyond OC_RUNNING");
                            if (!_axState[s_fOwnWindow])
                            {
                                InPlaceActivate();

                                if (!Visible && ContainingControl is not null && ContainingControl.Visible)
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
                                    if (!IsUserMode() && !_axState[s_ocxStateSet])
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
                _axState[s_inTransition] = false;
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
            _axState[s_ownDisposing] = true;
            ContainerControl f = ContainingControl;
            if (f is not null)
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
            Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"calling uiActivate for {ToString()}");
            Debug.Assert(CanUIActivate, "we have to be able to uiactivate");
            if (CanUIActivate)
            {
                DoVerb((int)Ole32.OLEIVERB.UIACTIVATE);
            }
        }

        private void DestroyFakeWindow()
        {
            Debug.Assert(_axState[s_fFakingWindow], "have to be faking it in order to destroy it...");

            // The problem seems to be that when we try to destroy the fake window,
            // we recurse in and transition the control down to OC_RUNNING. This causes the control's
            // new window to get destroyed also, and the control never shows up.
            // We now prevent this by changing our state about the fakeWindow _before_ we actually
            // destroy the window.
            //
            _axState[s_fFakingWindow] = false;
            base.DestroyHandle();
        }

        private void EnsureWindowPresent()
        {
            // if the ctl didn't call showobject, we need to do it for it...
            if (!IsHandleCreated)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "Naughty control didn't call showObject...");
                try
                {
                    ((Ole32.IOleClientSite)_oleSite).ShowObject();
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

            if (ParentInternal is not null)
            {
                Debug.Fail("extremely naughty ctl is refusing to give us an hWnd... giving up...");
                throw new NotSupportedException(string.Format(SR.AXNohWnd, GetType().Name));
            }
        }

        protected override void SetVisibleCore(bool value)
        {
            if (GetState(States.Visible) != value)
            {
                bool oldVisible = Visible;
                if ((IsHandleCreated || value) && ParentInternal is not null && ParentInternal.Created)
                {
                    if (!_axState[s_fOwnWindow])
                    {
                        TransitionUpTo(OC_RUNNING);
                        if (value)
                        {
                            if (_axState[s_fFakingWindow])
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
                                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "Could not make ctl visible by using INPLACE. Will try SHOW");
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
                            Debug.Assert(!_axState[s_fFakingWindow], "if we were visible, we could not have had a fake window...");
                            HideAxControl();
                        }
                    }
                }

                if (!value)
                {
                    _axState[s_fNeedOwnWindow] = false;
                }

                if (!_axState[s_fOwnWindow])
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
            if (f is not null && f.ActiveControl != ctl)
            {
                f.ActiveControl = ctl;
            }
        }

        private void HideAxControl()
        {
            Debug.Assert(!_axState[s_fOwnWindow], "can't own our window when hiding");
            Debug.Assert(IsHandleCreated, "gotta have a window to hide");
            Debug.Assert(GetOcState() >= OC_INPLACE, "have to be in place in order to hide.");

            DoVerb((int)Ole32.OLEIVERB.HIDE);
            if (GetOcState() < OC_INPLACE)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "Naughty control inplace deactivated on a hide verb...");
                Debug.Assert(!IsHandleCreated, "if we are inplace deactivated we should not have a window.");
                // all we do here is set a flag saying that we need the window to be created if
                // create handle is ever called...
                _axState[s_fNeedOwnWindow] = true;

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
            if (_ignoreDialogKeys)
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
        ///  (1) Call the OCX's TranslateAccelerator. This may or may not call back into us using IOleControlSite::TranslateAccelerator()
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
            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, $"AxHost.PreProcessMessage {msg}");

            if (IsUserMode())
            {
                if (_axState[s_siteProcessedInputKey])
                {
                    // In this case, the control called the us back through the IControlSite
                    // and giving us a chance to see if we want to process it. We in turn
                    // call the base implementation which normally would call the control's
                    // IsInputKey() or IsInputChar(). So, we short-circuit those to return false
                    // and only return true, if the container-chain wanted to process the keystroke
                    // (e.g. tab, accelerators etc.)
                    return base.PreProcessMessage(ref msg);
                }

                MSG win32Message = msg;
                _axState[s_siteProcessedInputKey] = false;
                try
                {
                    Ole32.IOleInPlaceActiveObject activeObj = GetInPlaceActiveObject();
                    if (activeObj is not null)
                    {
                        HRESULT hr = activeObj.TranslateAccelerator(&win32Message);
                        msg.MsgInternal = (User32.WM)win32Message.message;
                        msg.WParamInternal = win32Message.wParam;
                        msg.LParamInternal = win32Message.lParam;
                        msg.HWnd = win32Message.hwnd;

                        if (hr == HRESULT.Values.S_OK)
                        {
                            Debug.WriteLineIf(
                                s_controlKeyboardRouting.TraceVerbose,
                                $"\t Message translated by control to {msg}");
                            return true;
                        }
                        else if (hr == HRESULT.Values.S_FALSE)
                        {
                            bool ret = false;

                            _ignoreDialogKeys = true;
                            try
                            {
                                ret = base.PreProcessMessage(ref msg);
                            }
                            finally
                            {
                                _ignoreDialogKeys = false;
                            }

                            return ret;
                        }
                        else if (_axState[s_siteProcessedInputKey])
                        {
                            Debug.WriteLineIf(
                                s_controlKeyboardRouting.TraceVerbose,
                                $"\t Message processed by site. Calling base.PreProcessMessage() {msg}");
                            return base.PreProcessMessage(ref msg);
                        }
                        else
                        {
                            Debug.WriteLineIf(
                                s_controlKeyboardRouting.TraceVerbose,
                                $"\t Message not processed by site. Returning false. {msg}");
                            return false;
                        }
                    }
                }
                finally
                {
                    _axState[s_siteProcessedInputKey] = false;
                }
            }

            return false;
        }

        /// <summary>
        ///  Process a mnemonic character. This is done by manufacturing a WM_SYSKEYDOWN message and passing it to the
        ///  ActiveX control.
        /// </summary>
        protected internal unsafe override bool ProcessMnemonic(char charCode)
        {
            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, $"In AxHost.ProcessMnemonic: {(int)charCode}");
            if (CanSelect)
            {
                try
                {
                    var ctlInfo = new Ole32.CONTROLINFO
                    {
                        cb = (uint)Marshal.SizeOf<Ole32.CONTROLINFO>()
                    };
                    HRESULT hr = GetOleControl().GetControlInfo(&ctlInfo);
                    if (!hr.Succeeded)
                    {
                        return false;
                    }

                    var msg = new MSG
                    {
                        // Sadly, we don't have a message so we must fake one ourselves...
                        // A bit of ugliness here (a bit?  more like a bucket...)
                        // The message we are faking is a WM_SYSKEYDOWN w/ the right alt key setting...
                        hwnd = (ContainingControl is null) ? (HWND)0 : ContainingControl.HWND,
                        message = (uint)User32.WM.SYSKEYDOWN,
                        wParam = (WPARAM)char.ToUpper(charCode, CultureInfo.CurrentCulture),
                        lParam = 0x20180001,
                        time = PInvoke.GetTickCount()
                    };
                    User32.GetCursorPos(out Point p);
                    msg.pt = p;
                    if (Ole32.IsAccelerator(new HandleRef(ctlInfo, ctlInfo.hAccel), ctlInfo.cAccel, ref msg, null))
                    {
                        GetOleControl().OnMnemonic(&msg);
                        Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, $"\t Processed mnemonic {msg}");
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
            _aboutBoxDelegate += d;
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
                if (IsDirty() || _ocxState is null)
                {
                    Debug.Assert(!_axState[s_disposed], "we could not be asking for the object when we are axState[disposed]...");
                    _ocxState = CreateNewOcxState(_ocxState);
                }

                return _ocxState;
            }

            set
            {
                _axState[s_ocxStateSet] = true;

                if (value is null)
                {
                    return;
                }

                if (_storageType != STG_UNKNOWN && _storageType != value.Type)
                {
                    Debug.Fail("Trying to reload with a OcxState that is of a different type.");
                    throw new InvalidOperationException(SR.AXOcxStateLoaded);
                }

                if (_ocxState == value)
                {
                    return;
                }

                _ocxState = value;

                if (_ocxState is not null)
                {
                    _axState[s_manualUpdate] = _ocxState._GetManualUpdate();
                    _licenseKey = _ocxState._GetLicenseKey();
                }
                else
                {
                    _axState[s_manualUpdate] = false;
                    _licenseKey = null;
                }

                if (_ocxState is not null && GetOcState() >= OC_RUNNING)
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

                    if (_iPersistPropBag is not null)
                    {
                        propBag = new PropertyBagStream();
                        _iPersistPropBag.Save(propBag, true, true);
                    }

                    MemoryStream ms = null;
                    switch (_storageType)
                    {
                        case STG_STREAM:
                        case STG_STREAMINIT:
                            ms = new MemoryStream();
                            if (_storageType == STG_STREAM)
                            {
                                _iPersistStream.Save(new Ole32.GPStream(ms), true);
                            }
                            else
                            {
                                _iPersistStreamInit.Save(new Ole32.GPStream(ms), true);
                            }

                            break;
                        case STG_STORAGE:
                            Debug.Assert(oldOcxState is not null, "we got to have an old state which holds out scribble storage...");
                            if (oldOcxState is not null)
                            {
                                return oldOcxState.RefreshStorage(_iPersistStorage);
                            }

                            return null;
                        default:
                            Debug.Fail("unknown storage type.");
                            return null;
                    }

                    if (ms is not null)
                    {
                        return new State(ms, _storageType, this, propBag);
                    }
                    else if (propBag is not null)
                    {
                        return new State(propBag);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"Could not create new OCX State: {e}");
                }
            }
            finally
            {
                NoComponentChangeEvents--;
            }

            return null;
        }

        /// <summary>
        ///  Returns this control's logically containing form.
        ///  At design time this is always the form being designed.
        ///  At runtime it is either the form set with setContainingForm or,
        ///  by default, the parent form.
        ///  Sets the form which is the logical container of this control.
        ///  By default, the parent form performs that function.  It is
        ///  however possible for another form higher in the parent chain
        ///  to serve in that role.  The logical container of this
        ///  control determines the set of logical sibling control.
        ///  In general this property exists only to enable some specific
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
                if (_containingControl is null)
                {
                    _containingControl = FindContainerControlInternal();
                }

                return _containingControl;
            }

            set
            {
                _containingControl = value;
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
            if (Site is not null)
            {
                IDesignerHost host = (IDesignerHost)Site.GetService(typeof(IDesignerHost));
                if (host is not null)
                {
                    if (host.RootComponent is ContainerControl rootControl)
                    {
                        return rootControl;
                    }
                }
            }

            ContainerControl cc = null;
            Control control = this;
            while (control is not null)
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

            Debug.Assert(_storageType != STG_UNKNOWN, "if we are loaded, out storage type must be set!");

            if (_axState[s_valueChanged])
            {
                _axState[s_valueChanged] = false;
                return true;
            }

#if DEBUG
            if (s_axAlwaysSaveSwitch.Enabled)
            {
                return true;
            }
#endif
            HRESULT hr = HRESULT.Values.E_FAIL;
            switch (_storageType)
            {
                case STG_STREAM:
                    hr = _iPersistStream.IsDirty();
                    break;
                case STG_STREAMINIT:
                    hr = _iPersistStreamInit.IsDirty();
                    break;
                case STG_STORAGE:
                    hr = _iPersistStorage.IsDirty();
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
            return hr != HRESULT.Values.S_FALSE;
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
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "asked for usermode");
                    return IsUserMode();
                case Ole32.DispatchID.AMBIENT_AUTOCLIP:
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "asked for autoclip");
                    return true;
                case Ole32.DispatchID.AMBIENT_MESSAGEREFLECT:
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "asked for message reflect");
                    return true;
                case Ole32.DispatchID.AMBIENT_UIDEAD:
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "asked for uidead");
                    return false;
                case Ole32.DispatchID.AMBIENT_DISPLAYASDEFAULT:
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "asked for displayasdefault");
                    return false;
                case Ole32.DispatchID.AMBIENT_FONT:
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "asked for font");
                    if (richParent is not null)
                    {
                        return GetIFontFromFont(richParent.Font);
                    }

                    return null;
                case Ole32.DispatchID.AMBIENT_SHOWGRABHANDLES:
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "asked for showGrabHandles");
                    return false;
                case Ole32.DispatchID.AMBIENT_SHOWHATCHING:
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "asked for showHatching");
                    return false;
                case Ole32.DispatchID.AMBIENT_BACKCOLOR:
                    if (richParent is not null)
                    {
                        return GetOleColorFromColor(richParent.BackColor);
                    }

                    return null;
                case Ole32.DispatchID.AMBIENT_FORECOLOR:
                    if (richParent is not null)
                    {
                        return GetOleColorFromColor(richParent.ForeColor);
                    }

                    return null;
                case Ole32.DispatchID.AMBIENT_DISPLAYNAME:
                    string rval = AxContainer.GetNameForControl(this);
                    if (rval is null)
                    {
                        rval = string.Empty;
                    }

                    return rval;
                case Ole32.DispatchID.AMBIENT_LOCALEID:
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "asked for localeid");
                    return PInvoke.GetThreadLocale();
                case Ole32.DispatchID.AMBIENT_RIGHTTOLEFT:
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "asked for right to left");
                    Control ctl = this;
                    while (ctl is not null)
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
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"unsupported ambient {dispid}");
                    return null;
            }
        }

        public unsafe void DoVerb(int verb)
        {
            Control parent = ParentInternal;
            RECT posRect = Bounds;
            GetOleObject().DoVerb((Ole32.OLEIVERB)verb, null, _oleSite, -1, parent is not null ? parent.Handle : IntPtr.Zero, &posRect);
        }

        private bool AwaitingDefreezing()
        {
            return _freezeCount > 0;
        }

        private void Freeze(bool v)
        {
            Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"freezing {v}");
            if (v)
            {
                GetOleControl().FreezeEvents(true);
                _freezeCount++;
            }
            else
            {
                GetOleControl().FreezeEvents(false);
                _freezeCount--;
            }

            Debug.Assert(_freezeCount >= 0, "invalid freeze count!");
        }

        private HRESULT UiDeactivate()
        {
            Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"calling uiDeactivate for {ToString()}");
            bool ownDispose = _axState[s_ownDisposing];
            _axState[s_ownDisposing] = true;
            try
            {
                return GetInPlaceObject().UIDeactivate();
            }
            finally
            {
                _axState[s_ownDisposing] = ownDispose;
            }
        }

        private int GetOcState()
        {
            return _ocState;
        }

        private void SetOcState(int nv)
        {
            _ocState = nv;
        }

        private string GetLicenseKey()
        {
            return GetLicenseKey(_clsid);
        }

        private unsafe string GetLicenseKey(Guid clsid)
        {
            if (_licenseKey is not null || !_axState[s_needLicenseKey])
            {
                return _licenseKey;
            }

            HRESULT hr = Ole32.CoGetClassObject(
                ref clsid,
                Ole32.CLSCTX.INPROC_SERVER,
                IntPtr.Zero,
                in s_icf2_Guid,
                out Ole32.IClassFactory2 icf2);

            if (!hr.Succeeded)
            {
                if (hr == HRESULT.Values.E_NOINTERFACE)
                {
                    return null;
                }

                _axState[s_needLicenseKey] = false;
                return null;
            }

            var licInfo = new Ole32.LICINFO
            {
                cbLicInfo = sizeof(Ole32.LICINFO)
            };
            icf2.GetLicInfo(&licInfo);
            if (licInfo.fRuntimeAvailable)
            {
                var rval = new string[1];
                icf2.RequestLicKey(0, rval);
                _licenseKey = rval[0];
                return _licenseKey;
            }

            return null;
        }

        private void CreateWithoutLicense(Guid clsid)
        {
            Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"Creating object without license: {clsid}");
            HRESULT hr = Ole32.CoCreateInstance(
                ref clsid,
                IntPtr.Zero,
                Ole32.CLSCTX.INPROC_SERVER,
                ref NativeMethods.ActiveX.IID_IUnknown,
                out object ret);
            hr.ThrowOnFailure();

            _instance = ret;
            Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"\t{(_instance is not null)}");
        }

        private void CreateWithLicense(string license, Guid clsid)
        {
            if (license is not null)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"Creating object with license: {clsid}");
                HRESULT hr = Ole32.CoGetClassObject(
                    ref clsid,
                    Ole32.CLSCTX.INPROC_SERVER,
                    IntPtr.Zero,
                    in s_icf2_Guid,
                    out Ole32.IClassFactory2 icf2);
                if (hr.Succeeded)
                {
                    icf2.CreateInstanceLic(IntPtr.Zero, IntPtr.Zero, ref NativeMethods.ActiveX.IID_IUnknown, license, out _instance);
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"\t{(_instance is not null)}");
                }
            }

            if (_instance is null)
            {
                CreateWithoutLicense(clsid);
            }
        }

        private void CreateInstance()
        {
            Debug.Assert(_instance is null, "instance must be null");
            try
            {
                _instance = CreateInstanceCore(_clsid);
                Debug.Assert(_instance is not null, "w/o an exception being thrown we must have an object...");
            }
            catch (ExternalException e)
            {
                if (e.ErrorCode == unchecked((int)0x80040112))
                {
                    // CLASS_E_NOTLICENSED
                    throw new LicenseException(GetType(), this, SR.AXNoLicenseToUse);
                }

                throw;
            }

            Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "created");
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
                CreateWithLicense(_licenseKey, clsid);
            }
            else
            {
                CreateWithoutLicense(clsid);
            }

            return _instance;
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
            if (hr != HRESULT.Values.S_OK || propcat == 0)
            {
                return null;
            }

            int index = -(int)propcat;
            if (index > 0 && index < s_categoryNames.Length && s_categoryNames[index] is not null)
            {
                return s_categoryNames[index];
            }

            if (_objectDefinedCategoryNames is not null)
            {
                CategoryAttribute rval = (CategoryAttribute)_objectDefinedCategoryNames[propcat];
                if (rval is not null)
                {
                    return rval;
                }
            }

            hr = icp.GetCategoryName(propcat, PInvoke.GetThreadLocale(), out string name);
            if (hr == HRESULT.Values.S_OK && name is not null)
            {
                var rval = new CategoryAttribute(name);
                _objectDefinedCategoryNames ??= new Hashtable();
                _objectDefinedCategoryNames.Add(propcat, rval);
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
                _selectionStyle = selectionStyle;
                if (iss is not null && iss.GetComponentSelected(this))
                {
                    // The AX Host designer will offer an extender property called "SelectionStyle"
                    //
                    PropertyDescriptor prop = TypeDescriptor.GetProperties(this)["SelectionStyle"];
                    if (prop is not null && prop.PropertyType == typeof(int))
                    {
                        prop.SetValue(this, selectionStyle);
                    }
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void InvokeEditMode()
        {
            Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"invoking EditMode for {ToString()}");
            Debug.Assert((_flags & AxFlags.PreventEditMode) == 0, "edit mode should have been disabled");
            if (_editMode != EDITM_NONE)
            {
                return;
            }

            AddSelectionHandler();
            _editMode = EDITM_HOST;
            SetSelectionStyle(2);
            _ = PInvoke.GetFocus();
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
            if (!_axState[s_editorRefresh] && HasPropertyPages())
            {
                _axState[s_editorRefresh] = true;
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
        [RequiresUnreferencedCode(TrimmingConstants.AttributesRequiresUnreferencedCodeMessage)]
        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return null;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [RequiresUnreferencedCode(TrimmingConstants.EventDescriptorRequiresUnreferencedCodeMessage)]
        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [RequiresUnreferencedCode(TrimmingConstants.PropertyDescriptorPropertyTypeMessage)]
        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        /// <summary>
        ///  Retrieves the an editor for this object.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [RequiresUnreferencedCode(TrimmingConstants.EditorRequiresUnreferencedCode)]
        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            if (editorBaseType != typeof(ComponentEditor))
            {
                return null;
            }

            if (_editor is not null)
            {
                return _editor;
            }

            if (_editor is null && HasPropertyPages())
            {
                _editor = new AxComponentEditor();
            }

            return _editor;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
            => TypeDescriptor.GetEvents(this, noCustomTypeDesc: true);

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [RequiresUnreferencedCode(TrimmingConstants.FilterRequiresUnreferencedCodeMessage)]
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
            => TypeDescriptor.GetEvents(this, attributes, noCustomTypeDesc: true);

        private void OnIdle(object sender, EventArgs e)
        {
            if (_axState[s_refreshProperties])
            {
                TypeDescriptor.Refresh(GetType());
            }
        }

        private bool RefreshAllProperties
        {
            get
            {
                return _axState[s_refreshProperties];
            }
            set
            {
                _axState[s_refreshProperties] = value;
                if (value && !_axState[s_listeningToIdle])
                {
                    Application.Idle += new EventHandler(OnIdle);
                    _axState[s_listeningToIdle] = true;
                }
                else if (!value && _axState[s_listeningToIdle])
                {
                    Application.Idle -= new EventHandler(OnIdle);
                    _axState[s_listeningToIdle] = false;
                }
            }
        }

        private PropertyDescriptorCollection FillProperties(Attribute[] attributes)
        {
            if (RefreshAllProperties)
            {
                RefreshAllProperties = false;
                _propsStash = null;
                _attribsStash = null;
            }
            else if (_propsStash is not null)
            {
                if (attributes is null && _attribsStash is null)
                {
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "Returning stashed values for : <null>");
                    return _propsStash;
                }
                else if (attributes is not null && _attribsStash is not null && attributes.Length == _attribsStash.Length)
                {
                    bool attribsEqual = true;
                    int i = 0;
                    foreach (Attribute attrib in attributes)
                    {
                        if (!attrib.Equals(_attribsStash[i++]))
                        {
                            attribsEqual = false;
                            break;
                        }
                    }

                    if (attribsEqual)
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"Returning stashed values for : {attributes.Length}");
                        return _propsStash;
                    }
                }
            }

            ArrayList returnProperties = new ArrayList();

            if (_properties is null)
            {
                _properties = new Hashtable();
            }

            if (_propertyInfos is null)
            {
                _propertyInfos = new Hashtable();

                PropertyInfo[] propInfos = GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

                foreach (PropertyInfo propInfo in propInfos)
                {
                    _propertyInfos.Add(propInfo.Name, propInfo);
                }
            }

            PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(this, null, true);
            if (baseProps is not null)
            {
                for (int i = 0; i < baseProps.Count; ++i)
                {
                    Debug.Assert(baseProps[i] is not null, $"Null base prop at location: {i}");

                    if (baseProps[i].DesignTimeOnly)
                    {
                        returnProperties.Add(baseProps[i]);
                        continue;
                    }

                    string propName = baseProps[i].Name;
                    PropertyDescriptor prop = null;
                    PropertyInfo propInfo = (PropertyInfo)_propertyInfos[propName];

                    // We do not support "write-only" properties that some activex controls support.
                    if (propInfo is not null && !propInfo.CanRead)
                    {
                        continue;
                    }

                    if (!_properties.ContainsKey(propName))
                    {
                        if (propInfo is not null)
                        {
                            Debug.WriteLineIf(s_axPropTraceSwitch.TraceVerbose, $"Added AxPropertyDescriptor for: {propName}");
                            prop = new AxPropertyDescriptor(baseProps[i], this);
                            ((AxPropertyDescriptor)prop).UpdateAttributes();
                        }
                        else
                        {
                            Debug.WriteLineIf(s_axPropTraceSwitch.TraceVerbose, $"Added PropertyDescriptor for: {propName}");
                            prop = baseProps[i];
                        }

                        _properties.Add(propName, prop);
                        returnProperties.Add(prop);
                    }
                    else
                    {
                        PropertyDescriptor propDesc = (PropertyDescriptor)_properties[propName];
                        Debug.Assert(propDesc is not null, $"Cannot find cached entry for: {propName}");
                        AxPropertyDescriptor axPropDesc = propDesc as AxPropertyDescriptor;
                        if ((propInfo is null && axPropDesc is not null) || (propInfo is not null && axPropDesc is null))
                        {
                            Debug.Fail($"Duplicate property with same name: {propName}");
                            Debug.WriteLineIf(s_axPropTraceSwitch.TraceVerbose, $"Duplicate property with same name: {propName}");
                        }
                        else
                        {
                            if (axPropDesc is not null)
                            {
                                axPropDesc.UpdateAttributes();
                            }

                            returnProperties.Add(propDesc);
                        }
                    }
                }

                // Filter only the Browsable attribute, since that is the only one we mess with.
                if (attributes is not null)
                {
                    Attribute browse = null;
                    foreach (Attribute attribute in attributes)
                    {
                        if (attribute is BrowsableAttribute)
                        {
                            browse = attribute;
                        }
                    }

                    if (browse is not null)
                    {
                        ArrayList removeList = null;

                        foreach (PropertyDescriptor prop in returnProperties)
                        {
                            if (prop is AxPropertyDescriptor
                                && prop.TryGetAttribute(out BrowsableAttribute browsableAttribute)
                                && !browsableAttribute.Equals(browse))
                            {
                                removeList ??= new ArrayList();
                                removeList.Add(prop);
                            }
                        }

                        if (removeList is not null)
                        {
                            foreach (object prop in removeList)
                            {
                                returnProperties.Remove(prop);
                            }
                        }
                    }
                }
            }

            PropertyDescriptor[] temp = new PropertyDescriptor[returnProperties.Count];
            returnProperties.CopyTo(temp, 0);

            // Update our stashed values.
            Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"Updating stashed values for : {attributes?.Length.ToString() ?? "<null>"}");
            _propsStash = new PropertyDescriptorCollection(temp);
            _attribsStash = attributes;

            return _propsStash;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [RequiresUnreferencedCode(TrimmingConstants.PropertyDescriptorPropertyTypeMessage)]
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return FillProperties(null);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [RequiresUnreferencedCode(TrimmingConstants.PropertyDescriptorPropertyTypeMessage + " " + TrimmingConstants.FilterRequiresUnreferencedCodeMessage)]
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
            _iPersistPropBag.Load(propBag, null);
        }

        private void DepersistFromIStream(Ole32.IStream istream)
        {
            _storageType = STG_STREAM;
            _iPersistStream.Load(istream);
        }

        private void DepersistFromIStreamInit(Ole32.IStream istream)
        {
            _storageType = STG_STREAMINIT;
            _iPersistStreamInit.Load(istream);
        }

        private void DepersistFromIStorage(Ole32.IStorage storage)
        {
            _storageType = STG_STORAGE;

            // Looks like MapPoint control does not create a valid IStorage
            // until some property has changed. Since we end up creating a bogus (empty)
            // storage, we end up not being able to re-create a valid one and this would
            // fail.
            //
            if (storage is not null)
            {
                HRESULT hr = _iPersistStorage.Load(storage);
                if (hr != HRESULT.Values.S_OK)
                {
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"Error trying load depersist from IStorage: {hr}");
                }
            }
        }

        private void DepersistControl()
        {
            Freeze(true);

            if (_ocxState is null)
            {
                // Must init new:
                if (_instance is Ole32.IPersistStreamInit init)
                {
                    _iPersistStreamInit = init;
                    try
                    {
                        _storageType = STG_STREAMINIT;
                        _iPersistStreamInit.InitNew();
                    }
                    catch (Exception e1)
                    {
                        Debug.WriteLineIf(
                            s_axHTraceSwitch.TraceVerbose,
                            $"Exception thrown trying to IPersistStreamInit.InitNew(). Is this good? {e1}");
                    }

                    return;
                }

                if (_instance is Ole32.IPersistStream persistStream)
                {
                    _storageType = STG_STREAM;
                    _iPersistStream = persistStream;
                    return;
                }

                if (_instance is Ole32.IPersistStorage persistStorage)
                {
                    _storageType = STG_STORAGE;
                    _ocxState = new State(this);
                    _iPersistStorage = persistStorage;
                    try
                    {
                        _iPersistStorage.InitNew(_ocxState.GetStorage());
                    }
                    catch (Exception e2)
                    {
                        Debug.WriteLineIf(
                            s_axHTraceSwitch.TraceVerbose,
                            $"Exception thrown trying to IPersistStorage.InitNew(). Is this good? {e2}");
                    }

                    return;
                }

                if (_instance is Oleaut32.IPersistPropertyBag persistPropertyBag)
                {
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"{this} supports IPersistPropertyBag.");
                    _iPersistPropBag = persistPropertyBag;
                    try
                    {
                        _iPersistPropBag.InitNew();
                    }
                    catch (Exception e1)
                    {
                        Debug.WriteLineIf(
                            s_axHTraceSwitch.TraceVerbose,
                            $"Exception thrown trying to IPersistPropertyBag.InitNew(). Is this good? {e1}");
                    }
                }

                Debug.Fail("no implemented persistence interfaces on object");
                throw new InvalidOperationException(SR.UnableToInitComponent);
            }

            // Otherwise, we have state to depersist from:
            switch (_ocxState.Type)
            {
                case STG_STREAM:
                    try
                    {
                        _iPersistStream = (Ole32.IPersistStream)_instance;
                        DepersistFromIStream(_ocxState.GetStream());
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLineIf(
                            s_axHTraceSwitch.TraceVerbose,
                            $"Exception thrown trying to IPersistStream.DepersistFromIStream(). Is this good? {e}");
                    }

                    break;
                case STG_STREAMINIT:
                    if (_instance is Ole32.IPersistStreamInit persistStreamInit)
                    {
                        try
                        {
                            _iPersistStreamInit = persistStreamInit;
                            DepersistFromIStreamInit(_ocxState.GetStream());
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLineIf(
                                s_axHTraceSwitch.TraceVerbose,
                                $"Exception thrown trying to IPersistStreamInit.DepersistFromIStreamInit(). Is this good? {e}");
                        }

                        GetControlEnabled();
                    }
                    else
                    {
                        _ocxState.Type = STG_STREAM;
                        DepersistControl();
                        return;
                    }

                    break;
                case STG_STORAGE:
                    try
                    {
                        _iPersistStorage = (Ole32.IPersistStorage)_instance;
                        DepersistFromIStorage(_ocxState.GetStorage());
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLineIf(
                            s_axHTraceSwitch.TraceVerbose,
                            $"Exception thrown trying to IPersistStorage.DepersistFromIStorage(). Is this good? {e}");
                    }

                    break;
                default:
                    Debug.Fail("unknown storage type.");
                    throw new InvalidOperationException(SR.UnableToInitComponent);
            }

            if (_ocxState.GetPropBag() is not null)
            {
                try
                {
                    _iPersistPropBag = (Oleaut32.IPersistPropertyBag)_instance;
                    DepersistFromIPropertyBag(_ocxState.GetPropBag());
                }
                catch (Exception e)
                {
                    Debug.WriteLineIf(
                        s_axHTraceSwitch.TraceVerbose,
                        $"Exception thrown trying to IPersistPropertyBag.DepersistFromIPropertyBag(). Is this good? {e}");
                }
            }
        }

        /// <summary>
        ///  Returns the IUnknown pointer to the enclosed ActiveX control.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public object GetOcx()
        {
            return _instance;
        }

        private object GetOcxCreate()
        {
            if (_instance is null)
            {
                CreateInstance();
                RealizeStyles();
                AttachInterfaces();
                _oleSite.OnOcxCreate();
            }

            return _instance;
        }

        private void StartEvents()
        {
            if (!_axState[s_sinkAttached])
            {
                try
                {
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "Creating sink for events...");
                    CreateSink();
                    _oleSite.StartEvents();
                }
                catch (Exception t)
                {
                    Debug.Fail(t.ToString());
                }

                _axState[s_sinkAttached] = true;
            }
        }

        private void StopEvents()
        {
            if (_axState[s_sinkAttached])
            {
                try
                {
                    DetachSink();
                }
                catch (Exception t)
                {
                    Debug.Fail(t.ToString());
                }

                _axState[s_sinkAttached] = false;
            }

            _oleSite.StopEvents();
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
                if (!hr.Succeeded)
                {
                    return false;
                }

                return uuids.cElems > 0;
            }
            finally
            {
                if (uuids.pElems is not null)
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
                        hwndOwner = (ContainingControl is null) ? 0 : ContainingControl.Handle,
                        lpszCaption = pName,
                        cObjects = 1,
                        ppUnk = (nint)(&pUnk),
                        cPages = 1,
                        lpPages = (nint)(&guid),
                        lcid = PInvoke.GetThreadLocale(),
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
            if (Site.TryGetService(out IComponentChangeService changeService))
            {
                changeService.OnComponentChanging(this);
                changeService.OnComponentChanged(this);
            }
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

#pragma warning disable IDE0060 // Remove unused parameter - public API
        public unsafe void ShowPropertyPages(Control control)
#pragma warning restore IDE0060
        {
            if (!CanShowPropertyPages())
            {
                return;
            }

            Ole32.ISpecifyPropertyPages ispp = (Ole32.ISpecifyPropertyPages)GetOcx();
            var uuids = new Ole32.CAUUID();
            HRESULT hr = ispp.GetPages(&uuids);
            if (!hr.Succeeded || uuids.cElems == 0)
            {
                return;
            }

            IDesignerHost host = null;
            if (Site is not null)
            {
                host = (IDesignerHost)Site.GetService(typeof(IDesignerHost));
            }

            DesignerTransaction trans = null;
            try
            {
                if (host is not null)
                {
                    trans = host.CreateTransaction(SR.AXEditProperties);
                }

                nint handle = (ContainingControl is null) ? 0 : ContainingControl.Handle;
                nint pUnk = Marshal.GetIUnknownForObject(GetOcx());
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
                        PInvoke.GetThreadLocale(),
                        0,
                        0);
                }
                finally
                {
                    Marshal.Release(pUnk);
                }
            }
            finally
            {
                if (_oleSite is not null)
                {
                    ((Ole32.IPropertyNotifySink)_oleSite).OnChanged(Ole32.DispatchID.UNKNOWN);
                }

                if (trans is not null)
                {
                    trans.Commit();
                }

                if (uuids.pElems is not null)
                {
                    Marshal.FreeCoTaskMem((IntPtr)uuids.pElems);
                }
            }
        }

        internal override HBRUSH InitializeDCForWmCtlColor(HDC dc, User32.WM msg)
        {
            if (_isMaskEdit)
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
            if (false && (_axState[s_manualUpdate] && IsUserMode()))
            {
                DefWndProc(ref m);
                return;
            }

            switch (m.MsgInternal)
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
                        _hwndFocus = (HWND)(nint)m.WParamInternal;
                        try
                        {
                            base.WndProc(ref m);
                        }
                        finally
                        {
                            _hwndFocus = default;
                        }

                        break;
                    }

                case User32.WM.COMMAND:
                    if (!ReflectMessage(m.LParamInternal, ref m))
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
                        Debug.WriteLineIf(
                            s_axHTraceSwitch.TraceVerbose,
                            $"WM_DESTROY control is destroying the window from under us...{GetType()}");
                    }
#endif
                    // If we are currently in a state of InPlaceActive or above,
                    // we should first reparent the ActiveX control to our parking
                    // window before we transition to a state below InPlaceActive.
                    // Otherwise we face all sorts of problems when we try to
                    // transition back to a state >= InPlaceActive.
                    if (GetOcState() >= OC_INPLACE)
                    {
                        Ole32.IOleInPlaceObject ipo = GetInPlaceObject();
                        HWND hwnd = HWND.Null;
                        if (ipo.GetWindow(&hwnd).Succeeded)
                        {
                            Application.ParkHandle(handle: new(ipo, hwnd), DpiAwarenessContext);
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
                    if (_axState[s_processingKeyUp])
                    {
                        break;
                    }

                    _axState[s_processingKeyUp] = true;
                    try
                    {
                        if (PreProcessControlMessage(ref m) != PreProcessControlState.MessageProcessed)
                        {
                            DefWndProc(ref m);
                        }
                    }
                    finally
                    {
                        _axState[s_processingKeyUp] = false;
                    }

                    break;

                case User32.WM.NCDESTROY:
#if DEBUG
                    if (!OwnWindow())
                    {
                        Debug.WriteLineIf(
                            s_axHTraceSwitch.TraceVerbose,
                            $"WM_NCDESTROY control is destroying the window from under us...{GetType()}");
                    }
#endif
                    // Need to detach it now.
                    DetachAndForward(ref m);
                    break;

                default:
                    if (m.MsgInternal == _registeredMessage)
                    {
                        m.ResultInternal = (LRESULT)REGMSG_RETVAL;
                        return;
                    }

                    // Other things we may care about and we will pass them to the Control's wndProc
                    base.WndProc(ref m);
                    break;
            }
        }

        private unsafe void DetachAndForward(ref Message m)
        {
            DetachWindow();
            if (IsHandleCreated)
            {
                void* wndProc = (void*)PInvoke.GetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_WNDPROC);
                m.ResultInternal = PInvoke.CallWindowProc(
                    (delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, LRESULT>)wndProc,
                    HWND,
                    (uint)m.Msg,
                    m.WParamInternal,
                    m.LParamInternal);

                GC.KeepAlive(this);
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
            _wndprocAddr = PInvoke.GetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_WNDPROC);
        }

        private void AttachWindow(HWND hwnd)
        {
            Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"attaching window for {ToString()} {hwnd}");
            if (!_axState[s_fFakingWindow])
            {
                WindowAssignHandle(hwnd, _axState[s_assignUniqueID]);
            }

            UpdateZOrder();

            // Get the latest bounds set by the user.
            Size setExtent = Size;
            Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"SetBounds {setExtent}");

            // Get the default bounds set by the ActiveX control.
            UpdateBounds();
            Size ocxExtent = GetExtent();
            Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"OcxBounds {ocxExtent}");

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
        private static int Pix2HM(int pix, int logP)
        {
            return (HMperInch * pix + (logP >> 1)) / logP;
        }

        private static int HM2Pix(int hm, int logP)
        {
            return (logP * hm + HMperInch / 2) / HMperInch;
        }

        private unsafe bool QuickActivate()
        {
            if (_instance is not Ole32.IQuickActivate)
            {
                return false;
            }

            Ole32.IQuickActivate iqa = (Ole32.IQuickActivate)_instance;

            var qaContainer = new Ole32.QACONTAINER
            {
                cbSize = (uint)Marshal.SizeOf<Ole32.QACONTAINER>()
            };
            var qaControl = new Ole32.QACONTROL
            {
                cbSize = (uint)Marshal.SizeOf<Ole32.QACONTROL>()
            };

            qaContainer.pClientSite = _oleSite;
            qaContainer.pPropertyNotifySink = _oleSite;
            qaContainer.pFont = (Ole32.IFont)GetIFontFromFont(GetParentContainer()._parent.Font);
            qaContainer.dwAppearance = 0;
            qaContainer.lcid = PInvoke.GetThreadLocale();

            Control p = ParentInternal;

            if (p is not null)
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
            if (!hr.Succeeded)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"Failed to QuickActivate: {hr}");
                DisposeAxControl();
                return false;
            }

            _miscStatusBits = qaControl.dwMiscStatus;
            ParseMiscBits(_miscStatusBits);
            return true;
        }

        internal override void DisposeAxControls()
        {
            _axState[s_rejectSelection] = true;
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
            if (!GetControlEnabled() || _axState[s_rejectSelection])
            {
                return false;
            }

            return base.CanSelectCore();
        }

        /// <summary>
        ///  Frees all resources associated with this control. This method may not be
        ///  called at runtime. Any resources used by the control should be setup to
        ///  be released when the control is garbage collected. Inheriting classes should always
        ///  call base.dispose.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                TransitionDownTo(OC_PASSIVE);
                if (_newParent is not null)
                {
                    _newParent.Dispose();
                }

                if (_oleSite is not null)
                {
                    _oleSite.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        private bool GetSiteOwnsDeactivation()
        {
            return _axState[s_ownDisposing];
        }

        private void DisposeAxControl()
        {
            if (GetParentContainer() is not null)
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
            if (f is not null)
            {
                f.VisibleChanged -= _onContainerVisibleChanged;
            }

            try
            {
                if (_instance is not null)
                {
                    Marshal.FinalReleaseComObject(_instance);
                    _instance = null;
                    _iOleInPlaceObject = null;
                    _iOleObject = null;
                    _iOleControl = null;
                    _iOleInPlaceActiveObject = null;
                    _iOleInPlaceActiveObjectExternal = null;
                    _iPerPropertyBrowsing = null;
                    _iCategorizeProperties = null;
                    _iPersistStream = null;
                    _iPersistStreamInit = null;
                    _iPersistStorage = null;
                }

                _axState[s_checkedIppb] = false;
                _axState[s_checkedCP] = false;
                _axState[s_disposed] = true;

                _freezeCount = 0;
                _axState[s_sinkAttached] = false;
                _wndprocAddr = IntPtr.Zero;

                SetOcState(OC_PASSIVE);
            }
            finally
            {
                NoComponentChangeEvents--;
            }
        }

        private void ParseMiscBits(Ole32.OLEMISC bits)
        {
            _axState[s_fOwnWindow] = ((bits & Ole32.OLEMISC.INVISIBLEATRUNTIME) != 0) && IsUserMode();
            _axState[s_fSimpleFrame] = ((bits & Ole32.OLEMISC.SIMPLEFRAME) != 0);
        }

        private void SlowActivate()
        {
            bool setClientSite = false;

            if ((_miscStatusBits & Ole32.OLEMISC.SETCLIENTSITEFIRST) != 0)
            {
                GetOleObject().SetClientSite(_oleSite);
                setClientSite = true;
            }

            DepersistControl();

            if (!setClientSite)
            {
                GetOleObject().SetClientSite(_oleSite);
            }
        }

        private AxContainer GetParentContainer()
        {
            if (_container is null)
            {
                _container = AxContainer.FindContainerForControl(this);
            }

            if (_container is null)
            {
                ContainerControl f = ContainingControl;
                if (f is null)
                {
                    // ContainingControl can be null if the AxHost is still not parented to a containerControl
                    // In everett we used to return a parking window.
                    // now we just set the containingControl to a dummyValue.
                    if (_newParent is null)
                    {
                        _newParent = new ContainerControl();
                        _axContainer = _newParent.CreateAxContainer();
                        _axContainer.AddControl(this);
                    }

                    return _axContainer;
                }
                else
                {
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"calling upon {f} to create a container");
                    _container = f.CreateAxContainer();
                    _container.AddControl(this);
                    _containingControl = f;
                }
            }

            return _container;
        }

        private Ole32.IOleControl GetOleControl() => _iOleControl ??= (Ole32.IOleControl)_instance;

        private Ole32.IOleInPlaceActiveObject GetInPlaceActiveObject()
        {
            // if our AxContainer was set an external active object then use it.
            if (_iOleInPlaceActiveObjectExternal is not null)
            {
                return _iOleInPlaceActiveObjectExternal;
            }

            // otherwise use our instance.
            if (_iOleInPlaceActiveObject is null)
            {
                Debug.Assert(_instance is not null, "must have the ocx");
                try
                {
                    _iOleInPlaceActiveObject = (Ole32.IOleInPlaceActiveObject)_instance;
                }
                catch (InvalidCastException e)
                {
                    Debug.Fail($"Invalid cast in GetInPlaceActiveObject: {e}");
                }
            }

            return _iOleInPlaceActiveObject;
        }

        private Ole32.IOleObject GetOleObject() => _iOleObject ??= (Ole32.IOleObject)_instance;

        private Ole32.IOleInPlaceObject GetInPlaceObject()
        {
            if (_iOleInPlaceObject is null)
            {
                Debug.Assert(_instance is not null, "must have the ocx");
                _iOleInPlaceObject = (Ole32.IOleInPlaceObject)_instance;

#if DEBUG
                if (_iOleInPlaceObject is Ole32.IOleInPlaceObjectWindowless)
                {
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"{GetType().FullName} Can also be a Windowless control.");
                }
#endif //DEBUG
            }

            return _iOleInPlaceObject;
        }

        private VSSDK.ICategorizeProperties GetCategorizeProperties()
        {
            if (_iCategorizeProperties is null && !_axState[s_checkedCP] && _instance is not null)
            {
                _axState[s_checkedCP] = true;
                if (_instance is VSSDK.ICategorizeProperties properties)
                {
                    _iCategorizeProperties = properties;
                }
            }

            return _iCategorizeProperties;
        }

        private Oleaut32.IPerPropertyBrowsing GetPerPropertyBrowsing()
        {
            if (_iPerPropertyBrowsing is null && !_axState[s_checkedIppb] && _instance is not null)
            {
                _axState[s_checkedIppb] = true;
                if (_instance is Oleaut32.IPerPropertyBrowsing browsing)
                {
                    _iPerPropertyBrowsing = browsing;
                }
            }

            return _iPerPropertyBrowsing;
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
            return Ole32.OleCreatePictureIndirect(ref pictdesc, in s_ipicture_Guid, fOwn: true);
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
            return Ole32.OleCreatePictureIndirect(ref desc, in s_ipicture_Guid, fOwn: true);
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
            return Ole32.OleCreatePictureIndirect(ref desc, in s_ipictureDisp_Guid, fOwn: true);
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
            if (s_fontTable is null)
            {
                s_fontTable = new Dictionary<Font, Oleaut32.FONTDESC>();
            }
            else if (s_fontTable.TryGetValue(font, out Oleaut32.FONTDESC cachedFDesc))
            {
                return cachedFDesc;
            }

            LOGFONTW logfont = LOGFONTW.FromFont(font);
            var fdesc = new Oleaut32.FONTDESC
            {
                cbSizeOfStruct = (uint)Marshal.SizeOf<Oleaut32.FONTDESC>(),
                lpstrName = font.Name,
                cySize = (long)(font.SizeInPoints * 10000),
                sWeight = (short)logfont.lfWeight,
                sCharset = logfont.lfCharSet,
                fItalic = font.Italic,
                fUnderline = font.Underline,
                fStrikethrough = font.Strikeout
            };
            s_fontTable[font] = fdesc;
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
                return Oleaut32.OleCreateFontIndirect(ref fontDesc, in s_ifont_Guid);
            }
            catch
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"Failed to create IFrom from font: {font}");
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
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"Could not create font. {e.Message}");
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
            return Oleaut32.OleCreateIFontDispIndirect(ref fontdesc, in s_ifontDisp_Guid);
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

                return new Font(oleFont.Name, (float)oleFont.Size / 10000, style, GraphicsUnit.Point, (byte)oleFont.Charset);
            }
            catch (Exception e)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"Could not create font from: {oleFont.Name}. {e.Message}");
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

        private static int Convert2int(object o, bool xDirection)
        {
            o = ((Array)o).GetValue(0);

            // User controls & other visual basic related controls give us coordinates as floats in twips
            // but MFC controls give us integers as pixels.
            if (o.GetType() == typeof(float))
            {
                return Twip2Pixel(Convert.ToDouble(o, CultureInfo.InvariantCulture), xDirection);
            }

            return Convert.ToInt32(o, CultureInfo.InvariantCulture);
        }

        private static short Convert2short(object o)
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
            base.OnMouseMove(new MouseEventArgs((MouseButtons)(button << 20), 1, x, y, 0));
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
            base.OnMouseUp(new MouseEventArgs((MouseButtons)(button << 20), 1, x, y, 0));
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
            base.OnMouseDown(new MouseEventArgs((MouseButtons)(button << 20), 1, x, y, 0));
        }

        protected delegate void AboutBoxDelegate();
    }
}
