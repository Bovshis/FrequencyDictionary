using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrequencyDictionary.Infrastructure
{
    public class SaveFileDialogOpener : ISaveFileDialogOpener
    {
        private readonly SaveFileDialog _dialog;

        public SaveFileDialogOpener(SaveFileDialog dialog)
        {
            _dialog = dialog;
        }

        public void OpenFileDialog()
        {
            _dialog.ShowDialog();
        }

        public string GetSelectedFilePath()
        {
            return _dialog.FileName;
        }
    }
}
