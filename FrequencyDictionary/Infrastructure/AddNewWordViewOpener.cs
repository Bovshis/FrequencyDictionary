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
    public class AddNewWordViewOpener : IWindowOpener
    {
        private readonly Window _dialog;

        public AddNewWordViewOpener(DictionaryModel dictionaryModel)
        {
            var addNewWordView = new AddNewWordView
            {
                DataContext = new AddNewWordViewModel(dictionaryModel)
            };

            _dialog = addNewWordView;
        }

        public void OpenDialog()
        {
            _dialog.ShowDialog();
        }
    }
}
