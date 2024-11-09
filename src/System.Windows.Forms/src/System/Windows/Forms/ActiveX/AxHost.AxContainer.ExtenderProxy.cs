// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;

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
            UnknownDispatch,
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
                uint dwOleContF,
                ENUM_CONTROLS_WHICH_FLAGS dwWhich,
                IEnumUnknown** ppenum)
            {
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
                    // VB6 passes this back as the IOleObject interface.
                    *ppvObj = ComHelpers.GetComPointer<IOleObject>(hostControl.GetOcx());
                    return HRESULT.S_OK;
                }

                return HRESULT.E_FAIL;
            }

            HRESULT IGetVBAObject.Interface.GetObject(Guid* riid, void** ppvObj, uint dwReserved)
            {
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
                    DockStyle result = GetControl()?.Dock ?? DockStyle.None;
                    if (result is < DockStyle.None or > DockStyle.Right)
                    {
                        result = DockStyle.None;
                    }

                    return (int)result;
                }
                set
                {
                    if (GetControl() is { } control)
                    {
                        control.Dock = (DockStyle)value;
                    }
                }
            }

            public uint BackColor
            {
                get => GetOleColorFromColor(GetControl()?.BackColor ?? default);
                set
                {
                    if (GetControl() is { } control)
                    {
                        control.BackColor = GetColorFromOleColor(value);
                    }
                }
            }

            public BOOL Enabled
            {
                get => GetControl()?.Enabled ?? BOOL.FALSE;
                set
                {
                    if (GetControl() is { } control)
                    {
                        control.Enabled = value;
                    }
                }
            }

            public uint ForeColor
            {
                get => GetOleColorFromColor(GetControl()?.ForeColor ?? default);
                set
                {
                    if (GetControl() is { } control)
                    {
                        control.ForeColor = GetColorFromOleColor(value);
                    }
                }
            }

            public int Height
            {
                get => Pixel2Twip(GetControl()?.Height ?? 0, xDirection: false);
                set
                {
                    if (GetControl() is { } control)
                    {
                        control.Height = Twip2Pixel(value, xDirection: false);
                    }
                }
            }

            public int Left
            {
                get => Pixel2Twip(GetControl()?.Left ?? 0, xDirection: true);
                set
                {
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
                    IExtender.Interface? extender = GetContainer() is { } container
                        ? container.GetExtenderProxyForControl(container._parent)
                        : null;

                    return extender is null ? null : ComHelpers.GetComPointer<IUnknown>(extender);
                }
            }

            public short TabIndex
            {
                get => (short)(GetControl()?.TabIndex ?? 0);
                set
                {
                    if (GetControl() is { } control)
                    {
                        control.TabIndex = value;
                    }
                }
            }

            public BOOL TabStop
            {
                get => GetControl()?.TabStop ?? BOOL.FALSE;
                set
                {
                    if (GetControl() is { } control)
                    {
                        control.TabStop = value;
                    }
                }
            }

            public int Top
            {
                get => Pixel2Twip(GetControl()?.Top ?? 0, xDirection: false);
                set
                {
                    if (GetControl() is { } control)
                    {
                        control.Top = Twip2Pixel(value, xDirection: false);
                    }
                }
            }

            public BOOL Visible
            {
                get => GetControl()?.Visible ?? false;
                set
                {
                    if (GetControl() is { } control)
                    {
                        control.Visible = value;
                    }
                }
            }

            public int Width
            {
                get => Pixel2Twip(GetControl()?.Width ?? 0, xDirection: true);
                set
                {
                    if (GetControl() is { } control)
                    {
                        control.Width = Twip2Pixel(value, xDirection: true);
                    }
                }
            }

            public BSTR Name => new(GetControl() is { } control ? GetNameForControl(control) : string.Empty);

            public HWND Hwnd => GetControl()?.HWND ?? HWND.Null;

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
                get => GetControl()?.Text ?? string.Empty;
                set
                {
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

                *pid = PInvokeCore.DISPID_UNKNOWN;
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

                *pid = PInvokeCore.DISPID_UNKNOWN;
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
