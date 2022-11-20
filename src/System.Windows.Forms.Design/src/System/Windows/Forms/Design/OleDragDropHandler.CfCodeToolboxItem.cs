// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.Serialization;

namespace System.Windows.Forms.Design
{
    internal partial class OleDragDropHandler
    {
        [Serializable] // designer related
        internal class CfCodeToolboxItem : ToolboxItem
        {
            private object _serializationData;
            private static int s_template;
            private bool _displayNameSet;

            public CfCodeToolboxItem(object serializationData) : base()
            {
                this._serializationData = serializationData;
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
                    DisplayName = "Template" + (++s_template).ToString(CultureInfo.CurrentCulture);
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

            protected override IComponent[] CreateComponentsCore(IDesignerHost host, IDictionary defaultValues)
            {
                IDesignerSerializationService ds = (IDesignerSerializationService)host.GetService(typeof(IDesignerSerializationService));
                if (ds is null)
                {
                    return null;
                }

                // Deserialize to components collection
                //
                ICollection objects = ds.Deserialize(_serializationData);
                ArrayList components = new ArrayList();
                foreach (object obj in objects)
                {
                    if (obj is not null && obj is IComponent)
                    {
                        components.Add(obj);
                    }
                }

                IComponent[] componentsArray = new IComponent[components.Count];
                components.CopyTo(componentsArray, 0);

                // Parent and locate each Control
                //
                defaultValues ??= new Hashtable();
                Control parentControl = defaultValues["Parent"] as Control;
                if (parentControl is not null)
                {
                    ParentControlDesigner parentControlDesigner = host.GetDesigner(parentControl) as ParentControlDesigner;
                    if (parentControlDesigner is not null)
                    {
                        // Determine bounds of all controls
                        //
                        Rectangle bounds = Rectangle.Empty;

                        foreach (IComponent component in componentsArray)
                        {
                            Control childControl = component as Control;

                            if (childControl is not null && childControl != parentControl && childControl.Parent is null)
                            {
                                if (bounds.IsEmpty)
                                {
                                    bounds = childControl.Bounds;
                                }
                                else
                                {
                                    bounds = Rectangle.Union(bounds, childControl.Bounds);
                                }
                            }
                        }

                        defaultValues.Remove("Size");    // don't care about the drag size
                        foreach (IComponent component in componentsArray)
                        {
                            Control childControl = component as Control;
                            Form form = childControl as Form;
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
                //
                ComponentTray tray = (ComponentTray)host.GetService(typeof(ComponentTray));
                List<Control> trayComponents = null;
                if (tray is not null)
                {
                    foreach (IComponent component in componentsArray)
                    {
                        ComponentTray.TrayControl c = ComponentTray.GetTrayControlFromComponent(component);

                        if (c is not null)
                        {
                            trayComponents ??= new();

                            trayComponents.Add(c);
                        }
                    }

                    if (trayComponents is not null)
                    {
                        tray.UpdatePastePositions(trayComponents);
                    }
                }

                return componentsArray;
            }

            protected override IComponent[] CreateComponentsCore(IDesignerHost host)
            {
                return CreateComponentsCore(host, null);
            }
        }
    }
}
