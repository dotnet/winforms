' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.ObjectModel
Imports System.Reflection

Imports VbUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic.ApplicationServices

    ''' <summary>
    '''  A class that contains the information about an Application. This information
    '''  can be specified using the <see langword="Assembly"/> attributes (contained in AssemblyInfo.vb
    '''  file in case of a VB project in Visual Studio .NET).
    ''' </summary>
    ''' <remarks>
    '''  This class is based on the <see cref="FileVersionInfo"/> class of the
    '''  framework, but reduced to a number of relevant properties.
    ''' </remarks>
    Public Class AssemblyInfo

        ' The assembly with the information.
        Private ReadOnly _assembly As Assembly

        ' Since these properties will not change during runtime, they're cached.
        ' String.Empty is not Nothing so use Nothing to mark an un-accessed property.
        ' Cache the Assembly's company name.
        Private _companyName As String
        ' Cache the Assembly's copyright.
        Private _copyright As String
        ' Cache the Assembly's description.
        Private _description As String
        ' Cache the Assembly's product name.
        Private _productName As String
        ' Cache the Assembly's title.
        Private _title As String
        ' Cache the Assembly's trademark.
        Private _trademark As String

        ''' <summary>
        '''  Creates an <see cref="AssemblyInfo"/> from an <see langword="Assembly"/>.
        ''' </summary>
        ''' <param name="currentAssembly">The <see langword="Assembly"/> for which we want to obtain the information.</param>
        Public Sub New(currentAssembly As Assembly)
            If currentAssembly Is Nothing Then
                Throw VbUtils.GetArgumentNullException(NameOf(currentAssembly))
            End If
            _assembly = currentAssembly
        End Sub

        ''' <summary>
        '''  Gets a <see langword="string"/> containing the name of the file
        '''  containing the manifest (usually the .exe file).
        ''' </summary>
        ''' <value>A <see langword="String"/> containing the file name.</value>
        Public ReadOnly Property AssemblyName() As String
            Get
                Return _assembly.GetName.Name
            End Get
        End Property

        ''' <summary>
        '''  Gets the company name associated with the <see langword="Assembly"/>.
        ''' </summary>
        ''' <value>
        '''  A <see langword="String"/> containing the <see cref="AssemblyCompanyAttribute"/>
        '''  associated with the <see langword="assembly"/>.
        ''' </value>
        ''' <exception cref="InvalidOperationException">
        '''  Thrown if <see cref="AssemblyCompanyAttribute"/> is not defined.
        ''' </exception>
        Public ReadOnly Property CompanyName() As String
            Get
                If _companyName Is Nothing Then
                    _companyName = String.Empty
                    Dim attribute As AssemblyCompanyAttribute =
                        CType(GetAttribute(GetType(AssemblyCompanyAttribute)), AssemblyCompanyAttribute)
                    If attribute IsNot Nothing Then
                        _companyName = attribute.Company
                    End If
                End If
                Return _companyName
            End Get
        End Property

        ''' <summary>
        '''  Gets a <see langword="String"/> containing the directory where the <see langword="Assembly"/> lives.
        ''' </summary>
        Public ReadOnly Property DirectoryPath() As String
            Get
                Return IO.Path.GetDirectoryName(_assembly.Location)
            End Get
        End Property

        ''' <summary>
        '''  Returns the names of all assemblies loaded by the current application.
        ''' </summary>
        ''' <value>
        '''  A <see cref="ReadOnlyCollection(Of Assembly)"/> containing all the loaded assemblies.
        ''' </value>
        ''' <exception cref="AppDomainUnloadedException">
        '''  Attempt on an unloaded application domain.
        ''' </exception>
        Public ReadOnly Property LoadedAssemblies() As ReadOnlyCollection(Of Assembly)
            Get
                Dim result As New Collection(Of Assembly)
                For Each assembly As Assembly In AppDomain.CurrentDomain.GetAssemblies()
                    result.Add(assembly)
                Next
                Return New ReadOnlyCollection(Of Assembly)(result)
            End Get
        End Property

        ''' <summary>
        '''  Gets the product name associated with the <see langword="Assembly"/>.
        ''' </summary>
        ''' <value>
        '''  <see langword="String"/> containing the <see cref="AssemblyProductAttribute"/>
        '''  associated with the <see langword="Assembly"/>.
        ''' </value>
        ''' <exception cref="InvalidOperationException">
        '''  Thrown if <see cref="AssemblyProductAttribute"/> is not defined.
        ''' </exception>
        Public ReadOnly Property ProductName() As String
            Get
                If _productName Is Nothing Then
                    Dim attribute As AssemblyProductAttribute = CType(GetAttribute(GetType(AssemblyProductAttribute)), AssemblyProductAttribute)
                    _productName = String.Empty
                    If attribute IsNot Nothing Then
                        _productName = attribute.Product
                    End If
                End If
                Return _productName
            End Get
        End Property

        ''' <summary>
        '''  Gets the copyright notices associated with the <see langword="Assembly"/>.
        ''' </summary>
        ''' <value>
        '''  A <see langword="String"/> containing the <see cref="AssemblyCopyrightAttribute"/>
        '''  associated with the <see langword="Assembly"/>.
        ''' </value>
        ''' <exception cref="InvalidOperationException">
        '''  Thrown if <see cref="AssemblyCopyrightAttribute"/> is not defined.
        ''' </exception>
        Public ReadOnly Property Copyright() As String
            Get
                If _copyright Is Nothing Then
                    _copyright = String.Empty
                    Dim attribute As AssemblyCopyrightAttribute =
                        CType(GetAttribute(GetType(AssemblyCopyrightAttribute)), AssemblyCopyrightAttribute)
                    If attribute IsNot Nothing Then
                        _copyright = attribute.Copyright
                    End If
                End If
                Return _copyright
            End Get
        End Property

        ''' <summary>
        '''  Gets the description associated with the <see langword="Assembly"/>.
        ''' </summary>
        ''' <value>
        '''  <see langword="String"/> containing the <see cref="AssemblyDescriptionAttribute"/>
        '''  associated with the <see langword="Assembly"/>.
        ''' </value>
        ''' <exception cref="InvalidOperationException">
        '''  Thrown if <see cref="AssemblyDescriptionAttribute"/> is not defined.
        ''' </exception>
        Public ReadOnly Property Description() As String
            Get
                If _description Is Nothing Then
                    _description = String.Empty
                    Dim attribute As AssemblyDescriptionAttribute =
                        CType(GetAttribute(GetType(AssemblyDescriptionAttribute)), AssemblyDescriptionAttribute)
                    If attribute IsNot Nothing Then
                        _description = attribute.Description
                    End If
                End If
                Return _description
            End Get
        End Property

        ''' <summary>
        '''  Returns the current stack trace information.
        ''' </summary>
        ''' <value>
        '''  A <see langword="String"/> containing stack trace information.
        '''  Value can be <see cref="String.Empty"/>.
        ''' </value>
        ''' <exception cref="ArgumentOutOfRangeException">
        '''  Thrown if the requested stack trace information is out of range.
        ''' </exception>
        Public ReadOnly Property StackTrace() As String
            Get
                Return Environment.StackTrace
            End Get
        End Property

        ''' <summary>
        '''  Gets the company name associated with the <see langword="Assembly"/>.
        ''' </summary>
        ''' <value>
        '''  A <see langword="String"/> containing the <see cref="AssemblyTitleAttribute"/>
        '''  associated with the <see langword="Assembly"/>.
        ''' </value>
        ''' <exception cref="InvalidOperationException">
        ''' Thrown if <see cref="AssemblyTitleAttribute"/> is not defined.</exception>
        Public ReadOnly Property Title() As String
            Get
                If _title Is Nothing Then
                    _title = String.Empty
                    Dim attribute As AssemblyTitleAttribute =
                        CType(GetAttribute(GetType(AssemblyTitleAttribute)), AssemblyTitleAttribute)
                    If attribute IsNot Nothing Then
                        _title = attribute.Title
                    End If
                End If
                Return _title
            End Get
        End Property

        ''' <summary>
        '''  Gets the trademark notices associated with the <see langword="Assembly"/>.
        ''' </summary>
        ''' <value>
        '''  A <see langword="String"/> containing the <see cref="AssemblyTrademarkAttribute"/> associated
        '''  with the <see langword="Assembly"/>.
        ''' </value>
        ''' <exception cref="InvalidOperationException">
        '''  Thrown if the <see cref="AssemblyTrademarkAttribute"/> is not defined.
        ''' </exception>
        Public ReadOnly Property Trademark() As String
            Get
                If _trademark Is Nothing Then
                    _trademark = String.Empty
                    Dim attribute As AssemblyTrademarkAttribute = CType(GetAttribute(GetType(AssemblyTrademarkAttribute)), AssemblyTrademarkAttribute)
                    If attribute IsNot Nothing Then
                        _trademark = attribute.Trademark
                    End If
                End If
                Return _trademark
            End Get
        End Property

        ''' <summary>
        '''  Gets the version number of the <see langword="Assembly"/>.
        ''' </summary>
        ''' <value>
        '''  A <see cref="System.Version"/> class containing the version number of the <see langword="Assembly"/>.
        ''' </value>
        ''' <remarks>
        '''  Cannot use <see cref="AssemblyVersionAttribute"/> since it always return Nothing.
        ''' </remarks>
        Public ReadOnly Property Version() As Version
            Get
                Return _assembly.GetName().Version
            End Get
        End Property

        ''' <summary>
        '''  Gets the amount of physical memory mapped to the process context.
        ''' </summary>
        ''' <value>
        '''  A <see langword="Long"/> containing the size of physical memory mapped
        '''  to the process context, in bytes.
        ''' </value>>
        Public ReadOnly Property WorkingSet() As Long
            Get
                Return Environment.WorkingSet
            End Get
        End Property

        ''' <summary>
        '''  Gets an <see cref="Attribute"/> from the <see langword="Assembly"/> and
        '''  throws <see cref="Exception"/> if the <see cref="Attribute"/> does not exist.
        ''' </summary>
        ''' <param name="attributeType">The type of the required <see cref="Attribute"/>.</param>
        ''' <returns>
        '''  The <see cref="Attribute"/> with the given type gotten from the <see langword="Assembly"/>,
        '''  or <see langword="Nothing"/>.
        ''' </returns>
        Private Function GetAttribute(attributeType As Type) As Object
            Debug.Assert(_assembly IsNot Nothing, $"Null {NameOf(_assembly)}")

            Dim firstAttribute As Object = Nothing
            Dim attributes() As Object = _assembly.GetCustomAttributes(attributeType, inherit:=True)
            If attributes.Length > 0 Then
                firstAttribute = attributes(0)
            End If
            Return firstAttribute
        End Function

    End Class
End Namespace
