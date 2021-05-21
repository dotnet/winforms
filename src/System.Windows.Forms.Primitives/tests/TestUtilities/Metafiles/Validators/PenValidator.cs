// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Drawing;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    internal class PenValidator : IStateValidator
    {
        private readonly int _penWidth;
        private readonly Gdi32.PS _penStyle;
        private readonly Color _penColor;

        public PenValidator(int penWidth, Color penColor, Gdi32.PS penStyle)
        {
            _penWidth = penWidth;
            _penColor = penColor;
            _penStyle = penStyle;
        }

        public void Validate(DeviceContextState state)
        {
            Assert.Equal(_penWidth, (int)state.SelectedPen.elpWidth);
            Assert.Equal((COLORREF)_penColor, state.SelectedPen.elpColor);
            Assert.Equal(_penStyle, state.SelectedPen.elpPenStyle);
        }
    }
}
