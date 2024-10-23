using ConnectFour;
using System.Net.Sockets;
using System.Text;

public class Client
{
    private NetworkStream stream;
    private TcpClient client;
    public string PlayerNumber { get; private set; } // Add this property
    public string PlayerName { get; private set; }
    public string ServerIp { get; private set; }
    public int Port { get; private set; }

    public Form1 FormInstance { get; private set; }

    public Client(string name, string ip, int port, string pnum)
    {
        PlayerName = name;
        ServerIp = ip;
        Port = port;
        PlayerNumber = pnum;

        //ConnectToServer(); 
       
    }
 

    private void ConnectToServer()
    {
        try
        {
            // Initialize and connect the client here
            client = new TcpClient(ServerIp, Port);
            stream = client.GetStream(); // Initialize stream AFTER connection is established
            //SendMessage($"NAME:{PlayerName};NUMBER:{PlayerNumber}");
            Task.Run(() => ListenForUpdates()); // Start listening for server updates in a separate thread



        }
        catch (Exception ex)
        {
            MessageBox.Show("Could not connect to server: " + ex.Message);
        }
    }

    public void SendMessage(string message)
    {
        if (client.Connected && stream != null)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
    }

    private void ListenForUpdates()
    {
        try
        {
            while (client.Connected)
            {
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                if (bytesRead > 0)
                {
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    // Handle messages
                    if (message == "SECOND_PLAYER_JOINED")
                    {
                        FormInstance.SecondPlayerJoined();
                    }
                    else if (message.StartsWith("MOVE"))
                    {
                        // Extract column from the message
                        int column = int.Parse(message.Split(' ')[1]);

                        // Update the board state for the other player
                        FormInstance.UpdateBoardState(message); // Pass the whole message to update the board

                        // Notify that it is now the other player's turn
                        FormInstance.UpdateCurrentTurn(); // Call this to switch turns on the UI
                    }
                    else if (message == "YOUR_TURN")
                    {
                        // Notify the client that it's their turn
                        FormInstance.UpdateCurrentTurn();
                    }
                }
            }
        }
        catch (IOException ex)
        {
            MessageBox.Show($"Connection lost: {ex.Message}");
        }
    }

}
