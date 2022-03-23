// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System
{
    internal class MallocSpy : IMallocSpy
    {
        public virtual nuint PreAlloc(nuint cbRequest) => cbRequest;
        public virtual unsafe void* PostAlloc(void* pActual) => pActual;
        public virtual unsafe void* PreFree(void* pRequest, BOOL fSpyed) => pRequest;
        public virtual void PostFree(BOOL fSpyed) { }
        public virtual unsafe nuint PreRealloc(void* pRequest, nuint cbRequest, void** ppNewRequest, BOOL fSpyed) => cbRequest;
        public virtual unsafe void* PostRealloc(void* pActual, BOOL fSpyed) => pActual;
        public virtual unsafe void* PreGetSize(void* pRequest, BOOL fSpyed) => pRequest;
        public virtual nuint PostGetSize(nuint cbActual, BOOL fSpyed) => cbActual;
        public virtual unsafe void* PreDidAlloc(void* pRequest, BOOL fSpyed) => pRequest;
        public virtual unsafe int PostDidAlloc(void* pRequest, BOOL fSpyed, int fActual) => fActual;
        public virtual void PreHeapMinimize() { }
        public virtual void PostHeapMinimize() { }

        internal class FreeTracker : MallocSpy
        {
            public List<IntPtr> FreedBlocks { get; } = new();

            public override unsafe void* PreFree(void* pRequest, BOOL fSpyed)
            {
                FreedBlocks.Add((IntPtr)pRequest);
                return pRequest;
            }
        }
    }
}
