// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class UiaCore
    {
        [DllImport(Libraries.UiaCore, ExactSpelling = true)]
        private static extern HRESULT UiaRaiseStructureChangedEvent(IntPtr pProvider, StructureChangeType structureChangeType, int[] pRuntimeId, int cRuntimeIdLen);

        public static HRESULT UiaRaiseStructureChangedEvent(IRawElementProviderSimple pProvider, StructureChangeType structureChangeType, int[] pRuntimeId, int cRuntimeIdLen)
        {
            var providerPtr = WinFormsComWrappers.Instance.GetComPointer(pProvider, IID.IRawElementProviderSimple);
            return UiaRaiseStructureChangedEvent(providerPtr, structureChangeType, pRuntimeId, cRuntimeIdLen);
        }
    }
}
