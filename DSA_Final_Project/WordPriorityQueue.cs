using System;
using System.Collections.Generic;

public class WordPriorityQueue
{
    private List<Tuple<string, int>> words;

    public WordPriorityQueue()
    {
        words = new List<Tuple<string, int>>();
    }

    public void AddWord(string word, int priority)
    {
        words.Add(new Tuple<string, int>(word, priority));
        SortWords();
    }

    public string GetNextWord()
    {
        if (words.Count > 0)
        {
            return words[0].Item1; // Return word with highest priority
        }
        return null;
    }

    public void RemoveNextWord()
    {
        if (words.Count > 0)
        {
            words.RemoveAt(0); // Remove the word with the highest priority
        }
    }

    public void SortWords()
    {
        words.Sort((x, y) => x.Item2.CompareTo(y.Item2)); // Sort by priority (ascending)
    }

    public bool HasWords()
    {
        return words.Count > 0;
    }
}
