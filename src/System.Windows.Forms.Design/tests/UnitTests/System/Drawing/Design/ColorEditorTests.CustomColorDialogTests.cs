// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Windows.Forms;
using Xunit;

namespace System.Drawing.Design.Tests
{
    public partial class ColorEditor_CustomColorDialogTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void CustomColorDialog_Ctor_Default()
        {
            Type? typeCustomColorDialog = typeof(ColorEditor).Assembly.GetTypes().SingleOrDefault(t => t.Name == "CustomColorDialog");
            Assert.NotNull(typeCustomColorDialog);

            using ColorDialog dialog = (ColorDialog)Activator.CreateInstance(typeCustomColorDialog!)!;
            Assert.NotNull(dialog);
        }
    }
}
