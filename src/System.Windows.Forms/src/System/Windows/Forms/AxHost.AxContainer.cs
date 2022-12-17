// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using static Interop;
using static Interop.Ole32;
using Com = Windows.Win32.System.Com;
using Ole = Windows.Win32.System.Ole;
using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms;

public abstract partial class AxHost
{
    internal class AxContainer : Ole.IOleContainer.Interface, IOleInPlaceFrame, IReflect
    {
        internal ContainerControl _parent;

        // The associated container may be null, in which case all this container does is
        // forward [de]activation messages to the requisite container.
        private IContainer? _associatedContainer;

        private AxHost? _siteUIActive;
        private AxHost? _siteActive;
        private bool _formAlreadyCreated;
        private readonly HashSet<AxHost> _containerCache = new();
        private int _lockCount;
        private HashSet<Control>? _components;
        private Dictionary<Control, Oleaut32.IExtender>? _proxyCache;
        private AxHost? _controlInEditMode;

        internal AxContainer(ContainerControl parent)
        {
            s_axHTraceSwitch.TraceVerbose($"in constructor.  Parent created : {parent.Created}");
            _parent = parent;
            if (parent.Created)
            {
                FormCreated();
            }
        }

        // IReflect methods:

        MethodInfo? IReflect.GetMethod(
            string name,
            BindingFlags bindingAttr,
            Binder? binder,
            Type[] types,
            ParameterModifier[]? modifiers) => null;

        MethodInfo? IReflect.GetMethod(string name, BindingFlags bindingAttr) => null;

        MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr) => Array.Empty<MethodInfo>();

        FieldInfo? IReflect.GetField(string name, BindingFlags bindingAttr) => null;

        FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr) => Array.Empty<FieldInfo>();

        PropertyInfo? IReflect.GetProperty(string name, BindingFlags bindingAttr) => null;

        PropertyInfo? IReflect.GetProperty(
            string name,
            BindingFlags bindingAttr,
            Binder? binder,
            Type? returnType,
            Type[] types,
            ParameterModifier[]? modifiers) => null;

        PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr) => Array.Empty<PropertyInfo>();

        MemberInfo[] IReflect.GetMember(string name, BindingFlags bindingAttr) => Array.Empty<MemberInfo>();

        MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr) => Array.Empty<MemberInfo>();

        object? IReflect.InvokeMember(
            string name,
            BindingFlags invokeAttr,
            Binder? binder,
            object? target,
            object?[]? args,
            ParameterModifier[]? modifiers,
            CultureInfo? culture,
            string[]? namedParameters)
        {
            foreach (AxHost control in _containerCache)
            {
                if (GetNameForControl(control).Equals(name))
                {
                    return GetProxyForControl(control);
                }
            }

            throw s_unknownErrorException;
        }

        // We returned null here historically, the interface is now annotated as not returning null.
        // Risky to change.
        Type IReflect.UnderlyingSystemType => null!;

        internal Oleaut32.IExtender? GetProxyForControl(Control control)
        {
            Oleaut32.IExtender? extender = null;
            if (_proxyCache is null)
            {
                _proxyCache = new();
            }
            else
            {
                _proxyCache.TryGetValue(control, out extender);
            }

            if (extender is null)
            {
                if (control != _parent && !GetComponents().Contains(control))
                {
                    s_axHTraceSwitch.TraceVerbose("!parent || !belongs NYI");
                    AxContainer? container = FindContainerForControl(control);
                    if (container is not null)
                    {
                        extender = new ExtenderProxy(control, container);
                    }
                    else
                    {
                        s_axHTraceSwitch.TraceVerbose("unable to find proxy, returning null");
                        return null;
                    }
                }
                else
                {
                    extender = new ExtenderProxy(control, this);
                }

                _proxyCache.Add(control, extender);
            }

            s_axHTraceSwitch.TraceVerbose($"found proxy {extender}");
            return extender;
        }

        internal static string GetNameForControl(Control control)
        {
            string? name = control.Site is { } site ? site.Name : control.Name;
            return name ?? string.Empty;
        }

        internal void AddControl(AxHost control)
        {
            lock (this)
            {
                if (_containerCache.Contains(control))
                {
                    throw new ArgumentException(string.Format(SR.AXDuplicateControl, GetNameForControl(control)), nameof(control));
                }

                _containerCache.Add(control);

                if (_associatedContainer is null)
                {
                    if (control.Site is { } site)
                    {
                        _associatedContainer = site.Container;
                        if (site.TryGetService(out IComponentChangeService? changeService))
                        {
                            changeService.ComponentRemoved += OnComponentRemoved;
                        }
                    }
                }
                else
                {
#if DEBUG
                    if (control.Site is { } site && _associatedContainer != site.Container)
                    {
                        s_axHTraceSwitch.TraceVerbose("mismatch between assoc container & added control");
                    }
#endif
                }
            }
        }

        internal void RemoveControl(AxHost control)
        {
            lock (this)
            {
                _containerCache.Remove(control);
            }
        }

        internal Com.IEnumUnknown.Interface EnumControls(Control control, OLECONTF dwOleContF, GC_WCH dwWhich)
        {
            GetComponents();
            _lockCount++;

            Control? additionalControl = control;

            try
            {
                // Results are IUnknown
                List<object>? results = null;

                bool selected = dwWhich.HasFlag(GC_WCH.FSELECTED);
                bool reverse = dwWhich.HasFlag(GC_WCH.FREVERSEDIR);

                // Note that Visual Basic actually ignores the next/prev flags. We will not.
                bool onlyNext = dwWhich.HasFlag(GC_WCH.FONLYNEXT);
                bool onlyPrevious = dwWhich.HasFlag(GC_WCH.FONLYPREV);

                dwWhich &= ~(GC_WCH.FSELECTED | GC_WCH.FREVERSEDIR | GC_WCH.FONLYNEXT | GC_WCH.FONLYPREV);
                if (onlyNext && onlyPrevious)
                {
                    Debug.Fail("onlyNext && onlyPrevious are both set");
                    throw s_invalidArgumentException;
                }

                if (dwWhich is GC_WCH.CONTAINER or GC_WCH.CONTAINED && (onlyNext || onlyPrevious))
                {
                    Debug.Fail("GC_WCH_FONLYNEXT or FONLYPREV used with CONTAINER or CONTAINED");
                    throw s_invalidArgumentException;
                }

                int first = 0;
                int last = -1; // meaning all
                Control[] controls = Array.Empty<Control>();
                switch (dwWhich)
                {
                    default:
                        Debug.Fail("Bad GC_WCH");
                        throw s_invalidArgumentException;
                    case GC_WCH.CONTAINED:
                        controls = control.GetChildControlsInTabOrder(handleCreatedOnly: false);
                        additionalControl = null;
                        break;
                    case GC_WCH.SIBLING:
                        if (control.ParentInternal is { } parent)
                        {
                            controls = parent.GetChildControlsInTabOrder(handleCreatedOnly: false);
                            if (onlyPrevious)
                            {
                                last = control.TabIndex;
                            }
                            else if (onlyNext)
                            {
                                first = control.TabIndex + 1;
                            }
                        }

                        additionalControl = null;
                        break;
                    case GC_WCH.CONTAINER:
                        results = new();
                        additionalControl = null;
                        MaybeAdd(results, control, selected, dwOleContF, allowContainingControls: false);

                        while (control is not null)
                        {
                            if (FindContainerForControl(control) is { } container)
                            {
                                MaybeAdd(results, container._parent, selected, dwOleContF, allowContainingControls: true);
                                control = container._parent;
                            }
                            else
                            {
                                break;
                            }
                        }

                        break;
                    case GC_WCH.ALL:
                        controls = GetComponents().ToArray();
                        additionalControl = _parent;
                        break;
                }

                if (results is null)
                {
                    results = new();
                    if (last == -1 && controls is not null)
                    {
                        last = controls.Length;
                    }

                    if (additionalControl is not null)
                    {
                        MaybeAdd(results, additionalControl, selected, dwOleContF, allowContainingControls: false);
                    }

                    if (controls is not null)
                    {
                        for (int i = first; i < last; i++)
                        {
                            MaybeAdd(results, controls[i], selected, dwOleContF, allowContainingControls: false);
                        }
                    }
                }

                if (reverse)
                {
                    results.Reverse();
                }

                return new EnumUnknown(results.ToArray());
            }
            finally
            {
                _lockCount--;
                if (_lockCount == 0)
                {
                    _components = null;
                }
            }
        }

        private void MaybeAdd(List<object> controls, Control control, bool selected, OLECONTF flags, bool allowContainingControls)
        {
            if (!allowContainingControls && control != _parent && !GetComponents().Contains(control))
            {
                return;
            }

            if (selected)
            {
                if (control.Site is not { } site
                    || !site.TryGetService(out ISelectionService? selectionService)
                    || !selectionService.GetComponentSelected(this))
                {
                    return;
                }
            }

            if (control is AxHost hostControl && flags.HasFlag(OLECONTF.EMBEDDINGS))
            {
                controls.Add(hostControl.GetOcx());
            }
            else if (flags.HasFlag(OLECONTF.OTHERS))
            {
                if (GetProxyForControl(control) is { } proxy)
                {
                    controls.Add(proxy);
                }
            }
        }

        [MemberNotNull(nameof(_components))]
        private void FillComponentsTable(IContainer? container)
        {
            if (container?.Components is { } components)
            {
                _components = new();
                foreach (IComponent component in components)
                {
                    if (component is Control control && component != _parent && component.Site is not null)
                    {
                        _components.Add(control);
                    }
                }

                return;
            }

            Debug.Assert(_parent.Site is null, "Parent is sited but we could not find IContainer");
            s_axHTraceSwitch.TraceVerbose("Did not find a container in FillComponentsTable");

            if (_containerCache.Count > 0)
            {
                if (_components is null)
                {
                    _components = new();
                }
                else
                {
                    foreach (AxHost control in _containerCache)
                    {
                        _components.Add(control);
                    }
                }
            }

            GetAllChildren(_parent);
            Debug.Assert(_components is not null);

            [MemberNotNull(nameof(_components))]
            void GetAllChildren(Control control)
            {
                _components ??= new();

                if (control != _parent)
                {
                    _components.Add(control);
                }

                foreach (Control child in control.Controls)
                {
                    GetAllChildren(child);
                }
            }
        }

        private HashSet<Control> GetComponents()
        {
            if (_lockCount == 0)
            {
                FillComponentsTable(GetParentsContainer());
            }

            Debug.Assert(_components is not null);

            return _components!;
        }

        private IContainer? GetParentIsDesigned()
            => _parent.Site is { } site && site.DesignMode ? site.Container : null;

        private IContainer? GetParentsContainer()
        {
            IContainer? container = GetParentIsDesigned();
            Debug.Assert(
                container is null || _associatedContainer is null || (container == _associatedContainer),
                "mismatch between getIPD & aContainer");
            return container ?? _associatedContainer;
        }

        private bool RegisterControl(AxHost control)
        {
            if (control.Site is { } site && site.Container is { } container)
            {
                if (_associatedContainer is not null)
                {
                    return container == _associatedContainer;
                }
                else
                {
                    _associatedContainer = container;
                    if (site.TryGetService(out IComponentChangeService? changeService))
                    {
                        changeService.ComponentRemoved += OnComponentRemoved;
                    }

                    return true;
                }
            }

            return false;
        }

        private void OnComponentRemoved(object? sender, ComponentEventArgs e)
        {
            if (sender == _associatedContainer && e.Component is AxHost control)
            {
                RemoveControl(control);
            }
        }

        internal static AxContainer? FindContainerForControl(Control control)
        {
            if (control is AxHost axHost)
            {
                if (axHost._container is { } existing)
                {
                    return existing;
                }

                if (axHost.ContainingControl?.CreateAxContainer() is { } container)
                {
                    if (container.RegisterControl(axHost))
                    {
                        container.AddControl(axHost);
                        return container;
                    }
                }
            }

            return null;
        }

        internal void OnInPlaceDeactivate(AxHost site)
        {
            if (_siteActive == site)
            {
                _siteActive = null;
                if (site.GetSiteOwnsDeactivation())
                {
                    _parent.ActiveControl = null;
                }
                else
                {
                    // We need to tell the form to switch activation to the next item
                    Debug.Fail("Control is calling OnInPlaceDeactivate by itself");
                }
            }
        }

        internal void OnUIDeactivate(AxHost site)
        {
            Debug.Assert(_siteUIActive is null || _siteUIActive == site, "Deactivating when not active");

            _siteUIActive = null;
            site.RemoveSelectionHandler();
            site.SetSelectionStyle(1);
            site._editMode = EDITM_NONE;
        }

        internal void OnUIActivate(AxHost site)
        {
            if (_siteUIActive == site)
            {
                return;
            }

            if (_siteUIActive is not null && _siteUIActive != site)
            {
                AxHost tempSite = _siteUIActive;
                bool ownDisposing = tempSite.GetAxState(s_ownDisposing);
                try
                {
                    tempSite.SetAxState(s_ownDisposing, true);
                    tempSite.GetInPlaceObject().UIDeactivate();
                }
                finally
                {
                    tempSite.SetAxState(s_ownDisposing, ownDisposing);
                }
            }

            site.AddSelectionHandler();

            // The ShDocVw control repeatedly calls OnUIActivate() with the same site.
            // This causes the assert below to fire.
            Debug.Assert(_siteUIActive is null, "Object did not call OnUIDeactivate");
            s_axHTraceSwitch.TraceVerbose($"active Object is now {site}");
            _siteUIActive = site;
            if (site.ContainingControl is { } container)
            {
                container.ActiveControl = site;
            }
        }

        internal void ControlCreated(AxHost invoker)
        {
            s_axHTraceSwitch.TraceVerbose($"in controlCreated for {invoker} fAC: {_formAlreadyCreated}");
            if (_formAlreadyCreated)
            {
                if (invoker.IsUserMode() && invoker.AwaitingDefreezing())
                {
                    invoker.FreezeEvents(false);
                }
            }
            else
            {
                // The form will be created in the future.
                _parent.CreateAxContainer();
            }
        }

        internal void FormCreated()
        {
            if (_formAlreadyCreated)
            {
                return;
            }

            _formAlreadyCreated = true;

            List<AxHost> hostControls = new();
            foreach (Control control in GetComponents())
            {
                if (control is AxHost hostControl)
                {
                    hostControls.Add(hostControl);
                }
            }

            foreach (AxHost hostControl in hostControls)
            {
                if (hostControl.GetOcState() >= OC_RUNNING && hostControl.IsUserMode() && hostControl.AwaitingDefreezing())
                {
                    hostControl.FreezeEvents(freeze: false);
                }
            }
        }

        unsafe HRESULT Ole.IParseDisplayName.Interface.ParseDisplayName(
            Com.IBindCtx* pbc,
            PWSTR pszDisplayName,
            uint* pchEaten,
            Com.IMoniker** ppmkOut)
            => ((Ole.IOleContainer.Interface)this).ParseDisplayName(pbc, pszDisplayName, pchEaten, ppmkOut);

        // IOleContainer methods:
        unsafe HRESULT Ole.IOleContainer.Interface.ParseDisplayName(
            Com.IBindCtx* pbc,
            PWSTR pszDisplayName,
            uint* pchEaten,
            Com.IMoniker** ppmkOut)
        {
            s_axHTraceSwitch.TraceVerbose("in ParseDisplayName");
            if (ppmkOut is not null)
            {
                *ppmkOut = null;
            }

            return HRESULT.E_NOTIMPL;
        }

        unsafe HRESULT Ole.IOleContainer.Interface.EnumObjects(Ole.OLECONTF grfFlags, Com.IEnumUnknown** ppenum)
        {
            if (ppenum is null)
            {
                return HRESULT.E_POINTER;
            }

            s_axHTraceSwitch.TraceVerbose("in EnumObjects");

            if ((grfFlags & Ole.OLECONTF.OLECONTF_EMBEDDINGS) != 0)
            {
                Debug.Assert(_parent is not null);

                List<object> oleControls = new();
                foreach (Control control in GetComponents())
                {
                    if (control is AxHost hostControl)
                    {
                        oleControls.Add(hostControl.GetOcx());
                    }
                }

                if (oleControls.Count > 0)
                {
                    ComHelpers.GetComPointer(new EnumUnknown(oleControls.ToArray()), out *ppenum);
                    return HRESULT.S_OK;
                }
            }

            ComHelpers.GetComPointer(new EnumUnknown(null), out *ppenum);
            return HRESULT.S_OK;
        }

        HRESULT Ole.IOleContainer.Interface.LockContainer(BOOL fLock)
        {
            s_axHTraceSwitch.TraceVerbose("in LockContainer");
            return HRESULT.E_NOTIMPL;
        }

        // IOleInPlaceFrame methods:
        unsafe HRESULT IOleInPlaceFrame.GetWindow(IntPtr* phwnd)
        {
            if (phwnd is null)
            {
                return HRESULT.E_POINTER;
            }

            s_axHTraceSwitch.TraceVerbose("in GetWindow");
            *phwnd = _parent.Handle;
            return HRESULT.S_OK;
        }

        HRESULT IOleInPlaceFrame.ContextSensitiveHelp(BOOL fEnterMode)
        {
            s_axHTraceSwitch.TraceVerbose("in ContextSensitiveHelp");
            return HRESULT.S_OK;
        }

        unsafe HRESULT IOleInPlaceFrame.GetBorder(RECT* lprectBorder)
        {
            s_axHTraceSwitch.TraceVerbose("in GetBorder");
            return HRESULT.E_NOTIMPL;
        }

        unsafe HRESULT IOleInPlaceFrame.RequestBorderSpace(RECT* pborderwidths)
        {
            s_axHTraceSwitch.TraceVerbose("in RequestBorderSpace");
            return HRESULT.E_NOTIMPL;
        }

        unsafe HRESULT IOleInPlaceFrame.SetBorderSpace(RECT* pborderwidths)
        {
            s_axHTraceSwitch.TraceVerbose("in SetBorderSpace");
            return HRESULT.E_NOTIMPL;
        }

        internal void OnExitEditMode(AxHost ctl)
        {
            Debug.Assert(_controlInEditMode is null || _controlInEditMode == ctl, "who is exiting edit mode?");
            if (_controlInEditMode is null || _controlInEditMode != ctl)
            {
                return;
            }

            _controlInEditMode = null;
        }

        unsafe HRESULT IOleInPlaceFrame.SetActiveObject(Ole.IOleInPlaceActiveObject.Interface? pActiveObject, string? pszObjName)
        {
            s_axHTraceSwitch.TraceVerbose($"in SetActiveObject {pszObjName ?? "<null>"}");
            if (_siteUIActive is { } activeHost && activeHost._iOleInPlaceActiveObjectExternal != pActiveObject)
            {
                if (activeHost._iOleInPlaceActiveObjectExternal is not null)
                {
                    Marshal.ReleaseComObject(activeHost._iOleInPlaceActiveObjectExternal);
                }

                activeHost._iOleInPlaceActiveObjectExternal = pActiveObject;
            }

            if (pActiveObject is null)
            {
                if (_controlInEditMode is not null)
                {
                    _controlInEditMode._editMode = EDITM_NONE;
                    _controlInEditMode = null;
                }

                return HRESULT.S_OK;
            }

            if (pActiveObject is not Ole.IOleObject.Interface oleObject)
            {
                return HRESULT.S_OK;
            }

            AxHost? host = null;
            Ole.IOleClientSite* clientSite;
            HRESULT hr = oleObject.GetClientSite(&clientSite);
            Debug.Assert(hr.Succeeded);

            var clientSiteObject = (Ole.IOleClientSite.Interface)Marshal.GetObjectForIUnknown((nint)clientSite);
            if (clientSiteObject is OleInterfaces interfaces)
            {
                host = interfaces.GetAxHost();
            }

            if (_controlInEditMode is not null)
            {
                Debug.Fail($"control {_controlInEditMode} did not reset its edit mode to null");
                _controlInEditMode.SetSelectionStyle(1);
                _controlInEditMode._editMode = EDITM_NONE;
            }

            if (host is null)
            {
                s_axHTraceSwitch.TraceVerbose("control w/o a valid site called setactiveobject");
                _controlInEditMode = null;
            }
            else
            {
                s_axHTraceSwitch.TraceVerbose($"resolved to {host}");
                if (!host.IsUserMode())
                {
                    _controlInEditMode = host;
                    host._editMode = EDITM_OBJECT;
                    host.AddSelectionHandler();
                    host.SetSelectionStyle(2);
                }
            }

            return HRESULT.S_OK;
        }

        unsafe HRESULT IOleInPlaceFrame.InsertMenus(IntPtr hmenuShared, OLEMENUGROUPWIDTHS* lpMenuWidths)
        {
            s_axHTraceSwitch.TraceVerbose("in InsertMenus");
            return HRESULT.S_OK;
        }

        HRESULT IOleInPlaceFrame.SetMenu(IntPtr hmenuShared, IntPtr holemenu, IntPtr hwndActiveObject)
        {
            s_axHTraceSwitch.TraceVerbose("in SetMenu");
            return HRESULT.E_NOTIMPL;
        }

        HRESULT IOleInPlaceFrame.RemoveMenus(IntPtr hmenuShared)
        {
            s_axHTraceSwitch.TraceVerbose("in RemoveMenus");
            return HRESULT.E_NOTIMPL;
        }

        HRESULT IOleInPlaceFrame.SetStatusText(string pszStatusText)
        {
            s_axHTraceSwitch.TraceVerbose("in SetStatusText");
            return HRESULT.E_NOTIMPL;
        }

        HRESULT IOleInPlaceFrame.EnableModeless(BOOL fEnable)
        {
            s_axHTraceSwitch.TraceVerbose("in EnableModeless");
            return HRESULT.E_NOTIMPL;
        }

        unsafe HRESULT IOleInPlaceFrame.TranslateAccelerator(MSG* lpmsg, ushort wID)
        {
            s_axHTraceSwitch.TraceVerbose("in IOleInPlaceFrame.TranslateAccelerator");
            return HRESULT.S_FALSE;
        }

        private class ExtenderProxy :
            Oleaut32.IExtender,
            IVBGetControl,
            IGetVBAObject,
            IGetOleObject,
            IReflect
        {
            private readonly WeakReference<Control> _principal;
            private readonly WeakReference<AxContainer> _container;

            internal ExtenderProxy(Control principal, AxContainer container)
            {
                _principal = new(principal);
                _container = new(container);
            }

            private Control? GetPrincipal()
            {
                _principal.TryGetTarget(out Control? target);
                return target;
            }

            private AxContainer? GetContainer()
            {
                _container.TryGetTarget(out AxContainer? container);
                return container;
            }

            HRESULT IVBGetControl.EnumControls(OLECONTF dwOleContF, GC_WCH dwWhich, out Com.IEnumUnknown.Interface ppenum)
            {
                s_axHTraceSwitch.TraceVerbose("in EnumControls for proxy");
                ppenum = GetContainer() is { } container && GetPrincipal() is { } principal
                    ? container.EnumControls(principal, dwOleContF, dwWhich)
                    : new EnumUnknown(null);
                return HRESULT.S_OK;
            }

            unsafe HRESULT IGetOleObject.GetOleObject(Guid* riid, out object? ppvObj)
            {
                s_axHTraceSwitch.TraceVerbose("in GetOleObject for proxy");
                ppvObj = null;
                if (riid is null || !riid->Equals(s_ioleobject_Guid))
                {
                    return HRESULT.E_INVALIDARG;
                }

                if (GetPrincipal() is AxHost hostControl)
                {
                    ppvObj = hostControl.GetOcx();
                    return HRESULT.S_OK;
                }

                return HRESULT.E_FAIL;
            }

            unsafe HRESULT IGetVBAObject.GetObject(Guid* riid, IVBFormat?[] rval, uint dwReserved)
            {
                s_axHTraceSwitch.TraceVerbose("in GetObject for proxy");
                if (rval is null || riid is null || rval.Length == 0)
                {
                    return HRESULT.E_INVALIDARG;
                }

                if (!riid->Equals(s_ivbformat_Guid))
                {
                    rval[0] = null;
                    return HRESULT.E_NOINTERFACE;
                }

                rval[0] = new VBFormat();
                return HRESULT.S_OK;
            }

            public int Align
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getAlign for proxy for {GetPrincipal()}");
                    int result = (int)(GetPrincipal()?.Dock ?? NativeMethods.ActiveX.ALIGN_NO_CHANGE);
                    if (result is < NativeMethods.ActiveX.ALIGN_MIN or > NativeMethods.ActiveX.ALIGN_MAX)
                    {
                        result = NativeMethods.ActiveX.ALIGN_NO_CHANGE;
                    }

                    return result;
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setAlign for proxy for {GetPrincipal()} {value}");
                    if (GetPrincipal() is { } control)
                    {
                        control.Dock = (DockStyle)value;
                    }
                }
            }

            public uint BackColor
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getBackColor for proxy for {GetPrincipal()}");
                    return GetOleColorFromColor(GetPrincipal()?.BackColor ?? default);
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setBackColor for proxy for {GetPrincipal()} {value}");
                    if (GetPrincipal() is { } control)
                    {
                        control.BackColor = GetColorFromOleColor(value);
                    }
                }
            }

            public BOOL Enabled
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getEnabled for proxy for {GetPrincipal()}");
                    return GetPrincipal()?.Enabled ?? BOOL.FALSE;
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setEnabled for proxy for {GetPrincipal()} {value}");
                    if (GetPrincipal() is { } control)
                    {
                        control.Enabled = value;
                    }
                }
            }

            public uint ForeColor
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getForeColor for proxy for {GetPrincipal()}");
                    return GetOleColorFromColor(GetPrincipal()?.ForeColor ?? default);
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setForeColor for proxy for {GetPrincipal()} {value}");
                    if (GetPrincipal() is { } control)
                    {
                        control.ForeColor = GetColorFromOleColor(value);
                    }
                }
            }

            public int Height
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getHeight for proxy for {GetPrincipal()}");
                    return Pixel2Twip(GetPrincipal()?.Height ?? 0, xDirection: false);
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setHeight for proxy for {GetPrincipal()} {Twip2Pixel(value, false)}");
                    if (GetPrincipal() is { } control)
                    {
                        control.Height = Twip2Pixel(value, xDirection: false);
                    }
                }
            }

            public int Left
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getLeft for proxy for {GetPrincipal()}");
                    return Pixel2Twip(GetPrincipal()?.Left ?? 0, xDirection: true);
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setLeft for proxy for {GetPrincipal()} {Twip2Pixel(value, true)}");
                    if (GetPrincipal() is { } control)
                    {
                        control.Left = Twip2Pixel(value, xDirection: true);
                    }
                }
            }

            public object? Parent
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getParent for proxy for {GetPrincipal()}");
                    return GetContainer() is { } container ? container.GetProxyForControl(container._parent) : null;
                }
            }

            public short TabIndex
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getTabIndex for proxy for {GetPrincipal()}");
                    return (short)(GetPrincipal()?.TabIndex ?? 0);
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setTabIndex for proxy for {GetPrincipal()} {value}");
                    if (GetPrincipal() is { } control)
                    {
                        control.TabIndex = value;
                    }
                }
            }

            public BOOL TabStop
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getTabStop for proxy for {GetPrincipal()}");
                    return GetPrincipal()?.TabStop ?? BOOL.FALSE;
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setTabStop for proxy for {GetPrincipal()} {value}");
                    if (GetPrincipal() is { } control)
                    {
                        control.TabStop = value;
                    }
                }
            }

            public int Top
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getTop for proxy for {GetPrincipal()}");
                    return Pixel2Twip(GetPrincipal()?.Top ?? 0, xDirection: false);
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setTop for proxy for {GetPrincipal()} {Twip2Pixel(value, false)}");
                    if (GetPrincipal() is { } control)
                    {
                        control.Top = Twip2Pixel(value, xDirection: false);
                    }
                }
            }

            public BOOL Visible
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getVisible for proxy for {GetPrincipal()}");
                    return GetPrincipal()?.Visible ?? false;
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setVisible for proxy for {GetPrincipal()} {value}");
                    if (GetPrincipal() is { } control)
                    {
                        control.Visible = value;
                    }
                }
            }

            public int Width
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getWidth for proxy for {GetPrincipal()}");
                    return Pixel2Twip(GetPrincipal()?.Width ?? 0, xDirection: true);
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setWidth for proxy for {GetPrincipal()} {Twip2Pixel(value, true)}");
                    if (GetPrincipal() is { } control)
                    {
                        control.Width = Twip2Pixel(value, xDirection: true);
                    }
                }
            }

            public string Name
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getName for proxy for {GetPrincipal()}");
                    return GetPrincipal() is { } control ? GetNameForControl(control) : string.Empty;
                }
            }

            public IntPtr Hwnd
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getHwnd for proxy for {GetPrincipal()}");
                    return GetPrincipal()?.Handle ?? default;
                }
            }

            public object? Container => GetContainer();

            public string Text
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getText for proxy for {GetPrincipal()}");
                    return GetPrincipal()?.Text ?? string.Empty;
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setText for proxy for {GetPrincipal()}");
                    if (GetPrincipal() is { } control)
                    {
                        control.Text = value;
                    }
                }
            }

            public void Move(object left, object top, object width, object height)
            {
            }

            // IReflect methods:

            MethodInfo? IReflect.GetMethod(
                string name,
                BindingFlags bindingAttr,
                Binder? binder,
                Type[] types,
                ParameterModifier[]? modifiers) => null;

            MethodInfo? IReflect.GetMethod(string name, BindingFlags bindingAttr) => null;
            MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr) => new[] { GetType().GetMethod("Move")! };

            FieldInfo? IReflect.GetField(string name, BindingFlags bindingAttr) => null;
            FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr) => Array.Empty<FieldInfo>();

            PropertyInfo? IReflect.GetProperty(string name, BindingFlags bindingAttr)
            {
                PropertyInfo? property = GetPrincipal()?.GetType().GetProperty(name, bindingAttr);
                property ??= GetType().GetProperty(name, bindingAttr);
                return property;
            }

            PropertyInfo? IReflect.GetProperty(
                string name,
                BindingFlags bindingAttr,
                Binder? binder,
                Type? returnType,
                Type[] types,
                ParameterModifier[]? modifiers)
            {
                PropertyInfo? property = GetPrincipal()?.GetType().GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
                property ??= GetType().GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
                return property;
            }

            PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr)
            {
                PropertyInfo[] extenderProperties = GetType().GetProperties(bindingAttr);
                PropertyInfo[]? controlProperties = GetPrincipal()?.GetType().GetProperties(bindingAttr);

                if (extenderProperties is null)
                {
                    return controlProperties ?? Array.Empty<PropertyInfo>();
                }
                else if (controlProperties is null)
                {
                    return extenderProperties ?? Array.Empty<PropertyInfo>();
                }
                else
                {
                    int i = 0;
                    PropertyInfo[] properties = new PropertyInfo[extenderProperties.Length + controlProperties.Length];

                    foreach (PropertyInfo property in extenderProperties)
                    {
                        properties[i++] = property;
                    }

                    foreach (PropertyInfo property in controlProperties)
                    {
                        properties[i++] = property;
                    }

                    return properties;
                }
            }

            MemberInfo[] IReflect.GetMember(string name, BindingFlags bindingAttr)
            {
                MemberInfo[]? member = GetPrincipal()?.GetType().GetMember(name, bindingAttr);
                member ??= GetType().GetMember(name, bindingAttr);
                return member;
            }

            MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr)
            {
                MemberInfo[] extenderMembers = GetType().GetMembers(bindingAttr);
                MemberInfo[]? controlMembers = GetPrincipal()?.GetType().GetMembers(bindingAttr);

                if (extenderMembers is null)
                {
                    return controlMembers ?? Array.Empty<MemberInfo>();
                }
                else if (controlMembers is null)
                {
                    return extenderMembers ?? Array.Empty<MemberInfo>();
                }
                else
                {
                    MemberInfo[] members = new MemberInfo[extenderMembers.Length + controlMembers.Length];
                    Array.Copy(extenderMembers, 0, members, 0, extenderMembers.Length);
                    Array.Copy(controlMembers, 0, members, extenderMembers.Length, controlMembers.Length);
                    return members;
                }
            }

            object? IReflect.InvokeMember(
                string name,
                BindingFlags invokeAttr,
                Binder? binder,
                object? target,
                object?[]? args,
                ParameterModifier[]? modifiers,
                CultureInfo? culture,
                string[]? namedParameters)
            {
                try
                {
                    return GetType().InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
                }
                catch (MissingMethodException)
                {
                    return GetPrincipal()?.GetType().InvokeMember(name, invokeAttr, binder, GetPrincipal(), args, modifiers, culture, namedParameters);
                }
            }

            Type IReflect.UnderlyingSystemType
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose("In UnderlyingSystemType");
                    return null!;
                }
            }
        }
    }
}
