using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConnectFourServer
{
    class Server
    {
        private TcpListener listener;
        private List<TcpClient> clients;
        private int currentPlayer; // Start with Player 1
        private int[,] board; // 6 rows, 7 columns
        private List<string> playerNames = new List<string>();

        public Server(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            clients = new List<TcpClient>();
            currentPlayer = 1; // Start with Player 1
            board = new int[7, 6]; // 7 columns, 6 rows
        }

        public void Start()
        {
            listener.Start();
            Console.WriteLine("Server started... waiting for players to join.");

            while (true)
            {
                var client = listener.AcceptTcpClient();
                var clientEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
                    
                Console.WriteLine("Client connected: " + clientEndPoint.Address.ToString());

                Task.Run(() => HandleClient(client)); // Handle each client in a separate task
            }
        }

        private void HandleClient(TcpClient client)
        {
            var stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            // Check if the maximum number of players has been reached
            lock (clients) // Ensure thread-safety when checking client count
            {
                if (clients.Count >= 2)
                {
                    Console.WriteLine("No more players can join.");
                    client.Close();
                    return;
                }
            }

            string playerName = string.Empty;
            string playerNumber = string.Empty;

          

            // Read player name first
            bytesRead = stream.Read(buffer, 0, buffer.Length);
            string receivedMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            if (receivedMessage.StartsWith("NAME:"))
            {
                playerName = receivedMessage.Substring(5).Trim(); // Extract player name
                Console.WriteLine($"Player name received: {playerName}");
            }

            // Lock access to playerNames to prevent race conditions
            lock (playerNames)
            {
                // Check if the player name is already taken
                if (playerNames.Contains(playerName))
                {
                    Console.WriteLine($"Player name {playerName} is already taken.");
                    //Console.WriteLine("I am here for the second time, this is dumb.");
                    client.Close();
                    return; // Reject the connection
                }
                else
                {
                    playerNames.Add(playerName);
                    Console.WriteLine($"{playerName} connected!");

                    // Print contents of playerNames after each player joins
                    Console.WriteLine("Current player names: " + string.Join(", ", playerNames));
                }
            }

            // Read player number next
            bytesRead = stream.Read(buffer, 0, buffer.Length);
            receivedMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            // Log the received message for debugging
            Console.WriteLine($"Received player number message: '{receivedMessage}'");

            if (receivedMessage.StartsWith("NUMBER:"))
            {
                playerNumber = receivedMessage.Substring(7).Trim(); // Extract player number
                Console.WriteLine($"Player number received: {playerNumber}");
            }

            // Assign the player based on the number they chose
            lock (clients) // Ensure thread-safety when adding clients
            {
                if (playerNumber == "1")
                {
                    if (clients.Count == 0 || clients.Count == 1)
                    {
                        Console.WriteLine($"{playerName} has been assigned to Player 1.");
                        clients.Add(client);
                                         
                    }
                    else
                    {
                        Console.WriteLine("Player 1 slot is already taken.");
                        client.Close();
                        return;
                    }
                }
                else if (playerNumber == "2")
                {
                    if (clients.Count == 0 || clients.Count == 1)
                    {
                        Console.WriteLine($"{playerName} has been assigned to Player 2.");
                        BroadcastMessage("SECOND_PLAYER_JOINED");
                        clients.Add(client);
                        
                    }
                    else
                    {
                        Console.WriteLine("Player 2 slot is already taken.");
                        client.Close();
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid player number.");
                    client.Close();
                    return;
                }

                // Check if both players have joined
                if (clients.Count == 2)
                {
                    Console.WriteLine("Both players have joined. Game can start.");
                }
            }

            // Now process the moves from this client
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {message}");
                ProcessMove(message, playerName);
            }

            client.Close();
            lock (clients) // Ensure thread-safety
            {
                clients.Remove(client); // Remove the client
            }
        }



        private void BroadcastMessage(string message)
        {
            byte[] messageBytes = Encoding.ASCII.GetBytes(message);

            lock (clients) // Ensure thread-safety
            {
                foreach (var client in clients)
                {
                    var stream = client.GetStream();
                    stream.Write(messageBytes, 0, messageBytes.Length); // Send the message to all clients
                }
            }
        }

        private void ProcessMove(string message, string playerName)
        {
            if (message.StartsWith("MOVE "))
            {
                int column = int.Parse(message.Substring(5));
                Console.WriteLine($"{playerName} made a move in column {column}.");

                // Logic to drop a piece in the column, update the board
                UpdateBoard(column, playerName);

                // Notify both players of whose turn it is
                NotifyTurn();

                // After updating the board, broadcast the new game state
                BroadcastGameState();
            }
        }

        private void NotifyTurn()
        {
            string yourTurnMessage1 = "YOUR_TURN"; // Message for Player 1
            string yourTurnMessage2 = "YOUR_TURN"; // Message for Player 2

            // Determine the current player and send the appropriate message
            lock (clients) // Ensure thread-safety
            {
                foreach (var client in clients)
                {
                    var stream = client.GetStream();
                    if (clients.IndexOf(client) == currentPlayer - 1) // Current player
                    {
                        // Send "YOUR_TURN" to the current player
                        byte[] messageBytes = Encoding.ASCII.GetBytes(yourTurnMessage1);
                        stream.Write(messageBytes, 0, messageBytes.Length);
                    }
                    else // Other player
                    {
                        // Notify other player that it's their turn
                        byte[] messageBytes = Encoding.ASCII.GetBytes(yourTurnMessage2);
                        stream.Write(messageBytes, 0, messageBytes.Length);
                    }
                }
            }
        }

        private void UpdateBoard(int column, string playerName)
        {
            // Determine which player is making the move
            int player = (currentPlayer == 1) ? 1 : 2; // Use the last player

            // Check if the column is valid and not full
            for (int row = 5; row >= 0; row--)
            {
                if (board[column, row] == 0) // If the spot is empty
                {
                    board[column, row] = player; // Place the player's piece
                    Console.WriteLine($"Placed piece for Player {player} in column {column}.");
                    currentPlayer = (currentPlayer == 1) ? 2 : 1; // Switch players
                    break; // Exit the loop after placing the piece
                }
            }
        }

        private void BroadcastGameState()
        {
            string gameState = GetGameState();
            byte[] message = Encoding.ASCII.GetBytes(gameState);

            lock (clients) // Ensure thread-safety
            {
                foreach (var client in clients)
                {
                    var stream = client.GetStream();
                    stream.Write(message, 0, message.Length); // Send the updated game state to all clients
                }
            }
        }

        private string GetGameState()
        {
            // Generate a string representation of the board for clients
            StringBuilder sb = new StringBuilder();
            for (int col = 0; col < 7; col++)
            {
                for (int row = 0; row < 6; row++)
                {
                    sb.Append(board[col, row]).Append(",");
                }
                sb.Append(";");
            }
            return sb.ToString();
        }

        static void Main(string[] args)
        {
            Server server = new Server(12345); // Use your desired port
            server.Start();
        }
    }
}
