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
            internal ContainerControl _parent;

            // The associated container may be null, in which case all this container does is
            // forward [de]activation messages to the requisite container.
            private IContainer _associatedContainer;

            private AxHost _siteUIActive;
            private AxHost _siteActive;
            private bool _formAlreadyCreated;
            private readonly Dictionary<Control, Control> _containerCache = new();  // name -> Control
            private int _lockCount;
            private Dictionary<Control, Control> _components;  // Control -> any
            private Dictionary<Control, Oleaut32.IExtender> _proxyCache;
            private AxHost _controlInEditMode;

            internal AxContainer(ContainerControl parent)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in constructor.  Parent created : {parent.Created}");
                _parent = parent;
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

            object IReflect.InvokeMember(
                string name,
                BindingFlags invokeAttr,
                Binder binder,
                object target,
                object[] args,
                ParameterModifier[] modifiers,
                CultureInfo culture,
                string[] namedParameters)
            {
                foreach (var (keyControl, valueControl) in _containerCache)
                {
                    string ctlName = GetNameForControl(keyControl);
                    if (ctlName.Equals(name))
                    {
                        return GetProxyForControl(valueControl);
                    }
                }

                throw s_unknownErrorException;
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
                if (_proxyCache is null)
                {
                    _proxyCache = new();
                }
                else
                {
                    _proxyCache.TryGetValue(ctl, out rval);
                }

                if (rval is null)
                {
                    if (ctl != _parent && !GetControlBelongs(ctl))
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "!parent || !belongs NYI");
                        AxContainer c = FindContainerForControl(ctl);
                        if (c is not null)
                        {
                            rval = new ExtenderProxy(ctl, c);
                        }
                        else
                        {
                            Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "unable to find proxy, returning null");
                            return null;
                        }
                    }
                    else
                    {
                        rval = new ExtenderProxy(ctl, this);
                    }

                    _proxyCache.Add(ctl, rval);
                }

                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"found proxy {rval}");
                return rval;
            }

            internal static string GetNameForControl(Control ctl)
            {
                string name = (ctl.Site is not null) ? ctl.Site.Name : ctl.Name;
                return name ?? "";
            }

            internal void AddControl(Control ctl)
            {
                lock (this)
                {
                    if (_containerCache.ContainsKey(ctl))
                    {
                        throw new ArgumentException(string.Format(SR.AXDuplicateControl, GetNameForControl(ctl)), nameof(ctl));
                    }

                    _containerCache.Add(ctl, ctl);

                    if (_associatedContainer is null)
                    {
                        ISite site = ctl.Site;
                        if (site is not null)
                        {
                            _associatedContainer = site.Container;
                            IComponentChangeService ccs = (IComponentChangeService)site.GetService(typeof(IComponentChangeService));
                            if (ccs is not null)
                            {
                                ccs.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
                            }
                        }
                    }
                    else
                    {
#if DEBUG
                        ISite site = ctl.Site;
                        if (site is not null && _associatedContainer != site.Container)
                        {
                            Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "mismatch between assoc container & added control");
                        }
#endif
                    }
                }
            }

            internal void RemoveControl(Control ctl)
            {
                lock (this)
                {
                    if (_containerCache.ContainsKey(ctl))
                    {
                        _containerCache.Remove(ctl);
                    }
                }
            }

            private void LockComponents()
            {
                _lockCount++;
            }

            private void UnlockComponents()
            {
                _lockCount--;
                if (_lockCount == 0)
                {
                    _components = null;
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
                        throw s_invalidArgumentException;
                    }

                    if (dwWhich == GC_WCH.CONTAINER || dwWhich == GC_WCH.CONTAINED)
                    {
                        if (onlyNext || onlyPrev)
                        {
                            Debug.Fail("GC_WCH_FONLYNEXT or FONLYPREV used with CONTAINER or CONTAINED");
                            throw s_invalidArgumentException;
                        }
                    }

                    int first = 0;
                    int last = -1; // meaning all
                    Control[] ctls = null;
                    switch (dwWhich)
                    {
                        default:
                            Debug.Fail("Bad GC_WCH");
                            throw s_invalidArgumentException;
                        case GC_WCH.CONTAINED:
                            ctls = ctl.GetChildControlsInTabOrder(false);
                            ctl = null;
                            break;
                        case GC_WCH.SIBLING:
                            Control p = ctl.ParentInternal;
                            if (p is not null)
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
                            while (ctl is not null)
                            {
                                AxContainer cont = FindContainerForControl(ctl);
                                if (cont is not null)
                                {
                                    MaybeAdd(l, cont._parent, selected, dwOleContF, true);
                                    ctl = cont._parent;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            break;
                        case GC_WCH.ALL:
                            Dictionary<Control, Control> htbl = GetComponents();
                            ctls = new Control[htbl.Keys.Count];
                            htbl.Keys.CopyTo(ctls, 0);
                            ctl = _parent;
                            break;
                    }

                    if (l is null)
                    {
                        l = new ArrayList();
                        if (last == -1 && ctls is not null)
                        {
                            last = ctls.Length;
                        }

                        if (ctl is not null)
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
                if (!ignoreBelong && ctl != _parent && !GetControlBelongs(ctl))
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
                    if (item is not null)
                    {
                        l.Add(item);
                    }
                }
            }

            private void FillComponentsTable(IContainer container)
            {
                if (container is not null)
                {
                    ComponentCollection comps = container.Components;
                    if (comps is not null)
                    {
                        _components = new();
                        foreach (IComponent comp in comps)
                        {
                            if (comp is Control control && comp != _parent && comp.Site is not null)
                            {
                                _components.Add(control, control);
                            }
                        }

                        return;
                    }
                }

                Debug.Assert(_parent.Site is null, "Parent is sited but we could not find IContainer!!!");
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "Did not find a container in FillComponentsTable!!!");

                bool checkHashTable = true;
                Control[] ctls = new Control[_containerCache.Values.Count];
                _containerCache.Values.CopyTo(ctls, 0);
                if (ctls is not null)
                {
                    if (ctls.Length > 0 && _components is null)
                    {
                        _components = new();
                        checkHashTable = false;
                    }

                    for (int i = 0; i < ctls.Length; i++)
                    {
                        if (checkHashTable && !_components.ContainsKey(ctls[i]))
                        {
                            _components.Add(ctls[i], ctls[i]);
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

                if (_components is null)
                {
                    _components = new();
                }

                if (ctl != _parent && !_components.ContainsKey(ctl))
                {
                    _components.Add(ctl, ctl);
                }

                foreach (Control c in ctl.Controls)
                {
                    GetAllChildren(c);
                }
            }

            private Dictionary<Control, Control> GetComponents()
            {
                return GetComponents(GetParentsContainer());
            }

            private Dictionary<Control, Control> GetComponents(IContainer cont)
            {
                if (_lockCount == 0)
                {
                    FillComponentsTable(cont);
                }

                return _components;
            }

            private bool GetControlBelongs(Control ctl)
            {
                Dictionary<Control, Control> comps = GetComponents();
                return comps.ContainsKey(ctl);
            }

            private IContainer GetParentIsDesigned()
            {
                ISite site = _parent.Site;
                if (site is not null && site.DesignMode)
                {
                    return site.Container;
                }

                return null;
            }

            private IContainer GetParentsContainer()
            {
                IContainer rval = GetParentIsDesigned();
                Debug.Assert(rval is null || _associatedContainer is null || (rval == _associatedContainer),
                             "mismatch between getIPD & aContainer");
                return rval ?? _associatedContainer;
            }

            private bool RegisterControl(AxHost ctl)
            {
                ISite site = ctl.Site;
                if (site is not null)
                {
                    IContainer cont = site.Container;
                    if (cont is not null)
                    {
                        if (_associatedContainer is not null)
                        {
                            return cont == _associatedContainer;
                        }
                        else
                        {
                            _associatedContainer = cont;
                            IComponentChangeService ccs = (IComponentChangeService)site.GetService(typeof(IComponentChangeService));
                            if (ccs is not null)
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
                if (sender == _associatedContainer && e.Component is Control c)
                {
                    RemoveControl(c);
                }
            }

            internal static AxContainer FindContainerForControl(Control ctl)
            {
                if (ctl is AxHost axctl)
                {
                    if (axctl._container is not null)
                    {
                        return axctl._container;
                    }

                    ContainerControl f = axctl.ContainingControl;
                    if (f is not null)
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
                if (_siteActive == site)
                {
                    _siteActive = null;
                    if (site.GetSiteOwnsDeactivation())
                    {
                        _parent.ActiveControl = null;
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
                Debug.Assert(_siteUIActive is null || _siteUIActive == site, "deactivating when not active...");

                _siteUIActive = null;
                site.RemoveSelectionHandler();
                site.SetSelectionStyle(1);
                site._editMode = EDITM_NONE;
            }

            internal void OnUIActivate(AxHost site)
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
                Debug.Assert(_siteUIActive is null, "Object did not call OnUIDeactivate");
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"active Object is now {site}");
                _siteUIActive = site;
                ContainerControl f = site.ContainingControl;
                if (f is not null)
                {
                    f.ActiveControl = site;
                }
            }

            private void ListAxControls(ArrayList list, bool fuseOcx)
            {
                var components = GetComponents();
                if (components is null)
                {
                    return;
                }

                Control[] ctls = new Control[components.Keys.Count];
                components.Keys.CopyTo(ctls, 0);
                if (ctls is not null)
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
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in controlCreated for {invoker} fAC: {_formAlreadyCreated}");
                if (_formAlreadyCreated)
                {
                    if (invoker.IsUserMode() && invoker.AwaitingDefreezing())
                    {
                        invoker.Freeze(false);
                    }
                }
                else
                {
                    // the form will be created in the future
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
            unsafe HRESULT IOleContainer.ParseDisplayName(IntPtr pbc, string pszDisplayName, uint* pchEaten, IntPtr* ppmkOut)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in ParseDisplayName");
                if (ppmkOut is not null)
                {
                    *ppmkOut = IntPtr.Zero;
                }

                return HRESULT.Values.E_NOTIMPL;
            }

            HRESULT IOleContainer.EnumObjects(OLECONTF grfFlags, out IEnumUnknown ppenum)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in EnumObjects");
                if ((grfFlags & OLECONTF.EMBEDDINGS) != 0)
                {
                    Debug.Assert(_parent is not null, "gotta have it...");
                    ArrayList list = new ArrayList();
                    ListAxControls(list, true);
                    if (list.Count > 0)
                    {
                        object[] temp = new object[list.Count];
                        list.CopyTo(temp, 0);
                        ppenum = new EnumUnknown(temp);
                        return HRESULT.Values.S_OK;
                    }
                }

                ppenum = new EnumUnknown(null);
                return HRESULT.Values.S_OK;
            }

            HRESULT IOleContainer.LockContainer(BOOL fLock)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in LockContainer");
                return HRESULT.Values.E_NOTIMPL;
            }

            // IOleInPlaceFrame methods:
            unsafe HRESULT IOleInPlaceFrame.GetWindow(IntPtr* phwnd)
            {
                if (phwnd is null)
                {
                    return HRESULT.Values.E_POINTER;
                }

                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in GetWindow");
                *phwnd = _parent.Handle;
                return HRESULT.Values.S_OK;
            }

            HRESULT IOleInPlaceFrame.ContextSensitiveHelp(BOOL fEnterMode)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in ContextSensitiveHelp");
                return HRESULT.Values.S_OK;
            }

            unsafe HRESULT IOleInPlaceFrame.GetBorder(RECT* lprectBorder)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in GetBorder");
                return HRESULT.Values.E_NOTIMPL;
            }

            unsafe HRESULT IOleInPlaceFrame.RequestBorderSpace(RECT* pborderwidths)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in RequestBorderSpace");
                return HRESULT.Values.E_NOTIMPL;
            }

            unsafe HRESULT IOleInPlaceFrame.SetBorderSpace(RECT* pborderwidths)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in SetBorderSpace");
                return HRESULT.Values.E_NOTIMPL;
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

            HRESULT IOleInPlaceFrame.SetActiveObject(IOleInPlaceActiveObject pActiveObject, string pszObjName)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in SetActiveObject {pszObjName ?? "<null>"}");
                if (_siteUIActive is not null)
                {
                    if (_siteUIActive._iOleInPlaceActiveObjectExternal != pActiveObject)
                    {
                        if (_siteUIActive._iOleInPlaceActiveObjectExternal is not null)
                        {
                            Marshal.ReleaseComObject(_siteUIActive._iOleInPlaceActiveObjectExternal);
                        }

                        _siteUIActive._iOleInPlaceActiveObjectExternal = pActiveObject;
                    }
                }

                if (pActiveObject is null)
                {
                    if (_controlInEditMode is not null)
                    {
                        _controlInEditMode._editMode = EDITM_NONE;
                        _controlInEditMode = null;
                    }

                    return HRESULT.Values.S_OK;
                }

                AxHost ctl = null;
                if (pActiveObject is IOleObject oleObject)
                {
                    HRESULT hr = oleObject.GetClientSite(out IOleClientSite clientSite);
                    Debug.Assert(hr.Succeeded);
                    if (clientSite is OleInterfaces interfaces)
                    {
                        ctl = interfaces.GetAxHost();
                    }

                    if (_controlInEditMode is not null)
                    {
                        Debug.Fail($"control {_controlInEditMode} did not reset its edit mode to null");
                        _controlInEditMode.SetSelectionStyle(1);
                        _controlInEditMode._editMode = EDITM_NONE;
                    }

                    if (ctl is null)
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "control w/o a valid site called setactiveobject");
                        _controlInEditMode = null;
                    }
                    else
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"resolved to {ctl}");
                        if (!ctl.IsUserMode())
                        {
                            _controlInEditMode = ctl;
                            ctl._editMode = EDITM_OBJECT;
                            ctl.AddSelectionHandler();
                            ctl.SetSelectionStyle(2);
                        }
                    }
                }

                return HRESULT.Values.S_OK;
            }

            unsafe HRESULT IOleInPlaceFrame.InsertMenus(IntPtr hmenuShared, OLEMENUGROUPWIDTHS* lpMenuWidths)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in InsertMenus");
                return HRESULT.Values.S_OK;
            }

            HRESULT IOleInPlaceFrame.SetMenu(IntPtr hmenuShared, IntPtr holemenu, IntPtr hwndActiveObject)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in SetMenu");
                return HRESULT.Values.E_NOTIMPL;
            }

            HRESULT IOleInPlaceFrame.RemoveMenus(IntPtr hmenuShared)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in RemoveMenus");
                return HRESULT.Values.E_NOTIMPL;
            }

            HRESULT IOleInPlaceFrame.SetStatusText(string pszStatusText)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in SetStatusText");
                return HRESULT.Values.E_NOTIMPL;
            }

            HRESULT IOleInPlaceFrame.EnableModeless(BOOL fEnable)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in EnableModeless");
                return HRESULT.Values.E_NOTIMPL;
            }

            unsafe HRESULT IOleInPlaceFrame.TranslateAccelerator(MSG* lpmsg, ushort wID)
            {
                Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in IOleInPlaceFrame.TranslateAccelerator");
                return HRESULT.Values.S_FALSE;
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
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in EnumControls for proxy");
                    ppenum = GetC().EnumControls(GetP(), dwOleContF, dwWhich);
                    return HRESULT.Values.S_OK;
                }

                unsafe HRESULT IGetOleObject.GetOleObject(Guid* riid, out object ppvObj)
                {
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in GetOleObject for proxy");
                    ppvObj = null;
                    if (riid is null || !riid->Equals(s_ioleobject_Guid))
                    {
                        return HRESULT.Values.E_INVALIDARG;
                    }

                    if (GetP() is AxHost ctl)
                    {
                        ppvObj = ctl.GetOcx();
                        return HRESULT.Values.S_OK;
                    }

                    return HRESULT.Values.E_FAIL;
                }

                unsafe HRESULT IGetVBAObject.GetObject(Guid* riid, IVBFormat[] rval, uint dwReserved)
                {
                    Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "in GetObject for proxy");
                    if (rval is null || riid is null)
                    {
                        return HRESULT.Values.E_INVALIDARG;
                    }

                    if (!riid->Equals(s_ivbformat_Guid))
                    {
                        rval[0] = null;
                        return HRESULT.Values.E_NOINTERFACE;
                    }

                    rval[0] = new VBFormat();
                    return HRESULT.Values.S_OK;
                }

                public int Align
                {
                    get
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in getAlign for proxy for {GetP()}");
                        int rval = (int)(GetP()).Dock;
                        if (rval < NativeMethods.ActiveX.ALIGN_MIN || rval > NativeMethods.ActiveX.ALIGN_MAX)
                        {
                            rval = NativeMethods.ActiveX.ALIGN_NO_CHANGE;
                        }

                        return rval;
                    }
                    set
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in setAlign for proxy for {GetP()} {value}");
                        GetP().Dock = (DockStyle)value;
                    }
                }

                public uint BackColor
                {
                    get
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in getBackColor for proxy for {GetP()}");
                        return GetOleColorFromColor(GetP().BackColor);
                    }
                    set
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in setBackColor for proxy for {GetP()} {value}");
                        GetP().BackColor = GetColorFromOleColor(value);
                    }
                }

                public BOOL Enabled
                {
                    get
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in getEnabled for proxy for {GetP()}");
                        return GetP().Enabled;
                    }
                    set
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in setEnabled for proxy for {GetP()} {value}");
                        GetP().Enabled = value;
                    }
                }

                public uint ForeColor
                {
                    get
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in getForeColor for proxy for {GetP()}");
                        return GetOleColorFromColor(GetP().ForeColor);
                    }
                    set
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in setForeColor for proxy for {GetP()} {value}");
                        GetP().ForeColor = GetColorFromOleColor(value);
                    }
                }

                public int Height
                {
                    get
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in getHeight for proxy for {GetP()}");
                        return Pixel2Twip(GetP().Height, xDirection: false);
                    }
                    set
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in setHeight for proxy for {GetP()} {Twip2Pixel(value, false)}");
                        GetP().Height = Twip2Pixel(value, xDirection: false);
                    }
                }

                public int Left
                {
                    get
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in getLeft for proxy for {GetP()}");
                        return Pixel2Twip(GetP().Left, xDirection: true);
                    }
                    set
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in setLeft for proxy for {GetP()} {Twip2Pixel(value, true)}");
                        GetP().Left = Twip2Pixel(value, xDirection: true);
                    }
                }

                public object Parent
                {
                    get
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in getParent for proxy for {GetP()}");
                        return GetC().GetProxyForControl(GetC()._parent);
                    }
                }

                public short TabIndex
                {
                    get
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in getTabIndex for proxy for {GetP()}");
                        return (short)GetP().TabIndex;
                    }
                    set
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in setTabIndex for proxy for {GetP()} {value}");
                        GetP().TabIndex = value;
                    }
                }

                public BOOL TabStop
                {
                    get
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in getTabStop for proxy for {GetP()}");
                        return GetP().TabStop;
                    }
                    set
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in setTabStop for proxy for {GetP()} {value}");
                        GetP().TabStop = value;
                    }
                }

                public int Top
                {
                    get
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in getTop for proxy for {GetP()}");
                        return Pixel2Twip(GetP().Top, xDirection: false);
                    }
                    set
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in setTop for proxy for {GetP()} {Twip2Pixel(value, false)}");
                        GetP().Top = Twip2Pixel(value, xDirection: false);
                    }
                }

                public BOOL Visible
                {
                    get
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in getVisible for proxy for {GetP()}");
                        return GetP().Visible;
                    }
                    set
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in setVisible for proxy for {GetP()} {value}");
                        GetP().Visible = value;
                    }
                }

                public int Width
                {
                    get
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in getWidth for proxy for {GetP()}");
                        return Pixel2Twip(GetP().Width, xDirection: true);
                    }
                    set
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in setWidth for proxy for {GetP()} {Twip2Pixel(value, true)}");
                        GetP().Width = Twip2Pixel(value, xDirection: true);
                    }
                }

                public string Name
                {
                    get
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in getName for proxy for {GetP()}");
                        return GetNameForControl(GetP());
                    }
                }

                public IntPtr Hwnd
                {
                    get
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in getHwnd for proxy for {GetP()}");
                        return GetP().Handle;
                    }
                }

                public object Container => GetC();

                public string Text
                {
                    get
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in getText for proxy for {GetP()}");
                        return GetP().Text;
                    }
                    set
                    {
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"in setText for proxy for {GetP()}");
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

                PropertyInfo IReflect.GetProperty(
                    string name,
                    BindingFlags bindingAttr,
                    Binder binder,
                    Type returnType,
                    Type[] types,
                    ParameterModifier[] modifiers)
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

                object IReflect.InvokeMember(
                    string name,
                    BindingFlags invokeAttr,
                    Binder binder,
                    object target,
                    object[] args,
                    ParameterModifier[] modifiers,
                    CultureInfo culture,
                    string[] namedParameters)
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
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "In UnderlyingSystemType");
                        return null;
                    }
                }
            }
        }
    }
}
