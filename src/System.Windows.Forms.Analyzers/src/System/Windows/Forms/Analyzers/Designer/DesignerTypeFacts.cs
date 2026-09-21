// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace System.Windows.Forms.Analyzers;

/// <summary>
///  Provides compilation-scoped facts about WinForms Designer partial types and their owned members.
/// </summary>
/// <remarks>
///  <para>
///   Instances and cached symbols belong to one compilation and must not be shared between compilations.
///  </para>
/// </remarks>
internal sealed class DesignerTypeFacts
{
    private readonly INamedTypeSymbol? _control;
    private readonly INamedTypeSymbol? _component;
    private readonly INamedTypeSymbol? _container;
    private readonly ConcurrentDictionary<INamedTypeSymbol, bool> _designerTypes = new(SymbolEqualityComparer.Default);

    public DesignerTypeFacts(Compilation compilation)
    {
        _control = compilation.GetTypeByMetadataName("System.Windows.Forms.Control");
        _component = compilation.GetTypeByMetadataName("System.ComponentModel.Component");
        _container = compilation.GetTypeByMetadataName("System.ComponentModel.IContainer");
    }

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
    ///  Determines whether a source type is a component split across user and Designer files.
    /// </summary>
    public bool IsDesignerType(INamedTypeSymbol type)
        => _designerTypes.GetOrAdd(type, IsDesignerTypeCore);

    private bool IsDesignerTypeCore(INamedTypeSymbol type)
        => IsComponent(type)
            && HasDesignerDeclaration(type)
            && HasNonDesignerDeclaration(type)
            && HasDesignerInitializeComponent(type);

    /// <summary>
    ///  Determines whether a declaration belongs to the Designer part of a WinForms type.
    /// </summary>
    public bool IsDesignerDeclaration(
        INamedTypeSymbol type,
        SyntaxTree syntaxTree)
        => IsDesignerFile(syntaxTree)
            && type.DeclaringSyntaxReferences.Any(
                declaration => declaration.SyntaxTree == syntaxTree)
            && IsDesignerType(type);

    public bool IsComponent(ITypeSymbol? type)
    {
        for (ITypeSymbol? current = type; current is not null; current = current.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(current, _control)
                || SymbolEqualityComparer.Default.Equals(current, _component))
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsInitializeComponent(IMethodSymbol method)
        => NameEquals(method, "InitializeComponent")
            && !method.IsStatic
            && !method.IsGenericMethod
            && method.MethodKind == MethodKind.Ordinary
            && method.ReturnsVoid
            && method.Parameters.IsEmpty;

    public static bool IsAllowedMethod(IMethodSymbol method)
        => IsInitializeComponent(method)
            || !method.ExplicitInterfaceImplementations.IsEmpty
            || (NameEquals(method, "Dispose")
                && method.IsOverride
                && !method.IsStatic
                && method.ReturnsVoid
                && method.Parameters.Length == 1
                && method.Parameters[0].RefKind == RefKind.None
                && method.Parameters[0].Type.SpecialType == SpecialType.System_Boolean);

    public bool IsComponentsMember(ISymbol member)
        => NameEquals(member, "components")
            && member is IFieldSymbol field
            && SymbolEqualityComparer.Default.Equals(field.Type, _container);

    public ISymbol? GetInitializedComponent(ISimpleAssignmentOperation assignment, INamedTypeSymbol owner)
    {
        IOperation value = assignment.Value;
        while (value is IConversionOperation { OperatorMethod: null } or IParenthesizedOperation)
        {
            value = value is IConversionOperation conversion
                ? conversion.Operand
                : ((IParenthesizedOperation)value).Operand;
        }

        if (value is not IObjectCreationOperation || !IsComponent(value.Type) || IsInNestedFunction(assignment))
        {
            return null;
        }

        ISymbol? member = GetInstanceMember(assignment.Target);
        ITypeSymbol? memberType = member switch
        {
            IFieldSymbol field => field.Type,
            IPropertySymbol property => property.Type,
            _ => null
        };

        return member is not null
            && IsComponent(memberType)
            && SymbolEqualityComparer.Default.Equals(member.ContainingType, owner)
            && !member.IsImplicitlyDeclared
            ? member
            : null;
    }

    public static bool IsInNestedFunction(IOperation operation)
    {
        for (IOperation? parent = operation.Parent; parent is not null; parent = parent.Parent)
        {
            if (parent is IAnonymousFunctionOperation or ILocalFunctionOperation)
            {
                return true;
            }
        }

        return false;
    }

    public static ISymbol? GetInstanceMember(IOperation operation)
    {
        (ISymbol? Member, IOperation? Instance) reference = operation switch
        {
            IFieldReferenceOperation { Field.IsStatic: false } field => (field.Field, field.Instance),
            IPropertyReferenceOperation { Property.IsWithEvents: true } property => (property.Property, property.Instance),
            _ => (null, null)
        };

        return reference.Instance is IInstanceReferenceOperation { ReferenceKind: InstanceReferenceKind.ContainingTypeInstance }
            ? reference.Member
            : null;
    }

    private static bool NameEquals(ISymbol symbol, string name)
        => string.Equals(symbol.Name, name, symbol.Language == LanguageNames.VisualBasic
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal);

    private static bool HasDesignerDeclaration(INamedTypeSymbol type)
        => type.DeclaringSyntaxReferences.Any(
            declaration => IsDesignerFile(declaration.SyntaxTree));

    private static bool HasNonDesignerDeclaration(INamedTypeSymbol type)
        => type.DeclaringSyntaxReferences.Any(
            declaration => !IsDesignerFile(declaration.SyntaxTree));

    private static bool HasDesignerInitializeComponent(INamedTypeSymbol type)
        => type.GetMembers()
            .OfType<IMethodSymbol>()
            .Any(
                method => IsInitializeComponent(method)
                    && method.DeclaringSyntaxReferences.Any(
                        declaration => IsDesignerFile(declaration.SyntaxTree)));
}
