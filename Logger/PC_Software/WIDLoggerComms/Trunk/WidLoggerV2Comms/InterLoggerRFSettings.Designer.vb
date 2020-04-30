<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class InterLoggerRFSettings
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.cmbProfileName = New System.Windows.Forms.ComboBox()
        Me.btnLoadProfile = New System.Windows.Forms.Button()
        Me.btnSaveProfile = New System.Windows.Forms.Button()
        Me.btnSaveProfileAs = New System.Windows.Forms.Button()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.AdvBaseFreq = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cmbAdvXtalFreq = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cmbAdvModFmt = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.nudAdvChanNum = New System.Windows.Forms.NumericUpDown()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.txtAdvDataRate = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.txtAdvDeviation = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.txtAdvChanSpacing = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.txtAdvRxFilterBW = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.cmbAdvTxPower = New System.Windows.Forms.ComboBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.txtAdvCarrierFreq = New System.Windows.Forms.TextBox()
        Me.chkAdvManchesterEn = New System.Windows.Forms.CheckBox()
        Me.chkAdvPARamping = New System.Windows.Forms.CheckBox()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.cmbAdvSyncWordLen = New System.Windows.Forms.ComboBox()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.cmbAdvPreambleCnt = New System.Windows.Forms.ComboBox()
        Me.txtOutput = New System.Windows.Forms.TextBox()
        Me.btnCopyToDevice = New System.Windows.Forms.Button()
        Me.TabControl1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        CType(Me.nudAdvChanNum, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'cmbProfileName
        '
        Me.cmbProfileName.FormattingEnabled = True
        Me.cmbProfileName.Location = New System.Drawing.Point(12, 13)
        Me.cmbProfileName.Name = "cmbProfileName"
        Me.cmbProfileName.Size = New System.Drawing.Size(306, 21)
        Me.cmbProfileName.TabIndex = 0
        '
        'btnLoadProfile
        '
        Me.btnLoadProfile.Location = New System.Drawing.Point(324, 13)
        Me.btnLoadProfile.Name = "btnLoadProfile"
        Me.btnLoadProfile.Size = New System.Drawing.Size(48, 21)
        Me.btnLoadProfile.TabIndex = 1
        Me.btnLoadProfile.Text = "Load"
        Me.btnLoadProfile.UseVisualStyleBackColor = True
        '
        'btnSaveProfile
        '
        Me.btnSaveProfile.Location = New System.Drawing.Point(378, 13)
        Me.btnSaveProfile.Name = "btnSaveProfile"
        Me.btnSaveProfile.Size = New System.Drawing.Size(48, 21)
        Me.btnSaveProfile.TabIndex = 2
        Me.btnSaveProfile.Text = "Save"
        Me.btnSaveProfile.UseVisualStyleBackColor = True
        '
        'btnSaveProfileAs
        '
        Me.btnSaveProfileAs.Location = New System.Drawing.Point(432, 13)
        Me.btnSaveProfileAs.Name = "btnSaveProfileAs"
        Me.btnSaveProfileAs.Size = New System.Drawing.Size(75, 21)
        Me.btnSaveProfileAs.TabIndex = 3
        Me.btnSaveProfileAs.Text = "Save New Profile"
        Me.btnSaveProfileAs.UseVisualStyleBackColor = True
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Location = New System.Drawing.Point(12, 40)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(618, 418)
        Me.TabControl1.TabIndex = 4
        '
        'TabPage1
        '
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(610, 450)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Basic"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.Label20)
        Me.TabPage2.Controls.Add(Me.cmbAdvPreambleCnt)
        Me.TabPage2.Controls.Add(Me.Label19)
        Me.TabPage2.Controls.Add(Me.cmbAdvSyncWordLen)
        Me.TabPage2.Controls.Add(Me.chkAdvPARamping)
        Me.TabPage2.Controls.Add(Me.chkAdvManchesterEn)
        Me.TabPage2.Controls.Add(Me.Label17)
        Me.TabPage2.Controls.Add(Me.Label18)
        Me.TabPage2.Controls.Add(Me.txtAdvCarrierFreq)
        Me.TabPage2.Controls.Add(Me.Label15)
        Me.TabPage2.Controls.Add(Me.Label16)
        Me.TabPage2.Controls.Add(Me.cmbAdvTxPower)
        Me.TabPage2.Controls.Add(Me.Label13)
        Me.TabPage2.Controls.Add(Me.Label14)
        Me.TabPage2.Controls.Add(Me.txtAdvRxFilterBW)
        Me.TabPage2.Controls.Add(Me.Label11)
        Me.TabPage2.Controls.Add(Me.Label12)
        Me.TabPage2.Controls.Add(Me.txtAdvChanSpacing)
        Me.TabPage2.Controls.Add(Me.Label9)
        Me.TabPage2.Controls.Add(Me.Label10)
        Me.TabPage2.Controls.Add(Me.txtAdvDeviation)
        Me.TabPage2.Controls.Add(Me.Label7)
        Me.TabPage2.Controls.Add(Me.Label8)
        Me.TabPage2.Controls.Add(Me.txtAdvDataRate)
        Me.TabPage2.Controls.Add(Me.Label6)
        Me.TabPage2.Controls.Add(Me.nudAdvChanNum)
        Me.TabPage2.Controls.Add(Me.Label5)
        Me.TabPage2.Controls.Add(Me.Label4)
        Me.TabPage2.Controls.Add(Me.Label3)
        Me.TabPage2.Controls.Add(Me.cmbAdvModFmt)
        Me.TabPage2.Controls.Add(Me.Label2)
        Me.TabPage2.Controls.Add(Me.cmbAdvXtalFreq)
        Me.TabPage2.Controls.Add(Me.Label1)
        Me.TabPage2.Controls.Add(Me.AdvBaseFreq)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(610, 392)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Advanced"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'AdvBaseFreq
        '
        Me.AdvBaseFreq.Location = New System.Drawing.Point(6, 34)
        Me.AdvBaseFreq.Name = "AdvBaseFreq"
        Me.AdvBaseFreq.Size = New System.Drawing.Size(100, 20)
        Me.AdvBaseFreq.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 18)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(81, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Base frequency"
        '
        'cmbAdvXtalFreq
        '
        Me.cmbAdvXtalFreq.FormattingEnabled = True
        Me.cmbAdvXtalFreq.Items.AddRange(New Object() {"26.000", "27.000"})
        Me.cmbAdvXtalFreq.Location = New System.Drawing.Point(6, 88)
        Me.cmbAdvXtalFreq.Name = "cmbAdvXtalFreq"
        Me.cmbAdvXtalFreq.Size = New System.Drawing.Size(121, 21)
        Me.cmbAdvXtalFreq.TabIndex = 2
        Me.cmbAdvXtalFreq.Text = "26.000"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(6, 72)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(75, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Xtal frequency"
        '
        'cmbAdvModFmt
        '
        Me.cmbAdvModFmt.FormattingEnabled = True
        Me.cmbAdvModFmt.Items.AddRange(New Object() {"2-FSK", "GFSK", "ASK/OOK", "4-FSK"})
        Me.cmbAdvModFmt.Location = New System.Drawing.Point(6, 144)
        Me.cmbAdvModFmt.Name = "cmbAdvModFmt"
        Me.cmbAdvModFmt.Size = New System.Drawing.Size(121, 21)
        Me.cmbAdvModFmt.TabIndex = 4
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(6, 128)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(91, 13)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "Modulation format"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(133, 91)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(29, 13)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "MHz"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(112, 37)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(29, 13)
        Me.Label5.TabIndex = 7
        Me.Label5.Text = "MHz"
        '
        'nudAdvChanNum
        '
        Me.nudAdvChanNum.Location = New System.Drawing.Point(197, 34)
        Me.nudAdvChanNum.Name = "nudAdvChanNum"
        Me.nudAdvChanNum.Size = New System.Drawing.Size(56, 20)
        Me.nudAdvChanNum.TabIndex = 8
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(194, 18)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(86, 13)
        Me.Label6.TabIndex = 9
        Me.Label6.Text = "Channel Number"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(303, 91)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(38, 13)
        Me.Label7.TabIndex = 12
        Me.Label7.Text = "kBaud"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(197, 72)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(51, 13)
        Me.Label8.TabIndex = 11
        Me.Label8.Text = "Data rate"
        '
        'txtAdvDataRate
        '
        Me.txtAdvDataRate.Location = New System.Drawing.Point(197, 88)
        Me.txtAdvDataRate.Name = "txtAdvDataRate"
        Me.txtAdvDataRate.Size = New System.Drawing.Size(100, 20)
        Me.txtAdvDataRate.TabIndex = 10
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(303, 147)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(26, 13)
        Me.Label9.TabIndex = 15
        Me.Label9.Text = "kHz"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(197, 128)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(52, 13)
        Me.Label10.TabIndex = 14
        Me.Label10.Text = "Deviation"
        '
        'txtAdvDeviation
        '
        Me.txtAdvDeviation.Enabled = False
        Me.txtAdvDeviation.Location = New System.Drawing.Point(197, 144)
        Me.txtAdvDeviation.Name = "txtAdvDeviation"
        Me.txtAdvDeviation.Size = New System.Drawing.Size(100, 20)
        Me.txtAdvDeviation.TabIndex = 13
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(479, 37)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(26, 13)
        Me.Label11.TabIndex = 18
        Me.Label11.Text = "kHz"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(373, 18)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(86, 13)
        Me.Label12.TabIndex = 17
        Me.Label12.Text = "Channel spacing"
        '
        'txtAdvChanSpacing
        '
        Me.txtAdvChanSpacing.Location = New System.Drawing.Point(373, 34)
        Me.txtAdvChanSpacing.Name = "txtAdvChanSpacing"
        Me.txtAdvChanSpacing.Size = New System.Drawing.Size(100, 20)
        Me.txtAdvChanSpacing.TabIndex = 16
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(479, 87)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(26, 13)
        Me.Label13.TabIndex = 21
        Me.Label13.Text = "kHz"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(373, 68)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(65, 13)
        Me.Label14.TabIndex = 20
        Me.Label14.Text = "RX filter BW"
        '
        'txtAdvRxFilterBW
        '
        Me.txtAdvRxFilterBW.Location = New System.Drawing.Point(373, 84)
        Me.txtAdvRxFilterBW.Name = "txtAdvRxFilterBW"
        Me.txtAdvRxFilterBW.Size = New System.Drawing.Size(100, 20)
        Me.txtAdvRxFilterBW.TabIndex = 19
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(454, 147)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(28, 13)
        Me.Label15.TabIndex = 24
        Me.Label15.Text = "dBm"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(373, 127)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(53, 13)
        Me.Label16.TabIndex = 23
        Me.Label16.Text = "TX power"
        '
        'cmbAdvTxPower
        '
        Me.cmbAdvTxPower.FormattingEnabled = True
        Me.cmbAdvTxPower.Items.AddRange(New Object() {"12", "10", "7", "5", "0", "-6", "-10", "-15", "-20", "-30"})
        Me.cmbAdvTxPower.Location = New System.Drawing.Point(373, 143)
        Me.cmbAdvTxPower.Name = "cmbAdvTxPower"
        Me.cmbAdvTxPower.Size = New System.Drawing.Size(75, 21)
        Me.cmbAdvTxPower.TabIndex = 22
        Me.cmbAdvTxPower.Text = "12"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(112, 227)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(29, 13)
        Me.Label17.TabIndex = 27
        Me.Label17.Text = "MHz"
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(6, 208)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(87, 13)
        Me.Label18.TabIndex = 26
        Me.Label18.Text = "Carrier frequency"
        '
        'txtAdvCarrierFreq
        '
        Me.txtAdvCarrierFreq.Enabled = False
        Me.txtAdvCarrierFreq.Location = New System.Drawing.Point(6, 224)
        Me.txtAdvCarrierFreq.Name = "txtAdvCarrierFreq"
        Me.txtAdvCarrierFreq.Size = New System.Drawing.Size(100, 20)
        Me.txtAdvCarrierFreq.TabIndex = 25
        '
        'chkAdvManchesterEn
        '
        Me.chkAdvManchesterEn.AutoSize = True
        Me.chkAdvManchesterEn.Location = New System.Drawing.Point(197, 226)
        Me.chkAdvManchesterEn.Name = "chkAdvManchesterEn"
        Me.chkAdvManchesterEn.Size = New System.Drawing.Size(117, 17)
        Me.chkAdvManchesterEn.TabIndex = 28
        Me.chkAdvManchesterEn.Text = "Manchester enable"
        Me.chkAdvManchesterEn.UseVisualStyleBackColor = True
        '
        'chkAdvPARamping
        '
        Me.chkAdvPARamping.AutoSize = True
        Me.chkAdvPARamping.Location = New System.Drawing.Point(373, 227)
        Me.chkAdvPARamping.Name = "chkAdvPARamping"
        Me.chkAdvPARamping.Size = New System.Drawing.Size(80, 17)
        Me.chkAdvPARamping.TabIndex = 29
        Me.chkAdvPARamping.Text = "PA ramping"
        Me.chkAdvPARamping.UseVisualStyleBackColor = True
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(6, 300)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(89, 13)
        Me.Label19.TabIndex = 31
        Me.Label19.Text = "Sync word length"
        '
        'cmbAdvSyncWordLen
        '
        Me.cmbAdvSyncWordLen.FormattingEnabled = True
        Me.cmbAdvSyncWordLen.Items.AddRange(New Object() {"No preamble/sync", "15/16 sync word bits detected", "16/16 sync word bits detected", "30/32 sync word bits detected", "No preamble/sync, carrier-sense above threshold", "15/16 + carrier-sense above threshold", "16/16 + carrier-sense above threshold", "30/32 + carrier-sense above threshold"})
        Me.cmbAdvSyncWordLen.Location = New System.Drawing.Point(6, 316)
        Me.cmbAdvSyncWordLen.Name = "cmbAdvSyncWordLen"
        Me.cmbAdvSyncWordLen.Size = New System.Drawing.Size(296, 21)
        Me.cmbAdvSyncWordLen.TabIndex = 30
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Location = New System.Drawing.Point(6, 342)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(81, 13)
        Me.Label20.TabIndex = 33
        Me.Label20.Text = "Preamble count"
        '
        'cmbAdvPreambleCnt
        '
        Me.cmbAdvPreambleCnt.FormattingEnabled = True
        Me.cmbAdvPreambleCnt.Location = New System.Drawing.Point(6, 358)
        Me.cmbAdvPreambleCnt.Name = "cmbAdvPreambleCnt"
        Me.cmbAdvPreambleCnt.Size = New System.Drawing.Size(81, 21)
        Me.cmbAdvPreambleCnt.TabIndex = 32
        '
        'txtOutput
        '
        Me.txtOutput.Location = New System.Drawing.Point(12, 464)
        Me.txtOutput.Multiline = True
        Me.txtOutput.Name = "txtOutput"
        Me.txtOutput.Size = New System.Drawing.Size(509, 71)
        Me.txtOutput.TabIndex = 5
        '
        'btnCopyToDevice
        '
        Me.btnCopyToDevice.Location = New System.Drawing.Point(527, 463)
        Me.btnCopyToDevice.Name = "btnCopyToDevice"
        Me.btnCopyToDevice.Size = New System.Drawing.Size(103, 72)
        Me.btnCopyToDevice.TabIndex = 6
        Me.btnCopyToDevice.Text = "Write To Device"
        Me.btnCopyToDevice.UseVisualStyleBackColor = True
        '
        'InterLoggerRFSettings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(642, 547)
        Me.Controls.Add(Me.btnCopyToDevice)
        Me.Controls.Add(Me.txtOutput)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.btnSaveProfileAs)
        Me.Controls.Add(Me.btnSaveProfile)
        Me.Controls.Add(Me.btnLoadProfile)
        Me.Controls.Add(Me.cmbProfileName)
        Me.Name = "InterLoggerRFSettings"
        Me.Text = "Inter-Logger RF Settings"
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage2.PerformLayout()
        CType(Me.nudAdvChanNum, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnLoadProfile As System.Windows.Forms.Button
    Friend WithEvents btnSaveProfile As System.Windows.Forms.Button
    Friend WithEvents btnSaveProfileAs As System.Windows.Forms.Button
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents cmbAdvPreambleCnt As System.Windows.Forms.ComboBox
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents cmbAdvSyncWordLen As System.Windows.Forms.ComboBox
    Friend WithEvents chkAdvPARamping As System.Windows.Forms.CheckBox
    Friend WithEvents chkAdvManchesterEn As System.Windows.Forms.CheckBox
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents txtAdvCarrierFreq As System.Windows.Forms.TextBox
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents cmbAdvTxPower As System.Windows.Forms.ComboBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents txtAdvRxFilterBW As System.Windows.Forms.TextBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents txtAdvChanSpacing As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents txtAdvDeviation As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents txtAdvDataRate As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents nudAdvChanNum As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents cmbAdvModFmt As System.Windows.Forms.ComboBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents cmbAdvXtalFreq As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents AdvBaseFreq As System.Windows.Forms.TextBox
    Friend WithEvents cmbProfileName As System.Windows.Forms.ComboBox
    Friend WithEvents txtOutput As System.Windows.Forms.TextBox
    Friend WithEvents btnCopyToDevice As System.Windows.Forms.Button
End Class
