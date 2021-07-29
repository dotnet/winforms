// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using static Microsoft.CodeAnalysis.Testing.ReferenceAssemblies;

namespace System.Windows.Forms.Analyzers.Tests
{
    public class CSharpSourceGeneratorTest<TSourceGenerator> : CSharpSourceGeneratorTest<TSourceGenerator, XUnitVerifier>
        where TSourceGenerator : ISourceGenerator, new()
    {
        public CSharpSourceGeneratorTest()
        {
            ReferenceAssemblies = NetFramework.Net472.WindowsForms;
        }

        /// <summary>
        /// Allows you to specify additional global options that will appear in the context.AnalyzerConfigOptions.GlobalOptions object.
        /// </summary>
        public List<(string, string)> GlobalOptions { get; } = new();

        protected override GeneratorDriver CreateGeneratorDriver(Project project, ImmutableArray<ISourceGenerator> sourceGenerators)
            => CSharpGeneratorDriver.Create(
                sourceGenerators,
                project.AnalyzerOptions.AdditionalFiles,
                (CSharpParseOptions)project.ParseOptions!,
                new OptionsProvider(project.AnalyzerOptions.AnalyzerConfigOptionsProvider, GlobalOptions));

        /// <summary>
        /// This class just passes argument through to the projects options provider and it used to provider custom global options
        /// </summary>
        private class OptionsProvider : AnalyzerConfigOptionsProvider
        {
            private readonly AnalyzerConfigOptionsProvider _analyzerConfigOptionsProvider;

            public OptionsProvider(AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider, List<(string, string)> globalOptions)
            {
                _analyzerConfigOptionsProvider = analyzerConfigOptionsProvider;
                GlobalOptions = new ConfigOptions(_analyzerConfigOptionsProvider.GlobalOptions, globalOptions);
            }

            public override AnalyzerConfigOptions GlobalOptions { get; }

            public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
                => _analyzerConfigOptionsProvider.GetOptions(tree);

            public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
                => _analyzerConfigOptionsProvider.GetOptions(textFile);
        }

        /// <summary>
        /// Allows adding additional global options
        /// </summary>
        private class ConfigOptions : AnalyzerConfigOptions
        {
            private readonly AnalyzerConfigOptions _workspaceOptions;
            private readonly Dictionary<string, string> _globalOptions;

            public ConfigOptions(AnalyzerConfigOptions workspaceOptions, List<(string, string)> globalOptions)
            {
                _workspaceOptions = workspaceOptions;
                _globalOptions = globalOptions.ToDictionary(t => t.Item1, t => t.Item2);
            }

            public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
                => _workspaceOptions.TryGetValue(key, out value) || _globalOptions.TryGetValue(key, out value);
        }
    }
}
