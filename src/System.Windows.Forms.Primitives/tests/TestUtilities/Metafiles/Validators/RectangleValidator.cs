// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Drawing;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    internal sealed class RectangleValidator : Validator
    {
        private readonly Rectangle _bounds;

        public RectangleValidator(
            RECT bounds,
            params IStateValidator[] stateValidators) : base (stateValidators)
        {
            _bounds = bounds;
        }

        public override bool ShouldValidate(Gdi32.EMR recordType) => recordType == Gdi32.EMR.RECTANGLE;

        public override unsafe void Validate(ref EmfRecord record, DeviceContextState state, out bool complete)
        {
            base.Validate(ref record, state, out _);

            // We're only checking one Rectangle record, so this call completes our work.
            complete = true;

            EMRRECTRECORD* rectangle = record.RectangleRecord;
            Assert.Equal(_bounds, (Rectangle)rectangle->rect);
        }
    }
}
