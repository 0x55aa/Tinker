Imports Tinker.Bnet.Protocol

Namespace Bnet
    'Verification disabled because of many warnings in generated code
    <ContractVerification(False)>
    Public Class BnetClientControl
        Private ReadOnly inQueue As CallQueue = MakeControlCallQueue(Me)
        Private ReadOnly _component As Bnet.ClientComponent
        Private ReadOnly _client As Bnet.Client
        Private ReadOnly _life As New CancellationTokenSource()
        Private numPrimaryStates As Integer

        <ContractInvariantMethod()> Private Sub ObjectInvariant()
            Contract.Invariant(inQueue IsNot Nothing)
            Contract.Invariant(_component IsNot Nothing)
            Contract.Invariant(_client IsNot Nothing)
            Contract.Invariant(_life IsNot Nothing)
        End Sub

        Public Shared Async Function FromComponentAsync(component As Bnet.ClientComponent) As Task(Of BnetClientControl)
            Await component.Bot.UIContext.AwaitableEntrance()
            Return New BnetClientControl(component)
        End Function
        Private Sub New(component As Bnet.ClientComponent)
            Contract.Assert(component IsNot Nothing)
            InitializeComponent()

            Me._client = component.Client
            Me._component = component

            Init()
        End Sub

        Private Sub Init()
            logClient.SetLogger(Me._client.Logger, "Client")

            _client.IncludePacketHandlerAsync(Packets.ServerToClient.ChatEvent, AddressOf OnClientReceivedChatEventAsync, _life.Token)
            _client.IncludePacketHandlerAsync(Packets.ServerToClient.QueryGamesList(_client.Clock), AddressOf OnClientReceivedQueryGamesListAsync, _life.Token)

            Call Async Sub()
                     Dim state = Await _client.GetStateAsync()
                     OnClientStateChangedAsync(_client, state, state)
                 End Sub
            AddHandler _client.StateChanged, AddressOf OnClientStateChangedAsync
            AddHandler _client.AdvertisedGame, AddressOf OnClientAdvertisedGameAsync
            _life.Token.Register(Sub() RemoveHandler _client.StateChanged, AddressOf OnClientStateChangedAsync)
            _life.Token.Register(Sub() RemoveHandler _client.AdvertisedGame, AddressOf OnClientAdvertisedGameAsync)
        End Sub

        Private Sub BnetClientControl_Disposed() Handles Me.Disposed
            _life.Cancel()
        End Sub

        Private Async Function OnClientReceivedQueryGamesListAsync(value As QueryGamesListResponse) As Task
            Await inQueue
            While lstState.Items.Count > numPrimaryStates
                lstState.Items.RemoveAt(lstState.Items.Count - 1)
            End While
            lstState.Items.Add("--------")
            lstState.Items.Add("Games List")
            lstState.Items.Add(Date.Now().ToString("hh:mm:ss", Globalization.CultureInfo.CurrentCulture))
            For Each game In value.Games
                lstState.Items.Add("---")
                lstState.Items.Add(game.Name)
                lstState.Items.Add(game.GameStats.HostName)
                lstState.Items.Add(game.GameStats.AdvertisedPath.ToString.Split("\"c).Last)
            Next game
        End Function
        Private Async Function OnClientReceivedChatEventAsync(vals As NamedValueMap) As Task
            Await inQueue
            If IsDisposed Then Return
            Dim id = vals.ItemAs(Of ChatEventId)("event id")
            Dim user = vals.ItemAs(Of String)("username")
            Dim text = vals.ItemAs(Of String)("text")
            Select Case id
                Case ChatEventId.ShowUser, ChatEventId.UserJoined
                    If Not lstState.Items.Contains(user) OrElse lstState.Items.IndexOf(user) >= numPrimaryStates Then
                        lstState.Items.Insert(numPrimaryStates, user)
                        numPrimaryStates += 1
                    End If
                    logClient.LogMessage("{0} entered the channel".Frmt(user), Color.LightGray)
                Case ChatEventId.UserLeft
                    If lstState.Items.Contains(user) AndAlso lstState.Items.IndexOf(user) < numPrimaryStates Then
                        numPrimaryStates -= 1
                        lstState.Items.Remove(user)
                    End If
                    logClient.LogMessage("{0} left the channel".Frmt(user), Color.LightGray)
                Case ChatEventId.Channel
                    logClient.LogMessage("--- Entered Channel: " + text, Color.DarkGray)
                    lstState.Items.Clear()
                    lstState.Items.Add("Channel " + text)
                    lstState.Items.Add(New String("-"c, 50))
                    numPrimaryStates = 2
                Case ChatEventId.Whisper
                    logClient.LogMessage("{0} whispers: {1}".Frmt(user, text), Color.DarkGreen)
                Case ChatEventId.Talk
                    logClient.LogMessage("{0}: {1}".Frmt(user, text), Color.Black)
                Case ChatEventId.Broadcast
                    logClient.LogMessage("(server broadcast) {0}: {1}".Frmt(user, text), Color.Red)
                Case ChatEventId.Channel
                    logClient.LogMessage("Entered channel {0}".Frmt(text), Color.DarkGray)
                Case ChatEventId.WhisperSent
                    logClient.LogMessage("You whisper to {0}: {1}".Frmt(user, text), Color.DarkGreen)
                Case ChatEventId.ChannelFull
                    logClient.LogMessage("Channel was full", Color.Red)
                Case ChatEventId.ChannelDoesNotExist
                    logClient.LogMessage("Channel didn't exist", Color.Red)
                Case ChatEventId.ChannelRestricted
                    logClient.LogMessage("Channel was restricted", Color.Red)
                Case ChatEventId.Info
                    logClient.LogMessage(text, Color.Gray)
                Case ChatEventId.Errors
                    logClient.LogMessage(text, Color.Red)
                Case ChatEventId.Emote
                    logClient.LogMessage("{0} {1}".Frmt(user, text), Color.DarkGray)
            End Select
        End Function

        Private Async Sub txtTalk_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles txtTalk.KeyDown
            If e.KeyCode <> Keys.Enter Then Return
            If e.Shift Then Return
            If txtTalk.Text = "" Then Return
            If _client Is Nothing Then Return
            e.Handled = True
            e.SuppressKeyPress = True
            Dim msg = txtTalk.Text
            txtTalk.Text = ""
            Try
                Await _client.SendTextAsync(txtTalk.Text)
                logClient.LogMessage("{0}: {1}".Frmt(_client.UserName, msg), Color.DarkBlue)
            Catch ex As Exception
                logClient.LogMessage("Error sending text: {0}".Frmt(ex.Summarize), Color.Red)
                ex.RaiseAsUnexpected("Sending bnet client text.")
            End Try
        End Sub

        Private Async Sub OnClientStateChangedAsync(sender As Bnet.Client,
                                                    oldState As Bnet.ClientState,
                                                    newState As Bnet.ClientState)
            Contract.Requires(sender IsNot Nothing)
            Await inQueue

            If IsDisposed Then Return
            If sender IsNot _client Then Return
            txtTalk.Enabled = False
            lstState.Enabled = True
            lstState.BackColor = SystemColors.Window
            Select Case newState
                Case Bnet.ClientState.Channel, Bnet.ClientState.CreatingGame
                    If oldState = Bnet.ClientState.AdvertisingGame Then lstState.Items.Clear()
                    txtTalk.Enabled = True
                Case Bnet.ClientState.AdvertisingGame
                    'advertised event will handle it
                Case Else
                    lstState.Items.Clear()
                    lstState.Enabled = False
                    lstState.BackColor = SystemColors.ButtonFace
            End Select
        End Sub
        Private Async Sub OnClientAdvertisedGameAsync(sender As Bnet.Client,
                                                      gameDescription As WC3.LocalGameDescription,
                                                      [private] As Boolean,
                                                      refreshed As Boolean)
            Contract.Requires(sender IsNot Nothing)
            Contract.Requires(gameDescription IsNot Nothing)
            Await inQueue

            If IsDisposed Then Return
            If sender IsNot _client Then Return
            txtTalk.Enabled = False
            lstState.Enabled = True
            lstState.BackColor = SystemColors.Window

            lstState.Items.Clear()
            lstState.Items.Add("Game")
            lstState.Items.Add(gameDescription.Name)
            lstState.Items.Add(gameDescription.GameStats.AdvertisedPath)
            lstState.Items.Add(If([private], "Private", "Public"))
            lstState.Items.Add("{0}: {1}".Frmt(If(refreshed, "Refreshed", "Created"),
                                               DateTime.Now.ToString("hh:mm:ss", Globalization.CultureInfo.CurrentCulture)))
            numPrimaryStates = lstState.Items.Count
        End Sub

        Private Sub OnIssuedCommand(sender As CommandControl, argument As String) Handles comClient.IssuedCommand
            Contract.Requires(argument IsNot Nothing)
            Tinker.Components.UIInvokeCommand(_component, argument)
        End Sub
    End Class
End Namespace
