// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.ComponentModel.Design.Tests
{
    public class DesignerCommandSetTests
    {
        [Fact]
        public void DesignerCommandSet_Ctor_Default()
        {
            var set = new DesignerCommandSet();
            Assert.Null(set.ActionLists);
            Assert.Null(set.Verbs);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void DesignerCommandSet_GetCommands_Invoke_ReturnsNull(string name)
        {
            var set = new DesignerCommandSet();
            Assert.Null(set.GetCommands(name));
        }

        [Fact]
        public void DesignerCommandSet_Verbs_OverridenGetCommands_ReturnsExpected()
        {
            var collection = new DesignerVerbCollection();
            var mockSet = new Mock<DesignerCommandSet>(MockBehavior.Strict);
            mockSet
                .Setup(s => s.GetCommands("Verbs"))
                .Returns(collection);
            Assert.Same(collection, mockSet.Object.Verbs);
        }

        [Fact]
        public void DesignerCommandSet_Verbs_InvalidOverridenGetCommands_ThrowsInvalidCastException()
        {
            var mockSet = new Mock<DesignerCommandSet>(MockBehavior.Strict);
            mockSet
                .Setup(s => s.GetCommands("Verbs"))
                .Returns(Array.Empty<object>());
            Assert.Throws<InvalidCastException>(() => mockSet.Object.Verbs);
        }

        [Fact]
        public void DesignerCommandSet_ActionLists_OverridenGetCommands_ReturnsExpected()
        {
            var collection = new DesignerActionListCollection();
            var mockSet = new Mock<DesignerCommandSet>(MockBehavior.Strict);
            mockSet
                .Setup(s => s.GetCommands("ActionLists"))
                .Returns(collection);
            Assert.Same(collection, mockSet.Object.ActionLists);
        }

        [Fact]
        public void DesignerCommandSet_ActionLists_InvalidOverridenGetCommands_ThrowsInvalidCastException()
        {
            var mockSet = new Mock<DesignerCommandSet>(MockBehavior.Strict);
            mockSet
                .Setup(s => s.GetCommands("ActionLists"))
                .Returns(Array.Empty<object>());
            Assert.Throws<InvalidCastException>(() => mockSet.Object.ActionLists);
        }
    }
}
