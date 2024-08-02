// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class RefCacheTests
{
    [Fact]
    public void RefCountTests()
    {
        ObjectCache<object> cache = new(5, 10);
        var firstScope = cache.GetEntry(1).CreateScope();
        Assert.Equal(1, firstScope.RefCount);
        object first = firstScope.Object;
        var secondScope = cache.GetEntry(1).CreateScope();
        object second = secondScope.Object;
        Assert.Equal(2, firstScope.RefCount);
        Assert.Equal(2, secondScope.RefCount);
        Assert.Same(first, second);
        firstScope.Dispose();
        Assert.Equal(1, secondScope.RefCount);
        secondScope.Dispose();
        Assert.Equal(0, secondScope.RefCount);
        using var thirdScope = cache.GetEntry(1).CreateScope();
        Assert.Equal(1, thirdScope.RefCount);
        Assert.Same(first, thirdScope.Object);
    }

    [Fact]
    public void LimitTests()
    {
        ObjectCache<DisposalCounter> cache = new(2, 4);

        // Fill to the hard limit
        var firstScope = cache.GetEntry(1).CreateScope();
        var secondScope = cache.GetEntry(2).CreateScope();
        var thirdScope = cache.GetEntry(3).CreateScope();
        var fourthScope = cache.GetEntry(4).CreateScope();
        Assert.Equal(0, firstScope.Object.DisposeCount);
        Assert.Equal(0, secondScope.Object.DisposeCount);
        Assert.Equal(0, thirdScope.Object.DisposeCount);
        Assert.Equal(0, fourthScope.Object.DisposeCount);

        // New objects shouldn't be cached
        var fifthScope = cache.GetEntry(5).CreateScope();
        var sixthScope = cache.GetEntry(5).CreateScope();

        Assert.NotSame(fifthScope.Object, sixthScope.Object);

        // Dispose all scopes
        firstScope.Dispose();
        secondScope.Dispose();
        thirdScope.Dispose();
        fourthScope.Dispose();
        fifthScope.Dispose();
        sixthScope.Dispose();

        Assert.Equal(0, firstScope.Object.DisposeCount);
        Assert.Equal(0, secondScope.Object.DisposeCount);
        Assert.Equal(0, thirdScope.Object.DisposeCount);
        Assert.Equal(0, fourthScope.Object.DisposeCount);

        // As these were never cached they should be disposed
        Assert.Equal(1, fifthScope.Object.DisposeCount);
        Assert.Equal(1, sixthScope.Object.DisposeCount);

        using var seventhScope = cache.GetEntry(7).CreateScope();

        // Now that the other scopes are closed, the earliest entries should have been cleaned
        Assert.Equal(1, firstScope.Object.DisposeCount);
        Assert.Equal(1, secondScope.Object.DisposeCount);
        Assert.Equal(0, thirdScope.Object.DisposeCount);
        Assert.Equal(0, fourthScope.Object.DisposeCount);
    }

    // Example to show that the entry data can match the object type when it has enough context to match the key
    internal sealed class IntColorCache : RefCountedCache<Color, Color, int>
    {
        public IntColorCache(int softLimit, int hardLimit) : base(softLimit, hardLimit) { }

        protected override CacheEntry CreateEntry(int key, bool cached)
            => new ColorCacheEntry(Color.FromArgb(key), cached);

        protected override bool IsMatch(int key, CacheEntry data) => key == data.Data.ToArgb();

        private class ColorCacheEntry : CacheEntry
        {
            public ColorCacheEntry(Color color, bool cached) : base(color, cached) { }
            public override Color Object => Data;
        }
    }

    public class DisposalCounter : IDisposable
    {
        public int DisposeCount { get; private set; }
        public void Dispose() => ++DisposeCount;
    }

    internal class ObjectCache<T> : RefCountedCache<T, int, int> where T : class, new()
    {
        public ObjectCache(int softLimit = 40, int hardLimit = 60) : base(softLimit, hardLimit) { }

        protected override CacheEntry CreateEntry(int key, bool cached) => new ObjectCacheEntry(key, cached);
        protected override bool IsMatch(int key, CacheEntry data) => key == data.Data;

        protected class ObjectCacheEntry : CacheEntry
        {
            private readonly T _object;
            public ObjectCacheEntry(int value, bool cached) : base(value, cached) => _object = new T();
            public override T Object => _object;
        }
    }
}
