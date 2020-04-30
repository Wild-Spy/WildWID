<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmProg
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmProg))
        Me.btnRun = New System.Windows.Forms.Button
        Me.txtStdout = New System.Windows.Forms.TextBox
        Me.txtStderr = New System.Windows.Forms.TextBox
        Me.lvOperations = New System.Windows.Forms.ListView
        Me.ColumnHeader1 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader2 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader3 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader4 = New System.Windows.Forms.ColumnHeader
        Me.cbMemory = New System.Windows.Forms.ComboBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.cbOperation = New System.Windows.Forms.ComboBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.txtFileName = New System.Windows.Forms.TextBox
        Me.Label4 = New System.Windows.Forms.Label
        Me.cbFormat = New System.Windows.Forms.ComboBox
        Me.btnAddOperation = New System.Windows.Forms.Button
        Me.rtbFormatInfo = New System.Windows.Forms.RichTextBox
        Me.btnDeleteOperation = New System.Windows.Forms.Button
        Me.Label5 = New System.Windows.Forms.Label
        Me.cbProgrammer = New System.Windows.Forms.ComboBox
        Me.Label6 = New System.Windows.Forms.Label
        Me.cbChip = New System.Windows.Forms.ComboBox
        Me.Label7 = New System.Windows.Forms.Label
        Me.txtPort = New System.Windows.Forms.TextBox
        Me.SFD = New System.Windows.Forms.SaveFileDialog
        Me.OFD = New System.Windows.Forms.OpenFileDialog
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.OpenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.txtWorkingDir = New System.Windows.Forms.TextBox
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.Label10 = New System.Windows.Forms.Label
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnRun
        '
        Me.btnRun.Location = New System.Drawing.Point(377, 228)
        Me.btnRun.Name = "btnRun"
        Me.btnRun.Size = New System.Drawing.Size(131, 23)
        Me.btnRun.TabIndex = 0
        Me.btnRun.Text = "Run"
        Me.btnRun.UseVisualStyleBackColor = True
        '
        'txtStdout
        '
        Me.txtStdout.Location = New System.Drawing.Point(12, 309)
        Me.txtStdout.Multiline = True
        Me.txtStdout.Name = "txtStdout"
        Me.txtStdout.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtStdout.Size = New System.Drawing.Size(496, 120)
        Me.txtStdout.TabIndex = 1
        '
        'txtStderr
        '
        Me.txtStderr.Location = New System.Drawing.Point(12, 448)
        Me.txtStderr.Multiline = True
        Me.txtStderr.Name = "txtStderr"
        Me.txtStderr.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtStderr.Size = New System.Drawing.Size(496, 142)
        Me.txtStderr.TabIndex = 2
        '
        'lvOperations
        '
        Me.lvOperations.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3, Me.ColumnHeader4})
        Me.lvOperations.FullRowSelect = True
        Me.lvOperations.GridLines = True
        Me.lvOperations.Location = New System.Drawing.Point(12, 72)
        Me.lvOperations.Name = "lvOperations"
        Me.lvOperations.Size = New System.Drawing.Size(496, 138)
        Me.lvOperations.TabIndex = 3
        Me.lvOperations.UseCompatibleStateImageBehavior = False
        Me.lvOperations.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Memory"
        Me.ColumnHeader1.Width = 137
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Operation"
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "File Name"
        Me.ColumnHeader3.Width = 207
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Text = "File Format"
        Me.ColumnHeader4.Width = 64
        '
        'cbMemory
        '
        Me.cbMemory.FormattingEnabled = True
        Me.cbMemory.Items.AddRange(New Object() {"flash", "eeprom", "fuse0", "fuse1", "fuse2", "fuse3", "fuse4", "fuse5", "lock", "application", "apptable", "boot", "prodsig", "usersig", "signature"})
        Me.cbMemory.Location = New System.Drawing.Point(12, 45)
        Me.cbMemory.Name = "cbMemory"
        Me.cbMemory.Size = New System.Drawing.Size(135, 21)
        Me.cbMemory.TabIndex = 4
        Me.cbMemory.Text = "flash"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(9, 29)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(47, 13)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "Memory:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(150, 29)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(56, 13)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = "Operation:"
        '
        'cbOperation
        '
        Me.cbOperation.FormattingEnabled = True
        Me.cbOperation.Items.AddRange(New Object() {"r", "w", "v"})
        Me.cbOperation.Location = New System.Drawing.Point(153, 45)
        Me.cbOperation.Name = "cbOperation"
        Me.cbOperation.Size = New System.Drawing.Size(53, 21)
        Me.cbOperation.TabIndex = 6
        Me.cbOperation.Text = "w"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(209, 29)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(57, 13)
        Me.Label3.TabIndex = 9
        Me.Label3.Text = "File Name:"
        '
        'txtFileName
        '
        Me.txtFileName.Location = New System.Drawing.Point(212, 46)
        Me.txtFileName.Name = "txtFileName"
        Me.txtFileName.Size = New System.Drawing.Size(144, 20)
        Me.txtFileName.TabIndex = 10
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(359, 29)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(42, 13)
        Me.Label4.TabIndex = 12
        Me.Label4.Text = "Format:"
        '
        'cbFormat
        '
        Me.cbFormat.FormattingEnabled = True
        Me.cbFormat.Items.AddRange(New Object() {"i", "s", "r", "m", "a", "d", "h", "o", "b"})
        Me.cbFormat.Location = New System.Drawing.Point(362, 45)
        Me.cbFormat.Name = "cbFormat"
        Me.cbFormat.Size = New System.Drawing.Size(53, 21)
        Me.cbFormat.TabIndex = 11
        Me.cbFormat.Text = "i"
        '
        'btnAddOperation
        '
        Me.btnAddOperation.Location = New System.Drawing.Point(421, 43)
        Me.btnAddOperation.Name = "btnAddOperation"
        Me.btnAddOperation.Size = New System.Drawing.Size(34, 23)
        Me.btnAddOperation.TabIndex = 13
        Me.btnAddOperation.Text = "Add"
        Me.btnAddOperation.UseVisualStyleBackColor = True
        '
        'rtbFormatInfo
        '
        Me.rtbFormatInfo.Location = New System.Drawing.Point(492, 577)
        Me.rtbFormatInfo.Name = "rtbFormatInfo"
        Me.rtbFormatInfo.Size = New System.Drawing.Size(279, 262)
        Me.rtbFormatInfo.TabIndex = 14
        Me.rtbFormatInfo.Text = resources.GetString("rtbFormatInfo.Text")
        Me.rtbFormatInfo.Visible = False
        '
        'btnDeleteOperation
        '
        Me.btnDeleteOperation.Location = New System.Drawing.Point(461, 43)
        Me.btnDeleteOperation.Name = "btnDeleteOperation"
        Me.btnDeleteOperation.Size = New System.Drawing.Size(47, 23)
        Me.btnDeleteOperation.TabIndex = 15
        Me.btnDeleteOperation.Text = "Delete"
        Me.btnDeleteOperation.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(9, 214)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(66, 13)
        Me.Label5.TabIndex = 17
        Me.Label5.Text = "Programmer:"
        '
        'cbProgrammer
        '
        Me.cbProgrammer.FormattingEnabled = True
        Me.cbProgrammer.Items.AddRange(New Object() {"dragon_pdi", "dragon_dw", "dragon_hvsp", "dragon_pp", "dragon_isp", "dragon_jtag", "avrisp2", "avrispmkII", "ponyser"})
        Me.cbProgrammer.Location = New System.Drawing.Point(12, 230)
        Me.cbProgrammer.Name = "cbProgrammer"
        Me.cbProgrammer.Size = New System.Drawing.Size(135, 21)
        Me.cbProgrammer.TabIndex = 16
        Me.cbProgrammer.Text = "dragon_pdi"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(150, 214)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(31, 13)
        Me.Label6.TabIndex = 19
        Me.Label6.Text = "Chip:"
        '
        'cbChip
        '
        Me.cbChip.FormattingEnabled = True
        Me.cbChip.Items.AddRange(New Object() {"ATXMEGA32D4", "ATXMEGA32A4", "ATXMEGA16A4", "ATMEGA328P", "ATMEGA168", "ATMEGA88", ""})
        Me.cbChip.Location = New System.Drawing.Point(153, 230)
        Me.cbChip.Name = "cbChip"
        Me.cbChip.Size = New System.Drawing.Size(135, 21)
        Me.cbChip.TabIndex = 18
        Me.cbChip.Text = "x32d4"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(291, 214)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(29, 13)
        Me.Label7.TabIndex = 21
        Me.Label7.Text = "Port:"
        '
        'txtPort
        '
        Me.txtPort.Location = New System.Drawing.Point(294, 230)
        Me.txtPort.Name = "txtPort"
        Me.txtPort.Size = New System.Drawing.Size(77, 20)
        Me.txtPort.TabIndex = 22
        Me.txtPort.Text = "usb"
        '
        'SFD
        '
        Me.SFD.Filter = "WS Production Prog File(*.wspp)|*.wspp|All Files (*.*)|*.*"
        '
        'OFD
        '
        Me.OFD.Filter = "WS Production Prog File(*.wspp)|*.wspp|All Files (*.*)|*.*"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(520, 24)
        Me.MenuStrip1.TabIndex = 23
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SaveToolStripMenuItem, Me.OpenToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'SaveToolStripMenuItem
        '
        Me.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem"
        Me.SaveToolStripMenuItem.Size = New System.Drawing.Size(126, 22)
        Me.SaveToolStripMenuItem.Text = "Save As ..."
        '
        'OpenToolStripMenuItem
        '
        Me.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem"
        Me.OpenToolStripMenuItem.Size = New System.Drawing.Size(126, 22)
        Me.OpenToolStripMenuItem.Text = "Open ..."
        '
        'txtWorkingDir
        '
        Me.txtWorkingDir.Location = New System.Drawing.Point(12, 270)
        Me.txtWorkingDir.Name = "txtWorkingDir"
        Me.txtWorkingDir.Size = New System.Drawing.Size(359, 20)
        Me.txtWorkingDir.TabIndex = 25
        Me.txtWorkingDir.Text = "D:\Wild Spy\Projects\WS004 - Wireless ID\8 - Production\2 - Programming\New folde" & _
            "r"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(9, 254)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(95, 13)
        Me.Label8.TabIndex = 24
        Me.Label8.Text = "Working Directory:"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(9, 432)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(93, 13)
        Me.Label9.TabIndex = 26
        Me.Label9.Text = "AVRDude Output:"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(12, 293)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(94, 13)
        Me.Label10.TabIndex = 27
        Me.Label10.Text = "Operation Results:"
        '
        'FrmProg
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(520, 602)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.txtWorkingDir)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.txtPort)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.cbChip)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.cbProgrammer)
        Me.Controls.Add(Me.btnDeleteOperation)
        Me.Controls.Add(Me.rtbFormatInfo)
        Me.Controls.Add(Me.btnAddOperation)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.cbFormat)
        Me.Controls.Add(Me.txtFileName)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.cbOperation)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cbMemory)
        Me.Controls.Add(Me.lvOperations)
        Me.Controls.Add(Me.txtStderr)
        Me.Controls.Add(Me.txtStdout)
        Me.Controls.Add(Me.btnRun)
        Me.Controls.Add(Me.MenuStrip1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.MaximizeBox = False
        Me.Name = "FrmProg"
        Me.Text = "AVR Programmer"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnRun As System.Windows.Forms.Button
    Friend WithEvents txtStdout As System.Windows.Forms.TextBox
    Friend WithEvents txtStderr As System.Windows.Forms.TextBox
    Friend WithEvents lvOperations As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader4 As System.Windows.Forms.ColumnHeader
    Friend WithEvents cbMemory As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents cbOperation As System.Windows.Forms.ComboBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtFileName As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents cbFormat As System.Windows.Forms.ComboBox
    Friend WithEvents btnAddOperation As System.Windows.Forms.Button
    Friend WithEvents rtbFormatInfo As System.Windows.Forms.RichTextBox
    Friend WithEvents btnDeleteOperation As System.Windows.Forms.Button
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents cbProgrammer As System.Windows.Forms.ComboBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents cbChip As System.Windows.Forms.ComboBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents txtPort As System.Windows.Forms.TextBox
    Friend WithEvents SFD As System.Windows.Forms.SaveFileDialog
    Friend WithEvents OFD As System.Windows.Forms.OpenFileDialog
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents txtWorkingDir As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label

End Class
