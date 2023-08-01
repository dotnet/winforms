// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

internal static partial class UiaClient
{
    [ComImport]
    [Guid("30CBE57D-D9D0-452A-AB13-7AC5AC4825EE")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IUIAutomation
    {
        void CompareElements();

        void CompareRuntimeIds();

        void GetRootElement();

        void ElementFromHandle();

        void ElementFromPoint();

        int GetFocusedElement(out IUIAutomationElement element);

        void GetRootElementBuildCache();

        void ElementFromHandleBuildCache();

        void ElementFromPointBuildCache();

        void GetFocusedElementBuildCache();

        void CreateTreeWalker();

        void get_ControlViewWalker();

        void get_ContentViewWalker();

        void get_RawViewWalker();

        void get_RawViewCondition();

        void get_ControlViewCondition();

        void get_ContentViewCondition();

        void CreateCacheRequest();

        void CreateTrueCondition();

        void CreateFalseCondition();

        void CreatePropertyCondition();

        void CreatePropertyConditionEx();

        void CreateAndCondition();

        void CreateAndConditionFromArray();

        void CreateAndConditionFromNativeArray();

        void CreateOrCondition();

        void CreateOrConditionFromArray();

        void CreateOrConditionFromNativeArray();

        void CreateNotCondition();

        void AddAutomationEventHandler();

        void RemoveAutomationEventHandler();

        void AddPropertyChangedEventHandlerNativeArray();

        void AddPropertyChangedEventHandler();

        void RemovePropertyChangedEventHandler();

        void AddStructureChangedEventHandler();

        void RemoveStructureChangedEventHandler();

        void AddFocusChangedEventHandler();

        void RemoveFocusChangedEventHandler();

        void RemoveAllEventHandlers();

        void IntNativeArrayToSafeArray();

        void IntSafeArrayToNativeArray();

        void RectToVariant();

        void VariantToRect();

        void SafeArrayToRectNativeArray();

        void CreateProxyFactoryEntry();

        void get_ProxyFactoryMapping();

        void GetPropertyProgrammaticName();

        void GetPatternProgrammaticName();

        void PollForPotentialSupportedPatterns();

        void PollForPotentialSupportedProperties();

        void CheckNotSupported();

        void get_ReservedNotSupportedValue();

        void get_ReservedMixedAttributeValue();

        void ElementFromIAccessible();

        void ElementFromIAccessibleBuildCache();
    }
}
