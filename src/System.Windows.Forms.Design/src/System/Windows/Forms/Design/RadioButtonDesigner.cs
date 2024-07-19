// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms.Design;

/// <summary>
///  <para>
///  Provides a designer that can design components
///  that extend ButtonBase.</para>
/// </summary>
internal class RadioButtonDesigner : ButtonBaseDesigner
{
    public override void InitializeNewComponent(IDictionary? defaultValues)
    {
        base.InitializeNewComponent(defaultValues);

        // In Whidbey, default the TabStop to true.
        PropertyDescriptor? prop = TypeDescriptor.GetProperties(Component)["TabStop"];
        if (prop is not null && prop.PropertyType == typeof(bool) && !prop.IsReadOnly && prop.IsBrowsable)
        {
            prop.SetValue(Component, true);
        }
    }
}
