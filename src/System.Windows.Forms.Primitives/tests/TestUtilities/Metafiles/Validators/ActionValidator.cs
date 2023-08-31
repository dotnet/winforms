// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Metafiles;

/// <summary>
///  Simple delegate wrapping validator.
/// </summary>
internal class ActionValidator : IEmfValidator
{
    private readonly ENHANCED_METAFILE_RECORD_TYPE _recordType;
    private readonly ProcessRecordDelegate? _processor;
    private readonly ProcessRecordWithStateDelegate? _processorWithState;

    public ActionValidator(ENHANCED_METAFILE_RECORD_TYPE recordType, ProcessRecordDelegate processor)
    {
        _recordType = recordType;
        _processor = processor;
    }

    public ActionValidator(ENHANCED_METAFILE_RECORD_TYPE recordType, ProcessRecordWithStateDelegate processor)
    {
        _recordType = recordType;
        _processorWithState = processor;
    }

    public bool ShouldValidate(ENHANCED_METAFILE_RECORD_TYPE recordType) => recordType == _recordType;

    public void Validate(ref EmfRecord record, DeviceContextState state, out bool complete)
    {
        complete = true;

        _processor?.Invoke(ref record);
        _processorWithState?.Invoke(ref record, state);
    }
}
