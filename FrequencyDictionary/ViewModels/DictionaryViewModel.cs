using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FrequencyDictionary.Infrastructure;
using FrequencyDictionary.Models;
using FrequencyDictionary.ViewModels.Commands;

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

        public Dictionary<string, WordInformation> FrequencyDictionary => _dictionaryModel;

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
                        if (item is KeyValuePair<string, WordInformation> pair)
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

        public int WordsCount => _dictionaryView.Items.Cast<KeyValuePair<string, WordInformation>>().Select(pair => pair.Value.Count).Sum();

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
            var selectedWord = ((KeyValuePair<string, WordInformation>)_dictionaryView.SelectedItem).Key;
            _dictionaryModel.RemoveWord(selectedWord);
            OnCountsChanged();
        });

        public ICommand ClearCommand => new RelayCommand((_) =>
        {
            _dictionaryModel.ClearDictionary();
            OnCountsChanged();
        });

        public ICommand CorrectWordCommand => new RelayCommand((_) =>
        {
            var correctWordOpener = new CorrectWordViewOpener(_dictionaryModel);

            var incorrectWord = ((KeyValuePair<string, WordInformation>)_dictionaryView.SelectedItem).Key;
            correctWordOpener.CorrectWordViewModel.IncorrectForm = incorrectWord;
            correctWordOpener.OpenDialog();
        });

        public ICommand SaveToCsvCommand => new RelayCommand((_) =>
        {
            _saveFileDialogOpener.OpenFileDialog();
            _dictionaryModel.SaveToCsv(_saveFileDialogOpener.GetSelectedFilePath());
        });

        public ICommand ShowTagsTranscriptions => new RelayCommand((_) =>
        {
            var sb = new StringBuilder();
            sb.Append("Codes meaning:\n")
                .Append("CC	coordinating conjunction\n")
                .Append("CD	cardinal digit\n")
                .Append("DT	determiner\n")
                .Append("EX	existential there\n")
                .Append("FW	foreign word\n")
                .Append("IN	preposition/subordinating conjunction\n")
                .Append("JJ	This NLTK POS Tag is an adjective (large)\n")
                .Append("JJR	adjective, comparative (larger)\n")
                .Append("JJS	adjective, superlative (largest)\n")
                .Append("LS	list market\n")
                .Append("MD	modal (could, will)\n")
                .Append("NN	noun, singular (cat, tree)n\n")
                .Append("NNS	noun plural (desks)\n")
                .Append("NNP	proper noun, singular (sarah)\n")
                .Append("NNPS	proper noun, plural (indians or americans)\n")
                .Append("PDT	predeterminer (all, both, half)\n")
                .Append("POS	possessive ending \n")
                .Append("PRP	personal pronoun (hers, herself, him, himself)\n")
                .Append("PRP$	possessive pronoun (her, his, mine, my, our )\n")
                .Append("RB	adverb (occasionally, swiftly)\n")
                .Append("RBR	adverb, comparative (greater)\n")
                .Append("RBS	adverb, superlative (biggest)\n")
                .Append("RP	particle (about)\n")
                .Append("TO	infinite marker (to)\n")
                .Append("UH	interjection (goodbye)\n")
                .Append("VB	verb (ask)\n")
                .Append("VBG	verb gerund (judging)\n")
                .Append("VBD	verb past tense (pleaded)\n")
                .Append("VBN	verb past participle (reunified)\n")
                .Append("VBP	verb, present tense not 3rd person singular(wrap)\n")
                .Append("VBZ	verb, present tense with 3rd person singular (bases)\n")
                .Append("WDT	wh-determiner (that, what)\n")
                .Append("WP	wh- pronoun (who)\n")
                .Append("WRB	wh- adverb (how)\n");
            MessageBox.Show(sb.ToString(), "Tags transcriptions");
        });

        private void OnCountsChanged()
        {
            OnPropertyChanged(nameof(UniqueWordsCount));
            OnPropertyChanged(nameof(WordsCount));
        }
    }
}
