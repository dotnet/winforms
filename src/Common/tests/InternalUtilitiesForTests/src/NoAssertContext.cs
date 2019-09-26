// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace System
{
    /// <summary>
    ///  Use (within a using) to eat asserts.
    /// </summary>
    public sealed class NoAssertContext : IDisposable
    {
        private static readonly object s_lock = new object();
        private static readonly HashSet<int> s_suppressedThreads = new HashSet<int>();
        private static TraceListener s_defaultListener;
        private static readonly NoAssertListener s_noAssertListener = new NoAssertListener();

        public NoAssertContext()
        {
            lock (s_lock)
            {
                s_suppressedThreads.Add(Thread.CurrentThread.ManagedThreadId);
                if (s_suppressedThreads.Count == 1)
                {
                    // Hook our custom listener first so we don't lose assertions from other threads when
                    // we disconnect the default listener.
                    Trace.Listeners.Add(s_noAssertListener);

                    // "Default" is the listener that terminates the process when debug assertions fail.
                    s_defaultListener = Trace.Listeners["Default"];
                    Trace.Listeners.Remove(s_defaultListener);
                }
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            lock (s_lock)
            {
                s_suppressedThreads.Remove(Thread.CurrentThread.ManagedThreadId);
                if (s_suppressedThreads.Count == 0 && s_defaultListener != null)
                {
                    // Add the default listener back first to make sure we don't lose any
                    // asserts from other threads.
                    Trace.Listeners.Add(s_defaultListener);
                    Trace.Listeners.Remove(s_noAssertListener);
                    s_defaultListener = null;
                }
            }
        }

        ~NoAssertContext()
        {
            // We need this class to be used in a using to effectively rationalize about a test.
            throw new InvalidOperationException($"Did not dispose {nameof(NoAssertContext)}");
        }

        private class NoAssertListener : TraceListener
        {
            public NoAssertListener()
                : base(typeof(NoAssertListener).FullName)
            {
            }

            public override void Fail(string message)
            {
                lock (s_lock)
                {
                    if (!s_suppressedThreads.Contains(Thread.CurrentThread.ManagedThreadId))
                    {
                        s_defaultListener?.Fail(message);
                    }
                }
            }

            public override void Fail(string message, string detailMessage)
            {
                lock (s_lock)
                {
                    if (!s_suppressedThreads.Contains(Thread.CurrentThread.ManagedThreadId))
                    {
                        s_defaultListener?.Fail(message, detailMessage);
                    }
                }
            }

            // Write and WriteLine are virtual

            public override void Write(string message)
            {
                lock (s_lock)
                {
                    s_defaultListener?.Write(message);
                }
            }

            public override void WriteLine(string message)
            {
                lock (s_lock)
                {
                    s_defaultListener?.WriteLine(message);
                }
            }
        }
    }
}
