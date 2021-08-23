// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;
using Xunit;

namespace System.Windows.Forms.Design.Editors.Tests
{
    public class EnsureDesignerTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void Ensure_designer_type_forwarded()
        {
            SystemDesignMetadataReader metadataReader = new();
            IReadOnlyList<string> forwardedTypes = metadataReader.GetExportedTypeNames();

            IEnumerable<Type> designers = typeof(ComponentDesigner).Assembly
                                .GetTypes()
                                .Where(t => t.IsSubclassOf(typeof(ComponentDesigner))
                                        && !t.IsPublic);
            foreach (Type designer in designers)
            {
                Assert.True(forwardedTypes.Contains(designer.FullName), $"{designer.FullName} must be type forwarded");
            }
        }
    }
}
