// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests
{
    public class ToolStripTests : ControlTestBase
    {
        private readonly ImageList _sharedImageList;

        public ToolStripTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
            _sharedImageList = new ImageList();
            _sharedImageList.Images.Add("square", Image.FromFile("./Resources/image.png"));
        }

        // Regression test for https://github.com/dotnet/winforms/issues/6881
        [WinFormsFact]
        public void ToolStrip_shared_imagelist_should_not_get_disposed_when_toolstrip_does()
        {
            // establish a scope
            {
                using Form f1 = new();
                using ToolStripButton toolStripButton = new()
                {
                    ImageKey = "square",
                    ToolTipText = "Random Button"
                };
                using ToolStrip toolStrip = new()
                {
                    ImageList = _sharedImageList
                };
                toolStrip.Items.Add(toolStripButton);

                f1.Controls.Add(toolStrip);

                f1.Show();

                Assert.NotNull(_sharedImageList);
                Assert.False(_sharedImageList.IsDisposed);
                Assert.Single(_sharedImageList.Images);

                f1.Close();

                Assert.NotNull(_sharedImageList);
                Assert.False(_sharedImageList.IsDisposed);
                Assert.Single(_sharedImageList.Images);
            }

            // establish another scope
            {
                using Form f2 = new();
                using ToolStripButton toolStripButton = new()
                {
                    ImageKey = "square",
                    ToolTipText = "Random Button"
                };
                using ToolStrip toolStrip = new()
                {
                    ImageList = _sharedImageList
                };
                toolStrip.Items.Add(toolStripButton);

                f2.Controls.Add(toolStrip);

                Assert.NotNull(_sharedImageList);
                Assert.False(_sharedImageList.IsDisposed);
                Assert.Single(_sharedImageList.Images);

                f2.Close();

                Assert.NotNull(_sharedImageList);
                Assert.False(_sharedImageList.IsDisposed);
                Assert.Single(_sharedImageList.Images);
            }
        }
    }
}
