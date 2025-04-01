# Writing Test Cases for WinForms Analyzers and CodeFixes

## Purpose and Overview
This guide provides instructions for using AI to create comprehensive test cases for WinForms Analyzers and CodeFixes in both C# and Visual Basic. Following these guidelines will ensure consistent, maintainable, and effective tests across the codebase.

## Table of Contents
1. [Project Structure](#project-structure)
2. [Language-Specific Considerations](#language-specific-considerations)
3. [Test Creation Workflow](#test-creation-workflow)
4. [Required Files](#required-files)
5. [Test File Structure](#test-file-structure)
6. [Test Implementation](#test-implementation)
7. [Code Examples](#code-examples)
8. [Troubleshooting](#troubleshooting)
9. [Quality Checklist](#quality-checklist)

## Project Structure
We currently have 3 different test projects in the solution:
* `System.Windows.Forms.Analyzers.CSharp.Tests`
* `System.Windows.Forms.Analyzers.VisualBasic.Tests`
* `System.Windows.Forms.Analyzers.Tests`

**Important**: AI assistance is currently only used for adding tests to the CSharp and VisualBasic test projects.

## Language-Specific Considerations

### C# Tests
* Written in C# and target analyzers written in C#
* Use the following infrastructure:
  - Namespaces from `Microsoft.CodeAnalysis...`
  - Namespace `Microsoft.CodeAnalysis.CSharp.Testing`
  - Base class `RoslynAnalyzerAndCodeFixTestBase<TAnalyzer, DefaultVerifier>`
* Use `GetAnalyzerTestContext` and `GetCodeFixTestContext` methods
* Must include a `GlobalUsings.cs` file that includes at least `System.Windows.Forms` and `System.Drawing`

### Visual Basic Tests
* Written in Visual Basic and target analyzers written in Visual Basic
* Use the following infrastructure:
  - Namespaces from `Microsoft.CodeAnalysis...`
  - Namespace `Microsoft.CodeAnalysis.VisualBasic.Testing`
  - Base class `RoslynAnalyzerAndCodeFixTestBase(Of TAnalyzer, DefaultVerifier)`
  - Extension class `VisualBasicAnalyzerAndCodeFixExtensions` for VB-specific requirements
* Use `GetVisualBasicAnalyzerTestContext` and `GetVisualBasicCodeFixTestContext` methods

**Note**: Always specify whether you need C# or Visual Basic tests. Requests for generic "Analyzer tests" without specifying the language will be refused.

**Important**: Clearly distinguish whether you need only Analyzer tests or both Analyzer and CodeFix tests. Only create the necessary test files based on this distinction.

## Test Creation Workflow

1. **Identify the target language** (C# or Visual Basic)
2. **Determine test scope** (Analyzer-only or Analyzer with CodeFix)
3. **Create the appropriate folder structure**:
   ```
   Analyzer\[AnalyzerName]\TestData\
   ```
   Example: `Analyzer\EnsureModelDialogDisposed\TestData\`

4. **Create a new test class** with a descriptive name
   * For C#: Derive from `RoslynAnalyzerAndCodeFixTestBase<TAnalyzer, DefaultVerifier>`
   * For VB: Derive from `RoslynAnalyzerAndCodeFixTestBase(Of TAnalyzer, DefaultVerifier)`

5. **Create required files** (see [Required Files](#required-files))

6. **Create test data files**:
   * Create a subfolder named the same as your test class
   * Add the necessary test files based on test scope (see [Test File Structure](#test-file-structure))
   * Set BuildAction to `None` and "Copy to output directory" to `Do not copy`

7. **Implement test methods** appropriate for the test scope
8. **Run and validate** the tests

## Required Files

### For C# Tests
1. **GlobalUsings.cs** - Must include at minimum:
   ```csharp
   global using System.Windows.Forms;
   global using System.Drawing;
   ```

2. **Program.cs** - A starting point that should ideally use some of the test class objects:
   ```csharp
   namespace MyNamespace;

   internal static class Program
   {
       [STAThread]
       static void Main()
       {
           ApplicationConfiguration.Initialize();
           // Optionally use test class objects here
           Application.Run(new Form1());
       }
   }
   ```

### For Visual Basic Tests
Equivalent imports and program files in VB syntax as needed.

## Test File Structure

### For Analyzer-Only Tests
- **AnalyzerTestCode.cs/.vb**: 
  * Contains code that should trigger the analyzer or edge cases where it shouldn't trigger
  * Used to verify the analyzer produces correct diagnostics
- **Additional supporting files** as needed (with clear naming conventions)

### For CodeFix Tests
- **CodeFixTestCode.cs/.vb**: 
  * Contains code with marked regions that should be fixed by the CodeFixProvider
  * Uses special markers `[|` and `|]` to highlight the exact code segments that should be fixed
  * Example: `public SizeF [|ScaledSize|] { get; set; }`

- **FixedTestCode.cs/.vb**: 
  * Contains the expected code after the CodeFixProvider has been applied
  * Used to verify the CodeFix correctly transforms the code

### Additional Files
- The test data folder can contain additional supporting files if needed for the test scenario
- All supporting files should follow consistent naming conventions and be clearly documented

## Test Implementation

### Analyzer-Only Test Method
For testing just the analyzer functionality without code fixes, you must explicitly specify where diagnostics are expected using `ExpectedDiagnostics.Add()`:

```csharp
[Theory]
[CodeTestData(nameof(GetReferenceAssemblies))]
public async Task TestAnalyzerDiagnostics(
    ReferenceAssemblies referenceAssemblies,
    TestDataFileSet fileSet)
{
    // Make sure we can resolve the assembly we're testing against
    var referenceAssembly = await referenceAssemblies.ResolveAsync(
        language: string.Empty,
        cancellationToken: CancellationToken.None);

    string diagnosticId = DiagnosticIDs.YourDiagnosticRuleId;
    
    var context = GetAnalyzerTestContext(fileSet, referenceAssemblies);
    
    // Explicitly specify where diagnostics are expected
    context.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(41, 21, 41, 97));
    context.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(44, 21, 44, 97));
    
    await context.RunAsync();
}
```

### Full CodeFix Test Methods
When testing both the analyzer and a code fix:

```csharp
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

[Theory]
[CodeTestData(nameof(GetReferenceAssemblies))]
public async Task TestCodeFix(
    ReferenceAssemblies referenceAssemblies,
    TestDataFileSet fileSet)
{
    var context = GetCodeFixTestContext<YourCodeFixProvider>(
        fileSet,
        referenceAssemblies,
        numberOfFixAllIterations: -2);

    context.CodeFixTestBehaviors =
        CodeFixTestBehaviors.SkipFixAllInProjectCheck |
        CodeFixTestBehaviors.SkipFixAllInSolutionCheck;

    await context.RunAsync();
}
```

### Reference Assemblies Provider
Each test class should include a method to provide reference assemblies:

```csharp
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
```

## Code Examples

### GlobalUsings.cs Example for C#

```csharp
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

global using System;
global using System.Collections.Generic;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Windows.Forms;
global using System.Drawing;
global using Microsoft.CodeAnalysis;
global using Xunit;
```

### Program.cs Example for C#

```csharp
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace TestNamespace;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        
        // Optional: Use test class objects
        var testForm = new TestForm();
        
        Application.Run(new Form1());
    }
}
```

### C# Analyzer-Only Test Class Example

```csharp
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Analyzers.Tests.Microsoft.WinForms;
using System.Windows.Forms.CSharp.Analyzers.MyCustomAnalyzer;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.WinForms.Test;
using Microsoft.WinForms.Utilities.Shared;

namespace System.Windows.Forms.Analyzers.CSharp.Tests.AnalyzerTests.MyCustomAnalyzer;

/// <summary>
///  Tests for the MyCustomAnalyzer analyzer.
/// </summary>
public class MyCustomAnalyzerTests
    : RoslynAnalyzerAndCodeFixTestBase<MyCustomAnalyzer, DefaultVerifier>
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="MyCustomAnalyzerTests"/> class.
    /// </summary>
    public MyCustomAnalyzerTests()
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
    ///  Tests the diagnostics produced by the analyzer.
    /// </summary>
    [Theory]
    [CodeTestData(nameof(GetReferenceAssemblies))]
    public async Task TestDiagnostics(
        ReferenceAssemblies referenceAssemblies,
        TestDataFileSet fileSet)
    {
        // Make sure we can resolve the assembly we're testing against
        var referenceAssembly = await referenceAssemblies.ResolveAsync(
            language: string.Empty,
            cancellationToken: CancellationToken.None);
            
        string diagnosticId = DiagnosticIDs.MyCustomAnalyzerRuleId;
        
        var context = GetAnalyzerTestContext(fileSet, referenceAssemblies);
        
        // Explicitly specify where diagnostics are expected
        context.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(10, 15, 10, 25));
        context.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(20, 10, 20, 35));
        
        await context.RunAsync();
    }
}
```

### C# Complete Analyzer and CodeFix Test Class Example

```csharp
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

### Visual Basic Analyzer-Only Test Class Example

```vb
' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Windows.Forms.Analyzers.Tests.Microsoft.WinForms
Imports System.Windows.Forms.VisualBasic.Analyzers.MyCustomAnalyzer
Imports Microsoft.CodeAnalysis.Testing
Imports Microsoft.WinForms.Test
Imports Microsoft.WinForms.Utilities.Shared
Imports Xunit

Namespace System.Windows.Forms.Analyzers.VisualBasic.Tests.AnalyzerTests.MyCustomAnalyzer

    ''' <summary>
    '''  Tests for the MyCustomAnalyzer analyzer.
    ''' </summary>
    Public Class MyCustomAnalyzerTests
        Inherits RoslynAnalyzerAndCodeFixTestBase(Of MyCustomAnalyzer, DefaultVerifier)

        ''' <summary>
        '''  Initializes a new instance of the <see cref="MyCustomAnalyzerTests"/> class.
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
        '''  Tests the diagnostics produced by the analyzer.
        ''' </summary>
        <Theory>
        <CodeTestData(NameOf(GetReferenceAssemblies))>
        Public Async Function TestDiagnostics(
                referenceAssemblies As ReferenceAssemblies,
                fileSet As TestDataFileSet) As Task
                
            ' Make sure we can resolve the assembly we're testing against
            Dim referenceAssembly = Await referenceAssemblies.ResolveAsync(
                language:=String.Empty,
                cancellationToken:=CancellationToken.None)
                
            Dim diagnosticId As String = DiagnosticIDs.MyCustomAnalyzerRuleId
            
            Dim context = GetVisualBasicAnalyzerTestContext(fileSet, referenceAssemblies)
            
            ' Explicitly specify where diagnostics are expected
            context.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(10, 15, 10, 25))
            context.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(20, 10, 20, 35))
            
            Await context.RunAsync()
        End Function
    End Class

End Namespace
```

### Code Fix Markers Explanation

The code fix test files use special markers to denote regions that should be modified by the code fix:

```csharp
// Before code fix:
public SizeF [|ScaledSize|] { get; set; }

// After code fix:
[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
public SizeF ScaledSize { get; set; }
```

The `[|` and `|]` markers precisely identify the text that the analyzer should flag for a diagnostic and that the code fix should transform. When the test is run:

1. The test framework removes these markers before passing the code to the analyzer
2. It uses the marker positions to verify that diagnostics are reported at exactly these locations
3. It then applies the code fix and compares the result to the expected fixed code

## Troubleshooting

### Common Issues

1. **Incorrect diagnostic locations**
   - Ensure spans in `WithSpan()` match the actual code position
   - Check that the markers in code fix test files enclose the exact code that needs fixing

2. **Missing references**
   - If tests fail with missing types, add the required references to your test context:
   ```csharp
   context.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(typeof(YourType).Assembly.Location));
   ```

3. **Test failures in specific target frameworks**
   - Target framework-specific behavior can be handled with conditional checks:
   ```csharp
   if (referenceAssemblies.ToString().Contains("net7.0"))
   {
       // Special handling for .NET 7
   }
   ```

4. **File generation confusion**
   - Only create CodeFixTestCode.cs and FixedTestCode.cs files when specifically implementing a CodeFix test
   - For Analyzer-only tests, only create the AnalyzerTestCode.cs file

5. **Missing expected diagnostics**
   - For Analyzer-only tests, always explicitly specify where diagnostics are expected using `context.ExpectedDiagnostics.Add()`
   - The span coordinates (line, column, length) must precisely match where the diagnostic occurs in the code

### Tips for Debugging

- Use `context.ExpectedDiagnostics.Clear()` to handle cases where you want to test that no diagnostics are reported
- For complex code fix scenarios, consider breaking down the tests into smaller, focused test cases
- When diagnosing issues, temporarily add comments to your test files to mark important line numbers

## Naming Conventions

1. **Test Class Names**
   - Use descriptive names that clearly identify the analyzer being tested
   - End class names with "Tests" or "Scenarios"
   - Examples: `EnsureModalDialogDisposedTests`, `CustomControlScenarios`

2. **Test Method Names**
   - Methods should describe what they're testing
   - Use prefix "Test" for clarity
   - Examples: `TestDiagnostics`, `TestCodeFix`, `TestEdgeCases`

3. **Test File Names**
   - Follow the established pattern: `AnalyzerTestCode.cs`, `CodeFixTestCode.cs`, `FixedTestCode.cs`
   - For multiple scenarios, append a descriptive suffix: `AnalyzerTestCode_UserControl.cs`
   - Supporting files should have clear, descriptive names related to their purpose

## Quality Checklist

Before submitting your tests, verify the following:

- [ ] Test class inherits from the correct base class for the target language
- [ ] Test class uses the correct language in constructor (C# or VB)
- [ ] GlobalUsings.cs is included for C# tests with required imports
- [ ] Program.cs is included as a starting point
- [ ] Test files have correct BuildAction (None) and Copy settings
- [ ] Folder structure follows conventions
- [ ] All necessary usings/imports are included
- [ ] Tests cover both positive cases (diagnostic should trigger) and negative cases (diagnostic should not trigger)
- [ ] For Analyzer-only tests, explicit ExpectedDiagnostics are specified with exact locations
- [ ] Only appropriate test files are created (Analyzer-only vs. CodeFix scenarios)
- [ ] Code fix tests verify the correct transformation of code (when applicable)
- [ ] Tests include proper documentation and summaries
- [ ] Tests run successfully against all target frameworks
- [ ] Markers in code fix test files correctly identify the regions to be fixed (when applicable)
