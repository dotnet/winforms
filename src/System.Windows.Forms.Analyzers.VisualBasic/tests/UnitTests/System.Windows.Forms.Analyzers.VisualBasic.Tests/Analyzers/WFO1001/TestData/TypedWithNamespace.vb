Imports System.Windows
Imports System.Diagnostics.CodeAnalysis
Imports System.Reflection.Metadata
Imports System.Runtime.Versioning

Namespace System.Windows.Forms.Analyzers.VisualBasic.Tests

    Friend Class TypedWithNamespace
        Implements Forms.ITypedDataObject

        Public Function GetData(format As String, autoConvert As Boolean) As Object Implements IDataObject.GetData
            Return Nothing
        End Function

        Public Function GetData(format As String) As Object Implements IDataObject.GetData
            Return Nothing
        End Function

        Public Function GetData(format As Type) As Object Implements IDataObject.GetData
            Return Nothing
        End Function

        Public Function GetDataPresent(format As String, autoConvert As Boolean) As Boolean Implements IDataObject.GetDataPresent
            Return False
        End Function

        Public Function GetDataPresent(format As String) As Boolean Implements IDataObject.GetDataPresent
            Return False
        End Function

        Public Function GetDataPresent(format As Type) As Boolean Implements IDataObject.GetDataPresent
            Return False
        End Function

        Public Function GetFormats(autoConvert As Boolean) As String() Implements IDataObject.GetFormats
            Return New String() {"thing1"}
        End Function

        Public Function GetFormats() As String() Implements IDataObject.GetFormats
            Return New String() {"thing1"}
        End Function

        Public Sub SetData(format As String, autoConvert As Boolean, data As Object) Implements IDataObject.SetData
        End Sub

        Public Sub SetData(format As String, data As Object) Implements IDataObject.SetData
        End Sub

        Public Sub SetData(format As Type, data As Object) Implements IDataObject.SetData
        End Sub

        Public Sub SetData(data As Object) Implements IDataObject.SetData
        End Sub

        Public Function TryGetData(Of T)(<DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)> ByRef data As T) As Boolean Implements ITypedDataObject.TryGetData
            Throw New NotImplementedException()
        End Function

        Public Function TryGetData(Of T)(format As String, <DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)> ByRef data As T) As Boolean Implements ITypedDataObject.TryGetData
            Throw New NotImplementedException()
        End Function

        Public Function TryGetData(Of T)(format As String, autoConvert As Boolean, <DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)> ByRef data As T) As Boolean Implements ITypedDataObject.TryGetData
            Throw New NotImplementedException()
        End Function

        Public Function TryGetData(Of T)(format As String, resolver As Func(Of TypeName, Type), autoConvert As Boolean, <DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)> ByRef data As T) As Boolean Implements ITypedDataObject.TryGetData
            Throw New NotImplementedException()
        End Function

    End Class
End Namespace
