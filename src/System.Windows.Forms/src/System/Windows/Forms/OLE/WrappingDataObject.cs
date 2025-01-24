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

        dataObject = null;
        return false;
    }
}
