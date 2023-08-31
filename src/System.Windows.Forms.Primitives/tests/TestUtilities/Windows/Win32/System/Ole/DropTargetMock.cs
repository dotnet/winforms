// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;
using Windows.Win32.System.SystemServices;

namespace Windows.Win32.System.Ole;

internal unsafe class DropTargetMock : IDropTarget.Interface, IManagedWrapper<IDropTarget>
{
    public virtual HRESULT DragEnter(IDataObject* pDataObj, MODIFIERKEYS_FLAGS grfKeyState, POINTL pt, DROPEFFECT* pdwEffect)
    {
        throw new NotImplementedException();
    }

    public virtual HRESULT DragOver(MODIFIERKEYS_FLAGS grfKeyState, POINTL pt, DROPEFFECT* pdwEffect)
    {
        throw new NotImplementedException();
    }

    public virtual HRESULT DragLeave()
    {
        throw new NotImplementedException();
    }

    public virtual HRESULT Drop(IDataObject* pDataObj, MODIFIERKEYS_FLAGS grfKeyState, POINTL pt, DROPEFFECT* pdwEffect)
    {
        throw new NotImplementedException();
    }
}
