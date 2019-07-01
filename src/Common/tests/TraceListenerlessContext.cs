// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;

namespace WinForms.Common.Tests
{
    public sealed class TraceListenerlessContext : IDisposable
    {
        private readonly TraceListener[] _traceListenerCollection;

        public TraceListenerlessContext()
        {
            _traceListenerCollection = new TraceListener[Trace.Listeners.Count];
            Trace.Listeners.CopyTo(_traceListenerCollection, 0);
            Trace.Listeners.Clear();
        }

        public void Dispose()
        {
            // remove anything that may have been added to the collection
            Trace.Listeners.Clear();

            // restore the original collection
            Trace.Listeners.AddRange(_traceListenerCollection);
        }
    }
}
