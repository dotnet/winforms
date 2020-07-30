// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using static Interop;

namespace System.Windows.Forms
{
    public abstract partial class AxHost
    {
        public class ConnectionPointCookie
        {
            private Ole32.IConnectionPoint connectionPoint;
            private uint cookie;
            internal int threadId;
#if DEBUG
            private readonly string callStack;
#endif
            /// <summary>
            ///  Creates a connection point to of the given interface type.
            ///  which will call on a managed code sink that implements that interface.
            /// </summary>
            public ConnectionPointCookie(object source, object sink, Type eventInterface)
                : this(source, sink, eventInterface, true)
            {
            }

            internal unsafe ConnectionPointCookie(object source, object sink, Type eventInterface, bool throwException)
            {
                if (source is Ole32.IConnectionPointContainer cpc)
                {
                    try
                    {
                        Guid tmp = eventInterface.GUID;
                        if (cpc.FindConnectionPoint(&tmp, out connectionPoint) != HRESULT.S_OK)
                        {
                            connectionPoint = null;
                        }
                    }
                    catch
                    {
                        connectionPoint = null;
                    }

                    if (connectionPoint is null)
                    {
                        if (throwException)
                        {
                            throw new ArgumentException(string.Format(SR.AXNoEventInterface, eventInterface.Name));
                        }
                    }
                    else if (sink is null || !eventInterface.IsInstanceOfType(sink))
                    {
                        if (throwException)
                        {
                            throw new InvalidCastException(string.Format(SR.AXNoSinkImplementation, eventInterface.Name));
                        }
                    }
                    else
                    {
                        uint tempCookie = 0;
                        HRESULT hr = connectionPoint.Advise(sink, &tempCookie);
                        if (hr == HRESULT.S_OK)
                        {
                            cookie = tempCookie;
                            threadId = Thread.CurrentThread.ManagedThreadId;
                        }
                        else
                        {
                            cookie = 0;
                            Marshal.ReleaseComObject(connectionPoint);
                            connectionPoint = null;
                            if (throwException)
                            {
                                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, string.Format(SR.AXNoSinkAdvise, eventInterface.Name), hr));
                            }
                        }
                    }
                }
                else
                {
                    if (throwException)
                    {
                        throw new InvalidCastException(SR.AXNoConnectionPointContainer);
                    }
                }

                if (connectionPoint is null || cookie == 0)
                {
                    if (connectionPoint != null)
                    {
                        Marshal.ReleaseComObject(connectionPoint);
                    }

                    if (throwException)
                    {
                        throw new ArgumentException(string.Format(SR.AXNoConnectionPoint, eventInterface.Name));
                    }
                }
#if DEBUG
                callStack = Environment.StackTrace;
#endif
            }

            /// <summary>
            ///  Disconnect the current connection point.  If the object is not connected,
            ///  this method will do nothing.
            /// </summary>
            public void Disconnect()
            {
                if (connectionPoint != null && cookie != 0)
                {
                    try
                    {
                        connectionPoint.Unadvise(cookie);
                    }
                    catch (Exception ex) when (!ClientUtils.IsCriticalException(ex))
                    {
                    }
                    finally
                    {
                        cookie = 0;
                    }

                    try
                    {
                        Marshal.ReleaseComObject(connectionPoint);
                    }
                    catch (Exception ex) when (!ClientUtils.IsCriticalException(ex))
                    {
                    }
                    finally
                    {
                        connectionPoint = null;
                    }
                }
            }

            ~ConnectionPointCookie()
            {
                if (connectionPoint != null && cookie != 0)
                {
                    if (!AppDomain.CurrentDomain.IsFinalizingForUnload())
                    {
                        SynchronizationContext context = SynchronizationContext.Current;
                        context?.Post(new SendOrPostCallback(AttemptDisconnect), null);
                    }
                }
            }

            void AttemptDisconnect(object trash)
            {
                if (threadId == Thread.CurrentThread.ManagedThreadId)
                {
                    Disconnect();
                }
                else
                {
                    Debug.Fail("Attempted to disconnect ConnectionPointCookie from the wrong thread (finalizer).");
                }
            }

            internal bool Connected
            {
                get
                {
                    return connectionPoint != null && cookie != 0;
                }
            }
        }
    }
}
