// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Used to identify and wrap user provided data that isn't already a <see cref="DataObject"/> instance.
/// </summary>
internal sealed class WrappingDataObject : DataObject
{
    private readonly bool _originalIsIDataObject;

    public WrappingDataObject(object data) : base(data)
    {
        // Don't wrap existing DataObject instances.
        Debug.Assert(data is not DataObject);
        _originalIsIDataObject = data is IDataObject;
    }

    internal override bool TryUnwrapUserDataObject([NotNullWhen(true)] out IDataObject? dataObject)
    {
        // We only want to unwrap IDataObject instances from users.
        if (_originalIsIDataObject)
        {
            return base.TryUnwrapUserDataObject(out dataObject);
        }

        // When we hand back our wrapper we're emulating what would happen via the native IDataObject proxy.
        // As the native interface has no concept of "autoConvert", we need to always consider it "true"
        // as our native IDataObject implementation would.
        dataObject = this;
        return true;
    }

    public override string[] GetFormats(bool autoConvert)
        // Always auto convert to emulate native IDataObject behavior.
        => base.GetFormats(autoConvert: true);

    public override bool GetDataPresent(string format, bool autoConvert)
        // Always auto convert to emulate native IDataObject behavior.
        => base.GetDataPresent(format, autoConvert: true);

#pragma warning disable WFDEV005 // Type or member is obsolete
    [Obsolete]
    public override object? GetData(string format, bool autoConvert)
        // Always auto convert to emulate native IDataObject behavior.
        => base.GetData(format, autoConvert: true);
#pragma warning restore WFDEV005 // Type or member is obsolete
}
