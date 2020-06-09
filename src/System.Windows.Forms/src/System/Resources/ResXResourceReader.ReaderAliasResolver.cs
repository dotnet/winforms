// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Reflection;

namespace System.Resources
{
    public partial class ResXResourceReader
    {
        private sealed class ReaderAliasResolver : IAliasResolver
        {
            private readonly Hashtable cachedAliases;

            internal ReaderAliasResolver()
            {
                cachedAliases = new Hashtable();
            }

            public AssemblyName ResolveAlias(string alias)
            {
                AssemblyName result = null;
                if (cachedAliases != null)
                {
                    result = (AssemblyName)cachedAliases[alias];
                }
                return result;
            }

            public void PushAlias(string alias, AssemblyName name)
            {
                if (cachedAliases != null && !string.IsNullOrEmpty(alias))
                {
                    cachedAliases[alias] = name;
                }
            }
        }
    }
}
