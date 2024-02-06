// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class BindingSource
{
    [Flags]
    private enum BindingSourceStates
    {
        ParentsCurrentItemChanging = 1 << 0,
        DisposedOrFinalized = 1 << 1,
        IsBindingList = 1 << 2,
        ListRaisesItemChangedEvents = 1 << 3,
        ListExtractedFromEnumerable = 1 << 4,
        AllowNewIsSet = 1 << 5,
        AllowNewSetValue = 1 << 6,
        Initializing = 1 << 7,
        NeedToSetList = 1 << 8,
        RecursionDetectionFlag = 1 << 9,
        InnerListChanging = 1 << 10,
        EndingEdit = 1 << 11,
    }
}
