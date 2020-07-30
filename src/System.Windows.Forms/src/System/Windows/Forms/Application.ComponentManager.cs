// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static Interop;
using static Interop.Mso;

namespace System.Windows.Forms
{
    public sealed partial class Application
    {
        /// <summary>
        ///  This is our implementation of the MSO ComponentManager.  The Componoent Manager is
        ///  an object that is responsible for handling all message loop activity in a process.
        ///  The idea is that someone in the process implements the component manager and then
        ///  anyone who wants access to the message loop can get to it.  We implement this
        ///  so we have good interop with office and VS.  The first time we need a
        ///  component manager, we search the OLE message filter for one.  If that fails, we
        ///  create our own and install it in the message filter.
        ///
        ///  This class is not used when running inside the Visual Studio shell.
        /// </summary>
        private unsafe class ComponentManager : IMsoComponentManager
        {
            private struct ComponentHashtableEntry
            {
                public IMsoComponent component;
                public MSOCRINFO componentInfo;
            }

            private Dictionary<UIntPtr, ComponentHashtableEntry> _oleComponents;
            private UIntPtr _cookieCounter = UIntPtr.Zero;
            private IMsoComponent _activeComponent;
            private IMsoComponent _trackingComponent;
            private msocstate _currentState;

            private Dictionary<UIntPtr, ComponentHashtableEntry> OleComponents
            {
                get
                {
                    if (_oleComponents is null)
                    {
                        _oleComponents = new Dictionary<UIntPtr, ComponentHashtableEntry>();
                    }

                    return _oleComponents;
                }
            }

            /// <inheritdoc/>
            unsafe HRESULT IMsoComponentManager.QueryService(
                Guid* guidService,
                Guid* iid,
                void** ppvObj)
            {
                if (ppvObj != null)
                {
                    *ppvObj = null;
                }
                return HRESULT.E_NOINTERFACE;
            }

            /// <inheritdoc/>
            BOOL IMsoComponentManager.FDebugMessage(
                IntPtr dwReserved,
                uint msg,
                IntPtr wParam,
                IntPtr lParam)
            {
                return BOOL.TRUE;
            }

            /// <inheritdoc/>
            BOOL IMsoComponentManager.FRegisterComponent(
                IMsoComponent component,
                MSOCRINFO* pcrinfo,
                UIntPtr* pdwComponentID)
            {
                if (pcrinfo is null || pdwComponentID is null
                    || pcrinfo->cbSize < sizeof(MSOCRINFO))
                {
                    return BOOL.FALSE;
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
                return BOOL.TRUE;
            }

            /// <inheritdoc/>
            BOOL IMsoComponentManager.FRevokeComponent(UIntPtr dwComponentID)
            {
                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, $"ComponentManager: Revoking component {dwComponentID}.");

                if (!OleComponents.TryGetValue(dwComponentID, out ComponentHashtableEntry entry))
                {
                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "Compoenent not registered.");
                    return BOOL.FALSE;
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
                return BOOL.TRUE;
            }

            /// <inheritdoc/>
            BOOL IMsoComponentManager.FUpdateComponentRegistration(
                UIntPtr dwComponentID,
                MSOCRINFO* pcrinfo)
            {
                // Update the registration info
                if (pcrinfo is null
                    || !OleComponents.TryGetValue(dwComponentID, out ComponentHashtableEntry entry))
                {
                    return BOOL.FALSE;
                }

                entry.componentInfo = *pcrinfo;
                OleComponents[dwComponentID] = entry;
                return BOOL.TRUE;
            }

            /// <inheritdoc/>
            BOOL IMsoComponentManager.FOnComponentActivate(UIntPtr dwComponentID)
            {
                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, $"ComponentManager: Component activated.  ID: {dwComponentID}");

                if (!OleComponents.TryGetValue(dwComponentID, out ComponentHashtableEntry entry))
                {
                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "*** Component not registered ***");
                    return BOOL.FALSE;
                }

                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, $"New active component is : {entry}");
                _activeComponent = entry.component;
                return BOOL.TRUE;
            }

            /// <inheritdoc/>
            BOOL IMsoComponentManager.FSetTrackingComponent(UIntPtr dwComponentID, BOOL fTrack)
            {
                if (!OleComponents.TryGetValue(dwComponentID, out ComponentHashtableEntry entry)
                    || !((entry.component == _trackingComponent) ^ fTrack.IsTrue()))
                {
                    return BOOL.FALSE;
                }

                _trackingComponent = fTrack.IsTrue() ? entry.component : null;

                return BOOL.TRUE;
            }

            /// <inheritdoc/>
            void IMsoComponentManager.OnComponentEnterState(
                UIntPtr dwComponentID,
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
                        entry.component.OnEnterState(uStateID, BOOL.TRUE);
                    }

                    Debug.Unindent();
                }
            }

            /// <inheritdoc/>
            BOOL IMsoComponentManager.FOnComponentExitState(
                UIntPtr dwComponentID,
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
                        entry.component.OnEnterState(uStateID, BOOL.FALSE);
                    }

                    Debug.Unindent();
                }

                return BOOL.FALSE;
            }

            /// <inheritdoc/>
            BOOL IMsoComponentManager.FInState(msocstate uStateID, void* pvoid)
                => _currentState == uStateID ? BOOL.TRUE : BOOL.FALSE;

            /// <inheritdoc/>
            BOOL IMsoComponentManager.FContinueIdle()
            {
                // If we have a message on queue, then don't continue idle processing.
                var msg = new User32.MSG();
                return User32.PeekMessageW(ref msg);
            }

            /// <inheritdoc/>
            BOOL IMsoComponentManager.FPushMessageLoop(
                UIntPtr dwComponentID,
                msoloop uReason,
                void* pvLoopData)
            {
                // Hold onto old state to allow restore before we exit...
                msocstate currentLoopState = _currentState;
                BOOL continueLoop = BOOL.TRUE;

                if (!OleComponents.TryGetValue(dwComponentID, out ComponentHashtableEntry entry))
                {
                    return BOOL.FALSE;
                }

                IMsoComponent prevActive = _activeComponent;

                try
                {
                    User32.MSG msg = new User32.MSG();
                    IMsoComponent requestingComponent = entry.component;
                    _activeComponent = requestingComponent;

                    Debug.WriteLineIf(
                        CompModSwitches.MSOComponentManager.TraceInfo,
                        $"ComponentManager : Pushing message loop {uReason}");
                    Debug.Indent();

                    while (continueLoop.IsTrue())
                    {
                        // Determine the component to route the message to
                        IMsoComponent component = _trackingComponent ?? _activeComponent ?? requestingComponent;

                        bool useAnsi = false;
                        BOOL peeked = User32.PeekMessageW(ref msg);

                        if (peeked.IsTrue())
                        {
                            useAnsi = msg.hwnd != IntPtr.Zero && User32.IsWindowUnicode(msg.hwnd).IsFalse();
                            if (useAnsi)
                            {
                                peeked = User32.PeekMessageA(ref msg);
                            }
                        }

                        if (peeked.IsTrue())
                        {
                            continueLoop = component.FContinueMessageLoop(uReason, pvLoopData, &msg);

                            // If the component wants us to process the message, do it.
                            if (continueLoop.IsTrue())
                            {
                                if (useAnsi)
                                {
                                    User32.GetMessageA(ref msg);
                                    Debug.Assert(User32.IsWindowUnicode(msg.hwnd).IsFalse());
                                }
                                else
                                {
                                    User32.GetMessageW(ref msg);
                                    Debug.Assert(msg.hwnd == IntPtr.Zero || User32.IsWindowUnicode(msg.hwnd).IsTrue());
                                }

                                if (msg.message == User32.WM.QUIT)
                                {
                                    Debug.WriteLineIf(
                                        CompModSwitches.MSOComponentManager.TraceInfo,
                                        "ComponentManager : Normal message loop termination");

                                    ThreadContext.FromCurrent().DisposeThreadWindows();

                                    if (uReason != msoloop.Main)
                                    {
                                        User32.PostQuitMessage((int)msg.wParam);
                                    }

                                    continueLoop = BOOL.FALSE;
                                    break;
                                }

                                // Now translate and dispatch the message.
                                //
                                // Reading through the rather sparse documentation,
                                // it seems we should only call FPreTranslateMessage
                                // on the active component.
                                if (component.FPreTranslateMessage(&msg).IsFalse())
                                {
                                    User32.TranslateMessage(ref msg);
                                    if (useAnsi)
                                    {
                                        User32.DispatchMessageA(ref msg);
                                    }
                                    else
                                    {
                                        User32.DispatchMessageW(ref msg);
                                    }
                                }
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

                            if (OleComponents != null)
                            {
                                IEnumerator enumerator = OleComponents.Values.GetEnumerator();

                                while (enumerator.MoveNext())
                                {
                                    ComponentHashtableEntry idleEntry = (ComponentHashtableEntry)enumerator.Current;
                                    continueIdle |= idleEntry.component.FDoIdle(msoidlef.All).IsTrue();
                                }
                            }

                            // Give the component one more chance to terminate the message loop.
                            continueLoop = component.FContinueMessageLoop(uReason, pvLoopData, null);

                            if (continueLoop.IsTrue())
                            {
                                if (continueIdle)
                                {
                                    // If someone has asked for idle time, give it to them.  However,
                                    // don't cycle immediately; wait up to 100ms.  Why?  Because we don't
                                    // want someone to attach to idle, forget to detach, and then cause
                                    // CPU to end up in race condition.  For Windows Forms this generally isn't an issue because
                                    // our component always returns false from its idle request
                                    User32.MsgWaitForMultipleObjectsEx(0, IntPtr.Zero, 100, User32.QS.ALLINPUT, User32.MWMO.INPUTAVAILABLE);
                                }
                                else
                                {
                                    // We should call GetMessage here, but we cannot because
                                    // the component manager requires that we notify the
                                    // active component before we pull the message off the
                                    // queue.  This is a bit of a problem, because WaitMessage
                                    // waits for a NEW message to appear on the queue.  If a
                                    // message appeared between processing and now WaitMessage
                                    // would wait for the next message.  We minimize this here
                                    // by calling PeekMessage.
                                    if (User32.PeekMessageW(ref msg, IntPtr.Zero, 0, 0, User32.PM.NOREMOVE).IsFalse())
                                    {
                                        User32.WaitMessage();
                                    }
                                }
                            }
                        }
                    }

                    Debug.Unindent();
                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, $"ComponentManager : message loop {uReason} complete.");
                }
                finally
                {
                    _currentState = currentLoopState;
                    _activeComponent = prevActive;
                }

                return continueLoop.IsFalse() ? BOOL.TRUE : BOOL.FALSE;
            }

            /// <inheritdoc/>
            unsafe BOOL IMsoComponentManager.FCreateSubComponentManager(
                IntPtr punkOuter,
                IntPtr punkServProv,
                Guid* riid,
                void** ppvObj)
            {
                // We do not support sub component managers.
                if (ppvObj != null)
                {
                    *ppvObj = null;
                }
                return BOOL.FALSE;
            }

            /// <inheritdoc/>
            BOOL IMsoComponentManager.FGetParentComponentManager(void** ppicm)
            {
                // We have no parent.
                if (ppicm != null)
                {
                    *ppicm = null;
                }
                return BOOL.FALSE;
            }

            /// <inheritdoc/>
            BOOL IMsoComponentManager.FGetActiveComponent(
                msogac dwgac,
                void** ppic,
                MSOCRINFO* pcrinfo,
                uint dwReserved)
            {
                IMsoComponent component = dwgac switch
                {
                    msogac.Active => _activeComponent,
                    msogac.Tracking => _trackingComponent,
                    msogac.TrackingOrActive => _trackingComponent ?? _activeComponent,
                    _ => null
                };

                if (component is null)
                    return BOOL.FALSE;

                if (pcrinfo != null)
                {
                    if (pcrinfo->cbSize < sizeof(MSOCRINFO))
                    {
                        return BOOL.FALSE;
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

                if (ppic != null)
                {
                    // This will addref the interface
                    *ppic = (void*)Marshal.GetComInterfaceForObject<IMsoComponent, IMsoComponent>(component);
                }

                return BOOL.TRUE;
            }
        }
    }
}
