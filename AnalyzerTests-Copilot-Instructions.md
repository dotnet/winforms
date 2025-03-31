# Copilot-Instructions to write test cases for WinForms Analyzers and CodeFixes

For Analyzer tests, we have currently 3 different projects in the solution.
* System.Windows.Forms.Analyzers.CSharp.Tests
* System.Windows.Forms.Analyzers.VisualBasic.Tests
* System.Windows.Forms.Analyzers.Tests

Important:
We are using AI currently only for adding tests to CSharp and VisualBasic tests projects.

The approach is principally the same for both CSharp and VisualBasic.
But here are the important differences:

* The CSharp tests are written in CSharp and are targeting the Analyzers which are also written in CSharp.
* When CSharp tests are using the test existing infrastructure, they are based on
  - The namespaces around `Microsoft.CodeAnalysis...`
  - The namespace `Microsoft.CodeAnalysis.CSharp.Testing`
  - The test base class `RoslynAnalyzerAndCodeFixTestBase<TAnalyzer, DefaultVerifier>`
  
* The VisualBasic tests are written in VisualBasic and are targeting the Analyzers which are also written in VisualBasic.
* When VisualBasic tests are using the test existing infrastructure, they are based on
  - The namespaces around `Microsoft.CodeAnalysis...`
  - The namespace `Microsoft.CodeAnalysis.VisualBasic.Testing`
  - The test base class `RoslynAnalyzerAndCodeFixTestBase(Of TAnalyzer, DefaultVerifier)`
  - But in addition the extension class `VisualBasicAnalyzerAndCodeFixExtensions`, so that the requirements for Visual Basic are met.  

* We always ask for CSharp and Visual Basic tests separately. If not specified and being ask
  to write Analyzer tests, please refuse and ask to rephrase the request and specify the language.

## General approach

Inside of the respective test projects, a new folder for a new test series for an Analyzer should be
created in way, which is describing the Analyzer sufficiently, be under the folder `Analyzer`. Under that 
folder, please create an additional folder `TestData` in addition. For example in _System.Windows.Forms.Analyzers.CSharp.Tests_
you would create the folder path _Analyzer\EnsureModelDialogDisposed\TestData_ where the Analyzer is EnsureModalDialogDisposed for
this example.

* Create a new TestClass, and give it a name which is describing the test class sufficiently. This
  class derives from the base class `RoslynAnalyzerAndCodeFixTestBase<TAnalyzer, DefaultVerifier>` 
  for CSharp. Use the respective extension class for VB in addition, for getting the correct VB version.
* Since we're using for the actual tests
  - `Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>`
  - `Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<TAnalyzer, TCodeFixProvider, DefaultVerifier>`

  or the VB equivalents, 

  - `Microsoft.CodeAnalysis.VisualBasic.Testing.VisualBasicAnalyzerTest<TAnalyzer, DefaultVerifier>`
  - `Microsoft.CodeAnalysis.VisualBasic.Testing.VisualBasicCodeFixTest<TAnalyzer, TCodeFixProvider, DefaultVerifier>`
  you need to create a per test class a folder with a name which is the same as the name of the test class. And inside of that folder, 
  you create test data files which resemble CSharp or Visual Basic files, but are not part of the solution 
  (BuildAction: _None_, Copy to output directory: _Do not copy_).
  Those files are:
  - AnalyzerTestCode.cs:
    A CSharp/VB file which contains the code which is used testing the Analyzer _on_. It should
    contain code, which is triggering the Analyzer, or also makes sure, that in edge cases, the Analyzer is NOT triggered.
  - An analyzer test, which does not rely on special CodeFix markers, can test the correctness of the Analyzer
    by this:
  
```CSharp
    [Theory]
    [CodeTestData(nameof(GetReferenceAssemblies))]
    public async Task AvoidPassingTaskWithoutCancellationAnalyzer(
        ReferenceAssemblies referenceAssemblies,
        TestDataFileSet fileSet)
    {
        // Make sure, we can resolve the assembly we're testing against:
        // Always pass `string.empty` for the language here to keep it generic.
        var referenceAssembly = await referenceAssemblies.ResolveAsync(
            language: string.Empty,
            cancellationToken: CancellationToken.None);

        string diagnosticId = DiagnosticIDs.AvoidPassingFuncReturningTaskWithoutCancellationToken;

        var context = GetAnalyzerTestContext(fileSet, referenceAssemblies);
        context.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(41, 21, 41, 97));
        context.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(44, 21, 44, 97));
        context.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(47, 21, 47, 98));

        await context.RunAsync();
    }
```
    
  - CodeFixTestCode.cs - Only for CodeFix scenarios:
    A CSharp/VB file which contains the code which is used for the CodeFix test. Note, this file has the code parts tagged,
    which are supposed to be fixed by the CodeFix. This file is used to verify that the CodeFix is working correctly.
    Here is a sample of such a code file, with the respective Tags, which are '[|' and '|]'.
 
 ```CSharp
 namespace CSharpControls;

// We are writing the fully-qualified name here to make sure, the Simplifier doesn't remove it,
// since this is nothing our code fix touches.
public class ScalableControl : System.Windows.Forms.Control
{
    private SizeF _scaleSize = new SizeF(3, 14);

    /// <Summary>
    ///  Sets or gets the scaled size of some foo bar thing.
    /// </Summary>
    [System.ComponentModel.Description("Sets or gets the scaled size of some foo bar thing.")]
    public SizeF [|ScaledSize|]
            {
                get => _scaleSize;
                set => _scaleSize = value;
            }

public float [|ScaleFactor|] { get; set; } = 1.0f;

/// <Summary>
///  Sets or gets the scaled location of some foo bar thing.
/// </Summary>
public PointF [|ScaledLocation|]
{ get; set; }
        }

```
  
  - FixedTestCode.cs - Only for CodeFixes: 
    A CSharp/VB file which contains the code which is the code which has been fixed by the CodeFix. 
    This file is used to verify that the CodeFix is working correctly.

For the above scenario, the fixed code would look like this:

```CSharp
using System.ComponentModel;

namespace CSharpControls;

// We are writing the fully-qualified name here to make sure, the Simplifier doesn't remove it,
// since this is nothing our code fix touches.
public class ScalableControl : System.Windows.Forms.Control
{
    private SizeF _scaleSize = new SizeF(3, 14);

    /// <Summary>
    ///  Sets or gets the scaled size of some foo bar thing.
    /// </Summary>
    [System.ComponentModel.Description("Sets or gets the scaled size of some foo bar thing.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SizeF ScaledSize
            {
                get => _scaleSize;
                set => _scaleSize = value;
            }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public float ScaleFactor { get; set; } = 1.0f;

    /// <Summary>
    ///  Sets or gets the scaled location of some foo bar thing.
    /// </Summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public PointF ScaledLocation
{ get; set; }
        }
```

A sample test, which would use the above code would look like this:

```CSharp
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Analyzers.Tests.Microsoft.WinForms;
using System.Windows.Forms.CSharp.Analyzers.MissingPropertySerializationConfiguration;
using System.Windows.Forms.CSharp.CodeFixes.AddDesignerSerializationVisibility;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.WinForms.Test;
using Microsoft.WinForms.Utilities.Shared;

namespace System.Windows.Forms.Analyzers.CSharp.Tests.AnalyzerTests.MissingPropertySerializationConfiguration;

/// <summary>
///  Represents a set of test scenarios for custom controls to verify
///  property serialization behavior.
/// </summary>
/// <remarks>
///  <para>
///   This class is derived from <see cref="RoslynAnalyzerAndCodeFixTestBase{TAnalyzer, TVerifier}"/>"/>
///   and is intended to validate how properties are serialized in custom controls during
///   analyzer and code-fix operations.
///  </para>
/// </remarks>
public class CustomControlScenarios
    : RoslynAnalyzerAndCodeFixTestBase<MissingPropertySerializationConfigurationAnalyzer, DefaultVerifier>
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="CustomControlScenarios"/> class.
    /// </summary>
    public CustomControlScenarios()
        : base(SourceLanguage.CSharp)
    {
    }
    
    /// <summary>
    ///  Retrieves reference assemblies for the latest target framework versions.
    /// </summary>
    public static IEnumerable<object[]> GetReferenceAssemblies()
    {
        NetVersion[] tfms =
        [
            NetVersion.Net6_0,
            NetVersion.Net7_0,
            NetVersion.Net8_0,
            NetVersion.Net9_0
        ];

        foreach (ReferenceAssemblies refAssembly in ReferenceAssemblyGenerator.GetForLatestTFMs(tfms))
        {
            yield return new object[] { refAssembly };
        }
    }

    /// <summary>
    ///  Tests the diagnostics produced by
    ///  <see cref="MissingPropertySerializationConfigurationAnalyzer"/>.
    /// </summary>
    [Theory]
    [CodeTestData(nameof(GetReferenceAssemblies))]
    public async Task TestDiagnostics(
        ReferenceAssemblies referenceAssemblies,
        TestDataFileSet fileSet)
    {
        var context = GetAnalyzerTestContext(fileSet, referenceAssemblies);
        await context.RunAsync();

        context = GetFixedTestContext(fileSet, referenceAssemblies);
        await context.RunAsync();
    }

    /// <summary>
    ///  Tests the code-fix provider to ensure it correctly applies designer serialization attributes.
    /// </summary>
    [Theory]
    [CodeTestData(nameof(GetReferenceAssemblies))]
    public async Task TestCodeFix(
        ReferenceAssemblies referenceAssemblies,
        TestDataFileSet fileSet)
    {
        var context = GetCodeFixTestContext<AddDesignerSerializationVisibilityCodeFixProvider>(
            fileSet,
            referenceAssemblies,
            numberOfFixAllIterations: -2);

        context.CodeFixTestBehaviors =
            CodeFixTestBehaviors.SkipFixAllInProjectCheck |
            CodeFixTestBehaviors.SkipFixAllInSolutionCheck;

        await context.RunAsync();
    }
}
```

* The Visual Basic equivalent of the above test class would look like this:

```VB
' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Windows.Forms.Analyzers.Tests.Microsoft.WinForms
Imports System.Windows.Forms.VisualBasic.Analyzers.MissingPropertySerializationConfiguration
Imports System.Windows.Forms.VisualBasic.CodeFixes.AddDesignerSerializationVisibility
Imports Microsoft.CodeAnalysis.Testing
Imports Microsoft.WinForms.Test
Imports Microsoft.WinForms.Utilities.Shared
Imports Xunit

Namespace System.Windows.Forms.Analyzers.VisualBasic.Tests.AnalyzerTests.MissingPropertySerializationConfiguration

    ''' <summary>
    '''  Represents a set of test scenarios for custom controls to verify
    '''  property serialization behavior.
    Public Class CustomControlScenarios
        Inherits RoslynAnalyzerAndCodeFixTestBase(Of MissingPropertySerializationConfigurationAnalyzer, DefaultVerifier)

        ''' <summary>
        '''  Initializes a new instance of the <see cref="CustomControlScenarios"/> class.
        ''' </summary>
        Public Sub New()
            MyBase.New(SourceLanguage.VisualBasic)
        End Sub

        ''' <summary>
        '''  Retrieves reference assemblies for the latest target framework versions.
        ''' </summary>
        Public Shared Iterator Function GetReferenceAssemblies() As IEnumerable(Of Object())
            Dim tfms As NetVersion() = {
                NetVersion.Net6_0,
                NetVersion.Net7_0,
                NetVersion.Net8_0,
                NetVersion.Net9_0
            }

            For Each refAssembly In ReferenceAssemblyGenerator.GetForLatestTFMs(tfms)
                Yield New Object() {refAssembly}
            Next
        End Function

        ''' <summary>
        '''  Tests the diagnostics produced by
        '''  <see cref="MissingPropertySerializationConfigurationAnalyzer"/>.
        ''' </summary>
        <Theory>
        <CodeTestData(NameOf(GetReferenceAssemblies))>
        Public Async Function TestDiagnostics(
                referenceAssemblies As ReferenceAssemblies,
                fileSet As TestDataFileSet) As Task
            Dim context = GetVisualBasicAnalyzerTestContext(fileSet, referenceAssemblies)
            Await context.RunAsync()

            context = GetVisualBasicFixedTestContext(fileSet, referenceAssemblies)
            Await context.RunAsync()
        End Function

        ''' <summary>
        '''  Tests the code-fix provider to ensure it correctly applies designer serialization attributes.
        ''' </summary>
        <Theory>
        <CodeTestData(NameOf(GetReferenceAssemblies))>
        Public Async Function TestCodeFix(
                referenceAssemblies As ReferenceAssemblies,
                fileSet As TestDataFileSet) As Task
            Dim context = GetVisualBasicCodeFixTestContext(Of AddDesignerSerializationVisibilityCodeFixProvider)(
                fileSet,
                referenceAssemblies,
                numberOfFixAllIterations:=-2)

            context.CodeFixTestBehaviors =
                CodeFixTestBehaviors.SkipFixAllInProjectCheck Or
                CodeFixTestBehaviors.SkipFixAllInSolutionCheck

            Await context.RunAsync()
        End Function
    End Class

End Namespace
```

Please note here the changed methods to get the test context for the VB tests:
`GetVisualBasicAnalyzerTestContext` and `GetVisualBasicCodeFixTestContext`.
