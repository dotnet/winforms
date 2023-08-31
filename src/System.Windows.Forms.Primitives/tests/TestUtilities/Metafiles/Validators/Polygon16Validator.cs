// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Metafiles;

internal class Polygon16Validator : Poly16Validator
{
    /// <inheritdoc/>
    public Polygon16Validator(
        RECT? bounds,
        Point[]? points,
        params IStateValidator[] stateValidators) : base(
            bounds,
            points,
            stateValidators)
    {
    }

    public override bool ShouldValidate(ENHANCED_METAFILE_RECORD_TYPE recordType) => recordType == ENHANCED_METAFILE_RECORD_TYPE.EMR_POLYGON16;

    public override unsafe void Validate(ref EmfRecord record, DeviceContextState state, out bool complete)
    {
        base.Validate(ref record, state, out _);

        // We're only checking one Polygon16 record, so this call completes our work.
        complete = true;

        Validate(record.Polygon16Record);
    }
}
