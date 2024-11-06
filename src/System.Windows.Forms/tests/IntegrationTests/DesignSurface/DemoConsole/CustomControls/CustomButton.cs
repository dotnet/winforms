// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;
using System.ComponentModel;

namespace TestConsole;

[Designer(typeof(CustomButtonDesigner), typeof(System.ComponentModel.Design.IDesigner))]
public class CustomButton : Button
{
}
