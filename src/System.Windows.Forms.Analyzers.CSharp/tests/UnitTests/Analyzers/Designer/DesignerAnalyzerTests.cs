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
                    {|WFO3000:if|} (true)
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
    [InlineData("{|WFO3000:for|} (int i = 0; i < Controls.Count; i++) { }")]
    [InlineData("{|WFO3000:foreach|} (Control control in Controls) { }")]
    [InlineData("{|WFO3000:while|} (DesignMode) { }")]
    [InlineData("{|WFO3000:do|} { } while (DesignMode);")]
    [InlineData("{|WFO3000:if|} (DesignMode) { }")]
    [InlineData("{|WFO3000:switch|} (Controls.Count) { default: break; }")]
    [InlineData("_ = Controls.Count {|WFO3000:switch|} { _ => 0 };")]
    [InlineData("void {|WFO3000:Configure|}() { }")]
    [InlineData("{|WFO3000:goto|} End; End:;")]
    [InlineData("{|WFO3000:try|} { } catch { }")]
    [InlineData("{|WFO3000:lock|} (this) { }")]
    [InlineData("Tag {|WFO3007:??=|} new object();")]
    [InlineData("{|WFO3000:foreach|} (var (x, y) in new (int, int)[] { (1, 2) }) { }")]
    [InlineData("{|WFO3000:using|} (var component = new System.ComponentModel.Component()) { }")]
    [InlineData("{|WFO3000:using|} var component = new System.ComponentModel.Component();")]
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
                    string name = {|WFO3005:nameof|}(Form1);
                    string text = {|WFO3009:$"|}{name}: {Controls.Count}";
                    EventHandler handler = (_, _) {|WFO3010:=>|} Text = text;
                    EventHandler alternate = {|WFO3010:delegate|} { };
                    object value = Tag {|WFO3007:??|} this;
                    Control? control = Parent{|WFO3008:?|}.Parent;
                    int count = DesignMode {|WFO3006:?|} 0 : Controls.Count;
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
                    Control[] controls = {|WFO3004:[|}new Button()];
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
                private Button {|WFO3002:button1|};

                private void InitializeComponent()
                {
                    button1 = new Button();
                }

                public int {|WFO3001:Value|} { get; set; }
                public event EventHandler? {|WFO3003:Changed|};
                public delegate void {|WFO3003:Callback|}();
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
                private Button {|WFO3002:button1|};

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

                private Button {|WFO3001:CreateButton|}()
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
                    {|WFO3000:if|} (DesignMode) { }
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

                private void {|WFO3001:Dispose|}(int disposing)
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

            dotnet_diagnostic.WFO3000.severity = none
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

    [Fact]
    public async Task InitializeComponent_ExpressionBody_ReportsDiagnostics()
    {
        AnalyzerTestCase testCase = CreateTestCase(
            """
            namespace Test;
            partial class Form1
            {
                private void InitializeComponent() {|WFO3000:=>|} Tag = Tag {|WFO3007:??|} new object();
            }
            """);

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<InitializeComponentAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task InitializeComponent_NonDesignerOwnedFields_NoDiagnostic()
    {
        AnalyzerTestCase testCase = new(
            ReferenceAssemblies.Net.Net90Windows,
            new AnalyzerTestSource("Form1.cs",
                """
                using System.Windows.Forms;
                namespace Test;
                class BaseForm : Form { protected Button inherited; }
                class External { public static Button Value; }
                partial class Form1 : BaseForm
                {
                    private readonly int customValue = 42;
                    private Button runtimeButton = new Button();
                }
                """),
            new AnalyzerTestSource("Form1.Designer.cs",
                """
                using System.Windows.Forms;
                namespace Test;
                partial class Form1
                {
                    private void InitializeComponent()
                    {
                        inherited = new Button();
                        External.Value = new Button();
                        TabIndex = customValue;
                        Controls.Add(runtimeButton);
                    }
                }
                """));

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<DesignerFileStructureAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Theory]
    [InlineData("System.ComponentModel.Component", true)]
    [InlineData("ActualComponent", true)]
    [InlineData("DerivedComponent", true)]
    [InlineData("System.Windows.Forms.UserControl", true)]
    [InlineData("Other.Component", false)]
    [InlineData("Other.Control", false)]
    public async Task DesignerType_UsesFrameworkIdentity(string baseType, bool expected)
    {
        string condition = expected ? "{|WFO3000:if|}" : "if";
        AnalyzerTestCase testCase = new(
            ReferenceAssemblies.Net.Net90Windows,
            new AnalyzerTestSource("Form1.cs",
                $$"""
                using ActualComponent = System.ComponentModel.Component;
                class DerivedComponent : ActualComponent { }
                namespace Other { class Component { } class Control { } }
                namespace Test { partial class Form1 : {{baseType}} { } }
                """),
            new AnalyzerTestSource("Form1.Designer.cs",
                $$"""
                namespace Test
                {
                    partial class Form1
                    {
                        private void InitializeComponent()
                        {
                            {{condition}} (true) { }
                        }
                    }
                }
                """));

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<InitializeComponentAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task InitializeComponent_MethodNamedNameof_NoDiagnostic()
    {
        AnalyzerTestCase testCase = CreateTestCase(
            """
            namespace Test;
            partial class Form1
            {
                private void InitializeComponent() { Text = nameof(1); }
            }
            """);
        testCase.Sources.Add(new AnalyzerTestSource("Methods.cs",
            """
            namespace Test;
            partial class Form1 { private string nameof(int value) => value.ToString(); }
            """));

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<InitializeComponentAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task InitializeComponent_AsyncAwait_ReportsDiagnostic()
    {
        AnalyzerTestCase testCase = CreateTestCase(
            """
            namespace Test;
            partial class Form1
            {
                private async void InitializeComponent()
                {
                    {|WFO3000:await|} System.Threading.Tasks.Task.CompletedTask;
                }
            }
            """);

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<InitializeComponentAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task DesignerFile_NestedCollectionExpression_ReportsOnce()
    {
        AnalyzerTestCase testCase = CreateTestCase(
            """
            namespace Test;
            partial class Form1
            {
                private void InitializeComponent() { }
                partial class {|WFO3001:Nested|}
                {
                    private void InitializeComponent() { int[] values = {|WFO3004:[|}1, 2]; }
                }
            }
            """);
        testCase.Sources.Add(new AnalyzerTestSource("Nested.cs",
            """
            namespace Test;
            partial class Form1
            {
                partial class Nested : System.Windows.Forms.Form { }
            }
            """));

        await AnalyzerTestFactory.CreateCSharpAnalyzerTest<DesignerFileStructureAnalyzer>(testCase)
            .RunAsync(TestContext.Current.CancellationToken);
    }
}
