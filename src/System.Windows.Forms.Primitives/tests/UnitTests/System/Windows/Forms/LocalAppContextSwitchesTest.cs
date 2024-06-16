// Licensed to the.NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Versioning;
using System.Windows.Forms.Primitives;

namespace System.Windows.Forms.Tests;

public class LocalAppContextSwitchesTest
{
    private void ResetLocalSwitches(dynamic testAccessor)
    {
        testAccessor.s_anchorLayoutV2 = 0;
        testAccessor.s_scaleTopLevelFormMinMaxSizeForDpi = 0;
        testAccessor.s_trackBarModernRendering = 0;
        testAccessor.s_servicePointManagerCheckCrl = 0;
    }

    [WinFormsTheory]
    [InlineData(".NETCoreApp,Version=v8.0", true)]
    [InlineData(".NETCoreApp,Version=v7.0", false)]
    [InlineData(".NET Framework,Version=v4.8", false)]
    public void Validate_Default_Switch_Values(string targetFrameworkName, bool expected)
    {
        FrameworkName? previousTestTargetFramework = LocalAppContextSwitches.TargetFrameworkName;
        dynamic testAccessor = typeof(LocalAppContextSwitches).TestAccessor().Dynamic;
        ResetLocalSwitches(testAccessor);

        try
        {
            testAccessor.s_targetFrameworkName = new FrameworkName(targetFrameworkName);

            Assert.Equal(expected, LocalAppContextSwitches.TrackBarModernRendering);
            Assert.Equal(expected, LocalAppContextSwitches.ScaleTopLevelFormMinMaxSizeForDpi);
            Assert.Equal(expected, LocalAppContextSwitches.ServicePointManagerCheckCrl);
        }
        finally
        {
            // Reset target framework name.
            testAccessor.s_targetFrameworkName = previousTestTargetFramework;
            ResetLocalSwitches(testAccessor);
        }
    }
}
