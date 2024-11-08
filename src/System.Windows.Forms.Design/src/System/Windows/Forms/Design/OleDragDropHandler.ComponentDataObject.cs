// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Windows.Forms.Design;

internal partial class OleDragDropHandler
{
    protected class ComponentDataObject : IDataObject
    {
        private readonly IServiceProvider _serviceProvider;
        private object[]? _components;

        private MemoryStream? _serializationStream;
        private object? _serializationData;
        private CfCodeToolboxItem? _toolboxItemData;

        public ComponentDataObject(IServiceProvider serviceProvider, object[] components)
        {
            _serviceProvider = serviceProvider;
            _components = GetComponentList(components);
        }

        public ComponentDataObject(IServiceProvider serviceProvider, object serializationData)
        {
            _serviceProvider = serviceProvider;
            _serializationData = serializationData;
        }

        private MemoryStream? SerializationStream
        {
            get
            {
                if (_serializationStream is null && _components is not null)
                {
                    IDesignerSerializationService? ds = _serviceProvider.GetService<IDesignerSerializationService>();
                    if (ds is not null)
                    {
                        IComponent[] components = new IComponent[_components.Length];
                        for (int i = 0; i < _components.Length; i++)
                        {
                            Debug.Assert(_components[i] is IComponent, $"Item {_components[i].GetType().Name} is not an IComponent");
                            components[i] = (IComponent)_components[i];
                        }

                        object sd = ds.Serialize(components);
                        _serializationStream = new MemoryStream();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                        new BinaryFormatter().Serialize(_serializationStream, sd);
#pragma warning restore SYSLIB0011
                        _serializationStream.Seek(0, SeekOrigin.Begin);
                    }
                }

                return _serializationStream;
            }
        }

        public object[] Components
        {
            get
            {
                if (_components is null && (_serializationStream is not null || _serializationData is not null))
                {
                    Deserialize(null, false);
                }

                return (object[]?)_components?.Clone() ?? [];
            }
        }

        /// <summary>
        /// computes the IDataObject which constitutes this whole toolboxitem for storage in the toolbox.
        /// </summary>
        private CfCodeToolboxItem NestedToolboxItem => _toolboxItemData ??= new CfCodeToolboxItem(GetData(DataFormat));

        /// <summary>
        ///  Used to retrieve the selection for a copy. The default implementation
        ///  retrieves the current selection.
        /// </summary>
        private object[] GetComponentList(object[] components)
        {
            if (!_serviceProvider.TryGetService(out ISelectionService? selectionService))
            {
                return components;
            }

            ICollection selectedComponents;
            if (components is null)
                selectedComponents = selectionService.GetSelectedComponents();
            else
                selectedComponents = new List<object>(components);

            IDesignerHost? host = _serviceProvider.GetService<IDesignerHost>();
            if (host is not null)
            {
                List<IComponent> copySelection = [];
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

        private static void GetAssociatedComponents(IComponent component, IDesignerHost host, List<IComponent> list)
        {
            if (host.GetDesigner(component) is not ComponentDesigner designer)
            {
                return;
            }

            foreach (IComponent childComp in designer.AssociatedComponents)
            {
                list.Add(childComp);
                GetAssociatedComponents(childComp, host, list);
            }
        }

        public virtual object? GetData(string format)
        {
            return GetData(format, false);
        }

        public virtual object? GetData(string format, bool autoConvert)
        {
            if (format.Equals(DataFormat))
            {
                SerializationStream!.Seek(0, SeekOrigin.Begin);
#pragma warning disable SYSLIB0011 // Type or member is obsolete
#pragma warning disable CA2300 // Do not use insecure deserializer BinaryFormatter
#pragma warning disable CA2301 // Ensure BinaryFormatter.Binder is set before calling BinaryFormatter.Deserialize
                return new BinaryFormatter().Deserialize(SerializationStream); // CodeQL[SM03722, SM04191] : The operation is essential for the design experience when users are running their own designers they have created. This cannot be achieved without BinaryFormatter
#pragma warning restore CA2301
#pragma warning restore CA2300
#pragma warning restore SYSLIB0011
            }
            else if (format.Equals(NestedToolboxItemFormat))
            {
                NestedToolboxItem.SetDisplayName();
                return NestedToolboxItem;
            }

            return null;
        }

        public virtual object? GetData(Type t)
        {
            return GetData(t.FullName!);
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
            return GetDataPresent(format.FullName!, false);
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
            return [NestedToolboxItemFormat, DataFormat, DataFormats.Serializable, ExtraInfoFormat];
        }

        [MemberNotNull(nameof(_components))]
        public void Deserialize(IServiceProvider? serviceProvider, bool removeCurrentComponents)
        {
            serviceProvider ??= _serviceProvider;

            IDesignerSerializationService ds = serviceProvider.GetService<IDesignerSerializationService>()!;
            IDesignerHost? host = null;
            DesignerTransaction? trans = null;

            try
            {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
#pragma warning disable CA2300 // Do not use insecure deserializer BinaryFormatter
#pragma warning disable CA2301 // Ensure BinaryFormatter.Binder is set before calling BinaryFormatter.Deserialize
                _serializationData ??= new BinaryFormatter().Deserialize(SerializationStream!); // CodeQL[SM03722, SM04191] : The operation is essential for the design experience when users are running their own designers they have created. This cannot be achieved without BinaryFormatter
#pragma warning restore CA2301
#pragma warning restore CA2300
#pragma warning restore SYSLIB0011

                if (removeCurrentComponents && _components is not null)
                {
                    foreach (IComponent removeComp in _components)
                    {
                        if (host is null && removeComp.Site.TryGetService(out host))
                        {
                            trans = host.CreateTransaction(string.Format(SR.DragDropMoveComponents, _components.Length));
                        }

                        host?.DestroyComponent(removeComp);
                    }

                    _components = null;
                }

                ICollection objects = ds.Deserialize(_serializationData);
                _components = new object[objects.Count];
                objects.CopyTo(_components, 0);

                // only do top-level components here,
                // because other are already parented.
                // otherwise, when we process these
                // components it's too hard to know what we
                // should be reparenting.
                List<object> topComponents = new(_components.Length);
                for (int i = 0; i < _components.Length; i++)
                {
                    if (_components[i] is Control c)
                    {
                        if (c.Parent is null)
                        {
                            topComponents.Add(c);
                        }
                    }
                    else
                    {
                        topComponents.Add(_components[i]);
                    }
                }

                _components = [.. topComponents];
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
        public void SetData(string format, bool autoConvert, object? data)
        {
            SetData(format, data);
        }

        /// <summary>
        ///  Sets the data to be associated with the specific data format. For
        ///  a listing of predefined formats see System.Windows.Forms.DataFormats.
        /// </summary>
        public void SetData(string format, object? data)
        {
            throw new InvalidOperationException(SR.DragDropSetDataError);
        }

        /// <summary>
        ///  Sets the data to be associated with the specific data format.
        /// </summary>
        public void SetData(Type format, object? data)
        {
            SetData(format.FullName!, data);
        }

        /// <summary>
        ///  Stores data in the data object. The format assumed is the
        ///  class of data
        /// </summary>
        public void SetData(object? data)
        {
            if (data is not null)
            {
                SetData(data.GetType(), data);
            }
        }
    }
}
