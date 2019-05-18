﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public enum DataGridViewContentAlignment
    {
        NotSet = 0x000,

        /// <summary>
        /// Content is vertically aligned at the top, and horizontally aligned
        /// on the left.
        /// </devdoc>
        TopLeft = 0x001,
        
        /// <summary>
        /// Content is vertically aligned at the top, and horizontally aligned
        /// at the center.
        /// </devdoc>
        TopCenter = 0x002,
        
        /// <summary>
        /// Content is vertically aligned at the top, and horizontally aligned
        /// on the right.
        /// </devdoc>
        TopRight = 0x004,
        
        /// <summary>
        /// Content is vertically aligned in the middle, and horizontally aligned
        /// on the left.
        /// </devdoc>
        MiddleLeft = 0x010,
        
        /// <summary>
        /// Content is vertically aligned in the middle, and horizontally aligned
        /// at the center.
        /// </devdoc>
        MiddleCenter = 0x020,
        
        /// <summary>
        /// Content is vertically aligned in the middle, and horizontally aligned
        /// on the right.
        /// </devdoc>
        MiddleRight = 0x040,
        
        /// <summary>
        /// Content is vertically aligned at the bottom, and horizontally aligned
        /// on the left.
        /// </devdoc>
        BottomLeft = 0x100,
        
        /// <summary>
        /// Content is vertically aligned at the bottom, and horizontally aligned
        /// at the center.
        /// </devdoc>
        BottomCenter = 0x200,

        /// <summary>
        /// Content is vertically aligned at the bottom, and horizontally aligned
        /// on the right.
        /// </devdoc>
        BottomRight = 0x400,
    }
}
