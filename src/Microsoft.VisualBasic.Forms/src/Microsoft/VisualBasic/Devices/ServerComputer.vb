' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports Microsoft.VisualBasic.MyServices

Namespace Microsoft.VisualBasic.Devices

    ''' <summary>
    '''  A RAD object representing the server 'computer' for the web/Windows Services
    '''  that serves as a discovery mechanism for finding principle abstractions in
    '''  the system that you can code against.
    ''' </summary>
    Public Class ServerComputer

        'NOTE: The .Net design guidelines state that access to Instance members does not have to be thread-safe. Access to Shared members does have to be thread-safe.
        'Since My.Computer creates the instance of Computer in a thread-safe way, access to the Computer will necessarily be thread-safe.
        'There is nothing to prevent a user from passing our computer object across threads or creating their own instance and then getting into trouble.
        'But that is completely consistent with the rest of the FX design. It is MY.* that is thread safe and leads to best practice access to these objects.
        'If you dim them up yourself, you are responsible for managing the threading.

        'Lazy initialized cache for the Clock class. SHARED because Clock behaves as a readonly singleton class
        Private Shared s_clock As Clock

        'Lazy initialized cache for ComputerInfo
        Private _computerInfo As ComputerInfo

        'Lazy initialized cache for the FileSystem.
        Private _fileIO As FileSystemProxy

        'Lazy initialized cache for the Network class.
        Private _network As Network

        'Lazy initialized cache for the Registry class
        Private _registryInstance As RegistryProxy

        ''' <summary>
        '''  Returns the Clock object which contains the LocalTime and GMTTime.
        ''' </summary>
        Public ReadOnly Property Clock() As Clock
            Get
                If s_clock IsNot Nothing Then Return s_clock
                s_clock = New Clock
                Return s_clock
            End Get
        End Property

        ''' <summary>
        '''  Gets the object representing the file system of the computer.
        ''' </summary>
        ''' <value>A System.IO.FileSystem object.</value>
        ''' <remarks>
        '''  The instance returned by this property is lazy initialized and cached.
        ''' </remarks>
        Public ReadOnly Property FileSystem() As FileSystemProxy
            Get
                If _fileIO Is Nothing Then
                    _fileIO = New FileSystemProxy
                End If
                Return _fileIO
            End Get
        End Property

        ''' <summary>
        '''  Gets the object representing information about the computer's state
        ''' </summary>
        ''' <value>A <see cref="ComputerInfo"/> object.</value>
        ''' <remarks>
        '''  The instance returned by this property is lazy initialized and cached.
        ''' </remarks>
        Public ReadOnly Property Info() As ComputerInfo
            Get
                If _computerInfo Is Nothing Then
                    _computerInfo = New ComputerInfo
                End If
                Return _computerInfo
            End Get
        End Property

        ''' <summary>
        '''  This property wraps the <see cref="Environment.MachineName"/> property
        '''  in the .NET framework to return the name of the computer.
        ''' </summary>
        ''' <value>A string containing the name of the computer.</value>
        Public ReadOnly Property Name() As String
            Get
                Return Environment.MachineName
            End Get
        End Property

        ''' <summary>
        '''  This property returns the Network object containing information about
        '''  the network the machine is part of.
        ''' </summary>
        ''' <value>An instance of the Network.Network class.</value>
        Public ReadOnly Property Network() As Network
            Get
                If _network IsNot Nothing Then Return _network
                _network = New Network
                Return _network
            End Get
        End Property

        ''' <summary>
        '''  Gets the Registry object, which can be used to read, set and
        '''  enumerate keys and values in the system registry.
        ''' </summary>
        ''' <value>An instance of the RegistryProxy object</value>
        Public ReadOnly Property Registry() As RegistryProxy
            Get
                If _registryInstance IsNot Nothing Then Return _registryInstance
                _registryInstance = New RegistryProxy
                Return _registryInstance
            End Get
        End Property

    End Class
End Namespace
