// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Metafiles;

internal sealed class LineToValidator : StateValidator
{
    private readonly Point? _from;
    private readonly Point? _to;

    /// <param name="from">Optional source point to validate.</param>
    /// <param name="to">Optional destination point to validate.</param>
    /// <param name="stateValidators">Optional device context state validation to perform.</param>
    public LineToValidator(
        Point? from,
        Point? to,
        params IStateValidator[] stateValidators) : base(stateValidators)
    {
        _from = from;
        _to = to;
    }

    public override bool ShouldValidate(ENHANCED_METAFILE_RECORD_TYPE recordType) => recordType == ENHANCED_METAFILE_RECORD_TYPE.EMR_LINETO;

    public override unsafe void Validate(ref EmfRecord record, DeviceContextState state, out bool complete)
    {
        base.Validate(ref record, state, out _);

        // We're only checking one LineTo record, so this call completes our work.
        complete = true;

        EMRPOINTRECORD* lineTo = record.LineToRecord;

        if (_from.HasValue)
        {
            Assert.Equal(_from.Value, state.BrushOrigin);
        }

        if (_to.HasValue)
        {
            Assert.Equal(_to.Value, lineTo->point);
        }
    }
}
