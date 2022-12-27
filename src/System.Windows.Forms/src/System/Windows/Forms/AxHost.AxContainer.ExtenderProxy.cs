// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Reflection;
using static Interop;
using static Interop.Ole32;
using Com = Windows.Win32.System.Com;

namespace System.Windows.Forms;

public abstract partial class AxHost
{
    internal partial class AxContainer
    {
        private class ExtenderProxy :
            Oleaut32.IExtender,
            IVBGetControl,
            IGetVBAObject,
            IGetOleObject,
            IReflect
        {
            private readonly WeakReference<Control> _principal;
            private readonly WeakReference<AxContainer> _container;

            internal ExtenderProxy(Control principal, AxContainer container)
            {
                _principal = new(principal);
                _container = new(container);
            }

            private Control? GetPrincipal()
            {
                _principal.TryGetTarget(out Control? target);
                return target;
            }

            private AxContainer? GetContainer()
            {
                _container.TryGetTarget(out AxContainer? container);
                return container;
            }

            HRESULT IVBGetControl.EnumControls(OLECONTF dwOleContF, GC_WCH dwWhich, out Com.IEnumUnknown.Interface ppenum)
            {
                s_axHTraceSwitch.TraceVerbose("in EnumControls for proxy");
                ppenum = GetContainer() is { } container && GetPrincipal() is { } principal
                    ? container.EnumControls(principal, dwOleContF, dwWhich)
                    : new EnumUnknown(null);
                return HRESULT.S_OK;
            }

            unsafe HRESULT IGetOleObject.GetOleObject(Guid* riid, out object? ppvObj)
            {
                s_axHTraceSwitch.TraceVerbose("in GetOleObject for proxy");
                ppvObj = null;
                if (riid is null || !riid->Equals(s_ioleobject_Guid))
                {
                    return HRESULT.E_INVALIDARG;
                }

                if (GetPrincipal() is AxHost hostControl)
                {
                    ppvObj = hostControl.GetOcx();
                    return HRESULT.S_OK;
                }

                return HRESULT.E_FAIL;
            }

            unsafe HRESULT IGetVBAObject.GetObject(Guid* riid, IVBFormat?[] rval, uint dwReserved)
            {
                s_axHTraceSwitch.TraceVerbose("in GetObject for proxy");
                if (rval is null || riid is null || rval.Length == 0)
                {
                    return HRESULT.E_INVALIDARG;
                }

                if (!riid->Equals(s_ivbformat_Guid))
                {
                    rval[0] = null;
                    return HRESULT.E_NOINTERFACE;
                }

                rval[0] = new VBFormat();
                return HRESULT.S_OK;
            }

            public int Align
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getAlign for proxy for {GetPrincipal()}");
                    int result = (int)(GetPrincipal()?.Dock ?? NativeMethods.ActiveX.ALIGN_NO_CHANGE);
                    if (result is < NativeMethods.ActiveX.ALIGN_MIN or > NativeMethods.ActiveX.ALIGN_MAX)
                    {
                        result = NativeMethods.ActiveX.ALIGN_NO_CHANGE;
                    }

                    return result;
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setAlign for proxy for {GetPrincipal()} {value}");
                    if (GetPrincipal() is { } control)
                    {
                        control.Dock = (DockStyle)value;
                    }
                }
            }

            public uint BackColor
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getBackColor for proxy for {GetPrincipal()}");
                    return GetOleColorFromColor(GetPrincipal()?.BackColor ?? default);
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setBackColor for proxy for {GetPrincipal()} {value}");
                    if (GetPrincipal() is { } control)
                    {
                        control.BackColor = GetColorFromOleColor(value);
                    }
                }
            }

            public BOOL Enabled
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getEnabled for proxy for {GetPrincipal()}");
                    return GetPrincipal()?.Enabled ?? BOOL.FALSE;
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setEnabled for proxy for {GetPrincipal()} {value}");
                    if (GetPrincipal() is { } control)
                    {
                        control.Enabled = value;
                    }
                }
            }

            public uint ForeColor
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getForeColor for proxy for {GetPrincipal()}");
                    return GetOleColorFromColor(GetPrincipal()?.ForeColor ?? default);
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setForeColor for proxy for {GetPrincipal()} {value}");
                    if (GetPrincipal() is { } control)
                    {
                        control.ForeColor = GetColorFromOleColor(value);
                    }
                }
            }

            public int Height
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getHeight for proxy for {GetPrincipal()}");
                    return Pixel2Twip(GetPrincipal()?.Height ?? 0, xDirection: false);
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setHeight for proxy for {GetPrincipal()} {Twip2Pixel(value, false)}");
                    if (GetPrincipal() is { } control)
                    {
                        control.Height = Twip2Pixel(value, xDirection: false);
                    }
                }
            }

            public int Left
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getLeft for proxy for {GetPrincipal()}");
                    return Pixel2Twip(GetPrincipal()?.Left ?? 0, xDirection: true);
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setLeft for proxy for {GetPrincipal()} {Twip2Pixel(value, true)}");
                    if (GetPrincipal() is { } control)
                    {
                        control.Left = Twip2Pixel(value, xDirection: true);
                    }
                }
            }

            public object? Parent
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getParent for proxy for {GetPrincipal()}");
                    return GetContainer() is { } container ? container.GetProxyForControl(container._parent) : null;
                }
            }

            public short TabIndex
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getTabIndex for proxy for {GetPrincipal()}");
                    return (short)(GetPrincipal()?.TabIndex ?? 0);
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setTabIndex for proxy for {GetPrincipal()} {value}");
                    if (GetPrincipal() is { } control)
                    {
                        control.TabIndex = value;
                    }
                }
            }

            public BOOL TabStop
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getTabStop for proxy for {GetPrincipal()}");
                    return GetPrincipal()?.TabStop ?? BOOL.FALSE;
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setTabStop for proxy for {GetPrincipal()} {value}");
                    if (GetPrincipal() is { } control)
                    {
                        control.TabStop = value;
                    }
                }
            }

            public int Top
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getTop for proxy for {GetPrincipal()}");
                    return Pixel2Twip(GetPrincipal()?.Top ?? 0, xDirection: false);
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setTop for proxy for {GetPrincipal()} {Twip2Pixel(value, false)}");
                    if (GetPrincipal() is { } control)
                    {
                        control.Top = Twip2Pixel(value, xDirection: false);
                    }
                }
            }

            public BOOL Visible
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getVisible for proxy for {GetPrincipal()}");
                    return GetPrincipal()?.Visible ?? false;
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setVisible for proxy for {GetPrincipal()} {value}");
                    if (GetPrincipal() is { } control)
                    {
                        control.Visible = value;
                    }
                }
            }

            public int Width
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getWidth for proxy for {GetPrincipal()}");
                    return Pixel2Twip(GetPrincipal()?.Width ?? 0, xDirection: true);
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setWidth for proxy for {GetPrincipal()} {Twip2Pixel(value, true)}");
                    if (GetPrincipal() is { } control)
                    {
                        control.Width = Twip2Pixel(value, xDirection: true);
                    }
                }
            }

            public string Name
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getName for proxy for {GetPrincipal()}");
                    return GetPrincipal() is { } control ? GetNameForControl(control) : string.Empty;
                }
            }

            public IntPtr Hwnd
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getHwnd for proxy for {GetPrincipal()}");
                    return GetPrincipal()?.Handle ?? default;
                }
            }

            public object? Container => GetContainer();

            public string Text
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose($"in getText for proxy for {GetPrincipal()}");
                    return GetPrincipal()?.Text ?? string.Empty;
                }
                set
                {
                    s_axHTraceSwitch.TraceVerbose($"in setText for proxy for {GetPrincipal()}");
                    if (GetPrincipal() is { } control)
                    {
                        control.Text = value;
                    }
                }
            }

            public void Move(object left, object top, object width, object height)
            {
            }

            // IReflect methods:

            MethodInfo? IReflect.GetMethod(
                string name,
                BindingFlags bindingAttr,
                Binder? binder,
                Type[] types,
                ParameterModifier[]? modifiers) => null;

            MethodInfo? IReflect.GetMethod(string name, BindingFlags bindingAttr) => null;
            MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr) => new[] { GetType().GetMethod("Move")! };

            FieldInfo? IReflect.GetField(string name, BindingFlags bindingAttr) => null;
            FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr) => Array.Empty<FieldInfo>();

            PropertyInfo? IReflect.GetProperty(string name, BindingFlags bindingAttr)
            {
                PropertyInfo? property = GetPrincipal()?.GetType().GetProperty(name, bindingAttr);
                property ??= GetType().GetProperty(name, bindingAttr);
                return property;
            }

            PropertyInfo? IReflect.GetProperty(
                string name,
                BindingFlags bindingAttr,
                Binder? binder,
                Type? returnType,
                Type[] types,
                ParameterModifier[]? modifiers)
            {
                PropertyInfo? property = GetPrincipal()?.GetType().GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
                property ??= GetType().GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
                return property;
            }

            PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr)
            {
                PropertyInfo[] extenderProperties = GetType().GetProperties(bindingAttr);
                PropertyInfo[]? controlProperties = GetPrincipal()?.GetType().GetProperties(bindingAttr);

                if (extenderProperties is null)
                {
                    return controlProperties ?? Array.Empty<PropertyInfo>();
                }
                else if (controlProperties is null)
                {
                    return extenderProperties ?? Array.Empty<PropertyInfo>();
                }
                else
                {
                    int i = 0;
                    PropertyInfo[] properties = new PropertyInfo[extenderProperties.Length + controlProperties.Length];

                    foreach (PropertyInfo property in extenderProperties)
                    {
                        properties[i++] = property;
                    }

                    foreach (PropertyInfo property in controlProperties)
                    {
                        properties[i++] = property;
                    }

                    return properties;
                }
            }

            MemberInfo[] IReflect.GetMember(string name, BindingFlags bindingAttr)
            {
                MemberInfo[]? member = GetPrincipal()?.GetType().GetMember(name, bindingAttr);
                member ??= GetType().GetMember(name, bindingAttr);
                return member;
            }

            MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr)
            {
                MemberInfo[] extenderMembers = GetType().GetMembers(bindingAttr);
                MemberInfo[]? controlMembers = GetPrincipal()?.GetType().GetMembers(bindingAttr);

                if (extenderMembers is null)
                {
                    return controlMembers ?? Array.Empty<MemberInfo>();
                }
                else if (controlMembers is null)
                {
                    return extenderMembers ?? Array.Empty<MemberInfo>();
                }
                else
                {
                    MemberInfo[] members = new MemberInfo[extenderMembers.Length + controlMembers.Length];
                    Array.Copy(extenderMembers, 0, members, 0, extenderMembers.Length);
                    Array.Copy(controlMembers, 0, members, extenderMembers.Length, controlMembers.Length);
                    return members;
                }
            }

            object? IReflect.InvokeMember(
                string name,
                BindingFlags invokeAttr,
                Binder? binder,
                object? target,
                object?[]? args,
                ParameterModifier[]? modifiers,
                CultureInfo? culture,
                string[]? namedParameters)
            {
                try
                {
                    return GetType().InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
                }
                catch (MissingMethodException)
                {
                    return GetPrincipal()?.GetType().InvokeMember(name, invokeAttr, binder, GetPrincipal(), args, modifiers, culture, namedParameters);
                }
            }

            Type IReflect.UnderlyingSystemType
            {
                get
                {
                    s_axHTraceSwitch.TraceVerbose("In UnderlyingSystemType");
                    return null!;
                }
            }
        }
    }
}
