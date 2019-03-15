// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Design.Tests
{
    public class DesignerActionMethodItemTests
    {
        [Theory]
        [InlineData("memberName", "displayName", "category", "description", true)]
        [InlineData("memberName", "displayName", "category", "", true)]
        [InlineData("memberName", "displayName", "", "", false)]
        [InlineData("memberName", "", "", "", false)]
        public void DesignerActionMethodItem_Constructor(string memberName, string displayName, string category, string description, bool includeAsDesignerVerb)
        {
            DesignerActionMethodItem underTest = CreateDesignerActionMethodItem(memberName, displayName, category, description, includeAsDesignerVerb);
            Assert.NotNull(underTest);
            Assert.Equal(memberName, underTest.MemberName);
            Assert.Equal(displayName, underTest.DisplayName);
            Assert.Equal(category, underTest.Category);
            Assert.Equal(description, underTest.Description);
            Assert.Equal(includeAsDesignerVerb, underTest.IncludeAsDesignerVerb);
        }

        [Fact]
        public void DesignerActionMethodItem_Constructor2()
        {
            DesignerActionMethodItem underTest = CreateDesignerActionMethodItem("memberName", "displayName", "category");
            Assert.NotNull(underTest);
            Assert.Equal("memberName", underTest.MemberName);
            Assert.Equal("displayName", underTest.DisplayName);
            Assert.Equal("category", underTest.Category);
            Assert.Null(underTest.Description);
        }

        [Fact]
        public void DesignerActionMethodItem_Constructor3()
        {
            DesignerActionMethodItem underTest = CreateDesignerActionMethodItem("memberName", "displayName");
            Assert.NotNull(underTest);
            Assert.Equal("memberName", underTest.MemberName);
            Assert.Equal("displayName", underTest.DisplayName);
            Assert.Null(underTest.Category);
            Assert.Null(underTest.Description);
        }

        [Fact]
        public void DesignerActionMethodItem_RelatedComponent_getter_setter()
        {
            DesignerActionMethodItem underTest = CreateDesignerActionMethodItem("memberName", "displayName");

            Button button = new Button();
            underTest.RelatedComponent = button;
            Assert.Equal(button, underTest.RelatedComponent);
        }

        private DesignerActionMethodItem CreateDesignerActionMethodItem(string memberName, string displayName, string category, string description, bool includeAsDesignerVerb)
        {
            Button button = new Button();
            DesignerActionList actionList = new DesignerActionList(button);

            if (category == null && description == null)
            {
                return new DesignerActionMethodItem(actionList, memberName, displayName,  includeAsDesignerVerb);
            } 
            else if (description == null)
            {
                return new DesignerActionMethodItem(actionList, memberName, displayName, category, includeAsDesignerVerb);
            } 
            else
            {
                return new DesignerActionMethodItem(actionList, memberName, displayName, category, description, includeAsDesignerVerb);
            }
        }

        private DesignerActionMethodItem CreateDesignerActionMethodItem(string memberName, string displayName, string category, string description)
        {
            return CreateDesignerActionMethodItem(memberName, displayName, category, description, false);
        }

        private DesignerActionMethodItem CreateDesignerActionMethodItem(string memberName, string displayName, string category)
        {
            return CreateDesignerActionMethodItem(memberName, displayName, category, null, false);
        }
        private DesignerActionMethodItem CreateDesignerActionMethodItem(string memberName, string displayName)
        {
            return CreateDesignerActionMethodItem(memberName, displayName, null, null, false);
        }
    }
}
