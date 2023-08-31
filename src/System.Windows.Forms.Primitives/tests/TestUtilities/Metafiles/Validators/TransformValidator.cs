// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Numerics;

namespace System.Windows.Forms.Metafiles;

internal class TransformValidator : IStateValidator
{
    private readonly Matrix3x2 _transform;
    public TransformValidator(Matrix3x2 transform) => _transform = transform;
    public void Validate(DeviceContextState state) => Assert.Equal(_transform, state.Transform);
}
