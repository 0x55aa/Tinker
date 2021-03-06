﻿Imports Tinker.Pickling

Namespace WC3
    Public Enum DummyPlayerMode
        DownloadMap
        EnterGame
    End Enum

    'verification disabled until this class can be looked at more closely
    <ContractVerification(False)>
    Public NotInheritable Class W3DummyPlayer
        Inherits DisposableWithTask
        Private ReadOnly name As String
        Private ReadOnly listenPort As UShort
        Private ReadOnly inQueue As CallQueue
        Private ReadOnly otherPlayers As New List(Of W3Peer)
        Private ReadOnly logger As Logger
        Private ReadOnly _clock As IClock
        Private WithEvents socket As W3Socket
        Private WithEvents accepter As W3PeerConnectionAccepter
        Public readyDelay As TimeSpan = TimeSpan.Zero
        Private index As PlayerId
        Private dl As MapDownload
        Private poolPort As PortPool.PortHandle
        Private mode As DummyPlayerMode
        Private ReadOnly _playerHooks As New Dictionary(Of W3Peer, List(Of IDisposable))
        Private ReadOnly _packetHandlerLogger As PacketHandlerLogger(Of Protocol.PacketId)

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(inQueue IsNot Nothing)
            Contract.Invariant(name IsNot Nothing)
            Contract.Invariant(logger IsNot Nothing)
            Contract.Invariant(otherPlayers IsNot Nothing)
            Contract.Invariant(_playerHooks IsNot Nothing)
            Contract.Invariant(_clock IsNot Nothing)
            Contract.Invariant(_packetHandlerLogger IsNot Nothing)
        End Sub

        Public Sub New(name As InvariantString,
                       poolPort As PortPool.PortHandle,
                       clock As IClock,
                       Optional logger As Logger = Nothing,
                       Optional mode As DummyPlayerMode = DummyPlayerMode.DownloadMap)
            Me.New(name, clock, poolPort.Port, logger, mode)
            Contract.Requires(poolPort IsNot Nothing)
            Me.poolPort = poolPort
        End Sub
        Public Sub New(name As InvariantString,
                       clock As IClock,
                       Optional listenPort As UShort = 0,
                       Optional logger As Logger = Nothing,
                       Optional mode As DummyPlayerMode = DummyPlayerMode.DownloadMap)
            Me.accepter = New W3PeerConnectionAccepter(clock)
            Me.name = name
            Me.mode = mode
            Me._clock = clock
            Me.listenPort = listenPort
            Me.inQueue = MakeTaskedCallQueue()
            Me.logger = If(logger, New Logger)
            If listenPort <> 0 Then accepter.Accepter.OpenPort(listenPort)
            Me._packetHandlerLogger = Protocol.MakeW3PacketHandlerLogger("?", Me.logger)
        End Sub

#Region "Networking"
        Private Function AddPacketHandler(Of T)(packet As Protocol.Packets.Definition(Of T),
                                                handler As Func(Of IPickle(Of T), Task)) As IDisposable
            Contract.Requires(packet IsNot Nothing)
            Contract.Requires(handler IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IDisposable)() IsNot Nothing)
            Return _packetHandlerLogger.IncludeHandler(packet.Id, packet.Jar, handler)
        End Function
        Private Function AddQueuedPacketHandler(Of T)(packet As Protocol.Packets.Definition(Of T),
                                                      handler As Action(Of IPickle(Of T))) As IDisposable
            Contract.Requires(packet IsNot Nothing)
            Contract.Requires(handler IsNot Nothing)
            Contract.Ensures(Contract.Result(Of IDisposable)() IsNot Nothing)
            Return AddPacketHandler(packet, Function(pickle) inQueue.QueueAction(Sub() handler(pickle)))
        End Function

        Public Function QueueConnect(hostName As String, port As UShort) As Task
            Contract.Requires(hostName IsNot Nothing)
            Contract.Ensures(Contract.Result(Of Task)() IsNot Nothing)
            Return inQueue.QueueAction(Sub()
                                           Contract.Assume(hostName IsNot Nothing)
                                           Connect(hostName, port)
                                       End Sub)
        End Function
        Private Async Sub Connect(hostName As String, port As UShort)
            Contract.Assume(hostName IsNot Nothing)

            Dim tcp = New Net.Sockets.TcpClient()
            tcp.Connect(hostName, port)
            socket = New W3Socket(New PacketSocket(stream:=tcp.GetStream,
                                                   localendpoint:=DirectCast(tcp.Client.LocalEndPoint, Net.IPEndPoint),
                                                   remoteendpoint:=DirectCast(tcp.Client.RemoteEndPoint, Net.IPEndPoint),
                                                   timeout:=60.Seconds,
                                                   logger:=Me.logger,
                                                   clock:=_clock))

            AddQueuedPacketHandler(Protocol.ServerPackets.Greet, AddressOf OnReceiveGreet)
            AddQueuedPacketHandler(Protocol.ServerPackets.HostMapInfo, AddressOf OnReceiveHostMapInfo)
            AddQueuedPacketHandler(Protocol.ServerPackets.Ping, AddressOf OnReceivePing)
            AddQueuedPacketHandler(Protocol.ServerPackets.OtherPlayerJoined, AddressOf OnReceiveOtherPlayerJoined)
            AddQueuedPacketHandler(Protocol.ServerPackets.OtherPlayerLeft, AddressOf OnReceiveOtherPlayerLeft)
            AddQueuedPacketHandler(Protocol.ServerPackets.StartLoading, AddressOf OnReceiveStartLoading)
            AddQueuedPacketHandler(Protocol.ServerPackets.Tick, AddressOf OnReceiveTick)
            AddQueuedPacketHandler(Protocol.PeerPackets.MapFileData, AddressOf OnReceiveMapFileData)

            socket.SendPacket(Protocol.MakeKnock(name, listenPort, CUShort(socket.LocalEndPoint.Port)))

            Try
                Do
                    Dim data = Await _socket.AsyncReadPacket
                    Await _packetHandlerLogger.HandlePacket(data)
                Loop
            Catch ex As Exception
                'ignore (to match old behavior, should fix)
            End Try
        End Sub
        Private Sub OnReceiveGreet(pickle As IPickle(Of NamedValueMap))
            Contract.Requires(pickle IsNot Nothing)
            Me.index = pickle.Value.ItemAs(Of PlayerId)("player index")
        End Sub
        Private Sub OnReceiveHostMapInfo(pickle As IPickle(Of NamedValueMap))
            Contract.Requires(pickle IsNot Nothing)
            If mode = DummyPlayerMode.DownloadMap Then
                dl = New MapDownload(pickle.Value.ItemAs(Of String)("path"),
                                     pickle.Value.ItemAs(Of UInt32)("size"),
                                     pickle.Value.ItemAs(Of UInt32)("crc32"),
                                     pickle.Value.ItemAs(Of UInt32)("xoro checksum"),
                                     pickle.Value.ItemAs(Of IRist(Of Byte))("sha1 checksum"))
                socket.SendPacket(Protocol.MakeClientMapInfo(Protocol.MapTransferState.Idle, 0))
            Else
                socket.SendPacket(Protocol.MakeClientMapInfo(Protocol.MapTransferState.Idle, pickle.Value.ItemAs(Of UInt32)("size")))
            End If
        End Sub
        Private Sub OnReceivePing(pickle As IPickle(Of UInt32))
            Contract.Requires(pickle IsNot Nothing)
            socket.SendPacket(Protocol.MakePong(pickle.Value))
        End Sub
        Private Sub OnReceiveOtherPlayerJoined(pickle As IPickle(Of NamedValueMap))
            Contract.Requires(pickle IsNot Nothing)
            Dim ext_addr = pickle.Value.ItemAs(Of Net.IPEndPoint)("external address")
            Dim player = New W3Peer(pickle.Value.ItemAs(Of String)("name"),
                                    pickle.Value.ItemAs(Of PlayerId)("joiner id"),
                                    CUShort(ext_addr.Port),
                                    ext_addr.Address,
                                    pickle.Value.ItemAs(Of UInt32)("peer key"),
                                    New Logger)
            otherPlayers.Add(player)
            Dim hooks = New List(Of IDisposable)
            hooks.Add(player.AddPacketHandler(Protocol.PeerPackets.PeerPing, Function(value) inQueue.QueueAction(Sub() OnPeerReceivePeerPing(player, value))))
            hooks.Add(player.AddPacketHandler(Protocol.PeerPackets.MapFileData, Function(value) inQueue.QueueAction(Sub() OnPeerReceiveMapFileData(player, value))))
            AddHandler player.Disconnected, AddressOf OnPeerDisconnect
            hooks.Add(New DelegatedDisposable(Sub() RemoveHandler player.Disconnected, AddressOf OnPeerDisconnect))
            _playerHooks(player) = hooks
        End Sub
        Private Sub OnReceiveOtherPlayerLeft(pickle As IPickle(Of NamedValueMap))
            Dim player = (From p In otherPlayers Where p.Id = pickle.Value.ItemAs(Of PlayerId)("leaver")).FirstOrDefault
            If player IsNot Nothing Then
                otherPlayers.Remove(player)
                For Each e In _playerHooks(player).AssumeNotNull()
                    e.AssumeNotNull().Dispose()
                Next e
                _playerHooks.Remove(player)
            End If
        End Sub
        Private Async Sub OnReceiveStartLoading(pickle As IPickle(Of NoValue))
            If mode = DummyPlayerMode.DownloadMap Then
                Disconnect(expected:=False, reason:="Dummy player is in download mode but game is starting.")
            ElseIf mode = DummyPlayerMode.EnterGame Then
                Await _clock.Delay(readyDelay)
                socket.SendPacket(Protocol.MakeReady())
            End If
        End Sub
        Private Sub OnReceiveTick(pickle As IPickle(Of NamedValueMap))
            Contract.Requires(pickle IsNot Nothing)
            If pickle.Value.ItemAs(Of UInt16)("time span") > 0 Then
                socket.SendPacket(Protocol.MakeTock(0, 0))
            End If
        End Sub
        Private Sub OnReceiveMapFileData(pickle As IPickle(Of NamedValueMap))
            Contract.Requires(pickle IsNot Nothing)
            Dim pos = CUInt(dl.file.Position)
            If ReceiveDLMapChunk(pickle.Value) Then
                Disconnect(expected:=True, reason:="Download finished.")
            Else
                socket.SendPacket(Protocol.MakeMapFileDataReceived(New PlayerId(1), Me.index, pos))
            End If
        End Sub

        Private Function ReceiveDLMapChunk(vals As NamedValueMap) As Boolean
            Contract.Requires(vals IsNot Nothing)
            If dl Is Nothing OrElse dl.file Is Nothing Then Throw New InvalidOperationException()
            Dim position = CInt(vals.ItemAs(Of UInt32)("file position"))
            Dim fileData = vals.ItemAs(Of IRist(Of Byte))("file data")
            Contract.Assume(position > 0)
            Contract.Assume(fileData IsNot Nothing)

            If dl.ReceiveChunk(position, fileData) Then
                socket.SendPacket(Protocol.MakeClientMapInfo(Protocol.MapTransferState.Idle, dl.size))
                Return True
            Else
                socket.SendPacket(Protocol.MakeClientMapInfo(Protocol.MapTransferState.Downloading, CUInt(dl.file.Position)))
                Return False
            End If
        End Function
        Private Sub SendPlayersConnected()
            socket.SendPacket(Protocol.MakePeerConnectionInfo(From p In otherPlayers Where p.Socket IsNot Nothing Select p.Id))
        End Sub

        Private Sub OnDisconnect(sender As W3Socket, expected As Boolean, reason As String) Handles socket.Disconnected
            inQueue.QueueAction(Sub()
                                    Contract.Assume(reason IsNot Nothing)
                                    Disconnect(expected, reason)
                                End Sub)
        End Sub
        Private Sub Disconnect(expected As Boolean, reason As String)
            Contract.Requires(reason IsNot Nothing)
            socket.Disconnect(expected, reason)
            accepter.Accepter.CloseAllPorts()
            For Each player In otherPlayers
                If player.Socket IsNot Nothing Then
                    player.Socket.Disconnect(expected, reason)
                    player.SetSocket(Nothing)
                    For Each e In _playerHooks(player)
                        e.Dispose()
                    Next e
                    _playerHooks.Remove(player)
                End If
            Next player
            otherPlayers.Clear()
            If poolPort IsNot Nothing Then
                poolPort.Dispose()
                poolPort = Nothing
            End If
        End Sub
#End Region

#Region "Peer Networking"
        Private Sub OnPeerConnection(sender As W3PeerConnectionAccepter,
                                     acceptedPlayer As W3ConnectingPeer) Handles accepter.Connection
            inQueue.QueueAction(
                Sub()
                    Dim player = (From p In otherPlayers Where p.Id = acceptedPlayer.id).FirstOrDefault
                    Dim socket = acceptedPlayer.socket
                    If player Is Nothing Then
                        Dim msg = "{0} was not another player in the game.".Frmt(socket.Name)
                        logger.Log(msg, LogMessageType.Negative)
                        socket.Disconnect(expected:=True, reason:=msg)
                    Else
                        logger.Log("{0} is a peer connection from {1}.".Frmt(socket.Name, player.name), LogMessageType.Positive)
                        socket.Name = player.name
                        player.SetSocket(socket)
                        socket.SendPacket(Protocol.MakePeerKnock(player.peerKey, Me.index, {}))
                    End If
                End Sub
            )
        End Sub

        Private Sub OnPeerDisconnect(sender As W3Peer, expected As Boolean, reason As String)
            inQueue.QueueAction(
                Sub()
                    logger.Log("{0}'s peer connection has closed ({1}).".Frmt(sender.name, reason), LogMessageType.Negative)
                    sender.SetSocket(Nothing)
                    SendPlayersConnected()
                End Sub
            )
        End Sub

        Private Sub OnPeerReceivePeerPing(sender As W3Peer,
                                          pickle As IPickle(Of NamedValueMap))
            Contract.Requires(sender IsNot Nothing)
            Contract.Requires(pickle IsNot Nothing)
            Dim vals = pickle.Value
            sender.Socket.SendPacket(Protocol.MakePeerPing(vals.ItemAs(Of UInt32)("salt"), {New PlayerId(1)}))
            sender.Socket.SendPacket(Protocol.MakePeerPong(vals.ItemAs(Of UInt32)("salt")))
        End Sub
        Private Sub OnPeerReceiveMapFileData(sender As W3Peer,
                                             pickle As IPickle(Of NamedValueMap))
            Contract.Requires(sender IsNot Nothing)
            Contract.Requires(pickle IsNot Nothing)
            Dim vals = pickle.Value
            Dim pos = CUInt(dl.file.Position)
            If ReceiveDLMapChunk(vals) Then
                Disconnect(expected:=True, reason:="Download finished.")
            Else
                sender.Socket.SendPacket(Protocol.MakeMapFileDataReceived(sender.Id, Me.index, pos))
            End If
        End Sub
#End Region

        Protected Overrides Function PerformDispose(finalizing As Boolean) As Task
            If finalizing Then Return Nothing
            If dl IsNot Nothing Then dl.Dispose()
            Return Nothing
        End Function
    End Class
End Namespace