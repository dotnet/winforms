// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.Layout;
using Xunit;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests
{
    public class AnchorLayoutTests : ControlTestBase
    {
        public AnchorLayoutTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [WinFormsTheory]
        [InlineData(AnchorStyles.Top, 120, 30, 20, 30)]
        [InlineData(AnchorStyles.Left, 20, 180, 20, 30)]
        [InlineData(AnchorStyles.Right, 220, 180, 20, 30)]
        [InlineData(AnchorStyles.Bottom, 120, 330, 20, 30)]
        [InlineData(AnchorStyles.Top | AnchorStyles.Left, 20, 30, 20, 30)]
        [InlineData(AnchorStyles.Top | AnchorStyles.Right, 220, 30, 20, 30)]
        [InlineData(AnchorStyles.Top | AnchorStyles.Bottom, 120, 30, 20, 330)]
        [InlineData(AnchorStyles.Left | AnchorStyles.Right, 20, 180, 220, 30)]
        [InlineData(AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left, 20, 30, 220, 330)]
        public void Control_ResizeAnchoredControls_ParentHanldeCreated_NewAnchorsApplied(AnchorStyles anchors, int expectedX, int expectedY, int expectedWidth, int expectedHeight)
        {
            (Form form, Button button) = GetFormWithAnchoredButton(anchors);
            form.Shown += OnFormShown;

            // Showing the dialog will execute the assertions
            form.ShowDialog();

            button?.Dispose();
            form?.Dispose();
            return;

            void OnFormShown(object? sender, EventArgs e)
            {
                // Resize the form to compute button anchors.
                form.Size = new Size(400, 600);
                Rectangle buttonBounds = button.Bounds;

                Assert.Equal(expectedX, buttonBounds.X);
                Assert.Equal(expectedY, buttonBounds.Y);
                Assert.Equal(expectedWidth, buttonBounds.Width);
                Assert.Equal(expectedHeight, buttonBounds.Height);

                form.Close();
            }
        }

        private static (Form, Button) GetFormWithAnchoredButton(AnchorStyles buttonAnchors)
        {
            Form form = new()
            {
                Size = new Size(200, 300)
            };

            Button button = new()
            {
                Location = new Point(20, 30),
                Size = new Size(20, 30),
                Anchor = buttonAnchors
            };

            DefaultLayout.AnchorInfo anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.Null(anchorInfo);

            form.Controls.Add(button);

            return (form, button);
        }
    }
}
