// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Layout;
using Windows.Win32.Graphics.Dwm;

namespace System.Windows.Forms;

public partial class ComboBox
{
    internal bool UsesModernComboAdapter
        => EffectiveVisualStylesMode >= VisualStylesMode.Net11
            && FlatStyle != FlatStyle.System;

    private bool UsesComboAdapter
        => UsesModernComboAdapter
            || FlatStyle is FlatStyle.Flat or FlatStyle.Popup;

    private int ModernPreferredHeight
    {
        get
        {
            SystemVisualSettings settings = Application.SystemVisualSettings;
            Padding fieldPadding = ModernControlVisualStyles.GetFieldPadding(
                BorderStyle.Fixed3D,
                Padding,
                settings.FocusBorderMetrics,
                settings.TextScaleFactor,
                DeviceDpiInternal);

            return ModernControlVisualStyles.GetPreferredFieldHeight(
                FontHeight,
                fieldPadding,
                DeviceDpiInternal);
        }
    }

    private void ResetComboAdapter()
        => Properties.RemoveValue(s_propFlatComboAdapter);

    private void ApplyPreferredFieldHeight()
    {
        if (DropDownStyle == ComboBoxStyle.Simple)
        {
            return;
        }

        int preferredHeight = PreferredHeight;
        int previousHeight = Height;
        if (IsHandleCreated)
        {
            int currentSelectionHeight = (int)PInvokeCore.SendMessage(
                this,
                PInvoke.CB_GETITEMHEIGHT,
                (WPARAM)(-1));
            int heightDelta = preferredHeight - Height;
            if (currentSelectionHeight >= 0
                && heightDelta != 0)
            {
                PInvokeCore.SendMessage(
                    this,
                    PInvoke.CB_SETITEMHEIGHT,
                    (WPARAM)(-1),
                    (LPARAM)Math.Max(
                        1,
                        currentSelectionHeight + heightDelta));
            }
        }

        if (Height != preferredHeight)
        {
            Height = preferredHeight;
        }

        if (Height != previousHeight
            && ParentInternal is { } parent)
        {
            LayoutTransaction.DoLayout(
                parent,
                this,
                PropertyNames.Bounds);
        }
    }

    private void UpdateModernEditMargins()
    {
        if (_childEdit is null
            || _childEdit.HWND.IsNull
            || DropDownStyle != ComboBoxStyle.DropDown)
        {
            return;
        }

        int margin = UsesModernComboAdapter
            ? ScaleHelper.ScaleToDpi(
                ModernControlVisualStyles.Fixed3DBorderPadding
                    + ModernControlVisualStyles.InternalChromeInset,
                DeviceDpiInternal)
            : 0;
        PInvokeCore.SendMessage(
            _childEdit,
            PInvokeCore.EM_SETMARGINS,
            (WPARAM)(PInvoke.EC_LEFTMARGIN | PInvoke.EC_RIGHTMARGIN),
            LPARAM.MAKELPARAM(margin, margin));
    }

    private unsafe void ApplyModernDropDownCornerPreference(
        HWND dropDownHandle)
    {
        if (DropDownStyle == ComboBoxStyle.Simple
            || dropDownHandle.IsNull
            || !OsVersion.IsWindows11_OrGreater())
        {
            return;
        }

        DWM_WINDOW_CORNER_PREFERENCE preference = UsesModernComboAdapter
            ? DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUNDSMALL
            : DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DEFAULT;
        _ = PInvoke.DwmSetWindowAttribute(
            dropDownHandle,
            DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE,
            &preference,
            sizeof(DWM_WINDOW_CORNER_PREFERENCE));
    }

    private unsafe void RefreshModernDropDownCornerPreference()
    {
        if (!IsHandleCreated
            || DropDownStyle == ComboBoxStyle.Simple
            || !OsVersion.IsWindows11_OrGreater())
        {
            return;
        }

        COMBOBOXINFO comboBoxInfo = default;
        comboBoxInfo.cbSize = (uint)sizeof(COMBOBOXINFO);
        if (PInvoke.GetComboBoxInfo(
            HWND,
            ref comboBoxInfo))
        {
            ApplyModernDropDownCornerPreference(
                comboBoxInfo.hwndList);
        }

        ApplyModernDropDownCornerPreference(
            _dropDownHandle);
    }

    internal static bool SupportsModernDropDownCorners(
        Version windowsVersion)
        => windowsVersion >= new Version(10, 0, 22000);

    internal override Size GetPreferredSizeCore(
        Size proposedConstraints)
    {
        Size preferredSize = base.GetPreferredSizeCore(
            proposedConstraints);
        if (UsesModernComboAdapter
            && DropDownStyle != ComboBoxStyle.Simple)
        {
            preferredSize.Height = ModernPreferredHeight;
        }

        return preferredSize;
    }

    /// <inheritdoc/>
    protected override void OnVisualStylesModeChanged(EventArgs e)
    {
        ResetComboAdapter();
        ResetHeightCache();
        base.OnVisualStylesModeChanged(e);
        ApplyPreferredFieldHeight();
        UpdateModernEditMargins();
        RefreshModernDropDownCornerPreference();
    }

    /// <inheritdoc/>
    protected override void OnSystemVisualSettingsChanged(
        SystemVisualSettingsChangedEventArgs e)
    {
        base.OnSystemVisualSettingsChanged(e);

        if (!UsesModernComboAdapter)
        {
            return;
        }

        if ((e.Changed
            & (SystemVisualSettingsCategories.TextScale
                | SystemVisualSettingsCategories.FocusMetrics)) != 0)
        {
            ResetComboAdapter();
            ResetHeightCache();
            CommonProperties.xClearPreferredSizeCache(this);
            ApplyPreferredFieldHeight();
            UpdateModernEditMargins();
            LayoutTransaction.DoLayout(
                this,
                this,
                PropertyNames.SystemVisualSettings);
            if (ParentInternal is { } parent)
            {
                LayoutTransaction.DoLayout(
                    parent,
                    this,
                    PropertyNames.SystemVisualSettings);
            }
        }

        if ((e.Changed
            & (SystemVisualSettingsCategories.TextScale
                | SystemVisualSettingsCategories.FocusMetrics
                | SystemVisualSettingsCategories.AccentColor)) != 0)
        {
            Invalidate();
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    ///  <para>
    ///   Net11 and later share ComboBox field metrics. Crossing the classic or disabled
    ///   boundary changes preferred height for WinForms-painted styles; modern-to-modern
    ///   transitions repaint. <see cref="FlatStyle.System"/> remains native.
    ///  </para>
    /// </remarks>
    protected override VisualStylesModeChangeImpact GetVisualStylesModeChangeImpact(
        VisualStylesMode oldMode,
        VisualStylesMode newMode)
    {
        if (FlatStyle == FlatStyle.System)
        {
            return VisualStylesModeChangeImpact.None;
        }

        bool oldUsesModernMetrics = oldMode >= VisualStylesMode.Net11;
        bool newUsesModernMetrics = newMode >= VisualStylesMode.Net11;

        return oldUsesModernMetrics != newUsesModernMetrics
            ? VisualStylesModeChangeImpact.Metrics
            : VisualStylesModeChangeImpact.Repaint;
    }

    /// <inheritdoc/>
    protected override void RescaleConstantsForDpi(
        int deviceDpiOld,
        int deviceDpiNew)
    {
        ResetComboAdapter();
        ResetHeightCache();
        base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
        ApplyPreferredFieldHeight();
        UpdateModernEditMargins();
    }
}
