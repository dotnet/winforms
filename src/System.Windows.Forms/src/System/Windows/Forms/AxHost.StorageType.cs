// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public abstract partial class AxHost
{
    [Flags]
    internal enum StorageType
    {
        Unknown = -1,
        Stream = 0,
        StreamInit = 1,
        Storage = 2,
        PropertyBag = 3
    }
}
