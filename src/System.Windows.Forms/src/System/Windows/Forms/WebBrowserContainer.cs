// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using static Interop;

namespace System.Windows.Forms
{
    internal class WebBrowserContainer : Ole32.IOleContainer, Ole32.IOleInPlaceFrame
    {
        private readonly WebBrowserBase parent;
        private IContainer assocContainer;  // associated IContainer...
                                            // the assocContainer may be null, in which case all this container does is
                                            // forward [de]activation messages to the requisite container...
        private WebBrowserBase siteUIActive;
        private WebBrowserBase siteActive;
        private readonly Hashtable containerCache = new Hashtable();  // name -> Control
        private Hashtable components;  // Control -> any
        private WebBrowserBase ctlInEditMode;

        internal WebBrowserContainer(WebBrowserBase parent)
        {
            this.parent = parent;
        }

        // IOleContainer methods:
        unsafe HRESULT Ole32.IOleContainer.ParseDisplayName(IntPtr pbc, string pszDisplayName, uint* pchEaten, IntPtr* ppmkOut)
        {
            if (ppmkOut != null)
            {
                *ppmkOut = IntPtr.Zero;
            }

            return HRESULT.E_NOTIMPL;
        }

        HRESULT Ole32.IOleContainer.EnumObjects(Ole32.OLECONTF grfFlags, out Ole32.IEnumUnknown ppenum)
        {
            ppenum = null;
            if ((grfFlags & Ole32.OLECONTF.EMBEDDINGS) != 0)
            {
                Debug.Assert(parent != null, "gotta have it...");
                ArrayList list = new ArrayList();
                ListAXControls(list, true);
                if (list.Count > 0)
                {
                    object[] temp = new object[list.Count];
                    list.CopyTo(temp, 0);
                    ppenum = new AxHost.EnumUnknown(temp);
                    return HRESULT.S_OK;
                }
            }

            ppenum = new AxHost.EnumUnknown(null);
            return HRESULT.S_OK;
        }

        HRESULT Ole32.IOleContainer.LockContainer(BOOL fLock)
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

        HRESULT Ole32.IOleInPlaceFrame.SetActiveObject(Ole32.IOleInPlaceActiveObject pActiveObject, string pszObjName)
        {
            if (pActiveObject is null)
            {
                if (ctlInEditMode != null)
                {
                    ctlInEditMode.SetEditMode(WebBrowserHelper.AXEditMode.None);
                    ctlInEditMode = null;
                }
                return HRESULT.S_OK;
            }
            WebBrowserBase ctl = null;
            if (pActiveObject is Ole32.IOleObject oleObject)
            {
                oleObject.GetClientSite(out Ole32.IOleClientSite clientSite);
                if (clientSite is WebBrowserSiteBase webBrowserSiteBase)
                {
                    ctl = webBrowserSiteBase.Host;
                }

                if (ctlInEditMode != null)
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

        unsafe HRESULT Ole32.IOleInPlaceFrame.TranslateAccelerator(User32.MSG* lpmsg, ushort wID)
        {
            return HRESULT.S_FALSE;
        }

        //
        // Private helper methods:
        //
        private void ListAXControls(ArrayList list, bool fuseOcx)
        {
            Hashtable components = GetComponents();
            if (components is null)
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
                    if (ctl is WebBrowserBase webBrowserBase)
                    {
                        if (fuseOcx)
                        {
                            object ax = webBrowserBase.activeXInstance;
                            if (ax != null)
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
        }

        private Hashtable GetComponents()
        {
            return GetComponents(GetParentsContainer());
        }

        private IContainer GetParentsContainer()
        {
            //
            IContainer rval = GetParentIContainer();
            Debug.Assert(rval is null || assocContainer is null || rval == assocContainer,
                         "mismatch between getIPD & aContainer");
            return rval ?? assocContainer;
        }

        private IContainer GetParentIContainer()
        {
            ISite site = parent.Site;
            if (site != null && site.DesignMode)
            {
                return site.Container;
            }

            return null;
        }

        private Hashtable GetComponents(IContainer cont)
        {
            FillComponentsTable(cont);
            return components;
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

            Debug.Assert(parent.Site is null, "Parent is sited but we could not find IContainer!!!");

            bool checkHashTable = true;
            Control[] ctls = new Control[containerCache.Values.Count];
            containerCache.Values.CopyTo(ctls, 0);
            if (ctls != null)
            {
                if (ctls.Length > 0 && components is null)
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
            if (ctl is null)
            {
                return;
            }

            if (components is null)
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

        private bool RegisterControl(WebBrowserBase ctl)
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

        //
        // Internal helper methods:
        //
        internal void AddControl(Control ctl)
        {
            if (containerCache.Contains(ctl))
            {
                throw new ArgumentException(string.Format(SR.AXDuplicateControl, GetNameForControl(ctl)), nameof(ctl));
            }

            containerCache.Add(ctl, ctl);

            if (assocContainer is null)
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
        }

        internal void RemoveControl(Control ctl)
        {
            //ctl may not be in containerCache: Remove is a no-op if it's not there.
            containerCache.Remove(ctl);
        }

        internal static WebBrowserContainer FindContainerForControl(WebBrowserBase ctl)
        {
            if (ctl != null)
            {
                if (ctl.container != null)
                {
                    return ctl.container;
                }
                ScrollableControl f = ctl.ContainingControl;
                if (f != null)
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

        internal string GetNameForControl(Control ctl)
        {
            string name = (ctl.Site != null) ? ctl.Site.Name : ctl.Name;
            return name ?? "";
        }

        internal void OnUIActivate(WebBrowserBase site)
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
                WebBrowserBase tempSite = siteUIActive;
                tempSite.AXInPlaceObject.UIDeactivate();
            }
            site.AddSelectionHandler();
            Debug.Assert(siteUIActive is null, "Object did not call OnUIDeactivate");
            siteUIActive = site;
            ContainerControl f = site.ContainingControl;
            if (f != null && f.Contains(site))
            {
                f.SetActiveControl(site);
            }
        }

        internal void OnUIDeactivate(WebBrowserBase site)
        {
#if DEBUG
            if (siteUIActive != null)
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
                if (parentContainer != null)
                {
                    parentContainer.SetActiveControl(null);
                }
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
