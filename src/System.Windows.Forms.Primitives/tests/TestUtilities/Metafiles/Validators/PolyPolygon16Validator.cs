﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Metafiles;

internal class PolyPolygon16Validator : PolyPoly16Validator
{
    /// <inheritdoc/>
    public PolyPolygon16Validator(
        RECT? bounds,
        int? polyCount,
        params IStateValidator[] stateValidators) : base(
            bounds,
            polyCount,
            stateValidators)
    {
    }

    public override bool ShouldValidate(ENHANCED_METAFILE_RECORD_TYPE recordType) => recordType == ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYPOLYGON16;

    public override unsafe void Validate(ref EmfRecord record, DeviceContextState state, out bool complete)
    {
        base.Validate(ref record, state, out _);

        // We're only checking one PolyPolygon16 record, so this call completes our work.
        complete = true;

        Validate(record.PolyPolygon16Record);
    }
}
