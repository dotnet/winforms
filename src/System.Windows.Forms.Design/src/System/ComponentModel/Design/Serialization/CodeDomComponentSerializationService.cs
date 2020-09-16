// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.ComponentModel.Design.Serialization
{
    /// <summary>
    ///  This class serializes a set of components or serializable objects into a  serialization store.  The store can then be deserialized at a later  date.  CodeDomComponentSerializationService differs from other serialization  schemes in that the serialization format is opaque, and it allows for  partial serialization of objects.  For example, you can choose to  serialize only selected properties for an object.
    /// </summary>
    public sealed class CodeDomComponentSerializationService : ComponentSerializationService
    {
        private readonly IServiceProvider _provider;

        /// <summary>
        ///  Creates a new CodeDomComponentSerializationService object.
        /// </summary>
        public CodeDomComponentSerializationService() : this(null)
        {
        }

        /// <summary>
        ///  Creates a new CodeDomComponentSerializationService object using the given  service provider to resolve services.
        /// </summary>
        public CodeDomComponentSerializationService(IServiceProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        ///  This method creates a new SerializationStore.  The serialization store can be passed to any of the various Serialize methods to build up serialization  state for a group of objects.
        /// </summary>
        public override SerializationStore CreateStore()
        {
            return new CodeDomSerializationStore(_provider);
        }

        /// <summary>
        ///  This method loads a SerializationStore and from the given stream.  This store can then be used to deserialize objects by passing it to  the various Deserialize methods.
        /// </summary>
        public override SerializationStore LoadStore(Stream stream)
        {
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        ///  This method serializes the given object to the store.  The store  can be used to serialize more than one object by calling this method  more than once.
        /// </summary>
        public override void Serialize(SerializationStore store, object value)
        {
            if (store is null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (!(store is CodeDomSerializationStore cdStore))
            {
                throw new InvalidOperationException(SR.CodeDomComponentSerializationServiceUnknownStore);
            }

            cdStore.AddObject(value, false);
        }

        /// <summary>
        ///  This method serializes the given object to the store.  The store can be used to serialize more than one object by calling this method more than once.
        /// </summary>
        public override void SerializeAbsolute(SerializationStore store, object value)
        {
            if (store is null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (!(store is CodeDomSerializationStore cdStore))
            {
                throw new InvalidOperationException(SR.CodeDomComponentSerializationServiceUnknownStore);
            }

            cdStore.AddObject(value, true);
        }

        /// <summary>
        ///  This method serializes the given member on the given object.  This method  can be invoked multiple times for the same object to build up a list of  serialized members within the serialization store.  The member generally  has to be a property or an event.
        /// </summary>
        public override void SerializeMember(SerializationStore store, object owningObject, MemberDescriptor member)
        {
            if (store is null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            if (owningObject is null)
            {
                throw new ArgumentNullException(nameof(owningObject));
            }

            if (member is null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            if (!(store is CodeDomSerializationStore cdStore))
            {
                throw new InvalidOperationException(SR.CodeDomComponentSerializationServiceUnknownStore);
            }

            cdStore.AddMember(owningObject, member, false);
        }

        /// <summary>
        ///  This method serializes the given member on the given object,  but also serializes the member if it contains the default value.
        ///  Note that for some members, containing the default value and setting  the same value back to the member are different concepts.  For example, if a property inherits its value from a parent object if no local value  is set, setting the value back to the property can may not be what is desired.   SerializeMemberAbsolute takes this into account and would clear the state of  the property in this case.
        /// </summary>
        public override void SerializeMemberAbsolute(SerializationStore store, object owningObject, MemberDescriptor member)
        {
            if (store is null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            if (owningObject is null)
            {
                throw new ArgumentNullException(nameof(owningObject));
            }

            if (member is null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            if (!(store is CodeDomSerializationStore cdStore))
            {
                throw new InvalidOperationException(SR.CodeDomComponentSerializationServiceUnknownStore);
            }

            cdStore.AddMember(owningObject, member, true);
        }

        /// <summary>
        ///  This method deserializes the given store to produce a collection of  objects contained within it.  If a container is provided, objects  that are created that implement IComponent will be added to the container.
        /// </summary>
        public override ICollection Deserialize(SerializationStore store)
        {
            if (store is null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            if (!(store is CodeDomSerializationStore cdStore))
            {
                throw new InvalidOperationException(SR.CodeDomComponentSerializationServiceUnknownStore);
            }

            return cdStore.Deserialize(_provider);
        }

        /// <summary>
        ///  This method deserializes the given store to produce a collection of  objects contained within it.  If a container is provided, objects  that are created that implement IComponent will be added to the container.
        /// </summary>
        public override ICollection Deserialize(SerializationStore store, IContainer container)
        {
            if (store is null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            if (container is null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (!(store is CodeDomSerializationStore cdStore))
            {
                throw new InvalidOperationException(SR.CodeDomComponentSerializationServiceUnknownStore);
            }

            return cdStore.Deserialize(_provider, container);
        }

        /// <summary>
        ///  This method deserializes the given store, but rather than produce  new objects object, the data in the store is applied to an existing  set of objects that are taken from the provided container.  This  allows the caller to pre-create an object however it sees fit.  If an object has deserialization state and the object is not named in  the set of existing objects, a new object will be created.  If that  object also implements IComponent, it will be added to the given  container.  Objects in the container must have names and types that  match objects in the serialization store in order for an existing  object to be used.
        /// </summary>
        public override void DeserializeTo(SerializationStore store, IContainer container, bool validateRecycledTypes, bool applyDefaults)
        {
            if (store is null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            if (container is null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (!(store is CodeDomSerializationStore cdStore))
            {
                throw new InvalidOperationException(SR.CodeDomComponentSerializationServiceUnknownStore);
            }

            cdStore.DeserializeTo(_provider, container, validateRecycledTypes, applyDefaults);
        }

        /// <summary>
        ///  The SerializationStore class is an implementation-specific class that stores serialization data for the component serialization service.
        ///  The service adds state to this serialization store.  Once the store is closed it can be saved to a stream.  A serialization store can
        ///  be deserialized at a later date by the same type of serialization service. SerializationStore implements the IDisposable interface such
        ///  that Dispose  simply calls the Close method.  Dispose is implemented as a private interface to avoid confusion.
        ///  The <see cref="IDisposable" /> pattern is provided for languages that support a "using" syntax like C# and VB .NET.
        /// </summary>
        private sealed class CodeDomSerializationStore : SerializationStore, ISerializable
        {
#if DEBUG
            private static readonly TraceSwitch s_trace = new TraceSwitch("ComponentSerializationService", "Trace component serialization");
#endif
            private const string StateKey = "State";
            private const string NameKey = "Names";
            private const string AssembliesKey = "Assemblies";
            private const string ResourcesKey = "Resources";
            private const string ShimKey = "Shim";
            private const int StateCode = 0;
            private const int StateCtx = 1;
            private const int StateProperties = 2;
            private const int StateResources = 3;
            private const int StateEvents = 4;
            private const int StateModifier = 5;

            private MemoryStream _resourceStream;

            // Transient fields only used during creation of the store
            private Hashtable _objects;
            private readonly IServiceProvider _provider;

            // These fields persist across the store
            private readonly ArrayList _objectNames;
            private Hashtable _objectState;
            private LocalResourceManager _resources;
            private AssemblyName[] _assemblies;
            private readonly List<string> _shimObjectNames;

            // These fields are available after serialization or deserialization
            private ICollection _errors;

            /// <summary>
            ///  Creates a new store.
            /// </summary>
            internal CodeDomSerializationStore(IServiceProvider provider)
            {
                _provider = provider;
                _objects = new Hashtable();
                _objectNames = new ArrayList();
                _shimObjectNames = new List<string>();
            }

            /// <summary>
            ///  Nested classes within us access this property to get to our array of saved assembly names.
            /// </summary>
            private AssemblyName[] AssemblyNames
            {
                get => _assemblies;
            }

            /// <summary>
            ///  If there were errors generated during serialization or deserialization of the store, they will be added to this collection.
            /// </summary>
            public override ICollection Errors
            {
                get
                {
                    if (_errors is null)
                    {
                        _errors = Array.Empty<object>();
                    }

                    object[] errors = new object[_errors.Count];
                    _errors.CopyTo(errors, 0);
                    return errors;
                }
            }

            /// <summary>
            ///  Nested classes within us access this property to get to our collection of resources.
            /// </summary>
            private LocalResourceManager Resources
            {
                get
                {
                    if (_resources is null)
                    {
                        _resources = new LocalResourceManager();
                    }
                    return _resources;
                }
            }

            /// <summary>
            ///  Adds a new member serialization to our list of things to serialize.
            /// </summary>
            internal void AddMember(object value, MemberDescriptor member, bool absolute)
            {
                if (_objectState != null)
                {
                    throw new InvalidOperationException(SR.CodeDomComponentSerializationServiceClosedStore);
                }

                ObjectData data = (ObjectData)_objects[value];
                if (data is null)
                {
                    data = new ObjectData
                    {
                        _name = GetObjectName(value),
                        _value = value
                    };

                    _objects[value] = data;
                    _objectNames.Add(data._name);
                }

                Trace("Adding object '{0}' ({1}:{2}) {3}", data._name, data._value.GetType().FullName, member.Name, (absolute ? "NORMAL" : "ABSOLUTE"));
                data.Members.Add(new MemberData(member, absolute));
            }

            /// <summary>
            ///  Adds a new object serialization to our list of things to serialize.
            /// </summary>
            internal void AddObject(object value, bool absolute)
            {
                if (_objectState != null)
                {
                    throw new InvalidOperationException(SR.CodeDomComponentSerializationServiceClosedStore);
                }

                ObjectData data = (ObjectData)_objects[value];
                if (data is null)
                {
                    data = new ObjectData
                    {
                        _name = GetObjectName(value),
                        _value = value
                    };

                    _objects[value] = data;
                    _objectNames.Add(data._name);
                }

                Trace("Adding object '{0}' ({1}) {2}", data._name, data._value.GetType().FullName, (absolute ? "NORMAL" : "ABSOLUTE"));
                data.EntireObject = true;
                data.Absolute = absolute;
            }

            /// <summary>
            ///  The Close method closes this store and prevents any objects  from being serialized into it.  Once closed, the serialization store may be saved.
            /// </summary>
            public override void Close()
            {
                if (_objectState is null)
                {
                    Hashtable state = new Hashtable(_objects.Count);
                    DesignerSerializationManager manager = new DesignerSerializationManager(new LocalServices(this, _provider));
                    if (_provider?.GetService(typeof(IDesignerSerializationManager)) is DesignerSerializationManager hostManager)
                    {
                        foreach (IDesignerSerializationProvider provider in hostManager.SerializationProviders)
                        {
                            ((IDesignerSerializationManager)manager).AddSerializationProvider(provider);
                        }
                    }

                    Trace("Closing Store: serializing {0} objects", _objects.Count);
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
                    if (_resources != null)
                    {
                        Debug.Assert(_resourceStream is null, "Attempting to close a serialization store with already serialized resources");
                        if (_resourceStream is null)
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            _resourceStream = new MemoryStream();

#pragma warning disable SYSLIB0011
                            formatter.Serialize(_resourceStream, _resources.Data);
#pragma warning restore SYSLIB0011
                        }
                    }

                    Hashtable assemblies = new Hashtable(_objects.Count);
                    foreach (object obj in _objects.Keys)
                    {
                        // Save off the assembly for this object
                        Assembly a = obj.GetType().Assembly;
                        assemblies[a] = null;
                    }

                    _assemblies = new AssemblyName[assemblies.Count];
                    int idx = 0;

                    foreach (Assembly a in assemblies.Keys)
                    {
                        _assemblies[idx++] = a.GetName(true);
                    }
#if DEBUG
                    foreach (DictionaryEntry de in state)
                    {
                        TraceCode((string)de.Key, de.Value);
                    }
#endif
                    _objectState = state;
                    _objects = null;
                }
            }

            /// <summary>
            ///  Deserializes the saved bits.
            /// </summary>
            internal ICollection Deserialize(IServiceProvider provider)
            {
                return Deserialize(provider, null, false, true, true);
            }

            /// <summary>
            ///  Deserializes the saved bits.
            /// </summary>
            internal ICollection Deserialize(IServiceProvider provider, IContainer container)
            {
                return Deserialize(provider, container, false, true, true);
            }

            private ICollection Deserialize(IServiceProvider provider, IContainer container, bool recycleInstances, bool validateRecycledTypes, bool applyDefaults)
            {
                PassThroughSerializationManager delegator = new PassThroughSerializationManager(new LocalDesignerSerializationManager(this, new LocalServices(this, provider)));
                if (container != null)
                {
                    delegator.Manager.Container = container;
                }

                if (provider.GetService(typeof(IDesignerSerializationManager)) is DesignerSerializationManager hostManager)
                {
                    foreach (IDesignerSerializationProvider serProvider in hostManager.SerializationProviders)
                    {
                        ((IDesignerSerializationManager)delegator.Manager).AddSerializationProvider(serProvider);
                    }
                }

                // RecycleInstances is used so that we re-use objects already in the container.  PreserveNames is used raise errors in the case of duplicate names.  We only care about name preservation when we are recycling instances.  Otherwise, we'd prefer to create objects with different names.
                delegator.Manager.RecycleInstances = recycleInstances;
                delegator.Manager.PreserveNames = recycleInstances;
                delegator.Manager.ValidateRecycledTypes = validateRecycledTypes;
                ArrayList objects = null;
                // recreate resouces
                if (_resourceStream != null)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    _resourceStream.Seek(0, SeekOrigin.Begin);
#pragma warning disable SYSLIB0011
                    Hashtable resources = formatter.Deserialize(_resourceStream) as Hashtable;
#pragma warning restore SYSLIB0011
                    _resources = new LocalResourceManager(resources);
                }

                if (!recycleInstances)
                {
                    objects = new ArrayList(_objectNames.Count);
                }

                Trace("Deserializing {0} objects, recycling instances: {1}", _objectState.Count, recycleInstances);
                using (delegator.Manager.CreateSession())
                {
                    // before we deserialize, setup any references to components we faked during serialization
                    if (_shimObjectNames.Count > 0)
                    {
                        List<string> names = _shimObjectNames;
                        if (delegator is IDesignerSerializationManager dsm && container != null)
                        {
                            foreach (string compName in names)
                            {
                                object instance = container.Components[compName];
                                if (instance != null && dsm.GetInstance(compName) is null)
                                {
                                    dsm.SetName(instance, compName);
                                }
                            }
                        }
                    }

                    ComponentListCodeDomSerializer.s_instance.Deserialize(delegator, _objectState, _objectNames, applyDefaults);
                    if (!recycleInstances)
                    {
                        foreach (string name in _objectNames)
                        {
                            object instance = ((IDesignerSerializationManager)delegator.Manager).GetInstance(name);
                            Debug.Assert(instance != null, "Failed to deserialize object " + name);
                            if (instance != null)
                            {
                                objects.Add(instance);
                            }
                        }
                    }
                    _errors = delegator.Manager.Errors;
                }
                return objects;
            }

            /// <summary>
            ///  Deserializes the saved bits.
            /// </summary>
            internal void DeserializeTo(IServiceProvider provider, IContainer container, bool validateRecycledTypes, bool applyDefaults)
            {
                Deserialize(provider, container, true, validateRecycledTypes, applyDefaults);
            }

            /// <summary>
            ///  Gets a name for this object.  It first tries the object's site, if it exists, and otherwise fabricates a unique name.
            /// </summary>
            private string GetObjectName(object value)
            {
                if (value is IComponent comp)
                {
                    ISite site = comp.Site;
                    if (site != null)
                    {
                        if (site is INestedSite nestedSite && !string.IsNullOrEmpty(nestedSite.FullName))
                        {
                            return nestedSite.FullName;
                        }
                        else if (!string.IsNullOrEmpty(site.Name))
                        {
                            return site.Name;
                        }
                    }
                }

                Guid guid = Guid.NewGuid();
                string guidStr = guid.ToString();
                guidStr = guidStr.Replace("-", "_");
                return string.Format(CultureInfo.CurrentCulture, "object_{0}", guidStr);
            }

            /// <summary>
            ///  Loads our state from a stream.
            /// </summary>
            internal static CodeDomSerializationStore Load(Stream stream)
            {
                BinaryFormatter f = new BinaryFormatter();
#pragma warning disable SYSLIB0011
                return (CodeDomSerializationStore)f.Deserialize(stream);
#pragma warning restore SYSLIB0011
            }

            /// <summary>
            ///  The Save method is not supported.
            /// </summary>
            public override void Save(Stream stream)
            {
                throw new PlatformNotSupportedException();
            }

            [Conditional("DEBUG")]
            internal static void Trace(string message, params object[] args)
            {
#if DEBUG
                string msg = string.Format(CultureInfo.CurrentCulture, "ComponentSerialization: " + message, args);
                Debug.WriteLineIf(s_trace.TraceVerbose, msg);
#endif
            }

#if DEBUG
            internal static void TraceCode(string name, object code)
            {
                if (code is null || !s_trace.TraceVerbose)
                {
                    return;
                }

                // The code is stored as the first slot in an array.
                object[] state = (object[])code;
                code = state[StateCode];

                if (code is null)
                {
                    return;
                }

                System.CodeDom.Compiler.ICodeGenerator codeGenerator = new Microsoft.CSharp.CSharpCodeProvider().CreateGenerator();
                using var sw = new StringWriter(CultureInfo.InvariantCulture);
                Trace("Stored CodeDom for {0}: ", name);
                Debug.Indent();

                if (code is CodeTypeDeclaration codeTypeDeclaration)
                {
                    codeGenerator.GenerateCodeFromType(codeTypeDeclaration, sw, null);
                }
                else if (code is CodeStatementCollection statements)
                {
                    foreach (CodeStatement statement in statements)
                    {
                        codeGenerator.GenerateCodeFromStatement(statement, sw, null);
                    }
                }
                else if (code is CodeStatement codeStatement)
                {
                    codeGenerator.GenerateCodeFromStatement(codeStatement, sw, null);
                }
                else if (code is CodeExpression codeExpression)
                {
                    codeGenerator.GenerateCodeFromExpression(codeExpression, sw, null);
                }
                else
                {
                    sw.Write("Unknown code type: " + code.GetType().Name);
                    sw.Write("\r\n");
                }

                // spit this line by line so it respects the indent.
                StringReader sr = new StringReader(sw.ToString());
                for (string ln = sr.ReadLine(); ln != null; ln = sr.ReadLine())
                {
                    Debug.WriteLine(ln);
                }
                Debug.Unindent();
            }
#endif

            /// <summary>
            ///  Implements the save part of ISerializable. Used in unit tests only.
            /// </summary>
            void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
            {
                if (info is null)
                {
                    throw new ArgumentNullException(nameof(info));
                }

                info.AddValue(StateKey, _objectState);
                info.AddValue(NameKey, _objectNames);
                info.AddValue(AssembliesKey, _assemblies);
                info.AddValue(ResourcesKey, _resources?.Data);
                info.AddValue(ShimKey, _shimObjectNames);
            }

            /// <summary>
            ///  This is a simple code dom serializer that serializes a set of objects as a unit.
            /// </summary>
            private class ComponentListCodeDomSerializer : CodeDomSerializer
            {
                internal static ComponentListCodeDomSerializer s_instance = new ComponentListCodeDomSerializer();
                private Hashtable _statementsTable;
                Dictionary<string, ArrayList> _expressions;
                private Hashtable _objectState; // only used during deserialization
                private bool _applyDefaults = true;
                private readonly Hashtable _nameResolveGuard = new Hashtable();

                public override object Deserialize(IDesignerSerializationManager manager, object state)
                {
                    throw new NotSupportedException();
                }

                private void PopulateCompleteStatements(object data, string name, CodeStatementCollection completeStatements)
                {
                    if (data is CodeStatementCollection statements)
                    {
                        completeStatements.AddRange(statements);
                    }
                    else if (data is CodeStatement statement)
                    {
                        completeStatements.Add(statement);
                    }
                    else if (data is CodeExpression expression)
                    {
                        // we handle expressions a little differently since they don't have a LHS or RHS they won't show up correctly in the statement table. We will deserialize them explicitly.
                        ArrayList exps = null;
                        if (_expressions.ContainsKey(name))
                        {
                            exps = _expressions[name];
                        }
                        if (exps is null)
                        {
                            exps = new ArrayList();
                            _expressions[name] = exps;
                        }
                        exps.Add(expression);
                    }
                    else
                    {
                        Debug.Fail("No case for " + data.GetType().Name);
                    }
                }
                /// <summary>
                ///  Deserializes the given object state.  The results are contained within the  serialization manager's name table.  The objectNames list is used to  deserialize in the proper order, as objectState is unordered.
                /// </summary>
                internal void Deserialize(IDesignerSerializationManager manager, IDictionary objectState, IList objectNames, bool applyDefaults)
                {
                    CodeStatementCollection completeStatements = new CodeStatementCollection();
                    _expressions = new Dictionary<string, ArrayList>();
                    _applyDefaults = applyDefaults;
                    foreach (string name in objectNames)
                    {
                        object[] state = (object[])objectState[name];
                        if (state != null)
                        {
                            if (state[StateCode] != null)
                            {
                                PopulateCompleteStatements(state[StateCode], name, completeStatements);
                            }
                            if (state[StateCtx] != null)
                            {
                                PopulateCompleteStatements(state[StateCtx], name, completeStatements);
                            }
                        }
                    }
                    CodeStatementCollection mappedStatements = new CodeStatementCollection();
                    CodeMethodMap methodMap = new CodeMethodMap(mappedStatements, null);

                    methodMap.Add(completeStatements);
                    methodMap.Combine();
                    _statementsTable = new Hashtable();

                    // generate statement table keyed on component name
                    CodeDomSerializerBase.FillStatementTable(manager, _statementsTable, mappedStatements);

                    // We need to also ensure that for every entry in the statement table we have a corresponding entry in objectNames.  Otherwise, we won't deserialize completely.
                    ArrayList completeNames = new ArrayList(objectNames);
                    foreach (string mappedKey in _statementsTable.Keys)
                    {
                        if (!completeNames.Contains(mappedKey))
                        {
                            completeNames.Add(mappedKey);
                        }
                    }

                    _objectState = new Hashtable(objectState.Keys.Count);
                    foreach (DictionaryEntry de in objectState)
                    {
                        _objectState.Add(de.Key, de.Value);
                    }

                    ResolveNameEventHandler resolveNameHandler = new ResolveNameEventHandler(OnResolveName);
                    manager.ResolveName += resolveNameHandler;
                    try
                    {
                        foreach (string name in completeNames)
                        {
                            ResolveName(manager, name, true);
                        }
                    }
                    finally
                    {
                        _objectState = null;
                        manager.ResolveName -= resolveNameHandler;
                    }
                }

                private void OnResolveName(object sender, ResolveNameEventArgs e)
                {
                    //note: this recursionguard does not fix the problem, but rathar avoids a stack overflow which will bring down VS and cause loss of data.
                    if (_nameResolveGuard.ContainsKey(e.Name))
                    {
                        return;
                    }
                    _nameResolveGuard.Add(e.Name, true);
                    try
                    {
                        IDesignerSerializationManager manager = (IDesignerSerializationManager)sender;
                        if (ResolveName(manager, e.Name, false))
                        {
                            e.Value = manager.GetInstance(e.Name);
                        }
                    }
                    finally
                    {
                        _nameResolveGuard.Remove(e.Name);
                    }
                }

                private void DeserializeDefaultProperties(IDesignerSerializationManager manager, string name, object state)
                {
                    // Next, default properties, but only if we successfully  resolved.
                    if (state != null && _applyDefaults)
                    {
                        object comp = manager.GetInstance(name);
                        if (comp != null)
                        {
                            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(comp);
                            string[] defProps = (string[])state;
                            foreach (string propName in defProps)
                            {
                                PropertyDescriptor prop = props[propName];
                                if (prop != null && prop.CanResetValue(comp))
                                {
                                    Trace("Resetting default for {0}.{1}", name, propName);
                                    // If there is a member relationship setup for this property, we should disconnect it first. This makes sense, since if there was a previous relationship, we would have serialized it and not come here at all.
                                    if (manager.GetService(typeof(MemberRelationshipService)) is MemberRelationshipService relationships && relationships[comp, prop] != MemberRelationship.Empty)
                                    {
                                        relationships[comp, prop] = MemberRelationship.Empty;
                                    }
                                    prop.ResetValue(comp);
                                }
                            }
                        }
                    }
                }

                private void DeserializeDesignTimeProperties(IDesignerSerializationManager manager, string name, object state)
                {
                    if (state != null)
                    {
                        object comp = manager.GetInstance(name);
                        if (comp != null)
                        {
                            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(comp);

                            foreach (DictionaryEntry de in (IDictionary)state)
                            {
                                PropertyDescriptor prop = props[(string)de.Key];
                                if (prop != null)
                                {
                                    prop.SetValue(comp, de.Value);
                                }
                            }
                        }
                    }
                }

                /// <summary>
                ///  This is used to resolve nested component references.  NestedComponents don't exist as sited components within the DesignerHost, they are actually sited within a parent component.  This method takes the FullName defined on INestedSite and returns the component which matches it. outerComponent is the name of the topmost component which does exist in the DesignerHost
                ///  This code also exists in VSCodeDomDesignerLoader -- please keep them in sync.
                /// </summary>
                private IComponent ResolveNestedName(IDesignerSerializationManager manager, string name, ref string outerComponent)
                {
                    IComponent curComp = null;
                    if (name != null && manager != null)
                    {
                        bool moreChunks = true;
                        Debug.Assert(name.IndexOf('.') > 0, "ResolvedNestedName accepts only nested names!");
                        // We need to resolve the first chunk using the manager. other chunks will be resolved within the nested containers.
                        int firstIndex = name.IndexOf('.', 0);
                        outerComponent = name.Substring(0, firstIndex);
                        curComp = manager.GetInstance(outerComponent) as IComponent;

                        int prevIndex = firstIndex;
                        int curIndex = name.IndexOf('.', firstIndex + 1);
                        while (moreChunks)
                        {
                            moreChunks = curIndex != -1;
                            string compName = moreChunks ? name.Substring(prevIndex + 1, curIndex) : name.Substring(prevIndex + 1);
                            if (curComp != null && curComp.Site != null)
                            {
                                ISite site = curComp.Site;
                                if (site.GetService(typeof(INestedContainer)) is INestedContainer container && !string.IsNullOrEmpty(compName))
                                {
                                    curComp = container.Components[compName];
                                }
                                else
                                {
                                    return null;
                                }
                            }
                            else
                            {
                                return null;
                            }

                            if (moreChunks)
                            {
                                prevIndex = curIndex;
                                curIndex = name.IndexOf('.', curIndex + 1);
                            }
                        }
                    }
                    return curComp;
                }

                private bool ResolveName(IDesignerSerializationManager manager, string name, bool canInvokeManager)
                {
                    bool resolved = false;
                    object[] state = (object[])_objectState[name];
                    // Check for a nested name. Components that are sited within NestedContainers need to be looked up in their nested container, and won't be resolvable directly via the manager.
                    if (name.IndexOf('.') > 0)
                    {
                        string parentName = null;
                        IComponent nestedComp = ResolveNestedName(manager, name, ref parentName);
                        if (nestedComp != null && parentName != null)
                        {
                            manager.SetName(nestedComp, name);
                            // What is the point of this?  Well, the nested components won't be in the statement table with its nested name.  However, their most parent component will be, so forcing a resolve of their name will actually deserialize the nested statements.
                            ResolveName(manager, parentName, canInvokeManager);
                        }
                        else
                        {
                            Debug.Fail("Unable to resolve nested component: " + name);
                        }
                    }

                    // First we check to see if the statements table contains an OrderedCodeStatementCollection for this name.  If it does this means we have not resolved this name yet, so we grab its OrderedCodeStatementCollection and deserialize that, along with any default properties and design-time properties.
                    // If it doesn't contain an OrderedCodeStatementsCollection this means one of two things:
                    // 1. We already resolved this name and shoved an instance in there.  In this case we just return the instance
                    // 2. There are no statements corresponding to this name, but there might be expressions that have never been deserialized, so we check for that and deserailize those.
                    if (_statementsTable[name] is OrderedCodeStatementCollection statements)
                    {
                        _objectState[name] = null;
                        _statementsTable[name] = null; // prevent recursion
                        // we look through the statements to find the variableRef or fieldRef that matches this name
                        string typeName = null;
                        foreach (CodeStatement statement in statements)
                        {
                            if (statement is CodeVariableDeclarationStatement cvds)
                            {
                                typeName = cvds.Type.BaseType;
                                break;
                            }
                        }

                        // next, invoke the serializer for this component
                        if (typeName != null)
                        {
                            Type type = manager.GetType(typeName);
                            if (type is null)
                            {
                                TraceError("Type does not exist: {0}", typeName);
                                manager.ReportError(new CodeDomSerializerException(string.Format(SR.SerializerTypeNotFound, typeName), manager));
                            }
                            else
                            {
                                if (statements != null && statements.Count > 0)
                                {
                                    CodeDomSerializer serializer = GetSerializer(manager, type);
                                    if (serializer is null)
                                    {
                                        // We report this as an error.  This indicates that there are code statements in initialize component that we do not know how to load.
                                        TraceError("Type referenced in init method has no serializer: {0}", type.Name);
                                        manager.ReportError(new CodeDomSerializerException(string.Format(SR.SerializerNoSerializerForComponent, type.FullName), manager));
                                    }
                                    else
                                    {
                                        Trace("--------------------------------------------------------------------");
                                        Trace("     Beginning deserialization of {0}", name);
                                        Trace("--------------------------------------------------------------------");
                                        try
                                        {
                                            object instance = serializer.Deserialize(manager, statements);
                                            resolved = instance != null;
                                            if (resolved)
                                            {
                                                _statementsTable[name] = instance;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            manager.ReportError(ex);
                                        }
                                    }
                                }
                            }
                        }
                        // if we can't find a typeName to get a serializer with we fallback to deserializing each statement individually using the default serializer.
                        else
                        {
                            foreach (CodeStatement cs in statements)
                            {
                                DeserializeStatement(manager, cs);
                            }
                            resolved = true;
                        }

                        if (state != null && state[StateProperties] != null)
                        {
                            DeserializeDefaultProperties(manager, name, state[StateProperties]);
                        }

                        if (state != null && state[StateResources] != null)
                        {
                            DeserializeDesignTimeProperties(manager, name, state[StateResources]);
                        }

                        if (state != null && state[StateEvents] != null)
                        {
                            DeserializeEventResets(manager, name, state[StateEvents]);
                        }

                        if (state != null && state[StateModifier] != null)
                        {
                            DeserializeModifier(manager, name, state[StateModifier]);
                        }

                        if (_expressions.ContainsKey(name))
                        {
                            ArrayList exps = _expressions[name];
                            foreach (CodeExpression exp in exps)
                            {
                                object exValue = DeserializeExpression(manager, name, exp);
                            }
                            _expressions.Remove(name);
                            resolved = true;
                        }
                    }
                    else
                    {
                        resolved = _statementsTable[name] != null;
                        if (!resolved)
                        {
                            // this is condition 2 of the comment at the start of this method.
                            if (_expressions.ContainsKey(name))
                            {
                                ArrayList exps = _expressions[name];
                                foreach (CodeExpression exp in exps)
                                {
                                    object exValue = DeserializeExpression(manager, name, exp);
                                    if (exValue != null && !resolved)
                                    {
                                        if (canInvokeManager && manager.GetInstance(name) is null)
                                        {
                                            manager.SetName(exValue, name);
                                            resolved = true;
                                        }
                                    }
                                }
                            }

                            // Sometimes components won't be in either the statements table or the expressions table, for example, this occurs for resources  during undo/redo. In these cases the component should be resolvable by the manager. Never do this when we have been asked by the serialization manager to resolve the name;  otherwise we may infinitely recurse.
                            if (!resolved && canInvokeManager)
                            {
                                resolved = manager.GetInstance(name) != null;
                            }

                            // In this case we still need to correctly deserialize default properties &  design-time only properties.
                            if (resolved && state != null && state[StateProperties] != null)
                            {
                                DeserializeDefaultProperties(manager, name, state[StateProperties]);
                            }

                            if (resolved && state != null && state[StateResources] != null)
                            {
                                DeserializeDesignTimeProperties(manager, name, state[StateResources]);
                            }

                            if (resolved && state != null && state[StateEvents] != null)
                            {
                                DeserializeEventResets(manager, name, state[StateEvents]);
                            }

                            if (resolved && state != null && state[StateModifier] != null)
                            {
                                DeserializeModifier(manager, name, state[StateModifier]);
                            }
                        }

                        if (!(resolved || (!resolved && !canInvokeManager)))
                        {
                            manager.ReportError(new CodeDomSerializerException(string.Format(SR.CodeDomComponentSerializationServiceDeserializationError, name), manager));
                            Debug.Fail("No statements or instance for name and no lone experssions: " + name);
                        }
                    }
                    return resolved;
                }

                private void DeserializeEventResets(IDesignerSerializationManager manager, string name, object state)
                {
                    if (state is List<string> eventNames && manager != null && !string.IsNullOrEmpty(name))
                    {
                        object comp = manager.GetInstance(name);
                        if (comp != null && manager.GetService(typeof(IEventBindingService)) is IEventBindingService ebs)
                        {
                            PropertyDescriptorCollection eventProps = ebs.GetEventProperties(TypeDescriptor.GetEvents(comp));
                            if (eventProps != null)
                            {
                                foreach (string eventName in eventNames)
                                {
                                    PropertyDescriptor prop = eventProps[eventName];

                                    if (prop != null)
                                    {
                                        prop.SetValue(comp, null);
                                    }
                                }
                            }
                        }
                    }
                }

                private static void DeserializeModifier(IDesignerSerializationManager manager, string name, object state)
                {
                    Debug.Assert(state != null && state is MemberAttributes, "Attempting to deserialize a null modifier");
                    object comp = manager.GetInstance(name);
                    if (comp != null)
                    {
                        MemberAttributes modifierValue = (MemberAttributes)state;
                        PropertyDescriptor modifierProp = TypeDescriptor.GetProperties(comp)["Modifiers"];
                        if (modifierProp != null)
                        {
                            modifierProp.SetValue(comp, modifierValue);
                        }
                    }
                }

                public override object Serialize(IDesignerSerializationManager manager, object state)
                {
                    throw new NotSupportedException();
                }

                /// <summary>
                ///  For everything in the serialization manager's container, we need a variable ref,
                ///  just in case something that has changed has a reference to another object. We also
                ///  must do this for everything that we are serializing that is not marked as EntireObject.
                ///  Otherwise reference could leak and cause the entire object to be serialized.
                /// </summary>
                internal void SetupVariableReferences(IDesignerSerializationManager manager, IContainer container, IDictionary objectData, IList shimObjectNames)
                {
                    foreach (IComponent c in container.Components)
                    {
                        string name = TypeDescriptor.GetComponentName(c);
                        if (name != null && name.Length > 0)
                        {
                            bool needVar = true;
                            if (objectData.Contains(c) && ((ObjectData)objectData[c]).EntireObject)
                            {
                                needVar = false;
                            }

                            if (needVar)
                            {
                                CodeVariableReferenceExpression var = new CodeVariableReferenceExpression(name);
                                SetExpression(manager, c, var);
                                if (!shimObjectNames.Contains(name))
                                {
                                    shimObjectNames.Add(name);
                                }

                                if (c.Site != null)
                                {
                                    if (c.Site.GetService(typeof(INestedContainer)) is INestedContainer nested && nested.Components.Count > 0)
                                    {
                                        SetupVariableReferences(manager, nested, objectData, shimObjectNames);
                                    }
                                }
                            }
                        }
                    }
                }

                /// <summary>
                ///  Serializes the given set of objects (contained in objectData) into the given object state dictionary.
                /// </summary>
                internal void Serialize(IDesignerSerializationManager manager, IDictionary objectData, IDictionary objectState, IList shimObjectNames)
                {
                    if (manager.GetService(typeof(IContainer)) is IContainer container)
                    {
                        SetupVariableReferences(manager, container, objectData, shimObjectNames);
                    }

                    // Next, save a statement collection for each object.
                    StatementContext statementCxt = new StatementContext();
                    statementCxt.StatementCollection.Populate(objectData.Keys);
                    manager.Context.Push(statementCxt);
                    try
                    {
                        foreach (ObjectData data in objectData.Values)
                        {
                            CodeDomSerializer serializer = (CodeDomSerializer)manager.GetSerializer(data._value.GetType(), typeof(CodeDomSerializer));
                            // Saved state. Slot 0 is the code gen
                            // Slot 1 is for generated statements coming from the context.
                            // Slot 2 is an array of default properites.
                            // Slot 3 is for design time props.Any may be null.
                            // Slot 4 is for events that need to be reset.
                            // Slot 5 is for the modifier property of the object.
                            // Since it is DSV.Hidden, it won't be serialized. We special case it here.

                            object[] state = new object[6];
                            CodeStatementCollection extraStatements = new CodeStatementCollection();
                            manager.Context.Push(extraStatements);
                            if (serializer != null)
                            {
                                if (data.EntireObject)
                                {
                                    if (!IsSerialized(manager, data._value))
                                    {
                                        if (data.Absolute)
                                        {
                                            state[StateCode] = serializer.SerializeAbsolute(manager, data._value);
                                        }
                                        else
                                        {
                                            state[StateCode] = serializer.Serialize(manager, data._value);
                                        }
                                        CodeStatementCollection ctxStatements = statementCxt.StatementCollection[data._value];
                                        if (ctxStatements != null && ctxStatements.Count > 0)
                                        {
                                            state[StateCtx] = ctxStatements;
                                        }

                                        if (extraStatements.Count > 0)
                                        {
                                            if (state[StateCode] is CodeStatementCollection existingStatements)
                                            {
                                                existingStatements.AddRange(extraStatements);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        state[StateCode] = statementCxt.StatementCollection[data._value];
                                    }
                                }
                                else
                                {
                                    CodeStatementCollection codeStatements = new CodeStatementCollection();
                                    foreach (MemberData md in data.Members)
                                    {
                                        if (md._member.Attributes.Contains(DesignOnlyAttribute.Yes))
                                        {
                                            // For design time properties, we write their value into a resource blob.
                                            if (md._member is PropertyDescriptor prop && prop.PropertyType.IsSerializable)
                                            {
                                                if (state[StateResources] is null)
                                                {
                                                    state[StateResources] = new Hashtable();
                                                }
                                                ((Hashtable)state[StateResources])[prop.Name] = prop.GetValue(data._value);
                                            }
                                        }
                                        else
                                        {
                                            if (md._absolute)
                                            {
                                                codeStatements.AddRange(serializer.SerializeMemberAbsolute(manager, data._value, md._member));
                                            }
                                            else
                                            {
                                                codeStatements.AddRange(serializer.SerializeMember(manager, data._value, md._member));
                                            }
                                        }
                                    }
                                    state[StateCode] = codeStatements;
                                }
                            }

                            if (extraStatements.Count > 0)
                            {
                                if (state[StateCode] is CodeStatementCollection existingStatements)
                                {
                                    existingStatements.AddRange(extraStatements);
                                }
                            }

                            manager.Context.Pop();
                            // And now search for default properties and events
                            ArrayList defaultPropList = null;
                            List<string> defaultEventList = null;
                            IEventBindingService ebs = manager.GetService(typeof(IEventBindingService)) as IEventBindingService;
                            if (data.EntireObject)
                            {
                                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(data._value);
                                foreach (PropertyDescriptor prop in props)
                                {
                                    if (!prop.ShouldSerializeValue(data._value)
                                        && !prop.Attributes.Contains(DesignerSerializationVisibilityAttribute.Hidden))
                                    {
                                        if (prop.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content) || !prop.IsReadOnly)
                                        {
                                            if (defaultPropList is null)
                                            {
                                                defaultPropList = new ArrayList(data.Members.Count);
                                            }
                                            Trace("Adding default for {0}.{1}", data._name, prop.Name);
                                            defaultPropList.Add(prop.Name);
                                        }
                                    }
                                }

                                if (ebs != null)
                                {
                                    PropertyDescriptorCollection events = ebs.GetEventProperties(TypeDescriptor.GetEvents(data._value));
                                    foreach (PropertyDescriptor eventProp in events)
                                    {
                                        if (eventProp is null || eventProp.IsReadOnly)
                                        {
                                            continue;
                                        }

                                        if (eventProp.GetValue(data._value) is null)
                                        {
                                            if (defaultEventList is null)
                                            {
                                                defaultEventList = new List<string>();
                                            }
                                            defaultEventList.Add(eventProp.Name);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (MemberData md in data.Members)
                                {
                                    if (md._member is PropertyDescriptor prop && !prop.ShouldSerializeValue(data._value))
                                    {
                                        if (ebs != null && ebs.GetEvent(prop) != null)
                                        {
                                            Debug.Assert(prop.GetValue(data._value) is null, "ShouldSerializeValue and GetValue are differing");
                                            if (defaultEventList is null)
                                            {
                                                defaultEventList = new List<string>();
                                            }
                                            defaultEventList.Add(prop.Name);
                                        }
                                        else
                                        {
                                            if (defaultPropList is null)
                                            {
                                                defaultPropList = new ArrayList(data.Members.Count);
                                            }
                                            Trace("Adding default for {0}.{1}", data._name, prop.Name);
                                            defaultPropList.Add(prop.Name);
                                        }
                                    }
                                }
                            }

                            // Check for non-default modifiers property
                            if (TypeDescriptor.GetProperties(data._value)["Modifiers"] is PropertyDescriptor modifier)
                            {
                                state[StateModifier] = modifier.GetValue(data._value);
                            }

                            if (defaultPropList != null)
                            {
                                state[StateProperties] = (string[])defaultPropList.ToArray(typeof(string));
                            }

                            if (defaultEventList != null)
                            {
                                state[StateEvents] = defaultEventList;
                            }

                            if (state[StateCode] != null || state[StateProperties] != null)
                            {
                                objectState[data._name] = state;
                            }
                        }
                    }
                    finally
                    {
                        manager.Context.Pop();
                    }
                }
            }

            /// <summary>
            ///  We create one of these for each specific member on an object.
            /// </summary>
            private class MemberData
            {
                /// <summary>
                ///  The member we're serializing.
                /// </summary>
                internal MemberDescriptor _member;

                /// <summary>
                ///  True if we should try to serialize values that contain their defaults as well.
                /// </summary>
                internal bool _absolute;

                /// <summary>
                ///  Creates a new member data ready to be serialized.
                /// </summary>
                internal MemberData(MemberDescriptor member, bool absolute)
                {
                    _member = member;
                    _absolute = absolute;
                }
            }

            /// <summary>
            ///  We create one of these for each object we process.
            /// </summary>
            private class ObjectData
            {
                private bool _entireObject;
                private bool _absolute;
                private ArrayList _members;

                /// <summary>
                ///  The object value we're serializing.
                /// </summary>
                internal object _value;

                /// <summary>
                ///  The name of the object we're serializing.
                /// </summary>
                internal string _name;

                /// <summary>
                ///  If true, the entire object should be serialized. If false, only the members in the member list should be serialized.
                /// </summary>
                internal bool EntireObject
                {
                    get => _entireObject;
                    set
                    {
                        if (value && _members != null)
                        {
                            _members.Clear();
                        }
                        _entireObject = value;
                    }
                }

                /// <summary>
                ///  If true, the object should be serialized such that during deserialization to an existing object the object is reconstructed entirely. If false, serialize normally
                /// </summary>
                internal bool Absolute
                {
                    get => _absolute;
                    set => _absolute = value;
                }

                /// <summary>
                ///  A list of MemberData objects representing specific members that should be serialized.
                /// </summary>
                internal IList Members
                {
                    get
                    {
                        if (_members is null)
                        {
                            _members = new ArrayList();
                        }
                        return _members;
                    }
                }
            }

            /// <summary>
            ///  Our private resource manager...it just pushes all the data into a hashtable and then we serialize the hashtable.  On deseriaization, the hashtable is rebuilt for us and we have all the data we saved out.
            /// </summary>
            private class LocalResourceManager : ResourceManager, IResourceWriter, IResourceReader
            {
                private Hashtable _hashtable;

                internal LocalResourceManager() { }
                internal LocalResourceManager(Hashtable data) { _hashtable = data; }

                internal Hashtable Data
                {
                    get
                    {
                        if (_hashtable is null)
                        {
                            _hashtable = new Hashtable();
                        }
                        return _hashtable;
                    }
                }

                // IResourceWriter
                public void AddResource(string name, object value) { Data[name] = value; }
                public void AddResource(string name, string value) { Data[name] = value; }
                public void AddResource(string name, byte[] value) { Data[name] = value; }
                public void Close() { }
                public void Dispose() { Data.Clear(); }
                public void Generate() { }

                // IResourceReader / ResourceManager
                public override object GetObject(string name) { return Data[name]; }
                public override string GetString(string name) { return Data[name] as string; }
                public IDictionaryEnumerator GetEnumerator() { return Data.GetEnumerator(); }
                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }
            }

            /// <summary>
            ///  LocalServices contains the services that we add to our serialization manager.  We do this, rather than implement interfaces directly on CodeDomSerializationStore to prevent people from assuming what our implementation is (CodeDomSerializationStore is returned publicly as SerializationStore).
            /// </summary>
            private class LocalServices : IServiceProvider, IResourceService
            {
                private readonly CodeDomSerializationStore _store;
                private readonly IServiceProvider _provider;

                internal LocalServices(CodeDomSerializationStore store, IServiceProvider provider)
                {
                    _store = store;
                    _provider = provider;
                }

                // IResourceService
                IResourceReader IResourceService.GetResourceReader(CultureInfo info) { return _store.Resources; }
                IResourceWriter IResourceService.GetResourceWriter(CultureInfo info) { return _store.Resources; }

                // IServiceProvider
                object IServiceProvider.GetService(Type serviceType)
                {
                    if (serviceType is null)
                    {
                        throw new ArgumentNullException(nameof(serviceType));
                    }

                    if (serviceType == typeof(IResourceService))
                    {
                        return this;
                    }

                    if (_provider != null)
                    {
                        return _provider.GetService(serviceType);
                    }
                    return null;
                }
            }

            private class PassThroughSerializationManager : IDesignerSerializationManager
            {
                readonly Hashtable _resolved = new Hashtable();
                readonly DesignerSerializationManager _manager;
                private ResolveNameEventHandler _resolveNameEventHandler;

                public PassThroughSerializationManager(DesignerSerializationManager manager) => _manager = manager;

                public DesignerSerializationManager Manager
                {
                    get => _manager;
                }

                ContextStack IDesignerSerializationManager.Context
                {
                    get => ((IDesignerSerializationManager)_manager).Context;
                }

                PropertyDescriptorCollection IDesignerSerializationManager.Properties
                {
                    get => ((IDesignerSerializationManager)_manager).Properties;
                }

                event ResolveNameEventHandler IDesignerSerializationManager.ResolveName
                {
                    add
                    {
                        ((IDesignerSerializationManager)_manager).ResolveName += value;
                        _resolveNameEventHandler += value;
                    }
                    remove
                    {
                        ((IDesignerSerializationManager)_manager).ResolveName -= value;
                        _resolveNameEventHandler -= value;
                    }
                }

                event EventHandler IDesignerSerializationManager.SerializationComplete
                {
                    add => ((IDesignerSerializationManager)_manager).SerializationComplete += value;
                    remove => ((IDesignerSerializationManager)_manager).SerializationComplete -= value;
                }

                void IDesignerSerializationManager.AddSerializationProvider(IDesignerSerializationProvider provider)
                {
                    ((IDesignerSerializationManager)_manager).AddSerializationProvider(provider);
                }

                object IDesignerSerializationManager.CreateInstance(Type type, ICollection arguments, string name, bool addToContainer)
                {
                    return ((IDesignerSerializationManager)_manager).CreateInstance(type, arguments, name, addToContainer);
                }

                object IDesignerSerializationManager.GetInstance(string name)
                {
                    object instance = ((IDesignerSerializationManager)_manager).GetInstance(name);

                    // If an object is retrived from the current container as a result of GetInstance(), we need to make sure and fully deserialize it before returning it.  To do this, we will force a resolve on this name and not interfere the next time GetInstance() is called with this component.  This will force the component to completely deserialize.
                    if (_resolveNameEventHandler != null && instance != null && !_resolved.ContainsKey(name) &&
                        _manager.PreserveNames && _manager.Container != null && _manager.Container.Components[name] != null)
                    {
                        _resolved[name] = true;
                        _resolveNameEventHandler(this, new ResolveNameEventArgs(name));
                    }
                    return instance;
                }

                string IDesignerSerializationManager.GetName(object value)
                {
                    return ((IDesignerSerializationManager)_manager).GetName(value);
                }

                object IDesignerSerializationManager.GetSerializer(Type objectType, Type serializerType)
                {
                    return ((IDesignerSerializationManager)_manager).GetSerializer(objectType, serializerType);
                }

                Type IDesignerSerializationManager.GetType(string typeName)
                {
                    return ((IDesignerSerializationManager)_manager).GetType(typeName);
                }

                void IDesignerSerializationManager.RemoveSerializationProvider(IDesignerSerializationProvider provider)
                {
                    ((IDesignerSerializationManager)_manager).RemoveSerializationProvider(provider);
                }

                void IDesignerSerializationManager.ReportError(object errorInformation)
                {
                    ((IDesignerSerializationManager)_manager).ReportError(errorInformation);
                }

                void IDesignerSerializationManager.SetName(object instance, string name)
                {
                    ((IDesignerSerializationManager)_manager).SetName(instance, name);
                }

                object IServiceProvider.GetService(Type serviceType)
                {
                    return ((IDesignerSerializationManager)_manager).GetService(serviceType);
                }
            }

            /// <summary>
            ///  This is a serialzation manager that can load assemblies and search for types and provide a resource manager from our serialization store.
            /// </summary>
            private class LocalDesignerSerializationManager : DesignerSerializationManager
            {
                private readonly CodeDomSerializationStore _store;
                private bool? _typeSvcAvailable;

                /// <summary>
                ///  Creates a new serilalization manager.
                /// </summary>
                internal LocalDesignerSerializationManager(CodeDomSerializationStore store, IServiceProvider provider) : base(provider)
                {
                    _store = store;
                }

                /// <summary>
                ///  We override CreateInstance here to provide a hook to our resource manager.
                /// </summary>
                protected override object CreateInstance(Type type, ICollection arguments, string name, bool addToContainer)
                {
                    if (typeof(ResourceManager).IsAssignableFrom(type))
                    {
                        return _store.Resources;
                    }
                    return base.CreateInstance(type, arguments, name, addToContainer);
                }

                private Nullable<bool> TypeResolutionAvailable
                {
                    get
                    {
                        if (!_typeSvcAvailable.HasValue)
                        {
                            _typeSvcAvailable = new Nullable<bool>(GetService(typeof(ITypeResolutionService)) != null);
                        }
                        return _typeSvcAvailable.Value;
                    }
                }

                /// <summary>
                ///  Override of GetType.  We favor the base implementation first, which uses the type resolution service if it is available.  If that fails, we will try to load assemblies from the given array of assembly names.
                /// </summary>
                protected override Type GetType(string name)
                {
                    Type t = base.GetType(name);
                    if (t is null && !TypeResolutionAvailable.Value)
                    {
                        AssemblyName[] names = _store.AssemblyNames;
                        // First try the assembly names directly.
                        foreach (AssemblyName n in names)
                        {
                            Assembly a = Assembly.Load(n);
                            if (a != null)
                            {
                                t = a.GetType(name);
                                if (t != null)
                                {
                                    break;
                                }
                            }
                        }

                        // Failing that go after their dependencies.
                        if (t is null)
                        {
                            foreach (AssemblyName n in names)
                            {
                                Assembly a = Assembly.Load(n);
                                if (a != null)
                                {
                                    foreach (AssemblyName dep in a.GetReferencedAssemblies())
                                    {
                                        Assembly aDep = Assembly.Load(dep);
                                        if (aDep != null)
                                        {
                                            t = aDep.GetType(name);
                                            if (t != null)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (t != null)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    return t;
                }
            }
        }
    }
}
