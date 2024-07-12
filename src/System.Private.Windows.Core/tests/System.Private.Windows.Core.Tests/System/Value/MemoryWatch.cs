// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ValueTests;

public ref struct MemoryWatch
{
    private static bool s_jit;
    private long _allocations;

    public static void JIT()
    {
        if (s_jit)
        {
            return;
        }

        // JITing allocates, so make sure we've got all of our <T> methods created.

        Value.Create((bool)default).GetValue<bool>();
        Value.Create((byte)default).GetValue<byte>();
        Value.Create((sbyte)default).GetValue<sbyte>();
        Value.Create((char)default).GetValue<char>();
        Value.Create((double)default).GetValue<double>();
        Value.Create((short)default).GetValue<short>();
        Value.Create((int)default).GetValue<int>();
        Value.Create((long)default).GetValue<long>();
        Value.Create((ushort)default).GetValue<ushort>();
        Value.Create((uint)default).GetValue<uint>();
        Value.Create((ulong)default).GetValue<ulong>();
        Value.Create((float)default).GetValue<float>();
        Value.Create((double)default).GetValue<double>();
        Value.Create((DateTime)default).GetValue<DateTime>();
        Value.Create((DateTimeOffset)default).GetValue<DateTimeOffset>();

        Value.Create((bool?)default).GetValue<bool?>();
        Value.Create((byte?)default).GetValue<byte?>();
        Value.Create((sbyte?)default).GetValue<sbyte?>();
        Value.Create((char?)default).GetValue<char?>();
        Value.Create((double?)default).GetValue<double?>();
        Value.Create((short?)default).GetValue<short?>();
        Value.Create((int?)default).GetValue<int?>();
        Value.Create((long?)default).GetValue<long?>();
        Value.Create((ushort?)default).GetValue<ushort?>();
        Value.Create((uint?)default).GetValue<uint?>();
        Value.Create((ulong?)default).GetValue<ulong?>();
        Value.Create((float?)default).GetValue<float?>();
        Value.Create((double?)default).GetValue<double?>();
        Value.Create((DateTime?)default).GetValue<DateTime?>();
        Value.Create((DateTimeOffset?)default).GetValue<DateTimeOffset?>();

        Value value = default;
        value.TryGetValue(out bool _);
        value.TryGetValue(out byte _);
        value.TryGetValue(out sbyte _);
        value.TryGetValue(out char _);
        value.TryGetValue(out double _);
        value.TryGetValue(out short _);
        value.TryGetValue(out int _);
        value.TryGetValue(out long _);
        value.TryGetValue(out ushort _);
        value.TryGetValue(out uint _);
        value.TryGetValue(out ulong _);
        value.TryGetValue(out float _);
        value.TryGetValue(out double _);
        value.TryGetValue(out DateTime _);
        value.TryGetValue(out DateTimeOffset _);

        s_jit = true;
    }

    public MemoryWatch(long allocations) => _allocations = allocations;

    public static MemoryWatch Create
    {
        get
        {
            JIT();
            return new(GC.GetAllocatedBytesForCurrentThread());
        }
    }

    public void Dispose() => Validate();

    public void Validate()
    {
        Assert.Equal(0, GC.GetAllocatedBytesForCurrentThread() - _allocations);

        // Assert.Equal allocates
        _allocations = GC.GetAllocatedBytesForCurrentThread();
    }
}
