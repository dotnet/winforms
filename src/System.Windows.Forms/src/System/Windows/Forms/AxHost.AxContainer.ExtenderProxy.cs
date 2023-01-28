// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms;

public abstract partial class AxHost
{
    internal partial class AxContainer
    {
        /// <summary>
        ///  Provides an <see cref="IDispatch"/> and <see cref="IDispatchEx"/> view of <see cref="Control"/>
        ///  with added properties.
        /// </summary>
        internal unsafe class ExtenderProxy :
            StandardDispatch,
            IExtender.Interface,
            IVBGetControl.Interface,
            IGetVBAObject.Interface,
            IGetOleObject.Interface,
            IManagedWrapper<IDispatch, IDispatchEx, IExtender, IVBGetControl, IGetVBAObject, IGetOleObject>
        {
            private readonly WeakReference<Control> _control;
            private readonly WeakReference<AxContainer> _container;
            private readonly ClassPropertyDispatchAdapter _dispatchAdapter;

            internal ExtenderProxy(Control control, AxContainer container)
            {
                _control = new(control);

                // We want the proxy to override anything we find in the control.
                _dispatchAdapter = new(control, priorAdapter: new(this));
                _container = new(container);
            }

            private Control? GetControl()
            {
                _control.TryGetTarget(out Control? target);
                return target;
            }

            private AxContainer? GetContainer()
            {
                _container.TryGetTarget(out AxContainer? container);
                return container;
            }

            HRESULT IVBGetControl.Interface.EnumControls(
                OLECONTF dwOleContF,
                ENUM_CONTROLS_WHICH_FLAGS dwWhich,
                IEnumUnknown** ppenum)
            {
                s_axHTraceSwitch.TraceVerbose("in EnumControls for proxy");
                if (ppenum is null)
                {
                    return HRESULT.E_POINTER;
                }

                IEnumUnknown.Interface enumUnknown = GetContainer() is { } container && GetControl() is { } control
                    ? container.EnumControls(control, dwOleContF, dwWhich)
                    : new EnumUnknown(null);
                *ppenum = ComHelpers.GetComPointer<IEnumUnknown>(enumUnknown);
                return HRESULT.S_OK;
            }

            HRESULT IGetOleObject.Interface.GetOleObject(Guid* riid, void** ppvObj)
            {
                s_axHTraceSwitch.TraceVerbose("in GetOleObject for proxy");
                if (ppvObj is null)
                {
                    return HRESULT.E_POINTER;
                }

                *ppvObj = null;
                if (riid is null || !riid->Equals(s_ioleobject_Guid))
                {
                    return HRESULT.E_INVALIDARG;
                }

                if (GetControl() is AxHost hostControl)
                {
                    *ppvObj = ComHelpers.GetComPointer<IUnknown>(hostControl.GetOcx());
                    return HRESULT.S_OK;
                }

                return HRESULT.E_FAIL;
            }

            HRESULT IGetVBAObject.Interface.GetObject(Guid* riid, void** ppvObj, uint dwReserved)
            {
                s_axHTraceSwitch.TraceVerbose("in GetObject for proxy");
                if (ppvObj is null || riid is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                if (!riid->Equals(s_ivbformat_Guid))
                {
                    *ppvObj = null;
                    return HRESULT.E_NOINTERFACE;
                }

                *ppvObj = ComHelpers.GetComPointer<IVBFormat>(new VBFormat());
                return HRESULT.S_OK;
            }

            public int Align
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getAlign for proxy for {GetControl()}");
                    int result = (int)(GetControl()?.Dock ?? NativeMethods.ActiveX.ALIGN_NO_CHANGE);
                    if (result is < NativeMethods.ActiveX.ALIGN_MIN or > NativeMethods.ActiveX.ALIGN_MAX)
                    {
                        result = NativeMethods.ActiveX.ALIGN_NO_CHANGE;
                    }

                    return result;
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setAlign for proxy for {GetControl()} {value}");
                    if (GetControl() is { } control)
                    {
                        control.Dock = (DockStyle)value;
                    }
                }
            }

            public uint BackColor
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getBackColor for proxy for {GetControl()}");
                    return GetOleColorFromColor(GetControl()?.BackColor ?? default);
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setBackColor for proxy for {GetControl()} {value}");
                    if (GetControl() is { } control)
                    {
                        control.BackColor = GetColorFromOleColor(value);
                    }
                }
            }

            public BOOL Enabled
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getEnabled for proxy for {GetControl()}");
                    return GetControl()?.Enabled ?? BOOL.FALSE;
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setEnabled for proxy for {GetControl()} {value}");
                    if (GetControl() is { } control)
                    {
                        control.Enabled = value;
                    }
                }
            }

            public uint ForeColor
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getForeColor for proxy for {GetControl()}");
                    return GetOleColorFromColor(GetControl()?.ForeColor ?? default);
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setForeColor for proxy for {GetControl()} {value}");
                    if (GetControl() is { } control)
                    {
                        control.ForeColor = GetColorFromOleColor(value);
                    }
                }
            }

            public int Height
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getHeight for proxy for {GetControl()}");
                    return Pixel2Twip(GetControl()?.Height ?? 0, xDirection: false);
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setHeight for proxy for {GetControl()} {Twip2Pixel(value, false)}");
                    if (GetControl() is { } control)
                    {
                        control.Height = Twip2Pixel(value, xDirection: false);
                    }
                }
            }

            public int Left
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getLeft for proxy for {GetControl()}");
                    return Pixel2Twip(GetControl()?.Left ?? 0, xDirection: true);
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setLeft for proxy for {GetControl()} {Twip2Pixel(value, true)}");
                    if (GetControl() is { } control)
                    {
                        control.Left = Twip2Pixel(value, xDirection: true);
                    }
                }
            }

            public IUnknown* Parent
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getParent for proxy for {GetControl()}");
                    IExtender.Interface? extender = GetContainer() is { } container
                        ? container.GetExtenderProxyForControl(container._parent)
                        : null;

                    return extender is null ? null : ComHelpers.GetComPointer<IUnknown>(extender);
                }
            }

            public short TabIndex
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getTabIndex for proxy for {GetControl()}");
                    return (short)(GetControl()?.TabIndex ?? 0);
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setTabIndex for proxy for {GetControl()} {value}");
                    if (GetControl() is { } control)
                    {
                        control.TabIndex = value;
                    }
                }
            }

            public BOOL TabStop
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getTabStop for proxy for {GetControl()}");
                    return GetControl()?.TabStop ?? BOOL.FALSE;
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setTabStop for proxy for {GetControl()} {value}");
                    if (GetControl() is { } control)
                    {
                        control.TabStop = value;
                    }
                }
            }

            public int Top
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getTop for proxy for {GetControl()}");
                    return Pixel2Twip(GetControl()?.Top ?? 0, xDirection: false);
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setTop for proxy for {GetControl()} {Twip2Pixel(value, false)}");
                    if (GetControl() is { } control)
                    {
                        control.Top = Twip2Pixel(value, xDirection: false);
                    }
                }
            }

            public BOOL Visible
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getVisible for proxy for {GetControl()}");
                    return GetControl()?.Visible ?? false;
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setVisible for proxy for {GetControl()} {value}");
                    if (GetControl() is { } control)
                    {
                        control.Visible = value;
                    }
                }
            }

            public int Width
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getWidth for proxy for {GetControl()}");
                    return Pixel2Twip(GetControl()?.Width ?? 0, xDirection: true);
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setWidth for proxy for {GetControl()} {Twip2Pixel(value, true)}");
                    if (GetControl() is { } control)
                    {
                        control.Width = Twip2Pixel(value, xDirection: true);
                    }
                }
            }

            public BSTR Name
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getName for proxy for {GetControl()}");
                    return new(GetControl() is { } control ? GetNameForControl(control) : string.Empty);
                }
            }

            public HWND Hwnd
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getHwnd for proxy for {GetControl()}");
                    return GetControl()?.HWND ?? HWND.Null;
                }
            }

            public IUnknown* Container
            {
                get
                {
                    AxContainer? container = GetContainer();
                    return container is null ? null : ComHelpers.GetComPointer<IUnknown>(container);
                }
            }

            public string Text
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getText for proxy for {GetControl()}");
                    return GetControl()?.Text ?? string.Empty;
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setText for proxy for {GetControl()}");
                    if (GetControl() is { } control)
                    {
                        control.Text = value;
                    }
                }
            }

            public void Move(void* left, void* top, void* width, void* height)
            {
            }

            // Dispatch support used to be provided via IReflect. The following mostly replicates the legacy behavior
            // with the small exception of leaving out exposing "Move", which didn't do anything anyway. If it is found
            // to be necessary, it can be hacked in again.
            //
            // Note that the old code returned all members through IReflect.GetMembers, but that is never called by
            // the IReflect CCW dispatch projection.

            protected override HRESULT GetDispID(BSTR bstrName, uint grfdex, int* pid)
            {
                if (_dispatchAdapter.TryGetDispID(bstrName.ToString(), out int dispid))
                {
                    *pid = dispid;
                    return HRESULT.S_OK;
                }

                *pid = PInvoke.DISPID_UNKNOWN;
                return HRESULT.DISP_E_UNKNOWNNAME;
            }

            protected override HRESULT GetMemberName(int id, BSTR* pbstrName)
            {
                if (_dispatchAdapter.TryGetMemberName(id, out string? name))
                {
                    *pbstrName = new(name);
                    return HRESULT.S_OK;
                }

                *pbstrName = default;
                return HRESULT.DISP_E_UNKNOWNNAME;
            }

            protected override HRESULT GetNextDispID(uint grfdex, int id, int* pid)
            {
                if (_dispatchAdapter.TryGetNextDispId(id, out int dispId))
                {
                    *pid = dispId;
                    return HRESULT.S_OK;
                }

                *pid = PInvoke.DISPID_UNKNOWN;
                return HRESULT.S_FALSE;
            }

            protected override HRESULT Invoke(
                int dispId,
                uint lcid,
                DISPATCH_FLAGS flags,
                DISPPARAMS* parameters,
                VARIANT* result,
                EXCEPINFO* exceptionInfo,
                uint* argumentError) => _dispatchAdapter.Invoke(dispId, lcid, flags, parameters, result);

            protected override HRESULT GetMemberProperties(int dispId, out FDEX_PROP_FLAGS properties)
            {
                if (_dispatchAdapter.TryGetMemberProperties(dispId, out properties))
                {
                    return HRESULT.S_OK;
                }

                properties = default;
                return HRESULT.DISP_E_UNKNOWNNAME;
            }
        }
    }
}
