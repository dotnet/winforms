// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System
{
    internal ref partial struct MallocSpyScope
    {
        /// <summary>
        ///  Redirecting spy that we register as a global.
        /// </summary>
        private class MasterSpy : IMallocSpy
        {
            private IMallocSpy _currentSpy;
            private uint _registeredThread;
            private readonly object _lock = new();

            public IMallocSpy CurrentSpy
            {
                get
                {
                    lock (_lock)
                    {
                        return (_registeredThread == 0 || _registeredThread == Kernel32.GetCurrentThreadId())
                            ? _currentSpy : null;
                    }
                }
            }

            public void SetSpy(IMallocSpy spy, bool currentThreadOnly)
            {
                lock (_lock)
                {
                    _currentSpy = spy;
                    _registeredThread = currentThreadOnly ? Kernel32.GetCurrentThreadId() : 0;
                }
            }

            public nuint PreAlloc(nuint cbRequest) => CurrentSpy?.PreAlloc(cbRequest) ?? cbRequest;

            public unsafe void* PostAlloc(void* pActual)
            {
                IMallocSpy current = CurrentSpy;
                return current is null
                    ? pActual
                    : current.PostAlloc(pActual);
            }

            public unsafe void* PreFree(void* pRequest, BOOL fSpyed)
            {
                IMallocSpy current = CurrentSpy;
                return current is null
                    ? pRequest
                    : current.PreFree(pRequest, fSpyed);
            }

            public void PostFree(BOOL fSpyed) => CurrentSpy?.PostFree(fSpyed);

            public unsafe nuint PreRealloc(void* pRequest, nuint cbRequest, void** ppNewRequest, BOOL fSpyed)
                => CurrentSpy?.PreRealloc(pRequest, cbRequest, ppNewRequest, fSpyed) ?? cbRequest;

            public unsafe void* PostRealloc(void* pActual, BOOL fSpyed)
            {
                IMallocSpy current = CurrentSpy;
                return current is null
                    ? pActual
                    : current.PostRealloc(pActual, fSpyed);
            }

            public unsafe void* PreGetSize(void* pRequest, BOOL fSpyed)
            {
                IMallocSpy current = CurrentSpy;
                return current is null
                    ? pRequest
                    : current.PreGetSize(pRequest, fSpyed);
            }

            public nuint PostGetSize(nuint cbActual, BOOL fSpyed)
                => CurrentSpy?.PostGetSize(cbActual, fSpyed) ?? cbActual;

            public unsafe void* PreDidAlloc(void* pRequest, BOOL fSpyed)
            {
                IMallocSpy current = CurrentSpy;
                return current is null
                    ? pRequest
                    : current.PreDidAlloc(pRequest, fSpyed);
            }

            public unsafe int PostDidAlloc(void* pRequest, BOOL fSpyed, int fActual)
                => CurrentSpy?.PostDidAlloc(pRequest, fSpyed, fActual) ?? fActual;

            public void PreHeapMinimize() => CurrentSpy?.PreHeapMinimize();

            public void PostHeapMinimize() => CurrentSpy?.PostHeapMinimize();
        }
    }
}
