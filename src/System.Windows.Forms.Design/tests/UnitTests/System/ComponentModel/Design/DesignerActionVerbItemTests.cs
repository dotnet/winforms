// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Specialized;
using Xunit;

namespace System.ComponentModel.Design.Tests
{
    public class DesignerActionVerbItemTests
    {
        [Fact]
        public void DesignerActionVerbItem_Ctor_DesignerVerb()
        {
            var verb = new DesignerVerb("text", null);
            var item = new DesignerActionVerbItem(verb);
            Assert.Null(item.MemberName);
            Assert.Equal("text", item.DisplayName);
            Assert.Equal("Verbs", item.Category);
            Assert.Null(item.Description);
            Assert.False(item.IncludeAsDesignerVerb);
            Assert.False(item.AllowAssociate);
            Assert.Empty(item.Properties);
            Assert.Same(item.Properties, item.Properties);
            Assert.IsType<HybridDictionary>(item.Properties);
            Assert.True(item.ShowInSourceView);
            Assert.Null(item.RelatedComponent);
        }

        [Fact]
        public void DesignerActionVerbItem_Ctor_NullVerb_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("verb", () => new DesignerActionVerbItem(null));
        }

        [Fact]
        public void DesignerActionVerbItem_Invoke_Invoke_CallsVerbInvoke()
        {
            DesignerVerb verb = null;
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(verb, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            verb = new DesignerVerb("text", handler);
            var item = new DesignerActionVerbItem(verb);
            item.Invoke();
            Assert.Equal(1, callCount);
        }
    }
}
