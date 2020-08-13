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
        ///  Define a Selectable Item (only supported on logical elements that are a
        ///  child of an Element that supports SelectionPattern and is itself selectable).
        ///  This allows for manipulation of Selection from the element itself.
        /// </summary>
        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("2acad808-b2d4-452d-a407-91ff1ad167b2")]
        public interface ISelectionItemProvider
        {
            /// <summary>
            ///  Sets the current element as the selection
            ///  This clears the selection from other elements in the container.
            /// </summary>
            void Select();

            /// <summary>
            ///  Adds current element to selection.
            /// </summary>
            void AddToSelection();

            /// <summary>
            ///  Removes current element from selection.
            /// </summary>
            void RemoveFromSelection();

            /// <summary>
            ///  Check whether an element is selected.
            /// </summary>
            /// <returns>Returns true if the element is selected.</returns>
            BOOL IsSelected { get; }

            /// <summary>
            ///  The logical element that supports the SelectionPattern for this Item.
            /// </summary>
            /// <returns>Returns a IRawElementProviderSimple.</returns>
            IRawElementProviderSimple? SelectionContainer { get; }
        }
    }
}
