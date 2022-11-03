using FrequencyDictionary.ViewModels;

namespace FrequencyDictionary.Infrastructure;

public interface ICorrectWordViewOpener
{
    public void OpenDialog();

    public CorrectWordViewModel CorrectWordViewModel { get; set; }
}