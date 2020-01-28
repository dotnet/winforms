// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class Control
    {
        private sealed class MultithreadSafeCallScope : IDisposable
        {
            // Use local stack variable rather than a refcount since we're
            // guaranteed that these 'scopes' are properly nested.
            private readonly bool _resultedInSet;

            internal MultithreadSafeCallScope()
            {
                // Only access the thread-local stuff if we're going to be
                // checking for illegal thread calling (no need to incur the
                // expense otherwise).
                if (CheckForIllegalCrossThreadCalls && !t_inCrossThreadSafeCall)
                {
                    t_inCrossThreadSafeCall = true;
                    _resultedInSet = true;
                }
                else
                {
                    _resultedInSet = false;
                }
            }

            void IDisposable.Dispose()
            {
                if (_resultedInSet)
                {
                    t_inCrossThreadSafeCall = false;
                }
            }
        }
    }
}
