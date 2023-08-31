// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Metafiles;

/// <summary>
///  Base <see cref="IEmfValidator"/> that handles optional validation of device context state.
/// </summary>
internal abstract class StateValidator : IEmfValidator
{
    private readonly IStateValidator[] _stateValidators;
    public StateValidator(IStateValidator[] stateValidators) => _stateValidators = stateValidators;
    public abstract bool ShouldValidate(ENHANCED_METAFILE_RECORD_TYPE recordType);

    public virtual void Validate(ref EmfRecord record, DeviceContextState state, out bool complete)
    {
        complete = false;

        if (_stateValidators is null)
        {
            return;
        }

        foreach (IStateValidator validator in _stateValidators)
        {
            validator.Validate(state);
        }
    }
}
