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
    public class DesignerActionItemTests
    {
        [Theory]
        [InlineData("displayName", "category", "description", "displayName")]
        [InlineData("displa(&a)yName", "cate(&a)gory", "descr(&a)iption", "displayName")]
        [InlineData("", "", "", "")]
        [InlineData(null, null, null, null)]
        public void DesignerActionItem_Ctor_String_String_String(string displayName, string category, string description, string expectedDisplayName)
        {
            var item = new SubDesignerActionItem(displayName, category, description);
            Assert.Equal(expectedDisplayName, item.DisplayName);
            Assert.Equal(category, item.Category);
            Assert.Equal(description, item.Description);
            Assert.False(item.AllowAssociate);
            Assert.Empty(item.Properties);
            Assert.Same(item.Properties, item.Properties);
            Assert.IsType<HybridDictionary>(item.Properties);
            Assert.True(item.ShowInSourceView);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DesignerActionItem_AllowAssociate_Set_GetReturnsExpected(bool value)
        {
            var item = new SubDesignerActionItem("displayName", "category", "description")
            {
                AllowAssociate = value
            };
            Assert.Equal(value, item.AllowAssociate);

            // Set same.
            item.AllowAssociate = value;
            Assert.Equal(value, item.AllowAssociate);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DesignerActionItem_ShowInSourceView_Set_GetReturnsExpected(bool value)
        {
            var item = new SubDesignerActionItem("displayName", "category", "description")
            {
                ShowInSourceView = value
            };
            Assert.Equal(value, item.ShowInSourceView);

            // Set same.
            item.ShowInSourceView = value;
            Assert.Equal(value, item.ShowInSourceView);
        }

        private class SubDesignerActionItem : DesignerActionItem
        {
            public SubDesignerActionItem(string displayName, string category, string description) : base(displayName, category, description)
            {
            }
        }
    }
}
