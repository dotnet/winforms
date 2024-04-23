// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.BinaryFormat;

internal sealed partial class BinaryFormattedObject
{
    /// <summary>
    ///  Parsing state for <see cref="BinaryFormattedObject"/>.
    /// </summary>
    internal sealed class ParseState
    {
        private readonly BinaryFormattedObject _format;

        public ParseState(BinaryReader reader, BinaryFormattedObject format)
        {
            Reader = reader;
            _format = format;
        }

        public BinaryReader Reader { get; }
        public RecordMap RecordMap => _format._recordMap;

        [UnconditionalSuppressMessage(
            "Trimming",
            "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = """
                Incoming type names are coming off of the formatted stream. There is no way for user code to pass compile
                time context for preserialized data. If a type can't be found on deserialization it won't matter any more
                than any other case where the type can't be found (e.g. a missing assembly). The deserializer will fail
                with information on the missing type that can be used to attribute to keep said type.
                """)]
        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        public Type GetType(string typeName, Id libraryId) => _format.GetType(typeName, libraryId);
    }
}
