// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System
{
    /// <summary>
    ///  Scope for registering and revoking a malloc spy class.
    /// </summary>
    /// <remarks>
    ///  As you can't revoke a specific <see cref="Ole32.IMallocSpy"/> you MUST attribute test classes that use
    ///  this with `[Collection(nameof(MallocSpy))]` to force them to run sequentially.
    /// </remarks>
    internal ref struct MallocSpyScope
    {
        public MallocSpyScope(Ole32.IMallocSpy mallocSpy)
        {
            Ole32.CoRegisterMallocSpy(mallocSpy);
        }

        public void Dispose()
        {
            Ole32.CoRevokeMallocSpy();
        }
    }
}
