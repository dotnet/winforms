// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static Interop;
using static Interop.Mso;

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
        private static readonly object s_syncObject = new object();

        // We keep a static instance of ourself.  It will really be
        // per-domain, but we only have one domain.  The purpose
        // of this is to keep us alive.  Note that this variable
        // is re-used in two different domains -- in our special
        // domain, it keeps us alive.  In other domains it acts as
        // cache.

        private static ComponentManagerBroker s_broker;

        // Per-instance state
        [ThreadStatic]
        private ComponentManagerProxy t_proxy;

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
            if (s_broker == null)
            {
                s_broker = this;
            }
        }

        /// <summary>
        ///  Called during creation to account for an existing component manager
        ///  broker that was never remoted.  We try not to remote the broker
        ///  until we need to because it is very expensive.
        /// </summary>
        internal ComponentManagerBroker Singleton => s_broker;

        internal void ClearComponentManager() => t_proxy = null;

        #region Instance API only callable from a proxied object

        public IMsoComponentManager GetProxy(long pCM)
        {
            if (t_proxy == null)
            {
                IMsoComponentManager original = (IMsoComponentManager)Marshal.GetObjectForIUnknown((IntPtr)pCM);
                t_proxy = new ComponentManagerProxy(this, original);
            }

            return t_proxy;
        }

        #endregion

        /// <summary>
        ///  This method locates our per-process app domain and connects to a running
        ///  instance of ComponentManagerBroker.  That instance then demand-
        ///  creates an instance of ComponentManagerProxy for the calling thread
        ///  and returns it.
        /// </summary>
        internal static IMsoComponentManager GetComponentManager(IntPtr pOriginal)
        {
            lock (s_syncObject)
            {
                if (s_broker == null)
                {
                    s_broker = new ComponentManagerBroker();
                }
            }

            return s_broker.GetProxy((long)pOriginal);
        }
    }

    #region ComponentManagerProxy Class
    /// <summary>
    ///  The proxy object. This acts as a proxy between the unmanaged IMsoComponentManager and zero or more
    ///  managed components.
    /// </summary>
    internal class ComponentManagerProxy : MarshalByRefObject, IMsoComponentManager, IMsoComponent
    {
        private readonly ComponentManagerBroker _broker;
        private IMsoComponentManager _original;
        private int _refCount;
        private readonly int _creationThread;
        private UIntPtr _componentId;
        private UIntPtr _nextComponentId;
        private Dictionary<UIntPtr, IMsoComponent> _components;
        private IMsoComponent _activeComponent;
        private UIntPtr _activeComponentId;
        private IMsoComponent _trackingComponent;
        private UIntPtr _trackingComponentId;

        private static UIntPtr s_maxUIntPtr = Environment.Is64BitProcess ? (UIntPtr)ulong.MaxValue : (UIntPtr)uint.MaxValue;

        internal ComponentManagerProxy(ComponentManagerBroker broker, IMsoComponentManager original)
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
                _componentId = UIntPtr.Zero;
                _refCount = 0;
                _broker.ClearComponentManager();
            }
        }

        private unsafe bool RevokeComponent()
            => _original.FRevokeComponent(_componentId).IsTrue();

        private IMsoComponent Component => _trackingComponent ?? _activeComponent;

        #region IMsoComponent Implementation
        BOOL IMsoComponent.FDebugMessage(
            IntPtr hInst,
            uint msg,
            IntPtr wParam,
            IntPtr lParam)
            => Component?.FDebugMessage(hInst, msg, wParam, lParam) ?? BOOL.FALSE;

        unsafe BOOL IMsoComponent.FPreTranslateMessage(User32.MSG* msg)
            => Component?.FPreTranslateMessage(msg) ?? BOOL.FALSE;

        void IMsoComponent.OnEnterState(msocstate uStateID, BOOL fEnter)
        {
            if (_components != null)
            {
                foreach (IMsoComponent c in _components.Values)
                {
                    c.OnEnterState(uStateID, fEnter);
                }
            }
        }

        void IMsoComponent.OnAppActivate(BOOL fActive, uint dwOtherThreadID)
        {
            if (_components != null)
            {
                foreach (IMsoComponent c in _components.Values)
                {
                    c.OnAppActivate(fActive, dwOtherThreadID);
                }
            }
        }

        void IMsoComponent.OnLoseActivation() => _activeComponent?.OnLoseActivation();

        unsafe void IMsoComponent.OnActivationChange(
            IMsoComponent component,
            BOOL fSameComponent,
            MSOCRINFO* pcrinfo,
            BOOL fHostIsActivating,
            IntPtr pchostinfo,
            uint dwReserved)
        {
            if (_components != null)
            {
                foreach (IMsoComponent c in _components.Values)
                {
                    c.OnActivationChange(component, fSameComponent, pcrinfo, fHostIsActivating, pchostinfo, dwReserved);
                }
            }
        }

        BOOL IMsoComponent.FDoIdle(msoidlef grfidlef)
        {
            bool cont = false;

            if (_components != null)
            {
                foreach (IMsoComponent c in _components.Values)
                {
                    cont |= c.FDoIdle(grfidlef).IsTrue();
                }
            }

            return cont.ToBOOL();
        }

        unsafe BOOL IMsoComponent.FContinueMessageLoop(
            msoloop uReason,
            void* pvLoopData,
            User32.MSG* pMsgPeeked)
        {
            bool cont = false;

            if (_refCount == 0 && _componentId != UIntPtr.Zero)
            {
                if (RevokeComponent())
                {
                    _components.Clear();
                    _componentId = UIntPtr.Zero;
                }
            }

            if (_components != null)
            {
                foreach (IMsoComponent c in _components.Values)
                {
                    cont |= c.FContinueMessageLoop(uReason, pvLoopData, pMsgPeeked).IsTrue();
                }
            }

            return cont.ToBOOL();
        }

        BOOL IMsoComponent.FQueryTerminate(BOOL fPromptUser) => BOOL.TRUE;

        void IMsoComponent.Terminate()
        {
            if (_components != null && _components.Values.Count > 0)
            {
                IMsoComponent[] components = new IMsoComponent[_components.Values.Count];
                _components.Values.CopyTo(components, 0);
                foreach (IMsoComponent c in components)
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

        IntPtr IMsoComponent.HwndGetWindow(msocWindow dwWhich, uint dwReserved)
            => Component?.HwndGetWindow(dwWhich, dwReserved) ?? IntPtr.Zero;
        #endregion

        #region IMsoComponentManager Implementation
        unsafe HRESULT IMsoComponentManager.QueryService(Guid* guidService, Guid* iid, void** ppvObj)
            => _original?.QueryService(guidService, iid, ppvObj) ?? HRESULT.E_NOINTERFACE;

        BOOL IMsoComponentManager.FDebugMessage(
            IntPtr dwReserved,
            uint msg,
            IntPtr wParam,
            IntPtr lParam)
            => _original?.FDebugMessage(dwReserved, msg, wParam, lParam) ?? BOOL.TRUE;

        unsafe BOOL IMsoComponentManager.FRegisterComponent(
            IMsoComponent component,
            MSOCRINFO* pcrinfo,
            UIntPtr* dwComponentID)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            if (_refCount == 0)
            {
                // Our first time hooking up to the real component manager
                if (_original.FRegisterComponent(this, pcrinfo, dwComponentID).IsFalse())
                {
                    return BOOL.FALSE;
                }
            }

            _refCount++;

            if (_components == null)
            {
                _components = new Dictionary<UIntPtr, IMsoComponent>();
            }

            _nextComponentId += 1;
            if (_nextComponentId == s_maxUIntPtr)
            {
                _nextComponentId = (UIntPtr)1;
            }

            bool outofMemory = false;

            // Just in case we wrap, lets search for a free ID
            while (_components.ContainsKey(_nextComponentId))
            {
                _nextComponentId += 1;
                if (_nextComponentId == s_maxUIntPtr)
                {
                    if (outofMemory)
                    {
                        throw new InvalidOperationException(SR.ComponentManagerProxyOutOfMemory);
                    }
                    outofMemory = true;
                    _nextComponentId = (UIntPtr)1;
                }
            }

            _components.Add(_nextComponentId, component);
            *dwComponentID = _nextComponentId;

            return BOOL.TRUE;
        }

        unsafe BOOL IMsoComponentManager.FRevokeComponent(UIntPtr dwComponentID)
        {
            if (_original == null || _components == null || !_components.ContainsKey(dwComponentID))
            {
                return BOOL.FALSE;
            }

            if (_refCount == 1 && SafeNativeMethods.GetCurrentThreadId() == _creationThread)
            {
                if (!RevokeComponent())
                {
                    return BOOL.FALSE;
                }
            }

            _refCount--;
            _components.Remove(dwComponentID);

            Debug.Assert(_refCount >= 0, "underflow on ref count");
            if (_refCount <= 0)
            {
                Dispose();
            }

            if (dwComponentID == _activeComponentId)
            {
                _activeComponent = null;
                _activeComponentId = UIntPtr.Zero;
            }

            if (dwComponentID == _trackingComponentId)
            {
                _trackingComponent = null;
                _trackingComponentId = UIntPtr.Zero;
            }

            return BOOL.TRUE;
        }

        unsafe BOOL IMsoComponentManager.FUpdateComponentRegistration(
            UIntPtr dwComponentID,
            MSOCRINFO* pcrinfo)
            => _original?.FUpdateComponentRegistration(dwComponentID, pcrinfo) ?? BOOL.FALSE;

        unsafe BOOL IMsoComponentManager.FOnComponentActivate(UIntPtr dwComponentID)
        {
            // Activation requres us to store the currently active component.  We will send data to it
            if (_original == null || _components == null || !_components.ContainsKey(dwComponentID)
                || _original.FOnComponentActivate(dwComponentID).IsFalse())
            {
                return BOOL.FALSE;
            }

            _activeComponent = _components[dwComponentID];
            _activeComponentId = dwComponentID;
            return BOOL.TRUE;
        }

        unsafe BOOL IMsoComponentManager.FSetTrackingComponent(
            UIntPtr dwComponentID,
            BOOL fTrack)
        {
            // Tracking requres us to store the current tracking component.  We will send data to it
            if (_original == null || _components == null || !_components.ContainsKey(dwComponentID)
                    || _original.FSetTrackingComponent(dwComponentID, fTrack).IsFalse())
            {
                return BOOL.FALSE;
            }

            if (fTrack.IsTrue())
            {
                _trackingComponent = _components[dwComponentID];
                _trackingComponentId = dwComponentID;
            }
            else
            {
                _trackingComponent = null;
                _trackingComponentId = UIntPtr.Zero;
            }

            return BOOL.TRUE;
        }

        unsafe void IMsoComponentManager.OnComponentEnterState(
            UIntPtr dwComponentID,
            msocstate uStateID,
            msoccontext uContext,
            uint cpicmExclude,
            void** rgpicmExclude,
            uint dwReserved)
        {
            if (_original == null)
            {
                return;
            }

            if (uContext == msoccontext.All || uContext == msoccontext.Mine)
            {
                if (_components != null)
                {
                    foreach (IMsoComponent comp in _components.Values)
                    {
                        comp.OnEnterState(uStateID, BOOL.TRUE);
                    }
                }
            }

            _original.OnComponentEnterState(dwComponentID, uStateID, uContext, cpicmExclude, rgpicmExclude, dwReserved);
        }

        unsafe BOOL IMsoComponentManager.FOnComponentExitState(
            UIntPtr dwComponentID,
            msocstate uStateID,
            msoccontext uContext,
            uint cpicmExclude,
            void** rgpicmExclude)
        {
            if (_original == null)
            {
                return BOOL.FALSE;
            }

            if (uContext == msoccontext.All || uContext == msoccontext.Mine)
            {
                if (_components != null)
                {
                    foreach (IMsoComponent comp in _components.Values)
                    {
                        comp.OnEnterState(uStateID, BOOL.FALSE);
                    }
                }
            }

            return _original.FOnComponentExitState(dwComponentID, uStateID, uContext, cpicmExclude, rgpicmExclude);
        }

        unsafe BOOL IMsoComponentManager.FInState(msocstate uStateID, void* pvoid)
            => _original?.FInState(uStateID, pvoid) ?? BOOL.FALSE;

        BOOL IMsoComponentManager.FContinueIdle()
            => _original?.FContinueIdle() ?? BOOL.FALSE;

        unsafe BOOL IMsoComponentManager.FPushMessageLoop(
            UIntPtr dwComponentID,
            msoloop uReason,
            void* pvLoopData)
            => _original?.FPushMessageLoop(dwComponentID, uReason, pvLoopData) ?? BOOL.FALSE;

        unsafe BOOL IMsoComponentManager.FCreateSubComponentManager(IntPtr punkOuter, IntPtr punkServProv, Guid* riid, void** ppvObj)
        {
            if (_original == null)
            {
                if (ppvObj != null)
                {
                    *ppvObj = null;
                }
                return BOOL.FALSE;
            }
            return _original.FCreateSubComponentManager(punkOuter, punkServProv, riid, ppvObj);
        }

        unsafe BOOL IMsoComponentManager.FGetParentComponentManager(void** ppicm)
        {
            if (_original == null)
            {
                if (ppicm != null)
                {
                    *ppicm = null;
                }
                return BOOL.FALSE;
            }
            return _original.FGetParentComponentManager(ppicm);
        }

        unsafe BOOL IMsoComponentManager.FGetActiveComponent(
            msogac dwgac,
            void** ppic,
            MSOCRINFO* pcrinfo,
            uint dwReserved)
        {
            if (_original == null
                || _original.FGetActiveComponent(dwgac, ppic, pcrinfo, dwReserved).IsFalse())
            {
                return BOOL.FALSE;
            }

            // We got a component.  See if it's our proxy, and if it is,
            // return what we think is currently active.  We need only
            // check for "this", because we only have one of these
            // doo jabbers per process.

            IntPtr pUnk = Marshal.GetIUnknownForObject(this);
            if (*ppic == (void*)pUnk)
            {
                *ppic = (void*)Marshal.GetIUnknownForObject(dwgac switch
                {
                    msogac.Active => _activeComponent,
                    msogac.Tracking => _trackingComponent,
                    msogac.TrackingOrActive => _trackingComponent ?? _activeComponent,
                    _ => this
                });
            }
            Marshal.Release(pUnk);

            return ppic != null ? BOOL.TRUE : BOOL.FALSE;
        }
        #endregion
    }
    #endregion
}
