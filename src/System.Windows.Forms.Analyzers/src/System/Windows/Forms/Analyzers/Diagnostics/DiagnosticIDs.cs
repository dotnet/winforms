// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Analyzers.Diagnostics;

internal static class DiagnosticIDs
{
    public const string UrlFormat = "https://aka.ms/winforms-warnings/{0}";

    // Application Configuration, number group 0001+
    public const string UnsupportedProjectType = "WFO0001";
    public const string PropertyCantBeSetToValue = "WFO0002";
    public const string MigrateHighDpiSettings = "WFO0003";

    // WinForms Security, number group 1000+
    public const string MissingPropertySerializationConfiguration = "WFO1000";
    public const string ImplementITypedDataObject = "WFO1001";

    // WinForms Usage, number group 2000+
    public const string AvoidPassingFuncReturningTaskWithoutCancellationToken = "WFO2001";
    public const string UnsupportedInitializeComponentCode = "WFO2002";
    public const string UnexpectedDesignerMember = "WFO2003";
    public const string DesignerFieldPlacement = "WFO2004";
    public const string DesignerEventOrDelegate = "WFO2005";
    public const string DesignerCollectionExpression = "WFO2006";
    public const string UnsupportedNameOfExpression = "WFO2007";
    public const string UnsupportedConditionalExpression = "WFO2008";
    public const string UnsupportedNullCoalescingExpression = "WFO2009";
    public const string UnsupportedNullConditionalExpression = "WFO2010";
    public const string UnsupportedInterpolatedString = "WFO2011";
    public const string UnsupportedAnonymousFunction = "WFO2012";
    public const string PropertyAllocatesNewInstance = "WFO2013";
    public const string DesignerMissingWithEvents = "WFO2014";
    public const string DesignerAddHandler = "WFO2015";

    // Experimental, number group 5000+
    public const string ExperimentalDarkMode = "WFO5001";
    public const string ExperimentalAsync = "WFO5002";
    public const string ExperimentalAsyncDropTarget = "WFO5003";
}
