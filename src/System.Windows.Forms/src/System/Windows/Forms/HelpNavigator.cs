// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents the HTML 1.0 Help engine.
    /// </summary>
    public enum HelpNavigator
    {
        /// <summary>
        ///  Displays the topic referenced by the topic referenced by the specified
        ///  Url. This field is constant.
        /// </summary>
        Topic = unchecked((int)0x80000001),

        /// <summary>
        ///  Displays the contents of the HTML 1.0 Help file. This field is constant.
        /// </summary>
        TableOfContents = unchecked((int)0x80000002),

        /// <summary>
        ///  Displays the index of a specified Url. This field is constant.
        /// </summary>
        Index = unchecked((int)0x80000003),

        /// <summary>
        ///  Displays the search page of a specified Url. This field is constant.
        /// </summary>
        Find = unchecked((int)0x80000004),

        /// <summary>
        ///  Displays the topic referenced by the topic referenced by the specified
        ///  Url. This field is constant.
        /// </summary>
        AssociateIndex = unchecked((int)0x80000005),

        /// <summary>
        ///  Displays the topic referenced by the topic referenced by the specified
        ///  Url. This field is constant.
        /// </summary>
        KeywordIndex = unchecked((int)0x80000006),

        /// <summary>
        ///  Displays the topic referenced by the topic ID This field is constant.
        /// </summary>
        TopicId = unchecked((int)0x80000007)
    }
}
