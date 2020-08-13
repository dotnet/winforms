// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing.Design;
using System.Windows.Forms.Design.Editors;
using Xunit;

namespace System.Windows.Forms.Design.Tests
{
    public class DataGridViewCellStyleEditorTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void DataGridViewCellStyleEditor_Ctor_Default()
        {
            var editor = new DataGridViewCellStyleEditor();
            Assert.False(editor.IsDropDownResizable);
        }

        [Fact]
        public void DataGridViewCellStyleEditor_GetEditStyle_ReturnsModal()
        {
            var editor = new DataGridViewCellStyleEditor();
            Assert.Equal(UITypeEditorEditStyle.Modal, editor.GetEditStyle(null));
        }
    }
}
