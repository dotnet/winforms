// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms
{
    /// <summary>
    /// Partial DpiHelper class, with methods specific to thread DpiAwarenessContext
    /// </summary>
    internal static partial class DpiHelper
    {
        /// <summary>
        /// Enters a scope during which the current thread's DPI awareness context is set to
        /// <paramref name="awareness"/>
        /// </summary>
        /// <param name="awareness">The new DPI awareness for the current thread</param>
        /// <returns>An object that, when disposed, will reset the current thread's
        /// DPI awareness to the value it had when the object was created.</returns>
        public static IDisposable EnterDpiAwarenessScope(DpiAwarenessContext awareness)
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
            using (EnterDpiAwarenessScope(DpiAwarenessContext.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE))
            {
                return createInstance();
            }
        }

        #region Scoping DpiAwareness context helper class

        /// <summary>
        /// Class that help setting Dpi awareness context scope
        /// </summary>
        private class DpiAwarenessScope : IDisposable
        {
            private bool dpiAwarenessScopeIsSet = false;
            private DpiAwarenessContext originalAwareness = DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED;

            /// <summary>
            /// Enters given Dpi awareness scope
            /// </summary>
            public DpiAwarenessScope(DpiAwarenessContext awareness)
            {
                try
                {
                    if (!DpiUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(awareness, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED)) 
                    {
                        originalAwareness = DpiUnsafeNativeMethods.TryGetThreadDpiAwarenessContext();

                        // If desired awareness is not equal to current awareness
                        if (!DpiUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(originalAwareness, awareness))
                        {
                            // Unaware is allowed to open Unaware only; System Aware may open System and Unaware; etc...
                            if (CanContextAParentContextB(originalAwareness, awareness))
                            {
                                originalAwareness = DpiUnsafeNativeMethods.TrySetThreadDpiAwarenessContext(awareness);
                                dpiAwarenessScopeIsSet = true;
                            }
                        }
                    }
                }
                catch (EntryPointNotFoundException)
                {
                    dpiAwarenessScopeIsSet = false;
                }
            }

            /// <summary>
            /// Returns whether or not a window in context A should be allowed to open a window in context B
            /// Unaware windows may only open children in unaware mode; System Aware windows may open children in System Aware and Unaware;
            /// PMV1 windows may open children in PMV1, System Aware, and Unaware; finally PMV2 windows may open children in PMV2, PMV1, System Aware, and Unaware
            /// </summary>
            /// <param name="dpiAwarenessContextA"></param>
            /// <param name="dpiAwarenessContextB"></param>
            /// <returns></returns>
            /// <remarks>If a new member of the enumeration is added, this code will also have to change</remarks>
            /// <remarks>It would be better if there was a way to convert from the bit-masked dpi awareness to our own enumeration and compare the magnitude</remarks>
            private bool CanContextAParentContextB(DpiAwarenessContext dpiAwarenessContextA, DpiAwarenessContext dpiAwarenessContextB)
            {
                if (DpiUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(dpiAwarenessContextA, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2))
                {
                    return DpiUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(dpiAwarenessContextB, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2) ||
                        DpiUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(dpiAwarenessContextB, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE) ||
                        DpiUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(dpiAwarenessContextB, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE) ||
                        DpiUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(dpiAwarenessContextB, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNAWARE);
                }
                else if (DpiUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(dpiAwarenessContextA, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE))
                {
                    return DpiUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(dpiAwarenessContextB, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE) ||
                        DpiUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(dpiAwarenessContextB, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE) ||
                        DpiUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(dpiAwarenessContextB, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNAWARE);
                }
                else if (DpiUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(dpiAwarenessContextA, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE))
                {
                    return DpiUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(dpiAwarenessContextB, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE) ||
                        DpiUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(dpiAwarenessContextB, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNAWARE);
                }
                else if (DpiUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(dpiAwarenessContextA, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNAWARE))
                {
                    return DpiUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(dpiAwarenessContextB, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNAWARE);
                }
                return false;
            }

            /// <summary>
            /// Dispose object and resources
            /// </summary>
            public void Dispose()
            {
                ResetDpiAwarenessContextChanges();
            }
            
            /// <summary>
            /// resetting dpiawareness of the thread.
            /// </summary>
            private void ResetDpiAwarenessContextChanges()
            {
                if (dpiAwarenessScopeIsSet)
                {
                    DpiUnsafeNativeMethods.TrySetThreadDpiAwarenessContext(originalAwareness);
                    dpiAwarenessScopeIsSet = false;
                }
            }
        }
        #endregion
    }
}
