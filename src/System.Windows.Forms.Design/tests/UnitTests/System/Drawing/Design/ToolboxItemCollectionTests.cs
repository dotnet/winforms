// Licensed to the .NET Foundation under one or more agreements.	
// The .NET Foundation licenses this file to you under the MIT license.	
// See the LICENSE file in the project root for more information.	

 using Xunit;	

 namespace System.Drawing.Design.Tests	
{	
    public class ToolboxItemCollectionTests	
    {
        [Fact]
        public void ToolboxItemCollection_Creation_TypeOfBitmap()
        {
            ToolboxItem item = new ToolboxItem(typeof(Bitmap));

            ToolboxItem[] tools = { item };
            ToolboxItemCollection underTest = new ToolboxItemCollection(tools);

            Assert.True(underTest.Contains(item));
            Assert.Equal(item, underTest[0]);
            Assert.Equal(0, underTest.IndexOf(item));
        }

    }	
}
