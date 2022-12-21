using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Shapes;
using LemmaSharp;
using POSTagger.Corpora;
using POSTagger.Taggers;

namespace FrequencyDictionary.Models
{
    public class DictionaryModel : Dictionary<string, WordInformation>, INotifyCollectionChanged
    {
        private readonly Corpus _corpus;
        private readonly ITagger _tagger;
        private readonly Lemmatizer _lemmatizer;

        public DictionaryModel(Lemmatizer lemmatizer, Corpus corpus, ITagger tagger)
        {
            _lemmatizer = lemmatizer;
            _corpus = corpus;
            _tagger = tagger;
        }

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

        public void ClearDictionary()
        {
            this.Clear();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void ImportWordsFromCsv(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var pair = line.Split(',', 2);
                var word = pair[0];
                var wordInformation = WordInformation.Parse(pair[1]);
                this.AddWord(word, wordInformation);
            }
        }

        private void ImportWordsFromTxt(string filePath)
        {
            string text = File.ReadAllText(filePath);
            IEnumerable<string> words = SplitTextToWords(ref text);
            AddWords(words);
        }

        public void AddNewWord(string newWord, string newLemma, string newTags)
        {
            ValidateAgainstAbsence(newWord);
            ValidateAgainstEmptyString(newWord);
            var wordInformation = new WordInformation()
            {
                Count = 0, 
                Lemma = newLemma,
                Tags = newTags,
            };
            this.Add(newWord, wordInformation);

            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                    new KeyValuePair<string, WordInformation>(newWord, wordInformation)));
        }

        public void RemoveWord(string word)
        {
            ValidateAgainstExistence(word);
            var index = this.Keys.ToList().IndexOf(word);
            var wordInformation = this[word];
            var pair = new KeyValuePair<string, WordInformation>(word, wordInformation);
            this.Remove(word);

            CollectionChanged?.Invoke(this, 
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, pair, index));
        }

        public void UpdateWord(string oldWord, string newWord)
        {
            ValidateAgainstExistence(oldWord);
            ValidateAgainstEmptyString(newWord);
            var incorrectWordInformation = this[oldWord];
            this.Remove(oldWord);

            if (!this.ContainsKey(newWord))
            {
                incorrectWordInformation.Lemma = _lemmatizer.Lemmatize(newWord);
                incorrectWordInformation.Tags = _tagger.Tag(_corpus, newWord);
                this.Add(newWord, incorrectWordInformation);
            }
            else
            {
                this[newWord].Count += incorrectWordInformation.Count;
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

        private void AddWords(IEnumerable<string> words)
        {
            foreach (var word in words)
            {
                AddWord(word, 1);
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
            if (this.ContainsKey(word)) this[word].Count += count;
            else
            {
                var wordInformation = new WordInformation() 
                { 
                    Count = count,
                    Lemma = _lemmatizer.Lemmatize(word),
                    Tags = _tagger.Tag(_corpus, word)
                };
                this.Add(word, wordInformation);
            }
        }

        private void AddWord(string word, WordInformation wordInformation)
        {
            this[word] = wordInformation;
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
