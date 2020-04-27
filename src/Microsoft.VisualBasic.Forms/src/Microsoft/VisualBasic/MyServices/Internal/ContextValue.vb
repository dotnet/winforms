' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Option Explicit On
Option Strict On

Imports System.Threading

Namespace Microsoft.VisualBasic.MyServices.Internal

    ''' <summary>
    ''' Stores an object in a context appropriate for the environment we are 
    ''' running in (web/windows)
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <remarks>
    ''' "Thread appropriate" means that if we are running on ASP.Net the object will be stored in the 
    ''' context of the current request (meaning the object is stored per request on the web).
    ''' Note that an instance of this class can only be associated
    ''' with the one item to be stored/retrieved at a time.
    ''' </remarks>
    <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
    Public Class ContextValue(Of T)
        Public Sub New()
            _contextKey = System.Guid.NewGuid.ToString
        End Sub

        ''' <summary>
        ''' Get the object from the correct thread-appropriate location
        ''' </summary>
        Public Property Value() As T 'No SyncLocks required because we are operating upon instance data and the object is not shared across threads
            Get
                Dim dictionary As IDictionary = GetDictionary()
                Return DirectCast(dictionary(_contextKey), T) 'Note, IDictionary(key) can return Nothing and that's OK
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

        Private ReadOnly _contextKey As String 'An item is stored in the dictionary by a GUID which this string maintains

        Private Shared s_threadLocal As ThreadLocal(Of IDictionary)

    End Class 'ContextValue

End Namespace
