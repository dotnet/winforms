Imports System.Diagnostics.CodeAnalysis
Imports System.Reflection.Metadata
Imports IManagedDataObject = System.Windows.Forms.IDataObject
Imports System.Runtime.Versioning

Namespace System.Windows.Forms.Analyzers.VisualBasic.Tests

    Friend Class UntypedWithAlias
        Implements IManagedDataObject

        Public Function GetData(format As String, autoConvert As Boolean) As Object Implements IManagedDataObject.GetData
            Return Nothing
        End Function

        Public Function GetData(format As String) As Object Implements IManagedDataObject.GetData
            Return Nothing
        End Function

        Public Function GetData(format As Type) As Object Implements IManagedDataObject.GetData
            Return Nothing
        End Function

        Public Function GetDataPresent(format As String, autoConvert As Boolean) As Boolean Implements IManagedDataObject.GetDataPresent
            Return False
        End Function

        Public Function GetDataPresent(format As String) As Boolean Implements IManagedDataObject.GetDataPresent
            Return False
        End Function

        Public Function GetDataPresent(format As Type) As Boolean Implements IManagedDataObject.GetDataPresent
            Return False
        End Function

        Public Function GetFormats(autoConvert As Boolean) As String() Implements IManagedDataObject.GetFormats
            Return New String() {"thing1"}
        End Function

        Public Function GetFormats() As String() Implements IManagedDataObject.GetFormats
            Return New String() {"thing1"}
        End Function

        Public Sub SetData(format As String, autoConvert As Boolean, data As Object) Implements IManagedDataObject.SetData
        End Sub

        Public Sub SetData(format As String, data As Object) Implements IManagedDataObject.SetData
        End Sub

        Public Sub SetData(format As Type, data As Object) Implements IManagedDataObject.SetData
        End Sub

        Public Sub SetData(data As Object) Implements IManagedDataObject.SetData
        End Sub

    End Class
End Namespace
