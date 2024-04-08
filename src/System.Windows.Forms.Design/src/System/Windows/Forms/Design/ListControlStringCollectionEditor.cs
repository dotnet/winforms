// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.Design;

/// <summary>
///  <see cref="ListControlStringCollectionEditor"/> overrides <see cref="StringCollectionEditor"/>
///  to prevent the string collection from being edited if <see cref="ListControl.DataSource"/>
///  has been set.
/// </summary>
internal class ListControlStringCollectionEditor : StringCollectionEditor
{
    public ListControlStringCollectionEditor(Type type)
        : base(type)
    {
    }

    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        // If we're trying to edit the items in an object that has a DataSource set, throw an exception.
        if (context?.Instance is ListControl control && control.DataSource is not null)
        {
            throw new ArgumentException(SR.DataSourceLocksItems);
        }

        return base.EditValue(context, provider, value);
    }
}
