// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public abstract partial class AxHost
    {
        public class ConnectionPointCookie
        {
            private Ole32.IConnectionPoint? _connectionPoint;
            private uint _cookie;
            internal int _threadId;

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
                        if (cpc.FindConnectionPoint(&tmp, out _connectionPoint) != HRESULT.Values.S_OK)
                        {
                            _connectionPoint = null;
                        }
                    }
                    catch
                    {
                        _connectionPoint = null;
                    }

                    if (_connectionPoint is null)
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
                        HRESULT hr = _connectionPoint.Advise(sink, &tempCookie);
                        if (hr == HRESULT.Values.S_OK)
                        {
                            _cookie = tempCookie;
                            _threadId = Environment.CurrentManagedThreadId;
                        }
                        else
                        {
                            _cookie = 0;
                            Marshal.ReleaseComObject(_connectionPoint);
                            _connectionPoint = null;
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

                if (_connectionPoint is null || _cookie == 0)
                {
                    if (_connectionPoint is not null)
                    {
                        Marshal.ReleaseComObject(_connectionPoint);
                    }

                    if (throwException)
                    {
                        throw new ArgumentException(string.Format(SR.AXNoConnectionPoint, eventInterface.Name));
                    }
                }
            }

            /// <summary>
            ///  Disconnect the current connection point.  If the object is not connected,
            ///  this method will do nothing.
            /// </summary>
            public void Disconnect()
            {
                if (_connectionPoint is not null && _cookie != 0)
                {
                    try
                    {
                        _connectionPoint.Unadvise(_cookie);
                    }
                    catch (Exception ex) when (!ClientUtils.IsCriticalException(ex))
                    {
                    }
                    finally
                    {
                        _cookie = 0;
                    }

                    try
                    {
                        Marshal.ReleaseComObject(_connectionPoint);
                    }
                    catch (Exception ex) when (!ClientUtils.IsCriticalException(ex))
                    {
                    }
                    finally
                    {
                        _connectionPoint = null;
                    }
                }
            }

            ~ConnectionPointCookie()
            {
                if (_connectionPoint is not null && _cookie != 0)
                {
                    if (!AppDomain.CurrentDomain.IsFinalizingForUnload())
                    {
                        SynchronizationContext? context = SynchronizationContext.Current;
                        context?.Post(new SendOrPostCallback(AttemptDisconnect), null);
                    }
                }
            }

            void AttemptDisconnect(object? trash)
            {
                if (_threadId == Environment.CurrentManagedThreadId)
                {
                    Disconnect();
                }
                else
                {
                    Debug.Fail("Attempted to disconnect ConnectionPointCookie from the wrong thread (finalizer).");
                }
            }

            internal bool Connected => _connectionPoint is not null && _cookie != 0;
        }
    }
}
