// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Metafiles;

internal abstract class Poly16Validator : StateValidator
{
    private readonly Rectangle? _bounds;
    private readonly Point[]? _points;

    /// <param name="bounds">Optional bounds to validate.</param>
    /// <param name="points">Optional points to validate.</param>
    /// <param name="stateValidators">Optional device context state validation to perform.</param>
    public Poly16Validator(
        RECT? bounds,
        Point[]? points,
        params IStateValidator[] stateValidators) : base(stateValidators)
    {
        _bounds = bounds;
        _points = points;
    }

    protected unsafe void Validate(EMRPOLY16* poly)
    {
        if (_bounds.HasValue)
        {
            Assert.Equal(_bounds.Value, (Rectangle)poly->rclBounds);
        }

        if (_points is not null)
        {
            Assert.Equal(_points.Length, (int)poly->cpts);
            Assert.Equal(_points, poly->points.Transform<POINTS, Point>(p => p));
        }
    }
}
