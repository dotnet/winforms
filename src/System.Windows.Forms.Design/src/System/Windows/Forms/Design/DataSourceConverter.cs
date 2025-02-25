// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Globalization;

namespace System.Windows.Forms.Design;

internal class DataSourceConverter : ReferenceConverter
{
    private readonly ReferenceConverter _listConverter = new ReferenceConverter(typeof(IList));

    public DataSourceConverter() : base(typeof(IListSource))
    {
    }

    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
    {
        ArrayList listSources = new ArrayList(base.GetStandardValues(context));
        StandardValuesCollection lists = _listConverter.GetStandardValues(context);

        ArrayList listsList = [];

        BindingSource? bs = context?.Instance as BindingSource;

        foreach (object listSource in listSources)
        {
            if (listSource is not null)
            {
                // bug 46563: work around the TableMappings property on the OleDbDataAdapter
                ListBindableAttribute? listBindable = TypeDescriptor.GetAttributes(listSource)[typeof(ListBindableAttribute)] as ListBindableAttribute;
                if (listBindable is not null && !listBindable.ListBindable)
                {
                    continue;
                }

                // Prevent user from being able to connect a BindingSource to itself
                if (bs is not null && bs == listSource)
                {
                    continue;
                }

                // Per Whidbey spec : DataSourcePicker.doc, 3.4.1
                //
                // if this is a DataTable and the DataSet that owns the table is in the list,
                // don't add it.  this way we only show the top-level data sources and don't clutter the
                // list with duplicates like:
                //
                //   NorthWind1.Customers
                //   NorthWind1.Employees
                //   NorthWind1
                //
                // but instead just show "NorthWind1".  This does force the user to pick a data member but helps
                // with simplicity.
                //
                // we are doing an n^2 lookup here but this list will never be more than 10 or 15 entries long so it should
                // not be a problem.
                //
                if (listSource is not DataTable listSourceDataTable || !listSources.Contains(listSourceDataTable.DataSet))
                {
                    listsList.Add(listSource);
                }
            }
        }

        foreach (object list in lists)
        {
            if (list is not null)
            {
                // bug 46563: work around the TableMappings property on the OleDbDataAdapter
                ListBindableAttribute? listBindable = TypeDescriptor.GetAttributes(list)[typeof(ListBindableAttribute)] as ListBindableAttribute;
                if (listBindable is not null && !listBindable.ListBindable)
                {
                    continue;
                }

                // Prevent user from being able to connect a BindingSource to itself
                if (bs is not null && bs == list)
                {
                    continue;
                }

                listsList.Add(list);
            }
        }

        // bug 71417: add a null list to reset the dataSource
        listsList.Add(null);

        return new StandardValuesCollection(listsList);
    }

    public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context)
    {
        return true;
    }

    public override bool GetStandardValuesSupported(ITypeDescriptorContext? context)
    {
        return true;
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        // Types are now valid data sources, so we need to be able to
        // represent them as strings (since ReferenceConverter can't)
        if (destinationType == typeof(string) && value is Type)
        {
            return value.ToString();
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
