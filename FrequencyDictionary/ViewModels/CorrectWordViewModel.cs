using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using FrequencyDictionary.Infrastructure;
using FrequencyDictionary.Models;
using FrequencyDictionary.ViewModels.Commands;

namespace FrequencyDictionary.ViewModels
{
    public class CorrectWordViewModel : PropertyNotifier
    {
        private readonly DictionaryModel _dictionaryModel;
        private string _correctForm;

        public CorrectWordViewModel(DictionaryModel dictionaryModel)
        {
            _dictionaryModel = dictionaryModel;
            CorrectWord = new RelayCommand(CorrectCommand, () => true);
        }

        public ICommand CorrectWord { get; }

        public string CorrectForm
        {
            get => _correctForm;
            set
            {
                _correctForm = value;
                OnPropertyChanged();
            }
        }

        public string IncorrectForm { get; set; }

        public Action Close { get; set; }

        private void CorrectCommand(object obj)
        {
            try
            {
                _dictionaryModel.UpdateWord(IncorrectForm, _correctForm);
                Close?.Invoke();
            }
            catch (ArgumentException e)
            {
                MessageBox.Show(e.Message, "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                CorrectForm = string.Empty;
            }
        }
    }
}
