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
            public const string IID_IMsoComponent = "000C0600-0000-0000-C000-000000000046";
        }

        /// <remarks>
        ///  WM_MOUSEACTIVATE Note (for top level compoenents and host)
        ///
        ///  If the active (or tracking) comp's reg info indicates that it wants mouse
        ///  messages, then no MA_xxxANDEAT value should be returned  from WM_MOUSEACTIVATE,
        ///  so that the active (or tracking) comp will be able to process the resulting mouse
        ///  message. If one does not want to examine the reg info, no MA_xxxANDEAT value
        ///  should be returned from WM_MOUSEACTIVATE if any comp is active (or tracking). One
        ///  can query the reg info  of the active (or tracking) component at any time via
        ///  <see cref="IMsoComponentManager.FGetActiveComponent"/>.
        /// </remarks>
        [ComImport()]
        [Guid(ComponentIds.IID_IMsoComponent)]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IMsoComponent
        {
            /// <summary>
            ///  Standard <see cref="FDebugMessage"/> method.
            /// </summary>
            /// <remarks>
            ///  This method is reserved for internal use and is not intended to be used in your code.
            /// </remarks>
            /// <returns><see cref="BOOL.TRUE"/></returns>
            [PreserveSig]
            BOOL FDebugMessage(
                IntPtr hInst,
                uint msg,
                IntPtr wParam,
                IntPtr lParam);

            /// <summary>
            ///  Give component a chance to process <paramref name="msg"/> before it is translated
            ///  and dispatched. Component can modify <paramref name="msg"/>, call Win32 APIs such
            ///  as TranslateAccelerator or IsDialogMessage, or take some other action.
            /// </summary>
            /// <returns>
            ///  Return <see cref="BOOL.TRUE"/> if the message is consumed,
            ///  <see cref="BOOL.FALSE"/> otherwise.
            /// </returns>
            [PreserveSig]
            BOOL FPreTranslateMessage(User32.MSG* msg);

            /// <summary>
            ///  Notify component when app enters or exits (as indicated by <paramref name="fEnter"/>)
            ///  the state identified by <paramref name="uStateID"/>. Component should take action
            ///  depending on value of <paramref name="uStateID"/> (see olecstate comments, above).
            /// </summary>
            /// <remarks>
            ///  Note: If n calls are made with TRUE <paramref name="fEnter"/>, component should consider
            ///  the state to be in effect until n calls are made with FALSE <paramref name="fEnter"/>.
            ///
            ///  Note: Components should be aware that it is possible for this method to be called
            ///  with FALSE <paramref name="fEnter"/> more times than it was called with TRUE
            ///  <paramref name="fEnter"/> (so, for example, if component is maintaining a state counter
            ///  incremented when this method is called with BOOL.TRUE <paramref name="fEnter"/>,
            ///  decremented when called with FALSE <paramref name="fEnter"/>), the counter should not
            ///  be decremented for FALSE <paramref name="fEnter"/> if it is already at zero.)
            /// </remarks>
            [PreserveSig]
            void OnEnterState(
                msocstate uStateID,
                BOOL fEnter);

            /// <summary>
            ///  Notify component when the host application gains or loses activation.
            /// </summary>
            /// <remarks>
            ///  Note: this method is not called when both the window being activated and the one being
            ///  deactivated belong to the host app.
            /// </remarks>
            /// <param name="fActive">
            ///  If <see cref="BOOL.TRUE"/>, the host app is being activated and <paramref name="dwOtherThreadID"/>
            ///  is the ID of the thread owning the window being deactivated.
            ///
            ///  If <see cref="BOOL.FALSE"/>, the host app is being deactivated and <paramref name="dwOtherThreadID"/>
            ///  is the ID of the thread owning the window being activated.
            /// </param>
            [PreserveSig]
            void OnAppActivate(
                BOOL fActive,
                uint dwOtherThreadID);

            /// <summary>
            ///  Notify the active component that it has lost its active status because the host or
            ///  another component has become active.
            /// </summary>
            [PreserveSig]
            void OnLoseActivation();

            /// <summary>
            ///  Notify component when a new object is being activated.
            /// </summary>
            /// <remarks>
            ///  If <paramref name="pic"/> is being activated and <paramref name="pcrinfo"/>
            ///  <see cref="MSOCRINFO.grfcrf"/> has the <see cref="msocrf.ExclusiveBorderSpace"/>
            ///  bit set, component should hide its border space tools (toolbars, status bars, etc.);
            ///  component should also do this if host is activating and <paramref name="pchostinfo"/>
            ///  has the <see cref="msocrf.ExclusiveBorderSpace"/> bit set. In either of these cases,
            ///  component should unhide its border space tools the next time it is activated.
            ///
            ///  If <paramref name="pic"/> is being activated and <paramref name="pcrinfo"/>
            ///  <see cref="MSOCRINFO.grfcrf"/> has the <see cref="msocrf.ExclusiveActivation"/>
            ///  bit set, then <paramref name="pic"/> is being activated in "ExclusiveActive" mode.
            ///  Component should retrieve the top frame window that is hosting <paramref name="pic"/>
            ///  (via <see cref="HwndGetWindow"/> with <see cref="msocWindow.FrameToplevel"/>).
            ///
            ///  If this window is different from component's own top frame window, component should
            ///  disable its windows and do other things it would do when receiving
            ///  <see cref="OnEnterState"/> with <see cref="msocstate.Modal" /> notification.
            ///
            ///  Otherwise, if component is top-level, it should refuse to have its window activated
            ///  by appropriately processing WM_MOUSEACTIVATE (but see WM_MOUSEACTIVATE NOTE, above).
            ///  Component should remain in one of these states until the exclusive active mode ends,
            ///  indicated by a future call to <see cref="OnActivationChange"/> with
            ///  <see cref="msocrf.ExclusiveActivation" /> bit not set or with null
            ///  <paramref name="pcrinfo"/>.
            /// </remarks>
            /// <param name="pic">
            ///  If non-null, then it is the component that is being activated. In this case,
            ///  <paramref name="fSameComponent"/> is TRUE if <paramref name="pic"/> is the same
            ///  component as the callee of this method, and <paramref name="pcrinfo"/> is the
            ///  reg info of <paramref name="pic"/>.
            ///
            ///  If null and <paramref name="fHostIsActivating"/> is TRUE, then the host is the
            ///  object being activated, and <paramref name="pchostinfo"/> is its host info.
            ///
            ///  If null and <paramref name="fHostIsActivating"/> is FALSE, then there is no
            ///  current active object.
            /// </param>
            [PreserveSig]
            void OnActivationChange(
                IMsoComponent? pic,
                BOOL fSameComponent,
                MSOCRINFO* pcrinfo,
                BOOL fHostIsActivating,
                IntPtr pchostinfo,
                uint dwReserved);

            /// <summary>
            ///  Give component a chance to do idle time tasks.  grfidlef is a group of
            ///  bit flags taken from the enumeration of <see cref="msoidlef"/> values,
            ///  indicating the type of idle tasks to perform.
            /// </summary>
            /// <remarks>
            ///  Component may periodically call <see cref="IMsoComponentManager.FContinueIdle"/>;
            ///  if this method returns FALSE, component should terminate its idle time
            ///  processing and return.
            ///
            ///  Note: If a component reaches a point where it has no idle tasks and does not
            ///  need <see cref="FDoIdle"/> calls, it should remove its idle task registration
            ///  via <see cref="IMsoComponentManager.FUpdateComponentRegistration"/>.
            ///
            ///  Note: If this method is called on while component is performing a tracking
            ///  operation, component should only perform idle time tasks that it deems are
            ///  appropriate to perform during tracking.
            /// </remarks>
            /// <returns>
            ///  <see cref="BOOL.TRUE"/> if more time is needed to perform the idle time tasks,
            ///  <see cref="BOOL.FALSE"/> otherwise.
            /// </returns>
            [PreserveSig]
            BOOL FDoIdle(
                msoidlef grfidlef);

            /// <summary>
            ///  Called during each iteration of a message loop that the component pushed.
            /// </summary>
            /// <remarks>
            ///  This method is called after peeking the next message in the queue (via PeekMessage)
            ///  but before the message is removed from the queue. The peeked message is passed in
            ///  the <paramref name="pMsgPeeked"/> param (null if no message is in the queue).
            ///
            ///  This method may be additionally called when the next message has already been removed
            ///  from the queue, in which case <paramref name="pMsgPeeked"/> is passed as null.
            /// </remarks>
            /// <param name="uReason">
            ///  Reason that was passed to <see cref="IMsoComponentManager.FPushMessageLoop"/>.
            /// </param>
            /// <param name="pvLoopData">
            ///  Component private data passed to <see cref="IMsoComponentManager.FPushMessageLoop"/>.
            /// </param>
            /// <returns>
            ///  <see cref="BOOL.TRUE"/> if the message loop should continue,
            ///  <see cref="BOOL.FALSE"/> otherwise.
            ///
            ///  If <see cref="BOOL.FALSE"/> is returned, the component manager terminates the
            ///  loop without removing <paramref name="pMsgPeeked"/> from the queue.
            /// </returns>
            [PreserveSig]
            BOOL FContinueMessageLoop(
                msoloop uReason,
                void* pvLoopData,
                User32.MSG* pMsgPeeked);

            /// <summary>
            ///  Called when component manager wishes to know if the component is in a
            ///  state in which it can terminate.
            /// </summary>
            /// <remarks>
            ///  If <paramref name="fPromptUser"/> is FALSE, component should simply return
            ///  TRUE if it can terminate, FALSE otherwise.
            ///
            ///  If <paramref name="fPromptUser"/> is TRUE, component should return TRUE if
            ///  it can terminate without prompting the user; otherwise it should prompt the
            ///  user, either 1.) asking user if it can terminate and returning TRUE or FALSE
            ///  appropriately, or 2.) giving an indication as to why it cannot terminate and
            ///  returning FALSE.
            /// </remarks>
            [PreserveSig]
            BOOL FQueryTerminate(
                BOOL fPromptUser);

            /// <summary>
            ///  Called when component manager wishes to terminate the component's registration.
            /// </summary>
            /// <remarks>
            ///  Component should revoke its registration with component manager, release
            ///  references to component manager and perform any necessary cleanup.
            /// </remarks>
            [PreserveSig]
            void Terminate();

            /// <summary>
            ///  Called to retrieve a window associated with the component, as specified
            ///  by <paramref name="dwWhich"/>.
            /// </summary>
            /// <returns>
            ///  Desired window or null if no such window exists.
            /// </returns>
            /// <param name="dwReserved">Reserved for future use and should be zero.</param>
            [PreserveSig]
            IntPtr HwndGetWindow(
                msocWindow dwWhich,
                uint dwReserved);
        }
    }
}
