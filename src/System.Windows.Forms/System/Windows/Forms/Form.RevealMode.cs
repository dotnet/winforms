// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.Graphics.Dwm;

namespace System.Windows.Forms;

public partial class Form
{
#if NET11_0_OR_GREATER
    private bool DeferredAppearanceCloaked
    {
        get => Properties.GetValueOrDefault(s_propFormAppearanceCloaked, false);
        set => Properties.AddOrRemoveValue(s_propFormAppearanceCloaked, value, defaultValue: false);
    }

    private void CloakForDeferredAppearanceIfNeeded()
    {
        if (!ShouldUseDeferredAppearanceCloak())
        {
            return;
        }

        if (SetDwmCloak(cloaked: true))
        {
            DeferredAppearanceCloaked = true;
        }
    }

    private void UncloakDeferredAppearanceIfNeeded()
    {
        if (!DeferredAppearanceCloaked)
        {
            return;
        }

        if (SetDwmCloak(cloaked: false))
        {
            DeferredAppearanceCloaked = false;
        }
    }

    private void ClearDeferredAppearanceCloakState() => DeferredAppearanceCloaked = false;

    private bool ShouldUseDeferredAppearanceCloak()
        => Application.FormAppearanceMode == FormAppearanceMode.Deferred
            && TopLevel
            && !IsMdiChild
            && Visible
            && IsHandleCreated;

    private unsafe bool SetDwmCloak(bool cloaked)
    {
        BOOL cloak = cloaked;
        HRESULT result = PInvoke.DwmSetWindowAttribute(
            HWND,
            DWMWINDOWATTRIBUTE.DWMWA_CLOAK,
            &cloak,
            (uint)sizeof(BOOL));

        return result.Succeeded;
    }
#endif
}
