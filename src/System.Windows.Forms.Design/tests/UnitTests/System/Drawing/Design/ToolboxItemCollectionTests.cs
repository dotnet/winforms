// Licensed to the .NET Foundation under one or more agreements.	
// The .NET Foundation licenses this file to you under the MIT license.	
// See the LICENSE file in the project root for more information.	

 using Xunit;	

 namespace System.Drawing.Design.Tests	
{	
    public class ToolboxItemCollectionTests	
    {
        [Fact]
        public void ToolboxItemCollection_Creation()
        {
            ToolboxItem item1 = new ToolboxItem(typeof(Bitmap));
            ToolboxItem item2 = new ToolboxItem(typeof(string));

            ToolboxItem[] tools = { item1, item2 };
            ToolboxItemCollection underTest = new ToolboxItemCollection(tools);

            Assert.True(underTest.Contains(item1));
            Assert.Equal(item1, underTest[0]);
            Assert.Equal(1, underTest.IndexOf(item2));

            ToolboxItem[] tools2 = new ToolboxItem[2];
            underTest.CopyTo(tools2, 0);
            Assert.Equal(item1, tools[0]);
            Assert.Equal(item2, tools[1]);
        }
    }	
}
