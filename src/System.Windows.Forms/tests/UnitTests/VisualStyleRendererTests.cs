// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.Tests
{
    public class VisualStyleRendererTests
    {
        [Fact]
        public void VisualStyleRenderer_GetMargins()
        {
            var renderer = new VisualStyleRenderer(VisualStyleElement.Button.PushButton.Normal);

            using (var form = new System.Windows.Forms.Form())
            using (Drawing.Graphics graphics = form.CreateGraphics())
            {
                // GetMargins should not throw an exception.
                // See https://github.com/dotnet/winforms/issues/526.
                renderer.GetMargins(graphics, MarginProperty.SizingMargins);
            }
        }
    }
}
