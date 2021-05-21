// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Drawing;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    internal abstract class PolyPoly16Validator : StateValidator
    {
        private readonly Rectangle? _bounds;
        private readonly int? _polyCount;

        /// <param name="bounds">Optional bounds to validate.</param>
        /// <param name="polyCount">Number of expected polys.</param>
        /// <param name="stateValidators">Optional device context state validation to perform.</param>
        public PolyPoly16Validator(
            RECT? bounds,
            int? polyCount,
            params IStateValidator[] stateValidators) : base(stateValidators)
        {
            // Full point validation still needs implemented
            _polyCount = polyCount;
            _bounds = bounds;
        }

        protected unsafe void Validate(EMRPOLYPOLY16* poly)
        {
            if (_bounds.HasValue)
            {
                Assert.Equal(_bounds.Value, (Rectangle)poly->rclBounds);
            }

            if (_polyCount.HasValue)
            {
                Assert.Equal(_polyCount.Value, (int)poly->nPolys);
            }
        }
    }
}
