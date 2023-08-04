// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Office;

namespace System.Windows.Forms;

public sealed partial class Application
{
    /// <summary>
    ///  This is our implementation of the MSO ComponentManager. The Component Manager is an object that is
    ///  responsible for handling all message loop activity in a process. The idea is that someone in the process
    ///  implements the component manager and then anyone who wants access to the message loop can get to it.
    ///  We implement this so we have good interop with Office and VS. The first time we need a component manager,
    ///  we search the OLE message filter for one. If that fails, we create our own and install it in the message filter.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This class is not used when running inside the Visual Studio shell.
    ///  </para>
    /// </remarks>
    private unsafe class ComponentManager : IMsoComponentManager
    {
        private struct ComponentHashtableEntry
        {
            public IMsoComponent component;
            public MSOCRINFO componentInfo;
        }

        private Dictionary<nuint, ComponentHashtableEntry>? _oleComponents;
        private UIntPtr _cookieCounter = UIntPtr.Zero;
        private IMsoComponent? _activeComponent;
        private IMsoComponent? _trackingComponent;
        private msocstate _currentState;

        private Dictionary<nuint, ComponentHashtableEntry> OleComponents => _oleComponents ??= new();

        unsafe HRESULT IMsoComponentManager.QueryService(
            Guid* guidService,
            Guid* iid,
            void** ppvObj)
        {
            if (ppvObj is not null)
            {
                *ppvObj = null;
            }

            return HRESULT.E_NOINTERFACE;
        }

        BOOL IMsoComponentManager.FDebugMessage(
            nint dwReserved,
            uint msg,
            WPARAM wParam,
            LPARAM lParam)
        {
            return true;
        }

        BOOL IMsoComponentManager.FRegisterComponent(
            IMsoComponent component,
            MSOCRINFO* pcrinfo,
            nuint* pdwComponentID)
        {
            if (pcrinfo is null || pdwComponentID is null
                || pcrinfo->cbSize < sizeof(MSOCRINFO))
            {
                return false;
            }

            // Construct Hashtable entry for this component
            ComponentHashtableEntry entry = new ComponentHashtableEntry
            {
                component = component,
                componentInfo = *pcrinfo
            };

            _cookieCounter += 1;
            OleComponents.Add(_cookieCounter, entry);

            // Return the cookie
            *pdwComponentID = _cookieCounter;
            Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, $"ComponentManager: Component registered.  ID: {_cookieCounter}");
            return true;
        }

        BOOL IMsoComponentManager.FRevokeComponent(nuint dwComponentID)
        {
            Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, $"ComponentManager: Revoking component {dwComponentID}.");

            if (!OleComponents.TryGetValue(dwComponentID, out ComponentHashtableEntry entry))
            {
                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "Component not registered.");
                return false;
            }

            if (entry.component == _activeComponent)
            {
                _activeComponent = null;
            }

            if (entry.component == _trackingComponent)
            {
                _trackingComponent = null;
            }

            OleComponents.Remove(dwComponentID);
            return true;
        }

        BOOL IMsoComponentManager.FUpdateComponentRegistration(
            nuint dwComponentID,
            MSOCRINFO* pcrinfo)
        {
            // Update the registration info
            if (pcrinfo is null
                || !OleComponents.TryGetValue(dwComponentID, out ComponentHashtableEntry entry))
            {
                return false;
            }

            entry.componentInfo = *pcrinfo;
            OleComponents[dwComponentID] = entry;
            return true;
        }

        BOOL IMsoComponentManager.FOnComponentActivate(nuint dwComponentID)
        {
            Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, $"ComponentManager: Component activated.  ID: {dwComponentID}");

            if (!OleComponents.TryGetValue(dwComponentID, out ComponentHashtableEntry entry))
            {
                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "*** Component not registered ***");
                return false;
            }

            Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, $"New active component is : {entry}");
            _activeComponent = entry.component;
            return true;
        }

        BOOL IMsoComponentManager.FSetTrackingComponent(nuint dwComponentID, BOOL fTrack)
        {
            if (!OleComponents.TryGetValue(dwComponentID, out ComponentHashtableEntry entry)
                || !((entry.component == _trackingComponent) ^ fTrack))
            {
                return false;
            }

            _trackingComponent = fTrack ? entry.component : null;

            return true;
        }

        void IMsoComponentManager.OnComponentEnterState(
            nuint dwComponentID,
            msocstate uStateID,
            msoccontext uContext,
            uint cpicmExclude,
            void** rgpicmExclude,
            uint dwReserved)
        {
            _currentState = uStateID;

            Debug.WriteLineIf(
                CompModSwitches.MSOComponentManager.TraceInfo,
                $"ComponentManager: Component enter state.  ID: {dwComponentID} state: {uStateID}");

            if (uContext == msoccontext.All || uContext == msoccontext.Mine)
            {
                Debug.Indent();

                // We should notify all components we contain that the state has changed.
                foreach (ComponentHashtableEntry entry in OleComponents.Values)
                {
                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, $"Notifying {entry.component}");
                    entry.component.OnEnterState(uStateID, true);
                }

                Debug.Unindent();
            }
        }

        BOOL IMsoComponentManager.FOnComponentExitState(
            nuint dwComponentID,
            msocstate uStateID,
            msoccontext uContext,
            uint cpicmExclude,
            void** rgpicmExclude)
        {
            _currentState = 0;
            Debug.WriteLineIf(
                CompModSwitches.MSOComponentManager.TraceInfo,
                $"ComponentManager: Component exit state.  ID: {dwComponentID} state: {uStateID}");

            if (uContext == msoccontext.All || uContext == msoccontext.Mine)
            {
                Debug.Indent();

                // We should notify all components we contain that the state has changed.
                foreach (ComponentHashtableEntry entry in OleComponents.Values)
                {
                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, $"Notifying {entry.component}");
                    entry.component.OnEnterState(uStateID, false);
                }

                Debug.Unindent();
            }

            return false;
        }

        BOOL IMsoComponentManager.FInState(msocstate uStateID, void* pvoid)
            => _currentState == uStateID ? true : false;

        BOOL IMsoComponentManager.FContinueIdle()
        {
            // If we have a message in the queue, then don't continue idle processing.
            MSG msg = default;
            return PInvoke.PeekMessage(&msg, HWND.Null, 0, 0, PEEK_MESSAGE_REMOVE_TYPE.PM_NOREMOVE);
        }

        BOOL IMsoComponentManager.FPushMessageLoop(
            nuint dwComponentID,
            msoloop uReason,
            void* pvLoopData)
        {
            // Hold onto old state to allow restoring it before we exit.
            msocstate currentLoopState = _currentState;
            BOOL continueLoop = true;

            if (!OleComponents.TryGetValue(dwComponentID, out ComponentHashtableEntry entry))
            {
                return false;
            }

            IMsoComponent? prevActive = _activeComponent;

            try
            {
                MSG msg = default;
                IMsoComponent requestingComponent = entry.component;
                _activeComponent = requestingComponent;

                Debug.WriteLineIf(
                    CompModSwitches.MSOComponentManager.TraceInfo,
                    $"ComponentManager : Pushing message loop {uReason}");
                Debug.Indent();

                while (true)
                {
                    // Determine the component to route the message to
                    IMsoComponent component = _trackingComponent ?? _activeComponent ?? requestingComponent;

                    if (PInvoke.PeekMessage(&msg, HWND.Null, 0, 0, PEEK_MESSAGE_REMOVE_TYPE.PM_NOREMOVE))
                    {
                        if (!component.FContinueMessageLoop(uReason, pvLoopData, &msg))
                        {
                            return true;
                        }

                        // If the component wants us to process the message, do it.
                        PInvoke.GetMessage(&msg, HWND.Null, 0, 0);

                        if (msg.message == PInvoke.WM_QUIT)
                        {
                            Debug.WriteLineIf(
                                CompModSwitches.MSOComponentManager.TraceInfo,
                                "ComponentManager : Normal message loop termination");

                            ThreadContext.FromCurrent().DisposeThreadWindows();

                            if (uReason != msoloop.Main)
                            {
                                PInvoke.PostQuitMessage((int)msg.wParam);
                            }

                            return true;
                        }

                        // Now translate and dispatch the message.
                        //
                        // Reading through the rather sparse documentation, it seems we should only call
                        // FPreTranslateMessage on the active component.
                        if (!component.FPreTranslateMessage(&msg))
                        {
                            PInvoke.TranslateMessage(msg);
                            PInvoke.DispatchMessage(&msg);
                        }
                    }
                    else
                    {
                        // If this is a DoEvents loop, then get out. There's nothing left for us to do.
                        if (uReason == msoloop.DoEvents || uReason == msoloop.DoEventsModal)
                        {
                            break;
                        }

                        // Nothing is on the message queue. Perform idle processing and then do a WaitMessage.
                        bool continueIdle = false;

                        if (OleComponents is not null)
                        {
                            foreach (ComponentHashtableEntry idleEntry in OleComponents.Values)
                            {
                                continueIdle |= idleEntry.component.FDoIdle(msoidlef.All);
                            }
                        }

                        // Give the component one more chance to terminate the message loop.
                        if (!component.FContinueMessageLoop(uReason, pvLoopData, pMsgPeeked: null))
                        {
                            return true;
                        }

                        if (continueIdle)
                        {
                            // If someone has asked for idle time, give it to them. However, don't cycle immediately;
                            // wait up to 100ms. We don't want someone to attach to idle, forget to detach, and then
                            // cause CPU to end up in race condition. For Windows Forms this generally isn't an issue
                            // because our component always returns false from its idle request
                            PInvoke.MsgWaitForMultipleObjectsEx(
                                0,
                                null,
                                100,
                                QUEUE_STATUS_FLAGS.QS_ALLINPUT,
                                MSG_WAIT_FOR_MULTIPLE_OBJECTS_EX_FLAGS.MWMO_INPUTAVAILABLE);
                        }
                        else
                        {
                            // We should call GetMessage here, but we cannot because the component manager requires
                            // that we notify the active component before we pull the message off the queue. This is
                            // a bit of a problem, because WaitMessage waits for a NEW message to appear on the
                            // queue. If a message appeared between processing and now WaitMessage would wait for
                            // the next message. We minimize this here by calling PeekMessage.
                            if (!PInvoke.PeekMessage(&msg, HWND.Null, 0, 0, PEEK_MESSAGE_REMOVE_TYPE.PM_NOREMOVE))
                            {
                                PInvoke.WaitMessage();
                            }
                        }
                    }
                }

                Debug.Unindent();
                Debug.WriteLineIf(
                    CompModSwitches.MSOComponentManager.TraceInfo,
                    $"ComponentManager : message loop {uReason} complete.");
            }
            finally
            {
                _currentState = currentLoopState;
                _activeComponent = prevActive;
            }

            return !continueLoop;
        }

        unsafe BOOL IMsoComponentManager.FCreateSubComponentManager(
            IntPtr punkOuter,
            IntPtr punkServProv,
            Guid* riid,
            void** ppvObj)
        {
            // We do not support sub component managers.
            if (ppvObj is not null)
            {
                *ppvObj = null;
            }

            return false;
        }

        BOOL IMsoComponentManager.FGetParentComponentManager(void** ppicm)
        {
            // We have no parent.
            if (ppicm is not null)
            {
                *ppicm = null;
            }

            return false;
        }

        BOOL IMsoComponentManager.FGetActiveComponent(
            msogac dwgac,
            void** ppic,
            MSOCRINFO* pcrinfo,
            uint dwReserved)
        {
            IMsoComponent? component = dwgac switch
            {
                msogac.Active => _activeComponent,
                msogac.Tracking => _trackingComponent,
                msogac.TrackingOrActive => _trackingComponent ?? _activeComponent,
                _ => null
            };

            if (component is null)
            {
                return false;
            }

            if (pcrinfo is not null)
            {
                if (pcrinfo->cbSize < sizeof(MSOCRINFO))
                {
                    return false;
                }

                foreach (ComponentHashtableEntry entry in OleComponents.Values)
                {
                    if (entry.component == component)
                    {
                        *pcrinfo = entry.componentInfo;
                        break;
                    }
                }
            }

            if (ppic is not null)
            {
                // This will addref the interface
                *ppic = (void*)Marshal.GetComInterfaceForObject<IMsoComponent, IMsoComponent>(component);
            }

            return true;
        }
    }
}
