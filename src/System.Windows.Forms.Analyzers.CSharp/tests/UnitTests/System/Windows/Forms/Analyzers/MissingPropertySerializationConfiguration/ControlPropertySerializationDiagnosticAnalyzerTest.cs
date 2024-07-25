// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.CSharp.Analyzers.MissingPropertySerializationConfiguration;
using System.Windows.Forms.CSharp.CodeFixes.AddDesignerSerializationVisibility;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace System.Windows.Forms.Analyzers.Test;

public class ControlPropertySerializationDiagnosticAnalyzerTest
{
    private const string ProblematicCode = """
        using System.Drawing;
        using System.Windows.Forms;

        namespace CSharpControls;

        public static class Program
        {
            public static void Main()
            {
                var control = new ScalableControl();

         // We deliberately format this weirdly, to make sure we only format code our code fix touches.
         control.ScaleFactor = 1.5f;
            control.ScaledSize = new SizeF(100, 100);
               control.ScaledLocation = new PointF(10, 10);
            }
        }

        // We are writing the fully-qualified name here to make sure, the Simplifier doesn't remove it,
        // since this is nothing our code fix touches.
        public class ScalableControl : System.Windows.Forms.Control
        {
            public float [|ScaleFactor|] { get; set; } = 1.0f;

            public SizeF [|ScaledSize|] { get; set; }

            public PointF [|ScaledLocation|] { get; set; }
        }

        """;

    private const string CorrectCode = """
        using System.ComponentModel;
        using System.Drawing;
        using System.Windows.Forms;

        namespace CSharpControls;

        public static class Program
        {
            public static void Main()
            {
                var control = new ScalableControl();
        
                control.ScaleFactor = 1.5f;
                control.ScaledSize = new SizeF(100, 100);
                control.ScaledLocation = new PointF(10, 10);
            }
        }
        
        public class ScalableControl : Control
        {
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public float ScaleFactor { get; set; } = 1.0f;

            [DefaultValue(typeof(SizeF), "0,0")]
            public SizeF ScaledSize { get; set; }

            public PointF ScaledLocation { get; set; }
            private bool ShouldSerializeScaledLocation() => this.ScaledLocation != PointF.Empty;
        }
        
        """;

    private const string FixedCode = """
        using System.ComponentModel;
        using System.Drawing;
        using System.Windows.Forms;

        namespace CSharpControls;

        public static class Program
        {
            public static void Main()
            {
                var control = new ScalableControl();

         // We deliberately format this weirdly, to make sure we only format code our code fix touches.
         control.ScaleFactor = 1.5f;
            control.ScaledSize = new SizeF(100, 100);
               control.ScaledLocation = new PointF(10, 10);
            }
        }

        // We are writing the fully-qualified name here to make sure, the Simplifier doesn't remove it,
        // since this is nothing our code fix touches.
        public class ScalableControl : System.Windows.Forms.Control
        {
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public float ScaleFactor { get; set; } = 1.0f;

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public SizeF ScaledSize { get; set; }

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public PointF ScaledLocation { get; set; }
        }
        
        """;

    // We are testing the analyzer with all versions of the .NET SDK from 6.0 on.
    public static IEnumerable<object[]> GetReferenceAssemblies()
    {
        yield return [ReferenceAssemblies.Net.Net60Windows];
        yield return [ReferenceAssemblies.Net.Net70Windows];
        yield return [ReferenceAssemblies.Net.Net80Windows];
        yield return [ReferenceAssemblies.Net.Net90Windows];
    }

    [Theory]
    [MemberData(nameof(GetReferenceAssemblies))]
    public async Task CS_ControlPropertySerializationConfigurationAnalyzer(ReferenceAssemblies referenceAssemblies)
    {
        var context = new CSharpAnalyzerTest
            <MissingPropertySerializationConfigurationAnalyzer,
             DefaultVerifier>
        {
            TestCode = CorrectCode,
            TestState =
                {
                    OutputKind = OutputKind.WindowsApplication,
                },
            ReferenceAssemblies = referenceAssemblies
        };

        await context.RunAsync();
    }

    [Theory]
    [MemberData(nameof(GetReferenceAssemblies))]
    public async Task CS_MissingControlPropertySerializationConfigurationAnalyzer(ReferenceAssemblies referenceAssemblies)
    {
        var context = new CSharpAnalyzerTest
            <MissingPropertySerializationConfigurationAnalyzer,
             DefaultVerifier>
        {
            TestCode = ProblematicCode,
            TestState =
                {
                    OutputKind = OutputKind.WindowsApplication,
                },
            ReferenceAssemblies = referenceAssemblies
        };

        await context.RunAsync();
    }

    [Theory]
    [MemberData(nameof(GetReferenceAssemblies))]
    public async Task CS_AddDesignerSerializationVisibilityCodeFix(ReferenceAssemblies referenceAssemblies)
    {
        var context = new CSharpCodeFixTest
            <MissingPropertySerializationConfigurationAnalyzer,
             AddDesignerSerializationVisibilityCodeFixProvider,
             DefaultVerifier>
        {
            TestCode = ProblematicCode,
            FixedCode = FixedCode,
            TestState =
                {
                    OutputKind = OutputKind.WindowsApplication,
                },
            ReferenceAssemblies = referenceAssemblies,
            NumberOfFixAllIterations = 2
        };

        await context.RunAsync();
    }
}
