// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        internal static partial class ComponentIds
        {
            public const string IID_IServiceProvider = "6D5140C1-7436-11CE-8034-00AA006009FA";
        }

        /// <remarks>
        ///  As the name conflicts with <see cref="System.IServiceProvider"/> this is sometimes
        ///  called IOleServiceProvider, even though the name is actually IServiceProvider.
        ///  <see href="https://docs.microsoft.com/en-us/previous-versions/windows/internet-explorer/ie-developer/platform-apis/cc678966(v=vs.85)">QueryService method (Microsoft Docs)</see>
        /// </remarks>
        [ComImport]
        [Guid(ComponentIds.IID_IServiceProvider)]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IServiceProvider
        {
            [PreserveSig]
            HRESULT QueryService(
                Guid* guidService,
                Guid* riid,
                IntPtr* ppvObject);
        }
    }
}
