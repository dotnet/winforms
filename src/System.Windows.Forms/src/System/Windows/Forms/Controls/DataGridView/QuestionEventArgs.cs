// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class QuestionEventArgs : EventArgs
{
    public QuestionEventArgs()
    {
    }

    public QuestionEventArgs(bool response)
    {
        Response = response;
    }

    public bool Response { get; set; }
}
