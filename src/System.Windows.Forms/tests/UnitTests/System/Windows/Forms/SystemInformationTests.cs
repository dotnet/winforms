// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class SystemInformationTests
{
    private const int LogicalDpi = 96;

    [Fact]
    public void SystemInformation_ActiveWindowTrackingDelay_Get_ReturnsExpected()
    {
        int delay = SystemInformation.ActiveWindowTrackingDelay;
        Assert.True(delay >= 0);
        Assert.Equal(delay, SystemInformation.ActiveWindowTrackingDelay);
    }

    [Fact]
    public void SystemInformation_ArrangeDirection_Get_ReturnsExpected()
    {
        ArrangeDirection direction = SystemInformation.ArrangeDirection;
        Assert.Equal((ArrangeDirection)0, direction & ~(ArrangeDirection.Up | ArrangeDirection.Down | ArrangeDirection.Left | ArrangeDirection.Right));
        Assert.Equal(direction, SystemInformation.ArrangeDirection);
    }

    [Fact]
    public void SystemInformation_ArrangeStartingPosition_Get_ReturnsExpected()
    {
        ArrangeStartingPosition position = SystemInformation.ArrangeStartingPosition;
        Assert.Equal((ArrangeStartingPosition)0, position & ~(ArrangeStartingPosition.BottomLeft | ArrangeStartingPosition.BottomRight | ArrangeStartingPosition.Hide | ArrangeStartingPosition.TopLeft | ArrangeStartingPosition.TopRight));
        Assert.Equal(position, SystemInformation.ArrangeStartingPosition);
    }

    [Fact]
    public void SystemInformation_BootMode_Get_ReturnsExpected()
    {
        BootMode bootMode = SystemInformation.BootMode;
        Assert.True(Enum.IsDefined(bootMode));
        Assert.Equal(bootMode, SystemInformation.BootMode);
    }

    [Fact]
    public void SystemInformation_Border3DSize_Get_ReturnsExpected()
    {
        Size size = SystemInformation.Border3DSize;
        Assert.True(size.Width >= 0);
        Assert.True(size.Height >= 0);
        Assert.Equal(size, SystemInformation.Border3DSize);
    }

    [Fact]
    public void SystemInformation_BorderMultiplierFactor_Get_ReturnsExpected()
    {
        int factor = SystemInformation.BorderMultiplierFactor;
        Assert.True(factor >= 0);
        Assert.Equal(factor, SystemInformation.BorderMultiplierFactor);
    }

    [Fact]
    public void SystemInformation_BorderSize_Get_ReturnsExpected()
    {
        Size size = SystemInformation.BorderSize;
        Assert.True(size.Width >= 0);
        Assert.True(size.Height >= 0);
        Assert.Equal(size, SystemInformation.BorderSize);
    }

    [Fact]
    public void SystemInformation_CaptionButtonSize_Get_ReturnsExpected()
    {
        Size size = SystemInformation.CaptionButtonSize;
        Assert.True(size.Width >= 0);
        Assert.True(size.Height >= 0);
        Assert.Equal(size, SystemInformation.CaptionButtonSize);
    }

    [Fact]
    public void SystemInformation_CaptionHeight_Get_ReturnsExpected()
    {
        int height = SystemInformation.CaptionHeight;
        Assert.True(height >= 0);
        Assert.Equal(height, SystemInformation.CaptionHeight);
    }

    [Fact]
    public void SystemInformation_CaretBlinkTime_Get_ReturnsExpected()
    {
        int blinkTime = SystemInformation.CaretBlinkTime;
        Assert.True(blinkTime >= 0);
        Assert.Equal(blinkTime, SystemInformation.CaretBlinkTime);
    }

    [Fact]
    public void SystemInformation_CaretWidth_Get_ReturnsExpected()
    {
        int height = SystemInformation.CaretWidth;
        Assert.True(height >= 0);
        Assert.Equal(height, SystemInformation.CaretWidth);
    }

    [Fact]
    public void SystemInformation_ComputerName_Get_ReturnsExpected()
    {
        string name = SystemInformation.ComputerName;
        Assert.InRange(name.Length, 1, 256);
        Assert.Equal(name, name.Trim('\0'));
        Assert.Equal(name, SystemInformation.ComputerName);
    }

    [Fact]
    public void SystemInformation_CursorSize_Get_ReturnsExpected()
    {
        Size size = SystemInformation.CursorSize;
        Assert.Contains(size, new Size[] { new(32, 32), new(48, 48), new(64, 64) });
        Assert.Equal(size, SystemInformation.CursorSize);
    }

    [Fact]
    public void SystemInformation_DbcsEnabled_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.DbcsEnabled, SystemInformation.DbcsEnabled);
    }

    [Fact]
    public void SystemInformation_DebugOS_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.DbcsEnabled, SystemInformation.DebugOS);
    }

    [Fact]
    public void SystemInformation_DoubleClickSize_Get_ReturnsExpected()
    {
        Size size = SystemInformation.DoubleClickSize;
        Assert.True(size.Width >= 0);
        Assert.True(size.Height >= 0);
        Assert.Equal(size, SystemInformation.DoubleClickSize);
    }

    [Fact]
    public void SystemInformation_DoubleClickTime_Get_ReturnsExpected()
    {
        int time = SystemInformation.DoubleClickTime;
        Assert.True(time > 0);
        Assert.Equal(time, SystemInformation.DoubleClickTime);
    }

    [Fact]
    public void SystemInformation_DragFullWindows_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.DragFullWindows, SystemInformation.DragFullWindows);
    }

    [Fact]
    public void SystemInformation_DragSize_Get_ReturnsExpected()
    {
        Size size = SystemInformation.DragSize;
        Assert.True(size.Width >= 0);
        Assert.True(size.Height >= 0);
        Assert.Equal(size, SystemInformation.DragSize);
    }

    [Fact]
    public void SystemInformation_FixedFrameBorderSize_Get_ReturnsExpected()
    {
        Size size = SystemInformation.FixedFrameBorderSize;
        Assert.True(size.Width >= 0);
        Assert.True(size.Height >= 0);
        Assert.Equal(size, SystemInformation.FixedFrameBorderSize);
    }

    [Fact]
    public void SystemInformation_FontSmoothingContrast_Get_ReturnsExpected()
    {
        int contrast = SystemInformation.FontSmoothingContrast;
        Assert.True(contrast >= 0);
        Assert.Equal(contrast, SystemInformation.FontSmoothingContrast);
    }

    [Fact]
    public void SystemInformation_FontSmoothingType_Get_ReturnsExpected()
    {
        int contrast = SystemInformation.FontSmoothingType;
        Assert.True(contrast > 0);
        Assert.Equal(contrast, SystemInformation.FontSmoothingType);
    }

    [Fact]
    public void SystemInformation_FrameBorderSize_Get_ReturnsExpected()
    {
        Size size = SystemInformation.FrameBorderSize;
        Assert.True(size.Width >= 0);
        Assert.True(size.Height >= 0);
        Assert.Equal(size, SystemInformation.FrameBorderSize);
    }

    [Fact]
    public void SystemInformation_HighContrast_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.HighContrast, SystemInformation.HighContrast);
    }

    [Fact]
    public void SystemInformation_HorizontalFocusThickness_Get_ReturnsExpected()
    {
        int thickness = SystemInformation.HorizontalFocusThickness;
        Assert.True(thickness >= 0);
        Assert.Equal(thickness, SystemInformation.HorizontalFocusThickness);
    }

    [Fact]
    public void SystemInformation_HorizontalResizeBorderThickness_Get_ReturnsExpected()
    {
        int thickness = SystemInformation.HorizontalResizeBorderThickness;
        Assert.True(thickness >= 0);
        Assert.Equal(thickness, SystemInformation.HorizontalResizeBorderThickness);
    }

    [Fact]
    public void SystemInformation_HorizontalScrollBarArrowWidth_Get_ReturnsExpected()
    {
        int width = SystemInformation.HorizontalScrollBarArrowWidth;
        Assert.True(width >= 0);
        Assert.Equal(width, SystemInformation.GetHorizontalScrollBarArrowWidthForDpi(LogicalDpi));
        Assert.Equal(width, SystemInformation.HorizontalScrollBarArrowWidth);
    }

    [Fact]
    public void SystemInformation_HorizontalScrollBarHeight_Get_ReturnsExpected()
    {
        int height = SystemInformation.HorizontalScrollBarHeight;
        Assert.True(height >= 0);
        Assert.Equal(height, SystemInformation.HorizontalScrollBarHeight);
    }

    [Fact]
    public void SystemInformation_HorizontalScrollBarThumbWidth_Get_ReturnsExpected()
    {
        int width = SystemInformation.HorizontalScrollBarThumbWidth;
        Assert.True(width >= 0);
        Assert.Equal(width, SystemInformation.HorizontalScrollBarThumbWidth);
    }

    [Fact]
    public void SystemInformation_IconHorizontalSpacing_Get_ReturnsExpected()
    {
        int spacing = SystemInformation.IconHorizontalSpacing;
        Assert.True(spacing >= 0);
        Assert.Equal(spacing, SystemInformation.IconHorizontalSpacing);
    }

    [Fact]
    public void SystemInformation_IconSize_Get_ReturnsExpected()
    {
        Size size = SystemInformation.IconSize;
        Assert.True(size.Width >= 32);
        Assert.True(size.Height >= 32);
        Assert.Equal(size, SystemInformation.IconSize);
    }

    [Fact]
    public void SystemInformation_IconSpacingSize_Get_ReturnsExpected()
    {
        Size size = SystemInformation.IconSpacingSize;
        Assert.True(size.Width >= 0);
        Assert.True(size.Height >= 0);
        Assert.Equal(size, SystemInformation.IconSpacingSize);
    }

    [Fact]
    public void SystemInformation_IconVerticalSpacing_Get_ReturnsExpected()
    {
        int spacing = SystemInformation.IconVerticalSpacing;
        Assert.True(spacing >= 0);
        Assert.Equal(spacing, SystemInformation.IconVerticalSpacing);
    }

    [Fact]
    public void SystemInformation_IsActiveWindowTrackingEnabled_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.IsActiveWindowTrackingEnabled, SystemInformation.IsActiveWindowTrackingEnabled);
    }

    [Fact]
    public void SystemInformation_IsComboBoxAnimationEnabled_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.IsComboBoxAnimationEnabled, SystemInformation.IsComboBoxAnimationEnabled);
    }

    [Fact]
    public void SystemInformation_IsDropShadowEnabled_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.IsDropShadowEnabled, SystemInformation.IsDropShadowEnabled);
    }

    [Fact]
    public void SystemInformation_IsFlatMenuEnabled_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.IsFlatMenuEnabled, SystemInformation.IsFlatMenuEnabled);
    }

    [Fact]
    public void SystemInformation_IsFontSmoothingEnabled_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.IsFontSmoothingEnabled, SystemInformation.IsFontSmoothingEnabled);
    }

    [Fact]
    public void SystemInformation_IsHotTrackingEnabled_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.IsHotTrackingEnabled, SystemInformation.IsHotTrackingEnabled);
    }

    [Fact]
    public void SystemInformation_IsIconTitleWrappingEnabled_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.IsIconTitleWrappingEnabled, SystemInformation.IsIconTitleWrappingEnabled);
    }

    [Fact]
    public void SystemInformation_IsKeyboardPreferred_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.IsKeyboardPreferred, SystemInformation.IsKeyboardPreferred);
    }

    [Fact]
    public void SystemInformation_IsListBoxSmoothScrollingEnabled_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.IsListBoxSmoothScrollingEnabled, SystemInformation.IsListBoxSmoothScrollingEnabled);
    }

    [Fact]
    public void SystemInformation_IsMenuAnimationEnabled_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.IsMenuAnimationEnabled, SystemInformation.IsMenuAnimationEnabled);
    }

    [Fact]
    public void SystemInformation_IsMenuFadeEnabled_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.IsMenuFadeEnabled, SystemInformation.IsMenuFadeEnabled);
    }

    [Fact]
    public void SystemInformation_IsMinimizeRestoreAnimationEnabled_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.IsMinimizeRestoreAnimationEnabled, SystemInformation.IsMinimizeRestoreAnimationEnabled);
    }

    [Fact]
    public void SystemInformation_IsSelectionFadeEnabled_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.IsSelectionFadeEnabled, SystemInformation.IsSelectionFadeEnabled);
    }

    [Fact]
    public void SystemInformation_IsSnapToDefaultEnabled_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.IsSnapToDefaultEnabled, SystemInformation.IsSnapToDefaultEnabled);
    }

    [Fact]
    public void SystemInformation_IsTitleBarGradientEnabled_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.IsTitleBarGradientEnabled, SystemInformation.IsTitleBarGradientEnabled);
    }

    [Fact]
    public void SystemInformation_IsToolTipAnimationEnabled_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.IsToolTipAnimationEnabled, SystemInformation.IsToolTipAnimationEnabled);
    }

    [Fact]
    public void SystemInformation_KanjiWindowHeight_Get_ReturnsExpected()
    {
        int height = SystemInformation.KanjiWindowHeight;
        Assert.True(height >= 0);
        Assert.Equal(height, SystemInformation.KanjiWindowHeight);
    }

    [Fact]
    public void SystemInformation_KeyboardDelay_Get_ReturnsExpected()
    {
        int delay = SystemInformation.KeyboardDelay;
        Assert.True(delay >= 0);
        Assert.Equal(delay, SystemInformation.KeyboardDelay);
    }

    [Fact]
    public void SystemInformation_KeyboardSpeed_Get_ReturnsExpected()
    {
        int speed = SystemInformation.KeyboardSpeed;
        Assert.True(speed >= 0);
        Assert.Equal(speed, SystemInformation.KeyboardSpeed);
    }

    [Fact]
    public void SystemInformation_MaxWindowTrackSize_Get_ReturnsExpected()
    {
        Size size = SystemInformation.MaxWindowTrackSize;
        Assert.True(size.Width >= 0);
        Assert.True(size.Height >= 0);
        Assert.Equal(size, SystemInformation.MaxWindowTrackSize);
    }

    [Fact]
    public void SystemInformation_MenuAccessKeysUnderlined_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.MenuAccessKeysUnderlined, SystemInformation.MenuAccessKeysUnderlined);
    }

    [Fact]
    public void SystemInformation_MenuBarButtonSize_Get_ReturnsExpected()
    {
        Size size = SystemInformation.MenuBarButtonSize;
        Assert.True(size.Width >= 0);
        Assert.True(size.Height >= 0);
        Assert.Equal(size, SystemInformation.MenuBarButtonSize);
    }

    [Fact]
    public void SystemInformation_MenuButtonSize_Get_ReturnsExpected()
    {
        Size size = SystemInformation.MenuButtonSize;
        Assert.True(size.Width >= 0);
        Assert.True(size.Height >= 0);
        Assert.Equal(size, SystemInformation.MenuButtonSize);
    }

    [Fact]
    public void SystemInformation_MenuCheckSize_Get_ReturnsExpected()
    {
        Size size = SystemInformation.MenuCheckSize;
        Assert.True(size.Width >= 0);
        Assert.True(size.Height >= 0);
        Assert.Equal(size, SystemInformation.MenuCheckSize);
    }

    [Fact]
    public void SystemInformation_MenuFont_Get_ReturnsExpected()
    {
        Font font = SystemInformation.MenuFont;
        Assert.NotNull(font);
        Assert.Equal(font, SystemInformation.MenuFont);
    }

    [Fact]
    public void SystemInformation_MenuHeight_Get_ReturnsExpected()
    {
        int height = SystemInformation.MenuHeight;
        Assert.True(height >= 0);
        Assert.Equal(height, SystemInformation.MenuHeight);
    }

    [Fact]
    public void SystemInformation_MenuShowDelay_Get_ReturnsExpected()
    {
        int delay = SystemInformation.MenuShowDelay;
        Assert.True(delay >= 0);
        Assert.Equal(delay, SystemInformation.MenuShowDelay);
    }

    [Fact]
    public void SystemInformation_MidEastEnabled_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.MidEastEnabled, SystemInformation.MidEastEnabled);
    }

    [Fact]
    public void SystemInformation_MinimizedWindowSize_Get_ReturnsExpected()
    {
        Size size = SystemInformation.MinimizedWindowSize;
        Assert.True(size.Width >= 0);
        Assert.True(size.Height >= 0);
        Assert.Equal(size, SystemInformation.MinimizedWindowSize);
    }

    [Fact]
    public void SystemInformation_MinimizedWindowSpacingSize_Get_ReturnsExpected()
    {
        Size size = SystemInformation.MinimizedWindowSize;
        Assert.True(size.Width >= 0);
        Assert.True(size.Height >= 0);
        Assert.Equal(size, SystemInformation.MinimizedWindowSpacingSize);
    }

    [Fact]
    public void SystemInformation_MinimumWindowSize_Get_ReturnsExpected()
    {
        Size size = SystemInformation.MinimumWindowSize;
        Assert.True(size.Width > 0);
        Assert.True(size.Height > 0);
        Assert.Equal(size, SystemInformation.MinimumWindowSize);
    }

    [Fact]
    public void SystemInformation_MinWindowTrackSize_Get_ReturnsExpected()
    {
        Size size = SystemInformation.MinWindowTrackSize;
        Assert.True(size.Width > 0);
        Assert.True(size.Height > 0);
        Assert.Equal(size, SystemInformation.MinWindowTrackSize);
    }

    [Fact]
    public void SystemInformation_MonitorCount_Get_ReturnsExpected()
    {
        int count = SystemInformation.MonitorCount;
        Assert.True(count > 0);
        Assert.Equal(count, Screen.AllScreens.Length);
        Assert.Equal(count, SystemInformation.MonitorCount);
    }

    [Fact]
    public void SystemInformation_MonitorsSameDisplayFormat_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.MonitorsSameDisplayFormat, SystemInformation.MonitorsSameDisplayFormat);
    }

    [Fact]
    public void SystemInformation_MouseButtons_Get_ReturnsExpected()
    {
        int count = SystemInformation.MouseButtons;
        Assert.True(count >= 0);
        Assert.Equal(count, SystemInformation.MouseButtons);
    }

    [Fact]
    public void SystemInformation_MouseButtonsSwapped_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.MouseButtonsSwapped, SystemInformation.MouseButtonsSwapped);
    }

    [Fact]
    public void SystemInformation_MouseHoverSize_Get_ReturnsExpected()
    {
        Size size = SystemInformation.MouseHoverSize;
        Assert.True(size.Width >= 0);
        Assert.True(size.Height >= 0);
        Assert.Equal(size, SystemInformation.MouseHoverSize);
    }

    [Fact]
    public void SystemInformation_MouseHoverTime_Get_ReturnsExpected()
    {
        int count = SystemInformation.MouseHoverTime;
        Assert.True(count > 0);
        Assert.Equal(count, SystemInformation.MouseHoverTime);
    }

    [Fact]
    public void SystemInformation_MousePresent_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.MousePresent, SystemInformation.MousePresent);
    }

    [Fact]
    public void SystemInformation_MouseSpeed_Get_ReturnsExpected()
    {
        int count = SystemInformation.MouseSpeed;
        Assert.True(count > 0);
        Assert.Equal(count, SystemInformation.MouseSpeed);
    }

    [Fact]
    public void SystemInformation_MouseWheelPresent_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.MouseWheelPresent, SystemInformation.MouseWheelPresent);
    }

    [Fact]
    public void SystemInformation_MouseWheelScrollDelta_Get_ReturnsExpected()
    {
        int delta = SystemInformation.MouseWheelScrollDelta;
        Assert.Equal(120, delta);
        Assert.Equal(delta, SystemInformation.MouseWheelScrollDelta);
    }

    [Fact]
    public void SystemInformation_MouseWheelScrollLines_Get_ReturnsExpected()
    {
        int lines = SystemInformation.MouseWheelScrollLines;
        Assert.True(lines > 0);
        Assert.Equal(lines, SystemInformation.MouseWheelScrollLines);
    }

    [Fact]
    public void SystemInformation_NativeMouseWheelSupport_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.NativeMouseWheelSupport, SystemInformation.NativeMouseWheelSupport);
    }

    [Fact]
    public void SystemInformation_Network_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.Network, SystemInformation.Network);
    }

    [Fact]
    public void SystemInformation_PenWindows_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.PenWindows, SystemInformation.PenWindows);
    }

    [Fact]
    public void SystemInformation_PopupMenuAlignment_Get_ReturnsExpected()
    {
        LeftRightAlignment alignment = SystemInformation.PopupMenuAlignment;
        Assert.True(Enum.IsDefined(alignment));
        Assert.Equal(alignment, SystemInformation.PopupMenuAlignment);
    }

    [Fact]
    public void SystemInformation_PowerStatus_Get_ReturnsExpected()
    {
        PowerStatus status = SystemInformation.PowerStatus;
        Assert.NotNull(status);
        Assert.Same(status, SystemInformation.PowerStatus);
    }

    [Fact]
    public void SystemInformation_PrimaryMonitorMaximizedWindowSize_Get_ReturnsExpected()
    {
        Size size = SystemInformation.PrimaryMonitorMaximizedWindowSize;
        Assert.True(size.Width > 0);
        Assert.True(size.Height > 0);
        Assert.Equal(size, SystemInformation.PrimaryMonitorMaximizedWindowSize);
    }

    [Fact]
    public void SystemInformation_PrimaryMonitorSize_Get_ReturnsExpected()
    {
        Size size = SystemInformation.PrimaryMonitorSize;
        Assert.True(size.Width > 0);
        Assert.True(size.Height > 0);
        Assert.Equal(size, SystemInformation.PrimaryMonitorSize);
    }

    [Fact]
    public void SystemInformation_RightAlignedMenus_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.RightAlignedMenus, SystemInformation.RightAlignedMenus);
    }

    [Fact]
    public void SystemInformation_ScreenOrientation_Get_ReturnsExpected()
    {
        ScreenOrientation orientation = SystemInformation.ScreenOrientation;
        Assert.True(Enum.IsDefined(orientation));
        Assert.Equal(orientation, SystemInformation.ScreenOrientation);
    }

    [Fact]
    public void SystemInformation_Secure_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.Secure, SystemInformation.Secure);
    }

    [Fact]
    public void SystemInformation_ShowSounds_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.ShowSounds, SystemInformation.ShowSounds);
    }

    [Fact]
    public void SystemInformation_SizingBorderWidth_Get_ReturnsExpected()
    {
        int width = SystemInformation.SizingBorderWidth;
        Assert.True(width > 0);
        Assert.Equal(width, SystemInformation.SizingBorderWidth);
    }

    [Fact]
    public void SystemInformation_SmallCaptionButtonSize_Get_ReturnsExpected()
    {
        Size size = SystemInformation.SmallCaptionButtonSize;
        Assert.True(size.Width > 0);
        Assert.True(size.Height > 0);
        Assert.Equal(size, SystemInformation.SmallCaptionButtonSize);
    }

    [Fact]
    public void SystemInformation_SmallIconSize_Get_ReturnsExpected()
    {
        Size size = SystemInformation.SmallIconSize;
        Assert.True(size.Width > 0);
        Assert.True(size.Height > 0);
        Assert.Equal(size, SystemInformation.SmallIconSize);
    }

    [Fact]
    public void SystemInformation_TerminalServerSession_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.TerminalServerSession, SystemInformation.TerminalServerSession);
    }

    [Fact]
    public void SystemInformation_ToolWindowCaptionButtonSize_Get_ReturnsExpected()
    {
        Size size = SystemInformation.ToolWindowCaptionButtonSize;
        Assert.True(size.Width > 0);
        Assert.True(size.Height > 0);
        Assert.Equal(size, SystemInformation.ToolWindowCaptionButtonSize);
    }

    [Fact]
    public void SystemInformation_ToolWindowCaptionHeight_Get_ReturnsExpected()
    {
        int height = SystemInformation.ToolWindowCaptionHeight;
        Assert.True(height > 0);
        Assert.Equal(height, SystemInformation.ToolWindowCaptionHeight);
    }

    [Fact]
    public void SystemInformation_UIEffectsEnabled_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.UIEffectsEnabled, SystemInformation.UIEffectsEnabled);
    }

    [Fact]
    public void SystemInformation_UserDomainName_Get_ReturnsExpected()
    {
        string domainName = SystemInformation.UserDomainName;
        Assert.Equal(Environment.UserDomainName, domainName);
        Assert.Equal(domainName, SystemInformation.UserDomainName);
    }

    [Fact]
    public void SystemInformation_UserInteractive_Get_ReturnsExpected()
    {
        Assert.Equal(SystemInformation.UserInteractive, SystemInformation.UserInteractive);
    }

    [Fact]
    public void SystemInformation_UserName_Get_ReturnsExpected()
    {
        string name = SystemInformation.UserName;
        Assert.InRange(name.Length, 1, 256);
        Assert.Equal(name, name.Trim('\0'));
        Assert.Equal(name, SystemInformation.UserName);
    }

    [Fact]
    public void SystemInformation_VerticalFocusThickness_Get_ReturnsExpected()
    {
        int thickness = SystemInformation.VerticalFocusThickness;
        Assert.True(thickness >= 0);
        Assert.Equal(thickness, SystemInformation.VerticalFocusThickness);
    }

    [Fact]
    public void SystemInformation_VerticalResizeBorderThickness_Get_ReturnsExpected()
    {
        int thickness = SystemInformation.VerticalResizeBorderThickness;
        Assert.True(thickness >= 0);
        Assert.Equal(thickness, SystemInformation.VerticalResizeBorderThickness);
    }

    [Fact]
    public void SystemInformation_VerticalScrollBarArrowHeight_Get_ReturnsExpected()
    {
        int height = SystemInformation.VerticalScrollBarArrowHeight;
        Assert.True(height >= 0);
        Assert.Equal(height, SystemInformation.VerticalScrollBarArrowHeightForDpi(LogicalDpi));
        Assert.Equal(height, SystemInformation.VerticalScrollBarArrowHeight);
    }

    [Fact]
    public void SystemInformation_VerticalScrollBarThumbHeight_Get_ReturnsExpected()
    {
        int height = SystemInformation.VerticalScrollBarThumbHeight;
        Assert.True(height >= 0);
        Assert.Equal(height, SystemInformation.VerticalScrollBarThumbHeight);
    }

    [Fact]
    public void SystemInformation_VerticalScrollBarWidth_Get_ReturnsExpected()
    {
        int width = SystemInformation.VerticalScrollBarWidth;
        Assert.True(width >= 0);
        Assert.Equal(width, SystemInformation.GetVerticalScrollBarWidthForDpi(LogicalDpi));
        Assert.Equal(width, SystemInformation.VerticalScrollBarWidth);
    }

    [Fact]
    public void SystemInformation_VirtualScreen_Get_ReturnsExpected()
    {
        Rectangle screen = SystemInformation.VirtualScreen;
        Assert.NotEqual(0, screen.Width);
        Assert.NotEqual(0, screen.Height);
        Assert.Equal(screen, SystemInformation.VirtualScreen);
    }

    [Fact]
    public void SystemInformation_WorkingArea_Get_ReturnsExpected()
    {
        Rectangle workingArea = SystemInformation.WorkingArea;
        Assert.True(workingArea.X >= 0);
        Assert.True(workingArea.Y >= 0);
        Assert.True(workingArea.Width > 0);
        Assert.True(workingArea.Height > 0);
        Assert.Equal(workingArea, SystemInformation.WorkingArea);
    }

    public static IEnumerable<object[]> Dpi_TestData()
    {
        yield return new object[] { -10 };
        yield return new object[] { 0 };
        yield return new object[] { LogicalDpi };
        yield return new object[] { 300 };
    }

    [Theory]
    [MemberData(nameof(Dpi_TestData))]
    public void SystemInformation_GetBorderSizeForDpi_Invoke_ReturnsExpected(int dpi)
    {
        Size size = SystemInformation.GetBorderSizeForDpi(dpi);
        Assert.True(size.Width >= 0);
        Assert.True(size.Height >= 0);
    }

    [Theory]
    [MemberData(nameof(Dpi_TestData))]
    public void SystemInformation_GetHorizontalScrollBarArrowWidthForDpi_Invoke_ReturnsExpected(int dpi)
    {
        int width = SystemInformation.GetHorizontalScrollBarArrowWidthForDpi(dpi);
        Assert.True(width >= 0);
    }

    [Theory]
    [MemberData(nameof(Dpi_TestData))]
    public void SystemInformation_GetHorizontalScrollBarHeightForDpi_Invoke_ReturnsExpected(int dpi)
    {
        int height = SystemInformation.GetHorizontalScrollBarHeightForDpi(dpi);
        Assert.True(height >= 0);
    }

    [Theory]
    [MemberData(nameof(Dpi_TestData))]
    public void SystemInformation_GetMenuFontForDpi_Invoke_ReturnsExpected(int dpi)
    {
        Font font = SystemInformation.GetMenuFontForDpi(dpi);
        Assert.NotNull(font);
    }

    [Theory]
    [MemberData(nameof(Dpi_TestData))]
    public void SystemInformation_GetVerticalScrollBarWidthForDpi_Invoke_ReturnsExpected(int dpi)
    {
        int width = SystemInformation.GetVerticalScrollBarWidthForDpi(dpi);
        Assert.True(width >= 0);
    }

    [Theory]
    [MemberData(nameof(Dpi_TestData))]
    public void SystemInformation_VerticalScrollBarArrowHeightForDpi_Invoke_ReturnsExpected(int dpi)
    {
        int height = SystemInformation.VerticalScrollBarArrowHeightForDpi(dpi);
        Assert.True(height >= 0);
    }
}
