// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Threading.Tests;

public class InterlockedExtensionsTests
{
    [Fact]
    public void Increment_IncrementsValue()
    {
        uint value = 0;
        uint result = Interlocked.Increment(ref value);

        result.Should().Be(1u);
        value.Should().Be(1u);
    }

    [Fact]
    public void Increment_MultipleIncrements()
    {
        uint value = 0;
        Interlocked.Increment(ref value);
        Interlocked.Increment(ref value);
        Interlocked.Increment(ref value);

        value.Should().Be(3u);
    }

    [Fact]
    public void Increment_MaxValueWrapsToZero()
    {
        uint value = uint.MaxValue;
        uint result = Interlocked.Increment(ref value);

        result.Should().Be(0u);
        value.Should().Be(0u);
    }

    [Fact]
    public void Decrement_DecrementsValue()
    {
        uint value = 5;
        uint result = Interlocked.Decrement(ref value);

        result.Should().Be(4u);
        value.Should().Be(4u);
    }

    [Fact]
    public void Decrement_ZeroWrapsToMaxValue()
    {
        uint value = 0;
        uint result = Interlocked.Decrement(ref value);

        result.Should().Be(uint.MaxValue);
        value.Should().Be(uint.MaxValue);
    }

    [Fact]
    public void Exchange_SwapsValue()
    {
        uint value = 42;
        uint original = Interlocked.Exchange(ref value, 100);

        original.Should().Be(42u);
        value.Should().Be(100u);
    }

    [Fact]
    public void Exchange_ReturnsOriginal()
    {
        uint value = 0;
        uint original = Interlocked.Exchange(ref value, uint.MaxValue);

        original.Should().Be(0u);
        value.Should().Be(uint.MaxValue);
    }

    [Fact]
    public void Add_AddsValues()
    {
        uint value = 10;
        uint result = Interlocked.Add(ref value, 5);

        result.Should().Be(15u);
        value.Should().Be(15u);
    }

    [Fact]
    public void Add_Zero_DoesNotChange()
    {
        uint value = 42;
        uint result = Interlocked.Add(ref value, 0);

        result.Should().Be(42u);
        value.Should().Be(42u);
    }

    [Fact]
    public void Add_Overflow_Wraps()
    {
        uint value = uint.MaxValue;
        uint result = Interlocked.Add(ref value, 2);

        result.Should().Be(1u);
        value.Should().Be(1u);
    }

    [Fact]
    public void Increment_IsThreadSafe()
    {
        uint value = 0;
        int iterations = 10_000;
        int threadCount = 4;

        Thread[] threads = new Thread[threadCount];
        for (int i = 0; i < threadCount; i++)
        {
            threads[i] = new Thread(() =>
            {
                for (int j = 0; j < iterations; j++)
                {
                    Interlocked.Increment(ref value);
                }
            });
        }

        foreach (Thread thread in threads)
        {
            thread.Start();
        }

        foreach (Thread thread in threads)
        {
            thread.Join();
        }

        value.Should().Be((uint)(iterations * threadCount));
    }
}
