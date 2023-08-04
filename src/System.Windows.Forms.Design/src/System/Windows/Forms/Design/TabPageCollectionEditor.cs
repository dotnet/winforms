﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

/// <summary>
///  Collection editor for <see cref="TabControl.TabPageCollection"/>.
///  Allows a single level of <see cref="ToolStripItem"/> children to be designed.
/// </summary>
internal class TabPageCollectionEditor : CollectionEditor
{
    public TabPageCollectionEditor() : base(typeof(TabControl.TabPageCollection))
    {
    }

    /// <summary>
    ///  Sets the specified collection to have the specified array of items.
    /// </summary>
    protected override object SetItems(object editValue, object[] value)
    {
        var tabControl = Context.Instance as TabControl;
        tabControl?.SuspendLayout();

        object retValue = base.SetItems(editValue, value);

        tabControl?.ResumeLayout();
        return retValue;
    }

    protected override object CreateInstance(Type itemType)
    {
        object instance = base.CreateInstance(itemType);

        TabPage tabPage = instance as TabPage;
        tabPage.UseVisualStyleBackColor = true;

        return tabPage;
    }
}
