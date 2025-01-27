// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;

namespace System;

/// <summary>
///  Use (within a using) to eat asserts.
/// </summary>
public sealed class NoAssertContext : IDisposable
{
    // For any given thread we don't need to lock to decide how to route messages, as any messages for that
    // given thread will not happen while we're in the constructor or dispose method on that thread. That
    // means we can safely check to see if we've hooked our thread without locking (outside of using a
    // concurrent collection to make sure the collection is in a known state).
    //
    // We do, however need to lock around hooking/unhooking our custom listener to make sure that we
    // are rerouting correctly if multiple threads are creating/disposing this class concurrently.

    private static readonly Lock s_lock = new();
    private static bool s_hooked;
    private static bool s_hasDefaultListener;
    private static bool s_hasThrowingListener;

    private static readonly ConcurrentDictionary<int, int> s_suppressedThreads = new();

    // "Default" is the listener that terminates the process when debug assertions fail.
    private static readonly TraceListener? s_defaultListener = Trace.Listeners["Default"];
    private static readonly NoAssertListener s_noAssertListener = new();

    public NoAssertContext()
    {
        s_suppressedThreads.AddOrUpdate(Environment.CurrentManagedThreadId, 1, (key, oldValue) => oldValue + 1);

        // Lock to make sure we are hooked properly if two threads come into the constructor/dispose at the same time.
        lock (s_lock)
        {
            if (!s_hooked)
            {
                // Hook our custom listener first so we don't lose assertions from other threads when
                // we disconnect the default listener.
                Trace.Listeners.Add(s_noAssertListener);
                if (s_defaultListener is not null && Trace.Listeners.Contains(s_defaultListener))
                {
                    s_hasDefaultListener = true;
                    Trace.Listeners.Remove(s_defaultListener);
                }

                if (Trace.Listeners.OfType<ThrowingTraceListener>().FirstOrDefault() is { } throwingTraceListener)
                {
                    s_hasThrowingListener = true;
                    Trace.Listeners.Remove(throwingTraceListener);
                }

                s_hooked = true;
            }
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        int currentThread = Environment.CurrentManagedThreadId;
        if (s_suppressedThreads.TryRemove(currentThread, out int count))
        {
            if (count > 1)
            {
                // We're in a nested assert context on a given thread, re-add with a decremented count.
                // This doesn't need to be atomic as we're currently on the thread that would care about
                // being rerouted.
                s_suppressedThreads.TryAdd(currentThread, --count);
            }
        }

        lock (s_lock)
        {
            if (s_hooked && s_suppressedThreads.IsEmpty)
            {
                // We're the first to hit the need to unhook. Add the default listener back first to
                // ensure we don't lose any asserts from other threads.
                if (s_hasDefaultListener)
                {
                    Trace.Listeners.Add(s_defaultListener!);
                }

                if (s_hasThrowingListener)
                {
                    Trace.Listeners.Add(ThrowingTraceListener.Instance);
                }

                Trace.Listeners.Remove(s_noAssertListener);
                s_hooked = false;
            }
        }
    }

#pragma warning disable CA1821 // Remove empty Finalizers
    ~NoAssertContext()
#pragma warning restore CA1821
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

        private static TraceListener? DefaultListener
        {
            get
            {
                if (s_hasThrowingListener)
                    return ThrowingTraceListener.Instance;
                else if (s_hasDefaultListener)
                    return s_defaultListener;
                else
                    return null;
            }
        }

        public override void Fail(string? message)
        {
            if (!s_suppressedThreads.TryGetValue(Environment.CurrentManagedThreadId, out _))
            {
                DefaultListener?.Fail(message);
            }
        }

        public override void Fail(string? message, string? detailMessage)
        {
            if (!s_suppressedThreads.TryGetValue(Environment.CurrentManagedThreadId, out _))
            {
                DefaultListener?.Fail(message, detailMessage);
            }
        }

        // Write and WriteLine are virtual

        public override void Write(string? message)
        {
            if (!s_suppressedThreads.TryGetValue(Environment.CurrentManagedThreadId, out _))
            {
                DefaultListener?.Write(message);
            }
        }

        public override void WriteLine(string? message)
        {
            if (!s_suppressedThreads.TryGetValue(Environment.CurrentManagedThreadId, out _))
            {
                DefaultListener?.WriteLine(message);
            }
        }
    }
}
