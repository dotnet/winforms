// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;
using System.Runtime.Serialization;

namespace System.ComponentModel.Design.Serialization;

public sealed partial class CodeDomComponentSerializationService
{
    [Serializable]
    internal sealed class CodeDomComponentSerializationState : ISerializable
    {
        private const string PropertiesKey = "Properties";
        private const string ResourcesKey = "Resources";
        private const string EventsKey = "Events";
        private const string ModifierKey = "Modifier";

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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ArgumentNullException.ThrowIfNull(info);

            info.AddValue(PropertiesKey, Properties);
            info.AddValue(ResourcesKey, Resources);
            info.AddValue(EventsKey, Events);
            info.AddValue(ModifierKey, Modifier);
        }

        private CodeDomComponentSerializationState(SerializationInfo info, StreamingContext streamingContext)
        {
            ArgumentNullException.ThrowIfNull(info);

            Properties = (List<string>?)info.GetValue(PropertiesKey, type: typeof(List<string>));
            Resources = (Dictionary<string, object?>?)info.GetValue(ResourcesKey, type: typeof(Dictionary<string, object?>));
            Events = (List<string>?)info.GetValue(EventsKey, type: typeof(List<string>));
            Modifier = info.GetValue(ModifierKey, type: typeof(object));
        }
    }
}
