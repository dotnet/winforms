// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using Microsoft.Office;
using Windows.Win32.System.Com;
using ComIMessageFilter = Windows.Win32.Media.Audio.IMessageFilter;
using ComIServiceProvider = Windows.Win32.System.Com.IServiceProvider;

namespace System.Windows.Forms;

public sealed partial class Application
{
    /// <summary>
    ///  <see cref="ThreadContext"/> that supports <see cref="IMsoComponent"/>.
    /// </summary>
    internal sealed unsafe class ComponentThreadContext :
        ThreadContext,
        IMsoComponent.Interface,
        IManagedWrapper<IMsoComponent>
    {
        private bool _trackingComponent;

        private static readonly nuint s_invalidId = unchecked((nuint)(-1));

        // IMsoComponentManager stuff
        private IMsoComponentManager.Interface? _componentManager;
        private bool _externalComponentManager;
        private bool _fetchingComponentManager;

        // IMsoComponent stuff
        private nuint _componentID = s_invalidId;

        // We need to set this flag if we have started the ModalMessageLoop so that we don't create the ThreadWindows
        // when the ComponentManager calls on us (as IMSOComponent) during the OnEnterState.
        private bool _ourModalLoop;

        public override void EnsureReadyForIdle() =>
            // Ensure the component manager is created.
            _ = ComponentManager;

        /// <summary>
        ///  Retrieves the component manager for this process. If there is no component manager
        ///  currently installed, we install our own.
        /// </summary>
        internal IMsoComponentManager.Interface? ComponentManager
        {
            get
            {
                if (_componentManager is not null || _fetchingComponentManager)
                {
                    return _componentManager;
                }

                // The CLR is a good COM citizen and will pump messages when things are waiting.
                // This is nice; it keeps the world responsive. But, it is also very hard for
                // us because most of the code below causes waits, and the likelihood that
                // a message will come in and need a component manager is very high. Recursing
                // here is very very bad, and will almost certainly lead to application failure
                // later on as we come out of the recursion. So, we guard it here and return
                // null. EVERYONE who accesses the component manager must handle a NULL return!

                _fetchingComponentManager = true;

                try
                {
                    // Attempt to obtain the Host Application MSOComponentManager
                    _componentManager = GetExternalComponentManager();
                    if (_componentManager is not null)
                    {
                        _externalComponentManager = true;
                    }
                    else
                    {
                        _componentManager = new ComponentManager();
                    }

                    RegisterComponentManager();
                }
                finally
                {
                    _fetchingComponentManager = false;
                }

                return _componentManager;

                static IMsoComponentManager.Interface? GetExternalComponentManager()
                {
                    Application.OleRequired();
                    using ComScope<ComIMessageFilter> messageFilter = new(null);

                    // Clear the thread's message filter to see if there was an existing filter
                    if (PInvoke.CoRegisterMessageFilter(null, messageFilter).Failed || messageFilter.IsNull)
                    {
                        return null;
                    }

                    // There was an existing filter, reregister it
                    ComIMessageFilter* dummy = default;
                    PInvoke.CoRegisterMessageFilter(messageFilter, &dummy);

                    // Now look to see if it implements the native IServiceProvider
                    using var serviceProvider = messageFilter.TryQuery<ComIServiceProvider>(out HRESULT hr);
                    if (hr.Failed)
                    {
                        return null;
                    }

                    // Check the service provider for the service that provides IMsoComponentManager
                    using ComScope<IUnknown> serviceHandle = new(null);
                    Guid sid = new(MsoComponentIds.SID_SMsoComponentManager);
                    Guid iid = new(MsoComponentIds.IID_IMsoComponentManager);

                    if (serviceProvider.Value->QueryService(&sid, &iid, serviceHandle).Failed || serviceHandle.IsNull)
                    {
                        return null;
                    }

                    // We have the component manager service, now get the component manager interface
                    var componentManager = serviceHandle.TryQuery<IMsoComponentManager>(out hr);
                    if (hr.Succeeded && !componentManager.IsNull)
                    {
                        return new IMsoComponentManager.NativeAdapter(componentManager);
                    }

                    return null;
                }

                void RegisterComponentManager()
                {
                    MSOCRINFO info = new()
                    {
                        cbSize = (uint)sizeof(MSOCRINFO),
                        uIdleTimeInterval = 0,
                        grfcrf = msocrf.PreTranslateAll | msocrf.NeedIdleTime,
                        grfcadvf = msocadvf.Modal
                    };

                    UIntPtr id;
                    bool result = _componentManager.FRegisterComponent(ComHelpers.GetComPointer<IMsoComponent>(this), &info, &id);
                    _componentID = id;
                    Debug.Assert(_componentID != s_invalidId, "Our ID sentinel was returned as a valid ID");

                    if (result && _componentManager is not Application.ComponentManager)
                    {
                        _messageLoopCount++;
                    }

                    Debug.Assert(result,
                        $"Failed to register WindowsForms with the ComponentManager -- DoEvents and modal dialogs will be broken. size: {info.cbSize}");
                }
            }
        }

        protected override void BeginModalMessageLoop()
        {
            // Set the ourModalLoop flag so that the "IMSOComponent.OnEnterState" is a NOOP since we started the ModalMessageLoop.
            bool wasOurLoop = _ourModalLoop;
            _ourModalLoop = true;
            try
            {
                ComponentManager?.OnComponentEnterState(_componentID, msocstate.Modal, msoccontext.All, 0, null, 0);
            }
            finally
            {
                _ourModalLoop = wasOurLoop;
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                // If we had a component manager, detach from it.
                if (disposing && _componentManager is not null)
                {
                    RevokeComponent();
                }
            }
            catch (Exception ex)
            {
                Debug.Fail($"Unexpected exception thrown during Dispose: {ex.Message}");
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        protected override void EndModalMessageLoop()
        {
            bool wasOurLoop = _ourModalLoop;
            _ourModalLoop = true;
            try
            {
                // If We started the ModalMessageLoop .. this will call us back on the IMSOComponent.OnStateEnter and not do anything ...
                IMsoComponentManager.Interface? cm = ComponentManager;
                cm?.FOnComponentExitState(_componentID, msocstate.Modal, msoccontext.All, 0, null);
            }
            finally
            {
                // Reset the flag since we are exiting out of a ModalMessageLoop.
                _ourModalLoop = wasOurLoop;
            }
        }

        internal override void FormActivated(bool activate)
        {
            if (activate && ComponentManager is { } manager && manager is not Application.ComponentManager)
            {
                manager.FOnComponentActivate(_componentID);
            }
        }

        internal override void TrackInput(bool track)
        {
            // Protect against double setting, as this causes asserts in the VS component manager.
            if (_trackingComponent != track && ComponentManager is { } manager && manager is not Application.ComponentManager)
            {
                manager.FSetTrackingComponent(_componentID, track);
                _trackingComponent = track;
            }
        }

        protected override bool? GetMessageLoopInternal(bool mustBeActive, int loopCount)
        {
            // If we are already running a loop, we're fine.
            // If we are running in external manager we may need to make sure first the loop is active
            if (loopCount > (mustBeActive && _externalComponentManager ? 1 : 0))
            {
                return true;
            }

            // Also, access the ComponentManager property to demand create it, and we're also
            // fine if it is an external manager, because it has already pushed a loop.
            if (ComponentManager is not null && _externalComponentManager)
            {
                if (!mustBeActive)
                {
                    return true;
                }

                using ComScope<IMsoComponent> component = new(null);
                if (ComponentManager.FGetActiveComponent(msogac.Active, component, null, 0))
                {
                    return ComHelpers.WrapsManagedObject(this, component.Value);
                }
            }

            return null;
        }

        protected override bool RunMessageLoop(msoloop reason, bool fullModal)
        {
            bool result;

            if ((!fullModal && reason != msoloop.DoEventsModal) || ComponentManager is ComponentManager)
            {
                result = ComponentManager!.FPushMessageLoop(_componentID, reason, null);
            }
            else if (reason is msoloop.DoEvents or msoloop.DoEventsModal)
            {
                result = LocalModalMessageLoop(null);
            }
            else
            {
                result = LocalModalMessageLoop(CurrentForm);
            }

            return result;
        }

        protected override void EndOuterMessageLoop()
        {
            if (_componentManager is not null)
            {
                // If we had a component manager, detach from it.
                RevokeComponent();
            }
        }

        private bool LocalModalMessageLoop(Form? form)
        {
            try
            {
                // Execute the message loop until the active component tells us to stop.
                MSG msg = default;
                bool continueLoop = true;

                while (continueLoop)
                {
                    if (PInvoke.GetMessage(&msg, HWND.Null, 0, 0))
                    {
                        if (!PreTranslateMessage(ref msg))
                        {
                            PInvoke.TranslateMessage(msg);
                            PInvoke.DispatchMessage(&msg);
                        }

                        if (form is not null)
                        {
                            continueLoop = !form.CheckCloseDialog(false);
                        }
                    }
                    else if (form is null)
                    {
                        break;
                    }
                    else if (!PInvokeCore.PeekMessage(&msg, HWND.Null, 0, 0, PEEK_MESSAGE_REMOVE_TYPE.PM_NOREMOVE))
                    {
                        PInvoke.WaitMessage();
                    }
                }

                return continueLoop;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///  Revokes our component from the active component manager. Does nothing if there is no active
        ///  component manager or we are already invoked.
        /// </summary>
        private void RevokeComponent()
        {
            if (_componentManager is { } manager && _componentID != s_invalidId)
            {
                try
                {
                    _componentManager = null;
                    using (manager as IDisposable)
                    {
                        manager.FRevokeComponent(_componentID);
                    }
                }
                finally
                {
                    _componentID = s_invalidId;
                }
            }
        }

        // Things to test in VS when you change the IMsoComponent code:
        //
        // - You can bring up dialogs multiple times (ie, the editor for TextBox.Lines)
        // - Double-click DataFormWizard, cancel wizard
        // - When a dialog is open and you switch to another application, when you switch
        //   back to VS the dialog gets the focus
        // - If one modal dialog launches another, they are all modal (Try web forms Table\Rows\Cell)
        // - When a dialog is up, VS is completely disabled, including moving and resizing VS.
        // - After doing all this, you can ctrl-shift-N start a new project and VS is enabled.

        BOOL IMsoComponent.Interface.FDebugMessage(nint hInst, uint msg, WPARAM wparam, LPARAM lparam)
            => true;

        BOOL IMsoComponent.Interface.FPreTranslateMessage(MSG* msg)
            => PreTranslateMessage(ref Unsafe.AsRef<MSG>(msg));

        void IMsoComponent.Interface.OnEnterState(msocstate uStateID, BOOL fEnter)
        {
            // Return if our (WINFORMS) Modal Loop is still running.
            if (_ourModalLoop)
            {
                return;
            }

            if (uStateID == msocstate.Modal)
            {
                // We should only be messing with windows we own. See the "ctrl-shift-N" test above.
                if (fEnter)
                {
                    DisableWindowsForModalLoop(true, null); // WinFormsOnly = true
                }
                else
                {
                    EnableWindowsForModalLoop(true, null); // WinFormsOnly = true
                }
            }
        }

        void IMsoComponent.Interface.OnAppActivate(BOOL fActive, uint dwOtherThreadID)
        {
        }

        void IMsoComponent.Interface.OnLoseActivation()
        {
        }

        void IMsoComponent.Interface.OnActivationChange(
            IMsoComponent* component,
            BOOL fSameComponent,
            MSOCRINFO* pcrinfo,
            BOOL fHostIsActivating,
            nint pchostinfo,
            uint dwReserved)
        {
        }

        BOOL IMsoComponent.Interface.FDoIdle(msoidlef grfidlef)
        {
            _idleHandler?.Invoke(Thread.CurrentThread, EventArgs.Empty);
            return false;
        }

        BOOL IMsoComponent.Interface.FContinueMessageLoop(
            msoloop uReason,
            void* pvLoopData,
            MSG* pMsgPeeked)
        {
            bool continueLoop = true;

            // If we get a null message, and we have previously posted the WM_QUIT message,
            // then someone ate the message.
            if (pMsgPeeked is null && PostedQuit)
            {
                continueLoop = false;
            }
            else
            {
                switch (uReason)
                {
                    case msoloop.FocusWait:

                        // For focus wait, check to see if we are now the active application.
                        PInvoke.GetWindowThreadProcessId(PInvoke.GetActiveWindow(), out uint pid);
                        if (pid == PInvoke.GetCurrentProcessId())
                        {
                            continueLoop = false;
                        }

                        break;

                    case msoloop.ModalAlert:
                    case msoloop.ModalForm:

                        // For modal forms, check to see if the current active form has been
                        // dismissed. If there is no active form, then it is an error that
                        // we got into here, so we terminate the loop.

                        if (CurrentForm is null || CurrentForm.CheckCloseDialog(false))
                        {
                            continueLoop = false;
                        }

                        break;

                    case msoloop.DoEvents:
                    case msoloop.DoEventsModal:
                        // For DoEvents, just see if there are more messages on the queue.
                        MSG temp = default;
                        if (!PInvokeCore.PeekMessage(&temp, HWND.Null, 0, 0, PEEK_MESSAGE_REMOVE_TYPE.PM_NOREMOVE))
                        {
                            continueLoop = false;
                        }

                        break;
                }
            }

            return continueLoop;
        }

        BOOL IMsoComponent.Interface.FQueryTerminate(BOOL fPromptUser) => true;

        void IMsoComponent.Interface.Terminate()
        {
            if (_messageLoopCount > 0 && ComponentManager is not Application.ComponentManager)
            {
                _messageLoopCount--;
            }

            Dispose(false);
        }

        HWND IMsoComponent.Interface.HwndGetWindow(msocWindow dwWhich, uint dwReserved) => HWND.Null;
    }
}
