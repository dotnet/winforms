// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Metafiles;

internal class PenColorValidator : IStateValidator
{
    private readonly Color _penColor;
    public PenColorValidator(Color penColor) => _penColor = penColor;
    public void Validate(DeviceContextState state) => Assert.Equal((COLORREF)_penColor, state.SelectedPen.elpColor);
}
