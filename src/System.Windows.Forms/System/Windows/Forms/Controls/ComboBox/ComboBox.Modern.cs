// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Layout;
using Windows.Win32.Graphics.Dwm;

namespace System.Windows.Forms;

public partial class ComboBox
{
    private Rectangle _modernEditAppliedBounds;
    private Rectangle _modernEditBaseBounds;
    private Size _modernEditBaseClientSize;
    private bool _adjustingModernChildBounds;
    private bool _modernFieldHeightApplied;
    private bool _modernHandleInitialized;
    private Rectangle _modernSimpleListBaseBounds;

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
            return ModernControlVisualStyles.GetPreferredFieldHeight(
                FontHeight,
                GetModernFieldPadding(),
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

        bool usesModernMetrics = UsesModernComboAdapter;
        if (!usesModernMetrics
            && !_modernFieldHeightApplied)
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

        _modernFieldHeightApplied = usesModernMetrics;
    }

    private unsafe void UpdateModernEditMargins()
    {
        if (!_modernHandleInitialized
            || !IsHandleCreated
            || DropDownStyle == ComboBoxStyle.DropDownList
            || (!UsesModernComboAdapter
                && _modernEditBaseBounds.IsEmpty))
        {
            return;
        }

        COMBOBOXINFO comboBoxInfo = default;
        comboBoxInfo.cbSize = (uint)sizeof(COMBOBOXINFO);
        if (!PInvoke.GetComboBoxInfo(
            HWND,
            ref comboBoxInfo)
            || comboBoxInfo.hwndItem.IsNull)
        {
            return;
        }

        int styleMargin = UsesModernComboAdapter
            ? ScaleHelper.ScaleToDpi(
                ModernControlVisualStyles.Fixed3DBorderPadding
                    + ModernControlVisualStyles.InternalChromeInset
                    + ModernControlVisualStyles.ComboBoxStyleInset,
                DeviceDpiInternal)
            : 0;
        int leftMargin = styleMargin
            + (UsesModernComboAdapter ? Padding.Left : 0);
        int rightMargin = styleMargin
            + (UsesModernComboAdapter ? Padding.Right : 0);
        PInvokeCore.SendMessage(
            comboBoxInfo.hwndItem,
            PInvokeCore.EM_SETMARGINS,
            (WPARAM)(PInvoke.EC_LEFTMARGIN | PInvoke.EC_RIGHTMARGIN),
            LPARAM.MAKELPARAM(leftMargin, rightMargin));

        UpdateModernEditBounds(comboBoxInfo);
    }

    private void UpdateModernEditBounds(
        COMBOBOXINFO comboBoxInfo)
    {
        int styleInset = UsesModernComboAdapter
            ? ScaleHelper.ScaleToDpi(
                ModernControlVisualStyles.ComboBoxStyleInset,
                DeviceDpiInternal)
            : 0;
        if (DropDownStyle == ComboBoxStyle.Simple)
        {
            UpdateModernSimpleEditBounds(
                comboBoxInfo,
                styleInset);
            return;
        }

        int topInset = styleInset
            + (UsesModernComboAdapter ? Padding.Top : 0);
        int bottomInset = styleInset
            + (UsesModernComboAdapter ? Padding.Bottom : 0);
        Rectangle itemBounds = _modernEditBaseBounds.IsEmpty
            ? comboBoxInfo.rcItem
            : _modernEditBaseBounds;
        if (!_modernEditBaseBounds.IsEmpty
            && _modernEditBaseClientSize != ClientSize)
        {
            itemBounds.X = comboBoxInfo.rcItem.left;
            itemBounds.Width = comboBoxInfo.rcItem.Width;
        }

        Rectangle editBounds = new(
            itemBounds.Left,
            itemBounds.Top + topInset,
            itemBounds.Width,
            Math.Max(
                1,
                itemBounds.Height - topInset - bottomInset));

        PInvoke.SetWindowPos(
            comboBoxInfo.hwndItem,
            HWND.Null,
            editBounds.Left,
            editBounds.Top,
            editBounds.Width,
            editBounds.Height,
            SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE
                | SET_WINDOW_POS_FLAGS.SWP_NOZORDER);
        _modernEditBaseBounds = itemBounds;
        _modernEditBaseClientSize = ClientSize;
        _modernEditAppliedBounds = editBounds;
    }

    private void UpdateModernSimpleEditBounds(
        COMBOBOXINFO comboBoxInfo,
        int styleInset)
    {
        if (_modernEditBaseBounds.IsEmpty)
        {
            _modernEditBaseBounds = GetChildBounds(
                comboBoxInfo.hwndItem);
            _modernSimpleListBaseBounds = GetChildBounds(
                comboBoxInfo.hwndList);
            _modernEditBaseClientSize = ClientSize;
        }

        int topInset = styleInset
            + (UsesModernComboAdapter ? Padding.Top : 0);
        int bottomInset = styleInset
            + (UsesModernComboAdapter ? Padding.Bottom : 0);
        Rectangle editBounds = _modernEditBaseBounds;
        editBounds.Y += topInset;
        Rectangle listBounds = _modernSimpleListBaseBounds;
        listBounds.Y = editBounds.Bottom + bottomInset;
        listBounds.Height = Math.Max(
            1,
            _modernSimpleListBaseBounds.Bottom - listBounds.Top);

        _adjustingModernChildBounds = true;
        try
        {
            PInvoke.SetWindowPos(
                comboBoxInfo.hwndItem,
                HWND.Null,
                editBounds.Left,
                editBounds.Top,
                0,
                0,
                SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE
                    | SET_WINDOW_POS_FLAGS.SWP_NOSIZE
                    | SET_WINDOW_POS_FLAGS.SWP_NOZORDER);
            PInvoke.SetWindowPos(
                comboBoxInfo.hwndList,
                HWND.Null,
                listBounds.Left,
                listBounds.Top,
                listBounds.Width,
                listBounds.Height,
                SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE
                    | SET_WINDOW_POS_FLAGS.SWP_NOZORDER);
        }
        finally
        {
            _adjustingModernChildBounds = false;
        }

        _modernEditAppliedBounds = editBounds;
    }

    private unsafe void RestoreModernEditBounds()
    {
        if (!_modernHandleInitialized
            || !IsHandleCreated
            || _modernEditBaseBounds.IsEmpty)
        {
            return;
        }

        COMBOBOXINFO comboBoxInfo = default;
        comboBoxInfo.cbSize = (uint)sizeof(COMBOBOXINFO);
        if (!PInvoke.GetComboBoxInfo(
            HWND,
            ref comboBoxInfo))
        {
            return;
        }

        _adjustingModernChildBounds = true;
        try
        {
            PInvoke.SetWindowPos(
                comboBoxInfo.hwndItem,
                HWND.Null,
                _modernEditBaseBounds.Left,
                _modernEditBaseBounds.Top,
                _modernEditBaseBounds.Width,
                _modernEditBaseBounds.Height,
                SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE
                    | SET_WINDOW_POS_FLAGS.SWP_NOZORDER);
            if (DropDownStyle == ComboBoxStyle.Simple
                && !_modernSimpleListBaseBounds.IsEmpty)
            {
                PInvoke.SetWindowPos(
                    comboBoxInfo.hwndList,
                    HWND.Null,
                    _modernSimpleListBaseBounds.Left,
                    _modernSimpleListBaseBounds.Top,
                    _modernSimpleListBaseBounds.Width,
                    _modernSimpleListBaseBounds.Height,
                    SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE
                        | SET_WINDOW_POS_FLAGS.SWP_NOZORDER);
            }
        }
        finally
        {
            _adjustingModernChildBounds = false;
        }
    }

    private void RestoreAndResetModernEditBounds()
    {
        RestoreModernEditBounds();
        ResetModernEditBounds();
    }

    private void ResizeModernSimpleBaseBounds()
    {
        if (_modernEditBaseBounds.IsEmpty
            || _modernEditBaseClientSize.IsEmpty
            || _modernEditBaseClientSize == ClientSize)
        {
            return;
        }

        int widthDelta = ClientSize.Width
            - _modernEditBaseClientSize.Width;
        int heightDelta = ClientSize.Height
            - _modernEditBaseClientSize.Height;
        _modernEditBaseBounds.Width = Math.Max(
            1,
            _modernEditBaseBounds.Width + widthDelta);
        _modernSimpleListBaseBounds.Width = Math.Max(
            1,
            _modernSimpleListBaseBounds.Width + widthDelta);
        _modernSimpleListBaseBounds.Height = Math.Max(
            1,
            _modernSimpleListBaseBounds.Height + heightDelta);
        _modernEditBaseClientSize = ClientSize;
    }

    private Rectangle GetChildBounds(HWND child)
    {
        PInvokeCore.GetWindowRect(child, out RECT bounds);
        Point topLeft = PointToClient(
            new Point(bounds.left, bounds.top));

        return new Rectangle(
            topLeft,
            new Size(bounds.Width, bounds.Height));
    }

    private void ResetModernEditBounds()
    {
        _modernEditAppliedBounds = Rectangle.Empty;
        _modernEditBaseBounds = Rectangle.Empty;
        _modernEditBaseClientSize = Size.Empty;
        _modernSimpleListBaseBounds = Rectangle.Empty;
    }

    private Padding GetModernFieldPadding()
    {
        SystemVisualSettings settings = Application.SystemVisualSettings;
        int styleInset = ScaleHelper.ScaleToDpi(
            ModernControlVisualStyles.ComboBoxStyleInset,
            DeviceDpiInternal);

        return ModernControlVisualStyles.GetFieldPadding(
            BorderStyle.Fixed3D,
            Padding + new Padding(styleInset),
            settings.FocusBorderMetrics,
            settings.TextScaleFactor,
            DeviceDpiInternal);
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
            RestoreAndResetModernEditBounds();
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
    protected override void OnPaddingChanged(EventArgs e)
    {
        ResetComboAdapter();
        ResetHeightCache();
        base.OnPaddingChanged(e);

        if (!UsesModernComboAdapter)
        {
            return;
        }

        CommonProperties.xClearPreferredSizeCache(this);
        ApplyPreferredFieldHeight();
        UpdateModernEditMargins();
        Invalidate();
    }

    /// <inheritdoc/>
    protected override void OnLayout(LayoutEventArgs levent)
    {
        base.OnLayout(levent);

        if (UsesModernComboAdapter
            && DropDownStyle == ComboBoxStyle.Simple)
        {
            UpdateModernEditMargins();
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
        RestoreAndResetModernEditBounds();
        ResetComboAdapter();
        ResetHeightCache();
        base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
        ApplyPreferredFieldHeight();
        UpdateModernEditMargins();
    }
}
