namespace NRF24L01_PCBridge
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "AAAA0000",
            "10",
            "0"}, -1);
            this.ser = new System.IO.Ports.SerialPort(this.components);
            this.cbPorts = new System.Windows.Forms.ComboBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCon = new System.Windows.Forms.Button();
            this.lstIDs = new System.Windows.Forms.ListBox();
            this.btnAddMSG = new System.Windows.Forms.Button();
            this.btnRemMSG = new System.Windows.Forms.Button();
            this.chkSendMSG = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtIDAdd = new System.Windows.Forms.TextBox();
            this.lblMsgSent = new System.Windows.Forms.Label();
            this.lvWIDs = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.chkStart = new System.Windows.Forms.CheckBox();
            this.btnAddWID = new System.Windows.Forms.Button();
            this.btnRemWID = new System.Windows.Forms.Button();
            this.txtWIDID = new System.Windows.Forms.TextBox();
            this.txtWIDPer = new System.Windows.Forms.TextBox();
            this.txtWIDPhase = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnWIDHelp = new System.Windows.Forms.Button();
            this.bgwWIDs = new System.ComponentModel.BackgroundWorker();
            this.bgwWIDs2 = new System.ComponentModel.BackgroundWorker();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ser
            // 
            this.ser.BaudRate = 19200;
            this.ser.ReadTimeout = 1000;
            // 
            // cbPorts
            // 
            this.cbPorts.FormattingEnabled = true;
            this.cbPorts.Location = new System.Drawing.Point(92, 12);
            this.cbPorts.Name = "cbPorts";
            this.cbPorts.Size = new System.Drawing.Size(121, 21);
            this.cbPorts.TabIndex = 43;
            this.cbPorts.SelectedIndexChanged += new System.EventHandler(this.cbPorts_SelectedIndexChanged);
            // 
            // lblStatus
            // 
            this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblStatus.Location = new System.Drawing.Point(258, 17);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(120, 15);
            this.lblStatus.TabIndex = 42;
            this.lblStatus.Text = "Not Connected";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(219, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 41;
            this.label1.Text = "Status:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnCon
            // 
            this.btnCon.Location = new System.Drawing.Point(11, 12);
            this.btnCon.Name = "btnCon";
            this.btnCon.Size = new System.Drawing.Size(75, 23);
            this.btnCon.TabIndex = 40;
            this.btnCon.Text = "Connect";
            this.btnCon.UseVisualStyleBackColor = true;
            this.btnCon.Click += new System.EventHandler(this.btnCon_Click);
            // 
            // lstIDs
            // 
            this.lstIDs.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstIDs.FormattingEnabled = true;
            this.lstIDs.ItemHeight = 16;
            this.lstIDs.Items.AddRange(new object[] {
            "AAAA0001",
            "AAAA0002",
            "AAAA0003",
            "AAAA0004"});
            this.lstIDs.Location = new System.Drawing.Point(0, 0);
            this.lstIDs.Name = "lstIDs";
            this.lstIDs.Size = new System.Drawing.Size(99, 132);
            this.lstIDs.TabIndex = 44;
            this.lstIDs.Click += new System.EventHandler(this.lstIDs_Click);
            // 
            // btnAddMSG
            // 
            this.btnAddMSG.Location = new System.Drawing.Point(1, 138);
            this.btnAddMSG.Name = "btnAddMSG";
            this.btnAddMSG.Size = new System.Drawing.Size(22, 23);
            this.btnAddMSG.TabIndex = 45;
            this.btnAddMSG.Text = "+";
            this.btnAddMSG.UseVisualStyleBackColor = true;
            this.btnAddMSG.Click += new System.EventHandler(this.btnAddMSG_Click);
            // 
            // btnRemMSG
            // 
            this.btnRemMSG.Location = new System.Drawing.Point(77, 138);
            this.btnRemMSG.Name = "btnRemMSG";
            this.btnRemMSG.Size = new System.Drawing.Size(22, 23);
            this.btnRemMSG.TabIndex = 46;
            this.btnRemMSG.Text = "-";
            this.btnRemMSG.UseVisualStyleBackColor = true;
            this.btnRemMSG.Click += new System.EventHandler(this.btnRemMSG_Click);
            // 
            // chkSendMSG
            // 
            this.chkSendMSG.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkSendMSG.AutoSize = true;
            this.chkSendMSG.Location = new System.Drawing.Point(29, 138);
            this.chkSendMSG.Name = "chkSendMSG";
            this.chkSendMSG.Size = new System.Drawing.Size(42, 23);
            this.chkSendMSG.TabIndex = 47;
            this.chkSendMSG.Text = "Send";
            this.chkSendMSG.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtIDAdd);
            this.panel1.Controls.Add(this.lstIDs);
            this.panel1.Controls.Add(this.chkSendMSG);
            this.panel1.Controls.Add(this.btnAddMSG);
            this.panel1.Controls.Add(this.btnRemMSG);
            this.panel1.Location = new System.Drawing.Point(12, 41);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(100, 191);
            this.panel1.TabIndex = 48;
            // 
            // txtIDAdd
            // 
            this.txtIDAdd.Location = new System.Drawing.Point(3, 167);
            this.txtIDAdd.Name = "txtIDAdd";
            this.txtIDAdd.Size = new System.Drawing.Size(94, 20);
            this.txtIDAdd.TabIndex = 49;
            this.txtIDAdd.Text = "AAAA0000";
            // 
            // lblMsgSent
            // 
            this.lblMsgSent.AutoSize = true;
            this.lblMsgSent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblMsgSent.Location = new System.Drawing.Point(258, 41);
            this.lblMsgSent.Name = "lblMsgSent";
            this.lblMsgSent.Size = new System.Drawing.Size(72, 15);
            this.lblMsgSent.TabIndex = 49;
            this.lblMsgSent.Text = "Mesage Sent";
            // 
            // lvWIDs
            // 
            this.lvWIDs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvWIDs.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvWIDs.FullRowSelect = true;
            this.lvWIDs.GridLines = true;
            this.lvWIDs.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
            this.lvWIDs.LabelEdit = true;
            this.lvWIDs.Location = new System.Drawing.Point(3, 2);
            this.lvWIDs.MultiSelect = false;
            this.lvWIDs.Name = "lvWIDs";
            this.lvWIDs.Size = new System.Drawing.Size(230, 127);
            this.lvWIDs.TabIndex = 50;
            this.lvWIDs.UseCompatibleStateImageBehavior = false;
            this.lvWIDs.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "ID";
            this.columnHeader1.Width = 96;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Period";
            this.columnHeader2.Width = 68;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Phase";
            // 
            // chkStart
            // 
            this.chkStart.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkStart.AutoSize = true;
            this.chkStart.Location = new System.Drawing.Point(59, 135);
            this.chkStart.Name = "chkStart";
            this.chkStart.Size = new System.Drawing.Size(39, 23);
            this.chkStart.TabIndex = 53;
            this.chkStart.Text = "Start";
            this.chkStart.UseVisualStyleBackColor = true;
            this.chkStart.CheckedChanged += new System.EventHandler(this.chkStart_CheckedChanged);
            // 
            // btnAddWID
            // 
            this.btnAddWID.Location = new System.Drawing.Point(3, 135);
            this.btnAddWID.Name = "btnAddWID";
            this.btnAddWID.Size = new System.Drawing.Size(22, 23);
            this.btnAddWID.TabIndex = 51;
            this.btnAddWID.Text = "+";
            this.btnAddWID.UseVisualStyleBackColor = true;
            this.btnAddWID.Click += new System.EventHandler(this.btnAddWID_Click);
            // 
            // btnRemWID
            // 
            this.btnRemWID.Location = new System.Drawing.Point(211, 135);
            this.btnRemWID.Name = "btnRemWID";
            this.btnRemWID.Size = new System.Drawing.Size(22, 23);
            this.btnRemWID.TabIndex = 52;
            this.btnRemWID.Text = "-";
            this.btnRemWID.UseVisualStyleBackColor = true;
            this.btnRemWID.Click += new System.EventHandler(this.btnRemWID_Click);
            // 
            // txtWIDID
            // 
            this.txtWIDID.Location = new System.Drawing.Point(3, 164);
            this.txtWIDID.Name = "txtWIDID";
            this.txtWIDID.Size = new System.Drawing.Size(95, 20);
            this.txtWIDID.TabIndex = 54;
            this.txtWIDID.Text = "AAAA0001";
            // 
            // txtWIDPer
            // 
            this.txtWIDPer.Location = new System.Drawing.Point(104, 164);
            this.txtWIDPer.Name = "txtWIDPer";
            this.txtWIDPer.Size = new System.Drawing.Size(61, 20);
            this.txtWIDPer.TabIndex = 55;
            this.txtWIDPer.Text = "5";
            // 
            // txtWIDPhase
            // 
            this.txtWIDPhase.Location = new System.Drawing.Point(171, 164);
            this.txtWIDPhase.Name = "txtWIDPhase";
            this.txtWIDPhase.Size = new System.Drawing.Size(62, 20);
            this.txtWIDPhase.TabIndex = 56;
            this.txtWIDPhase.Text = "0.1";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.checkBox1);
            this.panel2.Controls.Add(this.btnWIDHelp);
            this.panel2.Controls.Add(this.txtWIDID);
            this.panel2.Controls.Add(this.txtWIDPhase);
            this.panel2.Controls.Add(this.lvWIDs);
            this.panel2.Controls.Add(this.txtWIDPer);
            this.panel2.Controls.Add(this.btnRemWID);
            this.panel2.Controls.Add(this.btnAddWID);
            this.panel2.Controls.Add(this.chkStart);
            this.panel2.Location = new System.Drawing.Point(118, 59);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(236, 187);
            this.panel2.TabIndex = 57;
            // 
            // btnWIDHelp
            // 
            this.btnWIDHelp.Location = new System.Drawing.Point(162, 135);
            this.btnWIDHelp.Name = "btnWIDHelp";
            this.btnWIDHelp.Size = new System.Drawing.Size(15, 23);
            this.btnWIDHelp.TabIndex = 57;
            this.btnWIDHelp.Text = "?";
            this.btnWIDHelp.UseVisualStyleBackColor = true;
            this.btnWIDHelp.Click += new System.EventHandler(this.btnWIDHelp_Click);
            // 
            // bgwWIDs
            // 
            this.bgwWIDs.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwWIDs_DoWork);
            // 
            // bgwWIDs2
            // 
            this.bgwWIDs2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwWIDs2_DoWork);
            // 
            // checkBox1
            // 
            this.checkBox1.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(102, 135);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(39, 23);
            this.checkBox1.TabIndex = 58;
            this.checkBox1.Text = "Start";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(383, 261);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.lblMsgSent);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cbPorts);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCon);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmMain";
            this.Text = "NRF24L01-PC Bridge";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.IO.Ports.SerialPort ser;
        private System.Windows.Forms.ComboBox cbPorts;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCon;
        private System.Windows.Forms.ListBox lstIDs;
        private System.Windows.Forms.Button btnAddMSG;
        private System.Windows.Forms.Button btnRemMSG;
        private System.Windows.Forms.CheckBox chkSendMSG;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtIDAdd;
        private System.Windows.Forms.Label lblMsgSent;
        private System.Windows.Forms.ListView lvWIDs;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.CheckBox chkStart;
        private System.Windows.Forms.Button btnAddWID;
        private System.Windows.Forms.Button btnRemWID;
        private System.Windows.Forms.TextBox txtWIDID;
        private System.Windows.Forms.TextBox txtWIDPer;
        private System.Windows.Forms.TextBox txtWIDPhase;
        private System.Windows.Forms.Panel panel2;
        private System.ComponentModel.BackgroundWorker bgwWIDs;
        private System.Windows.Forms.Button btnWIDHelp;
        private System.ComponentModel.BackgroundWorker bgwWIDs2;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

