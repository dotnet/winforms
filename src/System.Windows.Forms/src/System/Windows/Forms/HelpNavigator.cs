// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;

    /// <include file='doc\HelpNavigator.uex' path='docs/doc[@for="HelpNavigator"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Represents the HTML 1.0 Help engine.
    ///    </para>
    /// </devdoc>
    public enum HelpNavigator {

        /// <include file='doc\HelpNavigator.uex' path='docs/doc[@for="HelpNavigator.Topic"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays the topic referenced by the topic referenced by
        ///       the specified Url.
        ///       This field is
        ///       constant.
        ///    </para>
        /// </devdoc>
        Topic = unchecked((int)0x80000001),
        /// <include file='doc\HelpNavigator.uex' path='docs/doc[@for="HelpNavigator.TableOfContents"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays the contents of the 
        ///       HTML 1.0 Help file. This field is constant.
        ///    </para>
        /// </devdoc>
        TableOfContents = unchecked((int)0x80000002),
        /// <include file='doc\HelpNavigator.uex' path='docs/doc[@for="HelpNavigator.Index"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays the index of a specified
        ///       Url. This field is constant.
        ///    </para>
        /// </devdoc>
        Index = unchecked((int)0x80000003),
        /// <include file='doc\HelpNavigator.uex' path='docs/doc[@for="HelpNavigator.Find"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays the search page
        ///       of a specified Url. This field is constant.
        ///    </para>
        /// </devdoc>
        Find = unchecked((int)0x80000004),
        /// <include file='doc\HelpNavigator.uex' path='docs/doc[@for="HelpNavigator.AssociateIndex"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays the topic referenced by the topic referenced by
        ///       the specified Url.
        ///       This field is
        ///       constant.
        ///    </para>
        /// </devdoc>
        AssociateIndex = unchecked((int)0x80000005),
        /// <include file='doc\HelpNavigator.uex' path='docs/doc[@for="HelpNavigator.KeywordIndex"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays the topic referenced by the topic referenced by
        ///       the specified Url.
        ///       This field is
        ///       constant.
        ///    </para>
        /// </devdoc>
        KeywordIndex = unchecked((int)0x80000006),
        /// <include file='doc\HelpNavigator.uex' path='docs/doc[@for="HelpNavigator.TopicId"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays the topic referenced by the topic ID
        ///       This field is 
        ///       constant.
        ///    </para>
        /// </devdoc>
        TopicId = unchecked((int)0x80000007)


    }
}
