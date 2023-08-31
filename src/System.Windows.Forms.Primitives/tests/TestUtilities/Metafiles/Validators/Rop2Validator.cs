// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Metafiles;

internal class Rop2Validator : IStateValidator
{
    private readonly R2_MODE _rop2Mode;
    public Rop2Validator(R2_MODE rop2Mode) => _rop2Mode = rop2Mode;
    public void Validate(DeviceContextState state) => Assert.Equal(_rop2Mode, state.Rop2Mode);
}
