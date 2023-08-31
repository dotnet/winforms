// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Metafiles;

internal class PenWidthValidator : IStateValidator
{
    private readonly int _penWidth;
    public PenWidthValidator(int penWidth) => _penWidth = penWidth;
    public void Validate(DeviceContextState state) => Assert.Equal(_penWidth, (int)state.SelectedPen.elpWidth);
}
