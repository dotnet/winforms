// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;

namespace System.Windows.Forms.Design;

/// <summary>
///  Custom serializer for the TableLayoutPanel. We need this so we can push the TableLayoutSettings object
///  into the resx in localization mode. This is used by loc tools like WinRes to correctly setup the
///  TableLayoutPanel with all its settings. Note that we don't serialize code to access the settings.
/// </summary>
internal class TableLayoutPanelCodeDomSerializer : CodeDomSerializer
{
    private const string LayoutSettingsPropName = "LayoutSettings";

    public override object? Deserialize(IDesignerSerializationManager manager, object codeObject)
    {
        return GetBaseSerializer(manager).Deserialize(manager, codeObject);
    }

    private static CodeDomSerializer GetBaseSerializer(IDesignerSerializationManager manager)
    {
        return manager.GetSerializer<CodeDomSerializer>(typeof(TableLayoutPanel).BaseType)!;
    }

    /// <summary>
    ///  We don't actually want to serialize any code here, so we just delegate that to the base type's
    ///  serializer. All we want to do is if we are in a localizable form, we want to push a
    ///  'LayoutSettings' entry into the resx.
    /// </summary>
    public override object? Serialize(IDesignerSerializationManager manager, object value)
    {
        // First call the base serializer to serialize the object.
        object? codeObject = GetBaseSerializer(manager).Serialize(manager, value);

        // Now push our layout settings stuff into the resx if we are not inherited read only and
        // are in a localizable Form.
        TableLayoutPanel? panel = value as TableLayoutPanel;
        Debug.Assert(panel is not null, "Huh? We were expecting to be serializing a TableLayoutPanel here.");

        if (panel is not null)
        {
            if (!TypeDescriptorHelper.TryGetAttribute(panel, out InheritanceAttribute? ia) || ia.InheritanceLevel != InheritanceLevel.InheritedReadOnly)
            {
                IDesignerHost? host = manager.GetService<IDesignerHost>();

                if (IsLocalizable(host))
                {
                    PropertyDescriptor? lsProp = TypeDescriptor.GetProperties(panel)[LayoutSettingsPropName];
                    object? val = lsProp?.GetValue(panel);

                    if (val is not null)
                    {
                        string key = $"{manager.GetName(panel)}.{LayoutSettingsPropName}";
                        SerializeResourceInvariant(manager, key, val);
                    }
                }
            }
        }

        return codeObject;
    }

    private static bool IsLocalizable([NotNullWhen(true)] IDesignerHost? host)
    {
        if (host is not null)
        {
            if (TypeDescriptorHelper.TryGetPropertyValue(host.RootComponent, "Localizable", out bool b))
            {
                return b;
            }
        }

        return false;
    }
}
