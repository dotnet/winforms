// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Design.Tests
{
    public class DesignerActionPropertyItemTests
    {
        [Theory]
        [InlineData("memberName", "displayName", "category", "description")]
        [InlineData("memberName", "displayName", "category", "")]
        [InlineData("memberName", "displayName", "", "")]
        [InlineData("memberName", "", "", "")]
        public void DesignerActionPropertyItem_Constructor(string memberName, string displayName, string category, string description)
        {
            DesignerActionPropertyItem underTest = new DesignerActionPropertyItem(memberName, displayName, category, description);
            Assert.NotNull(underTest);
            Assert.Equal(memberName, underTest.MemberName);
            Assert.Equal(displayName, underTest.DisplayName);
            Assert.Equal(category, underTest.Category);
            Assert.Equal(description, underTest.Description);
        }

        [Fact]
        public void DesignerActionPropertyItem_Constructor2()
        {
            DesignerActionPropertyItem underTest = new DesignerActionPropertyItem("memberName", "displayName", "category");
            Assert.NotNull(underTest);
            Assert.Equal("memberName", underTest.MemberName);
            Assert.Equal("displayName", underTest.DisplayName);
            Assert.Equal("category", underTest.Category);
            Assert.Null(underTest.Description);
        }

        [Fact]
        public void DesignerActionPropertyItem_Constructor3()
        {
            DesignerActionPropertyItem underTest = new DesignerActionPropertyItem("memberName", "displayName");
            Assert.NotNull(underTest);
            Assert.Equal("memberName", underTest.MemberName);
            Assert.Equal("displayName", underTest.DisplayName);
            Assert.Null(underTest.Category);
            Assert.Null(underTest.Description);
        }

        [Fact]
        public void DesignerActionPropertyItem_RelatedComponent_getter_setter()
        {
            DesignerActionPropertyItem underTest = new DesignerActionPropertyItem("name", "displayname");

            Button button = new Button();
            underTest.RelatedComponent = button;
            Assert.Equal(button, underTest.RelatedComponent);

        }
    }
}
