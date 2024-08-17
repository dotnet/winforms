' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.Specialized

Namespace Microsoft.VisualBasic.Logging

    Partial Public Class Log

        ''' <summary>
        '''  Encapsulates a System.Diagnostics.TraceSource. The value add is that
        '''  it knows if it was initialized using a config file or not.
        ''' </summary>
        Friend NotInheritable Class DefaultTraceSource
            Inherits TraceSource

            Private _listenerAttributes As StringDictionary

            ''' <summary>
            '''  TraceSource has other constructors, this is the only one we care about
            '''  for this internal class.
            ''' </summary>
            ''' <param name="name"></param>
            Public Sub New(name As String)
                MyBase.New(name)
            End Sub

            ''' <summary>
            '''  Tells us whether this TraceSource found a config file to configure itself from.
            ''' </summary>
            ''' <value><see langword="True"/> - The TraceSource was configured from a config file.</value>
            Public ReadOnly Property HasBeenConfigured() As Boolean
                Get
                    ' This forces initialization of the attributes list
                    If _listenerAttributes Is Nothing Then
                        _listenerAttributes = Attributes
                    End If

                    ' TODO: This is a temporary fix, which will break configuring logging via
                    ' file for the time being. See: https://github.com/dotnet/winforms/pull/7590
                    ' REVIEWERS: Is this still true and should it be fixed?
                    Return False
                End Get
            End Property

        End Class
    End Class
End Namespace
