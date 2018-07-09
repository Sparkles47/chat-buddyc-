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

namespace Chat_Client_App
{
    public partial class Form1 : Form
    {
        Socket sck;
        EndPoint epLocal, epRemote;
        public Form1()
        {
            InitializeComponent();

            // set up socket on initialization
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            // get own IP
            textLocalIP.Text = GetLocalIP();
            textFriendsIP.Text = GetLocalIP();
        }

        // Return your own IP 
        private string GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
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
                
                // check if theres actually information 
                if (size > 0) 
                { 
                    // used to help us on getting the data    
                    byte[] receivedData = new byte[1464]; 
                
                    // getting the message data  
                    receivedData = (byte[])aResult.AsyncState;  
                
                    // converts message data byte array to string 
                    ASCIIEncoding eEncoding = new ASCIIEncoding();   
                    string receivedMessage = eEncoding.GetString(receivedData);  
                
                    // adding Message to the listbox 
                    listMessage.Items.Add("Friend: << " + receivedMessage);     
                }

                // starts to listen the socket again  
                byte[] buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }  
            catch (Exception exp)
            {    
                MessageBox.Show(exp.ToString());
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // binding socket (of your ip address) 
                epLocal = new IPEndPoint(IPAddress.Parse(textLocalIP.Text),
                                        Convert.ToInt32(textLocalPORT.Text));
                sck.Bind(epLocal);

                // connect to remote IP and port
                epRemote = new IPEndPoint(IPAddress.Parse(textFriendsIP.Text),
                                         Convert.ToInt32(textFriendsPORT.Text));
                sck.Connect(epRemote);

                // starts to listen to an specific port
                byte[] buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);

                // release button to send message    
                button1.Text = "Connected"; 
                button1.BackColor = Color.Green; //
                button1.Enabled = false;
                button2.Enabled = true;
                textMessage.Focus();
                button1.Enabled = true; //
                //button1.MouseHover += button1.Text.ToUpper(kml);
                
                textLocalIP.BackColor = Color.GreenYellow;
                textLocalPORT.BackColor = Color.GreenYellow;
                textFriendsIP.BackColor = Color.GreenYellow;
                textFriendsPORT.BackColor = Color.GreenYellow;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            try
            { 
                // converts from string to byte[]  
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                byte[] msg = new byte[1500];
                msg = enc.GetBytes(textMessage.Text);

                // sending the message      
                sck.Send(msg);

                // add to listbox 
                listMessage.Items.Add("You: >> " + textMessage.Text);

                // clear txtMessage
                textMessage.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            } 
        }

        private void listMessage_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
