// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class UiaCore
    {
        [ComImport]
        [Guid("d847d3a5-cab0-4a98-8c32-ecb45c59ad24")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IExpandCollapseProvider
        {
            /// <summary>
            ///  Blocking method that returns after the element has been expanded.
            /// </summary>
            void Expand();

            /// <summary>
            ///  Blocking method that returns after the element has been collapsed.
            /// </summary>
            void Collapse();

            /// <summary>
            ///  Indicates an element's current Collapsed or Expanded state
            /// </summary>
            ExpandCollapseState ExpandCollapseState { get; }
        }
    }
}
