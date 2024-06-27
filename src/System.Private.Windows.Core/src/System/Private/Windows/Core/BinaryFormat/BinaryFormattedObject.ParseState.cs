// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Core.BinaryFormat;

internal sealed partial class BinaryFormattedObject
{
    /// <summary>
    ///  Parsing state for <see cref="BinaryFormattedObject"/>.
    /// </summary>
    internal sealed class ParseState : IParseState
    {
        private readonly BinaryFormattedObject _format;

        public ParseState(BinaryReader reader, BinaryFormattedObject format)
        {
            Reader = reader;
            _format = format;
        }

        public BinaryReader Reader { get; }
        public RecordMap RecordMap => _format._recordMap;
        public Options Options => _format._options;
        public ITypeResolver TypeResolver => _format.TypeResolver;
    }
}
