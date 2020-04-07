Imports System.Xml.Serialization

Public Class NamedPipeXMLData
    Private _commandLineArguments As New List(Of String)

    Sub New()

    End Sub

    ''' <summary>
    '''     A list of command line arguments.
    ''' </summary>
    <XmlElement("CommandLineArguments")>
    Public Property CommandLineArguments As List(Of String)
        Get
            Return _commandLineArguments
        End Get
        Set(value As List(Of String))
            _commandLineArguments = value
        End Set
    End Property

End Class
