﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This class provides static methods to create, activate and deactivate the theming scope.
    /// </summary>
    internal class ThemingScope
    {
        private static Kernel32.ACTCTXW s_enableThemingActivationContext;
        private static IntPtr s_hActCtx;
        private static bool s_contextCreationSucceeded;

        /// <summary>
        ///  We now use explicitactivate everywhere and use this method to determine if we
        ///  really need to activate the activationcontext.  This should be pretty fast.
        /// </summary>
        private static bool IsContextActive()
        {
            if (s_contextCreationSucceeded && Kernel32.GetCurrentActCtx(out IntPtr current).IsTrue())
            {
                return current == s_hActCtx;
            }

            return false;
        }

        /// <summary>
        ///  Activate() does nothing if a theming context is already active on the current thread, which is good
        ///  for perf reasons. However, in some cases, like in the Timer callback, we need to put another context
        ///  on the stack even if one is already present. In such cases, this method helps - you get to manage
        ///  the cookie yourself though.
        /// </summary>
        public static IntPtr Activate()
        {
            IntPtr userCookie = IntPtr.Zero;

            if (Application.UseVisualStyles && s_contextCreationSucceeded && OSFeature.Feature.IsPresent(OSFeature.Themes))
            {
                if (!IsContextActive())
                {
                    if (!Kernel32.ActivateActCtx(s_hActCtx, out userCookie))
                    {
                        // Be sure cookie always zero if activation failed
                        userCookie = IntPtr.Zero;
                    }
                }
            }

            return userCookie;
        }

        /// <summary>
        ///  Use this to deactivate a context activated by calling ExplicitActivate.
        /// </summary>
        public static IntPtr Deactivate(IntPtr userCookie)
        {
            if (userCookie != IntPtr.Zero && OSFeature.Feature.IsPresent(OSFeature.Themes))
            {
                if (Kernel32.DeactivateActCtx(0, userCookie).IsTrue())
                {
                    // deactivation succeeded...
                    userCookie = IntPtr.Zero;
                }
            }

            return userCookie;
        }

        public unsafe static bool CreateActivationContext(string dllPath, int nativeResourceManifestID)
        {
            lock (typeof(ThemingScope))
            {
                if (!s_contextCreationSucceeded && OSFeature.Feature.IsPresent(OSFeature.Themes))
                {
                    fixed (char* pDllPath = dllPath)
                    {
                        s_enableThemingActivationContext = new Kernel32.ACTCTXW
                        {
                            cbSize = (uint)Marshal.SizeOf<Kernel32.ACTCTXW>(),
                            lpSource = pDllPath,
                            lpResourceName = (IntPtr)nativeResourceManifestID,
                            dwFlags = Kernel32.ACTCTX_FLAG.RESOURCE_NAME_VALID
                        };

                        s_hActCtx = Kernel32.CreateActCtxW(ref s_enableThemingActivationContext);
                    }
                    s_contextCreationSucceeded = (s_hActCtx != new IntPtr(-1));
                }

                return s_contextCreationSucceeded;
            }
        }
    }
}
