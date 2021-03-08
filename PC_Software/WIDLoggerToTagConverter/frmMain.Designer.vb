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
        Me.btnLoadFolder = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.lstCSVFiles = New System.Windows.Forms.ListBox()
        Me.dlgFolderBrowse = New System.Windows.Forms.FolderBrowserDialog()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.lstTags = New System.Windows.Forms.CheckedListBox()
        Me.btnFindUniqueTags = New System.Windows.Forms.Button()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.pb1 = New System.Windows.Forms.ProgressBar()
        Me.btnOutputTagFiles = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnLoadFolder
        '
        Me.btnLoadFolder.Location = New System.Drawing.Point(77, 19)
        Me.btnLoadFolder.Name = "btnLoadFolder"
        Me.btnLoadFolder.Size = New System.Drawing.Size(75, 23)
        Me.btnLoadFolder.TabIndex = 0
        Me.btnLoadFolder.Text = "Load"
        Me.btnLoadFolder.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.lstCSVFiles)
        Me.GroupBox1.Controls.Add(Me.btnLoadFolder)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(234, 378)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "1. Select Input Files Folder"
        '
        'lstCSVFiles
        '
        Me.lstCSVFiles.FormattingEnabled = True
        Me.lstCSVFiles.HorizontalScrollbar = True
        Me.lstCSVFiles.Location = New System.Drawing.Point(6, 48)
        Me.lstCSVFiles.Name = "lstCSVFiles"
        Me.lstCSVFiles.Size = New System.Drawing.Size(222, 316)
        Me.lstCSVFiles.TabIndex = 2
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.lstTags)
        Me.GroupBox2.Controls.Add(Me.btnFindUniqueTags)
        Me.GroupBox2.Enabled = False
        Me.GroupBox2.Location = New System.Drawing.Point(252, 12)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(234, 378)
        Me.GroupBox2.TabIndex = 2
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "2. Select Tags For Output"
        '
        'lstTags
        '
        Me.lstTags.FormattingEnabled = True
        Me.lstTags.Location = New System.Drawing.Point(6, 48)
        Me.lstTags.Name = "lstTags"
        Me.lstTags.Size = New System.Drawing.Size(222, 319)
        Me.lstTags.Sorted = True
        Me.lstTags.TabIndex = 3
        '
        'btnFindUniqueTags
        '
        Me.btnFindUniqueTags.Location = New System.Drawing.Point(56, 19)
        Me.btnFindUniqueTags.Name = "btnFindUniqueTags"
        Me.btnFindUniqueTags.Size = New System.Drawing.Size(115, 23)
        Me.btnFindUniqueTags.TabIndex = 0
        Me.btnFindUniqueTags.Text = "Find Unique Tags"
        Me.btnFindUniqueTags.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.pb1)
        Me.GroupBox3.Controls.Add(Me.btnOutputTagFiles)
        Me.GroupBox3.Enabled = False
        Me.GroupBox3.Location = New System.Drawing.Point(12, 396)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(473, 80)
        Me.GroupBox3.TabIndex = 3
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "3. Output to Tag Files"
        '
        'pb1
        '
        Me.pb1.Location = New System.Drawing.Point(6, 48)
        Me.pb1.Name = "pb1"
        Me.pb1.Size = New System.Drawing.Size(461, 23)
        Me.pb1.TabIndex = 1
        Me.pb1.Visible = False
        '
        'btnOutputTagFiles
        '
        Me.btnOutputTagFiles.Location = New System.Drawing.Point(6, 19)
        Me.btnOutputTagFiles.Name = "btnOutputTagFiles"
        Me.btnOutputTagFiles.Size = New System.Drawing.Size(461, 23)
        Me.btnOutputTagFiles.TabIndex = 0
        Me.btnOutputTagFiles.Text = "Output Selected Tag Files"
        Me.btnOutputTagFiles.UseVisualStyleBackColor = True
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(497, 485)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.Name = "frmMain"
        Me.Text = "Wild Spy WID LoggerTo Tag Converter"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnLoadFolder As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents lstCSVFiles As System.Windows.Forms.ListBox
    Friend WithEvents dlgFolderBrowse As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents btnFindUniqueTags As System.Windows.Forms.Button
    Friend WithEvents lstTags As System.Windows.Forms.CheckedListBox
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents btnOutputTagFiles As System.Windows.Forms.Button
    Friend WithEvents pb1 As System.Windows.Forms.ProgressBar

End Class
