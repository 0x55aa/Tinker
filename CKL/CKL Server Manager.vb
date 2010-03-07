﻿Imports Tinker.Components

Namespace CKL
    Public NotInheritable Class ServerManager
        Inherits DisposableWithTask
        Implements IBotComponent

        Public Shared ReadOnly WidgetTypeName As InvariantString = "CKL"
        Private Shared ReadOnly Commands As New CKL.ServerCommands()

        Private ReadOnly _server As CKL.Server
        Private ReadOnly _control As GenericBotComponentControl

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_control IsNot Nothing)
            Contract.Invariant(_server IsNot Nothing)
        End Sub

        Public Sub New(ByVal server As CKL.Server)
            Contract.Requires(server IsNot Nothing)

            Me._server = server
            Me._control = New GenericBotComponentControl(Me)
        End Sub

        Protected Overrides Function PerformDispose(ByVal finalizing As Boolean) As Task
            _server.Stop()
            _control.AsyncInvokedAction(Sub() _control.Dispose())
            Return Nothing
        End Function

        Public ReadOnly Property Control As System.Windows.Forms.Control Implements IBotComponent.Control
            Get
                Return _control
            End Get
        End Property
        Public ReadOnly Property HasControl As Boolean Implements IBotComponent.HasControl
            Get
                Contract.Ensures(Contract.Result(Of Boolean)())
                Return True
            End Get
        End Property
        Public Function InvokeCommand(ByVal user As BotUser, ByVal argument As String) As Task(Of String) Implements IBotComponent.InvokeCommand
            Return Commands.Invoke(_server, user, argument)
        End Function
        Public Function IsArgumentPrivate(ByVal argument As String) As Boolean Implements IBotComponent.IsArgumentPrivate
            Return Commands.IsArgumentPrivate(argument)
        End Function
        Public ReadOnly Property Logger As Logger Implements IBotComponent.Logger
            Get
                Return _server.Logger
            End Get
        End Property
        Public ReadOnly Property Name As InvariantString Implements IBotComponent.Name
            Get
                Return _server.name
            End Get
        End Property
        Public ReadOnly Property Type As InvariantString Implements IBotComponent.Type
            Get
                Return WidgetTypeName
            End Get
        End Property
    End Class
End Namespace
