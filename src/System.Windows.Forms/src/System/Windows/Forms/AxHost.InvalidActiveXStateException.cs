// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public abstract partial class AxHost
{
    public class InvalidActiveXStateException : Exception
    {
        private readonly string? _name;
        private readonly ActiveXInvokeKind _kind;

        public InvalidActiveXStateException(string? name, ActiveXInvokeKind kind)
        {
            _name = name;
            _kind = kind;
        }

        public InvalidActiveXStateException()
        {
        }

        public override string ToString() => _kind switch
        {
            ActiveXInvokeKind.MethodInvoke => string.Format(SR.AXInvalidMethodInvoke, _name),
            ActiveXInvokeKind.PropertyGet => string.Format(SR.AXInvalidPropertyGet, _name),
            ActiveXInvokeKind.PropertySet => string.Format(SR.AXInvalidPropertySet, _name),
            _ => base.ToString(),
        };
    }
}
