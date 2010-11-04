﻿Namespace Commands
    ''' <summary>
    ''' Performs actions specified by text arguments.
    ''' </summary>
    <ContractClass(GetType(ContractClassCommand(Of )))>
    Public MustInherit Class Command(Of TTarget)
        Implements ICommand(Of TTarget)
        Private ReadOnly _name As InvariantString
        Private ReadOnly _format As InvariantString
        Private ReadOnly _description As String
        Private ReadOnly _permissions As IDictionary(Of InvariantString, UInteger)
        Private ReadOnly _extraHelp As IDictionary(Of InvariantString, String)
        Private ReadOnly _hasPrivateArguments As Boolean

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(_description IsNot Nothing)
            Contract.Invariant(_permissions IsNot Nothing)
            Contract.Invariant(_extraHelp IsNot Nothing)
        End Sub

        Protected Sub New(ByVal name As InvariantString,
                          ByVal format As InvariantString,
                          ByVal description As String,
                          Optional ByVal permissions As String = Nothing,
                          Optional ByVal extraHelp As String = Nothing,
                          Optional ByVal hasPrivateArguments As Boolean = False)
            Contract.Requires(description IsNot Nothing)
            If name.Value.Contains(" "c) Then Throw New ArgumentException("Command names can't contain spaces.")

            Me._name = name
            Me._format = format
            Me._description = description
            Me._permissions = BuildDictionaryFromString(If(permissions, ""),
                                                        parser:=Function(x) UInteger.Parse(x, CultureInfo.InvariantCulture),
                                                        pairDivider:=",",
                                                        valueDivider:=":")
            Me._extraHelp = BuildDictionaryFromString(If(extraHelp, ""),
                                                      parser:=Function(x) x,
                                                      pairDivider:=Environment.NewLine,
                                                      valueDivider:="=")
            Me._hasPrivateArguments = hasPrivateArguments
        End Sub

        Public ReadOnly Property Name As InvariantString Implements ICommand(Of TTarget).Name
            Get
                Return _name
            End Get
        End Property
        Public ReadOnly Property Description As String Implements ICommand(Of TTarget).Description
            Get
                Return _description
            End Get
        End Property
        Public ReadOnly Property Format As InvariantString Implements ICommand(Of TTarget).Format
            Get
                Return _format
            End Get
        End Property
        Public Overridable ReadOnly Property HelpTopics As IDictionary(Of InvariantString, String) Implements ICommand(Of TTarget).HelpTopics
            Get
                Return _extraHelp
            End Get
        End Property
        Public ReadOnly Property Permissions As String Implements ICommand(Of TTarget).Permissions
            Get
                Return (From pair In Me._permissions Select "{0}:{1}".Frmt(pair.Key, pair.Value)).StringJoin(",")
            End Get
        End Property

        <Pure()>
        Public Overridable Function IsArgumentPrivate(ByVal argument As String) As Boolean Implements ICommand(Of TTarget).IsArgumentPrivate
            Return _hasPrivateArguments
        End Function
        <Pure()>
        Public Function IsUserAllowed(ByVal user As BotUser) As Boolean Implements ICommand(Of TTarget).IsUserAllowed
            If user Is Nothing Then Return True
            Return (From pair In _permissions Where user.Permission(pair.Key) < pair.Value).None
        End Function

        Public Async Function Invoke(ByVal target As TTarget, ByVal user As BotUser, ByVal argument As String) As Task(Of String) Implements ICommand(Of TTarget).Invoke
            If Not IsUserAllowed(user) Then Throw New InvalidOperationException("Insufficient permissions. Need {0}.".Frmt(Me.Permissions))

            Try
                Return Await PerformInvoke(target, user, argument)
            Catch ex As Exception
                ex.RaiseAsUnexpected("Error invoking command")
                Throw
            End Try
        End Function

        Protected MustOverride Function PerformInvoke(ByVal target As TTarget, ByVal user As BotUser, ByVal argument As String) As Task(Of String)
    End Class
    <ContractClassFor(GetType(Command(Of )))>
    MustInherit Class ContractClassCommand(Of TTarget)
        Inherits Command(Of TTarget)
        Protected Sub New()
            MyBase.New("", "", "")
        End Sub
        Protected Overrides Function PerformInvoke(ByVal target As TTarget, ByVal user As BotUser, ByVal argument As String) As Task(Of String)
            Contract.Requires(target IsNot Nothing)
            Contract.Requires(argument IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task(Of String))() IsNot Nothing)
            Throw New NotSupportedException
        End Function
    End Class
End Namespace
