// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///   A task dialog allows to display information and get simple input from the user. It is similar
    ///   to a <see cref="MessageBox"/> (in that it is formatted by the operating system) but provides
    ///   a lot more features.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   For more information, see
    /// <see href="https://docs.microsoft.com/windows/desktop/Controls/task-dialogs-overview">About Task Dialogs</see>.
    /// </para>
    /// <para>
    ///   Note: In order to use the dialog, you need ensure <see cref="Application.EnableVisualStyles"/>
    ///   has been called before showing the dialog, or the application needs to be compiled with a
    ///   manifest that contains a dependency to Microsoft.Windows.Common-Controls (6.0.0.0).
    ///   Additionally, the current thread should use the single-threaded apartment (STA) model.
    /// </para>
    /// </remarks>
    public partial class TaskDialog : IWin32Window
    {
        /// <summary>
        ///   A self-defined window message that we post to the task dialog when
        ///   handling a <see cref="ComCtl32.TDN.BUTTON_CLICKED"/>
        ///   notification, so that we will ignore further
        ///   <see cref="ComCtl32.TDN.BUTTON_CLICKED"/> notifications
        ///   until we process the posted message.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   This is used to work-around a bug in the native task dialog, where
        ///   a <see cref="ComCtl32.TDN.BUTTON_CLICKED"/> notification
        ///   seems to be sent twice to the callback when you "click" a button by
        ///   pressing its access key (mnemonic) and the dialog is still open when
        ///   continuing the message loop.
        /// </para>
        /// <para>
        ///   This work-around should not have negative effects, such as erroneously
        ///   ignoring a valid button clicked notification when the user presses the
        ///   button multiple times while the GUI thread is hangs - this seems
        ///   to work correctly, as our posted message will be processed before
        ///   further (valid) <see cref="ComCtl32.TDN.BUTTON_CLICKED"/>
        ///   notifications are processed.
        /// </para>
        /// <para>
        ///   See documentation/repro in
        ///   /Documentation/src/System/Windows/Forms/TaskDialog/Issue_ButtonClickHandlerCalledTwice.md
        /// </para>
        /// <para>
        ///   Note: We use a WM_APP message with a high value (WM_USER is not
        ///   appropriate as it is private to the control class), in order to avoid
        ///   conflicts with WM_APP messages which other parts of the application
        ///   might want to send when they also subclassed the dialog window, although
        ///   that should be unlikely.
        /// </para>
        /// </remarks>
        private const User32.WM ContinueButtonClickHandlingMessage = User32.WM.APP + 0x3FFF;

        /// <summary>
        ///   The delegate for <see cref="HandleTaskDialogNativeCallback"/> which is
        ///   marshaled as native callback.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   This delegate must be kept alive (protecting it from garbage collection)
        ///   to ensure the native function pointer doesn't become invalid.
        /// </para>
        /// </remarks>
        // Because marshaling a delegate this will allocate some memory required
        // to store the native code for the function pointer, we only do this
        // once by using a static function, and then identify the actual TaskDialog
        // instance by using a GCHandle in the reference data field (like an
        // object pointer).
        private static readonly ComCtl32.PFTASKDIALOGCALLBACK s_callbackProcDelegate =
            HandleTaskDialogNativeCallback;

        private TaskDialogPage? _currentPage;
        private TaskDialogPage? _boundPage;

        /// <summary>
        ///   A qeueue of <see cref="TaskDialogPage"/>s that have been bound by
        ///   navigating the dialog, but don't yet reflect the state of the
        ///   native dialog because the corresponding
        ///   <see cref="ComCtl32.TDN.NAVIGATED"/> notification was
        ///   not yet received.
        /// </summary>
        private readonly Queue<TaskDialogPage> _waitingNavigationPages = new Queue<TaskDialogPage>();

        /// <summary>
        ///   The <see cref="IntPtr"/> of a <see cref="GCHandle"/> that represents this
        ///   <see cref="TaskDialog"/> instance.
        /// </summary>
        private IntPtr _instanceHandlePtr;

        private WindowSubclassHandler? _windowSubclassHandler;

        /// <summary>
        ///   Stores a value that indicates if the
        ///   <see cref="TaskDialogPage.Created"/> event has been called for the
        ///   current <see cref="TaskDialogPage"/> and so the corresponding
        ///   <see cref="TaskDialogPage.Destroyed"/> can be called later.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   This is used to prevent raising the
        ///   <see cref="TaskDialogPage.Destroyed"/> event without raising the
        ///   <see cref="TaskDialogPage.Created"/> event first (e.g. if navigation
        ///   fails).
        /// </para>
        /// </remarks>
        private bool _raisedPageCreated;

        /// <summary>
        ///   A counter which is used to determine whether the dialog has been navigated
        ///   while being in a <see cref="ComCtl32.TDN.BUTTON_CLICKED"/> handler.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   When the dialog navigates within a ButtonClicked handler, the handler should
        ///   always return S_FALSE to prevent the dialog from applying the button that
        ///   raised the handler as dialog result. Otherwise, this can lead to memory access
        ///   problems like <see cref="AccessViolationException"/>s, especially if the
        ///   previous dialog page had radio buttons (but the new ones do not).
        /// </para>
        /// <para>
        ///   See the comment in <see cref="HandleTaskDialogCallback"/> for more
        ///   information.
        /// </para>
        /// <para>
        ///   When the dialog navigates, it sets the <c>navigationIndex</c> to the current
        ///   <c>stackCount</c> value, so that the ButtonClicked handler can determine
        ///   if the dialog has been navigated after it was called.
        ///   Tracking the stack count and navigation index is necessary as there
        ///   can be multiple ButtonClicked handlers on the call stack, for example
        ///   if a ButtonClicked handler runs the message loop so that new click events
        ///   can be processed.
        /// </para>
        /// </remarks>
        private (int stackCount, int navigationIndex) _buttonClickNavigationCounter;

        /// <summary>
        ///   The button designated as the dialog result by the handler for the
        ///   <see cref="ComCtl32.TDN.BUTTON_CLICKED"/>
        ///   notification.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   This will be set the first time the
        ///   <see cref="ComCtl32.TDN.BUTTON_CLICKED"/> handler returns
        ///   <see cref="HRESULT.S_OK"/> to cache the button instance,
        ///   so that <see cref="ShowDialog(IntPtr, TaskDialogPage, TaskDialogStartupLocation)"/> can then return it.
        /// </para>
        /// <para>
        ///   Additionally, this is used to check if there was already a
        ///   <see cref="ComCtl32.TDN.BUTTON_CLICKED"/> handler that
        ///   returned <see cref="HRESULT.S_OK"/>, so that further
        ///   handles will return <see cref="HRESULT.S_FALSE"/> to
        ///   not override the previously set result.
        /// </para>
        /// </remarks>
        private (TaskDialogButton button, int buttonID)? _resultButton;

        private bool _suppressButtonClickedEvent;

        /// <summary>
        ///   Specifies if the current code is called from within
        ///   <see cref="Navigate(TaskDialogPage)"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   This is used to detect if you call <see cref="Navigate(TaskDialogPage)"/>
        ///   from within an event raised by <see cref="Navigate(TaskDialogPage)"/>,
        ///   which is not supported.
        /// </para>
        /// </remarks>
        private bool _isInNavigate;

        /// <summary>
        ///   Specifies if the <see cref="HandleTaskDialogCallback"/> method should
        ///   currently ignore <see cref="ComCtl32.TDN.BUTTON_CLICKED"/>
        ///   notifications.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   See <see cref="ContinueButtonClickHandlingMessage"/> for more information.
        /// </para>
        /// </remarks>
        private bool _ignoreButtonClickedNotifications;

        /// <summary>
        ///   Specifies if the current <see cref="ComCtl32.TDN.BUTTON_CLICKED"/> notification
        ///   is caused by our own code sending a <see cref="ComCtl32.TDM.CLICK_BUTTON"/> message.
        /// </summary>
        private bool _buttonClickedProgrammatically;

        /// <summary>
        ///   Specifies if the currently showing task dialog already received the
        ///   <see cref="ComCtl32.TDN.DESTROYED"/> notification.
        /// </summary>
        private bool _receivedDestroyedNotification;

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialog"/> class using the
        ///   specified task dialog page.
        /// </summary>
        private TaskDialog()
        {
        }

        /// <summary>
        ///   Gets the window handle of the task dialog window, or <see cref="IntPtr.Zero"/>
        ///   if the dialog is currently not being shown.
        /// </summary>
        public IntPtr Handle { get; private set; }

        /// <summary>
        ///   Gets a value that indicates whether <see cref="ShowDialog(IntPtr, TaskDialogPage, TaskDialogStartupLocation)"/> is
        ///   currently being called.
        /// </summary>
        internal bool IsShown
        {
            get => _instanceHandlePtr != IntPtr.Zero;
        }

        /// <summary>
        ///   Gets a value that indicates whether the native task dialog window has
        ///   been created and its handle is available using the <see cref="Handle"/>
        ///   property.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   This property can only be <see langword="true"/> if <see cref="IsShown"/> is
        ///   also <see langword="true"/>. However, normally this property should be equivalent
        ///   to <see cref="IsShown"/>, because when showing the dialog, the
        ///   callback should have been called setting the handle.
        /// </para>
        /// </remarks>
        internal bool IsHandleCreated
        {
            get => Handle != IntPtr.Zero;
        }

        /// <summary>
        ///   Gets or sets the current count of stack frames that are in the
        ///   <see cref="TaskDialogRadioButton.CheckedChanged"/> event for the
        ///   current task dialog.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   This is used by the <see cref="TaskDialogRadioButton.Checked"/> setter
        ///   so that it can disallow the change when the count is greater than zero.
        ///   Additionally, it is used to deny navigation of the task dialog in that
        ///   case.
        /// </para>
        /// </remarks>
        internal int RadioButtonClickedStackCount
        {
            get;
            set;
        }

        private static void FreeConfig(IntPtr ptrToFree) => Marshal.FreeHGlobal(ptrToFree);

        private static HRESULT HandleTaskDialogNativeCallback(
            IntPtr hwnd,
            ComCtl32.TDN msg,
            IntPtr wParam,
            IntPtr lParam,
            IntPtr lpRefData) =>
            // Call the instance method by dereferencing the GC handle.
            (((GCHandle)lpRefData).Target as TaskDialog)!.HandleTaskDialogCallback(
                hwnd,
                msg,
                wParam,
                lParam);

        private static bool IsTaskDialogButtonCommitting(TaskDialogButton? button)
        {
            // All custom button as well as all standard buttons except for the
            // "Help" button (if it is shown in the dialog) will close the
            // dialog. If the "Help" button is not shown in the task dialog,
            // "button" will be null or its "IsCreated" property returns false.
            // In that case the "Help" button would close the dialog, so we
            // return true.
            return !(button?.IsCreated == true && button.IsStandardButton &&
                button.StandardButtonResult == TaskDialogResult.Help);
        }

        private static TaskDialogButton CreatePlaceholderButton(TaskDialogResult result)
        {
            // TODO: Maybe bind the button so that the user
            // cannot change the properties, like it is the
            // case with the regular buttons added to the
            // collections.
            return new TaskDialogButton(result)
            {
                Visible = false
            };
        }

#pragma warning disable RS0026 // Do not add multiple public overloads with optional parameters
        /// <summary>
        ///   Shows the task dialog.
        /// </summary>
        /// <param name="page">
        ///   The page instance that contains the contents which this task dialog will display.
        /// </param>
        /// <param name="startupLocation">
        ///   Gets or sets the position of the task dialog when it is shown.
        /// </param>
        /// <remarks>
        ///   <para>
        ///     Showing the dialog will bind the <paramref name="page"/> and its controls until
        ///     this method returns or the dialog is navigated to a different page.
        ///   </para>
        /// </remarks>
        /// <returns>
        ///   The <see cref="TaskDialogButton"/> which was clicked by the user to close the dialog.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="page"/> is <see langword="null"/>.
        /// </exception>
        public static TaskDialogButton ShowDialog(TaskDialogPage page,
                                                  TaskDialogStartupLocation startupLocation = TaskDialogStartupLocation.CenterParent)
            => ShowDialog(IntPtr.Zero,
                          page ?? throw new ArgumentNullException(nameof(page)),
                          startupLocation);

        /// <summary>
        ///   Shows the task dialog with the specified owner.
        /// </summary>
        /// <param name="page">
        ///   The page instance that contains the contents which this task dialog will display.
        /// </param>
        /// <param name="owner">The owner window, or <see langword="null"/> to show a modeless dialog.</param>
        /// <param name="startupLocation">
        ///   Gets or sets the position of the task dialog when it is shown.
        /// </param>
        /// <remarks>
        ///   <para>
        ///     Showing the dialog will bind the <paramref name="page"/> and its controls until
        ///     this method returns or the dialog is navigated to a different page.
        ///   </para>
        /// </remarks>
        /// <returns>
        ///   The <see cref="TaskDialogButton"/> which was clicked by the user to close the dialog.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="owner"/> is <see langword="null"/>
        ///   - or -
        ///   <paramref name="page"/> is <see langword="null"/>.
        /// </exception>
        public static TaskDialogButton ShowDialog(IWin32Window owner, TaskDialogPage page,
                                                  TaskDialogStartupLocation startupLocation = TaskDialogStartupLocation.CenterParent)
            => ShowDialog(owner?.Handle ?? throw new ArgumentNullException(nameof(owner)),
                          page ?? throw new ArgumentNullException(nameof(page)),
                          startupLocation);

        /// <summary>
        ///   Shows the task dialog with the specified owner.
        /// </summary>
        /// <param name="page">
        ///   The page instance that contains the contents which this task dialog will display.
        /// </param>
        /// <param name="hwndOwner">
        ///   The handle of the owner window, or <see cref="IntPtr.Zero"/> to show a
        ///   modeless dialog.
        /// </param>
        /// <param name="startupLocation">
        ///   Gets or sets the position of the task dialog when it is shown.
        /// </param>
        /// <remarks>
        ///   <para>
        ///     Showing the dialog will bind the <paramref name="page"/> and its controls until
        ///     this method returns or the dialog is navigated to a different page.
        ///   </para>
        /// </remarks>
        /// <returns>
        ///   The <see cref="TaskDialogButton"/> which was clicked by the user to close the dialog.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="hwndOwner"/> is <see langword="null"/>
        ///   - or -
        ///   <paramref name="page"/> is <see langword="null"/>.
        /// </exception>
        public static unsafe TaskDialogButton ShowDialog(IntPtr hwndOwner, TaskDialogPage page,
                                                  TaskDialogStartupLocation startupLocation = TaskDialogStartupLocation.CenterParent)
        {
            if (page == null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            TaskDialog dialog = new TaskDialog();
            return dialog.ShowDialogInternal(hwndOwner, page, startupLocation);
        }
#pragma warning restore RS0026 // Do not add multiple public overloads with optional parameters

        /// <summary>
        ///   Shows the task dialog with the specified owner.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   Showing the dialog will bind the <paramref name="page"/> and its controls until
        ///   this method returns or the dialog is navigated to a different page.
        /// </para>
        /// </remarks>
        /// <returns>
        ///   The <see cref="TaskDialogButton"/> which was clicked by the user to close the dialog.
        /// </returns>
        private unsafe TaskDialogButton ShowDialogInternal(IntPtr hwndOwner,
                                                  TaskDialogPage page,
                                                  TaskDialogStartupLocation startupLocation)
        {
            // Recursive Show() is not possible because a TaskDialog instance can only
            // represent a single native dialog.
            if (IsShown)
            {
                throw new InvalidOperationException(string.Format(SR.TaskDialogInstanceAlreadyShown, nameof(TaskDialog)));
            }

            page.Validate();
            _currentPage = page;

            // Allocate a GCHandle which we will use for the callback data.
            var instanceHandle = GCHandle.Alloc(this);
            try
            {
                _instanceHandlePtr = (IntPtr)instanceHandle;

                // Bind the page and allocate the memory.
                BindPageAndAllocateConfig(
                    page,
                    hwndOwner,
                    startupLocation,
                    out IntPtr ptrToFree,
                    out ComCtl32.TASKDIALOGCONFIG* ptrTaskDialogConfig);

                _boundPage = page;
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
                    // earlier). Otherwise, the "TaskDialogIndirect" entry point will
                    // not be available in comctl32.dll.
                    IntPtr themingCookie = ThemingScope.Activate(Application.UseVisualStyles);
                    HRESULT returnValue;
                    int resultButtonID;
                    try
                    {
                        returnValue = ComCtl32.TaskDialogIndirect(
                            ptrTaskDialogConfig,
                            out resultButtonID,
                            out _,
                            out _);
                    }
                    catch (EntryPointNotFoundException ex)
                    {
                        throw new InvalidOperationException(string.Format(
                            SR.TaskDialogVisualStylesNotEnabled,
                            $"{nameof(Application)}.{nameof(Application.EnableVisualStyles)}"),
                            ex);
                    }
                    finally
                    {
                        // Revert the theming scope.
                        ThemingScope.Deactivate(themingCookie);
                    }

                    // Marshal.ThrowExceptionForHR will use the IErrorInfo on the
                    // current thread if it exists.
                    if (returnValue.Failed())
                    {
                        Marshal.ThrowExceptionForHR((int)returnValue);
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
                    Debug.Assert(Handle == IntPtr.Zero);

                    // Ensure to keep the callback delegate alive until
                    // TaskDialogIndirect() returns (in case we could not undo the
                    // subclassing). See comment in UnsubclassWindow().
                    _windowSubclassHandler?.KeepCallbackDelegateAlive();
                    // Then, clear the subclass handler. Note that this only works
                    // correctly if we did not return from TaskDialogIndirect()
                    // due to an exception being thrown (as mentioned above).
                    _windowSubclassHandler = null;

                    // Also, ensure the window handle and the
                    // raiseClosed/raisePageDestroyed flags are is cleared even if
                    // the TDN_DESTROYED notification did not occur (although that
                    // should only happen when there was an exception).
                    Handle = IntPtr.Zero;
                    _raisedPageCreated = false;

                    // Clear cached objects and other fields.
                    _resultButton = null;
                    _ignoreButtonClickedNotifications = false;
                    _receivedDestroyedNotification = false;

                    // Unbind the page. The 'Destroyed' event of the TaskDialogPage
                    // will already have been called from the callback.
                    _boundPage.Unbind();
                    _boundPage = null;

                    // If we started navigating the dialog but navigation wasn't
                    // successful, we also need to unbind the new pages.
                    foreach (TaskDialogPage dialogPage in _waitingNavigationPages)
                    {
                        dialogPage.Unbind();
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
                    // Note: As this is a static field, the call to GC.KeepAlive() might be
                    // superfluous here, but we still do it to be safe.
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
        ///   Closes the shown task dialog with
        ///   <see cref="TaskDialogButton.Cancel"/> as resulting button.
        /// </summary>
        /// <remarks>
        /// <para>
        /// To close the dialog with a different result, call the
        /// <see cref="TaskDialogButton.PerformClick"/> method of the
        /// <see cref="TaskDialogButton"/> that you want to set as a result.
        /// </para>
        /// <para>
        ///   Note: This method can be called while the dialog is waiting for
        ///   navigation to complete, whereas <see cref="TaskDialogButton.PerformClick"/>
        ///   would throw in that case. When calling this method, the
        ///   <see cref="TaskDialogButton.Click"/> event won't be raised.
        /// </para>
        /// </remarks>
        public void Close()
        {
            _suppressButtonClickedEvent = true;
            try
            {
                // Send a click button message with the cancel result.
                // Note: We allow to click the cancel button even if we are waiting
                // for the TDN_NAVIGATED notification to occur, as in that case the
                // dialog will behave as if it still contains the controls of the
                // previous page, and we can click a standard button without the page
                // actually having to contain that button.
                ClickButton((int)TaskDialogResult.Cancel, false);
            }
            finally
            {
                _suppressButtonClickedEvent = false;
            }
        }

        /// <summary>
        ///   While the dialog is being shown, switches the progress bar mode to either a
        ///   marquee progress bar or to a regular progress bar.
        ///   For a marquee progress bar, you can enable or disable the marquee using
        ///   <see cref="SetProgressBarMarquee(bool, int)"/>.
        /// </summary>
        /// <param name="marqueeProgressBar"></param>
        internal void SwitchProgressBarMode(bool marqueeProgressBar) => SendTaskDialogMessage(
            ComCtl32.TDM.SET_MARQUEE_PROGRESS_BAR,
            PARAM.FromBool(marqueeProgressBar),
            IntPtr.Zero);

        /// <summary>
        ///   While the dialog is being shown, enables or disables progress bar marquee when
        ///   a marquee progress bar is displayed.
        /// </summary>
        /// <param name="enableMarquee"></param>
        /// <param name="animationSpeed">
        /// The time in milliseconds between marquee animation updates. If <c>0</c>, the
        /// animation will be updated every 30 milliseconds.
        /// </param>
        internal unsafe void SetProgressBarMarquee(bool enableMarquee, int animationSpeed = 0)
        {
            if (animationSpeed < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(animationSpeed));
            }

            SendTaskDialogMessage(
                ComCtl32.TDM.SET_PROGRESS_BAR_MARQUEE,
                PARAM.FromBool(enableMarquee),
                (IntPtr)(void*)(uint)animationSpeed);
        }

        /// <summary>
        ///   While the dialog is being shown, sets the progress bar range.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <remarks>
        /// <para>
        ///   The default range is 0 to 100.
        /// </para>
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

            SendTaskDialogMessage(
                ComCtl32.TDM.SET_PROGRESS_BAR_RANGE,
                IntPtr.Zero,
                PARAM.FromLowHighUnsigned(min, max));
        }

        /// <summary>
        ///   While the dialog is being shown, sets the progress bar position.
        /// </summary>
        /// <param name="pos"></param>
        internal void SetProgressBarPosition(int pos)
        {
            if (pos < 0 || pos > ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(pos));
            }

            SendTaskDialogMessage(
                ComCtl32.TDM.SET_PROGRESS_BAR_POS,
                (IntPtr)pos,
                IntPtr.Zero);
        }

        /// <summary>
        ///   While the dialog is being shown, sets the progress bar state.
        /// </summary>
        /// <param name="state"></param>
        internal void SetProgressBarState(ComCtl32.PBST state) => SendTaskDialogMessage(
            ComCtl32.TDM.SET_PROGRESS_BAR_STATE,
            (IntPtr)state,
            IntPtr.Zero);

        /// <summary>
        ///   While the dialog is being shown, sets the checkbox to the specified
        ///   state.
        /// </summary>
        /// <param name="isChecked"></param>
        /// <param name="focus"></param>
        internal void ClickCheckBox(bool isChecked, bool focus = false) => SendTaskDialogMessage(
            ComCtl32.TDM.CLICK_VERIFICATION,
            PARAM.FromBool(isChecked),
            PARAM.FromBool(focus));

        internal void SetButtonElevationRequiredState(int buttonID, bool requiresElevation) => SendTaskDialogMessage(
            ComCtl32.TDM.SET_BUTTON_ELEVATION_REQUIRED_STATE,
            (IntPtr)buttonID,
            PARAM.FromBool(requiresElevation));

        internal void SetButtonEnabled(int buttonID, bool enable) => SendTaskDialogMessage(
            ComCtl32.TDM.ENABLE_BUTTON,
            (IntPtr)buttonID,
            PARAM.FromBool(enable));

        internal void SetRadioButtonEnabled(int radioButtonID, bool enable) => SendTaskDialogMessage(
            ComCtl32.TDM.ENABLE_RADIO_BUTTON,
            (IntPtr)radioButtonID,
            PARAM.FromBool(enable));

        internal void ClickButton(int buttonID, bool checkWaitingForNavigation = true)
        {
            // Allow the handler of the TDN_BUTTON_CLICKED notification to detect that
            // the notification is caused by ourselves sending a TDM_CLICK_BUTTON message.
            _buttonClickedProgrammatically = true;
            try
            {
                SendTaskDialogMessage(
                    ComCtl32.TDM.CLICK_BUTTON,
                    (IntPtr)buttonID,
                    IntPtr.Zero,
                    checkWaitingForNavigation);
            }
            finally
            {
                // In most cases the flag will already have been set to false in the
                // TDN_BUTTON_CLICKED handler, but we do it again in case the notification
                // wasn't handled for some reason or an exception was thrown.
                _buttonClickedProgrammatically = false;
            }
        }

        internal void ClickRadioButton(int radioButtonID) => SendTaskDialogMessage(
            ComCtl32.TDM.CLICK_RADIO_BUTTON,
            (IntPtr)radioButtonID,
            IntPtr.Zero);

        internal unsafe void UpdateTextElement(ComCtl32.TDE element, string? text)
        {
            // Instead of null, we must specify the empty string; otherwise the update
            // would be ignored.
            text ??= string.Empty;

            // We can just pin the string because sending the message should take
            // a very short time only and the string will not be modified.
            fixed (char* textPtr = text)
            {
                // Note: SetElementText will resize the dialog while UpdateElementText
                // will not (which would lead to clipped controls), so we use the
                // former.
                SendTaskDialogMessage(ComCtl32.TDM.SET_ELEMENT_TEXT, (IntPtr)element, (IntPtr)textPtr);
            }
        }

        internal void UpdateIconElement(ComCtl32.TDIE element, IntPtr icon)
        {
            // Note: Updating the icon doesn't cause the task dialog to update
            // its size; in contrast to the text updates where we use a
            // TDM_SET_... message.
            //
            // For example, if you initially didn't specify an icon but later want to
            // set one, the dialog contents might get clipped.
            //
            // To fix this, we call UpdateWindowSize() after updating the icon, to
            // force the task dialog to update its size.
            SendTaskDialogMessage(ComCtl32.TDM.UPDATE_ICON, (IntPtr)element, icon);
            UpdateWindowSize();
        }

        internal void UpdateCaption(string? caption)
        {
            // We must not allow to change the caption if we are currently
            // waiting for a TDN_NAVIGATED notification, because in that case
            // the task dialog will already have set the caption from the new page.
            DenyIfDialogNotUpdatable();

            // Note: Because we use SetWindowText() directly (as there is no task
            // dialog message for setting the title), there is a small discrepancy
            // between specifying an empty title in the TASKDIALOGCONFIG structure
            // and setting an empty title with this method: An empty string (or null)
            // in the TASKDIALOGCONFIG struture causes the dialog title to be the
            // executable name (e.g. "MyApplication.exe"), but using an empty string
            // (or null) with SetWindowText() causes the window title to be empty.
            // Therefore, we replicate the task dialog behavior by also using the
            // executable's name as title if the string is null or empty.
            if (TaskDialogPage.IsNativeStringNullOrEmpty(caption))
            {
                caption = Path.GetFileName(
                    UnsafeNativeMethods.GetModuleFileNameLongPath(NativeMethods.NullHandleRef)
                    .ToString());
            }

            User32.SetWindowTextW(Handle, caption);
        }

        private HRESULT HandleTaskDialogCallback(
            IntPtr hWnd,
            ComCtl32.TDN notification,
            IntPtr wParam,
            IntPtr lParam)
        {
            Debug.Assert(_boundPage != null);

            // Set the hWnd as this may be the first time that we get it.
            bool isFirstNotification = Handle == IntPtr.Zero;
            Handle = hWnd;

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
                    case ComCtl32.TDN.CREATED:
                        _boundPage.ApplyInitialization();

                        // Don't raise the Created event of the bound page if we are
                        // waiting for the TDN_NAVIGATED notification, because that means
                        // the user has already navigated the dialog in one of the
                        // previous events, so eventually the TDN_NAVIGATED notification
                        // will occur where we will raise the Created event for the new
                        // page.
                        if (_waitingNavigationPages.Count == 0 && !_raisedPageCreated)
                        {
                            _raisedPageCreated = true;
                            _boundPage.OnCreated(EventArgs.Empty);
                        }
                        break;

                    case ComCtl32.TDN.NAVIGATED:
                        // Indicate to the ButtonClicked handlers currently on the stack
                        // that we received the TDN_NAVIGATED notification.
                        _buttonClickNavigationCounter.navigationIndex = _buttonClickNavigationCounter.stackCount;

                        // We can now unbind the previous page and then switch to the
                        // new page.
                        _boundPage.Unbind();
                        _boundPage = _waitingNavigationPages.Dequeue();

                        if (_waitingNavigationPages.Count == 0)
                        {
                            // Apply the initialization only if there are no further outstanding
                            // navigations. Otherwise, this might throw an InvalidOperationException,
                            // because the update methods would deny updating the dialog when there
                            // are further outstanding navigations, as that could cause the layout
                            // of the dialog to be changed erroneously (see comment in
                            // DenyIfDialogNotUpdatable).
                            _boundPage.ApplyInitialization();

                            // Also, raise the event only if we don't wait for yet another navigation
                            // (this is the same as we do in the TDN_CREATED handler).
                            Debug.Assert(!_raisedPageCreated);
                            if (!_raisedPageCreated)
                            {
                                _raisedPageCreated = true;
                                _boundPage.OnCreated(EventArgs.Empty);
                            }
                        }

                        break;

                    case ComCtl32.TDN.DESTROYED:
                        _receivedDestroyedNotification = true;

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
                            Handle = IntPtr.Zero;
                        }
                        break;

                    case ComCtl32.TDN.BUTTON_CLICKED:
                        // Only do the special handling (ignoring the notification and
                        // setting the _ignoreButtonClickedNotifications flag) if this
                        // BUTTON_CLICKED notification is not caused by our own code.
                        if (_buttonClickedProgrammatically)
                        {
                            // Clear the flag as early as possible.
                            _buttonClickedProgrammatically = false;
                        }
                        else
                        {
                            // Check if we should ignore this notification. If we process
                            // it, we set a flag to ignore further TDN_BUTTON_CLICKED
                            // notifications, and we post a message to the task dialog
                            // that, when we process it, causes us to reset the flag.
                            // This is used to work-around the access key bug in the
                            // native task dialog - see the remarks of the
                            // "ContinueButtonClickHandlingMessage" for more information.
                            if (_ignoreButtonClickedNotifications)
                            {
                                return HRESULT.S_FALSE;
                            }

                            // Post the message, and then set the flag to ignore further
                            // notifications until we receive the posted message.
                            if (User32.PostMessageW(
                                hWnd,
                                ContinueButtonClickHandlingMessage).IsTrue())
                            {
                                _ignoreButtonClickedNotifications = true;
                            }
                        }

                        int buttonID = PARAM.ToInt(wParam);
                        TaskDialogButton? button = _boundPage.GetBoundButtonByID(buttonID);

                        bool applyButtonResult = true;
                        if (button is { } && !_suppressButtonClickedEvent)
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
                                applyButtonResult = button.HandleButtonClicked();

                                // Check if the index was set to the current stack count,
                                // which means we received a TDN_NAVIGATED notification
                                // while we called the handler. In that case we need to
                                // return S_FALSE to prevent the dialog from closing
                                // (and applying the previous ButtonID and RadioButtonID
                                // as results).
                                if (_buttonClickNavigationCounter.navigationIndex >=
                                    _buttonClickNavigationCounter.stackCount)
                                {
                                    applyButtonResult = false;
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
                        if (applyButtonResult && IsTaskDialogButtonCommitting(button))
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
                                applyButtonResult = false;
                            }
                            else
                            {
                                // If we didn't find the button (e.g. when specifying
                                // AllowCancel but not adding a "Cancel" button), we need
                                // to create a new instance and save it, so that we can
                                // return that instance after TaskDialogIndirect() returns.
                                if (button is null)
                                {
                                    button = CreatePlaceholderButton((TaskDialogResult)buttonID);
                                }

                                // Cache the result button if we return S_OK.
                                _resultButton = (button, buttonID);
                            }
                        }

                        return applyButtonResult ? HRESULT.S_OK : HRESULT.S_FALSE;

                    case ComCtl32.TDN.RADIO_BUTTON_CLICKED:
                        int radioButtonID = PARAM.ToInt(wParam);
                        TaskDialogRadioButton radioButton = _boundPage.GetBoundRadioButtonByID(radioButtonID)!;

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

                    case ComCtl32.TDN.EXPANDO_BUTTON_CLICKED:
                        _boundPage.Expander!.HandleExpandoButtonClicked(wParam != IntPtr.Zero);
                        break;

                    case ComCtl32.TDN.VERIFICATION_CLICKED:
                        _boundPage.CheckBox!.HandleCheckBoxClicked(wParam != IntPtr.Zero);
                        break;

                    case ComCtl32.TDN.HELP:
                        _boundPage.OnHelpRequest(EventArgs.Empty);
                        break;
                }
            }
            catch (Exception ex) when (CanCatchCallbackException())
            {
                // When an exception occurs, handle it by calling the application's
                // ThreadException handler.
                // It is important that we don't let such exception bubble up to
                // the native -> managed transition, as otherwise the CLR would
                // unwind the stack even though the task dialog is still shown,
                // which means invalid memory access may occur if the callback
                // is called again as we already freed the object pointer.
                HandleCallbackException(ex);
            }

            return HRESULT.S_OK;
        }

        /// <summary>
        ///   While the dialog is being shown, recreates the dialog from the specified
        /// <paramref name="page"/>.
        /// </summary>
        internal unsafe void Navigate(TaskDialogPage page)
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
                throw new InvalidOperationException(string.Format(
                    SR.TaskDialogCannotNavigateWithinRadioButtonCheckedChanged,
                    $"{nameof(TaskDialogRadioButton)}.{nameof(TaskDialogRadioButton.CheckedChanged)}"));
            }

            // Don't allow to navigate the dialog if called from an event handler
            // (TaskDialogPage.Destroyed) that is raised from within this method.
            if (_isInNavigate)
            {
                throw new InvalidOperationException(SR.TaskDialogCannotNavigateWithinNavigationEventHandler);
            }

            // Don't allow navigation if the dialog window is already closed (and
            // therefore has set a result button), because that would result in
            // weird/undefined behavior (e.g. returning IDCANCEL (2) as button result
            // even though a different button has already been set as result).
            // Additionally, we check if we received the TDN_DESTROYED notification, in case
            // the dialog was closed abnormally without a prior TDN_BUTTON_CLICKED
            // notification (e.g. when closing the main application window while a modeless
            // task dialog is showing).
            if (_resultButton != null || _receivedDestroyedNotification)
            {
                throw new InvalidOperationException(SR.TaskDialogCannotNavigateClosedDialog);
            }

            _isInNavigate = true;
            try
            {
                page.Validate();

                // Need to raise the "Destroyed" event for the current page. The
                // "Created" event for the new page will occur from the callback.
                // Note: "this.raisedPageCreated" should always be true here.
                if (_raisedPageCreated)
                {
                    _raisedPageCreated = false;
                    _boundPage!.OnDestroyed(EventArgs.Empty);

                    // Need to check again if the dialog has not already been closed,
                    // since the Destroyed event handler could have performed a
                    // button click that closed the dialog.
                    // TODO: Another option would be to disallow button clicks while
                    // within the event handler.
                    if (_resultButton != null)
                    {
                        throw new InvalidOperationException(SR.TaskDialogCannotNavigateClosedDialog);
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

            TaskDialogPage previousPage = _currentPage!;
            _currentPage = page;
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
                    out IntPtr ptrToFree,
                    out ComCtl32.TASKDIALOGCONFIG* ptrTaskDialogConfig);
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
                            ComCtl32.TDM.NAVIGATE_PAGE,
                            IntPtr.Zero,
                            (IntPtr)ptrTaskDialogConfig,
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
                _currentPage = previousPage;
                throw;
            }
        }

        private unsafe void BindPageAndAllocateConfig(
            TaskDialogPage page,
            IntPtr hwndOwner,
            TaskDialogStartupLocation startupLocation,
            out IntPtr ptrToFree,
            out ComCtl32.TASKDIALOGCONFIG* ptrTaskDialogConfig)
        {
            page.Bind(
                this,
                out ComCtl32.TDF flags,
                out ComCtl32.TDCBF standardButtonFlags,
                out IEnumerable<(int buttonID, string text)> customButtonElements,
                out IEnumerable<(int buttonID, string text)> radioButtonElements,
                out ComCtl32.TASKDIALOGCONFIG.IconUnion mainIcon,
                out ComCtl32.TASKDIALOGCONFIG.IconUnion footerIcon,
                out int defaultButtonID,
                out int defaultRadioButtonID);

            try
            {
                if (startupLocation == TaskDialogStartupLocation.CenterParent)
                {
                    flags |= ComCtl32.TDF.POSITION_RELATIVE_TO_WINDOW;
                }

                checked
                {
                    // First, calculate the necessary memory size we need to allocate for
                    // all structs and strings.
                    // Note: Each Align() call when calculating the size must correspond
                    // with a Align() call when incrementing the pointer.
                    // Use a byte pointer so we can use byte-wise pointer arithmetics.
                    var sizeToAllocate = (byte*)0;
                    sizeToAllocate += sizeof(ComCtl32.TASKDIALOGCONFIG);

                    // Strings in TasDialogConfig
                    Align(ref sizeToAllocate, sizeof(char));
                    sizeToAllocate += SizeOfString(page.Caption);
                    sizeToAllocate += SizeOfString(page.MainInstruction);
                    sizeToAllocate += SizeOfString(page.Text);
                    sizeToAllocate += SizeOfString(page.CheckBox?.Text);
                    sizeToAllocate += SizeOfString(page.Expander?.Text);
                    sizeToAllocate += SizeOfString(page.Expander?.ExpandedButtonText);
                    sizeToAllocate += SizeOfString(page.Expander?.CollapsedButtonText);
                    sizeToAllocate += SizeOfString(page.Footer?.Text);

                    // Buttons array
                    if (customButtonElements.Any())
                    {
                        // Note: Theoretically we would not need to align the pointer here
                        // since the packing of the structure is set to 1. Note that this
                        // can cause an unaligned write when assigning the structure (the
                        // same happens with TaskDialogConfig).
                        Align(ref sizeToAllocate);
                        sizeToAllocate += sizeof(ComCtl32.TASKDIALOG_BUTTON) * customButtonElements.Count();

                        // Strings in buttons array
                        Align(ref sizeToAllocate, sizeof(char));
                        foreach ((int buttonID, string text) in customButtonElements)
                        {
                            sizeToAllocate += SizeOfString(text);
                        }
                    }

                    // Radio buttons array
                    if (radioButtonElements.Any())
                    {
                        // See comment above regarding alignment.
                        Align(ref sizeToAllocate);
                        sizeToAllocate += sizeof(ComCtl32.TASKDIALOG_BUTTON) * radioButtonElements.Count();

                        // Strings in radio buttons array
                        Align(ref sizeToAllocate, sizeof(char));
                        foreach ((int buttonID, string text) in radioButtonElements)
                        {
                            sizeToAllocate += SizeOfString(text);
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
                        ptrTaskDialogConfig = (ComCtl32.TASKDIALOGCONFIG*)currentPtr;

                        ref ComCtl32.TASKDIALOGCONFIG taskDialogConfig = ref *ptrTaskDialogConfig;
                        currentPtr += sizeof(ComCtl32.TASKDIALOGCONFIG);

                        // Assign the structure with the constructor syntax, which will
                        // automatically initialize its other members with their default
                        // value.
                        Align(ref currentPtr, sizeof(char));
                        taskDialogConfig = new ComCtl32.TASKDIALOGCONFIG()
                        {
                            cbSize = (uint)sizeof(ComCtl32.TASKDIALOGCONFIG),
                            hwndParent = hwndOwner,
                            dwFlags = flags,
                            dwCommonButtons = (ComCtl32.TDCBF)standardButtonFlags,
                            mainIcon = mainIcon,
                            footerIcon = footerIcon,
                            pszWindowTitle = MarshalString(page.Caption),
                            pszMainInstruction = MarshalString(page.MainInstruction),
                            pszContent = MarshalString(page.Text),
                            pszVerificationText = MarshalString(page.CheckBox?.Text),
                            pszExpandedInformation = MarshalString(page.Expander?.Text),
                            pszExpandedControlText = MarshalString(page.Expander?.ExpandedButtonText),
                            pszCollapsedControlText = MarshalString(page.Expander?.CollapsedButtonText),
                            pszFooter = MarshalString(page.Footer?.Text),
                            nDefaultButton = defaultButtonID,
                            nDefaultRadioButton = defaultRadioButtonID,
                            pfCallback = Marshal.GetFunctionPointerForDelegate(s_callbackProcDelegate),
                            lpCallbackData = _instanceHandlePtr,
                            cxWidth = checked((uint)page.Width)
                        };

                        // Buttons array
                        if (customButtonElements.Any())
                        {
                            int customButtonCount = customButtonElements.Count();

                            Align(ref currentPtr);
                            var customButtonStructs = (ComCtl32.TASKDIALOG_BUTTON*)currentPtr;
                            taskDialogConfig.pButtons = customButtonStructs;
                            taskDialogConfig.cButtons = (uint)customButtonCount;
                            currentPtr += sizeof(ComCtl32.TASKDIALOG_BUTTON) * customButtonCount;

                            Align(ref currentPtr, sizeof(char));
                            int i = 0;
                            foreach ((int buttonID, string text) in customButtonElements)
                            {
                                customButtonStructs[i] = new ComCtl32.TASKDIALOG_BUTTON()
                                {
                                    nButtonID = buttonID,
                                    pszButtonText = MarshalString(text)
                                };

                                i++;
                            }
                        }

                        // Radio buttons array
                        if (radioButtonElements.Any())
                        {
                            int radioButtonCount = radioButtonElements.Count();

                            Align(ref currentPtr);
                            var radioButtonStructs = (ComCtl32.TASKDIALOG_BUTTON*)currentPtr;
                            taskDialogConfig.pRadioButtons = radioButtonStructs;
                            taskDialogConfig.cRadioButtons = (uint)radioButtonCount;
                            currentPtr += sizeof(ComCtl32.TASKDIALOG_BUTTON) * radioButtonCount;

                            Align(ref currentPtr, sizeof(char));
                            int i = 0;
                            foreach ((int buttonID, string text) in radioButtonElements)
                            {
                                radioButtonStructs[i] = new ComCtl32.TASKDIALOG_BUTTON()
                                {
                                    nButtonID = buttonID,
                                    pszButtonText = MarshalString(text)
                                };

                                i++;
                            }
                        }

                        Debug.Assert(currentPtr == (long)ptrTaskDialogConfig + sizeToAllocate);

                        char* MarshalString(string? str)
                        {
                            if (str == null)
                            {
                                return null;
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
                                return (char*)ptrToReturn;
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
                        // TODO: Use nuint (System.UIntN) once available instead of
                        // ulong to avoid the overhead for 32-bit platforms.
                        ulong add = (ulong)(alignment ?? IntPtr.Size) - 1;
                        currentPtr = (byte*)(((ulong)currentPtr + add) & ~add);
                    }

                    static long SizeOfString(string? str)
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
                throw new InvalidOperationException(SR.TaskDialogCannotSetPropertyOfShownDialog);
            }
        }

        private void DenyIfDialogNotUpdatable(bool checkWaitingForNavigation = true)
        {
            if (!IsHandleCreated)
            {
                throw new InvalidOperationException(SR.TaskDialogCanUpdateStateOnlyWhenShown);
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
                throw new InvalidOperationException(string.Format(
                    SR.TaskDialogCannotUpdateAfterNavigation,
                    $"{nameof(TaskDialogPage)}.{nameof(TaskDialogPage.Created)}"));
            }
        }

        private bool CanCatchCallbackException()
        {
            // Catch all exceptions, except when the NativeWindow indicates
            // that a debuggable WndProc callback should be used, in which
            // case we don't catch any exception. This is so that an attached
            // debugger can stop at the original location where the exception
            // was thrown.
            return !NativeWindow.WndProcShouldBeDebuggable;
        }

        /// <summary>
        ///   Called when an exception occurs in dispatching messages through
        ///   the task dialog callback or its window procedure.
        /// </summary>
        private void HandleCallbackException(Exception e) => Application.OnThreadException(e);

        private void SendTaskDialogMessage(
            ComCtl32.TDM message,
            IntPtr wParam,
            IntPtr lParam,
            bool checkWaitingForNavigation = true)
        {
            DenyIfDialogNotUpdatable(checkWaitingForNavigation);

            User32.SendMessageW(
                Handle,
                (User32.WM)message,
                wParam,
                lParam);
        }

        /// <summary>
        ///   Forces the task dialog to update its window size according to its contents.
        /// </summary>
        private void UpdateWindowSize()
        {
            DenyIfDialogNotUpdatable();

            // Force the task dialog to update its size by doing an arbitrary
            // update of one of its text elements (as the TDM_SET_ELEMENT_TEXT
            // causes the size/layout to be updated).
            // We use the MainInstruction because it cannot contain hyperlinks
            // (and therefore there is no risk that one of the links loses focus).
            UpdateTextElement(ComCtl32.TDE.MAIN_INSTRUCTION, _boundPage!.MainInstruction);
        }
    }
}
