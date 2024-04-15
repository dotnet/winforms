// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
namespace System.Windows.Forms.Design;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Class | AttributeTargets.Method)]
internal sealed class SRDisplayNameAttribute : DisplayNameAttribute
{
    private bool _replaced;

    public SRDisplayNameAttribute(string displayName)
        : base(displayName)
    {
    }

    public override string DisplayName
    {
        get
        {
            if (!_replaced)
            {
                _replaced = true;
                DisplayNameValue = SR.GetResourceString(base.DisplayName);
            }

            return base.DisplayName;
        }
    }
}
