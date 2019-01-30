// Licensed to the .NET Foundation under one or more agreements.	
// The .NET Foundation licenses this file to you under the MIT license.	
// See the LICENSE file in the project root for more information.	

 using Xunit;	

 namespace System.Drawing.Design.Tests	
{	
    public class ToolboxItemTests	
    {	
        [Fact]
        public void TestToolBoxItem_Non_Default_Throw_NotImplemented_Exception()
        {	
            var type = typeof(Bitmap);
            bool rightExceptionThrown = false;
            try
            {
                var unitUnderTest = new ToolboxItem(type);
            }
            catch (System.NotImplementedException)
            {
                rightExceptionThrown = true;
            }
             Assert.True(rightExceptionThrown);
        }	
    }	
}
