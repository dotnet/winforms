// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Windows.Forms.Design
{
    internal partial class OleDragDropHandler
    {
        protected class ComponentDataObject : IDataObject
        {
            private readonly IServiceProvider serviceProvider;
            private object[] components;

            private Stream serializationStream;
            private object serializationData;
            private readonly int initialX;
            private readonly int initialY;
            private readonly IOleDragClient dragClient;
            private CfCodeToolboxItem toolboxitemdata;

            public ComponentDataObject(IOleDragClient dragClient, IServiceProvider sp, object[] comps, int x, int y)
            {
                serviceProvider = sp;
                components = GetComponentList(comps, null, -1);
                initialX = x;
                initialY = y;
                this.dragClient = dragClient;
            }

            public ComponentDataObject(IOleDragClient dragClient, IServiceProvider sp, object serializationData)
            {
                serviceProvider = sp;
                this.serializationData = serializationData;
                this.dragClient = dragClient;
            }

            private Stream SerializationStream
            {
                get
                {
                    if (serializationStream is null && Components != null)
                    {
                        IDesignerSerializationService ds = (IDesignerSerializationService)serviceProvider.GetService(typeof(IDesignerSerializationService));
                        if (ds != null)
                        {
                            object[] comps = new object[components.Length];
                            for (int i = 0; i < components.Length; i++)
                            {
                                Debug.Assert(components[i] is IComponent, "Item " + components[i].GetType().Name + " is not an IComponent");
                                comps[i] = (IComponent)components[i];
                            }

                            object sd = ds.Serialize(comps);
                            serializationStream = new MemoryStream();
                            BinaryFormatter formatter = new BinaryFormatter();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                            formatter.Serialize(serializationStream, sd);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
                            serializationStream.Seek(0, SeekOrigin.Begin);
                        }
                    }

                    return serializationStream;
                }
            }

            public object[] Components
            {
                get
                {
                    if (components is null && (serializationStream != null || serializationData != null))
                    {
                        Deserialize(null, false);
                        if (components is null)
                        {
                            return Array.Empty<object>();
                        }
                    }

                    return (object[])components.Clone();
                }
            }

            /// <summary>
            /// computes the IDataObject which constitutes this whole toolboxitem for storage in the toolbox.
            /// </summary>
            private CfCodeToolboxItem NestedToolboxItem
            {
                get
                {
                    toolboxitemdata ??= new CfCodeToolboxItem(GetData(DataFormat));

                    return toolboxitemdata;
                }
            }

            /// <summary>
            ///  Used to retrieve the selection for a copy.  The default implementation
            ///  retrieves the current selection.
            /// </summary>
            private object[] GetComponentList(object[] components, ArrayList list, int index)
            {
                if (serviceProvider is null)
                {
                    return components;
                }

                ISelectionService selSvc = (ISelectionService)serviceProvider.GetService(typeof(ISelectionService));

                if (selSvc is null)
                {
                    return components;
                }

                ICollection selectedComponents;
                if (components is null)
                    selectedComponents = selSvc.GetSelectedComponents();
                else
                    selectedComponents = new ArrayList(components);

                IDesignerHost host = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
                if (host != null)
                {
                    ArrayList copySelection = new ArrayList();
                    foreach (IComponent comp in selectedComponents)
                    {
                        copySelection.Add(comp);
                        GetAssociatedComponents(comp, host, copySelection);
                    }

                    selectedComponents = copySelection;
                }

                object[] comps = new object[selectedComponents.Count];
                selectedComponents.CopyTo(comps, 0);
                return comps;
            }

            private void GetAssociatedComponents(IComponent component, IDesignerHost host, ArrayList list)
            {
                ComponentDesigner designer = host.GetDesigner(component) as ComponentDesigner;
                if (designer is null)
                {
                    return;
                }

                foreach (IComponent childComp in designer.AssociatedComponents)
                {
                    list.Add(childComp);
                    GetAssociatedComponents(childComp, host, list);
                }
            }

            public virtual object GetData(string format)
            {
                return GetData(format, false);
            }

            public virtual object GetData(string format, bool autoConvert)
            {
                if (format.Equals(DataFormat))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    SerializationStream.Seek(0, SeekOrigin.Begin);
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                    return formatter.Deserialize(SerializationStream);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
                }
                else if (format.Equals(NestedToolboxItemFormat))
                {
                    NestedToolboxItem.SetDisplayName();
                    return NestedToolboxItem;
                }

                return null;
            }

            public virtual object GetData(Type t)
            {
                return GetData(t.FullName);
            }

            /// <summary>
            ///  If the there is data store in the data object associated with
            ///  format this will return true.
            /// </summary>
            public bool GetDataPresent(string format, bool autoConvert)
            {
                return Array.IndexOf(GetFormats(), format) != -1;
            }

            /// <summary>
            ///  If the there is data store in the data object associated with
            ///  format this will return true.
            /// </summary>
            public bool GetDataPresent(string format)
            {
                return GetDataPresent(format, false);
            }

            /// <summary>
            ///  If the there is data store in the data object associated with
            ///  format this will return true.
            /// </summary>
            public bool GetDataPresent(Type format)
            {
                return GetDataPresent(format.FullName, false);
            }

            /// <summary>
            ///  Retrieves a list of all formats stored in this data object.
            /// </summary>
            public string[] GetFormats(bool autoConvert)
            {
                return GetFormats();
            }

            /// <summary>
            ///  Retrieves a list of all formats stored in this data object.
            /// </summary>
            public string[] GetFormats()
            {
                return new string[] { NestedToolboxItemFormat, DataFormat, DataFormats.Serializable, ExtraInfoFormat };
            }

            public void Deserialize(IServiceProvider serviceProvider, bool removeCurrentComponents)
            {
                serviceProvider ??= this.serviceProvider;

                IDesignerSerializationService ds = (IDesignerSerializationService)serviceProvider.GetService(typeof(IDesignerSerializationService));
                IDesignerHost host = null;
                DesignerTransaction trans = null;

                try
                {
                    if (serializationData is null)
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                        serializationData = formatter.Deserialize(SerializationStream);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
                    }

                    if (removeCurrentComponents && components != null)
                    {
                        foreach (IComponent removeComp in components)
                        {
                            if (host is null && removeComp.Site != null)
                            {
                                host = (IDesignerHost)removeComp.Site.GetService(typeof(IDesignerHost));
                                if (host != null)
                                {
                                    trans = host.CreateTransaction(string.Format(SR.DragDropMoveComponents, components.Length));
                                }
                            }

                            host?.DestroyComponent(removeComp);
                        }

                        components = null;
                    }

                    ICollection objects = ds.Deserialize(serializationData);
                    components = new IComponent[objects.Count];
                    IEnumerator e = objects.GetEnumerator();
                    int idx = 0;

                    while (e.MoveNext())
                    {
                        components[idx++] = (IComponent)e.Current;
                    }

                    // only do top-level components here,
                    // because other are already parented.
                    // otherwise, when we process these
                    // components it's too hard to know what we
                    // should be reparenting.
                    ArrayList topComps = new ArrayList();
                    for (int i = 0; i < components.Length; i++)
                    {
                        if (components[i] is Control c)
                        {
                            if (c.Parent is null)
                            {
                                topComps.Add(components[i]);
                            }
                        }
                        else
                        {
                            topComps.Add(components[i]);
                        }
                    }

                    components = topComps.ToArray();
                }
                finally
                {
                    trans?.Commit();
                }
            }

            /// <summary>
            ///  Sets the data to be associated with the specific data format. For
            ///  a listing of predefined formats see System.Windows.Forms.DataFormats.
            /// </summary>
            public void SetData(string format, bool autoConvert, object data)
            {
                SetData(format, data);
            }

            /// <summary>
            ///  Sets the data to be associated with the specific data format. For
            ///  a listing of predefined formats see System.Windows.Forms.DataFormats.
            /// </summary>
            public void SetData(string format, object data)
            {
                throw new Exception(SR.DragDropSetDataError);
            }

            /// <summary>
            ///  Sets the data to be associated with the specific data format.
            /// </summary>
            public void SetData(Type format, object data)
            {
                SetData(format.FullName, data);
            }

            /// <summary>
            ///  Stores data in the data object. The format assumed is the
            ///  class of data
            /// </summary>
            public void SetData(object data)
            {
                SetData(data.GetType(), data);
            }
        }
    }
}
