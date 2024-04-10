// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;

namespace System.ComponentModel.Design.Serialization;

public sealed partial class CodeDomComponentSerializationService : ComponentSerializationService
{
    /// <summary>
    ///  <see cref="CodeDomComponentSerializationService"/> saves state of objects to this class.
    /// </summary>
    internal sealed class CodeDomComponentSerializationState
    {
        public readonly object? Code; // code gen

        /// <summary>
        ///  Generated statements coming from the context.
        /// </summary>
        public readonly CodeStatementCollection? Context;

        /// <summary>
        ///  Default properties.
        /// </summary>
        public readonly List<string>? Properties;
        public readonly Dictionary<string, object?>? Resources;

        /// <summary>
        ///  Events that need to be reset.
        /// </summary>
        public readonly List<string>? Events;

        /// <summary>
        ///  Modifier of the object.
        /// </summary>
        public readonly object? Modifier;

        public CodeDomComponentSerializationState(
            object? code,
            CodeStatementCollection? ctxStatements,
            List<string>? properties,
            Dictionary<string, object?>? resources,
            List<string>? events,
            object? modifier)
        {
            Code = code;
            Context = ctxStatements;
            Properties = properties;
            Resources = resources;
            Events = events;
            Modifier = modifier;
        }
    }
}
