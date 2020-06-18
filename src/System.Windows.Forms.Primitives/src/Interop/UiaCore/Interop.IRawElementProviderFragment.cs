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
        ///  Implemented by providers to expose elements that are part of
        ///  a structure more than one level deep. For simple one-level
        ///  structures which have no children, IRawElementProviderSimple
        ///  can be used instead.
        ///
        ///  The root node of the fragment must support the IRawElementProviderFragmentRoot
        ///  interface, which is derived from this, and has some additional methods.
        /// </summary>
        [ComImport]
        [Guid("f7063da8-8359-439c-9297-bbc5299a7d87")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IRawElementProviderFragment : IRawElementProviderSimple
        {
            /// <summary>
            ///  Request to return the element in the specified direction
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate</param>
            /// <returns>Returns the element in the specified direction</returns>
            [return: MarshalAs(UnmanagedType.IUnknown)]
            object? /*IRawElementProviderFragment*/ Navigate(NavigateDirection direction);

            /// <summary>
            ///  Gets the runtime ID of an elemenent. This should be unique
            ///  among elements on a desktop.
            /// </summary>
            /// <remarks>
            ///  Proxy implementations should return null for the top-level proxy which
            ///  correpsonds to the HWND; and should return an array which starts
            ///  with AutomationInteropProvider.AppendRuntimeId, followed by values
            ///  which are then unique within that proxy's HWNDs.
            /// </remarks>
            int[]? GetRuntimeId();

            /// <summary>
            ///  Return a bounding rectangle of this element
            /// </summary>
            UiaRect BoundingRectangle { get; }

            /// <summary>
            ///  If this UI is capable of hosting other UI that also supports UIAutomation, and
            ///  the subtree rooted at this element contains such hosted UI fragments, this should return
            ///  an array of those fragments.
            ///
            ///  If this UI does not host other UI, it may return null.
            /// </summary>
            [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UNKNOWN)]
            object[]? /*IRawElementProviderSimple[]*/ GetEmbeddedFragmentRoots();

            /// <summary>
            ///  Request that focus is set to this item.
            ///  The UIAutomation framework will ensure that the UI hosting this fragment is already
            ///  focused before calling this method, so this method should only update its internal
            ///  focus state; it should not attempt to give its own HWND the focus, for example.
            /// </summary>
            void SetFocus();

            /// <summary>
            ///  Return the element that is the root node of this fragment of UI.
            /// </summary>
            IRawElementProviderFragmentRoot? FragmentRoot { get; }
        }
    }
}
