# 2025-03-09 C# to VB Adaptation Prompt

## Metadata

Author: klloeffe
Status: V1
Tags: C#, VB.NET, AI, Adaptation

## Prompt

You are an Assistant to one of the current maintainers of the WinForms .NET 9+ runtime repo.
Main purpose of the current PR, which this file has just been added to, is to refactor the 
infrastructure of the Analyzer tests. We already migrated the Test infrastructure for the new
WinForms specific Analyzers, which have been introduced for the WinForms .NET 9 for C#.

Simplifying writing tests for new Analyzers, so that contributors and maintainers can be more
efficient, and LLMs can easier and more reliably assist, is one of the goals of this PR.

Before, the test data and the tests themselves were mixed in the same file. Now, we have been
separated out the test data into separate files, and we organized the file structure.

In the project #System.Windows.Forms.Analyzers.CSharp.Tests, we have a folder _Analyzer_, which again 
contains a folder for each Analyzer. Each folder can contain multiple test files, and we have for 
each test file a corresponding test folder under the lead folder TestData. So, when the Analyzer is 
general categorized under MissingPropertySerializationConfiguration, then we have at least one test file 
- in this case named CustomControlScenarios.cs, and the under TestData a folder named CustomControlScenarios,
- which can have multiple test files, whose Compile properties are set to None, and they are never copied.

A series of those test data files follow a special naming convention, which are dictated by the concept
pf the existing Roslyn test infrastructure we use:

* AnalyzerTestCode.cs: The actual test code, which is used to test the Analyzer.
* CodeFixTestCode.cs: The actual test code, which is used to test an optional CodeFix.
* FixedTestCode.cs: The expected code after the CodeFix has been applied.
* GlobalUsing.cs: The global using directives, which are used in the test code.

There can be multiple additional files, which are added as pure context to the above files, should additional
classes or methods be needed for the respective test code to work in general.

For this current example, the code file CustomControlScenarios.cs contains the test data for the Analyzer.

Here is, how the test class is defined, and with it, which Analyzer to test and which verifyer to use:

```c#
public class CustomControlScenarios
    : RoslynAnalyzerAndCodeFixTestBase<MissingPropertySerializationConfigurationAnalyzer, DefaultVerifier>
```

Next, we define in the constructor of the test class the language:

```c#
public CustomControlScenarios()
        : base(SourceLanguage.CSharp)
    {
    }
```


Now. Lastly, we need to explain following code as a whole:

```c#
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
    /// <remarks>
    ///  <para>
    ///   First, it verifies that the initial diagnostics match the
    ///   expected results. Then, it re-runs the test context to confirm
    ///   that any corrections remain consistent.
    ///  </para>
    ///  <para>
    ///   This method depends on the provided file set and reference
    ///   assemblies to generate the appropriate environment for analysis.
    ///  </para>
    /// </remarks>
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
```

* GetReferenceAssemblies: This method returns the reference assemblies for the test.
* CodeTestData: This is an attribute derived from `MemberDataAttribute`, which is used to provide the test data for the test method.
  Usually, this is used to provide just the test data by pointing to a method, which returns the test data.
  CodeTestData in addition "mutates" the result a bit, and returns data, which satisfies the signature of the test
  method: Is provides the ReferenceAssemblies to test against the respective TFMs, but it also utilizes parts of the
  other test infrastructure, which provides sets of the test data files, based on the test class name and test test
  files in the folder under TestData with the identical name.

