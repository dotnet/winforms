// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

internal static partial class Interop
{
    public sealed class HandleCollector
    {
        private static HandleType[] s_handleTypes;
        private static int s_handleTypeCount;
        private static int s_suspendCount;

        private static event HandleChangeEventHandler s_handleAdded;

        private static event HandleChangeEventHandler s_handleRemoved;

        private static readonly object s_internalSyncObject = new object();

        /// <summary>
        /// Adds the given handle to the handle collector. This keeps the
        /// handle on a "hot list" of objects that may need to be garbage
        /// collected.
        /// </summary>
        internal static IntPtr Add(IntPtr handle, int type)
        {
            s_handleTypes[type - 1].Add(handle);
            return handle;
        }

        /// <summary>
        /// Suspends GC.Collect
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal static void SuspendCollect()
        {
            lock (s_internalSyncObject)
            {
                s_suspendCount++;
            }
        }

        /// <summary>
        /// Resumes GC.Collect
        /// </summary>        
        [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal static void ResumeCollect()
        {
            bool performCollect = false;
            lock (s_internalSyncObject)
            {
                if (s_suspendCount > 0)
                {
                    s_suspendCount--;
                }

                if (s_suspendCount == 0)
                {
                    for (int i = 0; i < s_handleTypeCount; i++)
                    {
                        lock (s_handleTypes[i])
                        {
                            if (s_handleTypes[i].NeedCollection())
                            {
                                performCollect = true;
                            }
                        }
                    }
                }
            }

            if (performCollect)
            {
                GC.Collect();
            }
        }

        /// <summary>
        /// Registers a new type of handle with the handle collector.
        /// </summary>
        internal static int RegisterType(string typeName, int expense, int initialThreshold)
        {
            lock (s_internalSyncObject)
            {
                if (s_handleTypeCount == 0 || s_handleTypeCount == s_handleTypes.Length)
                {
                    HandleType[] newTypes = new HandleType[s_handleTypeCount + 10];
                    if (s_handleTypes != null)
                    {
                        Array.Copy(s_handleTypes, 0, newTypes, 0, s_handleTypeCount);
                    }
                    s_handleTypes = newTypes;
                }

                s_handleTypes[s_handleTypeCount++] = new HandleType(typeName, expense, initialThreshold);
                return s_handleTypeCount;
            }
        }

        /// <summary>
        /// Removes the given handle from the handle collector. Removing a
        /// handle removes it from our "hot list" of objects that should be
        /// frequently garbage collected.
        /// </summary>
        internal static IntPtr Remove(IntPtr handle, int type)
        {
            return s_handleTypes[type - 1].Remove(handle);
        }

        /// <summary>
        /// Represents a specific type of handle.
        /// </summary>
        private class HandleType
        {
            internal readonly string _name;

            private readonly int _initialThreshHold;
            private int _threshold;
            private int _handleCount;
            private readonly int _deltaPercent;

#if DEBUG_HANDLECOLLECTOR
            private readonly List<IntPtr> _handles = new List<IntPtr>();
#endif

            /// <summary>
            /// Creates a new handle type.
            /// </summary>
            internal HandleType(string name, int expense, int initialThreshHold)
            {
                _name = name;
                _initialThreshHold = initialThreshHold;
                _threshold = initialThreshHold;
                _deltaPercent = 100 - expense;
            }

            /// <summary>
            /// Adds a handle to this handle type for monitoring.
            /// </summary>            
            [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods")]
            internal void Add(IntPtr handle)
            {
                if (handle == IntPtr.Zero)
                {
                    return;
                }

                bool performCollect = false;
                int currentCount = 0;

                lock (this)
                {
                    _handleCount++;
#if DEBUG_HANDLECOLLECTOR
                    Debug.Assert(!_handles.Contains(handle));
                    _handles.Add(handle);
#endif
                    performCollect = NeedCollection();
                    currentCount = _handleCount;
                }
                lock (s_internalSyncObject)
                {
                    s_handleAdded?.Invoke(_name, handle, currentCount);
                }

                if (!performCollect)
                {
                    return;
                }

                if (performCollect)
                {
#if DEBUG_HANDLECOLLECTOR
                    Debug.WriteLine("HC> Forcing garbage collect");
                    Debug.WriteLine("HC>     name        :" + _name);
                    Debug.WriteLine("HC>     threshHold  :" + _threshHold.ToString());
                    Debug.WriteLine("HC>     handleCount :" + _handleCount).ToString());
                    Debug.WriteLine("HC>     deltaPercent:" + _deltaPercent.ToString());
#endif                  
                    GC.Collect();

                    // We just performed a GC. If the main thread is in a tight
                    // loop there is a this will cause us to increase handles forever and prevent handle collector
                    // from doing its job. Yield the thread here. This won't totally cause
                    // a finalization pass but it will effectively elevate the priority
                    // of the finalizer thread just for an instant. But how long should
                    // we sleep?  We base it on how expensive the handles are because the
                    // more expensive the handle, the more critical that it be reclaimed.
                    int sleep = (100 - _deltaPercent) / 4;
                    Thread.Sleep(sleep);
                }
            }

            /// <summary>
            /// Retrieves the outstanding handle count for this handle type.
            /// </summary>
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            internal int GetHandleCount()
            {
                lock (this)
                {
                    return _handleCount;
                }
            }

            /// <summary>
            /// Determines if this handle type needs a garbage collection pass.
            /// </summary>
            internal bool NeedCollection()
            {
                if (s_suspendCount > 0)
                {
                    return false;
                }
                if (_handleCount > _threshold)
                {
                    _threshold = _handleCount + ((_handleCount * _deltaPercent) / 100);
#if DEBUG_HANDLECOLLECTOR
                    Debug.WriteLine("HC> NeedCollection: increase threshHold to " + _threshjold);
#endif                  
                    return true;
                }

                // If handle count < threshHold, we don't
                // need to collect, but if it 10% below the next lowest threshhold we
                // will bump down a rung. We need to choose a percentage here or else
                // we will oscillate.
                int oldThreshHold = (100 * _threshold) / (100 + _deltaPercent);
                if (oldThreshHold >= _initialThreshHold && _handleCount < (int)(oldThreshHold * .9F))
                {
#if DEBUG_HANDLECOLLECTOR
                    Debug.WriteLine("HC> NeedCollection: throttle threshhold " + _threshHold + " down to " + oldThreshHold);
#endif                  
                    _threshold = oldThreshHold;
                }

                return false;
            }

            /// <summary>
            /// Removes the given handle from our monitor list.
            /// </summary>
            internal IntPtr Remove(IntPtr handle)
            {
                if (handle == IntPtr.Zero)
                {
                    return handle;
                }

                int currentCount = 0;
                lock (this)
                {
                    _handleCount--;
#if DEBUG_HANDLECOLLECTOR
                    Debug.Assert(_handles.Contains(handle));
                    _handles.Remove(handle);
#endif
                    if (_handleCount < 0)
                    {
                        Debug.Fail("Handle collector underflow for type '" + _name + "'");
                        _handleCount = 0;
                    }
                    currentCount = _handleCount;
                }
                lock (s_internalSyncObject)
                {
                    s_handleRemoved?.Invoke(_name, handle, currentCount);
                }
                return handle;
            }
        }
    }

    internal delegate void HandleChangeEventHandler(string handleType, IntPtr handleValue, int currentHandleCount);
}
