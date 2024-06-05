// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Analyzers;
using System.Windows.Forms.CSharp.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace System.Windows.Forms.CSharp.Generators.ApplicationConfiguration;

[Generator(LanguageNames.CSharp)]
internal class ApplicationConfigurationGenerator : IIncrementalGenerator
{
    private static void Execute(
        SourceProductionContext context,
        bool hasSupportedSyntaxNode,
        string? projectNamespace,
        OutputKind outputKind,
        ApplicationConfig? applicationConfig,
        Diagnostic? applicationConfigDiagnostics)
    {
        if (!hasSupportedSyntaxNode)
        {
            return;
        }

        if (applicationConfig is null)
        {
            if (applicationConfigDiagnostics is not null)
            {
                context.ReportDiagnostic(applicationConfigDiagnostics);
            }

            return;
        }

        if (outputKind is not OutputKind.WindowsApplication
            // Starting in the 5.0.100 version of the .NET SDK, when OutputType is set to Exe, it is automatically changed to WinExe
            // for WPF and Windows Forms apps that target any framework version, including .NET Framework.
            // https://docs.microsoft.com/en-us/dotnet/core/compatibility/sdk/5.0/automatically-infer-winexe-output-type
            and not OutputKind.ConsoleApplication)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                CSharpDiagnosticDescriptors.s_errorUnsupportedProjectType,
                Location.None, nameof(OutputKind.WindowsApplication)));

            return;
        }

        string? code = ApplicationConfigurationInitializeBuilder.GenerateInitialize(projectNamespace, applicationConfig);

        if (code is not null)
        {
            context.AddSource("ApplicationConfiguration.g.cs", code);
        }
    }

    private static string? GetUserProjectNamespace(SyntaxNode node)
    {
        string? ns = null;

        if (node
            .Ancestors()
            .FirstOrDefault(a => a is NamespaceDeclarationSyntax) is NamespaceDeclarationSyntax namespaceSyntax)
        {
            ns = namespaceSyntax.Name.ToString();
        }

        return ns;
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<OutputKind> outputKindProvider = context.CompilationProvider.Select((compilation, _)
            => compilation.Options.OutputKind);

        IncrementalValuesProvider<string?> syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (syntaxNode, _) => IsSupportedSyntaxNode(syntaxNode),
            transform: static (generatorSyntaxContext, _) => GetUserProjectNamespace(generatorSyntaxContext.Node));

        IncrementalValueProvider<(ApplicationConfig? ApplicationConfig, Diagnostic? Diagnostic)> globalConfig
            = ProjectFileReader.ReadApplicationConfig(context.AnalyzerConfigOptionsProvider);

        IncrementalValueProvider<(OutputKind OutputKind, Collections.Immutable.ImmutableArray<string?> ProjectNamespaces, ApplicationConfig? ApplicationConfig, Diagnostic? ApplicationConfigDiagnostics)> inputs
            = outputKindProvider
            .Combine(syntaxProvider.Collect())
            .Combine(globalConfig)
            .Select((data, cancellationToken)
                => (OutputKind: data.Left.Left,
                    ProjectNamespaces: data.Left.Right,
                    data.Right.ApplicationConfig,
                    ApplicationConfigDiagnostics: data.Right.Diagnostic));

        context.RegisterSourceOutput(
            inputs,
            (context, source)
                => Execute(
                    context: context,
                    hasSupportedSyntaxNode: source.ProjectNamespaces.Length > 0,
                    projectNamespace: source.ProjectNamespaces.Length > 0
                        ? source.ProjectNamespaces[0]
                        : null,
                    outputKind: source.OutputKind,
                    applicationConfig: source.ApplicationConfig,
                    applicationConfigDiagnostics: source.ApplicationConfigDiagnostics));
    }

    public static bool IsSupportedSyntaxNode(SyntaxNode syntaxNode)
    {
        return syntaxNode is InvocationExpressionSyntax
        {
            ArgumentList.Arguments.Count: 0,
            Expression: MemberAccessExpressionSyntax
            {
                Name.Identifier.ValueText: "Initialize",
                Expression:
                        MemberAccessExpressionSyntax  // For: SourceGenerated.ApplicationConfiguration.Initialize()
                        {
                            Name.Identifier.ValueText: "ApplicationConfiguration"
                        }

                        or

                        IdentifierNameSyntax           // For: ApplicationConfiguration.Initialize() with a using statement
                        {
                            Identifier.ValueText: "ApplicationConfiguration"
                        }
            }
        };
    }
}
