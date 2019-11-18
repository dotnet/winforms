// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class NumericUpDownAccessibleObjectTests
    {
        public NumericUpDownAccessibleObjectTests()
        {
            Application.ThreadException += (sender, e) => throw new Exception(e.Exception.StackTrace.ToString());
        }

        [WinFormsFact]
        public void NumericUpDownAccessibleObject_Ctor_Default()
        {
            using NumericUpDown numericUpDown = new NumericUpDown();
            AccessibleObject accessibleObject = numericUpDown.AccessibilityObject;
            Assert.NotNull(accessibleObject);
        }

        [WinFormsFact]
        public void NumericUpDownAccessibleObject_NameIsNotEqualControlType()
        {
            using NumericUpDown numericUpDown = new NumericUpDown();
            string accessibleName = numericUpDown.AccessibilityObject.Name;
            string controlType = SR.SpinnerAccessibleName;

            // Mas requires us to have no same AccessibleName and LocalizedControlType, 
            // please see the requirement (Section 508 502.3.1). 
            // So we need to check if these properties are not equal.
            // In this specific case, we can't have some positive Assert.
            // ToLower method used to be case insensitive.
            Assert.NotEqual(accessibleName.ToLower(), controlType.ToLower());
        }
    }
}
