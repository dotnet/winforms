// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using Xunit;

namespace System.Windows.Forms.Design.Editors.Tests
{
    public class EnsureEditorsTests
    {
        [Theory]
        [InlineData(typeof(Array))]
        [InlineData(typeof(IList))]
        [InlineData(typeof(ICollection))]
        [InlineData(typeof(byte[]))]
        [InlineData(typeof(string[]))]
        [InlineData(typeof(Bitmap))]
        [InlineData(typeof(Color))]
        [InlineData(typeof(Font))]
        [InlineData(typeof(Image))]
        [InlineData(typeof(Metafile))]
        public void EnsureUITypeEditorForType(Type type)
        {
            var editor = TypeDescriptor.GetEditor(type, typeof(UITypeEditor));
            Assert.NotNull(editor);
        }
    }
}
