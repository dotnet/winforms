// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using static Interop;

namespace System.Windows.Forms.Metafiles
{
    /// <summary>
    ///  Simple delegate wrapping validator.
    /// </summary>
    internal class ActionValidator : IEmfValidator
    {
        private readonly Gdi32.EMR _recordType;
        private readonly ProcessRecordDelegate? _processor;
        private readonly ProcessRecordWithStateDelegate? _processorWithState;

        public ActionValidator(Gdi32.EMR recordType, ProcessRecordDelegate processor)
        {
            _recordType = recordType;
            _processor = processor;
        }

        public ActionValidator(Gdi32.EMR recordType, ProcessRecordWithStateDelegate processor)
        {
            _recordType = recordType;
            _processorWithState = processor;
        }

        public bool ShouldValidate(Gdi32.EMR recordType) => recordType == _recordType;

        public void Validate(ref EmfRecord record, DeviceContextState state, out bool complete)
        {
            complete = true;

            _processor?.Invoke(ref record);
            _processorWithState?.Invoke(ref record, state);
        }
    }
}
