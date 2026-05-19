// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Input;
using Moq;

namespace System.Windows.Forms.Tests;

public class KioskModeManagerTests
{
    [WinFormsFact]
    public void KioskModeManager_Ctor_Default()
    {
        using SubKioskModeManager manager = new();

        Assert.Null(manager.Container);
        Assert.Null(manager.ContainerControl);
        Assert.False(manager.DesignMode);
        Assert.True(manager.EscapeExitsFullScreen);
        Assert.False(manager.HideTaskbar);
        Assert.False(manager.FullScreen);
        Assert.Equal(0, manager.MousePointerAutoHideDelay);
        Assert.Null(manager.Site);
        Assert.False(manager.SuppressPowerSaving);
        Assert.Equal(Keys.F11, manager.ToggleFullScreenKey);
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

    [WinFormsTheory]
    [EnumData<KioskModeWakeupSource>]
    public void KioskModeWakeupEventArgs_Ctor_Source(KioskModeWakeupSource source)
    {
        KioskModeWakeupEventArgs eventArgs = new(source);

        Assert.Equal(source, eventArgs.Source);
    }

    [WinFormsTheory]
    [InvalidEnumData<KioskModeWakeupSource>]
    public void KioskModeWakeupEventArgs_Ctor_InvalidSource_ThrowsInvalidEnumArgumentException(KioskModeWakeupSource source)
    {
        Assert.Throws<InvalidEnumArgumentException>("source", () => new KioskModeWakeupEventArgs(source));
    }

    [WinFormsTheory]
    [EnumData<KioskModeWakeupSource>]
    public void KioskModeManager_OnWakeup_Invoke_CallsWakeup(KioskModeWakeupSource source)
    {
        using SubKioskModeManager manager = new();
        KioskModeWakeupEventArgs eventArgs = new(source);
        int callCount = 0;
        KioskModeWakeupEventHandler handler = (sender, e) =>
        {
            Assert.Same(manager, sender);
            Assert.Same(eventArgs, e);
            Assert.Equal(source, e.Source);
            callCount++;
        };

        manager.Wakeup += handler;
        manager.OnWakeup(eventArgs);
        Assert.Equal(1, callCount);

        manager.Wakeup -= handler;
        manager.OnWakeup(eventArgs);
        Assert.Equal(1, callCount);
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
            HideTaskbar = false,
            TopMostInFullScreen = true
        };

        manager.ToggleFullScreen();

        Assert.True(manager.FullScreen);
        Assert.Equal(FormBorderStyle.None, form.FormBorderStyle);
        Assert.True(form.TopMost);
        Assert.Equal(FormWindowState.Maximized, form.WindowState);

        manager.ToggleFullScreen();

        Assert.False(manager.FullScreen);
        Assert.Equal(FormBorderStyle.FixedDialog, form.FormBorderStyle);
        Assert.False(form.TopMost);
        Assert.Equal(FormWindowState.Normal, form.WindowState);
        Assert.Equal(new Rectangle(10, 20, 300, 200), form.Bounds);
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
    [EnumData<KioskModeWakeupSource>]
    public void KioskModeManager_WakeUpCommand_ExecutedOnWakeup_WithSourceName(KioskModeWakeupSource source)
    {
        using SubKioskModeManager manager = new();
        TestCommand command = new();
        manager.WakeUpCommand = command;

        Assert.Same(command, manager.WakeUpCommand);

        manager.OnWakeup(new KioskModeWakeupEventArgs(source));

        Assert.Equal(1, command.ExecuteCount);
        Assert.Equal(source.ToString(), command.LastParameter);
    }

    [WinFormsFact]
    public void KioskModeManager_WakeUpCommand_Null_DoesNotThrowOnWakeup()
    {
        using SubKioskModeManager manager = new();

        Assert.Null(manager.WakeUpCommand);

        manager.OnWakeup(new KioskModeWakeupEventArgs(KioskModeWakeupSource.Keyboard));
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

    private class TestCommand : ICommand
    {
        public int ExecuteCount { get; private set; }

        public object LastParameter { get; private set; }

        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            ExecuteCount++;
            LastParameter = parameter;
        }
    }

    private class SubKioskModeManager : KioskModeManager
    {
        public new bool DesignMode => base.DesignMode;

        public new void OnWakeup(KioskModeWakeupEventArgs e)
            => base.OnWakeup(e);
    }
}
