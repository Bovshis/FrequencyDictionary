using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using LemmaSharp;
using Microsoft.Win32;
using POSTagger.Corpora;
using POSTagger.Taggers;
using POSTagger.Tokenizers;

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

        private const string LemInitializationFilePath = @"C:\Users\danii\source\repos\C#\АОТ\FrequencyDictionary\FrequencyDictionary\full7z-mlteast-en.lem";
        public MainWindow()
        {
            InitializeComponent();

            var lemmatizer = new Lemmatizer(File.OpenRead(LemInitializationFilePath));
            var corpus = CorpusFactory.GetCorpus("brills");
            var tagger = TaggerFactory.GetTagger("simple");

            _dictionaryModel = new DictionaryModel(lemmatizer, corpus, tagger);
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
