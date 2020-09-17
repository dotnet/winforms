// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using Xunit;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    internal class PenStyleValidator : IStateValidator
    {
        private readonly Gdi32.PS _penStyle;
        public PenStyleValidator(Gdi32.PS penStyle) => _penStyle = penStyle;
        public void Validate(DeviceContextState state) => Assert.Equal(_penStyle, state.SelectedPen.elpPenStyle);
    }
}
