// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Allows to subclass individual windows.
    /// </summary>
    /// <remarks>
    /// See
    /// https://docs.microsoft.com/en-us/windows/desktop/winmsg/about-window-procedures#instance-subclassing
    /// for more information.
    /// </remarks>
    internal class WindowSubclassHandler : IDisposable
    {
        private readonly IntPtr _handle;

        private bool _opened;

        private bool _disposed;

        private IntPtr _originalWindowProc;

        /// <summary>
        /// The delegate for the callback handler (that calls
        /// <see cref="WndProc(int, IntPtr, IntPtr)"/> from which the native function
        /// pointer <see cref="_windowProcDelegatePtr"/> is created. 
        /// </summary>
        /// <remarks>
        /// We must store this delegate (and prevent it from being garbage-collected)
        /// to ensure the function pointer doesn't become invalid.
        /// 
        /// Note: We create a new delegate (and native function pointer) for each
        /// instance because even though creation will be slower (and requires a
        /// bit of memory to store the native code) it will be faster when the window
        /// procedure is invoked, because otherwise we would need to use a dictionary
        /// to map the hWnd to the instance, as the window procedure doesn't allow
        /// to store reference data. However, this is also the way that the
        /// NativeWindow class of WinForms does it.
        /// </remarks>
        private readonly NativeMethods.WndProc _windowProcDelegate;

        /// <summary>
        /// The function pointer created from <see cref="_windowProcDelegate"/>.
        /// </summary>
        private readonly IntPtr _windowProcDelegatePtr;

        public WindowSubclassHandler(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                throw new ArgumentNullException(nameof(handle));

            _handle = handle;

            // Create a delegate for our window procedure, and get a function
            // pointer for it.
            _windowProcDelegate = (hWnd, msg, wParam, lParam) =>
            {
                Debug.Assert(hWnd == _handle);

                var m = Message.Create(hWnd, msg, wParam, lParam);
                try
                {
                    WndProc(ref m);
                }
                catch (Exception ex) when (CanCatchWndProcException(ex))
                {
                    HandleWndProcException(ex);
                }

                return m.Result;
            };

            _windowProcDelegatePtr = Marshal.GetFunctionPointerForDelegate(
                    _windowProcDelegate);
        }

        /// <summary>
        /// Subclasses the window.
        /// </summary>
        /// <remarks>
        /// You must call <see cref="Dispose()"/> to undo the subclassing before
        /// the window is destroyed.
        /// </remarks>
        /// <returns></returns>
        public void Open()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(WindowSubclassHandler));
            if (_opened)
                throw new InvalidOperationException();

            // Replace the existing window procedure with our one
            // ("instance subclassing").
            // We need to explicitely clear the last Win32 error and then retrieve
            // it, to check if the call succeeded, because if the function returns null
            // but it succeeded, it doesn't necessarily set the last Win32 error to 0.
            NativeMethods.SetLastError(0);
            _originalWindowProc = UnsafeNativeMethods.SetWindowLong(
                    new HandleRef(this, _handle),
                    NativeMethods.GWL_WNDPROC,
                    new HandleRef(null, _windowProcDelegatePtr));
            if (_originalWindowProc == IntPtr.Zero && Marshal.GetLastWin32Error() != 0)
                throw new Win32Exception();

            Debug.Assert(_originalWindowProc != _windowProcDelegatePtr);

            _opened = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void KeepCallbackDelegateAlive()
        {
            GC.KeepAlive(_windowProcDelegate);
        }

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
                NativeMethods.SetLastError(0);
                IntPtr currentWindowProcedure = UnsafeNativeMethods.GetWindowLong(
                        new HandleRef(this, _handle),
                        NativeMethods.GWL_WNDPROC);
                if (currentWindowProcedure == IntPtr.Zero && Marshal.GetLastWin32Error() != 0)
                    throw new Win32Exception();

                if (currentWindowProcedure != _windowProcDelegatePtr)
                    throw new InvalidOperationException(
                            "The current window procedure is not the expected one.");

                // Undo the subclassing by restoring the original window
                // procedure.
                NativeMethods.SetLastError(0);
                if (UnsafeNativeMethods.SetWindowLong(
                        new HandleRef(this, _handle),
                        NativeMethods.GWL_WNDPROC,
                        new HandleRef(null, _originalWindowProc)) == IntPtr.Zero &&
                        Marshal.GetLastWin32Error() != 0)
                    throw new Win32Exception();

                // Ensure to keep the delegate alive up to the point after we
                // have undone the subclassing.
                KeepCallbackDelegateAlive();
            }

            _disposed = true;
        }

        protected virtual void WndProc(ref Message m)
        {
            // Call the original window procedure to process the message.
            if (_originalWindowProc != IntPtr.Zero)
            {
                m.Result = UnsafeNativeMethods.CallWindowProc(
                        _originalWindowProc,
                        m.HWnd,
                        m.Msg,
                        m.WParam,
                        m.LParam);
            }
        }

        protected virtual bool CanCatchWndProcException(Exception ex)
        {
            // By default, don't catch exceptions.
            return false;
        }

        protected virtual void HandleWndProcException(Exception ex)
        {
            // Simply rethrow the exception here.
            ExceptionDispatchInfo.Capture(ex).Throw();
        }
    }
}
