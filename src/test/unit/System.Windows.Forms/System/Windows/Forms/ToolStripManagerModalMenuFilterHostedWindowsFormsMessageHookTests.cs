// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class ToolStripManagerModalMenuFilterHostedWindowsFormsMessageHookTests
{
    private static Type GetHookType()
    {
        Type? hookType = typeof(ToolStripManager).Assembly
            .GetType("System.Windows.Forms.ToolStripManager+ModalMenuFilter+HostedWindowsFormsMessageHook", throwOnError: false);

        ArgumentNullException.ThrowIfNull(hookType);

        return hookType;
    }

    [Fact]
    public void Constructor_DoesNotThrow()
    {
        Type hookType = GetHookType();

        object? hook = Activator.CreateInstance(hookType, nonPublic: true);

        hook.Should().BeOfType(hookType);
    }

    [Fact]
    public void Constructor_SetsCallingStack()
    {
        Type hookType = GetHookType();

        object? hook = Activator.CreateInstance(hookType, nonPublic: true);

        string? callingStack = hook?.TestAccessor().Dynamic._callingStack;

        callingStack.Should().NotBeNullOrEmpty();
        callingStack.Should().Contain("ToolStripManagerModalMenuFilterHostedWindowsFormsMessageHookTests");
    }

    [Fact]
    public void InstallMessageHook_DoesNotThrow_WhenCalledTwice()
    {
        Type hookType = GetHookType();

        object? hook = Activator.CreateInstance(hookType, nonPublic: true);

        hook?.TestAccessor().Dynamic.InstallMessageHook();
        hook?.TestAccessor().Dynamic.InstallMessageHook();

        bool isHooked = hook?.TestAccessor().Dynamic._isHooked;
        isHooked.Should().BeTrue();

        IntPtr messageHookHandle = hook?.TestAccessor().Dynamic._messageHookHandle;
        messageHookHandle.Should().NotBe(IntPtr.Zero);
    }

    [Fact]
    public void UninstallMessageHook_DoesNotThrow_WhenCalledTwice()
    {
        Type hookType = GetHookType();

        object? hook = Activator.CreateInstance(hookType, nonPublic: true);

        hook?.TestAccessor().Dynamic.UninstallMessageHook();
        hook?.TestAccessor().Dynamic.UninstallMessageHook();

        bool isHooked = hook?.TestAccessor().Dynamic._isHooked;
        isHooked.Should().BeFalse();

        IntPtr messageHookHandle = hook?.TestAccessor().Dynamic._messageHookHandle;
        messageHookHandle.Should().Be(IntPtr.Zero);
    }
}
