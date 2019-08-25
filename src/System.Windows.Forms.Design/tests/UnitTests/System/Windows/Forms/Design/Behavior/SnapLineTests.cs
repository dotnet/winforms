// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Windows.Forms.Design.Behavior.Tests
{
    public class SnapLineTests
    {
        private static string[] s_Margins = new[] { SnapLine.MarginLeft, SnapLine.MarginTop, SnapLine.MarginRight, SnapLine.MarginBottom };
        private static string[] s_Paddings = new[] { SnapLine.PaddingLeft, SnapLine.PaddingTop, SnapLine.PaddingRight, SnapLine.PaddingBottom };

        private const int DefaultOffset = 123;
        private const string DefaultFilter = "filter";
        private const SnapLinePriority DefaultPriority = SnapLinePriority.Medium;

        [Fact]
        public void SnapLine_Ctor_type_offset()
        {
            var snapLine = new SnapLine(SnapLineType.Baseline, DefaultOffset);

            Assert.Equal(SnapLineType.Baseline, snapLine.SnapLineType);
            Assert.Equal(DefaultOffset, snapLine.Offset);
            Assert.Null(snapLine.Filter);
            Assert.Equal(SnapLinePriority.Low, snapLine.Priority);
        }

        [Fact]
        public void SnapLine_Ctor_type_offset_filter()
        {
            var snapLine = new SnapLine(SnapLineType.Baseline, DefaultOffset, DefaultFilter);

            Assert.Equal(SnapLineType.Baseline, snapLine.SnapLineType);
            Assert.Equal(DefaultOffset, snapLine.Offset);
            Assert.Equal(DefaultFilter, snapLine.Filter);
            Assert.Equal(SnapLinePriority.Low, snapLine.Priority);
        }

        [Fact]
        public void SnapLine_Ctor_type_offset_priority()
        {
            var snapLine = new SnapLine(SnapLineType.Baseline, DefaultOffset, DefaultPriority);

            Assert.Equal(SnapLineType.Baseline, snapLine.SnapLineType);
            Assert.Equal(DefaultOffset, snapLine.Offset);
            Assert.Null(snapLine.Filter);
            Assert.Equal(DefaultPriority, snapLine.Priority);
        }

        [Fact]
        public void SnapLine_Ctor_type_offset_filter_priority()
        {
            var snapLine = new SnapLine(SnapLineType.Baseline, DefaultOffset, DefaultFilter, DefaultPriority);

            Assert.Equal(SnapLineType.Baseline, snapLine.SnapLineType);
            Assert.Equal(DefaultOffset, snapLine.Offset);
            Assert.Equal(DefaultFilter, snapLine.Filter);
            Assert.Equal(DefaultPriority, snapLine.Priority);
        }

        [Theory]
        [InlineData(SnapLineType.Top, true)]
        [InlineData(SnapLineType.Bottom, true)]
        [InlineData(SnapLineType.Horizontal, true)]
        [InlineData(SnapLineType.Baseline, true)]
        [InlineData(SnapLineType.Left, false)]
        [InlineData(SnapLineType.Right, false)]
        [InlineData(SnapLineType.Vertical, false)]
        public void SnapLine_IsHorizontal(SnapLineType type, bool expected)
        {
            var snapLine = new SnapLine(type, DefaultOffset, DefaultFilter, DefaultPriority);

            Assert.Equal(expected, snapLine.IsHorizontal);
        }

        [Theory]
        [InlineData(SnapLineType.Top, false)]
        [InlineData(SnapLineType.Bottom, false)]
        [InlineData(SnapLineType.Horizontal, false)]
        [InlineData(SnapLineType.Baseline, false)]
        [InlineData(SnapLineType.Left, true)]
        [InlineData(SnapLineType.Right, true)]
        [InlineData(SnapLineType.Vertical, true)]
        public void SnapLine_IsVertical(SnapLineType type, bool expected)
        {
            var snapLine = new SnapLine(type, DefaultOffset, DefaultFilter, DefaultPriority);

            Assert.Equal(expected, snapLine.IsVertical);
        }

        public static IEnumerable<object[]> SnapLineType_Set_TestData()
        {
            foreach (var type in Enum.GetValues(typeof(SnapLineType)))
            {
                yield return new[] { type };
            }
        }

        [Theory]
        [MemberData(nameof(SnapLineType_Set_TestData))]
        public void SnapLine_ensure_IsHorizontal_IsVertical_do_not_overlap(SnapLineType type)
        {
            var snapLine = new SnapLine(type, DefaultOffset, DefaultFilter, DefaultPriority);

            Assert.NotEqual(snapLine.IsHorizontal, snapLine.IsVertical);
        }

        [Theory]
        [InlineData(DefaultOffset, 10, DefaultOffset + 10)]
        [InlineData(DefaultOffset, -10, DefaultOffset - 10)]
        [InlineData(DefaultOffset, int.MaxValue, /* overflown */-2147483526)]
        [InlineData(-DefaultOffset, int.MinValue, /* overflown */2147483525)]
        public void SnapLine_AdjustOffset(int offset, int adjustment, int expected)
        {
            var snapLine = new SnapLine(SnapLineType.Baseline, offset, DefaultFilter, DefaultPriority);

            snapLine.AdjustOffset(adjustment);
            Assert.Equal(expected, snapLine.Offset);
        }

        [Fact]
        public void SnapLine_ShouldSnap_should_return_false_if_types_differ()
        {
            var snapLine1 = new SnapLine(SnapLineType.Top, DefaultOffset, DefaultFilter, DefaultPriority);
            var snapLine2 = new SnapLine(SnapLineType.Baseline, DefaultOffset, DefaultFilter, DefaultPriority);

            Assert.False(SnapLine.ShouldSnap(snapLine1, snapLine2));
        }

        [Fact]
        public void SnapLine_ShouldSnap_should_return_true_if_types_equal_and_both_filters_null()
        {
            var snapLine1 = new SnapLine(SnapLineType.Top, DefaultOffset, null, DefaultPriority);
            var snapLine2 = new SnapLine(SnapLineType.Top, DefaultOffset, null, SnapLinePriority.Low);

            Assert.True(SnapLine.ShouldSnap(snapLine1, snapLine2));
        }

        [Fact]
        public void SnapLine_ShouldSnap_should_return_false_if_types_equal_and_one_of_filters_not_null()
        {
            var snapLine1 = new SnapLine(SnapLineType.Top, DefaultOffset, null, DefaultPriority);
            var snapLine2 = new SnapLine(SnapLineType.Top, DefaultOffset, DefaultFilter, SnapLinePriority.Low);

            Assert.False(SnapLine.ShouldSnap(snapLine1, snapLine2));
        }

        public static IEnumerable<object[]> SnapLineFilter_Margin_TestData()
        {
            return EnumerateFilterMarginPaths(SnapLine.MarginRight, SnapLine.MarginLeft, SnapLine.PaddingRight)
                .Union(EnumerateFilterMarginPaths(SnapLine.MarginLeft, SnapLine.MarginRight, SnapLine.PaddingLeft))
                .Union(EnumerateFilterMarginPaths(SnapLine.MarginTop, SnapLine.MarginBottom, SnapLine.PaddingTop))
                .Union(EnumerateFilterMarginPaths(SnapLine.MarginBottom, SnapLine.MarginTop, SnapLine.PaddingBottom));

            IEnumerable<object[]> EnumerateFilterMarginPaths(string snapLine1Filter, string snapLine2MarginFilter, string snapLine2PaddingFilter)
            {
                // happy paths
                yield return new object[] { snapLine1Filter, snapLine2MarginFilter, true };
                yield return new object[] { snapLine1Filter, snapLine2PaddingFilter, true };

                // unhappy paths
                foreach (var margin in s_Margins.Except(new[] { snapLine2MarginFilter }))
                {
                    yield return new object[] { snapLine1Filter, margin, false };
                }
                foreach (var margin in s_Paddings.Except(new[] { snapLine2PaddingFilter }))
                {
                    yield return new object[] { snapLine1Filter, margin, false };
                }
            }
        }

        [Theory]
        [MemberData(nameof(SnapLineFilter_Margin_TestData))]
        public void SnapLine_ShouldSnap_should_return_expected_if_types_equal_and_filter_contains_margin(string snapLine1Filter, string snapLine2Filter, bool expected)
        {
            var snapLine1 = new SnapLine(SnapLineType.Top, DefaultOffset, snapLine1Filter, DefaultPriority);
            var snapLine2 = new SnapLine(SnapLineType.Top, DefaultOffset, snapLine2Filter, SnapLinePriority.Low);

            Assert.Equal(expected, SnapLine.ShouldSnap(snapLine1, snapLine2));
        }

        public static IEnumerable<object[]> SnapLineFilter_Padding_TestData()
        {
            return EnumerateFilterPaddingPaths(SnapLine.PaddingLeft, SnapLine.MarginLeft)
                .Union(EnumerateFilterPaddingPaths(SnapLine.PaddingRight, SnapLine.MarginRight))
                .Union(EnumerateFilterPaddingPaths(SnapLine.PaddingTop, SnapLine.MarginTop))
                .Union(EnumerateFilterPaddingPaths(SnapLine.PaddingBottom, SnapLine.MarginBottom));

            IEnumerable<object[]> EnumerateFilterPaddingPaths(string snapLine1Filter, string snapLine2Filter)
            {
                // happy paths
                yield return new object[] { snapLine1Filter, snapLine2Filter, true };

                // unhappy paths
                foreach (var margin in s_Margins.Except(new[] { snapLine2Filter }))
                {
                    yield return new object[] { snapLine1Filter, margin, false };
                }
            }
        }

        [Theory]
        [MemberData(nameof(SnapLineFilter_Padding_TestData))]
        public void SnapLine_ShouldSnap_should_return_expected_if_types_equal_and_filter_contains_padding(string snapLine1Filter, string snapLine2Filter, bool expected)
        {
            var snapLine1 = new SnapLine(SnapLineType.Top, DefaultOffset, snapLine1Filter, DefaultPriority);
            var snapLine2 = new SnapLine(SnapLineType.Top, DefaultOffset, snapLine2Filter, SnapLinePriority.Low);

            Assert.Equal(expected, SnapLine.ShouldSnap(snapLine1, snapLine2));
        }

        [Fact]
        public void SnapLine_ShouldSnap_should_return_true_if_types_equal_and_filters_match()
        {
            var snapLine1 = new SnapLine(SnapLineType.Top, DefaultOffset, "custom filter", DefaultPriority);
            var snapLine2 = new SnapLine(SnapLineType.Top, DefaultOffset, "custom filter", SnapLinePriority.Low);

            Assert.True(SnapLine.ShouldSnap(snapLine1, snapLine2));
        }

        [Fact]
        public void SnapLine_ShouldSnap_should_return_false_if_types_equal_and_filters_not_match()
        {
            var snapLine1 = new SnapLine(SnapLineType.Top, DefaultOffset, "custom filter", DefaultPriority);
            var snapLine2 = new SnapLine(SnapLineType.Top, DefaultOffset, "another filter", SnapLinePriority.Low);

            Assert.False(SnapLine.ShouldSnap(snapLine1, snapLine2));
        }

        [Theory]
        [InlineData(null, "SnapLine: {type = Baseline, offset = 123, priority = Medium, filter = <null>}")]
        [InlineData(DefaultFilter, "SnapLine: {type = Baseline, offset = 123, priority = Medium, filter = filter}")]
        public void SnapLine_ToString(string filter, string expected)
        {
            var snapLine = new SnapLine(SnapLineType.Baseline, DefaultOffset, filter, DefaultPriority);

            Assert.Equal(expected, snapLine.ToString());
        }
    }
}
