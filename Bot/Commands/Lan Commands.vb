﻿Imports Tinker.Commands
Imports Tinker.Links

Namespace Commands.Specializations
    Public NotInheritable Class LanCommands
        Inherits CommandSet(Of Components.LanAdvertiserManager)

        Public Sub New()
            AddCommand(Add)
            AddCommand(Remove)
            AddCommand(Host)
        End Sub

        Public Overloads Function AddCommand(ByVal command As Command(Of WC3.LanAdvertiser)) As IDisposable
            Contract.Requires(command IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IDisposable)() IsNot Nothing)
            Return AddCommand(New ProjectedCommand(Of Components.LanAdvertiserManager, WC3.LanAdvertiser)(
                    command:=command,
                    projection:=Function(manager) manager.LanAdvertiser))
        End Function

        Private Shared ReadOnly Add As New DelegatedTemplatedCommand(Of WC3.LanAdvertiser)(
            Name:="Add",
            template:="id=# name=<game name> map=<search query>",
            Description:="Adds a game to be advertised, but doesn't create a new server to go with it.",
            func:=Function(target, user, argument)
                      Dim name = argument.NamedValue("name")
                      Dim map = WC3.Map.FromArgument(argument.NamedValue("map"))
                      Dim gameStats = New WC3.GameStats(map, If(user Is Nothing, Application.ProductName, user.Name.Value), argument)
                      Dim gameDescription = WC3.LocalGameDescription.FromArguments(name, map, gameStats)

                      Return target.queueAddGame(gameDescription).EvalOnSuccess(Function() "Started advertising game '{0}' for map '{1}'.".Frmt(name, gameStats.relativePath))
                  End Function)

        Private Shared ReadOnly Remove As New DelegatedTemplatedCommand(Of WC3.LanAdvertiser)(
            Name:="Remove",
            template:="id",
            Description:="Removes a game being advertised.",
            func:=Function(target, user, argument)
                      Dim id As UInteger
                      If Not UInteger.TryParse(argument.RawValue(0), id) Then
                          Throw New InvalidOperationException("Invalid game id.")
                      End If
                      Return target.QueueRemoveGame(id).select(
                          Function(val)
                              If val Then
                                  Return "Removed game with id {0}".Frmt(id)
                              Else
                                  Throw New InvalidOperationException("Invalid game id.")
                              End If
                          End Function)
                          End Function)

        Private Shared ReadOnly Host As New DelegatedTemplatedCommand(Of Components.LanAdvertiserManager)(
            Name:="Host",
            template:=Concat({"name=<game name>", "map=<search query>"},
                             WC3.GameSettings.PartialArgumentTemplates,
                             WC3.GameStats.PartialArgumentTemplates).StringJoin(" "),
            Description:="Creates a server of a game and advertises it on lan. More help topics under 'Help Host *'.",
            Permissions:="games:1",
            extraHelp:=Concat(New String() {},
                              WC3.GameSettings.PartialArgumentHelp,
                              WC3.GameStats.PartialArgumentHelp).StringJoin(Environment.NewLine),
            func:=Function(target, user, argument)
                      Dim futureServer = target.Bot.QueueGetAnyGameServerManager()
                      Dim futureGameSet = (From server In futureServer
                                           Select server.QueueAddGameFromArguments(argument, user)
                                           ).Defuturized
                      Dim futureAdvertised = futureGameSet.select(Function(gameSet) target.LanAdvertiser.QueueAddGame(
                                  gameDescription:=gameSet.GameSettings.GameDescription)).Defuturized
                      futureAdvertised.Catch(Sub() If futureGameSet.State = FutureState.Succeeded Then futuregameset.value.dispose())
                      Dim futureDesc = futureAdvertised.EvalOnSuccess(Function() futureGameSet.Value.GameSettings.GameDescription)
                      Return futureDesc.select(Function(desc) "Hosted game '{0}' for map '{1}'".Frmt(desc.name, desc.GameStats.relativePath))
                  End Function)
    End Class
End Namespace
