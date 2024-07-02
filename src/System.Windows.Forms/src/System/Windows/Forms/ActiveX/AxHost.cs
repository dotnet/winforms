// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Windows.Win32.System.Com;
using Windows.Win32.System.Com.StructuredStorage;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms;

/// <summary>
///  Wraps ActiveX controls and exposes them as fully featured windows forms controls.
/// </summary>
[ToolboxItem(false)]
[DesignTimeVisible(false)]
[DefaultEvent(nameof(Enter))]
[Designer($"System.Windows.Forms.Design.AxHostDesigner, {AssemblyRef.SystemDesign}")]
public abstract unsafe partial class AxHost : Control, ISupportInitialize, ICustomTypeDescriptor
{
#if DEBUG
    private static readonly BooleanSwitch s_axAlwaysSaveSwitch = new(
        "AxAlwaysSave",
        "ActiveX to save all controls regardless of their IsDirty function return value");
#endif

    // E_INVALID_ARG
    private static readonly COMException s_invalidArgumentException = new(SR.AXInvalidArgument, unchecked((int)0x80070057));

    private const int OC_PASSIVE = 0;
    private const int OC_LOADED = 1;    // handler, but no server   [ocx created]
    private const int OC_RUNNING = 2;   // server running, invisible [iqa & depersistance]
    private const int OC_INPLACE = 4;   // server in-place active [inplace]
    private const int OC_UIACTIVE = 8;  // server is UI active [uiactive]
    private const int OC_OPEN = 16;     // server is being open edited [not used]

    private const int EDITM_NONE = 0;   // object not being edited
    private const int EDITM_OBJECT = 1; // object provided an edit verb and we invoked it
    private const int EDITM_HOST = 2;   // we invoked our own edit verb

    private readonly uint _subclassCheckMessage
        = PInvoke.RegisterWindowMessage($"{Application.WindowMessagesVersion}_subclassCheck");

    private const int REGMSG_RETVAL = 123;

    private static int s_logPixelsX = -1;
    private static int s_logPixelsY = -1;

    private static readonly Guid s_ivbformat_Guid = IID.GetRef<IVBFormat>();
    private static readonly Guid s_ioleobject_Guid = IID.GetRef<IOleObject>();
    private static readonly Guid s_dataSource_Guid = new("{7C0FFAB3-CD84-11D0-949A-00A0C91110ED}");
    private static readonly Guid s_windowsMediaPlayer_Clsid = new("{22d6f312-b0f6-11d0-94ab-0080c74c7e95}");
    private static readonly Guid s_comctlImageCombo_Clsid = new("{a98a24c0-b06f-3684-8c12-c52ae341e0bc}");
    private static readonly Guid s_maskEdit_Clsid = new("{c932ba85-4374-101b-a56c-00aa003668dc}");

    // Static state for perf optimization
    private static ConditionalWeakTable<Font, object>? s_fontTable;

    // BitVector32 masks for various internal state flags.
    private static readonly int s_ocxStateSet = BitVector32.CreateMask();
    private static readonly int s_editorRefresh = BitVector32.CreateMask(s_ocxStateSet);
    private static readonly int s_listeningToIdle = BitVector32.CreateMask(s_editorRefresh);
    private static readonly int s_refreshProperties = BitVector32.CreateMask(s_listeningToIdle);

    /// <summary>True if a window needs created when <see cref="CreateHandle"/> is called.</summary>
    private static readonly int s_fNeedOwnWindow = BitVector32.CreateMask(s_refreshProperties);

    /// <summary>True if the OCX is design time only and we're in user mode.</summary>
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

    private StorageType _storageType = StorageType.Unknown;
    private int _ocState = OC_PASSIVE;
    private OLEMISC _miscStatusBits;
    private int _freezeCount;
    private readonly int _flags;
    private int _selectionStyle;
    private int _editMode = EDITM_NONE;
    private int _noComponentChange;

    private IntPtr _wndprocAddr = IntPtr.Zero;

    private readonly Guid _clsid;
    private string _text = string.Empty;
    private string? _licenseKey;

    private readonly OleInterfaces _oleSite;
    private AxComponentEditor? _editor;
    private AxContainer? _container;
    private ContainerControl? _containingControl;
    private ContainerControl? _newParent;
    private AxContainer? _axContainer;
    private State? _ocxState;
    private HWND _hwndFocus;

    // CustomTypeDescriptor related state

    private Dictionary<string, PropertyDescriptor>? _properties;
    private Dictionary<string, PropertyInfo>? _propertyInfos;
    private PropertyDescriptorCollection? _propsStash;
    private Attribute[]? _attribsStash;

    // Interface pointers to the ocx

    private object? _instance;
    private AgileComPointer<IOleInPlaceActiveObject>? _iOleInPlaceActiveObjectExternal;

    private AboutBoxDelegate? _aboutBoxDelegate;
    private readonly EventHandler _selectionChangeHandler;

    private readonly bool _isMaskEdit;
    private bool _ignoreDialogKeys;

    private readonly EventHandler _onContainerVisibleChanged;

    // These should be in the order given by the PROPCAT_X values
    // Also, note that they are not to be localized...

    private static readonly CategoryAttribute?[] s_categoryNames =
    [
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
    ];

    private Dictionary<PROPCAT, CategoryAttribute>? _objectDefinedCategoryNames;

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
    protected AxHost(string clsid)
        : this(clsid, 0)
    {
    }

    /// <summary>
    ///  Creates a new instance of a control which wraps an activeX control given by the
    ///  clsid and flags parameters.
    /// </summary>
    protected AxHost(string clsid, int flags)
        : base()
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
        using var oleObject = GetComScope<IOleObject>();
        HRESULT hr = oleObject.Value->GetMiscStatus(DVASPECT.DVASPECT_CONTENT, out OLEMISC bits);
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
    public override Image? BackgroundImage
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
    public new ImeMode ImeMode
    {
        get => base.ImeMode;
        set => base.ImeMode = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? MouseClick
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseClick"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? MouseDoubleClick
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseDoubleClick"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AllowNull]
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
    public new virtual bool Enabled
    {
        get => base.Enabled;
        set => base.Enabled = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AllowNull]
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
            return rtol == Forms.RightToLeft.Yes;
        }
        set => base.RightToLeft = (value) ? Forms.RightToLeft.Yes : Forms.RightToLeft.No;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AllowNull]
    public override string Text
    {
        get => _text;
        set => _text = value ?? string.Empty;
    }

    internal override bool CanAccessProperties
    {
        get
        {
            int ocState = GetOcState();
            return (_axState[s_fOwnWindow] && (ocState > OC_RUNNING || (IsUserMode() && ocState >= OC_RUNNING)))
                || ocState >= OC_INPLACE;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected bool PropsValid() => CanAccessProperties;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public void BeginInit()
    {
    }

    /// <summary>
    ///  Signals the object that loading of all peer components and property sets are complete.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   It should be possible to invoke any property get or set after calling this method. Note that a side effect
    ///   of this method is the creation of the parent control's handle, therefore, this control must be parented
    ///   before begin is called.
    ///  </para>
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public void EndInit()
    {
        if (ParentInternal is not null)
        {
            ParentInternal.CreateControl(true);

            ContainerControl? containingControl = ContainingControl;
            if (containingControl is not null)
            {
                containingControl.VisibleChanged += _onContainerVisibleChanged;
            }
        }
    }

    private void OnContainerVisibleChanged(object? sender, EventArgs e)
    {
        ContainerControl? containingControl = ContainingControl;
        if (containingControl is not null)
        {
            if (containingControl.Visible && Visible && !_axState[s_fOwnWindow])
            {
                MakeVisibleWithShow();
            }
            else if (!containingControl.Visible && Visible && IsHandleCreated && GetOcState() >= OC_INPLACE)
            {
                HideAxControl();
            }
            else if (containingControl.Visible && !GetState(States.Visible) && IsHandleCreated && GetOcState() >= OC_INPLACE)
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
    public new event EventHandler? BackColorChanged
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "BackColorChanged"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackgroundImageChanged
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "BackgroundImageChanged"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackgroundImageLayoutChanged
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "BackgroundImageLayoutChanged"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BindingContextChanged
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "BindingContextChanged"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? CursorChanged
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "CursorChanged"));
        remove { }
    }

    /// <summary>
    ///  Occurs when the control is enabled.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? EnabledChanged
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "EnabledChanged"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? FontChanged
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "FontChanged"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? ForeColorChanged
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "ForeColorChanged"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? RightToLeftChanged
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "RightToLeftChanged"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? TextChanged
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "TextChanged"));
        remove { }
    }

    /// <summary>
    ///  Occurs when the control is clicked.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? Click
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "Click"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event DragEventHandler? DragDrop
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "DragDrop"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event DragEventHandler? DragEnter
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "DragEnter"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event DragEventHandler? DragOver
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "DragOver"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? DragLeave
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "DragLeave"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event GiveFeedbackEventHandler? GiveFeedback
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "GiveFeedback"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event HelpEventHandler? HelpRequested
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "HelpRequested"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event PaintEventHandler? Paint
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "Paint"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event QueryContinueDragEventHandler? QueryContinueDrag
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "QueryContinueDrag"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event QueryAccessibilityHelpEventHandler? QueryAccessibilityHelp
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "QueryAccessibilityHelp"));
        remove { }
    }

    /// <summary>
    ///  Occurs when the control is double clicked.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? DoubleClick
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "DoubleClick"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? ImeModeChanged
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "ImeModeChanged"));
        remove { }
    }

    /// <summary>
    ///  Occurs when a key is pressed down while the control has focus.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event KeyEventHandler? KeyDown
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "KeyDown"));
        remove { }
    }

    /// <summary>
    ///  Occurs when a key is pressed while the control has focus.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event KeyPressEventHandler? KeyPress
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "KeyPress"));
        remove { }
    }

    /// <summary>
    ///  Occurs when a key is released while the control has focus.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event KeyEventHandler? KeyUp
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "KeyUp"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event LayoutEventHandler? Layout
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "Layout"));
        remove { }
    }

    /// <summary>
    ///  Occurs when the mouse pointer is over the control and a mouse button is pressed.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event MouseEventHandler? MouseDown
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseDown"));
        remove { }
    }

    /// <summary>
    ///  Occurs when the mouse pointer enters the AxHost.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? MouseEnter
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseEnter"));
        remove { }
    }

    /// <summary>
    ///  Occurs when the mouse pointer leaves the AxHost.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? MouseLeave
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseLeave"));
        remove { }
    }

    /// <summary>
    ///  Occurs when the mouse pointer hovers over the control.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? MouseHover
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseHover"));
        remove { }
    }

    /// <summary>
    ///  Occurs when the mouse pointer is moved over the AxHost.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event MouseEventHandler? MouseMove
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseMove"));
        remove { }
    }

    /// <summary>
    ///  Occurs when the mouse pointer is over the control and a mouse button is released.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event MouseEventHandler? MouseUp
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseUp"));
        remove { }
    }

    /// <summary>
    ///  Occurs when the mouse wheel moves while the control has focus.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event MouseEventHandler? MouseWheel
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseWheel"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event UICuesEventHandler? ChangeUICues
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "ChangeUICues"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? StyleChanged
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "StyleChanged"));
        remove { }
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        AmbientChanged(PInvokeCore.DISPID_AMBIENT_FONT);
    }

    protected override void OnForeColorChanged(EventArgs e)
    {
        base.OnForeColorChanged(e);
        AmbientChanged(PInvokeCore.DISPID_AMBIENT_FORECOLOR);
    }

    protected override void OnBackColorChanged(EventArgs e)
    {
        base.OnBackColorChanged(e);
        AmbientChanged(PInvokeCore.DISPID_AMBIENT_BACKCOLOR);
    }

    private void AmbientChanged(int dispid)
    {
        if (GetOcx() is not null)
        {
            Invalidate();
            using var oleControl = GetComScope<IOleControl>();
            oleControl.Value->OnAmbientPropertyChange(dispid).AssertSuccess();
        }
    }

    private bool OwnWindow() => _axState[s_fOwnWindow] || _axState[s_fFakingWindow];

    private HWND GetHandleNoCreate() => IsHandleCreated ? (HWND)Handle : default;

    private void AddSelectionHandler()
    {
        if (_axState[s_addedSelectionHandler])
        {
            return;
        }

        if (Site.TryGetService(out ISelectionService? selectionService))
        {
            selectionService.SelectionChanging += _selectionChangeHandler;
        }

        _axState[s_addedSelectionHandler] = true;
    }

    private void OnComponentRename(object? sender, ComponentRenameEventArgs e)
    {
        // When we're notified of a rename, see if this is the component that is being renamed.
        if (e.Component == this)
        {
            // If it is, call DISPID_AMBIENT_DISPLAYNAME directly on the control itself.
            if (GetOcx() is IOleControl.Interface oleCtl)
            {
                oleCtl.OnAmbientPropertyChange(PInvokeCore.DISPID_AMBIENT_DISPLAYNAME);
            }
        }
    }

    private bool RemoveSelectionHandler()
    {
        if (!_axState[s_addedSelectionHandler])
        {
            return false;
        }

        if (Site.TryGetService(out ISelectionService? selectionService))
        {
            selectionService.SelectionChanging -= _selectionChangeHandler;
        }

        _axState[s_addedSelectionHandler] = false;
        return true;
    }

    private void SyncRenameNotification(bool hook)
    {
        if (DesignMode && hook != _axState[s_renameEventHooked])
        {
            // If we're in design mode, listen to the following events from the component change service.
            IComponentChangeService? changeService = (IComponentChangeService?)GetService(typeof(IComponentChangeService));

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

    public override ISite? Site
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
                ContainerControl? containingControl = ContainingControl;
                if (containingControl is not null && containingControl.Visible && Visible)
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
                // SetupClass_Info(this);
            }
        }
    }

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

    private void OnNewSelection(object? sender, EventArgs e)
    {
        if (IsUserMode() || !Site.TryGetService(out ISelectionService? selectionService))
        {
            return;
        }

        // If we are uiactive and we lose selection, then we need to uideactivate ourselves.

        if (GetOcState() >= OC_UIACTIVE && !selectionService.GetComponentSelected(this))
        {
            // Need to deactivate.
            UiDeactivate().AssertSuccess();
        }

        if (!selectionService.GetComponentSelected(this))
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
            // The AX Host designer will offer an extender property called "SelectionStyle".
            if (TypeDescriptor.GetProperties(this)["SelectionStyle"] is { } property && property.PropertyType == typeof(int))
            {
                if ((int)property.GetValue(this)! != _selectionStyle)
                {
                    property.SetValue(this, _selectionStyle);
                }
            }
        }
    }

    // DrawToBitmap doesn't work for this control, so we should hide it. We'll
    // still call base so that this has a chance to work if it can.
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds)
    {
        base.DrawToBitmap(bitmap, targetBounds);
    }

    protected override void CreateHandle()
    {
        if (IsHandleCreated)
        {
            return;
        }

        TransitionUpTo(OC_RUNNING);
        if (_axState[s_fOwnWindow])
        {
            // Design time only OCX and we're not in design mode. We shouldn't be visible.
            SetState(States.Visible, false);
            base.CreateHandle();
        }
        else if (_axState[s_fNeedOwnWindow])
        {
            Debug.Assert(!Visible, "If we were visible we would not be need a fake window.");
            _axState[s_fNeedOwnWindow] = false;
            _axState[s_fFakingWindow] = true;
            base.CreateHandle();

            // Note that we do not need to attach the handle because the work usually done in there
            // will be done in Control's wndProc on WM_CREATE.
        }
        else
        {
            TransitionUpTo(OC_INPLACE);

            // It is possible that we were hidden while in place activating, in which case we don't really have a
            // handle now because the act of hiding could have destroyed it. Just call ourselves again recursively,
            // and if we don't have a handle, we will just take the "axState[fNeedOwnWindow]" path above.
            if (_axState[s_fNeedOwnWindow])
            {
                Debug.Assert(!IsHandleCreated, "if we need a fake window, we can't have a real one");
                CreateHandle();
                return;
            }
        }

        GetParentContainer().ControlCreated(this);
    }

    private static HRESULT SetupLogPixels(bool force)
    {
        if (s_logPixelsX == -1 || force)
        {
            using var dc = GetDcScope.ScreenDC;
            if (dc.IsNull)
            {
                return HRESULT.E_FAIL;
            }

            s_logPixelsX = PInvokeCore.GetDeviceCaps(dc, GET_DEVICE_CAPS_INDEX.LOGPIXELSX);
            s_logPixelsY = PInvokeCore.GetDeviceCaps(dc, GET_DEVICE_CAPS_INDEX.LOGPIXELSY);
        }

        return HRESULT.S_OK;
    }

    private unsafe void HiMetric2Pixel(ref Size sz)
    {
        Point phm = new(sz.Width, sz.Height);
        PointF pcont = default;
        ((IOleControlSite.Interface)_oleSite).TransformCoords(
            (POINTL*)&phm,
            &pcont,
            (uint)(XFORMCOORDS.XFORMCOORDS_SIZE | XFORMCOORDS.XFORMCOORDS_HIMETRICTOCONTAINER));

        sz.Width = (int)pcont.X;
        sz.Height = (int)pcont.Y;
    }

    private unsafe void Pixel2hiMetric(ref Size sz)
    {
        Point phm = default;
        PointF pcont = new(sz.Width, sz.Height);
        ((IOleControlSite.Interface)_oleSite).TransformCoords(
            (POINTL*)&phm,
            &pcont,
            (uint)(XFORMCOORDS.XFORMCOORDS_SIZE | XFORMCOORDS.XFORMCOORDS_CONTAINERTOHIMETRIC));

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
        Size size = new(width, height);
        bool resetExtents = !IsUserMode();
        Pixel2hiMetric(ref size);
        using var oleObject = GetComScope<IOleObject>();
        HRESULT hr = oleObject.Value->SetExtent(DVASPECT.DVASPECT_CONTENT, (SIZE*)&size);
        if (hr != HRESULT.S_OK)
        {
            resetExtents = true;
        }

        if (resetExtents)
        {
            oleObject.Value->GetExtent(DVASPECT.DVASPECT_CONTENT, (SIZE*)&size).AssertSuccess();
            oleObject.Value->SetExtent(DVASPECT.DVASPECT_CONTENT, (SIZE*)&size).AssertSuccess();
        }

        return GetExtent();
    }

    private unsafe Size GetExtent()
    {
        Size size = default;
        using var oleObject = GetComScope<IOleObject>();
        oleObject.Value->GetExtent(DVASPECT.DVASPECT_CONTENT, (SIZE*)&size).AssertSuccess();
        HiMetric2Pixel(ref size);
        return size;
    }

    /// <summary>
    ///  ActiveX controls scale themselves, so GetScaledBounds simply returns their original unscaled bounds.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified) => bounds;

    private unsafe void SetObjectRects(Rectangle bounds)
    {
        if (GetOcState() < OC_INPLACE)
        {
            return;
        }

        RECT posRect = bounds;
        RECT clipRect = WebBrowserHelper.GetClipRect();
        using var inPlaceObject = GetComScope<IOleInPlaceObject>();
        inPlaceObject.Value->SetObjectRects(&posRect, &clipRect).ThrowOnFailure();
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

            if (oldBounds.X == x && oldBounds.Y == y && oldBounds.Width == width && oldBounds.Height == height)
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

        if ((int)PInvoke.SendMessage(this, _subclassCheckMessage) == REGMSG_RETVAL)
        {
            _wndprocAddr = currentWndproc;
            return true;
        }

        // We were resubclassed, we need to resublass ourselves.
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
        else if (IsHandleCreated)
        {
            TransitionDownTo(OC_RUNNING);
        }
    }

    private void TransitionDownTo(int state)
    {
        if (_axState[s_inTransition])
        {
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
                        UiDeactivate().AssertSuccess();
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
            return;
        }

        try
        {
            _axState[s_inTransition] = true;

            while (state > GetOcState())
            {
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
                            // createSink();
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
                        DoVerb((int)OLEIVERB.OLEIVERB_SHOW);
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
            DoVerb((int)OLEIVERB.OLEIVERB_INPLACEACTIVATE);
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
        ContainerControl? containingControl = ContainingControl;
        if (containingControl is not null)
        {
            if (containingControl.ActiveControl == this)
            {
                containingControl.ActiveControl = null;
            }
        }

        using var inPlaceObject = GetComScope<IOleInPlaceObject>();
        return inPlaceObject.Value->InPlaceDeactivate();
    }

    private void UiActivate()
    {
        Debug.Assert(CanUIActivate, "we have to be able to uiactivate");
        if (CanUIActivate)
        {
            DoVerb((int)OLEIVERB.OLEIVERB_UIACTIVATE);
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
            try
            {
                ((IOleClientSite.Interface)_oleSite).ShowObject();
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
        if (GetState(States.Visible) == value)
        {
            return;
        }

        bool oldVisible = Visible;
        if ((IsHandleCreated || value) && ParentInternal is not null && ParentInternal.Created && !_axState[s_fOwnWindow])
        {
            TransitionUpTo(OC_RUNNING);
            if (value)
            {
                if (_axState[s_fFakingWindow])
                {
                    // First we need to destroy the fake window.
                    DestroyFakeWindow();
                }

                // We want to avoid using SHOW since that may uiactivate us, and we don't want that.
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

    private void MakeVisibleWithShow()
    {
        ContainerControl? container = ContainingControl;
        Control? control = container?.ActiveControl;
        try
        {
            DoVerb((int)OLEIVERB.OLEIVERB_SHOW);
        }
        catch (Exception t)
        {
            Debug.Fail(t.ToString());
            throw new TargetInvocationException(string.Format(SR.AXNohWnd, GetType().Name), t);
        }

        EnsureWindowPresent();
        CreateControl(ignoreVisible: true);
        if (container is not null && container.ActiveControl != control)
        {
            container.ActiveControl = control;
        }
    }

    private void HideAxControl()
    {
        Debug.Assert(!_axState[s_fOwnWindow], "can't own our window when hiding");
        Debug.Assert(IsHandleCreated, "gotta have a window to hide");
        Debug.Assert(GetOcState() >= OC_INPLACE, "have to be in place in order to hide.");

        DoVerb((int)OLEIVERB.OLEIVERB_HIDE);
        if (GetOcState() < OC_INPLACE)
        {
            Debug.Assert(!IsHandleCreated, "if we are inplace deactivated we should not have a window.");

            // Set a flag saying that we need the window to be created if create handle is ever called.
            _axState[s_fNeedOwnWindow] = true;

            // Set the state to our "pretend oc_inplace state".
            SetOcState(OC_INPLACE);
        }
    }

    protected override bool IsInputChar(char charCode) => true;

    protected override bool ProcessDialogKey(Keys keyData) => !_ignoreDialogKeys && base.ProcessDialogKey(keyData);

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
    public override unsafe bool PreProcessMessage(ref Message msg)
    {
        if (!IsUserMode())
        {
            return false;
        }

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
            // If our AxContainer was set an external active object then use it.
            using var activeObject = _iOleInPlaceActiveObjectExternal is { } external
                ? external.GetInterface()
                : TryGetComScope<IOleInPlaceActiveObject>(out HRESULT hr);

            if (activeObject.IsNull)
            {
                return false;
            }

            hr = activeObject.Value->TranslateAccelerator(&win32Message);
            msg.MsgInternal = (MessageId)win32Message.message;
            msg.WParamInternal = win32Message.wParam;
            msg.LParamInternal = win32Message.lParam;
            msg.HWnd = win32Message.hwnd;

            if (hr == HRESULT.S_OK)
            {
                return true;
            }
            else if (hr == HRESULT.S_FALSE)
            {
                _ignoreDialogKeys = true;
                try
                {
                    return base.PreProcessMessage(ref msg);
                }
                finally
                {
                    _ignoreDialogKeys = false;
                }
            }
            else
            {
                return _axState[s_siteProcessedInputKey] && base.PreProcessMessage(ref msg);
            }
        }
        finally
        {
            _axState[s_siteProcessedInputKey] = false;
        }
    }

    /// <summary>
    ///  Process a mnemonic character. This is done by manufacturing a WM_SYSKEYDOWN message and passing it to the
    ///  ActiveX control.
    /// </summary>
    protected internal override unsafe bool ProcessMnemonic(char charCode)
    {
        if (!CanSelect)
        {
            return false;
        }

        CONTROLINFO ctlInfo = new()
        {
            cb = (uint)sizeof(CONTROLINFO)
        };

        using var oleControl = GetComScope<IOleControl>();
        if (oleControl.Value->GetControlInfo(&ctlInfo).Failed)
        {
            return false;
        }

        PInvoke.GetCursorPos(out Point point);

        MSG msg = new()
        {
            // We don't have a message so we must create one ourselves.
            // The message we are creating is a WM_SYSKEYDOWN with the right alt key setting.
            hwnd = (ContainingControl is null) ? HWND.Null : ContainingControl.HWND,
            message = PInvoke.WM_SYSKEYDOWN,
            wParam = (WPARAM)char.ToUpper(charCode, CultureInfo.CurrentCulture),
            // 0x20180001
            lParam = LPARAM.MAKELPARAM(
                1, // Repeat count
                (int)(PInvoke.KF_ALTDOWN | 0x18)), // 0x18 => O key scan code
            time = PInvoke.GetTickCount(),
            pt = point
        };

        if (PInvoke.IsAccelerator(new HandleRef<HACCEL>(this, ctlInfo.hAccel), ctlInfo.cAccel, &msg, lpwCmd: null))
        {
            oleControl.Value->OnMnemonic(&msg).AssertSuccess();

            try
            {
                Focus();
            }
            catch (Exception t)
            {
                Debug.Fail($"error in processMnemonic: {t}");
            }

            return true;
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
    ///  Gets or sets the persisted state of the ActiveX control.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Computes the persisted state of the underlying ActiveX control and returns it in the encapsulated
    ///   <see cref="State"/> object. If the control has been modified since it was last saved to a persisted
    ///   state, it will be asked to save itself.
    ///  </para>
    ///  <para>
    ///   This should either be null, obtained from the getter, or read from a resource. The value of this property
    ///   will be used after the control is created but before it is shown.
    ///  </para>
    /// </remarks>
    [DefaultValue(null)]
    [RefreshProperties(RefreshProperties.All)]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public State? OcxState
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

            if (_storageType != StorageType.Unknown && _storageType != value.Type)
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
                _axState[s_manualUpdate] = _ocxState.ManualUpdate;
                _licenseKey = _ocxState.LicenseKey;
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

    private State? CreateNewOcxState(State? oldOcxState)
    {
        NoComponentChangeEvents++;

        try
        {
            if (GetOcState() < OC_RUNNING)
            {
                return null;
            }

            PropertyBagStream? propBag = null;
            MemoryStream? ms = null;
            switch (_storageType)
            {
                case StorageType.Stream:
                case StorageType.StreamInit:
                    ms = new MemoryStream();
                    using (var stream = ms.ToIStream())
                    {
                        if (_storageType == StorageType.Stream)
                        {
                            using var persistStream = ComHelpers.GetComScope<IPersistStream>(_instance);
                            persistStream.Value->Save(stream, fClearDirty: true).AssertSuccess();
                        }
                        else
                        {
                            using var persistStreamInit = ComHelpers.GetComScope<IPersistStreamInit>(_instance);
                            persistStreamInit.Value->Save(stream, fClearDirty: true).AssertSuccess();
                        }
                    }

                    return new State(ms, _storageType, this);
                case StorageType.Storage:
                    Debug.Assert(oldOcxState is not null, "we got to have an old state which holds out scribble storage...");
                    if (oldOcxState is not null)
                    {
                        using var persistStorage = ComHelpers.GetComScope<IPersistStorage>(_instance);
                        return oldOcxState.RefreshStorage(persistStorage);
                    }

                    return null;
                case StorageType.PropertyBag:
                    propBag = new PropertyBagStream();
                    using (var propertyBag = ComHelpers.GetComScope<IPropertyBag>(propBag))
                    using (var persistPropBag = ComHelpers.GetComScope<IPersistPropertyBag>(_instance))
                    {
                        persistPropBag.Value->Save(propertyBag, fClearDirty: true, fSaveAllProperties: true).AssertSuccess();
                    }

                    return new State(propBag);
                default:
                    Debug.Fail("unknown storage type.");
                    return null;
            }
        }
        catch (Exception)
        {
        }
        finally
        {
            NoComponentChangeEvents--;
        }

        return null;
    }

    /// <summary>
    ///  Gets or sets the control logically containing the ActiveX control.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The <see cref="ContainingControl"/> property value can be different from the <see cref="Control.Parent"/>
    ///   property. The <see cref="ContainingControl"/> represented by this property is the ActiveX control's
    ///   logical container. For example, if an ActiveX control is hosted in a <see cref="GroupBox"/> control, and
    ///   the <see cref="GroupBox"/> is contained on a <see cref="Form"/>, then the <see cref="ContainingControl"/>
    ///   property value of the ActiveX control is the <see cref="Form"/>, and the <see cref="Control.Parent"/>
    ///   property value is the <see cref="GroupBox"/> control.
    ///  </para>
    /// </remarks>
    /// <devdoc>
    ///  <para>
    ///   At design time this is always the form being designed. At design time the default is the first
    ///   <see cref="ContainerControl"/> in the parent hierarchy.
    ///  </para>
    ///  <para>
    ///   The logical container of this control determines the set of logical sibling controls. In general this
    ///   property exists only to enable some specific behaviours of ActiveX controls and should not be set by
    ///   the user.
    ///  </para>
    /// </devdoc>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ContainerControl? ContainingControl
    {
        get => _containingControl ??= FindContainerControlInternal();
        set => _containingControl = value;
    }

    /// <summary>
    ///  Determines if the Text property needs to be persisted.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal override bool ShouldSerializeText()
    {
        try
        {
            return Text.Length != 0;
        }
        catch (COMException)
        {
        }

        return false;
    }

    /// <summary>
    ///  Determines whether to persist the ContainingControl property.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    private bool ShouldSerializeContainingControl() => ContainingControl != ParentInternal;

    private ContainerControl? FindContainerControlInternal()
    {
        if (Site.TryGetService(out IDesignerHost? host) && host.RootComponent is ContainerControl rootControl)
        {
            return rootControl;
        }

        Control? control = this;
        while (control is not null)
        {
            if (control is ContainerControl containerControl)
            {
                return containerControl;
            }

            control = control.ParentInternal;
        }

        return null;
    }

    private bool IsDirty()
    {
        if (GetOcState() < OC_RUNNING)
        {
            return false;
        }

        Debug.Assert(_storageType != StorageType.Unknown, "if we are loaded, out storage type must be set!");

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
        HRESULT hr = HRESULT.E_FAIL;
        switch (_storageType)
        {
            case StorageType.Stream:
                using (var persistStream = ComHelpers.GetComScope<IPersistStream>(_instance))
                {
                    hr = persistStream.Value->IsDirty();
                }

                break;
            case StorageType.StreamInit:
                using (var persistStreamInit = ComHelpers.GetComScope<IPersistStreamInit>(_instance))
                {
                    hr = persistStreamInit.Value->IsDirty();
                }

                break;
            case StorageType.Storage:
                using (var persistStorage = ComHelpers.GetComScope<IPersistStorage>(_instance))
                {
                    hr = persistStorage.Value->IsDirty();
                }

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
        ISite? site = Site;
        return site is null || !site.DesignMode;
    }

    private object? GetAmbientProperty(int dispid)
    {
        Control? richParent = ParentInternal;

        switch (dispid)
        {
            case PInvokeCore.DISPID_AMBIENT_USERMODE:
                return IsUserMode();
            case PInvokeCore.DISPID_AMBIENT_AUTOCLIP:
                return true;
            case PInvokeCore.DISPID_AMBIENT_MESSAGEREFLECT:
                return true;
            case PInvokeCore.DISPID_AMBIENT_UIDEAD:
                return false;
            case PInvokeCore.DISPID_AMBIENT_DISPLAYASDEFAULT:
                return false;
            case PInvokeCore.DISPID_AMBIENT_FONT:
                if (richParent is not null)
                {
                    return GetIFontFromFont(richParent.Font);
                }

                return null;
            case PInvokeCore.DISPID_AMBIENT_SHOWGRABHANDLES:
                return false;
            case PInvokeCore.DISPID_AMBIENT_SHOWHATCHING:
                return false;
            case PInvokeCore.DISPID_AMBIENT_BACKCOLOR:
                if (richParent is not null)
                {
                    return GetOleColorFromColor(richParent.BackColor);
                }

                return null;
            case PInvokeCore.DISPID_AMBIENT_FORECOLOR:
                if (richParent is not null)
                {
                    return GetOleColorFromColor(richParent.ForeColor);
                }

                return null;
            case PInvokeCore.DISPID_AMBIENT_DISPLAYNAME:
                return AxContainer.GetNameForControl(this) ?? string.Empty;
            case PInvokeCore.DISPID_AMBIENT_LOCALEID:
                return PInvokeCore.GetThreadLocale();
            case PInvokeCore.DISPID_AMBIENT_RIGHTTOLEFT:
                Control? control = this;
                while (control is not null)
                {
                    if (control.RightToLeft == Forms.RightToLeft.No)
                    {
                        return false;
                    }

                    if (control.RightToLeft == Forms.RightToLeft.Yes)
                    {
                        return true;
                    }

                    if (control.RightToLeft == Forms.RightToLeft.Inherit)
                    {
                        control = control.Parent;
                    }
                }

                return null;
            default:
                return null;
        }
    }

    public unsafe void DoVerb(int verb)
    {
        Control? parent = ParentInternal;
        RECT posRect = Bounds;
        using var pClientSite = ComHelpers.TryGetComScope<IOleClientSite>(_oleSite, out HRESULT hr);
        hr.AssertSuccess();
        using var oleObject = GetComScope<IOleObject>();
        oleObject.Value->DoVerb(
            verb,
            lpmsg: null,
            pClientSite,
            -1,
            parent is null ? HWND.Null : parent.HWND,
            &posRect).AssertSuccess();
    }

    private bool AwaitingDefreezing() => _freezeCount > 0;

    private void FreezeEvents(bool freeze)
    {
        using var oleControl = GetComScope<IOleControl>();

        if (freeze)
        {
            oleControl.Value->FreezeEvents(true).AssertSuccess();
            _freezeCount++;
        }
        else
        {
            oleControl.Value->FreezeEvents(false).AssertSuccess();
            _freezeCount--;
        }

        Debug.Assert(_freezeCount >= 0, "invalid freeze count!");
    }

    private HRESULT UiDeactivate()
    {
        bool ownDispose = _axState[s_ownDisposing];
        _axState[s_ownDisposing] = true;
        try
        {
            using var inPlaceObject = GetComScope<IOleInPlaceObject>();
            return inPlaceObject.Value->UIDeactivate();
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

    private string? GetLicenseKey()
    {
        return GetLicenseKey(_clsid);
    }

    private unsafe string? GetLicenseKey(Guid clsid)
    {
        if (_licenseKey is not null || !_axState[s_needLicenseKey])
        {
            return _licenseKey;
        }

        using ComScope<IClassFactory2> factory = new(null);

        HRESULT hr = PInvoke.CoGetClassObject(
            &clsid,
            CLSCTX.CLSCTX_INPROC_SERVER,
            null,
            IID.Get<IClassFactory2>(),
            factory);

        if (!hr.Succeeded)
        {
            if (hr == HRESULT.E_NOINTERFACE)
            {
                return null;
            }

            _axState[s_needLicenseKey] = false;
            return null;
        }

        LICINFO licInfo = new()
        {
            cbLicInfo = sizeof(LICINFO)
        };

        hr = factory.Value->GetLicInfo(&licInfo);
        if (hr.Succeeded && licInfo.fRuntimeKeyAvail)
        {
            using BSTR key = default;
            factory.Value->RequestLicKey(0, &key);
            _licenseKey = key.ToString();
            return _licenseKey;
        }

        return null;
    }

    private void CreateWithoutLicense(Guid clsid)
    {
        IUnknown* unknown;
        HRESULT hr = PInvokeCore.CoCreateInstance(
            &clsid,
            (IUnknown*)null,
            CLSCTX.CLSCTX_INPROC_SERVER,
            IID.Get<IUnknown>(),
            (void**)&unknown);
        hr.ThrowOnFailure();

        _instance = ComHelpers.GetObjectForIUnknown(unknown);
    }

    private void CreateWithLicense(string? license, Guid clsid)
    {
        if (license is not null)
        {
            using ComScope<IClassFactory2> factory = new(null);

            HRESULT hr = PInvoke.CoGetClassObject(
                &clsid,
                CLSCTX.CLSCTX_INPROC_SERVER,
                null,
                IID.Get<IClassFactory2>(),
                (void**)factory);

            if (hr.Succeeded)
            {
                IUnknown* unknown;
                hr = factory.Value->CreateInstanceLic(null, null, IID.Get<IUnknown>(), new BSTR(license), (void**)&unknown);
                hr.ThrowOnFailure();

                _instance = ComHelpers.GetObjectForIUnknown(unknown);
            }
        }

        if (_instance is null)
        {
            CreateWithoutLicense(clsid);
        }
    }

    [MemberNotNull(nameof(_instance))]
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

        SetOcState(OC_LOADED);
    }

    /// <summary>
    ///  Called to create the ActiveX control.  Override this member to perform your own creation logic
    ///  or call base to do the default creation logic.
    /// </summary>
    protected virtual object? CreateInstanceCore(Guid clsid)
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

    private unsafe CategoryAttribute? GetCategoryForDispid(int dispid)
    {
        using var categorizeProperties = ComHelpers.TryGetComScope<ICategorizeProperties>(_instance, out HRESULT hr);
        if (hr.Failed)
        {
            return null;
        }

        PROPCAT propcat = 0;
        hr = categorizeProperties.Value->MapPropertyToCategory(dispid, &propcat);
        if (!hr.Succeeded || propcat == 0)
        {
            return null;
        }

        int index = -(int)propcat;
        if (index > 0 && index < s_categoryNames.Length && s_categoryNames[index] is not null)
        {
            return s_categoryNames[index];
        }

        if (_objectDefinedCategoryNames?.TryGetValue(propcat, out CategoryAttribute? category) ?? false
            && category is not null)
        {
            return category;
        }

        using BSTR name = default;
        hr = categorizeProperties.Value->GetCategoryName(propcat, (int)PInvokeCore.GetThreadLocale(), &name);
        if (hr.Succeeded && !name.IsNull)
        {
            category = new CategoryAttribute(name.ToString());
            _objectDefinedCategoryNames ??= [];
            _objectDefinedCategoryNames[propcat] = category;
            return category;
        }

        return null;
    }

    private void SetSelectionStyle(int selectionStyle)
    {
        if (IsUserMode())
        {
            return;
        }

        // SelectionStyle can be 0 (not selected), 1 (selected) or 2 (active)
        Debug.Assert(selectionStyle is >= 0 and <= 2, "Invalid selection style");
        _selectionStyle = selectionStyle;

        if (Site.TryGetService(out ISelectionService? selectionService) && selectionService.GetComponentSelected(this))
        {
            // The AX Host designer will offer an extender property called "SelectionStyle"
            if (TypeDescriptor.GetProperties(this)["SelectionStyle"] is { } property && property.PropertyType == typeof(int))
            {
                property.SetValue(this, selectionStyle);
            }
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public void InvokeEditMode()
    {
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
    string? ICustomTypeDescriptor.GetClassName()
    {
        return null;
    }

    /// <summary>
    ///  Retrieves the name for this object.  If null is returned,
    ///  the default is used.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    string? ICustomTypeDescriptor.GetComponentName()
    {
        return null;
    }

    /// <summary>
    ///  Retrieves the type converter for this object.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [RequiresUnreferencedCode(TrimmingConstants.AttributesRequiresUnreferencedCodeMessage)]
    TypeConverter? ICustomTypeDescriptor.GetConverter()
    {
        return null;
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [RequiresUnreferencedCode(TrimmingConstants.EventDescriptorRequiresUnreferencedCodeMessage)]
    EventDescriptor? ICustomTypeDescriptor.GetDefaultEvent()
    {
        return TypeDescriptor.GetDefaultEvent(this, true);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [RequiresUnreferencedCode(TrimmingConstants.PropertyDescriptorPropertyTypeMessage)]
    PropertyDescriptor? ICustomTypeDescriptor.GetDefaultProperty()
    {
        return TypeDescriptor.GetDefaultProperty(this, true);
    }

    /// <summary>
    ///  Retrieves the an editor for this object.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [RequiresUnreferencedCode(TrimmingConstants.EditorRequiresUnreferencedCode)]
    object? ICustomTypeDescriptor.GetEditor(Type editorBaseType)
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
    EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[]? attributes)
        => TypeDescriptor.GetEvents(this, attributes, noCustomTypeDesc: true);

    private void OnIdle(object? sender, EventArgs e)
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

    private PropertyDescriptorCollection FillProperties(Attribute[]? attributes)
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
                    return _propsStash;
                }
            }
        }

        List<PropertyDescriptor> returnProperties = [];
        _properties ??= [];

        if (_propertyInfos is null)
        {
            _propertyInfos = [];

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
                PropertyDescriptor? prop = null;

                _propertyInfos.TryGetValue(propName, out PropertyInfo? propInfo);

                // We do not support "write-only" properties that some activex controls support.
                if (propInfo is not null && !propInfo.CanRead)
                {
                    continue;
                }

                if (!_properties.TryGetValue(propName, out PropertyDescriptor? propDesc))
                {
                    if (propInfo is not null)
                    {
                        prop = new AxPropertyDescriptor(baseProps[i], this);
                        ((AxPropertyDescriptor)prop).UpdateAttributes();
                    }
                    else
                    {
                        prop = baseProps[i];
                    }

                    _properties.Add(propName, prop);
                    returnProperties.Add(prop);
                }
                else
                {
                    Debug.Assert(propDesc is not null, $"Cannot find cached entry for: {propName}");
                    AxPropertyDescriptor? axPropDesc = propDesc as AxPropertyDescriptor;
                    if ((propInfo is null && axPropDesc is not null) || (propInfo is not null && axPropDesc is null))
                    {
                        Debug.Fail($"Duplicate property with same name: {propName}");
                    }
                    else
                    {
                        axPropDesc?.UpdateAttributes();

                        returnProperties.Add(propDesc);
                    }
                }
            }

            // Filter only the Browsable attribute, since that is the only one we mess with.
            if (attributes is not null)
            {
                Attribute? browse = null;
                foreach (Attribute attribute in attributes)
                {
                    if (attribute is BrowsableAttribute)
                    {
                        browse = attribute;
                    }
                }

                if (browse is not null)
                {
                    List<PropertyDescriptor>? removeList = null;

                    foreach (PropertyDescriptor prop in returnProperties)
                    {
                        if (prop is AxPropertyDescriptor
                            && prop.TryGetAttribute(out BrowsableAttribute? browsableAttribute)
                            && !browsableAttribute.Equals(browse))
                        {
                            removeList ??= [];
                            removeList.Add(prop);
                        }
                    }

                    if (removeList is not null)
                    {
                        foreach (PropertyDescriptor prop in removeList)
                        {
                            returnProperties.Remove(prop);
                        }
                    }
                }
            }
        }

        // Update our stashed values.
        _propsStash = new PropertyDescriptorCollection([.. returnProperties]);
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
    [RequiresUnreferencedCode($"{TrimmingConstants.PropertyDescriptorPropertyTypeMessage} {TrimmingConstants.FilterRequiresUnreferencedCodeMessage}")]
    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[]? attributes)
    {
        return FillProperties(attributes);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    object? ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor? pd)
    {
        return this;
    }

    private AxPropertyDescriptor? GetPropertyDescriptorFromDispid(int dispid)
    {
        Debug.Assert(dispid != PInvokeCore.DISPID_UNKNOWN, "Wrong dispid sent to GetPropertyDescriptorFromDispid");

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

    private void DepersistControl()
    {
        FreezeEvents(true);

        if (_ocxState is null)
        {
            // Must init new:
            using var persistStreamInit = ComHelpers.TryGetComScope<IPersistStreamInit>(_instance, out HRESULT hr);
            if (hr.Succeeded)
            {
                _storageType = StorageType.StreamInit;
                persistStreamInit.Value->InitNew().AssertSuccess();
                return;
            }

            if (ComHelpers.SupportsInterface<IPersistStream>(_instance))
            {
                _storageType = StorageType.Stream;
                return;
            }

            using var persistStoragePtr = ComHelpers.TryGetComScope<IPersistStorage>(_instance, out hr);
            if (hr.Succeeded)
            {
                _storageType = StorageType.Storage;
                _ocxState = new State(this);
                using var storage = _ocxState.GetStorage();
                persistStoragePtr.Value->InitNew(storage).AssertSuccess();
                return;
            }

            using var persistPropBag = ComHelpers.TryGetComScope<IPersistPropertyBag>(_instance, out hr);
            if (hr.Succeeded)
            {
                _storageType = StorageType.PropertyBag;
                persistPropBag.Value->InitNew().AssertSuccess();
                return;
            }

            Debug.Fail("no implemented persistence interfaces on object");
            throw new InvalidOperationException(SR.UnableToInitComponent);
        }

        // Otherwise, we have state to depersist from:
        switch (_ocxState.Type)
        {
            case StorageType.Stream:
                using (var stream = _ocxState.GetStream())
                {
                    _storageType = StorageType.Stream;
                    using var persistStream = ComHelpers.GetComScope<IPersistStream>(_instance);
                    persistStream.Value->Load(stream).AssertSuccess();
                }

                break;
            case StorageType.StreamInit:
                using (var persistStreamInit = ComHelpers.TryGetComScope<IPersistStreamInit>(_instance, out HRESULT hr))
                {
                    if (hr.Succeeded)
                    {
                        using (var stream = _ocxState.GetStream())
                        {
                            _storageType = StorageType.StreamInit;
                            persistStreamInit.Value->Load(stream).AssertSuccess();
                        }

                        GetControlEnabled();
                    }
                    else
                    {
                        _ocxState.Type = StorageType.Stream;
                        DepersistControl();
                        return;
                    }
                }

                break;
            case StorageType.Storage:
                using (var storage = _ocxState.GetStorage())
                {
                    _storageType = StorageType.Storage;

                    // MapPoint control does not create a valid IStorage until some property has changed.
                    // Since we end up creating an empty storage, we are not able to re-create a valid one and this would fail.
                    if (!storage.IsNull)
                    {
                        using var persistStorage = ComHelpers.GetComScope<IPersistStorage>(_instance);
                        persistStorage.Value->Load(storage).AssertSuccess();
                    }
                }

                break;
            case StorageType.PropertyBag:
                using (var propBag = _ocxState.GetPropBag())
                {
                    _storageType = StorageType.PropertyBag;

                    if (!propBag.IsNull)
                    {
                        using var persistPropBag = ComHelpers.GetComScope<IPersistPropertyBag>(_instance);
                        persistPropBag.Value->Load(propBag, pErrorLog: null).AssertSuccess();
                    }
                }

                break;
            default:
                Debug.Fail("unknown storage type.");
                throw new InvalidOperationException(SR.UnableToInitComponent);
        }
    }

    /// <summary>
    ///  Returns the IUnknown pointer to the enclosed ActiveX control.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public object? GetOcx()
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
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DetachSink()
    {
    }

    private bool CanShowPropertyPages()
    {
        if (GetOcState() < OC_RUNNING)
        {
            return false;
        }

        using var pages = ComHelpers.TryGetComScope<ISpecifyPropertyPages>(_instance, out HRESULT hr);
        return hr.Succeeded;
    }

    public unsafe bool HasPropertyPages()
    {
        if (GetOcState() < OC_RUNNING)
        {
            return false;
        }

        using var pages = ComHelpers.TryGetComScope<ISpecifyPropertyPages>(_instance, out HRESULT hr);

        if (hr.Failed)
        {
            return false;
        }

        CAUUID uuids = default;
        try
        {
            return pages.Value->GetPages(&uuids).Succeeded && uuids.cElems > 0;
        }
        finally
        {
            if (uuids.pElems is not null)
            {
                Marshal.FreeCoTaskMem((IntPtr)uuids.pElems);
            }
        }
    }

    private unsafe void ShowPropertyPageForDispid(int dispid, Guid guid)
    {
        using ComScope<IUnknown> unknown = ComHelpers.TryGetComScope<IUnknown>(_instance, out HRESULT hr);
        if (hr.Failed)
        {
            Debug.Fail($"Failed to get instance in {nameof(ShowPropertyPageForDispid)}.");
            return;
        }

        fixed (char* pName = Name)
        {
            OCPFIPARAMS parameters = new()
            {
                cbStructSize = (uint)sizeof(OCPFIPARAMS),
                hWndOwner = (ContainingControl is null) ? HWND.Null : ContainingControl.HWND,
                lpszCaption = pName,
                cObjects = 1,
                lplpUnk = unknown,
                cPages = 1,
                lpPages = &guid,
                lcid = PInvokeCore.GetThreadLocale(),
                dispidInitialProperty = dispid
            };

            PInvoke.OleCreatePropertyFrameIndirect(&parameters).AssertSuccess();
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public void MakeDirty()
    {
        if (Site.TryGetService(out IComponentChangeService? changeService))
        {
            changeService.OnComponentChanging(this);
            changeService.OnComponentChanged(this);
        }
    }

    public void ShowPropertyPages()
    {
        if (ParentInternal is null || !ParentInternal.IsHandleCreated)
        {
            return;
        }

        ShowPropertyPages(ParentInternal);
    }

    public unsafe void ShowPropertyPages(Control? control)
    {
        if (!CanShowPropertyPages())
        {
            return;
        }

        using var pages = ComHelpers.TryGetComScope<ISpecifyPropertyPages>(_instance, out HRESULT hr);
        CAUUID uuids = default;
        if (hr.Failed || pages.Value->GetPages(&uuids).Failed || uuids.cElems == 0)
        {
            return;
        }

        Site.TryGetService(out IDesignerHost? host);

        DesignerTransaction? transaction = null;
        try
        {
            transaction = host?.CreateTransaction(SR.AXEditProperties);

            HWND handle = ContainingControl is null ? HWND.Null : ContainingControl.HWND;
            IUnknown* unknown = ComHelpers.GetComPointer<IUnknown>(_instance);
            PInvoke.OleCreatePropertyFrame(
                handle,
                0,
                0,
                (PCWSTR)null,
                1,
                &unknown,
                uuids.cElems,
                uuids.pElems,
                PInvokeCore.GetThreadLocale(),
                0,
                (void*)null).AssertSuccess();
        }
        finally
        {
            if (_oleSite is IPropertyNotifySink.Interface sink)
            {
                sink.OnChanged(PInvokeCore.DISPID_UNKNOWN);
            }

            transaction?.Commit();

            if (uuids.pElems is not null)
            {
                Marshal.FreeCoTaskMem((IntPtr)uuids.pElems);
            }
        }
    }

    internal override HBRUSH InitializeDCForWmCtlColor(HDC dc, MessageId msg)
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

    protected override unsafe void WndProc(ref Message m)
    {
        // Route input related messages directly to the ActiveX control where appropriate and do other special logic.

        switch (m.MsgInternal)
        {
            // Things we explicitly ignore and pass to the ActiveX Control's windproc
            case PInvoke.WM_ERASEBKGND:
            case MessageId.WM_REFLECT_NOTIFYFORMAT:
            case PInvoke.WM_SETCURSOR:
            case PInvoke.WM_SYSCOLORCHANGE:

            // Some of the common controls respond to this message to do some custom painting.
            // So, we should just pass this message through.
            case PInvoke.WM_DRAWITEM:

            case PInvoke.WM_LBUTTONDBLCLK:
            case PInvoke.WM_LBUTTONUP:
            case PInvoke.WM_MBUTTONDBLCLK:
            case PInvoke.WM_MBUTTONUP:
            case PInvoke.WM_RBUTTONDBLCLK:
            case PInvoke.WM_RBUTTONUP:
                DefWndProc(ref m);
                break;

            case PInvoke.WM_LBUTTONDOWN:
            case PInvoke.WM_MBUTTONDOWN:
            case PInvoke.WM_RBUTTONDOWN:
                if (IsUserMode())
                {
                    Focus();
                }

                DefWndProc(ref m);
                break;

            case PInvoke.WM_KILLFOCUS:
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

            case PInvoke.WM_COMMAND:
                if (!ReflectMessage(m.LParamInternal, ref m))
                {
                    DefWndProc(ref m);
                }

                break;

            case PInvoke.WM_CONTEXTMENU:
                DefWndProc(ref m);
                break;

            case PInvoke.WM_DESTROY:
                // If we are currently in a state of InPlaceActive or above, we should first reparent the ActiveX
                // control to our parking window before we transition to a state below InPlaceActive. Otherwise we
                // face all sorts of problems when we try to transition back to a state >= InPlaceActive.
                if (GetOcState() >= OC_INPLACE)
                {
                    using var inPlaceObject = GetComScope<IOleInPlaceObject>();
                    HWND hwnd = HWND.Null;
                    if (inPlaceObject.Value->GetWindow(&hwnd).Succeeded)
                    {
                        Application.ParkHandle(new HandleRef<HWND>(this, hwnd), DpiAwarenessContext);
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
            case PInvoke.WM_HELP:
                // We want to both fire the event, and let the ActiveX Control have the message.
                base.WndProc(ref m);
                DefWndProc(ref m);
                break;

            case PInvoke.WM_KEYUP:
                // Pass WM_KEYUP messages to PreProcessControlMessage, which comes back to our PreProcessMessage
                // to give the ActiveX control a chance to handle accelerator keys (command shortcuts).

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

            case PInvoke.WM_NCDESTROY:
                // Need to detach it now.
                DetachAndForward(ref m);
                break;

            default:
                if (m.MsgInternal == _subclassCheckMessage)
                {
                    m.ResultInternal = (LRESULT)REGMSG_RETVAL;
                    return;
                }

                // Pass all other messages to Control's WndProc.
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
        if (!_axState[s_fFakingWindow])
        {
            WindowAssignHandle(hwnd, _axState[s_assignUniqueID]);
        }

        UpdateZOrder();

        // Get the latest bounds set by the user.
        Size setExtent = Size;

        // Get the default bounds set by the ActiveX control.
        UpdateBounds();
        Size ocxExtent = GetExtent();

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
        if (Application.OleRequired() != ApartmentState.STA)
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

    private static int HM2Pix(int hm, int logP) => (logP * hm + HMperInch / 2) / HMperInch;

    private unsafe bool QuickActivate()
    {
        if (_instance is not IQuickActivate.Interface iqa)
        {
            return false;
        }

        QACONTAINER qaContainer = new()
        {
            cbSize = (uint)sizeof(QACONTAINER)
        };

        QACONTROL qaControl = new()
        {
            cbSize = (uint)sizeof(QACONTROL)
        };

        qaContainer.pClientSite = ComHelpers.GetComPointer<IOleClientSite>(_oleSite);
        qaContainer.pPropertyNotifySink = ComHelpers.GetComPointer<IPropertyNotifySink>(_oleSite);
        qaContainer.pFont = GetIFontPointerFromFont(GetParentContainer()._parent.Font);
        qaContainer.dwAppearance = 0;
        qaContainer.lcid = (int)PInvokeCore.GetThreadLocale();

        Control? parent = ParentInternal;

        if (parent is not null)
        {
            qaContainer.colorFore = GetOleColorFromColor(parent.ForeColor);
            qaContainer.colorBack = GetOleColorFromColor(parent.BackColor);
        }
        else
        {
            qaContainer.colorFore = GetOleColorFromColor(Application.ApplicationColors.WindowText);
            qaContainer.colorBack = GetOleColorFromColor(Application.ApplicationColors.Window);
        }

        qaContainer.dwAmbientFlags = QACONTAINERFLAGS.QACONTAINER_AUTOCLIP
            | QACONTAINERFLAGS.QACONTAINER_MESSAGEREFLECT
            | QACONTAINERFLAGS.QACONTAINER_SUPPORTSMNEMONICS;

        if (IsUserMode())
        {
            // In design mode we'd ideally set QACONTAINER_UIDEAD on dwAmbientFlags
            // so controls don't take keyboard input, but MFC controls return NOWHERE on
            // NCHITTEST, which messes up the designer.
            qaContainer.dwAmbientFlags |= QACONTAINERFLAGS.QACONTAINER_USERMODE;
        }

        HRESULT hr = iqa.QuickActivate(&qaContainer, &qaControl);
        if (!hr.Succeeded)
        {
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
        => GetControlEnabled() && !_axState[s_rejectSelection] && base.CanSelectCore();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            try
            {
                TransitionDownTo(OC_PASSIVE);
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message);
            }

            _newParent?.Dispose();

            _oleSite?.Dispose();
            _ocxState?.Dispose();
            _container?.Dispose();
            _axContainer?.Dispose();
        }

        base.Dispose(disposing);
    }

    private bool GetSiteOwnsDeactivation() => _axState[s_ownDisposing];

    private void DisposeAxControl()
    {
        GetParentContainer()?.RemoveControl(this);

        TransitionDownTo(OC_RUNNING);
        if (GetOcState() == OC_RUNNING)
        {
            using var oleObject = GetComScope<IOleObject>();
            oleObject.Value->SetClientSite(null).AssertSuccess();
            SetOcState(OC_LOADED);
        }
    }

    private void ReleaseAxControl()
    {
        NoComponentChangeEvents++;

        if (ContainingControl is { } container)
        {
            container.VisibleChanged -= _onContainerVisibleChanged;
        }

        try
        {
            if (_instance is not null)
            {
                Marshal.ReleaseComObject(_instance);
                _instance = null;
                DisposeHelper.NullAndDispose(ref _iOleInPlaceActiveObjectExternal);
            }

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

    private void ParseMiscBits(OLEMISC bits)
    {
        // Does this control only have a design-time UI?
        _axState[s_fOwnWindow] = bits.HasFlag(OLEMISC.OLEMISC_INVISIBLEATRUNTIME) && IsUserMode();

        // Requires ISimpleFrameSite?
        _axState[s_fSimpleFrame] = bits.HasFlag(OLEMISC.OLEMISC_SIMPLEFRAME);
    }

    private void SlowActivate()
    {
        bool setClientSite = false;
        using var oleObject = GetComScope<IOleObject>();

        if (_miscStatusBits.HasFlag(OLEMISC.OLEMISC_SETCLIENTSITEFIRST))
        {
            using var clientSite = ComHelpers.GetComScope<IOleClientSite>(_oleSite);
            oleObject.Value->SetClientSite(clientSite).AssertSuccess();
            setClientSite = true;
        }

        DepersistControl();

        if (!setClientSite)
        {
            using var clientSite = ComHelpers.GetComScope<IOleClientSite>(_oleSite);
            oleObject.Value->SetClientSite(clientSite).AssertSuccess();
        }
    }

    private AxContainer GetParentContainer()
    {
        _container ??= AxContainer.FindContainerForControl(this);

        if (_container is not null)
        {
            return _container;
        }

        if (ContainingControl is not { } container)
        {
            // ContainingControl can be null if the AxHost is still not parented to a ContainerControl.
            // Use a temporary container until one gets created.
            if (_newParent is null || _axContainer is null)
            {
                _newParent = new ContainerControl();
                _axContainer = _newParent.CreateAxContainer();
                _axContainer.AddControl(this);
            }

            return _axContainer;
        }
        else
        {
            _container = container.CreateAxContainer();
            _container.AddControl(this);
            _containingControl = container;
        }

        return _container;
    }

    private ComScope<T> GetComScope<T>() where T : unmanaged, IComIID
        => ComHelpers.GetComScope<T>(_instance);

    private ComScope<T> TryGetComScope<T>(out HRESULT hr) where T : unmanaged, IComIID
        => ComHelpers.TryGetComScope<T>(_instance, out hr);

    // Mapping functions:

    /// <summary>
    ///  Maps from a System.Drawing.Image to an OLE IPicture
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected static object? GetIPictureFromPicture(Image? image)
        => image is null ? null : IPicture.CreateObjectFromImage(image);

    /// <summary>
    ///  Maps from a System.Drawing.Cursor to an OLE IPicture
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected static object? GetIPictureFromCursor(Cursor? cursor)
        => cursor is null ? null : IPicture.CreateObjectFromIcon(Icon.FromHandle(cursor.Handle), copy: true);

    /// <summary>
    ///  Maps from a System.Drawing.Image to an OLE IPictureDisp
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected static object? GetIPictureDispFromPicture(Image? image)
        => image is null ? null : IPictureDisp.CreateObjectFromImage(image);

    /// <summary>
    ///  Maps from an OLE IPicture to a System.Drawing.Image
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected static Image? GetPictureFromIPicture(object? picture)
    {
        if (picture is null)
        {
            return null;
        }

        using var iPicture = ComHelpers.TryGetComScope<IPictureDisp>(picture, out HRESULT hr);
        hr.ThrowOnFailure();

        try
        {
            return ImageExtensions.ToImage(iPicture);
        }
        catch (InvalidOperationException)
        {
            throw new ArgumentException(SR.AXUnknownImage, nameof(picture));
        }
    }

    /// <summary>
    ///  Maps from an OLE IPictureDisp to a System.Drawing.Image
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected static unsafe Image? GetPictureFromIPictureDisp(object? picture)
    {
        if (picture is null)
        {
            return null;
        }

        using var pictureDisp = ComHelpers.TryGetComScope<IPictureDisp>(picture, out HRESULT hr);
        hr.ThrowOnFailure();

        try
        {
            return ImageExtensions.ToImage(pictureDisp);
        }
        catch (InvalidOperationException)
        {
            throw new ArgumentException(SR.AXUnknownImage, nameof(picture));
        }
    }

    /// <summary>
    ///  Gets a cached <see cref="FONTDESC"/> for a given <see cref="Font"/>. The returned
    ///  <see cref="FONTDESC"/> must have it's <see cref="FONTDESC.lpstrName"/> populated with
    ///  a newly pinned string before usage.
    /// </summary>
    private static FONTDESC GetFONTDESCFromFont(Font font)
    {
        if (s_fontTable is null)
        {
            s_fontTable = [];
        }
        else if (s_fontTable.TryGetValue(font, out object? cachedFDesc))
        {
            return (FONTDESC)cachedFDesc;
        }

        LOGFONTW logfont = font.ToLogicalFont();
        FONTDESC fdesc = new()
        {
            cbSizeofstruct = (uint)sizeof(FONTDESC),
            cySize = (CY)font.SizeInPoints,
            sWeight = (short)logfont.lfWeight,
            sCharset = (short)logfont.lfCharSet,
            fItalic = font.Italic,
            fUnderline = font.Underline,
            fStrikethrough = font.Strikeout
        };

        s_fontTable.AddOrUpdate(font, fdesc);
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
    protected static object? GetIFontFromFont(Font? font)
    {
        IFont* ifont = GetIFontPointerFromFont(font);
        if (ifont is null)
        {
            return null;
        }

        try
        {
            return ComHelpers.GetObjectForIUnknown(ifont);
        }
        catch
        {
        }

        return null;
    }

    private protected static IFont* GetIFontPointerFromFont(Font? font)
    {
        if (font is null)
        {
            return null;
        }

        if (font.Unit != GraphicsUnit.Point)
        {
            throw new ArgumentException(SR.AXFontUnitNotPoint, nameof(font));
        }

        FONTDESC fontDesc = GetFONTDESCFromFont(font);
        fixed (char* n = font.Name)
        {
            fontDesc.lpstrName = n;
            HRESULT hr = PInvoke.OleCreateFontIndirect(in fontDesc, in IID.GetRef<IFont>(), out void* lplpvObj);
            if (hr.Succeeded)
            {
                return (IFont*)lplpvObj;
            }
        }

        return null;
    }

    /// <summary>
    ///  Maps from an OLE IFont to a System.Drawing.Font object
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected static Font? GetFontFromIFont(object? font)
    {
        if (font is null)
        {
            return null;
        }

        IFont.Interface oleFont = (IFont.Interface)font;
        try
        {
            Font f = Font.FromHfont(oleFont.hFont);
            return f.Unit == GraphicsUnit.Point
                ? f
                : new(f.Name, f.SizeInPoints, f.Style, GraphicsUnit.Point, f.GdiCharSet, f.GdiVerticalFont);
        }
        catch (Exception)
        {
            return DefaultFont;
        }
    }

    /// <summary>
    ///  Maps from a System.Drawing.Font object to an OLE IFontDisp
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected static object? GetIFontDispFromFont(Font? font)
    {
        if (font is null)
        {
            return null;
        }

        if (font.Unit != GraphicsUnit.Point)
        {
            throw new ArgumentException(SR.AXFontUnitNotPoint, nameof(font));
        }

        fixed (char* n = font.Name)
        {
            FONTDESC fontdesc = GetFONTDESCFromFont(font);
            fontdesc.lpstrName = n;
            PInvoke.OleCreateFontIndirect(in fontdesc, in IID.GetRef<IFontDisp>(), out void* lplpvObj).ThrowOnFailure();
            return ComHelpers.GetObjectForIUnknown((IFontDisp*)lplpvObj);
        }
    }

    /// <summary>
    ///  Maps from an IFontDisp to a System.Drawing.Font object
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected static Font? GetFontFromIFontDisp(object? font)
    {
        if (font is null)
        {
            return null;
        }

        if (font is IFont.Interface ifont)
        {
            return GetFontFromIFont(ifont);
        }

        IFontDisp.Interface oleFont = (IFontDisp.Interface)font;
        using ComScope<IDispatch> dispatch = new((IDispatch*)Marshal.GetIDispatchForObject(oleFont));

        FontStyle style = FontStyle.Regular;

        try
        {
            if ((bool)dispatch.Value->GetProperty(PInvokeCore.DISPID_FONT_BOLD))
            {
                style |= FontStyle.Bold;
            }

            if ((bool)dispatch.Value->GetProperty(PInvokeCore.DISPID_FONT_ITALIC))
            {
                style |= FontStyle.Italic;
            }

            if ((bool)dispatch.Value->GetProperty(PInvokeCore.DISPID_FONT_UNDER))
            {
                style |= FontStyle.Underline;
            }

            if ((bool)dispatch.Value->GetProperty(PInvokeCore.DISPID_FONT_STRIKE))
            {
                style |= FontStyle.Strikeout;
            }

            if ((short)dispatch.Value->GetProperty(PInvokeCore.DISPID_FONT_WEIGHT) >= 700)
            {
                style |= FontStyle.Bold;
            }

            using BSTR name = (BSTR)dispatch.Value->GetProperty(PInvokeCore.DISPID_FONT_NAME);

            return new Font(
                name.ToString(),
                (float)(CY)dispatch.Value->GetProperty(PInvokeCore.DISPID_FONT_SIZE),
                style,
                GraphicsUnit.Point,
                (byte)(short)dispatch.Value->GetProperty(PInvokeCore.DISPID_FONT_CHARSET));
        }
        catch (Exception)
        {
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
        o = ((Array)o).GetValue(0)!;

        // User controls & other visual basic related controls give us coordinates as floats in twips
        // but MFC controls give us integers as pixels.
        return o.GetType() == typeof(float)
            ? Twip2Pixel(Convert.ToDouble(o, CultureInfo.InvariantCulture), xDirection)
            : Convert.ToInt32(o, CultureInfo.InvariantCulture);
    }

    private static short Convert2short(object o)
    {
        o = ((Array)o).GetValue(0)!;
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
