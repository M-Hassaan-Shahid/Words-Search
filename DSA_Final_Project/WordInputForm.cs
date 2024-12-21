using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSA_Final_Project
{
    public partial class WordInputForm : Form
    {
        // Form components
        private Graph gameGraph;
        private Panel gamePanel;
        private Label scoreLabel, wordLabel, timerLabel; 
        private Timer gameTimer; 
        private int timeLeft; 
        private Button hintButton;
        private int hintCount = 0;
        private const int MaxHints = 3;
        private List<string> wordsToFind;  
        private SoundManager soundManager;

        // Backend Components

        private GameManager gameEngine;
        private PictureBox[,] pictureGrid;
        private List<(int row, int col)> selectedCells; 
        private string selectedWord; 

        public WordInputForm(List<string> userWords)
        {
            this.StartPosition = FormStartPosition.Manual; 
            InitializeComponent();

            wordsToFind = new List<string>(userWords);
        }
        
        public int FindMaxWordLength(List<string> userWords)
        {
            if(userWords == null  || userWords.Count == 0)
            {
                return 0;
            }
            int max = 0;
            foreach(string word in wordsToFind) 
            {
                if(word.Length > max)
                {
                    max = word.Length;
                }
            }
            return max;
        }
        private void WordInputForm_Load(object sender, EventArgs e)
        {
            int x = (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2;
            int y = (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2;
            this.Location = new Point(x, y);  
            InitializeComponents();
            int max = FindMaxWordLength(wordsToFind);
            gameEngine = new GameManager(max, max, true);
            gameEngine.InitializeGame();            
            gameEngine.SetWordsToFind(wordsToFind);
            InitializePictureGrid(max, max);
            gameGraph = new Graph(max, max, gameEngine.Grid);
            UpdateGrid();
            DisplayHardcodedWords();
            timeLeft = 60;
            gameTimer.Start();
        }

        private void InitializeComponents()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Hard Mode - Word Search";
            this.Size = new Size(770, 610); 
            this.Location = new Point(100, 100);
            this.BackColor = Color.White;

            int gamePanelWidth = 319;
            int gamePanelHeight = 319;
            Point gamePanelLocation = new Point(75, 180);
            gamePanel = new Panel
            {
                Location = gamePanelLocation,
                Width = gamePanelWidth,
                Height = gamePanelHeight,
                BackColor = Color.LightGray
            };
            this.Controls.Add(gamePanel);

            int rows = 10;
            int cols = 10;
            int boxSize = gamePanel.Width / cols;
            pictureGrid = new PictureBox[rows, cols];

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var pictureBox = new PictureBox
                    {
                        Width = boxSize,
                        Height = boxSize,
                        Location = new Point(col * boxSize, row * boxSize), 
                        BorderStyle = BorderStyle.FixedSingle,
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        BackColor = Color.White
                    };

                    pictureGrid[row, col] = pictureBox;
                    gamePanel.Controls.Add(pictureBox);
                }
            }

            int timerLabelHeight = 25;
            Point timerLabelLocation = new Point(70, 545);
            timerLabel = new Label
            {
                Text = "Time Left: 60",
                Location = timerLabelLocation,
                Width = 200,
                Height = timerLabelHeight,
                Font = new Font("Verdana", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(timerLabel);
          
            int wordLabelHeight = 25;
            Point wordLabelLocation = new Point(140, 107);
            wordLabel = new Label
            {
                Text = "Selected Word:  ",
                Location = wordLabelLocation,
                Width = 200,
                Height = wordLabelHeight,
                Font = new Font("Verdana", 12),
                TextAlign = ContentAlignment.MiddleLeft

            };
            this.Controls.Add(wordLabel);


            gameTimer = new Timer();
            gameTimer.Interval = 1000; 
            gameTimer.Tick += GameTimer_Tick; 


            selectedCells = new List<(int, int)>();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                timeLeft--; 
                timerLabel.Text = $"Time Left: {timeLeft}";
            }
            else
            {
                gameTimer.Stop();
                Lose winingForm = new Lose("Time's Up! You Lose!");
                winingForm.Show();
                this.Hide();
            }
        }


        private void InitializePictureGrid(int rows, int cols)
        {
            gamePanel.Controls.Clear(); 
            pictureGrid = new PictureBox[rows, cols]; 

            int boxSize = gamePanel.Width / cols;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var pictureBox = new PictureBox
                    {
                        Width = boxSize,
                        Height = boxSize,
                        Location = new Point(col * boxSize, row * boxSize),
                        BorderStyle = BorderStyle.FixedSingle,
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        BackColor = Color.White
                    };

                    int r = row;
                    int c = col;

                    pictureBox.Click += (s, e) => PictureBox_Click(s, e, r, c);
                    pictureGrid[row, col] = pictureBox;
                    gamePanel.Controls.Add(pictureBox);
                }
            }

            FillGridWithWords(rows, cols);
        }
        private void FillGridWithWords(int rows, int cols)
        {
            Random random = new Random();
            char[,] grid = new char[rows, cols];

            foreach (var word in wordsToFind)
            {
                PlaceWordInGrid(word, grid, random);
            }

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (grid[row, col] == '\0') 
                    {
                        grid[row, col] = (char)random.Next('A', 'Z' + 1); 
                    }
                }
            }

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    string imageName = $"letter_{grid[row, col].ToString().ToUpper()}";
                    pictureGrid[row, col].Image = (Image)Properties.Resources.ResourceManager.GetObject(imageName);
                }
            }

            gameEngine.Grid = grid; 
        }

        private void PlaceWordInGrid(string word, char[,] grid, Random random)
        {
            bool placed = false;
            int attempts = 0;
            while (!placed && attempts < 100) 
            {
                attempts++;
                int direction = random.Next(0, 2);
                int row, col;

                if (direction == 0) 
                {
                    row = random.Next(0, grid.GetLength(0));
                    col = random.Next(0, grid.GetLength(1) - word.Length);
                }
                else 
                {
                    row = random.Next(0, grid.GetLength(0) - word.Length);
                    col = random.Next(0, grid.GetLength(1));
                }

                bool canPlace = true;
                for (int i = 0; i < word.Length; i++)
                {
                    if (grid[row + (direction == 0 ? 0 : i), col + (direction == 1 ? 0 : i)] != '\0')
                    {
                        canPlace = false;
                        break;
                    }
                }

                if (canPlace)
                {
                    for (int i = 0; i < word.Length; i++)
                    {
                        grid[row + (direction == 0 ? 0 : i), col + (direction == 1 ? 0 : i)] = word[i];
                    }
                    placed = true;
                }
            }
        }
        private void PictureBox_Click(object sender, EventArgs e, int row, int col)
        {
            //soundManager.click.Play();
            char clickedLetter = gameEngine.Grid[row, col];

            if (selectedCells.Contains((row, col)))
            {
                soundManager.error.Play();
                MessageBox.Show("This cell is already selected.");
                ResetSelection();
                return;
            }

            if (selectedCells.Count > 0)
            {
                var lastCell = selectedCells.Last();
                var neighbors = gameGraph.GetNeighbors(lastCell.row, lastCell.col); 
                if (!neighbors.Contains((row, col))) 
                {
                    soundManager.error.Play();
                    MessageBox.Show("You must select adjacent letters in sequence.");
                    ResetSelection();
                    return;
                }
            }

            selectedCells.Add((row, col));
            selectedWord += clickedLetter;
            pictureGrid[row, col].BackColor = Color.LightBlue;
            wordLabel.Text = $"Selected Word: {selectedWord}";

            foreach (var word in gameEngine.GetWordsToFind())
            {
                if (word.StartsWith(selectedWord)) 
                {
                    if (word == selectedWord) 
                    {
                        MessageBox.Show($"Correct! Word '{selectedWord}' found!");
                        //soundManager.found.Play();
                        foreach (var cell in selectedCells)
                        {
                            pictureGrid[cell.row, cell.col].BackColor = Color.White;
                            pictureGrid[cell.row, cell.col].Image = Properties.Resources.letter;
                        }

                        gameEngine.GetWordsToFind().Remove(word);

                        RemoveWordFromDisplay(word);

                        ResetSelection();

                        if (gameEngine.GetWordsToFind().Count == 0)
                        {
                            gameTimer.Stop(); 
                            winingForm winingForm = new winingForm($"Congratulations! You finished in {60 - timeLeft} seconds.");
                            winingForm.Show();
                            this.Hide();
                        }

                        return; 
                    }
                    return; 
                }
            }

            MessageBox.Show($"The sequence '{selectedWord}' is not part of any word.");
            ResetSelection();
        }

        private void RemoveWordFromDisplay(string word)
        {
            var controlsToRemove = this.Controls.OfType<Label>()
                .Where(label => label.Text == word)
                .ToList();

            foreach (var label in controlsToRemove)
            {
                this.Controls.Remove(label);
                label.Dispose();
            }

            int yOffset = 150;
            foreach (var label in this.Controls.OfType<Label>().Where(label => label.Location.X == 420))
            {
                label.Location = new Point(label.Location.X, yOffset);
                yOffset += 25;
            }
        }

        private void guna2PictureBox38_Click(object sender, EventArgs e)
        {

        }

        private void ResetSelection()
        {
            foreach (var cell in selectedCells)
            {
                pictureGrid[cell.row, cell.col].BackColor = Color.White;
            }
            selectedCells.Clear();
            selectedWord = string.Empty;
            wordLabel.Text = "Selected Word: ";
        }

        private void guna2PictureBox37_Click(object sender, EventArgs e)
        {
            Start start = new Start();
            start.Show();
            this.Hide();
        }

        private void UpdateGrid()
        {
            if (gameEngine.Grid == null)
            {
                MessageBox.Show("Grid not initialized!");
                return;
            }

            char[,] grid = gameEngine.Grid;
            for (int row = 0; row < grid.GetLength(0); row++)
            {
                for (int col = 0; col < grid.GetLength(1); col++)
                {
                    if (row < 0 || row >= grid.GetLength(0) || col < 0 || col >= grid.GetLength(1))
                    {
                        continue; 
                    }

                    char letter = grid[row, col];
                    string imageName = $"letter_{letter.ToString().ToUpper()}";

                    pictureGrid[row, col].Image = (Image)Properties.Resources.ResourceManager.GetObject(imageName);
                }
            }
        }

        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void guna2ControlBox1_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

      
        private async void HintButton_Click(object sender, EventArgs e)
        {
            //soundManager.click.Play();
            if (hintCount >= MaxHints)
            {
                MessageBox.Show("No hints left!");
                return;
            }

            
            var hintLetter = gameEngine.GetHintLetter();
            if (hintLetter.HasValue)
            {
                int row = hintLetter.Value.row;
                int col = hintLetter.Value.col;
                char originalLetter = hintLetter.Value.letter;

                pictureGrid[row, col].Image = Properties.Resources.letter;
                pictureGrid[row, col].BackColor = Color.LightGreen; 
                await Task.Delay(1000); 

                string imageName = $"letter_{originalLetter.ToString().ToUpper()}";
                pictureGrid[row, col].Image = (Image)Properties.Resources.ResourceManager.GetObject(imageName);
                pictureGrid[row, col].BackColor = Color.White;

                hintCount++;
                hintButton.Text = $"Hint ({MaxHints - hintCount})";

                if (hintCount >= MaxHints)
                {
                    hintButton.Enabled = false;
                }
            }
            else
            {
                MessageBox.Show("No more letters available for hints!");
            }
        }

        private void DisplayHardcodedWords()
        {

            int yOffset = 220;
            foreach (var word in wordsToFind)
            {
                var wordLabel = new Label
                {
                    Text = word,
                    Location = new Point(512, yOffset),
                    Font = new Font("Verdana", 17, FontStyle.Regular),
                    Width = 200
                };
                yOffset += 50;
                this.Controls.Add(wordLabel);
            }
        }
    }
}