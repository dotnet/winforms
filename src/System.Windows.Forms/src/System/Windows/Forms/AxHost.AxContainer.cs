// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using static Interop;
using static Interop.Ole32;

namespace System.Windows.Forms
{
    public abstract partial class AxHost
    {
        internal class AxContainer : IOleContainer, IOleInPlaceFrame, IReflect
        {
            internal ContainerControl parent;
            private IContainer assocContainer; // associated IContainer...
            // the assocContainer may be null, in which case all this container does is
            // forward [de]activation messages to the requisite container...
            private AxHost siteUIActive;
            private AxHost siteActive;
            private bool formAlreadyCreated;
            private readonly Hashtable containerCache = new Hashtable();  // name -> Control
            private int lockCount;
            private Hashtable components;  // Control -> any
            private Hashtable proxyCache;
            private AxHost ctlInEditMode;

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

            internal Oleaut32.IExtender GetProxyForControl(Control ctl)
            {
                Oleaut32.IExtender rval = null;
                if (proxyCache is null)
                {
                    proxyCache = new Hashtable();
                }
                else
                {
                    rval = (Oleaut32.IExtender)proxyCache[ctl];
                }
                if (rval is null)
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

            internal void AddControl(Control ctl)
            {
                //
                lock (this)
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

            internal IEnumUnknown EnumControls(Control ctl, OLECONTF dwOleContF, GC_WCH dwWhich)
            {
                GetComponents();

                LockComponents();
                try
                {
                    ArrayList l = null;
                    bool selected = (dwWhich & GC_WCH.FSELECTED) != 0;
                    bool reverse = (dwWhich & GC_WCH.FREVERSEDIR) != 0;
                    // Note that visual basic actually ignores the next/prev flags... we will not
                    bool onlyNext = (dwWhich & GC_WCH.FONLYNEXT) != 0;
                    bool onlyPrev = (dwWhich & GC_WCH.FONLYPREV) != 0;
                    dwWhich &= ~(GC_WCH.FSELECTED | GC_WCH.FREVERSEDIR |
                                          GC_WCH.FONLYNEXT | GC_WCH.FONLYPREV);
                    if (onlyNext && onlyPrev)
                    {
                        Debug.Fail("onlyNext && onlyPrev are both set!");
                        throw E_INVALIDARG;
                    }
                    if (dwWhich == GC_WCH.CONTAINER || dwWhich == GC_WCH.CONTAINED)
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
                        case GC_WCH.CONTAINED:
                            ctls = ctl.GetChildControlsInTabOrder(false);
                            ctl = null;
                            break;
                        case GC_WCH.SIBLING:
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
                        case GC_WCH.CONTAINER:
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
                        case GC_WCH.ALL:
                            Hashtable htbl = GetComponents();
                            ctls = new Control[htbl.Keys.Count];
                            htbl.Keys.CopyTo(ctls, 0);
                            ctl = parent;
                            break;
                    }
                    if (l is null)
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

            private void MaybeAdd(ArrayList l, Control ctl, bool selected, OLECONTF dwOleContF, bool ignoreBelong)
            {
                if (!ignoreBelong && ctl != parent && !GetControlBelongs(ctl))
                {
                    return;
                }

                if (selected)
                {
                    ISelectionService iss = GetSelectionService(ctl);
                    if (iss is null || !iss.GetComponentSelected(this))
                    {
                        return;
                    }
                }
                if (ctl is AxHost hostctl && (dwOleContF & OLECONTF.EMBEDDINGS) != 0)
                {
                    l.Add(hostctl.GetOcx());
                }
                else if ((dwOleContF & OLECONTF.OTHERS) != 0)
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

                Debug.Assert(parent.Site is null, "Parent is sited but we could not find IContainer!!!");
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Did not find a container in FillComponentsTable!!!");

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
                Debug.Assert(rval is null || assocContainer is null || (rval == assocContainer),
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
                Debug.Assert(siteUIActive is null || siteUIActive == site, "deactivating when not active...");

                siteUIActive = null;
                site.RemoveSelectionHandler();
                site.SetSelectionStyle(1);
                site.editMode = EDITM_NONE;
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
                Debug.Assert(siteUIActive is null, "Object did not call OnUIDeactivate");
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "active Object is now " + site.ToString());
                siteUIActive = site;
                ContainerControl f = site.ContainingControl;
                if (f != null)
                {
                    f.ActiveControl = site;
                }
            }

            private void ListAxControls(ArrayList list, bool fuseOcx)
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
            unsafe HRESULT IOleContainer.ParseDisplayName(IntPtr pbc, string pszDisplayName, uint *pchEaten, IntPtr* ppmkOut)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in ParseDisplayName");
                if (ppmkOut != null)
                {
                    *ppmkOut = IntPtr.Zero;
                }

                return HRESULT.E_NOTIMPL;
            }

            HRESULT IOleContainer.EnumObjects(OLECONTF grfFlags, out IEnumUnknown ppenum)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in EnumObjects");
                ppenum = null;
                if ((grfFlags & OLECONTF.EMBEDDINGS) != 0)
                {
                    Debug.Assert(parent != null, "gotta have it...");
                    ArrayList list = new ArrayList();
                    ListAxControls(list, true);
                    if (list.Count > 0)
                    {
                        object[] temp = new object[list.Count];
                        list.CopyTo(temp, 0);
                        ppenum = new EnumUnknown(temp);
                        return HRESULT.S_OK;
                    }
                }

                ppenum = new EnumUnknown(null);
                return HRESULT.S_OK;
            }

            HRESULT IOleContainer.LockContainer(BOOL fLock)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in LockContainer");
                return HRESULT.E_NOTIMPL;
            }

            // IOleInPlaceFrame methods:
            unsafe HRESULT IOleInPlaceFrame.GetWindow(IntPtr* phwnd)
            {
                if (phwnd is null)
                {
                    return HRESULT.E_POINTER;
                }

                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in GetWindow");
                *phwnd = parent.Handle;
                return HRESULT.S_OK;
            }

            HRESULT IOleInPlaceFrame.ContextSensitiveHelp(BOOL fEnterMode)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in ContextSensitiveHelp");
                return HRESULT.S_OK;
            }

            unsafe HRESULT IOleInPlaceFrame.GetBorder(RECT* lprectBorder)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in GetBorder");
                return HRESULT.E_NOTIMPL;
            }

            unsafe HRESULT IOleInPlaceFrame.RequestBorderSpace(RECT* pborderwidths)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in RequestBorderSpace");
                return HRESULT.E_NOTIMPL;
            }

            unsafe HRESULT IOleInPlaceFrame.SetBorderSpace(RECT* pborderwidths)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in SetBorderSpace");
                return HRESULT.E_NOTIMPL;
            }

            internal void OnExitEditMode(AxHost ctl)
            {
                Debug.Assert(ctlInEditMode is null || ctlInEditMode == ctl, "who is exiting edit mode?");
                if (ctlInEditMode is null || ctlInEditMode != ctl)
                {
                    return;
                }

                ctlInEditMode = null;
            }

            HRESULT IOleInPlaceFrame.SetActiveObject(IOleInPlaceActiveObject pActiveObject, string pszObjName)
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
                if (pActiveObject is null)
                {
                    if (ctlInEditMode != null)
                    {
                        ctlInEditMode.editMode = EDITM_NONE;
                        ctlInEditMode = null;
                    }
                    return HRESULT.S_OK;
                }
                AxHost ctl = null;
                if (pActiveObject is IOleObject oleObject)
                {
                    HRESULT hr = oleObject.GetClientSite(out IOleClientSite clientSite);
                    Debug.Assert(hr.Succeeded());
                    if (clientSite is OleInterfaces)
                    {
                        ctl = ((OleInterfaces)(clientSite)).GetAxHost();
                    }

                    if (ctlInEditMode != null)
                    {
                        Debug.Fail("control " + ctlInEditMode.ToString() + " did not reset its edit mode to null");
                        ctlInEditMode.SetSelectionStyle(1);
                        ctlInEditMode.editMode = EDITM_NONE;
                    }

                    if (ctl is null)
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

                return HRESULT.S_OK;
            }

            unsafe HRESULT IOleInPlaceFrame.InsertMenus(IntPtr hmenuShared, OLEMENUGROUPWIDTHS* lpMenuWidths)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in InsertMenus");
                return HRESULT.S_OK;
            }

            HRESULT IOleInPlaceFrame.SetMenu(IntPtr hmenuShared, IntPtr holemenu, IntPtr hwndActiveObject)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in SetMenu");
                return HRESULT.E_NOTIMPL;
            }

            HRESULT IOleInPlaceFrame.RemoveMenus(IntPtr hmenuShared)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in RemoveMenus");
                return HRESULT.E_NOTIMPL;
            }

            HRESULT IOleInPlaceFrame.SetStatusText(string pszStatusText)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in SetStatusText");
                return HRESULT.E_NOTIMPL;
            }

            HRESULT IOleInPlaceFrame.EnableModeless(BOOL fEnable)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in EnableModeless");
                return HRESULT.E_NOTIMPL;
            }

            unsafe HRESULT IOleInPlaceFrame.TranslateAccelerator(User32.MSG* lpmsg, ushort wID)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in IOleInPlaceFrame.TranslateAccelerator");
                return HRESULT.S_FALSE;
            }

            // EXPOSED

            private class ExtenderProxy :
                Oleaut32.IExtender,
                IVBGetControl,
                IGetVBAObject,
                IGetOleObject,
                IReflect
            {
                private readonly WeakReference _pRef;
                private readonly WeakReference _pContainer;

                internal ExtenderProxy(Control principal, AxContainer container)
                {
                    _pRef = new WeakReference(principal);
                    _pContainer = new WeakReference(container);
                }

                private Control GetP() => (Control)_pRef.Target;

                private AxContainer GetC() => (AxContainer)_pContainer.Target;

                HRESULT IVBGetControl.EnumControls(OLECONTF dwOleContF, GC_WCH dwWhich, out IEnumUnknown ppenum)
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in EnumControls for proxy");
                    ppenum = GetC().EnumControls(GetP(), dwOleContF, dwWhich);
                    return HRESULT.S_OK;
                }

                unsafe HRESULT IGetOleObject.GetOleObject(Guid* riid, out object ppvObj)
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in GetOleObject for proxy");
                    ppvObj = null;
                    if (riid is null || !riid->Equals(ioleobject_Guid))
                    {
                        return HRESULT.E_INVALIDARG;
                    }

                    if (GetP() is AxHost ctl)
                    {
                        ppvObj = ctl.GetOcx();
                        return HRESULT.S_OK;
                    }

                    return HRESULT.E_FAIL;
                }

                unsafe HRESULT IGetVBAObject.GetObject(Guid* riid, IVBFormat[] rval, uint dwReserved)
                {
                    Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in GetObject for proxy");
                    if (rval is null || riid is null)
                    {
                        return HRESULT.E_INVALIDARG;
                    }

                    if (!riid->Equals(ivbformat_Guid))
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

                public BOOL Enabled
                {
                    get
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in getEnabled for proxy for " + GetP().ToString());
                        return GetP().Enabled.ToBOOL();
                    }
                    set
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in setEnabled for proxy for " + GetP().ToString() + " " + value.ToString());
                        GetP().Enabled = value.IsTrue();
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

                public BOOL TabStop
                {
                    get
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in getTabStop for proxy for " + GetP().ToString());
                        return GetP().TabStop.ToBOOL();
                    }
                    set
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in setTabStop for proxy for " + GetP().ToString() + " " + value.ToString());
                        GetP().TabStop = value.IsTrue();
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

                public BOOL Visible
                {
                    get
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in getVisible for proxy for " + GetP().ToString());
                        return GetP().Visible.ToBOOL();
                    }
                    set
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in setVisible for proxy for " + GetP().ToString() + " " + value.ToString());
                        GetP().Visible = value.IsTrue();
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

                public object Container => GetC();

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
                    if (prop is null)
                    {
                        prop = GetType().GetProperty(name, bindingAttr);
                    }
                    return prop;
                }

                PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
                {
                    PropertyInfo prop = GetP().GetType().GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
                    if (prop is null)
                    {
                        prop = GetType().GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
                    }
                    return prop;
                }

                PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr)
                {
                    PropertyInfo[] extenderProps = GetType().GetProperties(bindingAttr);
                    PropertyInfo[] ctlProps = GetP().GetType().GetProperties(bindingAttr);

                    if (extenderProps is null)
                    {
                        return ctlProps;
                    }
                    else if (ctlProps is null)
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
                    if (memb is null)
                    {
                        memb = GetType().GetMember(name, bindingAttr);
                    }
                    return memb;
                }

                MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr)
                {
                    MemberInfo[] extenderMembs = GetType().GetMembers(bindingAttr);
                    MemberInfo[] ctlMembs = GetP().GetType().GetMembers(bindingAttr);

                    if (extenderMembs is null)
                    {
                        return ctlMembs;
                    }
                    else if (ctlMembs is null)
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
    }
}
