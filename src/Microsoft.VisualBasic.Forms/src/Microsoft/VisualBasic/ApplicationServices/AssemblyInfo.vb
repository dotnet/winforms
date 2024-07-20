﻿' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.ObjectModel
Imports System.Reflection

Imports ExUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic.ApplicationServices

    ''' <summary>
    '''  A class that contains the information about an Application. This information can be
    '''  specified using the assembly attributes (contained in AssemblyInfo.vb file in case of
    '''  a VB project in Visual Studio .NET).
    ''' </summary>
    ''' <remarks>
    '''  This class is based on the <see cref="FileVersionInfo"/> class of the framework, but
    '''  reduced to a number of relevant properties.
    ''' </remarks>
    Public Class AssemblyInfo

        ' The assembly with the information.
        Private ReadOnly _assembly As Assembly

        ' Since these properties will not change during runtime, they're cached.
        ' String.Empty is not Nothing so use Nothing to mark an un-accessed property.
        ' Cache the assembly's company name.
        Private _companyName As String

        ' Cache the assembly's copyright.
        Private _copyright As String

        ' Cache the assembly's description.
        Private _description As String

        ' Cache the assembly's product name.
        Private _productName As String

        ' Cache the assembly's title.
        Private _title As String

        ' Cache the assembly's trademark.
        Private _trademark As String

        ''' <summary>
        '''  Creates an AssemblyInfo from an assembly.
        ''' </summary>
        ''' <param name="currentAssembly">The assembly for which we want to obtain the information.</param>
        Public Sub New(currentAssembly As Assembly)
            If currentAssembly Is Nothing Then
                Throw ExUtils.GetArgumentNullException(NameOf(currentAssembly))
            End If
            _assembly = currentAssembly
        End Sub

        ''' <summary>
        '''  Gets the name of the file containing the manifest (usually the .exe file).
        ''' </summary>
        ''' <returns>A String containing the file name.</returns>
        Public ReadOnly Property AssemblyName() As String
            Get
                Return _assembly.GetName.Name
            End Get
        End Property

        ''' <summary>
        '''  Gets the company name associated with the assembly.
        ''' </summary>
        ''' <returns>A String containing the AssemblyCompanyAttribute associated with the assembly.</returns>
        ''' <exception cref="InvalidOperationException">if the AssemblyCompanyAttribute is not defined.</exception>
        Public ReadOnly Property CompanyName() As String
            Get
                If _companyName Is Nothing Then
                    Dim attribute As AssemblyCompanyAttribute =
                        CType(GetAttribute(GetType(AssemblyCompanyAttribute)), AssemblyCompanyAttribute)
                    If attribute Is Nothing Then
                        _companyName = String.Empty
                    Else
                        _companyName = attribute.Company
                    End If
                End If
                Return _companyName
            End Get
        End Property

        ''' <summary>
        '''  Gets the copyright notices associated with the assembly.
        ''' </summary>
        ''' <returns>A String containing the AssemblyCopyrightAttribute associated with the assembly.</returns>
        ''' <exception cref="InvalidOperationException">if the AssemblyCopyrightAttribute is not defined.</exception>
        Public ReadOnly Property Copyright() As String
            Get
                If _copyright Is Nothing Then
                    Dim attribute As AssemblyCopyrightAttribute = CType(GetAttribute(GetType(AssemblyCopyrightAttribute)), AssemblyCopyrightAttribute)
                    If attribute Is Nothing Then
                        _copyright = String.Empty
                    Else
                        _copyright = attribute.Copyright
                    End If
                End If
                Return _copyright
            End Get
        End Property

        ''' <summary>
        '''  Gets the description associated with the assembly.
        ''' </summary>
        ''' <returns>A String containing the AssemblyDescriptionAttribute associated with the assembly.</returns>
        ''' <exception cref="InvalidOperationException">if the AssemblyDescriptionAttribute is not defined.</exception>
        Public ReadOnly Property Description() As String
            Get
                If _description Is Nothing Then
                    Dim attribute As AssemblyDescriptionAttribute =
                        CType(GetAttribute(GetType(AssemblyDescriptionAttribute)), AssemblyDescriptionAttribute)
                    If attribute Is Nothing Then
                        _description = String.Empty
                    Else
                        _description = attribute.Description
                    End If
                End If
                Return _description
            End Get
        End Property

        ''' <summary>
        '''  Gets the directory where the assembly lives.
        ''' </summary>
        Public ReadOnly Property DirectoryPath() As String
            Get
                Return IO.Path.GetDirectoryName(_assembly.Location)
            End Get
        End Property

        ''' <summary>
        '''  Returns the names of all assemblies loaded by the current application.
        ''' </summary>
        ''' <returns>A ReadOnlyCollection(Of Assembly) containing all the loaded assemblies.</returns>
        ''' <exception cref="AppDomainUnloadedException">attempt on an unloaded application domain.</exception>
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
        '''  Gets the product name associated with the assembly.
        ''' </summary>
        ''' <returns>A String containing the AssemblyProductAttribute associated with the assembly.</returns>
        ''' <exception cref="InvalidOperationException">if the AssemblyProductAttribute is not defined.</exception>
        Public ReadOnly Property ProductName() As String
            Get
                If _productName Is Nothing Then
                    Dim attribute As AssemblyProductAttribute = CType(GetAttribute(GetType(AssemblyProductAttribute)), AssemblyProductAttribute)
                    If attribute Is Nothing Then
                        _productName = String.Empty
                    Else
                        _productName = attribute.Product
                    End If
                End If
                Return _productName
            End Get
        End Property

        ''' <summary>
        '''  Returns the current stack trace information.
        ''' </summary>
        ''' <returns>A string containing stack trace information. Value can be String.Empty.</returns>
        ''' <exception cref="ArgumentOutOfRangeException">The requested stack trace information is out of range.</exception>
        Public ReadOnly Property StackTrace() As String
            Get
                Return Environment.StackTrace
            End Get
        End Property

        ''' <summary>
        '''  Gets the company name associated with the assembly.
        ''' </summary>
        ''' <returns>A String containing the AssemblyTitleAttribute associated with the assembly.</returns>
        ''' <exception cref="InvalidOperationException">if the AssemblyTitleAttribute is not defined.</exception>
        Public ReadOnly Property Title() As String
            Get
                If _title Is Nothing Then
                    Dim attribute As AssemblyTitleAttribute =
                        CType(GetAttribute(GetType(AssemblyTitleAttribute)), AssemblyTitleAttribute)
                    If attribute Is Nothing Then
                        _title = String.Empty
                    Else
                        _title = attribute.Title
                    End If
                End If
                Return _title
            End Get
        End Property

        ''' <summary>
        '''  Gets the trademark notices associated with the assembly.
        ''' </summary>
        ''' <returns>A String containing the AssemblyTrademarkAttribute associated with the assembly.</returns>
        ''' <exception cref="InvalidOperationException">if the AssemblyTrademarkAttribute is not defined.</exception>
        Public ReadOnly Property Trademark() As String
            Get
                If _trademark Is Nothing Then
                    Dim attribute As AssemblyTrademarkAttribute = CType(GetAttribute(GetType(AssemblyTrademarkAttribute)), AssemblyTrademarkAttribute)
                    If attribute Is Nothing Then
                        _trademark = String.Empty
                    Else
                        _trademark = attribute.Trademark
                    End If
                End If
                Return _trademark
            End Get
        End Property

        ''' <summary>
        '''  Gets the version number of the assembly.
        ''' </summary>
        ''' <returns>A System.Version class containing the version number of the assembly.</returns>>
        ''' <remarks>Cannot use AssemblyVersionAttribute since it always return Nothing.</remarks>
        Public ReadOnly Property Version() As Version
            Get
                Return _assembly.GetName().Version
            End Get
        End Property

        ''' <summary>
        '''  Gets the amount of physical memory mapped to the process context.
        ''' </summary>
        ''' <returns>
        '''  A 64-bit signed integer containing the size of physical memory mapped to the process context, in bytes.
        ''' </returns>
        Public ReadOnly Property WorkingSet() As Long
            Get
                Return Environment.WorkingSet
            End Get
        End Property

        ''' <summary>
        '''  Gets an attribute from the assembly and throw exception if the attribute does not exist.
        ''' </summary>
        ''' <param name="attributeType">The type of the required attribute.</param>
        ''' <returns>
        '''  The attribute with the given type gotten from the assembly, or Nothing.
        ''' </returns>
        Private Function GetAttribute(attributeType As Type) As Object

            Debug.Assert(_assembly IsNot Nothing, "Null m_Assembly")

            Dim attributes() As Object = _assembly.GetCustomAttributes(attributeType, inherit:=True)

            If attributes.Length = 0 Then
                Return Nothing
            Else
                Return attributes(0)
            End If
        End Function

    End Class
End Namespace
