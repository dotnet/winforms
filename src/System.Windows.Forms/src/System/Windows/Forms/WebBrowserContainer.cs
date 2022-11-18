// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using static Interop;

namespace System.Windows.Forms
{
    internal class WebBrowserContainer : IOleContainer.Interface, Ole32.IOleInPlaceFrame
    {
        private readonly WebBrowserBase parent;
        private IContainer? assocContainer;  // associated IContainer...
                                             // the assocContainer may be null, in which case all this container does is
                                             // forward [de]activation messages to the requisite container...
        private WebBrowserBase? siteUIActive;
        private WebBrowserBase? siteActive;
        private readonly HashSet<Control> containerCache = new();
        private HashSet<Control>? components;
        private WebBrowserBase? ctlInEditMode;

        internal WebBrowserContainer(WebBrowserBase parent)
        {
            this.parent = parent;
        }

        unsafe HRESULT IParseDisplayName.Interface.ParseDisplayName(IBindCtx* pbc, PWSTR pszDisplayName, uint* pchEaten, IMoniker** ppmkOut)
           => ((IOleContainer.Interface)this).ParseDisplayName(pbc, pszDisplayName, pchEaten, ppmkOut);

        // IOleContainer methods:
        unsafe HRESULT IOleContainer.Interface.ParseDisplayName(IBindCtx* pbc, PWSTR pszDisplayName, uint* pchEaten, IMoniker** ppmkOut)
        {
            if (ppmkOut is not null)
            {
                *ppmkOut = null;
            }

            return HRESULT.E_NOTIMPL;
        }

        unsafe HRESULT IOleContainer.Interface.EnumObjects(OLECONTF grfFlags, IEnumUnknown** ppenum)
        {
            if (ppenum is null)
            {
                return HRESULT.E_POINTER;
            }

            if ((grfFlags & OLECONTF.OLECONTF_EMBEDDINGS) != 0)
            {
                Debug.Assert(parent is not null, "gotta have it...");
                List<object> list = new();
                ListAXControls(list, true);
                if (list.Count > 0)
                {
                    object[] temp = new object[list.Count];
                    list.CopyTo(temp, 0);
                    bool hr = ComHelpers.TryGetComPointer(new AxHost.EnumUnknown(temp), out *ppenum);
                    Debug.Assert(hr);
                    return HRESULT.S_OK;
                }
            }

            bool result = ComHelpers.TryGetComPointer(new AxHost.EnumUnknown(null), out *ppenum);
            Debug.Assert(result);
            return HRESULT.S_OK;
        }

        HRESULT IOleContainer.Interface.LockContainer(BOOL fLock)
        {
            return HRESULT.E_NOTIMPL;
        }

        // IOleInPlaceFrame methods:
        unsafe HRESULT Ole32.IOleInPlaceFrame.GetWindow(IntPtr* phwnd)
        {
            if (phwnd is null)
            {
                return HRESULT.E_POINTER;
            }

            *phwnd = parent.Handle;
            return HRESULT.S_OK;
        }

        HRESULT Ole32.IOleInPlaceFrame.ContextSensitiveHelp(BOOL fEnterMode)
        {
            return HRESULT.S_OK;
        }

        unsafe HRESULT Ole32.IOleInPlaceFrame.GetBorder(RECT* lprectBorder)
        {
            return HRESULT.E_NOTIMPL;
        }

        unsafe HRESULT Ole32.IOleInPlaceFrame.RequestBorderSpace(RECT* pborderwidths)
        {
            return HRESULT.E_NOTIMPL;
        }

        unsafe HRESULT Ole32.IOleInPlaceFrame.SetBorderSpace(RECT* pborderwidths)
        {
            return HRESULT.E_NOTIMPL;
        }

        unsafe HRESULT Ole32.IOleInPlaceFrame.SetActiveObject(IOleInPlaceActiveObject.Interface? pActiveObject, string? pszObjName)
        {
            if (pActiveObject is null)
            {
                if (ctlInEditMode is not null)
                {
                    ctlInEditMode.SetEditMode(WebBrowserHelper.AXEditMode.None);
                    ctlInEditMode = null;
                }

                return HRESULT.S_OK;
            }

            WebBrowserBase? ctl = null;
            if (pActiveObject is IOleObject.Interface oleObject)
            {
                IOleClientSite* clientSite;
                oleObject.GetClientSite(&clientSite);
                var clientSiteObject = Marshal.GetObjectForIUnknown((nint)clientSite);
                if (clientSiteObject is WebBrowserSiteBase webBrowserSiteBase)
                {
                    ctl = webBrowserSiteBase.Host;
                }

                if (ctlInEditMode is not null)
                {
                    Debug.Fail("control " + ctlInEditMode.ToString() + " did not reset its edit mode to null");
                    ctlInEditMode.SetSelectionStyle(WebBrowserHelper.SelectionStyle.Selected);
                    ctlInEditMode.SetEditMode(WebBrowserHelper.AXEditMode.None);
                }

                if (ctl is null)
                {
                    ctlInEditMode = null;
                }
                else
                {
                    if (!ctl.IsUserMode)
                    {
                        ctlInEditMode = ctl;
                        ctl.SetEditMode(WebBrowserHelper.AXEditMode.Object);
                        ctl.AddSelectionHandler();
                        ctl.SetSelectionStyle(WebBrowserHelper.SelectionStyle.Active);
                    }
                }
            }

            return HRESULT.S_OK;
        }

        unsafe HRESULT Ole32.IOleInPlaceFrame.InsertMenus(IntPtr hmenuShared, Ole32.OLEMENUGROUPWIDTHS* lpMenuWidths)
        {
            return HRESULT.S_OK;
        }

        HRESULT Ole32.IOleInPlaceFrame.SetMenu(IntPtr hmenuShared, IntPtr holemenu, IntPtr hwndActiveObject)
        {
            return HRESULT.E_NOTIMPL;
        }

        HRESULT Ole32.IOleInPlaceFrame.RemoveMenus(IntPtr hmenuShared)
        {
            return HRESULT.E_NOTIMPL;
        }

        HRESULT Ole32.IOleInPlaceFrame.SetStatusText(string pszStatusText)
        {
            return HRESULT.E_NOTIMPL;
        }

        HRESULT Ole32.IOleInPlaceFrame.EnableModeless(BOOL fEnable)
        {
            return HRESULT.E_NOTIMPL;
        }

        unsafe HRESULT Ole32.IOleInPlaceFrame.TranslateAccelerator(MSG* lpmsg, ushort wID)
        {
            return HRESULT.S_FALSE;
        }

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
                        object ax = webBrowserBase.activeXInstance;
                        if (ax is not null)
                        {
                            list.Add(ax);
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
            return components;
        }

        private IContainer? GetParentsContainer()
        {
            IContainer? rval = GetParentIContainer();
            Debug.Assert(rval is null || assocContainer is null || rval == assocContainer,
                         "mismatch between getIPD & aContainer");
            return rval ?? assocContainer;
        }

        private IContainer? GetParentIContainer()
        {
            ISite site = parent.Site;
            return site is not null && site.DesignMode ? site.Container : null;
        }

        private void FillComponentsTable(IContainer? container)
        {
            if (container is not null)
            {
                ComponentCollection comps = container.Components;
                if (comps is not null)
                {
                    components = new();
                    foreach (IComponent comp in comps)
                    {
                        if (comp is Control ctrl && comp != parent && comp.Site is not null)
                        {
                            components.Add(ctrl);
                        }
                    }

                    return;
                }
            }

            Debug.Assert(parent.Site is null, "Parent is sited but we could not find IContainer!!!");

            bool checkHashSet = true;
            Control[] ctls = containerCache.ToArray();
            if (ctls is not null)
            {
                if (ctls.Length > 0 && components is null)
                {
                    components = new();
                    checkHashSet = false;
                }

                for (int i = 0; i < ctls.Length; i++)
                {
                    if (checkHashSet)
                    {
                        components?.Add(ctls[i]);
                    }
                }
            }

            GetAllChildren(parent);
        }

        private void GetAllChildren(Control ctl)
        {
            if (ctl is null)
            {
                return;
            }

            components ??= new();

            if (ctl != parent && !components.Contains(ctl))
            {
                components.Add(ctl);
            }

            foreach (Control c in ctl.Controls)
            {
                GetAllChildren(c);
            }
        }

        private bool RegisterControl(WebBrowserBase ctl)
        {
            if (ctl.Site is ISite site)
            {
                if (site.Container is IContainer cont)
                {
                    if (assocContainer is not null)
                    {
                        return cont == assocContainer;
                    }
                    else
                    {
                        assocContainer = cont;
                        if (site.GetService(typeof(IComponentChangeService)) is IComponentChangeService ccs)
                        {
                            ccs.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        private void OnComponentRemoved(object? sender, ComponentEventArgs e)
        {
            if (sender == assocContainer && e.Component is Control c)
            {
                RemoveControl(c);
            }
        }

        //
        // Internal helper methods:
        //
        internal void AddControl(Control ctl)
        {
            if (containerCache.Contains(ctl))
            {
                throw new ArgumentException(string.Format(SR.AXDuplicateControl, GetNameForControl(ctl)), nameof(ctl));
            }

            containerCache.Add(ctl);

            if (assocContainer is null)
            {
                if (ctl.Site is ISite site)
                {
                    assocContainer = site.Container;
                    if (site.GetService(typeof(IComponentChangeService)) is IComponentChangeService ccs)
                    {
                        ccs.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
                    }
                }
            }
        }

        internal void RemoveControl(Control ctl)
        {
            //ctl may not be in containerCache: Remove is a no-op if it's not there.
            containerCache.Remove(ctl);
        }

        internal static WebBrowserContainer? FindContainerForControl(WebBrowserBase ctl)
        {
            if (ctl is not null)
            {
                if (ctl.container is not null)
                {
                    return ctl.container;
                }

                ScrollableControl f = ctl.ContainingControl;
                if (f is not null)
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
            if (siteUIActive == site)
            {
                return;
            }

            if (siteUIActive is not null && siteUIActive != site)
            {
                WebBrowserBase tempSite = siteUIActive;
                tempSite.AXInPlaceObject.UIDeactivate();
            }

            site.AddSelectionHandler();
            Debug.Assert(siteUIActive is null, "Object did not call OnUIDeactivate");
            siteUIActive = site;
            ContainerControl f = site.ContainingControl;
            if (f is not null && f.Contains(site))
            {
                f.SetActiveControl(site);
            }
        }

        internal void OnUIDeactivate(WebBrowserBase site)
        {
#if DEBUG
            if (siteUIActive is not null)
            {
                Debug.Assert(siteUIActive == site, "deactivating when not active...");
            }
#endif // DEBUG

            siteUIActive = null;
            site.RemoveSelectionHandler();
            site.SetSelectionStyle(WebBrowserHelper.SelectionStyle.Selected);
            site.SetEditMode(WebBrowserHelper.AXEditMode.None);
        }

        internal void OnInPlaceDeactivate(WebBrowserBase site)
        {
            if (siteActive == site)
            {
                siteActive = null;
                ContainerControl parentContainer = parent.FindContainerControlInternal();
                parentContainer?.SetActiveControl(null);
            }
        }

        internal void OnExitEditMode(WebBrowserBase ctl)
        {
            Debug.Assert(ctlInEditMode is null || ctlInEditMode == ctl, "who is exiting edit mode?");
            if (ctlInEditMode == ctl)
            {
                ctlInEditMode = null;
            }
        }
    }
}
