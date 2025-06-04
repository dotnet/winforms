// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

internal class TextBoxAutoCompleteSourceConverter : EnumConverter
{
    public TextBoxAutoCompleteSourceConverter(Type type) : base(type)
    {
    }

    /// <summary>
    ///  Gets a collection of standard values for the data type this validator is
    ///  designed for.
    /// </summary>
    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
    {
        StandardValuesCollection values = base.GetStandardValues(context);
        List<object> list = [];
        for (int i = 0; i < values.Count; i++)
        {
            if (values[i] is object currentItem)
            {
                if (!string.Equals(currentItem.ToString(), nameof(AutoCompleteSource.ListItems)))
                {
                    list.Add(currentItem);
                }
            }
        }

        return new StandardValuesCollection(list);
    }
}
