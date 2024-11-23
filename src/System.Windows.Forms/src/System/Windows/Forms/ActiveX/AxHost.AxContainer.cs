// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;

namespace System.Windows.Forms;

public abstract partial class AxHost
{
    internal unsafe partial class AxContainer :
        UnknownDispatch,
        IOleContainer.Interface,
        IOleInPlaceFrame.Interface,
        IOleInPlaceUIWindow.Interface,
        IOleWindow.Interface,
        IManagedWrapper<IOleContainer, IOleInPlaceFrame, IOleInPlaceUIWindow, IOleWindow, IDispatch, IDispatchEx>
    {
        internal ContainerControl _parent;

        // The associated container may be null, in which case all this container does is
        // forward [de]activation messages to the requisite container.
        private IContainer? _associatedContainer;

        private AxHost? _siteUIActive;
        private AxHost? _siteActive;
        private bool _formAlreadyCreated;
        private readonly HashSet<AxHost> _containerCache = [];
        private int _lockCount;
        private HashSet<Control>? _components;
        private Dictionary<Control, ExtenderProxy>? _extenderCache;
        private AxHost? _controlInEditMode;
        private readonly Lock _lock = new();

        internal AxContainer(ContainerControl parent)
        {
            _parent = parent;
            if (parent.Created)
            {
                FormCreated();
            }
        }

        protected override unsafe HRESULT Invoke(
            int dispId,
            uint lcid,
            DISPATCH_FLAGS flags,
            DISPPARAMS* parameters,
            VARIANT* result,
            EXCEPINFO* exceptionInfo,
            uint* argumentError)
        {
            // This was provided via IReflect. No members were actually exposed via IReflect, so the only way this could
            // ever come in was as a [DISPID] string. Not clear if this was accidental or intentional.

            string name = $"[DISPID={dispId}]";
            foreach (AxHost control in _containerCache)
            {
                if (GetNameForControl(control).Equals(name))
                {
                    IExtender.Interface? extender = GetExtenderProxyForControl(control);
                    if (extender is null)
                    {
                        Debug.WriteLine($"No proxy for {name}, returning null");
                        result = default;
                    }
                    else
                    {
                        *result = (VARIANT)ComHelpers.GetComPointer<IUnknown>(extender);
                    }

                    return HRESULT.S_OK;
                }
            }

            return HRESULT.DISP_E_MEMBERNOTFOUND;
        }

        internal ExtenderProxy? GetExtenderProxyForControl(Control control)
        {
            ExtenderProxy? extender = null;
            if (_extenderCache is null)
            {
                _extenderCache = [];
            }
            else
            {
                _extenderCache.TryGetValue(control, out extender);
            }

            if (extender is null)
            {
                if (control != _parent && !GetComponents().Contains(control))
                {
                    AxContainer? container = FindContainerForControl(control);
                    if (container is not null)
                    {
                        extender = new ExtenderProxy(control, container);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    extender = new ExtenderProxy(control, this);
                }

                _extenderCache.Add(control, extender);
            }

            return extender;
        }

        internal static string GetNameForControl(Control control)
        {
            string? name = control.Site is { } site ? site.Name : control.Name;
            return name ?? string.Empty;
        }

        internal void AddControl(AxHost control)
        {
            lock (_lock)
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
            }
        }

        internal void RemoveControl(AxHost control)
        {
            lock (_lock)
            {
                _containerCache.Remove(control);
            }
        }

        internal IEnumUnknown.Interface EnumControls(Control control, uint dwOleContF, ENUM_CONTROLS_WHICH_FLAGS dwWhich)
        {
            GetComponents();
            _lockCount++;

            Control? additionalControl;

            try
            {
                // Results are IUnknown
                List<object>? results = null;

                bool selected = dwWhich.HasFlag(ENUM_CONTROLS_WHICH_FLAGS.GC_WCH_FSELECTED);
                bool reverse = dwWhich.HasFlag(ENUM_CONTROLS_WHICH_FLAGS.GC_WCH_FREVERSEDIR);

                // Note that Visual Basic actually ignores the next/prev flags. We will not.
                bool onlyNext = dwWhich.HasFlag(ENUM_CONTROLS_WHICH_FLAGS.GC_WCH_FONLYAFTER);
                bool onlyPrevious = dwWhich.HasFlag(ENUM_CONTROLS_WHICH_FLAGS.GC_WCH_FONLYBEFORE);

                dwWhich &= ~(ENUM_CONTROLS_WHICH_FLAGS.GC_WCH_FSELECTED
                    | ENUM_CONTROLS_WHICH_FLAGS.GC_WCH_FREVERSEDIR
                    | ENUM_CONTROLS_WHICH_FLAGS.GC_WCH_FONLYAFTER
                    | ENUM_CONTROLS_WHICH_FLAGS.GC_WCH_FONLYBEFORE);

                if (onlyNext && onlyPrevious)
                {
                    Debug.Fail("onlyNext && onlyPrevious are both set");
                    throw s_invalidArgumentException;
                }

                if (dwWhich is ENUM_CONTROLS_WHICH_FLAGS.GC_WCH_CONTAINER or ENUM_CONTROLS_WHICH_FLAGS.GC_WCH_CONTAINED && (onlyNext || onlyPrevious))
                {
                    Debug.Fail("GC_WCH_FONLYNEXT or FONLYPREV used with CONTAINER or CONTAINED");
                    throw s_invalidArgumentException;
                }

                int first = 0;
                int last = -1; // meaning all
                Control[] controls = [];
                switch (dwWhich)
                {
                    default:
                        Debug.Fail("Bad GC_WCH");
                        throw s_invalidArgumentException;
                    case ENUM_CONTROLS_WHICH_FLAGS.GC_WCH_CONTAINED:
                        controls = control.GetChildControlsInTabOrder(handleCreatedOnly: false);
                        additionalControl = null;
                        break;
                    case ENUM_CONTROLS_WHICH_FLAGS.GCW_WCH_SIBLING:
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
                    case ENUM_CONTROLS_WHICH_FLAGS.GC_WCH_CONTAINER:
                        results = [];
                        additionalControl = null;
                        MaybeAdd(results, control, selected, (OLECONTF)dwOleContF, allowContainingControls: false);

                        while (control is not null)
                        {
                            if (FindContainerForControl(control) is { } container)
                            {
                                MaybeAdd(results, container._parent, selected, (OLECONTF)dwOleContF, allowContainingControls: true);
                                control = container._parent;
                            }
                            else
                            {
                                break;
                            }
                        }

                        break;
                    case ENUM_CONTROLS_WHICH_FLAGS.GC_WCH_ALL:
                        controls = [.. GetComponents()];
                        additionalControl = _parent;
                        break;
                }

                if (results is null)
                {
                    results = [];
                    if (last == -1 && controls is not null)
                    {
                        last = controls.Length;
                    }

                    if (additionalControl is not null)
                    {
                        MaybeAdd(results, additionalControl, selected, (OLECONTF)dwOleContF, allowContainingControls: false);
                    }

                    if (controls is not null)
                    {
                        for (int i = first; i < last; i++)
                        {
                            MaybeAdd(results, controls[i], selected, (OLECONTF)dwOleContF, allowContainingControls: false);
                        }
                    }
                }

                if (reverse)
                {
                    results.Reverse();
                }

                return new EnumUnknown([.. results]);
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

            if (control is AxHost hostControl && flags.HasFlag(OLECONTF.OLECONTF_EMBEDDINGS))
            {
                controls.Add(hostControl.GetOcx()!);
            }
            else if (flags.HasFlag(OLECONTF.OLECONTF_OTHERS))
            {
                if (GetExtenderProxyForControl(control) is { } proxy)
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
                _components = [];
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

            if (_containerCache.Count > 0)
            {
                if (_components is null)
                {
                    _components = [];
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
                _components ??= [];

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
                    using var inPlaceObject = tempSite.GetComScope<IOleInPlaceObject>();
                    inPlaceObject.Value->UIDeactivate().AssertSuccess();
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
            _siteUIActive = site;
            if (site.ContainingControl is { } container)
            {
                container.ActiveControl = site;
            }
        }

        internal void ControlCreated(AxHost invoker)
        {
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

            List<AxHost> hostControls = [];
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

        HRESULT IParseDisplayName.Interface.ParseDisplayName(
            IBindCtx* pbc,
            PWSTR pszDisplayName,
            uint* pchEaten,
            IMoniker** ppmkOut)
            => ((IOleContainer.Interface)this).ParseDisplayName(pbc, pszDisplayName, pchEaten, ppmkOut);

        // IOleContainer methods:
        HRESULT IOleContainer.Interface.ParseDisplayName(
            IBindCtx* pbc,
            PWSTR pszDisplayName,
            uint* pchEaten,
            IMoniker** ppmkOut)
        {
            if (ppmkOut is not null)
            {
                *ppmkOut = null;
            }

            return HRESULT.E_NOTIMPL;
        }

        HRESULT IOleContainer.Interface.EnumObjects(uint grfFlags, IEnumUnknown** ppenum)
        {
            if (ppenum is null)
            {
                return HRESULT.E_POINTER;
            }

            if (((OLECONTF)grfFlags).HasFlag(OLECONTF.OLECONTF_EMBEDDINGS))
            {
                Debug.Assert(_parent is not null);

                List<object> oleControls = [];
                foreach (Control control in GetComponents())
                {
                    if (control is AxHost hostControl)
                    {
                        oleControls.Add(hostControl.GetOcx()!);
                    }
                }

                if (oleControls.Count > 0)
                {
                    *ppenum = ComHelpers.GetComPointer<IEnumUnknown>(new EnumUnknown([.. oleControls]));
                    return HRESULT.S_OK;
                }
            }

            *ppenum = ComHelpers.GetComPointer<IEnumUnknown>(new EnumUnknown(null));
            return HRESULT.S_OK;
        }

        HRESULT IOleContainer.Interface.LockContainer(BOOL fLock) => HRESULT.E_NOTIMPL;

        // IOleInPlaceFrame methods:
        HRESULT IOleInPlaceFrame.Interface.GetWindow(HWND* phwnd)
        {
            if (phwnd is null)
            {
                return HRESULT.E_POINTER;
            }

            *phwnd = _parent.HWND;
            return HRESULT.S_OK;
        }

        HRESULT IOleInPlaceFrame.Interface.ContextSensitiveHelp(BOOL fEnterMode) => HRESULT.S_OK;

        HRESULT IOleInPlaceFrame.Interface.GetBorder(RECT* lprectBorder) => HRESULT.E_NOTIMPL;

        HRESULT IOleInPlaceFrame.Interface.RequestBorderSpace(RECT* pborderwidths) => HRESULT.E_NOTIMPL;

        HRESULT IOleInPlaceFrame.Interface.SetBorderSpace(RECT* pborderwidths) => HRESULT.E_NOTIMPL;

        internal void OnExitEditMode(AxHost ctl)
        {
            Debug.Assert(_controlInEditMode is null || _controlInEditMode == ctl, "who is exiting edit mode?");
            if (_controlInEditMode is null || _controlInEditMode != ctl)
            {
                return;
            }

            _controlInEditMode = null;
        }

        HRESULT IOleInPlaceFrame.Interface.SetActiveObject(IOleInPlaceActiveObject* pActiveObject, PCWSTR pszObjName)
        {
            if (_siteUIActive is { } activeHost)
            {
                var existing = activeHost._iOleInPlaceActiveObjectExternal;
                activeHost._iOleInPlaceActiveObjectExternal = pActiveObject is null
                    ? null
                    : new(pActiveObject, takeOwnership: false);

                existing?.Dispose();
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

            using var oleObject = ComScope<IOleObject>.TryQueryFrom(pActiveObject, out HRESULT hr);
            if (hr.Failed)
            {
                return HRESULT.S_OK;
            }

            AxHost? host = null;
            using ComScope<IOleClientSite> clientSite = new(null);
            hr = oleObject.Value->GetClientSite(clientSite);
            Debug.Assert(hr.Succeeded);

            if (ComHelpers.TryGetObjectForIUnknown(clientSite.AsUnknown, takeOwnership: false, out OleInterfaces? interfaces))
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
                _controlInEditMode = null;
            }
            else
            {
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

        HRESULT IOleInPlaceFrame.Interface.InsertMenus(HMENU hmenuShared, OLEMENUGROUPWIDTHS* lpMenuWidths) => HRESULT.S_OK;

        HRESULT IOleInPlaceFrame.Interface.SetMenu(HMENU hmenuShared, nint holemenu, HWND hwndActiveObject) => HRESULT.E_NOTIMPL;

        HRESULT IOleInPlaceFrame.Interface.RemoveMenus(HMENU hmenuShared) => HRESULT.E_NOTIMPL;

        HRESULT IOleInPlaceFrame.Interface.SetStatusText(PCWSTR pszStatusText) => HRESULT.E_NOTIMPL;

        HRESULT IOleInPlaceFrame.Interface.EnableModeless(BOOL fEnable) => HRESULT.E_NOTIMPL;

        HRESULT IOleInPlaceFrame.Interface.TranslateAccelerator(MSG* lpmsg, ushort wID) => HRESULT.S_FALSE;

        HRESULT IOleInPlaceUIWindow.Interface.GetWindow(HWND* phwnd)
            => ((IOleInPlaceFrame.Interface)this).GetWindow(phwnd);

        HRESULT IOleInPlaceUIWindow.Interface.ContextSensitiveHelp(BOOL fEnterMode)
            => ((IOleInPlaceFrame.Interface)this).ContextSensitiveHelp(fEnterMode);

        HRESULT IOleInPlaceUIWindow.Interface.GetBorder(RECT* lprectBorder)
            => ((IOleInPlaceFrame.Interface)this).GetBorder(lprectBorder);

        HRESULT IOleInPlaceUIWindow.Interface.RequestBorderSpace(RECT* pborderwidths)
            => ((IOleInPlaceFrame.Interface)this).RequestBorderSpace(pborderwidths);

        HRESULT IOleInPlaceUIWindow.Interface.SetBorderSpace(RECT* pborderwidths)
            => ((IOleInPlaceFrame.Interface)this).SetBorderSpace(pborderwidths);

        HRESULT IOleInPlaceUIWindow.Interface.SetActiveObject(IOleInPlaceActiveObject* pActiveObject, PCWSTR pszObjName)
            => ((IOleInPlaceFrame.Interface)this).SetActiveObject(pActiveObject, pszObjName);

        HRESULT IOleWindow.Interface.GetWindow(HWND* phwnd)
            => ((IOleInPlaceFrame.Interface)this).GetWindow(phwnd);

        HRESULT IOleWindow.Interface.ContextSensitiveHelp(BOOL fEnterMode)
            => ((IOleInPlaceFrame.Interface)this).ContextSensitiveHelp(fEnterMode);

        protected override void Dispose(bool disposing)
        {
            if (_extenderCache is { } cache)
            {
                foreach (var extender in cache.Values)
                {
                    extender.Dispose();
                }

                cache.Clear();
            }

            base.Dispose(disposing);
        }
    }
}
