// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using Xunit;

namespace System.Windows.Forms.Metafiles
{
    internal class PenStyleValidator : IStateValidator
    {
        private readonly PEN_STYLE _penStyle;
        public PenStyleValidator(PEN_STYLE penStyle) => _penStyle = penStyle;
        public void Validate(DeviceContextState state) => Assert.Equal(_penStyle, state.SelectedPen.elpPenStyle);
    }
}
