// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms.Primitives.Resources;

namespace System.Windows.Forms
{
    internal static partial class DpiHelper
    {
        /// <summary>
        ///  Class that help setting <see cref="DPI_AWARENESS_CONTEXT"/> scope
        /// </summary>
        private class DpiAwarenessScope : IDisposable
        {
            private bool dpiAwarenessScopeIsSet;
            private DPI_HOSTING_BEHAVIOR _originalDpiHostingBehavior = DPI_HOSTING_BEHAVIOR.DPI_HOSTING_BEHAVIOR_INVALID;
            private readonly DPI_AWARENESS_CONTEXT _originalDpiAwarenessContext;

            /// <summary>
            ///  Sets <see cref="DPI_AWARENESS_CONTEXT"/> for the thread.
            /// </summary>
            /// <param name="context"><see cref="DPI_AWARENESS_CONTEXT"/> to be set on the thread.</param>
            /// <param name="behavior"><see cref="DPI_HOSTING_BEHAVIOR"/> to be set on the thread.</param>
            public DpiAwarenessScope(DPI_AWARENESS_CONTEXT context, DPI_HOSTING_BEHAVIOR behavior)
            {
                // Full support for DPI_AWARENESS_CONTEXT and mixed mode DPI_HOSTING_BEHAVIOR on the thread is only available after the RS4 OS release.
                if (!OsVersion.IsWindows10_18030rGreater())
                {
                    throw new PlatformNotSupportedException();
                }

                // Unsupported DPI_AWARENESS_CONTEXT result in no-op.
                if (PInvoke.AreDpiAwarenessContextsEqualInternal(context, DPI_AWARENESS_CONTEXT.UNSPECIFIED_DPI_AWARENESS_CONTEXT))
                {
                    // DpiContext specified for the scope is not supported.
                    throw new NotSupportedException();
                }

                _originalDpiAwarenessContext = PInvoke.GetThreadDpiAwarenessContext();
                if (_originalDpiAwarenessContext == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), SR.Win32GetThreadsDpiContextFailed);
                }

                // No-op when requested DpiContext and current threads context is same.
                if (PInvoke.AreDpiAwarenessContextsEqual(_originalDpiAwarenessContext, context))
                {
                    if (PInvoke.AreDpiAwarenessContextsEqual(_originalDpiAwarenessContext, DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_UNAWARE))
                    {
                        // It is not appropriate to establish the DPI scope for a process or thread that is unaware.
                        // Any windows that are created within this scope cannot be parented to windows created in unaware processes or threads.
                        throw new NotSupportedException();
                    }

                    if (PInvoke.SetThreadDpiAwarenessContext(context) == 0)
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error(), SR.Win32GetThreadsDpiContextFailed);
                    }

                    dpiAwarenessScopeIsSet = true;
                }

                if (behavior != DPI_HOSTING_BEHAVIOR.DPI_HOSTING_BEHAVIOR_MIXED)
                {
                    return;
                }

                _originalDpiHostingBehavior = PInvoke.SetThreadDpiHostingBehavior(behavior);
                if (_originalDpiHostingBehavior == DPI_HOSTING_BEHAVIOR.DPI_HOSTING_BEHAVIOR_INVALID)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), string.Format(SR.Win32SetThreadsDpiHostingBehaviorFailed, behavior));
                }
            }

            /// <summary>
            ///  Reset thread's <see cref="DPI_AWARENESS_CONTEXT"/> and <see cref="DPI_HOSTING_BEHAVIOR"/>.
            /// </summary>
            public void Dispose()
            {
#pragma warning disable CA1416 // OS version check should already be covered if the _originalDpiHostingBehavior or dpiAwarenessScopeIsSet is set with valid values.
                if (_originalDpiHostingBehavior != DPI_HOSTING_BEHAVIOR.DPI_HOSTING_BEHAVIOR_INVALID)
                {
                    if (PInvoke.SetThreadDpiHostingBehavior(_originalDpiHostingBehavior) == DPI_HOSTING_BEHAVIOR.DPI_HOSTING_BEHAVIOR_INVALID)
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error(), string.Format(SR.Win32SetThreadsDpiHostingBehaviorFailed, _originalDpiHostingBehavior));
                    }
                }

                if (!dpiAwarenessScopeIsSet)
                {
                    return;
                }

                if (PInvoke.SetThreadDpiAwarenessContext(_originalDpiAwarenessContext) == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error(), string.Format(SR.Win32SetThreadsDpiContextFailed, _originalDpiAwarenessContext));
                }

#pragma warning restore CA1416 // Validate platform compatibility
            }
        }
    }
}
