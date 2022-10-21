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

        private static (Form, Button) GetFormWithAnchoredButton()
        {
            Form form = new();
            form.Size = new Size(200, 300);
            Button button = new();
            button.Location = new Point(20, 30);
            button.Size = new Size(20, 30);
            button.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

            return (form, button);
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
        public void ResizeAnchoredControlsParent_HanldeCreated_AnchorsApplied(AnchorStyles anchor, int x, int y, int width, int height)
        {
            var (form, button) = GetFormWithAnchoredButton();
            button.Anchor = anchor;
            DefaultLayout.AnchorInfo anchorInfo = DefaultLayout.GetAnchorInfo(button);
            Assert.Null(anchorInfo);
            form.Controls.Add(button);
            form.Shown += OnFormShown;
            form.ShowDialog();
            button?.Dispose();
            form?.Dispose();

            void OnFormShown(object? sender, EventArgs e)
            {
                Form formLocal = (Form)sender!;
                Control button1 = formLocal.Controls[0];

                //Resize Form to compute button anchors.
                formLocal.Size = new Size(400, 600);

                Assert.Equal(x, button1.Bounds.X);
                Assert.Equal(y, button1.Bounds.Y);
                Assert.Equal(width, button1.Bounds.Width);
                Assert.Equal(height, button1.Bounds.Height);                
                form.Close();
            }
        }
    }
}
