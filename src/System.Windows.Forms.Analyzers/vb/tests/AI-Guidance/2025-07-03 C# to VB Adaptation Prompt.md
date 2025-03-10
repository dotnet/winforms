# 2025-03-09 C# to VB Test Adaptation Guide

## Context
You are assisting with refactoring the WinForms .NET 9+ runtime analyzer test infrastructure.
The goal: make tests easier to maintain, contribute to, and automate via LLM assistance.

## Current State
The C# analyzer tests follow this structure in `System.Windows.Forms.Analyzers.CSharp.Tests`:

```
/_Analyzer_/
  /<AnalyzerName>/
    <TestClass>.cs

/TestData/
  /<TestClass>/
    AnalyzerTestCode.cs
    CodeFixTestCode.cs
    FixedTestCode.cs
    GlobalUsing.cs
```

## Task: Create Visual Basic Test Equivalents

### 1. Folder Structure
Mirror the C# structure in `System.Windows.Forms.Analyzers.VisualBasic.Tests`:

```
/_Analyzer_/
  /<AnalyzerName>/
    <TestClass>.vb

/TestData/
  /<TestClass>/
    AnalyzerTestCode.vb
    CodeFixTestCode.vb
    FixedTestCode.vb
    GlobalImports.vb
```

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
    Dim context = GetAnalyzerTestContext(fileSet, referenceAssemblies)
    Await context.RunAsync()

    context = GetFixedTestContext(fileSet, referenceAssemblies)
    Await context.RunAsync()
  End Function
End Class
```

### 3. Test Data Files
- Translate the test data files from C# to VB:
  - `AnalyzerTestCode.cs` ➡️ `AnalyzerTestCode.vb`
  - `CodeFixTestCode.cs` ➡️ `CodeFixTestCode.vb`
  - `FixedTestCode.cs` ➡️ `FixedTestCode.vb`
  - `GlobalUsing.cs` ➡️ `GlobalImports.vb`
- Ensure:
  - `<Compile>` is set to `None`
  - `CopyToOutputDirectory` is `Never`

### 4. Additional Notes
- The analyzer type and verifier remain the same.
- `SourceLanguage.VisualBasic` differentiates VB from C# tests.
- Optional helper files must be translated as well.
- Should the tests use Roslyn specific API, ensure that the C# compatible Nodes-APIs
  are "translated" to VB compatible Roslyn APIs. 
  As an example. While a method in C# is represented as `MethodDeclarationSyntax` 
  in VB it is `MethodBlockSyntax`.

## Summary
Mirror the structure, translate the content, and ensure parity between C# and VB tests.
