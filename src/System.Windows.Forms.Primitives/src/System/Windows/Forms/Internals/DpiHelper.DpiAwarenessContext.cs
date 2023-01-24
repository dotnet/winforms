// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Partial DpiHelper class, with methods specific to thread DpiAwarenessContext
    /// </summary>
    internal static partial class DpiHelper
    {
        /// <summary>
        ///  Enters a scope during which the current thread's DPI awareness context is set to
        /// <paramref name="awareness"/>
        /// </summary>
        /// <param name="awareness">The new DPI awareness for the current thread</param>
        /// <returns>An object that, when disposed, will reset the current thread's
        ///  DPI awareness to the value it had when the object was created.</returns>
        public static IDisposable EnterDpiAwarenessScope(DPI_AWARENESS_CONTEXT awareness, DPI_HOSTING_BEHAVIOR dpiHosting = DPI_HOSTING_BEHAVIOR.DPI_HOSTING_BEHAVIOR_DEFAULT)
        {
            return new DpiAwarenessScope(awareness, dpiHosting);
        }

        /// <summary>
        ///  Create an object in system aware context. This method is mainly designed to create control objects in system aware context.
        ///  class is internal and is mainly used for modal dialogs.
        /// </summary>
        /// <typeparam name="T">return type of the object</typeparam>
        /// <param name="createInstance">lambda expression</param>
        /// <returns> returns object created in system aware mode</returns>
        public static T CreateInstanceInSystemAwareContext<T>(Func<T> createInstance)
        {
            using (EnterDpiAwarenessScope(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE))
            {
                return createInstance();
            }
        }

        /// <summary>
        ///  Class that help setting Dpi awareness context scope
        /// </summary>
        private class DpiAwarenessScope : IDisposable
        {
            private bool dpiAwarenessScopeIsSet;
            private DPI_HOSTING_BEHAVIOR currentThreadHostingBehavior = DPI_HOSTING_BEHAVIOR.DPI_HOSTING_BEHAVIOR_INVALID;
            private readonly DPI_AWARENESS_CONTEXT originalAwareness = DPI_AWARENESS_CONTEXT.UNSPECIFIED_DPI_AWARENESS_CONTEXT;

            /// <summary>
            ///  Sets DpiAwarenessContext for the thread.
            /// </summary>
            /// <param name="awarenessContext">DpiawarenessContext to be set on the thread.</param>
            /// <param name="dpiHostingBehavior">DPI hosting behavior to be set on the thread.</param>
            public DpiAwarenessScope(DPI_AWARENESS_CONTEXT awarenessContext, DPI_HOSTING_BEHAVIOR dpiHostingBehavior)
            {
                // Full support for DpiawarenessContext and mixed mode DPI hosting behavior on the thread is only available after the RS4 OS release.
                if (!OsVersion.IsWindows10_18030rGreater())
                {
                    return;
                }

                // Unsupported DpiawarenessContext result in no-op.
                if (PInvoke.AreDpiAwarenessContextsEqualInternal(awarenessContext, DPI_AWARENESS_CONTEXT.UNSPECIFIED_DPI_AWARENESS_CONTEXT))
                {
                    return;
                }

                originalAwareness = PInvoke.GetThreadDpiAwarenessContext();
                if (!PInvoke.AreDpiAwarenessContextsEqual(originalAwareness, awarenessContext) &&
                    !PInvoke.AreDpiAwarenessContextsEqual(originalAwareness, DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_UNAWARE))
                {
                    originalAwareness = PInvoke.SetThreadDpiAwarenessContext(awarenessContext);

                    // As reported in https://github.com/dotnet/winforms/issues/2969
                    // under unknown conditions originalAwareness may remain 0 (which means the call failed)
                    // causing us to set dpiAwarenessScopeIsSet=true, which would lead to a crash when we attempt to dispose the scope.
                    dpiAwarenessScopeIsSet = originalAwareness != IntPtr.Zero;
                }

                // Ignore thread's hosting behaviour if thread's DpiawarenessContext can not be set.
                if (!dpiAwarenessScopeIsSet || dpiHostingBehavior != DPI_HOSTING_BEHAVIOR.DPI_HOSTING_BEHAVIOR_MIXED)
                {
                    return;
                }

                currentThreadHostingBehavior = PInvoke.SetThreadDpiHostingBehavior(dpiHostingBehavior);
            }

            /// <summary>
            ///  Reset thread's DpiawarenessContext and DPI hosting behavior.
            /// </summary>
            public void Dispose()
            {
                if (!dpiAwarenessScopeIsSet)
                {
                    return;
                }

#pragma warning disable CA1416 // OS version check is already covered when `dpiAwarenessScopeIsSet` flag is set to `true`.
                PInvoke.SetThreadDpiAwarenessContext(originalAwareness);
                dpiAwarenessScopeIsSet = false;

                if (currentThreadHostingBehavior != DPI_HOSTING_BEHAVIOR.DPI_HOSTING_BEHAVIOR_INVALID)
                {
                    PInvoke.SetThreadDpiHostingBehavior(currentThreadHostingBehavior);
                    currentThreadHostingBehavior = DPI_HOSTING_BEHAVIOR.DPI_HOSTING_BEHAVIOR_INVALID;
                }
#pragma warning restore CA1416 // Validate platform compatibility
            }
        }
    }
}
