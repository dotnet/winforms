// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;
using Xunit;

namespace System.Windows.Forms.Design.Tests
{
    public class DesignerActionListsChangedEventArgsTests
    {
        [Fact]
        public void DesignerActionListsChangedEventArgs_Constructor()
        {
            var button = new Button();
            var designerActionListsChangedEventArgs = new DesignerActionListsChangedEventArgs(button, 
                DesignerActionListsChangedType.ActionListsAdded, new DesignerActionListCollection());

            Assert.NotNull(designerActionListsChangedEventArgs);

            //Test properties
            Assert.Equal(designerActionListsChangedEventArgs.RelatedObject, button);
            Assert.NotNull(designerActionListsChangedEventArgs.ActionLists);
        }
    }
}
