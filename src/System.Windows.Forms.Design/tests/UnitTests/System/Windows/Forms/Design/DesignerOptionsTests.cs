// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Design.Tests
{
    public class DesignerOptionsTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void DesignerOptions_Ctor_Default()
        {
            var options = new DesignerOptions();
            Assert.True(options.EnableInSituEditing);
            Assert.Equal(new Size(8, 8), options.GridSize);
            Assert.True(options.ObjectBoundSmartTagAutoShow);
            Assert.True(options.ShowGrid);
            Assert.True(options.SnapToGrid);
            Assert.False(options.UseOptimizedCodeGeneration);
            Assert.False(options.UseSmartTags);
            Assert.False(options.UseSnapLines);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DesignerOptions_EnableInSituEditing_Set_GetReturnsExpected(bool value)
        {
            var options = new DesignerOptions
            {
                EnableInSituEditing = value
            };
            Assert.Equal(value, options.EnableInSituEditing);

            // Set same.
            options.EnableInSituEditing = value;
            Assert.Equal(value, options.EnableInSituEditing);

            // Set different.
            options.EnableInSituEditing = !value;
            Assert.Equal(!value, options.EnableInSituEditing);
        }

        public static IEnumerable<object[]> GridSize_Set_TestData()
        {
            yield return new object[] { new Size(0, 0), new Size(2, 2) };
            yield return new object[] { new Size(0, 2), new Size(2, 2) };
            yield return new object[] { new Size(2, 0), new Size(2, 2) };
            yield return new object[] { new Size(2, 2), new Size(2, 2) };
            yield return new object[] { new Size(200, 200), new Size(200, 200) };
            yield return new object[] { new Size(201, 200), new Size(200, 200) };
            yield return new object[] { new Size(200, 201), new Size(200, 200) };
        }

        [Theory]
        [MemberData(nameof(GridSize_Set_TestData))]
        public void DesignerOptions_GridSize_Set_GetReturnsExpected(Size value, Size expected)
        {
            var options = new DesignerOptions
            {
                GridSize = value
            };
            Assert.Equal(expected, options.GridSize);

            // Set same.
            options.GridSize = value;
            Assert.Equal(expected, options.GridSize);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DesignerOptions_ObjectBoundSmartTagAutoShow_Set_GetReturnsExpected(bool value)
        {
            var options = new DesignerOptions
            {
                ObjectBoundSmartTagAutoShow = value
            };
            Assert.Equal(value, options.ObjectBoundSmartTagAutoShow);

            // Set same.
            options.ObjectBoundSmartTagAutoShow = value;
            Assert.Equal(value, options.ObjectBoundSmartTagAutoShow);

            // Set different.
            options.ObjectBoundSmartTagAutoShow = !value;
            Assert.Equal(!value, options.ObjectBoundSmartTagAutoShow);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DesignerOptions_ShowGrid_Set_GetReturnsExpected(bool value)
        {
            var options = new DesignerOptions
            {
                ShowGrid = value
            };
            Assert.Equal(value, options.ShowGrid);

            // Set same.
            options.ShowGrid = value;
            Assert.Equal(value, options.ShowGrid);

            // Set different.
            options.ShowGrid = !value;
            Assert.Equal(!value, options.ShowGrid);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DesignerOptions_SnapToGrid_Set_GetReturnsExpected(bool value)
        {
            var options = new DesignerOptions
            {
                SnapToGrid = value
            };
            Assert.Equal(value, options.SnapToGrid);

            // Set same.
            options.SnapToGrid = value;
            Assert.Equal(value, options.SnapToGrid);

            // Set different.
            options.SnapToGrid = !value;
            Assert.Equal(!value, options.SnapToGrid);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DesignerOptions_UseOptimizedCodeGeneration_Set_GetReturnsExpected(bool value)
        {
            var options = new DesignerOptions
            {
                UseOptimizedCodeGeneration = value
            };
            Assert.Equal(value, options.UseOptimizedCodeGeneration);

            // Set same.
            options.UseOptimizedCodeGeneration = value;
            Assert.Equal(value, options.UseOptimizedCodeGeneration);

            // Set different.
            options.UseOptimizedCodeGeneration = !value;
            Assert.Equal(!value, options.UseOptimizedCodeGeneration);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DesignerOptions_UseSmartTags_Set_GetReturnsExpected(bool value)
        {
            var options = new DesignerOptions
            {
                UseSmartTags = value
            };
            Assert.Equal(value, options.UseSmartTags);

            // Set same.
            options.UseSmartTags = value;
            Assert.Equal(value, options.UseSmartTags);

            // Set different.
            options.UseSmartTags = !value;
            Assert.Equal(!value, options.UseSmartTags);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DesignerOptions_UseSnapLines_Set_GetReturnsExpected(bool value)
        {
            var options = new DesignerOptions
            {
                UseSnapLines = value
            };
            Assert.Equal(value, options.UseSnapLines);

            // Set same.
            options.UseSnapLines = value;
            Assert.Equal(value, options.UseSnapLines);

            // Set different.
            options.UseSnapLines = !value;
            Assert.Equal(!value, options.UseSnapLines);
        }
    }
}
