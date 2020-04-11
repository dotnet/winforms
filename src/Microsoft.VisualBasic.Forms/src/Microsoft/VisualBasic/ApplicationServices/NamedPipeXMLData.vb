Imports System.Xml.Serialization

Namespace Microsoft.VisualBasic.ApplicationServices

    Public Class NamedPipeXMLData
        Sub New()

        End Sub

#Disable Warning RS0016 ' Add public types and members to the declared API
        ''' <summary>
        '''     A list of command line arguments.
        ''' </summary>
        <XmlElement("CommandLineArguments")>
        Public Property CommandLineArguments As New List(Of String)
#Enable Warning RS0016 ' Add public types and members to the declared API
    End Class

End Namespace
