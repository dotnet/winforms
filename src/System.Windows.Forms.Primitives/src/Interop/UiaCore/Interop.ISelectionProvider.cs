// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class UiaCore
    {
        /// <summary>
        ///  The interface representing containers that manage selection.
        /// </summary>
        /// <remarks>
        ///  Client code uses this public interface; server implementers implent the
        ///  ISelectionProvider public interface instead.
        /// </remarks>
        [ComImport]
        [Guid("fb8b03af-3bdf-48d4-bd36-1a65793be168")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ISelectionProvider
        {
            /// <summary>
            ///  Get the currently selected elements
            /// </summary>
            /// <returns>An AutomationElement array containing the currently selected elements</returns>
            [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
            object[]? /* IRawElementProviderSimple */ GetSelection();

            /// <summary>
            ///  Indicates whether the control allows more than one element to be selected
            /// </summary>
            /// <returns>Boolean indicating whether the control allows more than one element to be selected</returns>
            /// <remarks>If this is false, then the control is a single-select ccntrol</remarks>
            BOOL CanSelectMultiple { get; }

            /// <summary>
            ///  Indicates whether the control requires at least one element to be selected
            /// </summary>
            /// <returns>Boolean indicating whether the control requires at least one element to be selected</returns>
            /// <remarks>If this is false, then the control allows all elements to be unselected</remarks>
            BOOL IsSelectionRequired { get; }
        }
    }
}
