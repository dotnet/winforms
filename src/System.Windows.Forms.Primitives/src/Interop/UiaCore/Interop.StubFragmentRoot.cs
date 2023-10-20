// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;
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

            Windows.Win32.UI.Accessibility.ProviderOptions IRawElementProviderSimple.Interface.ProviderOptions => default;

            unsafe HRESULT IRawElementProviderSimple.Interface.GetPatternProvider(UIA_PATTERN_ID patternId, IUnknown** pRetVal)
            {
                if (pRetVal is null)
                {
                    return HRESULT.E_POINTER;
                }

                *pRetVal = default;
                return HRESULT.S_OK;
            }

            unsafe HRESULT IRawElementProviderSimple.Interface.GetPropertyValue(UIA_PROPERTY_ID propertyId, VARIANT* pRetVal)
            {
                if (pRetVal is null)
                {
                    return HRESULT.E_POINTER;
                }

                *pRetVal = default;
                return HRESULT.S_OK;
            }

            unsafe IRawElementProviderSimple* IRawElementProviderSimple.Interface.HostRawElementProvider => default;
        }
    }
}
