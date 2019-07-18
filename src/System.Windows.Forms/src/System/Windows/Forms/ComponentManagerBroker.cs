// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Globalization;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Ok, this class needs some explanation.  We share message loops with other applications through
    ///  an interface called IMsoComponentManager. A "component' is fairly coarse here:  Windows Forms
    ///  is a single component.  The component manager is the application that owns and runs the message
    ///  loop.  And, consequently, an IMsoComponent is a component that plugs into that message loop
    ///  to listen in on Windows messages.  So far, so good.
    ///
    ///  Because message loops are per-thread, IMsoComponentManager is also per-thread, which means
    ///  we will register a new IMsoComponent for each thread that is running a message loop.
    ///
    ///  In a purely managed application, we satisfy both halves of the equation:  Windows Forms
    ///  implements both the IMsoComponentManager and the IMsoComponent.  Things start
    ///  to get complicated when the IMsoComponentManager comes from the COM world.
    ///
    ///  There's a wrinkle with this design, however:  It is illegal to call IMsoComponentManager on a
    ///  different thread than it expects. In fact, it will throw an exception.  That's a probolem for key
    ///  events that we receive during shutdown, like app domain unload and process exit.  These
    ///  events occur on a thread pool thread, and because as soon as we return from that thread the
    ///  domain will typically be torn down, we don't have much of a chance to marshal the call to
    ///  the right thread.
    ///
    ///  That's where this set of classes comes in.  We actually maintain a single process-wide
    ///  application domain, and within this app domain is where we keep all of our precious
    ///  IMsoComponent objects.  These objects can marshal to other domains and that is how
    ///  all other user-created Windows Forms app domains talke to the component manager.
    ///  When one of these user-created domains is shut down, it notifies a proxied
    ///  IMsoComponent, which simply decrements a ref count.  When the ref count reaches zero,
    ///  the component waits until a new message comes into it from the component manager.
    ///  At that point it knows that it is on the right thread, and it unregisters itself from the
    ///  component manager.
    ///
    ///  If all this sounds expensive and complex, you should get a gold star.  It is.  But, we take
    ///  some care to only do it if we absolutely have to. For example, If we only need the additional
    ///  app domain if there is no executing assembly (so there is no managed entry point) and if
    ///  the component manager we get is a native COM object.
    ///
    ///
    ///  There are two main classes here:  ComponentManagerBroker and ComponentManagerProxy.
    ///
    ///  ComponentManagerBroker:
    ///  This class has a static API that can be used to retrieve a component manager proxy.
    ///  The API uses managed remoting to attempt to communicate with our secondary domain.
    ///  It will create the domain if it doesn't exist.  It communicates with an instance of itself
    ///  on the other side of the domain.  That instance maintains a ComponentManagerProxy
    ///  object for each thread that comes in with a request.
    ///
    ///  ComponentManagerProxy:
    ///  This class implements both IMsoComponentManager and IMsoComponent. It implements
    ///  IMsoComponent so it can register with with the real IMsoComponentManager that was
    ///  passed into this method.  After registering itself it will return an instance of itself
    ///  as IMsoComponentManager.  After that the component manager broker stays
    ///  out of the picture.  Here's a diagram to help:
    ///
    ///  UCM <-> CProxy / CMProxy <-> AC
    ///
    ///  UCM: Unmanaged component manager
    ///  CProxy: IMsoComponent half of ComponentManagerProxy
    ///  CMProxy: IMsoComponentManager half of ComponentManagerProxy
    ///  AC: Application's IMsoComponent implementation
    /// </summary>
    internal sealed class ComponentManagerBroker : MarshalByRefObject
    {
        // These are constants per process and are initialized in
        // a class cctor below.
        private static readonly object _syncObject;
        private static readonly string _remoteObjectName;

        // We keep a static instance of ourself.  It will really be
        // per-domain, but we only have one domain.  The purpose
        // of this is to keep us alive.  Note that this variable
        // is re-used in two different domains -- in our special
        // domain, it keeps us alive.  In other domains it acts as
        // cache.

        private static ComponentManagerBroker _broker;

        // Per-instance state
        [ThreadStatic]
        private ComponentManagerProxy _proxy;

        /// <summary>
        ///  Static ctor.  We just set up a few per-process globals here
        /// </summary>
        static ComponentManagerBroker()
        {
            int pid = SafeNativeMethods.GetCurrentProcessId();
            _syncObject = new object();
            _remoteObjectName = string.Format(CultureInfo.CurrentCulture, "ComponentManagerBroker.{0}.{1:X}", Application.WindowsFormsVersion, pid);
        }

        /// <summary>
        ///  Ctor.  Quite a bit happens here. Here, we register a channel so
        ///  we can be found and we publish ouru object by calling Marshal.
        ///
        ///  Note that we have a single static _broker field.  We assign this
        ///  If it is already assigned that means that someone in the default
        ///  app domain needed a component manger broker and craeted it directly,
        ///  passing in false for "remoteObject".  Therefore, all we need to do
        ///  is remote the existing broker.  This makes the instance of
        ///  ComponentManagerBroker we are creating in this ctor a temporary
        ///  object, because the calling code will use the Singleton property
        ///  to extract the actual _broker value.
        ///  NOTE: ctor must be public here for remoting to grab hold.
        /// </summary>
        public ComponentManagerBroker()
        {
            // Note that we only ever configure a single broker object.
            // We could be extremely transient here.
            if (_broker == null)
            {
                _broker = this;
            }
        }

        /// <summary>
        ///  Called during creation to account for an existing component manager
        ///  broker that was never remoted.  We try not to remote the broker
        ///  until we need to because it is very expensive.
        /// </summary>
        internal ComponentManagerBroker Singleton
        {
            get
            {
                return _broker;
            }
        }

        internal void ClearComponentManager()
        {
            _proxy = null;
        }

        #region Instance API only callable from a proxied object

        public UnsafeNativeMethods.IMsoComponentManager GetProxy(long pCM)
        {
            if (_proxy == null)
            {
                UnsafeNativeMethods.IMsoComponentManager original = (UnsafeNativeMethods.IMsoComponentManager)Marshal.GetObjectForIUnknown((IntPtr)pCM);
                _proxy = new ComponentManagerProxy(this, original);
            }

            return _proxy;
        }

        #endregion

        #region Static API callable from any domain

        /// <summary>
        ///  This method locates our per-process app domain and connects to a running
        ///  instance of ComponentManagerBroker.  That instance then demand-
        ///  creates an instance of ComponentManagerProxy for the calling thread
        ///  and returns it.
        /// </summary>
        internal static UnsafeNativeMethods.IMsoComponentManager GetComponentManager(IntPtr pOriginal)
        {
            lock (_syncObject)
            {

                if (_broker == null)
                {

                    // We need the default domain for the process. That's the domain we will use
                    // for all component managers.  There is no managed way to get this domain, however,
                    // so we use ICorRuntimeHost.
                    UnsafeNativeMethods.ICorRuntimeHost host = (UnsafeNativeMethods.ICorRuntimeHost)RuntimeEnvironment.GetRuntimeInterfaceAsObject(typeof(UnsafeNativeMethods.CorRuntimeHost).GUID, typeof(UnsafeNativeMethods.ICorRuntimeHost).GUID);
                    int hr = host.GetDefaultDomain(out object domainObj);
                    Debug.Assert(NativeMethods.Succeeded(hr), "ICorRuntimeHost failed to return the default domain.  The only way that should happen is if it hasn't been started yet, but if it hasn't been started how are we running managed code?");
                    AppDomain domain = domainObj as AppDomain;

                    if (domain == null)
                    {
                        Debug.Assert(NativeMethods.Failed(hr) || domain != null, "ICorRuntimeHost::GetDefaultDomain succeeded but didn't retrn us an app domain.");
                        domain = AppDomain.CurrentDomain;
                    }

                    // Ok, we have a domain.  Next, check to see if it is our current domain.
                    // If it is, we bypass the CreateInstanceAndUnwrap logic because we
                    // can directly go with the broker.  In this case we will create a broker
                    // and  NOT remote it.  We will defer the remoting until we have a different
                    // domain.  To detect this, the _broker static variable will be assigned
                    // a broker in the primary app domain.  The CreateInstance code looks at this
                    // and if it is aready set, simply remotes that broker.  That is why there
                    // is a "Singleton" property on the broker -- just in case we had to create
                    // a temporary broker during CreateInstanceAndUnwrap.

                    if (domain == AppDomain.CurrentDomain)
                    {
                        _broker = new ComponentManagerBroker();
                    }
                }
            }

            // However we got here, we got here.  What's important is that we have a proxied instance to the broker object
            // and we can now call on it.
            return _broker.GetProxy((long)pOriginal);
        }
        #endregion
    }

    #region ComponentManagerProxy Class
    /// <summary>
    ///  The proxy object. This acts as, well, a proxy between the unmanaged IMsoComponentManager and zero or more
    ///  managed components.
    /// </summary>
    internal class ComponentManagerProxy : MarshalByRefObject, UnsafeNativeMethods.IMsoComponentManager, UnsafeNativeMethods.IMsoComponent
    {
        private readonly ComponentManagerBroker _broker;
        private UnsafeNativeMethods.IMsoComponentManager _original;
        private int _refCount;
        private readonly int _creationThread;
        private IntPtr _componentId;
        private int _nextComponentId;
        private Dictionary<int, UnsafeNativeMethods.IMsoComponent> _components;
        private UnsafeNativeMethods.IMsoComponent _activeComponent;
        private int _activeComponentId;
        private UnsafeNativeMethods.IMsoComponent _trackingComponent;
        private int _trackingComponentId;

        internal ComponentManagerProxy(ComponentManagerBroker broker, UnsafeNativeMethods.IMsoComponentManager original)
        {
            _broker = broker;
            _original = original;
            _creationThread = SafeNativeMethods.GetCurrentThreadId();
            _refCount = 0;
        }

        private void Dispose()
        {
            if (_original != null)
            {
                Marshal.ReleaseComObject(_original);
                _original = null;
                _components = null;
                _componentId = (IntPtr)0;
                _refCount = 0;
                _broker.ClearComponentManager();
            }
        }

        private bool RevokeComponent()
        {
            return _original.FRevokeComponent(_componentId);
        }

        private UnsafeNativeMethods.IMsoComponent Component
        {
            get
            {
                if (_trackingComponent != null)
                {
                    return _trackingComponent;
                }

                if (_activeComponent != null)
                {
                    return _activeComponent;
                }

                return null;
            }
        }

        #region IMsoComponent Implementation
        bool UnsafeNativeMethods.IMsoComponent.FDebugMessage(IntPtr hInst, int msg, IntPtr wparam, IntPtr lparam)
        {
            UnsafeNativeMethods.IMsoComponent c = Component;

            if (c != null)
            {
                return c.FDebugMessage(hInst, msg, wparam, lparam);
            }

            return false;
        }

        bool UnsafeNativeMethods.IMsoComponent.FPreTranslateMessage(ref NativeMethods.MSG msg)
        {
            UnsafeNativeMethods.IMsoComponent c = Component;

            if (c != null)
            {
                return c.FPreTranslateMessage(ref msg);
            }

            return false;
        }

        void UnsafeNativeMethods.IMsoComponent.OnEnterState(int uStateID, bool fEnter)
        {
            if (_components != null)
            {
                foreach (UnsafeNativeMethods.IMsoComponent c in _components.Values)
                {
                    c.OnEnterState(uStateID, fEnter);
                }
            }
        }

        void UnsafeNativeMethods.IMsoComponent.OnAppActivate(bool fActive, int dwOtherThreadID)
        {
            if (_components != null)
            {
                foreach (UnsafeNativeMethods.IMsoComponent c in _components.Values)
                {
                    c.OnAppActivate(fActive, dwOtherThreadID);
                }
            }
        }

        void UnsafeNativeMethods.IMsoComponent.OnLoseActivation()
        {
            if (_activeComponent != null)
            {
                _activeComponent.OnLoseActivation();
            }
        }

        void UnsafeNativeMethods.IMsoComponent.OnActivationChange(UnsafeNativeMethods.IMsoComponent component, bool fSameComponent, int pcrinfo, bool fHostIsActivating, int pchostinfo, int dwReserved)
        {
            if (_components != null)
            {
                foreach (UnsafeNativeMethods.IMsoComponent c in _components.Values)
                {
                    c.OnActivationChange(component, fSameComponent, pcrinfo, fHostIsActivating, pchostinfo, dwReserved);
                }
            }
        }

        bool UnsafeNativeMethods.IMsoComponent.FDoIdle(int grfidlef)
        {
            bool cont = false;

            if (_components != null)
            {
                foreach (UnsafeNativeMethods.IMsoComponent c in _components.Values)
                {
                    cont |= c.FDoIdle(grfidlef);
                }
            }

            return cont;
        }

        bool UnsafeNativeMethods.IMsoComponent.FContinueMessageLoop(int reason, int pvLoopData, NativeMethods.MSG[] msgPeeked)
        {
            bool cont = false;

            if (_refCount == 0 && _componentId != (IntPtr)0)
            {
                if (RevokeComponent())
                {
                    _components.Clear();
                    _componentId = (IntPtr)0;
                }
            }

            if (_components != null)
            {
                foreach (UnsafeNativeMethods.IMsoComponent c in _components.Values)
                {
                    cont |= c.FContinueMessageLoop(reason, pvLoopData, msgPeeked);
                }
            }

            return cont;
        }

        bool UnsafeNativeMethods.IMsoComponent.FQueryTerminate(bool fPromptUser)
        {
            return true;
        }

        void UnsafeNativeMethods.IMsoComponent.Terminate()
        {
            if (_components != null && _components.Values.Count > 0)
            {
                UnsafeNativeMethods.IMsoComponent[] components = new UnsafeNativeMethods.IMsoComponent[_components.Values.Count];
                _components.Values.CopyTo(components, 0);
                foreach (UnsafeNativeMethods.IMsoComponent c in components)
                {
                    c.Terminate();
                }
            }

            if (_original != null)
            {
                RevokeComponent();
            }

            Dispose();
        }

        IntPtr UnsafeNativeMethods.IMsoComponent.HwndGetWindow(int dwWhich, int dwReserved)
        {
            UnsafeNativeMethods.IMsoComponent c = Component;

            if (c != null)
            {
                return c.HwndGetWindow(dwWhich, dwReserved);
            }

            return IntPtr.Zero;
        }
        #endregion

        #region IMsoComponentManager Implementation
        int UnsafeNativeMethods.IMsoComponentManager.QueryService(ref Guid guidService, ref Guid iid, out object ppvObj)
        {
            return _original.QueryService(ref guidService, ref iid, out ppvObj);
        }

        bool UnsafeNativeMethods.IMsoComponentManager.FDebugMessage(IntPtr hInst, int msg, IntPtr wparam, IntPtr lparam)
        {
            return _original.FDebugMessage(hInst, msg, wparam, lparam);
        }

        bool UnsafeNativeMethods.IMsoComponentManager.FRegisterComponent(UnsafeNativeMethods.IMsoComponent component, NativeMethods.MSOCRINFOSTRUCT pcrinfo, out IntPtr dwComponentID)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            dwComponentID = (IntPtr)0;

            if (_refCount == 0)
            {
                // Our first time hooking up to the real component manager
                if (!_original.FRegisterComponent(this, pcrinfo, out _componentId))
                {
                    return false;
                }
            }

            _refCount++;

            if (_components == null)
            {
                _components = new Dictionary<int, UnsafeNativeMethods.IMsoComponent>();
            }

            _nextComponentId++;
            if (_nextComponentId == int.MaxValue)
            {
                _nextComponentId = 1;
            }

            bool outofMemory = false;
            //just in case we wrap, lets search for a free ID
            while (_components.ContainsKey(_nextComponentId))
            {
                _nextComponentId++;
                if (_nextComponentId == int.MaxValue)
                {
                    if (outofMemory)
                    {
                        throw new InvalidOperationException(SR.ComponentManagerProxyOutOfMemory);
                    }
                    outofMemory = true;
                    _nextComponentId = 1;
                }
            }

            _components.Add(_nextComponentId, component);
            dwComponentID = (IntPtr)_nextComponentId;

            return true;
        }

        bool UnsafeNativeMethods.IMsoComponentManager.FRevokeComponent(IntPtr dwComponentID)
        {
            int dwLocalComponentID = unchecked((int)(long)dwComponentID);
            if (_original == null)
            {
                return false;
            }

            if (_components == null || dwLocalComponentID <= 0 || !_components.ContainsKey(dwLocalComponentID))
            {
                return false;
            }

            if (_refCount == 1 && SafeNativeMethods.GetCurrentThreadId() == _creationThread)
            {
                if (!RevokeComponent())
                {
                    return false;
                }
            }

            _refCount--;
            _components.Remove(dwLocalComponentID);

            Debug.Assert(_refCount >= 0, "underflow on ref count");
            if (_refCount <= 0)
            {
                Dispose();
            }

            if (dwLocalComponentID == _activeComponentId)
            {
                _activeComponent = null;
                _activeComponentId = 0;
            }
            if (dwLocalComponentID == _trackingComponentId)
            {
                _trackingComponent = null;
                _trackingComponentId = 0;
            }

            return true;
        }

        bool UnsafeNativeMethods.IMsoComponentManager.FUpdateComponentRegistration(IntPtr dwComponentID, NativeMethods.MSOCRINFOSTRUCT info)
        {
            if (_original == null)
            {
                return false;
            }
            // We assume that all winforms domains use the same registration.
            return _original.FUpdateComponentRegistration(_componentId, info);
        }

        bool UnsafeNativeMethods.IMsoComponentManager.FOnComponentActivate(IntPtr dwComponentID)
        {
            int dwLocalComponentID = unchecked((int)(long)dwComponentID);
            if (_original == null)
            {
                return false;
            }
            // Activation requres us to store the currently active component.  We will send data to it
            if (_components == null || dwLocalComponentID <= 0 || !_components.ContainsKey(dwLocalComponentID))
            {
                return false;
            }

            if (!_original.FOnComponentActivate(_componentId))
            {
                return false;
            }

            _activeComponent = _components[dwLocalComponentID];
            _activeComponentId = dwLocalComponentID;
            return true;
        }

        bool UnsafeNativeMethods.IMsoComponentManager.FSetTrackingComponent(IntPtr dwComponentID, bool fTrack)
        {
            // Tracking requres us to store the current tracking component.  We will send data to it
            int dwLocalComponentID = unchecked((int)(long)dwComponentID);

            if (_original == null)
            {
                return false;
            }

            if (_components == null || dwLocalComponentID <= 0 || !_components.ContainsKey(dwLocalComponentID))
            {
                return false;
            }

            if (!_original.FSetTrackingComponent(_componentId, fTrack))
            {
                return false;
            }

            if (fTrack)
            {
                _trackingComponent = _components[dwLocalComponentID];
                _trackingComponentId = dwLocalComponentID;
            }
            else
            {
                _trackingComponent = null;
                _trackingComponentId = 0;
            }

            return true;
        }

        void UnsafeNativeMethods.IMsoComponentManager.OnComponentEnterState(IntPtr dwComponentID, int uStateID, int uContext, int cpicmExclude, int rgpicmExclude, int dwReserved)
        {
            if (_original == null)
            {
                return;
            }

            if (uContext == NativeMethods.MSOCM.msoccontextAll || uContext == NativeMethods.MSOCM.msoccontextMine)
            {
                if (_components != null)
                {
                    foreach (UnsafeNativeMethods.IMsoComponent comp in _components.Values)
                    {
                        comp.OnEnterState(uStateID, true);
                    }
                }
            }

            _original.OnComponentEnterState(_componentId, uStateID, uContext, cpicmExclude, rgpicmExclude, dwReserved);
        }

        bool UnsafeNativeMethods.IMsoComponentManager.FOnComponentExitState(IntPtr dwComponentID, int uStateID, int uContext, int cpicmExclude, int rgpicmExclude)
        {
            if (_original == null)
            {
                return false;
            }

            if (uContext == NativeMethods.MSOCM.msoccontextAll || uContext == NativeMethods.MSOCM.msoccontextMine)
            {
                if (_components != null)
                {
                    foreach (UnsafeNativeMethods.IMsoComponent comp in _components.Values)
                    {
                        comp.OnEnterState(uStateID, false);
                    }
                }
            }

            return _original.FOnComponentExitState(_componentId, uStateID, uContext, cpicmExclude, rgpicmExclude);
        }

        bool UnsafeNativeMethods.IMsoComponentManager.FInState(int uStateID, IntPtr pvoid)
        {
            if (_original == null)
            {
                return false;
            }

            return _original.FInState(uStateID, pvoid);
        }

        bool UnsafeNativeMethods.IMsoComponentManager.FContinueIdle()
        {
            if (_original == null)
            {
                return false;
            }

            return _original.FContinueIdle();
        }

        bool UnsafeNativeMethods.IMsoComponentManager.FPushMessageLoop(IntPtr dwComponentID, int reason, int pvLoopData)
        {
            if (_original == null)
            {
                return false;
            }

            return _original.FPushMessageLoop(_componentId, reason, pvLoopData);
        }

        bool UnsafeNativeMethods.IMsoComponentManager.FCreateSubComponentManager(object punkOuter, object punkServProv, ref Guid riid, out IntPtr ppvObj)
        {
            if (_original == null)
            { ppvObj = IntPtr.Zero; return false; }
            return _original.FCreateSubComponentManager(punkOuter, punkServProv, ref riid, out ppvObj);
        }

        bool UnsafeNativeMethods.IMsoComponentManager.FGetParentComponentManager(out UnsafeNativeMethods.IMsoComponentManager ppicm)
        {
            if (_original == null)
            { ppicm = null; return false; }
            return _original.FGetParentComponentManager(out ppicm);
        }

        bool UnsafeNativeMethods.IMsoComponentManager.FGetActiveComponent(int dwgac, UnsafeNativeMethods.IMsoComponent[] ppic, NativeMethods.MSOCRINFOSTRUCT info, int dwReserved)
        {
            if (_original == null)
            {
                return false;
            }

            if (_original.FGetActiveComponent(dwgac, ppic, info, dwReserved))
            {
                // We got a component.  See if it's our proxy, and if it is,
                // return what we think is currently active.  We need only
                // check for "this", because we only have one of these
                // doo jabbers per process.
                if (ppic[0] == this)
                {
                    if (dwgac == NativeMethods.MSOCM.msogacActive)
                    {
                        ppic[0] = _activeComponent;
                    }
                    else if (dwgac == NativeMethods.MSOCM.msogacTracking)
                    {
                        ppic[0] = _trackingComponent;
                    }
                    else if (dwgac == NativeMethods.MSOCM.msogacTrackingOrActive)
                    {
                        if (_trackingComponent != null)
                        {
                            ppic[0] = _trackingComponent;
                        }
                    }
                }

                return ppic[0] != null;
            }
            else
            {
                return false;
            }
        }
        #endregion

    }
    #endregion
}
