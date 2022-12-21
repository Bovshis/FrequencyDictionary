using System;
using System.Windows;
using System.Windows.Input;
using FrequencyDictionary.Infrastructure;
using FrequencyDictionary.Models;
using FrequencyDictionary.ViewModels.Commands;

namespace FrequencyDictionary.ViewModels
{
    public class AddNewWordViewModel : PropertyNotifier
    {
        private readonly DictionaryModel _dictionaryModel;
        private string _newWord;
        private string _newLemma;
        private string _newTags;

        public AddNewWordViewModel(DictionaryModel dictionaryModel)
        {
            _dictionaryModel = dictionaryModel;
            AddCommand = new RelayCommand(AddNewWord, () => true);
        }

        public ICommand AddCommand { get; set; }

        public string NewWord
        {
            get => _newWord;
            set
            {
                _newWord = value;
                OnPropertyChanged();
            }
        }

        public string NewLemma
        {
            get => _newLemma;
            set
            {
                _newLemma = value;
                OnPropertyChanged();
            }
        }

        public string NewTags
        {
            get => _newTags;
            set
            {
                _newTags = value;
                OnPropertyChanged();
            }
        }

        public void AddNewWord(object obj = null)
        {
            try
            {
                _dictionaryModel.AddNewWord(_newWord, _newLemma, _newTags);
            }
            catch (ArgumentException e)
            {
                MessageBox.Show(e.Message, "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                NewWord = string.Empty;
            }
        }
    }
}
