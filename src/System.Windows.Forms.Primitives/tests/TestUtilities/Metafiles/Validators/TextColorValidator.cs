// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Drawing;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    internal class TextColorValidator : IStateValidator
    {
        private readonly Color _textColor;
        public TextColorValidator(Color textColor) => _textColor = textColor;
        public void Validate(DeviceContextState state) => Assert.Equal((COLORREF)_textColor, state.TextColor);
    }
}
