// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.CSharp.Analyzers.Designer;
using Microsoft.CodeAnalysis.Testing;

namespace System.Windows.Forms.Analyzers.Tests;

/// <summary>
///  Tests the C# WinForms Designer guardrails.
/// </summary>
[ForceGC]
[SkipOnArchitecture(TestArchitectures.X86, "Analyzer tests require Roslyn reference assembly extraction")]
public class DesignerAnalyzerTests
{
    private const string MainSource =
        """
        using System.Windows.Forms;

        namespace Test;

        partial class Form1 : Form
        {
            public Form1()
            {
                InitializeComponent();
            }
        }
        """;

    [Fact]
    public async Task InitializeComponent_ValidGeneratedCode_NoDiagnostic()
    {
        const string designerSource =
            """
            using System.ComponentModel;
            using System.Windows.Forms;

            namespace Test;

            partial class Form1
            {
                private IContainer? components;

                protected override void Dispose(bool disposing)
                {
                    components?.Dispose();
                    base.Dispose(disposing);
                }

                private void InitializeComponent()
                {
                    button1 = new Button();
                    Controls.Add(button1);
                }

                private Button button1;
            }
            """;

        AnalyzerTestCase testCase = CreateTestCase(designerSource);

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<InitializeComponentAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<DesignerFileStructureAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task InitializeComponent_ControlFlow_ReportsDiagnostic()
    {
        const string designerSource =
            """
            using System.Windows.Forms;

            namespace Test;

            partial class Form1
            {
                private void InitializeComponent()
                {
                    {|WFO2002:if|} (true)
                    {
                        Controls.Add(new Button());
                    }
                }
            }
            """;

        AnalyzerTestCase testCase = CreateTestCase(designerSource);

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<InitializeComponentAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Theory]
    [InlineData("{|WFO2002:for|} (int i = 0; i < Controls.Count; i++) { }")]
    [InlineData("{|WFO2002:foreach|} (Control control in Controls) { }")]
    [InlineData("{|WFO2002:while|} (DesignMode) { }")]
    [InlineData("{|WFO2002:do|} { } while (DesignMode);")]
    [InlineData("{|WFO2002:if|} (DesignMode) { }")]
    [InlineData("{|WFO2002:switch|} (Controls.Count) { default: break; }")]
    [InlineData("_ = Controls.Count {|WFO2002:switch|} { _ => 0 };")]
    [InlineData("void {|WFO2002:Configure|}() { }")]
    [InlineData("{|WFO2002:goto|} End; End:;")]
    [InlineData("{|WFO2002:try|} { } catch { }")]
    [InlineData("{|WFO2002:lock|} (this) { }")]
    public async Task InitializeComponent_UnsupportedConstruct_ReportsDiagnostic(string statement)
    {
        string designerSource =
            $$"""
            using System.Windows.Forms;

            namespace Test;

            partial class Form1
            {
                private void InitializeComponent()
                {
                    {{statement}}
                }
            }
            """;

        AnalyzerTestCase testCase = CreateTestCase(designerSource);

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<InitializeComponentAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task InitializeComponent_CodeDomUnsupportedExpressions_ReportDiagnostics()
    {
        const string designerSource =
            """
            using System;
            using System.Windows.Forms;

            namespace Test;

            partial class Form1
            {
                private void InitializeComponent()
                {
                    string name = {|WFO2007:nameof|}(Form1);
                    string text = {|WFO2011:$"|}{name}: {Controls.Count}";
                    EventHandler handler = (_, _) {|WFO2012:=>|} Text = text;
                    EventHandler alternate = {|WFO2012:delegate|} { };
                    object value = Tag {|WFO2009:??|} this;
                    Control? control = Parent{|WFO2010:?|}.Parent;
                    int count = DesignMode {|WFO2008:?|} 0 : Controls.Count;
                }
            }
            """;

        AnalyzerTestCase testCase = CreateTestCase(designerSource);

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<InitializeComponentAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DesignerFile_CollectionExpression_ReportsDiagnostic()
    {
        const string designerSource =
            """
            using System.Windows.Forms;

            namespace Test;

            partial class Form1
            {
                private void InitializeComponent()
                {
                    Control[] controls = {|WFO2006:[|}new Button()];
                }
            }
            """;

        AnalyzerTestCase testCase = CreateTestCase(designerSource);

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<DesignerFileStructureAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DesignerMembers_ReportStructuralDiagnostics()
    {
        const string designerSource =
            """
            using System;
            using System.Windows.Forms;

            namespace Test;

            partial class Form1
            {
                private Button {|WFO2004:button1|};

                private void InitializeComponent()
                {
                    button1 = new Button();
                }

                public int {|WFO2003:Value|} { get; set; }
                public event EventHandler? {|WFO2005:Changed|};
                public delegate void {|WFO2005:Callback|}();
            }
            """;

        AnalyzerTestCase testCase = CreateTestCase(designerSource);

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<DesignerFileStructureAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task MainFileField_ReferencedByInitializeComponent_ReportsDiagnosticAtDeclaration()
    {
        const string mainSource =
            """
            using System.Windows.Forms;

            namespace Test;

            partial class Form1 : Form
            {
                private Button {|WFO2004:button1|};

                public Form1()
                {
                    InitializeComponent();
                }
            }
            """;

        const string designerSource =
            """
            using System.Windows.Forms;

            namespace Test;

            partial class Form1
            {
                private void InitializeComponent()
                {
                    button1 = new Button();
                    Controls.Add(button1);
                }
            }
            """;

        AnalyzerTestCase testCase = new(
            ReferenceAssemblies.Net.Net90Windows,
            new AnalyzerTestSource("Form1.cs", mainSource),
            new AnalyzerTestSource("Form1.Designer.cs", designerSource));

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<DesignerFileStructureAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task InitializeComponent_LocalAndMethodReferences_DoNotReportFieldDiagnostic()
    {
        const string designerSource =
            """
            using System.Windows.Forms;

            namespace Test;

            partial class Form1
            {
                private void InitializeComponent()
                {
                    Button button1 = CreateButton();
                    Controls.Add(button1);
                }

                private Button {|WFO2003:CreateButton|}()
                {
                    return new Button();
                }
            }
            """;

        AnalyzerTestCase testCase = CreateTestCase(designerSource);

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<DesignerFileStructureAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DesignerFile_MultipleTypes_AnalyzesMatchingControlOnly()
    {
        const string mainSource =
            """
            using System.Windows.Forms;

            namespace Test;

            partial class Form1<T> : Form
            {
                public Form1()
                {
                    InitializeComponent();
                }

                public partial class Nested : UserControl
                {
                }
            }

            partial class Model
            {
            }
            """;

        const string designerSource =
            """
            namespace Test;

            partial class Form1<T>
            {
                private void InitializeComponent()
                {
                    {|WFO2002:if|} (DesignMode) { }
                }

                public partial class Nested
                {
                    private void InitializeComponent()
                    {
                    }

                    public int Value { get; set; }
                }
            }

            partial class Model
            {
                private void InitializeComponent()
                {
                    if (true) { }
                }

                public int Value { get; set; }
            }
            """;

        AnalyzerTestCase testCase = new(
            ReferenceAssemblies.Net.Net90Windows,
            new AnalyzerTestSource("Form1.cs", mainSource),
            new AnalyzerTestSource("Form1.Designer.cs", designerSource));

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<InitializeComponentAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DesignerFile_MalformedDispose_ReportsUnexpectedMember()
    {
        const string designerSource =
            """
            namespace Test;

            partial class Form1
            {
                private void InitializeComponent()
                {
                }

                private void {|WFO2003:Dispose|}(int disposing)
                {
                }
            }
            """;

        AnalyzerTestCase testCase = CreateTestCase(designerSource);

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<DesignerFileStructureAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DesignerFile_MalformedInitializeComponent_IsNotDesignerPair()
    {
        const string designerSource =
            """
            namespace Test;

            partial class Form1
            {
                private int InitializeComponent()
                {
                    if (DesignMode) { }
                    return 0;
                }

                public int Value { get; set; }
            }
            """;

        AnalyzerTestCase testCase = CreateTestCase(designerSource);

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<InitializeComponentAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<DesignerFileStructureAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task InitializeComponent_DiagnosticSuppressedByAnalyzerConfig()
    {
        const string designerSource =
            """
            namespace Test;

            partial class Form1
            {
                private void InitializeComponent()
                {
                    if (DesignMode) { }
                }
            }
            """;

        AnalyzerTestCase testCase = CreateTestCase(designerSource);
        testCase.AnalyzerConfigFiles.Add(
            ("/.globalconfig",
            """
            is_global = true

            dotnet_diagnostic.WFO2002.severity = none
            """));

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<InitializeComponentAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task UnrelatedDesignerNamedFile_NoDiagnostic()
    {
        const string mainSource =
            """
            namespace Test;

            partial class Model
            {
            }
            """;

        const string designerSource =
            """
            namespace Test;

            partial class Model
            {
                private void InitializeComponent()
                {
                    if (true)
                    {
                    }
                }

                public int Value { get; set; }
            }
            """;

        AnalyzerTestCase testCase = new(
            ReferenceAssemblies.Net.Net90Windows,
            new AnalyzerTestSource("Model.cs", mainSource),
            new AnalyzerTestSource("Model.Designer.cs", designerSource));

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<InitializeComponentAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<DesignerFileStructureAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    private static AnalyzerTestCase CreateTestCase(string designerSource)
        => new(
            ReferenceAssemblies.Net.Net90Windows,
            new AnalyzerTestSource("Form1.cs", MainSource),
            new AnalyzerTestSource("Form1.Designer.cs", designerSource));
}
