// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System
{
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
