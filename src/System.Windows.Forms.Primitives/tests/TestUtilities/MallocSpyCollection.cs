// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System
{
    /// <summary>
    ///  Collection definition to ensure tests that are watching allocations run sequentially.
    /// </summary>
    [CollectionDefinition(nameof(MallocSpy), DisableParallelization = true)]
    public class MallocSpyCollection
    {
    }
}
