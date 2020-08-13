// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Design.Tests
{
    public class ColumnHeaderCollectionEditorTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void ColumnHeaderCollectionEditor_Ctor_Default()
        {
            var editor = new ColumnHeaderCollectionEditor(typeof(string));
            Assert.False(editor.IsDropDownResizable);
        }

        [Fact]
        public void ColumnHeaderCollectionEditor_EditValue_ReturnsValue()
        {
            var editor = new ColumnHeaderCollectionEditor(typeof(string));
            string[] value = new string[] { "asdf", "qwer", "zxcv" };

            Assert.Same(value, editor.EditValue(null, value));
        }
    }
}
