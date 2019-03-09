﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    [Flags]
    [SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames", Justification = "PM reviewed the enum name")]
    public enum GetChildAtPointSkip
    {
        None = 0x0000,
        Invisible = 0x0001,         
        Disabled = 0x0002,   
        Transparent = 0x0004
    }
}
