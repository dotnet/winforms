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
partial type from `Form`, `UserControl`, another `Control` type, or
`System.ComponentModel.Component`. Include negative cases with same-named types in unrelated
namespaces. For VB, use realistic `WithEvents`/`Handles` fixtures and distinguish member events from
`GenerateMember=False` local-variable wiring. Never modernize generated `Dispose` fixtures merely
to match restrictions that apply only to `InitializeComponent`.

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
The helper reads the configured SDK's runtime metadata and requires its exact reference pack; an
SDK version is not a runtime-pack version, and another installed preview is not a safe substitute.

Designer diagnostics must include a concise supported repair in the emitted message, not only in
the help page. Test representative rendered messages under an invariant culture as well as IDs,
arguments, paths, spans, suppression, and absence of analyzer exceptions. Preserve separate IDs for
independently repairable constructs.

## Syntax coverage boundary

The existing expression rules remain enforced because their constructs cannot round-trip through
the designer's CodeDOM contract. WFO3000 also covers structural `using`/`Using` and `await`/`Await`,
and the C# variants include expression-bodied initialization and deconstructing `foreach`. WFO3007
includes coalescing assignment.

Do not infer support for all C# or VB syntax from this list. Standalone patterns, tuples/deconstruction,
implicit object creation, object/collection initializers, queries, VB `With`, XML/anonymous objects,
and unstructured error handling are not additional rules in this change. Some require source-parser
lowering evidence outside this repository before an accurate restriction and mitigation can be
defined. Unlisted does not mean supported. The current Roslyn package is 4.12; syntax from newer
compiler versions also needs explicit compatibility work rather than guessed syntax matching.

## Targeted validation

Run these from the repository root:

```powershell
dotnet test src\System.Windows.Forms.Analyzers\tests\UnitTests\System.Windows.Forms.Analyzers.Tests.csproj -nologo
dotnet test src\System.Windows.Forms.Analyzers.CSharp\tests\UnitTests\System.Windows.Forms.Analyzers.CSharp.Tests.csproj -nologo
dotnet test src\System.Windows.Forms.Analyzers.VisualBasic\tests\UnitTests\System.Windows.Forms.Analyzers.VisualBasic.Tests\System.Windows.Forms.Analyzers.VisualBasic.Tests.vbproj -nologo
```

Before reporting the change as build-clean, run the repository-supported `.\build.cmd`; a plain
solution build does not apply all CI analyzers and warning settings.
