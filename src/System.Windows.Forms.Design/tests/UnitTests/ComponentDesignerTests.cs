// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;
using Xunit;

namespace System.Windows.Forms.Design.Tests
{
    public class ComponentDesignerTests
    {
        [Fact]
        public void ComponentDesigner_Constructor()
        {
            var underTest = new ComponentDesigner();
            Assert.NotNull(underTest);
            Assert.NotNull(underTest.Verbs);
            Assert.Null(underTest.Component);
            Assert.NotNull(underTest.AssociatedComponents);
            Assert.NotNull(underTest.ActionLists);
        }

        [Fact]
        public void ComponentDesigner_Initialize()
        {
            var underTest = new ComponentDesigner();
            var button = new Button();

            underTest.Initialize(button);
            Assert.NotNull(underTest.Component);
            Assert.Equal(button, underTest.Component);
            Assert.NotNull(underTest.AssociatedComponents);
        }
    }
}
