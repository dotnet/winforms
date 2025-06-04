// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using TASKDIALOGCONFIG_MainIcon = Windows.Win32.UI.Controls.TASKDIALOGCONFIG._Anonymous1_e__Union;

namespace System.Windows.Forms.Primitives.Tests.Interop.ComCtl32;

public class TASKDIALOGCONFIGIconUnionTests
{
    [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
    public unsafe void TASKDIALOGCONFIGIconUnion_x32_Size()
    {
        if (Environment.Is64BitProcess)
        {
            return;
        }

        Assert.Equal(4, sizeof(TASKDIALOGCONFIG_MainIcon));
    }

    [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
    public unsafe void TASKDIALOGCONFIGIconUnion_x32_ensure_layout()
    {
        if (Environment.Is64BitProcess)
        {
            return;
        }

        TASKDIALOGCONFIG_MainIcon icon = default;
        byte* addr = (byte*)&icon;

        Assert.Equal(0, (byte*)&icon.hMainIcon - addr);  // 4, HICON
        Assert.Equal(0, (byte*)&icon.pszMainIcon - addr);  // 4, PCWSTR
    }

    [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
    public unsafe void TASKDIALOGCONFIGIconUnion_x64_Size()
    {
        if (!Environment.Is64BitProcess)
        {
            return;
        }

        Assert.Equal(8, sizeof(TASKDIALOGCONFIG_MainIcon));
    }

    [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
    public unsafe void TASKDIALOGCONFIGIconUnion_x64_ensure_layout()
    {
        if (!Environment.Is64BitProcess)
        {
            return;
        }

        TASKDIALOGCONFIG_MainIcon icon = default;
        byte* addr = (byte*)&icon;

        Assert.Equal(0, (byte*)&icon.hMainIcon - addr);  // 8, HICON
        Assert.Equal(0, (byte*)&icon.pszMainIcon - addr);  // 8, PCWSTR
    }
}
