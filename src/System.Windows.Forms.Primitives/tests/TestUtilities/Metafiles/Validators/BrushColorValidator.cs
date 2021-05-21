// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Drawing;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    internal class BrushColorValidator : IStateValidator
    {
        private readonly Color _brushColor;
        public BrushColorValidator(Color brushColor) => _brushColor = brushColor;
        public void Validate(DeviceContextState state) => Assert.Equal((COLORREF)_brushColor, state.SelectedBrush.lbColor);
    }
}
