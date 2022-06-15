// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using static Interop;

namespace System.Windows.Forms.Metafiles
{
    internal sealed class SkipToValidator : IEmfValidator
    {
        private readonly IEmfValidator _validator;

        public SkipToValidator(IEmfValidator validator) => _validator = validator;

        public bool ShouldValidate(Gdi32.EMR recordType) => true;

        public void Validate(ref EmfRecord record, DeviceContextState state, out bool complete)
        {
            if (_validator.ShouldValidate(record.Type))
            {
                // Hit our validator, pass through.
                _validator.Validate(ref record, state, out complete);
                return;
            }

            // Still skipping.
            complete = false;
        }
    }
}
