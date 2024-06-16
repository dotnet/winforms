// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Analyzers.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
    System.Windows.Forms.CSharp.Analyzers.ControlPropertySerialization.ControlPropertySerializationDiagnosticAnalyzer,
    System.Windows.Forms.CSharp.CodeFixes.AddDesignerSerializationVisibility.AddDesignerSerializationVisibilityCodeFixProvider,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace System.Windows.Forms.Analyzers.CSharp.Tests;

public class ControlPropertySerializationDiagnosticAnalyzer
{
    private const string ProblematicCode = """
        using System.Drawing;
        using System.Windows.Forms;

        namespace CSharpControls;

        public class ScalableControlNoSerializationConfiguration : Control
        {
            public float ScaleFactor { get; set; } = 1.0f;

            public SizeF ScaledSize { get; set; }

            public PointF ScaledLocation { get; set; }
        }

        """;

    private const string CorrectCode = """
        using System.ComponentModel;
        using System.Drawing;
        using System.Windows.Forms;

        namespace CSharpControls;

        public class ScalableControlResolved : Control
        {
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public float ScaleFactor { get; set; } = 1.0f;

            [DefaultValue(typeof(SizeF), "0,0")]
            public SizeF ScaledSize { get; set; }

            public PointF ScaledLocation { get; set; }
            private bool ShouldSerializeScaledSize() => this.ScaledSize != SizeF.Empty;
        }
        
        """;

    private const string FixedCode = """
        using System.ComponentModel;
        using System.Drawing;
        using System.Windows.Forms;
        
        namespace CSharpControls;

        public class ScalableControlResolved : Control
        {
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public float ScaleFactor { get; set; } = 1.0f;

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public SizeF ScaledSize { get; set; }

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public PointF ScaledLocation { get; set; }
        }
        
        """;

    [Fact]
    public async Task Test_CSharp_AddDesignerSerializationVisibility()
    {
        DiagnosticDescriptor diagnostic = new(
            DiagnosticIDs.ControlPropertySerialization,
            "Control property serialization",
            "Control property serialization",
            "ControlPropertySerialization",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        var expected = VerifyCS.Diagnostic(diagnostic);
        var expected1 = expected.WithLocation(new LinePosition(7, 17));

        await VerifyCS.VerifyAnalyzerAsync(ProblematicCode, expected1, expected, expected);
        await VerifyCS.VerifyAnalyzerAsync(CorrectCode);

        await VerifyCS
            .VerifyCodeFixAsync(ProblematicCode, expected, FixedCode);
    }
}
