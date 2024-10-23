using System;
using System.Drawing;
using System.Windows.Forms;

namespace ConnectFour
{
    public partial class Form1 : Form
    {
        private int currentPlayer; // 1 for Player 1 (Red), 2 for Player 2 (Yellow)
        private int[,] board = new int[7, 6]; // 7 columns, 6 rows
        private Client client;
        private bool isSecondPlayerConnected = false;

        public Form1(Client client)
        {
            InitializeComponent();
            this.client = client;

            // Use the player's number directly from the client
            currentPlayer = int.Parse(client.PlayerNumber);   

            // Initialize default game state
            lblWinnerTitle.Text = "Winner displayed below";
            lblWinner.Visible = false;
            btnRestart.Enabled = false;

            // Set player color and the current turn label
            UpdatePlayerColor();
            UpdateCurrentTurn();

            // Disable buttons for Player 2 if Player 1 goes first
            UpdateButtonState();
        }

        public void SecondPlayerJoined()
        {
            isSecondPlayerConnected = true;
            MessageBox.Show("Second player has joined! Let the game begin.");
        }

        public void UpdateBoardState(string gameState)
        {
            string[] columns = gameState.Split(';');
            for (int col = 0; col < columns.Length; col++)
            {
                string[] rows = columns[col].Split(',');
                for (int row = 0; row < rows.Length; row++)
                {
                    int cellValue = int.Parse(rows[row]);
                    if (cellValue == 1)
                        UpdateBoardVisual(col, row, false, 'r'); // Player 1 (Red)
                    else if (cellValue == 2)
                        UpdateBoardVisual(col, row, false, 'y'); // Player 2 (Yellow)
                    else
                        UpdateBoardVisual(col, row, true, 'n');  // Empty
                }
            }
            // Switch the current player after updating the board
            currentPlayer = (currentPlayer == 1) ? 2 : 1;
            UpdatePlayerColor();
            UpdateCurrentTurn();
            UpdateButtonState();
        }

        private void btnColumn1_Click(object sender, EventArgs e) => MakeMove(0);
        private void btnColumn2_Click(object sender, EventArgs e) => MakeMove(1);
        private void btnColumn3_Click(object sender, EventArgs e) => MakeMove(2);
        private void btnColumn4_Click(object sender, EventArgs e) => MakeMove(3);
        private void btnColumn5_Click(object sender, EventArgs e) => MakeMove(4);
        private void btnColumn6_Click(object sender, EventArgs e) => MakeMove(5);
        private void btnColumn7_Click(object sender, EventArgs e) => MakeMove(6);

        private void UpdatePlayerColor()
        {
            if (currentPlayer == 1)
            {
                lblColorVal.Text = "RED";
                lblColorVal.ForeColor = Color.Red;
            }
            else if (currentPlayer == 2)
            {
                lblColorVal.Text = "YELLOW";
                lblColorVal.ForeColor = Color.Yellow;
            }
            else
            {
                MessageBox.Show("Invalid player number received!");
            }
        }

        public void UpdateCurrentTurn()
        {
            lblCurrentTurn.Text = $"Current Turn: Player {currentPlayer}";
        }

        private void UpdateButtonState()
        {
            bool isMyTurn = currentPlayer == int.Parse(client.PlayerNumber);
            btnColumn1.Enabled = isMyTurn;
            btnColumn2.Enabled = isMyTurn;
            btnColumn3.Enabled = isMyTurn;
            btnColumn4.Enabled = isMyTurn;
            btnColumn5.Enabled = isMyTurn;
            btnColumn6.Enabled = isMyTurn;
            btnColumn7.Enabled = isMyTurn;
        }

        private void MakeMove(int column)
        {
            if (!isSecondPlayerConnected)
            {
                MessageBox.Show("Waiting for the second player to join!");
                return;
            }

            if (currentPlayer != int.Parse(client.PlayerNumber))
            {
                MessageBox.Show("It's not your turn!");
                return;
            }

            int row = -1;
            for (int i = 5; i >= 0; i--)
            {
                if (board[column, i] == 0)
                {
                    row = i;
                    break;
                }
            }

            if (row != -1)
            {
                board[column, row] = currentPlayer;

                if (currentPlayer == 1)
                    UpdateBoardVisual(column, row, false, 'r'); // Red for Player 1
                else
                    UpdateBoardVisual(column, row, false, 'y'); // Yellow for Player 2

                client.SendMessage($"MOVE {column}");

                CheckForWinner();

                currentPlayer = (currentPlayer == 1) ? 2 : 1; // Switch turns
                UpdatePlayerColor();
                UpdateCurrentTurn();
                UpdateButtonState(); // Update button enable/disable after move
            }
        }

        private void CheckForWinner()
        {
            if (HasPlayerWon(1))
            {
                lblWinner.Text = "Player 1 Wins!";
                lblWinner.ForeColor = Color.Red;
                lblWinner.Visible = true;
                EndGame();
            }
            else if (HasPlayerWon(2))
            {
                lblWinner.Text = "Player 2 Wins!";
                lblWinner.ForeColor = Color.Yellow;
                lblWinner.Visible = true;
                EndGame();
            }
            else if (IsBoardFull())
            {
                lblWinner.Text = "It's a Draw!";
                lblWinner.ForeColor = Color.Gray;
                lblWinner.Visible = true;
                EndGame();
            }
        }

        private void EndGame()
        {
            DisableGame();
            btnRestart.Visible = true;
        }

        private void ResetGame()
        {
            for (int column = 0; column < 7; column++)
            {
                for (int row = 0; row < 6; row++)
                {
                    board[column, row] = 0;
                    UpdateBoardVisual(column, row, true, 'n');
                }
            }

            lblWinner.Visible = false;
            btnRestart.Visible = false;
            EnableGame();

            currentPlayer = 1; // Reset to Player 1
            UpdatePlayerColor();
            UpdateCurrentTurn();
            UpdateButtonState();
        }

        private void EnableGame()
        {
            btnColumn1.Enabled = true;
            btnColumn2.Enabled = true;
            btnColumn3.Enabled = true;
            btnColumn4.Enabled = true;
            btnColumn5.Enabled = true;
            btnColumn6.Enabled = true;
            btnColumn7.Enabled = true;
        }

        private void DisableGame()
        {
            btnColumn1.Enabled = false;
            btnColumn2.Enabled = false;
            btnColumn3.Enabled = false;
            btnColumn4.Enabled = false;
            btnColumn5.Enabled = false;
            btnColumn6.Enabled = false;
            btnColumn7.Enabled = false;
        }

        private bool HasPlayerWon(int player)
        {
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    if (board[col, row] == player && board[col + 1, row] == player && board[col + 2, row] == player && board[col + 3, row] == player)
                        return true;
                }
            }

            for (int col = 0; col < 7; col++)
            {
                for (int row = 0; row < 3; row++)
                {
                    if (board[col, row] == player && board[col, row + 1] == player && board[col, row + 2] == player && board[col, row + 3] == player)
                        return true;
                }
            }

            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    if (board[col, row] == player && board[col + 1, row + 1] == player && board[col + 2, row + 2] == player && board[col + 3, row + 3] == player)
                        return true;
                }
            }

            for (int row = 3; row < 6; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    if (board[col, row] == player && board[col + 1, row - 1] == player && board[col + 2, row - 2] == player && board[col + 3, row - 3] == player)
                        return true;
                }
            }

            return false;
        }

        private bool IsBoardFull()
        {
            for (int col = 0; col < 7; col++)
            {
                for (int row = 0; row < 6; row++)
                {
                    if (board[col, row] == 0)
                        return false;
                }
            }
            return true;
        }

        private void UpdateBoardVisual(int column, int row, bool clear, char color)
        {
            PictureBox pictureBox = GetPictureBox(column, row);

            if (clear && color == 'n')
            {
                pictureBox.Image = null;
            }
            else
            {
                pictureBox.Image = color == 'r' ? Image.FromFile(@"Images\RedCircle.png") : Image.FromFile(@"Images\YellowCircle.png");
            }
        }

        private PictureBox GetPictureBox(int column, int row)
        {
            return (PictureBox)this.Controls[$"pictureBox{column * 6 + row + 1}"] ?? throw new InvalidOperationException("PictureBox not found.");
        }
    }
}
