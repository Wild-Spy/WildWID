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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.rtbInterpretText = New System.Windows.Forms.RichTextBox()
        Me.btnInterpretAppend = New System.Windows.Forms.Button()
        Me.btnInterpret = New System.Windows.Forms.Button()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer()
        Me.TabControlRecords = New System.Windows.Forms.TabControl()
        Me.TabPage4 = New System.Windows.Forms.TabPage()
        Me.lvLogs = New System.Windows.Forms.ListView()
        Me.columnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.columnHeader8 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.columnHeader5 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.columnHeader6 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TabPage5 = New System.Windows.Forms.TabPage()
        Me.lvSysEvents = New System.Windows.Forms.ListView()
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader7 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader9 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader10 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.lvTimeline = New System.Windows.Forms.ListView()
        Me.ColumnHeader11 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader12 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader13 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader14 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader15 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader16 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.btnSaveAll = New System.Windows.Forms.Button()
        Me.btnSaveTimelines = New System.Windows.Forms.Button()
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
        Me.btnLoadLog = New System.Windows.Forms.Button()
        Me.saveDialog = New System.Windows.Forms.SaveFileDialog()
        Me.OpenDialog = New System.Windows.Forms.OpenFileDialog()
        Me.SaveDialog1 = New System.Windows.Forms.SaveFileDialog()
        Me.SaveDialog2 = New System.Windows.Forms.SaveFileDialog()
        Me.TabPage1.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer3.Panel1.SuspendLayout()
        Me.SplitContainer3.Panel2.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        Me.TabControlRecords.SuspendLayout()
        Me.TabPage4.SuspendLayout()
        Me.TabPage5.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.SplitContainer1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(624, 511)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Interpret"
        Me.TabPage1.UseVisualStyleBackColor = True
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
        Me.SplitContainer1.Panel1.Controls.Add(Me.rtbInterpretText)
        Me.SplitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.btnInterpretAppend)
        Me.SplitContainer1.Panel2.Controls.Add(Me.btnInterpret)
        Me.SplitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.SplitContainer1.Size = New System.Drawing.Size(618, 505)
        Me.SplitContainer1.SplitterDistance = 455
        Me.SplitContainer1.TabIndex = 0
        '
        'rtbInterpretText
        '
        Me.rtbInterpretText.Dock = System.Windows.Forms.DockStyle.Fill
        Me.rtbInterpretText.Location = New System.Drawing.Point(0, 0)
        Me.rtbInterpretText.Name = "rtbInterpretText"
        Me.rtbInterpretText.Size = New System.Drawing.Size(618, 455)
        Me.rtbInterpretText.TabIndex = 0
        Me.rtbInterpretText.Text = ""
        '
        'btnInterpretAppend
        '
        Me.btnInterpretAppend.Location = New System.Drawing.Point(86, 3)
        Me.btnInterpretAppend.Name = "btnInterpretAppend"
        Me.btnInterpretAppend.Size = New System.Drawing.Size(115, 23)
        Me.btnInterpretAppend.TabIndex = 1
        Me.btnInterpretAppend.Text = "Interpret and Append"
        Me.btnInterpretAppend.UseVisualStyleBackColor = True
        '
        'btnInterpret
        '
        Me.btnInterpret.Location = New System.Drawing.Point(5, 3)
        Me.btnInterpret.Name = "btnInterpret"
        Me.btnInterpret.Size = New System.Drawing.Size(75, 23)
        Me.btnInterpret.TabIndex = 0
        Me.btnInterpret.Text = "Interpret"
        Me.btnInterpret.UseVisualStyleBackColor = True
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(632, 537)
        Me.TabControl1.TabIndex = 0
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.SplitContainer3)
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Margin = New System.Windows.Forms.Padding(0)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage3.Size = New System.Drawing.Size(624, 511)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "Details"
        Me.TabPage3.UseVisualStyleBackColor = True
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
        Me.SplitContainer3.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No
        '
        'SplitContainer3.Panel2
        '
        Me.SplitContainer3.Panel2.Controls.Add(Me.TableLayoutPanel2)
        Me.SplitContainer3.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.SplitContainer3.Panel2MinSize = 30
        Me.SplitContainer3.Size = New System.Drawing.Size(618, 505)
        Me.SplitContainer3.SplitterDistance = 471
        Me.SplitContainer3.TabIndex = 4
        '
        'TabControlRecords
        '
        Me.TabControlRecords.Controls.Add(Me.TabPage4)
        Me.TabControlRecords.Controls.Add(Me.TabPage5)
        Me.TabControlRecords.Controls.Add(Me.TabPage2)
        Me.TabControlRecords.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControlRecords.Location = New System.Drawing.Point(0, 0)
        Me.TabControlRecords.Name = "TabControlRecords"
        Me.TabControlRecords.SelectedIndex = 0
        Me.TabControlRecords.Size = New System.Drawing.Size(618, 471)
        Me.TabControlRecords.TabIndex = 0
        '
        'TabPage4
        '
        Me.TabPage4.Controls.Add(Me.lvLogs)
        Me.TabPage4.Location = New System.Drawing.Point(4, 22)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage4.Size = New System.Drawing.Size(610, 445)
        Me.TabPage4.TabIndex = 0
        Me.TabPage4.Text = "Tags"
        Me.TabPage4.UseVisualStyleBackColor = True
        '
        'lvLogs
        '
        Me.lvLogs.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.columnHeader4, Me.columnHeader8, Me.columnHeader5, Me.columnHeader6, Me.ColumnHeader1})
        Me.lvLogs.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvLogs.FullRowSelect = True
        Me.lvLogs.Location = New System.Drawing.Point(3, 3)
        Me.lvLogs.Name = "lvLogs"
        Me.lvLogs.Size = New System.Drawing.Size(604, 439)
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
        'TabPage5
        '
        Me.TabPage5.Controls.Add(Me.lvSysEvents)
        Me.TabPage5.Location = New System.Drawing.Point(4, 22)
        Me.TabPage5.Name = "TabPage5"
        Me.TabPage5.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage5.Size = New System.Drawing.Size(610, 445)
        Me.TabPage5.TabIndex = 1
        Me.TabPage5.Text = "System Events"
        Me.TabPage5.UseVisualStyleBackColor = True
        '
        'lvSysEvents
        '
        Me.lvSysEvents.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader2, Me.ColumnHeader3, Me.ColumnHeader7, Me.ColumnHeader9, Me.ColumnHeader10})
        Me.lvSysEvents.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvSysEvents.FullRowSelect = True
        Me.lvSysEvents.Location = New System.Drawing.Point(3, 3)
        Me.lvSysEvents.Name = "lvSysEvents"
        Me.lvSysEvents.Size = New System.Drawing.Size(604, 439)
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
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.lvTimeline)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(610, 445)
        Me.TabPage2.TabIndex = 2
        Me.TabPage2.Text = "TagTag Timelines"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'lvTimeline
        '
        Me.lvTimeline.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader11, Me.ColumnHeader12, Me.ColumnHeader13, Me.ColumnHeader14, Me.ColumnHeader15, Me.ColumnHeader16})
        Me.lvTimeline.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvTimeline.FullRowSelect = True
        Me.lvTimeline.Location = New System.Drawing.Point(3, 3)
        Me.lvTimeline.Name = "lvTimeline"
        Me.lvTimeline.Size = New System.Drawing.Size(604, 439)
        Me.lvTimeline.TabIndex = 5
        Me.lvTimeline.UseCompatibleStateImageBehavior = False
        Me.lvTimeline.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader11
        '
        Me.ColumnHeader11.Text = "Start"
        Me.ColumnHeader11.Width = 142
        '
        'ColumnHeader12
        '
        Me.ColumnHeader12.Text = "Stop"
        Me.ColumnHeader12.Width = 146
        '
        'ColumnHeader13
        '
        Me.ColumnHeader13.Text = "Period"
        Me.ColumnHeader13.Width = 129
        '
        'ColumnHeader14
        '
        Me.ColumnHeader14.Text = "ID"
        Me.ColumnHeader14.Width = 75
        '
        'ColumnHeader15
        '
        Me.ColumnHeader15.Text = "Start Addy"
        Me.ColumnHeader15.Width = 70
        '
        'ColumnHeader16
        '
        Me.ColumnHeader16.Text = "Stop Addy"
        Me.ColumnHeader16.Width = 78
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 3
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.btnSaveAll, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.btnSaveTimelines, 2, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.Panel2, 1, 0)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 1
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(618, 30)
        Me.TableLayoutPanel2.TabIndex = 1
        '
        'btnSaveAll
        '
        Me.btnSaveAll.Location = New System.Drawing.Point(3, 3)
        Me.btnSaveAll.Name = "btnSaveAll"
        Me.btnSaveAll.Size = New System.Drawing.Size(77, 22)
        Me.btnSaveAll.TabIndex = 60
        Me.btnSaveAll.Text = "Save All"
        Me.btnSaveAll.UseVisualStyleBackColor = True
        '
        'btnSaveTimelines
        '
        Me.btnSaveTimelines.Location = New System.Drawing.Point(520, 3)
        Me.btnSaveTimelines.Name = "btnSaveTimelines"
        Me.btnSaveTimelines.Size = New System.Drawing.Size(91, 23)
        Me.btnSaveTimelines.TabIndex = 60
        Me.btnSaveTimelines.Text = "Save Timelines"
        Me.btnSaveTimelines.UseVisualStyleBackColor = True
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
        Me.Panel2.Controls.Add(Me.btnLoadLog)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(104, 3)
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
        'btnLoadLog
        '
        Me.btnLoadLog.Location = New System.Drawing.Point(11, 2)
        Me.btnLoadLog.Name = "btnLoadLog"
        Me.btnLoadLog.Size = New System.Drawing.Size(90, 21)
        Me.btnLoadLog.TabIndex = 50
        Me.btnLoadLog.Text = "Load Log"
        Me.btnLoadLog.UseVisualStyleBackColor = True
        '
        'saveDialog
        '
        Me.saveDialog.DefaultExt = "wid"
        Me.saveDialog.Filter = "WID Logger File (*.wid)|*.wid|All Files (*.*)|*.*"
        '
        'OpenDialog
        '
        Me.OpenDialog.DefaultExt = "wid"
        Me.OpenDialog.Filter = "WID Logger File (*.wid)|*.wid|All Files (*.*)|*.*"
        '
        'SaveDialog1
        '
        Me.SaveDialog1.DefaultExt = "csv"
        Me.SaveDialog1.Filter = "CSV File (*.csv)|*.csv|All Files (*.*)|*.*"
        '
        'SaveDialog2
        '
        Me.SaveDialog2.DefaultExt = "csv"
        Me.SaveDialog2.Filter = "HTML File (*.html)|*.html|All Files (*.*)|*.*"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(632, 537)
        Me.Controls.Add(Me.TabControl1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMain"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Text = "WIDLogger Log Interpreter"
        Me.TabPage1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage3.ResumeLayout(False)
        Me.SplitContainer3.Panel1.ResumeLayout(False)
        Me.SplitContainer3.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer3.ResumeLayout(False)
        Me.TabControlRecords.ResumeLayout(False)
        Me.TabPage4.ResumeLayout(False)
        Me.TabPage5.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents rtbInterpretText As System.Windows.Forms.RichTextBox
    Friend WithEvents btnInterpretAppend As System.Windows.Forms.Button
    Friend WithEvents btnInterpret As System.Windows.Forms.Button
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents SplitContainer3 As System.Windows.Forms.SplitContainer
    Friend WithEvents TabControlRecords As System.Windows.Forms.TabControl
    Friend WithEvents TabPage4 As System.Windows.Forms.TabPage
    Private WithEvents lvLogs As System.Windows.Forms.ListView
    Private WithEvents columnHeader4 As System.Windows.Forms.ColumnHeader
    Private WithEvents columnHeader8 As System.Windows.Forms.ColumnHeader
    Private WithEvents columnHeader5 As System.Windows.Forms.ColumnHeader
    Private WithEvents columnHeader6 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents TabPage5 As System.Windows.Forms.TabPage
    Private WithEvents lvSysEvents As System.Windows.Forms.ListView
    Private WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Private WithEvents ColumnHeader3 As System.Windows.Forms.ColumnHeader
    Private WithEvents ColumnHeader7 As System.Windows.Forms.ColumnHeader
    Private WithEvents ColumnHeader9 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader10 As System.Windows.Forms.ColumnHeader
    Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Private WithEvents lblTimeRemain As System.Windows.Forms.Label
    Private WithEvents lblTransfrRate As System.Windows.Forms.Label
    Private WithEvents lblRecCount As System.Windows.Forms.Label
    Private WithEvents lbl1 As System.Windows.Forms.Label
    Private WithEvents lbl2 As System.Windows.Forms.Label
    Private WithEvents label2 As System.Windows.Forms.Label
    Private WithEvents pBTransfer As System.Windows.Forms.ProgressBar
    Private WithEvents btnSaveLog As System.Windows.Forms.Button
    Private WithEvents btnClearLog As System.Windows.Forms.Button
    Private WithEvents btnLoadLog As System.Windows.Forms.Button
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Private WithEvents lvTimeline As System.Windows.Forms.ListView
    Private WithEvents ColumnHeader11 As System.Windows.Forms.ColumnHeader
    Private WithEvents ColumnHeader12 As System.Windows.Forms.ColumnHeader
    Private WithEvents ColumnHeader13 As System.Windows.Forms.ColumnHeader
    Private WithEvents ColumnHeader14 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader15 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader16 As System.Windows.Forms.ColumnHeader
    Private WithEvents saveDialog As System.Windows.Forms.SaveFileDialog
    Friend WithEvents OpenDialog As System.Windows.Forms.OpenFileDialog
    Friend WithEvents btnSaveTimelines As System.Windows.Forms.Button
    Private WithEvents SaveDialog1 As System.Windows.Forms.SaveFileDialog
    Friend WithEvents btnSaveAll As System.Windows.Forms.Button
    Private WithEvents SaveDialog2 As System.Windows.Forms.SaveFileDialog

End Class
