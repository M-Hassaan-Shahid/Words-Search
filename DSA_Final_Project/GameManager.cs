using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_Final_Project
{
    public class GameManager
    {
        public int Score { get; private set; }
        public char[,] Grid { get; set; }
        private List<string> wordsToFind;
        private Dictionary<string, List<(int, int)>> wordCoordinates;
        private bool isHardMode;
        private Graph gameGraph;
        public bool checker = false;
        public static string[] words =
        {
            "CAT", "DOG", "TREE", "HOUSE", "AHMAD", "BOOK", "NARAN", "MOUSE", "PHONE", "UET",
            "TEACH", "MATH", "SCENE", "BRAIN", "KEY", "LAP", "TABLE", "KALAM", "PEN", "SMILE", "NICE"
        };

        public GameManager(int rows, int cols, bool hardMode)
        {
            checker = true;
            Grid = new char[rows, cols];
            wordsToFind = new List<string>();
            wordCoordinates = new Dictionary<string, List<(int, int)>>();
            Score = 0;
            isHardMode = hardMode;
            gameGraph = new Graph(rows, cols, Grid);  // Initialize the graph with the grid
        }

        public void SetWordsToFind(List<string> selectedWords)
        {
            wordsToFind.Clear(); // Clear any existing words
            foreach (var word in selectedWords)
            {
                wordsToFind.Add(word); // Add the selected words to the list
            }
        }

        public void InitializeGame()
        {
            PopulateWordTrie();

            if (isHardMode)
            {
                PopulateWordsInGridHardMode();
            }
            else
            {
                PopulateWordsInGridEasyMode();
            }

            PopulateRemainingGrid();
        }

        private void PopulateWordTrie()
        {
            Random rand = new Random();
            var randomWords = words.OrderBy(x => rand.Next()).Take(5).ToList(); // Randomly pick 5 words

            wordsToFind.Clear(); // Clear any existing words
            foreach (var word in randomWords)
            {
                wordsToFind.Add(word); // Add the randomly chosen words to the list
            }
        }

        private void PopulateWordsInGridEasyMode()
        {
            foreach (var word in wordsToFind)  // Iterate over the randomly selected words
            {
                PlaceWordInGridEasy(word);
            }
        }

        private void PlaceWordInGridEasy(string word)
        {
            Random rand = new Random();
            int row, col;
            bool isHorizontal = rand.Next(0, 2) == 0;
            bool wordPlaced = false;

            while (!wordPlaced)
            {
                row = rand.Next(0, Grid.GetLength(0));
                col = rand.Next(0, Grid.GetLength(1));

                if (CanPlaceWordWithGraph(word, row, col, isHorizontal))
                {
                    for (int i = 0; i < word.Length; i++)
                    {
                        if (isHorizontal)
                        {
                            Grid[row, col + i] = word[i];
                        }
                        else
                        {
                            Grid[row + i, col] = word[i];
                        }
                    }
                    wordPlaced = true;
                }
            }
        }

        private bool CanPlaceWordWithGraph(string word, int startRow, int startCol, bool isHorizontal)
        {
            List<(int, int)> selectedCells = new List<(int, int)>();

            for (int i = 0; i < word.Length; i++)
            {
                int row = startRow + (isHorizontal ? 0 : i);
                int col = startCol + (isHorizontal ? i : 0);

                if (!gameGraph.IsValidCell(row, col)) return false;
                if (Grid[row, col] != '\0' && Grid[row, col] != word[i]) return false;

                selectedCells.Add((row, col));
            }

            return gameGraph.ValidateWordDFS(word, selectedCells);  // Use DFS to validate the word
        }

        public List<string> GetWordsToFind()
        {
            return wordsToFind;
        }

        private void PopulateWordsInGridHardMode()
        {
            foreach (var word in wordsToFind)  // Iterate over the randomly selected words
            {
                PlaceWordInGridHard(word);
            }
        }

        private void PlaceWordInGridHard(string word)
        {
            Random rand = new Random();
            int row, col, direction;
            bool wordPlaced = false;

            while (!wordPlaced)
            {
                row = rand.Next(0, Grid.GetLength(0));
                col = rand.Next(0, Grid.GetLength(1));
                direction = rand.Next(0, 4);

                if (CanPlaceWordWithGraphHard(word, row, col, direction))
                {
                    var wordCoordinatesList = new List<(int, int)>();

                    for (int i = 0; i < word.Length; i++)
                    {
                        if (direction == 0)
                        {
                            Grid[row, col + i] = word[i];
                            wordCoordinatesList.Add((row, col + i));
                        }
                        else if (direction == 1)
                        {
                            Grid[row + i, col] = word[i];
                            wordCoordinatesList.Add((row + i, col));
                        }
                        else if (direction == 2)
                        {
                            Grid[row + i, col + i] = word[i];
                            wordCoordinatesList.Add((row + i, col + i));
                        }
                        else if (direction == 3)
                        {
                            Grid[row, col - i] = word[i];
                            wordCoordinatesList.Add((row, col - i));
                        }
                    }

                    wordCoordinates[word] = wordCoordinatesList;
                    wordPlaced = true;
                }
            }
        }

        private bool CanPlaceWordWithGraphHard(string word, int startRow, int startCol, int direction)
        {
            for (int i = 0; i < word.Length; i++)
            {
                int row = startRow + (direction == 1 || direction == 2 ? i : 0);
                int col = startCol + (direction == 0 || direction == 2 ? i : 0);
                col -= (direction == 3 ? i : 0);

                if (!gameGraph.IsValidCell(row, col)) return false;
                if (Grid[row, col] != '\0' && Grid[row, col] != word[i]) return false;
            }
            return true;
        }

        private void PopulateRemainingGrid()
        {
            Random rand = new Random();
            for (int row = 0; row < Grid.GetLength(0); row++)
            {
                for (int col = 0; col < Grid.GetLength(1); col++)
                {
                    if (Grid[row, col] == '\0')
                    {
                        Grid[row, col] = (char)rand.Next('A', 'Z' + 1);
                    }
                }
            }
        }

        public (int row, int col, char letter)? GetHintLetter()
        {
            foreach (var word in wordsToFind)
            {
                if (wordCoordinates.ContainsKey(word) && wordCoordinates[word].Count > 0)
                {
                    var firstLetterCoords = wordCoordinates[word][0];
                    char firstLetter = Grid[firstLetterCoords.Item1, firstLetterCoords.Item2];
                    return (firstLetterCoords.Item1, firstLetterCoords.Item2, firstLetter);
                }
            }
            return null;
        }
    }
}
