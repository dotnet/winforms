// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///   Allows to subclass individual windows.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   To actually subclass the window, call <see cref="Open"/>. To ensure the
    ///   subclassing is correctly undone, you must call <see cref="Dispose()"/> before the
    ///   window is destroyed.
    /// </para>
    /// <para>
    ///   See https://docs.microsoft.com/en-us/windows/desktop/winmsg/about-window-procedures#instance-subclassing
    ///   for more information about subclassing.
    /// </para>
    /// </remarks>
    internal class WindowSubclassHandler : IDisposable
    {
        private readonly IntPtr _handle;
        private bool _opened;
        private bool _disposed;
        private IntPtr _originalWindowProc;

        /// <summary>
        ///   The delegate for <see cref="WndProc(ref Message)"/>
        ///   which is marshaled as native callback.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   We must store this delegate (and prevent it from being garbage-collected)
        ///   to ensure the function pointer doesn't become invalid.
        /// </para>
        /// <para>
        ///   Note: We create a new delegate (and native function pointer) for each
        ///   instance because even though creation will be slower (and requires a
        ///   bit of memory to store the native code) it will be faster when the window
        ///   procedure is invoked, because otherwise we would need to use a dictionary
        ///   to map the hWnd to the instance, as the window procedure doesn't allow
        ///   to store reference data. However, creating a new delegate for each instance
        ///   is also the way that the <see cref="NativeWindow"/> class does it.
        /// </para>
        /// </remarks>
        private readonly User32.WNDPROC _windowProcDelegate;

        /// <summary>
        ///   The function pointer created from <see cref="_windowProcDelegate"/>.
        /// </summary>
        private readonly IntPtr _windowProcDelegatePtr;

        /// <summary>
        ///   Initializes a new instance of the <see cref="WindowSubclassHandler"/> class.
        /// </summary>
        /// <param name="handle">The window handle of the window to subclass.</param>
        public WindowSubclassHandler(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentNullException(nameof(handle));

            _handle = handle;

            // Create a delegate for our window procedure and get a function
            // pointer for it.
            _windowProcDelegate = NativeWndProc;
            _windowProcDelegatePtr = Marshal.GetFunctionPointerForDelegate(
                    _windowProcDelegate);
        }

        /// <summary>
        ///   Subclasses the window.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   You must call <see cref="Dispose()"/> to undo the subclassing before
        ///   the window is destroyed.
        /// </para>
        /// </remarks>
        /// <exception cref="Win32Exception">The window could not be subclassed.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Open"/> was already called.</exception>
        public void Open()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(WindowSubclassHandler));
            }
            if (_opened)
            {
                throw new InvalidOperationException();
            }

            // Replace the existing window procedure with our one ("instance subclassing").
            // Note: It shouldn't be possible to set a null pointer as window procedure, so we
            // can use the return value to determine if the call succeeded.
            _originalWindowProc = User32.SetWindowLong(
                _handle,
                User32.GWL.WNDPROC,
                _windowProcDelegatePtr);

            if (_originalWindowProc == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            Debug.Assert(_originalWindowProc != _windowProcDelegatePtr);

            _opened = true;
        }

        /// <summary>
        ///   Releases all resources used by the <see cref="WindowSubclassHandler"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   This method undoes the subclassing that was initiated by calling <see cref="Open"/>.
        ///   You must call this method before the window that was subclassed is destroyed.
        /// </para>
        /// <para>
        ///   If undoing the subclassing fails, this method will throw an exception. In that case,
        ///   you should call <see cref="KeepCallbackDelegateAlive"/> after the window is destroyed
        ///   to ensure the managed callback delegate is kept alive until the window procedure will
        ///   no longer be called.
        /// </para>
        /// </remarks>
        /// <exception cref="Win32Exception">The subclassing could not be undone.</exception>
        /// <exception cref="InvalidOperationException">The current window procedure is not the
        /// expected one.</exception>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Keeps the managed callback delegate alive from which the native function pointer
        ///   for the window procedure is created.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   When subclassing a window, a native function pointer is created from a managed
        ///   callback delegate which is then set as the window procedure. The callback is
        ///   automatically kept alive until <see cref="Dispose()"/> is called to undo the
        ///   subclassing.
        /// </para>
        /// <para>
        ///   However, if <see cref="Dispose()"/> fails (indicated by throwing an exception),
        ///   e.g. because the current window procedure pointer is not the expected one,
        ///   you should call this method after the window is actually destroyed, to ensure
        ///   the callback delegate is kept alive up to that time. Failing to do this might
        ///   result in undefined behavior.
        /// </para>
        /// </remarks>
        public void KeepCallbackDelegateAlive()
        {
            GC.KeepAlive(_windowProcDelegate);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="WindowSubclassHandler"/> and
        /// optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources;
        /// <see langword="false"/> to release only unmanaged resources.</param>
        /// <exception cref="Win32Exception">The subclassing could not be undone.</exception>
        /// <exception cref="InvalidOperationException">The current window procedure is not the
        /// expected one.</exception>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            // We cannot do anything from the finalizer thread since we have
            // resoures that must only be accessed from the GUI thread.
            if (disposing && _opened)
            {
                // Check if the current window procedure is the correct one.
                IntPtr currentWindowProcedure = User32.GetWindowLong(
                    _handle,
                    User32.GWL.WNDPROC);

                if (currentWindowProcedure == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }

                if (currentWindowProcedure != _windowProcDelegatePtr)
                {
                    // This can mean other code has also subclassed the window but failed
                    // to undo it.
                    // Note: Instead of failing, we could simply cut off the subclass chain and
                    // always restore the original window procedure here.
                    throw new InvalidOperationException(SR.WindowSubclassHandlerWndProcIsNotExceptedOne);
                }

                // Undo the subclassing by restoring the original window
                // procedure.
                if (User32.SetWindowLong(
                    _handle,
                    User32.GWL.WNDPROC,
                    _originalWindowProc) == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }

                // Ensure to keep the delegate alive up to the point after we
                // have undone the subclassing.
                KeepCallbackDelegateAlive();
            }

            _disposed = true;
        }

        /// <summary>
        ///   Processes Windows messages for the subclassed window.
        /// </summary>
        /// <param name="m">The message to process.</param>
        protected virtual void WndProc(ref Message m)
        {
            // Call the original window procedure to process the message.
            if (_originalWindowProc != IntPtr.Zero)
            {
                m.Result = User32.CallWindowProcW(
                    _originalWindowProc,
                    m.HWnd,
                    (User32.WM)m.Msg,
                    m.WParam,
                    m.LParam);
            }
        }

        /// <summary>
        ///   Determines if the specified <paramref name="exception"/> that was thrown
        ///   by <see cref="WndProc(ref Message)"/> shall be caught and passed to
        ///   <see cref="HandleWndProcException(Exception)"/>.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns><see langword="true"/> to catch the exception, or <see langword="false"/>
        /// to let it bubble up to the caller.</returns>
        protected virtual bool CanCatchWndProcException(Exception exception)
        {
            // By default, don't catch exceptions.
            return false;
        }

        /// <summary>
        ///   Called when an exception thrown by <see cref="WndProc(ref Message)"/> was caught.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> that was caught.</param>
        protected virtual void HandleWndProcException(Exception exception)
        {
            // Simply rethrow the exception here.
            ExceptionDispatchInfo.Throw(exception);
        }

        private IntPtr NativeWndProc(
            IntPtr hWnd,
            User32.WM msg,
            IntPtr wParam,
            IntPtr lParam)
        {
            Debug.Assert(hWnd == _handle);

            Message m = Message.Create(hWnd, msg, wParam, lParam);
            try
            {
                WndProc(ref m);
            }
            catch (Exception ex) when (CanCatchWndProcException(ex))
            {
                HandleWndProcException(ex);
            }

            return m.Result;
        }
    }
}
