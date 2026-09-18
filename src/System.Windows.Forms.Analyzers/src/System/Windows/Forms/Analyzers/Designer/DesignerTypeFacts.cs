// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;

namespace System.Windows.Forms.Analyzers;

/// <summary>
///  Provides language-neutral facts about WinForms Designer partial types.
/// </summary>
internal static class DesignerTypeFacts
{
    private const string ControlMetadataName = "System.Windows.Forms.Control";

    /// <summary>
    ///  Determines whether a syntax tree uses a conventional WinForms Designer file name.
    /// </summary>
    public static bool IsDesignerFile(SyntaxTree syntaxTree)
    {
        string path = syntaxTree.FilePath;

        return path.EndsWith(".Designer.cs", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".Designer.vb", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///  Determines whether a source type is a WinForms control split across main and Designer files.
    /// </summary>
    public static bool IsDesignerType(INamedTypeSymbol type)
        => DerivesFromControl(type)
            && HasDesignerDeclaration(type)
            && HasNonDesignerDeclaration(type)
            && HasDesignerInitializeComponent(type);

    /// <summary>
    ///  Determines whether a declaration belongs to the Designer part of a WinForms type.
    /// </summary>
    public static bool IsDesignerDeclaration(
        INamedTypeSymbol type,
        SyntaxTree syntaxTree)
        => IsDesignerFile(syntaxTree)
            && type.DeclaringSyntaxReferences.Any(
                declaration => declaration.SyntaxTree == syntaxTree)
            && IsDesignerType(type);

    private static bool DerivesFromControl(INamedTypeSymbol type)
    {
        for (INamedTypeSymbol? current = type; current is not null; current = current.BaseType)
        {
            if (current.ToDisplayString() == ControlMetadataName)
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasDesignerDeclaration(INamedTypeSymbol type)
        => type.DeclaringSyntaxReferences.Any(
            declaration => IsDesignerFile(declaration.SyntaxTree));

    private static bool HasNonDesignerDeclaration(INamedTypeSymbol type)
        => type.DeclaringSyntaxReferences.Any(
            declaration => !IsDesignerFile(declaration.SyntaxTree));

    private static bool HasDesignerInitializeComponent(INamedTypeSymbol type)
        => type.GetMembers("InitializeComponent")
            .OfType<IMethodSymbol>()
            .Any(
                method => !method.IsStatic
                    && method.MethodKind == MethodKind.Ordinary
                    && method.ReturnsVoid
                    && method.Parameters.IsEmpty
                    && method.DeclaringSyntaxReferences.Any(
                        declaration => IsDesignerFile(declaration.SyntaxTree)));
}
