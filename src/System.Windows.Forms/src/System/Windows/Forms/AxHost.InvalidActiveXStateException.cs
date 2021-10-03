// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
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

            public override string ToString()
            {
                switch (_kind)
                {
                    case ActiveXInvokeKind.MethodInvoke:
                        return string.Format(SR.AXInvalidMethodInvoke, _name);
                    case ActiveXInvokeKind.PropertyGet:
                        return string.Format(SR.AXInvalidPropertyGet, _name);
                    case ActiveXInvokeKind.PropertySet:
                        return string.Format(SR.AXInvalidPropertySet, _name);
                    default:
                        return base.ToString();
                }
            }
        }
    }
}
