<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmBatchProgrammer
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmBatchProgrammer))
        Me.tcMain = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.nudLoggerNameStart = New System.Windows.Forms.NumericUpDown()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.txtBaseLoggerName = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.txtExpectedVersionString = New System.Windows.Forms.TextBox()
        Me.cbIncLgrName = New System.Windows.Forms.ComboBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.cbIncLgrGrp = New System.Windows.Forms.ComboBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.txtTagWorstRSSI = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.txtTagId = New System.Windows.Forms.TextBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.txtLogDir = New System.Windows.Forms.TextBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.txtProgramFile = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cbProgDevice = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cbProgTool = New System.Windows.Forms.ComboBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.chkEraseFlash = New System.Windows.Forms.CheckBox()
        Me.chkTestIridiumNetworkConnect = New System.Windows.Forms.CheckBox()
        Me.chkTestIridiumTx = New System.Windows.Forms.CheckBox()
        Me.chkRead9602IMEI = New System.Windows.Forms.CheckBox()
        Me.chkConfig9602 = New System.Windows.Forms.CheckBox()
        Me.chkTest433MHzInterLogger = New System.Windows.Forms.CheckBox()
        Me.chk24GHzPickup = New System.Windows.Forms.CheckBox()
        Me.chkSetSettings = New System.Windows.Forms.CheckBox()
        Me.chkProgramLogger = New System.Windows.Forms.CheckBox()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.TabControlMain = New System.Windows.Forms.TabControl()
        Me.TabPage6 = New System.Windows.Forms.TabPage()
        Me.SplitContainer4 = New System.Windows.Forms.SplitContainer()
        Me.TLPOverview = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel5 = New System.Windows.Forms.TableLayoutPanel()
        Me.pbRecordCount = New System.Windows.Forms.PictureBox()
        Me.btnEraseDataflash = New System.Windows.Forms.Button()
        Me.txtRecordCount = New System.Windows.Forms.TextBox()
        Me.TableLayoutPanel4 = New System.Windows.Forms.TableLayoutPanel()
        Me.PBDevDateTime = New System.Windows.Forms.PictureBox()
        Me.btnDevTimeSync = New System.Windows.Forms.Button()
        Me.dtpDevDateTime = New System.Windows.Forms.DateTimePicker()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.btnWriteVals = New System.Windows.Forms.Button()
        Me.btnReadVals = New System.Windows.Forms.Button()
        Me.TabPage7 = New System.Windows.Forms.TabPage()
        Me.SplitContainer5 = New System.Windows.Forms.SplitContainer()
        Me.TLPRFSet = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel9 = New System.Windows.Forms.TableLayoutPanel()
        Me.pbRFParams = New System.Windows.Forms.PictureBox()
        Me.txtRFParams = New System.Windows.Forms.TextBox()
        Me.cbRFParams = New System.Windows.Forms.ComboBox()
        Me.TableLayoutPanel8 = New System.Windows.Forms.TableLayoutPanel()
        Me.pbInterloggerRFParams = New System.Windows.Forms.PictureBox()
        Me.txtInterloggerRFParams = New System.Windows.Forms.TextBox()
        Me.cbInterloggerRFParams = New System.Windows.Forms.ComboBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel()
        Me.pbVerboseMode = New System.Windows.Forms.PictureBox()
        Me.cbVerboseMode = New System.Windows.Forms.ComboBox()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.btnWriteRFVals = New System.Windows.Forms.Button()
        Me.btnReadRFVals = New System.Windows.Forms.Button()
        Me.TabPage8 = New System.Windows.Forms.TabPage()
        Me.SplitContainer6 = New System.Windows.Forms.SplitContainer()
        Me.TLPGSMSet = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel10 = New System.Windows.Forms.TableLayoutPanel()
        Me.pbNextDataReset = New System.Windows.Forms.PictureBox()
        Me.dtpNextDataReset = New System.Windows.Forms.DateTimePicker()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.TableLayoutPanel7 = New System.Windows.Forms.TableLayoutPanel()
        Me.pbGPRSSettings = New System.Windows.Forms.PictureBox()
        Me.cbGPRSSettings = New System.Windows.Forms.ComboBox()
        Me.TableLayoutPanel6 = New System.Windows.Forms.TableLayoutPanel()
        Me.pbSendEmailNext = New System.Windows.Forms.PictureBox()
        Me.dtpSendEmailNext = New System.Windows.Forms.DateTimePicker()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.btnWriteGSMVals = New System.Windows.Forms.Button()
        Me.btnReadGSMVals = New System.Windows.Forms.Button()
        Me.TabPage5 = New System.Windows.Forms.TabPage()
        Me.rtbSerOut = New System.Windows.Forms.RichTextBox()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.rtbProgConsole = New System.Windows.Forms.RichTextBox()
        Me.btnStartProgramming = New System.Windows.Forms.Button()
        Me.ofdProgFile = New System.Windows.Forms.OpenFileDialog()
        Me.fbdLogDir = New System.Windows.Forms.FolderBrowserDialog()
        Me.lblInfo = New System.Windows.Forms.Label()
        Me.chkMeasureCurrent = New System.Windows.Forms.CheckBox()
        Me.tcMain.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        CType(Me.nudLoggerNameStart, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox4.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.TabControlMain.SuspendLayout()
        Me.TabPage6.SuspendLayout()
        CType(Me.SplitContainer4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer4.Panel1.SuspendLayout()
        Me.SplitContainer4.Panel2.SuspendLayout()
        Me.SplitContainer4.SuspendLayout()
        Me.TLPOverview.SuspendLayout()
        Me.TableLayoutPanel5.SuspendLayout()
        CType(Me.pbRecordCount, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanel4.SuspendLayout()
        CType(Me.PBDevDateTime, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage7.SuspendLayout()
        CType(Me.SplitContainer5, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer5.Panel1.SuspendLayout()
        Me.SplitContainer5.Panel2.SuspendLayout()
        Me.SplitContainer5.SuspendLayout()
        Me.TLPRFSet.SuspendLayout()
        Me.TableLayoutPanel9.SuspendLayout()
        CType(Me.pbRFParams, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanel8.SuspendLayout()
        CType(Me.pbInterloggerRFParams, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanel3.SuspendLayout()
        CType(Me.pbVerboseMode, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage8.SuspendLayout()
        CType(Me.SplitContainer6, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer6.Panel1.SuspendLayout()
        Me.SplitContainer6.Panel2.SuspendLayout()
        Me.SplitContainer6.SuspendLayout()
        Me.TLPGSMSet.SuspendLayout()
        Me.TableLayoutPanel10.SuspendLayout()
        CType(Me.pbNextDataReset, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanel7.SuspendLayout()
        CType(Me.pbGPRSSettings, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanel6.SuspendLayout()
        CType(Me.pbSendEmailNext, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage5.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'tcMain
        '
        Me.tcMain.Controls.Add(Me.TabPage1)
        Me.tcMain.Controls.Add(Me.TabPage2)
        Me.tcMain.Controls.Add(Me.TabPage3)
        Me.tcMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tcMain.Location = New System.Drawing.Point(0, 0)
        Me.tcMain.Name = "tcMain"
        Me.tcMain.SelectedIndex = 0
        Me.tcMain.Size = New System.Drawing.Size(520, 516)
        Me.tcMain.TabIndex = 0
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.GroupBox5)
        Me.TabPage1.Controls.Add(Me.GroupBox4)
        Me.TabPage1.Controls.Add(Me.GroupBox3)
        Me.TabPage1.Controls.Add(Me.GroupBox2)
        Me.TabPage1.Controls.Add(Me.GroupBox1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(512, 490)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Setup 1"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.Label17)
        Me.GroupBox5.Controls.Add(Me.nudLoggerNameStart)
        Me.GroupBox5.Controls.Add(Me.Label16)
        Me.GroupBox5.Controls.Add(Me.txtBaseLoggerName)
        Me.GroupBox5.Controls.Add(Me.Label15)
        Me.GroupBox5.Controls.Add(Me.txtExpectedVersionString)
        Me.GroupBox5.Controls.Add(Me.cbIncLgrName)
        Me.GroupBox5.Controls.Add(Me.Label14)
        Me.GroupBox5.Controls.Add(Me.cbIncLgrGrp)
        Me.GroupBox5.Controls.Add(Me.Label13)
        Me.GroupBox5.Location = New System.Drawing.Point(224, 273)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(280, 180)
        Me.GroupBox5.TabIndex = 11
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Settings"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(215, 95)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(54, 13)
        Me.Label17.TabIndex = 16
        Me.Label17.Text = "Start Num"
        '
        'nudLoggerNameStart
        '
        Me.nudLoggerNameStart.Location = New System.Drawing.Point(218, 111)
        Me.nudLoggerNameStart.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
        Me.nudLoggerNameStart.Name = "nudLoggerNameStart"
        Me.nudLoggerNameStart.Size = New System.Drawing.Size(56, 20)
        Me.nudLoggerNameStart.TabIndex = 15
        Me.nudLoggerNameStart.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(6, 95)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(98, 13)
        Me.Label16.TabIndex = 14
        Me.Label16.Text = "Base Logger Name"
        '
        'txtBaseLoggerName
        '
        Me.txtBaseLoggerName.Location = New System.Drawing.Point(9, 111)
        Me.txtBaseLoggerName.Name = "txtBaseLoggerName"
        Me.txtBaseLoggerName.Size = New System.Drawing.Size(203, 20)
        Me.txtBaseLoggerName.TabIndex = 13
        Me.txtBaseLoggerName.Text = "Monitor Proj "
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(6, 56)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(120, 13)
        Me.Label15.TabIndex = 12
        Me.Label15.Text = "Expected Version String"
        '
        'txtExpectedVersionString
        '
        Me.txtExpectedVersionString.Location = New System.Drawing.Point(9, 72)
        Me.txtExpectedVersionString.Name = "txtExpectedVersionString"
        Me.txtExpectedVersionString.Size = New System.Drawing.Size(203, 20)
        Me.txtExpectedVersionString.TabIndex = 11
        Me.txtExpectedVersionString.Text = "WID Logger 3 - V2.3.1:MP"
        '
        'cbIncLgrName
        '
        Me.cbIncLgrName.FormattingEnabled = True
        Me.cbIncLgrName.Items.AddRange(New Object() {"Yes", "Ask For New Name"})
        Me.cbIncLgrName.Location = New System.Drawing.Point(135, 32)
        Me.cbIncLgrName.Name = "cbIncLgrName"
        Me.cbIncLgrName.Size = New System.Drawing.Size(81, 21)
        Me.cbIncLgrName.TabIndex = 10
        Me.cbIncLgrName.Text = "Yes"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(132, 16)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(121, 13)
        Me.Label14.TabIndex = 9
        Me.Label14.Text = "Increment Logger Name"
        '
        'cbIncLgrGrp
        '
        Me.cbIncLgrGrp.FormattingEnabled = True
        Me.cbIncLgrGrp.Items.AddRange(New Object() {"Yes", "No", "Ask", "Ask For Value"})
        Me.cbIncLgrGrp.Location = New System.Drawing.Point(7, 32)
        Me.cbIncLgrGrp.Name = "cbIncLgrGrp"
        Me.cbIncLgrGrp.Size = New System.Drawing.Size(81, 21)
        Me.cbIncLgrGrp.TabIndex = 8
        Me.cbIncLgrGrp.Text = "No"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(6, 16)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(122, 13)
        Me.Label13.TabIndex = 7
        Me.Label13.Text = "Increment Logger Group"
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.Label12)
        Me.GroupBox4.Controls.Add(Me.Label11)
        Me.GroupBox4.Controls.Add(Me.txtTagWorstRSSI)
        Me.GroupBox4.Controls.Add(Me.Label10)
        Me.GroupBox4.Controls.Add(Me.txtTagId)
        Me.GroupBox4.Location = New System.Drawing.Point(224, 198)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(280, 69)
        Me.GroupBox4.TabIndex = 10
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Testing"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(225, 40)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(22, 13)
        Me.Label12.TabIndex = 10
        Me.Label12.Text = "dBi"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(130, 21)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(129, 13)
        Me.Label11.TabIndex = 8
        Me.Label11.Text = "2.4GHz Test Worst RSSI:"
        '
        'txtTagWorstRSSI
        '
        Me.txtTagWorstRSSI.Location = New System.Drawing.Point(133, 37)
        Me.txtTagWorstRSSI.Name = "txtTagWorstRSSI"
        Me.txtTagWorstRSSI.Size = New System.Drawing.Size(86, 20)
        Me.txtTagWorstRSSI.TabIndex = 9
        Me.txtTagWorstRSSI.Text = "-37"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(6, 21)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(106, 13)
        Me.Label10.TabIndex = 6
        Me.Label10.Text = "2.4GHz Test Tag ID:"
        '
        'txtTagId
        '
        Me.txtTagId.Location = New System.Drawing.Point(9, 37)
        Me.txtTagId.Name = "txtTagId"
        Me.txtTagId.Size = New System.Drawing.Size(110, 20)
        Me.txtTagId.TabIndex = 7
        Me.txtTagId.Text = "16000048"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.Label9)
        Me.GroupBox3.Controls.Add(Me.Button2)
        Me.GroupBox3.Controls.Add(Me.txtLogDir)
        Me.GroupBox3.Location = New System.Drawing.Point(224, 123)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(280, 69)
        Me.GroupBox3.TabIndex = 9
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Logging"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(6, 21)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(73, 13)
        Me.Label9.TabIndex = 6
        Me.Label9.Text = "Log Directory:"
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(249, 37)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(24, 20)
        Me.Button2.TabIndex = 8
        Me.Button2.Text = "..."
        Me.Button2.UseVisualStyleBackColor = True
        '
        'txtLogDir
        '
        Me.txtLogDir.Location = New System.Drawing.Point(9, 37)
        Me.txtLogDir.Name = "txtLogDir"
        Me.txtLogDir.Size = New System.Drawing.Size(234, 20)
        Me.txtLogDir.TabIndex = 7
        Me.txtLogDir.Text = "..."
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Button1)
        Me.GroupBox2.Controls.Add(Me.txtProgramFile)
        Me.GroupBox2.Controls.Add(Me.Label8)
        Me.GroupBox2.Controls.Add(Me.Label2)
        Me.GroupBox2.Controls.Add(Me.cbProgDevice)
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Controls.Add(Me.cbProgTool)
        Me.GroupBox2.Location = New System.Drawing.Point(224, 6)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(280, 111)
        Me.GroupBox2.TabIndex = 1
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Programming Options"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(249, 81)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(24, 20)
        Me.Button1.TabIndex = 5
        Me.Button1.Text = "..."
        Me.Button1.UseVisualStyleBackColor = True
        '
        'txtProgramFile
        '
        Me.txtProgramFile.Location = New System.Drawing.Point(9, 81)
        Me.txtProgramFile.Name = "txtProgramFile"
        Me.txtProgramFile.Size = New System.Drawing.Size(234, 20)
        Me.txtProgramFile.TabIndex = 4
        Me.txtProgramFile.Text = "F:\Wild Spy\Projects_Online\WS005 - WID Logger\3 - Code\Device Firmware\Tags\WidL" & _
    "ogger_V2.3.1-MP_MonitoringProject\Debug\WIDLoggerV2.elf"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(6, 65)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(84, 13)
        Me.Label8.TabIndex = 2
        Me.Label8.Text = "File To Program:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(125, 23)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(44, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Device:"
        '
        'cbProgDevice
        '
        Me.cbProgDevice.FormattingEnabled = True
        Me.cbProgDevice.Items.AddRange(New Object() {"atxmega32a4", "atxmega64a4", "atxmega128a4", "atxmega32a1", "atxmega64a1", "atxmega128a1"})
        Me.cbProgDevice.Location = New System.Drawing.Point(125, 38)
        Me.cbProgDevice.Name = "cbProgDevice"
        Me.cbProgDevice.Size = New System.Drawing.Size(148, 21)
        Me.cbProgDevice.TabIndex = 2
        Me.cbProgDevice.Text = "atxmega32a4"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 23)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(31, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Tool:"
        '
        'cbProgTool
        '
        Me.cbProgTool.FormattingEnabled = True
        Me.cbProgTool.Items.AddRange(New Object() {"avrdragon", "avrispmk2", "avrone", "jtagice3", "jtagicemkii", "qt600", "stk500", "stk600", "samice", "edbg", "medbg", "atmelice", "powerdebugger", "megadfu", "flip"})
        Me.cbProgTool.Location = New System.Drawing.Point(6, 38)
        Me.cbProgTool.Name = "cbProgTool"
        Me.cbProgTool.Size = New System.Drawing.Size(113, 21)
        Me.cbProgTool.TabIndex = 0
        Me.cbProgTool.Text = "avrdragon"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.chkMeasureCurrent)
        Me.GroupBox1.Controls.Add(Me.chkEraseFlash)
        Me.GroupBox1.Controls.Add(Me.chkTestIridiumNetworkConnect)
        Me.GroupBox1.Controls.Add(Me.chkTestIridiumTx)
        Me.GroupBox1.Controls.Add(Me.chkRead9602IMEI)
        Me.GroupBox1.Controls.Add(Me.chkConfig9602)
        Me.GroupBox1.Controls.Add(Me.chkTest433MHzInterLogger)
        Me.GroupBox1.Controls.Add(Me.chk24GHzPickup)
        Me.GroupBox1.Controls.Add(Me.chkSetSettings)
        Me.GroupBox1.Controls.Add(Me.chkProgramLogger)
        Me.GroupBox1.Location = New System.Drawing.Point(8, 6)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(210, 261)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Actions"
        '
        'chkEraseFlash
        '
        Me.chkEraseFlash.AutoSize = True
        Me.chkEraseFlash.Checked = True
        Me.chkEraseFlash.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkEraseFlash.Location = New System.Drawing.Point(22, 226)
        Me.chkEraseFlash.Name = "chkEraseFlash"
        Me.chkEraseFlash.Size = New System.Drawing.Size(81, 17)
        Me.chkEraseFlash.TabIndex = 10
        Me.chkEraseFlash.Text = "Erase Flash"
        Me.chkEraseFlash.UseVisualStyleBackColor = True
        '
        'chkTestIridiumNetworkConnect
        '
        Me.chkTestIridiumNetworkConnect.AutoSize = True
        Me.chkTestIridiumNetworkConnect.Location = New System.Drawing.Point(22, 203)
        Me.chkTestIridiumNetworkConnect.Name = "chkTestIridiumNetworkConnect"
        Me.chkTestIridiumNetworkConnect.Size = New System.Drawing.Size(182, 17)
        Me.chkTestIridiumNetworkConnect.TabIndex = 8
        Me.chkTestIridiumNetworkConnect.Text = "Test Iridium Connect To Network"
        Me.chkTestIridiumNetworkConnect.UseVisualStyleBackColor = True
        '
        'chkTestIridiumTx
        '
        Me.chkTestIridiumTx.AutoSize = True
        Me.chkTestIridiumTx.Location = New System.Drawing.Point(22, 157)
        Me.chkTestIridiumTx.Name = "chkTestIridiumTx"
        Me.chkTestIridiumTx.Size = New System.Drawing.Size(123, 17)
        Me.chkTestIridiumTx.TabIndex = 7
        Me.chkTestIridiumTx.Text = "Test Iridium Transmit"
        Me.chkTestIridiumTx.UseVisualStyleBackColor = True
        '
        'chkRead9602IMEI
        '
        Me.chkRead9602IMEI.AutoSize = True
        Me.chkRead9602IMEI.Checked = True
        Me.chkRead9602IMEI.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkRead9602IMEI.Location = New System.Drawing.Point(22, 134)
        Me.chkRead9602IMEI.Name = "chkRead9602IMEI"
        Me.chkRead9602IMEI.Size = New System.Drawing.Size(127, 17)
        Me.chkRead9602IMEI.TabIndex = 6
        Me.chkRead9602IMEI.Text = "Read IMEI from 9602"
        Me.chkRead9602IMEI.UseVisualStyleBackColor = True
        '
        'chkConfig9602
        '
        Me.chkConfig9602.AutoSize = True
        Me.chkConfig9602.Checked = True
        Me.chkConfig9602.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkConfig9602.Location = New System.Drawing.Point(22, 111)
        Me.chkConfig9602.Name = "chkConfig9602"
        Me.chkConfig9602.Size = New System.Drawing.Size(131, 17)
        Me.chkConfig9602.TabIndex = 5
        Me.chkConfig9602.Text = "Configure Iridium 9602"
        Me.chkConfig9602.UseVisualStyleBackColor = True
        '
        'chkTest433MHzInterLogger
        '
        Me.chkTest433MHzInterLogger.AutoSize = True
        Me.chkTest433MHzInterLogger.Location = New System.Drawing.Point(22, 88)
        Me.chkTest433MHzInterLogger.Name = "chkTest433MHzInterLogger"
        Me.chkTest433MHzInterLogger.Size = New System.Drawing.Size(187, 17)
        Me.chkTest433MHzInterLogger.TabIndex = 4
        Me.chkTest433MHzInterLogger.Text = "Test 433MHz Inter Logger Comms"
        Me.chkTest433MHzInterLogger.UseVisualStyleBackColor = True
        '
        'chk24GHzPickup
        '
        Me.chk24GHzPickup.AutoSize = True
        Me.chk24GHzPickup.Checked = True
        Me.chk24GHzPickup.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chk24GHzPickup.Location = New System.Drawing.Point(22, 65)
        Me.chk24GHzPickup.Name = "chk24GHzPickup"
        Me.chk24GHzPickup.Size = New System.Drawing.Size(149, 17)
        Me.chk24GHzPickup.TabIndex = 3
        Me.chk24GHzPickup.Text = "Test 2.4GHz Tag Pickups"
        Me.chk24GHzPickup.UseVisualStyleBackColor = True
        '
        'chkSetSettings
        '
        Me.chkSetSettings.AutoSize = True
        Me.chkSetSettings.Checked = True
        Me.chkSetSettings.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkSetSettings.Location = New System.Drawing.Point(22, 42)
        Me.chkSetSettings.Name = "chkSetSettings"
        Me.chkSetSettings.Size = New System.Drawing.Size(180, 17)
        Me.chkSetSettings.TabIndex = 2
        Me.chkSetSettings.Text = "Set Settings (as on Setup 2 Tab)"
        Me.chkSetSettings.UseVisualStyleBackColor = True
        '
        'chkProgramLogger
        '
        Me.chkProgramLogger.AutoSize = True
        Me.chkProgramLogger.Checked = True
        Me.chkProgramLogger.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkProgramLogger.Location = New System.Drawing.Point(22, 19)
        Me.chkProgramLogger.Name = "chkProgramLogger"
        Me.chkProgramLogger.Size = New System.Drawing.Size(101, 17)
        Me.chkProgramLogger.TabIndex = 1
        Me.chkProgramLogger.Text = "Program Logger"
        Me.chkProgramLogger.UseVisualStyleBackColor = True
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.TabControlMain)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(512, 490)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Setup 2"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'TabControlMain
        '
        Me.TabControlMain.Controls.Add(Me.TabPage6)
        Me.TabControlMain.Controls.Add(Me.TabPage7)
        Me.TabControlMain.Controls.Add(Me.TabPage8)
        Me.TabControlMain.Controls.Add(Me.TabPage5)
        Me.TabControlMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControlMain.Location = New System.Drawing.Point(3, 3)
        Me.TabControlMain.Name = "TabControlMain"
        Me.TabControlMain.SelectedIndex = 0
        Me.TabControlMain.Size = New System.Drawing.Size(506, 484)
        Me.TabControlMain.TabIndex = 3
        '
        'TabPage6
        '
        Me.TabPage6.Controls.Add(Me.SplitContainer4)
        Me.TabPage6.Location = New System.Drawing.Point(4, 22)
        Me.TabPage6.Name = "TabPage6"
        Me.TabPage6.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage6.Size = New System.Drawing.Size(498, 458)
        Me.TabPage6.TabIndex = 5
        Me.TabPage6.Text = "Overview"
        Me.TabPage6.UseVisualStyleBackColor = True
        '
        'SplitContainer4
        '
        Me.SplitContainer4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.SplitContainer4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer4.Location = New System.Drawing.Point(3, 3)
        Me.SplitContainer4.Name = "SplitContainer4"
        Me.SplitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer4.Panel1
        '
        Me.SplitContainer4.Panel1.Controls.Add(Me.TLPOverview)
        '
        'SplitContainer4.Panel2
        '
        Me.SplitContainer4.Panel2.Controls.Add(Me.btnWriteVals)
        Me.SplitContainer4.Panel2.Controls.Add(Me.btnReadVals)
        Me.SplitContainer4.Size = New System.Drawing.Size(492, 452)
        Me.SplitContainer4.SplitterDistance = 357
        Me.SplitContainer4.TabIndex = 1
        '
        'TLPOverview
        '
        Me.TLPOverview.AutoScroll = True
        Me.TLPOverview.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset
        Me.TLPOverview.ColumnCount = 2
        Me.TLPOverview.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 136.0!))
        Me.TLPOverview.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TLPOverview.Controls.Add(Me.TableLayoutPanel5, 1, 4)
        Me.TLPOverview.Controls.Add(Me.TableLayoutPanel4, 1, 5)
        Me.TLPOverview.Controls.Add(Me.Label22, 1, 0)
        Me.TLPOverview.Controls.Add(Me.Label23, 0, 0)
        Me.TLPOverview.Controls.Add(Me.Label20, 0, 5)
        Me.TLPOverview.Controls.Add(Me.Label3, 0, 4)
        Me.TLPOverview.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TLPOverview.Location = New System.Drawing.Point(0, 0)
        Me.TLPOverview.Margin = New System.Windows.Forms.Padding(0)
        Me.TLPOverview.Name = "TLPOverview"
        Me.TLPOverview.Padding = New System.Windows.Forms.Padding(0, 0, 5, 0)
        Me.TLPOverview.RowCount = 10
        Me.TLPOverview.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TLPOverview.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TLPOverview.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TLPOverview.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TLPOverview.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TLPOverview.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TLPOverview.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27.0!))
        Me.TLPOverview.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TLPOverview.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TLPOverview.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 43.0!))
        Me.TLPOverview.Size = New System.Drawing.Size(490, 355)
        Me.TLPOverview.TabIndex = 1
        '
        'TableLayoutPanel5
        '
        Me.TableLayoutPanel5.ColumnCount = 3
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 42.0!))
        Me.TableLayoutPanel5.Controls.Add(Me.pbRecordCount, 0, 0)
        Me.TableLayoutPanel5.Controls.Add(Me.btnEraseDataflash, 2, 0)
        Me.TableLayoutPanel5.Controls.Add(Me.txtRecordCount, 1, 0)
        Me.TableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel5.Location = New System.Drawing.Point(143, 111)
        Me.TableLayoutPanel5.Name = "TableLayoutPanel5"
        Me.TableLayoutPanel5.RowCount = 1
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel5.Size = New System.Drawing.Size(337, 20)
        Me.TableLayoutPanel5.TabIndex = 22
        '
        'pbRecordCount
        '
        Me.pbRecordCount.BackColor = System.Drawing.Color.White
        Me.pbRecordCount.BackgroundImage = CType(resources.GetObject("pbRecordCount.BackgroundImage"), System.Drawing.Image)
        Me.pbRecordCount.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.pbRecordCount.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pbRecordCount.Location = New System.Drawing.Point(0, 0)
        Me.pbRecordCount.Margin = New System.Windows.Forms.Padding(0)
        Me.pbRecordCount.Name = "pbRecordCount"
        Me.pbRecordCount.Size = New System.Drawing.Size(20, 20)
        Me.pbRecordCount.TabIndex = 13
        Me.pbRecordCount.TabStop = False
        '
        'btnEraseDataflash
        '
        Me.btnEraseDataflash.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnEraseDataflash.Location = New System.Drawing.Point(295, 0)
        Me.btnEraseDataflash.Margin = New System.Windows.Forms.Padding(0)
        Me.btnEraseDataflash.Name = "btnEraseDataflash"
        Me.btnEraseDataflash.Size = New System.Drawing.Size(42, 20)
        Me.btnEraseDataflash.TabIndex = 18
        Me.btnEraseDataflash.Text = "Erase"
        Me.btnEraseDataflash.UseVisualStyleBackColor = True
        '
        'txtRecordCount
        '
        Me.txtRecordCount.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtRecordCount.Location = New System.Drawing.Point(23, 0)
        Me.txtRecordCount.Margin = New System.Windows.Forms.Padding(3, 0, 3, 0)
        Me.txtRecordCount.Name = "txtRecordCount"
        Me.txtRecordCount.Size = New System.Drawing.Size(269, 20)
        Me.txtRecordCount.TabIndex = 19
        '
        'TableLayoutPanel4
        '
        Me.TableLayoutPanel4.ColumnCount = 3
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 42.0!))
        Me.TableLayoutPanel4.Controls.Add(Me.PBDevDateTime, 0, 0)
        Me.TableLayoutPanel4.Controls.Add(Me.btnDevTimeSync, 2, 0)
        Me.TableLayoutPanel4.Controls.Add(Me.dtpDevDateTime, 1, 0)
        Me.TableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel4.Location = New System.Drawing.Point(143, 139)
        Me.TableLayoutPanel4.Name = "TableLayoutPanel4"
        Me.TableLayoutPanel4.RowCount = 1
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel4.Size = New System.Drawing.Size(337, 20)
        Me.TableLayoutPanel4.TabIndex = 20
        '
        'PBDevDateTime
        '
        Me.PBDevDateTime.BackColor = System.Drawing.Color.White
        Me.PBDevDateTime.BackgroundImage = CType(resources.GetObject("PBDevDateTime.BackgroundImage"), System.Drawing.Image)
        Me.PBDevDateTime.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.PBDevDateTime.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PBDevDateTime.Location = New System.Drawing.Point(0, 0)
        Me.PBDevDateTime.Margin = New System.Windows.Forms.Padding(0)
        Me.PBDevDateTime.Name = "PBDevDateTime"
        Me.PBDevDateTime.Size = New System.Drawing.Size(20, 20)
        Me.PBDevDateTime.TabIndex = 25
        Me.PBDevDateTime.TabStop = False
        '
        'btnDevTimeSync
        '
        Me.btnDevTimeSync.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnDevTimeSync.Location = New System.Drawing.Point(295, 0)
        Me.btnDevTimeSync.Margin = New System.Windows.Forms.Padding(0)
        Me.btnDevTimeSync.Name = "btnDevTimeSync"
        Me.btnDevTimeSync.Size = New System.Drawing.Size(42, 20)
        Me.btnDevTimeSync.TabIndex = 18
        Me.btnDevTimeSync.Text = "Sync"
        Me.btnDevTimeSync.UseVisualStyleBackColor = True
        '
        'dtpDevDateTime
        '
        Me.dtpDevDateTime.CalendarTitleBackColor = System.Drawing.SystemColors.ControlText
        Me.dtpDevDateTime.CalendarTitleForeColor = System.Drawing.SystemColors.HotTrack
        Me.dtpDevDateTime.CustomFormat = "dddd dd,MMMM, yyyy @ HH:mm:ss"
        Me.dtpDevDateTime.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dtpDevDateTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpDevDateTime.Location = New System.Drawing.Point(23, 0)
        Me.dtpDevDateTime.Margin = New System.Windows.Forms.Padding(3, 0, 3, 0)
        Me.dtpDevDateTime.Name = "dtpDevDateTime"
        Me.dtpDevDateTime.Size = New System.Drawing.Size(269, 20)
        Me.dtpDevDateTime.TabIndex = 17
        '
        'Label22
        '
        Me.Label22.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.Label22.AutoSize = True
        Me.Label22.BackColor = System.Drawing.Color.Transparent
        Me.Label22.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label22.Location = New System.Drawing.Point(287, 2)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(48, 16)
        Me.Label22.TabIndex = 6
        Me.Label22.Text = "Value"
        '
        'Label23
        '
        Me.Label23.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.Label23.AutoSize = True
        Me.Label23.BackColor = System.Drawing.Color.Transparent
        Me.Label23.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label23.Location = New System.Drawing.Point(45, 2)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(49, 16)
        Me.Label23.TabIndex = 1
        Me.Label23.Text = "Name"
        '
        'Label20
        '
        Me.Label20.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.Label20.AutoSize = True
        Me.Label20.Location = New System.Drawing.Point(5, 142)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(55, 13)
        Me.Label20.TabIndex = 21
        Me.Label20.Text = "RTC Time"
        '
        'Label3
        '
        Me.Label3.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(5, 114)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(73, 13)
        Me.Label3.TabIndex = 23
        Me.Label3.Text = "Record Count"
        '
        'btnWriteVals
        '
        Me.btnWriteVals.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnWriteVals.Location = New System.Drawing.Point(411, 3)
        Me.btnWriteVals.Name = "btnWriteVals"
        Me.btnWriteVals.Size = New System.Drawing.Size(75, 23)
        Me.btnWriteVals.TabIndex = 1
        Me.btnWriteVals.Text = "Write"
        Me.btnWriteVals.UseVisualStyleBackColor = True
        '
        'btnReadVals
        '
        Me.btnReadVals.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnReadVals.Location = New System.Drawing.Point(330, 3)
        Me.btnReadVals.Name = "btnReadVals"
        Me.btnReadVals.Size = New System.Drawing.Size(75, 23)
        Me.btnReadVals.TabIndex = 0
        Me.btnReadVals.Text = "Read"
        Me.btnReadVals.UseVisualStyleBackColor = True
        '
        'TabPage7
        '
        Me.TabPage7.Controls.Add(Me.SplitContainer5)
        Me.TabPage7.Location = New System.Drawing.Point(4, 22)
        Me.TabPage7.Name = "TabPage7"
        Me.TabPage7.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage7.Size = New System.Drawing.Size(498, 458)
        Me.TabPage7.TabIndex = 6
        Me.TabPage7.Text = "RF Settings"
        Me.TabPage7.UseVisualStyleBackColor = True
        '
        'SplitContainer5
        '
        Me.SplitContainer5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.SplitContainer5.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer5.Location = New System.Drawing.Point(3, 3)
        Me.SplitContainer5.Name = "SplitContainer5"
        Me.SplitContainer5.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer5.Panel1
        '
        Me.SplitContainer5.Panel1.Controls.Add(Me.TLPRFSet)
        '
        'SplitContainer5.Panel2
        '
        Me.SplitContainer5.Panel2.Controls.Add(Me.btnWriteRFVals)
        Me.SplitContainer5.Panel2.Controls.Add(Me.btnReadRFVals)
        Me.SplitContainer5.Size = New System.Drawing.Size(492, 452)
        Me.SplitContainer5.SplitterDistance = 323
        Me.SplitContainer5.TabIndex = 1
        '
        'TLPRFSet
        '
        Me.TLPRFSet.AutoScroll = True
        Me.TLPRFSet.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset
        Me.TLPRFSet.ColumnCount = 2
        Me.TLPRFSet.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 136.0!))
        Me.TLPRFSet.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TLPRFSet.Controls.Add(Me.TableLayoutPanel9, 1, 3)
        Me.TLPRFSet.Controls.Add(Me.TableLayoutPanel8, 1, 4)
        Me.TLPRFSet.Controls.Add(Me.Label6, 0, 4)
        Me.TLPRFSet.Controls.Add(Me.Label5, 0, 3)
        Me.TLPRFSet.Controls.Add(Me.TableLayoutPanel3, 1, 2)
        Me.TLPRFSet.Controls.Add(Me.Label21, 1, 0)
        Me.TLPRFSet.Controls.Add(Me.Label26, 0, 0)
        Me.TLPRFSet.Controls.Add(Me.Label24, 0, 2)
        Me.TLPRFSet.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TLPRFSet.Location = New System.Drawing.Point(0, 0)
        Me.TLPRFSet.Margin = New System.Windows.Forms.Padding(0)
        Me.TLPRFSet.Name = "TLPRFSet"
        Me.TLPRFSet.Padding = New System.Windows.Forms.Padding(0, 0, 5, 0)
        Me.TLPRFSet.RowCount = 5
        Me.TLPRFSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TLPRFSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TLPRFSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TLPRFSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 115.0!))
        Me.TLPRFSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 115.0!))
        Me.TLPRFSet.Size = New System.Drawing.Size(490, 321)
        Me.TLPRFSet.TabIndex = 0
        '
        'TableLayoutPanel9
        '
        Me.TableLayoutPanel9.ColumnCount = 2
        Me.TableLayoutPanel9.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel9.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel9.Controls.Add(Me.pbRFParams, 0, 0)
        Me.TableLayoutPanel9.Controls.Add(Me.txtRFParams, 1, 0)
        Me.TableLayoutPanel9.Controls.Add(Me.cbRFParams, 1, 1)
        Me.TableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel9.Location = New System.Drawing.Point(143, 83)
        Me.TableLayoutPanel9.Name = "TableLayoutPanel9"
        Me.TableLayoutPanel9.RowCount = 2
        Me.TableLayoutPanel9.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel9.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel9.Size = New System.Drawing.Size(337, 109)
        Me.TableLayoutPanel9.TabIndex = 26
        '
        'pbRFParams
        '
        Me.pbRFParams.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.pbRFParams.BackColor = System.Drawing.Color.White
        Me.pbRFParams.BackgroundImage = CType(resources.GetObject("pbRFParams.BackgroundImage"), System.Drawing.Image)
        Me.pbRFParams.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.pbRFParams.Location = New System.Drawing.Point(0, 31)
        Me.pbRFParams.Margin = New System.Windows.Forms.Padding(0)
        Me.pbRFParams.Name = "pbRFParams"
        Me.pbRFParams.Size = New System.Drawing.Size(20, 20)
        Me.pbRFParams.TabIndex = 25
        Me.pbRFParams.TabStop = False
        '
        'txtRFParams
        '
        Me.txtRFParams.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtRFParams.Location = New System.Drawing.Point(23, 3)
        Me.txtRFParams.Multiline = True
        Me.txtRFParams.Name = "txtRFParams"
        Me.txtRFParams.Size = New System.Drawing.Size(311, 77)
        Me.txtRFParams.TabIndex = 26
        '
        'cbRFParams
        '
        Me.cbRFParams.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cbRFParams.FormattingEnabled = True
        Me.cbRFParams.Location = New System.Drawing.Point(23, 86)
        Me.cbRFParams.Name = "cbRFParams"
        Me.cbRFParams.Size = New System.Drawing.Size(311, 21)
        Me.cbRFParams.TabIndex = 27
        '
        'TableLayoutPanel8
        '
        Me.TableLayoutPanel8.ColumnCount = 2
        Me.TableLayoutPanel8.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel8.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel8.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel8.Controls.Add(Me.pbInterloggerRFParams, 0, 0)
        Me.TableLayoutPanel8.Controls.Add(Me.txtInterloggerRFParams, 1, 0)
        Me.TableLayoutPanel8.Controls.Add(Me.cbInterloggerRFParams, 1, 1)
        Me.TableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel8.Location = New System.Drawing.Point(143, 200)
        Me.TableLayoutPanel8.Name = "TableLayoutPanel8"
        Me.TableLayoutPanel8.RowCount = 2
        Me.TableLayoutPanel8.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel8.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TableLayoutPanel8.Size = New System.Drawing.Size(337, 116)
        Me.TableLayoutPanel8.TabIndex = 25
        '
        'pbInterloggerRFParams
        '
        Me.pbInterloggerRFParams.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.pbInterloggerRFParams.BackColor = System.Drawing.Color.White
        Me.pbInterloggerRFParams.BackgroundImage = CType(resources.GetObject("pbInterloggerRFParams.BackgroundImage"), System.Drawing.Image)
        Me.pbInterloggerRFParams.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.pbInterloggerRFParams.Location = New System.Drawing.Point(0, 35)
        Me.pbInterloggerRFParams.Margin = New System.Windows.Forms.Padding(0)
        Me.pbInterloggerRFParams.Name = "pbInterloggerRFParams"
        Me.pbInterloggerRFParams.Size = New System.Drawing.Size(20, 20)
        Me.pbInterloggerRFParams.TabIndex = 25
        Me.pbInterloggerRFParams.TabStop = False
        '
        'txtInterloggerRFParams
        '
        Me.txtInterloggerRFParams.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtInterloggerRFParams.Location = New System.Drawing.Point(23, 3)
        Me.txtInterloggerRFParams.Multiline = True
        Me.txtInterloggerRFParams.Name = "txtInterloggerRFParams"
        Me.txtInterloggerRFParams.Size = New System.Drawing.Size(311, 84)
        Me.txtInterloggerRFParams.TabIndex = 26
        '
        'cbInterloggerRFParams
        '
        Me.cbInterloggerRFParams.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cbInterloggerRFParams.FormattingEnabled = True
        Me.cbInterloggerRFParams.Location = New System.Drawing.Point(23, 93)
        Me.cbInterloggerRFParams.Name = "cbInterloggerRFParams"
        Me.cbInterloggerRFParams.Size = New System.Drawing.Size(311, 21)
        Me.cbInterloggerRFParams.TabIndex = 27
        '
        'Label6
        '
        Me.Label6.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(5, 251)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(119, 13)
        Me.Label6.TabIndex = 24
        Me.Label6.Text = "Inter Logger RF Params"
        '
        'Label5
        '
        Me.Label5.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(5, 131)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(59, 13)
        Me.Label5.TabIndex = 23
        Me.Label5.Text = "RF Params"
        '
        'TableLayoutPanel3
        '
        Me.TableLayoutPanel3.ColumnCount = 2
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel3.Controls.Add(Me.pbVerboseMode, 0, 0)
        Me.TableLayoutPanel3.Controls.Add(Me.cbVerboseMode, 1, 0)
        Me.TableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel3.Location = New System.Drawing.Point(143, 55)
        Me.TableLayoutPanel3.Name = "TableLayoutPanel3"
        Me.TableLayoutPanel3.RowCount = 1
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel3.Size = New System.Drawing.Size(337, 20)
        Me.TableLayoutPanel3.TabIndex = 21
        '
        'pbVerboseMode
        '
        Me.pbVerboseMode.BackColor = System.Drawing.Color.White
        Me.pbVerboseMode.BackgroundImage = CType(resources.GetObject("pbVerboseMode.BackgroundImage"), System.Drawing.Image)
        Me.pbVerboseMode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.pbVerboseMode.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pbVerboseMode.Location = New System.Drawing.Point(0, 0)
        Me.pbVerboseMode.Margin = New System.Windows.Forms.Padding(0)
        Me.pbVerboseMode.Name = "pbVerboseMode"
        Me.pbVerboseMode.Size = New System.Drawing.Size(20, 20)
        Me.pbVerboseMode.TabIndex = 28
        Me.pbVerboseMode.TabStop = False
        '
        'cbVerboseMode
        '
        Me.cbVerboseMode.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cbVerboseMode.FormattingEnabled = True
        Me.cbVerboseMode.Items.AddRange(New Object() {"1 - Silent", "2 - Print Tag ID", "3 - Pint Tag ID and Time", "4 - Print Tag ID, Time and Address"})
        Me.cbVerboseMode.Location = New System.Drawing.Point(23, 0)
        Me.cbVerboseMode.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.cbVerboseMode.Name = "cbVerboseMode"
        Me.cbVerboseMode.Size = New System.Drawing.Size(314, 21)
        Me.cbVerboseMode.TabIndex = 27
        Me.cbVerboseMode.Text = "1 - Silent"
        '
        'Label21
        '
        Me.Label21.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.Label21.AutoSize = True
        Me.Label21.BackColor = System.Drawing.Color.Transparent
        Me.Label21.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label21.Location = New System.Drawing.Point(287, 2)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(48, 16)
        Me.Label21.TabIndex = 6
        Me.Label21.Text = "Value"
        '
        'Label26
        '
        Me.Label26.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.Label26.AutoSize = True
        Me.Label26.BackColor = System.Drawing.Color.Transparent
        Me.Label26.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label26.Location = New System.Drawing.Point(45, 2)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(49, 16)
        Me.Label26.TabIndex = 1
        Me.Label26.Text = "Name"
        '
        'Label24
        '
        Me.Label24.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.Label24.AutoSize = True
        Me.Label24.Location = New System.Drawing.Point(5, 58)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(76, 13)
        Me.Label24.TabIndex = 22
        Me.Label24.Text = "Verbose Mode"
        '
        'btnWriteRFVals
        '
        Me.btnWriteRFVals.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnWriteRFVals.Location = New System.Drawing.Point(411, 3)
        Me.btnWriteRFVals.Name = "btnWriteRFVals"
        Me.btnWriteRFVals.Size = New System.Drawing.Size(75, 23)
        Me.btnWriteRFVals.TabIndex = 1
        Me.btnWriteRFVals.Text = "Write"
        Me.btnWriteRFVals.UseVisualStyleBackColor = True
        '
        'btnReadRFVals
        '
        Me.btnReadRFVals.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnReadRFVals.Location = New System.Drawing.Point(330, 3)
        Me.btnReadRFVals.Name = "btnReadRFVals"
        Me.btnReadRFVals.Size = New System.Drawing.Size(75, 23)
        Me.btnReadRFVals.TabIndex = 1
        Me.btnReadRFVals.Text = "Read"
        Me.btnReadRFVals.UseVisualStyleBackColor = True
        '
        'TabPage8
        '
        Me.TabPage8.Controls.Add(Me.SplitContainer6)
        Me.TabPage8.Location = New System.Drawing.Point(4, 22)
        Me.TabPage8.Name = "TabPage8"
        Me.TabPage8.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage8.Size = New System.Drawing.Size(498, 458)
        Me.TabPage8.TabIndex = 7
        Me.TabPage8.Text = "GSM Settings"
        Me.TabPage8.UseVisualStyleBackColor = True
        '
        'SplitContainer6
        '
        Me.SplitContainer6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.SplitContainer6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer6.Location = New System.Drawing.Point(3, 3)
        Me.SplitContainer6.Name = "SplitContainer6"
        Me.SplitContainer6.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer6.Panel1
        '
        Me.SplitContainer6.Panel1.Controls.Add(Me.TLPGSMSet)
        '
        'SplitContainer6.Panel2
        '
        Me.SplitContainer6.Panel2.Controls.Add(Me.btnWriteGSMVals)
        Me.SplitContainer6.Panel2.Controls.Add(Me.btnReadGSMVals)
        Me.SplitContainer6.Size = New System.Drawing.Size(492, 452)
        Me.SplitContainer6.SplitterDistance = 358
        Me.SplitContainer6.TabIndex = 1
        '
        'TLPGSMSet
        '
        Me.TLPGSMSet.AutoScroll = True
        Me.TLPGSMSet.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset
        Me.TLPGSMSet.ColumnCount = 2
        Me.TLPGSMSet.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 136.0!))
        Me.TLPGSMSet.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TLPGSMSet.Controls.Add(Me.TableLayoutPanel10, 1, 3)
        Me.TLPGSMSet.Controls.Add(Me.Label7, 0, 3)
        Me.TLPGSMSet.Controls.Add(Me.TableLayoutPanel7, 1, 7)
        Me.TLPGSMSet.Controls.Add(Me.TableLayoutPanel6, 1, 2)
        Me.TLPGSMSet.Controls.Add(Me.Label25, 1, 0)
        Me.TLPGSMSet.Controls.Add(Me.Label27, 0, 0)
        Me.TLPGSMSet.Controls.Add(Me.Label28, 0, 2)
        Me.TLPGSMSet.Controls.Add(Me.Label4, 0, 7)
        Me.TLPGSMSet.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TLPGSMSet.Location = New System.Drawing.Point(0, 0)
        Me.TLPGSMSet.Margin = New System.Windows.Forms.Padding(0)
        Me.TLPGSMSet.Name = "TLPGSMSet"
        Me.TLPGSMSet.Padding = New System.Windows.Forms.Padding(0, 0, 5, 0)
        Me.TLPGSMSet.RowCount = 10
        Me.TLPGSMSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TLPGSMSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TLPGSMSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TLPGSMSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TLPGSMSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27.0!))
        Me.TLPGSMSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TLPGSMSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TLPGSMSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TLPGSMSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TLPGSMSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 43.0!))
        Me.TLPGSMSet.Size = New System.Drawing.Size(490, 356)
        Me.TLPGSMSet.TabIndex = 0
        '
        'TableLayoutPanel10
        '
        Me.TableLayoutPanel10.ColumnCount = 2
        Me.TableLayoutPanel10.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel10.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel10.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel10.Controls.Add(Me.pbNextDataReset, 0, 0)
        Me.TableLayoutPanel10.Controls.Add(Me.dtpNextDataReset, 1, 0)
        Me.TableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel10.Location = New System.Drawing.Point(143, 83)
        Me.TableLayoutPanel10.Name = "TableLayoutPanel10"
        Me.TableLayoutPanel10.RowCount = 1
        Me.TableLayoutPanel10.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel10.Size = New System.Drawing.Size(337, 20)
        Me.TableLayoutPanel10.TabIndex = 28
        '
        'pbNextDataReset
        '
        Me.pbNextDataReset.BackColor = System.Drawing.Color.White
        Me.pbNextDataReset.BackgroundImage = CType(resources.GetObject("pbNextDataReset.BackgroundImage"), System.Drawing.Image)
        Me.pbNextDataReset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.pbNextDataReset.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pbNextDataReset.Location = New System.Drawing.Point(0, 0)
        Me.pbNextDataReset.Margin = New System.Windows.Forms.Padding(0)
        Me.pbNextDataReset.Name = "pbNextDataReset"
        Me.pbNextDataReset.Size = New System.Drawing.Size(20, 20)
        Me.pbNextDataReset.TabIndex = 28
        Me.pbNextDataReset.TabStop = False
        '
        'dtpNextDataReset
        '
        Me.dtpNextDataReset.CalendarTitleBackColor = System.Drawing.SystemColors.ControlText
        Me.dtpNextDataReset.CalendarTitleForeColor = System.Drawing.SystemColors.HotTrack
        Me.dtpNextDataReset.CustomFormat = "dddd dd,MMMM, yyyy @ HH:mm:ss"
        Me.dtpNextDataReset.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dtpNextDataReset.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpNextDataReset.Location = New System.Drawing.Point(23, 0)
        Me.dtpNextDataReset.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.dtpNextDataReset.Name = "dtpNextDataReset"
        Me.dtpNextDataReset.Size = New System.Drawing.Size(314, 20)
        Me.dtpNextDataReset.TabIndex = 27
        '
        'Label7
        '
        Me.Label7.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(5, 86)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(86, 13)
        Me.Label7.TabIndex = 27
        Me.Label7.Text = "Next Data Reset"
        '
        'TableLayoutPanel7
        '
        Me.TableLayoutPanel7.ColumnCount = 2
        Me.TableLayoutPanel7.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel7.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel7.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel7.Controls.Add(Me.pbGPRSSettings, 0, 0)
        Me.TableLayoutPanel7.Controls.Add(Me.cbGPRSSettings, 1, 0)
        Me.TableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel7.Location = New System.Drawing.Point(143, 196)
        Me.TableLayoutPanel7.Name = "TableLayoutPanel7"
        Me.TableLayoutPanel7.RowCount = 1
        Me.TableLayoutPanel7.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel7.Size = New System.Drawing.Size(337, 20)
        Me.TableLayoutPanel7.TabIndex = 26
        Me.TableLayoutPanel7.Visible = False
        '
        'pbGPRSSettings
        '
        Me.pbGPRSSettings.BackColor = System.Drawing.Color.White
        Me.pbGPRSSettings.BackgroundImage = CType(resources.GetObject("pbGPRSSettings.BackgroundImage"), System.Drawing.Image)
        Me.pbGPRSSettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.pbGPRSSettings.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pbGPRSSettings.Location = New System.Drawing.Point(0, 0)
        Me.pbGPRSSettings.Margin = New System.Windows.Forms.Padding(0)
        Me.pbGPRSSettings.Name = "pbGPRSSettings"
        Me.pbGPRSSettings.Size = New System.Drawing.Size(20, 20)
        Me.pbGPRSSettings.TabIndex = 29
        Me.pbGPRSSettings.TabStop = False
        '
        'cbGPRSSettings
        '
        Me.cbGPRSSettings.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cbGPRSSettings.FormattingEnabled = True
        Me.cbGPRSSettings.Items.AddRange(New Object() {"NONE - APN="""", USERID="""", PASSW=""""", "OPTUS - APN=""Internet"", USERID="""", PASSW="""""})
        Me.cbGPRSSettings.Location = New System.Drawing.Point(23, 0)
        Me.cbGPRSSettings.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.cbGPRSSettings.Name = "cbGPRSSettings"
        Me.cbGPRSSettings.Size = New System.Drawing.Size(314, 21)
        Me.cbGPRSSettings.TabIndex = 28
        '
        'TableLayoutPanel6
        '
        Me.TableLayoutPanel6.ColumnCount = 2
        Me.TableLayoutPanel6.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel6.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel6.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel6.Controls.Add(Me.pbSendEmailNext, 0, 0)
        Me.TableLayoutPanel6.Controls.Add(Me.dtpSendEmailNext, 1, 0)
        Me.TableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel6.Location = New System.Drawing.Point(143, 55)
        Me.TableLayoutPanel6.Name = "TableLayoutPanel6"
        Me.TableLayoutPanel6.RowCount = 1
        Me.TableLayoutPanel6.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel6.Size = New System.Drawing.Size(337, 20)
        Me.TableLayoutPanel6.TabIndex = 23
        '
        'pbSendEmailNext
        '
        Me.pbSendEmailNext.BackColor = System.Drawing.Color.White
        Me.pbSendEmailNext.BackgroundImage = CType(resources.GetObject("pbSendEmailNext.BackgroundImage"), System.Drawing.Image)
        Me.pbSendEmailNext.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.pbSendEmailNext.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pbSendEmailNext.Location = New System.Drawing.Point(0, 0)
        Me.pbSendEmailNext.Margin = New System.Windows.Forms.Padding(0)
        Me.pbSendEmailNext.Name = "pbSendEmailNext"
        Me.pbSendEmailNext.Size = New System.Drawing.Size(20, 20)
        Me.pbSendEmailNext.TabIndex = 28
        Me.pbSendEmailNext.TabStop = False
        '
        'dtpSendEmailNext
        '
        Me.dtpSendEmailNext.CalendarTitleBackColor = System.Drawing.SystemColors.ControlText
        Me.dtpSendEmailNext.CalendarTitleForeColor = System.Drawing.SystemColors.HotTrack
        Me.dtpSendEmailNext.CustomFormat = "dddd dd,MMMM, yyyy @ HH:mm:ss"
        Me.dtpSendEmailNext.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dtpSendEmailNext.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpSendEmailNext.Location = New System.Drawing.Point(23, 0)
        Me.dtpSendEmailNext.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.dtpSendEmailNext.Name = "dtpSendEmailNext"
        Me.dtpSendEmailNext.Size = New System.Drawing.Size(314, 20)
        Me.dtpSendEmailNext.TabIndex = 27
        '
        'Label25
        '
        Me.Label25.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.Label25.AutoSize = True
        Me.Label25.BackColor = System.Drawing.Color.Transparent
        Me.Label25.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label25.Location = New System.Drawing.Point(287, 2)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(48, 16)
        Me.Label25.TabIndex = 6
        Me.Label25.Text = "Value"
        '
        'Label27
        '
        Me.Label27.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.Label27.AutoSize = True
        Me.Label27.BackColor = System.Drawing.Color.Transparent
        Me.Label27.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label27.Location = New System.Drawing.Point(45, 2)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(49, 16)
        Me.Label27.TabIndex = 1
        Me.Label27.Text = "Name"
        '
        'Label28
        '
        Me.Label28.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.Label28.AutoSize = True
        Me.Label28.Location = New System.Drawing.Point(5, 58)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(73, 13)
        Me.Label28.TabIndex = 22
        Me.Label28.Text = "Next Wakeup"
        '
        'Label4
        '
        Me.Label4.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(5, 199)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(78, 13)
        Me.Label4.TabIndex = 24
        Me.Label4.Text = "GPRS Settings"
        Me.Label4.Visible = False
        '
        'btnWriteGSMVals
        '
        Me.btnWriteGSMVals.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnWriteGSMVals.Location = New System.Drawing.Point(411, 3)
        Me.btnWriteGSMVals.Name = "btnWriteGSMVals"
        Me.btnWriteGSMVals.Size = New System.Drawing.Size(75, 23)
        Me.btnWriteGSMVals.TabIndex = 1
        Me.btnWriteGSMVals.Text = "Write"
        Me.btnWriteGSMVals.UseVisualStyleBackColor = True
        '
        'btnReadGSMVals
        '
        Me.btnReadGSMVals.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnReadGSMVals.Location = New System.Drawing.Point(330, 3)
        Me.btnReadGSMVals.Name = "btnReadGSMVals"
        Me.btnReadGSMVals.Size = New System.Drawing.Size(75, 23)
        Me.btnReadGSMVals.TabIndex = 1
        Me.btnReadGSMVals.Text = "Read"
        Me.btnReadGSMVals.UseVisualStyleBackColor = True
        '
        'TabPage5
        '
        Me.TabPage5.Controls.Add(Me.rtbSerOut)
        Me.TabPage5.Location = New System.Drawing.Point(4, 22)
        Me.TabPage5.Name = "TabPage5"
        Me.TabPage5.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage5.Size = New System.Drawing.Size(498, 458)
        Me.TabPage5.TabIndex = 4
        Me.TabPage5.Text = "Tag Pickup"
        Me.TabPage5.UseVisualStyleBackColor = True
        '
        'rtbSerOut
        '
        Me.rtbSerOut.BackColor = System.Drawing.Color.Black
        Me.rtbSerOut.Dock = System.Windows.Forms.DockStyle.Fill
        Me.rtbSerOut.Font = New System.Drawing.Font("Courier New", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rtbSerOut.ForeColor = System.Drawing.Color.Yellow
        Me.rtbSerOut.Location = New System.Drawing.Point(3, 3)
        Me.rtbSerOut.Name = "rtbSerOut"
        Me.rtbSerOut.ReadOnly = True
        Me.rtbSerOut.Size = New System.Drawing.Size(492, 452)
        Me.rtbSerOut.TabIndex = 0
        Me.rtbSerOut.Text = ""
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.SplitContainer1)
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage3.Size = New System.Drawing.Size(512, 490)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "Program"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(3, 3)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.rtbProgConsole)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.lblInfo)
        Me.SplitContainer1.Panel2.Controls.Add(Me.btnStartProgramming)
        Me.SplitContainer1.Size = New System.Drawing.Size(506, 484)
        Me.SplitContainer1.SplitterDistance = 400
        Me.SplitContainer1.TabIndex = 0
        '
        'rtbProgConsole
        '
        Me.rtbProgConsole.BackColor = System.Drawing.Color.Black
        Me.rtbProgConsole.Dock = System.Windows.Forms.DockStyle.Fill
        Me.rtbProgConsole.Font = New System.Drawing.Font("Courier New", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rtbProgConsole.ForeColor = System.Drawing.Color.Yellow
        Me.rtbProgConsole.Location = New System.Drawing.Point(0, 0)
        Me.rtbProgConsole.Name = "rtbProgConsole"
        Me.rtbProgConsole.ReadOnly = True
        Me.rtbProgConsole.Size = New System.Drawing.Size(506, 400)
        Me.rtbProgConsole.TabIndex = 2
        Me.rtbProgConsole.Text = ""
        '
        'btnStartProgramming
        '
        Me.btnStartProgramming.Location = New System.Drawing.Point(5, 3)
        Me.btnStartProgramming.Name = "btnStartProgramming"
        Me.btnStartProgramming.Size = New System.Drawing.Size(75, 23)
        Me.btnStartProgramming.TabIndex = 0
        Me.btnStartProgramming.Text = "Start"
        Me.btnStartProgramming.UseVisualStyleBackColor = True
        '
        'ofdProgFile
        '
        Me.ofdProgFile.Filter = "elf Files (*.elf)|*.elf|All Files (*.*) |*.*"
        '
        'lblInfo
        '
        Me.lblInfo.AutoSize = True
        Me.lblInfo.Location = New System.Drawing.Point(5, 29)
        Me.lblInfo.Name = "lblInfo"
        Me.lblInfo.Size = New System.Drawing.Size(136, 13)
        Me.lblInfo.TabIndex = 1
        Me.lblInfo.Text = "Programming Logger ??/??"
        '
        'chkMeasureCurrent
        '
        Me.chkMeasureCurrent.AutoSize = True
        Me.chkMeasureCurrent.Checked = True
        Me.chkMeasureCurrent.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkMeasureCurrent.Location = New System.Drawing.Point(22, 180)
        Me.chkMeasureCurrent.Name = "chkMeasureCurrent"
        Me.chkMeasureCurrent.Size = New System.Drawing.Size(132, 17)
        Me.chkMeasureCurrent.TabIndex = 11
        Me.chkMeasureCurrent.Text = "Measure Current Draw"
        Me.chkMeasureCurrent.UseVisualStyleBackColor = True
        '
        'FrmBatchProgrammer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(520, 516)
        Me.Controls.Add(Me.tcMain)
        Me.Name = "FrmBatchProgrammer"
        Me.Text = "FrmBatchProgrammer"
        Me.tcMain.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        CType(Me.nudLoggerNameStart, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.TabPage2.ResumeLayout(False)
        Me.TabControlMain.ResumeLayout(False)
        Me.TabPage6.ResumeLayout(False)
        Me.SplitContainer4.Panel1.ResumeLayout(False)
        Me.SplitContainer4.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer4.ResumeLayout(False)
        Me.TLPOverview.ResumeLayout(False)
        Me.TLPOverview.PerformLayout()
        Me.TableLayoutPanel5.ResumeLayout(False)
        Me.TableLayoutPanel5.PerformLayout()
        CType(Me.pbRecordCount, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanel4.ResumeLayout(False)
        CType(Me.PBDevDateTime, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage7.ResumeLayout(False)
        Me.SplitContainer5.Panel1.ResumeLayout(False)
        Me.SplitContainer5.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer5, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer5.ResumeLayout(False)
        Me.TLPRFSet.ResumeLayout(False)
        Me.TLPRFSet.PerformLayout()
        Me.TableLayoutPanel9.ResumeLayout(False)
        Me.TableLayoutPanel9.PerformLayout()
        CType(Me.pbRFParams, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanel8.ResumeLayout(False)
        Me.TableLayoutPanel8.PerformLayout()
        CType(Me.pbInterloggerRFParams, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanel3.ResumeLayout(False)
        CType(Me.pbVerboseMode, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage8.ResumeLayout(False)
        Me.SplitContainer6.Panel1.ResumeLayout(False)
        Me.SplitContainer6.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer6, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer6.ResumeLayout(False)
        Me.TLPGSMSet.ResumeLayout(False)
        Me.TLPGSMSet.PerformLayout()
        Me.TableLayoutPanel10.ResumeLayout(False)
        CType(Me.pbNextDataReset, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanel7.ResumeLayout(False)
        CType(Me.pbGPRSSettings, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanel6.ResumeLayout(False)
        CType(Me.pbSendEmailNext, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage5.ResumeLayout(False)
        Me.TabPage3.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.Panel2.PerformLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tcMain As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents TabControlMain As System.Windows.Forms.TabControl
    Friend WithEvents TabPage6 As System.Windows.Forms.TabPage
    Friend WithEvents SplitContainer4 As System.Windows.Forms.SplitContainer
    Friend WithEvents TLPOverview As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel5 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents pbRecordCount As System.Windows.Forms.PictureBox
    Friend WithEvents btnEraseDataflash As System.Windows.Forms.Button
    Friend WithEvents txtRecordCount As System.Windows.Forms.TextBox
    Friend WithEvents TableLayoutPanel4 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents PBDevDateTime As System.Windows.Forms.PictureBox
    Friend WithEvents btnDevTimeSync As System.Windows.Forms.Button
    Friend WithEvents dtpDevDateTime As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents btnWriteVals As System.Windows.Forms.Button
    Friend WithEvents btnReadVals As System.Windows.Forms.Button
    Friend WithEvents TabPage7 As System.Windows.Forms.TabPage
    Friend WithEvents SplitContainer5 As System.Windows.Forms.SplitContainer
    Friend WithEvents TLPRFSet As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel9 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents pbRFParams As System.Windows.Forms.PictureBox
    Friend WithEvents txtRFParams As System.Windows.Forms.TextBox
    Friend WithEvents cbRFParams As System.Windows.Forms.ComboBox
    Friend WithEvents TableLayoutPanel8 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents pbInterloggerRFParams As System.Windows.Forms.PictureBox
    Friend WithEvents txtInterloggerRFParams As System.Windows.Forms.TextBox
    Friend WithEvents cbInterloggerRFParams As System.Windows.Forms.ComboBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents TableLayoutPanel3 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents pbVerboseMode As System.Windows.Forms.PictureBox
    Friend WithEvents cbVerboseMode As System.Windows.Forms.ComboBox
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents Label26 As System.Windows.Forms.Label
    Friend WithEvents Label24 As System.Windows.Forms.Label
    Friend WithEvents btnWriteRFVals As System.Windows.Forms.Button
    Friend WithEvents btnReadRFVals As System.Windows.Forms.Button
    Friend WithEvents TabPage8 As System.Windows.Forms.TabPage
    Friend WithEvents SplitContainer6 As System.Windows.Forms.SplitContainer
    Friend WithEvents TLPGSMSet As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel10 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents pbNextDataReset As System.Windows.Forms.PictureBox
    Friend WithEvents dtpNextDataReset As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents TableLayoutPanel7 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents pbGPRSSettings As System.Windows.Forms.PictureBox
    Friend WithEvents cbGPRSSettings As System.Windows.Forms.ComboBox
    Friend WithEvents TableLayoutPanel6 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents pbSendEmailNext As System.Windows.Forms.PictureBox
    Friend WithEvents dtpSendEmailNext As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label25 As System.Windows.Forms.Label
    Friend WithEvents Label27 As System.Windows.Forms.Label
    Friend WithEvents Label28 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents btnWriteGSMVals As System.Windows.Forms.Button
    Friend WithEvents btnReadGSMVals As System.Windows.Forms.Button
    Friend WithEvents TabPage5 As System.Windows.Forms.TabPage
    Friend WithEvents rtbSerOut As System.Windows.Forms.RichTextBox
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents rtbProgConsole As System.Windows.Forms.RichTextBox
    Friend WithEvents btnStartProgramming As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents chkTestIridiumNetworkConnect As System.Windows.Forms.CheckBox
    Friend WithEvents chkTestIridiumTx As System.Windows.Forms.CheckBox
    Friend WithEvents chkRead9602IMEI As System.Windows.Forms.CheckBox
    Friend WithEvents chkConfig9602 As System.Windows.Forms.CheckBox
    Friend WithEvents chkTest433MHzInterLogger As System.Windows.Forms.CheckBox
    Friend WithEvents chk24GHzPickup As System.Windows.Forms.CheckBox
    Friend WithEvents chkSetSettings As System.Windows.Forms.CheckBox
    Friend WithEvents chkProgramLogger As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents cbProgDevice As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cbProgTool As System.Windows.Forms.ComboBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents txtProgramFile As System.Windows.Forms.TextBox
    Friend WithEvents ofdProgFile As System.Windows.Forms.OpenFileDialog
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents txtLogDir As System.Windows.Forms.TextBox
    Friend WithEvents fbdLogDir As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents chkEraseFlash As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents txtTagId As System.Windows.Forms.TextBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents txtTagWorstRSSI As System.Windows.Forms.TextBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
    Friend WithEvents cbIncLgrName As System.Windows.Forms.ComboBox
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents cbIncLgrGrp As System.Windows.Forms.ComboBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents txtExpectedVersionString As System.Windows.Forms.TextBox
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents txtBaseLoggerName As System.Windows.Forms.TextBox
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents nudLoggerNameStart As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblInfo As System.Windows.Forms.Label
    Friend WithEvents chkMeasureCurrent As System.Windows.Forms.CheckBox
End Class
