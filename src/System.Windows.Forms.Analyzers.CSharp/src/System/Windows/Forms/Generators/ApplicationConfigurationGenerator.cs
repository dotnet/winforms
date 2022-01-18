// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Windows.Forms.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace System.Windows.Forms.Generators
{
    [Generator(LanguageNames.CSharp)]
    internal class ApplicationConfigurationGenerator : IIncrementalGenerator
    {
        private void Execute(
            SourceProductionContext context,
            ImmutableArray<SyntaxNode> syntaxNodes,
            OutputKind outputKind,
            ApplicationConfig? applicationConfig,
            Diagnostic? applicationConfigDiagnostics)
        {
            if (syntaxNodes.IsEmpty)
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

            if (outputKind != OutputKind.WindowsApplication &&
                // Starting in the 5.0.100 version of the .NET SDK, when OutputType is set to Exe, it is automatically changed to WinExe
                // for WPF and Windows Forms apps that target any framework version, including .NET Framework.
                // https://docs.microsoft.com/en-us/dotnet/core/compatibility/sdk/5.0/automatically-infer-winexe-output-type
                outputKind != OutputKind.ConsoleApplication)
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.s_errorUnsupportedProjectType, Location.None, nameof(OutputKind.WindowsApplication)));
                return;
            }

            string? code = ApplicationConfigurationInitializeBuilder.GenerateInitialize(projectNamespace: GetUserProjectNamespace(syntaxNodes[0]), applicationConfig);
            if (code is not null)
            {
                context.AddSource("ApplicationConfiguration.g.cs", code);
            }
        }

        private string? GetUserProjectNamespace(SyntaxNode node)
        {
            string? ns = null;

            if (node.Ancestors().FirstOrDefault(a => a is NamespaceDeclarationSyntax) is NamespaceDeclarationSyntax namespaceSyntax)
            {
                ns = namespaceSyntax.Name.ToString();
            }

            return ns;
        }

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: (syntaxNode, _) => IsSupportedSyntaxNode(syntaxNode),
                transform: (generatorSyntaxContext, _) => generatorSyntaxContext.Node);

            var globalConfig = ProjectFileReader.ReadApplicationConfig(context.AnalyzerConfigOptionsProvider);

            var inputs = context.CompilationProvider
                .Combine(syntaxProvider.Collect())
                .Combine(globalConfig)
                .Select((data, cancellationToken)
                    => (Compilation: data.Left.Left,
                        Nodes: data.Left.Right,
                        ApplicationConfig: data.Right.ApplicationConfig,
                        ApplicationConfigDiagnostics: data.Right.Diagnostic));

            context.RegisterSourceOutput(
                inputs,
                (context, source) => Execute(context, source.Nodes, source.Compilation.Options.OutputKind, source.ApplicationConfig, source.ApplicationConfigDiagnostics));
        }

        public static bool IsSupportedSyntaxNode(SyntaxNode syntaxNode)
        {
#pragma warning disable SA1513 // Closing brace should be followed by blank line
            if (syntaxNode is InvocationExpressionSyntax
                {
                    ArgumentList:
                    {
                        Arguments: { Count: 0 }
                    },
                    Expression: MemberAccessExpressionSyntax
                    {
                        Name:
                        {
                            Identifier:
                            {
                                ValueText: "Initialize"
                            }
                        },
                        Expression:
                            MemberAccessExpressionSyntax  // For: SourceGenerated.ApplicationConfiguration.Initialize()
                            {
                                Name:
                                {
                                    Identifier:
                                    {
                                        ValueText: "ApplicationConfiguration"
                                    }
                                }
                            }
                            or
                            IdentifierNameSyntax           // For: ApplicationConfiguration.Initialize() with a using statement
                            {
                                Identifier:
                                {
                                    ValueText: "ApplicationConfiguration"
                                }
                            }
                    }
                })
            {
                return true;
            }
#pragma warning restore SA1513 // Closing brace should be followed by blank line

            return false;
        }
    }
}
