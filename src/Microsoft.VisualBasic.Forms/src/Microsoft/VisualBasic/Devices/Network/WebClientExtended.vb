' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Net

Namespace Microsoft.VisualBasic.Devices

    ''' <summary>
    '''  Temporary class used to provide WebClient with a timeout property.
    ''' </summary>
    ''' <remarks>This class will be deleted when Timeout is added to WebClient.</remarks>
    Friend NotInheritable Class WebClientExtended
        Inherits WebClient

        ' The Timeout value to be used by WebClient's WebRequest for Downloading or Uploading a file
        Private _timeout As Integer = 100000

        ' Flag used to indicate whether or not we should use passive mode when ftp downloading
        Private _useNonPassiveFtp As Boolean

#Disable Warning BC41004 ' First statement of this 'Sub New' should be an explicit call to 'MyBase.New' or 'MyClass.New' because the constructor in the base class is marked obsolete

        Friend Sub New()
        End Sub

#Enable Warning BC41004 ' First statement of this 'Sub New' should be an explicit call to 'MyBase.New' or 'MyClass.New' because the constructor in the base class is marked obsolete

        ''' <summary>
        ''' Sets or indicates the timeout used by WebRequest used by WebClient
        ''' </summary>
        Public WriteOnly Property Timeout() As Integer
            Set(value As Integer)
                Debug.Assert(value > 0, "illegal value for timeout")
                _timeout = value
            End Set
        End Property

        ''' <summary>
        '''  Enables switching the server to non passive mode.
        ''' </summary>
        ''' <remarks>We need this in order for the progress UI on a download to work</remarks>
        Public WriteOnly Property UseNonPassiveFtp() As Boolean
            Set(value As Boolean)
                _useNonPassiveFtp = value
            End Set
        End Property

        ''' <summary>
        '''  Makes sure that the timeout value for WebRequests (used for all Download
        '''  and Upload methods) is set to the Timeout value.
        ''' </summary>
        ''' <param name="address"></param>
        Protected Overrides Function GetWebRequest(address As Uri) As WebRequest
            Dim request As WebRequest = MyBase.GetWebRequest(address)

            Debug.Assert(request IsNot Nothing, "Unable to get WebRequest from base class")
            If request IsNot Nothing Then
                request.Timeout = _timeout
                If _useNonPassiveFtp Then
                    Dim ftpRequest As FtpWebRequest = TryCast(request, FtpWebRequest)
                    If ftpRequest IsNot Nothing Then
                        ftpRequest.UsePassive = False
                    End If
                End If

                Dim httpRequest As HttpWebRequest = TryCast(request, HttpWebRequest)
                If httpRequest IsNot Nothing Then
                    httpRequest.AllowAutoRedirect = False
                End If

            End If

            Return request
        End Function

    End Class
End Namespace
