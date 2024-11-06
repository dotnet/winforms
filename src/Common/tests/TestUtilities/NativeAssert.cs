// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

public static unsafe class NativeAssert
{
    public static void Null(void* pointer) => Assert.True(pointer is null);

    public static void NotNull(void* pointer) => Assert.True(pointer is not null);
}
