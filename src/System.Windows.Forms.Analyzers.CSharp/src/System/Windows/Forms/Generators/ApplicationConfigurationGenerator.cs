// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Linq;
using System.Windows.Forms.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace System.Windows.Forms.Generators
{
    [Generator(LanguageNames.CSharp)]
    internal class ApplicationConfigurationGenerator : IIncrementalGenerator
    {
        private void Execute(Compilation compilation, ImmutableArray<SyntaxNode> syntaxNodes, SourceProductionContext context)
        {
            if (syntaxNodes.IsEmpty)
            {
                return;
            }

            if (compilation.Options.OutputKind != OutputKind.WindowsApplication &&
                // Starting in the 5.0.100 version of the .NET SDK, when OutputType is set to Exe, it is automatically changed to WinExe
                // for WPF and Windows Forms apps that target any framework version, including .NET Framework.
                // https://docs.microsoft.com/en-us/dotnet/core/compatibility/sdk/5.0/automatically-infer-winexe-output-type
                compilation.Options.OutputKind != OutputKind.ConsoleApplication)
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.s_errorUnsupportedProjectType, Location.None, nameof(OutputKind.WindowsApplication)));
                return;
            }

            ApplicationConfig? projectConfig = ProjectFileReader.ReadApplicationConfig(compilation);
            if (projectConfig is null)
            {
                return;
            }

            string? code = ApplicationConfigurationInitializeBuilder.GenerateInitialize(projectNamespace: GetUserProjectNamespace(syntaxNodes[0]), projectConfig);
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
                transform: (generatorSyntaxContext, _) => generatorSyntaxContext.Node
                );

            IncrementalValueProvider<(Compilation Compilation, ImmutableArray<SyntaxNode> Nodes)> compilationProvider = context.CompilationProvider.Combine(syntaxProvider.Collect());

            context.RegisterSourceOutput(compilationProvider, (spc, source) => Execute(source.Compilation, source.Nodes, spc));
        }

        public static bool IsSupportedSyntaxNode(SyntaxNode syntaxNode)
        {
#pragma warning disable SA1513 // Closing brace should be followed by blank line
            if (syntaxNode is InvocationExpressionSyntax
                {
                    ArgumentList:
                    {
                        Arguments: { Count: < 1 }
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
