// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Primitives
{
    internal static partial class LocalAppContextSwitches
    {
        private class TargetFramework
        {
            public string? Name { get; private set; }
            public string? Version { get; private set; }

            public TargetFramework(string? targetFramework)
            {
                if (targetFramework is null)
                {
                    return;
                }

                // Expected string is in the format of ".NETCoreApp,Version=v7.0"
                var tokens = targetFramework.Split(',');

                if(tokens.Length != 2)
                {
                    throw new ArgumentException(nameof(targetFramework));
                }

                Name = tokens[0];

                tokens = tokens[1].Split('=');
                if (tokens.Length != 2)
                {
                    throw new ArgumentException(nameof(targetFramework));
                }

                Version = tokens[1];
            }
        }
    }
}
