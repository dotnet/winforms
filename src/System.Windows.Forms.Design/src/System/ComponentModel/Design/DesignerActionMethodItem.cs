// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;

namespace System.ComponentModel.Design;

public class DesignerActionMethodItem : DesignerActionItem
{
    private readonly DesignerActionList? _actionList;
    private MethodInfo? _methodInfo;

    public DesignerActionMethodItem(
        DesignerActionList? actionList,
        string? memberName,
        string? displayName,
        string? category,
        string? description,
        bool includeAsDesignerVerb)
        : base(displayName, category, description)
    {
        _actionList = actionList;
        MemberName = memberName;
        IncludeAsDesignerVerb = includeAsDesignerVerb;
    }

    public DesignerActionMethodItem(
        DesignerActionList? actionList,
        string? memberName,
        string? displayName)
        : this(
            actionList,
            memberName,
            displayName,
            category: null,
            description: null,
            includeAsDesignerVerb: false)
    {
    }

    public DesignerActionMethodItem(
        DesignerActionList? actionList,
        string? memberName,
        string? displayName,
        bool includeAsDesignerVerb)
        : this(
            actionList,
            memberName,
            displayName,
            category: null,
            description: null,
            includeAsDesignerVerb)
    {
    }

    public DesignerActionMethodItem(
        DesignerActionList? actionList,
        string? memberName,
        string? displayName,
        string? category)
        : this(
            actionList,
            memberName,
            displayName,
            category,
            description: null,
            includeAsDesignerVerb: false)
    {
    }

    public DesignerActionMethodItem(
        DesignerActionList? actionList,
        string? memberName,
        string? displayName,
        string? category,
        bool includeAsDesignerVerb)
        : this(
            actionList,
            memberName,
            displayName,
            category,
            description: null,
            includeAsDesignerVerb)
    {
    }

    public DesignerActionMethodItem(
        DesignerActionList? actionList,
        string? memberName,
        string? displayName,
        string? category,
        string? description)
        : this(
            actionList,
            memberName,
            displayName,
            category,
            description,
            includeAsDesignerVerb: false)
    {
    }

    public virtual string? MemberName { get; }

    public IComponent? RelatedComponent { get; set; }

    public virtual bool IncludeAsDesignerVerb { get; }

    public virtual void Invoke()
    {
        if (MemberName is null)
        {
            throw new InvalidOperationException();
        }

        _methodInfo ??= _actionList?.GetType()?.GetMethod(MemberName, BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (_methodInfo is null)
        {
            throw new InvalidOperationException(string.Format(SR.DesignerActionPanel_CouldNotFindMethod, MemberName));
        }

        _methodInfo.Invoke(_actionList, null);
    }

    // this is only use for verbs so that a designer action method item can
    // be converted to a verb. Verbs use an EventHandler to call their invoke
    // so we need a way to translate the EventHandler Invoke into ou own Invoke
    internal void Invoke(object? sender, EventArgs args)
    {
        Invoke();
    }
}
