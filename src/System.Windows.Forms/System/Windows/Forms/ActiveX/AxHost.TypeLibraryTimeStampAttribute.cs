// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace System.Windows.Forms;

public abstract partial class AxHost
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class TypeLibraryTimeStampAttribute : Attribute
    {
        public TypeLibraryTimeStampAttribute(string timestamp)
        {
            Value = DateTime.Parse(timestamp, CultureInfo.InvariantCulture);
        }

        public DateTime Value { get; }
    }
}
