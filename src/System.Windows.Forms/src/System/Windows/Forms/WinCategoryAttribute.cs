﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    
    using System;
    using System.ComponentModel;   
    using System.Diagnostics;

    /// <devdoc>
    ///    <para>
    ///       CategoryAttribute that can access WinForms localized strings.
    ///    </para>
    /// </devdoc>
    [AttributeUsage(AttributeTargets.All)]
    internal sealed class WinCategoryAttribute : CategoryAttribute {

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.ComponentModel.CategoryAttribute'/> class.
        ///    </para>
        /// </devdoc>
        public WinCategoryAttribute(string category) : base(category) {
        }

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
                localizedValue = (string)GetSRObject("WinFormsCategory" + value);
            }
            // This attribute is internal, and we should never have a missing resource string.
            //
            Debug.Assert(localizedValue != null, "All Windows Forms category attributes should have localized strings.  Category '" + value + "' not found.");
            return localizedValue;
        }

        private static object GetSRObject(string name)
        {
            object resourceObject = null;
            try { resourceObject = SR.ResourceManager.GetObject(name); }
            catch (System.Resources.MissingManifestResourceException) { }
            return resourceObject;
        }
    }
}
