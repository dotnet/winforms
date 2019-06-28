// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Moq;
using Xunit;

namespace System.Windows.Forms.Design.Editors.Tests.System.Drawing.Design
{
    public class FontNameEditorTests
    {
        private readonly FontNameEditor _fontNameEditor;
        private readonly ITypeDescriptorContext _typeDescriptorContext;

        public FontNameEditorTests()
        {
            _fontNameEditor = new FontNameEditor();
            _typeDescriptorContext = new Mock<ITypeDescriptorContext>(MockBehavior.Strict).Object;
        }

        [Fact]
        public void FontNameEditor_Constructor()
        {
            Assert.NotNull(_fontNameEditor);
        }

        [Fact]
        public void FontNameEditor_Getters()
        {
            Assert.Equal(UITypeEditorEditStyle.None, _fontNameEditor.GetEditStyle(_typeDescriptorContext));
            Assert.True(_fontNameEditor.GetPaintValueSupported(_typeDescriptorContext));
        }

        [Fact]
        public void FontNameEditor_EditValue()
        {
            var provider = new Mock<IServiceProvider>(MockBehavior.Default);
            provider.Setup(s => s.GetService(typeof(Type))).Returns(null);
            var value = "this is the value";

            Assert.Equal(value, _fontNameEditor.EditValue(_typeDescriptorContext, provider.Object, value));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        public void FontNameEditor_PaintValue_ReturnsEarly_InvalidPaintValueEventArgsValue(string fontName)
        {
            PaintValueEventArgs e;
            using (var bitmap = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    e = new PaintValueEventArgs(_typeDescriptorContext, fontName, g, Rectangle.Empty);
                }
            }

            // assert by the virtue of calling the method
            // if the impementation is incorrect, having disposed of the Graphics object
            // we would received an AE attempting to call e.Graphics.FillRectangle()
            _fontNameEditor.PaintValue(e);
        }
    }
}
