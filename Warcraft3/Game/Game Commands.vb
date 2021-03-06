Imports Tinker.Commands

Namespace WC3
    Public NotInheritable Class GameCommands
        Private Sub New()
        End Sub

        Private Shared Function Conv(command As ICommand(Of Game)) As ICommand(Of GameManager)
            Contract.Requires(command IsNot Nothing)
            Contract.Ensures(Contract.Result(Of ICommand(Of GameManager))() IsNot Nothing)
            Return command.ProjectedFrom(Function(x As GameManager) x.Game)
        End Function

        Public Shared Function MakeBotAdminCommands() As CommandSet(Of GameManager)
            Contract.Ensures(Contract.Result(Of CommandSet(Of GameManager))() IsNot Nothing)
            Dim result = New CommandSet(Of GameManager)
            result.IncludeCommand(New CommandBot())
            Return result
        End Function
        Public Shared Function MakeGuestLobbyCommands() As CommandSet(Of GameManager)
            Contract.Ensures(Contract.Result(Of CommandSet(Of GameManager))() IsNot Nothing)
            Dim result = New CommandSet(Of GameManager)
            result.IncludeCommand(Conv(New CommandPing))
            Return result
        End Function
        Public Shared Function MakeGuestInGameCommands() As CommandSet(Of GameManager)
            Contract.Ensures(Contract.Result(Of CommandSet(Of GameManager))() IsNot Nothing)
            Dim result = New CommandSet(Of GameManager)
            For Each command In From c In New ICommand(Of Game)() {
                                    New CommandElevate,
                                    New CommandPing,
                                    New CommandVoteStart
                                } Select Conv(c)
                Contract.Assume(command IsNot Nothing)
                result.IncludeCommand(command)
            Next command
            Return result
        End Function
        Public Shared Function MakeHostLobbyCommands() As CommandSet(Of GameManager)
            Contract.Ensures(Contract.Result(Of CommandSet(Of GameManager))() IsNot Nothing)
            Dim result = New CommandSet(Of GameManager)
            For Each command In From c In New ICommand(Of Game)() {
                                    New CommandBoot,
                                    New CommandCancel,
                                    New CommandClose,
                                    New CommandColor,
                                    New CommandCPU,
                                    New CommandGet,
                                    New CommandHandicap,
                                    New CommandLock,
                                    New CommandOpen,
                                    New CommandPing,
                                    New CommandRace,
                                    New CommandReserve,
                                    New CommandSet,
                                    New CommandSetTeam,
                                    New CommandSetupTeams,
                                    New CommandStart,
                                    New CommandSwap,
                                    New CommandUnlock
                                } Select Conv(c)
                Contract.Assume(command IsNot Nothing)
                result.IncludeCommand(command)
            Next command
            Return result
        End Function
        Public Shared Function MakeHostInGameCommands() As CommandSet(Of GameManager)
            Contract.Ensures(Contract.Result(Of CommandSet(Of GameManager))() IsNot Nothing)
            Dim result = New CommandSet(Of GameManager)
            For Each command In From c In New ICommand(Of Game)() {
                                    New CommandBoot,
                                    New CommandDisconnect,
                                    New CommandGet,
                                    New CommandPing,
                                    New CommandSet
                                } Select Conv(c)
                Contract.Assume(command IsNot Nothing)
                result.IncludeCommand(command)
            Next command
            Return result
        End Function

        Private NotInheritable Class CommandBoot
            Inherits TemplatedCommand(Of Game)
            Public Sub New()
                MyBase.New(Name:="Boot",
                           template:="Name/Color -close",
                           Description:="Kicks a player from the game. Closes their slot if -close is specified.")
            End Sub
            <SuppressMessage("Microsoft.Contracts", "Ensures-40-81")>
            Protected Overloads Overrides Async Function PerformInvoke(target As Game, user As BotUser, argument As CommandArgument) As Task(Of String)
                Dim slotQuery = argument.RawValue(0)
                Dim shouldClose = argument.HasOptionalSwitch("close")
                Await target.QueueBoot(slotQuery, shouldClose)
                Return "Booted"
            End Function
        End Class

        Private NotInheritable Class CommandBot
            Inherits BaseCommand(Of GameManager)

            Public Sub New()
                MyBase.New(Name:="Bot",
                           Format:="subcommand...",
                           Description:="Forwards commands to the bot.")
            End Sub
            <SuppressMessage("Microsoft.Contracts", "Ensures-40-81")>
            Protected Overrides Async Function PerformInvoke(target As GameManager, user As BotUser, argument As String) As Task(Of String)
                Dim botManagers = (Await target.Bot.Components.QueueGetAllComponents()).OfType(Of Bot.MainBotManager)()
                Return Await botManagers.Single().InvokeCommand(user, argument)
            End Function
        End Class

        Private NotInheritable Class CommandCancel
            Inherits TemplatedCommand(Of Game)
            Public Sub New()
                MyBase.New(Name:="Cancel",
                           template:="",
                           Description:="Closes this game instance.")
            End Sub
            <SuppressMessage("Microsoft.Contracts", "Ensures-40-81")>
            Protected Overloads Overrides Async Function PerformInvoke(target As Game, user As BotUser, argument As CommandArgument) As Task(Of String)
                target.Dispose()
                Await target.DisposalTask
                Return "Cancelled"
            End Function
        End Class

        Private NotInheritable Class CommandClose
            Inherits TemplatedCommand(Of Game)
            Public Sub New()
                MyBase.New(Name:="Close",
                           template:="slot",
                           Description:="Closes a slot.")
            End Sub
            <SuppressMessage("Microsoft.Contracts", "Ensures-40-81")>
            Protected Overloads Overrides Async Function PerformInvoke(target As Game, user As BotUser, argument As CommandArgument) As Task(Of String)
                Await target.QueueCloseSlot(argument.RawValue(0))
                Return "Closed"
            End Function
        End Class

        Private NotInheritable Class CommandColor
            Inherits TemplatedCommand(Of Game)
            Public Sub New()
                MyBase.New(Name:="Color",
                           template:="slot value",
                           Description:="Sets the color of a slot. Not allowed when the map uses Fixed Player Settings.")
            End Sub
            <SuppressMessage("Microsoft.Contracts", "Ensures-40-81")>
            Protected Overloads Overrides Async Function PerformInvoke(target As Game, user As BotUser, argument As CommandArgument) As Task(Of String)
                Dim argSlot = argument.RawValue(0)
                Dim argColor = argument.RawValue(1)
                Dim color = argColor.EnumTryParse(Of Protocol.PlayerColor)(ignoreCase:=True)
                If Not color.HasValue Then Throw New InvalidOperationException("Unrecognized color: '{0}'.".Frmt(argColor))
                Await target.QueueSetSlotColor(argSlot, color.Value)
                Return "Set Color"
            End Function
        End Class

        Private NotInheritable Class CommandCPU
            Inherits TemplatedCommand(Of Game)
            Public Sub New()
                MyBase.New(Name:="CPU",
                           template:="slot ?difficulty",
                           Description:="Places a computer in a slot, unless it contains a player.")
            End Sub
            <SuppressMessage("Microsoft.Contracts", "Ensures-40-81")>
            Protected Overloads Overrides Async Function PerformInvoke(target As Game, user As BotUser, argument As CommandArgument) As Task(Of String)
                Dim argSlot = argument.RawValue(0)
                Dim argDifficulty = If(argument.RawValueCount >= 2, argument.RawValue(1), WC3.Protocol.ComputerLevel.Normal.ToString)
                Dim difficulty = argDifficulty.EnumTryParse(Of Protocol.ComputerLevel)(ignoreCase:=True)
                If Not difficulty.HasValue Then Throw New InvalidOperationException("Unrecognized difficulty: '{0}'.".Frmt(argDifficulty))
                Await target.QueueSetSlotCpu(argSlot, difficulty.Value)
                Return "Set {0} to Computer ({1})".Frmt(argSlot, argDifficulty)
            End Function
        End Class

        Private NotInheritable Class CommandDisconnect
            Inherits TemplatedCommand(Of Game)
            Public Sub New()
                MyBase.New(Name:="Disconnect",
                           template:="",
                           Description:="Causes the bot to disconnect from the game. The game might continue if one of the players can host.")
            End Sub
            <SuppressMessage("Microsoft.Contracts", "Ensures-40-81")>
            Protected Overloads Overrides Async Function PerformInvoke(target As Game, user As BotUser, argument As CommandArgument) As Task(Of String)
                target.Dispose()
                Await target.DisposalTask
                Return "Disconnected"
            End Function
        End Class

        Private NotInheritable Class CommandElevate
            Inherits TemplatedCommand(Of Game)
            Public Sub New()
                MyBase.New(Name:="Elevate",
                           template:="password",
                           Description:="Gives access to admin or host commands.")
            End Sub
            <SuppressMessage("Microsoft.Contracts", "Ensures-40-81")>
            Protected Overloads Overrides Async Function PerformInvoke(target As Game, user As BotUser, argument As CommandArgument) As Task(Of String)
                If user Is Nothing Then Throw New InvalidOperationException("User not specified.")
                Await target.QueueElevatePlayer(user.Name, argument.RawValue(0))
                Return "Elevated"
            End Function
        End Class

        Private NotInheritable Class CommandGet
            Inherits TemplatedCommand(Of Game)
            Public Sub New()
                MyBase.New(Name:="Get",
                           template:="setting",
                           Description:="Returns the current value of a game setting {tickperiod, laglimit, gamerate}.")
            End Sub
            Protected Overloads Overrides Async Function PerformInvoke(target As Game, user As BotUser, argument As CommandArgument) As Task(Of String)
                Dim val As Double
                Dim argSetting = argument.RawValue(0).ToInvariant
                Select Case argSetting
                    Case "TickPeriod" : val = (Await target.Motor.QueueGetTickPeriod).TotalMilliseconds
                    Case "LagLimit" : val = (Await target.Motor.QueueGetLagLimit).TotalMilliseconds
                    Case "GameRate" : val = Await target.Motor.QueueGetSpeedFactor
                    Case Else : Throw New ArgumentException("Unrecognized setting '{0}'.".Frmt(argSetting))
                End Select
                Return "{0} = '{1}'".Frmt(argSetting, val)
            End Function
        End Class

        Private NotInheritable Class CommandHandicap
            Inherits TemplatedCommand(Of Game)
            Public Sub New()
                MyBase.New(Name:="Handicap",
                           template:="slot value",
                           Description:="Sets the handicap of a slot.")
            End Sub
            <SuppressMessage("Microsoft.Contracts", "Ensures-40-81")>
            Protected Overloads Overrides Async Function PerformInvoke(target As Game, user As BotUser, argument As CommandArgument) As Task(Of String)
                Dim argSlot = argument.RawValue(0)
                Dim argHandicap = argument.RawValue(1)
                Dim newHandicap As Byte
                If Not Byte.TryParse(argHandicap, newHandicap) Then newHandicap = 0
                Select Case newHandicap
                    Case 50, 60, 70, 80, 90, 100
                        Await target.QueueSetSlotHandicap(argSlot, newHandicap)
                        Return "Set Handicap to {0}".Frmt(newHandicap)
                    Case Else
                        Throw New InvalidOperationException("Invalid handicap: '{0}'.".Frmt(argHandicap))
                End Select
            End Function
        End Class

        Private NotInheritable Class CommandLock
            Inherits TemplatedCommand(Of Game)
            Public Sub New()
                MyBase.New(Name:="Lock",
                           template:="?slot -full",
                           Description:="Prevents players from leaving a slot or from changing slot properties (if -full). Omit the slot argument to affect all slots.")
            End Sub
            <SuppressMessage("Microsoft.Contracts", "Ensures-40-81")>
            Protected Overloads Overrides Async Function PerformInvoke(target As Game, user As BotUser, argument As CommandArgument) As Task(Of String)
                Dim lockType = If(argument.HasOptionalSwitch("full"), WC3.Slot.LockState.Frozen, WC3.Slot.LockState.Sticky)
                If argument.RawValueCount = 0 Then
                    Await target.QueueSetAllSlotsLocked(lockType)
                    Return "Locked slots"
                Else
                    Await target.QueueSetSlotLocked(argument.RawValue(0), lockType)
                    Return "Locked slot"
                End If
            End Function
        End Class

        Private NotInheritable Class CommandOpen
            Inherits TemplatedCommand(Of Game)
            Public Sub New()
                MyBase.New(Name:="Open",
                           template:="slot",
                           Description:="Opens a slot.")
            End Sub
            <SuppressMessage("Microsoft.Contracts", "Ensures-40-81")>
            Protected Overloads Overrides Async Function PerformInvoke(target As Game, user As BotUser, argument As CommandArgument) As Task(Of String)
                Await target.QueueOpenSlot(argument.RawValue(0))
                Return "Opened"
            End Function
        End Class

        Private NotInheritable Class CommandPing
            Inherits TemplatedCommand(Of Game)
            Public Sub New()
                MyBase.New(Name:="Ping",
                           template:="",
                           Description:="Returns estimated network round trip times for each player.")
            End Sub
            Protected Overloads Overrides Async Function PerformInvoke(target As Game, user As BotUser, argument As CommandArgument) As Task(Of String)
                Dim players = Await target.QueueGetPlayers()
                Dim latencies = Await Task.WhenAll((From player In players Select player.QueueGetLatencyDescription).Cache)
                Return "Estimated RTT: {0}".Frmt((From pair In players.Zip(latencies)
                                                  Where Not pair.Item1.IsFake
                                                  Select "{0}={1}".Frmt(pair.Item1.Name, pair.Item2)
                                                  ).StringJoin(" "))
            End Function
        End Class

        Private NotInheritable Class CommandRace
            Inherits TemplatedCommand(Of Game)
            Public Sub New()
                MyBase.New(Name:="Race",
                           template:="slot race",
                           Description:="Sets the race of a slot. Not allowed when the map uses Fixed Player Settings and the slot race is not Selectable.")
            End Sub
            <SuppressMessage("Microsoft.Contracts", "Ensures-40-81")>
            Protected Overloads Overrides Async Function PerformInvoke(target As Game, user As BotUser, argument As CommandArgument) As Task(Of String)
                Dim argSlot = argument.RawValue(0)
                Dim argRace = argument.RawValue(1)
                Dim race = argRace.EnumTryParse(Of Protocol.Races)(ignoreCase:=True)
                If Not race.HasValue Then Throw New InvalidOperationException("Unrecognized race: '{0}'.".Frmt(argRace))
                Await target.QueueSetSlotRace(argSlot, race.Value)
                Return "Set Race"
            End Function
        End Class

        Private NotInheritable Class CommandReserve
            Inherits TemplatedCommand(Of Game)
            Public Sub New()
                MyBase.New(Name:="Reserve",
                           template:="name -slot=any",
                           Description:="Reserves a slot for a player.")
            End Sub
            <SuppressMessage("Microsoft.Contracts", "Ensures-40-81")>
            Protected Overloads Overrides Async Function PerformInvoke(target As Game, user As BotUser, argument As CommandArgument) As Task(Of String)
                Dim name = argument.RawValue(0)
                Dim slotQueryString = argument.TryGetOptionalNamedValue("slot")
                Dim slotQuery = If(slotQueryString Is Nothing, Nothing, New InvariantString?(slotQueryString))

                Await target.QueueReserveSlot(name, slotQuery)
                Return "Reserved {0} for {1}.".Frmt(If(slotQueryString, "slot"), name)
            End Function
        End Class

        Private NotInheritable Class CommandSet
            Inherits TemplatedCommand(Of Game)
            Public Sub New()
                MyBase.New(Name:="Set",
                           template:="setting value",
                           Description:="Sets the value of a game setting {tickperiod, laglimit, gamerate}.")
            End Sub
            Protected Overloads Overrides Function PerformInvoke(target As Game, user As BotUser, argument As CommandArgument) As Task(Of String)
                Dim val_us As UShort
                Dim vald As Double
                Dim isShort = UShort.TryParse(argument.RawValue(1), val_us)
                Dim isDouble = Double.TryParse(argument.RawValue(1), vald)
                Dim argSetting = argument.RawValue(0).ToInvariant
                Select Case argSetting
                    Case "TickPeriod"
                        If Not isShort OrElse val_us < 1 OrElse val_us > 20000 Then Throw New ArgumentException("Invalid value")
                        Dim t = CInt(val_us).Milliseconds
                        target.Motor.QueueSetTickPeriod(t)
                    Case "LagLimit"
                        If Not isShort OrElse val_us < 1 OrElse val_us > 20000 Then Throw New ArgumentException("Invalid value")
                        Dim t = CInt(val_us).Milliseconds
                        target.Motor.QueueSetLagLimit(t)
                    Case "GameRate"
                        If Not isDouble OrElse vald < 0.01 OrElse vald > 10 Then Throw New ArgumentException("Invalid value")
                        Contract.Assume(vald > 0)
                        target.Motor.QueueSetSpeedFactor(vald)
                    Case Else
                        Throw New ArgumentException("Unrecognized setting '{0}'.".Frmt(argument.RawValue(0)))
                End Select
                Return "{0} set to {1}".Frmt(argument.RawValue(0), argument.RawValue(1)).AsTask
            End Function
        End Class

        Private NotInheritable Class CommandSetTeam
            Inherits TemplatedCommand(Of Game)
            Public Sub New()
                MyBase.New(Name:="SetTeam",
                           template:="slot team",
                           Description:="Sets a slot's team. Only works in melee games.")
            End Sub
            <SuppressMessage("Microsoft.Contracts", "Ensures-40-81")>
            Protected Overloads Overrides Async Function PerformInvoke(target As Game, user As BotUser, argument As CommandArgument) As Task(Of String)
                Dim argSlot = argument.RawValue(0)
                Dim argTeam = argument.RawValue(1)
                Dim team As Byte
                If Not Byte.TryParse(argTeam, team) OrElse team < 1 OrElse team > 13 Then
                    Throw New ArgumentException("Invalid team: '{0}'.".Frmt(argTeam))
                End If
                team -= CByte(1)
                Await target.QueueSetSlotTeam(argSlot, team)
                Return "Set Team"
            End Function
        End Class

        Private NotInheritable Class CommandSetupTeams
            Inherits TemplatedCommand(Of Game)
            Public Sub New()
                MyBase.New(Name:="SetupTeams",
                           template:="teams",
                           Description:="Sets up the number of slots on each team (eg. 'SetupTeams 2v2' will leave two open slots on each team).")
            End Sub
            <SuppressMessage("Microsoft.Contracts", "Ensures-40-81")>
            Protected Overloads Overrides Async Function PerformInvoke(target As Game, user As BotUser, argument As CommandArgument) As Task(Of String)
                Await target.QueueTrySetTeamSizes(TeamVersusStringToTeamSizes(argument.RawValue(0)))
                Return "Set Teams"
            End Function
        End Class

        Private NotInheritable Class CommandStart
            Inherits TemplatedCommand(Of Game)
            Public Sub New()
                MyBase.New(Name:="Start",
                           template:="",
                           Description:="Starts the launch countdown.")
            End Sub
            <SuppressMessage("Microsoft.Contracts", "Ensures-40-81")>
            Protected Overloads Overrides Async Function PerformInvoke(target As Game, user As BotUser, argument As CommandArgument) As Task(Of String)
                Await target.QueueStartCountdown()
                Return "Started Countdown"
            End Function
        End Class

        Private NotInheritable Class CommandSwap
            Inherits TemplatedCommand(Of Game)
            Public Sub New()
                MyBase.New(Name:="Swap",
                           template:="slot1 slot2",
                           Description:="Swaps the contents of two slots.")
            End Sub
            <SuppressMessage("Microsoft.Contracts", "Ensures-40-81")>
            Protected Overloads Overrides Async Function PerformInvoke(target As Game, user As BotUser, argument As CommandArgument) As Task(Of String)
                Await target.QueueSwapSlotContents(argument.RawValue(0), argument.RawValue(1))
                Return "Swapped Slots"
            End Function
        End Class

        Private NotInheritable Class CommandUnlock
            Inherits TemplatedCommand(Of Game)
            Public Sub New()
                MyBase.New(Name:="Unlock",
                           template:="?slot",
                           Description:="Allows players to move from a slot and change its properties. Omit the slot argument to affect all slots.")
            End Sub
            <SuppressMessage("Microsoft.Contracts", "Ensures-40-81")>
            Protected Overloads Overrides Async Function PerformInvoke(target As Game, user As BotUser, argument As CommandArgument) As Task(Of String)
                Dim lockType = WC3.Slot.LockState.Unlocked
                If argument.RawValueCount = 0 Then
                    Await target.QueueSetAllSlotsLocked(lockType)
                    Return "Unlocked slots"
                Else
                    Await target.QueueSetSlotLocked(argument.RawValue(0), lockType)
                    Return "Unlocked slot"
                End If
            End Function
        End Class

        Private NotInheritable Class CommandVoteStart
            Inherits TemplatedCommand(Of Game)
            Public Sub New()
                MyBase.New(Name:="VoteStart",
                           template:="-cancel",
                           Description:="Places or cancels a vote to prematurely start an autostarted game. Requires at least 2 players and at least a 2/3 majority.")
            End Sub
            <SuppressMessage("Microsoft.Contracts", "Ensures-40-81")>
            Protected Overloads Overrides Async Function PerformInvoke(target As Game, user As BotUser, argument As CommandArgument) As Task(Of String)
                If user Is Nothing Then Throw New InvalidOperationException("User not specified.")
                Await target.QueueSetPlayerVoteToStart(user.Name, wantsToStart:=Not argument.HasOptionalSwitch("cancel"))
                Return "Voted to start"
            End Function
        End Class
    End Class
End Namespace
