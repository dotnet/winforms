// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

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
            object? IRawElementProviderSimple.GetPatternProvider(UIA_PATTERN_ID patternId) => default;
            object? IRawElementProviderSimple.GetPropertyValue(UIA_PROPERTY_ID propertyId) => default;
            IRawElementProviderSimple? IRawElementProviderSimple.HostRawElementProvider => default;
        }
    }
}
