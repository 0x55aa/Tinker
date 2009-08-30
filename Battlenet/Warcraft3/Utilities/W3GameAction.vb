﻿Namespace Warcraft3
    ''' <source>
    ''' http://www.wc3c.net/tools/specs/W3GActions.txt
    ''' </source>
    Public Enum W3GameActionId As Byte
        PauseGame = &H1
        ResumeGame = &H2
        SetGameSpeed = &H3
        IncreaseGameSpeed = &H4
        DecreaseGameSpeed = &H5
        SaveGameStarted = &H6
        SaveGameFinished = &H7
        _unseen_0x08 = &H8
        _unseen_0x09 = &H9
        _unseen_0x0A = &HA
        _unseen_0x0B = &HB
        _unseen_0x0C = &HC
        _unseen_0x0D = &HD
        _unseen_0x0E = &HE
        _unseen_0x0F = &HF
        SelfOrder = &H10
        PointOrder = &H11
        ObjectOrder = &H12
        DropOrGiveItem = &H13
        FogObjectOrder = &H14
        _unseen_0x15 = &H15
        ChangeSelection = &H16
        AssignGroupHotkey = &H17
        SelectGroupHotkey = &H18
        SelectSubgroup = &H19
        _PreSubSelection = &H1A
        _unknown0x1B = &H1B
        SelectGroundItem = &H1C
        CancelHeroRevive = &H1D
        DequeueBuildingOrder = &H1E
        _unseen_0x1F = &H1F
        CheatFastCooldown = &H20
        _unseen_0x21 = &H21
        CheatInstantDefeat = &H22
        CheatSpeedConstruction = &H23
        CheatFastDeathDecay = &H24
        CheatNoFoodLimit = &H25
        CheatGodMode = &H26
        CheatGold = &H27
        CheatLumber = &H28
        CheatUnlimitedMana = &H29
        CheatNoDefeat = &H2A
        CheatDisableVictoryConditions = &H2B
        CheatEnableResearch = &H2C
        CheatGoldAndLumber = &H2D
        CheatSetTimeOfDay = &H2E
        CheatRemoveFogOfWar = &H2F
        CheatDisableTechRequirements = &H30
        CheatResearchUpgrades = &H31
        CheatInstantVictory = &H32
        _unseen_0x33 = &H33
        _unseen_0x34 = &H34
        _unseen_0x35 = &H35
        _unseen_0x36 = &H36
        _unseen_0x37 = &H37
        _unseen_0x38 = &H38
        _unseen_0x39 = &H39
        _unseen_0x3A = &H3A
        _unseen_0x3B = &H3B
        _unseen_0x3C = &H3C
        _unseen_0x3D = &H3D
        _unseen_0x3E = &H3E
        _unseen_0x3F = &H3F
        _unseen_0x40 = &H40
        _unseen_0x41 = &H41
        _unseen_0x42 = &H42
        _unseen_0x43 = &H43
        _unseen_0x44 = &H44
        _unseen_0x45 = &H45
        _unseen_0x46 = &H46
        _unseen_0x47 = &H47
        _unseen_0x48 = &H48
        _unseen_0x49 = &H49
        _unseen_0x4A = &H4A
        _unseen_0x4B = &H4B
        _unseen_0x4C = &H4C
        _unseen_0x4D = &H4D
        _unseen_0x4E = &H4E
        _unseen_0x4F = &H4F
        ChangeAllyOptions = &H50
        TransferResources = &H51
        _unseen_0x52 = &H52
        _unseen_0x53 = &H53
        _unseen_0x54 = &H54
        _unseen_0x55 = &H55
        _unseen_0x56 = &H56
        _unseen_0x57 = &H57
        _unseen_0x58 = &H58
        _unseen_0x59 = &H59
        _unseen_0x5A = &H5A
        _unseen_0x5B = &H5B
        _unseen_0x5C = &H5C
        _unseen_0x5D = &H5D
        _unseen_0x5E = &H5E
        _unseen_0x5F = &H5F
        ChatTriggerEventFired = &H60
        PressedEscape = &H61
        ScenarioTrigger = &H62
        _unseen_0x63 = &H63
        MouseClickedTrackable = &H64
        MouseTouchedTrackable = &H65
        EnterChooseHeroSkillSubmenu = &H66
        EnterChooseBuildingSubmenu = &H67
        MinimapPing = &H68
        DialogButtonClicked2 = &H69
        DialogButtonClicked = &H6A
        GameCacheSyncInteger = &H6B
        GameCacheSyncReal = &H6C
        GameCacheSyncBoolean = &H6D
        GameCacheSyncString = &H6E 'this is a guess; the SyncStoredString function does nothing so I've never actually seen the packet
        GameCacheSyncUnit = &H6F
        _unseen_0x70 = &H70
        _unseen_0x71 = &H71
        _unseen_0x72 = &H72
        _unseen_0x73 = &H73
        _unseen_0x74 = &H74
        PressedArrowKey = &H75
    End Enum

    Public Class W3GameAction
        Public ReadOnly id As W3GameActionId
        Public ReadOnly payload As IPickle(Of Object)
        Public Shared ReadOnly packetJar As SwitchJar = MakeJar()

        Private Sub New(ByVal id As W3GameActionId, ByVal payload As IPickle(Of Object))
            Contract.Requires(payload IsNot Nothing)
            Me.payload = payload
            Me.id = id
        End Sub
        Private Sub New(ByVal id As W3GameActionId, ByVal value As Object)
            Me.New(id, packetJar.Pack(id, value))
            Contract.Requires(value IsNot Nothing)
        End Sub

        Private Shared Sub reg(ByVal jar As SwitchJar, ByVal id As W3GameActionId, ByVal ParamArray subjars() As IJar(Of Object))
            jar.reg(id, New TupleJar("data", subjars).Weaken)
        End Sub

        Public Enum GameSpeedSetting As Byte
            Slow = 0
            Normal = 1
            Fast = 2
        End Enum
        Public Enum OrderFlags As UShort
            Queue = 1 << 0
            unknown_0x0002 = 1 << 1
            unknown_0x0004 = 1 << 2
            Group = 1 << 3
            NoFormation = 1 << 4
            unknown_0x0020 = 1 << 5
            SubGroup = 1 << 6
            unknown_0x0080 = 1 << 7
            AutocastOn = 1 << 8
        End Enum
        Public Enum SelectionModification As Byte
            Add = 1
            Remove = 2
        End Enum
        Public Enum AllianceFlags As UInteger
            Allied = (1 << 0) Or (1 << 1) Or (1 << 2) Or (1 << 3) Or (1 << 4)
            SharedVision = 1 << 5
            SharedControl = 1 << 6
            AlliedVictory = 1 << 10
        End Enum
        Public Enum ArrowKeyButtons As Byte
            Left = 0
            Right = 2
            Down = 4
            Up = 6
        End Enum
        Private Shared Function MakeJar() As SwitchJar
            Dim jar = New SwitchJar()
            RegJar(jar)
            Return jar
        End Function
        Public Shared Sub RegJar(ByVal jar As SwitchJar)
            reg(jar, W3GameActionId.PauseGame)
            reg(jar, W3GameActionId.ResumeGame)
            reg(jar, W3GameActionId.SetGameSpeed, New EnumJar(Of GameSpeedSetting)("speed", 1, flags:=False).Weaken)
            reg(jar, W3GameActionId.IncreaseGameSpeed)
            reg(jar, W3GameActionId.DecreaseGameSpeed)
            reg(jar, W3GameActionId.SaveGameStarted, New StringJar("filename").Weaken)
            reg(jar, W3GameActionId.SaveGameFinished, New ValueJar("unknown1_0x01", 4).Weaken)
            reg(jar, W3GameActionId.SelfOrder,
                        New EnumJar(Of OrderFlags)("flags", 2, flags:=True).Weaken,
                        New IdValueJar("source").Weaken,
                        New IdValueJar("_unknownA").Weaken,
                        New IdValueJar("_unknownB").Weaken)
            reg(jar, W3GameActionId.PointOrder,
                        New EnumJar(Of OrderFlags)("flags", 2, flags:=True).Weaken,
                        New IdValueJar("source").Weaken,
                        New IdValueJar("_unknownA").Weaken,
                        New IdValueJar("_unknownB").Weaken,
                        New FloatSingleJar("target x").Weaken,
                        New FloatSingleJar("target y").Weaken)
            reg(jar, W3GameActionId.ObjectOrder,
                        New EnumJar(Of OrderFlags)("flags", 2, flags:=True).Weaken,
                        New IdValueJar("source").Weaken,
                        New IdValueJar("_unknownA").Weaken,
                        New IdValueJar("_unknownB").Weaken,
                        New FloatSingleJar("x").Weaken,
                        New FloatSingleJar("y").Weaken,
                        New IdValueJar("target id1").Weaken,
                        New IdValueJar("target id2").Weaken)
            reg(jar, W3GameActionId.DropOrGiveItem,
                        New EnumJar(Of OrderFlags)("flags", 2, flags:=True).Weaken,
                        New IdValueJar("source").Weaken,
                        New IdValueJar("_unknownA").Weaken,
                        New IdValueJar("_unknownB").Weaken,
                        New FloatSingleJar("x").Weaken,
                        New FloatSingleJar("y").Weaken,
                        New IdValueJar("target id1").Weaken,
                        New IdValueJar("target id2").Weaken,
                        New IdValueJar("item id1").Weaken,
                        New IdValueJar("item id2").Weaken)
            reg(jar, W3GameActionId.FogObjectOrder,
                        New EnumJar(Of OrderFlags)("flags", 2, flags:=True).Weaken,
                        New ValueJar("source id", 4).Weaken,
                        New IdValueJar("_unknownA").Weaken,
                        New IdValueJar("_unknownB").Weaken,
                        New FloatSingleJar("fog target x").Weaken,
                        New FloatSingleJar("fog target y").Weaken,
                        New IdValueJar("fog target type").Weaken,
                        New ArrayJar("unknown", expectedSize:=9).Weaken,
                        New FloatSingleJar("actual target x").Weaken,
                        New FloatSingleJar("actual target y").Weaken)
            reg(jar, W3GameActionId.ChangeSelection,
                        New EnumJar(Of SelectionModification)("mode", 1, flags:=True).Weaken,
                        New ListJar(Of Object)("targets",
                            New TupleJar("target",
                            New ValueJar("target id 1", 4).Weaken,
                            New ValueJar("target id 2", 4).Weaken).Weaken, 2).Weaken)
            reg(jar, W3GameActionId.AssignGroupHotkey,
                        New ValueJar("group index", 1).Weaken,
                        New ListJar(Of Object)("targets",
                            New TupleJar("target",
                            New ValueJar("target id 1", 4).Weaken,
                            New ValueJar("target id 2", 4).Weaken).Weaken, 2).Weaken)
            reg(jar, W3GameActionId.SelectGroupHotkey,
                        New ValueJar("group index", 1).Weaken,
                        New ValueJar("unknown1_0x03", 1).Weaken)
            reg(jar, W3GameActionId.SelectSubgroup,
                        New IdValueJar("unit type").Weaken,
                        New ValueJar("target id 1", 4).Weaken,
                        New ValueJar("target id 2", 4).Weaken)
            reg(jar, W3GameActionId._PreSubSelection)
            reg(jar, W3GameActionId.SelectGroundItem,
                        New ValueJar("unknown1_0x04", 1).Weaken,
                        New IdValueJar("target id 1").Weaken,
                        New IdValueJar("target id 2").Weaken)
            reg(jar, W3GameActionId.CancelHeroRevive,
                        New IdValueJar("unit id 1").Weaken,
                        New IdValueJar("unit id 2").Weaken)
            reg(jar, W3GameActionId.DequeueBuildingOrder,
                        New ValueJar("slot number", 1).Weaken,
                        New IdValueJar("object type").Weaken)
            reg(jar, W3GameActionId.CheatDisableTechRequirements)
            reg(jar, W3GameActionId.CheatDisableVictoryConditions)
            reg(jar, W3GameActionId.CheatEnableResearch)
            reg(jar, W3GameActionId.CheatFastCooldown)
            reg(jar, W3GameActionId.CheatFastDeathDecay)
            reg(jar, W3GameActionId.CheatGodMode)
            reg(jar, W3GameActionId.CheatInstantDefeat)
            reg(jar, W3GameActionId.CheatInstantVictory)
            reg(jar, W3GameActionId.CheatNoDefeat)
            reg(jar, W3GameActionId.CheatNoFoodLimit)
            reg(jar, W3GameActionId.CheatRemoveFogOfWar)
            reg(jar, W3GameActionId.CheatResearchUpgrades)
            reg(jar, W3GameActionId.CheatSpeedConstruction)
            reg(jar, W3GameActionId.CheatUnlimitedMana)
            reg(jar, W3GameActionId.CheatSetTimeOfDay,
                        New FloatSingleJar("time").Weaken)
            reg(jar, W3GameActionId.CheatGold,
                        New ValueJar("unknown", 1).Weaken,
                        New ValueJar("amount", 4).Weaken)
            reg(jar, W3GameActionId.CheatGoldAndLumber,
                        New ValueJar("unknown", 1).Weaken,
                        New ValueJar("amount", 4).Weaken)
            reg(jar, W3GameActionId.CheatLumber,
                        New ValueJar("unknown", 1).Weaken,
                        New ValueJar("amount", 4).Weaken)
            reg(jar, W3GameActionId.ChangeAllyOptions,
                        New ValueJar("player slot id", 1).Weaken,
                        New EnumJar(Of AllianceFlags)("flags", 4, flags:=True).Weaken)
            reg(jar, W3GameActionId.TransferResources,
                        New ValueJar("player slot id", 1).Weaken,
                        New ValueJar("gold", 4).Weaken,
                        New ValueJar("lumber", 4).Weaken)
            reg(jar, W3GameActionId.ChatTriggerEventFired,
                        New IdValueJar("trigger event id").Weaken,
                        New IdValueJar("trigger event id (again)").Weaken,
                        New StringJar("text").Weaken)
            reg(jar, W3GameActionId.PressedEscape)
            reg(jar, W3GameActionId.ScenarioTrigger,
                        New IdValueJar("unknown1").Weaken,
                        New IdValueJar("unknown2").Weaken,
                        New ValueJar("counter", 4).Weaken)
            reg(jar, W3GameActionId.EnterChooseHeroSkillSubmenu)
            reg(jar, W3GameActionId.EnterChooseBuildingSubmenu)
            reg(jar, W3GameActionId.MinimapPing,
                        New FloatSingleJar("x").Weaken,
                        New FloatSingleJar("y").Weaken,
                        New IdValueJar("unknown").Weaken)
            reg(jar, W3GameActionId.MouseTouchedTrackable,
                        New IdValueJar("trigger event id").Weaken,
                        New IdValueJar("trackable id").Weaken)
            reg(jar, W3GameActionId.MouseClickedTrackable,
                        New IdValueJar("trackable id").Weaken,
                        New IdValueJar("trigger event id").Weaken)
            reg(jar, W3GameActionId.DialogButtonClicked,
                        New RepeatedIdValueJar("dialog id").Weaken,
                        New IdValueJar("button id").Weaken,
                        New IdValueJar("trigger event id").Weaken)
            reg(jar, W3GameActionId.DialogButtonClicked2,
                        New IdValueJar("button id").Weaken,
                        New IdValueJar("trigger event id").Weaken,
                        New RepeatedIdValueJar("dialog id").Weaken)
            reg(jar, W3GameActionId.GameCacheSyncInteger,
                        New StringJar("filename").Weaken,
                        New StringJar("mission key").Weaken,
                        New StringJar("key").Weaken,
                        New ValueJar("value", 4).Weaken)
            reg(jar, W3GameActionId.GameCacheSyncBoolean,
                        New StringJar("filename").Weaken,
                        New StringJar("mission key").Weaken,
                        New StringJar("key").Weaken,
                        New ValueJar("value", 4).Weaken)
            reg(jar, W3GameActionId.GameCacheSyncReal,
                        New StringJar("filename").Weaken,
                        New StringJar("mission key").Weaken,
                        New StringJar("key").Weaken,
                        New FloatSingleJar("value").Weaken)
            reg(jar, W3GameActionId.PressedArrowKey,
                        New EnumJar(Of ArrowKeyButtons)("button", numBytes:=1, flags:=False).Weaken)

            reg(jar, W3GameActionId.GameCacheSyncUnit,
                        New StringJar("filename").Weaken,
                        New StringJar("mission key").Weaken,
                        New StringJar("key").Weaken,
                        New IdValueJar("unit type").Weaken,
                        New ArrayJar("unknown data", expectedSize:=86).Weaken)

            reg(jar, W3GameActionId.GameCacheSyncString,
                        New StringJar("filename").Weaken,
                        New StringJar("mission key").Weaken,
                        New StringJar("key").Weaken,
                        New StringJar("value").Weaken) 'this is a guess based on the other syncs; I've never actually seen this packet.


            reg(jar, W3GameActionId._unknown0x1B,
                        New ValueJar("unknown0x01", 1).Weaken,
                        New ValueJar("unknown2", 4).Weaken,
                        New ValueJar("unknown3", 4).Weaken)



            '6D 
            '74 65 73 74 2E 64 73 00 
            '00 
            '62 00 
            '01 00 00 00 

            '6B 
            '74 65 73 74 2E 64 73 00 
            '00 
            '69 00 
            '17 00 00 00 

            '6C 
            '74 65 73 74 2E 64 73 00 
            '00 
            '72 00 
            '33 33 A3 40 

            '6F
            '6E 74 65 73 74 2E 64 73 00
            '00
            '75 00
            '61 65 70 68 
            '00 00 00 00 
            '00 00 00 00 
            '00 00 00 00 
            '00 00 00 00 
            '00 00 00 00 
            '00 00 00 00 
            '00 00 00 00 
            '00 00 00 00 
            '00 00 00 00 
            '00 00 00 00 
            '00 00 00 00 
            '00 00 00 00 
            '00 00 00 00 
            '00 00 00 00 
            '00 00 00 00 
            '00 00 00 00 
            '00 00 00 00 
            '02 00 00 00 
            '00 00 00 00 
            '00 00 00 00 
            '00 00 00 00 
            '00 00




            'reg(jar,
            '1B 
            '01
            'DF 04 00 00 DF 04 00 00

            '75 00

            '1B 01 5D 04 00 00 5D 04 00 00 16 01 01 00 5D 04 00 00 5D 04 00 00 1A 19 31 30 30 68 5D 04 00 00 5D 04 00 00
        End Sub

        Public Shared Function FromData(ByVal data As ViewableList(Of Byte)) As W3GameAction
            Contract.Requires(data IsNot Nothing)
            Contract.Ensures(Contract.Result(Of W3GameAction)() IsNot Nothing)
            Dim id = CType(data(0), W3GameActionId)
            Return New W3GameAction(id, packetJar.Parse(id, data.SubView(1)))
        End Function

        Private Class RepeatedIdValueJar
            Inherits Jar(Of UInteger)

            Public Sub New(ByVal name As String)
                MyBase.New(name)
                Contract.Requires(name IsNot Nothing)
            End Sub

            Public Overrides Function Pack(Of R As UInteger)(ByVal value As R) As IPickle(Of R)
                Return New Pickle(Of R)(Me.Name, value, value.Bytes(ByteOrder.LittleEndian, 4).ToView(), Function() ToIdString(value, value))
            End Function

            Public Overrides Function Parse(ByVal data As ViewableList(Of Byte)) As IPickle(Of UInteger)
                data = data.SubView(0, 8)
                Dim value1 = data.SubView(0, 4).ToUInt32(ByteOrder.LittleEndian)
                Dim value2 = data.SubView(4, 4).ToUInt32(ByteOrder.LittleEndian)
                Return New Pickle(Of UInteger)(Me.Name, value1, data, Function() ToIdString(value1, value2))
            End Function

            Private Function ToIdString(ByVal value1 As UInteger, ByVal value2 As UInteger) As String
                If value1 = value2 Then Return value1.ToString
                Return "{0}, {1}".Frmt(value1, value2)
            End Function
        End Class
        Private Class IdValueJar
            Inherits Jar(Of UInteger)

            Public Sub New(ByVal name As String)
                MyBase.New(name)
                Contract.Requires(name IsNot Nothing)
            End Sub

            Public Overrides Function Pack(Of R As UInteger)(ByVal value As R) As IPickle(Of R)
                Return New Pickle(Of R)(Me.Name, value, value.Bytes(ByteOrder.LittleEndian, 4).ToView(), Function() ToIdString(value))
            End Function

            Public Overrides Function Parse(ByVal data As ViewableList(Of Byte)) As IPickle(Of UInteger)
                data = data.SubView(0, 4)
                Dim value = data.ToUInt32(ByteOrder.LittleEndian)
                Return New Pickle(Of UInteger)(Me.Name, value, data, Function() ToIdString(value))
            End Function

            Private Function ToIdString(ByVal val As UInteger) As String
                Dim bytes = val.Bytes(ByteOrder.LittleEndian)
                Dim id = ""
                For i = 0 To bytes.Length - 1
                    If bytes(i) >= 32 And bytes(i) < 128 Then
                        id = Chr(bytes(i)) + id
                    Else
                        id = "."c + id
                    End If
                Next i

                Return "'{0}' = {1}".Frmt(id, bytes.ToHexString)
            End Function
        End Class

        Public Overrides Function ToString() As String
            Return "{0} = {1}".Frmt(id, payload.Description.Value())
        End Function
    End Class

    Public Class W3GameActionJar
        Inherits Jar(Of W3GameAction)
        Public Sub New(ByVal name As String)
            MyBase.New(name)
        End Sub

        Public Overrides Function Pack(Of R As W3GameAction)(ByVal value As R) As Pickling.IPickle(Of R)
            Return New Pickle(Of R)(Name, value, Concat({value.id}, value.payload.Data.ToArray).ToView)
        End Function

        Public Overrides Function Parse(ByVal data As ViewableList(Of Byte)) As Pickling.IPickle(Of W3GameAction)
            Dim val = W3GameAction.FromData(data)
            Dim n = val.payload.Data.Length
            Return New Pickle(Of W3GameAction)(Name, val, data.SubView(0, n + 1))
        End Function
    End Class
End Namespace