// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace System.Windows.Forms.Analyzers.Tests;

public static partial class CSharpCodeRefactoringVerifier<TCodeRefactoring>
    where TCodeRefactoring : CodeRefactoringProvider, new()
{
    public class Test : CSharpCodeRefactoringTest<TCodeRefactoring, XUnitVerifier>
    {
        public Test()
        {
            ReferenceAssemblies = ReferenceAssemblies.NetFramework.Net472.WindowsForms;

            SolutionTransforms.Add((solution, projectId) =>
            {
                CompilationOptions compilationOptions = solution.GetProject(projectId)!.CompilationOptions!;
                compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(
                    compilationOptions.SpecificDiagnosticOptions.SetItems(CSharpVerifierHelper.NullableWarnings));
                solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

                return solution;
            });
        }
    }
}
