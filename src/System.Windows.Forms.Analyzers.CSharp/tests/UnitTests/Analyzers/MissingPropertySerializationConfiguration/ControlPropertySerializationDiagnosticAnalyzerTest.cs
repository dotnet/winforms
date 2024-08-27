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
    private const string GlobalUsingCode = """
        global using System.Drawing;
        global using System.Windows.Forms;
        """;

    private const string ProblematicCode = """
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
            private SizeF _scaleSize = new SizeF(3, 14);

            /// <Summary>
            ///  Sets or gets the scaled size of some foo bar thing.
            /// </Summary>
            [System.ComponentModel.Description("Sets or gets the scaled size of some foo bar thing.")]
            public SizeF [|ScaledSize|]
            {
                get => _scaleSize;
                set => _scaleSize = value;
            }

            public float [|ScaleFactor|] { get; set; } = 1.0f;

            /// <Summary>
            ///  Sets or gets the scaled location of some foo bar thing.
            /// </Summary>
            public PointF [|ScaledLocation|] { get; set; }
        }

        """;

    private const string CorrectCode = """
        using System.ComponentModel;

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
            private SizeF _scaleSize = new SizeF(3, 14);
        
            /// <Summary>
            ///  Sets or gets the scaled size of some foo bar thing.
            /// </Summary>
            [System.ComponentModel.Description("Sets or gets the scaled size of some foo bar thing.")]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public SizeF ScaledSize
            {
                get => _scaleSize;
                set => _scaleSize = value;
            }
        
            [DefaultValue(1.0f)]
            public float ScaleFactor { get; set; } = 1.0f;
        
            /// <Summary>
            ///  Sets or gets the scaled location of some foo bar thing.
            /// </Summary>
            public PointF ScaledLocation { get; set; }

            private bool ShouldSerializeScaledLocation() => false;
        }
                
        """;

    private const string FixedCode = """
        using System.ComponentModel;

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
            private SizeF _scaleSize = new SizeF(3, 14);
        
            /// <Summary>
            ///  Sets or gets the scaled size of some foo bar thing.
            /// </Summary>
            [System.ComponentModel.Description("Sets or gets the scaled size of some foo bar thing.")]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public SizeF ScaledSize
            {
                get => _scaleSize;
                set => _scaleSize = value;
            }
        
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public float ScaleFactor { get; set; } = 1.0f;
        
            /// <Summary>
            ///  Sets or gets the scaled location of some foo bar thing.
            /// </Summary>
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
    public async Task CS_ControlPropertySerializationConfigurationDiagnosticsEngage(ReferenceAssemblies referenceAssemblies)
    {
        var context = new CSharpAnalyzerTest
            <MissingPropertySerializationConfigurationAnalyzer,
             DefaultVerifier>
        {
            // Note: The ProblematicCode includes the expected Diagnostic's span in the areas
            // where the code is enclosed in limiting characters ("[|...|]"),
            // like `public SizeF [|ScaledSize|]`.
            TestCode = ProblematicCode,
            TestState =
                {
                    OutputKind = OutputKind.WindowsApplication,
                },
            ReferenceAssemblies = referenceAssemblies
        };

        context.TestState.Sources.Add(GlobalUsingCode);

        await context.RunAsync();
    }

    [Theory]
    [MemberData(nameof(GetReferenceAssemblies))]
    public async Task CS_ControlPropertySerializationConfigurationDiagnosticPass(ReferenceAssemblies referenceAssemblies)
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

        context.TestState.Sources.Add(GlobalUsingCode);

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
                    Sources = { GlobalUsingCode }
                },
            ReferenceAssemblies = referenceAssemblies,
            NumberOfFixAllIterations = 2,
            FixedState =
                {
                    Sources = { GlobalUsingCode }
                },
        };

        await context.RunAsync();
    }
}
