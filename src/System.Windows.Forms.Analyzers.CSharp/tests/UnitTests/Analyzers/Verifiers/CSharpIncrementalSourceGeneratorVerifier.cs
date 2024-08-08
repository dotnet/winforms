// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;

namespace System.Windows.Forms.Analyzers.Tests.Verifiers;

// Note: this is bleeding edge, eventually this can be removed once it is all baked in to Roslyn SDK.
public static partial class CSharpIncrementalSourceGeneratorVerifier<TIncrementalGenerator>
    where TIncrementalGenerator : IIncrementalGenerator, new()
{
    public class Test : CSharpSourceGeneratorTest<EmptySourceGeneratorProvider>
    {
        public Test()
        {
            SolutionTransforms.Add((solution, projectId) =>
            {
                var compilationOptions = solution.GetProject(projectId)!.CompilationOptions;

                compilationOptions = compilationOptions!.WithSpecificDiagnosticOptions(
                    compilationOptions.SpecificDiagnosticOptions.SetItems(CSharpVerifierHelper.NullableWarnings));

                solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

                return solution;
            });
        }

        protected override IEnumerable<Type> GetSourceGenerators()
        {
            yield return typeof(TIncrementalGenerator);
        }

        protected override ParseOptions CreateParseOptions()
        {
            var parseOptions = (CSharpParseOptions)base.CreateParseOptions();
            return parseOptions.WithLanguageVersion(LanguageVersion.Preview);
        }
    }
}
