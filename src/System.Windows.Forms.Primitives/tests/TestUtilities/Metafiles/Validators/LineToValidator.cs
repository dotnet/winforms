// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Drawing;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    internal sealed class LineToValidator : Validator
    {
        private readonly Point _from;
        private readonly Point _to;

        public LineToValidator(
            Point from,
            Point to,
            params IStateValidator[] stateValidators) : base(stateValidators)
        {
            _from = from;
            _to = to;
        }

        public override bool ShouldValidate(Gdi32.EMR recordType) => recordType == Gdi32.EMR.LINETO;

        public override unsafe void Validate(ref EmfRecord record, DeviceContextState state, out bool complete)
        {
            base.Validate(ref record, state, out _);

            // We're only checking one LineTo record, so this call completes our work.
            complete = true;

            EMRPOINTRECORD* lineTo = record.LineToRecord;
            Assert.Equal(_from, state.BrushOrigin);
            Assert.Equal(_to, lineTo->point);
        }
    }
}
