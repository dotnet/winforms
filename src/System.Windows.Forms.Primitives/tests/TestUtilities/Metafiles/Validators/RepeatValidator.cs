// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Metafiles;

internal sealed class RepeatValidator : IEmfValidator
{
    private readonly IEmfValidator _validator;
    private int _count;

    public RepeatValidator(IEmfValidator validator, int count)
    {
        _validator = validator;
        _count = count;
    }

    public bool ShouldValidate(ENHANCED_METAFILE_RECORD_TYPE recordType) => _validator.ShouldValidate(recordType);

    public void Validate(ref EmfRecord record, DeviceContextState state, out bool complete)
    {
        if (_count <= 0)
            throw new InvalidOperationException();

        _validator.Validate(ref record, state, out _);
        _count--;
        complete = _count == 0;
    }
}
