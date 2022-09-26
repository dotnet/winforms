// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal class ErrorInfoWrapper
        {
            private IntPtr _wrappedInstance;

            public ErrorInfoWrapper(IntPtr wrappedInstance)
            {
                _wrappedInstance = wrappedInstance.OrThrowIfZero();
            }

            internal IntPtr Instance => _wrappedInstance;

            public void Dispose()
            {
                Marshal.Release(_wrappedInstance);
                _wrappedInstance = IntPtr.Zero;
            }

            public bool GetDescription([NotNullWhen(true)] out string? pBstrDescription)
            {
                IntPtr descriptionPtr;
                var result = ((delegate* unmanaged<IntPtr, IntPtr*, HRESULT>)(*(*(void***)_wrappedInstance + 5 /* IErrorInfo.GetDescription */)))
                    (_wrappedInstance, &descriptionPtr);
                if (result.Succeeded)
                {
                    pBstrDescription = Marshal.PtrToStringUni(descriptionPtr);
                    Marshal.FreeBSTR(descriptionPtr);
                }
                else
                {
                    pBstrDescription = null;
                }

                return result.Succeeded;
            }
        }
    }
}
