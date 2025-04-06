// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using Xunit.Abstractions;

namespace System.Windows.Forms.Design.Tests;

// NB: doesn't require thread affinity
public class DesignerAttributeTests
{
    private readonly ITestOutputHelper _output;

    private static ImmutableHashSet<string> SkipList { get; } =
    [
        $"System.Windows.Forms.Design.AxDesigner, {Assemblies.SystemDesign}",
        $"System.Windows.Forms.Design.AxHostDesigner, {Assemblies.SystemDesign}",
        $"System.Windows.Forms.Design.ControlBindingsConverter, {Assemblies.SystemDesign}",
        $"System.Windows.Forms.Design.DataGridColumnCollectionEditor, {Assemblies.SystemDesign}",
        $"System.Windows.Forms.Design.DataGridColumnStyleFormatEditor, {Assemblies.SystemDesign}",
        $"System.Windows.Forms.Design.DataGridColumnStyleMappingNameEditor, {Assemblies.SystemDesign}",
        $"System.Windows.Forms.Design.DataGridTableStyleMappingNameEditor, {Assemblies.SystemDesign}",
        $"System.Windows.Forms.Design.DataGridDesigner, {Assemblies.SystemDesign}",
        $"System.Windows.Forms.Design.DataGridViewComponentEditor, {Assemblies.SystemDesign}",
        $"System.Windows.Forms.Design.StatusBarDesigner, {Assemblies.SystemDesign}",
        $"System.Windows.Forms.Design.ToolBarButtonDesigner, {Assemblies.SystemDesign}",
        $"System.Windows.Forms.Design.ToolBarDesigner, {Assemblies.SystemDesign}",
        $"System.Windows.Forms.Design.ToolStripImageIndexEditor, {Assemblies.SystemDesign}",
        $"System.Windows.Forms.Design.WebBrowserDesigner, {Assemblies.SystemDesign}",
    ];

    public DesignerAttributeTests(ITestOutputHelper output)
    {
        _output = output;
    }

    public static IEnumerable<object[]> GetAttributeOfType_TestData(string assembly, Type attributeType)
    {
        foreach (var type in Assembly.Load(assembly).GetTypes())
            foreach (object attribute in type.GetCustomAttributes(attributeType, false))
                yield return new[] { type, attribute };
    }

    public static IEnumerable<object[]> GetAttributeOfTypeAndProperty_TestData(string assembly, Type attributeType)
    {
        foreach (var type in Assembly.Load(assembly).GetTypes())
        {
            foreach (object attribute in type.GetCustomAttributes(attributeType, false))
                yield return new[] { type.FullName, attribute };

            foreach (var property in type.GetProperties())
                foreach (object attribute in property.GetCustomAttributes(attributeType, false))
                    yield return new[] { $"{type.FullName}, property {property.Name}", attribute };
        }
    }

    public static IEnumerable<object[]> GetAttributeWithType_TestData(string assembly, Type attributeType)
    {
        foreach (var type in Assembly.Load(assembly).GetTypes())
            foreach (object attribute in type.GetCustomAttributes(attributeType, false))
                yield return new[] { type, attribute };
    }

    public static IEnumerable<object[]> GetAttributeWithProperty_TestData(string assembly, Type attributeType)
    {
        foreach (var type in Assembly.Load(assembly).GetTypes())
            foreach (var property in type.GetProperties())
                foreach (object attribute in property.GetCustomAttributes(attributeType, false))
                    yield return new[] { property, attribute };
    }

    [Theory]
    [MemberData(nameof(GetAttributeOfType_TestData), Assemblies.SystemWindowsForms, typeof(DesignerAttribute))]
    public void DesignerAttribute_TypeExists(Type annotatedType, DesignerAttribute attribute)
    {
        var type = Type.GetType(attribute.DesignerTypeName, throwOnError: false);
        _output.WriteLine($"{annotatedType.FullName}: {attribute.DesignerTypeName} --> {type?.FullName}");

        if (SkipList.Contains(attribute.DesignerTypeName))
        {
            Assert.Null(type);
            return;
        }

        Assert.NotNull(type);
    }

    [Theory]
    [MemberData(nameof(GetAttributeOfType_TestData), Assemblies.SystemDrawing, typeof(DesignerSerializerAttribute))]
    [MemberData(nameof(GetAttributeOfType_TestData), Assemblies.SystemWindowsForms, typeof(DesignerSerializerAttribute))]
    [MemberData(nameof(GetAttributeOfType_TestData), Assemblies.SystemDesign, typeof(DesignerSerializerAttribute))]
    public void DesignerSerializerAttribute_TypeExists(Type annotatedType, DesignerSerializerAttribute attribute)
    {
        var type = Type.GetType(attribute.SerializerTypeName, false);
        _output.WriteLine($"{annotatedType.FullName}: {attribute.SerializerTypeName} --> {type?.FullName}");

        if (SkipList.Contains(attribute.SerializerTypeName))
        {
            Assert.Null(type);
            return;
        }

        Assert.NotNull(type);
    }

    [Theory]
    [MemberData(nameof(GetAttributeWithType_TestData), Assemblies.SystemWindowsForms, typeof(DefaultPropertyAttribute))]
    public void DefaultPropertyAttribute_PropertyExists(Type type, DefaultPropertyAttribute attribute)
    {
        string property = attribute.Name;
        var propertyInfo = type.GetProperty(property);
        _output.WriteLine($"{type.FullName}: {property} --> {propertyInfo?.Name}");

#pragma warning disable WFDEV006 // Type or member is obsolete
        if ((type == typeof(DataGridColumnStyle) && property == "Header") || (type == typeof(DataGridTextBox) && property == "GridEditName"))
#pragma warning restore WFDEV006
        {
            // Attributes are defined on non-instanciatiable base classes, but the corresponding properties are available on the derived classes.
            Assert.Null(propertyInfo);
        }
        else
        {
            Assert.NotNull(propertyInfo);
        }
    }

    [Theory]
    [MemberData(nameof(GetAttributeWithType_TestData), Assemblies.SystemWindowsForms, typeof(DefaultBindingPropertyAttribute))]
    public void DefaultBindingPropertyAttribute_PropertyExists(Type type, DefaultBindingPropertyAttribute attribute)
    {
        var propertyInfo = type.GetProperty(attribute.Name);
        _output.WriteLine($"{type.FullName}: {attribute.Name} --> {propertyInfo?.Name}");

        Assert.NotNull(propertyInfo);
    }

    [Theory]
    [MemberData(nameof(GetAttributeWithType_TestData), Assemblies.SystemWindowsForms, typeof(DefaultEventAttribute))]
    public void DefaultEventAttribute_EventExists(Type type, DefaultEventAttribute attribute)
    {
        var eventInfo = type.GetEvent(attribute.Name);
        _output.WriteLine($"{type.FullName}: {attribute.Name} --> {eventInfo?.Name}");

        Assert.NotNull(eventInfo);
    }

    [Theory]
    [MemberData(nameof(GetAttributeOfTypeAndProperty_TestData), Assemblies.SystemDrawing, typeof(TypeConverterAttribute))]
    [MemberData(nameof(GetAttributeOfTypeAndProperty_TestData), Assemblies.SystemWindowsForms, typeof(TypeConverterAttribute))]
    public void TypeConverterAttribute_TypeExists(string subject, TypeConverterAttribute attribute)
    {
        var type = Type.GetType(attribute.ConverterTypeName, false);
        _output.WriteLine($"{subject}: {attribute.ConverterTypeName} --> {type?.Name}");

        if (SkipList.Contains(attribute.ConverterTypeName))
        {
            Assert.Null(type);
            return;
        }

        Assert.NotNull(type);
    }

    [Theory]
    [MemberData(nameof(GetAttributeOfTypeAndProperty_TestData), Assemblies.SystemDrawing, typeof(EditorAttribute))]
    [MemberData(nameof(GetAttributeOfTypeAndProperty_TestData), Assemblies.SystemWindowsForms, typeof(EditorAttribute))]
    public void EditorAttribute_TypeExists(string subject, EditorAttribute attribute)
    {
        var type = Type.GetType(attribute.EditorTypeName, throwOnError: false);
        _output.WriteLine($"{subject}: {attribute.EditorTypeName} --> {type?.Name}");

        if (SkipList.Contains(attribute.EditorTypeName))
        {
            Assert.Null(type);
            return;
        }

        Assert.NotNull(type);
    }
}
