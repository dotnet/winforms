// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;

    internal interface ISupportOleDropSource {
	
        /// <include file='doc\ISupportOleDropSource.uex' path='docs/doc[@for=ISupportOleDropSource.OnQueryContinueDrag]/*'/ />
        /// <devdoc>
        /// Summary of OnQueryContinueDrag.
        /// </devdoc>
        /// <param name=qcdevent></param>	
        void OnQueryContinueDrag(QueryContinueDragEventArgs qcdevent);
	
        /// <include file='doc\ISupportOleDropSource.uex' path='docs/doc[@for=ISupportOleDropSource.OnGiveFeedback]/*'/ />
        /// <devdoc>
        /// Summary of OnGiveFeedback.
        /// </devdoc>
        /// <param name=gfbevent></param>	
        void OnGiveFeedback(GiveFeedbackEventArgs gfbevent);
    }
}
