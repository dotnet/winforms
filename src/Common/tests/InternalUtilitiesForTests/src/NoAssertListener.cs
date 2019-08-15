// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Threading;

namespace System
{
    /// <summary>
    ///  Use (within a using) to eat asserts.
    /// </summary>
    public sealed class NoAssertListener : TraceListener
    {
        private static readonly object s_lock = new object();
        private static int s_count;
        private static TraceListener s_defaultListener;
        private static string AssertHandlerName { get; } = typeof(NoAssertListener).FullName;

        private readonly int _threadId;

        public NoAssertListener()
            : base(AssertHandlerName)
        {
            _threadId = Thread.CurrentThread.ManagedThreadId;

            lock (s_lock)
            {
                s_count += 1;
                Trace.Listeners.Add(this);
                if (s_count == 1)
                {
                    s_defaultListener = Trace.Listeners["Default"];
                    Trace.Listeners.Remove(s_defaultListener);
                }
            }
        }

        public override void Fail(string message)
        {
            lock (s_lock)
            {
                if (_threadId != Thread.CurrentThread.ManagedThreadId)
                {
                    s_defaultListener.Fail(message);
                }
            }
        }

        public override void Fail(string message, string detailMessage)
        {
            lock (s_lock)
            {
                if (_threadId != Thread.CurrentThread.ManagedThreadId)
                {
                    s_defaultListener.Fail(message, detailMessage);
                }
            }
        }

        // Write and WriteLine are virtual

        public override void Write(string message)
        {
            lock (s_lock)
            {
                s_defaultListener.Write(message);
            }
        }

        public override void WriteLine(string message)
        {
            lock (s_lock)
            {
                s_defaultListener.WriteLine(message);
            }
        }

        protected override void Dispose(bool disposing)
        {
            lock (s_lock)
            {
                s_count -= 1;
                if (s_count == 0 && s_defaultListener != null)
                {
                    Trace.Listeners.Add(s_defaultListener);
                }
                Trace.Listeners.Remove(this);
            }

            base.Dispose(disposing);
        }

        ~NoAssertListener()
        {
            // We need this class to be used in a using to effectively rationalize about a test.
            throw new InvalidOperationException($"Did not dispose {nameof(NoAssertListener)}");
        }
    }
}
