// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [DllImport(Libraries.Ole32, ExactSpelling = true)]
        private static extern HRESULT DoDragDrop(
            IDataObject pDataObj,
            IntPtr pDropSource,
            DROPEFFECT dwOKEffects,
            out DROPEFFECT pdwEffect);

        public static HRESULT DoDragDrop(
            IDataObject pDataObj,
            IDropSource pDropSource,
            DROPEFFECT dwOKEffects,
            out DROPEFFECT pdwEffect)
        {
            var result = WinFormsComWrappers.Instance.TryGetComPointer(pDropSource, IID.IDropSource, out var dropSourcePtr);
            if (result.Failed())
            {
                pdwEffect = DROPEFFECT.NONE;
                return result;
            }

            result = DoDragDrop(pDataObj, dropSourcePtr, dwOKEffects, out pdwEffect);
            return result;
        }
    }
}
