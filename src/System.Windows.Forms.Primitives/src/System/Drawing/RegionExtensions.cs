// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing;

internal static class RegionExtensions
{
    public static Region ToRegion(this RegionScope scope) => Region.FromHrgn(scope.Region);
}
