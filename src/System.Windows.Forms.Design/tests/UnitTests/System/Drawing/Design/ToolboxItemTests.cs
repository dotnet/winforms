// Licensed to the .NET Foundation under one or more agreements.	
// The .NET Foundation licenses this file to you under the MIT license.	
// See the LICENSE file in the project root for more information.	

 using Xunit;	

 namespace System.Drawing.Design.Tests	
{	
    public class ToolboxItemTests	
    {	
        [Fact]
        public void ToolBoxItem_Constructor()
        {
            var type = typeof(Bitmap);
            ToolboxItem underTest = new ToolboxItem(type);

            Assert.Equal("System.Drawing.Bitmap", underTest.TypeName);
            Assert.Equal("Bitmap", underTest.DisplayName);
            Assert.Equal("Bitmap", underTest.ToString());
            Assert.False(underTest.Locked);
            Assert.True(underTest.GetType().IsSerializable);
        }

        [Fact]
        public void ToolBoxItem_Lock()
        {
            var type = typeof(Bitmap);
            ToolboxItem underTest = new ToolboxItem(type);

            underTest.Lock();
            Assert.True(underTest.Locked);
        }

        [Fact]
        public void ToolBoxItem_Equals()
        {
            var type = typeof(Bitmap);
            ToolboxItem underTest = new ToolboxItem(type);
            ToolboxItem underTest2 = new ToolboxItem(type);
            ToolboxItem underTest3 = new ToolboxItem(typeof(string));

            Assert.True(underTest.Equals(underTest2));
            Assert.False(underTest.Equals(underTest3));
        }
    }
}
