// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Design.Editors.Tests
{
    public class BitmapEditorTests
    {
        [Fact]
        public void Defaults()
        {
            MockBitmapEditor mockBitmapEditor = new MockBitmapEditor();
            Assert.NotNull(mockBitmapEditor.GetBitmapEditorFileDialogDescription());

            string[] actualExtensions = mockBitmapEditor.GetBitmapExtensions();
            List<string> expectedExtensions = mockBitmapEditor.GetExpectedBitmapExtensions();
            Assert.NotNull(actualExtensions);
            Assert.NotNull(expectedExtensions);
            Assert.Equal(expectedExtensions.Count, actualExtensions.Length);

            foreach(string extension in actualExtensions)
            {
                Assert.Contains(extension, expectedExtensions);
            }
        }
    }
}
