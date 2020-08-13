// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using WinForms.Common.Tests;
using Xunit;

namespace System.ComponentModel.Design.Tests
{
    public class DesignerActionPropertyItemTests : IClassFixture<ThreadExceptionFixture>
    {
        [Theory]
        [InlineData("memberName", "displayName", "category", "description", "displayName")]
        [InlineData("member(&a)Name", "displa(&a)yName", "cate(&a)gory", "descr(&a)iption", "displayName")]
        [InlineData("", "", "", "", "")]
        [InlineData(null, null, null, null, null)]
        public void DesignerActionPropertyItem_Ctor_String_String_String_String(string memberName, string displayName, string category, string description, string expectedDisplayName)
        {
            var item = new DesignerActionPropertyItem(memberName, displayName, category, description);
            Assert.Equal(memberName, item.MemberName);
            Assert.Equal(expectedDisplayName, item.DisplayName);
            Assert.Equal(category, item.Category);
            Assert.Equal(description, item.Description);
            Assert.False(item.AllowAssociate);
            Assert.Empty(item.Properties);
            Assert.Same(item.Properties, item.Properties);
            Assert.IsType<HybridDictionary>(item.Properties);
            Assert.True(item.ShowInSourceView);
            Assert.Null(item.RelatedComponent);
        }

        [Theory]
        [InlineData("memberName", "displayName", "category", "displayName")]
        [InlineData("member(&a)Name", "displa(&a)yName", "cate(&a)gory", "displayName")]
        [InlineData("", "", "", "")]
        [InlineData(null, null, null, null)]
        public void DesignerActionPropertyItem_Ctor_String_String_String(string memberName, string displayName, string category, string expectedDisplayName)
        {
            var item = new DesignerActionPropertyItem(memberName, displayName, category);
            Assert.Equal(memberName, item.MemberName);
            Assert.Equal(expectedDisplayName, item.DisplayName);
            Assert.Equal(category, item.Category);
            Assert.Null(item.Description);
            Assert.False(item.AllowAssociate);
            Assert.Empty(item.Properties);
            Assert.Same(item.Properties, item.Properties);
            Assert.IsType<HybridDictionary>(item.Properties);
            Assert.True(item.ShowInSourceView);
            Assert.Null(item.RelatedComponent);
        }

        [Theory]
        [InlineData("memberName", "displayName", "displayName")]
        [InlineData("member(&a)Name", "displa(&a)yName", "displayName")]
        [InlineData("", "", "")]
        [InlineData(null, null, null)]
        public void DesignerActionPropertyItem_Ctor_String_String(string memberName, string displayName, string expectedDisplayName)
        {
            var item = new DesignerActionPropertyItem(memberName, displayName);
            Assert.Equal(memberName, item.MemberName);
            Assert.Equal(expectedDisplayName, item.DisplayName);
            Assert.Null(item.Category);
            Assert.Null(item.Description);
            Assert.False(item.AllowAssociate);
            Assert.Empty(item.Properties);
            Assert.Same(item.Properties, item.Properties);
            Assert.IsType<HybridDictionary>(item.Properties);
            Assert.True(item.ShowInSourceView);
            Assert.Null(item.RelatedComponent);
        }

        public static IEnumerable<object[]> RelatedComponent_Set_TestData()
        {
            yield return new object[] { new Component() };
            yield return new object[] { null };
        }

        [Theory]
        [MemberData(nameof(RelatedComponent_Set_TestData))]
        public void DesignerActionPropertyItem_RelatedComponent_Set_GetReturnsExpected(IComponent value)
        {
            var item = new DesignerActionPropertyItem("memberName", "displayName", "category", "description")
            {
                RelatedComponent = value
            };
            Assert.Same(value, item.RelatedComponent);

            // Set same.
            item.RelatedComponent = value;
            Assert.Same(value, item.RelatedComponent);
        }
    }
}
