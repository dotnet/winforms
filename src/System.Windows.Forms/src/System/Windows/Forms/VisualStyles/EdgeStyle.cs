// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.VisualStyles
{
    public enum EdgeStyle
    {
        Raised = 0x0001 | 0x0004,
        Sunken = 0x0002 | 0x0008,
        Etched = 0x0002 | 0x0004,
        Bump = 0x0001 | 0x0008

        //		#define BDR_RAISEDOUTER 0x0001
        //		#define BDR_SUNKENOUTER 0x0002
        //		#define BDR_RAISEDINNER 0x0004
        //		#define BDR_SUNKENINNER 0x0008
        //		#define EDGE_RAISED     (BDR_RAISEDOUTER | BDR_RAISEDINNER)
        //		#define EDGE_SUNKEN     (BDR_SUNKENOUTER | BDR_SUNKENINNER)
        //		#define EDGE_ETCHED     (BDR_SUNKENOUTER | BDR_RAISEDINNER)
        //		#define EDGE_BUMP       (BDR_RAISEDOUTER | BDR_SUNKENINNER)
    }
}
