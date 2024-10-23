' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports Microsoft.CodeAnalysis
Imports System.Windows.Forms.Analyzers.Diagnostics
Imports System.Windows.Forms.Analyzers.VisualBasic.Resources

Friend Module VisualBasicDiagnosticDescriptors

    Public ReadOnly s_missingPropertySerializationConfiguration As New DiagnosticDescriptor(
        id:=DiagnosticIDs.MissingPropertySerializationConfiguration,
        title:=New LocalizableResourceString(
            nameOfLocalizableResource:=NameOf(SR.WFO1000AnalyzerTitle),
            resourceManager:=SR.ResourceManager,
            resourceSource:=GetType(SR)),
        messageFormat:=New LocalizableResourceString(
            nameOfLocalizableResource:=NameOf(SR.WFO1000AnalyzerMessageFormat),
            resourceManager:=SR.ResourceManager,
            resourceSource:=GetType(SR)),
        category:=DiagnosticCategories.WinFormsSecurity,
        defaultSeverity:=DiagnosticSeverity.Error,
        isEnabledByDefault:=True,
        description:=New LocalizableResourceString(
            nameOfLocalizableResource:=NameOf(SR.WFO1000AnalyzerDescription),
            resourceManager:=SR.ResourceManager,
            resourceSource:=GetType(SR)))

    Public ReadOnly s_avoidFuncReturningTaskWithoutCancellationToken As New DiagnosticDescriptor(
        id:=DiagnosticIDs.AvoidPassingFuncReturningTaskWithoutCancellationToken,
        title:=New LocalizableResourceString(
            nameOfLocalizableResource:=NameOf(SR.WFO2001AnalyzerTitle),
            resourceManager:=SR.ResourceManager,
            resourceSource:=GetType(SR)),
        messageFormat:=New LocalizableResourceString(
            nameOfLocalizableResource:=NameOf(SR.WFO2001AnalyzerMessageFormat),
            resourceManager:=SR.ResourceManager,
            resourceSource:=GetType(SR)),
        category:=DiagnosticCategories.WinFormsSecurity,
        defaultSeverity:=DiagnosticSeverity.Warning,
        isEnabledByDefault:=True,
        description:=New LocalizableResourceString(
            nameOfLocalizableResource:=NameOf(SR.WFO2001AnalyzerDescription),
            resourceManager:=SR.ResourceManager,
            resourceSource:=GetType(SR)))

End Module
