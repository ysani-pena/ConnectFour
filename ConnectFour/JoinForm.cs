using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets; 
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ConnectFour
{
    public partial class JoinForm : Form
    {
        public string PlayerName { get; private set; }
        public string PlayerNumber { get; private set; }
        public string ServerIp { get; private set; }
        public int Port { get; private set; }
        private TcpClient? client;

        public JoinForm()
        {
            InitializeComponent();
        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            PlayerName = txtName.Text;
            ServerIp = txtIp.Text;
            PlayerNumber = txtPNumber.Text;

            if (int.TryParse(txtPort.Text, out int port))
            {
                Port = port;

                try
                {
                    client = new TcpClient(ServerIp, Port); // Attempt to connect to the server
                    SendPlayerName(); // Send the player's name after successful connection

                    // Adding a small delay to ensure server processes the name before the number
                    Task.Delay(100).Wait();

                    SendPlayerNumber(); // Send the player's number after successful connection
                    this.DialogResult = DialogResult.OK; // Indicate that the input is valid
                    this.Close(); // Close the join form
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not connect to the server: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Invalid port number.");
            }
        }

        private void SendPlayerName()
        {
            if (client != null && client.Connected)
            {
                var stream = client.GetStream();
                byte[] data = Encoding.ASCII.GetBytes($"NAME:{PlayerName}"); // Encode the player name
                stream.Write(data, 0, data.Length); // Send the player name to the server
                stream.Flush(); // Ensure the data is sent immediately
            }
        }

        private void SendPlayerNumber()
        {
            if (client != null && client.Connected)
            {
                var stream = client.GetStream();
                byte[] data = Encoding.ASCII.GetBytes($"NUMBER:{PlayerNumber}"); // Encode the player number
                stream.Write(data, 0, data.Length); // Send the player number to the server
                stream.Flush(); // Ensure the data is sent immediately
            }
        }




    }
}
