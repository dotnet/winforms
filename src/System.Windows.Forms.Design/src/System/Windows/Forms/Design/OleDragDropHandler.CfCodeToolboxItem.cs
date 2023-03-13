// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.Serialization;

namespace System.Windows.Forms.Design
{
    internal partial class OleDragDropHandler
    {
        [Serializable] // designer related
        internal class CfCodeToolboxItem : ToolboxItem
        {
            private object? _serializationData;
            private static int s_template;
            private bool _displayNameSet;

            public CfCodeToolboxItem(object? serializationData) : base()
            {
                _serializationData = serializationData;
            }

            private CfCodeToolboxItem(SerializationInfo info, StreamingContext context)
            {
                Deserialize(info, context);
            }

            /// <summary>
            /// </summary>
            public void SetDisplayName()
            {
                if (!_displayNameSet)
                {
                    _displayNameSet = true;
                    DisplayName = $"Template{++s_template}";
                }
            }

            /// <summary>
            /// <para>Saves the state of this <see cref="ToolboxItem"/> to
            ///  the specified serialization info.</para>
            /// </summary>
            protected override void Serialize(SerializationInfo info, StreamingContext context)
            {
                base.Serialize(info, context);
                if (_serializationData is not null)
                {
                    info.AddValue("CfCodeToolboxItem.serializationData", _serializationData);
                }
            }

            /// <summary>
            /// <para>Loads the state of this <see cref="ToolboxItem"/>
            /// from the stream.</para>
            /// </summary>
            protected override void Deserialize(SerializationInfo info, StreamingContext context)
            {
                base.Deserialize(info, context);

                foreach (SerializationEntry entry in info)
                {
                    if (entry.Name == "CfCodeToolboxItem.serializationData")
                    {
                        _serializationData = entry.Value;
                        break;
                    }
                }
            }

            protected override IComponent[]? CreateComponentsCore(IDesignerHost host, IDictionary? defaultValues)
            {
                IDesignerSerializationService? ds = host.GetService<IDesignerSerializationService>();
                if (ds is null || _serializationData is null)
                {
                    return null;
                }

                // Deserialize to components collection
                ICollection objects = ds.Deserialize(_serializationData);
                List<IComponent> components = new();
                foreach (object item in objects)
                {
                    if (item is not null and IComponent component)
                    {
                        components.Add(component);
                    }
                }

                // Parent and locate each Control
                defaultValues ??= new Dictionary<string, object>();
                Control? parentControl = defaultValues["Parent"] as Control;
                if (parentControl is not null)
                {
                    ParentControlDesigner? parentControlDesigner = host.GetDesigner(parentControl) as ParentControlDesigner;
                    if (parentControlDesigner is not null)
                    {
                        // Determine bounds of all controls
                        Rectangle bounds = Rectangle.Empty;

                        foreach (IComponent component in components)
                        {
                            Control? childControl = component as Control;
                            if (childControl is not null && childControl != parentControl && childControl.Parent is null)
                            {
                                bounds = bounds.IsEmpty ? childControl.Bounds : Rectangle.Union(bounds, childControl.Bounds);
                            }
                        }

                        defaultValues.Remove("Size"); // don't care about the drag size
                        foreach (IComponent component in components)
                        {
                            Control? childControl = component as Control;
                            Form? form = childControl as Form;
                            if (childControl is not null
                                && !(form is not null && form.TopLevel) // Don't add top-level forms
                                && childControl.Parent is null)
                            {
                                defaultValues["Offset"] = new Size(childControl.Bounds.X - bounds.X, childControl.Bounds.Y - bounds.Y);
                                parentControlDesigner.AddControl(childControl, defaultValues);
                            }
                        }
                    }
                }

                // VSWhidbey 516338 - When creating an item for the tray, template items will have
                // an old location stored in them, so they may show up on top of other items.
                // So we need to call UpdatePastePositions for each one to get the tray to
                // arrange them properly.
                ComponentTray? tray = host.GetService<ComponentTray>();
                List<Control>? trayComponents = null;
                if (tray is not null)
                {
                    foreach (IComponent component in components)
                    {
                        ComponentTray.TrayControl trayControl = ComponentTray.GetTrayControlFromComponent(component);

                        if (trayControl is not null)
                        {
                            trayComponents ??= new();
                            trayComponents.Add(trayControl);
                        }
                    }

                    if (trayComponents is not null)
                    {
                        tray.UpdatePastePositions(trayComponents);
                    }
                }

                return components.ToArray();
            }

            protected override IComponent[]? CreateComponentsCore(IDesignerHost host) => CreateComponentsCore(host, null);
        }
    }
}
