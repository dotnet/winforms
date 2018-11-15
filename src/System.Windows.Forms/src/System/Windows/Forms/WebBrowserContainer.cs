// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Diagnostics;
using System;
using System.Reflection;
using System.Globalization;
using System.Security.Permissions;
using System.Collections;
using System.Drawing;    
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.Security;

namespace System.Windows.Forms {
    internal class WebBrowserContainer : UnsafeNativeMethods.IOleContainer, UnsafeNativeMethods.IOleInPlaceFrame {
        //
        // Private fields:
        //
        private WebBrowserBase parent;
        private IContainer assocContainer;  // associated IContainer...
                                            // the assocContainer may be null, in which case all this container does is
                                            // forward [de]activation messages to the requisite container...
        private WebBrowserBase siteUIActive;
        private WebBrowserBase siteActive;
        private Hashtable containerCache = new Hashtable();  // name -> Control
        private Hashtable components = null;  // Control -> any
        private WebBrowserBase ctlInEditMode = null;

        internal WebBrowserContainer(WebBrowserBase parent) {
            this.parent = parent;
        }

        //
        // IOleContainer methods:
        //
        int UnsafeNativeMethods.IOleContainer.ParseDisplayName(Object pbc, string pszDisplayName, int[] pchEaten, Object[] ppmkOut) {
            if (ppmkOut != null)
                ppmkOut[0] = null;
             return NativeMethods.E_NOTIMPL;
        }

        int UnsafeNativeMethods.IOleContainer.EnumObjects(int grfFlags, out UnsafeNativeMethods.IEnumUnknown ppenum) {
            ppenum = null;
            if ((grfFlags & 1) != 0) { // 1 == OLECONTF_EMBEDDINGS
                Debug.Assert(parent != null, "gotta have it...");
                ArrayList list = new ArrayList();
                ListAXControls(list, true);
                if (list.Count > 0) {
                    Object[] temp = new Object[list.Count];
                    list.CopyTo(temp, 0);
                    ppenum = new AxHost.EnumUnknown(temp);
                    return NativeMethods.S_OK;
                }
            }
            ppenum = new AxHost.EnumUnknown(null);
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleContainer.LockContainer(bool fLock) {
            return NativeMethods.E_NOTIMPL;
        }

        //
        // IOleInPlaceFrame methods:
        //
        IntPtr UnsafeNativeMethods.IOleInPlaceFrame.GetWindow() {
            return parent.Handle;
        }

        int UnsafeNativeMethods.IOleInPlaceFrame.ContextSensitiveHelp(int fEnterMode) {
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleInPlaceFrame.GetBorder(NativeMethods.COMRECT lprectBorder) {
            return NativeMethods.E_NOTIMPL;
        }

        int UnsafeNativeMethods.IOleInPlaceFrame.RequestBorderSpace(NativeMethods.COMRECT pborderwidths) {
            return NativeMethods.E_NOTIMPL;
        }

        int UnsafeNativeMethods.IOleInPlaceFrame.SetBorderSpace(NativeMethods.COMRECT pborderwidths) {
            return NativeMethods.E_NOTIMPL;
        }

        int UnsafeNativeMethods.IOleInPlaceFrame.SetActiveObject(UnsafeNativeMethods.IOleInPlaceActiveObject pActiveObject, string pszObjName) {
            if (pActiveObject == null) {
                if (ctlInEditMode != null) {
                    ctlInEditMode.SetEditMode(WebBrowserHelper.AXEditMode.None);
                    ctlInEditMode = null;
                }
                return NativeMethods.S_OK;
            }
            WebBrowserBase ctl = null;
            UnsafeNativeMethods.IOleObject oleObject = pActiveObject as UnsafeNativeMethods.IOleObject;
            if (oleObject != null) {
                UnsafeNativeMethods.IOleClientSite clientSite = null;
                try {
                    clientSite = oleObject.GetClientSite();
                    WebBrowserSiteBase webBrowserSiteBase = clientSite as WebBrowserSiteBase;
                    if (webBrowserSiteBase != null)
                    {
                        ctl = webBrowserSiteBase.GetAXHost();
                    }
                }
                catch (COMException t) {
                    Debug.Fail(t.ToString());
                }
                if (ctlInEditMode != null) {
                    Debug.Fail("control " + ctlInEditMode.ToString() + " did not reset its edit mode to null");
                    ctlInEditMode.SetSelectionStyle(WebBrowserHelper.SelectionStyle.Selected);
                    ctlInEditMode.SetEditMode(WebBrowserHelper.AXEditMode.None);
                }
                
                if (ctl == null) {
                    ctlInEditMode = null;
                }
                else {
                    if (!ctl.IsUserMode) {
                        ctlInEditMode = ctl;
                        ctl.SetEditMode(WebBrowserHelper.AXEditMode.Object);
                        ctl.AddSelectionHandler();
                        ctl.SetSelectionStyle(WebBrowserHelper.SelectionStyle.Active);
                    }
                }
            }

            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleInPlaceFrame.InsertMenus(IntPtr hmenuShared, NativeMethods.tagOleMenuGroupWidths lpMenuWidths) {
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleInPlaceFrame.SetMenu(IntPtr hmenuShared, IntPtr holemenu, IntPtr hwndActiveObject) {
            return NativeMethods.E_NOTIMPL;
        }

        int UnsafeNativeMethods.IOleInPlaceFrame.RemoveMenus(IntPtr hmenuShared) {
            return NativeMethods.E_NOTIMPL;
        }

        int UnsafeNativeMethods.IOleInPlaceFrame.SetStatusText(string pszStatusText) {
            return NativeMethods.E_NOTIMPL;
        }

        int UnsafeNativeMethods.IOleInPlaceFrame.EnableModeless(bool fEnable) {
            return NativeMethods.E_NOTIMPL;
        }

        int UnsafeNativeMethods.IOleInPlaceFrame.TranslateAccelerator(ref NativeMethods.MSG lpmsg, short wID) {
            return NativeMethods.S_FALSE;
        }


        //
        // Private helper methods:
        //
        private void ListAXControls(ArrayList list, bool fuseOcx) {
            Hashtable components = GetComponents();
            if (components == null)
            {
                return;
            }
            Control[] ctls = new Control[components.Keys.Count];
            components.Keys.CopyTo(ctls, 0);
            if (ctls != null) {
                for (int i = 0; i < ctls.Length; i++) {
                    Control ctl = ctls[i];
                    WebBrowserBase webBrowserBase = ctl as WebBrowserBase;
                    if (webBrowserBase != null) {
                        if (fuseOcx) {
                            object ax = webBrowserBase.activeXInstance;
                            if (ax != null) {
                                list.Add(ax);
                            }
                        }
                        else {
                            list.Add(ctl);
                        }
                    }
                }
            }
        }

        private Hashtable GetComponents() {
            return GetComponents(GetParentsContainer());
        }

        private IContainer GetParentsContainer() {
            //
            IContainer rval = GetParentIContainer();
            Debug.Assert(rval == null || assocContainer == null || rval == assocContainer,
                         "mismatch between getIPD & aContainer");
            return rval == null ? assocContainer : rval;
        }

        private IContainer GetParentIContainer() {
            ISite site = parent.Site;
            if (site != null && site.DesignMode) return site.Container;
            return null;
        }

        private Hashtable GetComponents(IContainer cont) {
            FillComponentsTable(cont);
            return components;
        }

        private void FillComponentsTable(IContainer container) {
            if (container != null) {
                ComponentCollection comps = container.Components;
                if (comps != null) {
                    components = new Hashtable();
                    foreach (IComponent comp in comps) {
                        if (comp is Control && comp != parent && comp.Site != null) {
                            components.Add(comp, comp);
                        }
                    }
                    return;
                }
            }

            Debug.Assert(parent.Site == null, "Parent is sited but we could not find IContainer!!!");

            bool checkHashTable = true;
            Control[] ctls = new Control[containerCache.Values.Count];
            containerCache.Values.CopyTo(ctls, 0);
            if (ctls != null) {
                if (ctls.Length > 0 && components == null) {
                    components = new Hashtable();
                    checkHashTable = false;
                }
                for (int i = 0; i < ctls.Length; i ++) {
                    if (checkHashTable && !components.Contains(ctls[i])) {
                        components.Add(ctls[i], ctls[i]);
                    }
                }
            }

            GetAllChildren(this.parent);
        }

        private void GetAllChildren(Control ctl) {
            if (ctl == null)
                return;

            if (components == null) {
                components = new Hashtable();
            }

            if (ctl != this.parent && !components.Contains(ctl))
                components.Add(ctl, ctl);

            foreach(Control c in ctl.Controls) {
                GetAllChildren(c);
            }
        }

        private bool RegisterControl(WebBrowserBase ctl) {
            ISite site = ctl.Site;
            if (site != null) {
                IContainer cont = site.Container;
                if (cont != null) {
                    if (assocContainer != null) {
                        return cont == assocContainer;
                    }
                    else {
                        assocContainer = cont;
                        IComponentChangeService ccs = (IComponentChangeService)site.GetService(typeof(IComponentChangeService));
                        if (ccs != null) {
                            ccs.ComponentRemoved += new ComponentEventHandler(this.OnComponentRemoved);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        private void OnComponentRemoved(object sender, ComponentEventArgs e) {
            Control c = e.Component as Control;
            if (sender == assocContainer && c != null) {
                RemoveControl(c);
            }
        }

        //
        // Internal helper methods:
        //
        internal void AddControl(Control ctl) {
            if (containerCache.Contains(ctl))
                throw new ArgumentException(string.Format(SR.AXDuplicateControl, GetNameForControl(ctl)), "ctl");

            containerCache.Add(ctl, ctl);
                
            if (assocContainer == null) {
                ISite site = ctl.Site;
                if (site != null) {
                    assocContainer = site.Container;
                    IComponentChangeService ccs = (IComponentChangeService)site.GetService(typeof(IComponentChangeService));
                    if (ccs != null) {
                        ccs.ComponentRemoved += new ComponentEventHandler(this.OnComponentRemoved);
                    }
                }
            }
        }

        internal void RemoveControl(Control ctl) {
            //ctl may not be in containerCache: Remove is a no-op if it's not there.
            containerCache.Remove(ctl);
        }

        internal static WebBrowserContainer FindContainerForControl(WebBrowserBase ctl) {
            if (ctl != null) {
                if (ctl.container != null)
                {
                    return ctl.container;
                }
                ScrollableControl f = ctl.ContainingControl;
                if (f != null) {
                    WebBrowserContainer container = ctl.CreateWebBrowserContainer();
                    if (container.RegisterControl(ctl)) {
                        container.AddControl(ctl);
                        return container;
                    }
                }
            }
            return null;
        }

        internal string GetNameForControl(Control ctl) {
            string name = (ctl.Site != null) ? ctl.Site.Name : ctl.Name;
            return name ?? "";
        }

        internal void OnUIActivate(WebBrowserBase site) {
            // The ShDocVw control repeatedly calls OnUIActivate() with the same
            // site. This causes the assert below to fire.
            //
            if (siteUIActive == site)
                return;

            if (siteUIActive != null && siteUIActive != site) {
                WebBrowserBase tempSite = siteUIActive;
                tempSite.AXInPlaceObject.UIDeactivate();
            }
            site.AddSelectionHandler();
            Debug.Assert(siteUIActive == null, "Object did not call OnUIDeactivate");
            siteUIActive = site;
            ContainerControl f = site.ContainingControl;
            if (f != null && f.Contains(site)) {
                f.SetActiveControlInternal(site);
            }
        }

        internal void OnUIDeactivate(WebBrowserBase site) {
#if DEBUG
            if (siteUIActive != null) {
                Debug.Assert(siteUIActive == site, "deactivating when not active...");
            }
#endif // DEBUG

            siteUIActive = null;
            site.RemoveSelectionHandler();
            site.SetSelectionStyle(WebBrowserHelper.SelectionStyle.Selected);
            site.SetEditMode(WebBrowserHelper.AXEditMode.None);
        }

        internal void OnInPlaceDeactivate(WebBrowserBase site) {
            if (siteActive == site) {
                siteActive = null;
                ContainerControl parentContainer = parent.FindContainerControlInternal();
                if (parentContainer != null) {
                    parentContainer.SetActiveControlInternal(null);
                }
            }
        }

        internal void OnExitEditMode(WebBrowserBase ctl) {
            Debug.Assert(ctlInEditMode == null || ctlInEditMode == ctl, "who is exiting edit mode?");
            if (ctlInEditMode == ctl) {
                ctlInEditMode = null;
            }
        }
    }
    
}
