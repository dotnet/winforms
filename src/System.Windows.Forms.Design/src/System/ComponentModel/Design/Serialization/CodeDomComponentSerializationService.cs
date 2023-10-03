// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;
using System.Collections;

namespace System.ComponentModel.Design.Serialization;

/// <summary>
///  This class serializes a set of components or serializable objects into a  serialization store.  The store can then be deserialized at a later  date.  CodeDomComponentSerializationService differs from other serialization  schemes in that the serialization format is opaque, and it allows for  partial serialization of objects.  For example, you can choose to  serialize only selected properties for an object.
/// </summary>
public sealed partial class CodeDomComponentSerializationService : ComponentSerializationService
{
    private readonly IServiceProvider? _provider;

    /// <summary>
    ///  Creates a new CodeDomComponentSerializationService object.
    /// </summary>
    public CodeDomComponentSerializationService() : this(null)
    {
    }

    /// <summary>
    ///  Creates a new CodeDomComponentSerializationService object using the given  service provider to resolve services.
    /// </summary>
    public CodeDomComponentSerializationService(IServiceProvider? provider)
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
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(value);

        if (store is not CodeDomSerializationStore cdStore)
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
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(value);

        if (store is not CodeDomSerializationStore cdStore)
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
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(owningObject);
        ArgumentNullException.ThrowIfNull(member);

        if (store is not CodeDomSerializationStore cdStore)
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
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(owningObject);
        ArgumentNullException.ThrowIfNull(member);

        if (store is not CodeDomSerializationStore cdStore)
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
        ArgumentNullException.ThrowIfNull(store);

        if (store is not CodeDomSerializationStore cdStore)
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
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(container);

        if (store is not CodeDomSerializationStore cdStore)
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
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(container);

        if (store is not CodeDomSerializationStore cdStore)
        {
            throw new InvalidOperationException(SR.CodeDomComponentSerializationServiceUnknownStore);
        }

        cdStore.DeserializeTo(_provider!, container, validateRecycledTypes, applyDefaults);
    }

    // Saved state
    internal sealed class CodeDomComponentSerializationState
    {
        public readonly object? Code; // code gen
        public readonly CodeStatementCollection? Ctx; // generated statements coming from the context
        public readonly List<string>? Properties; // default properties
        public readonly Dictionary<string, object?>? Resources; // design time properties
        public readonly List<string>? Events; // events that need to be reset
        public readonly object? Modifier; // modifier property of the object

        public CodeDomComponentSerializationState(object? code,
            CodeStatementCollection? ctxStatements,
            List<string>? properties,
            Dictionary<string, object?>? resources,
            List<string>? events,
            object? modifier)
        {
            Code = code;
            Ctx = ctxStatements;
            Properties = properties;
            Resources = resources;
            Events = events;
            Modifier = modifier;
        }
    }
}
