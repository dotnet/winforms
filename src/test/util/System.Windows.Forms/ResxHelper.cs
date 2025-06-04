// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.TestUtilities;

public static class ResxHelper
{
    public static string CreateResx(string data = null) =>
    $"""
    <root>
        <resheader name="resmimetype">
            <value>text/microsoft-resx</value>
        </resheader>
        <resheader name="version">
            <value>2.0</value>
        </resheader>
        <resheader name="reader">
            <value>System.Resources.ResXResourceReader</value>
        </resheader>
        <resheader name="writer">
            <value>System.Resources.ResXResourceWriter</value>
        </resheader>
        {data ?? string.Empty}
    </root>
    """;
}
