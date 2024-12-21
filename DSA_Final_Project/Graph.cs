using System;
using System.Collections.Generic;

namespace DSA_Final_Project
{
    public class Graph
    {
        private Dictionary<(int, int), HashSet<(int, int)>> gridGraph;
        private int rows, cols;
        private char[,] board; // Assuming a board is used to store letters

        public Graph(int rows, int cols, char[,] board)
        {
            this.rows = rows;
            this.cols = cols;
            this.board = board;
            BuildGraph();
        }

        private void BuildGraph()
        {
            gridGraph = new Dictionary<(int, int), HashSet<(int, int)>>();

            int[] dRow = { -1, 0, 1, 0, -1, 1, -1, 1 };
            int[] dCol = { 0, 1, 0, -1, -1, -1, 1, 1 };

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    var neighbors = new HashSet<(int, int)>();
                    for (int i = 0; i < 8; i++)
                    {
                        int newRow = r + dRow[i];
                        int newCol = c + dCol[i];

                        if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols)
                        {
                            neighbors.Add((newRow, newCol));
                        }
                    }
                    gridGraph[(r, c)] = neighbors;
                }
            }
        }

        public HashSet<(int, int)> GetNeighbors(int row, int col)
        {
            if (gridGraph.ContainsKey((row, col)))
            {
                return gridGraph[(row, col)];
            }
            return new HashSet<(int, int)>();
        }

        public bool IsValidCell(int row, int col)
        {
            return row >= 0 && row < rows && col >= 0 && col < cols;
        }

        public bool ValidateWordDFS(string word, List<(int, int)> selectedCells)
        {
            if (word.Length != selectedCells.Count)
                return false; 

            bool[,] visited = new bool[rows, cols];

         
            bool Dfs(int index, int row, int col)
            {
                if (index == word.Length)
                    return true;

                if (!IsValidCell(row, col) || visited[row, col])
                    return false;

                if (word[index] != GetLetterAt(row, col))
                    return false;

                visited[row, col] = true;

                foreach (var (newRow, newCol) in GetNeighbors(row, col))
                {
                    if (Dfs(index + 1, newRow, newCol))
                        return true; 
                }

                visited[row, col] = false;
                return false;
            }

            var (startRow, startCol) = selectedCells[0];
            return Dfs(0, startRow, startCol);
        }

        private char GetLetterAt(int row, int col)
        {
            return board[row, col];
        }
    }
}
