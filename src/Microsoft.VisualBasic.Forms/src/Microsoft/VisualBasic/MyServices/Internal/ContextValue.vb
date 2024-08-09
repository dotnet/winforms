' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Threading

Namespace Microsoft.VisualBasic.MyServices.Internal

    ''' <summary>
    '''  Stores an object in a context appropriate for the environment we are
    '''  running in (web/windows).
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <remarks>
    '''  "Thread appropriate" means that if we are running on ASP.Net the object will be stored in the
    '''  context of the current request (meaning the object is stored per request on the web).
    '''  Note that an instance of this class can only be associated
    '''  with the one item to be stored/retrieved at a time.
    ''' </remarks>
    <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
    Public Class ContextValue(Of T)

        Private Shared s_threadLocal As ThreadLocal(Of IDictionary)

        'An item is stored in the dictionary by a GUID which this string maintains
        Private ReadOnly _contextKey As String

        Public Sub New()
            _contextKey = Guid.NewGuid.ToString
        End Sub

        ''' <summary>
        '''  Get the object from the correct thread-appropriate location.
        ''' </summary>
        ''' <remarks>
        '''  No SyncLocks required because we are operating upon instance data
        '''  and the object is not shared across threads
        ''' </remarks>
        Public Property Value() As T
            Get
                Dim dictionary As IDictionary = GetDictionary()

                'Note, IDictionary(key) can return Nothing and that's OK
                Return DirectCast(dictionary(_contextKey), T)
            End Get
            Set(value As T)
                Dim dictionary As IDictionary = GetDictionary()
                dictionary(_contextKey) = value
            End Set
        End Property

        Private Shared Function GetDictionary() As IDictionary
            If s_threadLocal Is Nothing Then
                Interlocked.CompareExchange(s_threadLocal, New ThreadLocal(Of IDictionary)(Function() New Dictionary(Of String, T)), Nothing)
            End If
            Return s_threadLocal.Value
        End Function

    End Class
End Namespace
