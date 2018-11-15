// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.IO;

    // 




    public interface IFileReaderService {
        /// <include file='doc\IFileReaderService.uex' path='docs/doc[@for="IFileReaderService.OpenFileFromSource"]/*' />
        Stream OpenFileFromSource(string relativePath);
    }
}
