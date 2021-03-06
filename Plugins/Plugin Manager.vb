﻿Imports Tinker.Components
Imports Tinker.Commands

Namespace Plugins
    Friend Class PluginManager
        Inherits DisposableWithTask
        Implements IBotComponent

        Private Const TypeName As String = "Plugin"

        Private ReadOnly _socket As Plugins.Socket
        Private ReadOnly _hooks As New List(Of Task(Of IDisposable))

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_socket IsNot Nothing)
            Contract.Invariant(_hooks IsNot Nothing)
        End Sub

        Public Sub New(socket As Plugins.Socket)
            Contract.Requires(socket IsNot Nothing)
            Me._socket = socket
        End Sub

        Public ReadOnly Property Name As InvariantString Implements IBotComponent.Name
            Get
                Return _socket.Name
            End Get
        End Property
        Public ReadOnly Property Type As InvariantString Implements IBotComponent.Type
            Get
                Return TypeName
            End Get
        End Property
        Public ReadOnly Property Logger As Logger Implements IBotComponent.Logger
            Get
                Return _socket.Plugin.Logger
            End Get
        End Property
        Public ReadOnly Property HasControl As Boolean Implements IBotComponent.HasControl
            Get
                Contract.Ensures(Contract.Result(Of Boolean)() = _socket.Plugin.HasControl)
                Return _socket.Plugin.HasControl
            End Get
        End Property
        Public Function IsArgumentPrivate(argument As String) As Boolean Implements IBotComponent.IsArgumentPrivate
            Return _socket.Plugin.IsArgumentPrivate(argument)
        End Function
        Public Function InvokeCommand(user As BotUser, argument As String) As Task(Of String) Implements IBotComponent.InvokeCommand
            Return _socket.Plugin.InvokeCommand(user, argument)
        End Function

        Protected Overrides Function PerformDispose(finalizing As Boolean) As Task
            _socket.Dispose()
            Return _hooks.DisposeAllAsync()
        End Function

        Public ReadOnly Property Control As Control Implements IBotComponent.Control
            Get
                Return _socket.Plugin.Control
            End Get
        End Property

        Private Function IncludeCommandImpl(command As ICommand(Of IBotComponent)) As Task(Of IDisposable) Implements IBotComponent.IncludeCommand
            Dim converter = Function(plugin As IPlugin)
                                If plugin IsNot Me._socket.Plugin Then
                                    Throw New NotSupportedException("Command mapped from manager to plugin used on different plugin.")
                                End If
                                Return Me
                            End Function
            Return IncludeCommand(command.ProjectedFrom(converter))
        End Function
        Public Function IncludeCommand(command As ICommand(Of IPlugin)) As Task(Of IDisposable)
            Contract.Requires(command IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task(Of IDisposable))() IsNot Nothing)
            Return _socket.Plugin.IncludeCommand(command)
        End Function
    End Class
End Namespace
