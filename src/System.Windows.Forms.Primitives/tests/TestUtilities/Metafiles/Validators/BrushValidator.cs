// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Metafiles
{
    internal class BrushValidator : IStateValidator
    {
        private readonly Color _brushColor;
        private readonly BRUSH_STYLE _brushStyle;

        public BrushValidator(Color brushColor, BRUSH_STYLE brushStyle)
        {
            _brushColor = brushColor;
            _brushStyle = brushStyle;
        }

        public void Validate(DeviceContextState state)
        {
            Assert.Equal((COLORREF)_brushColor, state.SelectedBrush.lbColor);
            Assert.Equal(_brushStyle, state.SelectedBrush.lbStyle);
        }
    }
}
