// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms.Primitives.Resources;

namespace System.Windows.Forms;

internal static partial class ScaleHelper
{
    /// <summary>
    ///  Class that creates a temporary <see cref="DPI_AWARENESS_CONTEXT"/> scope.
    /// </summary>
    private class DpiAwarenessScope : IDisposable
    {
        private readonly bool _dpiAwarenessScopeIsSet;
        private readonly bool _threadHostingBehaviorIsSet;
        private readonly DPI_HOSTING_BEHAVIOR _originalDpiHostingBehavior;
        private readonly DPI_AWARENESS_CONTEXT _originalDpiAwarenessContext;

        /// <summary>
        ///  Sets <see cref="DPI_AWARENESS_CONTEXT"/> for the thread.
        /// </summary>
        /// <param name="context"><see cref="DPI_AWARENESS_CONTEXT"/> to be set on the thread.</param>
        /// <param name="behavior"><see cref="DPI_HOSTING_BEHAVIOR"/> to be set on the thread.</param>
        public DpiAwarenessScope(DPI_AWARENESS_CONTEXT context, DPI_HOSTING_BEHAVIOR behavior)
        {
            // Full support for DPI_AWARENESS_CONTEXT and mixed mode DPI_HOSTING_BEHAVIOR on the thread
            // is only available after the RS4 OS release.
            if (!OsVersion.IsWindows10_18030rGreater())
            {
                Debug.Fail("Full support for DPI_AWARENESS_CONTEXT and mixed mode DPI_HOSTING_BEHAVIOR on the thread is only available after the RS4 OS release");
                return;
            }

            if (context.IsEquivalent(DPI_AWARENESS_CONTEXT.UNSPECIFIED_DPI_AWARENESS_CONTEXT))
            {
                throw new NotSupportedException();
            }

            _originalDpiAwarenessContext = PInvoke.GetThreadDpiAwarenessContext();

            // No-op when requested DPI_AWARENESS_CONTEXT and current thread's context is the same.
            if (!PInvoke.AreDpiAwarenessContextsEqual(_originalDpiAwarenessContext, context))
            {
                if (PInvoke.SetThreadDpiAwarenessContext(context) == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), SR.Win32GetThreadsDpiContextFailed);
                }

                _dpiAwarenessScopeIsSet = true;
            }

            _originalDpiHostingBehavior = PInvoke.GetThreadDpiHostingBehavior();
            if (behavior == _originalDpiHostingBehavior)
            {
                return;
            }

            _originalDpiHostingBehavior = PInvoke.SetThreadDpiHostingBehavior(behavior);
            if (_originalDpiHostingBehavior == DPI_HOSTING_BEHAVIOR.DPI_HOSTING_BEHAVIOR_INVALID)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), string.Format(SR.Win32SetThreadsDpiHostingBehaviorFailed, behavior));
            }

            _threadHostingBehaviorIsSet = true;
        }

        /// <summary>
        ///  Reset thread's <see cref="DPI_AWARENESS_CONTEXT"/> and <see cref="DPI_HOSTING_BEHAVIOR"/>.
        /// </summary>
        public void Dispose()
        {
#pragma warning disable CA1416 // OS version check should already be covered if the _originalDpiHostingBehavior or dpiAwarenessScopeIsSet is set with valid values.
            if (_threadHostingBehaviorIsSet
                && PInvoke.SetThreadDpiHostingBehavior(_originalDpiHostingBehavior) == DPI_HOSTING_BEHAVIOR.DPI_HOSTING_BEHAVIOR_INVALID)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), string.Format(SR.Win32SetThreadsDpiHostingBehaviorFailed, _originalDpiHostingBehavior));
            }

            if (_dpiAwarenessScopeIsSet
                && PInvoke.SetThreadDpiAwarenessContext(_originalDpiAwarenessContext) == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), string.Format(SR.Win32SetThreadsDpiContextFailed, _originalDpiAwarenessContext));
            }
#pragma warning restore CA1416
        }
    }
}
