using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrequencyDictionary.Infrastructure
{
    public class OpenFileDialogOpener : IOpenFileDialogOpener
    {
        private readonly OpenFileDialog _dialog;

        public OpenFileDialogOpener(OpenFileDialog dialog)
        {
            _dialog = dialog;
        }

        public void OpenFileDialog()
        {
            _dialog.ShowDialog();
        }

        public List<string> GetSelectedFilePaths()
        {
            return _dialog.FileNames.ToList();
        }
    }
}
