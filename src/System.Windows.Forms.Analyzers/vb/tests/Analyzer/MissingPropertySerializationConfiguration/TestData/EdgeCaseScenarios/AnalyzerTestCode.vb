' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
Option Strict On
Option Explicit On

Imports System
Imports System.ComponentModel

Namespace Test

    ' Custom IComponent interface in a different namespace
    ' This should not be detected by the analyzer
    Namespace CustomComponents

        Public Interface IComponent
            Inherits IDisposable

            Property Site As ISite
            Event Disposed As EventHandler
        End Interface

        Public Interface ISite
            Inherits IServiceProvider

            ReadOnly Property Component As IComponent
            ReadOnly Property Container As IContainer
            ReadOnly Property DesignMode As Boolean
            Property Name As String
        End Interface

        Public Interface IContainer
            Inherits IDisposable

            ReadOnly Property Components As ComponentCollection
            Sub Add(component As IComponent)
            Sub Add(component As IComponent, name As String)
            Sub Remove(component As IComponent)
        End Interface

        Public Class ComponentCollection
            ' Implementation omitted
        End Class

        ' Component implementing the custom IComponent
        ' Properties here should not be flagged
        Public Class CustomComponent
            Implements CustomComponents.IComponent

            Private _site As ISite

            Public Property Site As ISite Implements IComponent.Site
                Get
                    Return _site
                End Get
                Set(value As ISite)
                    _site = value
                End Set
            End Property

            ' This should not be flagged because it's from a custom IComponent
            Public Property CustomProperty As String

            Public Event Disposed As EventHandler Implements IComponent.Disposed

            Public Sub Dispose() Implements IDisposable.Dispose
                RaiseEvent Disposed(Me, EventArgs.Empty)
            End Sub
        End Class
    End Namespace

    ' Component implementing System.ComponentModel.IComponent
    Public Class MyComponent
        Implements System.ComponentModel.IComponent

        Private _site As System.ComponentModel.ISite

        Public Property Site As System.ComponentModel.ISite Implements System.ComponentModel.IComponent.Site
            Get
                Return _site
            End Get
            Set(value As System.ComponentModel.ISite)
                _site = value
            End Set
        End Property

        Public Event Disposed As EventHandler Implements System.ComponentModel.IComponent.Disposed

        ' This should not be flagged because it's static
        Public Shared Property StaticProperty As String

        ' This should not be flagged because it has a private setter
        Public Property PrivateSetterProperty As String
            Get
                Return String.Empty
            End Get
            Private Set(value As String)
                ' Do nothing
            End Set
        End Property

        ' This should not be flagged because it's internal with a private setter
        Friend Property InternalPrivateSetterProperty As String
            Get
                Return String.Empty
            End Get
            Private Set(value As String)
                ' Do nothing
            End Set
        End Property

        ' This WOULD be flagged in a normal scenario (public read/write property)
        Public Property RegularProperty As String

        Public Sub Dispose() Implements IDisposable.Dispose
            RaiseEvent Disposed(Me, EventArgs.Empty)
        End Sub
    End Class

    ' Base component with properly attributed properties
    Public Class BaseComponent
        Implements System.ComponentModel.IComponent

        Private _site As System.ComponentModel.ISite

        Public Property Site As System.ComponentModel.ISite Implements System.ComponentModel.IComponent.Site
            Get
                Return _site
            End Get
            Set(value As System.ComponentModel.ISite)
                _site = value
            End Set
        End Property

        Public Event Disposed As EventHandler Implements System.ComponentModel.IComponent.Disposed

        ' Properly attributed with DesignerSerializationVisibility
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Overridable Property AttributedProperty As String

        ' Properly attributed with DefaultValue
        <DefaultValue("Default")>
        Public Overridable Property DefaultValueProperty As String

        Public Sub Dispose() Implements IDisposable.Dispose
            RaiseEvent Disposed(Me, EventArgs.Empty)
        End Sub
    End Class

    ' Derived component with overridden properties
    Public Class DerivedComponent
        Inherits BaseComponent

        ' These should not be flagged because they are overrides
        ' and the base property is already properly attributed
        Public Overrides Property AttributedProperty As String
        Public Overrides Property DefaultValueProperty As String
    End Class
End Namespace
