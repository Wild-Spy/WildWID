using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NRF24L01_PCBridge
{
    public partial class frmMain : Form
    {

        List<string> WIDIDs = new List<string>();
        List<int> WIDPers = new List<int>();
        List<int> WIDPhas = new List<int>();

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();

            int i;

            ser.ReadBufferSize = 1024 * 1024 * 32 / 8;
            //ser.BaudRate = 76800;

            for (i = 0; i < ports.Length; i++)
            {
                cbPorts.Items.Add(ports[i]);
            }
            cbPorts.SelectedIndex = 0;
            if (cbPorts.Text != "")
            {
                ser.PortName = cbPorts.Text;
            }

        }

        private void btnCon_Click(object sender, EventArgs e)
        {
            if (ser.IsOpen)
            {
                try
                {
                    ser.Close();
                    cbPorts.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                if (!ser.IsOpen)
                {
                    btnCon.Text = "Connect";
                    lblStatus.Text = "Not Connected";
                    //T1.Enabled = false;
                    //btnReadLog.Enabled = false;
                    //btnReadLogFast.Enabled = false;
                    //btnClearLog.Enabled = false;
                }
            }
            else
            {
                try
                {
                    ser.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                ser.ReadTimeout = 6000;
                if (ser.IsOpen)
                {
                    //if (CheckRx("Hardware Initialised.\r\n", 10) == 0)
                    //{
                        btnCon.Text = "Disconnect";
                        lblStatus.Text = "Asleep";
                        cbPorts.Enabled = false;
                        lblMsgSent.BackColor = frmMain.ActiveForm.BackColor;
                    //}
                    //else
                    //{
                    //    MessageBox.Show("Could not connect to target!");
                    //    ser.Close();
                    //}
                    //T1.Enabled = true;
                    //btnReadLog.Enabled = true;
                    //btnReadLogFast.Enabled = true;
                    //btnClearLog.Enabled = true;
                }
            }
        }

        private void btnAddMSG_Click(object sender, EventArgs e)
        {
            lstIDs.Items.Add(txtIDAdd.Text);
        }

        private void btnRemMSG_Click(object sender, EventArgs e)
        {
            if (lstIDs.SelectedItems.Count > 0)
                lstIDs.Items.RemoveAt(lstIDs.SelectedIndex);
        }

        private void lstIDs_Click(object sender, EventArgs e)
        {
            if (chkSendMSG.Checked == true)
            {
                SendRFMessage(lstIDs.SelectedItem.ToString());
            }
        }

        private bool SendHex(string strHex)
        {
            byte[] sendBytes;
            int len, i, a;
            byte b;
            string tmpStr, Hex;

            len = strHex.Length;

            if (len % 2 != 0)
            {
                strHex = string.Concat("0", strHex);
                len += 1;
            }

            sendBytes = new byte[len / 2];

            //tmpStr = "";

            for (i = 0; i < len; i+=2)
            //for (i = len-2; i >= 0 ; i -= 2)
            {
                Hex = strHex.Substring(i, 2);
                a = Convert.ToInt32(Hex, 16);
                b = Convert.ToByte(a);
                sendBytes[(len / 2)-(i / 2)-1] = b;
                //tmpStr = string.Concat(tmpStr, Convert.ToChar(c));
            }

            ser.Write(sendBytes, 0, len/2);

            return true;

        }

        private void SendRFMessage(string data)
        {
            lblMsgSent.BackColor = Color.Orange;
            //lblMsgSent.Text = "Sending Message...";
            //string must be of correct length!!
            if (data.Length != 8)
            {
                lblMsgSent.BackColor = Color.Red;
                //lblMsgSent.Text = "Error Sending!";
                return;
            }

            if (allignCommands() == false)
            {
                MessageBox.Show("Could not capture device!");
                lblMsgSent.BackColor = Color.Red;
                //lblMsgSent.Text = "Error Sending!";
                return;
            }

            System.Threading.Thread.Sleep(25);

            if (SendMessageWithCheck("T",">", 1) == 0)
            {
                SendHex(data); 
            }

            if (CheckRx("\r\n", 1) == 0)
            {
                lblMsgSent.BackColor = Color.LightGreen;
                //lblMsgSent.Text = string.Concat("Sent: ", data);
            }
            else
            {
                lblMsgSent.BackColor = Color.Red;
                //lblMsgSent.Text = "Error Sending!";
            }
        }

        private bool allignCommands()
        {
            DateTime startTime = DateTime.Now;

            string a = "";
            do {
                ser.Write("s");
                if (ser.BytesToRead >= 1)
                    a = ser.ReadExisting();
                Application.DoEvents();
            } while (!a.EndsWith("!") && (DateTime.Now - startTime).Seconds < 20);

            if (a.EndsWith("!"))
                return true;
            else
                return false;
        }

        private int SendMessageWithCheck(string Command, string expected, int timeout)
        {
            //clear rx buffer
            ser.ReadExisting();
            //send command
            ser.Write(Command);
            //check what's received
            return CheckRx(expected, timeout);
        }

        private int CheckRx(string expected, int timeout)
        {
            DateTime startTime;

            startTime = DateTime.Now;

            while (ser.BytesToRead < expected.Length && (DateTime.Now - startTime).Seconds < timeout)
                Application.DoEvents();


            if ((DateTime.Now - startTime).Seconds < timeout)
            {

                try
                {
                    string a = ser.ReadExisting();
                    if (a == expected)
                    {
                        return 0;
                    }
                    else
                    {
                        //MessageBox.Show("Expected string not found");
                        return 3;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return 2;
                }

            }
            else
            {
                //MessageBox.Show("Operation Timed Out!");
                return 1;
            }
        }

        private void cbPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPorts.Text != "")
            {
                ser.PortName = cbPorts.Text;
            }
        }

        private void btnAddWID_Click(object sender, EventArgs e)
        {
            ListViewItem tI = new ListViewItem();
            tI.Text  = txtWIDID.Text;
            tI.SubItems.Add(txtWIDPer.Text);
            tI.SubItems.Add(txtWIDPhase.Text);

            lvWIDs.Items.Add(tI);
        }

        private void btnRemWID_Click(object sender, EventArgs e)
        {
            if (lvWIDs.SelectedIndices.Count > 0)
            {
                lvWIDs.Items.RemoveAt(lvWIDs.SelectedIndices[0]);
            }
        }

        private void chkStart_CheckedChanged(object sender, EventArgs e)
        {
            if (chkStart.Checked == true)
            {
                ListViewItem tI = new ListViewItem();
                int i;

                WIDIDs.Clear();
                WIDPers.Clear();
                WIDPhas.Clear();

                for (i = 0; i < lvWIDs.Items.Count; i++)
                {
                    tI = lvWIDs.Items[i];

                    WIDIDs.Add(tI.Text);
                    WIDPers.Add(Convert.ToInt32(Convert.ToDecimal(tI.SubItems[1].Text) * 1000));
                    WIDPhas.Add(Convert.ToInt32(Convert.ToDecimal(tI.SubItems[2].Text) * 1000));
                }

                //bgwWIDs.RunWorkerAsync();
                bgwWIDs2.RunWorkerAsync();
            }
        }

        private void bgwWIDs_DoWork(object sender, DoWorkEventArgs e)
        {
            DateTime startTime = DateTime.Now;
            TimeSpan TimeSinceStart;
            long Tms;
            long TxCount = 0;

            int i;

            do
            {
                TimeSinceStart = DateTime.Now - startTime;
                Tms = (Convert.ToInt64(Math.Floor(TimeSinceStart.TotalMilliseconds / 50))) * 50;

                for (i = 0; i < WIDIDs.Count; i++)
                {

                    if ((Tms - (WIDPhas[i])) % (WIDPers[i]) == 0)
                    {
                        SendRFMessage(WIDIDs[i]);
                        TxCount++;
                    }
                }
                System.Threading.Thread.Sleep(50);
            } while (chkStart.Checked == true);
        }

        private void btnWIDHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Formats:\r\n  ID: 8 digit hexadecimal number eg. ABCD1234\r\n  Period: Period at which to transmit this ID in seconds.  Granularity is 50ms (0.05s)\r\n  Phase: Delay from time zero when this ID will start being transmitted.  Granularity of 50ms (0.05s)");
        }

        private void bgwWIDs2_DoWork(object sender, DoWorkEventArgs e)
        {
            DateTime startTime;
            TimeSpan TimeSinceStart;
            long Tms;
            long TxCount = 0;

            int i;

            do 
            {
                startTime = DateTime.Now;

                for (i = 0; i < WIDIDs.Count; i++)
                {
                    do 
                    {
                        TimeSinceStart = DateTime.Now - startTime;
                    } while (TimeSinceStart.Milliseconds < WIDPhas[i]);
                    SendRFMessage(WIDIDs[i]);
                }
                //do
                //{
                //    TimeSinceStart = DateTime.Now - startTime;
                //} while (TimeSinceStart.Milliseconds < WIDPers[0]);
                System.Threading.Thread.Sleep(10);
            } while (chkStart.Checked == true);

            /*do
            {
                TimeSinceStart = DateTime.Now - startTime;
                Tms = (Convert.ToInt64(Math.Floor(TimeSinceStart.TotalMilliseconds / 50))) * 50;

                for (i = 0; i < WIDIDs.Count; i++)
                {

                    if ((Tms - (WIDPhas[i])) % (WIDPers[i]) == 0)
                    {
                        SendRFMessage(WIDIDs[i]);
                        TxCount++;
                    }
                }
                System.Threading.Thread.Sleep(50);
            } while (chkStart.Checked == true);*/
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                do
                {
                    SendRFMessage("AAAA0000");
                    SendRFMessage("AAAA0001");
                } while (checkBox1.Checked == true);
            }
        }

    }



        
}
