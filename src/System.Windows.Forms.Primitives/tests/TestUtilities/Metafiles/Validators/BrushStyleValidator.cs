// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Metafiles;

internal class BrushStyleValidator : IStateValidator
{
    private readonly BRUSH_STYLE _brushStyle;
    public BrushStyleValidator(BRUSH_STYLE brushStyle) => _brushStyle = brushStyle;
    public void Validate(DeviceContextState state) => Assert.Equal(_brushStyle, state.SelectedBrush.lbStyle);
}
