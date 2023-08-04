﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Mshtml
    {
        [ComImport]
        [Guid("626FC520-A41E-11cf-A731-00A0C9082637")]
        [InterfaceType(ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLDocument
        {
            [return: MarshalAs(UnmanagedType.IDispatch)] object GetScript();
        }
    }
}
