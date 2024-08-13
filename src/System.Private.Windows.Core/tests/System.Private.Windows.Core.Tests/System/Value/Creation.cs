// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ValueTests;

public class Creation
{
    [Fact]
    public void CreateIsAllocationFree()
    {
        var watch = MemoryWatch.Create;

        Value.Create((byte)default);
        watch.Validate();
        Value.Create((sbyte)default);
        watch.Validate();
        Value.Create((char)default);
        watch.Validate();
        Value.Create((double)default);
        watch.Validate();
        Value.Create((short)default);
        watch.Validate();
        Value.Create((int)default);
        watch.Validate();
        Value.Create((long)default);
        watch.Validate();
        Value.Create((ushort)default);
        watch.Validate();
        Value.Create((uint)default);
        watch.Validate();
        Value.Create((ulong)default);
        watch.Validate();
        Value.Create((float)default);
        watch.Validate();
        Value.Create((double)default);
        watch.Validate();

        Value.Create((bool?)default);
        watch.Validate();
        Value.Create((byte?)default);
        watch.Validate();
        Value.Create((sbyte?)default);
        watch.Validate();
        Value.Create((char?)default);
        watch.Validate();
        Value.Create((double?)default);
        watch.Validate();
        Value.Create((short?)default);
        watch.Validate();
        Value.Create((int?)default);
        watch.Validate();
        Value.Create((long?)default);
        watch.Validate();
        Value.Create((ushort?)default);
        watch.Validate();
        Value.Create((uint?)default);
        watch.Validate();
        Value.Create((ulong?)default);
        watch.Validate();
        Value.Create((float?)default);
        watch.Validate();
        Value.Create((double?)default);
        watch.Validate();
    }
}
