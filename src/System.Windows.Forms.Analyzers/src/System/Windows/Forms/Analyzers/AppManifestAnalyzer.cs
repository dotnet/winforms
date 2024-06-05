// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.Windows.Forms.Analyzers.Diagnostics;
using System.Xml;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace System.Windows.Forms.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
internal partial class AppManifestAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(
            DiagnosticDescriptors.s_migrateHighDpiSettings_CSharp,
            DiagnosticDescriptors.s_migrateHighDpiSettings_VB);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze
            | GeneratedCodeAnalysisFlags.ReportDiagnostics);

        context.EnableConcurrentExecution();
        context.RegisterAdditionalFileAction(AdditionalFileAction);
    }

    private void AdditionalFileAction(AdditionalFileAnalysisContext context)
    {
        if (context.AdditionalFile.Path.EndsWith(".manifest", StringComparison.OrdinalIgnoreCase))
        {
            VerifyAppManifest(context, context.AdditionalFile);
        }
    }

    private static void VerifyAppManifest(AdditionalFileAnalysisContext context, AdditionalText appManifest)
    {
        SourceText? appManifestXml = appManifest.GetText(context.CancellationToken);
        if (appManifestXml is null)
        {
            return;
        }

        // If the manifest file is corrupt - let the build fail
        XmlDocument doc = new();
        try
        {
            doc.LoadXml(appManifestXml.ToString());
        }
        catch
        {
            // Invalid xml, don't care
            return;
        }

        XmlNamespaceManager nsMgr = new(doc.NameTable);
        nsMgr.AddNamespace("v1", "urn:schemas-microsoft-com:asm.v1");
        nsMgr.AddNamespace("v3", "urn:schemas-microsoft-com:asm.v3");
        nsMgr.AddNamespace("v3ws", "http://schemas.microsoft.com/SMI/2005/WindowsSettings");

        if (doc.DocumentElement.SelectSingleNode("//v3:application/v3:windowsSettings/v3ws:dpiAware", nsMgr) is not null)
        {
            switch (context.Compilation.Language)
            {
                case LanguageNames.CSharp:
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.s_migrateHighDpiSettings_CSharp,
                        Location.None,
                        appManifest.Path,
                        ApplicationConfig.PropertyNameCSharp.HighDpiMode));
                    break;

                case LanguageNames.VisualBasic:
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.s_migrateHighDpiSettings_VB,
                        Location.None,
                        appManifest.Path,
                        ApplicationConfig.PropertyNameVisualBasic.HighDpiMode));
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
