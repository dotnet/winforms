// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.Metafiles;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ButtonRenderingTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public unsafe void CaptureButton()
        {
            using Button button = new Button();

            using var emf = EmfScope.Create();
            button.CreateMetafile(emf);

            var types = new List<Gdi32.EMR>();
            var details = new List<string>();
            emf.Enumerate((ref EmfRecord record) =>
            {
                types.Add(record.Type);
                details.Add(record.ToString());
                return true;
            });
        }

        [WinFormsFact]
        public unsafe void CaptureButtonOnForm()
        {
            using Form form = new Form();
            using Button button = new Button();
            form.Controls.Add(button);

            using var emf = EmfScope.Create();
            form.CreateMetafile(emf);

            var types = new List<Gdi32.EMR>();
            var details = new List<string>();
            emf.Enumerate((ref EmfRecord record) =>
            {
                types.Add(record.Type);
                details.Add(record.ToString());
                return true;
            });
        }
    }
}
