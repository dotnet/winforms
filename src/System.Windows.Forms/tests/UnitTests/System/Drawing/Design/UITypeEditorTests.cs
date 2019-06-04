// Licensed to the .NET Foundation under one or more agreements.	
// The .NET Foundation licenses this file to you under the MIT license.	
// See the LICENSE file in the project root for more information.	

using Xunit;

namespace System.Drawing.Design
{
    public class UITypeEditorTests
    {
        [Fact]
        public void UITypeEditor_Ctor_Default()
        {
            var editor = new UITypeEditor();
            Assert.False(editor.IsDropDownResizable);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("value")]
        public void EditValue_Invoke_ReturnsValue(object value)
        {
            var editor = new UITypeEditor();
            Assert.Same(value, editor.EditValue(null, value));
            Assert.Same(value, editor.EditValue(null, null, value));
        }

        [Fact]
        public void GetEditStyle_Invoke_ReturnsNone()
        {
            var editor = new UITypeEditor();
            Assert.Equal(UITypeEditorEditStyle.None, editor.GetEditStyle());
            Assert.Equal(UITypeEditorEditStyle.None, editor.GetEditStyle(null));
        }

        [Fact]
        public void GetPaintValueSupported_Invoke_ReturnsFalse()
        {
            var editor = new UITypeEditor();
            Assert.False(editor.GetPaintValueSupported());
            Assert.False(editor.GetPaintValueSupported(null));
        }

        [Fact]
        public void PaintValue_Invoke_Nop()
        {
            using (var bm = new Bitmap(10, 10))
            using (var graphics = Graphics.FromImage(bm))
            {
                var editor = new UITypeEditor();
                editor.PaintValue(null, graphics, Rectangle.Empty);
                editor.PaintValue(new PaintValueEventArgs(null, null, graphics, Rectangle.Empty));
            }
        }
    }
}
