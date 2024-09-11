// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;

namespace System.Windows.Forms.Design;

/// <summary>
///  The Reason for having a CustomSerializer for ToolStripMenuItem is the existence of Dummy ToolStripMenuItem
///  for ContextMenuStrips. We add this Dummy ToolStripMenuItem on the "Non Site" ToolStrip to Host the DropDown
///  which facilitates the entry of New MenuItems. These items are then added to the ContextMenuStrip
///  that we are designing. But we don't want the Dummy ToolStripMenuItem to Serialize and hence the need for
///  this Custom Serializer.
/// </summary>
internal class ToolStripMenuItemCodeDomSerializer : CodeDomSerializer
{
    /// <summary>
    /// We implement this for the abstract method on CodeDomSerializer.
    /// </summary>
    public override object? Deserialize(IDesignerSerializationManager manager, object codeObject)
        => GetBaseSerializer(manager).Deserialize(manager, codeObject);

    /// <summary>
    /// This is a small helper method that returns the serializer for base Class
    /// </summary>
    private static CodeDomSerializer GetBaseSerializer(IDesignerSerializationManager manager)
        => manager.GetSerializer<CodeDomSerializer>(typeof(Component))!;

    /// <summary>
    /// We implement this for the abstract method on CodeDomSerializer. This method
    /// takes an object graph, and serializes the object into CodeDom elements.
    /// </summary>
    public override object? Serialize(IDesignerSerializationManager manager, object value)
    {
        // Don't Serialize if we are Dummy Item ...
        if (value is ToolStripMenuItem { IsOnDropDown: false } item)
        {
            ToolStrip? parent = item.GetCurrentParent();
            if (parent is not null && parent.Site is null)
            {
                // don't serialize anything...
                return null;
            }
        }

        CodeDomSerializer baseSerializer = manager.GetSerializer<CodeDomSerializer>(typeof(ImageList).BaseType)!;

        return baseSerializer.Serialize(manager, value);
    }
}
