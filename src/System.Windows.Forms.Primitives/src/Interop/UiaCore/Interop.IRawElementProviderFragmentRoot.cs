// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class UiaCore
    {
        public class StubFragmentRoot : IRawElementProviderFragmentRoot
        {
            private StubFragmentRoot() { }

            public static StubFragmentRoot Instance { get; } = new();

            object? IRawElementProviderFragmentRoot.ElementProviderFromPoint(double x, double y) => default;
            object? IRawElementProviderFragmentRoot.GetFocus() => default;
            object? IRawElementProviderFragment.Navigate(NavigateDirection direction) => default;
            int[]? IRawElementProviderFragment.GetRuntimeId() => default;
            UiaRect IRawElementProviderFragment.BoundingRectangle => default;
            object[]? IRawElementProviderFragment.GetEmbeddedFragmentRoots() => default;
            void IRawElementProviderFragment.SetFocus() { }
            IRawElementProviderFragmentRoot? IRawElementProviderFragment.FragmentRoot => default;
            ProviderOptions IRawElementProviderSimple.ProviderOptions => default;
            object? IRawElementProviderSimple.GetPatternProvider(UIA patternId) => default;
            object? IRawElementProviderSimple.GetPropertyValue(UIA propertyId) => default;
            IRawElementProviderSimple? IRawElementProviderSimple.HostRawElementProvider => default;
        }

        /// <summary>
        ///  The root element in a fragment of UI must support this interface. Other
        ///  elements in the same fragment need to support the IRawElementProviderFragment
        ///  interface.
        /// </summary>
        [ComImport]
        [Guid("620ce2a5-ab8f-40a9-86cb-de3c75599b58")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IRawElementProviderFragmentRoot : IRawElementProviderFragment
        {
            /// <summary>
            ///  Return the child element at the specified point, if one exists,
            ///  otherwise return this element if the point is on this element,
            ///  otherwise return null.
            /// </summary>
            /// <param name="x">x coordinate of point to check</param>
            /// <param name="y">y coordinate of point to check</param>
            /// <returns>Return the child element at the specified point, if one exists,
            ///  otherwise return this element if the point is on this element,
            ///  otherwise return null.
            /// </returns>
            [return: MarshalAs(UnmanagedType.IUnknown)]
            object? /*IRawElementProviderFragment*/ ElementProviderFromPoint(double x, double y);

            /// <summary>
            ///  Return the element in this fragment which has the keyboard focus,
            /// </summary>
            /// <returns>Return the element in this fragment which has the keyboard focus,
            ///  if any; otherwise return null.</returns>
            [return: MarshalAs(UnmanagedType.IUnknown)]
            object? /*IRawElementProviderFragment*/ GetFocus();
        }
    }
}
