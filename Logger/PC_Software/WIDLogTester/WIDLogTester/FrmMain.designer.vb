<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmMain
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
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmMain))
        Me.serLog = New System.IO.Ports.SerialPort(Me.components)
        Me.Label3 = New System.Windows.Forms.Label
        Me.cbLogPort = New System.Windows.Forms.ComboBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.txtRFChan = New System.Windows.Forms.MaskedTextBox
        Me.txtRFID = New System.Windows.Forms.MaskedTextBox
        Me.btnGetPorts = New System.Windows.Forms.Button
        Me.gbRFVars = New System.Windows.Forms.GroupBox
        Me.GroupBox3 = New System.Windows.Forms.GroupBox
        Me.lblLogStatus = New System.Windows.Forms.Label
        Me.RTBLog = New System.Windows.Forms.RichTextBox
        Me.lblLog = New System.Windows.Forms.Label
        Me.btnRUN = New System.Windows.Forms.Button
        Me.chkProgram = New System.Windows.Forms.CheckBox
        Me.chkSetVal = New System.Windows.Forms.CheckBox
        Me.chkTest = New System.Windows.Forms.CheckBox
        Me.btnShowSER = New System.Windows.Forms.Button
        Me.RTBSerLog = New System.Windows.Forms.RichTextBox
        Me.RTBProgLog = New System.Windows.Forms.RichTextBox
        Me.chkSaveLogs = New System.Windows.Forms.CheckBox
        Me.btnProg = New System.Windows.Forms.Button
        Me.txtTestTagID = New System.Windows.Forms.MaskedTextBox
        Me.Label8 = New System.Windows.Forms.Label
        Me.gbDevID = New System.Windows.Forms.GroupBox
        Me.Label6 = New System.Windows.Forms.Label
        Me.txtTestTxPer = New System.Windows.Forms.MaskedTextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.txtTestRFID = New System.Windows.Forms.MaskedTextBox
        Me.txtTestRFChan = New System.Windows.Forms.MaskedTextBox
        Me.lblWarning = New System.Windows.Forms.Label
        Me.chkPreserveFlash = New System.Windows.Forms.CheckBox
        Me.cbCodeVersion = New System.Windows.Forms.ComboBox
        Me.Label7 = New System.Windows.Forms.Label
        Me.gbRFVars.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.gbDevID.SuspendLayout()
        Me.SuspendLayout()
        '
        'serLog
        '
        Me.serLog.ReadTimeout = 1500
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(6, 17)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(65, 13)
        Me.Label3.TabIndex = 46
        Me.Label3.Text = "Logger Port:"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'cbLogPort
        '
        Me.cbLogPort.FormattingEnabled = True
        Me.cbLogPort.Location = New System.Drawing.Point(9, 33)
        Me.cbLogPort.Name = "cbLogPort"
        Me.cbLogPort.Size = New System.Drawing.Size(98, 21)
        Me.cbLogPort.TabIndex = 45
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(66, 13)
        Me.Label1.TabIndex = 47
        Me.Label1.Text = "RF Channel:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(110, 16)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(38, 13)
        Me.Label4.TabIndex = 49
        Me.Label4.Text = "RF ID:"
        '
        'txtRFChan
        '
        Me.txtRFChan.Location = New System.Drawing.Point(9, 32)
        Me.txtRFChan.Mask = "000"
        Me.txtRFChan.Name = "txtRFChan"
        Me.txtRFChan.Size = New System.Drawing.Size(97, 20)
        Me.txtRFChan.TabIndex = 55
        Me.txtRFChan.Text = "002"
        '
        'txtRFID
        '
        Me.txtRFID.Location = New System.Drawing.Point(111, 32)
        Me.txtRFID.Mask = "AA-AA-AA-AA-AA"
        Me.txtRFID.Name = "txtRFID"
        Me.txtRFID.Size = New System.Drawing.Size(100, 20)
        Me.txtRFID.TabIndex = 57
        Me.txtRFID.Text = "E6E6E6E6E6"
        Me.txtRFID.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals
        '
        'btnGetPorts
        '
        Me.btnGetPorts.Location = New System.Drawing.Point(111, 33)
        Me.btnGetPorts.Name = "btnGetPorts"
        Me.btnGetPorts.Size = New System.Drawing.Size(16, 39)
        Me.btnGetPorts.TabIndex = 58
        Me.btnGetPorts.UseVisualStyleBackColor = True
        '
        'gbRFVars
        '
        Me.gbRFVars.Controls.Add(Me.Label1)
        Me.gbRFVars.Controls.Add(Me.Label4)
        Me.gbRFVars.Controls.Add(Me.txtRFID)
        Me.gbRFVars.Controls.Add(Me.txtRFChan)
        Me.gbRFVars.Location = New System.Drawing.Point(12, 98)
        Me.gbRFVars.Name = "gbRFVars"
        Me.gbRFVars.Size = New System.Drawing.Size(220, 62)
        Me.gbRFVars.TabIndex = 61
        Me.gbRFVars.TabStop = False
        Me.gbRFVars.Text = "Device RF Variables:"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.lblLogStatus)
        Me.GroupBox3.Controls.Add(Me.btnGetPorts)
        Me.GroupBox3.Controls.Add(Me.cbLogPort)
        Me.GroupBox3.Controls.Add(Me.Label3)
        Me.GroupBox3.Location = New System.Drawing.Point(12, 12)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(136, 80)
        Me.GroupBox3.TabIndex = 63
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Connections:"
        '
        'lblLogStatus
        '
        Me.lblLogStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblLogStatus.Location = New System.Drawing.Point(9, 57)
        Me.lblLogStatus.Name = "lblLogStatus"
        Me.lblLogStatus.Size = New System.Drawing.Size(98, 15)
        Me.lblLogStatus.TabIndex = 66
        Me.lblLogStatus.Text = "Not Connected"
        '
        'RTBLog
        '
        Me.RTBLog.Location = New System.Drawing.Point(12, 289)
        Me.RTBLog.Name = "RTBLog"
        Me.RTBLog.Size = New System.Drawing.Size(318, 190)
        Me.RTBLog.TabIndex = 64
        Me.RTBLog.Text = ""
        '
        'lblLog
        '
        Me.lblLog.AutoSize = True
        Me.lblLog.Location = New System.Drawing.Point(12, 273)
        Me.lblLog.Name = "lblLog"
        Me.lblLog.Size = New System.Drawing.Size(52, 13)
        Me.lblLog.TabIndex = 65
        Me.lblLog.Text = "Test Log:"
        '
        'btnRUN
        '
        Me.btnRUN.BackColor = System.Drawing.Color.Green
        Me.btnRUN.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRUN.Location = New System.Drawing.Point(238, 235)
        Me.btnRUN.Name = "btnRUN"
        Me.btnRUN.Size = New System.Drawing.Size(96, 35)
        Me.btnRUN.TabIndex = 66
        Me.btnRUN.Text = "RUN"
        Me.btnRUN.UseVisualStyleBackColor = False
        '
        'chkProgram
        '
        Me.chkProgram.AutoSize = True
        Me.chkProgram.Checked = True
        Me.chkProgram.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkProgram.Location = New System.Drawing.Point(245, 165)
        Me.chkProgram.Name = "chkProgram"
        Me.chkProgram.Size = New System.Drawing.Size(65, 17)
        Me.chkProgram.TabIndex = 67
        Me.chkProgram.Text = "Program"
        Me.chkProgram.UseVisualStyleBackColor = True
        '
        'chkSetVal
        '
        Me.chkSetVal.AutoSize = True
        Me.chkSetVal.Checked = True
        Me.chkSetVal.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkSetVal.Location = New System.Drawing.Point(245, 215)
        Me.chkSetVal.Name = "chkSetVal"
        Me.chkSetVal.Size = New System.Drawing.Size(77, 17)
        Me.chkSetVal.TabIndex = 68
        Me.chkSetVal.Text = "Set Values"
        Me.chkSetVal.UseVisualStyleBackColor = True
        '
        'chkTest
        '
        Me.chkTest.AutoSize = True
        Me.chkTest.Checked = True
        Me.chkTest.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkTest.Location = New System.Drawing.Point(245, 190)
        Me.chkTest.Name = "chkTest"
        Me.chkTest.Size = New System.Drawing.Size(47, 17)
        Me.chkTest.TabIndex = 69
        Me.chkTest.Text = "Test"
        Me.chkTest.UseVisualStyleBackColor = True
        '
        'btnShowSER
        '
        Me.btnShowSER.Location = New System.Drawing.Point(154, 45)
        Me.btnShowSER.Name = "btnShowSER"
        Me.btnShowSER.Size = New System.Drawing.Size(78, 24)
        Me.btnShowSER.TabIndex = 70
        Me.btnShowSER.Text = "Cycle Logs"
        Me.btnShowSER.UseVisualStyleBackColor = True
        Me.btnShowSER.Visible = False
        '
        'RTBSerLog
        '
        Me.RTBSerLog.Location = New System.Drawing.Point(-234, 411)
        Me.RTBSerLog.Name = "RTBSerLog"
        Me.RTBSerLog.Size = New System.Drawing.Size(318, 190)
        Me.RTBSerLog.TabIndex = 72
        Me.RTBSerLog.Text = ""
        Me.RTBSerLog.Visible = False
        '
        'RTBProgLog
        '
        Me.RTBProgLog.Location = New System.Drawing.Point(12, 440)
        Me.RTBProgLog.Name = "RTBProgLog"
        Me.RTBProgLog.Size = New System.Drawing.Size(318, 190)
        Me.RTBProgLog.TabIndex = 73
        Me.RTBProgLog.Text = ""
        Me.RTBProgLog.Visible = False
        '
        'chkSaveLogs
        '
        Me.chkSaveLogs.AutoSize = True
        Me.chkSaveLogs.Checked = True
        Me.chkSaveLogs.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkSaveLogs.Location = New System.Drawing.Point(155, 75)
        Me.chkSaveLogs.Name = "chkSaveLogs"
        Me.chkSaveLogs.Size = New System.Drawing.Size(77, 17)
        Me.chkSaveLogs.TabIndex = 74
        Me.chkSaveLogs.Text = "Save Logs"
        Me.chkSaveLogs.UseVisualStyleBackColor = True
        Me.chkSaveLogs.Visible = False
        '
        'btnProg
        '
        Me.btnProg.Location = New System.Drawing.Point(154, 19)
        Me.btnProg.Name = "btnProg"
        Me.btnProg.Size = New System.Drawing.Size(78, 23)
        Me.btnProg.TabIndex = 75
        Me.btnProg.Text = "Programmer"
        Me.btnProg.UseVisualStyleBackColor = True
        Me.btnProg.Visible = False
        '
        'txtTestTagID
        '
        Me.txtTestTagID.Location = New System.Drawing.Point(10, 72)
        Me.txtTestTagID.Mask = "AAAAAAAA"
        Me.txtTestTagID.Name = "txtTestTagID"
        Me.txtTestTagID.Size = New System.Drawing.Size(98, 20)
        Me.txtTestTagID.TabIndex = 54
        Me.txtTestTagID.Text = "10000005"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(7, 56)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(44, 13)
        Me.Label8.TabIndex = 62
        Me.Label8.Text = "Dev ID:"
        '
        'gbDevID
        '
        Me.gbDevID.Controls.Add(Me.Label6)
        Me.gbDevID.Controls.Add(Me.txtTestTxPer)
        Me.gbDevID.Controls.Add(Me.Label2)
        Me.gbDevID.Controls.Add(Me.Label5)
        Me.gbDevID.Controls.Add(Me.txtTestRFID)
        Me.gbDevID.Controls.Add(Me.txtTestRFChan)
        Me.gbDevID.Controls.Add(Me.Label8)
        Me.gbDevID.Controls.Add(Me.txtTestTagID)
        Me.gbDevID.Location = New System.Drawing.Point(12, 166)
        Me.gbDevID.Name = "gbDevID"
        Me.gbDevID.Size = New System.Drawing.Size(220, 104)
        Me.gbDevID.TabIndex = 60
        Me.gbDevID.TabStop = False
        Me.gbDevID.Text = "RF Test Variables:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(111, 56)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(55, 13)
        Me.Label6.TabIndex = 67
        Me.Label6.Text = "Tx Period:"
        '
        'txtTestTxPer
        '
        Me.txtTestTxPer.Location = New System.Drawing.Point(114, 72)
        Me.txtTestTxPer.Mask = "0:00"
        Me.txtTestTxPer.Name = "txtTestTxPer"
        Me.txtTestTxPer.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtTestTxPer.Size = New System.Drawing.Size(97, 20)
        Me.txtTestTxPer.TabIndex = 68
        Me.txtTestTxPer.Text = "010"
        Me.txtTestTxPer.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(7, 17)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(66, 13)
        Me.Label2.TabIndex = 63
        Me.Label2.Text = "RF Channel:"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(111, 17)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(38, 13)
        Me.Label5.TabIndex = 64
        Me.Label5.Text = "RF ID:"
        '
        'txtTestRFID
        '
        Me.txtTestRFID.Location = New System.Drawing.Point(112, 33)
        Me.txtTestRFID.Mask = "AA-AA-AA-AA-AA"
        Me.txtTestRFID.Name = "txtTestRFID"
        Me.txtTestRFID.Size = New System.Drawing.Size(100, 20)
        Me.txtTestRFID.TabIndex = 66
        Me.txtTestRFID.Text = "E6E6E6E6E6"
        Me.txtTestRFID.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals
        '
        'txtTestRFChan
        '
        Me.txtTestRFChan.Location = New System.Drawing.Point(10, 33)
        Me.txtTestRFChan.Mask = "000"
        Me.txtTestRFChan.Name = "txtTestRFChan"
        Me.txtTestRFChan.Size = New System.Drawing.Size(97, 20)
        Me.txtTestRFChan.TabIndex = 65
        Me.txtTestRFChan.Text = "002"
        '
        'lblWarning
        '
        Me.lblWarning.AutoSize = True
        Me.lblWarning.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblWarning.ForeColor = System.Drawing.Color.Red
        Me.lblWarning.Location = New System.Drawing.Point(242, 75)
        Me.lblWarning.Name = "lblWarning"
        Me.lblWarning.Size = New System.Drawing.Size(101, 80)
        Me.lblWarning.TabIndex = 76
        Me.lblWarning.Text = "Running Test" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "or Set Values" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "mode will " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "ERASE data" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "flash!!!!!"
        '
        'chkPreserveFlash
        '
        Me.chkPreserveFlash.AutoSize = True
        Me.chkPreserveFlash.Location = New System.Drawing.Point(245, 50)
        Me.chkPreserveFlash.Name = "chkPreserveFlash"
        Me.chkPreserveFlash.Size = New System.Drawing.Size(96, 17)
        Me.chkPreserveFlash.TabIndex = 77
        Me.chkPreserveFlash.Text = "Preserve Flash"
        Me.chkPreserveFlash.UseVisualStyleBackColor = True
        '
        'cbCodeVersion
        '
        Me.cbCodeVersion.Enabled = False
        Me.cbCodeVersion.FormattingEnabled = True
        Me.cbCodeVersion.Location = New System.Drawing.Point(245, 22)
        Me.cbCodeVersion.Name = "cbCodeVersion"
        Me.cbCodeVersion.Size = New System.Drawing.Size(89, 21)
        Me.cbCodeVersion.TabIndex = 78
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(244, 6)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(73, 13)
        Me.Label7.TabIndex = 79
        Me.Label7.Text = "Code Version:"
        '
        'FrmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(342, 491)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.cbCodeVersion)
        Me.Controls.Add(Me.chkPreserveFlash)
        Me.Controls.Add(Me.lblWarning)
        Me.Controls.Add(Me.btnProg)
        Me.Controls.Add(Me.chkSaveLogs)
        Me.Controls.Add(Me.RTBProgLog)
        Me.Controls.Add(Me.RTBSerLog)
        Me.Controls.Add(Me.btnShowSER)
        Me.Controls.Add(Me.chkTest)
        Me.Controls.Add(Me.chkSetVal)
        Me.Controls.Add(Me.chkProgram)
        Me.Controls.Add(Me.btnRUN)
        Me.Controls.Add(Me.lblLog)
        Me.Controls.Add(Me.RTBLog)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.gbRFVars)
        Me.Controls.Add(Me.gbDevID)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "FrmMain"
        Me.Text = "WID Logger Programmer And Tester"
        Me.gbRFVars.ResumeLayout(False)
        Me.gbRFVars.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.gbDevID.ResumeLayout(False)
        Me.gbDevID.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents serLog As System.IO.Ports.SerialPort
    Private WithEvents Label3 As System.Windows.Forms.Label
    Private WithEvents cbLogPort As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents txtRFChan As System.Windows.Forms.MaskedTextBox
    Friend WithEvents txtRFID As System.Windows.Forms.MaskedTextBox
    Friend WithEvents btnGetPorts As System.Windows.Forms.Button
    Friend WithEvents gbRFVars As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Private WithEvents lblLogStatus As System.Windows.Forms.Label
    Friend WithEvents RTBLog As System.Windows.Forms.RichTextBox
    Friend WithEvents lblLog As System.Windows.Forms.Label
    Friend WithEvents btnRUN As System.Windows.Forms.Button
    Friend WithEvents chkProgram As System.Windows.Forms.CheckBox
    Friend WithEvents chkSetVal As System.Windows.Forms.CheckBox
    Friend WithEvents chkTest As System.Windows.Forms.CheckBox
    Friend WithEvents btnShowSER As System.Windows.Forms.Button
    Friend WithEvents RTBSerLog As System.Windows.Forms.RichTextBox
    Friend WithEvents RTBProgLog As System.Windows.Forms.RichTextBox
    Friend WithEvents chkSaveLogs As System.Windows.Forms.CheckBox
    Friend WithEvents btnProg As System.Windows.Forms.Button
    Friend WithEvents txtTestTagID As System.Windows.Forms.MaskedTextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents gbDevID As System.Windows.Forms.GroupBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents txtTestRFID As System.Windows.Forms.MaskedTextBox
    Friend WithEvents txtTestRFChan As System.Windows.Forms.MaskedTextBox
    Friend WithEvents lblWarning As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtTestTxPer As System.Windows.Forms.MaskedTextBox
    Friend WithEvents chkPreserveFlash As System.Windows.Forms.CheckBox
    Friend WithEvents cbCodeVersion As System.Windows.Forms.ComboBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
End Class
