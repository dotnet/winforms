// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CSharpControls;

using System.Windows.Forms;

public static class Program
{
    public static void Main(string[] args)
    {
        var form = new TestNamespace.MyForm();
        Application.Run(form);
    }
}
