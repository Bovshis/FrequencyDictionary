using System.Windows;
using System.Windows.Controls;

namespace FrequencyDictionary.Components
{
    class MenuItemWithConfirmation : MenuItem
    {
        public string Question { get; set; }

        protected override void OnClick()
        {
            if (string.IsNullOrWhiteSpace(Question))
            {
                base.OnClick();
                return;
            }

            var messageBoxResult = MessageBox.Show(Question, "Confirmation", MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
                base.OnClick();
        }
    }
}
