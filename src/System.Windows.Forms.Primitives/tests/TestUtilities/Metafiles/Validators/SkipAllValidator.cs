// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Metafiles;

internal sealed class SkipAllValidator : IEmfValidator
{
    public static IEmfValidator Instance { get; } = new SkipAllValidator();

    public bool ShouldValidate(ENHANCED_METAFILE_RECORD_TYPE recordType) => true;

    public void Validate(ref EmfRecord record, DeviceContextState state, out bool complete)
    {
        // Always want to remain in scope.
        complete = false;
    }

    // We don't require any more records to "pass"
    public bool FailIfIncomplete => false;
}
