// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Globalization;

namespace System.Windows.Forms
{
    public abstract partial class AxHost
    {
        [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
        public sealed class TypeLibraryTimeStampAttribute : Attribute
        {
            private readonly DateTime val;

            public TypeLibraryTimeStampAttribute(string timestamp)
            {
                val = DateTime.Parse(timestamp, CultureInfo.InvariantCulture);
            }

            public DateTime Value
            {
                get
                {
                    return val;
                }
            }
        }
    }
}
