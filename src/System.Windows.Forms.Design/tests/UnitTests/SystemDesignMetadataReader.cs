// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace System.Windows.Forms.Design.Editors.Tests;

internal sealed class SystemDesignMetadataReader
{
    public IReadOnlyList<string> GetExportedTypeNames()
    {
        // Force load System.Design into the appdomain
        DesignSurface designSurface = new();
        IDesigner designer = designSurface.CreateDesigner(new Control(), true);
        Assert.NotNull(designer);

        Assembly systemDesign = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "System.Design");

        using FileStream fs = new(systemDesign.Location, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using PEReader peReader = new(fs);

        MetadataReader metadataReader = peReader.GetMetadataReader();
        List<string> typeNames = [];

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
