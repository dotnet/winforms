// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using static Interop;

namespace System.Windows.Forms.Metafiles
{
    /// <summary>
    ///  Base <see cref="IEmfValidator"/> that handles optional validation of device context state.
    /// </summary>
    internal abstract class StateValidator : IEmfValidator
    {
        private readonly IStateValidator[] _stateValidators;
        public StateValidator(IStateValidator[] stateValidators) => _stateValidators = stateValidators;
        public abstract bool ShouldValidate(Gdi32.EMR recordType);

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
}
