using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using FrequencyDictionary.Infrastructure;
using FrequencyDictionary.Models;
using FrequencyDictionary.ViewModels.Commands;
using Microsoft.Win32;

namespace FrequencyDictionary.ViewModels
{
    internal class DictionaryViewModel : PropertyNotifier
    {
        private readonly IOpenFileDialogOpener _openFileDialogOpener;
        private readonly ISaveFileDialogOpener _saveFileDialogOpener;
        private readonly DictionaryModel _dictionaryModel;

        private string _searchedWord = string.Empty;
        private readonly ListView _dictionaryView;

        public DictionaryViewModel(DictionaryModel dictionaryModel,
            IOpenFileDialogOpener openFileDialogOpener,
            ISaveFileDialogOpener saveFileDialogOpener,
            ListView dictionaryView)
        {
            _dictionaryModel = dictionaryModel;
            _openFileDialogOpener = openFileDialogOpener;
            _saveFileDialogOpener = saveFileDialogOpener;

            _dictionaryView = dictionaryView;
        }

        public Dictionary<string, int> FrequencyDictionary => _dictionaryModel;

        public string SearchedWord
        {
            get => _searchedWord;
            set
            {
                _searchedWord = value;
                OnPropertyChanged();

                if (string.IsNullOrEmpty(value))
                {
                    _dictionaryView.Items.Filter = null;
                }
                else
                {
                    _dictionaryView.Items.Filter = (item) =>
                    {
                        if (item is KeyValuePair<string, int> pair)
                        {
                            return _searchedWord.StartsWith('-')
                                ? pair.Key.EndsWith(_searchedWord[1..])
                                : pair.Key.StartsWith(_searchedWord);
                        }
                        return false;
                    };
                }

                OnCountsChanged();
            }
        }

        public int UniqueWordsCount => _dictionaryView.Items.Count;

        public int WordsCount => _dictionaryView.Items.Cast<KeyValuePair<string, int>>().Select(pair => pair.Value).Sum();

        public ICommand ImportWordsCommand => new RelayCommand((_) =>
        {
            _openFileDialogOpener.OpenFileDialog();
            List<string> filePaths = _openFileDialogOpener.GetSelectedFilePaths();
            _dictionaryModel.ImportWords(filePaths);

            OnCountsChanged();
        });

        public ICommand SortWordsCommand => new RelayCommand((obj) =>
        {
            if (obj is not string columnName) return;

            var sortDirection = _dictionaryView.Items.SortDescriptions.First().Direction == ListSortDirection.Ascending ?
                ListSortDirection.Descending : ListSortDirection.Ascending;

            _dictionaryView.Items.SortDescriptions[0] = new SortDescription(columnName, sortDirection);
        });

        public ICommand AddWordCommand => new RelayCommand((_) =>
        {
            var addNewWordOpener = new AddNewWordViewOpener(_dictionaryModel);
            addNewWordOpener.OpenDialog();
            OnCountsChanged();
        });

        public ICommand RemoveWordCommand => new RelayCommand((_) =>
        {
            var selectedWord = ((KeyValuePair<string, int>)_dictionaryView.SelectedItem).Key;
            _dictionaryModel.RemoveWord(selectedWord);
            OnCountsChanged();
        });

        public ICommand CorrectWordCommand => new RelayCommand((_) =>
        {
            var correctWordOpener = new CorrectWordViewOpener(_dictionaryModel);

            var incorrectWord = ((KeyValuePair<string, int>)_dictionaryView.SelectedItem).Key;
            correctWordOpener.CorrectWordViewModel.IncorrectForm = incorrectWord;
            correctWordOpener.OpenDialog();
        });

        public ICommand SaveToCsvCommand => new RelayCommand((_) =>
        {
            _saveFileDialogOpener.OpenFileDialog();
            _dictionaryModel.SaveToCsv(_saveFileDialogOpener.GetSelectedFilePath());
        });

        private void OnCountsChanged()
        {
            OnPropertyChanged(nameof(UniqueWordsCount));
            OnPropertyChanged(nameof(WordsCount));
        }
    }
}
