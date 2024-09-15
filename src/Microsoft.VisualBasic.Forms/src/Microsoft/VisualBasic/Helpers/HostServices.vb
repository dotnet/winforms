' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Namespace Microsoft.VisualBasic.CompilerServices

    ''' <summary>
    '''  Information about the current Visual Basic host window.
    '''  This API supports the product infrastructure and is not intended to be used directly from your code.
    ''' </summary>
    <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
    Public NotInheritable Class HostServices

        Private Shared s_host As IVbHost

        ''' <summary>
        '''  A <see cref="IVBHost"/> object that returns a reference to and
        '''  information about the current Visual Basic host window.
        ''' </summary>
        ''' <returns>A cached version of <see cref="IVBHost"/></returns>
        ''' <remarks>
        '''  This class supports the Visual Basic compiler and
        '''  is not intended to be used directly from your code.
        '''</remarks>
        Public Shared Property VBHost() As IVbHost
            Get
                Return s_host
            End Get

            Set(Value As IVbHost)
                s_host = Value
            End Set
        End Property

    End Class
End Namespace
