// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using static Interop;

namespace System.Windows.Forms.Metafiles
{
    internal sealed class RepeatValidator : IEmfValidator
    {
        private readonly IEmfValidator _validator;
        private int _count;

        public RepeatValidator(IEmfValidator validator, int count)
        {
            _validator = validator;
            _count = count;
        }

        public bool ShouldValidate(Gdi32.EMR recordType) => _validator.ShouldValidate(recordType);

        public void Validate(ref EmfRecord record, DeviceContextState state, out bool complete)
        {
            _validator.Validate(ref record, state, out _);
            _count--;
            complete = _count > 0;
        }
    }
}
