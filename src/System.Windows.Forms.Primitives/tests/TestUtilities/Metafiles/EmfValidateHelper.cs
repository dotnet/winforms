// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Text;
using System.Windows.Forms.Metafiles;

namespace System.Windows.Forms.misc.Tests;

public class EmfValidateHelper
{
    /// <summary>
    ///  Helps immensely with debugging metafile tests.
    /// </summary>
    /// <param name="emf">The EmfScope instance to extract log information from.</param>
    /// <param name="methodName">
    ///  The name of the calling method. Use <see cref="MethodBase.GetCurrentMethod()"/> and property <see cref="MemberInfo.Name"/>.
    /// </param>
    /// <param name="parameters">Optional parameters to include in the log information.</param>
    internal void LogEmfValidateToFile(EmfScope emf, string methodName, params object[] parameters)
    {
        ArgumentNullException.ThrowIfNull(emf, nameof(emf));

        string timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm");

        StringBuilder sb = new();
        sb.Append($"# {timestamp}. Parameters: ");
        sb.AppendJoin(", ", parameters);
        sb.AppendLine("\r\n\r\n```c");
        sb.AppendLine($"{emf.RecordsToString()}```\r\n");

        File.AppendAllText(@$"c:\temp\{methodName}.md", sb.ToString());
    }
}
