// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;
using System.Windows.Forms.Tests.Dpi;

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

        [ActiveIssue("https://github.com/dotnet/winforms/issues/7579")]
        [WinFormsTheory(Skip = "Lab machines seems not setting thread's Dpi context. See https://github.com/dotnet/winforms/issues/7579")]
        [InlineData(3.5 * DpiHelper.LogicalDpi)]
        public void SplitContainer_Properties_HorizontalSplitter_Scaling(int newDpi)
        {
            // Run tests only on Windows 10 versions that support thread dpi awareness.
            if (!PlatformDetection.IsWindows10Version1803OrGreater)
            {
                return;
            }

            using var form = new Form();
            using SplitContainer splitContainer = new()
            {
                FixedPanel = FixedPanel.Panel1,
                Location = new Drawing.Point(0, 0),
                Margin = new Padding(0),
                Name = "splitContainer2",
                Orientation = Orientation.Horizontal,
                Size = new Drawing.Size(812, 619),
                SplitterDistance = 90,
                SplitterWidth = 2
            };

            form.AutoScaleMode = AutoScaleMode.Dpi;
            form.Controls.Add(splitContainer);
            form.Show();

            DpiMessageHelper.TriggerDpiMessage(User32.WM.DPICHANGED, form, newDpi);

            Assert.Equal((int)(3.5 * 90), splitContainer.SplitterDistance);
            Assert.Equal((int)(3.5 * 2), splitContainer.SplitterWidth);
            Assert.Equal(splitContainer.SplitterDistance, splitContainer.Panel1.Height);
            form.Close();
        }
    }
}
