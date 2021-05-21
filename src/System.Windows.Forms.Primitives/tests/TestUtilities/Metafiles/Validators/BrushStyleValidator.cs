// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using Xunit;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    internal class BrushStyleValidator : IStateValidator
    {
        private readonly Gdi32.BS _brushStyle;
        public BrushStyleValidator(Gdi32.BS brushStyle) => _brushStyle = brushStyle;
        public void Validate(DeviceContextState state) => Assert.Equal(_brushStyle, state.SelectedBrush.lbStyle);
    }
}
