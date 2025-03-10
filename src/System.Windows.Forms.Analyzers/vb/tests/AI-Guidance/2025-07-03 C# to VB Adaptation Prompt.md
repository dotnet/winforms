# 2025-03-09 C# to VB Test Adaptation Guide

## Context
You are assisting with porting C# tests to Visual Basic for the projects 
`System.Windows.Forms.Analyzers.CSharp.Tests` and `System.Windows.Forms.Analyzers.VisualBasic.Tests`
Most of the WinForms specific analyzers here in the WinForms runtime repo have been written in C#,
but the Analyzers are also existing in a respective VB version (written in VB). We need to make
sure that the tests for the VB analyzers are also available and up-to-date.

## Current State
The C# analyzer tests follow this structure in `System.Windows.Forms.Analyzers.CSharp.Tests`:

```
/_Analyzer_/
  /<AnalyzerName>/
    <TestClass1>.cs
    /TestData/
      /<TestClass1FolderName>/
        AnalyzerTestCode.cs
        [CodeFixTestCode.cs]
        [FixedTestCode.cs]
        [GlobalUsing.cs]
        [AdditionalFiles.cs]
    <TestClass2>.cs
    /TestData/
      /<TestClass2FolderName>/
        AnalyzerTestCode.cs
        [CodeFixTestCode.cs]
        [FixedTestCode.cs]
        [GlobalUsing.cs]
        [AdditionalFiles.cs]
```

## Task: Create Visual Basic Test Equivalents

The user will be prompting for either the Analyzer or for specific Test classes 
_of_ the Analyzer which they would need to be ported to VB. For example

"Please create the VB equivalent for <AnalyzerName>." or
"Please create the VB equivalent for <TestClass1> of the <AnalyzerName>."

### 1. Folder Structure
Mirror the C# structure in `System.Windows.Forms.Analyzers.VisualBasic.Tests`:

```
/_Analyzer_/
  /<AnalyzerName>/
    <TestClass1>.vb
    /TestData/
      /<TestClass1FolderName>/
        AnalyzerTestCode.vb
        [CodeFixTestCode.vb]
        [FixedTestCode.vb]
        [AdditionalFiles.vb]
    <TestClass2>.vb
    /TestData/
      /<TestClass2FolderName>/
        AnalyzerTestCode.vb
        [CodeFixTestCode.vb]
        [FixedTestCode.vb]
        [AdditionalFiles.vb]
```

Note, that there are no equivalents for the `GlobalUsing.cs` in VB,
so, make sure to adjust the `Imports` statements in all the VB files accordingly.

### 2. Test Class Example (VB)
Convert the C# test class to VB. Example:

```vb
Public Class CustomControlScenarios
  Inherits RoslynAnalyzerAndCodeFixTestBase(Of MissingPropertySerializationConfigurationAnalyzer, DefaultVerifier)

  Public Sub New()
    MyBase.New(SourceLanguage.VisualBasic)
  End Sub

  Public Shared Iterator Function GetReferenceAssemblies() As IEnumerable(Of Object())
    Dim tfms As NetVersion() =
    {
      NetVersion.Net6_0,
      NetVersion.Net7_0,
      NetVersion.Net8_0,
      NetVersion.Net9_0
    }

    For Each refAssembly In ReferenceAssemblyGenerator.GetForLatestTFMs(tfms)
      Yield New Object() {refAssembly}
    Next
  End Function

  <Theory>
  <CodeTestData(NameOf(GetReferenceAssemblies))>
  Public Async Function TestDiagnostics(referenceAssemblies As ReferenceAssemblies, fileSet As TestDataFileSet) As Task
    Dim context = GetVisualBasicAnalyzerTestContext(fileSet, referenceAssemblies)
    Await context.RunAsync()

    context = GetVisualBasicFixedTestContext(fileSet, referenceAssemblies)
    Await context.RunAsync()
  End Function
End Class
```

Note, that the actual test classes in VB not only need to be translated from C#.
Also, the test classes, if they are using Roslyn _language_ specific APIs, need to be
adjusted as well. That is the reason, why in the sample, the `GetVisualBasicAnalyzerTestContext` and
`GetVisualBasicFixedTestContext` extension methods for example are used.

Should a test later be using language specific SyntaxNode types, make sure that for example
a `MethodDeclarationSyntax` in C# is translated would need to be translated to respective 
`MethodBlockSyntax` which then could be of `Kind` `MethodBlockSyntaxKind.FunctionBlock` 
or `MethodBlockSyntaxKind.SubBlock` - as just one example.

### 3. Test Data Files
- Translate the test data files from C# to VB:
  - `AnalyzerTestCode.cs` ➡️ `AnalyzerTestCode.vb`
  - `CodeFixTestCode.cs` ➡️ `CodeFixTestCode.vb`
  - `FixedTestCode.cs` ➡️ `FixedTestCode.vb`

Note that every code file except the AnalyzerTestCode file are optional.
Also note, that the CodeFixTestCode always has Diagnostic marker in the code, like this:

```csharp
public float [|ScaleFactor|] { get; set; } = 1.0f;
```

Those marker would then need to be translated to VB as well, like this:
```vb
Public Property [|ScaleFactor|] As Single = 1.0F
```

### 4. Additional Notes
- The analyzer type and verifier remain the same.
- `SourceLanguage.VisualBasic` differentiates VB from C# tests.
- Optional helper files must be translated as well.

## Summary
Mirror the structure, translate the content, and ensure parity between C# and VB tests.
- Ensure that the VB tests are functionally equivalent to the C# tests, and where we
  have language specifics which need to be taken into account (for example, we can have
  `await` in catch blocks in C# but NOT in VB, please find respective workarounds).
