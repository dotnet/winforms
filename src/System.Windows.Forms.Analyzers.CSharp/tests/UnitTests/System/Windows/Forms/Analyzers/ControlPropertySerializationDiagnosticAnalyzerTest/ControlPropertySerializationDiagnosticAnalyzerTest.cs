// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Xunit;

using VerifyCS = System.Windows.Forms.Analyzers.Tests
    .CSharpCodeFixVerifier<
        System.Windows.Forms.CSharp.Analyzers.ControlPropertySerialization.ControlPropertySerializationDiagnosticAnalyzer,
        System.Windows.Forms.CSharp.CodeFixes.AddDesignerSerializationVisibility.AddDesignerSerializationVisibilityCodeFixProvider>;

namespace System.Windows.Forms.Analyzers.CSharp.Tests;

public class ControlPropertySerializationDiagnosticAnalyzer
{
    private const string ProblematicCode = """

        namespace CSharpControls;

        public class ScalableControlNoSerializationConfiguration : Control
        {
            public float ScaleFactor { get; set; } = 1.0f;

            public SizeF ScaledSize { get; set; }

            public PointF ScaledLocation { get; set; }
        }

        """;

    private const string FixedCode = """

        using System.ComponentModel;

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

    [Fact]
    public async Task Test_CSharp_AddDesignerSerializationVisibility()
    {
        var expected = VerifyCS.Diagnostic(nameof(ControlPropertySerializationDiagnosticAnalyzer));

        await VerifyCS
            .VerifyCodeFixAsync(ProblematicCode, expected, FixedCode);
    }
}
