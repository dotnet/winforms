// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Xunit;

namespace System.Windows.Forms.Design.Editors.Tests
{
    internal sealed class SystemDesignMetadataReader
    {
        public IReadOnlyList<string> GetExportedTypeNames()
        {
            // Force load System.Design into the appdomain
            DesignSurface designSurface = new();
            IDesigner designer = designSurface.CreateDesigner(new Control(), true);
            Assert.NotNull(designer);

            Assembly systemDesign = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "System.Design");

            using var fs = new FileStream(systemDesign.Location, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var peReader = new PEReader(fs);

            MetadataReader metadataReader = peReader.GetMetadataReader();
            List<string> typeNames = new();

            foreach (ExportedTypeHandle typeHandle in metadataReader.ExportedTypes)
            {
                ExportedType type = metadataReader.GetExportedType(typeHandle);

                string ns = metadataReader.GetString(type.Namespace);
                string name = metadataReader.GetString(type.Name);

                typeNames.Add($"{ns}.{name}");
            }

            return typeNames;
        }
    }
}
