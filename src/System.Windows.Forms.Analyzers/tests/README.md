# Extending WinForms analyzers

Analyzer tests use Roslyn's native C# and Visual Basic test types through the thin, language-neutral
`AnalyzerTestCase` and `AnalyzerTestFactory` helpers in `UnitTests`. Keep analyzer behavior in the
language project and reusable descriptors, resources, and semantic facts in the shared project.

## Adding a diagnostic

1. Reserve a WFO ID in
   `src\System.Windows.Forms.Analyzers\src\System\Windows\Forms\Analyzers\Diagnostics\DiagnosticIDs.cs`.
2. Add the localized title and message to `src\System.Windows.Forms.Analyzers\src\Resources\SR.resx`,
   then build so the XLF files are synchronized.
3. Add a descriptor to the shared or language-specific descriptor class.
4. Implement the analyzer under the applicable C# or Visual Basic `src\Analyzers` tree. Put common
   semantic detection in the shared analyzer project; keep syntax APIs language-specific.
5. Track the rule in `AnalyzerReleases.Unshipped.md` and document it in the applicable
   `docs\analyzers\WinFormsCSharpAnalyzers.Help.md` or
   `docs\analyzers\WinFormsVisualBasicAnalyzers.Help.md` file.
6. Add exact diagnostic ID, argument, path, and span assertions to the applicable test project.
   Document why a rule is language-specific when no equivalent syntax exists.

Do not add a code fix unless the transformation is safe and deterministic.

## Writing tests

Use inline `AnalyzerTestSource` values for focused behavior. Use named fixture files under a
`TestData` directory when a realistic source set would obscure the assertion. For Designer rules,
always provide both the user file and its `.Designer.cs` or `.Designer.vb` counterpart, and derive the
partial type from `Form`, `UserControl`, or another `Control` type.

Create the language-native Roslyn test through `AnalyzerTestFactory`; do not add another test base
class or block asynchronous calls:

```csharp
AnalyzerTestCase testCase = new(
    ReferenceAssemblies.Net.Net90Windows,
    new AnalyzerTestSource("Form1.cs", mainSource),
    new AnalyzerTestSource("Form1.Designer.cs", designerSource));

await AnalyzerTestFactory.CreateCSharpAnalyzerTest<MyAnalyzer>(testCase)
    .RunAsync(TestContext.Current.CancellationToken);
```

Use `AnalyzerConfigFiles` for `.editorconfig` or `.globalconfig` cases, `AdditionalFiles` for
non-source analyzer inputs, and `AdditionalReferences` for metadata paths. Tests that need the latest
WinForms API surface should use `CurrentReferences.NetCoreAppReferences` and add
`CurrentReferences.WinFormsRefPath`, following the existing WFO2001 tests.

## Targeted validation

Run these from the repository root:

```powershell
dotnet test src\System.Windows.Forms.Analyzers\tests\UnitTests\System.Windows.Forms.Analyzers.Tests.csproj -nologo
dotnet test src\System.Windows.Forms.Analyzers.CSharp\tests\UnitTests\System.Windows.Forms.Analyzers.CSharp.Tests.csproj -nologo
dotnet test src\System.Windows.Forms.Analyzers.VisualBasic\tests\UnitTests\System.Windows.Forms.Analyzers.VisualBasic.Tests\System.Windows.Forms.Analyzers.VisualBasic.Tests.vbproj -nologo
```

Before reporting the change as build-clean, run the repository-supported `.\build.cmd`; a plain
solution build does not apply all CI analyzers and warning settings.
