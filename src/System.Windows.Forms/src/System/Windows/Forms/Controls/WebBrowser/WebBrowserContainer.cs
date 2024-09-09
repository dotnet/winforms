// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms;

internal unsafe class WebBrowserContainer : IOleContainer.Interface, IOleInPlaceFrame.Interface, IOleInPlaceUIWindow.Interface, IOleWindow.Interface
{
    private readonly WebBrowserBase _parent;

    /// <summary>
    ///  May be null, in which case all this container does is forward [de]activation messages to the requisite container.
    /// </summary>
    private IContainer? _associatedContainer;
    private WebBrowserBase? _siteUIActive;
    private WebBrowserBase? _siteActive;
    private readonly HashSet<Control> _containerCache = [];
    private HashSet<Control>? _components;
    private WebBrowserBase? _controlInEditMode;

    internal WebBrowserContainer(WebBrowserBase parent) => _parent = parent;

    HRESULT IParseDisplayName.Interface.ParseDisplayName(IBindCtx* pbc, PWSTR pszDisplayName, uint* pchEaten, IMoniker** ppmkOut) =>
        ((IOleContainer.Interface)this).ParseDisplayName(pbc, pszDisplayName, pchEaten, ppmkOut);

    // IOleContainer methods:
    HRESULT IOleContainer.Interface.ParseDisplayName(IBindCtx* pbc, PWSTR pszDisplayName, uint* pchEaten, IMoniker** ppmkOut)
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
            List<object> list = [];
            ListAXControls(list, true);
            if (list.Count > 0)
            {
                object[] temp = new object[list.Count];
                list.CopyTo(temp, 0);
                *ppenum = ComHelpers.GetComPointer<IEnumUnknown>(new AxHost.EnumUnknown(temp));
                return HRESULT.S_OK;
            }
        }

        *ppenum = ComHelpers.GetComPointer<IEnumUnknown>(new AxHost.EnumUnknown(null));
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

    HRESULT IOleInPlaceFrame.Interface.SetActiveObject(IOleInPlaceActiveObject* pActiveObject, PCWSTR pszObjName)
    {
        if (pActiveObject is null)
        {
            if (_controlInEditMode is not null)
            {
                _controlInEditMode.SetEditMode(WebBrowserHelper.AXEditMode.None);
                _controlInEditMode = null;
            }

            return HRESULT.S_OK;
        }

        WebBrowserBase? control = null;

        using var oleObject = ComScope<IOleObject>.TryQueryFrom(pActiveObject, out HRESULT hr);
        if (hr.Failed)
        {
            return HRESULT.S_OK;
        }

        IOleClientSite* clientSite;
        oleObject.Value->GetClientSite(&clientSite);
        object clientSiteObject = ComHelpers.GetObjectForIUnknown(clientSite);
        if (clientSiteObject is WebBrowserSiteBase webBrowserSiteBase)
        {
            control = webBrowserSiteBase.Host;
        }

        if (_controlInEditMode is not null)
        {
            Debug.Fail($"Control {_controlInEditMode} did not reset its edit mode to null.");
            _controlInEditMode.SetSelectionStyle(WebBrowserHelper.SelectionStyle.Selected);
            _controlInEditMode.SetEditMode(WebBrowserHelper.AXEditMode.None);
        }

        if (control is null)
        {
            _controlInEditMode = null;
        }
        else
        {
            if (!control.IsUserMode)
            {
                _controlInEditMode = control;
                control.SetEditMode(WebBrowserHelper.AXEditMode.Object);
                control.AddSelectionHandler();
                control.SetSelectionStyle(WebBrowserHelper.SelectionStyle.Active);
            }
        }

        return HRESULT.S_OK;
    }

    HRESULT IOleInPlaceFrame.Interface.InsertMenus(HMENU hmenuShared, OLEMENUGROUPWIDTHS* lpMenuWidths)
        => HRESULT.S_OK;

    HRESULT IOleInPlaceFrame.Interface.SetMenu(HMENU hmenuShared, nint holemenu, HWND hwndActiveObject)
        => HRESULT.E_NOTIMPL;

    HRESULT IOleInPlaceFrame.Interface.RemoveMenus(HMENU hmenuShared) => HRESULT.E_NOTIMPL;

    HRESULT IOleInPlaceFrame.Interface.SetStatusText(PCWSTR pszStatusText) => HRESULT.E_NOTIMPL;

    HRESULT IOleInPlaceFrame.Interface.EnableModeless(BOOL fEnable) => HRESULT.E_NOTIMPL;

    HRESULT IOleInPlaceFrame.Interface.TranslateAccelerator(MSG* lpmsg, ushort wID) => HRESULT.S_FALSE;

    //
    // Private helper methods:
    //
    private void ListAXControls(List<object> list, bool fuseOcx)
    {
        HashSet<Control>? components = GetComponents();
        if (components is null)
        {
            return;
        }

        foreach (Control ctl in components.ToArray())
        {
            if (ctl is WebBrowserBase webBrowserBase)
            {
                if (fuseOcx)
                {
                    object? activeX = webBrowserBase._activeXInstance;
                    if (activeX is not null)
                    {
                        list.Add(activeX);
                    }
                }
                else
                {
                    list.Add(ctl);
                }
            }
        }
    }

    private HashSet<Control>? GetComponents()
    {
        FillComponentsTable(GetParentsContainer());
        return _components;
    }

    private IContainer? GetParentsContainer()
    {
        IContainer? rval = GetParentIContainer();
        Debug.Assert(rval is null || _associatedContainer is null || rval == _associatedContainer,
                     "mismatch between getIPD & aContainer");
        return rval ?? _associatedContainer;
    }

    private IContainer? GetParentIContainer()
    {
        ISite? site = _parent.Site;
        return site is not null && site.DesignMode ? site.Container : null;
    }

    private void FillComponentsTable(IContainer? container)
    {
        if (container is not null)
        {
            ComponentCollection comps = container.Components;
            if (comps is not null)
            {
                _components = [];
                foreach (IComponent comp in comps)
                {
                    if (comp is Control ctrl && comp != _parent && comp.Site is not null)
                    {
                        _components.Add(ctrl);
                    }
                }

                return;
            }
        }

        Debug.Assert(_parent.Site is null, "Parent is sited but we could not find IContainer!!!");

        bool checkHashSet = true;
        Control[] ctls = [.. _containerCache];
        if (ctls is not null)
        {
            if (ctls.Length > 0 && _components is null)
            {
                _components = [];
                checkHashSet = false;
            }

            for (int i = 0; i < ctls.Length; i++)
            {
                if (checkHashSet)
                {
                    _components?.Add(ctls[i]);
                }
            }
        }

        GetAllChildren(_parent);
    }

    private void GetAllChildren(Control ctl)
    {
        if (ctl is null)
        {
            return;
        }

        _components ??= [];

        if (ctl != _parent && !_components.Contains(ctl))
        {
            _components.Add(ctl);
        }

        foreach (Control c in ctl.Controls)
        {
            GetAllChildren(c);
        }
    }

    private bool RegisterControl(WebBrowserBase ctl)
    {
        if (ctl.Site is not ISite site || site.Container is not IContainer container)
        {
            return false;
        }

        if (_associatedContainer is not null)
        {
            return container == _associatedContainer;
        }

        _associatedContainer = container;
        if (site.TryGetService(out IComponentChangeService? service))
        {
            service.ComponentRemoved += OnComponentRemoved;
        }

        return true;
    }

    private void OnComponentRemoved(object? sender, ComponentEventArgs e)
    {
        if (sender == _associatedContainer && e.Component is Control c)
        {
            RemoveControl(c);
        }
    }

    //
    // Internal helper methods:
    //
    internal void AddControl(Control ctl)
    {
        if (_containerCache.Contains(ctl))
        {
            throw new ArgumentException(string.Format(SR.AXDuplicateControl, GetNameForControl(ctl)), nameof(ctl));
        }

        _containerCache.Add(ctl);

        if (_associatedContainer is not null || ctl.Site is not ISite site)
        {
            return;
        }

        _associatedContainer = site.Container;
        if (site.TryGetService(out IComponentChangeService? service))
        {
            service.ComponentRemoved += OnComponentRemoved;
        }
    }

    internal void RemoveControl(Control ctl)
    {
        // ctl may not be in containerCache: Remove is a no-op if it's not there.
        _containerCache.Remove(ctl);
    }

    internal static WebBrowserContainer? FindContainerForControl(WebBrowserBase ctl)
    {
        if (ctl is not null)
        {
            if (ctl._container is not null)
            {
                return ctl._container;
            }

            ScrollableControl? containingControl = ctl.ContainingControl;
            if (containingControl is not null)
            {
                WebBrowserContainer container = ctl.CreateWebBrowserContainer();
                if (container.RegisterControl(ctl))
                {
                    container.AddControl(ctl);
                    return container;
                }
            }
        }

        return null;
    }

    internal static string GetNameForControl(Control ctl)
        => ctl.Site is not null ? ctl.Site.Name ?? string.Empty : ctl.Name ?? string.Empty;

    internal void OnUIActivate(WebBrowserBase site)
    {
        // The ShDocVw control repeatedly calls OnUIActivate() with the same
        // site. This causes the assert below to fire.
        //
        if (_siteUIActive == site)
        {
            return;
        }

        if (_siteUIActive is not null && _siteUIActive != site)
        {
            WebBrowserBase tempSite = _siteUIActive;
            tempSite.AXInPlaceObject!.UIDeactivate();
        }

        site.AddSelectionHandler();
        Debug.Assert(_siteUIActive is null, "Object did not call OnUIDeactivate");
        _siteUIActive = site;
        ContainerControl? containingControl = site.ContainingControl;
        if (containingControl is not null && containingControl.Contains(site))
        {
            containingControl.SetActiveControl(site);
        }
    }

    internal void OnUIDeactivate(WebBrowserBase site)
    {
#if DEBUG
        if (_siteUIActive is not null)
        {
            Debug.Assert(_siteUIActive == site, "deactivating when not active...");
        }
#endif // DEBUG

        _siteUIActive = null;
        site.RemoveSelectionHandler();
        site.SetSelectionStyle(WebBrowserHelper.SelectionStyle.Selected);
        site.SetEditMode(WebBrowserHelper.AXEditMode.None);
    }

    internal void OnInPlaceDeactivate(WebBrowserBase site)
    {
        if (_siteActive == site)
        {
            _siteActive = null;
            ContainerControl? parentContainer = _parent.FindContainerControlInternal();
            parentContainer?.SetActiveControl(null);
        }
    }

    internal void OnExitEditMode(WebBrowserBase ctl)
    {
        Debug.Assert(_controlInEditMode is null || _controlInEditMode == ctl, "who is exiting edit mode?");
        if (_controlInEditMode == ctl)
        {
            _controlInEditMode = null;
        }
    }

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
}
