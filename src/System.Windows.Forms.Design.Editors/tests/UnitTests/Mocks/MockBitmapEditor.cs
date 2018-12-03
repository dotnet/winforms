// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Design;
using System.IO;

namespace System.Windows.Forms.Design.Editors.Tests
{
    internal class MockBitmapEditor : BitmapEditor
    {
        internal string GetBitmapEditorFileDialogDescription()
        {
            return GetFileDialogDescription();
        }

        internal string[] GetBitmapExtensions()
        {
            return GetExtensions();
        }

        internal Image LoadBitmapFromStream(Stream stream)
        {
            return LoadFromStream(stream);
        }

        internal List<string> GetExpectedBitmapExtensions()
        {
            return BitmapExtensions;
        }
    }
}
