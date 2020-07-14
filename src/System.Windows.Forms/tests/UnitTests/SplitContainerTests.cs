// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class SplitContainerTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void SplitContainer_Constructor()
        {
            using var sc = new SplitContainer();

            Assert.NotNull(sc);
            Assert.NotNull(sc.Panel1);
            Assert.Equal(sc, sc.Panel1.Owner);
            Assert.NotNull(sc.Panel2);
            Assert.Equal(sc, sc.Panel2.Owner);
            Assert.False(sc.SplitterRectangle.IsEmpty);
        }

        [WinFormsTheory]
        [InlineData(150, 100, 120)]
        [InlineData(150, 100, 80)]
        public void SplitContainer_Panel2MinSize_SetLargerThanPanel2SizeWithVerticalOrientation_SetsSplitterDistance(int width, int height, int panel2MinSize)
        {
            using var sc = new SplitContainer()
            {
                Orientation = Orientation.Vertical,
                Width = width,
                Height = height,
                SplitterDistance = width / 2
            };

            sc.Panel2MinSize = panel2MinSize;

            Assert.Equal(panel2MinSize, sc.Panel2.Width);
        }

        [WinFormsTheory]
        [InlineData(100, 150, 120)]
        [InlineData(100, 150, 80)]
        public void SplitContainer_Panel2MinSize_SetLargerThanPanel2SizeWithHorizontalOrientation_SetsSplitterDistance(int width, int height, int panel2MinSize)
        {
            using var sc = new SplitContainer()
            {
                Orientation = Orientation.Horizontal,
                Width = width,
                Height = height,
                SplitterDistance = height / 2
            };

            sc.Panel2MinSize = panel2MinSize;

            Assert.Equal(panel2MinSize, sc.Panel2.Height);
        }

        [WinFormsTheory]
        [InlineData(Orientation.Vertical, 150, 100, 75, 50)]
        [InlineData(Orientation.Vertical, 300, 100, 150, 120)]
        [InlineData(Orientation.Horizontal, 100, 150, 75, 50)]
        [InlineData(Orientation.Horizontal, 100, 300, 150, 120)]
        public void SplitContainer_Panel2MinSize_SetLessOrEqualPanel2Size_DoesNotSetSplitterDistance(Orientation orientation, int width, int height, int splitterDistance, int panel2MinSize)
        {
            using var sc = new SplitContainer()
            {
                Orientation = orientation,
                Width = width,
                Height = height,
                SplitterDistance = splitterDistance
            };

            sc.Panel2MinSize = panel2MinSize;

            Assert.Equal(splitterDistance, sc.SplitterDistance);
        }
    }
}
