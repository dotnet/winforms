// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing.Design;
using Moq;
using Xunit;

namespace System.ComponentModel.Design.Tests
{
    public class MultilineStringEditorTests
    {
        [Fact]
        public void MultilineStringEditor_Constructor()
        {
            var underTest = new MultilineStringEditor();
            Assert.NotNull(underTest);
        }

        [Fact]
        public void MultilineStringEditor_Getters()
        {
            var underTest = new MultilineStringEditor();
            var context = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            Assert.Equal(UITypeEditorEditStyle.DropDown, underTest.GetEditStyle(context.Object));
            Assert.False(underTest.GetPaintValueSupported(context.Object));
        }

        [Fact]
        public void MultilineStringEditor_EditValue()
        {
            var underTest = new MultilineStringEditor();
            var context = new Mock<ITypeDescriptorContext>(MockBehavior.Strict);
            var provider = new Mock<IServiceProvider>(MockBehavior.Default);
            provider.Setup(s => s.GetService(typeof(Type))).Returns(null);
            var value = "this is the value";
            Assert.Equal(value, underTest.EditValue(context.Object, provider.Object, value));
        }
    }
}
