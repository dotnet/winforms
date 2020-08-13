// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reflection;
using Moq;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class OSFeatureTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void OSFeature_Ctor_Default()
        {
            // Make sure it doesn't throw.
            new SubOSFeature();
        }

        [Fact]
        public void OSFeature_Feature_Get_ReturnsExpected()
        {
            OSFeature feature = OSFeature.Feature;
            Assert.NotNull(feature);
            Assert.Same(feature, OSFeature.Feature);
        }

        [Fact]
        public void OSFeature_LayeredWindows_Get_ReturnsExpected()
        {
            object feature = OSFeature.LayeredWindows;
            Assert.NotNull(feature);
            Assert.Same(feature, OSFeature.LayeredWindows);
        }

        [Fact]
        public void OSFeature_Themes_Get_ReturnsExpected()
        {
            object feature = OSFeature.Themes;
            Assert.NotNull(feature);
            Assert.Same(feature, OSFeature.Themes);
        }

        public static IEnumerable<object[]> GetVersionPresent_TestData()
        {
            yield return new object[] { OSFeature.LayeredWindows, new Version(0, 0, 0, 0) };
            yield return new object[] { OSFeature.Themes, new Version(0, 0, 0, 0) };
            yield return new object[] { new object(), null };
            yield return new object[] { null, null };
        }

        [Theory]
        [MemberData(nameof(GetVersionPresent_TestData))]
        public void OSFeature_GetVersionPresent_Invoke_ReturnsExpected(object feature, Version expected)
        {
            Assert.Equal(expected, OSFeature.Feature.GetVersionPresent(feature));

            // Call again to test caching.
            Assert.Equal(expected, OSFeature.Feature.GetVersionPresent(feature));
        }

        public static IEnumerable<object[]> IsPresent_TestData()
        {
            yield return new object[] { SystemParameter.DropShadow, true };
            yield return new object[] { SystemParameter.FlatMenu, true };
            yield return new object[] { SystemParameter.FontSmoothingContrastMetric, true };
            yield return new object[] { SystemParameter.FontSmoothingTypeMetric, true };
            yield return new object[] { SystemParameter.MenuFadeEnabled, true };
            yield return new object[] { SystemParameter.SelectionFade, true };
            yield return new object[] { SystemParameter.ToolTipAnimationMetric, true };
            yield return new object[] { SystemParameter.UIEffects, true };
            yield return new object[] { SystemParameter.CaretWidthMetric, true };
            yield return new object[] { SystemParameter.VerticalFocusThicknessMetric, true };
            yield return new object[] { SystemParameter.HorizontalFocusThicknessMetric, true };
            yield return new object[] { SystemParameter.DropShadow - 1, false };
            yield return new object[] { SystemParameter.HorizontalFocusThicknessMetric + 1, false };
        }

        [Theory]
        [MemberData(nameof(IsPresent_TestData))]
        public void OSFeature_IsPresent_Invoke_ReturnsExpected(SystemParameter feature, bool expected)
        {
            Assert.Equal(expected, OSFeature.IsPresent(feature));
        }

        private class SubOSFeature : OSFeature
        {
            public SubOSFeature() : base()
            {
            }
        }
    }
}
