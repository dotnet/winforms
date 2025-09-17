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

    [WinFormsFact]
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
