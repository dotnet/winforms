// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.ApplicationInstallationAndServicing;

namespace System.Windows.Forms;

/// <summary>
///  This class provides static methods to create, activate and deactivate the theming scope.
/// </summary>
internal unsafe ref struct ThemingScope
{
    private static ACTCTXW s_enableThemingActivationContext;
    private static HANDLE s_hActCtx;
    private static bool s_contextCreationSucceeded;

    private readonly nuint _cookie;

    /// <summary>
    ///  <see cref="ThemingScope"/> does nothing if a theming context is already active on the current thread.
    /// </summary>
    public ThemingScope(bool useVisualStyles)
    {
        HANDLE current;

        if (useVisualStyles
            && s_contextCreationSucceeded
            // No need to set the context if it is already active.
            && !(PInvoke.GetCurrentActCtx(&current) && current == s_hActCtx))
        {
            nuint cookie;
            bool success = PInvoke.ActivateActCtx(s_hActCtx, &cookie);
            _cookie = cookie;
            Debug.Assert(success);
        }
    }

    public readonly void Dispose()
    {
        if (_cookie != 0)
        {
            bool success = PInvoke.DeactivateActCtx(0, _cookie);
            Debug.Assert(success);
        }
    }

    public static unsafe bool CreateActivationContext(HINSTANCE module, int nativeResourceManifestID)
    {
        lock (typeof(ThemingScope))
        {
            if (s_contextCreationSucceeded)
            {
                return true;
            }

            s_enableThemingActivationContext = new ACTCTXW
            {
                cbSize = (uint)sizeof(ACTCTXW),
                lpResourceName = (char*)nativeResourceManifestID,
                dwFlags = PInvoke.ACTCTX_FLAG_HMODULE_VALID | PInvoke.ACTCTX_FLAG_RESOURCE_NAME_VALID,
                hModule = module
            };

            fixed (ACTCTXW* act = &s_enableThemingActivationContext)
            {
                s_hActCtx = PInvoke.CreateActCtx(act);
            }

            s_contextCreationSucceeded = s_hActCtx != -1;

            return s_contextCreationSucceeded;
        }
    }

    public static unsafe bool CreateActivationContext(Stream manifest)
    {
        lock (typeof(ThemingScope))
        {
            if (s_contextCreationSucceeded)
            {
                return true;
            }

            string tempFilePath = Path.Join(Path.GetTempPath(), Path.GetRandomFileName());
            using FileStream tempFileStream = new(
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

            s_contextCreationSucceeded = s_hActCtx != -1;

            try
            {
                File.Delete(tempFilePath);
            }
            catch (Exception e) when (e is UnauthorizedAccessException or IOException)
            {
                // Don't want to take down WinForms if we can't delete this file
            }

            return s_contextCreationSucceeded;
        }
    }
}
