// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Formats.Nrbf;

namespace System.Windows.Forms.BinaryFormat.Deserializer;

internal sealed partial class Deserializer
{
    /// <summary>
    ///  Encapsulates updates that need applied to objects in the graph after initial deserialization.
    /// </summary>
    private class PendingUpdates
    {
        // Queue of SerializationInfo objects that need to be applied. These are in depth first order,
        // if there are no cycles in the graph this ensures that all objects are available when the
        // SerializationInfo is applied.
        //
        // We also keep a hashset for quickly checking to make sure we do not complete objects before we
        // actually apply the SerializationInfo. While we could mark them in the incomplete dependencies
        // dictionary, to do so we'd need to know if any referenced object is going to get to this state
        // even if it hasn't finished parsing, which isn't easy to do with cycles involved.
        private Queue<PendingSerializationInfo>? _pendingSerializationInfo;
        private HashSet<SerializationRecordId>? _pendingSerializationInfoIds;

        // For a given object id, the set of ids that it is waiting on to complete.
        private Dictionary<SerializationRecordId, HashSet<SerializationRecordId>>? _incompleteDependencies;

        // The pending value updaters. Scanned each time an object is completed.
        private HashSet<ValueUpdater>? _pendingValueUpdates;

        private readonly IDictionary<SerializationRecordId, object> _deserializedObjects;

        internal PendingUpdates(IDictionary<SerializationRecordId, object> deserializedObjects) => _deserializedObjects = deserializedObjects;

        /// <summary>
        ///  Number of pending updates, if any.
        /// </summary>
        internal int PendingValueUpdatesCount => _pendingValueUpdates?.Count ?? 0;

        internal void Add(ValueUpdater updater)
        {
            _pendingValueUpdates ??= [];
            _pendingValueUpdates.Add(updater);

            _incompleteDependencies ??= [];

            if (_incompleteDependencies.TryGetValue(updater.ObjectId, out HashSet<SerializationRecordId>? dependencies))
            {
                dependencies.Add(updater.ValueId);
            }
            else
            {
                _incompleteDependencies.Add(updater.ObjectId, [updater.ValueId]);
            }
        }

        internal bool HasIncompleteDependencies(SerializationRecordId id) => _incompleteDependencies?.ContainsKey(id) ?? false;

        internal void RemoveIncompleteDependency(SerializationRecordId id) => _incompleteDependencies?.Remove(id);

        internal void Enqueue(PendingSerializationInfo pending)
        {
            _pendingSerializationInfo ??= new();
            _pendingSerializationInfo.Enqueue(pending);

            _pendingSerializationInfoIds ??= [];
            _pendingSerializationInfoIds.Add(pending.ObjectId);
        }

        internal bool TryDequeuePendingSerializationInfo([NotNullWhen(true)] out PendingSerializationInfo? pending)
        {
            if (_pendingSerializationInfo is not null && _pendingSerializationInfo.TryDequeue(out pending))
            {
                _pendingSerializationInfoIds?.Remove(pending.ObjectId);
                return true;
            }

            pending = null;
            return false;
        }

        internal bool ContainsPendingSerializationInfo(SerializationRecordId id) => _pendingSerializationInfoIds?.Contains(id) ?? false;

        internal int PendingSerializationInfoCount => _pendingSerializationInfo?.Count ?? 0;

        internal bool TryGetIncompleteDependencies(SerializationRecordId id, [NotNullWhen(true)] out HashSet<SerializationRecordId>? dependencies)
        {
            if (_incompleteDependencies is not null)
            {
                return _incompleteDependencies.TryGetValue(id, out dependencies);
            }

            dependencies = null;
            return false;
        }

        internal IEnumerable<SerializationRecordId> CompleteDependencies(SerializationRecordId completedId)
        {
            if (_incompleteDependencies is null)
            {
                yield break;
            }

            foreach ((SerializationRecordId incompleteId, HashSet<SerializationRecordId> dependencies) in _incompleteDependencies)
            {
                if (!dependencies.Remove(completedId))
                {
                    continue;
                }

                // Search for fixups that need to be applied for this dependency.
                int removals = _pendingValueUpdates!.RemoveWhere((ValueUpdater updater) =>
                {
                    if (!updater.ValueId.Equals(completedId))
                    {
                        return false;
                    }

                    updater.UpdateValue(_deserializedObjects);
                    return true;
                });

                if (dependencies.Count != 0)
                {
                    continue;
                }

                // No more dependencies, enqueue for completion
                yield return incompleteId;
            }
        }
    }
}
