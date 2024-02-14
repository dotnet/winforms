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
    internal unsafe sealed partial class ThreadContext :
        MarshalByRefObject,
        IHandle<HANDLE>,
        IMsoComponent.Interface,
        IManagedWrapper<IMsoComponent>
    {
        private static msoloop s_baseLoopReason;

        // IMsoComponentManager stuff
        private IMsoComponentManager.Interface? _componentManager;
        private bool _externalComponentManager;
        private bool _fetchingComponentManager;

        // IMsoComponent stuff
        private nuint _componentID = s_invalidId;
        private Form? _currentForm;
        private ThreadWindows? _threadWindows;
        private int _disposeCount;   // To make sure that we don't allow
                                     // reentrancy in Dispose()

        /// <summary>
        ///  Retrieves the component manager for this process.  If there is no component manager
        ///  currently installed, we install our own.
        /// </summary>
        internal unsafe IMsoComponentManager.Interface? ComponentManager
        {
            get
            {
                if (_componentManager is not null || _fetchingComponentManager)
                {
                    return _componentManager;
                }

                // The CLR is a good COM citizen and will pump messages when things are waiting.
                // This is nice; it keeps the world responsive.  But, it is also very hard for
                // us because most of the code below causes waits, and the likelihood that
                // a message will come in and need a component manager is very high.  Recursing
                // here is very very bad, and will almost certainly lead to application failure
                // later on as we come out of the recursion.  So, we guard it here and return
                // null.  EVERYONE who accesses the component manager must handle a NULL return!

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

                    if (_componentManager is not null)
                    {
                        RegisterComponentManager();
                    }
                }
                finally
                {
                    _fetchingComponentManager = false;
                }

                return _componentManager;

                unsafe static IMsoComponentManager.Interface? GetExternalComponentManager()
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

        /// <summary>
        ///  Revokes our component from the active component manager. Does nothing if there is no active
        ///  component manager or we are already invoked.
        /// </summary>
        private unsafe void RevokeComponent()
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
                // We should only be messing with windows we own.  See the "ctrl-shift-N" test above.
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

        unsafe void IMsoComponent.Interface.OnActivationChange(
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
            if (pMsgPeeked is null && GetState(STATE_POSTEDQUIT))
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
                        // dismissed.  If there is no active form, then it is an error that
                        // we got into here, so we terminate the loop.

                        if (_currentForm is null || _currentForm.CheckCloseDialog(false))
                        {
                            continueLoop = false;
                        }

                        break;

                    case msoloop.DoEvents:
                    case msoloop.DoEventsModal:
                        // For DoEvents, just see if there are more messages on the queue.
                        MSG temp = default;
                        if (!PInvoke.PeekMessage(&temp, HWND.Null, 0, 0, PEEK_MESSAGE_REMOVE_TYPE.PM_NOREMOVE))
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
