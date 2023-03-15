using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;
using System.Media;
using System.IO;


namespace connect_4_project
{
    public partial class Form1 : Form
    {
        // Declare the sound player object
        private SoundPlayer playera = new SoundPlayer();
        private SoundPlayer playerb = new SoundPlayer();
        private SoundPlayer playerc = new SoundPlayer();
       //Booleans for the players 
        public bool player1;
        public bool player2;
        int count = 0;
        //Color for the color of each game piece
        Color PlayerColor;
        //array of integers 
        int[,] board = new int[6, 7];
        //Turn
        int turn;
        //Locations
        private int previousLoc;
        private int previousRow;
        //for timer
        int sec = 0;
        //constructor of the Form
        public Form1()
        {
            InitializeComponent();
            //Set the sound file path of the audio
            playera.SoundLocation = Path.GetFullPath ("cukur.wav");
            // Enable looping
            playera.PlayLooping();
           //passing value to the players
            player1 = true;
            player2 = false;
            //passing Color 
            PlayerColor = Color.Blue;
        }
        //Method to handle painting of the form
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            this.panel1.BackColor = Color.SlateGray;
            Graphics g = e.Graphics;
            Pen line = new Pen(Color.Black);
            SolidBrush myBrush = new SolidBrush(Color.White);
            //draw circles
            for (int y = 10; y <= 710; y += 100)
            {
                for (int x = 10; x <= 610; x += 100)
                {
                    g.FillEllipse(myBrush, new Rectangle(x, y, 80, 80));
                }
            }
        }
        //Method to check if the row is empty
        private int emptyRow(int col)
        {
            for (int i = 5; i >= 0; i--)
            {
                if (board[i, col] == 0)
                {
                    return i;
                }
            }
            return -1;
        }
        //Method to handle mouse click in the panel
        //Method that changes the players turn and game piece color
        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            
            button1.Enabled = true;
            using (Graphics f = this.panel1.CreateGraphics())
               
                if (player1)
                {
                    label3.Text = "00:00";
                    sec = 0;
                    timer1.Start();
                    turn = 1;
                    PlayerColor = Color.Blue;
                    drawGamePiece(e, f);
                    player1 = false;
                    player2 = true;
                    label2.Text = "player 2";
                    label2.ForeColor = Color.OrangeRed;
                }
                else if (player2)
                {
                    label3.Text = "00:00";

                    sec = 0;
                    timer1.Start();
                    PlayerColor = Color.Red;
                    turn = 2;
                    drawGamePiece(e, f);
                    player2 = false;
                    player1 = true;
                    label2.Text = "player 1";
                    label2.ForeColor = Color.DarkTurquoise;
                }
            if(count==0)
                reset();
        }
        // method to take step back for the last player 
        public void clearGamePiece(Graphics f) 
        {
            SolidBrush myBrush = new SolidBrush(Color.White);
            f.FillEllipse(myBrush, new Rectangle(previousLoc * 100 + 10, previousRow * 100 + 10, 80, 80));

        }
        
        //method to handle drawing the pieces, and checking win
        public void drawGamePiece(MouseEventArgs e, Graphics f)
        {
            int col = e.X / 100;
            int row = emptyRow(col);
            this.previousRow = row;// save current row 
            this.previousLoc = col;// save current col
            SolidBrush myBrush = new SolidBrush(PlayerColor);
            int win = -1;
            for (int i = 0; i < 7; i++)
            {
                if (row != -1)
                {
                    board[row, col] = turn;
                    if (turn == 1 || turn == 2)
                    {
                        f.FillEllipse(myBrush, new Rectangle(col * 100 + 10, row * 100 + 10, 80, 80));
                    }
                    win = winSituation(turn);

                }
            }
            draw();
            if (win != -1)
            {
                timer1.Stop();
                label3.Text = "00:00";
                //Set the sound file path
                playerb.SoundLocation = Path.GetFullPath("Tada-sound.wav");
                // Enable looping
                playerb.Play();

                MessageBox.Show($"player {win} won");
                reset();
            }


        }
        //Method to check if there is a winner
        private int winSituation(int checkPlayer)
        {
            //For loop to check vertical win
            for (int row = 0; row < board.GetLength(0) - 3; row++)
            {
                for (int col = 0; col < board.GetLength(1); col++)
                {
                    if (checkWin(checkPlayer, board[row, col], board[row + 1, col], board[row + 2, col], board[row + 3, col]))
                        return checkPlayer;
                }
            }
            //For loop to check horizontal win
            for (int row = 0; row < board.GetLength(0); row++)
            {
                for (int col = 0; col < board.GetLength(1) - 3; col++)
                {
                    if (checkWin(checkPlayer, board[row, col], board[row, col + 1], board[row, col + 2], board[row, col + 3]))
                        return checkPlayer;
                }
            }
            //For loop to check diagonal going up and right from starting piece
            for (int row = 0; row < board.GetLength(0) - 3; row++)
            {
                for (int col = 0; col < board.GetLength(1) - 3; col++)
                {

                    if (checkWin(checkPlayer, board[row, col], board[row + 1, col + 1], board[row + 2, col + 2], board[row + 3, col + 3]))
                        return checkPlayer;
                }
            }
            //For loop to check diagonal win going up and to the left from starting piece
            for (int row = 0; row < board.GetLength(0) - 3; row++)
            {
                for (int col = 3; col < this.board.GetLength(1); col++)
                {

                    if (checkWin(checkPlayer, board[row, col], board[row + 1, col - 1], board[row + 2, col - 2], board[row + 3, col - 3]))
                        return checkPlayer;
                }
            }
            return -1;
        }
        //method to check Play
        private bool checkWin(int checkPlayer, params int[] number)
        {
            foreach (var num in number)
            {
                if (num != checkPlayer)
                {
                    return false; ;
                }

            }
            return true;
        }
        //Method to draw blank game board if no player won
        private void draw()
        {
            int drawcount = 0;
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] == 1|| board[i, j] == 2)
                    {
                        count++;
                        drawcount++;
                    }
                }
            }
         
            if (drawcount == 42)
            {
                //Set the sound file path
                playerc.SoundLocation = Path.GetFullPath("negative.wav");
                // Enable looping
                playerc.Play();
                timer1.Stop();
                label3.Text = "00:00";
                MessageBox.Show("Draw");
                reset();
            }
        }
        //Method for reset button, resets game when clicked
        private void reset()
        {
            button1.Enabled= false;
            count = 0;
            playerb.Stop();
            playera.PlayLooping();
            label2.ForeColor = Color.DarkTurquoise;
            player1 = true;
            player2 = false;
            PlayerColor = Color.Blue;
            board = new int[6, 7];
            label2.Text = "Player 1 Start";
            label3.Text = "00:00";
            sec = 0;
            panel1.Invalidate();
        }
        //Button to undo the last game when clicked
        private void button1_Click(object sender, EventArgs e)
        {
            board[previousRow, previousLoc] = 0; //Neither player1 or player2
            clearGamePiece(this.panel1.CreateGraphics());//draw white circle of previous
            if (player1)
            {
                label3.Text = "00:00";
                sec = 0;
                timer1.Start();
                turn = 1;
                PlayerColor = Color.Blue;
                player1 = false;
                player2 = true;
                label2.Text = "player 2";
                label2.ForeColor = Color.OrangeRed;
            }
            else if (player2)
            {
                label3.Text = "00:00";
                sec = 0;
                timer1.Start();
                PlayerColor = Color.Red;
                turn = 2;
                player2 = false;
                player1 = true;
                label2.Text = "player 1";
                label2.ForeColor = Color.DarkTurquoise;
            }
            button1.Enabled = false;
        }
        //Button to reset game when clicked
        private void button2_Click(object sender, EventArgs e)
        {
            reset();
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (turn!=0||label2.Text=="Player 1 start")
            {
               
                sec++;

                if (sec >= 10)
                {
                    //Set the sound file path
                    playerb.SoundLocation = Path.GetFullPath("Tada-sound.wav");
                    // Enable looping
                    playerb.Play();
                    label3.Text = "00:00";
                    timer1.Stop();
                    MessageBox.Show($"player {turn} won");
                    reset();
                }
                
                    label3.Text = "00:0" + sec;
            }
        }
    }
}
