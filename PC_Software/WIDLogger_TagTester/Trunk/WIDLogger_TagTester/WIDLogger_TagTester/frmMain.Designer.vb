<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Dim ChartArea3 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Legend3 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend()
        Dim Series3 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Me.btnTestRun = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnTestSave = New System.Windows.Forms.Button()
        Me.btnTestLoad = New System.Windows.Forms.Button()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.tabPageDevelopment = New System.Windows.Forms.TabPage()
        Me.TabPageSetupTest = New System.Windows.Forms.TabPage()
        Me.gbWIDs = New System.Windows.Forms.GroupBox()
        Me.btnEditWID = New System.Windows.Forms.Button()
        Me.btnRemoveWID = New System.Windows.Forms.Button()
        Me.btnAddWID = New System.Windows.Forms.Button()
        Me.lvWIDS = New System.Windows.Forms.ListView()
        Me.ColumnHeader5 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader6 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader7 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.gbTestParameters = New System.Windows.Forms.GroupBox()
        Me.mtbRFFilter = New System.Windows.Forms.MaskedTextBox()
        Me.mtbRFID = New System.Windows.Forms.MaskedTextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.nudRFChan = New System.Windows.Forms.NumericUpDown()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.nudTXPer = New System.Windows.Forms.NumericUpDown()
        Me.gbLoggers = New System.Windows.Forms.GroupBox()
        Me.btnAddLoggerManually = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.lvDevicesInTest = New System.Windows.Forms.ListView()
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Label2 = New System.Windows.Forms.Label()
        Me.lvAvailableDevices = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.btnAddDeviceToTest = New System.Windows.Forms.Button()
        Me.btnRemoveDevFromTest = New System.Windows.Forms.Button()
        Me.btnScanDevices = New System.Windows.Forms.Button()
        Me.tpRunTest = New System.Windows.Forms.TabPage()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.lvOVStats = New System.Windows.Forms.ListView()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.lblOVDataRate = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.lblOVReservedMemory = New System.Windows.Forms.Label()
        Me.lblOVTestStatus = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.btnRunTest = New System.Windows.Forms.Button()
        Me.btnStopTest = New System.Windows.Forms.Button()
        Me.lblOVChan = New System.Windows.Forms.Label()
        Me.lblOVStartTime = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.lblOVRFID = New System.Windows.Forms.Label()
        Me.lblOVRunningTime = New System.Windows.Forms.Label()
        Me.lblOVRFFilter = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.lblOVTestData = New System.Windows.Forms.Label()
        Me.lblOVTxPer = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.chrtOvf = New System.Windows.Forms.DataVisualization.Charting.Chart()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.lblAvgPoints = New System.Windows.Forms.Label()
        Me.lbltbAvgPoints = New System.Windows.Forms.Label()
        Me.tbAvgPoints = New System.Windows.Forms.TrackBar()
        Me.chkLockGraphTimeDif = New System.Windows.Forms.CheckBox()
        Me.lblEndTime = New System.Windows.Forms.Label()
        Me.lbltbEndTime = New System.Windows.Forms.Label()
        Me.tbEndTime = New System.Windows.Forms.TrackBar()
        Me.lblGraphPoints = New System.Windows.Forms.Label()
        Me.lblStartTime = New System.Windows.Forms.Label()
        Me.lbltbStartTime = New System.Windows.Forms.Label()
        Me.tbStartTime = New System.Windows.Forms.TrackBar()
        Me.lbltbPoints = New System.Windows.Forms.Label()
        Me.tbPoints = New System.Windows.Forms.TrackBar()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.ttMain = New System.Windows.Forms.ToolTip(Me.components)
        Me.NotifyIcon = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.mnuMain = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToTrayToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.tmrUpdateOVData = New System.Windows.Forms.Timer(Me.components)
        Me.StatusStrip = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.tmrUpdateGraph = New System.Windows.Forms.Timer(Me.components)
        Me.SFD = New System.Windows.Forms.SaveFileDialog()
        Me.OFD = New System.Windows.Forms.OpenFileDialog()
        Me.cmsChartOptions = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.cbChartType = New System.Windows.Forms.ToolStripComboBox()
        Me.RefreshToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.TrackBar3 = New System.Windows.Forms.TrackBar()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.TrackBar4 = New System.Windows.Forms.TrackBar()
        Me.TabControl1.SuspendLayout()
        Me.tabPageDevelopment.SuspendLayout()
        Me.TabPageSetupTest.SuspendLayout()
        Me.gbWIDs.SuspendLayout()
        Me.gbTestParameters.SuspendLayout()
        CType(Me.nudRFChan, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudTXPer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gbLoggers.SuspendLayout()
        Me.tpRunTest.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        CType(Me.chrtOvf, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        CType(Me.tbAvgPoints, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbEndTime, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbStartTime, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbPoints, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.mnuMain.SuspendLayout()
        Me.StatusStrip.SuspendLayout()
        Me.cmsChartOptions.SuspendLayout()
        Me.Panel3.SuspendLayout()
        CType(Me.TrackBar3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TrackBar4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnTestRun
        '
        Me.btnTestRun.Location = New System.Drawing.Point(6, 328)
        Me.btnTestRun.Name = "btnTestRun"
        Me.btnTestRun.Size = New System.Drawing.Size(75, 23)
        Me.btnTestRun.TabIndex = 2
        Me.btnTestRun.Text = "TEST RUN"
        Me.btnTestRun.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(141, 338)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(39, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Label1"
        '
        'btnTestSave
        '
        Me.btnTestSave.Location = New System.Drawing.Point(6, 357)
        Me.btnTestSave.Name = "btnTestSave"
        Me.btnTestSave.Size = New System.Drawing.Size(75, 23)
        Me.btnTestSave.TabIndex = 4
        Me.btnTestSave.Text = "TEST SAVE"
        Me.btnTestSave.UseVisualStyleBackColor = True
        '
        'btnTestLoad
        '
        Me.btnTestLoad.Location = New System.Drawing.Point(6, 386)
        Me.btnTestLoad.Name = "btnTestLoad"
        Me.btnTestLoad.Size = New System.Drawing.Size(75, 23)
        Me.btnTestLoad.TabIndex = 5
        Me.btnTestLoad.Text = "TEST LOAD"
        Me.btnTestLoad.UseVisualStyleBackColor = True
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.tabPageDevelopment)
        Me.TabControl1.Controls.Add(Me.TabPageSetupTest)
        Me.TabControl1.Controls.Add(Me.tpRunTest)
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Location = New System.Drawing.Point(0, 24)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(900, 538)
        Me.TabControl1.TabIndex = 6
        '
        'tabPageDevelopment
        '
        Me.tabPageDevelopment.Controls.Add(Me.btnTestLoad)
        Me.tabPageDevelopment.Controls.Add(Me.btnTestSave)
        Me.tabPageDevelopment.Controls.Add(Me.btnTestRun)
        Me.tabPageDevelopment.Controls.Add(Me.Label1)
        Me.tabPageDevelopment.Location = New System.Drawing.Point(4, 22)
        Me.tabPageDevelopment.Name = "tabPageDevelopment"
        Me.tabPageDevelopment.Padding = New System.Windows.Forms.Padding(3)
        Me.tabPageDevelopment.Size = New System.Drawing.Size(892, 512)
        Me.tabPageDevelopment.TabIndex = 0
        Me.tabPageDevelopment.Text = "DEV"
        Me.tabPageDevelopment.UseVisualStyleBackColor = True
        '
        'TabPageSetupTest
        '
        Me.TabPageSetupTest.Controls.Add(Me.gbWIDs)
        Me.TabPageSetupTest.Controls.Add(Me.gbTestParameters)
        Me.TabPageSetupTest.Controls.Add(Me.gbLoggers)
        Me.TabPageSetupTest.Location = New System.Drawing.Point(4, 22)
        Me.TabPageSetupTest.Name = "TabPageSetupTest"
        Me.TabPageSetupTest.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageSetupTest.Size = New System.Drawing.Size(892, 512)
        Me.TabPageSetupTest.TabIndex = 1
        Me.TabPageSetupTest.Text = "Setup Test"
        Me.TabPageSetupTest.UseVisualStyleBackColor = True
        '
        'gbWIDs
        '
        Me.gbWIDs.Controls.Add(Me.btnEditWID)
        Me.gbWIDs.Controls.Add(Me.btnRemoveWID)
        Me.gbWIDs.Controls.Add(Me.btnAddWID)
        Me.gbWIDs.Controls.Add(Me.lvWIDS)
        Me.gbWIDs.Location = New System.Drawing.Point(6, 213)
        Me.gbWIDs.Name = "gbWIDs"
        Me.gbWIDs.Size = New System.Drawing.Size(875, 202)
        Me.gbWIDs.TabIndex = 6
        Me.gbWIDs.TabStop = False
        Me.gbWIDs.Text = "WIDs"
        '
        'btnEditWID
        '
        Me.btnEditWID.BackgroundImage = Global.WIDLogger_TagTester.My.Resources.Resources.draw_line_2
        Me.btnEditWID.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnEditWID.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnEditWID.Location = New System.Drawing.Point(88, 161)
        Me.btnEditWID.Name = "btnEditWID"
        Me.btnEditWID.Size = New System.Drawing.Size(35, 34)
        Me.btnEditWID.TabIndex = 7
        Me.ttMain.SetToolTip(Me.btnEditWID, "Scan for devices attached to computer.")
        Me.btnEditWID.UseVisualStyleBackColor = True
        '
        'btnRemoveWID
        '
        Me.btnRemoveWID.BackgroundImage = CType(resources.GetObject("btnRemoveWID.BackgroundImage"), System.Drawing.Image)
        Me.btnRemoveWID.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnRemoveWID.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnRemoveWID.Location = New System.Drawing.Point(47, 161)
        Me.btnRemoveWID.Name = "btnRemoveWID"
        Me.btnRemoveWID.Size = New System.Drawing.Size(35, 34)
        Me.btnRemoveWID.TabIndex = 6
        Me.ttMain.SetToolTip(Me.btnRemoveWID, "Scan for devices attached to computer.")
        Me.btnRemoveWID.UseVisualStyleBackColor = True
        '
        'btnAddWID
        '
        Me.btnAddWID.BackgroundImage = CType(resources.GetObject("btnAddWID.BackgroundImage"), System.Drawing.Image)
        Me.btnAddWID.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnAddWID.Location = New System.Drawing.Point(6, 161)
        Me.btnAddWID.Name = "btnAddWID"
        Me.btnAddWID.Size = New System.Drawing.Size(35, 34)
        Me.btnAddWID.TabIndex = 5
        Me.ttMain.SetToolTip(Me.btnAddWID, "Scan for devices attached to computer.")
        Me.btnAddWID.UseVisualStyleBackColor = True
        '
        'lvWIDS
        '
        Me.lvWIDS.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader5, Me.ColumnHeader6, Me.ColumnHeader7})
        Me.lvWIDS.FullRowSelect = True
        Me.lvWIDS.HideSelection = False
        Me.lvWIDS.Location = New System.Drawing.Point(6, 19)
        Me.lvWIDS.Name = "lvWIDS"
        Me.lvWIDS.Size = New System.Drawing.Size(863, 136)
        Me.lvWIDS.TabIndex = 3
        Me.lvWIDS.UseCompatibleStateImageBehavior = False
        Me.lvWIDS.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader5
        '
        Me.ColumnHeader5.Text = "WID ID"
        Me.ColumnHeader5.Width = 95
        '
        'ColumnHeader6
        '
        Me.ColumnHeader6.Text = "Battery"
        Me.ColumnHeader6.Width = 146
        '
        'ColumnHeader7
        '
        Me.ColumnHeader7.Text = "Info"
        Me.ColumnHeader7.Width = 617
        '
        'gbTestParameters
        '
        Me.gbTestParameters.Controls.Add(Me.mtbRFFilter)
        Me.gbTestParameters.Controls.Add(Me.mtbRFID)
        Me.gbTestParameters.Controls.Add(Me.Label7)
        Me.gbTestParameters.Controls.Add(Me.Label6)
        Me.gbTestParameters.Controls.Add(Me.Label8)
        Me.gbTestParameters.Controls.Add(Me.Label5)
        Me.gbTestParameters.Controls.Add(Me.nudRFChan)
        Me.gbTestParameters.Controls.Add(Me.Label4)
        Me.gbTestParameters.Controls.Add(Me.nudTXPer)
        Me.gbTestParameters.Location = New System.Drawing.Point(562, 6)
        Me.gbTestParameters.Name = "gbTestParameters"
        Me.gbTestParameters.Size = New System.Drawing.Size(190, 126)
        Me.gbTestParameters.TabIndex = 5
        Me.gbTestParameters.TabStop = False
        Me.gbTestParameters.Text = "Test Parameters:"
        '
        'mtbRFFilter
        '
        Me.mtbRFFilter.Location = New System.Drawing.Point(78, 71)
        Me.mtbRFFilter.Mask = "CCCCCCCC"
        Me.mtbRFFilter.Name = "mtbRFFilter"
        Me.mtbRFFilter.Size = New System.Drawing.Size(100, 20)
        Me.mtbRFFilter.TabIndex = 14
        Me.mtbRFFilter.Text = "DEAD****"
        '
        'mtbRFID
        '
        Me.mtbRFID.Location = New System.Drawing.Point(78, 45)
        Me.mtbRFID.Mask = "AA-AA-AA-AA-AA"
        Me.mtbRFID.Name = "mtbRFID"
        Me.mtbRFID.Size = New System.Drawing.Size(100, 20)
        Me.mtbRFID.TabIndex = 13
        Me.mtbRFID.Text = "E6E6E6E6E6"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(12, 99)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(57, 13)
        Me.Label7.TabIndex = 9
        Me.Label7.Text = "TX Period:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(20, 74)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(49, 13)
        Me.Label6.TabIndex = 6
        Me.Label6.Text = "RF Filter:"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(131, 99)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(47, 13)
        Me.Label8.TabIndex = 11
        Me.Label8.Text = "seconds"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(31, 48)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(38, 13)
        Me.Label5.TabIndex = 8
        Me.Label5.Text = "RF ID:"
        '
        'nudRFChan
        '
        Me.nudRFChan.Location = New System.Drawing.Point(78, 19)
        Me.nudRFChan.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
        Me.nudRFChan.Name = "nudRFChan"
        Me.nudRFChan.Size = New System.Drawing.Size(49, 20)
        Me.nudRFChan.TabIndex = 6
        Me.nudRFChan.Value = New Decimal(New Integer() {3, 0, 0, 0})
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(3, 21)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(66, 13)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "RF Channel:"
        '
        'nudTXPer
        '
        Me.nudTXPer.Location = New System.Drawing.Point(78, 97)
        Me.nudTXPer.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
        Me.nudTXPer.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudTXPer.Name = "nudTXPer"
        Me.nudTXPer.Size = New System.Drawing.Size(49, 20)
        Me.nudTXPer.TabIndex = 10
        Me.nudTXPer.Value = New Decimal(New Integer() {10, 0, 0, 0})
        '
        'gbLoggers
        '
        Me.gbLoggers.Controls.Add(Me.btnAddLoggerManually)
        Me.gbLoggers.Controls.Add(Me.Label3)
        Me.gbLoggers.Controls.Add(Me.lvDevicesInTest)
        Me.gbLoggers.Controls.Add(Me.Label2)
        Me.gbLoggers.Controls.Add(Me.lvAvailableDevices)
        Me.gbLoggers.Controls.Add(Me.btnAddDeviceToTest)
        Me.gbLoggers.Controls.Add(Me.btnRemoveDevFromTest)
        Me.gbLoggers.Controls.Add(Me.btnScanDevices)
        Me.gbLoggers.Location = New System.Drawing.Point(6, 6)
        Me.gbLoggers.Name = "gbLoggers"
        Me.gbLoggers.Size = New System.Drawing.Size(550, 201)
        Me.gbLoggers.TabIndex = 4
        Me.gbLoggers.TabStop = False
        Me.gbLoggers.Text = "Loggers:"
        '
        'btnAddLoggerManually
        '
        Me.btnAddLoggerManually.BackgroundImage = CType(resources.GetObject("btnAddLoggerManually.BackgroundImage"), System.Drawing.Image)
        Me.btnAddLoggerManually.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnAddLoggerManually.Location = New System.Drawing.Point(257, 77)
        Me.btnAddLoggerManually.Name = "btnAddLoggerManually"
        Me.btnAddLoggerManually.Size = New System.Drawing.Size(35, 34)
        Me.btnAddLoggerManually.TabIndex = 8
        Me.ttMain.SetToolTip(Me.btnAddLoggerManually, "Add a logger manually.")
        Me.btnAddLoggerManually.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(298, 16)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(84, 13)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "Loggers In Test:"
        '
        'lvDevicesInTest
        '
        Me.lvDevicesInTest.Activation = System.Windows.Forms.ItemActivation.OneClick
        Me.lvDevicesInTest.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader3, Me.ColumnHeader4})
        Me.lvDevicesInTest.FullRowSelect = True
        Me.lvDevicesInTest.HideSelection = False
        Me.lvDevicesInTest.Location = New System.Drawing.Point(298, 32)
        Me.lvDevicesInTest.Name = "lvDevicesInTest"
        Me.lvDevicesInTest.Size = New System.Drawing.Size(245, 163)
        Me.lvDevicesInTest.TabIndex = 4
        Me.lvDevicesInTest.UseCompatibleStateImageBehavior = False
        Me.lvDevicesInTest.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "Port"
        Me.ColumnHeader3.Width = 71
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Text = "Name"
        Me.ColumnHeader4.Width = 170
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(6, 16)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(94, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Available Loggers:"
        '
        'lvAvailableDevices
        '
        Me.lvAvailableDevices.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2})
        Me.lvAvailableDevices.FullRowSelect = True
        Me.lvAvailableDevices.HideSelection = False
        Me.lvAvailableDevices.Location = New System.Drawing.Point(6, 32)
        Me.lvAvailableDevices.Name = "lvAvailableDevices"
        Me.lvAvailableDevices.Size = New System.Drawing.Size(245, 163)
        Me.lvAvailableDevices.TabIndex = 2
        Me.lvAvailableDevices.UseCompatibleStateImageBehavior = False
        Me.lvAvailableDevices.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Port"
        Me.ColumnHeader1.Width = 71
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Name"
        Me.ColumnHeader2.Width = 170
        '
        'btnAddDeviceToTest
        '
        Me.btnAddDeviceToTest.BackgroundImage = CType(resources.GetObject("btnAddDeviceToTest.BackgroundImage"), System.Drawing.Image)
        Me.btnAddDeviceToTest.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnAddDeviceToTest.Location = New System.Drawing.Point(257, 40)
        Me.btnAddDeviceToTest.Name = "btnAddDeviceToTest"
        Me.btnAddDeviceToTest.Size = New System.Drawing.Size(35, 31)
        Me.btnAddDeviceToTest.TabIndex = 6
        Me.ttMain.SetToolTip(Me.btnAddDeviceToTest, "Add selected logger to test.")
        Me.btnAddDeviceToTest.UseVisualStyleBackColor = True
        '
        'btnRemoveDevFromTest
        '
        Me.btnRemoveDevFromTest.BackgroundImage = CType(resources.GetObject("btnRemoveDevFromTest.BackgroundImage"), System.Drawing.Image)
        Me.btnRemoveDevFromTest.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.btnRemoveDevFromTest.Location = New System.Drawing.Point(257, 117)
        Me.btnRemoveDevFromTest.Name = "btnRemoveDevFromTest"
        Me.btnRemoveDevFromTest.Size = New System.Drawing.Size(35, 31)
        Me.btnRemoveDevFromTest.TabIndex = 7
        Me.ttMain.SetToolTip(Me.btnRemoveDevFromTest, "remove selected logger from test.")
        Me.btnRemoveDevFromTest.UseVisualStyleBackColor = True
        '
        'btnScanDevices
        '
        Me.btnScanDevices.BackgroundImage = CType(resources.GetObject("btnScanDevices.BackgroundImage"), System.Drawing.Image)
        Me.btnScanDevices.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnScanDevices.Font = New System.Drawing.Font("Consolas", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnScanDevices.Location = New System.Drawing.Point(257, 154)
        Me.btnScanDevices.Name = "btnScanDevices"
        Me.btnScanDevices.Size = New System.Drawing.Size(35, 34)
        Me.btnScanDevices.TabIndex = 3
        Me.ttMain.SetToolTip(Me.btnScanDevices, "Scan for devices attached to computer.")
        Me.btnScanDevices.UseVisualStyleBackColor = True
        '
        'tpRunTest
        '
        Me.tpRunTest.Controls.Add(Me.TableLayoutPanel1)
        Me.tpRunTest.Location = New System.Drawing.Point(4, 22)
        Me.tpRunTest.Name = "tpRunTest"
        Me.tpRunTest.Padding = New System.Windows.Forms.Padding(3)
        Me.tpRunTest.Size = New System.Drawing.Size(892, 512)
        Me.tpRunTest.TabIndex = 2
        Me.tpRunTest.Text = "Run Test"
        Me.tpRunTest.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.GroupBox1, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.chrtOvf, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Panel1, 0, 2)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(3, 3)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 3
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 132.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 185.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(886, 506)
        Me.TableLayoutPanel1.TabIndex = 4
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.lvOVStats)
        Me.GroupBox1.Controls.Add(Me.Label18)
        Me.GroupBox1.Controls.Add(Me.lblOVDataRate)
        Me.GroupBox1.Controls.Add(Me.Label17)
        Me.GroupBox1.Controls.Add(Me.lblOVReservedMemory)
        Me.GroupBox1.Controls.Add(Me.lblOVTestStatus)
        Me.GroupBox1.Controls.Add(Me.Label16)
        Me.GroupBox1.Controls.Add(Me.btnRunTest)
        Me.GroupBox1.Controls.Add(Me.btnStopTest)
        Me.GroupBox1.Controls.Add(Me.lblOVChan)
        Me.GroupBox1.Controls.Add(Me.lblOVStartTime)
        Me.GroupBox1.Controls.Add(Me.Label12)
        Me.GroupBox1.Controls.Add(Me.Label11)
        Me.GroupBox1.Controls.Add(Me.lblOVRFID)
        Me.GroupBox1.Controls.Add(Me.lblOVRunningTime)
        Me.GroupBox1.Controls.Add(Me.lblOVRFFilter)
        Me.GroupBox1.Controls.Add(Me.Label9)
        Me.GroupBox1.Controls.Add(Me.lblOVTestData)
        Me.GroupBox1.Controls.Add(Me.lblOVTxPer)
        Me.GroupBox1.Controls.Add(Me.Label10)
        Me.GroupBox1.Controls.Add(Me.Label15)
        Me.GroupBox1.Controls.Add(Me.Label13)
        Me.GroupBox1.Controls.Add(Me.Label14)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Location = New System.Drawing.Point(3, 3)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(880, 126)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Overview:"
        '
        'lvOVStats
        '
        Me.lvOVStats.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lvOVStats.FullRowSelect = True
        Me.lvOVStats.GridLines = True
        Me.lvOVStats.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lvOVStats.HideSelection = False
        Me.lvOVStats.Location = New System.Drawing.Point(425, 19)
        Me.lvOVStats.MultiSelect = False
        Me.lvOVStats.Name = "lvOVStats"
        Me.lvOVStats.Scrollable = False
        Me.lvOVStats.Size = New System.Drawing.Size(442, 97)
        Me.lvOVStats.TabIndex = 21
        Me.lvOVStats.UseCompatibleStateImageBehavior = False
        Me.lvOVStats.View = System.Windows.Forms.View.Details
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(44, 90)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(59, 13)
        Me.Label18.TabIndex = 19
        Me.Label18.Text = "Data Rate:"
        '
        'lblOVDataRate
        '
        Me.lblOVDataRate.AutoSize = True
        Me.lblOVDataRate.Location = New System.Drawing.Point(110, 90)
        Me.lblOVDataRate.Name = "lblOVDataRate"
        Me.lblOVDataRate.Size = New System.Drawing.Size(25, 13)
        Me.lblOVDataRate.TabIndex = 20
        Me.lblOVDataRate.Text = "???"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(7, 77)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(96, 13)
        Me.Label17.TabIndex = 17
        Me.Label17.Text = "Reserved Memory:"
        '
        'lblOVReservedMemory
        '
        Me.lblOVReservedMemory.AutoSize = True
        Me.lblOVReservedMemory.Location = New System.Drawing.Point(109, 77)
        Me.lblOVReservedMemory.Name = "lblOVReservedMemory"
        Me.lblOVReservedMemory.Size = New System.Drawing.Size(25, 13)
        Me.lblOVReservedMemory.TabIndex = 18
        Me.lblOVReservedMemory.Text = "???"
        '
        'lblOVTestStatus
        '
        Me.lblOVTestStatus.AutoSize = True
        Me.lblOVTestStatus.ForeColor = System.Drawing.Color.Red
        Me.lblOVTestStatus.Location = New System.Drawing.Point(109, 25)
        Me.lblOVTestStatus.Name = "lblOVTestStatus"
        Me.lblOVTestStatus.Size = New System.Drawing.Size(84, 13)
        Me.lblOVTestStatus.TabIndex = 16
        Me.lblOVTestStatus.Text = "NOT STARTED"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(39, 25)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(64, 13)
        Me.Label16.TabIndex = 15
        Me.Label16.Text = "Test Status:"
        '
        'btnRunTest
        '
        Me.btnRunTest.BackgroundImage = Global.WIDLogger_TagTester.My.Resources.Resources.run
        Me.btnRunTest.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnRunTest.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnRunTest.Location = New System.Drawing.Point(269, 82)
        Me.btnRunTest.Name = "btnRunTest"
        Me.btnRunTest.Size = New System.Drawing.Size(35, 34)
        Me.btnRunTest.TabIndex = 14
        Me.ttMain.SetToolTip(Me.btnRunTest, "Start Test")
        Me.btnRunTest.UseVisualStyleBackColor = True
        '
        'btnStopTest
        '
        Me.btnStopTest.BackgroundImage = Global.WIDLogger_TagTester.My.Resources.Resources.media_playback_stop_7
        Me.btnStopTest.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.btnStopTest.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.btnStopTest.Location = New System.Drawing.Point(310, 82)
        Me.btnStopTest.Name = "btnStopTest"
        Me.btnStopTest.Size = New System.Drawing.Size(35, 34)
        Me.btnStopTest.TabIndex = 8
        Me.ttMain.SetToolTip(Me.btnStopTest, "Stop Test")
        Me.btnStopTest.UseVisualStyleBackColor = True
        '
        'lblOVChan
        '
        Me.lblOVChan.AutoSize = True
        Me.lblOVChan.Location = New System.Drawing.Point(321, 25)
        Me.lblOVChan.Name = "lblOVChan"
        Me.lblOVChan.Size = New System.Drawing.Size(25, 13)
        Me.lblOVChan.TabIndex = 10
        Me.lblOVChan.Text = "???"
        '
        'lblOVStartTime
        '
        Me.lblOVStartTime.AutoSize = True
        Me.lblOVStartTime.Location = New System.Drawing.Point(109, 38)
        Me.lblOVStartTime.Name = "lblOVStartTime"
        Me.lblOVStartTime.Size = New System.Drawing.Size(25, 13)
        Me.lblOVStartTime.TabIndex = 13
        Me.lblOVStartTime.Text = "???"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(266, 25)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(49, 13)
        Me.Label12.TabIndex = 3
        Me.Label12.Text = "Channel:"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(46, 64)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(57, 13)
        Me.Label11.TabIndex = 2
        Me.Label11.Text = "Test Data:"
        '
        'lblOVRFID
        '
        Me.lblOVRFID.AutoSize = True
        Me.lblOVRFID.Location = New System.Drawing.Point(321, 38)
        Me.lblOVRFID.Name = "lblOVRFID"
        Me.lblOVRFID.Size = New System.Drawing.Size(25, 13)
        Me.lblOVRFID.TabIndex = 9
        Me.lblOVRFID.Text = "???"
        '
        'lblOVRunningTime
        '
        Me.lblOVRunningTime.AutoSize = True
        Me.lblOVRunningTime.Location = New System.Drawing.Point(109, 51)
        Me.lblOVRunningTime.Name = "lblOVRunningTime"
        Me.lblOVRunningTime.Size = New System.Drawing.Size(25, 13)
        Me.lblOVRunningTime.TabIndex = 12
        Me.lblOVRunningTime.Text = "???"
        '
        'lblOVRFFilter
        '
        Me.lblOVRFFilter.AutoSize = True
        Me.lblOVRFFilter.Location = New System.Drawing.Point(321, 51)
        Me.lblOVRFFilter.Name = "lblOVRFFilter"
        Me.lblOVRFFilter.Size = New System.Drawing.Size(25, 13)
        Me.lblOVRFFilter.TabIndex = 8
        Me.lblOVRFFilter.Text = "???"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(28, 51)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(76, 13)
        Me.Label9.TabIndex = 0
        Me.Label9.Text = "Time Running:"
        '
        'lblOVTestData
        '
        Me.lblOVTestData.AutoSize = True
        Me.lblOVTestData.Location = New System.Drawing.Point(109, 64)
        Me.lblOVTestData.Name = "lblOVTestData"
        Me.lblOVTestData.Size = New System.Drawing.Size(25, 13)
        Me.lblOVTestData.TabIndex = 11
        Me.lblOVTestData.Text = "???"
        '
        'lblOVTxPer
        '
        Me.lblOVTxPer.AutoSize = True
        Me.lblOVTxPer.Location = New System.Drawing.Point(321, 64)
        Me.lblOVTxPer.Name = "lblOVTxPer"
        Me.lblOVTxPer.Size = New System.Drawing.Size(25, 13)
        Me.lblOVTxPer.TabIndex = 7
        Me.lblOVTxPer.Text = "???"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(46, 38)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(58, 13)
        Me.Label10.TabIndex = 1
        Me.Label10.Text = "Start Time:"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(260, 64)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(55, 13)
        Me.Label15.TabIndex = 6
        Me.Label15.Text = "Tx Period:"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(280, 38)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(35, 13)
        Me.Label13.TabIndex = 4
        Me.Label13.Text = "RFID:"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(266, 51)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(49, 13)
        Me.Label14.TabIndex = 5
        Me.Label14.Text = "RF Filter:"
        '
        'chrtOvf
        '
        Me.chrtOvf.BackColor = System.Drawing.Color.Black
        Me.chrtOvf.BackSecondaryColor = System.Drawing.Color.Black
        Me.chrtOvf.BorderlineColor = System.Drawing.Color.Black
        Me.chrtOvf.BorderSkin.BackColor = System.Drawing.Color.WhiteSmoke
        Me.chrtOvf.BorderSkin.BorderColor = System.Drawing.Color.White
        Me.chrtOvf.BorderSkin.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid
        Me.chrtOvf.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss
        ChartArea3.AxisX.LineColor = System.Drawing.Color.White
        ChartArea3.AxisX.TitleForeColor = System.Drawing.Color.White
        ChartArea3.AxisX2.LineColor = System.Drawing.Color.White
        ChartArea3.AxisX2.TitleForeColor = System.Drawing.Color.White
        ChartArea3.AxisY.LineColor = System.Drawing.Color.White
        ChartArea3.AxisY.TitleForeColor = System.Drawing.Color.White
        ChartArea3.AxisY2.LineColor = System.Drawing.Color.White
        ChartArea3.AxisY2.TitleForeColor = System.Drawing.Color.White
        ChartArea3.BackColor = System.Drawing.Color.Black
        ChartArea3.BorderColor = System.Drawing.Color.White
        ChartArea3.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid
        ChartArea3.Name = "ChartArea1"
        Me.chrtOvf.ChartAreas.Add(ChartArea3)
        Me.chrtOvf.Dock = System.Windows.Forms.DockStyle.Fill
        Legend3.Name = "Legend1"
        Me.chrtOvf.Legends.Add(Legend3)
        Me.chrtOvf.Location = New System.Drawing.Point(3, 135)
        Me.chrtOvf.Name = "chrtOvf"
        Me.chrtOvf.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Series3.ChartArea = "ChartArea1"
        Series3.LabelForeColor = System.Drawing.Color.White
        Series3.Legend = "Legend1"
        Series3.Name = "Series1"
        Me.chrtOvf.Series.Add(Series3)
        Me.chrtOvf.Size = New System.Drawing.Size(880, 183)
        Me.chrtOvf.TabIndex = 1
        Me.chrtOvf.Text = "Chart1"
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.Panel3)
        Me.Panel1.Controls.Add(Me.Panel2)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(3, 324)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(880, 179)
        Me.Panel1.TabIndex = 2
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.lblAvgPoints)
        Me.Panel2.Controls.Add(Me.lbltbAvgPoints)
        Me.Panel2.Controls.Add(Me.tbAvgPoints)
        Me.Panel2.Controls.Add(Me.chkLockGraphTimeDif)
        Me.Panel2.Controls.Add(Me.lblEndTime)
        Me.Panel2.Controls.Add(Me.lbltbEndTime)
        Me.Panel2.Controls.Add(Me.tbEndTime)
        Me.Panel2.Controls.Add(Me.lblGraphPoints)
        Me.Panel2.Controls.Add(Me.lblStartTime)
        Me.Panel2.Controls.Add(Me.lbltbStartTime)
        Me.Panel2.Controls.Add(Me.tbStartTime)
        Me.Panel2.Controls.Add(Me.lbltbPoints)
        Me.Panel2.Controls.Add(Me.tbPoints)
        Me.Panel2.Location = New System.Drawing.Point(3, 3)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(387, 131)
        Me.Panel2.TabIndex = 4
        '
        'lblAvgPoints
        '
        Me.lblAvgPoints.AutoSize = True
        Me.lblAvgPoints.Location = New System.Drawing.Point(330, 77)
        Me.lblAvgPoints.Name = "lblAvgPoints"
        Me.lblAvgPoints.Size = New System.Drawing.Size(25, 13)
        Me.lblAvgPoints.TabIndex = 14
        Me.lblAvgPoints.Text = "100"
        '
        'lbltbAvgPoints
        '
        Me.lbltbAvgPoints.AutoSize = True
        Me.lbltbAvgPoints.Location = New System.Drawing.Point(17, 77)
        Me.lbltbAvgPoints.Name = "lbltbAvgPoints"
        Me.lbltbAvgPoints.Size = New System.Drawing.Size(61, 13)
        Me.lbltbAvgPoints.TabIndex = 13
        Me.lbltbAvgPoints.Text = "Avg Ponits:"
        '
        'tbAvgPoints
        '
        Me.tbAvgPoints.AutoSize = False
        Me.tbAvgPoints.BackColor = System.Drawing.SystemColors.Window
        Me.tbAvgPoints.LargeChange = 10
        Me.tbAvgPoints.Location = New System.Drawing.Point(74, 73)
        Me.tbAvgPoints.Maximum = 500
        Me.tbAvgPoints.Minimum = 1
        Me.tbAvgPoints.Name = "tbAvgPoints"
        Me.tbAvgPoints.Size = New System.Drawing.Size(250, 24)
        Me.tbAvgPoints.TabIndex = 12
        Me.tbAvgPoints.TickFrequency = 500
        Me.tbAvgPoints.TickStyle = System.Windows.Forms.TickStyle.None
        Me.tbAvgPoints.Value = 50
        '
        'chkLockGraphTimeDif
        '
        Me.chkLockGraphTimeDif.AutoSize = True
        Me.chkLockGraphTimeDif.Location = New System.Drawing.Point(28, 63)
        Me.chkLockGraphTimeDif.Name = "chkLockGraphTimeDif"
        Me.chkLockGraphTimeDif.Size = New System.Drawing.Size(50, 17)
        Me.chkLockGraphTimeDif.TabIndex = 11
        Me.chkLockGraphTimeDif.Text = "Lock"
        Me.chkLockGraphTimeDif.UseVisualStyleBackColor = True
        '
        'lblEndTime
        '
        Me.lblEndTime.AutoSize = True
        Me.lblEndTime.Location = New System.Drawing.Point(330, 47)
        Me.lblEndTime.Name = "lblEndTime"
        Me.lblEndTime.Size = New System.Drawing.Size(30, 13)
        Me.lblEndTime.TabIndex = 10
        Me.lblEndTime.Text = "END"
        '
        'lbltbEndTime
        '
        Me.lbltbEndTime.AutoSize = True
        Me.lbltbEndTime.Location = New System.Drawing.Point(25, 47)
        Me.lbltbEndTime.Name = "lbltbEndTime"
        Me.lbltbEndTime.Size = New System.Drawing.Size(55, 13)
        Me.lbltbEndTime.TabIndex = 9
        Me.lbltbEndTime.Text = "End Time:"
        '
        'tbEndTime
        '
        Me.tbEndTime.AutoSize = False
        Me.tbEndTime.BackColor = System.Drawing.SystemColors.Window
        Me.tbEndTime.LargeChange = 100
        Me.tbEndTime.Location = New System.Drawing.Point(74, 43)
        Me.tbEndTime.Maximum = 1000
        Me.tbEndTime.Minimum = 1
        Me.tbEndTime.Name = "tbEndTime"
        Me.tbEndTime.Size = New System.Drawing.Size(250, 24)
        Me.tbEndTime.SmallChange = 10
        Me.tbEndTime.TabIndex = 8
        Me.tbEndTime.TickFrequency = 500
        Me.tbEndTime.TickStyle = System.Windows.Forms.TickStyle.None
        Me.tbEndTime.Value = 100
        '
        'lblGraphPoints
        '
        Me.lblGraphPoints.AutoSize = True
        Me.lblGraphPoints.Location = New System.Drawing.Point(330, 7)
        Me.lblGraphPoints.Name = "lblGraphPoints"
        Me.lblGraphPoints.Size = New System.Drawing.Size(25, 13)
        Me.lblGraphPoints.TabIndex = 7
        Me.lblGraphPoints.Text = "100"
        '
        'lblStartTime
        '
        Me.lblStartTime.AutoSize = True
        Me.lblStartTime.Location = New System.Drawing.Point(330, 27)
        Me.lblStartTime.Name = "lblStartTime"
        Me.lblStartTime.Size = New System.Drawing.Size(43, 13)
        Me.lblStartTime.TabIndex = 6
        Me.lblStartTime.Text = "START"
        '
        'lbltbStartTime
        '
        Me.lbltbStartTime.AutoSize = True
        Me.lbltbStartTime.Location = New System.Drawing.Point(22, 27)
        Me.lbltbStartTime.Name = "lbltbStartTime"
        Me.lbltbStartTime.Size = New System.Drawing.Size(58, 13)
        Me.lbltbStartTime.TabIndex = 5
        Me.lbltbStartTime.Text = "Start Time:"
        '
        'tbStartTime
        '
        Me.tbStartTime.AutoSize = False
        Me.tbStartTime.BackColor = System.Drawing.SystemColors.Window
        Me.tbStartTime.LargeChange = 100
        Me.tbStartTime.Location = New System.Drawing.Point(74, 23)
        Me.tbStartTime.Maximum = 1000
        Me.tbStartTime.Name = "tbStartTime"
        Me.tbStartTime.Size = New System.Drawing.Size(250, 24)
        Me.tbStartTime.SmallChange = 10
        Me.tbStartTime.TabIndex = 4
        Me.tbStartTime.TickFrequency = 500
        Me.tbStartTime.TickStyle = System.Windows.Forms.TickStyle.None
        '
        'lbltbPoints
        '
        Me.lbltbPoints.AutoSize = True
        Me.lbltbPoints.Location = New System.Drawing.Point(9, 7)
        Me.lbltbPoints.Name = "lbltbPoints"
        Me.lbltbPoints.Size = New System.Drawing.Size(71, 13)
        Me.lbltbPoints.TabIndex = 3
        Me.lbltbPoints.Text = "Graph Points:"
        '
        'tbPoints
        '
        Me.tbPoints.AutoSize = False
        Me.tbPoints.BackColor = System.Drawing.SystemColors.Window
        Me.tbPoints.LargeChange = 100
        Me.tbPoints.Location = New System.Drawing.Point(74, 3)
        Me.tbPoints.Maximum = 1000
        Me.tbPoints.Name = "tbPoints"
        Me.tbPoints.Size = New System.Drawing.Size(250, 24)
        Me.tbPoints.SmallChange = 10
        Me.tbPoints.TabIndex = 2
        Me.tbPoints.TickFrequency = 500
        Me.tbPoints.TickStyle = System.Windows.Forms.TickStyle.None
        Me.tbPoints.Value = 100
        '
        'TabPage1
        '
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(892, 512)
        Me.TabPage1.TabIndex = 3
        Me.TabPage1.Text = "Visualise Data"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'NotifyIcon
        '
        Me.NotifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info
        Me.NotifyIcon.BalloonTipText = "WID Tag and Logger Analyser"
        Me.NotifyIcon.BalloonTipTitle = "Double Click to open!"
        Me.NotifyIcon.Icon = CType(resources.GetObject("NotifyIcon.Icon"), System.Drawing.Icon)
        Me.NotifyIcon.Text = "Logger Test Running In Background"
        '
        'mnuMain
        '
        Me.mnuMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem})
        Me.mnuMain.Location = New System.Drawing.Point(0, 0)
        Me.mnuMain.Name = "mnuMain"
        Me.mnuMain.Size = New System.Drawing.Size(900, 24)
        Me.mnuMain.TabIndex = 7
        Me.mnuMain.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenToolStripMenuItem, Me.SaveToolStripMenuItem, Me.ToolStripMenuItem1, Me.ToTrayToolStripMenuItem, Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'OpenToolStripMenuItem
        '
        Me.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem"
        Me.OpenToolStripMenuItem.Size = New System.Drawing.Size(114, 22)
        Me.OpenToolStripMenuItem.Text = "Open"
        '
        'SaveToolStripMenuItem
        '
        Me.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem"
        Me.SaveToolStripMenuItem.Size = New System.Drawing.Size(114, 22)
        Me.SaveToolStripMenuItem.Text = "Save"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(111, 6)
        '
        'ToTrayToolStripMenuItem
        '
        Me.ToTrayToolStripMenuItem.Name = "ToTrayToolStripMenuItem"
        Me.ToTrayToolStripMenuItem.Size = New System.Drawing.Size(114, 22)
        Me.ToTrayToolStripMenuItem.Text = "To Tray"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(114, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'tmrUpdateOVData
        '
        Me.tmrUpdateOVData.Interval = 1000
        '
        'StatusStrip
        '
        Me.StatusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1})
        Me.StatusStrip.Location = New System.Drawing.Point(0, 540)
        Me.StatusStrip.Name = "StatusStrip"
        Me.StatusStrip.Size = New System.Drawing.Size(900, 22)
        Me.StatusStrip.TabIndex = 8
        Me.StatusStrip.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(0, 17)
        '
        'tmrUpdateGraph
        '
        '
        'OFD
        '
        Me.OFD.FileName = "OpenFileDialog1"
        '
        'cmsChartOptions
        '
        Me.cmsChartOptions.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.cbChartType, Me.RefreshToolStripMenuItem})
        Me.cmsChartOptions.Name = "cmsChartOptions"
        Me.cmsChartOptions.Size = New System.Drawing.Size(182, 53)
        '
        'cbChartType
        '
        Me.cbChartType.Items.AddRange(New Object() {"Instantaneous", "Moving Average"})
        Me.cbChartType.Name = "cbChartType"
        Me.cbChartType.Size = New System.Drawing.Size(121, 23)
        Me.cbChartType.Text = "Moving Average"
        '
        'RefreshToolStripMenuItem
        '
        Me.RefreshToolStripMenuItem.Name = "RefreshToolStripMenuItem"
        Me.RefreshToolStripMenuItem.Size = New System.Drawing.Size(181, 22)
        Me.RefreshToolStripMenuItem.Text = "Refresh"
        '
        'Panel3
        '
        Me.Panel3.Controls.Add(Me.Label23)
        Me.Panel3.Controls.Add(Me.Label24)
        Me.Panel3.Controls.Add(Me.Label25)
        Me.Panel3.Controls.Add(Me.TrackBar3)
        Me.Panel3.Controls.Add(Me.Label26)
        Me.Panel3.Controls.Add(Me.TrackBar4)
        Me.Panel3.Location = New System.Drawing.Point(396, 3)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(387, 60)
        Me.Panel3.TabIndex = 5
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.Location = New System.Drawing.Point(330, 7)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(25, 13)
        Me.Label23.TabIndex = 7
        Me.Label23.Text = "100"
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Location = New System.Drawing.Point(330, 27)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(43, 13)
        Me.Label24.TabIndex = 6
        Me.Label24.Text = "START"
        '
        'Label25
        '
        Me.Label25.AutoSize = True
        Me.Label25.Location = New System.Drawing.Point(22, 27)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(58, 13)
        Me.Label25.TabIndex = 5
        Me.Label25.Text = "Start Time:"
        '
        'TrackBar3
        '
        Me.TrackBar3.AutoSize = False
        Me.TrackBar3.BackColor = System.Drawing.SystemColors.Window
        Me.TrackBar3.LargeChange = 100
        Me.TrackBar3.Location = New System.Drawing.Point(74, 23)
        Me.TrackBar3.Maximum = 1000
        Me.TrackBar3.Name = "TrackBar3"
        Me.TrackBar3.Size = New System.Drawing.Size(250, 24)
        Me.TrackBar3.SmallChange = 10
        Me.TrackBar3.TabIndex = 4
        Me.TrackBar3.TickFrequency = 500
        Me.TrackBar3.TickStyle = System.Windows.Forms.TickStyle.None
        '
        'Label26
        '
        Me.Label26.AutoSize = True
        Me.Label26.Location = New System.Drawing.Point(9, 7)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(71, 13)
        Me.Label26.TabIndex = 3
        Me.Label26.Text = "Graph Points:"
        '
        'TrackBar4
        '
        Me.TrackBar4.AutoSize = False
        Me.TrackBar4.BackColor = System.Drawing.SystemColors.Window
        Me.TrackBar4.LargeChange = 100
        Me.TrackBar4.Location = New System.Drawing.Point(74, 3)
        Me.TrackBar4.Maximum = 1000
        Me.TrackBar4.Name = "TrackBar4"
        Me.TrackBar4.Size = New System.Drawing.Size(250, 24)
        Me.TrackBar4.SmallChange = 10
        Me.TrackBar4.TabIndex = 2
        Me.TrackBar4.TickFrequency = 500
        Me.TrackBar4.TickStyle = System.Windows.Forms.TickStyle.None
        Me.TrackBar4.Value = 100
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(900, 562)
        Me.Controls.Add(Me.StatusStrip)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.mnuMain)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.mnuMain
        Me.Name = "frmMain"
        Me.Text = "WID Tag and Logger Analyser"
        Me.TabControl1.ResumeLayout(False)
        Me.tabPageDevelopment.ResumeLayout(False)
        Me.tabPageDevelopment.PerformLayout()
        Me.TabPageSetupTest.ResumeLayout(False)
        Me.gbWIDs.ResumeLayout(False)
        Me.gbTestParameters.ResumeLayout(False)
        Me.gbTestParameters.PerformLayout()
        CType(Me.nudRFChan, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudTXPer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gbLoggers.ResumeLayout(False)
        Me.gbLoggers.PerformLayout()
        Me.tpRunTest.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.chrtOvf, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        CType(Me.tbAvgPoints, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbEndTime, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbStartTime, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbPoints, System.ComponentModel.ISupportInitialize).EndInit()
        Me.mnuMain.ResumeLayout(False)
        Me.mnuMain.PerformLayout()
        Me.StatusStrip.ResumeLayout(False)
        Me.StatusStrip.PerformLayout()
        Me.cmsChartOptions.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        CType(Me.TrackBar3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TrackBar4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnTestRun As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnTestSave As System.Windows.Forms.Button
    Friend WithEvents btnTestLoad As System.Windows.Forms.Button
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents tabPageDevelopment As System.Windows.Forms.TabPage
    Friend WithEvents TabPageSetupTest As System.Windows.Forms.TabPage
    Friend WithEvents gbLoggers As System.Windows.Forms.GroupBox
    Friend WithEvents btnAddLoggerManually As System.Windows.Forms.Button
    Friend WithEvents ttMain As System.Windows.Forms.ToolTip
    Friend WithEvents btnScanDevices As System.Windows.Forms.Button
    Friend WithEvents btnRemoveDevFromTest As System.Windows.Forms.Button
    Friend WithEvents btnAddDeviceToTest As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents lvDevicesInTest As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader4 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lvAvailableDevices As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents gbTestParameters As System.Windows.Forms.GroupBox
    Friend WithEvents mtbRFID As System.Windows.Forms.MaskedTextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents nudTXPer As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents nudRFChan As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents mtbRFFilter As System.Windows.Forms.MaskedTextBox
    Friend WithEvents gbWIDs As System.Windows.Forms.GroupBox
    Friend WithEvents lvWIDS As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader5 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader6 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader7 As System.Windows.Forms.ColumnHeader
    Friend WithEvents btnRemoveWID As System.Windows.Forms.Button
    Friend WithEvents btnAddWID As System.Windows.Forms.Button
    Friend WithEvents btnEditWID As System.Windows.Forms.Button
    Friend WithEvents tpRunTest As System.Windows.Forms.TabPage
    Friend WithEvents NotifyIcon As System.Windows.Forms.NotifyIcon
    Friend WithEvents mnuMain As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToTrayToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents lblOVChan As System.Windows.Forms.Label
    Friend WithEvents lblOVStartTime As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents lblOVRFID As System.Windows.Forms.Label
    Friend WithEvents lblOVRunningTime As System.Windows.Forms.Label
    Friend WithEvents lblOVRFFilter As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents lblOVTestData As System.Windows.Forms.Label
    Friend WithEvents lblOVTxPer As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents tmrUpdateOVData As System.Windows.Forms.Timer
    Friend WithEvents btnRunTest As System.Windows.Forms.Button
    Friend WithEvents btnStopTest As System.Windows.Forms.Button
    Friend WithEvents lblOVTestStatus As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents lblOVReservedMemory As System.Windows.Forms.Label
    Friend WithEvents StatusStrip As System.Windows.Forms.StatusStrip
    Friend WithEvents ToolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents lblOVDataRate As System.Windows.Forms.Label
    Friend WithEvents lvOVStats As System.Windows.Forms.ListView
    Friend WithEvents chrtOvf As System.Windows.Forms.DataVisualization.Charting.Chart
    Friend WithEvents tmrUpdateGraph As System.Windows.Forms.Timer
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents SFD As System.Windows.Forms.SaveFileDialog
    Friend WithEvents OFD As System.Windows.Forms.OpenFileDialog
    Friend WithEvents cmsChartOptions As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents cbChartType As System.Windows.Forms.ToolStripComboBox
    Friend WithEvents RefreshToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lbltbPoints As System.Windows.Forms.Label
    Friend WithEvents tbPoints As System.Windows.Forms.TrackBar
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents lblGraphPoints As System.Windows.Forms.Label
    Friend WithEvents lblStartTime As System.Windows.Forms.Label
    Friend WithEvents lbltbStartTime As System.Windows.Forms.Label
    Friend WithEvents tbStartTime As System.Windows.Forms.TrackBar
    Friend WithEvents lblEndTime As System.Windows.Forms.Label
    Friend WithEvents lbltbEndTime As System.Windows.Forms.Label
    Friend WithEvents tbEndTime As System.Windows.Forms.TrackBar
    Friend WithEvents chkLockGraphTimeDif As System.Windows.Forms.CheckBox
    Friend WithEvents lblAvgPoints As System.Windows.Forms.Label
    Friend WithEvents lbltbAvgPoints As System.Windows.Forms.Label
    Friend WithEvents tbAvgPoints As System.Windows.Forms.TrackBar
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents Label24 As System.Windows.Forms.Label
    Friend WithEvents Label25 As System.Windows.Forms.Label
    Friend WithEvents TrackBar3 As System.Windows.Forms.TrackBar
    Friend WithEvents Label26 As System.Windows.Forms.Label
    Friend WithEvents TrackBar4 As System.Windows.Forms.TrackBar

End Class
