// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Drawing2D;

public enum CoordinateSpace
{
    World = GdiPlus.CoordinateSpace.CoordinateSpaceWorld,
    Page = GdiPlus.CoordinateSpace.CoordinateSpacePage,
    Device = GdiPlus.CoordinateSpace.CoordinateSpaceDevice
}
