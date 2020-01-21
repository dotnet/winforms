// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Mso
    {
        internal static partial class ComponentIds
        {
            public const string IID_IMsoComponentManager = "000C0601-0000-0000-C000-000000000046";
            public const string SID_SMsoComponentManager = "000C060B-0000-0000-C000-000000000046";
        }

        /// <remarks>
        ///  ** Comments on State Contexts **
        ///
        ///  <see cref="FCreateSubComponentManager"/> allows one to create a hierarchical tree of component managers.
        ///  This tree is used to maintain multiple contexts with regard to <see cref="msocstate"/> states. These
        ///  contexts are referred to as 'state contexts'. Each component manager in the tree defines a state context.
        ///  The components registered with a particular component manager or any of its descendents live within that
        ///  component manager's state context.
        ///
        ///  Calls to <see cref="OnComponentEnterState"/> and <see cref="FOnComponentExitState"/> can be used to affect
        ///  all components, only components within the component manager's state context, or only those components
        ///  that are outside of the component manager's state context. <see cref="FInState"/> is used to query the
        ///  state of the component manager's state context at its root.
        /// </remarks>
        [ComImport()]
        [Guid(ComponentIds.IID_IMsoComponentManager)]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IMsoComponentManager
        {
            /// <summary>
            ///  Returns in <paramref name="ppvObj"/> an implementation of interface <paramref name="iid"/>
            ///  for service <paramref name="guidService"/> (same as IServiceProvider::QueryService).
            /// </summary>
            /// <param name="ppvObj">The queried interface or null if failed.</param>
            /// <returns>
            ///  NOERROR if the requested service is supported, otherwise appropriate error such
            ///  as E_FAIL, E_NOINTERFACE.
            /// </returns>
            [PreserveSig]
            HRESULT QueryService(
                Guid* guidService,
                Guid* iid,
                void** ppvObj);

            /// <summary>
            ///  Standard <see cref="FDebugMessage"/> method.
            /// </summary>
            /// <remarks>
            ///  This method is reserved for internal use and is not intended to be used in your code.
            /// </remarks>
            /// <returns><see cref="BOOL.TRUE"/></returns>
            [PreserveSig]
            BOOL FDebugMessage(
                IntPtr dwReserved,
                uint msg,
                IntPtr wParam,
                IntPtr lParam);

            /// <summary>
            ///  Register component <paramref name="piComponent"/> and its registration info
            ///  <paramref name="pcrinfo"/> with this component manager.
            /// </summary>
            /// <param name="dwComponentID">
            ///  Returns a cookie which will identify the component when it calls other
            ///  <see cref="IMsoComponentManager"/> methods.
            /// </param>
            /// <returns><see cref="BOOL.TRUE"/> if successful.</returns>
            [PreserveSig]
            BOOL FRegisterComponent(
                IMsoComponent piComponent,
                MSOCRINFO* pcrinfo,
                UIntPtr* dwComponentID);

            /// <summary>
            ///  Undo the registration of the component identified by <paramref name="dwComponentID"/>
            ///  (the cookie returned from the <see cref="FRegisterComponent"/> method).
            /// </summary>
            /// <returns><see cref="BOOL.TRUE"/> if successful.</returns>
            [PreserveSig]
            BOOL FRevokeComponent(UIntPtr dwComponentID);

            /// <summary>
            ///  Update the registration info of the component identified by <paramref name="dwComponentID"/>
            ///  (the cookie returned from <see cref="FRegisterComponent"/>) with the new registration
            ///  information <paramref name="pcrinfo"/>.
            /// </summary>
            /// <remarks>
            ///  Typically this is used to update the idle time registration data, but it can be used to
            ///  update other registration data as well.
            /// </remarks>
            /// <returns><see cref="BOOL.TRUE"/> if successful.</returns>
            [PreserveSig]
            BOOL FUpdateComponentRegistration(
                UIntPtr dwComponentID,
                MSOCRINFO* pcrinfo);

            /// <summary>
            ///  Notify component manager that component identified by <paramref name="dwComponentID"/>
            ///  (cookie returned from <see cref="FRegisterComponent"/>) has been activated.
            /// </summary>
            /// <remarks>
            ///  The active component gets the chance to process messages before they are dispatched
            ///  (via <see cref="IMsoComponent.FPreTranslateMessage"/> and typically gets first chance
            ///  at idle time after the host.
            ///
            ///  This method fails if another component is already exclusively active. In this case,
            ///  <see cref="BOOL.FALSE"/> is returned and SetLastError is set to msoerrACompIsXActive
            ///  (comp usually need not take any special action in this case).
            /// </remarks>
            /// <returns><see cref="BOOL.TRUE"/> if successful.</returns>
            [PreserveSig]
            BOOL FOnComponentActivate(UIntPtr dwComponentID);

            /// <summary>
            ///  Called to inform component manager that component identified by <paramref name="dwComponentID"/>
            ///  (cookie returned from <see cref="FRegisterComponent"/>) wishes to perform a tracking operation
            ///  (such as mouse tracking).
            /// </summary>
            /// <remarks>
            ///  During the tracking operation the component manager routes messages to the tracking component
            ///  (via <see cref="IMsoComponent.FPreTranslateMessage"/>) rather than to the active component.
            ///  When the tracking operation ends, the component manager should resume routing messages to the active
            ///  component.
            ///
            ///  Note: component manager should perform no idle time processing during a tracking operation other
            ///  than give the tracking component idle time via <see cref="IMsoComponent.FDoIdle"/>.
            ///
            ///  Note: there can only be one tracking component at a time.
            /// </remarks>
            /// <param name="fTrack">
            ///  <see cref="BOOL.TRUE"/> to begin tracking operation. <see cref="BOOL.FALSE"/>
            ///  to end the operation.
            /// </param>
            /// <returns><see cref="BOOL.TRUE"/> if successful.</returns>
            [PreserveSig]
            BOOL FSetTrackingComponent(
                UIntPtr dwComponentID,
                BOOL fTrack);

            /// <summary>
            ///  Notify component manager that component identified by <paramref name="dwComponentID"/>
            ///  (cookie returned from <see cref="FRegisterComponent"/>) is entering the state identified
            ///  by <paramref name="uStateID"/>. (For convenience when dealing with sub CompMgrs, the host
            ///  can call this method passing 0 for <paramref name="dwComponentID"/>.
            /// </summary>
            /// <remarks>
            ///  Component manager should notify all other interested components within the state context
            ///  indicated by <paramref name="uContext"/>, excluding those within the state context of a
            ///  component manager in <paramref name="rgpicmExclude"/>, via
            ///  <see cref="IMsoComponent.OnEnterState"/> (see "Comments on State Contexts" in
            ///  <see cref="IMsoComponentManager"/> remarks).
            ///
            ///  Component Manager should also take appropriate action depending on the value of
            ///  <paramref name="uStateID"/>.
            ///
            ///  Note: Calls to this method are symmetric with calls to <see cref="FOnComponentExitState" />
            ///  That is, if n <see cref="OnComponentEnterState"/> calls are made, the component is
            ///  considered to be in the state until n <see cref="FOnComponentExitState" /> calls are
            ///  made.  Before revoking its registration a component must make a sufficient number of
            ///  <see cref="FOnComponentExitState" /> calls to offset any outstanding
            ///  <see cref="OnComponentEnterState"/> calls it has made.
            ///
            ///  Note: inplace objects should not call this method with <paramref name="uStateID" />
            ///  of <see cref="msocstate.Modal"/> when entering modal state. Such objects should call
            ///  <see cref="Ole32.IOleInPlaceFrame.EnableModeless"/> instead.
            /// </remarks>
            /// <param name="dwReserved">Reserved for future use. Should be zero.</param>
            /// <param name="cpicmExclude">Count of items in <paramref name="rgpicmExclude"/>.</param>
            /// <param name="rgpicmExclude">
            ///  Can be null. An array of component managers (can include root component manager and/or
            ///  sub component managers). Components within the state context of a component manager
            ///  appearing in this array should NOT be notified of the state change (note: if
            ///  <paramref name="uContext"/> is <see cref="msoccontext.Mine"/>, the only component
            ///  managers in <paramref name="rgpicmExclude"/> that are checked for exclusion are those that
            ///  are sub component managers of this component manager, since all other component
            ///  managers are outside of this component manager's state context anyway.)
            /// </param>
            [PreserveSig]
            void OnComponentEnterState(
                UIntPtr dwComponentID,
                msocstate uStateID,
                msoccontext uContext,
                uint cpicmExclude,
                /* IMsoComponentManager */ void** rgpicmExclude,
                uint dwReserved);

            /// <summary>
            ///  Notify component manager that component identified by <paramref name="dwComponentID"/>
            ///  (cookie returned from <see cref="FRegisterComponent"/>) is exiting the state identified by
            ///  <paramref name="uStateID"/>. For convenience when dealing with sub component managers, the
            ///  host can call this method passing 0 for <paramref name="dwComponentID"/>.
            /// </summary>
            /// <remarks>
            ///  <paramref name="uContext"/>, <paramref name="cpicmExclude"/>, and <paramref name="rgpicmExclude"/>
            ///  are as they are in <see cref="OnComponentEnterState"/>.
            ///
            ///  Component manager should notify all appropriate interested components (taking into account
            ///  <paramref name="uContext"/>, <paramref name="cpicmExclude"/>, <paramref name="rgpicmExclude"/>)
            ///  via <see cref="IMsoComponent.OnEnterState"/> (see "Comments on State Contexts", above).
            ///
            ///  Component manager should also take appropriate action depending on the value of <paramref name="uStateID"/>.
            ///
            ///  Note: n calls to this method are symmetric with n calls to <see cref="OnComponentEnterState"/>.
            /// </remarks>
            /// <returns>
            ///  <see cref="BOOL.TRUE"/> if, at the end of this call, the state is still in effect at the root
            ///  of this component manager's state context (because the host or some other component is still in the state),
            ///  otherwise return <see cref="BOOL.FALSE"/> (ie. return what <see cref="FInState"/> would return).
            ///
            ///  Callers can normally ignore the return value.
            /// </returns>
            [PreserveSig]
            BOOL FOnComponentExitState(
                UIntPtr dwComponentID,
                msocstate uStateID,
                msoccontext uContext,
                uint cpicmExclude,
                /* IMsoComponentManager */ void** rgpicmExclude);

            /// <summary>
            ///  Return <see cref="BOOL.TRUE"/> if the state identified by <paramref name="uStateID"/>
            ///  is in effect at the root of this component manager's state context, <see cref="BOOL.FALSE"/>
            ///  otherwise (see "Comments on State Contexts" in <see cref="IMsoComponentManager"/> remarks).
            /// </summary>
            /// <param name="pvoid">Reserved for future use and should be <see langword="null" />.</param>
            [PreserveSig]
            BOOL FInState(
                msocstate uStateID,
                void* pvoid);

            /// <summary>
            ///  Called periodically by a component during <see cref="IMsoComponent.FDoIdle"/>.
            /// </summary>
            /// <returns>
            ///  <see cref="BOOL.TRUE"/> if component can continue its idle time processing,
            ///  <see cref="BOOL.FALSE"/> if not (in which case component returns from FDoIdle.)
            /// </returns>
            [PreserveSig]
            BOOL FContinueIdle();

            /// <summary>
            ///  Component identified by <paramref name="dwComponentID"/> (cookie returned from
            ///  <see cref="FRegisterComponent"/>) wishes to push a message loop for reason
            ///  <paramref name="uReason"/>.
            /// </summary>
            /// <remarks>
            ///  The component manager should push its message loop, calling
            ///  <see cref="IMsoComponent.FContinueMessageLoop"/> during each loop iteration. When
            ///  <see cref="IMsoComponent.FContinueMessageLoop"/> returns <see cref="BOOL.FALSE"/>,
            ///  the component manager terminates the loop.
            /// </remarks>
            /// <param name="pvLoopData">Data private to the component.</param>
            /// <returns>
            ///  <see cref="BOOL.TRUE"/> if component manager terminates loop because component told it
            ///  to (by returning <see cref="BOOL.FALSE"/> from <see cref="IMsoComponent.FContinueMessageLoop"/>),
            ///  <see cref="BOOL.FALSE"/> if it had to terminate the loop for some other reason.  In the
            ///  latter case, component should perform any necessary action (such as cleanup).
            /// </returns>
            [PreserveSig]
            BOOL FPushMessageLoop(
                UIntPtr dwComponentID,
                msoloop uReason,
                void* pvLoopData);

            /// <summary>
            ///  Cause the component manager to create a "sub" component manager, which  will be one of its
            ///  children in the hierarchical tree of component managers used to maintain state contexts
            ///  (see "Comments on State Contexts" in <see cref="IMsoComponentManager"/> remarks).
            /// </summary>
            /// <param name="punkOuter">The controlling unknown. Can be null.</param>
            /// <param name="riid">Desired interface identifier (IID).</param>
            /// <param name="ppvObj">The created sub-component manager.</param>
            /// <param name="punkServProv">
            ///  IUnknown object that supports IServiceProvider that the sub component manager should
            ///  delegate <see cref="QueryService" /> calls to. Can be null.
            /// </param>
            /// <returns><see cref="BOOL.TRUE"/> if successful.</returns>
            [PreserveSig]
            BOOL FCreateSubComponentManager(
                IntPtr punkOuter,
                IntPtr punkServProv,
                Guid* riid,
                void** ppvObj);

            /// <summary>
            ///  Return in <paramref name="ppicm"/> an AddRef'ed ptr to this component manager's parent
            ///  in the hierarchical tree of component managers used to maintain state contexts (see
            ///  "Comments on State Contexts" in <see cref="IMsoComponentManager"/> remarks).
            /// </summary>
            /// <returns>
            ///  <see cref="BOOL.TRUE"/> if the parent is returned, <see cref="BOOL.FALSE"/>
            ///  if no parent exists or some error occurred.
            /// </returns>
            [PreserveSig]
            BOOL FGetParentComponentManager(
                /* IMsoComponentManager */ void** ppicm);

            /// <summary>
            ///  Return in <paramref name="ppic"/> an AddRef'ed ptr to the current active
            ///  or tracking component (as indicated by <paramref name="dwgac"/>, and its
            ///  registration information in <paramref name="pcrinfo"/>.
            ///
            ///  <paramref name="ppic"/> and/or <paramref name="pcrinfo"/> can be
            ///  NULL if caller is not interested these values.  If <paramref name="pcrinfo"/>
            ///  is not <see langword="null" /> caller should set <see cref="MSOCRINFO.cbSize"/>
            ///  before calling this method.
            /// </summary>
            /// <param name="dwReserved">Reserved for future use and should be zero.</param>
            /// <returns>
            ///  <see cref="BOOL.TRUE"/> if the component indicated by <paramref name="dwgac"/>
            ///  exists, <see cref="BOOL.FALSE"/> if no such component exists or some error occurred.
            /// </returns>
            [PreserveSig]
            BOOL FGetActiveComponent(
                msogac dwgac,
                /* IMsoComponent */ void** ppic,
                MSOCRINFO* pcrinfo,
                uint dwReserved);
        }
    }
}
