// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.ComponentModel.Design.Serialization;

public sealed partial class CodeDomComponentSerializationService
{
    /// <summary>
    ///  The SerializationStore class is an implementation-specific class that stores serialization data for the component serialization service.
    ///  The service adds state to this serialization store.  Once the store is closed it can be saved to a stream.  A serialization store can
    ///  be deserialized at a later date by the same type of serialization service. SerializationStore implements the IDisposable interface such
    ///  that Dispose  simply calls the Close method.  Dispose is implemented as a private interface to avoid confusion.
    ///  The <see cref="IDisposable" /> pattern is provided for languages that support a "using" syntax like C# and VB .NET.
    /// </summary>
    private sealed partial class CodeDomSerializationStore : SerializationStore, ISerializable
    {
        private const string StateKey = "State";
        private const string NameKey = "Names";
        private const string AssembliesKey = "Assemblies";
        private const string ResourcesKey = "Resources";
        private const string ShimKey = "Shim";

        private MemoryStream? _resourceStream;

        // Transient fields only used during creation of the store
        private readonly Dictionary<object, ObjectData> _objects;
        private readonly IServiceProvider? _provider;

        // These fields persist across the store
        private readonly List<string> _objectNames;
        private Dictionary<string, CodeDomComponentSerializationState>? _objectState;
        private LocalResourceManager? _resources;
        private readonly List<string> _shimObjectNames;

        // These fields are available after serialization or deserialization
        private ICollection? _errors;

        /// <summary>
        ///  Creates a new store.
        /// </summary>
        internal CodeDomSerializationStore(IServiceProvider? provider)
        {
            _provider = provider;
            _objects = [];
            _objectNames = [];
            _shimObjectNames = [];
        }

        /// <summary>
        ///  Nested classes within us access this property to get to our array of saved assembly names.
        /// </summary>
        private AssemblyName[]? AssemblyNames { get; set; }

        /// <summary>
        ///  If there were errors generated during serialization or deserialization of the store, they will be added to this collection.
        /// </summary>
        public override ICollection Errors
        {
            get
            {
                _errors ??= Array.Empty<object>();

                object[] errors = new object[_errors.Count];
                _errors.CopyTo(errors, 0);
                return errors;
            }
        }

        /// <summary>
        ///  Nested classes within us access this property to get to our collection of resources.
        /// </summary>
        private LocalResourceManager Resources => _resources ??= new LocalResourceManager();

        private ObjectData GetOrCreateObjectData(object value)
        {
            if (_objectState is not null)
            {
                throw new InvalidOperationException(SR.CodeDomComponentSerializationServiceClosedStore);
            }

            if (!_objects.TryGetValue(value, out ObjectData? data))
            {
                data = new ObjectData(GetObjectName(value), value);

                _objects[value] = data;
                _objectNames.Add(data._name);
            }

            return data;
        }

        /// <summary>
        ///  Adds a new member serialization to our list of things to serialize.
        /// </summary>
        internal void AddMember(object value, MemberDescriptor member, bool absolute)
        {
            ObjectData data = GetOrCreateObjectData(value);
            data.Members.Add(new MemberData(member, absolute));
        }

        /// <summary>
        ///  Adds a new object serialization to our list of things to serialize.
        /// </summary>
        internal void AddObject(object value, bool absolute)
        {
            ObjectData data = GetOrCreateObjectData(value);
            data.EntireObject = true;
            data.Absolute = absolute;
        }

        /// <summary>
        ///  The Close method closes this store and prevents any objects  from being serialized into it.  Once closed, the serialization store may be saved.
        /// </summary>
        [MemberNotNull(nameof(_objectState))]
        public override void Close()
        {
            if (_objectState is not null)
            {
                return;
            }

            Dictionary<string, CodeDomComponentSerializationState> state = new(_objects.Count);
            DesignerSerializationManager manager = new(new LocalServices(this, _provider));
            if (_provider?.GetService(typeof(IDesignerSerializationManager)) is DesignerSerializationManager hostManager)
            {
                foreach (IDesignerSerializationProvider provider in hostManager.SerializationProviders)
                {
                    ((IDesignerSerializationManager)manager).AddSerializationProvider(provider);
                }
            }

            using (manager.CreateSession())
            {
                // Walk through our objects and name them so the serialization manager knows what names we gave them.
                foreach (ObjectData data in _objects.Values)
                {
                    ((IDesignerSerializationManager)manager).SetName(data._value, data._name);
                }

                ComponentListCodeDomSerializer.s_instance.Serialize(manager, _objects, state, _shimObjectNames);
                _errors = manager.Errors;
            }

            // also serialize out resources if we have any  we force this in order for undo to work correctly
            if (_resources is not null)
            {
                Debug.Assert(_resourceStream is null, "Attempting to close a serialization store with already serialized resources");
                if (_resourceStream is null)
                {
                    _resourceStream = new MemoryStream();

#pragma warning disable SYSLIB0011 // Type or member is obsolete
                    new BinaryFormatter().Serialize(_resourceStream, _resources.Data);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
                }
            }

            Dictionary<Assembly, AssemblyName> assemblies = new(_objects.Count);
            foreach (object obj in _objects.Keys)
            {
                // Save off the assembly for this object
                Assembly a = obj.GetType().Assembly;
                if (!assemblies.ContainsKey(a))
                {
                    assemblies.Add(a, a.GetName(true));
                }
            }

            AssemblyNames = new AssemblyName[assemblies.Count];
            assemblies.Values.CopyTo(AssemblyNames, 0);

            _objectState = state;
            _objects.Clear();
        }

        /// <summary>
        ///  Deserializes the saved bits.
        /// </summary>
        internal List<object> Deserialize(IServiceProvider? provider, IContainer? container = null)
        {
            List<object> collection = [];
            Deserialize(provider, container, validateRecycledTypes: true, applyDefaults: true, collection);
            return collection;
        }

        private void Deserialize(IServiceProvider? provider, IContainer? container, bool validateRecycledTypes, bool applyDefaults, List<object>? objects)
        {
            PassThroughSerializationManager delegator = new(new LocalDesignerSerializationManager(this, new LocalServices(this, provider)));
            if (container is not null)
            {
                delegator.Manager.Container = container;
            }

            if (provider?.GetService(typeof(IDesignerSerializationManager)) is DesignerSerializationManager hostManager)
            {
                foreach (IDesignerSerializationProvider serProvider in hostManager.SerializationProviders)
                {
                    ((IDesignerSerializationManager)delegator.Manager).AddSerializationProvider(serProvider);
                }
            }

            bool recycleInstances = objects is null;

            // RecycleInstances is used so that we re-use objects already in the container.  PreserveNames is used raise errors in the case of duplicate names.  We only care about name preservation when we are recycling instances.  Otherwise, we'd prefer to create objects with different names.
            delegator.Manager.RecycleInstances = recycleInstances;
            delegator.Manager.PreserveNames = recycleInstances;
            delegator.Manager.ValidateRecycledTypes = validateRecycledTypes;

            // Recreate resources
            if (_resourceStream is not null)
            {
                _resourceStream.Seek(0, SeekOrigin.Begin);
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                Hashtable? resources = new BinaryFormatter().Deserialize(_resourceStream) as Hashtable;
#pragma warning restore SYSLIB0011 // Type or member is obsolete
                _resources = new LocalResourceManager(resources);
            }

            using (delegator.Manager.CreateSession())
            {
                // Before we deserialize, setup any references to components we faked during serialization
                if (_shimObjectNames.Count > 0)
                {
                    if (delegator is IDesignerSerializationManager dsm && container is not null)
                    {
                        foreach (string compName in _shimObjectNames)
                        {
                            object? instance = container.Components[compName];
                            if (instance is not null && dsm.GetInstance(compName) is null)
                            {
                                dsm.SetName(instance, compName);
                            }
                        }
                    }
                }

                ComponentListCodeDomSerializer.s_instance.Deserialize(delegator, _objectState!, _objectNames, applyDefaults);
                if (objects is not null)
                {
                    foreach (string name in _objectNames)
                    {
                        object? instance = ((IDesignerSerializationManager)delegator.Manager).GetInstance(name);
                        Debug.Assert(instance is not null, $"Failed to deserialize object {name}");
                        if (instance is not null)
                        {
                            objects.Add(instance);
                        }
                    }
                }

                _errors = delegator.Manager.Errors;
            }
        }

        /// <summary>
        ///  Deserializes the saved bits.
        /// </summary>
        internal void DeserializeTo(IServiceProvider provider, IContainer container, bool validateRecycledTypes, bool applyDefaults)
        {
            Deserialize(provider, container, validateRecycledTypes, applyDefaults, null);
        }

        /// <summary>
        ///  Gets a name for this object.  It first tries the object's site, if it exists, and otherwise fabricates a unique name.
        /// </summary>
        private static string GetObjectName(object value)
        {
            if (value is IComponent comp)
            {
                ISite? site = comp.Site;
                if (site is INestedSite nestedSite && !string.IsNullOrEmpty(nestedSite.FullName))
                {
                    return nestedSite.FullName;
                }

                if (!string.IsNullOrEmpty(site?.Name))
                {
                    return site.Name;
                }
            }

            Guid guid = Guid.NewGuid();
            string prefix = "object_";
            Span<char> chars = stackalloc char[prefix.Length + 36];
            prefix.CopyTo(chars);
            guid.TryFormat(chars[prefix.Length..], out _);
            chars[prefix.Length..].Replace('-', '_');
            return chars.ToString();
        }

        /// <summary>
        ///  Loads our state from a stream.
        /// </summary>
        internal static CodeDomSerializationStore Load(Stream stream)
        {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            return (CodeDomSerializationStore)new BinaryFormatter().Deserialize(stream);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
        }

        /// <summary>
        ///  The Save method is not supported.
        /// </summary>
        public override void Save(Stream stream) => throw new PlatformNotSupportedException();

        /// <summary>
        ///  Implements the save part of ISerializable. Used in unit tests only.
        /// </summary>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ArgumentNullException.ThrowIfNull(info);

            info.AddValue(StateKey, _objectState);
            info.AddValue(NameKey, _objectNames);
            info.AddValue(AssembliesKey, AssemblyNames);
            info.AddValue(ResourcesKey, _resources?.Data);
            info.AddValue(ShimKey, _shimObjectNames);
        }
    }
}
