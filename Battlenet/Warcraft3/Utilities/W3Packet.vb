Namespace Warcraft3
    '''<summary>Identifies a warcraft 3 packet type.</summary>
    '''<data>
    '''  0 BYTE GAME_PACKET_PREFIX
    '''  1 BYTE packet type
    '''  2 WORD size including header = n
    '''  3 BYTE[4:n] data
    '''</data>
    Public Enum W3PacketId As Byte
        _unseen_0 = &H0
        ''' <summary>
        ''' Sent periodically by server to clients as a keep-alive packet.
        ''' Clients should respond with an equivalent PONG.
        ''' Clients which do not receive a PING or TICK for ~60s will disconnect.
        ''' If the server does not receive PONG or GAME_TICK_GUEST from a client for ~60s, it will disconnect the client.
        ''' </summary>
        Ping = &H1
        _unseen_2 = &H2
        _unseen_3 = &H3
        ''' <summary>
        ''' Sent by server in response to KNOCK to indicate the client has entered the game.
        ''' This packet has two forms: one includes the data from the SLOT_LAYOUT packet, and the other doesn't.
        ''' </summary>
        Greet = &H4
        '''<summary>Sent by server in response to KNOCK to indicate the client did not enter the game.</summary>
        RejectEntry = &H5
        '''<summary>Broadcast by server to other clients when a client enters the game.</summary>
        OtherPlayerJoined = &H6
        '''<summary>Broadcast server to other clients when a client leaves the game.</summary>
        OtherPlayerLeft = &H7
        ''' <summary>
        ''' Broadcast by server to all clients in response to a client sending READY.
        ''' Clients start playing as soon as they have received this packet for each client.
        ''' </summary>
        OtherPlayerReady = &H8
        '''<summary>Broadcast by server to all clients when the lobby state changes.</summary>
        LobbyState = &H9
        ''' <summary>
        ''' Broadcast by server to all clients to start the countdown.
        ''' Clients will disconnect if they receive this packet more than once.
        ''' START_COUNTDOWN can be sent without sending START_LOADING afterwards (wc3 will wait at 0 seconds indefinitely).
        ''' </summary>
        StartCountdown = &HA
        ''' <summary>
        ''' Broadcast by server to all clients to tell them to start loading the map.
        ''' Clients will disconnect if they receive this packet more than once.
        ''' START_LOADING does not require START_COUNTDOWN to have been sent.
        ''' </summary>
        StartLoading = &HB
        ''' <summary>
        ''' Broadcast by server to all clients periodically during game play.
        ''' Contains client actions received by the server, which will be applied at the current game time.
        ''' Contains a timespan, in milliseconds, during which no more actions will be applied.
        ''' - The client will run the game up to 'current game time + given timespan' before host-lag-pausing.
        ''' - This is how synchronization and smooth progression of game time are achieved.
        ''' Significantly altering the reported timespan to real time ratio can have weird effects, including game time stopping and losing apparent game time.
        ''' 
        ''' The sub packet format:
        '''   0 WORD truncated crc32 of following data
        '''   1 BYTE player index of sender
        '''   2 DWORD following size of subpacket
        '''   3 BYTE subpacket id
        '''   ... [depends on subpacket] ...
        ''' </summary>
        Tick = &HC
        _unseen_D = &HD
        _unseen_E = &HE
        ''' <summary>
        ''' Relayed by server to clients not connected directly to the sender.
        ''' Different formats in game and in lobby.
        ''' Clients will only request relay to clients who should receive the message (eg. only allies for ally chat).
        ''' </summary>
        Text = &HF
        ShowLagScreen = &H10
        RemovePlayerFromLagScreen = &H11
        _unseen_12 = &H12
        _unseen_13 = &H13
        SetHost = &H14 'unsure
        _peer_unknown_15 = &H15
        _peer_unknown_16 = &H16
        ConfirmHost = &H17 'unsure
        _unseen_18 = &H18
        _peer_unknown_19 = &H19
        _unseen_1A = &H1A
        _unseen_1B = &H1B
        _unseen_1C = &H1C
        _unseen_1D = &H1D
        '''<summary>First thing sent by clients upon connection. Requests entry into the game.</summary>
        Knock = &H1E
        _unseen_1F = &H1F
        _unseen_20 = &H20
        '''<summary>Sent by clients before they intentionally disconnect.</summary>
        Leaving = &H21
        _unseen_22 = &H22
        '''<summary>Sent by clients once they have finished loading the map and are ready to start playing.</summary>
        Ready = &H23
        _unseen_24 = &H24
        _unseen_25 = &H25
        ''' <summary>
        ''' Sent by clients when they perform game actions such as orders, alliance changes, trigger events, etc.
        ''' The server includes this data in its next Tick packet, broadcast to all the clients.
        ''' Clients don't perform an action until it shows up in the Tick packet.
        ''' If the TICK packet's actions disagree with the client's actions, the client will disconnect.
        ''' </summary>
        GameAction = &H26
        ''' <summary>
        ''' Sent by clients in response to Tick.
        ''' Contains a checksum of the client's game state, which is used to detect desyncs.
        ''' The lag screen is shown if a client takes too long to send a response TOCK.
        ''' </summary>
        Tock = &H27
        NonGameAction = &H28
        ClientDropLagger = &H29
        _unseen_2A = &H2A
        _peer_unknown_2B = &H2B
        AcceptHost = &H2C 'unsure
        _unseen_2D = &H2D
        _unseen_2E = &H2E
        '''<summary>Response to LanRefreshGame or LanCreateGame when clients want to know game info.</summary>
        LanRequestGame = &H2F
        '''<summary>Response to LanRequestGame containing detailed game information.</summary>
        LanDescribeGame = &H30
        '''<summary>Broadcast on lan when a game is created.</summary>
        LanCreateGame = &H31
        ''' <summary>
        ''' Broadcast on lan periodically to inform new listening wc3 clients a game exists.
        ''' Contains only very basic information about the game [no map, name, etc].
        ''' </summary>
        LanRefreshGame = &H32
        '''<summary>Broadcast on lan when a game is cancelled.</summary>
        LanDestroyGame = &H33
        PeerChat = &H34
        PeerPing = &H35
        PeerPong = &H36 'No; I refuse to say it. It's a bad pun.
        PeerKnock = &H37
        _peer_unknown_38 = &H38
        _peer_unknown_39 = &H39
        _unseen_3A = &H3A
        '''<summary>Sent by clients to the server to inform the server when the set of other clients they are interconnected with changes.</summary>
        PeerConnectionInfo = &H3B
        _unseen_3C = &H3C
        ''' <summary>
        ''' Sent by the server to new clients after they have entered the game.
        ''' Contains information about the map they must have to play the game.
        ''' </summary>
        HostMapInfo = &H3D
        ''' <summary>
        ''' Sent by the server to tell a client to start uploading to another client.
        ''' SetDownloadSource must be sent to the other client for the transfer to work.
        ''' </summary>
        SetUploadTarget = &H3E
        ''' <summary>
        ''' Sent by the server to tell a client to start downloading the map from the server or from another client.
        ''' SetUploadTarget must be sent to the other client for the peer to peer transfer to work.
        ''' </summary>
        SetDownloadSource = &H3F
        _unseen_40 = &H40
        _unseen_41 = &H41
        '''<summary>Sent by clients to the server in response to HostMapInfo and when the client has received more of the map file.</summary>
        ClientMapInfo = &H42
        '''<summary>Sent to to downloaders during map transfer. Contains map file data.</summary>
        MapFileData = &H43
        '''<summary>Positive response to MapFileData.</summary>
        MapFileDataReceived = &H44
        ''' <summary>
        ''' Negative response to MapFileData.
        ''' This can be caused by corrupted data or by sending MapFileData before SetDownloadSource is sent.
        ''' Even though wc3 clients send this packet if data is sent before SetDownloadSource, they still accept and use the data.
        ''' </summary>
        MapFileDataProblem = &H45
        '''<summary>Sent by clients in response to PING.</summary>
        Pong = &H46
    End Enum

    Public Class W3Packet
        Public Const PACKET_PREFIX As Byte = &HF7
        Public ReadOnly id As W3PacketId
        Public ReadOnly payload As IPickle(Of Object)
        Private Shared ReadOnly packetJar As SwitchJar = MakeW3PacketJar()

#Region "New"
        Private Sub New(ByVal id As W3PacketId, ByVal payload As IPickle(Of Object))
            Contract.Requires(payload IsNot Nothing)
            Me.payload = payload
            Me.id = id
        End Sub
        Private Sub New(ByVal id As W3PacketId, ByVal value As Object)
            Me.New(id, packetJar.Pack(id, value))
            Contract.Requires(value IsNot Nothing)
        End Sub
#End Region

#Region "Jar"
        Public Shared Function MakeW3PacketJar() As SwitchJar
            Dim jar = New SwitchJar
            reg_general(jar)
            reg_leave(jar)
            reg_new(jar)
            reg_lobby_to_play(jar)
            reg_lobby(jar)
            reg_play(jar)
            reg_lan(jar)
            RegPeer(jar)
            reg_dl(jar)
            Return jar
        End Function
        Private Shared Sub reg(ByVal jar As SwitchJar, ByVal id As W3PacketId, ByVal ParamArray subjars() As IJar(Of Object))
            jar.reg(id, New TupleJar(id.ToString(), subjars).Weaken)
        End Sub
        Private Shared Sub reg_general(ByVal jar As SwitchJar)
            reg(jar, W3PacketId.Ping,
                    New ValueJar("salt", 4).Weaken)

            reg(jar, W3PacketId.Pong,
                    New ValueJar("salt", 4).Weaken)

            '[server receive] [Informs the server when the set of clients a client is interconnected with changes]
            reg(jar, W3PacketId.PeerConnectionInfo,
                    New ValueJar("player bitflags", 2).Weaken)

            '[server send] [Tells clients to display a message]
            Dim chatTypeJar = New MemoryJar(Of ChatType)(New EnumJar(Of ChatType)("type", 1, flags:=False)).Weaken
            reg(jar, W3PacketId.Text,
                    New ListJar(Of ULong)("receiving player indexes", New ValueJar("player index", 1)).Weaken,
                    New ValueJar("sending player index", 1).Weaken,
                    chatTypeJar.Weaken,
                    New SwitchJar("receiver type", chatTypeJar, New Dictionary(Of Byte, IJar(Of Object)) From {
                            {ChatType.Game, New EnumJar(Of ChatReceiverType)("receiver type", 4, flags:=False).Weaken},
                            {ChatType.Lobby, New EmptyJar("receiver type").Weaken}}),
                    New StringJar("message").Weaken)

            '[server receive] [Tells the server a client wants to perform a slot action or talk]
            Dim commandTypeJar = New MemoryJar(Of NonGameAction)(New EnumJar(Of NonGameAction)("command type", 1, flags:=False)).Weaken
            reg(jar, W3PacketId.NonGameAction,
                    New ArrayJar("receiving player indexes", , 1).Weaken,
                    New ValueJar("sending player", 1).Weaken,
                    commandTypeJar.Weaken,
                    New SwitchJar("command val", commandTypeJar, New Dictionary(Of Byte, IJar(Of Object)) From {
                            {NonGameAction.GameChat, New TupleJar("chat", New EnumJar(Of ChatReceiverType)("receiver type", 4, flags:=False).Weaken, New StringJar("message").Weaken).Weaken},
                            {NonGameAction.LobbyChat, New TupleJar("chat", New EmptyJar("receiver type"), New StringJar("message").Weaken).Weaken},
                            {NonGameAction.SetTeam, New ValueJar("new value", 1).Weaken},
                            {NonGameAction.SetHandicap, New ValueJar("new value", 1).Weaken},
                            {NonGameAction.SetRace, New EnumJar(Of W3Slot.RaceFlags)("new value", 1, flags:=True).Weaken},
                            {NonGameAction.SetColor, New EnumJar(Of W3Slot.PlayerColor)("new value", 1, flags:=False).Weaken}}))
        End Sub
        Private Shared Sub reg_leave(ByVal jar As SwitchJar)
            'EXPERIMENTAL
            reg(jar, W3PacketId.ConfirmHost)
            reg(jar, W3PacketId.SetHost,
                    New ValueJar("player index", 1).Weaken)
            reg(jar, W3PacketId.AcceptHost)

            '[server receive] [Informs the server a client is leaving the game]
            reg(jar, W3PacketId.Leaving,
                    New ValueJar("leave type", 4).Weaken)

            '[server send; broadcast when a player leaves] [informs other players a player has left]
            reg(jar, W3PacketId.OtherPlayerLeft,
                    New ValueJar("player index", 1).Weaken,
                    New ValueJar("leave type", 4, "1=disc, 7=lose, 8=melee lose, 9=win, 10=draw, 11=obs, 13=lobby").Weaken)
        End Sub
        Private Shared Sub reg_new(ByVal jar As SwitchJar)
            reg(jar, W3PacketId.Knock,
                    New ValueJar("game id", 4).Weaken,
                    New ValueJar("entry key", 4).Weaken,
                    New ValueJar("unknown2", 1, "=0?").Weaken,
                    New ValueJar("listen port", 2).Weaken,
                    New ValueJar("peer key", 4, "value other players must provide when interconnecting").Weaken,
                    New StringJar("name", , , , "max 15 characters + terminator").Weaken,
                    New ValueJar("unknown3", 2, "=1").Weaken,
                    New AddressJar("internal address").Weaken)

            reg(jar, W3PacketId.Greet,
                    New ValueJar("slot layout included", 2, "=0; other mode not supported").Weaken,
                    New ValueJar("player index", 1).Weaken,
                    New AddressJar("external address").Weaken)

            reg(jar, W3PacketId.HostMapInfo,
                    New ValueJar("unknown", 4, "=1").Weaken,
                    New StringJar("path").Weaken,
                    New ValueJar("size", 4).Weaken,
                    New ArrayJar("crc32", 4).Weaken,
                    New ArrayJar("xoro checksum", 4).Weaken,
                    New ArrayJar("sha1 checksum", 20).Weaken)

            reg(jar, W3PacketId.RejectEntry,
                    New EnumJar(Of RejectReason)("reason", 4, flags:=False).Weaken)
        End Sub
        Private Shared Sub reg_lobby_to_play(ByVal jar As SwitchJar)
            '[server send; broadcast when a player becomes ready to play] [informs other players a player is ready] [players auto start when all others ready]
            reg(jar, W3PacketId.OtherPlayerReady,
                    New ValueJar("player index", 1).Weaken)

            reg(jar, W3PacketId.StartLoading)
            reg(jar, W3PacketId.StartCountdown)
            reg(jar, W3PacketId.Ready)
        End Sub
        Private Shared Sub reg_lobby(ByVal jar As SwitchJar)
            reg(jar, W3PacketId.OtherPlayerJoined,
                    New ValueJar("peer key", 4).Weaken,
                    New ValueJar("index", 1).Weaken,
                    New StringJar("name", , , , "max 15 chars + terminator").Weaken,
                    New ValueJar("unknown[0x01]", 2, "=1").Weaken,
                    New AddressJar("external address").Weaken,
                    New AddressJar("internal address").Weaken)

            reg(jar, W3PacketId.LobbyState,
                    New ValueJar("state size", 2).Weaken,
                    New ListJar(Of Dictionary(Of String, Object))("slots", New SlotJar("slot")).Weaken,
                    New ValueJar("time", 4).Weaken,
                    New ValueJar("layout style", 1).Weaken,
                    New ValueJar("num player slots", 1).Weaken)
        End Sub
        Private Shared Sub reg_play(ByVal jar As SwitchJar)
            reg(jar, W3PacketId.ShowLagScreen,
                    New ListJar(Of Dictionary(Of String, Object))("laggers",
                        New TupleJar("lagger",
                            New ValueJar("player index", 1).Weaken,
                            New ValueJar("initial time used", 4, "in milliseconds").Weaken)).Weaken)
            reg(jar, W3PacketId.RemovePlayerFromLagScreen,
                    New ValueJar("player index", 1).Weaken,
                    New ValueJar("marginal time used", 4, "in milliseconds").Weaken)
            reg(jar, W3PacketId.ClientDropLagger)

            reg(jar, W3PacketId.Tick,
                    New ValueJar("time span", 2).Weaken,
                    New ArrayJar("subpacket", , , takerest:=True).Weaken)
            reg(jar, W3PacketId.Tock,
                    New ArrayJar("game state checksum", 5).Weaken)

            Dim idJar = New MemoryJar(Of W3GameActionId)(New EnumJar(Of W3GameActionId)("id", 1, flags:=False)).Weaken
            Dim switchJar = New SwitchJar("body", idJar)
            W3GameAction.RegJar(switchJar)
            reg(jar, W3PacketId.GameAction,
                    New ArrayJar("crc32", expectedSize:=4).Weaken,
                    New RepeatingJar(Of Dictionary(Of String, Object))("actions",
                        New TupleJar("action", idJar, switchJar)).Weaken)
        End Sub
        Private Shared Sub reg_lan(ByVal jar As SwitchJar)
            reg(jar, W3PacketId.LanRequestGame,
                    New StringJar("product id", nullTerminated:=False, reversed:=True, expectedsize:=4).Weaken,
                    New ValueJar("major version", 4).Weaken,
                    New ValueJar("unknown1", 4, "=0").Weaken)

            reg(jar, W3PacketId.LanRefreshGame,
                    New ValueJar("game id", 4, "=0").Weaken,
                    New ValueJar("num players", 4).Weaken,
                    New ValueJar("free slots", 4).Weaken)

            reg(jar, W3PacketId.LanCreateGame,
                    New StringJar("product id", False, True, 4).Weaken,
                    New ValueJar("major version", 4).Weaken,
                    New ValueJar("game id", 4).Weaken)

            reg(jar, W3PacketId.LanDestroyGame,
                    New ValueJar("game id", 4).Weaken)

            reg(jar, W3PacketId.LanDescribeGame,
                    New StringJar("product id", False, True, 4).Weaken,
                    New ValueJar("major version", 4).Weaken,
                    New ValueJar("game id", 4).Weaken,
                    New ValueJar("entry key", 4).Weaken,
                    New StringJar("name", True).Weaken,
                    New StringJar("password", True, , , "unused").Weaken,
                    New W3MapSettingsJar("statstring"),
                    New ValueJar("num slots", 4).Weaken,
                    New EnumJar(Of GameTypeFlags)("game type", 4, flags:=True).Weaken,
                    New ValueJar("num players + 1", 4).Weaken,
                    New ValueJar("free slots + 1", 4).Weaken,
                    New ValueJar("age", 4).Weaken,
                    New ValueJar("listen port", 2).Weaken)
        End Sub
        Private Shared Sub RegPeer(ByVal jar As SwitchJar)
            '[Peer introduction]
            reg(jar, W3PacketId.PeerKnock,
                    New ValueJar("receiver peer key", 4, "As received from host in OTHER_PLAYER_JOINED").Weaken,
                    New ValueJar("unknown1", 4, "=0").Weaken,
                    New ValueJar("sender player id", 1).Weaken,
                    New ValueJar("unknown3", 1, "=0xFF").Weaken,
                    New ValueJar("sender peer connection flags", 4, "connection bit flags").Weaken)

            '[Periodic update and keep-alive]
            reg(jar, W3PacketId.PeerPing,
                    New ArrayJar("salt", 4).Weaken,
                    New ValueJar("sender peer connection flags", 4, "connection bit flags").Weaken,
                    New ValueJar("unknown2", 4, "=0").Weaken)

            '[Response to periodic keep-alive]
            reg(jar, W3PacketId.PeerPong,
                    New ArrayJar("salt", 4).Weaken)
        End Sub
        Private Shared Sub reg_dl(ByVal jar As SwitchJar)
            reg(jar, W3PacketId.ClientMapInfo,
                    New ValueJar("unknown", 4, "=1").Weaken,
                    New ValueJar("dl state", 1, "1=no dl, 3=active dl").Weaken,
                    New ValueJar("total downloaded", 4).Weaken)

            reg(jar, W3PacketId.SetUploadTarget,
                    New ValueJar("unknown1", 4, "=1").Weaken,
                    New ValueJar("receiving player index", 1).Weaken,
                    New ValueJar("starting file pos", 4, "=0").Weaken)

            reg(jar, W3PacketId.SetDownloadSource,
                    New ValueJar("unknown", 4, "=1").Weaken,
                    New ValueJar("sending player index", 1).Weaken)

            reg(jar, W3PacketId.MapFileData,
                    New ValueJar("receiving player index", 1).Weaken,
                    New ValueJar("sending player index", 1).Weaken,
                    New ValueJar("unknown", 4, "=1").Weaken,
                    New ValueJar("file position", 4).Weaken,
                    New ArrayJar("crc32", 4).Weaken,
                    New ArrayJar("file data", , , True).Weaken)

            reg(jar, W3PacketId.MapFileDataReceived,
                    New ValueJar("sender index", 1).Weaken,
                    New ValueJar("receiver index", 1).Weaken,
                    New ValueJar("unknown", 4, "=1").Weaken,
                    New ValueJar("total downloaded", 4).Weaken)

            reg(jar, W3PacketId.MapFileDataProblem,
                    New ValueJar("sender index", 1).Weaken,
                    New ValueJar("receiver index", 1).Weaken,
                    New ValueJar("unknown", 4, "=1").Weaken)
        End Sub
#End Region

#Region "Parsing"
        Public Shared Function FromData(ByVal id As W3PacketId, ByVal data As ViewableList(Of Byte)) As W3Packet
            If data Is Nothing Then Throw New ArgumentException()
            Return New W3Packet(id, packetJar.Parse(id, data))
        End Function
#End Region

#Region "Enums"
        Public Enum DownloadState As Byte
            NotDownloading = 1
            Downloading = 3
        End Enum
        Public Enum RejectReason As UInteger
            GameNotFound = 0
            GameFull = 9
            GameAlreadyStarted = 10
            IncorrectPassword = 27
        End Enum
        Public Enum NonGameAction As Byte
            LobbyChat = &H10
            SetTeam = &H11
            SetColor = &H12
            SetRace = &H13
            SetHandicap = &H14
            GameChat = &H20
        End Enum
        Public Enum ChatType As Byte
            Lobby = &H10
            Game = &H20
        End Enum
        Public Enum ChatReceiverType As Byte
            AllPlayers = 0
            Allies = 1
            Observers = 2
            Player1 = 3
            Player2 = 4
            Player3 = 5
            Player4 = 6
            Player5 = 7
            Player6 = 8
            Player7 = 9
            Player8 = 10
            Player9 = 11
            Player10 = 12
            Player11 = 13
            Player12 = 14
        End Enum
#End Region

#Region "Packing: Misc Packets"
        <Pure()>
        Public Shared Function MakeShowLagScreen(ByVal laggers As IEnumerable(Of IW3Player)) As W3Packet
            Contract.Requires(laggers IsNot Nothing)
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.ShowLagScreen, New Dictionary(Of String, Object) From {
                    {"laggers", (From p In laggers
                                 Select New Dictionary(Of String, Object) From {
                                        {"player index", p.index},
                                        {"initial time used", 2000}}).ToList()}})
        End Function
        <Pure()>
        Public Shared Function MakeRemovePlayerFromLagScreen(ByVal player As IW3Player,
                                                             ByVal lagTimeInMilliseconds As UInteger) As W3Packet
            Contract.Requires(player IsNot Nothing)
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.RemovePlayerFromLagScreen, New Dictionary(Of String, Object) From {
                    {"player index", player.index},
                    {"marginal time used", lagTimeInMilliseconds}})
        End Function
        <Pure()>
        Public Shared Function MakeText(ByVal text As String,
                                        ByVal type As ChatType,
                                        ByVal receiverType As ChatReceiverType,
                                        ByVal receivingPlayers As IEnumerable(Of IW3Player),
                                        ByVal sender As IW3Player) As W3Packet
            Contract.Requires(text IsNot Nothing)
            Contract.Requires(receivingPlayers IsNot Nothing)
            Contract.Requires(sender IsNot Nothing)
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.Text, New Dictionary(Of String, Object) From {
                    {"receiving player indexes", (From p In receivingPlayers Select CULng(p.index)).ToList},
                    {"sending player index", sender.index},
                    {"type", type},
                    {"message", text},
                    {"receiver type", receiverType}})
        End Function
        <Pure()>
        Public Shared Function MakeGreet(ByVal p As IW3Player,
                                         ByVal assignedIndex As Byte,
                                         ByVal map As W3Map) As W3Packet
            Contract.Requires(p IsNot Nothing)
            Contract.Requires(map IsNot Nothing)
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.Greet, New Dictionary(Of String, Object) From {
                    {"slot layout included", 0},
                    {"player index", assignedIndex},
                    {"external address", If(p.IsFake,
                                            AddressJar.packIPv4Address({0, 0, 0, 0}, 0),
                                            AddressJar.packIPv4Address(p.RemoteEndPoint))}})
        End Function
        <Pure()>
        Public Shared Function MakeReject(ByVal reason As RejectReason) As W3Packet
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.RejectEntry, New Dictionary(Of String, Object) From {
                    {"reason", reason}})
        End Function
        <Pure()>
        Public Shared Function MakeHostMapInfo(ByVal map As W3Map) As W3Packet
            Contract.Requires(map IsNot Nothing)
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.HostMapInfo, New Dictionary(Of String, Object) From {
                    {"unknown", 1},
                    {"path", "Maps\" + map.RelativePath},
                    {"size", map.FileSize},
                    {"crc32", map.Crc32},
                    {"xoro checksum", map.ChecksumXoro},
                    {"sha1 checksum", map.ChecksumSha1}})
        End Function
        <Pure()>
        Public Shared Function MakeOtherPlayerJoined(ByVal stranger As IW3Player,
                                                     Optional ByVal overrideIndex As Byte = 0) As W3Packet
            Contract.Requires(stranger IsNot Nothing)
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Dim address = If(stranger.IsFake,
                             AddressJar.packIPv4Address({0, 0, 0, 0}, 0),
                             AddressJar.packIPv4Address(stranger.RemoteEndPoint.Address, stranger.ListenPort))
            Return New W3Packet(W3PacketId.OtherPlayerJoined, New Dictionary(Of String, Object) From {
                    {"peer key", stranger.peerKey},
                    {"index", If(overrideIndex <> 0, overrideIndex, stranger.index)},
                    {"name", stranger.name},
                    {"unknown[0x01]", 1},
                    {"external address", address},
                    {"internal address", address}})
        End Function
        <Pure()>
        Public Shared Function MakePing(ByVal salt As UInteger) As W3Packet
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.Ping, New Dictionary(Of String, Object) From {
                    {"salt", salt}})
        End Function

        <Pure()>
        Public Shared Function MakeOtherPlayerReady(ByVal player As IW3Player) As W3Packet
            Contract.Requires(player IsNot Nothing)
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.OtherPlayerReady, New Dictionary(Of String, Object) From {
                    {"player index", player.index}})
        End Function
        <Pure()>
        Public Shared Function MakeOtherPlayerLeft(ByVal player As IW3Player,
                                                   ByVal leave_type As W3PlayerLeaveTypes) As W3Packet
            Contract.Requires(player IsNot Nothing)
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.OtherPlayerLeft, New Dictionary(Of String, Object) From {
                                {"player index", player.index},
                                {"leave type", CByte(leave_type)}})
        End Function
        <Pure()>
        Public Shared Function MakeLobbyState(ByVal receiver As IW3Player,
                                              ByVal map As W3Map,
                                              ByVal slots As List(Of W3Slot),
                                              ByVal time As ModInt32,
                                              Optional ByVal hideSlots As Boolean = False) As W3Packet
            Contract.Requires(receiver IsNot Nothing)
            Contract.Requires(map IsNot Nothing)
            Contract.Requires(slots IsNot Nothing)
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Dim receiver_ = receiver 'avoid contract verification issue on hoisted arguments
            Return New W3Packet(W3PacketId.LobbyState, New Dictionary(Of String, Object) From {
                    {"state size", CUShort(slots.Count() * 9 + 7)},
                    {"slots", (From slot In slots Select SlotJar.packSlot(slot, receiver_)).ToList()},
                    {"time", CUInt(time)},
                    {"layout style", If(map.isMelee, 0, 3)},
                    {"num player slots", If(Not hideSlots, map.NumPlayerSlots, If(map.NumPlayerSlots = 12, 11, 12))}})
        End Function
        <Pure()>
        Public Shared Function MakeSetHost(ByVal new_host As Byte) As W3Packet
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.SetHost, New Dictionary(Of String, Object) From {
                    {"player index", new_host}})
        End Function
        <Pure()>
        Public Shared Function MakeStartCountdown() As W3Packet
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.StartCountdown, New Dictionary(Of String, Object))
        End Function
        <Pure()> Public Shared Function MakeStartLoading() As W3Packet
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.StartLoading, New Dictionary(Of String, Object))
        End Function
        <Pure()> Public Shared Function MakeConfirmHost() As W3Packet
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.ConfirmHost, New Dictionary(Of String, Object))
        End Function
        <Pure()> Public Shared Function MakeTick(Optional ByVal delta As UShort = 250,
                                                 Optional ByVal subdata() As Byte = Nothing) As W3Packet
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            subdata = If(subdata, {})
            If subdata.Length > 0 Then
                subdata = Concat(Bnet.Crypt.crc32(New IO.MemoryStream(subdata)).bytes(ByteOrder.LittleEndian).SubArray(0, 2), subdata)
            End If

            Return New W3Packet(W3PacketId.Tick, New Dictionary(Of String, Object) From {
                    {"subpacket", subdata},
                    {"time span", delta}})
        End Function
#End Region

#Region "Packing: DL Packets"
        Public Shared Function MakeMapFileData(ByVal map As W3Map,
                                               ByVal receiverIndex As Byte,
                                               ByVal filePosition As Integer,
                                               ByRef out_SizeDataSent As Integer,
                                               Optional ByVal senderIndex As Byte = 0) As W3Packet
            Contract.Requires(senderIndex >= 0)
            Contract.Requires(senderIndex <= 12)
            Contract.Requires(receiverIndex > 0)
            Contract.Requires(receiverIndex <= 12)
            Contract.Requires(filePosition >= 0)
            Contract.Requires(map IsNot Nothing)
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Dim filedata = map.getChunk(filePosition)
            out_SizeDataSent = 0
            If senderIndex = 0 Then senderIndex = If(receiverIndex = 1, CByte(2), CByte(1))

            out_SizeDataSent = filedata.Length
            Return New W3Packet(W3PacketId.MapFileData, New Dictionary(Of String, Object) From {
                    {"receiving player index", receiverIndex},
                    {"sending player index", senderIndex},
                    {"unknown", 1},
                    {"file position", filePosition},
                    {"crc32", Bnet.Crypt.crc32(New IO.MemoryStream(filedata)).bytes(ByteOrder.LittleEndian)},
                    {"file data", filedata}})
        End Function
        Public Shared Function MakeSetUploadTarget(ByVal receiverIndex As Byte,
                                                   ByVal filePosition As UInteger) As W3Packet
            Contract.Requires(receiverIndex > 0)
            Contract.Requires(receiverIndex <= 12)
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.SetUploadTarget, New Dictionary(Of String, Object) From {
                    {"unknown1", 1},
                    {"receiving player index", receiverIndex},
                    {"starting file pos", filePosition}})
        End Function
        Public Shared Function MakeSetDownloadSource(ByVal senderIndex As Byte) As W3Packet
            Contract.Requires(senderIndex > 0)
            Contract.Requires(senderIndex <= 12)
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.SetDownloadSource, New Dictionary(Of String, Object) From {
                    {"unknown", 1},
                    {"sending player index", senderIndex}})
        End Function
        Public Shared Function MakeClientMapInfo(ByVal state As DownloadState,
                                                 ByVal total_downloaded As UInteger) As W3Packet
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.ClientMapInfo, New Dictionary(Of String, Object) From {
                    {"unknown", 1},
                    {"dl state", state},
                    {"total downloaded", total_downloaded}})
        End Function
        Public Shared Function MakeMapFileDataReceived(ByVal senderIndex As Byte,
                                                       ByVal receiverIndex As Byte,
                                                       ByVal totalDownloaded As UInteger) As W3Packet
            Contract.Requires(senderIndex > 0)
            Contract.Requires(senderIndex <= 12)
            Contract.Requires(receiverIndex > 0)
            Contract.Requires(receiverIndex <= 12)
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.MapFileDataReceived, New Dictionary(Of String, Object) From {
                    {"sender index", senderIndex},
                    {"receiver index", receiverIndex},
                    {"unknown", 1},
                    {"total downloaded", totalDownloaded}})
        End Function
#End Region

#Region "Packing: Lan Packets"
        Public Shared Function MakeLanCreateGame(ByVal wc3MajorVersion As UInteger,
                                                 ByVal gameId As UInteger) As W3Packet
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.LanCreateGame, New Dictionary(Of String, Object) From {
                    {"product id", "W3XP"},
                    {"major version", wc3MajorVersion},
                    {"game id", gameId}})
        End Function
        Public Shared Function MakeLanRefreshGame(ByVal gameId As UInteger,
                                                  ByVal game As IW3GameDescription) As W3Packet
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.LanRefreshGame, New Dictionary(Of String, Object) From {
                    {"game id", gameId},
                    {"num players", 0},
                    {"free slots", game.NumPlayerAndObsSlots}})
        End Function
        Public Shared Function MakeLanDescribeGame(ByVal creationTime As ModInt32,
                                                   ByVal majorVersion As UInteger,
                                                   ByVal gameId As UInteger,
                                                   ByVal game As IW3GameDescription,
                                                   ByVal listenPort As UShort,
                                                   Optional ByVal gameType As GameTypeFlags = GameTypeFlags.CreateGameUnknown0) As W3Packet
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.LanDescribeGame, New Dictionary(Of String, Object) From {
                    {"product id", "W3XP"},
                    {"major version", majorVersion},
                    {"game id", gameId},
                    {"entry key", 2642024974UI},
                    {"name", game.Name},
                    {"password", ""},
                    {"statstring", New Dictionary(Of String, Object) From {{"settings", game.Settings}, {"username", game.HostUserName}}},
                    {"num slots", game.NumPlayerAndObsSlots()},
                    {"game type", gameType},
                    {"num players + 1", 1},
                    {"free slots + 1", game.NumPlayerAndObsSlots() + 1},
                    {"age", CUInt(Environment.TickCount - creationTime)},
                    {"listen port", listenPort}})
        End Function
        Public Shared Function MakeLanDestroyGame(ByVal gameId As UInteger) As W3Packet
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.LanDestroyGame, New Dictionary(Of String, Object) From {
                    {"game id", gameId}})
        End Function
#End Region

#Region "Packing: Client Packets"
        Public Shared Function MakeKnock(ByVal name As String,
                                         ByVal listenPort As UShort,
                                         ByVal sendingPort As UShort) As W3Packet
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.Knock, New Dictionary(Of String, Object) From {
                    {"game id", 0},
                    {"entry key", 0},
                    {"unknown2", 0},
                    {"listen port", listenPort},
                    {"peer key", 0},
                    {"name", name},
                    {"unknown3", 1},
                    {"internal address", AddressJar.packIPv4Address(GetCachedIpAddressBytes(external:=True), sendingPort)}})
        End Function
        Public Shared Function MakeReady() As W3Packet
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.Ready, New Dictionary(Of String, Object))
        End Function
        Public Shared Function MakePong(ByVal salt As UInteger) As W3Packet
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.Pong, New Dictionary(Of String, Object) From {
                    {"salt", salt}})
        End Function
        Public Shared Function MakeTock(Optional ByVal checksum As Byte() = Nothing) As W3Packet
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            If checksum Is Nothing Then checksum = New Byte() {0, 0, 0, 0, 0}
            If checksum.Length <> 5 Then Throw New ArgumentException("Checksum length must be 5.")
            Return New W3Packet(W3PacketId.Tock, New Dictionary(Of String, Object) From {
                    {"game state checksum", checksum}})
        End Function
        Public Shared Function MakePeerConnectionInfo(ByVal indexes As IEnumerable(Of Byte)) As W3Packet
            Contract.Requires(indexes IsNot Nothing)
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Dim bitFlags = From index In indexes Select CUShort(1) << (index - 1)
            Dim dword = bitFlags.ReduceUsing(Function(flag1, flag2) flag1 Or flag2)

            Return New W3Packet(W3PacketId.PeerConnectionInfo, New Dictionary(Of String, Object) From {
                    {"player bitflags", dword}})
        End Function
#End Region

#Region "Packing: Peer Packets"
        Public Shared Function MakePeerKnock(ByVal receiverPeerKey As UInteger,
                                             ByVal senderId As Byte,
                                             ByVal senderPeerConnectionFlags As UInteger) As W3Packet
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.PeerKnock, New Dictionary(Of String, Object) From {
                    {"receiver peer key", receiverPeerKey},
                    {"unknown1", 0},
                    {"sender player id", senderId},
                    {"unknown3", &HFF},
                    {"sender peer connection flags", senderPeerConnectionFlags}})
        End Function
        Public Shared Function MakePeerPing(ByVal salt As Byte(),
                                           ByVal senderFlags As UInteger) As W3Packet
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.PeerPing, New Dictionary(Of String, Object) From {
                    {"salt", salt},
                    {"sender peer connection flags", senderFlags},
                    {"unknown2", 0}})
        End Function
        Public Shared Function MakePeerPong(ByVal salt As Byte()) As W3Packet
            Contract.Ensures(Contract.Result(Of W3Packet)() IsNot Nothing)
            Return New W3Packet(W3PacketId.PeerPong, New Dictionary(Of String, Object) From {
                    {"salt", salt}})
        End Function
#End Region
    End Class

#Region "Jars"
    Public Class IpBytesJar
        Inherits ArrayJar
        Public Sub New(ByVal name As String,
                       Optional ByVal info As String = "No Info")
            MyBase.New(name, expectedSize:=4, info:=info)
            Contract.Requires(name IsNot Nothing)
        End Sub
        Protected Overrides Function DescribeValue(ByVal val As Byte()) As String
            Return GetReadableIpFromBytes(val)
        End Function
    End Class

    Public Class AddressJar
        Inherits TupleJar

        Public Sub New(ByVal name As String)
            MyBase.New(name,
                    New ValueJar("protocol", 2).Weaken,
                    New ValueJar("port", 2, ByteOrder:=ByteOrder.BigEndian).Weaken,
                    New IpBytesJar("ip").Weaken,
                    New ArrayJar("unknown", 8).Weaken)
            Contract.Requires(name IsNot Nothing)
        End Sub

        Public Shared Function ExtractIPEndpoint(ByVal vals As Dictionary(Of String, Object)) As Net.IPEndPoint
            Contract.Requires(vals IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Net.IPEndPoint)() IsNot Nothing)
            Return New Net.IPEndPoint(New Net.IPAddress(CType(vals("ip"), Byte())), CUShort(vals("port")))
        End Function

        Public Shared Function packIPv4Address(ByVal address As Net.IPAddress, ByVal port As UShort) As Dictionary(Of String, Object)
            Contract.Requires(address IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Dictionary(Of String, Object))() IsNot Nothing)
            Dim bytes = address.GetAddressBytes()
            Contract.Assume(bytes IsNot Nothing)
            Return packIPv4Address(bytes, port)
        End Function
        Public Shared Function packIPv4Address(ByVal address As Net.IPEndPoint) As Dictionary(Of String, Object)
            Contract.Requires(address IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Dictionary(Of String, Object))() IsNot Nothing)
            Dim bytes = address.Address.GetAddressBytes()
            Contract.Assume(bytes IsNot Nothing)
            Return packIPv4Address(bytes, CUShort(address.Port))
        End Function
        Public Shared Function packIPv4Address(ByVal ip As Byte(), ByVal port As UShort) As Dictionary(Of String, Object)
            Contract.Requires(ip IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Dictionary(Of String, Object))() IsNot Nothing)
            Dim d As New Dictionary(Of String, Object)
            d("unknown") = New Byte() {0, 0, 0, 0, 0, 0, 0, 0}
            If ip Is Nothing Then
                d("protocol") = 0
                d("ip") = New Byte() {0, 0, 0, 0}
                d("port") = 0
            Else
                d("protocol") = 2
                d("ip") = ip
                d("port") = port
            End If
            Return d
        End Function
    End Class

    Public Class SlotJar
        Inherits TupleJar

        Public Sub New(ByVal name As String)
            MyBase.New(name,
                    New ValueJar("player index", 1).Weaken,
                    New ValueJar("dl percent", 1).Weaken,
                    New EnumJar(Of W3SlotContents.State)("slot state", 1, flags:=False).Weaken,
                    New ValueJar("is computer", 1).Weaken,
                    New ValueJar("team index", 1).Weaken,
                    New EnumJar(Of W3Slot.PlayerColor)("color", 1, flags:=False).Weaken,
                    New EnumJar(Of W3Slot.RaceFlags)("race", 1, flags:=True).Weaken,
                    New EnumJar(Of W3Slot.ComputerLevel)("computer difficulty", 1, flags:=False).Weaken,
                    New ValueJar("handicap", 1).Weaken)
        End Sub

        Public Shared Function packSlot(ByVal s As W3Slot, ByVal receiver As IW3Player) As Dictionary(Of String, Object)
            Dim vals As New Dictionary(Of String, Object)
            vals("team index") = s.team
            vals("color") = If(s.team = W3Slot.OBS_TEAM, W3Slot.OBS_TEAM, s.color)
            vals("race") = If(s.game.map.isMelee, s.race Or W3Slot.RaceFlags.Unlocked, s.race)
            vals("computer difficulty") = W3Slot.ComputerLevel.Normal
            vals("handicap") = s.handicap
            vals("is computer") = If(s.contents.Type = W3SlotContents.ContentType.Computer, 1, 0)
            vals("computer difficulty") = s.contents.DataComputerLevel
            vals("slot state") = s.contents.DataState(receiver)
            vals("player index") = s.contents.DataPlayerIndex(receiver)
            vals("dl percent") = s.contents.DataDownloadPercent(receiver)
            Return vals
        End Function
    End Class
#End Region
End Namespace
