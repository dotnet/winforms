// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using static Interop;

namespace System
{
    /// <summary>
    ///  Scope for registering and revoking a malloc spy class.
    /// </summary>
    internal ref partial struct MallocSpyScope
    {
        private static readonly object s_lock = new();
        private static readonly MasterSpy s_masterSpy = new();
        private static bool s_registered;
        private readonly bool _lockTaken;

        public MallocSpyScope(IMallocSpy mallocSpy, bool currentThreadOnly = true)
        {
            _lockTaken = false;
            Monitor.Enter(s_lock, ref _lockTaken);

            if (!s_registered)
            {
                // If another thread allocated while we were registered and hasn't freed everything yet, we can't
                // deregister. As such we'll keep a permanent global spy and forward to whatever our current context is.

                var result = CoRegisterMallocSpy(s_masterSpy);
                if (result.Failed())
                {
                    throw new InvalidOperationException(result.AsString());
                }

                s_registered = true;
            }

            if (s_registered)
            {
                s_masterSpy.SetSpy(mallocSpy, currentThreadOnly);
            }
        }

        public void Dispose()
        {
            if (_lockTaken)
            {
                s_masterSpy.SetSpy(spy: null, currentThreadOnly: true);

                Monitor.Exit(s_lock);
            }
        }

        [DllImport("ole32.dll", ExactSpelling = true)]
        private static extern HRESULT CoRegisterMallocSpy(IMallocSpy pMallocSpy);
    }
}
