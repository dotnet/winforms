// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms;

/// <summary>
///  Wraps ActiveX controls and exposes them as fully featured windows forms controls
///  (by inheriting from Control). Some of Control's properties that don't make sense
///  for ActiveX controls are blocked here (by setting Browsable attributes on some and
///  throwing exceptions from others), to make life easy for the inheritors.
///
///  Inheritors of this class simply need to concentrate on defining and implementing the
///  properties/methods/events of the specific ActiveX control they are wrapping, the
///  default properties etc and the code to implement the activation etc. are
///  encapsulated in the class below.
///
///  The classid of the ActiveX control is specified in the constructor.
/// </summary>
[DefaultProperty(nameof(Name))]
[DefaultEvent(nameof(Enter))]
[Designer($"System.Windows.Forms.Design.AxDesigner, {AssemblyRef.SystemDesign}")]
public unsafe partial class WebBrowserBase : Control
{
    private WebBrowserHelper.AXState _axState = WebBrowserHelper.AXState.Passive;
    private WebBrowserHelper.AXState _axReloadingState = WebBrowserHelper.AXState.Passive;
    private WebBrowserHelper.AXEditMode _axEditMode = WebBrowserHelper.AXEditMode.None;
    private BitVector32 _axHostState;
    private WebBrowserHelper.SelectionStyle _selectionStyle = WebBrowserHelper.SelectionStyle.NotSelected;
    private WebBrowserSiteBase? _axSite;
    private ContainerControl? _containingControl;
    private EventHandler? _selectionChangeHandler;
    private readonly Guid _clsid;
    // Pointers to the ActiveX object: Interface pointers are cached for perf.
    private IOleObject.Interface? _axOleObject;
    private IOleInPlaceObject.Interface? _axOleInPlaceObject;
    private IOleInPlaceActiveObject.Interface? _axOleInPlaceActiveObject;
    private IOleControl.Interface? _axOleControl;
    private WebBrowserBaseNativeWindow? _axWindow;
    // We need to change the size of the inner ActiveX control before the
    // WebBrowserBase control's size is changed (i.e., before WebBrowserBase.Bounds
    // is changed) for better visual effect. We use this field to know what size
    // the WebBrowserBase control is changing to.
    private Size _webBrowserBaseChangingSize = Size.Empty;
    private WebBrowserContainer? _wbContainer;

    // This flags the WebBrowser not to process dialog keys when the ActiveX control is doing it
    // and calls back into the WebBrowser for some reason.
    private bool _ignoreDialogKeys;

    //
    // Internal fields:
    //
    internal WebBrowserContainer? _container;
    internal object? _activeXInstance;

    /// <summary>
    ///  Creates a new instance of a WinForms control which wraps an ActiveX control
    ///  given by the clsid parameter.
    /// </summary>
    internal WebBrowserBase(string clsidString) : base()
    {
        if (Application.OleRequired() != ApartmentState.STA)
        {
            throw new ThreadStateException(string.Format(SR.AXMTAThread, clsidString));
        }

        SetStyle(ControlStyles.UserPaint, false);

        _clsid = new Guid(clsidString);
        _webBrowserBaseChangingSize.Width = -1;  // Invalid value. Use WebBrowserBase.Bounds instead, when this is the case.
        SetAXHostState(WebBrowserHelper.s_isMaskEdit, _clsid.Equals(WebBrowserHelper.s_maskEdit_Clsid));
    }

    //
    // Public properties:
    //

    /// <summary>
    ///  Returns the native webbrowser object that this control wraps.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public object? ActiveXInstance => _activeXInstance;

    //
    // Virtual methods:
    //
    // The following are virtual methods that derived-classes can override
    //

    //
    // The native ActiveX control QI's for interfaces on it's site to see if
    // it needs to change its behavior. Since the WebBrowserSiteBaseBase class is generic,
    // it only implements site interfaces that are generic to all sites. QI's
    // for any more specific interfaces will fail. This is a problem if anyone
    // wants to support any other interfaces on the site. In order to overcome
    // this, one needs to extend WebBrowserSiteBaseBase and implement any additional interfaces
    // needed.
    //
    // ActiveX wrapper controls that derive from this class should override the
    // below method and return their own WebBrowserSiteBaseBase derived object.
    //
    /// <summary>
    ///  Returns an object that will be set as the site for the native ActiveX control.
    ///  Implementors of the site can derive from <see cref="WebBrowserSiteBase"/> class.
    /// </summary>
    protected virtual WebBrowserSiteBase CreateWebBrowserSiteBase()
    {
        return new WebBrowserSiteBase(this);
    }

    /// <summary>
    ///  This will be called when the native ActiveX control has just been created.
    ///  Inheritors of this class can override this method to cast the nativeActiveXObject
    ///  parameter to the appropriate interface. They can then cache this interface
    ///  value in a member variable. However, they must release this value when
    ///  DetachInterfaces is called (by setting the cached interface variable to null).
    /// </summary>
    protected virtual void AttachInterfaces(object nativeActiveXObject)
    {
    }

    /// <summary>
    ///  See AttachInterfaces for a description of when to override DetachInterfaces.
    /// </summary>
    protected virtual void DetachInterfaces()
    {
    }

    /// <summary>
    ///  This will be called when we are ready to start listening to events.
    ///  Inheritors can override this method to hook their own connection points.
    /// </summary>
    protected virtual void CreateSink()
    {
    }

    /// <summary>
    ///  This will be called when it is time to stop listening to events.
    ///  This is where inheritors have to disconnect their connection points.
    /// </summary>
    protected virtual void DetachSink()
    {
    }

    // DrawToBitmap doesn't work for this control, so we should hide it. We'll
    // still call base so that this has a chance to work if it can.
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds)
    {
        base.DrawToBitmap(bitmap, targetBounds);
    }

    //
    // Overriding methods: Overrides of some of Control's virtual methods.
    //

    //
    // Sets the site of this component. A non-null value indicates that this
    // component has been added to a container, and a null value indicates that
    // this component is being removed from a container.
    //
    public override ISite? Site
    {
        set
        {
            bool hadSelectionHandler = RemoveSelectionHandler();

            base.Site = value;

            if (hadSelectionHandler)
            {
                AddSelectionHandler();
            }
        }
    }

    /// <remarks>
    ///  <para>We have to resize the ActiveX control when our size changes.</para>
    /// </remarks>
    internal override unsafe void OnBoundsUpdate(int x, int y, int width, int height)
    {
        // If the ActiveX control is already InPlaceActive, make sure
        // it's bounds also change.
        if (ActiveXState >= WebBrowserHelper.AXState.InPlaceActive)
        {
            try
            {
                _webBrowserBaseChangingSize.Width = width;
                _webBrowserBaseChangingSize.Height = height;
                RECT posRect = new Rectangle(0, 0, width, height);
                RECT clipRect = WebBrowserHelper.GetClipRect();
                AXInPlaceObject!.SetObjectRects(&posRect, &clipRect);
            }
            finally
            {
                _webBrowserBaseChangingSize.Width = -1;  // Invalid value. Use WebBrowserBase.Bounds instead, when this is the case.
            }
        }

        base.OnBoundsUpdate(x, y, width, height);
    }

    protected override bool ProcessDialogKey(Keys keyData) =>
        !_ignoreDialogKeys && base.ProcessDialogKey(keyData);

    public override unsafe bool PreProcessMessage(ref Message msg)
    {
        // Let us assume that TAB key was pressed. In this case, we should first
        // give a chance to the ActiveX control to see if it wants to change focus
        // to other subitems within it. If we did not give the ActiveX control the
        // first chance to handle the key stroke, and called base.PreProcessMessage,
        // the focus would be changed to the next control on the form! We don't want
        // that

        // If the ActiveX control doesn't want to handle the key, it calls back into
        // WebBrowserSiteBase's IOleControlSite.TranslateAccelerator implementation. There, we
        // set a flag and call back into this method. In this method, we first check
        // if this flag is set. If so, we call base.PreProcessMessage.
        if (!IsUserMode)
        {
            return false;
        }

        if (GetAXHostState(WebBrowserHelper.s_siteProcessedInputKey))
        {
            // In this case, the control called us back through IOleControlSite
            // and is now giving us a chance to see if we want to process it.
            return base.PreProcessMessage(ref msg);
        }

        // Convert Message to MSG
        MSG win32Message = msg.ToMSG();
        SetAXHostState(WebBrowserHelper.s_siteProcessedInputKey, false);
        try
        {
            if (_axOleInPlaceObject is null)
            {
                return false;
            }

            // Give the ActiveX control a chance to process this key by calling
            // IOleInPlaceActiveObject::TranslateAccelerator.
            HRESULT hr = _axOleInPlaceActiveObject!.TranslateAccelerator(&win32Message);
            if (hr == HRESULT.S_OK)
            {
                return true;
            }
            else
            {
                // win32Message may have been modified. Lets copy it back.
                msg.MsgInternal = (MessageId)win32Message.message;
                msg.WParamInternal = win32Message.wParam;
                msg.LParamInternal = win32Message.lParam;
                msg.HWnd = win32Message.hwnd;

                if (hr == HRESULT.S_FALSE)
                {
                    // Same code as in AxHost (ignore dialog keys here).
                    // We have the same problem here.
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
                else if (GetAXHostState(WebBrowserHelper.s_siteProcessedInputKey))
                {
                    return base.PreProcessMessage(ref msg);
                }
                else
                {
                    return false;
                }
            }
        }
        finally
        {
            SetAXHostState(WebBrowserHelper.s_siteProcessedInputKey, false);
        }
    }

    //
    // Process a mnemonic character. This is done by manufacturing a
    // WM_SYSKEYDOWN message and passing it to the ActiveX control.
    //
    // We can't decide just by ourselves whether we can process the
    // mnemonic. We have to ask the ActiveX control for it.
    //
    protected internal override unsafe bool ProcessMnemonic(char charCode)
    {
        if (!CanSelect)
        {
            return false;
        }

        bool processed = false;

        try
        {
            CONTROLINFO controlInfo = new()
            {
                cb = (uint)sizeof(CONTROLINFO)
            };

            if (_axOleControl!.GetControlInfo(&controlInfo).Failed)
            {
                return processed;
            }

            // We don't have a message so we must create one ourselves.
            // The message we are creating is a WM_SYSKEYDOWN with the right alt key setting.
            MSG msg = new()
            {
                hwnd = HWND.Null,
                message = PInvokeCore.WM_SYSKEYDOWN,
                wParam = (WPARAM)char.ToUpper(charCode, CultureInfo.CurrentCulture),
                lParam = 0x20180001,
                time = PInvoke.GetTickCount()
            };

            PInvoke.GetCursorPos(out Point p);
            msg.pt = p;
            if (!PInvoke.IsAccelerator(new HandleRef<HACCEL>(this, controlInfo.hAccel), controlInfo.cAccel, &msg, lpwCmd: null))
            {
                _axOleControl.OnMnemonic(&msg);
                Focus();
                processed = true;
            }
        }
        catch (Exception ex) when (!ex.IsCriticalException())
        {
            Debug.Fail($"error in processMnemonic: {ex}");
        }

        return processed;
    }

    /// <remarks>
    ///  <para>
    ///   Certain messages are forwardef directly to the ActiveX control, others are first processed by the wndproc of
    ///   <see cref="Control"/>.
    ///  </para>
    /// </remarks>
    protected override unsafe void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            // Things we explicitly ignore and pass to the ActiveX's windproc
            case PInvokeCore.WM_ERASEBKGND:
            case MessageId.WM_REFLECT_NOTIFYFORMAT:
            case PInvokeCore.WM_SETCURSOR:
            case PInvokeCore.WM_SYSCOLORCHANGE:
            case PInvokeCore.WM_LBUTTONDBLCLK:
            case PInvokeCore.WM_LBUTTONUP:
            case PInvokeCore.WM_MBUTTONDBLCLK:
            case PInvokeCore.WM_MBUTTONUP:
            case PInvokeCore.WM_RBUTTONDBLCLK:
            case PInvokeCore.WM_RBUTTONUP:
            case PInvokeCore.WM_CONTEXTMENU:
            //
            // Some of the MSComCtl controls respond to this message to do some
            // custom painting. So, we should just pass this message through.
            case PInvokeCore.WM_DRAWITEM:
                DefWndProc(ref m);
                break;

            case PInvokeCore.WM_COMMAND:
                if (!ReflectMessage(m.LParamInternal, ref m))
                {
                    DefWndProc(ref m);
                }

                break;

            case PInvokeCore.WM_HELP:
                // We want to both fire the event, and let the ActiveX have the message...
                base.WndProc(ref m);
                DefWndProc(ref m);
                break;

            case PInvokeCore.WM_LBUTTONDOWN:
            case PInvokeCore.WM_MBUTTONDOWN:
            case PInvokeCore.WM_RBUTTONDOWN:
            case PInvokeCore.WM_MOUSEACTIVATE:
                if (!DesignMode)
                {
                    if (_containingControl is not null && _containingControl.ActiveControl != this)
                    {
                        Focus();
                    }
                }

                DefWndProc(ref m);
                break;

            case PInvokeCore.WM_DESTROY:
                //
                // If we are currently in a state of InPlaceActive or above,
                // we should first reparent the ActiveX control to our parking
                // window before we transition to a state below InPlaceActive.
                // Otherwise we face all sorts of problems when we try to
                // transition back to a state >= InPlaceActive.
                //
                if (ActiveXState >= WebBrowserHelper.AXState.InPlaceActive)
                {
                    HWND hwndInPlaceObject = HWND.Null;
                    if (AXInPlaceObject!.GetWindow(&hwndInPlaceObject).Succeeded)
                    {
                        Application.ParkHandle(new HandleRef<HWND>(AXInPlaceObject, hwndInPlaceObject), DpiAwarenessContext);
                    }
                }

                if (RecreatingHandle)
                {
                    _axReloadingState = _axState;
                }

                //
                // If the ActiveX control was holding on to our handle we need
                // to get it to throw it away. This, we do by transitioning it
                // down below InPlaceActivate (because it is when transitioning
                // up to InPlaceActivate that the ActiveX control grabs our handle).
                TransitionDownTo(WebBrowserHelper.AXState.Running);

                _axWindow?.ReleaseHandle();

                OnHandleDestroyed(EventArgs.Empty);
                break;

            default:
                if (m.MsgInternal == WebBrowserHelper.REGMSG_MSG)
                {
                    m.ResultInternal = (LRESULT)WebBrowserHelper.REGMSG_RETVAL;
                }
                else
                {
                    base.WndProc(ref m);
                }

                break;
        }
    }

    protected override void OnParentChanged(EventArgs e)
    {
        Control? parent = ParentInternal;
        if ((Visible && parent is not null && parent.Visible) || IsHandleCreated)
        {
            TransitionUpTo(WebBrowserHelper.AXState.InPlaceActive);
        }

        base.OnParentChanged(e);
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
        if (Visible && !Disposing && !IsDisposed)
        {
            TransitionUpTo(WebBrowserHelper.AXState.InPlaceActive);
        }

        base.OnVisibleChanged(e);
    }

    protected override void OnGotFocus(EventArgs e)
    {
        if (ActiveXState < WebBrowserHelper.AXState.UIActive)
        {
            TransitionUpTo(WebBrowserHelper.AXState.UIActive);
        }

        base.OnGotFocus(e);
    }

    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);

        // If the focus goes from our control window to one of the child windows,
        // we should not deactivate.
        if (!ContainsFocus)
        {
            TransitionDownTo(WebBrowserHelper.AXState.InPlaceActive);
        }
    }

    protected override void OnRightToLeftChanged(EventArgs e)
    {
        // Do nothing: no point in recreating the handle when we don't obey RTL
    }

    //
    // Can't select the control until the ActiveX control is InPlaceActive.
    //
    internal override bool CanSelectCore()
    {
        return ActiveXState >= WebBrowserHelper.AXState.InPlaceActive && base.CanSelectCore();
    }

    internal override bool AllowsKeyboardToolTip()
    {
        return false;
    }

    //
    // We have to inform the ActiveX control that an ambient property
    // has changed.
    //
    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        AmbientChanged(PInvokeCore.DISPID_AMBIENT_FONT);
    }

    //
    // We have to inform the ActiveX control that an ambient property
    // has changed.
    //
    protected override void OnForeColorChanged(EventArgs e)
    {
        base.OnForeColorChanged(e);
        AmbientChanged(PInvokeCore.DISPID_AMBIENT_FORECOLOR);
    }

    //
    // We have to inform the ActiveX control that an ambient property
    // has changed.
    //
    protected override void OnBackColorChanged(EventArgs e)
    {
        base.OnBackColorChanged(e);
        AmbientChanged(PInvokeCore.DISPID_AMBIENT_BACKCOLOR);
    }

    //
    // TransitionDownTo Passive when we are being disposed.
    //
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            TransitionDownTo(WebBrowserHelper.AXState.Passive);
        }

        base.Dispose(disposing);
    }

    //
    // Internal helper methods:
    //

    internal WebBrowserHelper.AXState ActiveXState
    {
        get => _axState;
        set => _axState = value;
    }

    internal bool GetAXHostState(int mask) => _axHostState[mask];

    internal void SetAXHostState(int mask, bool value) => _axHostState[mask] = value;

    internal IntPtr GetHandleNoCreate() => IsHandleCreated ? Handle : IntPtr.Zero;

    internal void TransitionUpTo(WebBrowserHelper.AXState state)
    {
        if (GetAXHostState(WebBrowserHelper.s_inTransition))
        {
            return;
        }

        SetAXHostState(WebBrowserHelper.s_inTransition, true);

        try
        {
            while (state > ActiveXState)
            {
                switch (ActiveXState)
                {
                    case WebBrowserHelper.AXState.Passive:
                        TransitionFromPassiveToLoaded();
                        Debug.Assert(ActiveXState == WebBrowserHelper.AXState.Loaded, "Failed transition");
                        break;
                    case WebBrowserHelper.AXState.Loaded:
                        TransitionFromLoadedToRunning();
                        Debug.Assert(ActiveXState == WebBrowserHelper.AXState.Running, "Failed transition");
                        break;
                    case WebBrowserHelper.AXState.Running:
                        TransitionFromRunningToInPlaceActive();
                        Debug.Assert(ActiveXState == WebBrowserHelper.AXState.InPlaceActive, "Failed transition");
                        break;
                    case WebBrowserHelper.AXState.InPlaceActive:
                        TransitionFromInPlaceActiveToUIActive();
                        Debug.Assert(ActiveXState == WebBrowserHelper.AXState.UIActive, "Failed transition");
                        break;
                    default:
                        Debug.Fail("bad state");
                        ActiveXState++; // To exit the loop
                        break;
                }
            }
        }
        finally
        {
            SetAXHostState(WebBrowserHelper.s_inTransition, false);
        }
    }

    internal void TransitionDownTo(WebBrowserHelper.AXState state)
    {
        if (!GetAXHostState(WebBrowserHelper.s_inTransition))
        {
            SetAXHostState(WebBrowserHelper.s_inTransition, true);

            try
            {
                while (state < ActiveXState)
                {
                    switch (ActiveXState)
                    {
                        case WebBrowserHelper.AXState.UIActive:
                            TransitionFromUIActiveToInPlaceActive();
                            Debug.Assert(ActiveXState == WebBrowserHelper.AXState.InPlaceActive, "Failed transition");
                            break;
                        case WebBrowserHelper.AXState.InPlaceActive:
                            TransitionFromInPlaceActiveToRunning();
                            Debug.Assert(ActiveXState == WebBrowserHelper.AXState.Running, "Failed transition");
                            break;
                        case WebBrowserHelper.AXState.Running:
                            TransitionFromRunningToLoaded();
                            Debug.Assert(ActiveXState == WebBrowserHelper.AXState.Loaded, "Failed transition");
                            break;
                        case WebBrowserHelper.AXState.Loaded:
                            TransitionFromLoadedToPassive();
                            Debug.Assert(ActiveXState == WebBrowserHelper.AXState.Passive, "Failed transition");
                            break;
                        default:
                            Debug.Fail("bad state");
                            ActiveXState--; // To exit the loop
                            break;
                    }
                }
            }
            finally
            {
                SetAXHostState(WebBrowserHelper.s_inTransition, false);
            }
        }
    }

    internal unsafe bool DoVerb(OLEIVERB verb)
    {
        RECT posRect = Bounds;
        using var clientSite = ComHelpers.GetComScope<IOleClientSite>(ActiveXSite);
        HRESULT hr = _axOleObject!.DoVerb((int)verb, null, clientSite, 0, HWND, &posRect);
        Debug.Assert(hr.Succeeded, $"DoVerb call failed for verb 0x{verb}");
        return hr.Succeeded;
    }

    //
    // Returns this control's logically containing form.
    // At design time this is always the form being designed.
    // At runtime it is the parent form.
    // By default, the parent form performs that function. It is
    // however possible for another form higher in the parent chain
    // to serve in that role. The logical container of this
    // control determines the set of logical sibling control.
    // This property exists only to enable some specific
    // behaviors of ActiveX controls.
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal ContainerControl? ContainingControl
    {
        get
        {
            if (_containingControl is null ||
                GetAXHostState(WebBrowserHelper.s_recomputeContainingControl))
            {
                _containingControl = FindContainerControlInternal();
            }

            return _containingControl;
        }
    }

    internal WebBrowserContainer CreateWebBrowserContainer() =>
        _wbContainer ??= new WebBrowserContainer(this);

    internal WebBrowserContainer GetParentContainer()
    {
        _container ??= WebBrowserContainer.FindContainerForControl(this);

        if (_container is null)
        {
            _container = CreateWebBrowserContainer();
            _container.AddControl(this);
        }

        return _container;
    }

    internal void SetEditMode(WebBrowserHelper.AXEditMode em)
    {
        _axEditMode = em;
    }

    internal void SetSelectionStyle(WebBrowserHelper.SelectionStyle selectionStyle)
    {
        if (DesignMode)
        {
            ISelectionService? iss = WebBrowserHelper.GetSelectionService(this);
            _selectionStyle = selectionStyle;
            if (iss is not null && iss.GetComponentSelected(this))
            {
                // The ActiveX Host designer will offer an extender property
                // called "SelectionStyle"
                PropertyDescriptor? prop = TypeDescriptor.GetProperties(this)["SelectionStyle"];
                if (prop is not null && prop.PropertyType == typeof(int))
                {
                    prop.SetValue(this, (int)selectionStyle);
                }
            }
        }
    }

    internal void AddSelectionHandler()
    {
        if (!GetAXHostState(WebBrowserHelper.s_addedSelectionHandler))
        {
            SetAXHostState(WebBrowserHelper.s_addedSelectionHandler, true);

            ISelectionService? iss = WebBrowserHelper.GetSelectionService(this);
            if (iss is not null)
            {
                iss.SelectionChanging += SelectionChangeHandler;
            }
        }
    }

    internal bool RemoveSelectionHandler()
    {
        bool retVal = GetAXHostState(WebBrowserHelper.s_addedSelectionHandler);
        if (retVal)
        {
            SetAXHostState(WebBrowserHelper.s_addedSelectionHandler, false);

            ISelectionService? iss = WebBrowserHelper.GetSelectionService(this);
            if (iss is not null)
            {
                iss.SelectionChanging -= SelectionChangeHandler;
            }
        }

        return retVal;
    }

    internal void AttachWindow(HWND hwnd)
    {
        PInvoke.SetParent(hwnd, this);

        _axWindow?.ReleaseHandle();

        _axWindow = new WebBrowserBaseNativeWindow(this);
        _axWindow.AssignHandle(hwnd, false);

        UpdateZOrder();
        UpdateBounds();

        Size extent = Size;
        extent = SetExtent(extent.Width, extent.Height);

        Point location = Location;
        Bounds = new Rectangle(location.X, location.Y, extent.Width, extent.Height);
    }

    internal bool IsUserMode => Site is null || !DesignMode;

    internal void MakeDirty()
    {
        if (Site.TryGetService(out IComponentChangeService? changeService))
        {
            changeService.OnComponentChanging(this);
            changeService.OnComponentChanged(this);
        }
    }

    internal int NoComponentChangeEvents { get; set; }

    //
    // Private helper methods:
    //

    private void StartEvents()
    {
        if (!GetAXHostState(WebBrowserHelper.s_sinkAttached))
        {
            SetAXHostState(WebBrowserHelper.s_sinkAttached, true);
            CreateSink();
        }

        ActiveXSite.StartEvents();
    }

    private void StopEvents()
    {
        if (GetAXHostState(WebBrowserHelper.s_sinkAttached))
        {
            SetAXHostState(WebBrowserHelper.s_sinkAttached, false);
            DetachSink();
        }

        ActiveXSite.StopEvents();
    }

    private void TransitionFromPassiveToLoaded()
    {
        Debug.Assert(ActiveXState == WebBrowserHelper.AXState.Passive, "Wrong start state to transition from");
        if (ActiveXState == WebBrowserHelper.AXState.Passive)
        {
            // First, create the ActiveX control
            Debug.Assert(_activeXInstance is null, "activeXInstance must be null");

            HRESULT hr = PInvokeCore.CoCreateInstance(
                in _clsid,
                null,
                CLSCTX.CLSCTX_INPROC_SERVER,
                IID.GetRef<IUnknown>(),
                out void* unknown);

            hr.ThrowOnFailure();

            _activeXInstance = ComHelpers.GetObjectForIUnknown((IUnknown*)unknown);

            Debug.Assert(_activeXInstance is not null, "w/o an exception being thrown we must have an object...");

            // We are now loaded.
            ActiveXState = WebBrowserHelper.AXState.Loaded;

            // Lets give them a chance to cast the ActiveX object to the appropriate interfaces.
            AttachInterfacesInternal();
        }
    }

    private void TransitionFromLoadedToPassive()
    {
        Debug.Assert(ActiveXState == WebBrowserHelper.AXState.Loaded, "Wrong start state to transition from");
        if (ActiveXState == WebBrowserHelper.AXState.Loaded)
        {
            // Need to make sure that we don't handle any PropertyChanged notifications at this point.
            NoComponentChangeEvents++;
            try
            {
                // Release the activeXInstance
                if (_activeXInstance is not null)
                {
                    // Lets first get the cached interface pointers of activeXInstance released.
                    DetachInterfacesInternal();

                    Marshal.FinalReleaseComObject(_activeXInstance);
                    _activeXInstance = null;
                }
            }
            finally
            {
                NoComponentChangeEvents--;
            }

            // We are now Passive.
            ActiveXState = WebBrowserHelper.AXState.Passive;
        }
    }

    private unsafe void TransitionFromLoadedToRunning()
    {
        Debug.Assert(ActiveXState == WebBrowserHelper.AXState.Loaded, "Wrong start state to transition from");
        if (ActiveXState == WebBrowserHelper.AXState.Loaded)
        {
            // See if the ActiveX control returns OLEMISC_SETCLIENTSITEFIRST
            HRESULT hr = _axOleObject.GetMiscStatus(DVASPECT.DVASPECT_CONTENT, out OLEMISC bits);
            if (hr.Succeeded && bits.HasFlag(OLEMISC.OLEMISC_SETCLIENTSITEFIRST))
            {
                // Simply setting the site to the ActiveX control should activate it.
                // And this will take us to the Running state.
                _axOleObject!.SetClientSite(ComHelpers.GetComPointer<IOleClientSite>(ActiveXSite));
            }

            // We start receiving events now (but we do this only if we are not in DesignMode).
            if (!DesignMode)
            {
                StartEvents();
            }

            // We are now Running.
            ActiveXState = WebBrowserHelper.AXState.Running;
        }
    }

    private unsafe void TransitionFromRunningToLoaded()
    {
        Debug.Assert(ActiveXState == WebBrowserHelper.AXState.Running, "Wrong start state to transition from");
        if (ActiveXState == WebBrowserHelper.AXState.Running)
        {
            StopEvents();

            // Remove ourselves from our parent container...
            WebBrowserContainer parentContainer = GetParentContainer();
            parentContainer?.RemoveControl(this);

            // Now inform the ActiveX control that it's been un-sited.
            _axOleObject!.SetClientSite(null);

            // We are now Loaded!
            ActiveXState = WebBrowserHelper.AXState.Loaded;
        }
    }

    private void TransitionFromRunningToInPlaceActive()
    {
        Debug.Assert(ActiveXState == WebBrowserHelper.AXState.Running, "Wrong start state to transition from");
        if (ActiveXState == WebBrowserHelper.AXState.Running)
        {
            try
            {
                DoVerb(OLEIVERB.OLEIVERB_INPLACEACTIVATE);
            }
            catch (Exception t)
            {
                throw new TargetInvocationException(string.Format(SR.AXNohWnd, GetType().Name), t);
            }

            CreateControl(true);

            //
            // We are now InPlaceActive!
            ActiveXState = WebBrowserHelper.AXState.InPlaceActive;
        }
    }

    private void TransitionFromInPlaceActiveToRunning()
    {
        Debug.Assert(ActiveXState == WebBrowserHelper.AXState.InPlaceActive, "Wrong start state to transition from");
        if (ActiveXState == WebBrowserHelper.AXState.InPlaceActive)
        {
            // First, lets make sure we transfer the ContainingControl's ActiveControl
            // before we InPlaceDeactivate.
            ContainerControl? containingControl = ContainingControl;
            if (containingControl is not null && containingControl.ActiveControl == this)
            {
                containingControl.SetActiveControl(null);
            }

            // Now, InPlaceDeactivate.
            AXInPlaceObject!.InPlaceDeactivate();

            // We are now Running!
            ActiveXState = WebBrowserHelper.AXState.Running;
        }
    }

    private void TransitionFromInPlaceActiveToUIActive()
    {
        Debug.Assert(ActiveXState == WebBrowserHelper.AXState.InPlaceActive, "Wrong start state to transition from");
        if (ActiveXState == WebBrowserHelper.AXState.InPlaceActive)
        {
            try
            {
                DoVerb(OLEIVERB.OLEIVERB_UIACTIVATE);
            }
            catch (Exception t)
            {
                throw new TargetInvocationException(string.Format(SR.AXNohWnd, GetType().Name), t);
            }

            //
            // We are now UIActive
            ActiveXState = WebBrowserHelper.AXState.UIActive;
        }
    }

    private void TransitionFromUIActiveToInPlaceActive()
    {
        Debug.Assert(ActiveXState == WebBrowserHelper.AXState.UIActive, "Wrong start state to transition from");
        if (ActiveXState == WebBrowserHelper.AXState.UIActive)
        {
            HRESULT hr = AXInPlaceObject!.UIDeactivate();
            Debug.Assert(hr.Succeeded, "Failed to UIDeactivate");

            // We are now InPlaceActive
            ActiveXState = WebBrowserHelper.AXState.InPlaceActive;
        }
    }

    internal WebBrowserSiteBase ActiveXSite =>
        _axSite ??= CreateWebBrowserSiteBase();

    private void AttachInterfacesInternal()
    {
        Debug.Assert(_activeXInstance is not null, "The native control is null");
        _axOleObject = (IOleObject.Interface)_activeXInstance;
        _axOleInPlaceObject = (IOleInPlaceObject.Interface)_activeXInstance;
        _axOleInPlaceActiveObject = (IOleInPlaceActiveObject.Interface)_activeXInstance;
        _axOleControl = (IOleControl.Interface)_activeXInstance;

        // Give the inheriting classes a chance to cast the ActiveX object to the
        // appropriate interfaces.
        AttachInterfaces(_activeXInstance);
    }

    private void DetachInterfacesInternal()
    {
        _axOleObject = null;
        _axOleInPlaceObject = null;
        _axOleInPlaceActiveObject = null;
        _axOleControl = null;

        // Lets give the inheriting classes a chance to release
        // their cached interfaces of the ActiveX object.
        DetachInterfaces();
    }

    // We need to change the ActiveX control's state when selection changes.
    private EventHandler SelectionChangeHandler => _selectionChangeHandler ??= OnNewSelection;

    // We need to do special stuff (convert window messages to interface calls)
    // during design time when selection changes.
    private void OnNewSelection(object? sender, EventArgs e)
    {
        if (DesignMode)
        {
            ISelectionService? iss = WebBrowserHelper.GetSelectionService(this);
            if (iss is not null)
            {
                // We are no longer selected.
                if (!iss.GetComponentSelected(this))
                {
                    // We need to exit editmode if we were in one.
                    if (EditMode)
                    {
                        GetParentContainer().OnExitEditMode(this);
                        SetEditMode(WebBrowserHelper.AXEditMode.None);
                    }

                    SetSelectionStyle(WebBrowserHelper.SelectionStyle.Selected);
                    RemoveSelectionHandler();
                }
                else
                {
                    //
                    // The AX Host designer will offer an extender property called "SelectionStyle"
                    PropertyDescriptor? prop = TypeDescriptor.GetProperties(this)["SelectionStyle"];
                    if (prop is not null && prop.PropertyType == typeof(int))
                    {
                        int curSelectionStyle = (int)prop.GetValue(this)!;
                        if (curSelectionStyle != (int)_selectionStyle)
                        {
                            prop.SetValue(this, _selectionStyle);
                        }
                    }
                }
            }
        }
    }

    private unsafe Size SetExtent(int width, int height)
    {
        Size sz = new(width, height);
        bool resetExtents = DesignMode;
        Pixel2hiMetric(ref sz);
        HRESULT hr = _axOleObject!.SetExtent(DVASPECT.DVASPECT_CONTENT, (SIZE*)&sz);
        if (hr != HRESULT.S_OK)
        {
            resetExtents = true;
        }

        if (resetExtents)
        {
            _axOleObject.GetExtent(DVASPECT.DVASPECT_CONTENT, (SIZE*)&sz);
            _axOleObject.SetExtent(DVASPECT.DVASPECT_CONTENT, (SIZE*)&sz);
        }

        return GetExtent();
    }

    private unsafe Size GetExtent()
    {
        Size size = default;
        _axOleObject!.GetExtent(DVASPECT.DVASPECT_CONTENT, (SIZE*)&size);
        HiMetric2Pixel(ref size);
        return size;
    }

    private unsafe void HiMetric2Pixel(ref Size sz)
    {
        Point phm = new(sz.Width, sz.Height);
        PointF pcont = default;
        ((IOleControlSite.Interface)ActiveXSite).TransformCoords(
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
        ((IOleControlSite.Interface)ActiveXSite).TransformCoords(
            (POINTL*)&phm,
            &pcont,
            (uint)(XFORMCOORDS.XFORMCOORDS_SIZE | XFORMCOORDS.XFORMCOORDS_CONTAINERTOHIMETRIC));
        sz.Width = phm.X;
        sz.Height = phm.Y;
    }

    private bool EditMode =>
        _axEditMode != WebBrowserHelper.AXEditMode.None;

    // Find the uppermost ContainerControl that this control lives in
    internal ContainerControl? FindContainerControlInternal()
    {
        if (Site.TryGetService(out IDesignerHost? host) && host.RootComponent is ContainerControl rootContainerControl)
        {
            return rootContainerControl;
        }

        ContainerControl? containerControl = null;
        for (Control? control = this; control is not null; control = control.ParentInternal)
        {
            if (control is ContainerControl tempCC)
            {
                containerControl = tempCC;
            }
        }

        if (containerControl is null && IsHandleCreated)
        {
            containerControl = FromHandle(PInvoke.GetParent(this)) as ContainerControl;
        }

        // Never use the parking window for this: its hwnd can be destroyed at any time.
        if (containerControl is Application.ParkingWindow)
        {
            containerControl = null;
        }

        SetAXHostState(WebBrowserHelper.s_recomputeContainingControl, containerControl is null);

        return containerControl;
    }

    private void AmbientChanged(int dispid)
    {
        if (_activeXInstance is not null)
        {
            Invalidate();
            HRESULT result = _axOleControl!.OnAmbientPropertyChange(dispid);
            Debug.Assert(!result.Failed, $"{result}");
        }
    }

    internal IOleInPlaceObject.Interface? AXInPlaceObject => _axOleInPlaceObject;

    // ---------------------------------------------------------------
    // The following properties implemented in the Control class don't make
    // sense for ActiveX controls. So we block them here.
    // ---------------------------------------------------------------

    //
    // Overridden properties:
    //

    protected override Size DefaultSize => new(75, 23);

    //
    // Overridden methods:
    //

    protected override bool IsInputChar(char charCode)
    {
        return true;
    }

    /// <summary>
    ///  Inheriting classes should override this method to find out when the
    ///  handle has been created. Call base.OnHandleCreated first.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnHandleCreated(EventArgs e)
    {
        //
        // This is needed to prevent some controls (for e.g. Office Web Components) from
        // failing to InPlaceActivate() when they call RegisterDragDrop() but do not call
        // OleInitialize(). The EE calls CoInitializeEx() on the thread, but I believe
        // that is not good enough for DragDrop.
        //
        if (Application.OleRequired() != ApartmentState.STA)
        {
            throw new ThreadStateException(SR.ThreadMustBeSTA);
        }

        base.OnHandleCreated(e);

        // make sure we restore whatever running state we had prior to the handle recreate.
        //
        if (_axReloadingState != WebBrowserHelper.AXState.Passive && _axReloadingState != _axState)
        {
            if (_axState < _axReloadingState)
            {
                TransitionUpTo(_axReloadingState);
            }
            else
            {
                TransitionDownTo(_axReloadingState);
            }

            _axReloadingState = WebBrowserHelper.AXState.Passive;
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Color BackColor
    {
        get => base.BackColor;
        set => base.BackColor = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [AllowNull]
    public override Font Font
    {
        get => base.Font;
        set => base.Font = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Color ForeColor
    {
        get => base.ForeColor;
        set => base.ForeColor = value;
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

    //
    // Properties blocked at design time and run time:
    //
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override bool AllowDrop
    {
        get => base.AllowDrop;
        set
        {
            throw new NotSupportedException(SR.WebBrowserAllowDropNotSupported);
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Image? BackgroundImage
    {
        get => base.BackgroundImage;
        set
        {
            throw new NotSupportedException(SR.WebBrowserBackgroundImageNotSupported);
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override ImageLayout BackgroundImageLayout
    {
        get => base.BackgroundImageLayout;
        set
        {
            throw new NotSupportedException(SR.WebBrowserBackgroundImageLayoutNotSupported);
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [AllowNull]
    public override Cursor Cursor
    {
        get => base.Cursor;
        set
        {
            throw new NotSupportedException(SR.WebBrowserCursorNotSupported);
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new bool Enabled
    {
        get => base.Enabled;
        set
        {
            throw new NotSupportedException(SR.WebBrowserEnabledNotSupported);
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Localizable(false)]
    public override RightToLeft RightToLeft
    {
        get => RightToLeft.No;
        set
        {
            throw new NotSupportedException(SR.WebBrowserRightToLeftNotSupported);
        }
    }

    // Override this property so that the Bindable attribute can be set to false.
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Bindable(false)]
    [AllowNull]
    public override string Text
    {
        get => string.Empty;
        set => throw new NotSupportedException(SR.WebBrowserTextNotSupported);
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new bool UseWaitCursor
    {
        get => base.UseWaitCursor;
        set
        {
            throw new NotSupportedException(SR.WebBrowserUseWaitCursorNotSupported);
        }
    }

    //
    // Unavailable events
    //

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackgroundImageLayoutChanged
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "BackgroundImageLayoutChanged"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? Enter
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "Enter"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? Leave
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "Leave"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? MouseCaptureChanged
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseCaptureChanged"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event MouseEventHandler? MouseClick
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseClick"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event MouseEventHandler? MouseDoubleClick
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseDoubleClick"));
        remove { }
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
    // Everett
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

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event KeyEventHandler? KeyDown
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "KeyDown"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event KeyPressEventHandler? KeyPress
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "KeyPress"));
        remove { }
    }

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

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event MouseEventHandler? MouseDown
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseDown"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? MouseEnter
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseEnter"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? MouseLeave
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseLeave"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? MouseHover
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseHover"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event MouseEventHandler? MouseMove
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseMove"));
        remove { }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event MouseEventHandler? MouseUp
    {
        add => throw new NotSupportedException(string.Format(SR.AXAddInvalidEvent, "MouseUp"));
        remove { }
    }

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
}
