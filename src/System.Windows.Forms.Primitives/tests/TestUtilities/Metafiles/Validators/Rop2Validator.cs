// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using Xunit;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    internal class Rop2Validator : IStateValidator
    {
        private readonly Gdi32.R2 _rop2Mode;
        public Rop2Validator(Gdi32.R2 rop2Mode) => _rop2Mode = rop2Mode;
        public void Validate(DeviceContextState state) => Assert.Equal(_rop2Mode, state.Rop2Mode);
    }
}
