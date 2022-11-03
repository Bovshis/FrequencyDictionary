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

        public void AddNewWord(object obj = null)
        {
            try
            {
                _dictionaryModel.AddNewWord(_newWord);
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
