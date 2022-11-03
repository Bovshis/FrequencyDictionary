using System.Collections.Generic;

namespace FrequencyDictionary.Infrastructure;

public interface IOpenFileDialogOpener
{
    public void OpenFileDialog();

    public List<string> GetSelectedFilePaths();
}