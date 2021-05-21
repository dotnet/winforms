// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using Xunit;

namespace System.Windows.Forms.Metafiles
{
    internal class FontFaceNameValidator : IStateValidator
    {
        private readonly string _fontFaceName;

        /// <param name="fontFaceName">The font face name to validate.</param>
        public FontFaceNameValidator(string fontFaceName) => _fontFaceName = fontFaceName;

        public void Validate(DeviceContextState state) => Assert.Equal(_fontFaceName, state.SelectedFont.FaceName.ToString());
    }
}
