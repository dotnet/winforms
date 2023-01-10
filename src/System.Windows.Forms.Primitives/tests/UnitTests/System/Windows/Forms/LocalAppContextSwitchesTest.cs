// Licensed to the.NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Versioning;
using System.Windows.Forms.Primitives;

namespace System.Windows.Forms.Tests
{
    [Collection(nameof(SynchronousCollection))]
    public class LocalAppContextSwitchesTest
    {
        private void ResetLocalSwitches(dynamic testAccessor)
        {
            testAccessor.s_AnchorLayoutV2 = 0;
            testAccessor.s_scaleTopLevelFormMinMaxSizeForDpi = 0;
            testAccessor.s_trackBarModernRendering = 0;
        }

        [WinFormsTheory]
        [InlineData(".NETCoreApp,Version=v8.0", true)]
        [InlineData(".NETCoreApp,Version=v7.0", false)]
        [InlineData(".NET Framework,Version=v4.8", false)]
        public void Validate_Default_Switch_Values(string tragetFrameworkName, bool expected)
        {
            FrameworkName? previousTestTargetFramework = LocalAppContextSwitches.TargetFrameworkName;
            dynamic testAccessor = typeof(LocalAppContextSwitches).TestAccessor().Dynamic;

            try
            {
                testAccessor.s_targetFrameworkName = new FrameworkName(tragetFrameworkName);

                Assert.Equal(expected, LocalAppContextSwitches.TrackBarModernRendering);
                Assert.Equal(expected, LocalAppContextSwitches.AnchorLayoutV2);
                Assert.Equal(expected, LocalAppContextSwitches.ScaleTopLevelFormMinMaxSizeForDpi);
            }
            finally
            {
                // Reset target framework name.
                testAccessor.s_targetFrameworkName = previousTestTargetFramework;
                ResetLocalSwitches(testAccessor);
            }
        }

        [WinFormsTheory]
        [InlineData(".NETCoreApp,Version=v8.0", true)]
        [InlineData(".NETCoreApp,Version=v7.0", true)]
        [InlineData(".NET Framework,Version=v4.8", false)]
        public void Validate_TargetFramework_Is_NETCore(string tragetFrameworkName, bool isNetCoreApp)
        {
            FrameworkName? previousTestTargetFramework = LocalAppContextSwitches.TargetFrameworkName;
            dynamic testAccessor = typeof(LocalAppContextSwitches).TestAccessor().Dynamic;

            try
            {
                testAccessor.s_targetFrameworkName = new FrameworkName(tragetFrameworkName);
                bool isCoreApplication = LocalAppContextSwitches.IsNetCoreApp;
                Assert.Equal(isNetCoreApp, isCoreApplication);
            }
            finally
            {
                // Reset target framework name.
                testAccessor.s_targetFrameworkName = previousTestTargetFramework;
            }
        }
    }
}
