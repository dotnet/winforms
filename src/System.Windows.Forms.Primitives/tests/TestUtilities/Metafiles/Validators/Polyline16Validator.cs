// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Drawing;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    internal class Polyline16Validator : Poly16Validator
    {
        /// <inheritdoc/>
        public Polyline16Validator(
            RECT? bounds,
            Point[]? points,
            params IStateValidator[] stateValidators) : base(
                bounds,
                points,
                stateValidators)
        {
        }

        public override bool ShouldValidate(Gdi32.EMR recordType) => recordType == Gdi32.EMR.POLYLINE16;

        public override unsafe void Validate(ref EmfRecord record, DeviceContextState state, out bool complete)
        {
            base.Validate(ref record, state, out _);

            // We're only checking one Polyline16 record, so this call completes our work.
            complete = true;

            Validate(record.Polyline16Record);
        }
    }
}
