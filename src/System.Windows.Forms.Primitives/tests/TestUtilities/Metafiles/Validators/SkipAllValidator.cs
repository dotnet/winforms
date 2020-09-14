// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using static Interop;

namespace System.Windows.Forms.Metafiles
{
    internal sealed class SkipAllValidator : IEmfValidator
    {
        public static IEmfValidator Instance = new SkipAllValidator();

        public bool ShouldValidate(Gdi32.EMR recordType) => true;

        public void Validate(ref EmfRecord record, DeviceContextState state, out bool complete)
        {
            // Always want to remain in scope.
            complete = false;
        }

        // We don't require any more records to "pass"
        public bool FailIfIncomplete => false;
    }
}
