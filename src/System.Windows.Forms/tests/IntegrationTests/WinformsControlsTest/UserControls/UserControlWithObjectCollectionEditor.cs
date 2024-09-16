// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Globalization;

namespace WinFormsControlsTest.UserControls;

[DesignerCategory("Default")]
internal class UserControlWithObjectCollectionEditor : UserControl
{
    public UserControlWithObjectCollectionEditor()
    {
        AutoScaleMode = AutoScaleMode.Font;
    }

    [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Category("Accessibility")]
    [TypeConverter(typeof(SomeCollectionTypeConverter))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IList<int> AAAAAFirstCollection
    {
        get { return new List<int>(new int[] { 1, 2, 3 }); }
        set { }
    }
}

internal class SomeCollectionTypeConverter : TypeConverter
{
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType is not null && destinationType.IsAssignableFrom(typeof(string)) && value is IList<int> list)
        {
            return string.Join(", ", list);
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
