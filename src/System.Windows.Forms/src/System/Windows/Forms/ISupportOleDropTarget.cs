// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
        using System;

        public interface IDropTarget {

            /// <devdoc>
            /// Summary of OnDragEnter.
            /// </devdoc>
            /// <param name=e></param>	
            void OnDragEnter(DragEventArgs e);
            /// <devdoc>
            /// Summary of OnDragLeave.
            /// </devdoc>
            /// <param name=e></param>	
            void OnDragLeave(System.EventArgs e);
            /// <devdoc>
            /// Summary of OnDragDrop.
            /// </devdoc>
            /// <param name=e></param>	
            void OnDragDrop(DragEventArgs e);
            /// <devdoc>
            /// Summary of OnDragOver.
            /// </devdoc>
            /// <param name=e></param>	
            void OnDragOver(DragEventArgs e);
        }
    }
