// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Layout;
using Windows.Win32.Graphics.Dwm;

namespace System.Windows.Forms;

public partial class ComboBox
{
    private NativeComboBaseline _nativeComboBaseline;
    private bool _applyingModernComboLayout;
    private bool _nativeComboHandleInitialized;
    private bool _normalizingNativeComboBaseline;
    private int _modernComboLayoutWriteCount;
    private HWND _modernSimpleListClipRegionHandle;
    private Size _modernSimpleListClipRegionSize;
    private int _modernSimpleListClipRegionApplyCount;
    private const int ModernSimpleBottomCropLogicalPixels = 2;

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
            if (DrawMode != DrawMode.Normal)
            {
                return ItemHeight
                    + (2 * SystemInformation.FixedFrameBorderSize.Height);
            }

            return ModernControlVisualStyles.GetPreferredFieldHeight(
                fontHeight: FontHeight,
                fieldPadding: GetModernFieldPadding(),
                deviceDpi: DeviceDpiInternal);
        }
    }

    private void ResetComboAdapter()
        => Properties.RemoveValue(s_propFlatComboAdapter);

    private unsafe void CaptureNativeComboBaseline(
        COMBOBOXINFO comboBoxInfo)
    {
        if (_nativeComboBaseline.IsCaptured)
        {
            return;
        }

        int selectionFieldItemHeight = (int)PInvokeCore.SendMessage(
            this,
            PInvoke.CB_GETITEMHEIGHT,
            (WPARAM)(-1));

        if (selectionFieldItemHeight <= 0)
        {
            return;
        }

        PInvokeCore.GetWindowRect(this, out RECT windowBounds);

        Rectangle editBounds = DropDownStyle == ComboBoxStyle.DropDownList
            || comboBoxInfo.hwndItem.IsNull
                ? Rectangle.Empty
                : GetChildBounds(comboBoxInfo.hwndItem);

        Rectangle simpleListBounds = DropDownStyle == ComboBoxStyle.Simple
            && !comboBoxInfo.hwndList.IsNull
                ? GetChildBounds(comboBoxInfo.hwndList)
                : Rectangle.Empty;

        _nativeComboBaseline = new NativeComboBaseline
        {
            IsCaptured = true,
            DeviceDpi = DeviceDpiInternal,
            SelectionFieldItemHeight = selectionFieldItemHeight,
            SelectionFieldFrameHeight = Math.Max(
                0,
                windowBounds.Height - selectionFieldItemHeight),
            EditBounds = editBounds,
            SimpleListBounds = simpleListBounds,
            ClientSize = ClientSize
        };
    }

    private unsafe void EnsureNativeComboBaseline()
    {
        if (_nativeComboBaseline.IsCaptured
            || !_nativeComboHandleInitialized
            || !IsHandleCreated)
        {
            return;
        }

        COMBOBOXINFO comboBoxInfo = default;
        comboBoxInfo.cbSize = (uint)sizeof(COMBOBOXINFO);

        if (PInvoke.GetComboBoxInfo(
            HWND,
            ref comboBoxInfo))
        {
            CaptureNativeComboBaseline(comboBoxInfo);
        }
    }

    private void InitializeNativeComboBaseline()
    {
        if (_nativeComboBaseline.IsCaptured
            || _normalizingNativeComboBaseline
            || !_nativeComboHandleInitialized
            || !IsHandleCreated)
        {
            return;
        }

        _normalizingNativeComboBaseline = true;

        try
        {
            // Force the native ComboBox to settle rcItem, rcButton, and child bounds before
            // capturing them. A handle created with a requested height has not done this yet.
            UpdateStylesCore();
        }
        finally
        {
            _normalizingNativeComboBaseline = false;
        }

        EnsureNativeComboBaseline();
    }

    private ModernComboTargetState ComputeModernComboTargetState()
    {
        bool usesModernMetrics = UsesModernComboAdapter;
        int selectionFieldItemHeight = 0;

        if (DropDownStyle != ComboBoxStyle.Simple
            && DrawMode == DrawMode.Normal)
        {
            int desiredHeight = usesModernMetrics
                ? ModernPreferredHeight
                : GetClassicPreferredHeight();

            selectionFieldItemHeight = Math.Max(
                1,
                desiredHeight
                    - ScaleNativeBaselineValue(_nativeComboBaseline.SelectionFieldFrameHeight));
        }

        Padding chromeInsets = usesModernMetrics
            ? GetModernChromeInsets()
            : Padding.Empty;

        GetAdjustedNativeChildBounds(
            out Rectangle editBounds,
            out Rectangle simpleListBounds);

        Padding editMargins = Padding.Empty;

        if (usesModernMetrics && !editBounds.IsEmpty)
        {
            int topInset = chromeInsets.Top + Padding.Top;
            int bottomInset = chromeInsets.Bottom + Padding.Bottom;

            // Inset the edit window horizontally so its rectangular corners clear the rounded
            // field arcs, and reserve the (now wider) drop-down button on the button side.
            int leftInset = chromeInsets.Left + Padding.Left;
            int rightInset = chromeInsets.Right + Padding.Right;
            int extraButtonWidth = DropDownStyle == ComboBoxStyle.Simple
                ? 0
                : ScaleHelper.ScaleToDpi(
                    ModernControlVisualStyles.ComboBoxButtonExtraWidth,
                    DeviceDpiInternal);

            if (DropDownStyle == ComboBoxStyle.Simple)
            {
                int dividerThickness = GetModernSimpleDividerThickness();
                int simpleBottomShrink = ScaleHelper.ScaleToDpi(
                    ModernSimpleBottomCropLogicalPixels,
                    DeviceDpiInternal);
                int simpleListBottom = ClientRectangle.Bottom - simpleBottomShrink - dividerThickness;
                int simpleEditTop = topInset + ScaleHelper.ScaleToDpi(1, DeviceDpiInternal);
                int selectionFieldHeight = Math.Max(
                    1,
                    (int)PInvokeCore.SendMessage(
                        this,
                        PInvoke.CB_GETITEMHEIGHT,
                        (WPARAM)(-1)));
                int minimumReadableHeight = Math.Max(
                    1,
                    FontHeight + ScaleHelper.ScaleToDpi(2, DeviceDpiInternal));
                int preferredSimpleEditHeight = Math.Max(selectionFieldHeight, minimumReadableHeight)
                    + ScaleHelper.ScaleToDpi(2, DeviceDpiInternal);
                int maxSimpleEditHeight = Math.Max(
                    1,
                    simpleListBottom - dividerThickness - simpleEditTop);

                // Keep the editor in the top field lane with enough height for full glyph rendering,
                // draw an accent divider below it, and let the permanent list consume the remaining
                // space down to the rounded field border.
                editBounds.Y = simpleEditTop;
                editBounds.Height = Math.Min(preferredSimpleEditHeight, maxSimpleEditHeight);
                editBounds.X = leftInset;
                editBounds.Width = Math.Max(
                    1,
                    ClientRectangle.Width - leftInset - rightInset);

                if (!simpleListBounds.IsEmpty)
                {
                    simpleListBounds.X = editBounds.Left;
                    simpleListBounds.Width = editBounds.Width;
                    simpleListBounds.Y = editBounds.Bottom + dividerThickness;
                    simpleListBounds.Height = Math.Max(
                        1,
                        simpleListBottom - simpleListBounds.Y);
                }
            }
            else
            {
                // Keep enough vertical room for glyphs in editable DropDown mode: when chrome
                // insets become large at some DPIs/styles, shrinking both top and bottom can make
                // the native edit window shorter than the rendered text box line height.
                int minimumReadableHeight = Math.Max(1, FontHeight);
                int totalVerticalInset = topInset + bottomInset;
                int availableHeight = Math.Max(1, editBounds.Height - totalVerticalInset);

                if (availableHeight < minimumReadableHeight)
                {
                    int required = minimumReadableHeight - availableHeight;
                    int topReduction = Math.Min(topInset, (required + 1) / 2);
                    topInset -= topReduction;
                    required -= topReduction;
                    bottomInset -= Math.Min(bottomInset, required);
                }

                editBounds.Y += topInset;

                editBounds.Height = Math.Max(
                    1,
                    editBounds.Height - topInset - bottomInset);

                editBounds.Height = Math.Max(
                    minimumReadableHeight,
                    Math.Min(
                        editBounds.Height,
                        ClientRectangle.Bottom
                            - bottomInset
                            - editBounds.Top));

                if (RightToLeft == RightToLeft.Yes)
                {
                    // In RTL the drop-down button sits on the left, so its reservation goes there.
                    editBounds.X += rightInset + extraButtonWidth;
                }
                else
                {
                    editBounds.X += leftInset;
                }

                editBounds.Width = Math.Max(
                    1,
                    editBounds.Width - leftInset - rightInset - extraButtonWidth);

                if (!simpleListBounds.IsEmpty)
                {
                    int simpleListBottom = GetCurrentSimpleListBottom(
                        simpleListBounds.Bottom);
                    simpleListBounds.Y = editBounds.Bottom + bottomInset;
                    simpleListBounds.Height = Math.Max(
                        1,
                        simpleListBottom - simpleListBounds.Y);
                }
            }
        }

        return new ModernComboTargetState
        {
            SelectionFieldItemHeight = selectionFieldItemHeight,
            EditMargins = editMargins,
            EditBounds = editBounds,
            SimpleListBounds = simpleListBounds
        };
    }

    private void GetAdjustedNativeChildBounds(
        out Rectangle editBounds,
        out Rectangle simpleListBounds)
    {
        editBounds = ScaleNativeBaselineRectangle(
            _nativeComboBaseline.EditBounds);

        simpleListBounds = ScaleNativeBaselineRectangle(
            _nativeComboBaseline.SimpleListBounds);

        Size baselineClientSize = ScaleNativeBaselineSize(
            _nativeComboBaseline.ClientSize);

        int widthDelta = ClientSize.Width - baselineClientSize.Width;
        int heightDelta = ClientSize.Height - baselineClientSize.Height;

        if (!editBounds.IsEmpty)
        {
            editBounds.Width = Math.Max(
                1,
                editBounds.Width + widthDelta);

            if (DropDownStyle == ComboBoxStyle.Simple)
            {
                int selectionFieldItemHeight = (int)PInvokeCore.SendMessage(
                    this,
                    PInvoke.CB_GETITEMHEIGHT,
                    (WPARAM)(-1));

                editBounds.Height = Math.Max(
                    1,
                    editBounds.Height
                        + selectionFieldItemHeight
                        - ScaleNativeBaselineValue(
                            _nativeComboBaseline.SelectionFieldItemHeight));
            }
            else
            {
                editBounds.Height = Math.Max(
                    1,
                    editBounds.Height + heightDelta);
            }
        }

        if (!simpleListBounds.IsEmpty)
        {
            simpleListBounds.Width = Math.Max(
                1,
                simpleListBounds.Width + widthDelta);

            simpleListBounds.Height = Math.Max(
                1,
                simpleListBounds.Height + heightDelta);
        }
    }

    private void ConfigureModernSimpleListSurface(HWND listHandle)
    {
        if (DropDownStyle != ComboBoxStyle.Simple
            || !UsesModernComboAdapter
            || listHandle.IsNull)
        {
            return;
        }

        WINDOW_STYLE style = (WINDOW_STYLE)PInvokeCore.GetWindowLong(
            listHandle,
            WINDOW_LONG_PTR_INDEX.GWL_STYLE);
        WINDOW_EX_STYLE exStyle = (WINDOW_EX_STYLE)PInvokeCore.GetWindowLong(
            listHandle,
            WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);

        WINDOW_STYLE updatedStyle = style & ~WINDOW_STYLE.WS_BORDER;
        WINDOW_EX_STYLE updatedExStyle = exStyle & ~WINDOW_EX_STYLE.WS_EX_CLIENTEDGE;

        if (updatedStyle == style && updatedExStyle == exStyle)
        {
            return;
        }

        if (updatedStyle != style)
        {
            PInvokeCore.SetWindowLong(
                listHandle,
                WINDOW_LONG_PTR_INDEX.GWL_STYLE,
                (nint)updatedStyle);
        }

        if (updatedExStyle != exStyle)
        {
            PInvokeCore.SetWindowLong(
                listHandle,
                WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE,
                (nint)updatedExStyle);
        }

        PInvoke.SetWindowPos(
            listHandle,
            HWND.Null,
            0,
            0,
            0,
            0,
            SET_WINDOW_POS_FLAGS.SWP_NOMOVE
                | SET_WINDOW_POS_FLAGS.SWP_NOSIZE
                | SET_WINDOW_POS_FLAGS.SWP_NOZORDER
                | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE
                | SET_WINDOW_POS_FLAGS.SWP_FRAMECHANGED);
    }

    private unsafe int GetCurrentSimpleListBottom(
        int fallback)
    {
        COMBOBOXINFO comboBoxInfo = default;
        comboBoxInfo.cbSize = (uint)sizeof(COMBOBOXINFO);

        return !PInvoke.GetComboBoxInfo(
            hwndCombo: HWND,
            pcbi: ref comboBoxInfo)
            || comboBoxInfo.hwndList.IsNull
                ? fallback
                : GetChildBounds(comboBoxInfo.hwndList).Bottom;
    }

    private int ScaleNativeBaselineValue(int value)
    {
        if (_nativeComboBaseline.DeviceDpi == DeviceDpiInternal)
        {
            return value;
        }

        return (int)Math.Round(
            value * DeviceDpiInternal / (double)_nativeComboBaseline.DeviceDpi);
    }

    private Rectangle ScaleNativeBaselineRectangle(Rectangle bounds)
        => bounds.IsEmpty
            ? Rectangle.Empty
            : new(
                x: ScaleNativeBaselineValue(bounds.X),
                y: ScaleNativeBaselineValue(bounds.Y),
                width: ScaleNativeBaselineValue(bounds.Width),
                height: ScaleNativeBaselineValue(bounds.Height));

    private Size ScaleNativeBaselineSize(Size size)
        => new(
            width: ScaleNativeBaselineValue(size.Width),
            height: ScaleNativeBaselineValue(size.Height));

    /// <summary>
    ///  Applies the complete native ComboBox target for the current managed state.
    /// </summary>
    private unsafe void ApplyModernComboLayout()
    {
        if (!IsHandleCreated)
        {
            ApplyManagedPreferredHeightBeforeHandle();
            return;
        }

        if (!_nativeComboBaseline.IsCaptured
            && !UsesModernComboAdapter)
        {
            return;
        }

        InitializeNativeComboBaseline();

        if (_applyingModernComboLayout
            || _normalizingNativeComboBaseline
            || !_nativeComboBaseline.IsCaptured)
        {
            return;
        }

        _applyingModernComboLayout = true;

        try
        {
            ModernComboTargetState target = ComputeModernComboTargetState();
            ApplySelectionFieldItemHeight(target.SelectionFieldItemHeight);

            // Changing the selection-field height synchronously changes the native client geometry.
            target = ComputeModernComboTargetState();

            COMBOBOXINFO comboBoxInfo = default;
            comboBoxInfo.cbSize = (uint)sizeof(COMBOBOXINFO);

            if (!PInvoke.GetComboBoxInfo(
                HWND,
                ref comboBoxInfo))
            {
                return;
            }

            ConfigureModernSimpleListSurface(comboBoxInfo.hwndList);

            if (!target.EditBounds.IsEmpty)
            {
                ApplyEditMargins(
                    comboBoxInfo.hwndItem,
                    target.EditMargins);
            }

            ApplyChildBounds(
                comboBoxInfo.hwndItem,
                target.EditBounds);

            ApplyChildBounds(
                comboBoxInfo.hwndList,
                target.SimpleListBounds);
        }
        finally
        {
            _applyingModernComboLayout = false;
        }
    }

    private void ApplyManagedPreferredHeightBeforeHandle()
    {
        if (DropDownStyle == ComboBoxStyle.Simple)
        {
            return;
        }

        int targetHeight = UsesModernComboAdapter
            ? ModernPreferredHeight
            : GetClassicPreferredHeight();

        if (Height != targetHeight)
        {
            Height = targetHeight;
        }
    }

    private void ApplySelectionFieldItemHeight(int targetHeight)
    {
        if (targetHeight <= 0)
        {
            return;
        }

        int currentHeight = (int)PInvokeCore.SendMessage(
            this,
            PInvoke.CB_GETITEMHEIGHT,
            (WPARAM)(-1));

        if (currentHeight == targetHeight)
        {
            return;
        }

        PInvokeCore.SendMessage(
            this,
            PInvoke.CB_SETITEMHEIGHT,
            (WPARAM)(-1),
            (LPARAM)targetHeight);

        _modernComboLayoutWriteCount++;
    }

    private void ApplyEditMargins(
        HWND editHandle,
        Padding targetMargins)
    {
        if (editHandle.IsNull)
        {
            return;
        }

        nint currentMargins = PInvokeCore.SendMessage(
            editHandle,
            PInvokeCore.EM_GETMARGINS);

        int currentLeft = PARAM.LOWORD(currentMargins);
        int currentRight = PARAM.HIWORD(currentMargins);

        if (currentLeft == targetMargins.Left
            && currentRight == targetMargins.Right)
        {
            return;
        }

        PInvokeCore.SendMessage(
            editHandle,
            PInvokeCore.EM_SETMARGINS,
            (WPARAM)(PInvoke.EC_LEFTMARGIN | PInvoke.EC_RIGHTMARGIN),
            LPARAM.MAKELPARAM(
                targetMargins.Left,
                targetMargins.Right));
        _modernComboLayoutWriteCount++;
    }

    private void ApplyChildBounds(HWND childHandle, Rectangle targetBounds)
    {
        bool isModernSimpleList = UsesModernComboAdapter
            && DropDownStyle == ComboBoxStyle.Simple
            && childHandle == _childListBox?.HWND;

        if (childHandle.IsNull
            || targetBounds.IsEmpty)
        {
            return;
        }

        if (GetChildBounds(childHandle) != targetBounds)
        {
            PInvoke.SetWindowPos(
                childHandle,
                HWND.Null,
                targetBounds.Left,
                targetBounds.Top,
                targetBounds.Width,
                targetBounds.Height,
                SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE
                    | SET_WINDOW_POS_FLAGS.SWP_NOZORDER);

            _modernComboLayoutWriteCount++;
        }

        if (isModernSimpleList)
        {
            ConfigureModernSimpleListClipRegion(
                childHandle,
                targetBounds);
        }
    }

    private void ConfigureModernSimpleListClipRegion(
        HWND listHandle,
        Rectangle targetBounds)
    {
        if (listHandle.IsNull)
        {
            return;
        }

        Size targetRegionSize = new(
            targetBounds.Width,
            Math.Max(1, targetBounds.Height));

        if (listHandle == _modernSimpleListClipRegionHandle
            && targetRegionSize == _modernSimpleListClipRegionSize)
        {
            return;
        }

        using RegionScope region = new(
            0,
            0,
            targetRegionSize.Width,
            targetRegionSize.Height);

        if (PInvoke.SetWindowRgn(
            listHandle,
            region,
            fRedraw: true) != 0)
        {
            region.RelinquishOwnership();
            _modernSimpleListClipRegionHandle = listHandle;
            _modernSimpleListClipRegionSize = targetRegionSize;
            _modernSimpleListClipRegionApplyCount++;
        }
    }

    private int GetModernSimpleDividerThickness()
        => Math.Max(
            1,
            ScaleHelper.ScaleToDpi(
                ModernControlVisualStyles.BorderThickness,
                DeviceDpiInternal));

    private Rectangle GetChildBounds(HWND child)
    {
        PInvokeCore.GetWindowRect(child, out RECT bounds);

        Point topLeft = PointToClient(
            new Point(bounds.left, bounds.top));

        return new Rectangle(
            topLeft,
            new Size(bounds.Width, bounds.Height));
    }

    private Padding GetModernChromeInsets()
    {
        // Minimal modern field inset plus a small arc-clearance so the flat native edit child's
        // rectangular corners are not painted into the rounded-corner arcs. This is chrome padding
        // only; the caller still adds the user's Padding on top (see ComputeModernComboTargetState
        // and GetModernFieldPadding). It deliberately excludes the classic 3D-border metrics
        // (Fixed3DBorderPadding / InternalChromeInset) that the previous implementation inherited
        // from the classic field model; the modern field owns a single rounded border across the
        // full control, so those insets only produced a spurious inner margin.
        int inset = ScaleHelper.ScaleToDpi(
            ModernControlVisualStyles.BorderThickness
                + ModernControlVisualStyles.ComboBoxStyleInset
                + ModernControlVisualStyles.ComboBoxFieldArcClearance,
            DeviceDpiInternal);

        return new Padding(inset);
    }

    private Rectangle GetNativeComboBaselineEditBounds()
        => _nativeComboBaseline.EditBounds;

    private Rectangle GetNativeComboBaselineSimpleListBounds()
        => _nativeComboBaseline.SimpleListBounds;

    private int GetNativeComboBaselineSelectionFieldItemHeight()
        => _nativeComboBaseline.SelectionFieldItemHeight;

    private int GetModernComboLayoutWriteCount()
        => _modernComboLayoutWriteCount;

    private int GetModernSimpleListClipRegionApplyCount()
        => _modernSimpleListClipRegionApplyCount;

    private Padding GetModernFieldPadding()
    {
        SystemVisualSettings settings = Application.SystemVisualSettings;

        int styleInset = ScaleHelper.ScaleToDpi(
            ModernControlVisualStyles.ComboBoxStyleInset,
            DeviceDpiInternal);

        // Vertical clearance is kept on the classic-derived field model so the modern preferred
        // height stays aligned with TextBox (GetPreferredFieldHeight only consumes the vertical
        // component). Horizontal clearance uses the minimal modern field inset so the field text
        // and drop-down-list caption are not over-inset by classic 3D metrics.
        Padding verticalSource = ModernControlVisualStyles.GetFieldPadding(
            BorderStyle.Fixed3D,
            Padding + new Padding(styleInset),
            settings.FocusBorderMetrics,
            settings.TextScaleFactor,
            DeviceDpiInternal);

        Padding horizontalSource = GetModernChromeInsets();

        return new Padding(
            left: horizontalSource.Left + Padding.Left,
            top: verticalSource.Top,
            right: horizontalSource.Right + Padding.Right,
            bottom: verticalSource.Bottom);
    }

    /// <summary>
    ///  Hit-tests a left-button click against the modern drop-down button.
    /// </summary>
    /// <param name="clientPoint">The click location in client coordinates.</param>
    /// <returns>
    ///  <see langword="true"/> when the click lands on the drop-down button (or anywhere in a
    ///  drop-down-list combo, which acts as a single button); otherwise <see langword="false"/>.
    /// </returns>
    /// <remarks>
    ///  <para>
    ///   Modern VisualStyles mode expands the client area over the themed drop-down button via
    ///   the <c>WM_NCCALCSIZE</c> handling in <see cref="WndProc"/>, so the button is no longer
    ///   part of the native non-client area. As a result comctl32 stops toggling the list when
    ///   the button is clicked, and <see cref="WndProc"/> drives <see cref="DroppedDown"/> from
    ///   this hit-test instead.
    ///  </para>
    /// </remarks>
    private bool IsModernDropDownButtonClick(Point clientPoint)
    {
        if (!UsesModernComboAdapter || DropDownStyle == ComboBoxStyle.Simple)
        {
            return false;
        }

        // A drop-down-list combo behaves as a single button, so any click toggles the list.
        // An editable combo toggles only when the click lands on the drop-down button itself,
        // leaving text-area clicks to the hosted edit control.
        return DropDownStyle == ComboBoxStyle.DropDownList
            || FlatComboBoxAdapter._dropDownRect.Contains(clientPoint);
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

    internal static bool SupportsModernDropDownCorners(Version windowsVersion)
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

        bool recreateSystemHandle = FlatStyle == FlatStyle.System
            && IsHandleCreated;

        // Crossing the modern/classic boundary is reported as VisualStylesModeChangeImpact.Recreate
        // (see GetVisualStylesModeChangeImpact), so the base recreates the handle here. That is the
        // only clean way to unwind the modern native-window state (the WM_NCCALCSIZE client
        // expansion and the per-handle modern baseline); a fresh classic handle then behaves exactly
        // as before, and a fresh modern handle captures a clean baseline.
        // FlatStyle.System stays native, but an existing native handle still needs a rebuild when
        // visual-style mode changes to avoid stale geometry in designer/runtime transitions.
        base.OnVisualStylesModeChanged(e);

        if (recreateSystemHandle && IsHandleCreated)
        {
            RecreateHandle();
        }

        ApplyModernComboLayout();
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
            ApplyModernComboLayout();

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
        ApplyModernComboLayout();
        Invalidate();
    }

    /// <inheritdoc/>
    protected override void OnLayout(LayoutEventArgs levent)
    {
        base.OnLayout(levent);
        ApplyModernComboLayout();
    }

    /// <inheritdoc/>
    /// <remarks>
    ///  <para>
    ///   Net11 and later share ComboBox field metrics. Crossing the modern/classic boundary bakes or
    ///   removes native-window state (the WM_NCCALCSIZE client expansion and the per-handle modern
    ///   baseline), which cannot be unwound in place, so the handle is recreated. Modern-to-modern
    ///   transitions only repaint. <see cref="FlatStyle.System"/> remains native.
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
            ? VisualStylesModeChangeImpact.Recreate
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
        ApplyModernComboLayout();
    }
}
