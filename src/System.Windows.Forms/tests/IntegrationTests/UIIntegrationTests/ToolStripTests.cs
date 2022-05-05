﻿// Licensed to the .NET Foundation under one or more agreements.
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
            ShowForm(_sharedImageList);
            ShowForm(_sharedImageList);

            return;

            static void ShowForm(ImageList sharedImageList)
            {
                using Form form = new();
                using ToolStripButton toolStripButton = new()
                {
                    ImageKey = "square",
                    ToolTipText = "Random Button"
                };
                using ToolStrip toolStrip = new()
                {
                    ImageList = sharedImageList
                };
                toolStrip.Items.Add(toolStripButton);

                form.Controls.Add(toolStrip);

                form.Show();

                Assert.NotNull(sharedImageList);
#if DEBUG
                Assert.False(sharedImageList.IsDisposed);
#endif
                Assert.Single(sharedImageList.Images);

                form.Close();

                Assert.NotNull(sharedImageList);
#if DEBUG
                Assert.False(sharedImageList.IsDisposed);
#endif
                Assert.Single(sharedImageList.Images);
            }
        }
    }
}
