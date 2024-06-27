// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System.Windows.Forms.BinaryFormat.Deserializer;

#pragma warning disable SYSLIB0050 // Type or member is obsolete

/// <summary>
///  General binary format deserializer.
/// </summary>
/// <remarks>
///  <para>
///   This has some constraints over the BinaryFormatter. Notably it does not support all <see cref="IObjectReference"/>
///   usages or surrogates that replace object instances. This greatly simplifies the deserialization. It also does not
///   allow offset arrays (arrays that have lower bounds other than zero) or multidimensional arrays that have more
///   than <see cref="int.MaxValue"/> elements.
///  </para>
///  <para>
///   This deserializer ensures that all value types are assigned to fields or populated in <see cref="SerializationInfo"/>
///   callbacks with their final state, throwing if that is impossible to attain due to graph cycles or data corruption.
///   The value type instance may contain references to uncompleted reference types when there are cycles in the graph.
///   In general it is risky to dereference reference types in <see cref="ISerializable"/> constructors or in
///   <see cref="ISerializationSurrogate"/> call backs if there is any risk of the objects enabling a cycle.
///  </para>
///  <para>
///   If you need to dereference reference types in <see cref="SerializationInfo"/> waiting for final state by
///   implementing <see cref="IDeserializationCallback"/> or <see cref="OnDeserializedAttribute"/> is the safer way to
///   do so. This deserializer does not fire completed events until the entire graph has been deserialized. If a
///   surrogate (<see cref="ISerializationSurrogate"/>) needs to dereference with potential cycles it would require
///   tracking instances by stashing them in a provided <see cref="StreamingContext"/> to handle after invoking the
///   deserializer.
///  </para>
/// </remarks>
/// <devdoc>
///  <see cref="IObjectReference"/> makes deserializing difficult as you don't know the final type until you've finished
///  populating the serialized type. If <see cref="SerializationInfo"/> is involved and you have a cycle you may never
///  be able to complete the deserialization as the reference type values in the <see cref="SerializationInfo"/> can't
///  get the final object.
///
///  <see cref="IObjectReference"/> is really the only practical way to represent singletons. A common pattern is to
///  nest an <see cref="IObjectReference"/> object in an <see cref="ISerializable"/> object. Specifying the nested
///  type when <see cref="ISerializable.GetObjectData(SerializationInfo, StreamingContext)"/> is called by invoking
///  <see cref="SerializationInfo.SetType(Type)"/> will get that type info serialized into the stream.
/// </devdoc>
internal sealed partial class Deserializer : IDeserializer
{
    private readonly IReadOnlyRecordMap _recordMap;
    private readonly BinaryFormattedObject.ITypeResolver _typeResolver;
    BinaryFormattedObject.ITypeResolver IDeserializer.TypeResolver => _typeResolver;

    /// <inheritdoc cref="IDeserializer.Options"/>
    private BinaryFormattedObject.Options Options { get; }
    BinaryFormattedObject.Options IDeserializer.Options => Options;

    /// <inheritdoc cref="IDeserializer.DeserializedObjects"/>
    private readonly Dictionary<int, object> _deserializedObjects = [];
    IDictionary<int, object> IDeserializer.DeserializedObjects => _deserializedObjects;

    // Surrogate cache.
    private readonly Dictionary<Type, ISerializationSurrogate?>? _surrogates;

    // Keeping a separate stack for ids for fast infinite loop checks.
    private readonly Stack<ObjectRecordDeserializer> _parserStack = [];

    /// <inheritdoc cref="IDeserializer.IncompleteObjects"/>
    private readonly HashSet<int> _incompleteObjects = [];
    public IReadOnlySet<int> IncompleteObjects => _incompleteObjects;

    private readonly PendingUpdates _pending;

    // Kept as a field to avoid allocating a new one every time we complete objects.
    private readonly Queue<int> _pendingCompletions = [];

    private readonly Id _rootId;

    // We group individual object events here to fire them all when we complete the graph.
    private event Action<object?>? OnDeserialization;
    private event Action<StreamingContext>? OnDeserialized;

    private Deserializer(
        Id rootId,
        IReadOnlyRecordMap recordMap,
        BinaryFormattedObject.ITypeResolver typeResolver,
        BinaryFormattedObject.Options options)
    {
        _rootId = rootId;
        _recordMap = recordMap;
        _typeResolver = typeResolver;
        Options = options;
        _pending = new(_deserializedObjects);

        if (Options.SurrogateSelector is not null)
        {
            _surrogates = [];
        }
    }

    /// <summary>
    ///  Deserializes the object graph for the given <paramref name="recordMap"/> and <paramref name="rootId"/>.
    /// </summary>
    [RequiresUnreferencedCode("Calls System.Windows.Forms.BinaryFormat.Deserializer.Deserializer.Deserialize()")]
    internal static object Deserialize(
        Id rootId,
        IReadOnlyRecordMap recordMap,
        BinaryFormattedObject.ITypeResolver typeResolver,
        BinaryFormattedObject.Options options)
    {
        var deserializer = new Deserializer(rootId, recordMap, typeResolver, options);
        return deserializer.Deserialize();
    }

    [RequiresUnreferencedCode("Calls System.Windows.Forms.BinaryFormat.Deserializer.Deserializer.DeserializeRoot(Id)")]
    private object Deserialize()
    {
        DeserializeRoot(_rootId);

        // Complete all pending SerializationInfo objects.
        int pendingCount = _pending.PendingSerializationInfoCount;
        while (_pending.TryDequeuePendingSerializationInfo(out PendingSerializationInfo? pending))
        {
            // Using pendingCount to only requeue on the first pass.
            if (--pendingCount >= 0
                && _pending.PendingSerializationInfoCount != 0
                && _pending.TryGetIncompleteDependencies(pending.ObjectId, out HashSet<int>? dependencies))
            {
                // We can get here with nested ISerializable value types.

                // Hopefully another pass will complete this.
                if (dependencies.Count > 0)
                {
                    _pending.Enqueue(pending);
                    continue;
                }

                Debug.Fail("Completed dependencies should have been removed from the dictionary.");
            }

            // All _pendingSerializationInfo objects are considered incomplete.
            pending.Populate(_deserializedObjects, Options.StreamingContext);
            ((IDeserializer)this).CompleteObject(pending.ObjectId);
        }

        if (_incompleteObjects.Count > 0 || _pending.PendingValueUpdatesCount > 0)
        {
            // This should never happen outside of corrupted data.
            throw new SerializationException("Objects could not be deserialized completely.");
        }

        // Notify [OnDeserialized] instance methods for all relevant deserialized objects,
        // then callback IDeserializationCallback on all objects that implement it.
        OnDeserialized?.Invoke(Options.StreamingContext);
        OnDeserialization?.Invoke(null);

        return _deserializedObjects[_rootId];
    }

    [RequiresUnreferencedCode("Calls DeserializeNew(Id)")]
    private void DeserializeRoot(Id rootId)
    {
        object root = DeserializeNew(rootId);
        if (root is not ObjectRecordDeserializer parser)
        {
            return;
        }

        _parserStack.Push(parser);

        while (_parserStack.TryPop(out ObjectRecordDeserializer? currentParser))
        {
            Id requiredId;
            while (!(requiredId = currentParser.Continue()).IsNull)
            {
                // A record is required to complete the current parser. Get it.
                object requiredObject = DeserializeNew(requiredId);
                Debug.Assert(requiredObject is not IRecord);

                if (requiredObject is ObjectRecordDeserializer requiredParser)
                {
                    // Push our current parser.
                    _parserStack.Push(currentParser);

                    // Push the required parser so we can complete it.
                    _parserStack.Push(requiredParser);

                    break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [RequiresUnreferencedCode("Calls System.Windows.Forms.BinaryFormat.Deserializer.ObjectRecordParser.Create(Id, IRecord, IDeserializer)")]
        object DeserializeNew(Id id)
        {
            // Strings, string arrays, and primitive arrays can be completed without creating a
            // parser object. Single primitives don't normally show up as records unless they are top
            // level or are boxed into an interface reference. Checking for these requires costly
            // string matches and as such we'll just create the parser object.

            IRecord record = _recordMap[id];
            if (record is BinaryObjectString binaryString)
            {
                _deserializedObjects.Add(id, binaryString.Value);
                return binaryString.Value;
            }

            if (record is ArrayRecord arrayRecord)
            {
                Array? values = arrayRecord switch
                {
                    ArraySingleString stringArray => stringArray.ToArray(),
                    IPrimitiveTypeRecord primitiveArray => primitiveArray.GetPrimitiveArray(),
                    _ => null
                };

                if (values is not null)
                {
                    _deserializedObjects.Add(arrayRecord.ObjectId, values);
                    return values;
                }
            }

            // Not a simple case, need to do a full deserialization of the record.
            if (!_incompleteObjects.Add(id))
            {
                // All objects should be available before they're asked for a second time.
                throw new SerializationException("Unexpected parser cycle.");
            }

            var deserializer = ObjectRecordDeserializer.Create(id, record, this);

            // Add the object as soon as possible to support circular references.
            _deserializedObjects.Add(id, deserializer.Object);
            return deserializer;
        }
    }

    ISerializationSurrogate? IDeserializer.GetSurrogate(Type type)
    {
        // If we decide not to cache, this method could be moved to the callsite.

        if (_surrogates is null)
        {
            return null;
        }

        Debug.Assert(Options.SurrogateSelector is not null);

        if (!_surrogates.TryGetValue(type, out ISerializationSurrogate? surrogate))
        {
            surrogate = Options.SurrogateSelector.GetSurrogate(type, Options.StreamingContext, out _);
            _surrogates[type] = surrogate;
        }

        return surrogate;
    }

    void IDeserializer.PendSerializationInfo(PendingSerializationInfo pending) => _pending.Enqueue(pending);

    void IDeserializer.PendValueUpdater(ValueUpdater updater) => _pending.Add(updater);

    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = "The type is already in the cache of the TypeResolver, no need to mark this one again.")]
    void IDeserializer.RegisterCompleteEvents(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type,
        object @object)
    {
        OnDeserialized += SerializationEvents.GetOnDeserializedForType(type, @object);

        if (@object is IDeserializationCallback callback)
        {
            OnDeserialization += callback.OnDeserialization;
        }
    }

    void IDeserializer.CompleteObject(Id id)
    {
        // Need to use a queue as Completion is recursive.

        _pendingCompletions.Enqueue(id);
        bool recursed = false;

        while (_pendingCompletions.TryDequeue(out int completedId))
        {
            _incompleteObjects.Remove(completedId);

            // When we've recursed, we've done so because there are no more dependencies for the current id, so we can
            // remove it from the dictionary. We have to pend as we can't remove while we're iterating the dictionary.
            if (recursed)
            {
                _pending.RemoveIncompleteDependency(completedId);

                if (_pending.ContainsPendingSerializationInfo(completedId))
                {
                    // We came back for an object that has no remaining direct dependencies, but still has
                    // PendingSerializationInfo. As such it cannot be considered completed yet.
                    continue;
                }
            }

            if (_recordMap[completedId] is ClassRecord classRecord && !_pending.HasIncompleteDependencies(completedId))
            {
                // There are no remaining dependencies. Get the real object if it's an IObjectReference.
                if (_deserializedObjects[completedId] is IObjectReference objectReference)
                {
                    _deserializedObjects[completedId] = objectReference.GetRealObject(Options.StreamingContext);
                }
            }

            foreach (int dependentCompletion in _pending.CompleteDependencies(completedId))
            {
                _pendingCompletions.Enqueue(dependentCompletion);
            }

            recursed = true;
        }
    }
}

#pragma warning restore SYSLIB0050 // Type or member is obsolete
