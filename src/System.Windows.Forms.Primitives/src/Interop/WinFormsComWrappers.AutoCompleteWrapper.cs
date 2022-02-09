// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal class AutoCompleteWrapper
        {
            private IntPtr _wrappedInstance;

            public AutoCompleteWrapper(IntPtr wrappedInstance)
            {
                _wrappedInstance = wrappedInstance.OrThrowIfZero();
            }

            internal IntPtr Instance => _wrappedInstance;

            public void Dispose()
            {
                Marshal.Release(_wrappedInstance);
                _wrappedInstance = IntPtr.Zero;
            }

            public HRESULT Init(IntPtr hwndEdit, IEnumString punkACL, IntPtr pwszRegKeyPath, IntPtr pwszQuickComplete)
            {
                var punkACLPtr = WinFormsComWrappers.Instance.GetOrCreateComInterfaceForObject(punkACL, CreateComInterfaceFlags.None);
                return ((delegate* unmanaged<IntPtr, IntPtr, IntPtr, IntPtr, IntPtr, HRESULT>)(*(*(void***)_wrappedInstance + 3)))
                        (_wrappedInstance, hwndEdit, punkACLPtr, pwszRegKeyPath, pwszQuickComplete);
            }

            public HRESULT SetOptions(Shell32.AUTOCOMPLETEOPTIONS dwFlag)
            {
                return ((delegate* unmanaged<IntPtr, Shell32.AUTOCOMPLETEOPTIONS, HRESULT>)(*(*(void***)_wrappedInstance + 5)))
                    (_wrappedInstance, dwFlag);
            }
        }
    }
}
