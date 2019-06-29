// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

using TaskDialogButtonStruct = Interop.TaskDialog.TASKDIALOG_BUTTON;
using TaskDialogCallbackDelegate = Interop.TaskDialog.PFTASKDIALOGCALLBACK;
using TaskDialogConfig = Interop.TaskDialog.TASKDIALOGCONFIG;
using TaskDialogFlags = Interop.TaskDialog.TASKDIALOG_FLAGS;
using TaskDialogIconElement = Interop.TaskDialog.TASKDIALOG_ICON_ELEMENTS;
using TaskDialogMessage = Interop.TaskDialog.TASKDIALOG_MESSAGES;
using TaskDialogNotification = Interop.TaskDialog.TASKDIALOG_NOTIFICATIONS;
using TaskDialogTextElement = Interop.TaskDialog.TASKDIALOG_ELEMENTS;

namespace System.Windows.Forms
{
    /// <summary>
    /// A task dialog allows to display information and get simple input from the user. It is similar
    /// to a <see cref="MessageBox"/> (in that it is formatted by the operating system) but provides
    /// a lot more features.
    /// </summary>
    /// <remarks>
    /// For more information, see:
    /// https://docs.microsoft.com/en-us/windows/desktop/Controls/task-dialogs-overview
    /// 
    /// Note: In order to use the dialog, you need ensure <see cref="Application.EnableVisualStyles"/>
    /// has been called before showing the dialog, or the application needs to be compiled with a
    /// manifest that contains a dependency to Microsoft.Windows.Common-Controls (6.0.0.0).
    /// Additionally, the current thread should use the single-threaded apartment (STA) model.
    /// </remarks>
    public partial class TaskDialog : IWin32Window
    {
        /// <summary>
        /// A self-defined window message that we post to the task dialog when
        /// handling a <see cref="TaskDialogNotification.TDN_BUTTON_CLICKED"/>
        /// notification, so that we will ignore further
        /// <see cref="TaskDialogNotification.TDN_BUTTON_CLICKED"/> notifications
        /// until we process the posted message.
        /// </summary>
        /// <remarks>
        /// This is used to work-around a bug in the native task dialog, where
        /// a <see cref="TaskDialogNotification.TDN_BUTTON_CLICKED"/> notification
        /// seems to be sent twice to the callback when you "click" a button by
        /// pressing its access key (mnemonic) and the dialog is still open when
        /// continuing the message loop.
        /// 
        /// This work-around should not have negative effects, such as erroneously
        /// ignoring a valid button clicked notification when the user presses the
        /// button multiple times while the GUI thread is hangs - this seems
        /// to work correctly, as our posted message will be processed before
        /// further (valid) <see cref="TaskDialogNotification.TDN_BUTTON_CLICKED"/>
        /// notifications are processed.
        /// 
        /// See documentation/repro in
        /// /Documentation/src/System/Windows/Forms/TaskDialog/Issue_ButtonClickHandlerCalledTwice.md
        /// 
        /// Note: We use a WM_APP message with a high value (WM_USER is not
        /// appropriate as it is private to the control class), in order to avoid
        /// conflicts with WM_APP messages which other parts of the application
        /// might want to send when they also subclassed the dialog window, although
        /// that should be unlikely.
        /// </remarks>
        private const int ContinueButtonClickHandlingMessage = Interop.WindowMessages.WM_APP + 0x3FFF;

        /// <summary>
        /// The delegate for the callback handler (that calls
        /// <see cref="HandleTaskDialogCallback"/>) from which the native function
        /// pointer <see cref="s_callbackProcDelegatePtr"/> is created. 
        /// </summary>
        /// <remarks>
        /// We must store this delegate (and prevent it from being garbage-collected)
        /// to ensure the function pointer doesn't become invalid.
        /// </remarks>
        private static readonly TaskDialogCallbackDelegate s_callbackProcDelegate;

        /// <summary>
        /// The function pointer created from <see cref="s_callbackProcDelegate"/>.
        /// </summary>
        private static readonly IntPtr s_callbackProcDelegatePtr;

        private TaskDialogStartupLocation _startupLocation;

        private bool _doNotSetForeground;

        private TaskDialogPage _page;

        private TaskDialogPage _boundPage;

        /// <summary>
        /// A qeueue of <see cref="TaskDialogPage"/>s that have been bound by
        /// navigating the dialog, but don't yet reflect the state of the
        /// native dialog because the corresponding
        /// <see cref="TaskDialogNotification.TDN_NAVIGATED"/> notification was
        /// not yet received.
        /// </summary>
        private readonly Queue<TaskDialogPage> _waitingNavigationPages = new Queue<TaskDialogPage>();

        /// <summary>
        /// Window handle of the task dialog when it is being shown.
        /// </summary>
        private IntPtr _hwndDialog;

        /// <summary>
        /// The <see cref="IntPtr"/> of a <see cref="GCHandle"/> that represents this
        /// <see cref="TaskDialog"/> instance.
        /// </summary>
        private IntPtr _instanceHandlePtr;

        private WindowSubclassHandler _windowSubclassHandler;

        /// <summary>
        /// Stores a value that indicates if the
        /// <see cref="Opened"/> event has been called and so the
        /// <see cref="Closed"/> event can be called later.
        /// </summary>
        /// <remarks>
        /// This is used to prevent raising the 
        /// <see cref="Closed"/> event without raising the
        /// <see cref="Opened"/> event first (e.g. if the dialog cannot be shown
        /// due to an invalid icon).
        /// </remarks>
        private bool _raisedOpened;

        /// <summary>
        /// Stores a value that indicates if the
        /// <see cref="TaskDialogPage.Created"/> event has been called for the
        /// current <see cref="TaskDialogPage"/> and so the corresponding
        /// <see cref="TaskDialogPage.Destroyed"/> can be called later.
        /// </summary>
        /// <remarks>
        /// This is used to prevent raising the 
        /// <see cref="TaskDialogPage.Destroyed"/> event without raising the
        /// <see cref="TaskDialogPage.Created"/> event first (e.g. if navigation
        /// fails).
        /// </remarks>
        private bool _raisedPageCreated;

        /// <summary>
        /// A counter which is used to determine whether the dialog has been navigated
        /// while being in a <see cref="TaskDialogNotification.TDN_BUTTON_CLICKED"/> handler.
        /// </summary>
        /// <remarks>
        /// When the dialog navigates within a ButtonClicked handler, the handler should
        /// always return S_FALSE to prevent the dialog from applying the button that
        /// raised the handler as dialog result. Otherwise, this can lead to memory access
        /// problems like <see cref="AccessViolationException"/>s, especially if the
        /// previous dialog page had radio buttons (but the new ones do not).
        /// 
        /// See the comment in <see cref="HandleTaskDialogCallback"/> for more
        /// information.
        /// 
        /// When the dialog navigates, it sets the <c>navigationIndex</c> to the current
        /// <c>stackCount</c> value, so that the ButtonClicked handler can determine
        /// if the dialog has been navigated after it was called.
        /// Tracking the stack count and navigation index is necessary as there
        /// can be multiple ButtonClicked handlers on the call stack, for example
        /// if a ButtonClicked handler runs the message loop so that new click events
        /// can be processed.
        /// </remarks>
        private (int stackCount, int navigationIndex) _buttonClickNavigationCounter;

        /// <summary>
        /// The button designated as the dialog result by the handler for the
        /// <see cref="TaskDialogNotification.TDN_BUTTON_CLICKED"/>
        /// notification.
        /// </summary>
        /// <remarks>
        /// This will be set the first time the
        /// <see cref="TaskDialogNotification.TDN_BUTTON_CLICKED"/> handler returns
        /// <see cref="TaskDialogNativeMethods.S_OK"/> to cache the button instance,
        /// so that <see cref="Show(IntPtr)"/> can then return it.
        /// 
        /// Additionally, this is used to check if there was already a 
        /// <see cref="TaskDialogNotification.TDN_BUTTON_CLICKED"/> handler that
        /// returned <see cref="TaskDialogNativeMethods.S_OK"/>, so that further
        /// handles will return <see cref="TaskDialogNativeMethods.S_FALSE"/> to
        /// not override the previously set result.
        /// </remarks>
        private (TaskDialogButton button, int buttonID)? _resultButton;

        private bool _suppressButtonClickedEvent;

        /// <summary>
        /// Specifies if the current code is called from within
        /// <see cref="Navigate(TaskDialogPage)"/>.
        /// </summary>
        /// <remarks>
        /// This is used to detect if you call <see cref="Navigate(TaskDialogPage)"/>
        /// from within an event raised by <see cref="Navigate(TaskDialogPage)"/>,
        /// which is not supported.
        /// </remarks>
        private bool _isInNavigate;

        /// <summary>
        /// Specifies if the <see cref="HandleTaskDialogCallback"/> method should
        /// currently ignore <see cref="TaskDialogNotification.TDN_BUTTON_CLICKED"/>
        /// notifications.
        /// </summary>
        /// <remarks>
        /// See <see cref="ContinueButtonClickHandlingMessage"/> for more information.
        /// </remarks>
        private bool _ignoreButtonClickedNotifications;

        /// <summary>
        /// Occurs after the task dialog has been created but before it is displayed.
        /// </summary>
        /// <remarks>
        /// You can use this event to allocate resources associated with the
        /// task dialog window handle, as it is the first event where
        /// <see cref="Handle"/> is available.
        /// 
        /// Note: The dialog will not show until this handler returns (even if the
        /// handler would run the message loop).
        /// </remarks>
        public event EventHandler Opened;

        /// <summary>
        /// Occurs when the task dialog is first displayed.
        /// </summary>
        public event EventHandler Shown;

        /// <summary>
        /// Occurs when the task dialog closing.
        /// </summary>
        /// <remarks>
        /// You can cancel the close by setting
        /// <see cref="CancelEventArgs.Cancel"/> to <c>true</c>. Otherwise, the
        /// dialog window will close, and the <see cref="Closed"/> event will be
        /// raised afterwards.
        /// 
        /// Note: The <see cref="Closed"/> event might not be called immediately
        /// after the <see cref="Closing"/> event (even though the dialog window
        /// has already closed). This can happen for example when showing multiple 
        /// (modeless) dialogs at the same time and then closing the one that
        /// was shown first – in that case, the <see cref="Closed"/> event for
        /// that dialog will be called only after the second dialog is also closed.
        /// 
        /// Note: This event might not always be called, e.g. if navigation of the
        /// dialog fails; however, the <see cref="Closed"/> event will always be
        /// called.
        /// </remarks>
        public event EventHandler<TaskDialogClosingEventArgs> Closing;

        /// <summary>
        /// Occurs when the task dialog is closed.
        /// </summary>
        /// <remarks>
        /// You can use this event to free resources associated with the
        /// task dialog window handle, as it is the last event where
        /// <see cref="Handle"/> is available.
        /// </remarks>
        public event EventHandler Closed;

        static TaskDialog()
        {
            // Create a delegate for the callback, and get a function pointer for it.
            // Because this will allocate some memory required to store the native
            // code for the function pointer, we only do this once by using a static
            // function, and then identify the actual TaskDialog instance by using a
            // GCHandle in the reference data field (like an object pointer).
            s_callbackProcDelegate = (hWnd, notification, wParam, lParam, referenceData) =>
                ((TaskDialog)GCHandle.FromIntPtr(referenceData).Target).HandleTaskDialogCallback(hWnd, notification, wParam, lParam);

            s_callbackProcDelegatePtr = Marshal.GetFunctionPointerForDelegate(s_callbackProcDelegate);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='TaskDialog'/> class.
        /// </summary>
        public TaskDialog()
        {
            // TaskDialog is only supported on Windows.
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException();
            }

            // Set default properties.
            _startupLocation = TaskDialogStartupLocation.CenterParent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='TaskDialog'/> class using the specified task dialog page.
        /// </summary>
        public TaskDialog(TaskDialogPage page)
            : this()
        {
            _page = page ?? throw new ArgumentNullException(nameof(page));
        }

        /// <summary>
        /// Gets the window handle of the task dialog window, or <see cref="IntPtr.Zero"/>
        /// if the dialog is currently not being shown.
        /// </summary>
        /// <remarks>
        /// When showing the dialog, the handle will be available first when the
        /// <see cref="Opened"/> event occurs, and last when the
        /// <see cref="Closed"/> event occurs after which you shouldn't use it any more.
        /// </remarks>
        public IntPtr Handle
        {
            get => _hwndDialog;
        }

        /// <summary>
        /// Gets or sets the <see cref="TaskDialogPage"/> instance that contains
        /// the contents which this task dialog will display.
        /// </summary>
        /// <remarks>
        /// When setting this property while the task dialog is displayed, it will
        /// recreate its contents from the specified <see cref="TaskDialogPage"/>
        /// ("navigation"). This means that the <see cref="TaskDialogPage.Destroyed"/>
        /// event will occur for the current page, and after the dialog
        /// completed navigation, the <see cref="TaskDialogPage.Created"/> event
        /// of the new page will occur.
        /// 
        /// Please note that you cannot manipulate the task dialog or its controls
        /// immediately after navigating it (except for calling <see cref="Close"/>
        /// or navigating the dialog again).
        /// You will need to wait for the <see cref="TaskDialogPage.Created"/>
        /// event to occur before you can manipulate the dialog or its controls.
        /// 
        /// Note that when navigating the dialog, the new page will be bound
        /// immediately, but the previous page will not be unbound until the
        /// <see cref="TaskDialogPage.Created"/> event of the new page is raised,
        /// because during that time the task dialog behaves as if it still
        /// showed the controls of the previous page.
        /// </remarks>
        public TaskDialogPage Page
        {
            get => _page ??= new TaskDialogPage();

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                // TODO: Maybe ignore the set call if the value is the same
                // (but we currently also don't do that for other properties).
                //if (value == _page)
                //{
                //    return;
                //}

                if (IsShown)
                {
                    // Try to navigate the dialog. This will validate the new page
                    // and assign it only if it is OK.
                    Navigate(value);
                }
                else
                {
                    _page = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the position of the task dialog when it is shown.
        /// </summary>
        /// <value>
        /// The <see cref="TaskDialogStartupLocation"/> that specifies the position of the task dialog
        /// when it is shown. The default value is <see cref="TaskDialogStartupLocation.CenterParent"/>.
        /// </value>
        public TaskDialogStartupLocation StartupLocation
        {
            get => _startupLocation;

            set
            {
                DenyIfBound();

                _startupLocation = value;
            }
        }

        // TODO: Maybe invert the property (like "SetToForeground") so that by default
        // the TDF_NO_SET_FOREGROUND flag is specified (as that is also the default
        // behavior of the MessageBox).
        /// <summary>
        /// Gets or sets a value that indicates if the task dialog should not set
        /// itself as foreground window when showing it.
        /// </summary>
        /// <value>
        /// <c>true</c> to indicate that the task dialog should not set itself as foreground window
        /// when showing it; otherwise, <c>false</c>. The default value is <c>false</c>.
        /// </value>
        /// <remarks>
        /// When setting this property to <c>true</c> and then showing the dialog, it
        /// causes the dialog to not set itself as foreground window. This means that
        /// if currently none of the application's windows has focus, the task dialog
        /// doesn't try to "steal" focus (which otherwise can result in the task dialog
        /// window being activated, or the taskbar button for the window flashing
        /// orange). However, if the application already has focus, the task dialog
        /// window will be activated anyway.
        /// 
        /// Note: This property only has an effect on Windows 8 and higher.
        /// </remarks>
        public bool DoNotSetForeground
        {
            get => _doNotSetForeground;

            set
            {
                DenyIfBound();

                _doNotSetForeground = value;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether <see cref="Show(IntPtr)"/> is
        /// currently being called.
        /// </summary>
        internal bool IsShown
        {
            get => _instanceHandlePtr != IntPtr.Zero;
        }

        /// <summary>
        /// Gets a value that indicates whether the native task dialog window has
        /// been created and its handle is available using the <see cref="Handle"/>
        /// property.
        /// </summary>
        /// <remarks>
        /// This property can only be <c>true</c> if <see cref="IsShown"/> is
        /// also <c>true</c>. However, normally this property should be equivalent
        /// to <see cref="IsShown"/>, because when showing the dialog, the
        /// callback should have been called setting the handle.
        /// </remarks>
        internal bool IsHandleCreated
        {
            get => _hwndDialog != IntPtr.Zero;
        }

        /// <summary>
        /// Gets or sets the current count of stack frames that are in the
        /// <see cref="TaskDialogRadioButton.CheckedChanged"/> event for the
        /// current task dialog.
        /// </summary>
        /// <remarks>
        /// This is used by the <see cref="TaskDialogRadioButton.Checked"/> setter
        /// so that it can disallow the change when the count is greater than zero.
        /// Additionally, it is used to deny navigation of the task dialog in that
        /// case.
        /// </remarks>
        internal int RadioButtonClickedStackCount
        {
            get;
            set;
        }

        /// <summary>
        /// Displays a task dialog with the specified text, instruction,
        /// title, buttons and icon.
        /// </summary>
        /// <param name="text">The text ("content") to display in the task dialog.</param>
        /// <param name="instruction">The instruction ("main instruction") to display in the task dialog.</param>
        /// <param name="title">The text to display in the title bar of the task dialog.</param>
        /// <param name="buttons">A combination of <see cref="TaskDialogButtons"/> flags to be shown
        /// in the task dialog.</param>
        /// <param name="icon">One of the <see cref="TaskDialogIcon"/> values that specifies which
        /// icon to display in the task dialog.</param>
        /// <returns>One of the <see cref="TaskDialogResult"/> values.</returns>
        public static TaskDialogResult Show(
            string text,
            string instruction = null,
            string title = null,
            TaskDialogButtons buttons = TaskDialogButtons.OK,
            TaskDialogStandardIcon icon = TaskDialogStandardIcon.None)
        {
            return Show(IntPtr.Zero, text, instruction, title, buttons, icon);
        }

        /// <summary>
        /// Displays a task dialog in front of the specified window and with the specified
        /// text, instruction, title, buttons and icon.
        /// </summary>
        /// <param name="owner">The owner window, or <c>null</c> to show a modeless dialog.</param>
        /// <param name="text">The text ("content") to display in the task dialog.</param>
        /// <param name="instruction">The instruction ("main instruction") to display in the task dialog.</param>
        /// <param name="title">The text to display in the title bar of the task dialog.</param>
        /// <param name="buttons">A combination of <see cref="TaskDialogButtons"/> flags to be shown
        /// in the task dialog.</param>
        /// <param name="icon">One of the <see cref="TaskDialogIcon"/> values that specifies which
        /// icon to display in the task dialog.</param>
        /// <returns>One of the <see cref="TaskDialogResult"/> values.</returns>
        public static TaskDialogResult Show(
            IWin32Window owner,
            string text,
            string instruction = null,
            string title = null,
            TaskDialogButtons buttons = TaskDialogButtons.OK,
            TaskDialogStandardIcon icon = TaskDialogStandardIcon.None)
        {
            return Show(owner.Handle, text, instruction, title, buttons, icon);
        }

        /// <summary>
        /// Displays a task dialog in front of the specified window and with the specified
        /// text, instruction, title, buttons and icon.
        /// </summary>
        /// <param name="hwndOwner">
        /// The handle of the owner window, or <see cref="IntPtr.Zero"/> to show a
        /// modeless dialog.
        /// </param>
        /// <param name="text">The text ("content") to display in the task dialog.</param>
        /// <param name="instruction">The instruction ("main instruction") to display in the task dialog.</param>
        /// <param name="title">The text to display in the title bar of the task dialog.</param>
        /// <param name="buttons">A combination of <see cref="TaskDialogButtons"/> flags to be shown
        /// in the task dialog.</param>
        /// <param name="icon">One of the <see cref="TaskDialogIcon"/> values that specifies which
        /// icon to display in the task dialog.</param>
        /// <returns>One of the <see cref="TaskDialogResult"/> values.</returns>
        public static TaskDialogResult Show(
            IntPtr hwndOwner,
            string text,
            string instruction = null,
            string title = null,
            TaskDialogButtons buttons = TaskDialogButtons.OK,
            TaskDialogStandardIcon icon = TaskDialogStandardIcon.None)
        {
            var dialog = new TaskDialog(new TaskDialogPage()
            {
                Text = text,
                Instruction = instruction,
                Title = title,
                Icon = icon,
                StandardButtons = buttons
            });

            return ((TaskDialogStandardButton)dialog.Show(hwndOwner)).Result;
        }

        private static void FreeConfig(IntPtr ptrToFree)
        {
            Marshal.FreeHGlobal(ptrToFree);
        }

        private static bool IsTaskDialogButtonCommitting(TaskDialogButton button)
        {
            // All custom button as well as all standard buttons except for the
            // "Help" button (if it is shown in the dialog) will close the
            // dialog. If the "Help" button is not shown in the task dialog,
            // "button" will be null or its "IsCreated" property returns false.
            // In that case the "Help" button would close the dialog, so we
            // return true.
            return !(button is TaskDialogStandardButton standardButton &&
                standardButton.IsCreated && standardButton.Result == TaskDialogResult.Help);
        }

        private static TaskDialogStandardButton CreatePlaceholderButton(TaskDialogResult result)
        {
            // TODO: Maybe bind the button so that the user
            // cannot change the properties, like it is the
            // case with the regular buttons added to the
            // collections.
            return new TaskDialogStandardButton(result)
            {
                Visible = false
            };
        }

        /// <summary>
        /// Shows the task dialog.
        /// </summary>
        /// <remarks>
        /// Showing the dialog will bind the <see cref="Page"/> and its
        /// controls until this method returns.
        /// </remarks>
        /// <returns>The <see cref="TaskDialogButton"/> which was clicked by the
        /// user to close the dialog.</returns>
        public TaskDialogButton Show()
        {
            return Show(IntPtr.Zero);
        }

        /// <summary>
        /// Shows the task dialog.
        /// </summary>
        /// <remarks>
        /// Showing the dialog will bind the <see cref="Page"/> and its
        /// controls until this method returns.
        /// </remarks>
        /// <param name="owner">The owner window, or <c>null</c> to show a modeless dialog.</param>
        /// <returns>The <see cref="TaskDialogButton"/> which was clicked by the
        /// user to close the dialog.</returns>
        public TaskDialogButton Show(IWin32Window owner)
        {
            return Show(owner.Handle);
        }

        /// <summary>
        /// Shows the task dialog.
        /// </summary>
        /// <remarks>
        /// Showing the dialog will bind the <see cref="Page"/> and its
        /// controls until this method returns.
        /// </remarks>
        /// <param name="hwndOwner">
        /// The handle of the owner window, or <see cref="IntPtr.Zero"/> to show a
        /// modeless dialog.
        /// </param>
        /// <returns>The <see cref="TaskDialogButton"/> which was clicked by the
        /// user to close the dialog.</returns>
        public TaskDialogButton Show(IntPtr hwndOwner)
        {
            // Recursive Show() is not possible because a TaskDialog instance can only
            // represent a single native dialog.
            if (IsShown)
                throw new InvalidOperationException($"This {nameof(TaskDialog)} instance is already being shown.");

            Page.Validate();

            // Allocate a GCHandle which we will use for the callback data.
            var instanceHandle = GCHandle.Alloc(this);
            try
            {
                _instanceHandlePtr = GCHandle.ToIntPtr(instanceHandle);

                // Bind the page and allocate the memory.
                BindPageAndAllocateConfig(
                    _page,
                    hwndOwner,
                    _startupLocation,
                    _doNotSetForeground,
                    out IntPtr ptrToFree,
                    out IntPtr ptrTaskDialogConfig);

                _boundPage = _page;
                try
                {
                    // Note: When an uncaught exception occurs in the callback or a
                    // WndProc handler, the CLR will manipulate the managed stack
                    // ("unwind") so that it doesn't contain the transition to and
                    // from native code. However, the TaskDialog still calls our
                    // callback when the message loop continues, but as we already
                    // freed the GCHandle, a NRE will occur (or other memory access
                    // problems because the callback delegate for the subclassed
                    // WndProc might already have been freed).
                    //
                    // Therefore, we neeed to catch all exceptions in the
                    // native -> managed transition, and when one occurs, call
                    // Application.OnThreadException().
                    //
                    // Note: The same issue can occur when using a message box with
                    // WPF or WinForms: If you do MessageBox.Show() wrapped in a
                    // try/catch on a button click, and before calling .Show() create
                    // and start a timer which stops and throws an exception on its
                    // Tick event, the application will crash with an
                    // AccessViolationException as soon as you close the MessageBox.

                    // Activate a theming scope so that the task dialog works without
                    // having to use an application manifest that enables common controls
                    // v6 (provided that Application.EnableVisualStyles() was called
                    // earlier).
                    IntPtr themingCookie = UnsafeNativeMethods.ThemingScope.Activate();
                    int returnValue, resultButtonID;
                    try
                    {
                        returnValue = Interop.TaskDialog.TaskDialogIndirect(
                            ptrTaskDialogConfig,
                            out resultButtonID,
                            out _,
                            out _);
                    }
                    catch (EntryPointNotFoundException ex)
                    {
                        throw new InvalidOperationException(
                            $"You need to call {nameof(Application)}.{nameof(Application.EnableVisualStyles)}() " +
                            $"before showing the task dialog. Alternatively, you can use an application manifest " +
                            $"that enables Microsoft.Windows.Common-Controls 6.0.0.0.",
                            ex);
                    }
                    finally
                    {
                        // Revert the theming scope.
                        UnsafeNativeMethods.ThemingScope.Deactivate(themingCookie);
                    }

                    // Marshal.ThrowExceptionForHR will use the IErrorInfo on the
                    // current thread if it exists.
                    if (returnValue < 0)
                    {
                        Marshal.ThrowExceptionForHR(returnValue);
                    }

                    // Normally, the returned button ID should always equal the cached
                    // result button ID. However, in some cases when the dialog is closed
                    // abormally (e.g. when closing the main window while a modeless task
                    // dialog is displayed), the dialog returns IDCANCEL (2) without
                    // priorly raising the TDN_BUTTON_CLICKED notification.
                    // Therefore, in that case we need to retrieve the button ourselves.
                    if (resultButtonID == _resultButton?.buttonID)
                    {
                        return _resultButton.Value.button;
                    }

                    return _boundPage.GetBoundButtonByID(resultButtonID) ??
                        CreatePlaceholderButton((TaskDialogResult)resultButtonID);
                }
                finally
                {
                    // Free the memory.
                    FreeConfig(ptrToFree);

                    // The window handle should already have been cleared from the
                    // TDN_DESTROYED notification. Otherwise, this would mean that
                    // TaskDialogIndirect() returned due to an exception being
                    // thrown (which means the native task dialog is still showing),
                    // which we should avoid as it is not supported.
                    // TODO: Maybe FailFast() in that case to prevent future errors.
                    Debug.Assert(_hwndDialog == IntPtr.Zero);

                    // Ensure to keep the callback delegate alive until
                    // TaskDialogIndirect() returns in case we could not undo the
                    // subclassing. See comment in UnsubclassWindow().
                    _windowSubclassHandler?.KeepCallbackDelegateAlive();
                    // Then, clear the subclass handler. Note that this only works
                    // correctly if we did not return from TaskDialogIndirect()
                    // due to an exception being thrown (as mentioned above).
                    _windowSubclassHandler = null;

                    // Also, ensure the window handle and the
                    // raiseClosed/raisePageDestroyed flags are is cleared even if
                    // the TDN_DESTROYED notification did not occur (although that
                    // should only happen when there was an exception).
                    _hwndDialog = IntPtr.Zero;
                    _raisedOpened = false;
                    _raisedPageCreated = false;

                    // Clear cached objects and other fields.
                    _resultButton = null;
                    _ignoreButtonClickedNotifications = false;

                    // Unbind the page. The 'Destroyed' event of the TaskDialogPage
                    // will already have been called from the callback.
                    _boundPage.Unbind();
                    _boundPage = null;

                    // If we started navigating the dialog but navigation wasn't
                    // successful, we also need to unbind the new pages.
                    foreach (TaskDialogPage page in _waitingNavigationPages)
                    {
                        page.Unbind();
                    }

                    _waitingNavigationPages.Clear();

                    // We need to ensure the callback delegate is not garbage-collected
                    // as long as TaskDialogIndirect doesn't return, by calling
                    // GC.KeepAlive().
                    // 
                    // This is not an exaggeration, as the comment for GC.KeepAlive()
                    // says the following:
                    // The JIT is very aggressive about keeping an 
                    // object's lifetime to as small a window as possible, to the point
                    // where a 'this' pointer isn't considered live in an instance method
                    // unless you read a value from the instance.
                    //
                    // Note: As this is a static field, in theory we would not need to
                    // call GC.KeepAlive() here, but we still do it to be safe.
                    GC.KeepAlive(s_callbackProcDelegate);
                }
            }
            finally
            {
                _instanceHandlePtr = IntPtr.Zero;
                instanceHandle.Free();
            }
        }

        // Messages that can be sent to the dialog while it is being shown.

        /// <summary>
        /// Closes the shown task dialog with a 
        /// <see cref="TaskDialogResult.Cancel"/> result.
        /// </summary>
        /// <remarks>
        /// To close the dialog with a different result, call the
        /// <see cref="TaskDialogButton.PerformClick"/> method of the
        /// <see cref="TaskDialogButton"/> which you want to set as result.
        /// 
        /// Note: This method can be called while the dialog is waiting for
        /// navigation to complete (whereas <see cref="TaskDialogButton.PerformClick"/>
        /// would throw in that case), and that when calling this method, the
        /// <see cref="TaskDialogButton.Click"/> event will not be raised.
        /// </remarks>
        public void Close()
        {
            _suppressButtonClickedEvent = true;
            try
            {
                // Send a click button message with the cancel result.
                ClickCancelButton();
            }
            finally
            {
                _suppressButtonClickedEvent = false;
            }
        }

        /// <summary>
        /// While the dialog is being shown, switches the progress bar mode to either a
        /// marquee progress bar or to a regular progress bar.
        /// For a marquee progress bar, you can enable or disable the marquee using
        /// <see cref="SetProgressBarMarquee(bool, int)"/>.
        /// </summary>
        /// <param name="marqueeProgressBar"></param>
        internal void SwitchProgressBarMode(bool marqueeProgressBar)
        {
            SendTaskDialogMessage(
                TaskDialogMessage.TDM_SET_MARQUEE_PROGRESS_BAR,
                marqueeProgressBar ? 1 : 0,
                IntPtr.Zero);
        }

        /// <summary>
        /// While the dialog is being shown, enables or disables progress bar marquee when
        /// an marquee progress bar is displayed.
        /// </summary>
        /// <param name="enableMarquee"></param>
        /// <param name="animationSpeed">
        /// The time in milliseconds between marquee animation updates. If <c>0</c>, the
        /// animation will be updated every 30 milliseconds.
        /// </param>
        internal void SetProgressBarMarquee(bool enableMarquee, int animationSpeed = 0)
        {
            if (animationSpeed < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(animationSpeed));
            }

            SendTaskDialogMessage(
                TaskDialogMessage.TDM_SET_PROGRESS_BAR_MARQUEE,
                enableMarquee ? 1 : 0,
                (IntPtr)animationSpeed);
        }

        /// <summary>
        /// While the dialog is being shown, sets the progress bar range.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <remarks>
        /// The default range is 0 to 100.
        /// </remarks>
        internal unsafe void SetProgressBarRange(int min, int max)
        {
            if (min < 0 || min > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(min));
            }
            if (max < 0 || max > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(max));
            }

            // Note: The MAKELPARAM macro converts the value to an unsigned int
            // before converting it to a pointer, so we should do the same.
            // However, this means we cannot convert the value directly to an
            // IntPtr; instead we need to first convert it to a pointer type
            // which requires unsafe code.
            // TODO: Use nuint instead of void* when it is available.
            var param = (IntPtr)(void*)unchecked((uint)(min | (max << 0x10)));

            SendTaskDialogMessage(
                TaskDialogMessage.TDM_SET_PROGRESS_BAR_RANGE,
                0,
                param);
        }

        /// <summary>
        /// While the dialog is being shown, sets the progress bar position.
        /// </summary>
        /// <param name="pos"></param>
        internal void SetProgressBarPosition(int pos)
        {
            if (pos < 0 || pos > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(pos));
            }

            SendTaskDialogMessage(
                TaskDialogMessage.TDM_SET_PROGRESS_BAR_POS,
                pos,
                IntPtr.Zero);
        }

        /// <summary>
        /// While the dialog is being shown, sets the progress bar state.
        /// </summary>
        /// <param name="state"></param>
        internal void SetProgressBarState(int state)
        {
            SendTaskDialogMessage(
                TaskDialogMessage.TDM_SET_PROGRESS_BAR_STATE,
                state,
                IntPtr.Zero);
        }

        /// <summary>
        /// While the dialog is being shown, sets the checkbox to the specified
        /// state.
        /// </summary>
        /// <param name="isChecked"></param>
        /// <param name="focus"></param>
        internal void ClickCheckBox(bool isChecked, bool focus = false)
        {
            SendTaskDialogMessage(
                TaskDialogMessage.TDM_CLICK_VERIFICATION,
                isChecked ? 1 : 0,
                (IntPtr)(focus ? 1 : 0));
        }

        internal void SetButtonElevationRequiredState(int buttonID, bool requiresElevation)
        {
            SendTaskDialogMessage(
                TaskDialogMessage.TDM_SET_BUTTON_ELEVATION_REQUIRED_STATE,
                buttonID,
                (IntPtr)(requiresElevation ? 1 : 0));
        }

        internal void SetButtonEnabled(int buttonID, bool enable)
        {
            SendTaskDialogMessage(
                TaskDialogMessage.TDM_ENABLE_BUTTON,
                buttonID,
                (IntPtr)(enable ? 1 : 0));
        }

        internal void SetRadioButtonEnabled(int radioButtonID, bool enable)
        {
            SendTaskDialogMessage(
                TaskDialogMessage.TDM_ENABLE_RADIO_BUTTON,
                radioButtonID,
                (IntPtr)(enable ? 1 : 0));
        }

        internal void ClickButton(int buttonID)
        {
            SendTaskDialogMessage(
                TaskDialogMessage.TDM_CLICK_BUTTON,
                buttonID,
                IntPtr.Zero);
        }

        internal void ClickCancelButton()
        {
            // We allow to simulate a button click (which might close the dialog),
            // even if we are waiting for the TDN_NAVIGATED notification to occur,
            // because between sending the TDM_NAVIGATE_PAGE message and receiving
            // the TDN_NAVIGATED notification, the dialog will behave as if it
            // still contains the controls of the previous page (though actually
            // handles for controls of both the previous and new page exist during
            // that time).
            SendTaskDialogMessage(
                TaskDialogMessage.TDM_CLICK_BUTTON,
                (int)TaskDialogResult.Cancel,
                IntPtr.Zero,
                false);
        }

        internal void ClickRadioButton(int radioButtonID)
        {
            SendTaskDialogMessage(
                TaskDialogMessage.TDM_CLICK_RADIO_BUTTON,
                radioButtonID,
                IntPtr.Zero);
        }

        internal void UpdateTextElement(TaskDialogTextElement element, string text)
        {
            DenyIfDialogNotUpdatable();

            // Note: Instead of null, we must specify the empty string; otherwise
            // the update would be ignored.
            IntPtr textPtr = Marshal.StringToHGlobalUni(text ?? string.Empty);
            try
            {
                // Note: SetElementText will resize the dialog while UpdateElementText
                // will not (which would lead to clipped controls), so we use the
                // former.
                SendTaskDialogMessage(TaskDialogMessage.TDM_SET_ELEMENT_TEXT, (int)element, textPtr);
            }
            finally
            {
                // We can now free the memory because SendMessage does not return
                // until the message has been processed.
                Marshal.FreeHGlobal(textPtr);
            }
        }

        internal void UpdateIconElement(TaskDialogIconElement element, IntPtr icon)
        {
            // Note: Updating the icon doesn't cause the task dialog to update
            // its size. For example, if you initially didn't specify an icon
            // but later want to set one, the dialog contents might get clipped.
            // To fix this, we might want to call UpdateSize() that forces the
            // task dialog to update its size.
            SendTaskDialogMessage(TaskDialogMessage.TDM_UPDATE_ICON, (int)element, icon);
        }

        internal void UpdateTitle(string title)
        {
            // Note: We must not allow to change the title if we are currently
            // waiting for a TDN_NAVIGATED notification, because in that case
            // the task dialog will already have set the title from the new page.
            DenyIfDialogNotUpdatable();

            // TODO: Because we use SetWindowText() directly (as there is no task
            // dialog message for setting the title), there is a small discrepancy
            // between specifying an empty title in the TASKDIALOGCONFIG structure
            // and setting an empty title with this method: An empty string (or null)
            // in the TASKDIALOGCONFIG struture causes the dialog title to be the
            // executable name (e.g. "MyApplication.exe"), but using an empty string
            // (or null) with this method causes the window title to be empty.
            // We could replicate the Task Dialog behavior by also using the
            // executable's name as title if the string is null or empty.
            if (Interop.User32.SetWindowTextW(new HandleRef(this, _hwndDialog), title) == 0)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
        }

        /// <summary>
        /// Raises the <see cref="Opened"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected void OnOpened(EventArgs e)
        {
            Opened?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="Shown"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected void OnShown(EventArgs e)
        {
            Shown?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="Closing"/> event.
        /// </summary>
        /// <param name="e">An <see cref="TaskDialogClosingEventArgs"/> that contains the event data.</param>
        protected void OnClosing(TaskDialogClosingEventArgs e)
        {
            Closing?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="Closed"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected void OnClosed(EventArgs e)
        {
            Closed?.Invoke(this, e);
        }

        private int HandleTaskDialogCallback(
            IntPtr hWnd,
            TaskDialogNotification notification,
            IntPtr wParam,
            IntPtr lParam)
        {
            // Set the hWnd as this may be the first time that we get it.
            bool isFirstNotification = _hwndDialog == IntPtr.Zero;
            _hwndDialog = hWnd;

            try
            {
                if (isFirstNotification)
                {
                    // Subclass the window as early as possible after the window handle
                    // is available.
                    SubclassWindow();
                }

                switch (notification)
                {
                    case TaskDialogNotification.TDN_CREATED:
                        _boundPage.ApplyInitialization();

                        // Note: If the user navigates the dialog within the Opened event
                        // and then runs the message loop, the Created and Destroyed events
                        // for the original page would never be called (because the callback
                        // would raise the Created event for the new page from the
                        // TDN_NAVIGATED notification and then the Opened event returns),
                        // but we consider this to be OK.
                        if (!_raisedOpened)
                        {
                            _raisedOpened = true;
                            OnOpened(EventArgs.Empty);
                        }

                        // Don't raise the Created event of the bound page if we are
                        // waiting for the TDN_NAVIGATED notification, because that means
                        // the user has already navigated the dialog in one of the
                        // previous events, so eventually the TDN_NAVIGATED notification
                        // will occur where we will raise the Created event for the new
                        // page.
                        if (!_raisedPageCreated && _waitingNavigationPages.Count == 0)
                        {
                            _raisedPageCreated = true;
                            _boundPage.OnCreated(EventArgs.Empty);
                        }
                        break;

                    case TaskDialogNotification.TDN_NAVIGATED:
                        // Indicate to the ButtonClicked handlers currently on the stack
                        // that we received the TDN_NAVIGATED notification.
                        _buttonClickNavigationCounter.navigationIndex = _buttonClickNavigationCounter.stackCount;

                        // We can now unbind the previous page and then switch to the
                        // new page.
                        _boundPage.Unbind();
                        _boundPage = _waitingNavigationPages.Dequeue();

                        _boundPage.ApplyInitialization();

                        // Only raise the event if we don't wait for yet another
                        // navigation (this is the same as we do in the TDN_CREATED
                        // handler).
                        Debug.Assert(!_raisedPageCreated);
                        if (!_raisedPageCreated && _waitingNavigationPages.Count == 0)
                        {
                            _raisedPageCreated = true;
                            _boundPage.OnCreated(EventArgs.Empty);
                        }
                        break;

                    case TaskDialogNotification.TDN_DESTROYED:
                        // Note: When multiple dialogs are shown (so Show() will occur
                        // multiple times in the call stack) and a previously opened
                        // dialog is closed, the Destroyed notification for the closed
                        // dialog will only occur after the newer dialogs are also
                        // closed.
                        try
                        {
                            // Only raise the destroyed/closed events if the corresponding
                            // created/opened events have been called. For example, when
                            // trying to show the dialog with an invalid configuration
                            // (so an error HResult will be returned), the callback is
                            // invoked only one time with the TDN_DESTROYED notification
                            // without being invoked with the TDN_CREATED notification.
                            if (_raisedPageCreated)
                            {
                                _raisedPageCreated = false;
                                _boundPage.OnDestroyed(EventArgs.Empty);
                            }

                            if (_raisedOpened)
                            {
                                _raisedOpened = false;
                                OnClosed(EventArgs.Empty);
                            }
                        }
                        finally
                        {
                            // Undo the subclassing as the window handle is about to
                            // be destroyed.
                            UnsubclassWindow();

                            // Clear the dialog handle, because according to the docs, we
                            // must not continue to send any notifications to the dialog
                            // after the callback function has returned from being called
                            // with the 'Destroyed' notification.                    
                            _hwndDialog = IntPtr.Zero;
                        }
                        break;

                    case TaskDialogNotification.TDN_HYPERLINK_CLICKED:
                        string link = Marshal.PtrToStringUni(lParam);

                        var eventArgs = new TaskDialogHyperlinkClickedEventArgs(link);
                        _boundPage.OnHyperlinkClicked(eventArgs);
                        break;

                    case TaskDialogNotification.TDN_BUTTON_CLICKED:
                        // Check if we should ignore this notification. If we process
                        // it, we set a flag to ignore further TDN_BUTTON_CLICKED
                        // notifications, and we post a message to the task dialog
                        // that, when we process it, causes us to reset the flag.
                        // This is used to work-around the access key bug in the
                        // native task dialog - see the remarks of the 
                        // "ContinueButtonClickHandlingMessage" for more information.
                        if (_ignoreButtonClickedNotifications)
                        {
                            return Interop.HRESULT.S_FALSE;
                        }

                        // Post the message, and then set the flag to ignore further
                        // notifications until we receive the posted message.
                        if (!UnsafeNativeMethods.PostMessage(
                            new HandleRef(this, hWnd),
                            ContinueButtonClickHandlingMessage,
                            IntPtr.Zero,
                            IntPtr.Zero))
                        {
                            throw new InvalidOperationException("Could not send message."); // TODO: Exception type
                        }

                        _ignoreButtonClickedNotifications = true;

                        int buttonID = (int)wParam;
                        TaskDialogButton button = _boundPage.GetBoundButtonByID(buttonID);

                        bool handlerResult = true;
                        if (button != null && !_suppressButtonClickedEvent)
                        {
                            // Note: When the event handler returned true but we received
                            // a TDN_NAVIGATED notification within the handler (e.g. by
                            // running the message loop there), the buttonID of the handler
                            // would be set as the dialog's result even if this ID is from
                            // the dialog page before the dialog was navigated.
                            // Additionally, memory access problems like
                            // AccessViolationExceptions seem to occur in this situation
                            // (especially if the dialog also had radio buttons before the
                            // navigation; these would also be set as result of the dialog),
                            // probably because this scenario isn't an expected usage of
                            // the native TaskDialog.
                            // 
                            // See documentation/repro in
                            // /Documentation/src/System/Windows/Forms/TaskDialog/Issue_AccessViolation_NavigationInButtonClicked.md
                            // 
                            // To fix the memory access problems, we simply always return
                            // S_FALSE when the callback received a TDN_NAVIGATED
                            // notification within the Button.Click event handler.
                            checked
                            {
                                _buttonClickNavigationCounter.stackCount++;
                            }
                            try
                            {
                                handlerResult = button.HandleButtonClicked();

                                // Check if the index was set to the current stack count,
                                // which means we received a TDN_NAVIGATED notification
                                // while we called the handler. In that case we need to
                                // return S_FALSE to prevent the dialog from closing
                                // (and applying the previous ButtonID and RadioButtonID
                                // as results).
                                if (_buttonClickNavigationCounter.navigationIndex >=
                                    _buttonClickNavigationCounter.stackCount)
                                {
                                    handlerResult = false;
                                }
                            }
                            finally
                            {
                                _buttonClickNavigationCounter.stackCount--;
                                _buttonClickNavigationCounter.navigationIndex = Math.Min(
                                    _buttonClickNavigationCounter.navigationIndex,
                                    _buttonClickNavigationCounter.stackCount);
                            }
                        }

                        // If the button would close the dialog, raise the Closing event
                        // so that the user can cancel the close.
                        if (handlerResult && IsTaskDialogButtonCommitting(button))
                        {
                            // For consistency, we only raise the event (and allow the handler
                            // to return S_OK) if it was not already raised for a previous
                            // handler which already set a button result. Otherwise, we
                            // would either raise the "Closing" event multiple times (which
                            // wouldn't make sense) or we would allow a later handler to
                            // override the previously set result, which would mean the
                            // button returned from Show() would not match one specified
                            // in the "Closing" event's args.
                            if (_resultButton != null)
                            {
                                handlerResult = false;
                            }
                            else
                            {
                                // If we didn't find the button (e.g. when specifying
                                // AllowCancel but not adding a "Cancel" button), we need
                                // to create a new instance and save it, so that we can
                                // return that instance after TaskDialogIndirect() returns.
                                if (button == null)
                                {
                                    button = CreatePlaceholderButton((TaskDialogResult)buttonID);
                                }

                                // The button would close the dialog, so raise the event.
                                var closingEventArgs = new TaskDialogClosingEventArgs(button);
                                OnClosing(closingEventArgs);

                                handlerResult = !closingEventArgs.Cancel;

                                // Cache the result button if we return S_OK.
                                if (handlerResult)
                                {
                                    _resultButton = (button, buttonID);
                                }
                            }
                        }

                        return handlerResult ? Interop.HRESULT.S_OK : Interop.HRESULT.S_FALSE;

                    case TaskDialogNotification.TDN_RADIO_BUTTON_CLICKED:
                        int radioButtonID = (int)wParam;
                        TaskDialogRadioButton radioButton = _boundPage.GetBoundRadioButtonByID(radioButtonID);

                        checked
                        {
                            RadioButtonClickedStackCount++;
                        }
                        try
                        {
                            radioButton.HandleRadioButtonClicked();
                        }
                        finally
                        {
                            RadioButtonClickedStackCount--;
                        }

                        break;

                    case TaskDialogNotification.TDN_EXPANDO_BUTTON_CLICKED:
                        _boundPage.Expander.HandleExpandoButtonClicked(wParam != IntPtr.Zero);
                        break;

                    case TaskDialogNotification.TDN_VERIFICATION_CLICKED:
                        _boundPage.CheckBox.HandleCheckBoxClicked(wParam != IntPtr.Zero);
                        break;

                    case TaskDialogNotification.TDN_HELP:
                        _boundPage.OnHelpRequest(EventArgs.Empty);
                        break;
                }
            }
            catch (Exception ex)
            {
                // When an exception occurs, handle it by calling the application's
                // ThreadException handler. 
                // It is important that we don't let such exception fall through
                // the native -> managed transition, as otherwise the CLR would
                // unwind the stack even though the task dialog is still shown,
                // which means invalid memory access may occur if the callback
                // is called again as we already freed the object pointer.
                HandleCallbackException(ex);
            }

            return Interop.HRESULT.S_OK;
        }

        /// <summary>
        /// While the dialog is being shown, recreates the dialog from the specified
        /// <paramref name="page"/>.
        /// </summary>
        /// <remarks>
        /// Note that you should not call this method in the <see cref="Opened"/>
        /// event because the task dialog is not yet displayed in that state.
        /// </remarks>
        private void Navigate(TaskDialogPage page)
        {
            // We allow to nagivate the dialog even if the previous navigation did
            // not complete yet, as this seems to work in the native implementation.
            DenyIfDialogNotUpdatable(checkWaitingForNavigation: false);

            // Don't allow to navigate the dialog when we are in a
            // TDN_RADIO_BUTTON_CLICKED notification, because the dialog doesn't
            // seem to correctly handle this (e.g. when running the message loop
            // after navigation, an AccessViolationException would occur after
            // the handler returns).
            // Note: Actually, the problem is when we receive a TDN_NAVIGATED
            // notification within a TDN_RADIO_BUTTON_CLICKED notification (due
            // to running the message loop there), but we can only prevent this
            // by not allowing to send the TDM_NAVIGATE_PAGE message here
            // (and then disallow to send any TDM_CLICK_RADIO_BUTTON messages
            // until we receive the TDN_NAVIGATED notification).
            // See:
            // https://github.com/dotnet/winforms/issues/146#issuecomment-466784079
            // and /Documentation/src/System/Windows/Forms/TaskDialog/Issue_AccessViolation_NavigationInRadioButtonClicked.md
            if (RadioButtonClickedStackCount > 0)
            {
                throw new InvalidOperationException(
                    $"Cannot navigate the dialog from within the " +
                    $"{nameof(TaskDialogRadioButton)}.{nameof(TaskDialogRadioButton.CheckedChanged)} " +
                    $"event of one of the radio buttons of the current task dialog.");
            }

            // Don't allow to navigate the dialog if called from an event handler
            // (TaskDialogPage.Destroyed) that is raised from within this method.
            if (_isInNavigate)
            {
                throw new InvalidOperationException(
                    "Cannot navigate the dialog from an event handler that is" +
                    "called from within navigation.");
            }

            // Don't allow navigation of the dialog window is already closed (and
            // therefore has set a result button), because that would result in
            // weird/undefined behavior (e.g. returning IDCANCEL (2) as button
            // result even though a different button has already been set as result).
            const string dialogAlreadyClosedMesssage = "Cannot navigate the dialog when it has already closed.";
            if (_resultButton != null)
            {
                throw new InvalidOperationException(dialogAlreadyClosedMesssage);
            }

            _isInNavigate = true;
            try
            {
                page.Validate();

                // After validation passed, we can now unbind the current page and
                // bind the new one.
                // Need to raise the "Destroyed" event for the current page. The
                // "Created" event for the new page will occur from the callback.
                // Note: "this.raisedPageCreated" should always be true here.
                if (_raisedPageCreated)
                {
                    _raisedPageCreated = false;
                    _boundPage.OnDestroyed(EventArgs.Empty);

                    // Need to check again if the dialog has not already been closed,
                    // since the Destroyed event handler could have performed a
                    // button click that closed the dialog.
                    // TODO: Another option would be to disallow button clicks while
                    // within the event handler.
                    if (_resultButton != null)
                    {
                        throw new InvalidOperationException(dialogAlreadyClosedMesssage);
                    }

                    // Also, we need to validate the page again. For example, the user
                    // might change the properties of the new page or its controls
                    // within the "Destroyed" event so that it would no longer be
                    // valid, or e.g. navigate a different dialog to that page in
                    // the meantime (although admittedly that would be a very
                    // strange pattern).
                    page.Validate();
                }
            }
            finally
            {
                _isInNavigate = false;
            }

            TaskDialogPage previousPage = _page;
            _page = page;
            try
            {
                // Note: We don't unbind the previous page here - this will be done
                // when the TDN_NAVIGATED notification occurs, because technically
                // the controls of both the previous page AND the new page exist
                // on the native Task Dialog until the TDN_NAVIGATED notification
                // occurs, and the dialog behaves as if it currently still showing
                // the previous page (which can be verified using the behavior of
                // the "Help" button, where simulating a click to that button will
                // raise the "Help" event if the dialog considers the button to
                // be shown, and otherwise will close the dialog without raising
                // the "Help" event; also, if you updated e.g. the dialog's text or
                // instruction during that time, these changes would be lost when
                // the TDN_NAVIGATED notification occurs).
                BindPageAndAllocateConfig(
                    page,
                    IntPtr.Zero,
                    startupLocation: default,
                    doNotSetForeground: false,
                    out IntPtr ptrToFree,
                    out IntPtr ptrTaskDialogConfig);
                try
                {
                    // Enqueue the page before sending the message. This ensures
                    // that if the native task dialog's behavior is ever changed
                    // to raise the TDN_NAVIGATED notification recursively from
                    // sending the TDM_NAVIGATE_PAGE message, we can correctly
                    // process the page in the callback.
                    _waitingNavigationPages.Enqueue(page);
                    try
                    {
                        // Note: If the task dialog cannot be recreated with the
                        // new page, the dialog will close and TaskDialogIndirect()
                        // returns with an error code; but this will not be
                        // noticeable in the SendMessage() return value.
                        SendTaskDialogMessage(
                            TaskDialogMessage.TDM_NAVIGATE_PAGE,
                            0,
                            ptrTaskDialogConfig,
                            checkWaitingForNavigation: false);
                    }
                    catch
                    {
                        // Since navigation failed, we need to remove our page
                        // from the queue.
                        // However, this should not happen because
                        // SendTaskDialogMessage() shouldn't throw here.
                        int originalCount = _waitingNavigationPages.Count;
                        for (int i = 0; i < originalCount; i++)
                        {
                            TaskDialogPage element = _waitingNavigationPages.Dequeue();
                            if (element != page)
                            {
                                _waitingNavigationPages.Enqueue(element);
                            }
                        }
                        throw;
                    }
                }
                catch
                {
                    page.Unbind();
                    throw;
                }
                finally
                {
                    // We can now free the memory because SendMessage does not
                    // return until the message has been processed.
                    FreeConfig(ptrToFree);
                }
            }
            catch
            {
                _page = previousPage;
                throw;
            }
        }

        private unsafe void BindPageAndAllocateConfig(
            TaskDialogPage page,
            IntPtr hwndOwner,
            TaskDialogStartupLocation startupLocation,
            bool doNotSetForeground,
            out IntPtr ptrToFree,
            out IntPtr ptrTaskDialogConfig)
        {
            page.Bind(
                this,
                out TaskDialogFlags flags,
                out TaskDialogButtons standardButtonFlags,
                out IntPtr iconValue,
                out IntPtr footerIconValue,
                out int defaultButtonID,
                out int defaultRadioButtonID);

            try
            {
                if (startupLocation == TaskDialogStartupLocation.CenterParent)
                {
                    flags |= TaskDialogFlags.TDF_POSITION_RELATIVE_TO_WINDOW;
                }
                if (doNotSetForeground)
                {
                    flags |= TaskDialogFlags.TDF_NO_SET_FOREGROUND;
                }

                checked
                {
                    // First, calculate the necessary memory size we need to allocate for
                    // all structs and strings.
                    // Note: Each Align() call when calculating the size must correspond
                    // with a Align() call when incrementing the pointer.
                    // Use a byte pointer so we can use byte-wise pointer arithmetics.
                    var sizeToAllocate = (byte*)0;
                    sizeToAllocate += sizeof(TaskDialogConfig);

                    // Strings in TasDialogConfig
                    Align(ref sizeToAllocate, sizeof(char));
                    sizeToAllocate += SizeOfString(page.Title);
                    sizeToAllocate += SizeOfString(page.Instruction);
                    sizeToAllocate += SizeOfString(page.Text);
                    sizeToAllocate += SizeOfString(page.CheckBox?.Text);
                    sizeToAllocate += SizeOfString(page.Expander?.Text);
                    sizeToAllocate += SizeOfString(page.Expander?.ExpandedButtonText);
                    sizeToAllocate += SizeOfString(page.Expander?.CollapsedButtonText);
                    sizeToAllocate += SizeOfString(page.Footer?.Text);

                    // Buttons array
                    if (page.CustomButtons.Count > 0)
                    {
                        // Note: Theoretically we would not need to align the pointer here
                        // since the packing of the structure is set to 1. Note that this
                        // can cause an unaligned write when assigning the structure (the
                        // same happens with TaskDialogConfig).
                        Align(ref sizeToAllocate);
                        sizeToAllocate += sizeof(TaskDialogButtonStruct) * page.CustomButtons.Count;

                        // Strings in buttons array
                        Align(ref sizeToAllocate, sizeof(char));
                        for (int i = 0; i < page.CustomButtons.Count; i++)
                        {
                            sizeToAllocate += SizeOfString(page.CustomButtons[i].GetResultingText());
                        }
                    }

                    // Radio buttons array
                    if (page.RadioButtons.Count > 0)
                    {
                        // See comment above regarding alignment.
                        Align(ref sizeToAllocate);
                        sizeToAllocate += sizeof(TaskDialogButtonStruct) * page.RadioButtons.Count;

                        // Strings in radio buttons array
                        Align(ref sizeToAllocate, sizeof(char));
                        for (int i = 0; i < page.RadioButtons.Count; i++)
                        {
                            sizeToAllocate += SizeOfString(page.RadioButtons[i].Text);
                        }
                    }

                    // Allocate the memory block. We add additional bytes to ensure we can
                    // align the returned pointer to IntPtr.Size (the biggest align size
                    // that we will use).
                    ptrToFree = Marshal.AllocHGlobal((IntPtr)(sizeToAllocate + (IntPtr.Size - 1)));
                    try
                    {
                        // Align the pointer before using it. This is important since we also
                        // started with an aligned "address" value (0) when calculating the
                        // required allocation size. We must use the same size that we added
                        // as additional size when allocating the memory.
                        var currentPtr = (byte*)ptrToFree;
                        Align(ref currentPtr);
                        ptrTaskDialogConfig = (IntPtr)currentPtr;

                        ref TaskDialogConfig taskDialogConfig = ref *(TaskDialogConfig*)currentPtr;
                        currentPtr += sizeof(TaskDialogConfig);

                        // Assign the structure with the constructor syntax, which will
                        // automatically initialize its other members with their default
                        // value.
                        Align(ref currentPtr, sizeof(char));
                        taskDialogConfig = new TaskDialogConfig()
                        {
                            cbSize = (uint)sizeof(TaskDialogConfig),
                            hwndParent = hwndOwner,
                            dwFlags = flags,
                            dwCommonButtons = (Interop.TaskDialog.TASKDIALOG_COMMON_BUTTON_FLAGS)standardButtonFlags,
                            mainIconUnion = iconValue,
                            footerIconUnion = footerIconValue,
                            pszWindowTitle = MarshalString(page.Title),
                            pszMainInstruction = MarshalString(page.Instruction),
                            pszContent = MarshalString(page.Text),
                            pszVerificationText = MarshalString(page.CheckBox?.Text),
                            pszExpandedInformation = MarshalString(page.Expander?.Text),
                            pszExpandedControlText = MarshalString(page.Expander?.ExpandedButtonText),
                            pszCollapsedControlText = MarshalString(page.Expander?.CollapsedButtonText),
                            pszFooter = MarshalString(page.Footer?.Text),
                            nDefaultButton = defaultButtonID,
                            nDefaultRadioButton = defaultRadioButtonID,
                            pfCallback = s_callbackProcDelegatePtr,
                            lpCallbackData = _instanceHandlePtr,
                            cxWidth = checked((uint)page.Width)
                        };

                        // Buttons array
                        if (page.CustomButtons.Count > 0)
                        {
                            Align(ref currentPtr);
                            var customButtonStructs = (TaskDialogButtonStruct*)currentPtr;
                            taskDialogConfig.pButtons = (IntPtr)customButtonStructs;
                            taskDialogConfig.cButtons = (uint)page.CustomButtons.Count;
                            currentPtr += sizeof(TaskDialogButtonStruct) * page.CustomButtons.Count;

                            Align(ref currentPtr, sizeof(char));
                            for (int i = 0; i < page.CustomButtons.Count; i++)
                            {
                                TaskDialogCustomButton currentCustomButton = page.CustomButtons[i];
                                customButtonStructs[i] = new TaskDialogButtonStruct()
                                {
                                    nButtonID = currentCustomButton.ButtonID,
                                    pszButtonText = MarshalString(currentCustomButton.GetResultingText())
                                };
                            }
                        }

                        // Radio buttons array
                        if (page.RadioButtons.Count > 0)
                        {
                            Align(ref currentPtr);
                            var customRadioButtonStructs = (TaskDialogButtonStruct*)currentPtr;
                            taskDialogConfig.pRadioButtons = (IntPtr)customRadioButtonStructs;
                            taskDialogConfig.cRadioButtons = (uint)page.RadioButtons.Count;
                            currentPtr += sizeof(TaskDialogButtonStruct) * page.RadioButtons.Count;

                            Align(ref currentPtr, sizeof(char));
                            for (int i = 0; i < page.RadioButtons.Count; i++)
                            {
                                TaskDialogRadioButton currentCustomButton = page.RadioButtons[i];
                                customRadioButtonStructs[i] = new TaskDialogButtonStruct()
                                {
                                    nButtonID = currentCustomButton.RadioButtonID,
                                    pszButtonText = MarshalString(currentCustomButton.Text)
                                };
                            }
                        }

                        Debug.Assert(currentPtr == (long)ptrTaskDialogConfig + sizeToAllocate);

                        IntPtr MarshalString(string str)
                        {
                            if (str == null)
                            {
                                return IntPtr.Zero;
                            }

                            fixed (char* strPtr = str)
                            {
                                // Copy the string. The C# language specification guarantees
                                // that a char* value produced by using the 'fixed'
                                // statement on a string always points to a null-terminated
                                // string, so we don't need to copy a NUL character
                                // separately.
                                long bytesToCopy = SizeOfString(str);
                                Buffer.MemoryCopy(strPtr, currentPtr, bytesToCopy, bytesToCopy);

                                var ptrToReturn = currentPtr;
                                currentPtr += bytesToCopy;
                                return (IntPtr)ptrToReturn;
                            }
                        }
                    }
                    catch
                    {
                        Marshal.FreeHGlobal(ptrToFree);
                        throw;
                    }

                    static void Align(ref byte* currentPtr, int? alignment = null)
                    {
                        if (alignment <= 0)
                        {
                            throw new ArgumentOutOfRangeException(nameof(alignment));
                        }

                        // Align the pointer to the next align size. If not specified,
                        // we will use the pointer (register) size.
                        uint add = (uint)(alignment ?? IntPtr.Size) - 1;
                        // TODO: Use nuint (System.UIntN) once available to avoid the
                        // separate code for 32-bit and 64-bit pointer sizes.
                        if (IntPtr.Size == 8)
                        {
                            // Disable incorrect IDE warning as the latter cast is
                            // actually not redundant.
                            // See: https://github.com/dotnet/roslyn/issues/20617
#pragma warning disable IDE0004 // Remove Unnecessary Cast
                            currentPtr = (byte*)(((ulong)currentPtr + add) & ~(ulong)add);
#pragma warning restore IDE0004 // Remove Unnecessary Cast
                        }
                        else
                        {
                            currentPtr = (byte*)(((uint)currentPtr + add) & ~add);
                        }
                    }

                    static long SizeOfString(string str)
                    {
                        return str == null ? 0 : ((long)str.Length + 1) * sizeof(char);
                    }
                }
            }
            catch
            {
                // Unbind the page, then rethrow the exception.
                page.Unbind();

                throw;
            }
        }

        private void SubclassWindow()
        {
            if (_windowSubclassHandler != null)
            {
                throw new InvalidOperationException();
            }

            // Subclass the window.
            _windowSubclassHandler = new WindowSubclassHandler(this);
            _windowSubclassHandler.Open();
        }

        private void UnsubclassWindow()
        {
            if (_windowSubclassHandler != null)
            {
                try
                {
                    _windowSubclassHandler.Dispose();
                    _windowSubclassHandler = null;
                }
                catch (Exception ex) when (ex is InvalidOperationException || ex is Win32Exception)
                {
                    // Ignore. This could happen for example if some other code
                    // also subclassed the window after us but didn't correctly
                    // revert it. However, this can mean that the callback can
                    // still be called until TaskDialogIndirect() returns, so we
                    // need to keep the delegate alive until that happens.
                }
            }
        }

        private void DenyIfBound()
        {
            if (_boundPage != null)
            {
                throw new InvalidOperationException(
                    "Cannot set this property or call this method while the " +
                    "task dialog is shown.");
            }
        }

        private void DenyIfDialogNotUpdatable(bool checkWaitingForNavigation = true)
        {
            if (!IsHandleCreated)
            {
                throw new InvalidOperationException("Can only update the state of a task dialog while it is shown.");
            }

            // When we wait for the navigated event to occur, also don't allow to
            // update the dialog because that could produce an unexpected state
            // (because it will change its size for the new page, but if we then
            // updated e.g. the text or instruction, it would update its size again
            // for the current page, and it would keep the (wrong) size after
            // navigation).
            // An exception is e.g. a button click (as that doesn't manipulate
            // the layout) so that the user can close the dialog even though we are
            // waiting for the navigation to finish.
            if (_waitingNavigationPages.Count > 0 && checkWaitingForNavigation)
            {
                throw new InvalidOperationException(
                    "Cannot manipulate the task dialog immediately after navigating it. " +
                    $"Please wait for the " +
                    $"{nameof(TaskDialogPage)}.{nameof(TaskDialogPage.Created)} " +
                    $"event of the next page to occur.");
            }
        }

        /// <summary>
        ///      Called when an exception occurs in dispatching messages through
        ///      the task dialog callback or its window procedure
        /// </summary>
        private void HandleCallbackException(Exception e)
        {
            Application.OnThreadException(e);
        }

        private void SendTaskDialogMessage(
            TaskDialogMessage message,
            int wParam,
            IntPtr lParam,
            bool checkWaitingForNavigation = true)
        {
            DenyIfDialogNotUpdatable(checkWaitingForNavigation);

            UnsafeNativeMethods.SendMessage(
                new HandleRef(this, _hwndDialog),
                (int)message,
                // Note: When a negative 32-bit integer is converted to a
                // 64-bit pointer, the high dword will be set to 0xFFFFFFFF.
                // This is consistent with the conversion from int to
                // WPARAM (in C) as shown in the Task Dialog documentation.
                (IntPtr)wParam,
                lParam);
        }

        /// <summary>
        /// Forces the task dialog to update its window size according to its contents.
        /// </summary>
        private void UpdateWindowSize()
        {
            // Force the task dialog to update its size by doing an arbitrary
            // update of one of its text elements (as the TDM_SET_ELEMENT_TEXT
            // causes the size/layout to be updated).
            // We use the MainInstruction because it cannot contain hyperlinks
            // (and therefore there is no risk that one of the links loses focus).
            UpdateTextElement(TaskDialogTextElement.TDE_MAIN_INSTRUCTION, _boundPage.Instruction);
        }
    }
}
