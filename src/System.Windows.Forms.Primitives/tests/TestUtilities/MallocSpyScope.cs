// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using static Interop;

namespace System
{
    /// <summary>
    ///  Scope for registering and revoking a malloc spy class.
    /// </summary>
    internal ref struct MallocSpyScope
    {
        private static readonly object s_lock = new();
        private readonly bool _lockTaken;

        public MallocSpyScope(Ole32.IMallocSpy mallocSpy)
        {
            _lockTaken = false;
            Monitor.Enter(s_lock, ref _lockTaken);
            HRESULT result = Ole32.CoRegisterMallocSpy(mallocSpy);
            if (result.Failed())
            {
                throw new InvalidOperationException(result.AsString());
            }
        }

        public void Dispose()
        {
            Ole32.CoRevokeMallocSpy();
            if (_lockTaken)
            {
                Monitor.Exit(s_lock);
            }
        }
    }
}
