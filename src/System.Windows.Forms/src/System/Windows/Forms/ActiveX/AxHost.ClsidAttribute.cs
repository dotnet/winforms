// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public abstract partial class AxHost
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ClsidAttribute : Attribute
    {
        public ClsidAttribute(string clsid)
        {
            Value = clsid;
        }

        public string Value { get; }
    }
}
