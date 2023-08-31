// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Metafiles;

internal sealed class RectangleValidator : StateValidator
{
    private readonly Rectangle? _bounds;

    /// <param name="bounds">Optional bounds to validate.</param>
    /// <param name="stateValidators">Optional device context state validation to perform.</param>
    public RectangleValidator(
        RECT? bounds,
        params IStateValidator[] stateValidators) : base(stateValidators)
    {
        _bounds = bounds;
    }

    public override bool ShouldValidate(ENHANCED_METAFILE_RECORD_TYPE recordType) => recordType == ENHANCED_METAFILE_RECORD_TYPE.EMR_RECTANGLE;

    public override unsafe void Validate(ref EmfRecord record, DeviceContextState state, out bool complete)
    {
        base.Validate(ref record, state, out _);

        // We're only checking one Rectangle record, so this call completes our work.
        complete = true;

        if (_bounds.HasValue)
        {
            EMRRECTRECORD* rectangle = record.RectangleRecord;
            Assert.Equal(_bounds.Value, (Rectangle)rectangle->rect);
        }
    }
}
