// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;
using Xunit;

namespace System.Windows.Forms.Analyzers.Tests;

/// <summary>
///  Tests language-neutral WinForms Designer type detection.
/// </summary>
public class DesignerTypeFactsTests
{
    private const string CSharpMain =
        """
        namespace System.Windows.Forms
        {
            public class Control { }
        }

        namespace Test
        {
            partial class TestControl : System.Windows.Forms.Control
            {
            }
        }
        """;

    private const string CSharpDesigner =
        """
        namespace Test;

        partial class TestControl
        {
            private void InitializeComponent()
            {
            }
        }
        """;

    private const string VisualBasicMain =
        """
        Namespace System.Windows.Forms
            Public Class Control
            End Class
        End Namespace

        Namespace Test
            Partial Class TestControl
                Inherits System.Windows.Forms.Control
            End Class
        End Namespace
        """;

    private const string VisualBasicDesigner =
        """
        Namespace Test
            Partial Class TestControl
                Private Sub InitializeComponent()
                End Sub
            End Class
        End Namespace
        """;

    [Fact]
    public void IsDesignerType_CSharpDesignerPair_ReturnsTrue()
    {
        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "DesignerFacts",
            syntaxTrees:
            [
                CSharpSyntaxTree.ParseText(
                    CSharpMain,
                    path: "TestControl.cs",
                    cancellationToken: TestContext.Current.CancellationToken),
                CSharpSyntaxTree.ParseText(
                    CSharpDesigner,
                    path: "TestControl.Designer.cs",
                    cancellationToken: TestContext.Current.CancellationToken)
            ],
            references: [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)]);

        INamedTypeSymbol type = compilation.GetTypeByMetadataName("Test.TestControl")!;

        DesignerTypeFacts facts = new(compilation);
        Assert.True(facts.IsDesignerType(type));
        Assert.True(facts.IsDesignerDeclaration(type, compilation.SyntaxTrees.Last()));
    }

    [Fact]
    public void IsDesignerType_VisualBasicDesignerPair_ReturnsTrue()
    {
        VisualBasicCompilation compilation = VisualBasicCompilation.Create(
            assemblyName: "DesignerFacts",
            syntaxTrees:
            [
                VisualBasicSyntaxTree.ParseText(
                    VisualBasicMain,
                    path: "TestControl.vb",
                    cancellationToken: TestContext.Current.CancellationToken),
                VisualBasicSyntaxTree.ParseText(
                    VisualBasicDesigner,
                    path: "TestControl.Designer.vb",
                    cancellationToken: TestContext.Current.CancellationToken)
            ],
            references: [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)]);

        INamedTypeSymbol type = compilation.GetTypeByMetadataName("Test.TestControl")!;

        DesignerTypeFacts facts = new(compilation);
        Assert.True(facts.IsDesignerType(type));
        Assert.True(facts.IsDesignerDeclaration(type, compilation.SyntaxTrees.Last()));
    }

    [Theory]
    [InlineData("TestControl.g.cs")]
    [InlineData("TestControl.Designer.generated.cs")]
    [InlineData("TestControl.cs")]
    public void IsDesignerFile_NonDesignerPath_ReturnsFalse(string path)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(
            CSharpDesigner,
            path: path,
            cancellationToken: TestContext.Current.CancellationToken);

        Assert.False(DesignerTypeFacts.IsDesignerFile(syntaxTree));
    }

    [Fact]
    public void IsDesignerType_UnrelatedType_ReturnsFalse()
    {
        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "DesignerFacts",
            syntaxTrees:
            [
                CSharpSyntaxTree.ParseText(
                    "partial class TestControl { }",
                    path: "TestControl.cs",
                    cancellationToken: TestContext.Current.CancellationToken),
                CSharpSyntaxTree.ParseText(
                    "partial class TestControl { private void InitializeComponent() { } }",
                    path: "TestControl.Designer.cs",
                    cancellationToken: TestContext.Current.CancellationToken)
            ],
            references: [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)]);

        INamedTypeSymbol type = compilation.GetTypeByMetadataName("TestControl")!;

        Assert.False(new DesignerTypeFacts(compilation).IsDesignerType(type));
    }
}
