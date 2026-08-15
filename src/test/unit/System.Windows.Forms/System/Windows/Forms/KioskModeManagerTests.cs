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
