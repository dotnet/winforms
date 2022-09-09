// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.ApplicationInstallationAndServicing;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This class provides static methods to create, activate and deactivate the theming scope.
    /// </summary>
    internal static class ThemingScope
    {
        private static ACTCTXW s_enableThemingActivationContext;
        private static nint s_hActCtx;
        private static bool s_contextCreationSucceeded;

        /// <summary>
        ///  We now use explicitactivate everywhere and use this method to determine if we
        ///  really need to activate the activationcontext.  This should be pretty fast.
        /// </summary>
        private unsafe static bool IsContextActive()
        {
            HANDLE current;
            return s_contextCreationSucceeded
                && PInvoke.GetCurrentActCtx(&current)
                && (nint)current == s_hActCtx;
        }

        /// <summary>
        ///  Activate() does nothing if a theming context is already active on the current thread, which is good
        ///  for perf reasons. However, in some cases, like in the Timer callback, we need to put another context
        ///  on the stack even if one is already present. In such cases, this method helps - you get to manage
        ///  the cookie yourself though.
        /// </summary>
        public unsafe static nint Activate(bool useVisualStyles)
        {
            nuint userCookie;
            if (IsContextActiveButNotCreated(useVisualStyles) && PInvoke.ActivateActCtx((HANDLE)s_hActCtx, &userCookie))
            {
                return (nint)userCookie;
            }

            return 0;
        }

        private static bool IsContextActiveButNotCreated(bool useVisualStyles)
            => useVisualStyles && s_contextCreationSucceeded && !IsContextActive();

        /// <summary>
        ///  Use this to deactivate a context activated by calling ExplicitActivate.
        /// </summary>
        public static IntPtr Deactivate(IntPtr userCookie)
        {
            if (userCookie == IntPtr.Zero || PInvoke.DeactivateActCtx(0, (nuint)userCookie))
            {
                return IntPtr.Zero;
            }

            return userCookie;
        }

        public unsafe static bool CreateActivationContext(IntPtr module, int nativeResourceManifestID)
        {
            lock (typeof(ThemingScope))
            {
                if (!s_contextCreationSucceeded)
                {
                    s_enableThemingActivationContext = new ACTCTXW
                    {
                        cbSize = (uint)sizeof(ACTCTXW),
                        lpResourceName = (char*)nativeResourceManifestID,
                        dwFlags = PInvoke.ACTCTX_FLAG_HMODULE_VALID | PInvoke.ACTCTX_FLAG_RESOURCE_NAME_VALID,
                        hModule = (HINSTANCE)module
                    };

                    fixed (ACTCTXW* act = &s_enableThemingActivationContext)
                    {
                        s_hActCtx = PInvoke.CreateActCtx(act);
                    }

                    s_contextCreationSucceeded = (s_hActCtx != new IntPtr(-1));
                }

                return s_contextCreationSucceeded;
            }
        }

        public unsafe static bool CreateActivationContext(Stream manifest)
        {
            lock (typeof(ThemingScope))
            {
                if (!s_contextCreationSucceeded)
                {
                    string tempFilePath = Path.Join(Path.GetTempPath(), Path.GetRandomFileName());
                    using FileStream tempFileStream = new FileStream(
                        tempFilePath,
                        FileMode.CreateNew,
                        FileAccess.ReadWrite,
                        FileShare.Delete | FileShare.ReadWrite);

                    manifest.CopyTo(tempFileStream);

                    // CreateActCtxW gives a sharing violation if we have the handle open
                    tempFileStream.Close();

                    fixed (char* p = tempFilePath)
                    {
                        s_enableThemingActivationContext = new ACTCTXW
                        {
                            cbSize = (uint)sizeof(ACTCTXW),
                            lpSource = p
                        };

                        fixed (ACTCTXW* act = &s_enableThemingActivationContext)
                        {
                            s_hActCtx = PInvoke.CreateActCtx(act);
                        }
                    }

                    s_contextCreationSucceeded = (s_hActCtx != new IntPtr(-1));

                    try
                    {
                        File.Delete(tempFilePath);
                    }
                    catch (Exception e) when (e is UnauthorizedAccessException or IOException)
                    {
                        // Don't want to take down WinForms if we can't delete is file
                    }
                }

                return s_contextCreationSucceeded;
            }
        }
    }
}
