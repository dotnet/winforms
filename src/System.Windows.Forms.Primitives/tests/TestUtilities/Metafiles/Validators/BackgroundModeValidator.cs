// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using Xunit;

namespace System.Windows.Forms.Metafiles
{
    internal class BackgroundModeValidator : IStateValidator
    {
        private readonly BACKGROUND_MODE _backgroundMode;
        public BackgroundModeValidator(BACKGROUND_MODE backgroundMode) => _backgroundMode = backgroundMode;
        public void Validate(DeviceContextState state) => Assert.Equal(_backgroundMode, state.BackgroundMode);
    }
}
