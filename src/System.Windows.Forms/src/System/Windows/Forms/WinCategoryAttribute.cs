// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    
    using System;
    using System.ComponentModel;   
    using System.Diagnostics;

    /// <include file='doc\WinCategoryAttribute.uex' path='docs/doc[@for="WinCategoryAttribute"]/*' />
    /// <internalonly/>
    /// <devdoc>
    ///    <para>
    ///       CategoryAttribute that can access WinForms localized strings.
    ///    </para>
    /// </devdoc>
    [AttributeUsage(AttributeTargets.All)]
    internal sealed class WinCategoryAttribute : CategoryAttribute {

        /// <include file='doc\WinCategoryAttribute.uex' path='docs/doc[@for="WinCategoryAttribute.WinCategoryAttribute"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.ComponentModel.CategoryAttribute'/> class.
        ///    </para>
        /// </devdoc>
        public WinCategoryAttribute(string category) : base(category) {
        }

        /// <include file='doc\WinCategoryAttribute.uex' path='docs/doc[@for="WinCategoryAttribute.GetLocalizedString"]/*' />
        /// <devdoc>
        ///     This method is called the first time the category property
        ///     is accessed.  It provides a way to lookup a localized string for
        ///     the given category.  Classes may override this to add their
        ///     own localized names to categories.  If a localized string is
        ///     available for the given value, the method should return it.
        ///     Otherwise, it should return null.
        /// </devdoc>
        protected override string GetLocalizedString(string value) {
            string localizedValue = base.GetLocalizedString(value);
            if (localizedValue == null) {
                localizedValue = (string)SR.GetObject("WinFormsCategory" + value);
            }
            // This attribute is internal, and we should never have a missing resource string.
            //
            Debug.Assert(localizedValue != null, "All Windows Forms category attributes should have localized strings.  Category '" + value + "' not found.");
            return localizedValue;
        }
    }
}
