// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Represents the HTML 1.0 Help engine.
    /// </devdoc>
    public enum HelpNavigator
    {
        /// <devdoc>
        /// Displays the topic referenced by the topic referenced by the specified
        /// Url. This field is constant.
        /// </devdoc>
        Topic = unchecked((int)0x80000001),

        /// <devdoc>
        /// Displays the contents of the HTML 1.0 Help file. This field is constant.
        /// </devdoc>
        TableOfContents = unchecked((int)0x80000002),

        /// <devdoc>
        /// Displays the index of a specified Url. This field is constant.
        /// </devdoc>
        Index = unchecked((int)0x80000003),

        /// <devdoc>
        /// Displays the search page of a specified Url. This field is constant.
        /// </devdoc>
        Find = unchecked((int)0x80000004),

        /// <devdoc>
        /// Displays the topic referenced by the topic referenced by the specified
        /// Url. This field is constant.
        /// </devdoc>
        AssociateIndex = unchecked((int)0x80000005),

        /// <devdoc>
        /// Displays the topic referenced by the topic referenced by the specified
        /// Url. This field is constant.
        /// </devdoc>
        KeywordIndex = unchecked((int)0x80000006),

        /// <devdoc>
        /// Displays the topic referenced by the topic ID This field is constant.
        /// </devdoc>
        TopicId = unchecked((int)0x80000007)
    }
}
