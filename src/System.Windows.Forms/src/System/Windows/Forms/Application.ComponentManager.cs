// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

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
        private class ComponentManager : UnsafeNativeMethods.IMsoComponentManager
        {
            // ComponentManager instance data.
            //
            private class ComponentHashtableEntry
            {
                public UnsafeNativeMethods.IMsoComponent component;
                public NativeMethods.MSOCRINFOSTRUCT componentInfo;
            }

            private Hashtable oleComponents;
            private int cookieCounter = 0;
            private UnsafeNativeMethods.IMsoComponent activeComponent = null;
            private UnsafeNativeMethods.IMsoComponent trackingComponent = null;
            private int currentState = 0;

            private Hashtable OleComponents
            {
                get
                {
                    if (oleComponents == null)
                    {
                        oleComponents = new Hashtable();
                        cookieCounter = 0;
                    }

                    return oleComponents;
                }
            }

            /// <summary>
            ///  Return in *ppvObj an implementation of interface iid for service
            ///  guidService (same as IServiceProvider::QueryService).
            ///  Return NOERROR if the requested service is supported, otherwise return
            ///  NULL in *ppvObj and an appropriate error (eg E_FAIL, E_NOINTERFACE).
            /// </summary>
            int UnsafeNativeMethods.IMsoComponentManager.QueryService(
                                                 ref Guid guidService,
                                                 ref Guid iid,
                                                 out object ppvObj)
            {

                ppvObj = null;
                return NativeMethods.E_NOINTERFACE;

            }

            /// <summary>
            ///  Standard FDebugMessage method.
            ///  Since IMsoComponentManager is a reference counted interface,
            ///  MsoDWGetChkMemCounter should be used when processing the
            ///  msodmWriteBe message.
            /// </summary>
            bool UnsafeNativeMethods.IMsoComponentManager.FDebugMessage(
                                                   IntPtr hInst,
                                                   int msg,
                                                   IntPtr wparam,
                                                   IntPtr lparam)
            {

                return true;
            }

            /// <summary>
            ///  Register component piComponent and its registration info pcrinfo with
            ///  this component manager.  Return in *pdwComponentID a cookie which will
            ///  identify the component when it calls other IMsoComponentManager
            ///  methods.
            ///  Return TRUE if successful, FALSE otherwise.
            /// </summary>
            bool UnsafeNativeMethods.IMsoComponentManager.FRegisterComponent(UnsafeNativeMethods.IMsoComponent component,
                                                         NativeMethods.MSOCRINFOSTRUCT pcrinfo,
                                                         out IntPtr dwComponentID)
            {

                // Construct Hashtable entry for this component
                //
                ComponentHashtableEntry entry = new ComponentHashtableEntry
                {
                    component = component,
                    componentInfo = pcrinfo
                };
                OleComponents.Add(++cookieCounter, entry);

                // Return the cookie
                //
                dwComponentID = (IntPtr)cookieCounter;
                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager: Component registered.  ID: " + cookieCounter.ToString(CultureInfo.InvariantCulture));
                return true;
            }

            /// <summary>
            ///  Undo the registration of the component identified by dwComponentID
            ///  (the cookie returned from the FRegisterComponent method).
            ///  Return TRUE if successful, FALSE otherwise.
            /// </summary>
            bool UnsafeNativeMethods.IMsoComponentManager.FRevokeComponent(IntPtr dwComponentID)
            {
                int dwLocalComponentID = unchecked((int)(long)dwComponentID);

                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager: Revoking component " + dwLocalComponentID.ToString(CultureInfo.InvariantCulture));

                ComponentHashtableEntry entry = (ComponentHashtableEntry)OleComponents[dwLocalComponentID];
                if (entry == null)
                {
                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "Compoenent not registered.");
                    return false;
                }

                if (entry.component == activeComponent)
                {
                    activeComponent = null;
                }
                if (entry.component == trackingComponent)
                {
                    trackingComponent = null;
                }

                OleComponents.Remove(dwLocalComponentID);

                return true;

            }

            /// <summary>
            ///  Update the registration info of the component identified by
            ///  dwComponentID (the cookie returned from FRegisterComponent) with the
            ///  new registration information pcrinfo.
            ///  Typically this is used to update the idle time registration data, but
            ///  can be used to update other registration data as well.
            ///  Return TRUE if successful, FALSE otherwise.
            /// </summary>
            bool UnsafeNativeMethods.IMsoComponentManager.FUpdateComponentRegistration(
                                                                  IntPtr dwComponentID,
                                                                  NativeMethods.MSOCRINFOSTRUCT info
                                                                  )
            {
                int dwLocalComponentID = unchecked((int)(long)dwComponentID);
                // Update the registration info
                //
                ComponentHashtableEntry entry = (ComponentHashtableEntry)OleComponents[dwLocalComponentID];
                if (entry == null)
                {
                    return false;
                }

                entry.componentInfo = info;

                return true;
            }

            /// <summary>
            ///  Notify component manager that component identified by dwComponentID
            ///  (cookie returned from FRegisterComponent) has been activated.
            ///  The active component gets the chance to process messages before they
            ///  are dispatched (via IMsoComponent::FPreTranslateMessage) and typically
            ///  gets first chance at idle time after the host.
            ///  This method fails if another component is already Exclusively Active.
            ///  In this case, FALSE is returned and SetLastError is set to
            ///  msoerrACompIsXActive (comp usually need not take any special action
            ///  in this case).
            ///  Return TRUE if successful.
            /// </summary>
            bool UnsafeNativeMethods.IMsoComponentManager.FOnComponentActivate(IntPtr dwComponentID)
            {

                int dwLocalComponentID = unchecked((int)(long)dwComponentID);
                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager: Component activated.  ID: " + dwLocalComponentID.ToString(CultureInfo.InvariantCulture));

                ComponentHashtableEntry entry = (ComponentHashtableEntry)OleComponents[dwLocalComponentID];
                if (entry == null)
                {
                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "*** Component not registered ***");
                    return false;
                }

                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "New active component is : " + entry.component.ToString());
                activeComponent = entry.component;
                return true;
            }

            /// <summary>
            ///  Called to inform component manager that  component identified by
            ///  dwComponentID (cookie returned from FRegisterComponent) wishes
            ///  to perform a tracking operation (such as mouse tracking).
            ///  The component calls this method with fTrack == TRUE to begin the
            ///  tracking operation and with fTrack == FALSE to end the operation.
            ///  During the tracking operation the component manager routes messages
            ///  to the tracking component (via IMsoComponent::FPreTranslateMessage)
            ///  rather than to the active component.  When the tracking operation ends,
            ///  the component manager should resume routing messages to the active
            ///  component.
            ///  Note: component manager should perform no idle time processing during a
            ///    tracking operation other than give the tracking component idle
            ///    time via IMsoComponent::FDoIdle.
            ///  Note: there can only be one tracking component at a time.
            ///  Return TRUE if successful, FALSE otherwise.
            /// </summary>
            bool UnsafeNativeMethods.IMsoComponentManager.FSetTrackingComponent(IntPtr dwComponentID, bool fTrack)
            {

                int dwLocalComponentID = unchecked((int)(long)dwComponentID);
                ComponentHashtableEntry entry = (ComponentHashtableEntry)OleComponents[dwLocalComponentID];
                if (entry == null)
                {
                    return false;
                }

                if (entry.component == trackingComponent ^ fTrack)
                {
                    return false;
                }

                if (fTrack)
                {
                    trackingComponent = entry.component;
                }
                else
                {
                    trackingComponent = null;
                }

                return true;
            }

            /// <summary>
            ///  Notify component manager that component identified by dwComponentID
            ///  (cookie returned from FRegisterComponent) is entering the state
            ///  identified by uStateID (msocstateXXX value).  (For convenience when
            ///  dealing with sub CompMgrs, the host can call this method passing 0 for
            ///  dwComponentID.)
            ///  Component manager should notify all other interested components within
            ///  the state context indicated by uContext (a msoccontextXXX value),
            ///  excluding those within the state context of a CompMgr in rgpicmExclude,
            ///  via IMsoComponent::OnEnterState (see "Comments on State Contexts",
            ///  above).
            ///  Component Manager should also take appropriate action depending on the
            ///  value of uStateID (see msocstate comments, above).
            ///  dwReserved is reserved for future use and should be zero.
            ///
            ///  rgpicmExclude (can be NULL) is an array of cpicmExclude CompMgrs (can
            ///  include root CompMgr and/or sub CompMgrs); components within the state
            ///  context of a CompMgr appearing in this     array should NOT be notified of
            ///  the state change (note: if uContext        is msoccontextMine, the only
            ///  CompMgrs in rgpicmExclude that are checked for exclusion are those that
            ///  are sub CompMgrs of this Component Manager, since all other CompMgrs
            ///  are outside of this Component Manager's state context anyway.)
            ///
            ///  Note: Calls to this method are symmetric with calls to
            ///  FOnComponentExitState.
            ///  That is, if n OnComponentEnterState calls are made, the component is
            ///  considered to be in the state until n FOnComponentExitState calls are
            ///  made.  Before revoking its registration a component must make a
            ///  sufficient number of FOnComponentExitState calls to offset any
            ///  outstanding OnComponentEnterState calls it has made.
            ///
            ///  Note: inplace objects should not call this method with
            ///  uStateID == msocstateModal when entering modal state. Such objects
            ///  should call IOleInPlaceFrame::EnableModeless instead.
            /// </summary>
            void UnsafeNativeMethods.IMsoComponentManager.OnComponentEnterState(
                                                           IntPtr dwComponentID,
                                                           int uStateID,
                                                           int uContext,
                                                           int cpicmExclude,
                                                           int rgpicmExclude,          // IMsoComponentManger**
                                                           int dwReserved)
            {

                int dwLocalComponentID = unchecked((int)(long)dwComponentID);
                currentState |= uStateID;

                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager: Component enter state.  ID: " + dwLocalComponentID.ToString(CultureInfo.InvariantCulture) + " state: " + uStateID.ToString(CultureInfo.InvariantCulture));

                if (uContext == NativeMethods.MSOCM.msoccontextAll || uContext == NativeMethods.MSOCM.msoccontextMine)
                {

                    Debug.Indent();

                    // We should notify all components we contain that the state has changed.
                    //
                    foreach (ComponentHashtableEntry entry in OleComponents.Values)
                    {
                        Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "Notifying " + entry.component.ToString());
                        entry.component.OnEnterState(uStateID, true);
                    }

                    Debug.Unindent();
                }
            }

            /// <summary>
            ///  Notify component manager that component identified by dwComponentID
            ///  (cookie returned from FRegisterComponent) is exiting the state
            ///  identified by uStateID (a msocstateXXX value).  (For convenience when
            ///  dealing with sub CompMgrs, the host can call this method passing 0 for
            ///  dwComponentID.)
            ///  uContext, cpicmExclude, and rgpicmExclude are as they are in
            ///  OnComponentEnterState.
            ///  Component manager  should notify all appropriate interested components
            ///  (taking into account uContext, cpicmExclude, rgpicmExclude) via
            ///  IMsoComponent::OnEnterState (see "Comments on State Contexts", above).
            ///  Component Manager should also take appropriate action depending on
            ///  the value of uStateID (see msocstate comments, above).
            ///  Return TRUE if, at the end of this call, the state is still in effect
            ///  at the root of this component manager's state context
            ///  (because the host or some other component is still in the state),
            ///  otherwise return FALSE (ie. return what FInState would return).
            ///  Caller can normally ignore the return value.
            ///
            ///  Note: n calls to this method are symmetric with n calls to
            ///  OnComponentEnterState (see OnComponentEnterState comments, above).
            /// </summary>
            bool UnsafeNativeMethods.IMsoComponentManager.FOnComponentExitState(
                                                           IntPtr dwComponentID,
                                                           int uStateID,
                                                           int uContext,
                                                           int cpicmExclude,
                                                           int rgpicmExclude       // IMsoComponentManager**
                                                           )
            {
                int dwLocalComponentID = unchecked((int)(long)dwComponentID);
                currentState &= ~uStateID;

                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager: Component exit state.  ID: " + dwLocalComponentID.ToString(CultureInfo.InvariantCulture) + " state: " + uStateID.ToString(CultureInfo.InvariantCulture));

                if (uContext == NativeMethods.MSOCM.msoccontextAll || uContext == NativeMethods.MSOCM.msoccontextMine)
                {

                    Debug.Indent();

                    // We should notify all components we contain that the state has changed.
                    //
                    foreach (ComponentHashtableEntry entry in OleComponents.Values)
                    {
                        Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "Notifying " + entry.component.ToString());
                        entry.component.OnEnterState(uStateID, false);
                    }

                    Debug.Unindent();
                }

                return false;
            }

            /// <summary>
            ///  Return TRUE if the state identified by uStateID (a msocstateXXX value)
            ///  is in effect at the root of this component manager's state context,
            ///  FALSE otherwise (see "Comments on State Contexts", above).
            ///  pvoid is reserved for future use and should be NULL.
            /// </summary>
            bool UnsafeNativeMethods.IMsoComponentManager.FInState(int uStateID, IntPtr pvoid)
            {
                return (currentState & uStateID) != 0;
            }

            /// <summary>
            ///  Called periodically by a component during IMsoComponent::FDoIdle.
            ///  Return TRUE if component can continue its idle time processing,
            ///  FALSE if not (in which case component returns from FDoIdle.)
            /// </summary>
            bool UnsafeNativeMethods.IMsoComponentManager.FContinueIdle()
            {

                // Essentially, if we have a message on queue, then don't continue
                // idle processing.
                //
                NativeMethods.MSG msg = new NativeMethods.MSG();
                return !UnsafeNativeMethods.PeekMessage(ref msg, NativeMethods.NullHandleRef, 0, 0, NativeMethods.PM_NOREMOVE);
            }

            /// <summary>
            ///  Component identified by dwComponentID (cookie returned from
            ///  FRegisterComponent) wishes to push a message loop for reason uReason.
            ///  uReason is one the values from the msoloop enumeration (above).
            ///  pvLoopData is data private to the component.
            ///  The component manager should push its message loop,
            ///  calling IMsoComponent::FContinueMessageLoop(uReason, pvLoopData)
            ///  during each loop iteration (see IMsoComponent::FContinueMessageLoop
            ///  comments).  When IMsoComponent::FContinueMessageLoop returns FALSE, the
            ///  component manager terminates the loop.
            ///  Returns TRUE if component manager terminates loop because component
            ///  told it to (by returning FALSE from IMsoComponent::FContinueMessageLoop),
            ///  FALSE if it had to terminate the loop for some other reason.  In the
            ///  latter case, component should perform any necessary action (such as
            ///  cleanup).
            /// </summary>
            bool UnsafeNativeMethods.IMsoComponentManager.FPushMessageLoop(
                                                      IntPtr dwComponentID,
                                                      int reason,
                                                      int pvLoopData          // PVOID
                                                      )
            {

                int dwLocalComponentID = unchecked((int)(long)dwComponentID);
                // Hold onto old state to allow restore before we exit...
                //
                int currentLoopState = currentState;
                bool continueLoop = true;

                if (!OleComponents.ContainsKey(dwLocalComponentID))
                {
                    return false;
                }

                UnsafeNativeMethods.IMsoComponent prevActive = activeComponent;

                try
                {
                    // Execute the message loop until the active component tells us to stop.
                    //
                    NativeMethods.MSG msg = new NativeMethods.MSG();
                    NativeMethods.MSG[] rgmsg = new NativeMethods.MSG[] { msg };
                    bool unicodeWindow = false;
                    UnsafeNativeMethods.IMsoComponent requestingComponent;

                    ComponentHashtableEntry entry = (ComponentHashtableEntry)OleComponents[dwLocalComponentID];
                    if (entry == null)
                    {
                        return false;
                    }

                    requestingComponent = entry.component;

                    activeComponent = requestingComponent;

                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager : Pushing message loop " + reason.ToString(CultureInfo.InvariantCulture));
                    Debug.Indent();

                    while (continueLoop)
                    {

                        // Determine the component to route the message to
                        //
                        UnsafeNativeMethods.IMsoComponent component;

                        if (trackingComponent != null)
                        {
                            component = trackingComponent;
                        }
                        else if (activeComponent != null)
                        {
                            component = activeComponent;
                        }
                        else
                        {
                            component = requestingComponent;
                        }

                        bool peeked = UnsafeNativeMethods.PeekMessage(ref msg, NativeMethods.NullHandleRef, 0, 0, NativeMethods.PM_NOREMOVE);

                        if (peeked)
                        {

                            rgmsg[0] = msg;
                            continueLoop = component.FContinueMessageLoop(reason, pvLoopData, rgmsg);

                            // If the component wants us to process the message, do it.
                            // The component manager hosts windows from many places.  We must be sensitive
                            // to ansi / Unicode windows here.
                            //
                            if (continueLoop)
                            {
                                if (msg.hwnd != IntPtr.Zero && SafeNativeMethods.IsWindowUnicode(new HandleRef(null, msg.hwnd)))
                                {
                                    unicodeWindow = true;
                                    UnsafeNativeMethods.GetMessageW(ref msg, NativeMethods.NullHandleRef, 0, 0);
                                }
                                else
                                {
                                    unicodeWindow = false;
                                    UnsafeNativeMethods.GetMessageA(ref msg, NativeMethods.NullHandleRef, 0, 0);
                                }

                                if (msg.message == Interop.WindowMessages.WM_QUIT)
                                {
                                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager : Normal message loop termination");

                                    Application.ThreadContext.FromCurrent().DisposeThreadWindows();

                                    if (reason != NativeMethods.MSOCM.msoloopMain)
                                    {
                                        UnsafeNativeMethods.PostQuitMessage((int)msg.wParam);
                                    }

                                    continueLoop = false;
                                    break;
                                }

                                // Now translate and dispatch the message.
                                //
                                // Reading through the rather sparse documentation,
                                // it seems we should only call FPreTranslateMessage
                                // on the active component.
                                if (!component.FPreTranslateMessage(ref msg))
                                {
                                    UnsafeNativeMethods.TranslateMessage(ref msg);
                                    if (unicodeWindow)
                                    {
                                        UnsafeNativeMethods.DispatchMessageW(ref msg);
                                    }
                                    else
                                    {
                                        UnsafeNativeMethods.DispatchMessageA(ref msg);
                                    }
                                }
                            }
                        }
                        else
                        {

                            // If this is a DoEvents loop, then get out.  There's nothing left
                            // for us to do.
                            //
                            if (reason == NativeMethods.MSOCM.msoloopDoEvents ||
                                reason == NativeMethods.MSOCM.msoloopDoEventsModal)
                            {
                                break;
                            }

                            // Nothing is on the message queue.  Perform idle processing
                            // and then do a WaitMessage.
                            //
                            bool continueIdle = false;

                            if (OleComponents != null)
                            {
                                IEnumerator enumerator = OleComponents.Values.GetEnumerator();

                                while (enumerator.MoveNext())
                                {
                                    ComponentHashtableEntry idleEntry = (ComponentHashtableEntry)enumerator.Current;
                                    continueIdle |= idleEntry.component.FDoIdle(-1);
                                }
                            }

                            // give the component one more chance to terminate the
                            // message loop.
                            //
                            continueLoop = component.FContinueMessageLoop(reason, pvLoopData, null);

                            if (continueLoop)
                            {
                                if (continueIdle)
                                {
                                    // If someone has asked for idle time, give it to them.  However,
                                    // don't cycle immediately; wait up to 100ms.  Why?  Because we don't
                                    // want someone to attach to idle, forget to detach, and then cause
                                    // CPU to end up in race condition.  For Windows Forms this generally isn't an issue because
                                    // our component always returns false from its idle request
                                    UnsafeNativeMethods.MsgWaitForMultipleObjectsEx(0, IntPtr.Zero, 100, NativeMethods.QS_ALLINPUT, NativeMethods.MWMO_INPUTAVAILABLE);
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
                                    //
                                    if (!UnsafeNativeMethods.PeekMessage(ref msg, NativeMethods.NullHandleRef, 0, 0, NativeMethods.PM_NOREMOVE))
                                    {
                                        UnsafeNativeMethods.WaitMessage();
                                    }
                                }
                            }
                        }
                    }

                    Debug.Unindent();
                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager : message loop " + reason.ToString(CultureInfo.InvariantCulture) + " complete.");
                }
                finally
                {
                    currentState = currentLoopState;
                    activeComponent = prevActive;
                }

                return !continueLoop;
            }

            /// <summary>
            ///  Cause the component manager to create a "sub" component manager, which
            ///  will be one of its children in the hierarchical tree of component
            ///  managers used to maintiain state contexts (see "Comments on State
            ///  Contexts", above).
            ///  piunkOuter is the controlling unknown (can be NULL), riid is the
            ///  desired IID, and *ppvObj returns   the created sub component manager.
            ///  piunkServProv (can be NULL) is a ptr to an object supporting
            ///  IServiceProvider interface to which the created sub component manager
            ///  will delegate its IMsoComponentManager::QueryService calls.
            ///  (see objext.h or docobj.h for definition of IServiceProvider).
            ///  Returns TRUE if successful.
            /// </summary>
            bool UnsafeNativeMethods.IMsoComponentManager.FCreateSubComponentManager(
                                                                object punkOuter,
                                                                object punkServProv,
                                                                ref Guid riid,
                                                                out IntPtr ppvObj)
            {

                // We do not support sub component managers.
                //
                ppvObj = IntPtr.Zero;
                return false;
            }

            /// <summary>
            ///  Return in *ppicm an AddRef'ed ptr to this component manager's parent
            ///  in the hierarchical tree of component managers used to maintain state
            ///  contexts (see "Comments on State   Contexts", above).
            ///  Returns TRUE if the parent is returned, FALSE if no parent exists or
            ///  some error occurred.
            /// </summary>
            bool UnsafeNativeMethods.IMsoComponentManager.FGetParentComponentManager(out UnsafeNativeMethods.IMsoComponentManager ppicm)
            {
                ppicm = null;
                return false;
            }

            /// <summary>
            ///  Return in *ppic an AddRef'ed ptr to the current active or tracking
            ///  component (as indicated by dwgac (a msogacXXX value)), and
            ///  its registration information in *pcrinfo.  ppic and/or pcrinfo can be
            ///  NULL if caller is not interested these values.  If pcrinfo is not NULL,
            ///  caller should set pcrinfo->cbSize before calling this method.
            ///  Returns TRUE if the component indicated by dwgac exists, FALSE if no
            ///  such component exists or some error occurred.
            ///  dwReserved is reserved for future use and should be zero.
            /// </summary>
            bool UnsafeNativeMethods.IMsoComponentManager.FGetActiveComponent(
                                                         int dwgac,
                                                         UnsafeNativeMethods.IMsoComponent[] ppic,
                                                         NativeMethods.MSOCRINFOSTRUCT info,
                                                         int dwReserved)
            {

                UnsafeNativeMethods.IMsoComponent component = null;

                if (dwgac == NativeMethods.MSOCM.msogacActive)
                {
                    component = activeComponent;
                }
                else if (dwgac == NativeMethods.MSOCM.msogacTracking)
                {
                    component = trackingComponent;
                }
                else if (dwgac == NativeMethods.MSOCM.msogacTrackingOrActive)
                {
                    if (trackingComponent != null)
                    {
                        component = trackingComponent;
                    }
                    else
                    {
                        component = activeComponent;
                    }
                }
                else
                {
                    Debug.Fail("Unknown dwgac in FGetActiveComponent");
                }

                if (ppic != null)
                {
                    ppic[0] = component;
                }
                if (info != null && component != null)
                {
                    foreach (ComponentHashtableEntry entry in OleComponents.Values)
                    {
                        if (entry.component == component)
                        {
                            info = entry.componentInfo;
                            break;
                        }
                    }
                }

                return component != null;
            }
        }
    }
}
