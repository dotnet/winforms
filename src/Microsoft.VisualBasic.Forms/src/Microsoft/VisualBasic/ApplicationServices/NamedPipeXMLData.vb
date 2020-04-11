Imports System.Xml.Serialization

Namespace Microsoft.VisualBasic.ApplicationServices

    Public Class NamedPipeXMLData
        Sub New()

        End Sub

        ''' <summary>
        '''     A list of command line arguments.
        ''' </summary>
        <XmlElement("CommandLineArguments")>
        Public Property CommandLineArguments As New List(Of String)
    End Class

End Namespace
