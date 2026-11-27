// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using Moq;

namespace System.Windows.Forms.Tests;

#if NET11_0_OR_GREATER
public class KioskModeManagerTests
{
    [WinFormsFact]
    public void KioskModeManager_Ctor_Default()
    {
        using SubKioskModeManager manager = new();

        Assert.False(manager.AlwaysOn);
        Assert.Null(manager.Container);
        Assert.Null(manager.ContainerControl);
        Assert.False(manager.DesignMode);
        Assert.True(manager.EscapeExitsFullScreen);
        Assert.False(manager.FullScreen);
        Assert.Equal(0, manager.MousePointerAutoHideDelay);
        Assert.Null(manager.Site);
        Assert.Equal(Keys.F11, manager.ToggleFullScreenKeys);
        Assert.False(manager.TopMostInFullScreen);
    }

    [WinFormsFact]
    public void KioskModeManager_Ctor_IContainer()
    {
        using Container container = new();
        using KioskModeManager manager = new(container);

        Assert.Same(container, manager.Container);
        Assert.NotNull(manager.Site);
    }

    [WinFormsFact]
    public void KioskModeManager_Ctor_NullContainer_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("container", () => new KioskModeManager(null));
    }

    [WinFormsFact]
    public void KioskModeManager_ContainerControl_Set_GetReturnsExpected()
    {
        using Form form = new();
        using KioskModeManager manager = new()
        {
            ContainerControl = form
        };

        Assert.Same(form, manager.ContainerControl);

        manager.ContainerControl = form;
        Assert.Same(form, manager.ContainerControl);

        manager.ContainerControl = null;
        Assert.Null(manager.ContainerControl);
    }

    [WinFormsFact]
    public void KioskModeManager_ContainerControl_SetWithHandler_CallsContainerControlChanged()
    {
        using Form form = new();
        using KioskModeManager manager = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(manager, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };

        manager.ContainerControlChanged += handler;
        manager.ContainerControl = form;
        Assert.Equal(1, callCount);

        manager.ContainerControl = form;
        Assert.Equal(1, callCount);

        manager.ContainerControl = null;
        Assert.Equal(2, callCount);

        manager.ContainerControlChanged -= handler;
        manager.ContainerControl = form;
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3000)]
    public void KioskModeManager_MousePointerAutoHideDelay_Set_GetReturnsExpected(int value)
    {
        using KioskModeManager manager = new()
        {
            MousePointerAutoHideDelay = value
        };

        Assert.Equal(value, manager.MousePointerAutoHideDelay);

        manager.MousePointerAutoHideDelay = value;
        Assert.Equal(value, manager.MousePointerAutoHideDelay);
    }

    [WinFormsFact]
    public void KioskModeManager_MousePointerAutoHideDelay_SetNegative_ThrowsArgumentOutOfRangeException()
    {
        using KioskModeManager manager = new();

        Assert.Throws<ArgumentOutOfRangeException>("value", () => manager.MousePointerAutoHideDelay = -1);
    }

    [WinFormsFact]
    public void KioskModeManager_FullScreen_WithMousePointerAutoHideDelay_StartsTimer()
    {
        using Form form = new();
        using KioskModeManager manager = new()
        {
            ContainerControl = form,
            MousePointerAutoHideDelay = 3000,
            FullScreen = true
        };

        Timer timer = manager.TestAccessor.Dynamic._mousePointerAutoHideTimer;

        Assert.True(timer.Enabled);
        Assert.Equal(3000, timer.Interval);
    }

    [WinFormsFact]
    public void KioskModeManager_MousePointerAutoHideDelay_SetWhileFullScreen_UpdatesTimer()
    {
        using Form form = new();
        using KioskModeManager manager = new()
        {
            ContainerControl = form,
            MousePointerAutoHideDelay = 1000,
            FullScreen = true
        };

        manager.MousePointerAutoHideDelay = 2500;
        Timer timer = manager.TestAccessor.Dynamic._mousePointerAutoHideTimer;

        Assert.True(timer.Enabled);
        Assert.Equal(2500, timer.Interval);
    }

    [WinFormsFact]
    public void KioskModeManager_MousePointerAutoHideDelay_SetZero_ShowsPointerAndStopsTimer()
    {
        using Form form = new();
        using KioskModeManager manager = new()
        {
            ContainerControl = form,
            MousePointerAutoHideDelay = 1000,
            FullScreen = true
        };
        manager.TestAccessor.Dynamic.OnMousePointerAutoHideTimerTick(null, EventArgs.Empty);

        manager.MousePointerAutoHideDelay = 0;
        Timer timer = manager.TestAccessor.Dynamic._mousePointerAutoHideTimer;

        Assert.False(manager.TestAccessor.Dynamic._isCursorHidden);
        Assert.False(timer.Enabled);
    }

    [WinFormsTheory]
    [BoolData]
    public void KioskModeManager_MousePointerAutoHideTimerTick_WhenInactive_DoesNotHidePointer(
        bool fullScreen)
    {
        using Form form = new();
        using KioskModeManager manager = new()
        {
            ContainerControl = form,
            MousePointerAutoHideDelay = fullScreen ? 0 : 1000,
            FullScreen = fullScreen
        };

        manager.TestAccessor.Dynamic.OnMousePointerAutoHideTimerTick(null, EventArgs.Empty);

        Assert.False(manager.TestAccessor.Dynamic._isCursorHidden);
    }

    [WinFormsTheory]
    [InlineData(Keys.F11)]
    [InlineData(Keys.None)]
    [InlineData(Keys.Alt | Keys.Enter)]
    [InlineData(Keys.Control | Keys.Shift | Keys.F)]
    public void KioskModeManager_ToggleFullScreenKeys_Set_GetReturnsExpected(Keys value)
    {
        using KioskModeManager manager = new()
        {
            ToggleFullScreenKeys = value
        };

        Assert.Equal(value, manager.ToggleFullScreenKeys);

        manager.ToggleFullScreenKeys = value;
        Assert.Equal(value, manager.ToggleFullScreenKeys);
    }

    [WinFormsTheory]
    [BoolData]
    public void KioskModeManager_AlwaysOn_Set_GetReturnsExpected(bool value)
    {
        using KioskModeManager manager = new()
        {
            AlwaysOn = value
        };

        Assert.Equal(value, manager.AlwaysOn);

        manager.AlwaysOn = value;
        Assert.Equal(value, manager.AlwaysOn);

        manager.AlwaysOn = !value;
        Assert.Equal(!value, manager.AlwaysOn);
    }

    [WinFormsTheory]
    [InlineData(Keys.Alt | Keys.Enter)]
    [InlineData(Keys.Control | Keys.Shift | Keys.F)]
    public void KioskModeManager_ProcessKeyboardActivity_MatchingKeyCombination_TogglesFullScreen(Keys keys)
    {
        using Form form = new();
        using KioskModeManager manager = new()
        {
            ContainerControl = form,
            ToggleFullScreenKeys = keys
        };

        manager.TestAccessor.Dynamic.ProcessKeyboardActivity(keys, false);
        Assert.True(manager.FullScreen);

        manager.TestAccessor.Dynamic.ProcessKeyboardActivity(keys & Keys.KeyCode, false);
        Assert.True(manager.FullScreen);

        manager.TestAccessor.Dynamic.ProcessKeyboardActivity(keys, false);
        Assert.False(manager.FullScreen);
    }

    [WinFormsFact]
    public void KioskModeManager_ProcessKeyboardActivity_KeysNone_DoesNotToggleFullScreen()
    {
        using Form form = new();
        using KioskModeManager manager = new()
        {
            ContainerControl = form,
            ToggleFullScreenKeys = Keys.None
        };

        manager.TestAccessor.Dynamic.ProcessKeyboardActivity(Keys.None, false);

        Assert.False(manager.FullScreen);
    }

    [WinFormsFact]
    public void KioskModeManager_ToggleFullScreen_Form_RestoresExpected()
    {
        using Form form = new()
        {
            Bounds = new Rectangle(10, 20, 300, 200),
            FormBorderStyle = FormBorderStyle.FixedDialog,
            WindowState = FormWindowState.Normal,
            TopMost = false
        };

        using KioskModeManager manager = new()
        {
            ContainerControl = form,
            TopMostInFullScreen = true
        };
        Rectangle screenBounds = Screen.FromRectangle(form.Bounds).Bounds;

        manager.ToggleFullScreen();

        Assert.True(manager.FullScreen);
        Assert.Equal(FormBorderStyle.None, form.FormBorderStyle);
        Assert.True(form.TopMost);
        Assert.Equal(FormWindowState.Normal, form.WindowState);
        Assert.Equal(screenBounds, form.Bounds);

        manager.ToggleFullScreen();

        Assert.False(manager.FullScreen);
        Assert.Equal(FormBorderStyle.FixedDialog, form.FormBorderStyle);
        Assert.False(form.TopMost);
        Assert.Equal(FormWindowState.Normal, form.WindowState);
        Assert.Equal(new Rectangle(10, 20, 300, 200), form.Bounds);
    }

    [WinFormsTheory]
    [BoolData]
    public void KioskModeManager_FullScreen_VisibleMaximizedForm_RestoresWindowPlacement(bool topMost)
    {
        using Form form = new()
        {
            Bounds = new Rectangle(100, 100, 800, 600),
            StartPosition = FormStartPosition.Manual,
            TopMost = topMost
        };
        form.Show();
        Rectangle normalBounds = form.Bounds;
        form.WindowState = FormWindowState.Maximized;
        Rectangle screenBounds = Screen.FromRectangle(form.RestoreBounds).Bounds;
        using KioskModeManager manager = new()
        {
            ContainerControl = form,
            TopMostInFullScreen = !topMost
        };

        manager.FullScreen = true;

        Assert.Equal(FormWindowState.Normal, form.WindowState);
        Assert.Equal(FormBorderStyle.None, form.FormBorderStyle);
        Assert.Equal(!topMost, form.TopMost);
        Assert.Equal(!topMost,
            ((WINDOW_EX_STYLE)PInvokeCore.GetWindowLong(form, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE)
                & WINDOW_EX_STYLE.WS_EX_TOPMOST) != 0);
        Assert.Equal(screenBounds, form.Bounds);

        manager.FullScreen = false;

        Assert.Equal(FormWindowState.Maximized, form.WindowState);
        Assert.Equal(FormBorderStyle.Sizable, form.FormBorderStyle);
        Assert.Equal(topMost, form.TopMost);
        Assert.Equal(topMost,
            ((WINDOW_EX_STYLE)PInvokeCore.GetWindowLong(form, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE)
                & WINDOW_EX_STYLE.WS_EX_TOPMOST) != 0);
        Assert.Equal(normalBounds, form.RestoreBounds);

        form.WindowState = FormWindowState.Normal;
        Assert.Equal(normalBounds, form.Bounds);
    }

    [WinFormsTheory]
    [InlineData(BoundsSpecified.All, false)]
    [InlineData(BoundsSpecified.Location, false)]
    [InlineData(BoundsSpecified.Size, false)]
    [InlineData(BoundsSpecified.All, true)]
    [InlineData(BoundsSpecified.Location, true)]
    [InlineData(BoundsSpecified.Size, true)]
    public void KioskModeManager_FullScreen_WithDeferredRestoreBounds_PreservesRequestedBounds(
        BoundsSpecified specified,
        bool recreateHandle)
    {
        using WindowPositionForm form = new()
        {
            Bounds = new Rectangle(100, 100, 800, 600),
            StartPosition = FormStartPosition.Manual
        };
        form.Show();
        Rectangle expectedBounds = form.Bounds;
        Rectangle requestedBounds = new(150, 150, 900, 650);
        form.WindowState = FormWindowState.Maximized;
        form.SetBounds(requestedBounds.X, requestedBounds.Y, requestedBounds.Width, requestedBounds.Height, specified);
        if ((specified & BoundsSpecified.Location) != 0)
        {
            expectedBounds.Location = requestedBounds.Location;
        }

        if ((specified & BoundsSpecified.Size) != 0)
        {
            expectedBounds.Size = requestedBounds.Size;
        }

        Rectangle screenBounds = Screen.FromRectangle(expectedBounds).Bounds;
        using KioskModeManager manager = new() { ContainerControl = form };
        form.NativeSizes.Clear();

        manager.FullScreen = true;

        Assert.Equal(screenBounds, form.Bounds);
        Assert.Equal(new[] { screenBounds.Size }, form.NativeSizes);

        // A later position notification must not consume the saved restore request either.
        form.TopMost = !form.TopMost;
        Assert.Equal(screenBounds, form.Bounds);
        if (recreateHandle)
        {
            form.RecreateWindow();
            Assert.Equal(screenBounds, form.Bounds);
        }

        manager.FullScreen = false;

        Assert.Equal(FormWindowState.Maximized, form.WindowState);
        Assert.Equal(expectedBounds, form.RestoreBounds);
        form.WindowState = FormWindowState.Normal;
        Assert.Equal(expectedBounds, form.Bounds);
    }

    [WinFormsTheory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    public void KioskModeManager_FullScreen_PreservesClientSizeRestoreFlags(int widthIsClientSize, int heightIsClientSize)
    {
        using Form form = new() { StartPosition = FormStartPosition.Manual };
        form.Show();
        Size frameSize = form.Size - form.ClientSize;
        form.WindowState = FormWindowState.Maximized;
        Form.KioskRestoreBoundsState state = new(new Rectangle(100, 100, 600, 400), BoundsSpecified.All,
            widthIsClientSize, heightIsClientSize);
        form.RestoreKioskRestoreBounds(state);
        using KioskModeManager manager = new() { ContainerControl = form };

        manager.FullScreen = true;
        Assert.Equal(Screen.FromRectangle(form.Bounds).Bounds, form.Bounds);
        manager.FullScreen = false;

        Form.KioskRestoreBoundsState restoredState = form.SuspendKioskRestoreBounds();
        Assert.Equal(state, restoredState);
        form.RestoreKioskRestoreBounds(restoredState);
        form.WindowState = FormWindowState.Normal;
        Assert.Equal(new Size(600 + widthIsClientSize * frameSize.Width, 400 + heightIsClientSize * frameSize.Height), form.Size);
        Assert.Equal(new Point(100, 100), form.Location);
    }

    [WinFormsTheory]
    [BoolData]
    public void KioskModeManager_FullScreen_TransitionThrows_PreservesDeferredRestoreBounds(bool entering)
    {
        using Form form = new() { StartPosition = FormStartPosition.Manual };
        form.Show();
        form.WindowState = FormWindowState.Maximized;
        form.Bounds = new Rectangle(150, 150, 900, 650);
        Form.KioskRestoreBoundsState expected = form.SuspendKioskRestoreBounds();
        form.RestoreKioskRestoreBounds(expected);
        using KioskModeManager manager = new() { ContainerControl = form };
        if (!entering)
        {
            manager.FullScreen = true;
        }

        InvalidOperationException exception = new("Transition failed.");
        EventHandler handler = (_, _) => throw exception;
        form.StyleChanged += handler;
        try
        {
            Assert.Same(exception, Assert.Throws<InvalidOperationException>(() => manager.FullScreen = entering));
        }
        finally
        {
            form.StyleChanged -= handler;
        }

        Form.KioskRestoreBoundsState actual = form.SuspendKioskRestoreBounds();
        Assert.Equal(expected, actual);
        form.RestoreKioskRestoreBounds(actual);
    }

    [WinFormsTheory]
    [InlineData(FormBorderStyle.Sizable, FormWindowState.Normal)]
    [InlineData(FormBorderStyle.Sizable, FormWindowState.Maximized)]
    [InlineData(FormBorderStyle.FixedDialog, FormWindowState.Normal)]
    [InlineData(FormBorderStyle.Fixed3D, FormWindowState.Normal)]
    [InlineData(FormBorderStyle.FixedSingle, FormWindowState.Normal)]
    [InlineData(FormBorderStyle.FixedToolWindow, FormWindowState.Normal)]
    [InlineData(FormBorderStyle.SizableToolWindow, FormWindowState.Normal)]
    [InlineData(FormBorderStyle.None, FormWindowState.Normal)]
    public void KioskModeManager_FullScreen_VisibleForm_ResizesOnlyToFinalBounds(
        FormBorderStyle borderStyle,
        FormWindowState initialState)
    {
        using WindowPositionForm form = new()
        {
            Bounds = new Rectangle(100, 100, 800, 600),
            StartPosition = FormStartPosition.Manual,
            FormBorderStyle = borderStyle
        };
        using TableLayoutPanel table = new() { Dock = DockStyle.Fill };
        using TextBox textBox = new() { Dock = DockStyle.Fill };
        table.Controls.Add(textBox);
        form.Controls.Add(table);
        form.Show();
        Rectangle normalBounds = form.Bounds;
        int layoutCount = 0;
        table.Layout += (_, _) => layoutCount++;

        // Compare with one ordinary resize using the same control tree.
        form.Size += new Size(20, 20);
        int singleResizeLayouts = layoutCount;
        Assert.True(singleResizeLayouts > 0);
        form.Bounds = normalBounds;
        form.WindowState = initialState;
        Rectangle originalBounds = form.Bounds;
        WINDOWPLACEMENT originalPlacement = form.GetKioskWindowPlacement();
        WINDOW_STYLE originalStyle = form.WindowStyle;
        WINDOW_EX_STYLE originalExStyle = (WINDOW_EX_STYLE)PInvokeCore.GetWindowLong(form, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
        const WINDOW_STYLE borderMask = WINDOW_STYLE.WS_CAPTION | WINDOW_STYLE.WS_THICKFRAME;
        const WINDOW_EX_STYLE extendedBorderMask = WINDOW_EX_STYLE.WS_EX_CLIENTEDGE
            | WINDOW_EX_STYLE.WS_EX_DLGMODALFRAME | WINDOW_EX_STYLE.WS_EX_TOOLWINDOW;
        Rectangle screenBounds = Screen.FromRectangle(form.RestoreBounds).Bounds;
        using KioskModeManager manager = new() { ContainerControl = form };
        List<Rectangle> sizes = [];
        List<int> layoutsAtStyleChange = [];
        form.SizeChanged += (_, _) => sizes.Add(form.Bounds);
        form.StyleChanged += (_, _) => layoutsAtStyleChange.Add(layoutCount);
        form.NativeSizes.Clear();
        layoutCount = 0;

        manager.FullScreen = true;

        Assert.True(manager.FullScreen);
        Assert.Equal(new[] { screenBounds }, sizes);
        Assert.Equal(new[] { screenBounds.Size }, form.NativeSizes);
        Assert.Equal(new[] { 0 }, layoutsAtStyleChange);
        Assert.Equal(singleResizeLayouts, layoutCount);
        Assert.Equal(FormBorderStyle.None, form.FormBorderStyle);
        Assert.Equal(FormWindowState.Normal, form.WindowState);
        Assert.Equal((WINDOW_STYLE)0, form.WindowStyle & (borderMask | WINDOW_STYLE.WS_MAXIMIZE | WINDOW_STYLE.WS_MINIMIZE));
        Assert.Equal((WINDOW_EX_STYLE)0,
            (WINDOW_EX_STYLE)PInvokeCore.GetWindowLong(form, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE) & extendedBorderMask);
        Assert.Equal(SHOW_WINDOW_CMD.SW_NORMAL, form.GetKioskWindowPlacement().showCmd);

        sizes.Clear();
        layoutsAtStyleChange.Clear();
        form.NativeSizes.Clear();
        layoutCount = 0;
        manager.FullScreen = false;

        Assert.False(manager.FullScreen);
        Assert.Equal(new[] { originalBounds }, sizes);
        Assert.Equal(new[] { originalBounds.Size }, form.NativeSizes);
        Assert.Equal(new[] { 0 }, layoutsAtStyleChange);
        Assert.Equal(singleResizeLayouts, layoutCount);
        Assert.Equal(borderStyle, form.FormBorderStyle);
        Assert.Equal(initialState, form.WindowState);
        Assert.Equal(originalBounds, form.Bounds);
        Assert.Equal(normalBounds, form.RestoreBounds);
        Assert.Equal(originalStyle & (borderMask | WINDOW_STYLE.WS_MAXIMIZE | WINDOW_STYLE.WS_MINIMIZE),
            form.WindowStyle & (borderMask | WINDOW_STYLE.WS_MAXIMIZE | WINDOW_STYLE.WS_MINIMIZE));
        Assert.Equal(originalExStyle & extendedBorderMask,
            (WINDOW_EX_STYLE)PInvokeCore.GetWindowLong(form, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE) & extendedBorderMask);
        WINDOWPLACEMENT restoredPlacement = form.GetKioskWindowPlacement();
        Assert.Equal(originalPlacement.showCmd, restoredPlacement.showCmd);
        Assert.Equal(originalPlacement.rcNormalPosition, restoredPlacement.rcNormalPosition);

        form.WindowState = FormWindowState.Normal;
        Assert.Equal(normalBounds, form.Bounds);
    }

    [WinFormsFact]
    public void KioskModeManager_FullScreen_VisibleForm_RepeatedEntryOnlyNotifiesFinalBounds()
    {
        using WindowPositionForm form = new()
        {
            Bounds = new Rectangle(100, 100, 1400, 600),
            StartPosition = FormStartPosition.Manual
        };
        form.Show();
        using KioskModeManager manager = new() { ContainerControl = form };
        Rectangle normalBounds = form.Bounds;
        Size normalClientSize = form.ClientSize;
        Rectangle fullscreenBounds = Screen.FromRectangle(normalBounds).Bounds;
        List<(Rectangle Bounds, Size ClientSize)> sizes = [];
        form.SizeChanged += (_, _) => sizes.Add((form.Bounds, form.ClientSize));

        // The extra notification can occur after a maximized fullscreen round trip and a restore to Normal.
        form.WindowState = FormWindowState.Maximized;
        manager.FullScreen = true;
        manager.FullScreen = false;
        form.WindowState = FormWindowState.Normal;
        Application.DoEvents();

        for (int i = 0; i < 3; i++)
        {
            sizes.Clear();
            form.NativeSizes.Clear();
            manager.FullScreen = true;
            Application.DoEvents();
            Assert.Equal(new[] { fullscreenBounds.Size }, form.NativeSizes);
            Assert.Equal(new[] { (fullscreenBounds, fullscreenBounds.Size) }, sizes);

            sizes.Clear();
            form.NativeSizes.Clear();
            manager.FullScreen = false;
            Application.DoEvents();
            Assert.Equal(new[] { normalBounds.Size }, form.NativeSizes);
            Assert.Equal(new[] { (normalBounds, normalClientSize) }, sizes);
        }
    }

    [WinFormsFact]
    public void KioskModeManager_FullScreen_RefreshBounds_WhenUnchanged_DoesNotReapplyStyle()
    {
        using WindowPositionForm form = new();
        form.Show();
        using KioskModeManager manager = new() { ContainerControl = form, FullScreen = true };
        Rectangle bounds = form.Bounds;
        int styleChangedCount = 0;
        int sizeChangedCount = 0;
        int layoutCount = 0;
        form.StyleChanged += (_, _) => styleChangedCount++;
        form.SizeChanged += (_, _) => sizeChangedCount++;
        form.Layout += (_, _) => layoutCount++;
        form.NativeSizes.Clear();

        PInvokeCore.SendMessage(form, PInvokeCore.WM_DISPLAYCHANGE);

        Assert.Equal(bounds, form.Bounds);
        Assert.True(manager.FullScreen);
        Assert.Equal(0, styleChangedCount);
        Assert.Equal(0, sizeChangedCount);
        Assert.Equal(0, layoutCount);
        Assert.Empty(form.NativeSizes);
    }

    [WinFormsTheory]
    [BoolData]
    public void KioskModeManager_FullScreen_HandleRecreated_RestoresOriginalState(bool maximized)
    {
        using WindowPositionForm form = new()
        {
            Bounds = new Rectangle(100, 100, 800, 600),
            StartPosition = FormStartPosition.Manual
        };
        form.Show();
        Rectangle normalBounds = form.Bounds;
        if (maximized)
        {
            form.WindowState = FormWindowState.Maximized;
        }

        using KioskModeManager manager = new() { ContainerControl = form, FullScreen = true };
        form.RecreateWindow();

        manager.FullScreen = false;

        Assert.Equal(FormBorderStyle.Sizable, form.FormBorderStyle);
        Assert.Equal(maximized ? FormWindowState.Maximized : FormWindowState.Normal, form.WindowState);
        form.WindowState = FormWindowState.Normal;
        Assert.Equal(normalBounds, form.Bounds);
    }

    [WinFormsFact]
    public void KioskModeManager_FullScreen_CoversCompleteScreenBounds()
    {
        using Form form = new()
        {
            Bounds = new Rectangle(10, 20, 300, 200)
        };
        using KioskModeManager manager = new()
        {
            ContainerControl = form
        };
        Rectangle screenBounds = Screen.FromRectangle(form.Bounds).Bounds;

        manager.FullScreen = true;

        Assert.Equal(FormWindowState.Normal, form.WindowState);
        Assert.Equal(screenBounds, form.Bounds);
    }

    [WinFormsFact]
    public void KioskModeManager_FullScreen_MinimizedForm_UsesRestoreMonitor()
    {
        using Form form = new()
        {
            Bounds = new Rectangle(10, 20, 300, 200),
            StartPosition = FormStartPosition.Manual
        };
        form.Show();
        form.WindowState = FormWindowState.Minimized;
        Rectangle restoreBounds = form.RestoreBounds;
        Rectangle screenBounds = Screen.FromRectangle(
            restoreBounds).Bounds;
        using KioskModeManager manager = new()
        {
            ContainerControl = form
        };

        manager.FullScreen = true;

        Assert.Equal(screenBounds, form.Bounds);
    }

    [WinFormsFact]
    public void KioskModeManager_FullScreen_RefreshBounds_ReappliesScreenBounds()
    {
        using Form form = new()
        {
            Bounds = new Rectangle(10, 20, 300, 200)
        };
        Rectangle screenBounds = Screen.FromRectangle(
            form.Bounds).Bounds;
        using KioskModeManager manager = new()
        {
            ContainerControl = form,
            FullScreen = true
        };
        form.Bounds = new Rectangle(
            screenBounds.X + 10,
            screenBounds.Y + 10,
            300,
            200);

        manager.TestAccessor.Dynamic.RefreshFullScreenBounds();

        Assert.Equal(screenBounds, form.Bounds);
    }

    [WinFormsFact]
    public void KioskModeManager_FullScreen_RefreshBounds_WhenMinimized_ReappliesBoundsAfterRestore()
    {
        using Form form = new()
        {
            Bounds = new Rectangle(10, 20, 300, 200),
            StartPosition = FormStartPosition.Manual
        };
        form.Show();
        using KioskModeManager manager = new()
        {
            ContainerControl = form,
            FullScreen = true
        };
        Rectangle screenBounds = form.Bounds;
        // Model the old fullscreen rectangle without changing the desktop resolution.
        form.Size = screenBounds.Size - new Size(50, 50);
        form.WindowState = FormWindowState.Minimized;
        Form.KioskRestoreBoundsState before = form.SuspendKioskRestoreBounds();
        form.RestoreKioskRestoreBounds(before);

        PInvokeCore.SendMessage(form, PInvokeCore.WM_DISPLAYCHANGE);

        Form.KioskRestoreBoundsState after = form.SuspendKioskRestoreBounds();
        form.RestoreKioskRestoreBounds(after);
        Assert.Equal(before, after);
        Assert.Equal(FormWindowState.Minimized, form.WindowState);

        form.WindowState = FormWindowState.Normal;
        Application.DoEvents();

        Assert.True(manager.FullScreen);
        Assert.Equal(screenBounds, form.Bounds);
    }

    [WinFormsFact]
    public void KioskModeManager_FullScreen_RefreshBounds_WhenMaximized_ResizesOnlyToScreenBounds()
    {
        using WindowPositionForm form = new() { StartPosition = FormStartPosition.Manual };
        form.Show();
        using KioskModeManager manager = new() { ContainerControl = form, FullScreen = true };
        Rectangle screenBounds = form.Bounds;
        form.SetMaximizedBounds(new Rectangle(screenBounds.Location, screenBounds.Size - new Size(50, 50)));
        form.WindowState = FormWindowState.Maximized;
        Assert.Equal(FormWindowState.Maximized, form.WindowState);
        Assert.NotEqual(screenBounds.Size, form.Size);
        form.Bounds = new Rectangle(20, 30, 400, 300);
        Form.KioskRestoreBoundsState before = form.SuspendKioskRestoreBounds();
        form.RestoreKioskRestoreBounds(before);
        form.NativeSizes.Clear();

        PInvokeCore.SendMessage(form, PInvokeCore.WM_DISPLAYCHANGE);

        Form.KioskRestoreBoundsState after = form.SuspendKioskRestoreBounds();
        form.RestoreKioskRestoreBounds(after);
        Assert.Equal(before, after);
        Assert.True(manager.FullScreen);
        Assert.Equal(FormWindowState.Normal, form.WindowState);
        Assert.Equal(screenBounds, form.Bounds);
        Assert.Equal(new[] { screenBounds.Size }, form.NativeSizes);
    }

    [WinFormsFact]
    public void KioskModeManager_FullScreen_ExitNormalForm_PreservesActualBoundsOverPlacement()
    {
        using WindowPositionForm form = new() { StartPosition = FormStartPosition.Manual };
        form.Show();
        Rectangle bounds = form.Bounds;
        using KioskModeManager manager = new() { ContainerControl = form, FullScreen = true };

        // Model a snapped window, whose placement retains its pre-snap rectangle.
        WINDOWPLACEMENT placement = manager.TestAccessor.Dynamic._savedPlacement;
        placement.rcNormalPosition.right += 50;
        manager.TestAccessor.Dynamic._savedPlacement = placement;
        form.NativeSizes.Clear();

        manager.FullScreen = false;

        Assert.Equal(FormWindowState.Normal, form.WindowState);
        Assert.Equal(bounds, form.Bounds);
        Assert.Equal(new[] { bounds.Size }, form.NativeSizes);
    }

    [WinFormsFact]
    public void KioskModeManager_FullScreen_StatusStripDropDown_RemainsAttached()
    {
        using Form form = new()
        {
            Bounds = new Rectangle(10, 20, 300, 200)
        };
        using StatusStrip statusStrip = new();
        using ToolStripDropDownButton dropDownButton = new("Options");
        dropDownButton.DropDownItems.Add("First");
        statusStrip.Items.Add(dropDownButton);
        form.Controls.Add(statusStrip);
        using KioskModeManager manager = new()
        {
            ContainerControl = form
        };

        form.Show();
        manager.FullScreen = true;
        dropDownButton.ShowDropDown();

        Rectangle itemScreenBounds = statusStrip.RectangleToScreen(dropDownButton.Bounds);
        Assert.InRange(
            Math.Abs(dropDownButton.DropDown.Bounds.Bottom - itemScreenBounds.Top),
            0,
            1);
    }

    [WinFormsFact]
    public void KioskModeManager_ToggleFullScreen_UserControlContainer_UsesParentForm()
    {
        using Form form = new();
        using UserControl userControl = new();
        form.Controls.Add(userControl);
        using KioskModeManager manager = new()
        {
            ContainerControl = userControl
        };

        manager.ToggleFullScreen();

        Assert.True(manager.FullScreen);
        Assert.Equal(FormBorderStyle.None, form.FormBorderStyle);
    }

    [WinFormsFact]
    public void KioskModeManager_ToggleFullScreen_UserControlContainerParentedAfterSet_UsesParentForm()
    {
        using Form form = new();
        using UserControl userControl = new();
        using KioskModeManager manager = new()
        {
            ContainerControl = userControl
        };

        form.Controls.Add(userControl);
        manager.ToggleFullScreen();

        Assert.True(manager.FullScreen);
        Assert.Equal(FormBorderStyle.None, form.FormBorderStyle);
    }

    [WinFormsFact]
    public void KioskModeManager_FullScreen_SetBeforeUserControlIsParented_EntersWhenParented()
    {
        using Form form = new();
        using UserControl userControl = new();
        using KioskModeManager manager = new()
        {
            ContainerControl = userControl,
            FullScreen = true
        };

        form.Controls.Add(userControl);

        Assert.True(manager.FullScreen);
        Assert.Equal(FormBorderStyle.None, form.FormBorderStyle);
    }

    [WinFormsFact]
    public void KioskModeManager_FullScreen_UserControlReparented_TransfersToNewForm()
    {
        using Form firstForm = new()
        {
            FormBorderStyle = FormBorderStyle.FixedDialog
        };
        using Form secondForm = new();
        using UserControl userControl = new();
        firstForm.Controls.Add(userControl);
        using KioskModeManager manager = new()
        {
            ContainerControl = userControl,
            FullScreen = true
        };

        secondForm.Controls.Add(userControl);

        Assert.True(manager.FullScreen);
        Assert.Equal(FormBorderStyle.FixedDialog, firstForm.FormBorderStyle);
        Assert.Equal(FormBorderStyle.None, secondForm.FormBorderStyle);
    }

    [WinFormsFact]
    public void KioskModeManager_FullScreen_AncestorReparented_TransfersToNewForm()
    {
        using Form firstForm = new()
        {
            FormBorderStyle = FormBorderStyle.FixedDialog
        };
        using Form secondForm = new();
        using Panel panel = new();
        using UserControl userControl = new();
        panel.Controls.Add(userControl);
        firstForm.Controls.Add(panel);
        using KioskModeManager manager = new()
        {
            ContainerControl = userControl,
            FullScreen = true
        };

        secondForm.Controls.Add(panel);

        Assert.True(manager.FullScreen);
        Assert.Equal(FormBorderStyle.FixedDialog, firstForm.FormBorderStyle);
        Assert.Equal(FormBorderStyle.None, secondForm.FormBorderStyle);
    }

    [WinFormsFact]
    public void KioskModeManager_FullScreen_SetBeforeAncestorIsParented_EntersWhenParented()
    {
        using Form form = new();
        using Panel panel = new();
        using UserControl userControl = new();
        panel.Controls.Add(userControl);
        using KioskModeManager manager = new()
        {
            ContainerControl = userControl,
            FullScreen = true
        };

        form.Controls.Add(panel);

        Assert.True(manager.FullScreen);
        Assert.Equal(FormBorderStyle.None, form.FormBorderStyle);
    }

    [WinFormsFact]
    public void KioskModeManager_ContainerControl_SetWhileFullScreen_RestoresPreviousForm()
    {
        using Form form = new()
        {
            Bounds = new Rectangle(10, 20, 300, 200),
            FormBorderStyle = FormBorderStyle.FixedDialog,
            WindowState = FormWindowState.Normal
        };

        using KioskModeManager manager = new()
        {
            ContainerControl = form
        };

        manager.ToggleFullScreen();
        Assert.True(manager.FullScreen);

        manager.ContainerControl = null;

        Assert.False(manager.FullScreen);
        Assert.Equal(FormBorderStyle.FixedDialog, form.FormBorderStyle);
        Assert.Equal(FormWindowState.Normal, form.WindowState);
        Assert.Equal(new Rectangle(10, 20, 300, 200), form.Bounds);
    }

    [WinFormsFact]
    public void KioskModeManager_ContainerControl_Set_DoesNotChangeFormKeyPreview()
    {
        using Form form = new()
        {
            KeyPreview = false
        };

        using KioskModeManager manager = new()
        {
            ContainerControl = form
        };

        Assert.False(form.KeyPreview);
    }

    [WinFormsFact]
    public void KioskModeManager_FullScreen_SetTrueThenFalse_EntersAndExitsFullScreen()
    {
        using Form form = new()
        {
            Bounds = new Rectangle(10, 20, 300, 200),
            FormBorderStyle = FormBorderStyle.FixedDialog,
            WindowState = FormWindowState.Normal
        };

        using KioskModeManager manager = new()
        {
            ContainerControl = form
        };

        manager.FullScreen = true;

        Assert.True(manager.FullScreen);
        Assert.Equal(FormBorderStyle.None, form.FormBorderStyle);

        manager.FullScreen = false;

        Assert.False(manager.FullScreen);
        Assert.Equal(FormBorderStyle.FixedDialog, form.FormBorderStyle);
        Assert.Equal(FormWindowState.Normal, form.WindowState);
        Assert.Equal(new Rectangle(10, 20, 300, 200), form.Bounds);
    }

    [WinFormsFact]
    public void KioskModeManager_FullScreen_TopMostDisabled_OverridesAndRestoresFormTopMost()
    {
        using Form form = new()
        {
            TopMost = true
        };
        using KioskModeManager manager = new()
        {
            ContainerControl = form,
            TopMostInFullScreen = false
        };

        manager.FullScreen = true;

        Assert.False(form.TopMost);

        manager.FullScreen = false;

        Assert.True(form.TopMost);
    }

    [WinFormsFact]
    public void KioskModeManager_FullScreen_SetWithHandler_CallsFullScreenChanged()
    {
        using Form form = new();
        using KioskModeManager manager = new()
        {
            ContainerControl = form
        };

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(manager, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };

        manager.FullScreenChanged += handler;

        manager.FullScreen = true;
        Assert.Equal(1, callCount);

        manager.FullScreen = true;
        Assert.Equal(1, callCount);

        manager.FullScreen = false;
        Assert.Equal(2, callCount);

        manager.FullScreenChanged -= handler;
        manager.FullScreen = true;
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void KioskModeManager_OnContainerControlChanged_Invoke_CallsContainerControlChanged(EventArgs eventArgs)
    {
        using SubKioskModeManager manager = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(manager, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        manager.ContainerControlChanged += handler;
        manager.OnContainerControlChanged(eventArgs);
        Assert.Equal(1, callCount);

        manager.ContainerControlChanged -= handler;
        manager.OnContainerControlChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void KioskModeManager_OnFullScreenChanged_Invoke_CallsFullScreenChanged(EventArgs eventArgs)
    {
        using SubKioskModeManager manager = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(manager, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        manager.FullScreenChanged += handler;
        manager.OnFullScreenChanged(eventArgs);
        Assert.Equal(1, callCount);

        manager.FullScreenChanged -= handler;
        manager.OnFullScreenChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void KioskModeManager_Site_Set_AssignsRootComponentAsContainerControl()
    {
        using Form form = new();
        Mock<IDesignerHost> host = new();
        host.Setup(h => h.RootComponent).Returns(form);

        Mock<ISite> site = new();
        site.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(host.Object);

        using KioskModeManager manager = new();
        manager.Site = site.Object;

        Assert.Same(form, manager.ContainerControl);
    }

    [WinFormsFact]
    public void KioskModeManager_Site_Set_DoesNotOverwriteExplicitContainerControl()
    {
        using Form explicitForm = new();
        using Form rootForm = new();
        Mock<IDesignerHost> host = new();
        host.Setup(h => h.RootComponent).Returns(rootForm);

        Mock<ISite> site = new();
        site.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(host.Object);

        using KioskModeManager manager = new()
        {
            ContainerControl = explicitForm
        };

        manager.Site = site.Object;

        Assert.Same(explicitForm, manager.ContainerControl);
    }

    [WinFormsFact]
    public void KioskModeManager_Site_SetWithExplicitNull_DoesNotAssignRootComponent()
    {
        using Form form = new();
        Mock<IDesignerHost> host = new();
        host.Setup(h => h.RootComponent).Returns(form);

        Mock<ISite> site = new();
        site.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(host.Object);

        using KioskModeManager manager = new()
        {
            ContainerControl = null
        };

        manager.Site = site.Object;

        Assert.Null(manager.ContainerControl);
    }

    [WinFormsFact]
    public void KioskModeManager_Site_SetWithoutExplicitContainerControl_UpdatesResolvedRootComponent()
    {
        using Form firstForm = new();
        using Form secondForm = new();

        Mock<IDesignerHost> firstHost = new();
        firstHost.Setup(h => h.RootComponent).Returns(firstForm);
        Mock<IDesignerHost> secondHost = new();
        secondHost.Setup(h => h.RootComponent).Returns(secondForm);

        Mock<ISite> firstSite = new();
        firstSite.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(firstHost.Object);
        Mock<ISite> secondSite = new();
        secondSite.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(secondHost.Object);

        using KioskModeManager manager = new();
        manager.Site = firstSite.Object;
        Assert.Same(firstForm, manager.ContainerControl);

        manager.Site = secondSite.Object;
        Assert.Same(secondForm, manager.ContainerControl);
    }

    [WinFormsFact]
    public void KioskModeManager_BeginInit_FullScreenSet_DoesNotEnterUntilEndInit()
    {
        using Form form = new()
        {
            Bounds = new Rectangle(10, 20, 300, 200),
            FormBorderStyle = FormBorderStyle.FixedDialog,
            WindowState = FormWindowState.Normal
        };

        using KioskModeManager manager = new();
        ISupportInitialize supportInitialize = manager;
        Rectangle screenBounds = Screen.FromRectangle(form.Bounds).Bounds;

        supportInitialize.BeginInit();
        manager.ContainerControl = form;
        manager.FullScreen = true;

        Assert.Equal(FormBorderStyle.FixedDialog, form.FormBorderStyle);
        Assert.Equal(FormWindowState.Normal, form.WindowState);

        supportInitialize.EndInit();

        Assert.True(manager.FullScreen);
        Assert.Equal(FormBorderStyle.None, form.FormBorderStyle);
        Assert.Equal(FormWindowState.Normal, form.WindowState);
        Assert.Equal(screenBounds, form.Bounds);
    }

    [WinFormsFact]
    public void KioskModeManager_DisposeWhileFullScreen_RestoresExpected()
    {
        using Form form = new()
        {
            Bounds = new Rectangle(10, 20, 300, 200),
            FormBorderStyle = FormBorderStyle.FixedDialog,
            WindowState = FormWindowState.Normal,
            TopMost = false
        };

        KioskModeManager manager = new()
        {
            ContainerControl = form,
            TopMostInFullScreen = true,
            FullScreen = true
        };
        manager.Dispose();

        Assert.Equal(FormBorderStyle.FixedDialog, form.FormBorderStyle);
        Assert.Equal(FormWindowState.Normal, form.WindowState);
        Assert.Equal(new Rectangle(10, 20, 300, 200), form.Bounds);
        Assert.False(form.TopMost);
    }

    [WinFormsFact]
    public void KioskModeManager_ProcessMessage_KeyDownThenKeyUp_TogglesOnce()
    {
        using Form form = new();
        using KioskModeManager manager = new()
        {
            ContainerControl = form
        };

        Message keyDownMessage = Message.Create(form.Handle, (int)PInvokeCore.WM_KEYDOWN, (nint)Keys.F11, 0);
        Message keyUpMessage = Message.Create(form.Handle, (int)PInvokeCore.WM_KEYUP, (nint)Keys.F11, 0);

        manager.TestAccessor.Dynamic.ProcessMessage(keyDownMessage);
        Assert.True(manager.FullScreen);

        manager.TestAccessor.Dynamic.ProcessMessage(keyUpMessage);
        Assert.True(manager.FullScreen);
    }

    [WinFormsFact]
    public void KioskModeManager_ProcessMessage_RepeatedKeyDown_DoesNotToggleAgain()
    {
        using Form form = new();
        using KioskModeManager manager = new()
        {
            ContainerControl = form
        };

        Message keyDownMessage = Message.Create(form.Handle, (int)PInvokeCore.WM_KEYDOWN, (nint)Keys.F11, 0);
        Message repeatedKeyDownMessage = Message.Create(
            form.Handle,
            (int)PInvokeCore.WM_KEYDOWN,
            (nint)Keys.F11,
            1 << 30);

        manager.TestAccessor.Dynamic.ProcessMessage(keyDownMessage);
        manager.TestAccessor.Dynamic.ProcessMessage(repeatedKeyDownMessage);

        Assert.True(manager.FullScreen);
    }

    [WinFormsFact]
    public void KioskModeManager_ProcessMessage_MouseMove_ShowsHiddenMousePointer()
    {
        using Form form = new();
        using KioskModeManager manager = new()
        {
            ContainerControl = form,
            MousePointerAutoHideDelay = 1,
            FullScreen = true
        };

        manager.TestAccessor.Dynamic.OnMousePointerAutoHideTimerTick(null, EventArgs.Empty);
        Assert.True(manager.TestAccessor.Dynamic._isCursorHidden);

        Message message = Message.Create(form.Handle, (int)PInvokeCore.WM_MOUSEMOVE, 0, 0);
        manager.TestAccessor.Dynamic.ProcessMessage(message);
        Timer timer = manager.TestAccessor.Dynamic._mousePointerAutoHideTimer;

        Assert.False(manager.TestAccessor.Dynamic._isCursorHidden);
        Assert.True(timer.Enabled);
        Assert.Equal(1, timer.Interval);
    }

    /// <summary>
    ///  Records native window size changes during fullscreen transitions.
    /// </summary>
    private class WindowPositionForm : Form
    {
        public List<Size> NativeSizes { get; } = [];

        public void SetMaximizedBounds(Rectangle bounds) => MaximizedBounds = bounds;

        public void RecreateWindow() => RecreateHandle();

        protected override unsafe void WndProc(ref Message m)
        {
            if (m.MsgInternal == PInvokeCore.WM_WINDOWPOSCHANGED)
            {
                WINDOWPOS* position = (WINDOWPOS*)(nint)m.LParamInternal;
                if ((position->flags & SET_WINDOW_POS_FLAGS.SWP_NOSIZE) == 0)
                {
                    NativeSizes.Add(new Size(position->cx, position->cy));
                }
            }

            base.WndProc(ref m);
        }
    }

    private class SubKioskModeManager : KioskModeManager
    {
        public new bool DesignMode => base.DesignMode;

        public new void OnContainerControlChanged(EventArgs e)
            => base.OnContainerControlChanged(e);

        public new void OnFullScreenChanged(EventArgs e)
            => base.OnFullScreenChanged(e);
    }
}
#endif
