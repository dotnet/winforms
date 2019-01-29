// Licensed to the .NET Foundation under one or more agreements.	
// The .NET Foundation licenses this file to you under the MIT license.	
// See the LICENSE file in the project root for more information.	

 using Xunit;	

 namespace System.Drawing.Design.Tests	
{	
    public class ToolboxItemCollectionTests	
    {	
        private ToolboxItemCollection CreateToolboxItemCollection()	
        {	
            ToolboxItem[]  tools = { new ToolboxItem(), new ToolboxItem() };	

             return new ToolboxItemCollection(tools);	
        }	

        [Fact]
        public void Contains_StateUnderTest_ExpectedBehavior()	
        {	
            var unitUnderTest = CreateToolboxItemCollection();	
            ToolboxItem value = unitUnderTest[0];	

             var result = unitUnderTest.Contains( value);	

             Assert.True(result);	
        }	

        [Fact]
        public void IndexOf_StateUnderTest_ExpectedBehavior()	
        {	
            var unitUnderTest = CreateToolboxItemCollection();	
            ToolboxItem value = unitUnderTest[0];	

             var result = unitUnderTest.IndexOf(	 value);	

             Assert.Equal(0,result);	
        }	
    }	
}
