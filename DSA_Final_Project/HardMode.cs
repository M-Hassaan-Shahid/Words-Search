using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace DSA_Final_Project
{
    public partial class HardMode : Form
    {
        private Graph gameGraph;
        private Panel gamePanel;
        private Label scoreLabel, wordLabel, timerLabel;
        private Timer gameTimer;
        private int timeLeft;
        private Button hintButton;
        private int hintCount = 0;
        private const int MaxHints = 3;
       SoundManager soundManager = new SoundManager();

        private GameManager gameEngine;
        private PictureBox[,] pictureGrid;
        private List<(int row, int col)> selectedCells;
        private string selectedWord;

        public HardMode()
        {
            this.StartPosition = FormStartPosition.Manual;
            InitializeComponent();
        }
        private void CenterForm()
        {
            this.Load += (sender, e) =>
            {
                int x = (Screen.PrimaryScreen.WorkingArea.Width - this.Width);
                int y = (Screen.PrimaryScreen.WorkingArea.Height - this.Height);
                this.Location = new Point(x, y);
            };
        }
        private void HardMode_Load(object sender, EventArgs e)
        {
            CenterForm();
            InitializeComponents();

            gameEngine = new GameManager(9, 9, true);
            gameEngine.InitializeGame();
            InitializePictureGrid(9,9);
            gameGraph = new Graph(9, 9, gameEngine.Grid);
            UpdateGrid();
            DisplayHardcodedWords();
            timeLeft = 70;
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
                Text = "Time Left: 25",
                Location = timerLabelLocation,
                Width = 200,
                Height = timerLabelHeight,
                Font = new Font("Verdana", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(timerLabel);
            hintButton = new Button
            {
                Text = $"Hint ({MaxHints - hintCount})",
                Location = new Point(330, 545),
                Width = 80,
                Height = 30,
                Font = new Font("Verdana", 10),
                BackColor = Color.LightGray
            };
            hintButton.Click += HintButton_Click;
            this.Controls.Add(hintButton);
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
        }
        private void PictureBox_Click(object sender, EventArgs e, int row, int col)
        {

            soundManager.click.Play();
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
                    List<(int, int)> selectedCellsList = selectedCells.ToList();
                    if (gameGraph.ValidateWordDFS(word, selectedCellsList))
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
                                winingForm winingForm = new winingForm($"Congratulations! You finished in {70 - timeLeft} seconds.");
                                winingForm.Show();
                                this.Hide();
                            }

                            return;
                        }
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
        private async void HintButton_Click(object sender, EventArgs e)
        {
            soundManager.click.Play();

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
                await Task.Delay(1000);
                string imageName = $"letter_{originalLetter.ToString().ToUpper()}";
                pictureGrid[row, col].Image = (Image)Properties.Resources.ResourceManager.GetObject(imageName);

                hintCount++;
                hintButton.Text = $"Hint ({MaxHints - hintCount})";

                if (hintCount >= MaxHints)
                {
                    hintButton.Enabled = false;
                }
            }
            else
            {
                MessageBox.Show("No more unsubmitted words to hint!");
            }
        }

        private void DisplayHardcodedWords()
        {
            int yOffset = 220;
            var wordsToFind = gameEngine.GetWordsToFind();
            foreach (var word in wordsToFind)
            {
                int len = word.Length;
                string st = "";
                for(int i = 0; i < len; i++) 
                {
                    st += "*";
                }
                var wordLabel = new Label
                {
                    Text = st,
                    Location = new Point(512, yOffset),
                    Font = new Font("Verdana", 17, FontStyle.Regular),
                    Width = 200

                };
                yOffset += 50;
                this.Controls.Add(wordLabel);
                st = "";
            }
        }
    }
}