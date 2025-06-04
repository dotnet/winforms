// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class BindingSource
{
    [Flags]
    private enum BindingSourceStates : short
    {
        ParentsCurrentItemChanging  = 0b0000_0000_0000_0001,
        DisposedOrFinalized         = 0b0000_0000_0000_0010,
        IsBindingList               = 0b0000_0000_0000_0100,
        ListRaisesItemChangedEvents = 0b0000_0000_0000_1000,
        ListExtractedFromEnumerable = 0b0000_0000_0001_0000,
        AllowNewIsSet               = 0b0000_0000_0010_0000,
        AllowNewSetValue            = 0b0000_0000_0100_0000,
        Initializing                = 0b0000_0000_1000_0000,
        NeedToSetList               = 0b0000_0001_0000_0000,
        RecursionDetectionFlag      = 0b0000_0010_0000_0000,
        InnerListChanging           = 0b0000_0100_0000_0000,
        EndingEdit                  = 0b0000_1000_0000_0000,
    }
}
