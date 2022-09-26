// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [DllImport(Libraries.Ole32)]
        private static extern HRESULT DoDragDrop(
            IntPtr pDataObj,
            IntPtr pDropSource,
            DROPEFFECT dwOKEffects,
            out DROPEFFECT pdwEffect);

        public static HRESULT DoDragDrop(
            IDataObject pDataObj,
            IDropSource pDropSource,
            DROPEFFECT dwOKEffects,
            out DROPEFFECT pdwEffect)
        {
            var result = WinFormsComWrappers.Instance.TryGetComPointer(pDataObj, IID.IDataObject, out var dataObjectPtr);
            if (result.Failed)
            {
                pdwEffect = DROPEFFECT.NONE;
                return result;
            }

            result = WinFormsComWrappers.Instance.TryGetComPointer(pDropSource, IID.IDropSource, out var dropSourcePtr);
            if (result.Failed)
            {
                Marshal.Release(dataObjectPtr);
                pdwEffect = DROPEFFECT.NONE;
                return result;
            }

            result = DoDragDrop(dataObjectPtr, dropSourcePtr, dwOKEffects, out pdwEffect);
            return result;
        }
    }
}
