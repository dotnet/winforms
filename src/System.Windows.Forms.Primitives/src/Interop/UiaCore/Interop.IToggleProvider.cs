// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using UIA = Windows.Win32.UI.Accessibility;

internal static partial class Interop
{
    internal static partial class UiaCore
    {
        [ComImport]
        [Guid("56D00BD0-C4F4-433C-A836-1A52A57E0892")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IToggleProvider
        {
            void Toggle();

            /// <summary>
            ///  Indicates an element's current on or off state
            /// </summary>
            UIA.ToggleState ToggleState { get; }
        }
    }
}
