﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class OwnerDrawPropertyBagTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void OwnerDrawPropertyBag_Ctor_Default()
        {
            var treeView = new SubTreeView();
            OwnerDrawPropertyBag bag = treeView.GetItemRenderStyles(null, 0);
            Assert.Equal(Color.Empty, bag.BackColor);
            Assert.Null(bag.Font);
            Assert.Equal(Color.Empty, bag.ForeColor);
            Assert.True(bag.IsEmpty());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
        public void OwnerDrawPropertyBag_BackColor_Set_GetReturnsExpected(Color value)
        {
            var treeView = new SubTreeView();
            OwnerDrawPropertyBag bag = treeView.GetItemRenderStyles(null, 0);

            bag.BackColor = value;
            Assert.Equal(value, bag.BackColor);
            Assert.Equal(value.IsEmpty, bag.IsEmpty());

            // Set same.
            bag.BackColor = value;
            Assert.Equal(value, bag.BackColor);
            Assert.Equal(value.IsEmpty, bag.IsEmpty());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetFontTheoryData))]
        public void OwnerDrawPropertyBag_Font_Set_GetReturnsExpected(Font value)
        {
            var treeView = new SubTreeView();
            OwnerDrawPropertyBag bag = treeView.GetItemRenderStyles(null, 0);

            bag.Font = value;
            Assert.Same(value, bag.Font);
            Assert.Equal(value == null, bag.IsEmpty());

            // Set same.
            bag.Font = value;
            Assert.Same(value, bag.Font);
            Assert.Equal(value == null, bag.IsEmpty());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
        public void OwnerDrawPropertyBag_ForeColor_Set_GetReturnsExpected(Color value)
        {
            var treeView = new SubTreeView();
            OwnerDrawPropertyBag bag = treeView.GetItemRenderStyles(null, 0);

            bag.ForeColor = value;
            Assert.Equal(value, bag.ForeColor);
            Assert.Equal(value.IsEmpty, bag.IsEmpty());

            // Set same.
            bag.ForeColor = value;
            Assert.Equal(value, bag.ForeColor);
            Assert.Equal(value.IsEmpty, bag.IsEmpty());
        }

        [Fact]
        public void OwnerDrawPropertyBag_Copy_CustomValue_ReturnsClone()
        {
            var treeView = new SubTreeView();
            OwnerDrawPropertyBag value = treeView.GetItemRenderStyles(null, 0);
            value.BackColor = Color.Blue;
            value.Font = SystemFonts.MenuFont;
            value.ForeColor = Color.Red;

            OwnerDrawPropertyBag bag = OwnerDrawPropertyBag.Copy(value);
            Assert.NotSame(value, bag);
            Assert.Equal(Color.Blue, bag.BackColor);
            Assert.Equal(SystemFonts.MenuFont.Name, bag.Font.Name);
            Assert.Equal(Color.Red, bag.ForeColor);
            Assert.False(bag.IsEmpty());
        }

        [Fact]
        public void OwnerDrawPropertyBag_Copy_NullValue_ReturnsDefault()
        {
            var treeView = new SubTreeView();
            OwnerDrawPropertyBag value = treeView.GetItemRenderStyles(null, 0);
            OwnerDrawPropertyBag bag = OwnerDrawPropertyBag.Copy(value);
            Assert.NotSame(value, bag);
            Assert.Equal(Color.Empty, bag.BackColor);
            Assert.Null(bag.Font);
            Assert.Equal(Color.Empty, bag.ForeColor);
            Assert.True(bag.IsEmpty());
        }

        [Fact]
        public void OwnerDrawPropertyBag_Serailize_Deserialize_Success()
        {
            var treeView = new SubTreeView();
            OwnerDrawPropertyBag original = treeView.GetItemRenderStyles(null, 0);
            original.BackColor = Color.Blue;
            original.Font = SystemFonts.MenuFont;
            original.ForeColor = Color.Red;

            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, original);

                stream.Position = 0;
                OwnerDrawPropertyBag bag = Assert.IsType<OwnerDrawPropertyBag>(formatter.Deserialize(stream));
                Assert.Equal(Color.Blue, bag.BackColor);
                Assert.Equal(SystemFonts.MenuFont.Name, bag.Font.Name);
                Assert.Equal(Color.Red, bag.ForeColor);
                Assert.False(bag.IsEmpty());
            }
        }

        private class SubTreeView : TreeView
        {
            public new OwnerDrawPropertyBag GetItemRenderStyles(TreeNode node, int state) => base.GetItemRenderStyles(node, state);
        }
    }
}
