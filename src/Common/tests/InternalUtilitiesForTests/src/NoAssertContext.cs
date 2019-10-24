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
    /// <remarks>
    ///  The <see cref="TraceListenerCollection"/> maintains a lock against which it is very easy to
    ///  cause lock inversion if we aren't careful:
    ///
    ///  (1) During installation the installing thread will first check if installation is necessary,
    ///      for this we need to check the list of suppressed threads and see if we are the first.
    ///      Checking this list requires synchronization. If we determine that we are the first thread
    ///      we will want to install the TraceListener which requires (implicitely) taking the
    ///      TraceListenerCollection lock. Furthermore all callers which pass through this code path,
    ///      regardless of whether they are installing or not, need to wait for the installation to
    ///      complete before returning.
    ///
    ///  (2) During logging we are called while the TraceListenerCollection lock is held. We want to
    ///      look at the list of suppressed threads to determine how to assert.
    ///
    ///  Note that both cases together form the base of a lock inversion deadlock with no easy way out.
    /// </remarks>
    public sealed class NoAssertContext : IDisposable
    {
        private static readonly object s_lock = new object();
        private static readonly HashSet<int> s_suppressedThreads = new HashSet<int>();
        private static TraceListener s_defaultListener;
        private static readonly NoAssertListener s_noAssertListener = new NoAssertListener();
        private static bool s_installing;
        private static bool s_uninstalling;

        private bool _registeredSuppression;

        public NoAssertContext()
        {
            // Warning: do not call NoAssertContext while TraceListenerCollection lock is held,
            // for example from within another TraceListener. If you do this you can deadlock.
            //
            // This happens when the "wrong" thread is picked for installation and has to wait
            // for the TraceListenerCollection lock, while he thread currently holding it waits
            // for the installing thread to complete installation.
            //
            // There is no way for us to check and throw if the lock is held so we'll just assume
            // it is not. Consider using NoAssertContext within the implementation of another
            // TraceListener unsupported.

            bool install = false;

            lock (s_lock)
            {
                // If we came in during uninstallation we need to wait until it completes.
                while (s_uninstalling)
                    Monitor.Wait(s_lock);

                if (!s_suppressedThreads.Add(Thread.CurrentThread.ManagedThreadId))
                {
                    // If this thread already is suppressed then this is a nested NoAssertContext.
                    // Just ignore this call and leave _registeredSuppression false.
                    return;
                }

                _registeredSuppression = true;

                if (s_suppressedThreads.Count == 1)
                {
                    install = true;
                    s_installing = true;
                }
                else
                {
                    // If we came in during installation we need to wait until it completes,
                    // otherwise we are not guaranteed that asserts are suppressed properly.
                    while (s_installing)
                        Monitor.Wait(s_lock);
                }
            }

            // Installation must happen outside of the lock to avoid deadlocks from lock inversion.
            if (install)
            {
                // Hook our custom listener first so we don't lose assertions from other threads when
                // we disconnect the default listener.
                Trace.Listeners.Add(s_noAssertListener);

                // "Default" is the listener that terminates the process when debug assertions fail.
                var defaultListener = Trace.Listeners["Default"];

                lock (s_lock)
                {
                    // Wire up the NoAssertListener to the Default listener. This must happen under
                    // the lock because NoAssertListener may already be getting triggered from other
                    // threads.
                    s_defaultListener = defaultListener;
                }

                // Don't remove the default listener before the static field has been hooked up.
                // This ensures that either the NoAssertListener or the Default listener will
                // pick up the asserts and logging.
                Trace.Listeners.Remove(defaultListener);

                // Notify waiting threads that we are done installing and NoAssertContext works.
                lock (s_lock)
                {
                    s_installing = false;
                    Monitor.PulseAll(s_lock);
                }
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            // If this was a nested NoAssertContext or if Dispose was called twice do nothing.
            if (!_registeredSuppression)
                return;

            _registeredSuppression = false;
            bool uninstall = false;
            TraceListener defaultListener = null;

            lock (s_lock)
            {
                while (s_installing)
                    Monitor.Wait(s_lock);

                s_suppressedThreads.Remove(Thread.CurrentThread.ManagedThreadId);
                if (s_suppressedThreads.Count == 0)
                {
                    uninstall = true;
                    defaultListener = s_noAssertListener;
                    s_uninstalling = true;
                }
            }

            if (uninstall)
            {
                // Add the default listener back first to make sure we don't lose any
                // asserts from other threads.
                if (defaultListener != null)
                    Trace.Listeners.Add(defaultListener);

                Trace.Listeners.Remove(s_noAssertListener);

                lock (s_lock)
                {
                    // Reset s_defaultListener after everything else because it would
                    // disconncet the NoAssertListener and prevent forwarding asserts.
                    s_defaultListener = null;
                    s_uninstalling = false;
                    Monitor.PulseAll(s_lock);
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
