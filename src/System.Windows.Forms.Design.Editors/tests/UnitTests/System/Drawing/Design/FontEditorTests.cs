// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Windows.Forms.Design;
using Moq;
using Xunit;

namespace System.Drawing.Design.Tests
{
    public class FontEditorTests
    {
        public static IEnumerable<object[]> EditValue_InvalidProvider_TestData()
        {
            yield return new object[] { null };

            var nullServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            nullServiceProviderMock
                .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
                .Returns(null);
            yield return new object[] { nullServiceProviderMock.Object };

            var invalidServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
            invalidServiceProviderMock
                .Setup(p => p.GetService(typeof(IWindowsFormsEditorService)))
                .Returns(new object());
            yield return new object[] { invalidServiceProviderMock.Object };
        }

        [Theory]
        [MemberData(nameof(EditValue_InvalidProvider_TestData))]
        public void EditValue_InvalidProvider_ReturnsValue(IServiceProvider provider)
        {
            var editor = new FontEditor();
            var value = new object();
            Assert.Same(value, editor.EditValue(null, provider, value));
        }

        [Fact]
        public void GetEditStyle_Invoke_ReturnsModal()
        {
            var editor = new FontEditor();
            Assert.Equal(UITypeEditorEditStyle.Modal, editor.GetEditStyle(null));
        }
    }
}
