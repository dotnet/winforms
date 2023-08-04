﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Mshtml
    {
        [ComImport]
        [Guid("FECEAAA2-8405-11CF-8BA1-00AA00476DA6")]
        [InterfaceType(ComInterfaceType.InterfaceIsDual)]
        public interface IOmHistory
        {
            short GetLength();
            void Back();
            void Forward();
            void Go([In] ref object pvargdistance);
        }
    }
}
