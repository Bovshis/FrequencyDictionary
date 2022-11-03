using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace FrequencyDictionary.Models
{
    public class DictionaryModel : Dictionary<string, int>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public void ImportWords(List<string> filePaths)
        {
            foreach (var filePath in filePaths)
            {
                if (filePath.EndsWith(".txt")) ImportWordsFromTxt(filePath);
                if (filePath.EndsWith(".csv")) ImportWordsFromCsv(filePath);
            }

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void ImportWordsFromCsv(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var pair = line.Split(',');
                var word = pair[0];
                var count = int.Parse(pair[1]);
                this.AddWord(word, count);
            }
        }

        public void AddNewWord(string word, int count = 0)
        {
            ValidateAgainstAbsence(word);
            ValidateAgainstEmptyString(word);
            this.Add(word, 0);

            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                    new KeyValuePair<string, int>(word, count)));
        }

        public void RemoveWord(string word)
        {
            ValidateAgainstExistence(word);
            var index = this.Keys.ToList().IndexOf(word);
            var count = this[word];
            var pair = new KeyValuePair<string, int>(word, count);
            this.Remove(word);

            CollectionChanged?.Invoke(this, 
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, pair, index));
        }

        public void UpdateWord(string oldWord, string newWord)
        {
            ValidateAgainstExistence(oldWord);
            ValidateAgainstEmptyString(newWord);
            var incorrectWordCount = this[oldWord];
            this.Remove(oldWord);

            if (!this.ContainsKey(newWord))
            {
                this.Add(newWord, incorrectWordCount);
            }
            else
            {
                this[newWord] += incorrectWordCount;
            }

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void SaveToCsv(string fileName)
        {
            using var fileStream = new FileStream(fileName, FileMode.Create);
            using var streamWrite = new StreamWriter(fileStream);
            foreach (var pair in this)
            {
                streamWrite.WriteLine($"{pair.Key},{pair.Value}");
            }

        }

        private void ImportWordsFromTxt(string filePath)
        {
            string text = File.ReadAllText(filePath);
            IEnumerable<string> words = SplitTextToWords(ref text);
            AddWords(words);
        }

        private void AddWords(IEnumerable<string> words)
        {
            foreach (var word in words)
            {
                if (!this.ContainsKey(word)) this[word] = 0;
                AddWord(word);
                this[word]++;
            }
        }

        private static IEnumerable<string> SplitTextToWords(ref string text)
        {
            var separators = text
                .Where(c => !char.IsLetter(c))
                .Distinct()
                .ToArray();

            return text
                .Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }

        private void AddWord(string word, int count = 0)
        {
            if (this.ContainsKey(word)) this[word] += count;
            else this.Add(word, count);
        }

        private void ValidateAgainstAbsence(string word)
        {
            if (this.ContainsKey(word))
            {
                throw new ArgumentException($"Word: '{word}' already exists in dictionary.");
            }
        }

        private void ValidateAgainstExistence(string word)
        {
            if (!this.ContainsKey(word))
            {
                throw new ArgumentException($"Word: '{word}' doesn't exist in dictionary.");
            }
        }

        private void ValidateAgainstEmptyString(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                throw new ArgumentException("Word cannot be empty.");
            }
        }
    }
}
