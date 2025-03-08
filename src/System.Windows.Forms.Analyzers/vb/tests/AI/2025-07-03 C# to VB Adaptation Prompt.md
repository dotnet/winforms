# 2025-07-03 C# to VB Adaptation Prompt

## Metadata

Author: klloeffe
Status: Draft
Tags: C#, VB.NET, AI, Adaptation

## Prompt

You are an Assistant to one of the current maintainers of the WinForms .NET 9+ runtime repo.
Main purpose of the current PR, which this file has just been added to, is to refactor the 
infrastructure of the Analyzer tests. We already migrated the Test infrastructure for the new
WinForms specific Analyzers, which have been introduced for the WinForms .NET 9 for C#.
Simplifying writing tests for new Analyzers, so that contributers and maintainers can be more
efficient, and LLMs can easier and more reliably assist, is one of the goals of this PR.

Before, the test data and the tests themselves were mixed in the same file. Now, we have been
separated out the test data into separate files, and we organized the file structure.

Like in #System.Windows.Forms.Analyzers.CSharp.Tests, we have a folder Analyzer, which again contains
a folder for each Analyzer. Each folder can contain multiple test files, and we have for each test file
a corresponding test folder under the lead folder TestData. So, when the Analyzer is general categorized
under MissingPropertySerializationConfiguration, then we have at least one test file - in this case named
CustomControlScenarios.cs, and the under TestData a folder named CustomControlScenarios, which can have
multiple test files, whose Compile properties are set to None, and they are never copied.

A series of those test data files follow a special naming convention, which are dictated by the concept
pf the existing Roslyn test infrastructure we use:

* AnalyzerTestCode.cs: The actual test code, which is used to test the Analyzer.
* CodeFixTestCode.cs: The actual test code, which is used to test an optional CodeFix.
* FixedTestCode.cs: The expected code after the CodeFix has been applied.
* GlobalUsing.cs: The global using directives, which are used in the test code.

There can be multiple additional files, which are added as pure context to the above files, should additional
classes or methods be needed for the respective test code to work in general.

For this current example, the code file CustomControlScenarios.cs contains the test data for the Analyzer:
