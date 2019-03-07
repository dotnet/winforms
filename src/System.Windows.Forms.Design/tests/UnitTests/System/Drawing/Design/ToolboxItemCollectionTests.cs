﻿// Licensed to the .NET Foundation under one or more agreements.	
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
        public void TestToolboxItemCollection_Throw_NotImplemented_Exception()
        {
            Assert.Throws<System.NotImplementedException>(() => CreateToolboxItemCollection());         
        }
    }	
}
