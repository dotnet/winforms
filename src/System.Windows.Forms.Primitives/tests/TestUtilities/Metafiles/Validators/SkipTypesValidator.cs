// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Metafiles;

internal sealed class SkipTypesValidator : IEmfValidator
{
    private readonly ENHANCED_METAFILE_RECORD_TYPE[] _skipTypes;

    public SkipTypesValidator(params ENHANCED_METAFILE_RECORD_TYPE[] skipTypes) => _skipTypes = skipTypes;

    public bool ShouldValidate(ENHANCED_METAFILE_RECORD_TYPE recordType) => _skipTypes.Contains(recordType);

    public void Validate(ref EmfRecord record, DeviceContextState state, out bool complete) => complete = true;
}
