// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using static Interop;

namespace System
{
    [ComImport]
    [Guid("0000001d-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal unsafe interface IMallocSpy
    {
        [PreserveSig]
        nuint PreAlloc(
            nuint cbRequest);

        [PreserveSig]
        void* PostAlloc(
            void* pActual);

        [PreserveSig]
        void* PreFree(
            void* pRequest,
            BOOL fSpyed);

        [PreserveSig]
        void PostFree(
            BOOL fSpyed);

        [PreserveSig]
        nuint PreRealloc(
            void* pRequest,
            nuint cbRequest,
            void** ppNewRequest,
            BOOL fSpyed);

        [PreserveSig]
        void* PostRealloc(
            void* pActual,
            BOOL fSpyed);

        [PreserveSig]
        void* PreGetSize(
            void* pRequest,
            BOOL fSpyed);

        [PreserveSig]
        nuint PostGetSize(
            nuint cbActual,
            BOOL fSpyed);

        [PreserveSig]
        void* PreDidAlloc(
            void* pRequest,
            BOOL fSpyed);

        [PreserveSig]
        int PostDidAlloc(
            void* pRequest,
            BOOL fSpyed,
            int fActual);

        [PreserveSig]
        void PreHeapMinimize();

        [PreserveSig]
        void PostHeapMinimize();
    }
}
