Imports System
Imports System.Runtime.Versioning

Namespace System.Windows.Forms.Analyzers.VisualBasic.Tests.ImplementITypedDataObjectInAdditionToIDataObjectAnalyzer.Data

    Friend Interface IDataObject
        Sub DoStuff()
    End Interface

    Friend Class UnrelatedIDataObject
        Implements IDataObject

        Public Sub DoStuff() Implements IDataObject.DoStuff
            Throw New NotImplementedException()
        End Sub

    End Class

End Namespace
