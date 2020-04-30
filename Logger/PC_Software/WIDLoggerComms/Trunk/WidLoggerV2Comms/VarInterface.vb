Imports WidLoggerV2Comms.SerComms


Public Enum VarNameList As Integer
    DevVersion
    LoggerName
    LoggerId
    Records
    RTCTime
    LoggerGroupId
    WarningSignalDuration

    RFChan
    RFID
    RFFilter
    VerboseMode
    RFParams
    InterLogRFParams

    SendEmailPeriod
    SendEmailNext
    EmailToAddress
    GPRSSettings
    NextDataReset
    MonthlyData
End Enum

Public Enum VarCatPage As Integer
    General
    RFSettings
    GSMSettings
End Enum

Public Class VarInterface
    Public Delegate Function TestFunc_Type(ByVal sender As VarInterface) As Boolean
    Public Delegate Function GetHandlerFunc_Type(ByVal sender As VarInterface, ByRef retVals() As Object) As Boolean
    Public Delegate Function SetHandlerFunc_Type(ByVal sender As VarInterface) As Object()
    Public Delegate Function PostSetHandlerFunc_Type(ByVal sender As VarInterface)

    Public Enum VarState As Integer
        VarState_OutDated = 0   '(Yellow) Value shown has been modified and no longer reflects the value on the device
        VarState_Current = 1    '(Green)  Value shown matches the value on the device
        VarState_Unknown = 2    '(White)  Error reading device value
        VarState_WriteError = 3 '(Red)    Error writing device value
    End Enum

    Public PicB As PictureBox
    Public WithEvents Control As Control
    Public ID As Integer
    Public Category As Integer
    Private SerComLib As SerComms
    Private GetCMD As SerDev_LoggerV2_ComFuncs.CMD_ByteID
    Private SetCMD As SerDev_LoggerV2_ComFuncs.CMD_ByteID
    Public UpdateCurVal As Boolean = False
    Public ComboBoxUseTextAsProperty As Boolean = False

    Public FormatTestFunc As TestFunc_Type
    Public GetHandlerFunc As GetHandlerFunc_Type
    Public SetHandlerFunc As SetHandlerFunc_Type
    Public PostSetHandlerFunc As SetHandlerFunc_Type = Nothing

    Public OrigVal As Object = Nothing
    Private _State As VarState
    Private _ReadOnly As Boolean = False
    Private _isProgrammer As Boolean = False

    Public Sub AddCtrlToHandle(ByVal ctrl As Object)
        Select Case ctrl.GetType.ToString
            Case "System.Windows.Forms.TextBox"
                AddHandler CType(ctrl, TextBox).TextChanged, AddressOf CtrlEditHandler
            Case "System.Windows.Forms.DateTimePicker"
                AddHandler CType(ctrl, DateTimePicker).ValueChanged, AddressOf CtrlEditHandler
            Case "System.Windows.Forms.NumericUpDown"
                AddHandler CType(ctrl, NumericUpDown).ValueChanged, AddressOf CtrlEditHandler
            Case "System.Windows.Forms.ComboBox"
                AddHandler CType(ctrl, ComboBox).SelectedIndexChanged, AddressOf CtrlEditHandler
                AddHandler CType(ctrl, ComboBox).TextChanged, AddressOf CtrlEditHandler
        End Select

        AddHandler CType(ctrl, Control).KeyUp, AddressOf CtrlKeyUpHandler
    End Sub

    Private Sub Init(ByVal mID As Integer, ByVal cat As Integer, ByVal fmtTstFunc As TestFunc_Type, ByRef SerComsLib As SerComms, ByVal Get_Cmd As SerDev_LoggerV2_ComFuncs.CMD_ByteID, ByVal Set_Cmd As SerDev_LoggerV2_ComFuncs.CMD_ByteID)
        ID = mID
        State = VarState.VarState_Unknown
        FormatTestFunc = fmtTstFunc
        SerComLib = SerComsLib
        GetCMD = Get_Cmd
        SetCMD = Set_Cmd
        Category = cat

        If Set_Cmd = SerDev_LoggerV2_ComFuncs.CMD_ByteID.NONE Then
            _ReadOnly = True
            SetControlReadOnly()
        End If

        AddCtrlToHandle(Control)

    End Sub

    Public Sub New(ByVal mID As Integer, ByVal cat As Integer, ByRef pb As PictureBox, ByRef ctrl As Control, ByVal fmtTstFunc As TestFunc_Type, ByRef SerComsLib As SerComms, ByVal Get_Cmd As SerDev_LoggerV2_ComFuncs.CMD_ByteID, ByVal Set_Cmd As SerDev_LoggerV2_ComFuncs.CMD_ByteID, Optional isProgrammer As Boolean = False)
        PicB = pb
        Control = ctrl
        _isProgrammer = isProgrammer

        Init(mID, cat, fmtTstFunc, SerComsLib, Get_Cmd, Set_Cmd)

    End Sub

    Public Sub New(ByVal mID As Integer, ByVal cat As Integer, ByRef tlpParent As TableLayoutPanel, ByVal Row As Integer, ByVal labelText As String, ByVal fmtTstFunc As TestFunc_Type, ByRef SerComsLib As SerComms, ByVal Get_Cmd As SerDev_LoggerV2_ComFuncs.CMD_ByteID, ByVal Set_Cmd As SerDev_LoggerV2_ComFuncs.CMD_ByteID, Optional isProgrammer As Boolean = False)

        _isProgrammer = isProgrammer

        Dim tmpLbl As New Label()
        tmpLbl.Text = labelText
        Dim TLPtmp As New TableLayoutPanel()
        TLPtmp.Padding = New Padding(0)
        PicB = New PictureBox()
        PicB.Margin = New Padding(0)
        PicB.BackgroundImageLayout = ImageLayout.Stretch
        Control = New TextBox()
        CType(Control, TextBox).Margin = New Padding(3, 0, 0, 0)

        TLPtmp.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 20))
        TLPtmp.Controls.Add(PicB, 0, 0)
        TLPtmp.Controls.Add(Control, 1, 0)
        CType(Control, TextBox).Dock = DockStyle.Fill
        'PicB.Dock = DockStyle.Fill
        PicB.Width = 20
        PicB.Height = 20

        If Row > tlpParent.RowCount - 1 Then
            tlpParent.RowStyles.Add(New RowStyle(SizeType.Absolute, 26))
        End If
        tlpParent.Controls.Add(tmpLbl, 0, Row)
        tlpParent.Controls.Add(TLPtmp, 1, Row)

        TLPtmp.Dock = DockStyle.Fill
        tmpLbl.Dock = DockStyle.Fill
        tmpLbl.TextAlign = ContentAlignment.MiddleLeft

        Init(mID, cat, fmtTstFunc, SerComsLib, Get_Cmd, Set_Cmd)

    End Sub

    Private Sub CtrlEditHandler(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim FormatGood As Boolean = True

        If FormatTestFunc <> Nothing Then
            FormatGood = FormatTestFunc(Me)
        End If

        Try

            If FormatGood Then
                If _ReadOnly = True Then
                    CType(Control, Control).BackColor = Color.WhiteSmoke  'SystemColors.Control
                Else
                    CType(Control, Control).BackColor = Color.White
                End If
            Else
                CType(Control, Control).BackColor = Color.Red
            End If
        Catch ex As Exception
            Stop
        End Try

        'Skip the entire test??
        If UpdateCurVal = True Then
            OrigVal = Value
            UpdateCurVal = False
        End If

        If State = VarState.VarState_Current Then
            'value was current
            Try
                If Value <> OrigVal Then
                    'value has now changed
                    'set to outdated
                    State = VarState.VarState_OutDated
                End If
            Catch ex As Exception
                State = VarState.VarState_OutDated
            End Try
        ElseIf State = VarState.VarState_OutDated Then
            'value was outdated
            If Value = OrigVal Then
                'Match the actual device value
                'set back to Current value
                State = VarState.VarState_Current
            End If
        End If
    End Sub

    Private Sub CtrlKeyUpHandler(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs)
        If e.KeyCode = Keys.Escape Then
            'If State = VarState.VarState_OutDated Then
            ReadFromDev()
            'Value = OrigVal
            'State = VarState.VarState_Current
            'End If
            'if key = enter - update value now! ??
        End If
    End Sub

    Private Sub SetControlReadOnly()
        Select Case Control.GetType.ToString
            Case "System.Windows.Forms.TextBox"
                CType(Control, TextBox).ReadOnly = True
                'CType(Control, TextBox).BackColor = Color.LightGray
            Case "System.Windows.Forms.DateTimePicker"
                CType(Control, DateTimePicker).Enabled = False
            Case "System.Windows.Forms.NumericUpDown"
                CType(Control, NumericUpDown).ReadOnly = True
                'CType(Control, NumericUpDown).BackColor = Color.Gray
            Case "System.Windows.Forms.ComboBox"
                CType(Control, ComboBox).Enabled = False
            Case Else
                CType(Control, Control).Enabled = False
        End Select
    End Sub

    Public Property Value As Object
        Get
            Select Case Control.GetType.ToString
                Case "System.Windows.Forms.TextBox"
                    Return CType(Control, TextBox).Text
                Case "System.Windows.Forms.DateTimePicker"
                    Return CType(Control, DateTimePicker).Value
                Case "System.Windows.Forms.NumericUpDown"
                    Return CType(Control, NumericUpDown).Value
                Case "System.Windows.Forms.ComboBox"
                    If ComboBoxUseTextAsProperty = True Then
                        Return CType(Control, ComboBox).Text
                    Else
                        Return CType(Control, ComboBox).SelectedIndex
                    End If

            End Select
            Return Nothing
        End Get
        Set(ByVal value As Object)
            OrigVal = value
            Try
                If value Is Nothing Then
                    Select Case Control.GetType.ToString
                        Case "System.Windows.Forms.TextBox"
                            CType(Control, TextBox).Text = ""
                        Case "System.Windows.Forms.DateTimePicker"
                            CType(Control, DateTimePicker).Value = CType(Control, DateTimePicker).MinDate
                        Case "System.Windows.Forms.NumericUpDown"
                            CType(Control, NumericUpDown).Value = 0
                        Case "System.Windows.Forms.ComboBox"
                            CType(Control, ComboBox).Text = "INVALID"
                    End Select
                Else
                    Select Case Control.GetType.ToString
                        Case "System.Windows.Forms.TextBox"
                            CType(Control, TextBox).Text = CType(value, String)
                        Case "System.Windows.Forms.DateTimePicker"
                            CType(Control, DateTimePicker).Value = CType(value, DateTime)
                        Case "System.Windows.Forms.NumericUpDown"
                            CType(Control, NumericUpDown).Value = CType(value, Decimal)
                        Case "System.Windows.Forms.ComboBox"
                            If ComboBoxUseTextAsProperty = True Then
                                CType(Control, ComboBox).Text = CType(value, String)
                            Else
                                CType(Control, ComboBox).SelectedIndex = CType(value, Integer)
                            End If
                    End Select
                End If

                'If _ReadOnly = True Then SetControlReadOnly()

            Catch ex As Exception
                State = VarState.VarState_Unknown
            End Try
        End Set
    End Property

    Public Property State As VarState
        Get
            Return _State
        End Get
        Set(ByVal value As VarState)
            _State = value
            Select Case value
                Case VarState.VarState_Current
                    'PicB.BackColor = Color.Green
                    PicB.BackgroundImage = My.Resources.Green

                Case VarState.VarState_OutDated
                    'PicB.BackColor = Color.Red
                    PicB.BackgroundImage = My.Resources.Yellow

                Case VarState.VarState_Unknown
                    'PicB.BackColor = Color.White
                    PicB.BackgroundImage = My.Resources.White
                    'Me.Value = Nothing

                Case VarState.VarState_WriteError
                    'PicB.BackColor = Color.Black
                    PicB.BackgroundImage = My.Resources.Red

                Case Else
                    'PicB.BackColor = Color.White
                    PicB.BackgroundImage = My.Resources.White
                    _State = VarState.VarState_Unknown
                    'Me.Value = Nothing
            End Select
        End Set
    End Property

    Public Function ReadFromDev(Optional retries As Integer = 3) As MyErr_Enum
        Dim tmpErr As MyErr_Enum = MyErr_Enum.MyErr_Unknown
        Dim RetVals(0) As Object

        Do While (retries And tmpErr <> MyErr_Enum.MyErr_Success)
            'If State <> VarState.VarState_Current Then

            If waitforParam(SerComLib.Port_InUse, False, 5000) = False Then Return MyErr_Enum.MyErr_PortInUse
            SerComLib.Port_InUse = True

            SerComLib.Ser_Timeout = 500
            tmpErr = SerComLib.Ser_SendCmd_NS(GetCMD, Nothing, RetVals)
            retries = retries - 1
        Loop

        If tmpErr < 0 Then
            State = VarState.VarState_Unknown
            Return tmpErr
        Else
            If _isProgrammer = True Then
                'UpdateCurVal = True
                If GetHandlerFunc <> Nothing Then
                    If GetHandlerFunc(Me, RetVals) = True Then
                        State = VarState.VarState_Current
                    End If
                Else
                    If Value = RetVals(0) Then
                        State = VarState.VarState_Current
                    Else
                        State = VarState.VarState_Unknown
                    End If
                End If
            Else
                'UpdateCurVal = True
                If GetHandlerFunc <> Nothing Then
                    If GetHandlerFunc(Me, RetVals) = True Then
                        State = VarState.VarState_Current
                    End If
                Else
                    Value = RetVals(0)
                    State = VarState.VarState_Current
                End If
            End If
        End If
        Return MyErr_Enum.MyErr_Success

    End Function

    Public ReadOnly Property isReadOnly As Boolean
        Get
            Return _ReadOnly
        End Get
    End Property


    Public Function WriteToDev(Optional retries As Integer = 3) As MyErr_Enum
        Dim tmpErr As MyErr_Enum = MyErr_Enum.MyErr_Unknown
        Dim Params(0) As Object

        If _ReadOnly = True Then Return MyErr_Enum.MyErr_BadCmd

        If FormatTestFunc <> Nothing Then
            If FormatTestFunc(Me) = False Then
                State = VarState.VarState_WriteError
                Return MyErr_Enum.MyErr_BadCmd
            End If
        End If

        'If State <> VarState.VarState_Current Then

        If waitforParam(SerComLib.Port_InUse, False, 1000) = False Then Return MyErr_Enum.MyErr_PortInUse
        'SerComLib.Port_InUse = True

        If SetHandlerFunc = Nothing Then
            Params(0) = Value
        Else
            Params = SetHandlerFunc(Me)
        End If
        SerComLib.Ser_Timeout = 500

        Do While (retries And tmpErr <> MyErr_Enum.MyErr_Success)
            tmpErr = SerComLib.Ser_SendCmd_NS(SetCMD, Params, Nothing)
            retries = retries - 1
        Loop

        If tmpErr < 0 Then
            State = VarState.VarState_WriteError
        Else
            OrigVal = Value
            State = VarState.VarState_Current
        End If

        If PostSetHandlerFunc <> Nothing Then
            PostSetHandlerFunc(Me)
        End If
        'End If
        Return tmpErr
    End Function

    'the function will return once param = matchvalue (returns true) or the function times out (returns false)
    Public Shared Function waitforParam(ByRef Param As Boolean, ByVal Matchvalue As Boolean, ByVal msTimeout As Integer) As Boolean
        Dim starttime As DateTime = Now()

        Try
            Do
                'Threading.Thread.Sleep(1)
                Threading.Thread.Yield()
                Application.DoEvents()
            Loop While Param <> Matchvalue And (Now() - starttime).TotalMilliseconds < msTimeout
        Catch ex As Exception
            Throw ex
        End Try

        If Param <> Matchvalue Then '(Now() - starttime).TotalMilliseconds >= msTimeout Then
            Return False
        Else
            Return True
        End If

    End Function

End Class