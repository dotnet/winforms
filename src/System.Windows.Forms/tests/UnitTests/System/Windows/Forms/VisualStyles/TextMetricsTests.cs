// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.VisualStyles.Tests
{
    // NB: doesn't require thread affinity
    public class TextMetricsTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void TextMetrics_Ctor_Default()
        {
            var metric = new TextMetrics();
            Assert.Equal(0, metric.Ascent);
            Assert.Equal(0, metric.AverageCharWidth);
            Assert.Equal('\0', metric.BreakChar);
            Assert.Equal(TextMetricsCharacterSet.Ansi, metric.CharSet);
            Assert.Equal('\0', metric.DefaultChar);
            Assert.Equal(0, metric.Descent);
            Assert.Equal(0, metric.DigitizedAspectX);
            Assert.Equal(0, metric.DigitizedAspectY);
            Assert.Equal(0, metric.ExternalLeading);
            Assert.Equal('\0', metric.FirstChar);
            Assert.Equal(0, metric.Height);
            Assert.Equal(0, metric.InternalLeading);
            Assert.False(metric.Italic);
            Assert.Equal(0, metric.MaxCharWidth);
            Assert.Equal(0, metric.Overhang);
            Assert.Equal((TextMetricsPitchAndFamilyValues)0, metric.PitchAndFamily);
            Assert.False(metric.StruckOut);
            Assert.False(metric.Underlined);
            Assert.Equal(0, metric.Weight);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void TextMetrics_Ascent_Set_GetReturnsExpected(int value)
        {
            var metric = new TextMetrics
            {
                Ascent = value
            };
            Assert.Equal(value, metric.Ascent);

            // Set same.
            metric.Ascent = value;
            Assert.Equal(value, metric.Ascent);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void TextMetrics_AverageCharWidth_Set_GetReturnsExpected(int value)
        {
            var metric = new TextMetrics
            {
                AverageCharWidth = value
            };
            Assert.Equal(value, metric.AverageCharWidth);

            // Set same.
            metric.AverageCharWidth = value;
            Assert.Equal(value, metric.AverageCharWidth);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetCharTheoryData))]
        public void TextMetrics_BreakChar_Set_GetReturnsExpected(char value)
        {
            var metric = new TextMetrics
            {
                BreakChar = value
            };
            Assert.Equal(value, metric.BreakChar);

            // Set same.
            metric.BreakChar = value;
            Assert.Equal(value, metric.BreakChar);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(TextMetricsCharacterSet))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(TextMetricsCharacterSet))]
        public void TextMetrics_CharSet_Set_GetReturnsExpected(TextMetricsCharacterSet value)
        {
            var metric = new TextMetrics
            {
                CharSet = value
            };
            Assert.Equal(value, metric.CharSet);

            // Set same.
            metric.CharSet = value;
            Assert.Equal(value, metric.CharSet);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetCharTheoryData))]
        public void TextMetrics_DefaultChar_Set_GetReturnsExpected(char value)
        {
            var metric = new TextMetrics
            {
                DefaultChar = value
            };
            Assert.Equal(value, metric.DefaultChar);

            // Set same.
            metric.DefaultChar = value;
            Assert.Equal(value, metric.DefaultChar);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void TextMetrics_Descent_Set_GetReturnsExpected(int value)
        {
            var metric = new TextMetrics
            {
                Descent = value
            };
            Assert.Equal(value, metric.Descent);

            // Set same.
            metric.Descent = value;
            Assert.Equal(value, metric.Descent);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void TextMetrics_DigitizedAspectX_Set_GetReturnsExpected(int value)
        {
            var metric = new TextMetrics
            {
                DigitizedAspectX = value
            };
            Assert.Equal(value, metric.DigitizedAspectX);

            // Set same.
            metric.DigitizedAspectX = value;
            Assert.Equal(value, metric.DigitizedAspectX);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void TextMetrics_DigitizedAspectY_Set_GetReturnsExpected(int value)
        {
            var metric = new TextMetrics
            {
                DigitizedAspectY = value
            };
            Assert.Equal(value, metric.DigitizedAspectY);

            // Set same.
            metric.DigitizedAspectY = value;
            Assert.Equal(value, metric.DigitizedAspectY);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void TextMetrics_ExternalLeading_Set_GetReturnsExpected(int value)
        {
            var metric = new TextMetrics
            {
                ExternalLeading = value
            };
            Assert.Equal(value, metric.ExternalLeading);

            // Set same.
            metric.ExternalLeading = value;
            Assert.Equal(value, metric.ExternalLeading);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetCharTheoryData))]
        public void TextMetrics_FirstChar_Set_GetReturnsExpected(char value)
        {
            var metric = new TextMetrics
            {
                FirstChar = value
            };
            Assert.Equal(value, metric.FirstChar);

            // Set same.
            metric.FirstChar = value;
            Assert.Equal(value, metric.FirstChar);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void TextMetrics_Height_Set_GetReturnsExpected(int value)
        {
            var metric = new TextMetrics
            {
                Height = value
            };
            Assert.Equal(value, metric.Height);

            // Set same.
            metric.Height = value;
            Assert.Equal(value, metric.Height);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void TextMetrics_InternalLeading_Set_GetReturnsExpected(int value)
        {
            var metric = new TextMetrics
            {
                InternalLeading = value
            };
            Assert.Equal(value, metric.InternalLeading);

            // Set same.
            metric.InternalLeading = value;
            Assert.Equal(value, metric.InternalLeading);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TextMetrics_Italic_Set_GetReturnsExpected(bool value)
        {
            var metric = new TextMetrics
            {
                Italic = value
            };
            Assert.Equal(value, metric.Italic);

            // Set same.
            metric.Italic = value;
            Assert.Equal(value, metric.Italic);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetCharTheoryData))]
        public void TextMetrics_LastChar_Set_GetReturnsExpected(char value)
        {
            var metric = new TextMetrics
            {
                LastChar = value
            };
            Assert.Equal(value, metric.LastChar);

            // Set same.
            metric.LastChar = value;
            Assert.Equal(value, metric.LastChar);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void TextMetrics_MaxCharWidth_Set_GetReturnsExpected(int value)
        {
            var metric = new TextMetrics
            {
                MaxCharWidth = value
            };
            Assert.Equal(value, metric.MaxCharWidth);

            // Set same.
            metric.MaxCharWidth = value;
            Assert.Equal(value, metric.MaxCharWidth);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void TextMetrics_Overhang_Set_GetReturnsExpected(int value)
        {
            var metric = new TextMetrics
            {
                Overhang = value
            };
            Assert.Equal(value, metric.Overhang);

            // Set same.
            metric.Overhang = value;
            Assert.Equal(value, metric.Overhang);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(TextMetricsPitchAndFamilyValues))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(TextMetricsPitchAndFamilyValues))]
        public void TextMetrics_PitchAndFamily_GetReturnsExpected(TextMetricsPitchAndFamilyValues value)
        {
            var metric = new TextMetrics
            {
                PitchAndFamily = value
            };
            Assert.Equal(value, metric.PitchAndFamily);

            // Set same.
            metric.PitchAndFamily = value;
            Assert.Equal(value, metric.PitchAndFamily);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TextMetrics_StruckOut_Set_GetReturnsExpected(bool value)
        {
            var metric = new TextMetrics
            {
                StruckOut = value
            };
            Assert.Equal(value, metric.StruckOut);

            // Set same.
            metric.StruckOut = value;
            Assert.Equal(value, metric.StruckOut);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TextMetrics_Underlined_Set_GetReturnsExpected(bool value)
        {
            var metric = new TextMetrics
            {
                Underlined = value
            };
            Assert.Equal(value, metric.Underlined);

            // Set same.
            metric.Underlined = value;
            Assert.Equal(value, metric.Underlined);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void TextMetrics_Weight_Set_GetReturnsExpected(int value)
        {
            var metric = new TextMetrics
            {
                Weight = value
            };
            Assert.Equal(value, metric.Weight);

            // Set same.
            metric.Weight = value;
            Assert.Equal(value, metric.Weight);
        }
    }
}
