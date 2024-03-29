﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

[AttributeUsage(AttributeTargets.All)]
internal sealed class SRCategoryAttribute : CategoryAttribute
{
    public SRCategoryAttribute(string category) : base(category)
    {
    }

    protected override string? GetLocalizedString(string value)
    {
        return SR.GetResourceString(value);
    }
}
