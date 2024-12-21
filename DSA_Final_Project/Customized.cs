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
    public partial class Customized : Form
    {
        private TextBox wordInputTextBox;
        private ListBox wordListBox;
        private Button addWordButton;
        private Button startGameButton;
        private List<string> userWords;

        public Customized()
        {
            this.StartPosition = FormStartPosition.Manual;
            userWords = new List<string>();
            InitializeControls();
        }

        private void InitializeControls()
        {
            this.Text = "Customize Words - Word Search";
            this.Size = new System.Drawing.Size(400, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.None;

            wordInputTextBox = new TextBox
            {
                Location = new System.Drawing.Point(50, 50),
                Width = 200
            };
            this.Controls.Add(wordInputTextBox);

            addWordButton = new Button
            {
                Text = "Add Word",
                Location = new System.Drawing.Point(260, 50)
            };
            addWordButton.Click += AddWordButton_Click;
            this.Controls.Add(addWordButton);

            wordListBox = new ListBox
            {
                Location = new System.Drawing.Point(50, 100),
                Width = 300,
                Height = 150
            };
            this.Controls.Add(wordListBox);

            startGameButton = new Button
            {
                Text = "Start Game",
                Location = new System.Drawing.Point(150, 300)
            };
            startGameButton.Click += StartGameButton_Click;
            this.Controls.Add(startGameButton);
        }

        private void Customized_Load(object sender, EventArgs e)
        {
            int x = (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2;
            int y = (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2;
            this.Location = new Point(x, y);
        }

        private void AddWordButton_Click(object sender, EventArgs e)
        {
            string word = wordInputTextBox.Text.Trim().ToUpper();
            if (!string.IsNullOrEmpty(word) && !userWords.Contains(word))
            {
                if(word.Length < 5) 
                {
                    MessageBox.Show("Enter a word with length greater than 4.");
                    return;
                }
                userWords.Add(word);
                wordListBox.Items.Add(word);
                wordInputTextBox.Clear();
            }
            else
            {
                MessageBox.Show("Enter a valid, unique word.");
            }
        }

        private void StartGameButton_Click(object sender, EventArgs e)
        {
            if (userWords.Count == 0)
            {
                MessageBox.Show("Please add at least one word.");
                return;
            }

            WordInputForm hardModeForm = new WordInputForm(userWords);
            hardModeForm.Show();
            this.Hide();
        }
    }
}
