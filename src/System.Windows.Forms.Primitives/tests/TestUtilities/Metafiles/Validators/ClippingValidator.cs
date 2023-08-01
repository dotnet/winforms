// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Metafiles;

internal class ClippingValidator : IStateValidator
{
    private readonly RECT[] _clippingRectangles;
    public ClippingValidator(RECT[] clippingRectangles) => _clippingRectangles = clippingRectangles;

    public void Validate(DeviceContextState state)
        => Assert.Equal(_clippingRectangles, state.ClipRegion);
}
