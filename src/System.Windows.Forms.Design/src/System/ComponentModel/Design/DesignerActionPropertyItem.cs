// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design;

public sealed class DesignerActionPropertyItem : DesignerActionItem
{
    public DesignerActionPropertyItem(string memberName, string? displayName, string? category, string? description)
        : base(displayName, category, description)
    {
        MemberName = memberName;
    }

    public DesignerActionPropertyItem(string memberName, string? displayName)
        : this(memberName, displayName, category: null, description: null)
    {
    }

    public DesignerActionPropertyItem(string memberName, string? displayName, string? category)
        : this(memberName, displayName, category, description: null)
    {
    }

    public string MemberName { get; }

    public IComponent? RelatedComponent { get; set; }
}
