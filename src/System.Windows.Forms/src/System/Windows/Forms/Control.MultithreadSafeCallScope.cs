// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;
using System.Reflection.Metadata.Ecma335;

namespace System.Windows.Forms
{
    public partial class Control
    {
        private readonly ref struct MultithreadSafeCallScope
        {
            // Use local stack variable rather than a refcount since we're
            // guaranteed that these 'scopes' are properly nested.
            private readonly bool _resultedInSet;

            private MultithreadSafeCallScope(bool resultedInSet)
                => _resultedInSet = resultedInSet;

            internal static MultithreadSafeCallScope Create()
            {
                // Only access the thread-local stuff if we're going to be
                // checking for illegal thread calling (no need to incur the
                // expense otherwise).
                if (CheckForIllegalCrossThreadCalls && !t_inCrossThreadSafeCall)
                {
                    t_inCrossThreadSafeCall = true;
                    return new MultithreadSafeCallScope(true);
                }
                else
                {
                    return new MultithreadSafeCallScope(false);
                }
            }

            public void Dispose()
            {
                if (_resultedInSet)
                {
                    t_inCrossThreadSafeCall = false;
                }
            }
        }
    }
}
