// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class UiaCore
    {
        [DllImport(Libraries.UiaCore, ExactSpelling = true)]
        private static extern HRESULT UiaRaiseAutomationEvent(IntPtr provider, UIA id);

        public static HRESULT UiaRaiseAutomationEvent(IRawElementProviderSimple provider, UIA id)
        {
            var providerPtr = WinFormsComWrappers.Instance.GetComPointer(provider, IID.IRawElementProviderSimple);
            return UiaRaiseAutomationEvent(providerPtr, id);
        }
    }
}
