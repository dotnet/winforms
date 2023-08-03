// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.InteropServices;

internal static partial class UiaClient
{
    [ComImport]
    [Guid("D22108AA-8AC5-49A5-837B-37BBB3D7591E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IUIAutomationElement
    {
        int SetFocus();

        void GetRuntimeId();

        void FindFirst();

        void FindAll();

        void FindFirstBuildCache();

        void FindAllBuildCache();

        void BuildUpdatedCache();

        void GetCurrentPropertyValue();

        void GetCurrentPropertyValueEx();

        void GetCachedPropertyValue();

        void GetCachedPropertyValueEx();

        void GetCurrentPatternAs();

        void GetCachedPatternAs();

        void GetCurrentPattern();

        void GetCachedPattern();

        void GetCachedParent();

        void GetCachedChildren();

        void get_CurrentProcessId();

        void get_CurrentControlType();

        void get_CurrentLocalizedControlType();

        HRESULT get_CurrentName(out BSTR retVal);

        void get_CurrentAcceleratorKey();

        void get_CurrentAccessKey();

        void get_CurrentHasKeyboardFocus();

        void get_CurrentIsKeyboardFocusable();

        void get_CurrentIsEnabled();

        void get_CurrentAutomationId();

        void get_CurrentClassName();

        void get_CurrentHelpText();

        void get_CurrentCulture();

        void get_CurrentIsControlElement();

        void get_CurrentIsContentElement();

        void get_CurrentIsPassword();

        void get_CurrentNativeWindowHandle();

        void get_CurrentItemType();

        void get_CurrentIsOffscreen();

        void get_CurrentOrientation();

        void get_CurrentFrameworkId();

        void get_CurrentIsRequiredForForm();

        void get_CurrentItemStatus();

        HRESULT get_CurrentBoundingRectangle(out RECT retVal);

        void get_CurrentLabeledBy();

        void get_CurrentAriaRole();

        void get_CurrentAriaProperties();

        void get_CurrentIsDataValidForForm();

        void get_CurrentControllerFor();

        void get_CurrentDescribedBy();

        void get_CurrentFlowsTo();

        void get_CurrentProviderDescription();

        void get_CachedProcessId();

        void get_CachedControlType();

        void get_CachedLocalizedControlType();

        void get_CachedName();

        void get_CachedAcceleratorKey();

        void get_CachedAccessKey();

        void get_CachedHasKeyboardFocus();

        void get_CachedIsKeyboardFocusable();

        void get_CachedIsEnabled();

        void get_CachedAutomationId();

        void get_CachedClassName();

        void get_CachedHelpText();

        void get_CachedCulture();

        void get_CachedIsControlElement();

        void get_CachedIsContentElement();

        void get_CachedIsPassword();

        void get_CachedNativeWindowHandle();

        void get_CachedItemType();

        void get_CachedIsOffscreen();

        void get_CachedOrientation();

        void get_CachedFrameworkId();

        void get_CachedIsRequiredForForm();

        void get_CachedItemStatus();

        void get_CachedBoundingRectangle();

        void get_CachedLabeledBy();

        void get_CachedAriaRole();

        void get_CachedAriaProperties();

        void get_CachedIsDataValidForForm();

        void get_CachedControllerFor();

        void get_CachedDescribedBy();

        void get_CachedFlowsTo();

        void get_CachedProviderDescription();

        HRESULT GetClickablePoint(out Point clickable, out bool gotClickable);
    }
}
