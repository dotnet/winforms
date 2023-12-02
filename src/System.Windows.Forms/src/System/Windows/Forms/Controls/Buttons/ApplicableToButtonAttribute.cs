// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

[AttributeUsage(AttributeTargets.All)]
internal sealed class ApplicableToButtonAttribute : Attribute
{
    public ApplicableToButtonAttribute()
    {
    }
}
