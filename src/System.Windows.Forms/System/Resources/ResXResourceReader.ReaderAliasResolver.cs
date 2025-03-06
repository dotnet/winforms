// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;

namespace System.Resources;

public partial class ResXResourceReader
{
    private sealed class ReaderAliasResolver : IAliasResolver
    {
        private readonly Dictionary<string, AssemblyName> _cachedAliases;

        internal ReaderAliasResolver()
        {
            _cachedAliases = [];
        }

        public AssemblyName? ResolveAlias(string alias)
        {
            _cachedAliases.TryGetValue(alias, out AssemblyName? result);
            return result;
        }

        public void PushAlias(string? alias, AssemblyName name)
        {
            if (!string.IsNullOrEmpty(alias))
            {
                _cachedAliases[alias] = name;
            }
        }
    }
}
