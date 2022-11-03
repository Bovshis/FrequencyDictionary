using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FrequencyDictionary.Views;
using FrequencyDictionary.Models;
using FrequencyDictionary.ViewModels;

namespace FrequencyDictionary.Infrastructure
{
    public class CorrectWordViewOpener : ICorrectWordViewOpener
    {
        private readonly Window _dialog;

        public CorrectWordViewOpener(DictionaryModel dictionaryModel)
        {
            CorrectWordViewModel = new CorrectWordViewModel(dictionaryModel);
            var correctWordView = new CorrectWordView()
            {
                DataContext = CorrectWordViewModel
            };
            CorrectWordViewModel.Close += correctWordView.Close;


           _dialog = correctWordView;
        }

        public CorrectWordViewModel CorrectWordViewModel { get; set; }

        public void OpenDialog()
        {
            _dialog.ShowDialog();
        }
    }
}
