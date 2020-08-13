// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    public abstract partial class AxHost
    {
        public class InvalidActiveXStateException : Exception
        {
            private readonly string name;
            private readonly ActiveXInvokeKind kind;

            public InvalidActiveXStateException(string name, ActiveXInvokeKind kind)
            {
                this.name = name;
                this.kind = kind;
            }

            public InvalidActiveXStateException()
            {
            }

            public override string ToString()
            {
                switch (kind)
                {
                    case ActiveXInvokeKind.MethodInvoke:
                        return string.Format(SR.AXInvalidMethodInvoke, name);
                    case ActiveXInvokeKind.PropertyGet:
                        return string.Format(SR.AXInvalidPropertyGet, name);
                    case ActiveXInvokeKind.PropertySet:
                        return string.Format(SR.AXInvalidPropertySet, name);
                    default:
                        return base.ToString();
                }
            }
        }
    }
}
