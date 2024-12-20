' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Namespace Microsoft.VisualBasic.ApplicationServices

    ''' <summary>
    ''' Indicates which condition should cause a Windows Forms application to shut down.
    ''' </summary>
    ''' <remarks>Any changes to this enum must be reflected in ValidateShutdownModeEnumValue().</remarks>
    Public Enum ShutdownMode
        AfterMainFormCloses
        AfterAllFormsClose
    End Enum

End Namespace
