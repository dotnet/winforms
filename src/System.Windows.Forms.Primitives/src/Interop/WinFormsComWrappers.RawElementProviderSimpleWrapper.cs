// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Windows.Forms;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal class RawElementProviderSimpleWrapper: UiaCore.IRawElementProviderSimple
        {
            private IntPtr _wrappedInstance;

            public RawElementProviderSimpleWrapper(IntPtr wrappedInstance)
            {
                _wrappedInstance = wrappedInstance.OrThrowIfZero();
            }

            internal IntPtr Instance => _wrappedInstance;

            ~RawElementProviderSimpleWrapper()
            {
                Marshal.Release(_wrappedInstance);
                _wrappedInstance = IntPtr.Zero;
            }

            UiaCore.ProviderOptions UiaCore.IRawElementProviderSimple.ProviderOptions
            {
                get
                {
                    UiaCore.ProviderOptions options = default;
                    ((delegate* unmanaged<IntPtr, UiaCore.ProviderOptions*, HRESULT>)(*(*(void***)_wrappedInstance + 3)))
                        (_wrappedInstance, &options).ThrowIfFailed();
                    return options;
                }
            }

            object? UiaCore.IRawElementProviderSimple.GetPatternProvider(UiaCore.UIA patternId)
            {
                IntPtr result = default;
                ((delegate* unmanaged<IntPtr, UiaCore.UIA, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 4)))
                    (_wrappedInstance, patternId, &result).ThrowIfFailed();
                return Marshal.GetObjectForIUnknown(result);
            }

            object? UiaCore.IRawElementProviderSimple.GetPropertyValue(UiaCore.UIA propertyId)
            {
                IntPtr result = default;
                ((delegate* unmanaged<IntPtr, UiaCore.UIA, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 5)))
                    (_wrappedInstance, propertyId, &result).ThrowIfFailed();
                return Marshal.GetObjectForIUnknown(result);
            }

            UiaCore.IRawElementProviderSimple? UiaCore.IRawElementProviderSimple.HostRawElementProvider
            {
                get
                {
                    IntPtr result = default;
                    ((delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 6)))
                        (_wrappedInstance, &result).ThrowIfFailed();
                    return result == IntPtr.Zero ? null : (UiaCore.IRawElementProviderSimple)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(result, CreateObjectFlags.Unwrap);
                }
            }
        }
    }
}
