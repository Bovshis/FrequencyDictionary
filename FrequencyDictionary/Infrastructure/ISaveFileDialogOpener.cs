namespace FrequencyDictionary.Infrastructure;

public interface ISaveFileDialogOpener
{
    public void OpenFileDialog();
    public string GetSelectedFilePath();
}