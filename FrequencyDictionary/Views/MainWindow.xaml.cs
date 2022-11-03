using System;
using System.ComponentModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using FrequencyDictionary.Infrastructure;
using FrequencyDictionary.Models;
using FrequencyDictionary.ViewModels;
using Microsoft.Win32;

namespace FrequencyDictionary.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DictionaryModel _dictionaryModel;
        private readonly DictionaryViewModel _dictionaryViewModel;

        private readonly IOpenFileDialogOpener _openFileDialogOpener;
        private readonly ISaveFileDialogOpener _saveFileDialogOpener;

        private readonly IWindowOpener _addNewWordOpener;
        private readonly ICorrectWordViewOpener _correctWordOpener;

        public MainWindow()
        {
            InitializeComponent();
            _dictionaryModel = new DictionaryModel();
            var openFileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = "files (*.txt;*.csv)|*.txt;*.csv",
            };
            _openFileDialogOpener = new OpenFileDialogOpener(openFileDialog);
            var saveFileDialog = new SaveFileDialog()
            {
                Filter = "csv files (*.csv)|*.csv"
            };
            _saveFileDialogOpener = new SaveFileDialogOpener(saveFileDialog);
            _addNewWordOpener = new AddNewWordViewOpener(_dictionaryModel);
            _correctWordOpener = new CorrectWordViewOpener(_dictionaryModel);
            _dictionaryViewModel = new DictionaryViewModel(
                _dictionaryModel,
                _openFileDialogOpener,
                _saveFileDialogOpener,
                DictionaryView);

            DataContext = _dictionaryViewModel;
            DictionaryView.Items.SortDescriptions.Add(new SortDescription("Key", ListSortDirection.Ascending));
        }

        private void MainWindow_OnClosing(object? sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
