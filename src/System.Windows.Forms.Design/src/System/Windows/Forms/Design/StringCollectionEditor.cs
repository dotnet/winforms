// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

/// <summary>
///  The StringCollectionEditor is a collection editor that is specifically designed to edit collections containing
///  strings. The collection can be of any type that can accept a string value; we just present a string-centric
///  dialog for the user.
/// </summary>
internal partial class StringCollectionEditor : CollectionEditor
{
    public StringCollectionEditor(Type type)
        : base(type)
    {
    }

    /// <inheritdoc />
    protected override CollectionForm CreateCollectionForm() => new StringCollectionForm(this);

    /// <inheritdoc />
    protected override string HelpTopic => "net.ComponentModel.StringCollectionEditor";
}
