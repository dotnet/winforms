// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class UiaCore
    {
        [ComImport]
        [Guid("6278cab1-b556-4a1a-b4e0-418acc523201")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMultipleViewProvider
        {
            string? GetViewName(int viewId);

            void SetCurrentView(int viewId);

            int CurrentView { get; }

            int[]? GetSupportedViews();
        }
    }
}
