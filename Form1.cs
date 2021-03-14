using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace pókchat
{
    public partial class Form1 : Form
    {
        //Declare Variables
        Socket sck;
        EndPoint epLocal, epRemote;
        public Form1()
        {
            InitializeComponent();
        //Declare Sockets
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            //tbClientIP.PlaceholderText = GetLocalIP();
            
        }

        private string GetLocalIP()
        {
        //Get Local Ip (For ease of Use)
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork);
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";
        }


        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {
                int size = sck.EndReceiveFrom(aResult, ref epRemote);
                if (size > 0)
                {
                    //detecting messages and showing
                    byte[] receivedData = new byte[1464];
                    receivedData = (byte[])aResult.AsyncState;
                    ASCIIEncoding eEncoding = new ASCIIEncoding();
                    string receivedMessage = eEncoding.GetString(receivedData);

                    lbChat.Items.Add("Conferee: " + receivedMessage);

                }
                byte[] buffer = new byte[1500];

                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btConnect_Click(object sender, EventArgs e)
        {//Connecting
            try
            {
                epLocal = new IPEndPoint(IPAddress.Parse(tbClientIP.Text), Convert.ToInt32(tbClientPort.Text));
                sck.Bind(epLocal);

                epRemote = new IPEndPoint(IPAddress.Parse(tbRecievingIP.Text), Convert.ToInt32(tbRecievingPort.Text));
                    sck.Connect(epRemote);

                byte[] buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);

                btConnect.Enabled = false;
                btSend.Enabled = true;
                tbChat.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString()); ;
            }
        }

        private void btSend_Click(object sender, EventArgs e)
        {//sending
            try
            {
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                byte[] msg = new byte[1500];
                msg = enc.GetBytes(tbChat.Text);

                sck.Send(msg);
                lbChat.Items.Add("Client: " + tbChat.Text);
                tbChat.Clear();
                tbChat.Focus();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btConnect.Enabled = true;
            //btSend.Enabled = false;
            btSend.BackColor = Color.FromArgb(44, 47, 51);
            btConnect.BackColor = Color.FromArgb(44, 47, 51);
            List<Color> lstColour = new List<Color>();
            foreach (Control c in groupBox1.Controls)
                lstColour.Add(c.ForeColor);

            groupBox1.ForeColor = Color.White; //the colour you prefer for the text

            int index = 0;
            foreach (Control c in groupBox1.Controls)
            {
                c.ForeColor = lstColour[index];
                index++;
            }

            List<Color> lstColour2 = new List<Color>();
            foreach (Control c in groupBox2.Controls)
                lstColour2.Add(c.ForeColor);

            groupBox2.ForeColor = Color.White; //the colour you prefer for the text

            int index2 = 0;
            foreach (Control c in groupBox1.Controls)
            {
                c.ForeColor = lstColour[index2];
                index++;
            }
            //MessageBox.Show("Pókchat (Spiderchat) V1.0 Unencrypted, Made by crinobaka and CaptainK", "Credits");
        }
    }
}
