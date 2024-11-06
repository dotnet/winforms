// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.Diagnostics;

namespace TestConsole;

internal class DesignerActionVerbItem : DesignerActionMethodItem
{
    private readonly DesignerVerb _targetVerb;

    public DesignerActionVerbItem(DesignerVerb verb) : base(null, null, null)
    {
        Debug.Assert(verb is not null, "All callers check whether the verb is null.");
        _targetVerb = verb;
    }

    public override string Category => "Verbs";

    public override string DisplayName => _targetVerb.Text;

    public override void Invoke() => _targetVerb.Invoke();
}
