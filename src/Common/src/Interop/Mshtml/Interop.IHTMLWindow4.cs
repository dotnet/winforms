// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Mshtml
    {
        [ComVisible(true)]
        [Guid("3050f6cf-98b5-11cf-bb82-00aa00bdce0b")]
        [InterfaceType(ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLWindow4
        {
            [return: MarshalAs(UnmanagedType.IDispatch)] object CreatePopup([In] ref object reserved);
            [return: MarshalAs(UnmanagedType.Interface)] object frameElement();
        }
    }
}
