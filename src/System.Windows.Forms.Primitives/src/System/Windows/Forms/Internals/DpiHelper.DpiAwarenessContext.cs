// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.User32;

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
        public static IDisposable EnterDpiAwarenessScope(IntPtr awareness)
        {
            return new DpiAwarenessScope(awareness);
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
            using (EnterDpiAwarenessScope(DPI_AWARENESS_CONTEXT.SYSTEM_AWARE))
            {
                return createInstance();
            }
        }

        #region Scoping DpiAwareness context helper class

        /// <summary>
        ///  Class that help setting Dpi awareness context scope
        /// </summary>
        private class DpiAwarenessScope : IDisposable
        {
            private bool dpiAwarenessScopeIsSet = false;
            private readonly IntPtr originalAwareness = DPI_AWARENESS_CONTEXT.UNSPECIFIED;

            /// <summary>
            ///  Enters given Dpi awareness scope
            /// </summary>
            public DpiAwarenessScope(IntPtr awareness)
            {
                try
                {
                    if (!CommonUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(awareness, DPI_AWARENESS_CONTEXT.UNSPECIFIED))
                    {
                        originalAwareness = CommonUnsafeNativeMethods.GetThreadDpiAwarenessContext();

                        // If current process dpiawareness is SYSTEM_UNAWARE or SYSTEM_AWARE (must be equal to awareness), calling this method will be a no-op.
                        if (!CommonUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(originalAwareness, awareness) &&
                            !CommonUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(originalAwareness, DPI_AWARENESS_CONTEXT.UNAWARE))
                        {
                            originalAwareness = CommonUnsafeNativeMethods.SetThreadDpiAwarenessContext(awareness);
                            dpiAwarenessScopeIsSet = true;
                        }
                    }
                }
                catch (EntryPointNotFoundException)
                {
                    dpiAwarenessScopeIsSet = false;
                }
            }

            /// <summary>
            ///  Dispose object and resources
            /// </summary>
            public void Dispose()
            {
                ResetDpiAwarenessContextChanges();
            }

            /// <summary>
            ///  resetting dpiawareness of the thread.
            /// </summary>
            private void ResetDpiAwarenessContextChanges()
            {
                if (dpiAwarenessScopeIsSet)
                {
                    CommonUnsafeNativeMethods.TrySetThreadDpiAwarenessContext(originalAwareness);
                    dpiAwarenessScopeIsSet = false;
                }
            }
        }
        #endregion
    }
}
