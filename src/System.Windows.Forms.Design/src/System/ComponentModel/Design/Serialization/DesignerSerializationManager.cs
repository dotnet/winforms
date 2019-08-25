// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace System.ComponentModel.Design.Serialization
{
    /// <summary>
    ///  This object is a complete implementation of IDesignerSerializationManager.  It can be used to  begin the serialization / deserialization process for any serialization scheme that utilizes  IDesignerSerializationManager.
    /// </summary>
    public class DesignerSerializationManager : IDesignerSerializationManager
    {
        private readonly IServiceProvider provider;
        private ITypeResolutionService typeResolver;
        private bool searchedTypeResolver;
        private bool recycleInstances;
        private bool validateRecycledTypes;
        private bool preserveNames;
        private IContainer container;
        private IDisposable session;
        private ResolveNameEventHandler resolveNameEventHandler;
        private EventHandler serializationCompleteEventHandler;
        private EventHandler sessionCreatedEventHandler;
        private EventHandler sessionDisposedEventHandler;
        private ArrayList designerSerializationProviders;
        private Hashtable defaultProviderTable;
        private Hashtable instancesByName;
        private Hashtable namesByInstance;
        private Hashtable serializers;
        private ArrayList errorList;
        private ContextStack contextStack;
        private PropertyDescriptorCollection properties;
        private object propertyProvider;

        /// <summary>
        ///  Creates a new serialization manager.
        /// </summary>
        public DesignerSerializationManager()
        {
            preserveNames = true;
            validateRecycledTypes = true;
        }

        /// <summary>
        ///  Creates a new serialization manager.
        /// </summary>
        public DesignerSerializationManager(IServiceProvider provider)
        {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
            preserveNames = true;
            validateRecycledTypes = true;
        }

        /// <summary>
        ///  Provides access to the container that components will be added to.  The default implementation searches for IDesignerHost in the service provider and uses its container if it exists.
        /// </summary>
        public IContainer Container
        {
            get
            {
                if (container == null)
                {
                    if (GetService(typeof(IDesignerHost)) is IDesignerHost host)
                    {
                        container = host.Container;
                    }
                }
                return container;
            }
            set
            {
                CheckNoSession();
                container = value;
            }
        }

        /// <summary>
        ///  This retrieves the collection of errors that have been reported to the serialization manager.  Additionaly, new errors can be added to the list by accessing this property.
        /// </summary>
        public IList Errors
        {
            get
            {
                CheckSession();
                if (errorList == null)
                {
                    errorList = new ArrayList();
                }
                return errorList;
            }
        }

        /// <summary>
        ///  This property determines the behavior of the CreateInstance method.  If true, CreateInstance will  pass the given component name.  If false, CreateInstance will check for the presence of the given name in the container.  If it does not exist, it will use the given name. If it does exist, it will pass a null value as the name of a component when adding  it to the container, thereby giving it a new name. This second variation is useful for implementing a serializer that always duplicates objects, rather than assuming those objects do not exist.  Paste commands often use this type of serializer. The default value of this property is true.
        /// </summary>
        public bool PreserveNames
        {
            get => preserveNames;
            set
            {
                CheckNoSession();
                preserveNames = value;
            }
        }

        /// <summary>
        ///  This property returns the object that should be used to provide properties to the serialization manager's Propeties property.  This object's  public properties will be inspected and wrapped in new property descriptors that have a target object of the serialization manager.
        /// </summary>
        public object PropertyProvider
        {
            get => propertyProvider;
            set
            {
                if (propertyProvider != value)
                {
                    propertyProvider = value;
                    properties = null;
                }
            }
        }

        /// <summary>
        ///  This property determines the behavior of the CreateInstance method.  If false, CreateInstance will always create a new instance of an object.  If true, CreateInstance will first search the nametable and container for an object of the same name.  If such an object exists and is of the same type, CreateInstance will return the existing object instance.  This second variation is useful for implemeting a serializer that applies serialization state to an existing set of objects, rather than always creating a new tree.  Undo often uses this type of serializer. The default value of this property is false.
        /// </summary>
        public bool RecycleInstances
        {
            get => recycleInstances;
            set
            {
                CheckNoSession();
                recycleInstances = value;
            }
        }

        /// <summary>
        ///  This property determines the behavior of the CreateInstance method and only applies if RecyleInstances is true.  If true, and  an existing instance is found for the given name, it will only be returned if the two types match.  If false, the instance will be returned even if the two types do not match.  This is useful for "morphing" one type of object to another if they have similar properties but share no common parent or interface. The default value of this property is true.
        /// </summary>
        public bool ValidateRecycledTypes
        {
            get => validateRecycledTypes;
            set
            {
                CheckNoSession();
                validateRecycledTypes = value;
            }
        }

        /// <summary>
        ///  Event that is raised when a session is created.
        /// </summary>
        public event EventHandler SessionCreated
        {
            add => sessionCreatedEventHandler += value;
            remove => sessionCreatedEventHandler -= value;
        }

        /// <summary>
        ///  Event that is raised when a session is disposed.
        /// </summary>
        public event EventHandler SessionDisposed
        {
            add => sessionDisposedEventHandler += value;
            remove => sessionDisposedEventHandler -= value;
        }

        /// <summary>
        ///  Used to verify that no session is active.  If there is, this method throws.
        /// </summary>
        private void CheckNoSession()
        {
            if (session != null)
            {
                throw new InvalidOperationException(SR.SerializationManagerWithinSession);
            }
        }

        /// <summary>
        ///  Used to verify that there is an open session.  If there isn't, this method throws.
        /// </summary>
        private void CheckSession()
        {
            if (session == null)
            {
                throw new InvalidOperationException(SR.SerializationManagerNoSession);
            }
        }

        /// <summary>
        ///  Creates an instance of the specified type.  The default implementation will create the object.  If the object implements IComponent and only requires an empty or IContainer style constructor, this will search for IDesignerHost and create through the host.  Otherwise it will use reflection.  If addToContainer is true, this will add to the container using this class's Conainer property, using the name provided if it is not null.
        /// </summary>
        protected virtual object CreateInstance(Type type, ICollection arguments, string name, bool addToContainer)
        {
            object[] argArray = null;
            if (arguments != null && arguments.Count > 0)
            {
                argArray = new object[arguments.Count];
                arguments.CopyTo(argArray, 0);
            }

            object instance = null;
            // If we have been asked to recycle instances, look in our nametable and container first for an object matching this name and type.  If we find it, we will use it.
            if (RecycleInstances && name != null)
            {

                if (instancesByName != null)
                {
                    instance = instancesByName[name];
                }

                if (instance == null && addToContainer && Container != null)
                {
                    instance = Container.Components[name];
                }

                if (instance != null && ValidateRecycledTypes && instance.GetType() != type)
                {
                    // We got an instance, but it is not of the correct type.  We don't allow this.
                    instance = null;
                }
            }

            // If the stars properly align, we will let the designer host create the component.   For this to happen, the following criteria must hold true:
            // 1.  The type must be a component.
            // 2.  addToContainer is true.
            // 3.  The type has a null ctor or an IContainer ctor.
            // 4.  The host is available and its container matches our container.
            // The reason for this is that if we went through activator, and if the object already specified a constructor that took an IContainer, our deserialization mechanism would equate the container to the designer host.  This is the correct thing to do, but it has the side effect of adding the compnent to the designer host twice -- once with a default name, and a second time with the name we provide.  This equates to a component rename, which isn't cheap,  so we don't want to do it when we load each and every component.
            if (instance == null && addToContainer && typeof(IComponent).IsAssignableFrom(type) && (argArray == null || argArray.Length == 0 || (argArray.Length == 1 && argArray[0] == Container)))
            {
                if (GetService(typeof(IDesignerHost)) is IDesignerHost host && host.Container == Container)
                {
                    bool ignoreName = false;
                    if (!PreserveNames && name != null)
                    {
                        // Check if this name exists in the container.  If so, don't use it.
                        if (Container.Components[name] != null)
                        {
                            ignoreName = true;
                        }
                    }

                    if (name == null || ignoreName)
                    {
                        instance = host.CreateComponent(type);
                    }
                    else
                    {
                        instance = host.CreateComponent(type, name);
                    }
                }
            }

            // Default case, just create the component through reflection.
            if (instance == null)
            {
                try
                {
                    try
                    {
                        // First, just try to create the object directly with the arguments.  generaly this should work.
                        instance = TypeDescriptor.CreateInstance(provider, type, null, argArray);
                    }
                    catch (MissingMethodException mmex)
                    {
                        // okay, the create failed because the argArray didn't match the types of ctors that are available.  don't panic, we're tough.  we'll try to coerce the types to match the ctor.
                        Type[] types = new Type[argArray.Length];
                        // first, get the types of the arguments we've got.
                        for (int index = 0; index < argArray.Length; index++)
                        {
                            if (argArray[index] != null)
                            {
                                types[index] = argArray[index].GetType();
                            }
                        }

                        object[] tempArgs = new object[argArray.Length];
                        // now, walk the public ctors looking for one to  invoke here with the arguments we have.
                        foreach (ConstructorInfo ci in TypeDescriptor.GetReflectionType(type).GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance))
                        {
                            ParameterInfo[] pi = ci.GetParameters();
                            // obviously the count has to match
                            if (pi != null && pi.Length == types.Length)
                            {
                                bool match = true;
                                // now walk every type of argument and compare it to the corresponding argument.  if it matches up exactly or is a derived type, great. otherwise, we'll try to use IConvertible to make it into the right thing.
                                for (int t = 0; t < types.Length; t++)
                                {
                                    if (types[t] == null || pi[t].ParameterType.IsAssignableFrom(types[t]))
                                    {
                                        tempArgs[t] = argArray[t];
                                        continue;
                                    }

                                    if (argArray[t] is IConvertible)
                                    {
                                        try
                                        {
                                            // try the IConvertible route.  If it works, we'll call it a match for this parameter and continue on.
                                            tempArgs[t] = ((IConvertible)argArray[t]).ToType(pi[t].ParameterType, null);
                                            continue;
                                        }
                                        catch (InvalidCastException)
                                        {
                                        }
                                    }
                                    match = false;
                                    break;
                                }
                                // all of the parameters were converted or matched, so try the creation again. if that works, we're in the money.
                                if (match)
                                {
                                    instance = TypeDescriptor.CreateInstance(provider, type, null, tempArgs);
                                    break;
                                }
                            }
                        }

                        // we still failed...rethrow the original exception.
                        if (instance == null)
                        {
                            throw mmex;
                        }
                    }
                }
                catch (MissingMethodException)
                {
                    StringBuilder argTypes = new StringBuilder();
                    foreach (object o in argArray)
                    {
                        if (argTypes.Length > 0)
                        {
                            argTypes.Append(", ");
                        }

                        if (o != null)
                        {
                            argTypes.Append(o.GetType().Name);
                        }
                        else
                        {
                            argTypes.Append("null");
                        }

                    }
                    Exception ex = new SerializationException(string.Format(SR.SerializationManagerNoMatchingCtor, type.FullName, argTypes.ToString()))
                    {
                        HelpLink = SR.SerializationManagerNoMatchingCtor
                    };
                    throw ex;
                }

                // Now, if we needed to add this to the container, do so .
                if (addToContainer && instance is IComponent && Container != null)
                {
                    bool ignoreName = false;
                    if (!PreserveNames && name != null)
                    {
                        // Check if this name exists in the container.  If so, don't use it.
                        if (Container.Components[name] != null)
                        {
                            ignoreName = true;
                        }
                    }

                    if (name == null || ignoreName)
                    {
                        Container.Add((IComponent)instance);
                    }
                    else
                    {
                        Container.Add((IComponent)instance, name);
                    }
                }
            }
            return instance;
        }

        /// <summary>
        ///  Creates a new serialization session.  Most data within the serialization manager is transient and only lives for the life of a serialization session.  When a session is disposed, serialization is considered to be complete and this transient state is cleared.  This allows a single instance of a serialization manager to be used to serialize multiple object trees.  Some state, including the service provider and any custom serialization providers that were added to the serialization manager, span sessions.	
        /// </summary>
        public IDisposable CreateSession()
        {
            if (session != null)
            {
                throw new InvalidOperationException(SR.SerializationManagerAreadyInSession);
            }

            session = new SerializationSession(this);
            OnSessionCreated(EventArgs.Empty);
            return session;
        }

        /// <summary>
        ///  This retrieves the serializer for the given object type. You can request what type of serializer you would like. It is possible for this method to return null if there is no serializer of the requested type.
        /// </summary>
        public object GetSerializer(Type objectType, Type serializerType)
        {
            if (serializerType == null)
            {
                throw new ArgumentNullException(nameof(serializerType));
            }

            object serializer = null;
            if (objectType != null)
            {
                if (serializers != null)
                {
                    // I don't double hash here.  It will be a very rare day where multiple types of serializers will be used in the same scheme. We still handle it, but we just don't cache.
                    serializer = serializers[objectType];
                    if (serializer != null && !serializerType.IsAssignableFrom(serializer.GetType()))
                    {
                        serializer = null;
                    }
                }

                // Now actually look in the type's metadata.
                if (serializer == null)
                {
                    AttributeCollection attributes = TypeDescriptor.GetAttributes(objectType);
                    foreach (Attribute attr in attributes)
                    {
                        if (attr is DesignerSerializerAttribute da)
                        {
                            string typeName = da.SerializerBaseTypeName;

                            // This serializer must support the correct base type or we're not interested.
                            if (typeName != null)
                            {
                                Type baseType = GetRuntimeType(typeName);
                                if (baseType == serializerType && da.SerializerTypeName != null && da.SerializerTypeName.Length > 0)
                                {
                                    Type type = GetRuntimeType(da.SerializerTypeName);
                                    if (type != null)
                                    {
                                        serializer = Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    // And stash this little guy for later, but only if we're in a session. If we're outside of a session this should still be useable for resolving serializers, but we don't cache them.
                    if (serializer != null && session != null)
                    {
                        if (serializers == null)
                        {
                            serializers = new Hashtable();
                        }
                        serializers[objectType] = serializer;
                    }
                }
            }

            // Check for a default serialization provider
            if (defaultProviderTable == null || !defaultProviderTable.ContainsKey(serializerType))
            {
                Type defaultSerializerType = null;
                DefaultSerializationProviderAttribute a = (DefaultSerializationProviderAttribute)TypeDescriptor.GetAttributes(serializerType)[typeof(DefaultSerializationProviderAttribute)];
                if (a != null)
                {
                    defaultSerializerType = GetRuntimeType(a.ProviderTypeName);
                    if (defaultSerializerType != null && typeof(IDesignerSerializationProvider).IsAssignableFrom(defaultSerializerType))
                    {
                        IDesignerSerializationProvider p = (IDesignerSerializationProvider)Activator.CreateInstance(
                            defaultSerializerType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, null, null);
                        ((IDesignerSerializationManager)this).AddSerializationProvider(p);
                    }
                }

                if (defaultProviderTable == null)
                {
                    defaultProviderTable = new Hashtable();
                }
                defaultProviderTable[serializerType] = defaultSerializerType;
            }

            // Designer serialization providers can override our metadata discovery. We loop until we reach steady state.  This breaks order dependencies by allowing all providers a chance to party on each other's serializers.
            if (designerSerializationProviders != null)
            {
                bool continueLoop = true;
                for (int i = 0; continueLoop && i < designerSerializationProviders.Count; i++)
                {
                    continueLoop = false;
                    foreach (IDesignerSerializationProvider provider in designerSerializationProviders)
                    {
                        object newSerializer = provider.GetSerializer(this, serializer, objectType, serializerType);
                        if (newSerializer != null)
                        {
                            continueLoop = (serializer != newSerializer);
                            serializer = newSerializer;
                        }
                    }
                }
            }
            return serializer;
        }

        /// <summary>
        ///  Provides access to the underlying IServiceProvider
        /// </summary>
        protected virtual object GetService(Type serviceType)
        {
            if (serviceType == typeof(IContainer))
            {
                return Container;
            }

            if (provider != null)
            {
                return provider.GetService(serviceType);
            }
            return null;
        }

        /// <summary>
        ///  Retrieves the type for the given name using the type resolution service, if available.
        /// </summary>
        protected virtual Type GetType(string typeName)
        {
            Type type = GetRuntimeType(typeName);
            if (type != null)
            {
                if (GetService(typeof(TypeDescriptionProviderService)) is TypeDescriptionProviderService typeProviderService)
                {
                    TypeDescriptionProvider typeProvider = typeProviderService.GetProvider(type);
                    if (typeProvider != null && !typeProvider.IsSupportedType(type))
                    {
                        type = null;
                    }
                }
            }
            return type;
        }

        /// <summary>
        ///  Use this one for Serializer, converter, designer and editor types, types which  are only used by the IDE internally
        /// </summary>
        public Type GetRuntimeType(string typeName)
        {
            if (typeResolver == null && !searchedTypeResolver)
            {
                typeResolver = GetService(typeof(ITypeResolutionService)) as ITypeResolutionService;
                searchedTypeResolver = true;
            }

            Type type;
            if (typeResolver == null)
            {
                type = Type.GetType(typeName);
            }
            else
            {
                type = typeResolver.GetType(typeName);
            }
            return type;
        }

        /// <summary>
        ///  Event that is raised when a name needs to be resolved to an object instance..
        /// </summary>
        protected virtual void OnResolveName(ResolveNameEventArgs e)
        {
            resolveNameEventHandler?.Invoke(this, e);
        }

        /// <summary>
        ///  Event that is raised when a session is created.
        /// </summary>
        protected virtual void OnSessionCreated(EventArgs e)
        {
            sessionCreatedEventHandler?.Invoke(this, e);
        }

        /// <summary>
        ///  Event that is raised when a session is disposed.
        /// </summary>
        protected virtual void OnSessionDisposed(EventArgs e)
        {
            try
            {
                try
                {
                    sessionDisposedEventHandler?.Invoke(this, e);
                }
                finally
                {
                    serializationCompleteEventHandler?.Invoke(this, e);
                }
            }
            finally
            {
                resolveNameEventHandler = null;
                serializationCompleteEventHandler = null;
                instancesByName = null;
                namesByInstance = null;
                serializers = null;
                contextStack = null;
                errorList = null;
                session = null;
            }
        }

        /// <summary>
        ///  This method takes a property that is owned by the given owner, and it wraps them in new property that is owned by the serialization manager.
        /// </summary>
        private PropertyDescriptor WrapProperty(PropertyDescriptor property, object owner)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }
            // owner can be null for static properties.
            return new WrappedPropertyDescriptor(property, owner);
        }

        /// <summary>
        ///  The Context property provides a user-defined storage area implemented as a stack.  This storage area is a useful way to provide communication across serializers, as serialization is a generally hierarchial process.
        /// </summary>
        ContextStack IDesignerSerializationManager.Context
        {
            get
            {
                if (contextStack == null)
                {
                    CheckSession();
                    contextStack = new ContextStack();
                }
                return contextStack;
            }
        }

        /// <summary>
        ///  The Properties property provides a set of custom properties the serialization manager may surface.  The set of properties exposed here is defined by the implementor of  IDesignerSerializationManager.
        /// </summary>
        PropertyDescriptorCollection IDesignerSerializationManager.Properties
        {
            get
            {
                if (properties == null)
                {
                    object propObject = PropertyProvider;
                    PropertyDescriptor[] propArray;
                    if (propObject == null)
                    {
                        propArray = Array.Empty<PropertyDescriptor>();
                    }
                    else
                    {
                        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(propObject);
                        propArray = new PropertyDescriptor[props.Count];
                        for (int i = 0; i < propArray.Length; i++)
                        {
                            propArray[i] = WrapProperty(props[i], propObject);
                        }

                    }
                    properties = new PropertyDescriptorCollection(propArray);
                }
                return properties;
            }
        }

        /// <summary>
        ///  ResolveName event.  This event is raised when GetName is called, but the name is not found in the serialization manager's name table.  It provides a  way for a serializer to demand-create an object so the serializer does not have to order object creation by dependency.  This delegate is cleared immediately after serialization or deserialization is complete.
        /// </summary>
        event ResolveNameEventHandler IDesignerSerializationManager.ResolveName
        {
            add
            {
                CheckSession();
                resolveNameEventHandler += value;
            }
            remove => resolveNameEventHandler -= value;
        }

        /// <summary>
        ///  This event is raised when serialization or deserialization has been completed.  Generally, serialization code should be written to be stateless.  Should some sort of state be necessary to maintain, a serializer can listen to this event to know when that state should be cleared. An example of this is if a serializer needs to write to another file, such as a resource file.  In this case it would be inefficient to design the serializer to close the file when finished because serialization of an object graph generally requires several serializers. The resource file would be opened and closed many times. Instead, the resource file could be accessed through an object that listened to the SerializationComplete event, and that object could close the resource file at the end of serialization.
        /// </summary>
        event EventHandler IDesignerSerializationManager.SerializationComplete
        {
            add
            {
                CheckSession();
                serializationCompleteEventHandler += value;
            }
            remove => serializationCompleteEventHandler -= value;
        }

        /// <summary>
        ///  This method adds a custom serialization provider to the serialization manager. A custom serialization provider will get the opportunity to return a serializer for a data type before the serialization manager looks in the type's metadata.
        /// </summary>
        void IDesignerSerializationManager.AddSerializationProvider(IDesignerSerializationProvider provider)
        {
            if (designerSerializationProviders == null)
            {
                designerSerializationProviders = new ArrayList();
            }
            if (!designerSerializationProviders.Contains(provider))
            {
                designerSerializationProviders.Add(provider);
            }
        }

        /// <summary>
        ///  Creates an instance of the given type and adds it to a collection of named instances.  Objects that implement IComponent will be added to the design time container if addToContainer is true.
        /// </summary>
        object IDesignerSerializationManager.CreateInstance(Type type, ICollection arguments, string name, bool addToContainer)
        {
            CheckSession();
            // If we were given a name verify that the name doesn't already exist.  We do not verify uniqueness in our parent container here because CreateInstance may modify the name for us.  If it didn't, the container itself will throw, so we're covered.
            if (name != null)
            {
                if (instancesByName != null && instancesByName.ContainsKey(name))
                {
                    Exception ex = new SerializationException(string.Format(SR.SerializationManagerDuplicateComponentDecl, name))
                    {
                        HelpLink = SR.SerializationManagerDuplicateComponentDecl
                    };
                    throw ex;
                }
            }

            object instance = CreateInstance(type, arguments, name, addToContainer);
            // If we have a name save it into our own nametable.  We do this even for objects that were added to the container, because containers reserve the right to change the name in case of collision. Changing the name is fine, but it would be very difficult to map that to the rest of the serializers.  Instead, we keep the old name in our local nametable so a serializer can ask for an object by the old name.  Because the old nametable is searched first, the old name is preserved.  Note that this technique is ONLY valid if RecycleInstances is false.  If it is true, we cannot store in the old nametable because it would be possible to fetch the wrong value from the container. Consider a request for "button1' that results in the creation of an object in the container called "button2" due to a collision.  If a request later came in for "button2", the wrong object would be returned because RecycleInstances checks the container first.  So, it is only safe to store the local value if RecycleInstances is false. When RecycleInstances is true if there was a collision the object would have been returned, so there is no need to store locally.
            if (name != null // If we were given a name
                && (!(instance is IComponent) // And it's not an icomponent
                    || !RecycleInstances))
            { // Or it is an icomponent but recycle instances is turned off
                if (instancesByName == null)
                {
                    instancesByName = new Hashtable();
                    namesByInstance = new Hashtable(new ReferenceComparer());
                }
                instancesByName[name] = instance;
                namesByInstance[instance] = name;
            }
            return instance;
        }

        /// <summary>
        ///  Retrieves an instance of a created object of the given name, or null if that object does not exist.
        /// </summary>
        object IDesignerSerializationManager.GetInstance(string name)
        {
            object instance = null;
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            CheckSession();
            // Check our local nametable first
            if (instancesByName != null)
            {
                instance = instancesByName[name];
            }

            if (instance == null && PreserveNames && Container != null)
            {
                instance = Container.Components[name];
            }

            if (instance == null)
            {
                ResolveNameEventArgs e = new ResolveNameEventArgs(name);
                OnResolveName(e);
                instance = e.Value;
            }
            return instance;
        }

        /// <summary>
        ///  Retrieves a name for the specified object, or null if the object has no name.
        /// </summary>
        string IDesignerSerializationManager.GetName(object value)
        {
            string name = null;
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            CheckSession();
            // Check our local nametable first
            if (namesByInstance != null)
            {
                name = (string)namesByInstance[value];
            }

            if (name == null && value is IComponent)
            {
                ISite site = ((IComponent)value).Site;
                if (site != null)
                {
                    if (site is INestedSite nestedSite)
                    {
                        name = nestedSite.FullName;
                    }
                    else
                    {
                        name = site.Name;
                    }
                }
            }
            return name;
        }

        /// <summary>
        ///  Retrieves a serializer of the requested type for the given object type.
        /// </summary>
        object IDesignerSerializationManager.GetSerializer(Type objectType, Type serializerType)
        {
            return GetSerializer(objectType, serializerType);
        }

        /// <summary>
        ///  Retrieves a type of the given name.
        /// </summary>
        Type IDesignerSerializationManager.GetType(string typeName)
        {
            CheckSession();
            Type t = null;
            while (t == null)
            {
                t = GetType(typeName);
                if (t == null)
                {
                    if (string.IsNullOrEmpty(typeName))
                    {
                        break;
                    }

                    int dotIndex = typeName.LastIndexOf('.');
                    if (dotIndex == -1 || dotIndex == typeName.Length - 1)
                    {
                        break;
                    }
                    typeName = typeName.Substring(0, dotIndex) + "+" + typeName.Substring(dotIndex + 1, typeName.Length - dotIndex - 1);
                }
            }
            return t;
        }

        /// <summary>
        ///  Removes a previously added serialization provider.
        /// </summary>
        void IDesignerSerializationManager.RemoveSerializationProvider(IDesignerSerializationProvider provider)
        {
            if (designerSerializationProviders != null)
            {
                designerSerializationProviders.Remove(provider);
            }
        }

        /// <summary>
        ///  Reports a non-fatal error in serialization.  The serialization manager may implement a logging scheme to alert the caller to all non-fatal errors at once.  If it doesn't, it should immediately throw in this method, which should abort serialization.   Serialization may continue after calling this function.
        /// </summary>
        void IDesignerSerializationManager.ReportError(object errorInformation)
        {
            CheckSession();
            if (errorInformation != null)
            {
                Errors.Add(errorInformation);
            }
        }

        internal ArrayList SerializationProviders
        {
            get => (designerSerializationProviders != null) ? designerSerializationProviders.Clone() as ArrayList : new ArrayList();
        }

        /// <summary>
        ///  Provides a way to set the name of an existing object. This is useful when it is necessary to create an  instance of an object without going through CreateInstance. An exception will be thrown if you try to rename an existing object or if you try to give a new object a name that is already taken.
        /// </summary>
        void IDesignerSerializationManager.SetName(object instance, string name)
        {
            CheckSession();
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (instancesByName == null)
            {
                instancesByName = new Hashtable();
                namesByInstance = new Hashtable(new ReferenceComparer());
            }

            if (instancesByName[name] != null)
            {
                throw new ArgumentException(string.Format(SR.SerializationManagerNameInUse, name), nameof(name));
            }

            if (namesByInstance[instance] != null)
            {
                throw new ArgumentException(string.Format(SR.SerializationManagerObjectHasName, name, (string)namesByInstance[instance]), nameof(instance));
            }
            instancesByName[name] = instance;
            namesByInstance[instance] = name;
        }

        /// <summary>
        ///  IServiceProvider implementation.
        /// </summary>
        object IServiceProvider.GetService(Type serviceType)
        {
            return GetService(serviceType);
        }

        /// <summary>
        ///  Session object that implements IDisposable.
        /// </summary>
        private sealed class SerializationSession : IDisposable
        {
            private readonly DesignerSerializationManager _serializationManager;

            internal SerializationSession(DesignerSerializationManager serializationManager)
            {
                _serializationManager = serializationManager;
            }

            public void Dispose()
            {
                _serializationManager.OnSessionDisposed(EventArgs.Empty);
            }
        }

        /// <summary>
        ///  A key comparer that can be passed to a hash table to use object reference identity as the key comparision.
        /// </summary>
        private sealed class ReferenceComparer : IEqualityComparer
        {
            bool IEqualityComparer.Equals(object x, object y)
            {
                return object.ReferenceEquals(x, y);
            }

            int IEqualityComparer.GetHashCode(object x)
            {
                if (x != null)
                {
                    return x.GetHashCode();
                }
                return 0;
            }
        }

        /// <summary>
        ///  Wrapped property descriptor.  Takes the given property  and wraps it in a new one that can take the designer serialization manager as a target.
        /// </summary>
        private sealed class WrappedPropertyDescriptor : PropertyDescriptor
        {
            private readonly object target;
            private readonly PropertyDescriptor property;

            internal WrappedPropertyDescriptor(PropertyDescriptor property, object target) : base(property.Name, null)
            {
                this.property = property;
                this.target = target;
            }

            public override AttributeCollection Attributes
            {
                get => property.Attributes;
            }

            public override Type ComponentType
            {
                get => property.ComponentType;
            }

            public override bool IsReadOnly
            {
                get => property.IsReadOnly;
            }

            public override Type PropertyType
            {
                get => property.PropertyType;
            }

            public override bool CanResetValue(object component)
            {
                return property.CanResetValue(target);
            }

            public override object GetValue(object component)
            {
                return property.GetValue(target);
            }

            public override void ResetValue(object component)
            {
                property.ResetValue(target);
            }

            public override void SetValue(object component, object value)
            {
                property.SetValue(target, value);
            }

            public override bool ShouldSerializeValue(object component)
            {
                return property.ShouldSerializeValue(target);
            }
        }
    }
}
