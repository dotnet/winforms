// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Linq;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    internal sealed class SkipTypesValidator : IEmfValidator
    {
        private readonly Gdi32.EMR[] _skipTypes;

        public SkipTypesValidator(params Gdi32.EMR[] skipTypes) => _skipTypes = skipTypes;

        public bool ShouldValidate(Gdi32.EMR recordType) => _skipTypes.Contains(recordType);

        public void Validate(ref EmfRecord record, DeviceContextState state, out bool complete) => complete = true;
    }
}
