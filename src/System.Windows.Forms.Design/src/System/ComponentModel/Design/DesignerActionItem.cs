// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace System.ComponentModel.Design;

/// <summary>
///  A menu command that defines text and other metadata to describe a targeted task that can be performed.
///  Tasks typically walk the user through some multi-step process, such as configuring a data source for a component.
///  Designer tasks are shown in a custom piece of UI (Chrome).
/// </summary>
public abstract partial class DesignerActionItem
{
    private IDictionary? _properties;

    public DesignerActionItem(string? displayName, string? category, string? description)
    {
        DisplayName = displayName is null ? null : SanitizeNameRegex().Replace(displayName, "");
        Category = category;
        Description = description;
    }

    [GeneratedRegex(@"\(\&.\)")]
    private static partial Regex SanitizeNameRegex();

    public bool AllowAssociate { get; set; }

    public virtual string? Category { get; }

    public virtual string? Description { get; }

    public virtual string? DisplayName { get; }

    public IDictionary Properties => _properties ??= new HybridDictionary();

    public bool ShowInSourceView { get; set; } = true;
}
