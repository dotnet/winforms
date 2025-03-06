' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports Microsoft.CodeAnalysis
Imports System.Windows.Forms.Analyzers.Diagnostics
Imports System.Windows.Forms.Analyzers.VisualBasic.Resources

Friend Module VisualBasicDiagnosticDescriptors

    Public ReadOnly s_missingPropertySerializationConfiguration As New DiagnosticDescriptor(
            id:=DiagnosticIDs.MissingPropertySerializationConfiguration,
            title:=New LocalizableResourceString(NameOf(SR.WFO1000AnalyzerTitle), SR.ResourceManager, GetType(SR)),
            messageFormat:=New LocalizableResourceString(NameOf(SR.WFO1000AnalyzerMessageFormat), SR.ResourceManager, GetType(SR)),
            category:=DiagnosticCategories.WinFormsSecurity,
            defaultSeverity:=DiagnosticSeverity.Error,
            isEnabledByDefault:=True,
            description:=New LocalizableResourceString(NameOf(SR.WFO1000AnalyzerDescription), SR.ResourceManager, GetType(SR)))

    Public ReadOnly s_implementITypedDataObjectInAdditionToIDataObject As New DiagnosticDescriptor(
            id:=DiagnosticIDs.ImplementITypedDataObject,
            title:=New LocalizableResourceString(NameOf(SR.WFO1001AnalyzerTitle), SR.ResourceManager, GetType(SR)),
            messageFormat:=New LocalizableResourceString(NameOf(SR.WFO1001AnalyzerMessageFormat), SR.ResourceManager, GetType(SR)),
            category:=DiagnosticCategories.WinFormsSecurity,
            defaultSeverity:=DiagnosticSeverity.Warning,
            isEnabledByDefault:=True,
            description:=New LocalizableResourceString(NameOf(SR.WFO1001AnalyzerDescription), SR.ResourceManager, GetType(SR)))

    Public ReadOnly s_avoidFuncReturningTaskWithoutCancellationToken As New DiagnosticDescriptor(
            id:=DiagnosticIDs.AvoidPassingFuncReturningTaskWithoutCancellationToken,
            title:=New LocalizableResourceString(NameOf(SR.WFO2001AnalyzerTitle), SR.ResourceManager, GetType(SR)),
            messageFormat:=New LocalizableResourceString(NameOf(SR.WFO2001AnalyzerMessageFormat), SR.ResourceManager, GetType(SR)),
            category:=DiagnosticCategories.WinFormsSecurity,
            defaultSeverity:=DiagnosticSeverity.Warning,
            isEnabledByDefault:=True,
            description:=New LocalizableResourceString(NameOf(SR.WFO2001AnalyzerDescription), SR.ResourceManager, GetType(SR)))

End Module
