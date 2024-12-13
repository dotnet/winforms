﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing;

internal interface IIcon : IHandle<HICON>
{
    public Size Size { get; }
}
