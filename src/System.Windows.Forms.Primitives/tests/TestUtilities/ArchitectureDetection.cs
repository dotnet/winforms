﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

public static class ArchitectureDetection
{
    public static bool Is32bit => IntPtr.Size == 4;
    public static bool Is64bit => IntPtr.Size == 8;
}
