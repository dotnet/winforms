// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public partial class Control
    {
private unsafe partial class ActiveXImpl
        {
            /// <summary>
            ///  Helper class. Calls IConnectionPoint.Advise to hook up a native COM event sink
            ///  to a manage .NET event interface.
            ///  The events are exposed to COM by the CLR-supplied COM-callable Wrapper (CCW).
            /// </summary>
            internal static class AdviseHelper
            {
                /// <summary>
                ///  Get the COM connection point container from the CLR's CCW and advise for the given event id.
                /// </summary>
                public static bool AdviseConnectionPoint(object connectionPoint, object sink, Type eventInterface, out uint pdwCookie)
                {
                    // Note that we cannot simply cast the connectionPoint object to
                    // System.Runtime.InteropServices.ComTypes.IConnectionPointContainer because the .NET
                    // object doesn't implement it directly. When the object is exposed to COM, the CLR
                    // implements IConnectionPointContainer on the proxy object called the CCW or COM-callable wrapper.
                    // We use the helper class ComConnectionPointContainer to get to the CCW directly
                    // to to call the interface.
                    // It is critical to call Dispose to ensure that the IUnknown is released.

                    using (ComConnectionPointContainer cpc = new ComConnectionPointContainer(connectionPoint, true))
                    {
                        return AdviseConnectionPoint(cpc, sink, eventInterface, out pdwCookie);
                    }
                }

                /// <summary>
                ///  Find the COM connection point and call Advise for the given event id.
                /// </summary>
                internal static bool AdviseConnectionPoint(ComConnectionPointContainer cpc, object sink, Type eventInterface, out uint pdwCookie)
                {
                    // Note that we cannot simply cast the returned IConnectionPoint to
                    // System.Runtime.InteropServices.ComTypes.IConnectionPoint because the .NET
                    // object doesn't implement it directly. When the object is exposed to COM, the CLR
                    // implements IConnectionPoint for the proxy object via the CCW or COM-callable wrapper.
                    // We use the helper class ComConnectionPoint to get to the CCW directly to to call the interface.
                    // It is critical to call Dispose to ensure that the IUnknown is released.
                    using (ComConnectionPoint cp = cpc.FindConnectionPoint(eventInterface))
                    {
                        using (SafeIUnknown punkEventsSink = new SafeIUnknown(sink, true))
                        {
                            // Finally...we can call IConnectionPoint.Advise to hook up a native COM event sink
                            // to a managed .NET event interface.
                            return cp.Advise(punkEventsSink.DangerousGetHandle(), out pdwCookie);
                        }
                    }
                }

                /// <summary>
                ///  Wraps a native IUnknown in a SafeHandle.
                /// </summary>
                internal class SafeIUnknown : SafeHandle
                {
                    /// <summary>
                    ///  Wrap an incoming unknown or get the unknown for the CCW (COM-callable wrapper).
                    /// </summary>
                    public SafeIUnknown(object obj, bool addRefIntPtr)
                        : this(obj, addRefIntPtr, Guid.Empty)
                    {
                    }

                    /// <summary>
                    ///  Wrap an incoming unknown or get the unknown for the CCW (COM-callable wrapper).
                    ///  If an iid is supplied, QI for the interface and wrap that unknown instead.
                    /// </summary>
                    public SafeIUnknown(object obj, bool addRefIntPtr, Guid iid)
                        : base(IntPtr.Zero, true)
                    {
#pragma warning disable SYSLIB0004 // Type or member is obsolete
                        RuntimeHelpers.PrepareConstrainedRegions();
#pragma warning restore SYSLIB0004 // Type or member is obsolete
                        try
                        {
                            // Set this.handle in a finally block to ensure the com ptr is set in the SafeHandle
                            // even if the runtime throws a exception (such as ThreadAbortException) during the call.
                            // This ensures that the finalizer will clean up the COM reference.
                        }
                        finally
                        {
                            // Get a raw IUnknown for this object.
                            // We are responsible for releasing the IUnknown ourselves.
                            IntPtr unknown;

                            if (obj is IntPtr)
                            {
                                unknown = (IntPtr)obj;

                                // The incoming IntPtr may already be reference counted or not, depending on
                                // where it came from. The caller needs to tell us whether to add-ref or not.
                                if (addRefIntPtr)
                                {
                                    Marshal.AddRef(unknown);
                                }
                            }
                            else
                            {
                                // GetIUnknownForObject will return a reference-counted object
                                unknown = Marshal.GetIUnknownForObject(obj);
                            }

                            // Attempt QueryInterface if an iid is specified.
                            if (iid != Guid.Empty)
                            {
                                IntPtr oldUnknown = unknown;
                                try
                                {
                                    unknown = InternalQueryInterface(unknown, ref iid);
                                }
                                finally
                                {
                                    // It is critical to release the original unknown if
                                    // InternalQueryInterface throws out so we don't leak ref counts.
                                    Marshal.Release(oldUnknown);
                                }
                            }

                            // Preserve the com ptr in the SafeHandle.
                            handle = unknown;
                        }
                    }

                    /// <summary>
                    ///  Helper function for QueryInterface.
                    /// </summary>
                    private static IntPtr InternalQueryInterface(IntPtr pUnk, ref Guid iid)
                    {
                        int hresult = Marshal.QueryInterface(pUnk, ref iid, out IntPtr ppv);
                        if (hresult != 0 || ppv == IntPtr.Zero)
                        {
                            throw new InvalidCastException(SR.AxInterfaceNotSupported);
                        }

                        return ppv;
                    }

                    /// <summary>
                    ///  Return whether the handle is invalid.
                    /// </summary>
                    public sealed override bool IsInvalid
                    {
                        get
                        {
                            if (!IsClosed)
                            {
                                return (IntPtr.Zero == handle);
                            }

                            return true;
                        }
                    }

                    /// <summary>
                    ///  Release the IUnknown.
                    /// </summary>
                    protected sealed override bool ReleaseHandle()
                    {
                        IntPtr ptr1 = handle;
                        handle = IntPtr.Zero;
                        if (IntPtr.Zero != ptr1)
                        {
                            Marshal.Release(ptr1);
                        }

                        return true;
                    }

                    /// <summary>
                    ///  Helper function to load a COM v-table from a com object pointer.
                    /// </summary>
                    protected V LoadVtable<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]V>()
                        where V : struct
                    {
                        IntPtr vtblptr = Marshal.ReadIntPtr(handle, 0);
                        return Marshal.PtrToStructure<V>(vtblptr);
                    }
                }

                /// <summary>
                ///  Helper class to access IConnectionPointContainer from a .NET COM-callable wrapper.
                ///  The IConnectionPointContainer COM pointer is wrapped in a SafeHandle.
                /// </summary>
                internal sealed class ComConnectionPointContainer
                    : SafeIUnknown
                {
                    public ComConnectionPointContainer(object obj, bool addRefIntPtr)
                        : base(obj, addRefIntPtr, typeof(Ole32.IConnectionPointContainer).GUID)
                    {
                        _vtbl = LoadVtable<VTABLE>();
                    }

                    private readonly VTABLE _vtbl;

                    [StructLayout(LayoutKind.Sequential)]
                    private struct VTABLE
                    {
                        public IntPtr QueryInterfacePtr;
                        public IntPtr AddRefPtr;
                        public IntPtr ReleasePtr;
                        public IntPtr EnumConnectionPointsPtr;
                        public IntPtr FindConnectionPointPtr;
                    }

                    /// <summary>
                    ///  Call IConnectionPointContainer.FindConnectionPoint using Delegate.Invoke on the v-table slot.
                    /// </summary>
                    public ComConnectionPoint FindConnectionPoint(Type eventInterface)
                    {
                        FindConnectionPointD findConnectionPoint = (FindConnectionPointD)Marshal.GetDelegateForFunctionPointer(_vtbl.FindConnectionPointPtr, typeof(FindConnectionPointD));

                        Guid iid = eventInterface.GUID;
                        int hresult = findConnectionPoint.Invoke(handle, ref iid, out IntPtr result);
                        if (hresult != 0 || result == IntPtr.Zero)
                        {
                            throw new ArgumentException(string.Format(SR.AXNoConnectionPoint, eventInterface.Name));
                        }

                        return new ComConnectionPoint(result, false);   // result is already ref-counted as an out-param so pass in false
                    }

                    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                    private delegate int FindConnectionPointD(IntPtr This, ref Guid iid, out IntPtr ppv);
                }

                /// <summary>
                ///  Helper class to access IConnectionPoint from a .NET COM-callable wrapper.
                ///  The IConnectionPoint COM pointer is wrapped in a SafeHandle.
                /// </summary>
                internal sealed class ComConnectionPoint
                    : SafeIUnknown
                {
                    public ComConnectionPoint(object obj, bool addRefIntPtr)
                        : base(obj, addRefIntPtr, typeof(Ole32.IConnectionPoint).GUID)
                    {
                        _vtbl = LoadVtable<VTABLE>();
                    }

                    [StructLayout(LayoutKind.Sequential)]
                    private struct VTABLE
                    {
                        public IntPtr QueryInterfacePtr;
                        public IntPtr AddRefPtr;
                        public IntPtr ReleasePtr;
                        public IntPtr GetConnectionInterfacePtr;
                        public IntPtr GetConnectionPointContainerPtr;
                        public IntPtr AdvisePtr;
                        public IntPtr UnadvisePtr;
                        public IntPtr EnumConnectionsPtr;
                    }

                    private readonly VTABLE _vtbl;

                    /// <summary>
                    ///  Call IConnectionPoint.Advise using Delegate.Invoke on the v-table slot.
                    /// </summary>
                    public bool Advise(IntPtr punkEventSink, out uint pdwCookie)
                    {
                        AdviseD advise = (AdviseD)Marshal.GetDelegateForFunctionPointer(_vtbl.AdvisePtr, typeof(AdviseD));
                        if (advise.Invoke(handle, punkEventSink, out pdwCookie) == 0)
                        {
                            return true;
                        }

                        return false;
                    }

                    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                    private delegate int AdviseD(IntPtr This, IntPtr punkEventSink, out uint pdwCookie);
                }
            }
        }
    }
}
