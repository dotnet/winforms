// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using Xunit;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    internal class BackgroundModeValidator : IStateValidator
    {
        private readonly Gdi32.BKMODE _backgroundMode;
        public BackgroundModeValidator(Gdi32.BKMODE backgroundMode) => _backgroundMode = backgroundMode;
        public void Validate(DeviceContextState state) => Assert.Equal(_backgroundMode, state.BackgroundMode);
    }
}
