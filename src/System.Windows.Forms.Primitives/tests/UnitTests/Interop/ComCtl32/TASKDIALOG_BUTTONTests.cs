// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Controls;

namespace System.Windows.Forms.Primitives.Tests.Interop.ComCtl32;

public class TASKDIALOG_BUTTONTests
{
    [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
    public unsafe void TASKDIALOG_BUTTON_x32_Size()
    {
        if (Environment.Is64BitProcess)
        {
            return;
        }

        Assert.Equal(8, sizeof(TASKDIALOG_BUTTON));
    }

    [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
    public unsafe void TASKDIALOG_BUTTON_x32_ensure_layout()
    {
        if (Environment.Is64BitProcess)
        {
            return;
        }

        TASKDIALOG_BUTTON sut = default;
        byte* addr = (byte*)&sut;

        Assert.Equal(0, (byte*)&sut.nButtonID - addr);                // 4, int
        Assert.Equal(4, (byte*)&sut.pszButtonText - addr);            // 4, PCWSTR
    }

    [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
    public unsafe void TASKDIALOG_BUTTON_x64_Size()
    {
        if (!Environment.Is64BitProcess)
        {
            return;
        }

        Assert.Equal(12, sizeof(TASKDIALOG_BUTTON));
    }

    [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
    public unsafe void TASKDIALOG_BUTTON_x64_ensure_layout()
    {
        if (!Environment.Is64BitProcess)
        {
            return;
        }

        TASKDIALOG_BUTTON sut = default;
        byte* addr = (byte*)&sut;

        Assert.Equal(0, (byte*)&sut.nButtonID - addr);                // 4, int
        Assert.Equal(4, (byte*)&sut.pszButtonText - addr);            // 8, PCWSTR
    }
}
