// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    public abstract partial class AxHost
    {
        [AttributeUsage(AttributeTargets.Class, Inherited = false)]
        public sealed class ClsidAttribute : Attribute
        {
            private readonly string val;

            public ClsidAttribute(string clsid)
            {
                val = clsid;
            }

            public string Value
            {
                get
                {
                    return val;
                }
            }
        }
    }
}
