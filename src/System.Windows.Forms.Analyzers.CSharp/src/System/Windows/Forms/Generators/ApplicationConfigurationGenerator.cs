// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Windows.Forms.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace System.Windows.Forms.Generators
{
    [Generator]
    internal class ApplicationConfigurationGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not ApplicationConfigurationSyntaxReceiver syntaxReceiver)
            {
                throw new InvalidOperationException("We were given the wrong syntax receiver.");
            }

            if (syntaxReceiver.Nodes.Count == 0)
            {
                return;
            }

            if (context.Compilation.Options.OutputKind != OutputKind.WindowsApplication &&
                // Starting in the 5.0.100 version of the .NET SDK, when OutputType is set to Exe, it is automatically changed to WinExe
                // for WPF and Windows Forms apps that target any framework version, including .NET Framework.
                // https://docs.microsoft.com/en-us/dotnet/core/compatibility/sdk/5.0/automatically-infer-winexe-output-type
                context.Compilation.Options.OutputKind != OutputKind.ConsoleApplication)
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.s_errorUnsupportedProjectType, Location.None, nameof(OutputKind.WindowsApplication)));
                return;
            }

            ApplicationConfig? projectConfig = ProjectFileReader.ReadApplicationConfig(context);
            if (projectConfig is null)
            {
                return;
            }

            string? code = ApplicationConfigurationInitializeBuilder.GenerateInitialize(projectNamespace: GetUserProjectNamespace(syntaxReceiver.Nodes[0]), projectConfig);
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

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new ApplicationConfigurationSyntaxReceiver());
        }
    }
}
