// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

/// <summary>
///  Provides an editor for a ListView items collection.
/// </summary>
internal class ListViewItemCollectionEditor : CollectionEditor
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="ListViewItemCollectionEditor"/> class.
    /// </summary>
    public ListViewItemCollectionEditor(Type type) : base(type)
    { }

    /// <inheritdoc />
    protected override string GetDisplayText(object value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        string text;

        PropertyDescriptor property = TypeDescriptor.GetDefaultProperty(CollectionType);
        if (property?.PropertyType == typeof(string))
        {
            text = (string)property.GetValue(value);

            if (!string.IsNullOrEmpty(text))
            {
                return text;
            }
        }

        text = TypeDescriptor.GetConverter(value).ConvertToString(value);

        if (string.IsNullOrEmpty(text))
        {
            text = value.GetType().Name;
        }

        return text;
    }
}
