// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

internal class MdiWindowListItemConverter : ComponentConverter
{
    public MdiWindowListItemConverter(Type type) : base(type)
    {
    }

    /// <summary>
    ///  Gets a collection of standard values for the data type this validator is
    ///  designed for.
    /// </summary>
    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
    {
        if (context?.Instance is MenuStrip menu)
        {
            StandardValuesCollection values = base.GetStandardValues(context);
            List<ToolStripItem> list = [];
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] is ToolStripItem currentItem && currentItem.Owner == menu)
                {
                    list.Add(currentItem);
                }
            }

            return new StandardValuesCollection(list);
        }

        return base.GetStandardValues(context);
    }
}
