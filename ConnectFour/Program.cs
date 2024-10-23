using System;
using System.Windows.Forms;

namespace ConnectFour
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Show JoinForm to gather player info (name, server IP, port)
            var joinForm = new JoinForm();
            if (joinForm.ShowDialog() == DialogResult.OK)
            {
                // Step 1: Gather player info (PlayerName, ServerIp, Port) from JoinForm
                string playerName = joinForm.PlayerName;
                string serverIp = joinForm.ServerIp;
                int port = joinForm.Port;
                string playerNumber = joinForm.PlayerNumber;

                // Step 2: Create the client and connect to the server
                Client client = new Client(playerName, serverIp, port, playerNumber);

                // Check the player number after client connects
                if (playerNumber == "1" || playerNumber == "2") // Compare as strings
                {
                    Form1 gameForm = new Form1(client);
                    Application.Run(gameForm); // Start the game if the player number is valid
                }
                else
                {
                    MessageBox.Show("Failed to receive a valid player number.");
                }
            }
        }
    }
}

