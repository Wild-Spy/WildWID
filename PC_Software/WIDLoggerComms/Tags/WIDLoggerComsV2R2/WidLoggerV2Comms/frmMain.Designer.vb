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
        Dim ChartArea2 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim Legend2 As System.Windows.Forms.DataVisualization.Charting.Legend = New System.Windows.Forms.DataVisualization.Charting.Legend()
        Dim Series2 As System.Windows.Forms.DataVisualization.Charting.Series = New System.Windows.Forms.DataVisualization.Charting.Series()
        Me.splitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.cbDevNames = New System.Windows.Forms.ComboBox()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.label1 = New System.Windows.Forms.Label()
        Me.btnScan = New System.Windows.Forms.Button()
        Me.splitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.TabControlMain = New System.Windows.Forms.TabControl()
        Me.TabPage6 = New System.Windows.Forms.TabPage()
        Me.SplitContainer4 = New System.Windows.Forms.SplitContainer()
        Me.TLPOverview = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel5 = New System.Windows.Forms.TableLayoutPanel()
        Me.pbRecordCount = New System.Windows.Forms.PictureBox()
        Me.btnEraseDataflash = New System.Windows.Forms.Button()
        Me.txtRecordCount = New System.Windows.Forms.TextBox()
        Me.TableLayoutPanel4 = New System.Windows.Forms.TableLayoutPanel()
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
        Me.TableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel()
        Me.cbVerboseMode = New System.Windows.Forms.ComboBox()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.btnWriteRFVals = New System.Windows.Forms.Button()
        Me.btnReadRFVals = New System.Windows.Forms.Button()
        Me.TabPage8 = New System.Windows.Forms.TabPage()
        Me.SplitContainer6 = New System.Windows.Forms.SplitContainer()
        Me.TLPGSMSet = New System.Windows.Forms.TableLayoutPanel()
        Me.TableLayoutPanel7 = New System.Windows.Forms.TableLayoutPanel()
        Me.cbGPRSSettings = New System.Windows.Forms.ComboBox()
        Me.TableLayoutPanel6 = New System.Windows.Forms.TableLayoutPanel()
        Me.dtpSendEmailNext = New System.Windows.Forms.DateTimePicker()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.btnWriteGSMVals = New System.Windows.Forms.Button()
        Me.btnReadGSMVals = New System.Windows.Forms.Button()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer()
        Me.TabControlRecords = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.lvLogs = New System.Windows.Forms.ListView()
        Me.columnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.columnHeader8 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.columnHeader5 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.columnHeader6 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader11 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.lvSysEvents = New System.Windows.Forms.ListView()
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader7 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader9 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader10 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TabPage4 = New System.Windows.Forms.TabPage()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage9 = New System.Windows.Forms.TabPage()
        Me.lvParsedTags = New System.Windows.Forms.ListView()
        Me.ColumnHeader12 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader13 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader14 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TabPage10 = New System.Windows.Forms.TabPage()
        Me.chrtTagParse = New System.Windows.Forms.DataVisualization.Charting.Chart()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.lblTimeRemain = New System.Windows.Forms.Label()
        Me.lblTransfrRate = New System.Windows.Forms.Label()
        Me.lblRecCount = New System.Windows.Forms.Label()
        Me.lbl1 = New System.Windows.Forms.Label()
        Me.lbl2 = New System.Windows.Forms.Label()
        Me.label2 = New System.Windows.Forms.Label()
        Me.pBTransfer = New System.Windows.Forms.ProgressBar()
        Me.btnSaveLog = New System.Windows.Forms.Button()
        Me.btnClearLog = New System.Windows.Forms.Button()
        Me.btnReadLog = New System.Windows.Forms.Button()
        Me.TabPage5 = New System.Windows.Forms.TabPage()
        Me.rtbSerOut = New System.Windows.Forms.RichTextBox()
        Me.btnConfigureGM862 = New System.Windows.Forms.Button()
        Me.chkGM862BridgeMode = New System.Windows.Forms.CheckBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.saveDialog = New System.Windows.Forms.SaveFileDialog()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.bgwRxRecords = New System.ComponentModel.BackgroundWorker()
        Me.TimerRTC = New System.Windows.Forms.Timer(Me.components)
        Me.bgwPing = New System.ComponentModel.BackgroundWorker()
        Me.PBDevDateTime = New System.Windows.Forms.PictureBox()
        Me.pbVerboseMode = New System.Windows.Forms.PictureBox()
        Me.pbSendEmailNext = New System.Windows.Forms.PictureBox()
        Me.pbGPRSSettings = New System.Windows.Forms.PictureBox()
        Me.Button2 = New System.Windows.Forms.Button()
        CType(Me.splitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitContainer1.Panel1.SuspendLayout()
        Me.splitContainer1.Panel2.SuspendLayout()
        Me.splitContainer1.SuspendLayout()
        CType(Me.splitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitContainer2.Panel1.SuspendLayout()
        Me.splitContainer2.Panel2.SuspendLayout()
        Me.splitContainer2.SuspendLayout()
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
        Me.TabPage7.SuspendLayout()
        CType(Me.SplitContainer5, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer5.Panel1.SuspendLayout()
        Me.SplitContainer5.Panel2.SuspendLayout()
        Me.SplitContainer5.SuspendLayout()
        Me.TLPRFSet.SuspendLayout()
        Me.TableLayoutPanel3.SuspendLayout()
        Me.TabPage8.SuspendLayout()
        CType(Me.SplitContainer6, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer6.Panel1.SuspendLayout()
        Me.SplitContainer6.Panel2.SuspendLayout()
        Me.SplitContainer6.SuspendLayout()
        Me.TLPGSMSet.SuspendLayout()
        Me.TableLayoutPanel7.SuspendLayout()
        Me.TableLayoutPanel6.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer3.Panel1.SuspendLayout()
        Me.SplitContainer3.Panel2.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        Me.TabControlRecords.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.TabPage4.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.TabPage9.SuspendLayout()
        Me.TabPage10.SuspendLayout()
        CType(Me.chrtTagParse, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.TabPage5.SuspendLayout()
        CType(Me.PBDevDateTime, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pbVerboseMode, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pbSendEmailNext, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pbGPRSSettings, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'splitContainer1
        '
        Me.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.splitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.splitContainer1.Name = "splitContainer1"
        Me.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'splitContainer1.Panel1
        '
        Me.splitContainer1.Panel1.Controls.Add(Me.cbDevNames)
        Me.splitContainer1.Panel1.Controls.Add(Me.lblStatus)
        Me.splitContainer1.Panel1.Controls.Add(Me.label1)
        Me.splitContainer1.Panel1.Controls.Add(Me.btnScan)
        Me.splitContainer1.Panel1MinSize = 46
        '
        'splitContainer1.Panel2
        '
        Me.splitContainer1.Panel2.Controls.Add(Me.splitContainer2)
        Me.splitContainer1.Size = New System.Drawing.Size(498, 540)
        Me.splitContainer1.SplitterDistance = 46
        Me.splitContainer1.TabIndex = 1
        '
        'cbDevNames
        '
        Me.cbDevNames.FormattingEnabled = True
        Me.cbDevNames.Location = New System.Drawing.Point(93, 12)
        Me.cbDevNames.Name = "cbDevNames"
        Me.cbDevNames.Size = New System.Drawing.Size(121, 21)
        Me.cbDevNames.TabIndex = 39
        '
        'lblStatus
        '
        Me.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblStatus.Location = New System.Drawing.Point(259, 17)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(120, 15)
        Me.lblStatus.TabIndex = 38
        Me.lblStatus.Text = "Not Connected"
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.Location = New System.Drawing.Point(220, 17)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(40, 13)
        Me.label1.TabIndex = 37
        Me.label1.Text = "Status:"
        Me.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'btnScan
        '
        Me.btnScan.Location = New System.Drawing.Point(12, 12)
        Me.btnScan.Name = "btnScan"
        Me.btnScan.Size = New System.Drawing.Size(75, 23)
        Me.btnScan.TabIndex = 36
        Me.btnScan.Text = "Scan"
        Me.btnScan.UseVisualStyleBackColor = True
        '
        'splitContainer2
        '
        Me.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.splitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.splitContainer2.Name = "splitContainer2"
        Me.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'splitContainer2.Panel1
        '
        Me.splitContainer2.Panel1.Controls.Add(Me.TabControlMain)
        '
        'splitContainer2.Panel2
        '
        Me.splitContainer2.Panel2.Controls.Add(Me.Button2)
        Me.splitContainer2.Panel2.Controls.Add(Me.btnConfigureGM862)
        Me.splitContainer2.Panel2.Controls.Add(Me.chkGM862BridgeMode)
        Me.splitContainer2.Panel2.Controls.Add(Me.Button1)
        Me.splitContainer2.Size = New System.Drawing.Size(498, 490)
        Me.splitContainer2.SplitterDistance = 414
        Me.splitContainer2.TabIndex = 2
        '
        'TabControlMain
        '
        Me.TabControlMain.Controls.Add(Me.TabPage6)
        Me.TabControlMain.Controls.Add(Me.TabPage7)
        Me.TabControlMain.Controls.Add(Me.TabPage8)
        Me.TabControlMain.Controls.Add(Me.TabPage2)
        Me.TabControlMain.Controls.Add(Me.TabPage5)
        Me.TabControlMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControlMain.Location = New System.Drawing.Point(0, 0)
        Me.TabControlMain.Name = "TabControlMain"
        Me.TabControlMain.SelectedIndex = 0
        Me.TabControlMain.Size = New System.Drawing.Size(498, 414)
        Me.TabControlMain.TabIndex = 1
        '
        'TabPage6
        '
        Me.TabPage6.Controls.Add(Me.SplitContainer4)
        Me.TabPage6.Location = New System.Drawing.Point(4, 22)
        Me.TabPage6.Name = "TabPage6"
        Me.TabPage6.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage6.Size = New System.Drawing.Size(490, 388)
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
        Me.SplitContainer4.Size = New System.Drawing.Size(484, 382)
        Me.SplitContainer4.SplitterDistance = 304
        Me.SplitContainer4.TabIndex = 1
        '
        'TLPOverview
        '
        Me.TLPOverview.AutoScroll = True
        Me.TLPOverview.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset
        Me.TLPOverview.ColumnCount = 2
        Me.TLPOverview.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 136.0!))
        Me.TLPOverview.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TLPOverview.Controls.Add(Me.TableLayoutPanel5, 1, 3)
        Me.TLPOverview.Controls.Add(Me.TableLayoutPanel4, 1, 4)
        Me.TLPOverview.Controls.Add(Me.Label22, 1, 0)
        Me.TLPOverview.Controls.Add(Me.Label23, 0, 0)
        Me.TLPOverview.Controls.Add(Me.Label20, 0, 4)
        Me.TLPOverview.Controls.Add(Me.Label3, 0, 3)
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
        Me.TLPOverview.Size = New System.Drawing.Size(482, 302)
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
        Me.TableLayoutPanel5.Location = New System.Drawing.Point(143, 83)
        Me.TableLayoutPanel5.Name = "TableLayoutPanel5"
        Me.TableLayoutPanel5.RowCount = 1
        Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel5.Size = New System.Drawing.Size(329, 20)
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
        Me.btnEraseDataflash.Location = New System.Drawing.Point(287, 0)
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
        Me.txtRecordCount.Size = New System.Drawing.Size(261, 20)
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
        Me.TableLayoutPanel4.Location = New System.Drawing.Point(143, 111)
        Me.TableLayoutPanel4.Name = "TableLayoutPanel4"
        Me.TableLayoutPanel4.RowCount = 1
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel4.Size = New System.Drawing.Size(329, 20)
        Me.TableLayoutPanel4.TabIndex = 20
        '
        'btnDevTimeSync
        '
        Me.btnDevTimeSync.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnDevTimeSync.Location = New System.Drawing.Point(287, 0)
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
        Me.dtpDevDateTime.Size = New System.Drawing.Size(261, 20)
        Me.dtpDevDateTime.TabIndex = 17
        '
        'Label22
        '
        Me.Label22.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.Label22.AutoSize = True
        Me.Label22.BackColor = System.Drawing.Color.Transparent
        Me.Label22.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label22.Location = New System.Drawing.Point(283, 2)
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
        Me.Label20.Location = New System.Drawing.Point(5, 114)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(55, 13)
        Me.Label20.TabIndex = 21
        Me.Label20.Text = "RTC Time"
        '
        'Label3
        '
        Me.Label3.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(5, 86)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(73, 13)
        Me.Label3.TabIndex = 23
        Me.Label3.Text = "Record Count"
        '
        'btnWriteVals
        '
        Me.btnWriteVals.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnWriteVals.Location = New System.Drawing.Point(403, 3)
        Me.btnWriteVals.Name = "btnWriteVals"
        Me.btnWriteVals.Size = New System.Drawing.Size(75, 23)
        Me.btnWriteVals.TabIndex = 1
        Me.btnWriteVals.Text = "Write"
        Me.btnWriteVals.UseVisualStyleBackColor = True
        '
        'btnReadVals
        '
        Me.btnReadVals.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnReadVals.Location = New System.Drawing.Point(322, 3)
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
        Me.TabPage7.Size = New System.Drawing.Size(490, 388)
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
        Me.SplitContainer5.Size = New System.Drawing.Size(484, 382)
        Me.SplitContainer5.SplitterDistance = 304
        Me.SplitContainer5.TabIndex = 1
        '
        'TLPRFSet
        '
        Me.TLPRFSet.AutoScroll = True
        Me.TLPRFSet.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset
        Me.TLPRFSet.ColumnCount = 2
        Me.TLPRFSet.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 136.0!))
        Me.TLPRFSet.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TLPRFSet.Controls.Add(Me.TableLayoutPanel3, 1, 4)
        Me.TLPRFSet.Controls.Add(Me.Label21, 1, 0)
        Me.TLPRFSet.Controls.Add(Me.Label26, 0, 0)
        Me.TLPRFSet.Controls.Add(Me.Label24, 0, 4)
        Me.TLPRFSet.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TLPRFSet.Location = New System.Drawing.Point(0, 0)
        Me.TLPRFSet.Margin = New System.Windows.Forms.Padding(0)
        Me.TLPRFSet.Name = "TLPRFSet"
        Me.TLPRFSet.Padding = New System.Windows.Forms.Padding(0, 0, 5, 0)
        Me.TLPRFSet.RowCount = 7
        Me.TLPRFSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TLPRFSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TLPRFSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TLPRFSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TLPRFSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27.0!))
        Me.TLPRFSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 85.0!))
        Me.TLPRFSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27.0!))
        Me.TLPRFSet.Size = New System.Drawing.Size(482, 302)
        Me.TLPRFSet.TabIndex = 0
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
        Me.TableLayoutPanel3.Location = New System.Drawing.Point(143, 111)
        Me.TableLayoutPanel3.Name = "TableLayoutPanel3"
        Me.TableLayoutPanel3.RowCount = 1
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel3.Size = New System.Drawing.Size(329, 21)
        Me.TableLayoutPanel3.TabIndex = 21
        '
        'cbVerboseMode
        '
        Me.cbVerboseMode.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cbVerboseMode.FormattingEnabled = True
        Me.cbVerboseMode.Items.AddRange(New Object() {"1 - Silent", "2 - Print Tag ID", "3 - Pint Tag ID and Time", "4 - Print Tag ID, Time and Address"})
        Me.cbVerboseMode.Location = New System.Drawing.Point(23, 0)
        Me.cbVerboseMode.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.cbVerboseMode.Name = "cbVerboseMode"
        Me.cbVerboseMode.Size = New System.Drawing.Size(306, 21)
        Me.cbVerboseMode.TabIndex = 27
        '
        'Label21
        '
        Me.Label21.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.Label21.AutoSize = True
        Me.Label21.BackColor = System.Drawing.Color.Transparent
        Me.Label21.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label21.Location = New System.Drawing.Point(283, 2)
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
        Me.Label24.Location = New System.Drawing.Point(5, 115)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(76, 13)
        Me.Label24.TabIndex = 22
        Me.Label24.Text = "Verbose Mode"
        '
        'btnWriteRFVals
        '
        Me.btnWriteRFVals.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnWriteRFVals.Location = New System.Drawing.Point(403, 3)
        Me.btnWriteRFVals.Name = "btnWriteRFVals"
        Me.btnWriteRFVals.Size = New System.Drawing.Size(75, 23)
        Me.btnWriteRFVals.TabIndex = 1
        Me.btnWriteRFVals.Text = "Write"
        Me.btnWriteRFVals.UseVisualStyleBackColor = True
        '
        'btnReadRFVals
        '
        Me.btnReadRFVals.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnReadRFVals.Location = New System.Drawing.Point(322, 3)
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
        Me.TabPage8.Size = New System.Drawing.Size(490, 388)
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
        Me.SplitContainer6.Size = New System.Drawing.Size(484, 382)
        Me.SplitContainer6.SplitterDistance = 304
        Me.SplitContainer6.TabIndex = 1
        '
        'TLPGSMSet
        '
        Me.TLPGSMSet.AutoScroll = True
        Me.TLPGSMSet.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset
        Me.TLPGSMSet.ColumnCount = 2
        Me.TLPGSMSet.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 136.0!))
        Me.TLPGSMSet.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TLPGSMSet.Controls.Add(Me.TableLayoutPanel7, 1, 4)
        Me.TLPGSMSet.Controls.Add(Me.TableLayoutPanel6, 1, 2)
        Me.TLPGSMSet.Controls.Add(Me.Label25, 1, 0)
        Me.TLPGSMSet.Controls.Add(Me.Label27, 0, 0)
        Me.TLPGSMSet.Controls.Add(Me.Label28, 0, 2)
        Me.TLPGSMSet.Controls.Add(Me.Label4, 0, 4)
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
        Me.TLPGSMSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27.0!))
        Me.TLPGSMSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26.0!))
        Me.TLPGSMSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TLPGSMSet.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 43.0!))
        Me.TLPGSMSet.Size = New System.Drawing.Size(482, 302)
        Me.TLPGSMSet.TabIndex = 0
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
        Me.TableLayoutPanel7.Location = New System.Drawing.Point(143, 111)
        Me.TableLayoutPanel7.Name = "TableLayoutPanel7"
        Me.TableLayoutPanel7.RowCount = 1
        Me.TableLayoutPanel7.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel7.Size = New System.Drawing.Size(329, 21)
        Me.TableLayoutPanel7.TabIndex = 26
        '
        'cbGPRSSettings
        '
        Me.cbGPRSSettings.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cbGPRSSettings.FormattingEnabled = True
        Me.cbGPRSSettings.Items.AddRange(New Object() {"NONE - APN="""", USERID="""", PASSW=""""", "OPTUS - APN=""Internet"", USERID="""", PASSW="""""})
        Me.cbGPRSSettings.Location = New System.Drawing.Point(23, 0)
        Me.cbGPRSSettings.Margin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.cbGPRSSettings.Name = "cbGPRSSettings"
        Me.cbGPRSSettings.Size = New System.Drawing.Size(306, 21)
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
        Me.TableLayoutPanel6.Size = New System.Drawing.Size(329, 20)
        Me.TableLayoutPanel6.TabIndex = 23
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
        Me.dtpSendEmailNext.Size = New System.Drawing.Size(306, 20)
        Me.dtpSendEmailNext.TabIndex = 27
        '
        'Label25
        '
        Me.Label25.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.Label25.AutoSize = True
        Me.Label25.BackColor = System.Drawing.Color.Transparent
        Me.Label25.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label25.Location = New System.Drawing.Point(283, 2)
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
        Me.Label4.Location = New System.Drawing.Point(5, 115)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(78, 13)
        Me.Label4.TabIndex = 24
        Me.Label4.Text = "GPRS Settings"
        '
        'btnWriteGSMVals
        '
        Me.btnWriteGSMVals.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnWriteGSMVals.Location = New System.Drawing.Point(403, 3)
        Me.btnWriteGSMVals.Name = "btnWriteGSMVals"
        Me.btnWriteGSMVals.Size = New System.Drawing.Size(75, 23)
        Me.btnWriteGSMVals.TabIndex = 1
        Me.btnWriteGSMVals.Text = "Write"
        Me.btnWriteGSMVals.UseVisualStyleBackColor = True
        '
        'btnReadGSMVals
        '
        Me.btnReadGSMVals.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnReadGSMVals.Location = New System.Drawing.Point(322, 3)
        Me.btnReadGSMVals.Name = "btnReadGSMVals"
        Me.btnReadGSMVals.Size = New System.Drawing.Size(75, 23)
        Me.btnReadGSMVals.TabIndex = 1
        Me.btnReadGSMVals.Text = "Read"
        Me.btnReadGSMVals.UseVisualStyleBackColor = True
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.SplitContainer3)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Margin = New System.Windows.Forms.Padding(0)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(490, 388)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Records"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'SplitContainer3
        '
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer3.IsSplitterFixed = True
        Me.SplitContainer3.Location = New System.Drawing.Point(3, 3)
        Me.SplitContainer3.Name = "SplitContainer3"
        Me.SplitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer3.Panel1
        '
        Me.SplitContainer3.Panel1.Controls.Add(Me.TabControlRecords)
        '
        'SplitContainer3.Panel2
        '
        Me.SplitContainer3.Panel2.Controls.Add(Me.TableLayoutPanel2)
        Me.SplitContainer3.Panel2MinSize = 30
        Me.SplitContainer3.Size = New System.Drawing.Size(484, 382)
        Me.SplitContainer3.SplitterDistance = 348
        Me.SplitContainer3.TabIndex = 4
        '
        'TabControlRecords
        '
        Me.TabControlRecords.Controls.Add(Me.TabPage1)
        Me.TabControlRecords.Controls.Add(Me.TabPage3)
        Me.TabControlRecords.Controls.Add(Me.TabPage4)
        Me.TabControlRecords.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControlRecords.Location = New System.Drawing.Point(0, 0)
        Me.TabControlRecords.Name = "TabControlRecords"
        Me.TabControlRecords.SelectedIndex = 0
        Me.TabControlRecords.Size = New System.Drawing.Size(484, 348)
        Me.TabControlRecords.TabIndex = 0
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.lvLogs)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(476, 322)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Tags"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'lvLogs
        '
        Me.lvLogs.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.columnHeader4, Me.columnHeader8, Me.columnHeader5, Me.columnHeader6, Me.ColumnHeader1, Me.ColumnHeader11})
        Me.lvLogs.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvLogs.FullRowSelect = True
        Me.lvLogs.Location = New System.Drawing.Point(3, 3)
        Me.lvLogs.Name = "lvLogs"
        Me.lvLogs.Size = New System.Drawing.Size(470, 316)
        Me.lvLogs.TabIndex = 4
        Me.lvLogs.UseCompatibleStateImageBehavior = False
        Me.lvLogs.View = System.Windows.Forms.View.Details
        '
        'columnHeader4
        '
        Me.columnHeader4.Text = "Entry"
        Me.columnHeader4.Width = 65
        '
        'columnHeader8
        '
        Me.columnHeader8.Text = "Date"
        Me.columnHeader8.Width = 72
        '
        'columnHeader5
        '
        Me.columnHeader5.Text = "Time"
        Me.columnHeader5.Width = 79
        '
        'columnHeader6
        '
        Me.columnHeader6.Text = "ID"
        Me.columnHeader6.Width = 75
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Flags"
        Me.ColumnHeader1.Width = 76
        '
        'ColumnHeader11
        '
        Me.ColumnHeader11.Text = "RSSI"
        Me.ColumnHeader11.Width = 73
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.lvSysEvents)
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage3.Size = New System.Drawing.Size(476, 322)
        Me.TabPage3.TabIndex = 1
        Me.TabPage3.Text = "System Events"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'lvSysEvents
        '
        Me.lvSysEvents.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader2, Me.ColumnHeader3, Me.ColumnHeader7, Me.ColumnHeader9, Me.ColumnHeader10})
        Me.lvSysEvents.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvSysEvents.FullRowSelect = True
        Me.lvSysEvents.Location = New System.Drawing.Point(3, 3)
        Me.lvSysEvents.Name = "lvSysEvents"
        Me.lvSysEvents.Size = New System.Drawing.Size(470, 316)
        Me.lvSysEvents.TabIndex = 5
        Me.lvSysEvents.UseCompatibleStateImageBehavior = False
        Me.lvSysEvents.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Entry"
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "Date"
        Me.ColumnHeader3.Width = 72
        '
        'ColumnHeader7
        '
        Me.ColumnHeader7.Text = "Time"
        Me.ColumnHeader7.Width = 73
        '
        'ColumnHeader9
        '
        Me.ColumnHeader9.Text = "Command"
        Me.ColumnHeader9.Width = 167
        '
        'ColumnHeader10
        '
        Me.ColumnHeader10.Text = "Data"
        Me.ColumnHeader10.Width = 63
        '
        'TabPage4
        '
        Me.TabPage4.Controls.Add(Me.TabControl1)
        Me.TabPage4.Location = New System.Drawing.Point(4, 22)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage4.Size = New System.Drawing.Size(476, 322)
        Me.TabPage4.TabIndex = 2
        Me.TabPage4.Text = "Parsed Tags"
        Me.TabPage4.UseVisualStyleBackColor = True
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage9)
        Me.TabControl1.Controls.Add(Me.TabPage10)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Location = New System.Drawing.Point(3, 3)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(470, 316)
        Me.TabControl1.TabIndex = 6
        '
        'TabPage9
        '
        Me.TabPage9.Controls.Add(Me.lvParsedTags)
        Me.TabPage9.Location = New System.Drawing.Point(4, 22)
        Me.TabPage9.Name = "TabPage9"
        Me.TabPage9.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage9.Size = New System.Drawing.Size(462, 290)
        Me.TabPage9.TabIndex = 0
        Me.TabPage9.Text = "Data"
        Me.TabPage9.UseVisualStyleBackColor = True
        '
        'lvParsedTags
        '
        Me.lvParsedTags.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader12, Me.ColumnHeader13, Me.ColumnHeader14})
        Me.lvParsedTags.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvParsedTags.FullRowSelect = True
        Me.lvParsedTags.Location = New System.Drawing.Point(3, 3)
        Me.lvParsedTags.Name = "lvParsedTags"
        Me.lvParsedTags.Size = New System.Drawing.Size(456, 284)
        Me.lvParsedTags.TabIndex = 6
        Me.lvParsedTags.UseCompatibleStateImageBehavior = False
        Me.lvParsedTags.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader12
        '
        Me.ColumnHeader12.Text = "Entry"
        Me.ColumnHeader12.Width = 65
        '
        'ColumnHeader13
        '
        Me.ColumnHeader13.Text = "Date"
        Me.ColumnHeader13.Width = 72
        '
        'ColumnHeader14
        '
        Me.ColumnHeader14.Text = "Time"
        Me.ColumnHeader14.Width = 79
        '
        'TabPage10
        '
        Me.TabPage10.Controls.Add(Me.chrtTagParse)
        Me.TabPage10.Location = New System.Drawing.Point(4, 22)
        Me.TabPage10.Name = "TabPage10"
        Me.TabPage10.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage10.Size = New System.Drawing.Size(462, 290)
        Me.TabPage10.TabIndex = 1
        Me.TabPage10.Text = "Graph"
        Me.TabPage10.UseVisualStyleBackColor = True
        '
        'chrtTagParse
        '
        ChartArea2.Name = "ChartArea1"
        Me.chrtTagParse.ChartAreas.Add(ChartArea2)
        Me.chrtTagParse.Dock = System.Windows.Forms.DockStyle.Fill
        Legend2.Name = "Legend1"
        Me.chrtTagParse.Legends.Add(Legend2)
        Me.chrtTagParse.Location = New System.Drawing.Point(3, 3)
        Me.chrtTagParse.Name = "chrtTagParse"
        Series2.ChartArea = "ChartArea1"
        Series2.Legend = "Legend1"
        Series2.Name = "Series1"
        Me.chrtTagParse.Series.Add(Series2)
        Me.chrtTagParse.Size = New System.Drawing.Size(456, 284)
        Me.chrtTagParse.TabIndex = 0
        Me.chrtTagParse.Text = "Chart1"
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 3
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.Panel2, 1, 0)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 1
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(484, 30)
        Me.TableLayoutPanel2.TabIndex = 1
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.lblTimeRemain)
        Me.Panel2.Controls.Add(Me.lblTransfrRate)
        Me.Panel2.Controls.Add(Me.lblRecCount)
        Me.Panel2.Controls.Add(Me.lbl1)
        Me.Panel2.Controls.Add(Me.lbl2)
        Me.Panel2.Controls.Add(Me.label2)
        Me.Panel2.Controls.Add(Me.pBTransfer)
        Me.Panel2.Controls.Add(Me.btnSaveLog)
        Me.Panel2.Controls.Add(Me.btnClearLog)
        Me.Panel2.Controls.Add(Me.btnReadLog)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(37, 3)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(410, 24)
        Me.Panel2.TabIndex = 0
        '
        'lblTimeRemain
        '
        Me.lblTimeRemain.AutoSize = True
        Me.lblTimeRemain.Location = New System.Drawing.Point(367, 45)
        Me.lblTimeRemain.Name = "lblTimeRemain"
        Me.lblTimeRemain.Size = New System.Drawing.Size(13, 13)
        Me.lblTimeRemain.TabIndex = 59
        Me.lblTimeRemain.Text = "0"
        '
        'lblTransfrRate
        '
        Me.lblTransfrRate.AutoSize = True
        Me.lblTransfrRate.Location = New System.Drawing.Point(269, 45)
        Me.lblTransfrRate.Name = "lblTransfrRate"
        Me.lblTransfrRate.Size = New System.Drawing.Size(13, 13)
        Me.lblTransfrRate.TabIndex = 58
        Me.lblTransfrRate.Text = "0"
        '
        'lblRecCount
        '
        Me.lblRecCount.AutoSize = True
        Me.lblRecCount.Location = New System.Drawing.Point(88, 45)
        Me.lblRecCount.Name = "lblRecCount"
        Me.lblRecCount.Size = New System.Drawing.Size(13, 13)
        Me.lblRecCount.TabIndex = 57
        Me.lblRecCount.Text = "0"
        '
        'lbl1
        '
        Me.lbl1.AutoSize = True
        Me.lbl1.Location = New System.Drawing.Point(17, 45)
        Me.lbl1.Name = "lbl1"
        Me.lbl1.Size = New System.Drawing.Size(76, 13)
        Me.lbl1.TabIndex = 54
        Me.lbl1.Text = "Record Count:"
        '
        'lbl2
        '
        Me.lbl2.AutoSize = True
        Me.lbl2.Location = New System.Drawing.Point(318, 45)
        Me.lbl2.Name = "lbl2"
        Me.lbl2.Size = New System.Drawing.Size(57, 13)
        Me.lbl2.TabIndex = 56
        Me.lbl2.Text = "Time Left: "
        '
        'label2
        '
        Me.label2.AutoSize = True
        Me.label2.Location = New System.Drawing.Point(199, 45)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(75, 13)
        Me.label2.TabIndex = 55
        Me.label2.Text = "Transfer Rate:"
        '
        'pBTransfer
        '
        Me.pBTransfer.Location = New System.Drawing.Point(11, 26)
        Me.pBTransfer.Name = "pBTransfer"
        Me.pBTransfer.Size = New System.Drawing.Size(388, 16)
        Me.pBTransfer.TabIndex = 53
        '
        'btnSaveLog
        '
        Me.btnSaveLog.Location = New System.Drawing.Point(168, 3)
        Me.btnSaveLog.Name = "btnSaveLog"
        Me.btnSaveLog.Size = New System.Drawing.Size(75, 20)
        Me.btnSaveLog.TabIndex = 52
        Me.btnSaveLog.Text = "Save"
        Me.btnSaveLog.UseVisualStyleBackColor = True
        '
        'btnClearLog
        '
        Me.btnClearLog.Location = New System.Drawing.Point(310, 1)
        Me.btnClearLog.Name = "btnClearLog"
        Me.btnClearLog.Size = New System.Drawing.Size(89, 21)
        Me.btnClearLog.TabIndex = 51
        Me.btnClearLog.Text = "Clear Log"
        Me.btnClearLog.UseVisualStyleBackColor = True
        '
        'btnReadLog
        '
        Me.btnReadLog.Location = New System.Drawing.Point(11, 2)
        Me.btnReadLog.Name = "btnReadLog"
        Me.btnReadLog.Size = New System.Drawing.Size(90, 21)
        Me.btnReadLog.TabIndex = 50
        Me.btnReadLog.Text = "Read Log"
        Me.btnReadLog.UseVisualStyleBackColor = True
        '
        'TabPage5
        '
        Me.TabPage5.Controls.Add(Me.rtbSerOut)
        Me.TabPage5.Location = New System.Drawing.Point(4, 22)
        Me.TabPage5.Name = "TabPage5"
        Me.TabPage5.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage5.Size = New System.Drawing.Size(490, 388)
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
        Me.rtbSerOut.Size = New System.Drawing.Size(484, 382)
        Me.rtbSerOut.TabIndex = 0
        Me.rtbSerOut.Text = ""
        '
        'btnConfigureGM862
        '
        Me.btnConfigureGM862.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnConfigureGM862.Location = New System.Drawing.Point(380, 31)
        Me.btnConfigureGM862.Name = "btnConfigureGM862"
        Me.btnConfigureGM862.Size = New System.Drawing.Size(114, 23)
        Me.btnConfigureGM862.TabIndex = 3
        Me.btnConfigureGM862.Text = "Configure GM862"
        Me.btnConfigureGM862.UseVisualStyleBackColor = True
        Me.btnConfigureGM862.Visible = False
        '
        'chkGM862BridgeMode
        '
        Me.chkGM862BridgeMode.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkGM862BridgeMode.Appearance = System.Windows.Forms.Appearance.Button
        Me.chkGM862BridgeMode.AutoSize = True
        Me.chkGM862BridgeMode.Location = New System.Drawing.Point(380, 2)
        Me.chkGM862BridgeMode.Name = "chkGM862BridgeMode"
        Me.chkGM862BridgeMode.Size = New System.Drawing.Size(115, 23)
        Me.chkGM862BridgeMode.TabIndex = 2
        Me.chkGM862BridgeMode.Text = "GM862 Bridge Mode"
        Me.chkGM862BridgeMode.UseVisualStyleBackColor = True
        Me.chkGM862BridgeMode.Visible = False
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(4, 2)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 0
        Me.Button1.Text = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        Me.Button1.Visible = False
        '
        'saveDialog
        '
        Me.saveDialog.DefaultExt = "csv"
        Me.saveDialog.Filter = "Comma Seperated List File (*.csv)|*.csv|All Files (*.*)|*.*"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(200, 100)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'bgwRxRecords
        '
        '
        'TimerRTC
        '
        Me.TimerRTC.Interval = 1000
        '
        'bgwPing
        '
        Me.bgwPing.WorkerSupportsCancellation = True
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
        'pbVerboseMode
        '
        Me.pbVerboseMode.BackColor = System.Drawing.Color.White
        Me.pbVerboseMode.BackgroundImage = CType(resources.GetObject("pbVerboseMode.BackgroundImage"), System.Drawing.Image)
        Me.pbVerboseMode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.pbVerboseMode.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pbVerboseMode.Location = New System.Drawing.Point(0, 0)
        Me.pbVerboseMode.Margin = New System.Windows.Forms.Padding(0)
        Me.pbVerboseMode.Name = "pbVerboseMode"
        Me.pbVerboseMode.Size = New System.Drawing.Size(20, 21)
        Me.pbVerboseMode.TabIndex = 28
        Me.pbVerboseMode.TabStop = False
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
        'pbGPRSSettings
        '
        Me.pbGPRSSettings.BackColor = System.Drawing.Color.White
        Me.pbGPRSSettings.BackgroundImage = CType(resources.GetObject("pbGPRSSettings.BackgroundImage"), System.Drawing.Image)
        Me.pbGPRSSettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.pbGPRSSettings.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pbGPRSSettings.Location = New System.Drawing.Point(0, 0)
        Me.pbGPRSSettings.Margin = New System.Windows.Forms.Padding(0)
        Me.pbGPRSSettings.Name = "pbGPRSSettings"
        Me.pbGPRSSettings.Size = New System.Drawing.Size(20, 21)
        Me.pbGPRSSettings.TabIndex = 29
        Me.pbGPRSSettings.TabStop = False
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(4, 31)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 4
        Me.Button2.Text = "Button2"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(498, 540)
        Me.Controls.Add(Me.splitContainer1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMain"
        Me.Text = "WildSpy WID Logger Comms"
        Me.splitContainer1.Panel1.ResumeLayout(False)
        Me.splitContainer1.Panel1.PerformLayout()
        Me.splitContainer1.Panel2.ResumeLayout(False)
        CType(Me.splitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitContainer1.ResumeLayout(False)
        Me.splitContainer2.Panel1.ResumeLayout(False)
        Me.splitContainer2.Panel2.ResumeLayout(False)
        Me.splitContainer2.Panel2.PerformLayout()
        CType(Me.splitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitContainer2.ResumeLayout(False)
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
        Me.TabPage7.ResumeLayout(False)
        Me.SplitContainer5.Panel1.ResumeLayout(False)
        Me.SplitContainer5.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer5, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer5.ResumeLayout(False)
        Me.TLPRFSet.ResumeLayout(False)
        Me.TLPRFSet.PerformLayout()
        Me.TableLayoutPanel3.ResumeLayout(False)
        Me.TabPage8.ResumeLayout(False)
        Me.SplitContainer6.Panel1.ResumeLayout(False)
        Me.SplitContainer6.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer6, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer6.ResumeLayout(False)
        Me.TLPGSMSet.ResumeLayout(False)
        Me.TLPGSMSet.PerformLayout()
        Me.TableLayoutPanel7.ResumeLayout(False)
        Me.TableLayoutPanel6.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.SplitContainer3.Panel1.ResumeLayout(False)
        Me.SplitContainer3.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer3.ResumeLayout(False)
        Me.TabControlRecords.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage3.ResumeLayout(False)
        Me.TabPage4.ResumeLayout(False)
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage9.ResumeLayout(False)
        Me.TabPage10.ResumeLayout(False)
        CType(Me.chrtTagParse, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.TabPage5.ResumeLayout(False)
        CType(Me.PBDevDateTime, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pbVerboseMode, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pbSendEmailNext, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pbGPRSSettings, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents splitContainer1 As System.Windows.Forms.SplitContainer
    Private WithEvents cbDevNames As System.Windows.Forms.ComboBox
    Private WithEvents lblStatus As System.Windows.Forms.Label
    Private WithEvents label1 As System.Windows.Forms.Label
    Private WithEvents btnScan As System.Windows.Forms.Button
    Private WithEvents splitContainer2 As System.Windows.Forms.SplitContainer
    Private WithEvents saveDialog As System.Windows.Forms.SaveFileDialog
    Friend WithEvents TabControlMain As System.Windows.Forms.TabControl
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents SplitContainer3 As System.Windows.Forms.SplitContainer
    Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Private WithEvents lblTimeRemain As System.Windows.Forms.Label
    Private WithEvents lblTransfrRate As System.Windows.Forms.Label
    Private WithEvents lblRecCount As System.Windows.Forms.Label
    Private WithEvents lbl1 As System.Windows.Forms.Label
    Private WithEvents lbl2 As System.Windows.Forms.Label
    Private WithEvents label2 As System.Windows.Forms.Label
    Private WithEvents pBTransfer As System.Windows.Forms.ProgressBar
    Private WithEvents btnSaveLog As System.Windows.Forms.Button
    Private WithEvents btnClearLog As System.Windows.Forms.Button
    Private WithEvents btnReadLog As System.Windows.Forms.Button
    Friend WithEvents bgwRxRecords As System.ComponentModel.BackgroundWorker
    Private WithEvents TimerRTC As System.Windows.Forms.Timer
    Friend WithEvents TabPage5 As System.Windows.Forms.TabPage
    Friend WithEvents rtbSerOut As System.Windows.Forms.RichTextBox
    Friend WithEvents TabPage6 As System.Windows.Forms.TabPage
    Friend WithEvents SplitContainer4 As System.Windows.Forms.SplitContainer
    Friend WithEvents btnWriteVals As System.Windows.Forms.Button
    Friend WithEvents btnReadVals As System.Windows.Forms.Button
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents TabPage7 As System.Windows.Forms.TabPage
    Friend WithEvents SplitContainer5 As System.Windows.Forms.SplitContainer
    Friend WithEvents TLPRFSet As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents Label26 As System.Windows.Forms.Label
    Friend WithEvents btnWriteRFVals As System.Windows.Forms.Button
    Friend WithEvents btnReadRFVals As System.Windows.Forms.Button
    Friend WithEvents TLPOverview As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents TableLayoutPanel4 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents btnDevTimeSync As System.Windows.Forms.Button
    Friend WithEvents dtpDevDateTime As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents TableLayoutPanel3 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents cbVerboseMode As System.Windows.Forms.ComboBox
    Friend WithEvents Label24 As System.Windows.Forms.Label
    Friend WithEvents bgwPing As System.ComponentModel.BackgroundWorker
    Friend WithEvents TabPage8 As System.Windows.Forms.TabPage
    Friend WithEvents SplitContainer6 As System.Windows.Forms.SplitContainer
    Friend WithEvents TLPGSMSet As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Label25 As System.Windows.Forms.Label
    Friend WithEvents Label27 As System.Windows.Forms.Label
    Friend WithEvents Label28 As System.Windows.Forms.Label
    Friend WithEvents btnWriteGSMVals As System.Windows.Forms.Button
    Friend WithEvents btnReadGSMVals As System.Windows.Forms.Button
    Friend WithEvents TableLayoutPanel6 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents dtpSendEmailNext As System.Windows.Forms.DateTimePicker
    Friend WithEvents TableLayoutPanel5 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents pbRecordCount As System.Windows.Forms.PictureBox
    Friend WithEvents btnEraseDataflash As System.Windows.Forms.Button
    Friend WithEvents txtRecordCount As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TabControlRecords As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Private WithEvents lvLogs As System.Windows.Forms.ListView
    Private WithEvents columnHeader4 As System.Windows.Forms.ColumnHeader
    Private WithEvents columnHeader8 As System.Windows.Forms.ColumnHeader
    Private WithEvents columnHeader5 As System.Windows.Forms.ColumnHeader
    Private WithEvents columnHeader6 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Private WithEvents lvSysEvents As System.Windows.Forms.ListView
    Private WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Private WithEvents ColumnHeader3 As System.Windows.Forms.ColumnHeader
    Private WithEvents ColumnHeader7 As System.Windows.Forms.ColumnHeader
    Private WithEvents ColumnHeader9 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader10 As System.Windows.Forms.ColumnHeader
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents chkGM862BridgeMode As System.Windows.Forms.CheckBox
    Friend WithEvents TableLayoutPanel7 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents cbGPRSSettings As System.Windows.Forms.ComboBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents btnConfigureGM862 As System.Windows.Forms.Button
    Friend WithEvents ColumnHeader11 As System.Windows.Forms.ColumnHeader
    Friend WithEvents TabPage4 As System.Windows.Forms.TabPage
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage9 As System.Windows.Forms.TabPage
    Private WithEvents lvParsedTags As System.Windows.Forms.ListView
    Private WithEvents ColumnHeader12 As System.Windows.Forms.ColumnHeader
    Private WithEvents ColumnHeader13 As System.Windows.Forms.ColumnHeader
    Private WithEvents ColumnHeader14 As System.Windows.Forms.ColumnHeader
    Friend WithEvents TabPage10 As System.Windows.Forms.TabPage
    Friend WithEvents chrtTagParse As System.Windows.Forms.DataVisualization.Charting.Chart
    Friend WithEvents PBDevDateTime As System.Windows.Forms.PictureBox
    Friend WithEvents pbVerboseMode As System.Windows.Forms.PictureBox
    Friend WithEvents pbGPRSSettings As System.Windows.Forms.PictureBox
    Friend WithEvents pbSendEmailNext As System.Windows.Forms.PictureBox
    Friend WithEvents Button2 As System.Windows.Forms.Button
End Class
